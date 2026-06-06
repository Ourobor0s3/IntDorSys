namespace IntDorSys.Core.Constants
{
    public static class MessageConstants
    {
        private static readonly Dictionary<string, string> _dictMessage = new()
        {
            [MessageKeyConstants.Start] = "Привет, пользователь!",
            [MessageKeyConstants.Rules] =
                "Правила использования прачечной:\n1. После использования скидывать следующие фото: раковины, стиралка, сушилка (дверца, отсек для фильтра и сам фильтр)\n2. Если не получится постираться, то убери запись",
            [MessageKeyConstants.Menu] = "Меню",
            [MessageKeyConstants.Back] = "Назад",
            [MessageKeyConstants.AllFreeRecords] = "✍️ Записаться",
            [MessageKeyConstants.GetUsers] = "😎 Доступ есть",
            [MessageKeyConstants.GetBlockedUsers] = "🤬 Доступа нет",
            [MessageKeyConstants.AllRecords] = "📝 Все записи",
            [MessageKeyConstants.DeleteRecords] = "🗑️ Удалить записи",
            [MessageKeyConstants.CreateNotification] = "Отправлен запрос на подтверждение",
            [MessageKeyConstants.GetUserInfo] =
                "Чтобы получить доступ, ответьте на это сообщение (функция <Ответить>) введя свои ФИО (Отчество, если имеется), номер группы, номер комнаты\nПример: Иванов Иван Иванович, 3114, 1511",
            [MessageKeyConstants.MyRecords] = "📝 Мои записи",
            [MessageKeyConstants.NotCorrect] = "Данные введены некорректно",
            [MessageKeyConstants.ConfirmTg] = "Телеграм подтвержден",
            [MessageKeyConstants.NotConfirmTg] = "Телеграм не подтвержден",
            [MessageKeyConstants.NoEntries] = "Записей нет",
        };

        public static string GetValue(string key)
        {
            return _dictMessage.TryGetValue(key, out var value) ? value : string.Empty;
        }
    }
}
