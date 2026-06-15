using System.Net.Sockets;
using IntDorSys.Core.Constants;

namespace IntDorSys.Web.Api.Bot
{
    internal sealed class BotConnectivityCheck
    {
        private const int telegramConnectTimeoutMs = 3000;

        private readonly ILogger<BotConnectivityCheck> _logger;
        private readonly string _host;
        private readonly int _port;

        public BotConnectivityCheck(ILogger<BotConnectivityCheck> logger, IConfiguration configuration)
        {
            _logger = logger;
            _host = configuration.GetValue<string>(ConfigSectionNames.TelegramApiHost) ?? "api.telegram.org";
            _port = configuration.GetValue<int>(ConfigSectionNames.TelegramApiPort);
            if (_port == 0) _port = 443;
        }

        internal bool IsTelegramReachable()
        {
            try
            {
                using var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                var result = socket.BeginConnect(_host, _port, null, null);
                return result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(telegramConnectTimeoutMs)) && socket.Connected;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Telegram connectivity check failed");
                return false;
            }
        }
    }
}