using IntDorSys.Core.Models;

namespace IntDorSys.Core.Constants
{
    public static class DefaultSettings
    {
        public const int WashDurationHours = 2;
        public const int MaxConcurrentBookings = 2;

        public static readonly DefaultSettingEntry[] All =
        [
            new()
            {
                Key = "Rules",
                Value = "Правила использования прачечной:\n1. После использования скидывать следующие фото: раковины, стиралка, сушилка (дверца, отсек для фильтра и сам фильтр)\n2. Если не получится постираться, то убери запись",
                IsEditable = true
            },
            new()
            {
                Key = "WashDurationHours",
                Value = WashDurationHours.ToString(),
                IsEditable = false
            },
            new()
            {
                Key = "MaxConcurrentBookings",
                Value = MaxConcurrentBookings.ToString(),
                IsEditable = true
            },
            new()
            {
                Key = "TimeZone",
                Value = "+03:00",
                IsEditable = true
            },
        ];
    }
}