namespace IntDorSys.Core.Constants
{
    public static class MessageConstants
    {
        //public static string GetMessage(string key) => FileExtensions.GetReplacedTextForMessage(key,)
        // private static readonly Dictionary<string, string> DictMessage = FileExtensions.GetReplacedAllTextForMessage();
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

    public static class MessageKeyConstants
    {
        //ключи в джсоне, которым соответствуют текста
        public const string Start = "/start";
        public const string Rules = "/rules";
        public const string Menu = "/menu";
        public const string Back = "back";
        public const string AllFreeRecords = "allFreeRecords";
        public const string GetUsers = "getUsers";
        public const string GetBlockedUsers = "getBlockedUsers";
        public const string MyRecords = "myRecords";
        public const string AllRecords = "allRecords";
        public const string CreateNotification = "createNotification";
        public const string GetUserInfo = "getUserInfo";
        public const string NotCorrect = "notCorrect";
        public const string ConfirmTg = "confirmTg";
        public const string NotConfirmTg = "notConfirmTg";
        public const string NoEntries = "noEntries";
    }

    public static class MessageText
    {
        public static readonly string Start = MessageConstants.GetValue(MessageKeyConstants.Start);
        public static readonly string Rules = MessageConstants.GetValue(MessageKeyConstants.Rules);
        public static readonly string Menu = MessageConstants.GetValue(MessageKeyConstants.Menu);
        public static readonly string Back = MessageConstants.GetValue(MessageKeyConstants.Back);
        public static readonly string AllFreeRecords = MessageConstants.GetValue(MessageKeyConstants.AllFreeRecords);
        public static readonly string MyRecords = MessageConstants.GetValue(MessageKeyConstants.MyRecords);
        public static readonly string AllRecords = MessageConstants.GetValue(MessageKeyConstants.AllRecords);
        public static readonly string GetUsers = MessageConstants.GetValue(MessageKeyConstants.GetUsers);
        public static readonly string GetBlockedUsers = MessageConstants.GetValue(MessageKeyConstants.GetBlockedUsers);

        public static readonly string CreateNotification =
            MessageConstants.GetValue(MessageKeyConstants.CreateNotification);

        public static readonly string GetUserInfo = MessageConstants.GetValue(MessageKeyConstants.GetUserInfo);
        public static readonly string NotCorrect = MessageConstants.GetValue(MessageKeyConstants.NotCorrect);
        public static readonly string ConfirmTg = MessageConstants.GetValue(MessageKeyConstants.ConfirmTg);
        public static readonly string NotConfirmTg = MessageConstants.GetValue(MessageKeyConstants.NotConfirmTg);
        public static readonly string NoEntries = MessageConstants.GetValue(MessageKeyConstants.NoEntries);
    }
}
