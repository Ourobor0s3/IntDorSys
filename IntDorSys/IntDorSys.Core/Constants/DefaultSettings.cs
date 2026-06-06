using IntDorSys.Core.Models;

namespace IntDorSys.Core.Constants
{
    public static class DefaultSettings
    {
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
                Value = "2",
                IsEditable = false
            }
        ];
    }
}