using System.Net.Sockets;

namespace IntDorSys.Web.Api.Bot
{
    internal static class BotConnectivityCheck
    {
        private const string TelegramApiHost = "api.telegram.org";
        private const int TelegramApiPort = 443;
        private const int TelegramConnectTimeoutMs = 3000;

        internal static bool IsTelegramReachable()
        {
            try
            {
                using var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                var result = socket.BeginConnect(TelegramApiHost, TelegramApiPort, null, null);
                return result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(TelegramConnectTimeoutMs)) && socket.Connected;
            }
            catch
            {
                return false;
            }
        }
    }
}