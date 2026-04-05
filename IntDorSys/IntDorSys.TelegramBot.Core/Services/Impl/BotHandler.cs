using IntDorSys.TelegramBot.Core.Enum;
using IntDorSys.TelegramBot.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace IntDorSys.TelegramBot.Core.Services.Impl
{
    internal sealed class BotHandler : IBotHandler
    {
        private readonly TelegramBotClient _botClient;
        private readonly ILogger<BotHandler> _logger;
        private readonly CancellationToken _ct = new();

        private readonly SettingsAndHandlers _settingsAndHandler;
        private readonly Dictionary<long, UserState> _userStates = new();


        /// <inheritdoc cref="IBotHandler" />
        public BotHandler(
            ILogger<BotHandler> logger,
            IConfiguration configuration,
            SettingsAndHandlers settingsAndHandler)
        {
            _logger = logger;

            _settingsAndHandler = settingsAndHandler;
            _botClient = new TelegramBotClient((Convert.ToBoolean(configuration["BuildTest"])
                ? configuration["Telegram:Token:Test"]
                : configuration["Telegram:Token:Battle"] ?? throw new InvalidOperationException())!);
            // _botClient.Timeout = TimeSpan.Parse(configuration["Telegram:Timeout"]!);
        }

        /// <inheritdoc />
        public async Task StartBot()
        {
            var me = await _botClient.GetMe(_ct);
            _logger.LogInformation($"Bot started: {me.Username}");
            _botClient.StartReceiving(UpdateHandler, ErrorHandler, cancellationToken: _ct);
        }

        private async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            try
            {
                var user = await _settingsAndHandler.AuthorizationUser(update, ct);

                if (!user)
                {
                    return;
                }

                if (update.Message != null)
                {
                    var message = update.Message;
                    if (_userStates.ContainsKey(message.Chat.Id))
                    {
                        // Обработка состояний
                    }
                    else if (message.Text != null
                          && _settingsAndHandler.CommandHandler.TryGetValue(message.Text, out var value))
                    {
                        await value(message, ct);
                    }
                    else
                    {
                        await _settingsAndHandler.MessageHandler(message, ct);
                    }
                }
                else if (update.CallbackQuery != null)
                {
                    await _settingsAndHandler.CallbackHandler(update.CallbackQuery, ct);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        private Task ErrorHandler(
            ITelegramBotClient botClient,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Ошибка в обработке обновления");
            return Task.CompletedTask;
        }
    }
}