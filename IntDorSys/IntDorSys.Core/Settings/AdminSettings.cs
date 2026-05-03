namespace IntDorSys.Core.Settings
{
    public class AdminSettings
    {
        public required long AdminChatId { get; set; }
        public required long ManagerLaundressId { get; set; }

        public List<long> AdminsChatId => [AdminChatId];
        public List<long> ManagersLaundress => [AdminChatId, ManagerLaundressId];
    }
}