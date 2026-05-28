using System.Net.Sockets;

namespace IntDorSys.Web.Api
{
    internal static class BotConnectivityCheck
    {
        private const int TimeoutMs = 3000;

        internal static bool IsTelegramReachable()
        {
            try
            {
                using var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                var result = socket.BeginConnect("api.telegram.org", 443, null, null);
                return result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(TimeoutMs)) && socket.Connected;
            }
            catch
            {
                return false;
            }
        }
    }
}
