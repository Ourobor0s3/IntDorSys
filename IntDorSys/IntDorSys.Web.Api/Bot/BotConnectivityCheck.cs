using System.Net.Sockets;

namespace IntDorSys.Web.Api.Bot
{
    internal static class BotConnectivityCheck
    {
        private const string telegramApiHost = "api.telegram.org";
        private const int telegramApiPort = 443;
        private const int telegramConnectTimeoutMs = 3000;

        internal static bool IsTelegramReachable()
        {
            try
            {
                using var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                var result = socket.BeginConnect(telegramApiHost, telegramApiPort, null, null);
                return result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(telegramConnectTimeoutMs)) && socket.Connected;
            }
            catch
            {
                return false;
            }
        }
    }
}