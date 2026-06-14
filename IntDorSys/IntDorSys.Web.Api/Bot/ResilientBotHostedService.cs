using IntDorSys.Core.Constants;
using IntDorSys.TelegramBot.Service;
using IntDorSys.TelegramBot.Service.Authorize;
using IntDorSys.TelegramBot.Service.CallbackServices;
using IntDorSys.TelegramBot.Service.CommandServices;
using IntDorSys.TelegramBot.Service.MessageServices;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace IntDorSys.Web.Api.Bot
{
    internal sealed class ResilientBotHostedService : IHostedService, IBotControlService, IDisposable
    {
        private static readonly TimeSpan _retryInterval = TimeSpan.FromMinutes(15);
        private static readonly TimeSpan _restartInterval = TimeSpan.FromHours(5);

        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ResilientBotHostedService> _logger;
        private readonly BotStatus _botStatus;
        private readonly BotConnectivityCheck _connectivityCheck;

        private CancellationTokenSource? _globalCts;
        private CancellationTokenSource? _botCts;
        private bool _botRunning;

        public ResilientBotHostedService(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            BotStatus botStatus,
            ILogger<ResilientBotHostedService> logger,
            BotConnectivityCheck connectivityCheck)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _botStatus = botStatus;
            _logger = logger;
            _connectivityCheck = connectivityCheck;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _globalCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _ = TryRunAsync(_globalCts.Token);
            return Task.CompletedTask;
        }

        private async Task TryRunAsync(CancellationToken ct)
        {
            try
            {
                await RunAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bot loop terminated unexpectedly");
            }
        }

        private async Task RunAsync(CancellationToken globalCt)
        {
            _logger.LogInformation("Bot host starting — initial connect attempt");
            await TryStartBotAsync(globalCt);

            using var retryTimer = new PeriodicTimer(_retryInterval);
            using var restartTimer = new PeriodicTimer(_restartInterval);

            try
            {
                while (!globalCt.IsCancellationRequested)
                {
                    var retryTask = retryTimer.WaitForNextTickAsync(globalCt).AsTask();
                    var restartTask = restartTimer.WaitForNextTickAsync(globalCt).AsTask();
                    var completed = await Task.WhenAny(retryTask, restartTask);

                    if (completed == retryTask)
                    {
                        if (!_botRunning)
                        {
                            _logger.LogWarning("Bot not running — retrying connection");
                            await TryStartBotAsync(globalCt);
                        }
                    }
                    else if (completed == restartTask && _botRunning)
                    {
                        _logger.LogInformation("5-hour scheduled restart");
                        await StopBotAsync();
                        await TryStartBotAsync(globalCt);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // graceful shutdown
            }
        }

        private async Task TryStartBotAsync(CancellationToken globalCt)
        {
            try
            {
                if (!_connectivityCheck.IsTelegramReachable())
                {
                    _logger.LogWarning("Telegram API unreachable — retry in {Interval}", _retryInterval);
                    return;
                }

                var isBattle = !_configuration.GetValue<bool>("BuildTest");
                var token = _configuration.GetSection(isBattle
                    ? ConfigSectionNames.TelegramBattleSection
                    : ConfigSectionNames.TelegramTestSection)["Token"];

                if (string.IsNullOrWhiteSpace(token))
                {
                    _logger.LogError("Telegram bot token not configured");
                    return;
                }

                var botCts = CancellationTokenSource.CreateLinkedTokenSource(globalCt);
                var oldCts = Interlocked.Exchange(ref _botCts, botCts);
                oldCts?.Cancel();
                oldCts?.Dispose();

                var botClient = new TelegramBotClient(token);
                var me = await botClient.GetMe(globalCt);
                _logger.LogInformation("Bot started as @{Username}", me.Username);

                _botStatus.IsRunning = true;
                _botStatus.BotUsername = me.Username;
                _botStatus.LastStartedAt = DateTime.UtcNow;

                botClient.StartReceiving(
                    HandleUpdateAsync,
                    HandleErrorAsync,
                    receiverOptions: null,
                    cancellationToken: botCts.Token);

                _botRunning = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start bot");
                _botRunning = false;
            }
        }

        private async Task HandleUpdateAsync(
            ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            using var scope = _serviceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var handlers = ServicesInstaller.GetSettingsAndHandlers(
                sp.GetRequiredService<ICallbackHandlerService>(),
                sp.GetRequiredService<IMessageHandlerService>(),
                sp.GetRequiredService<IAuthService>(),
                sp.GetRequiredService<ICommandService>());

            if (!await handlers.AuthorizationUser(update, ct))
                return;

            if (update.Message?.Text != null
                && handlers.CommandHandler.TryGetValue(update.Message.Text, out var commandHandler))
            {
                await commandHandler(update.Message, ct);
                return;
            }

            if (update.Message != null)
            {
                await handlers.MessageHandler(update.Message, ct);
                return;
            }

            if (update.CallbackQuery != null)
            {
                await handlers.CallbackHandler(update.CallbackQuery, ct);
            }
        }

        private Task HandleErrorAsync(
            ITelegramBotClient botClient, Exception exception, CancellationToken ct)
        {
            _logger.LogError(exception, "Bot polling error");
            return Task.CompletedTask;
        }

        private async Task StopBotAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Stopping bot...");
            _botRunning = false;
            _botStatus.IsRunning = false;

            var oldCts = Interlocked.Exchange(ref _botCts, null);
            if (oldCts != null)
            {
                oldCts.Cancel();
                oldCts.Dispose();
            }

            await Task.Delay(500, ct);
        }

        public async Task RestartAsync()
        {
            if (!_botRunning)
            {
                _logger.LogInformation("Manual restart requested but bot not running — starting");
                await TryStartBotAsync(_globalCts?.Token ?? CancellationToken.None);
                return;
            }

            _logger.LogInformation("Manual restart requested");
            await StopBotAsync();
            await TryStartBotAsync(_globalCts?.Token ?? CancellationToken.None);

            if (!_botRunning)
            {
                throw new InvalidOperationException("Bot failed to restart");
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _globalCts?.Cancel();
            await StopBotAsync();
        }

        public void Dispose()
        {
            _globalCts?.Cancel();
            _globalCts?.Dispose();
            _botCts?.Cancel();
            _botCts?.Dispose();
        }
    }
}