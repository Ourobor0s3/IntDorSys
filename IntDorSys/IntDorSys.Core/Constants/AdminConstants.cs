namespace IntDorSys.Core.Constants
{
    public static class AdminConstants
    {
        // TODO вынести по хорошему в конфиг
        public const long AdminChatId = 1;
        public const long ManagerLaundressId = 2;

        public static List<long> AdminsChatId =>
            [AdminChatId];

        public static List<long> ManagersLaundress =>
            [AdminChatId, ManagerLaundressId];
    }
}