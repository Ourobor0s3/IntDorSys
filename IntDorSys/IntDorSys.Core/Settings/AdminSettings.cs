namespace IntDorSys.Core.Settings
{
    public class AdminSettings
    {
        required public long AdminChatId { get; set; }
        required public long ManagerLaundressId { get; set; }

        public List<long> AdminsChatId => [AdminChatId];
        public List<long> ManagersLaundress => new List<long> { AdminChatId, ManagerLaundressId }.Distinct().ToList();
    }
}