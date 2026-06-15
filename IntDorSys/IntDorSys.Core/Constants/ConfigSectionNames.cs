namespace IntDorSys.Core.Constants
{
    /// <summary>
    ///     Contains section names for different modules
    /// </summary>
    public static class ConfigSectionNames
    {
        /// <summary>
        ///     Section with Quartz configuration
        /// </summary>
        public const string QuartzSection = "Quartz";

        /// <summary>
        ///     Section with Telegram Test configuration
        /// </summary>
        public const string TelegramTestSection = "Telegram:Test";

        /// <summary>
        ///     Section with Telegram Battle configuration
        /// </summary>
        public const string TelegramBattleSection = "Telegram:Battle";

        /// <summary>
        ///     Section with Dump service configuration
        /// </summary>
        public const string DumpSection = "DumpSettings";

        /// <summary>
        ///     Section with link settings
        /// </summary>
        public const string LinkSection = "Links";

        /// <summary>
        ///     Section with admin ids
        /// </summary>
        public const string AdminIdsSection = "AdminIds";

        /// <summary>
        ///     Section with Telegram API connection configuration
        /// </summary>
        public const string TelegramApiHost = "Telegram:Api:Host";

        /// <summary>
        ///     Section with Telegram API port
        /// </summary>
        public const string TelegramApiPort = "Telegram:Api:Port";

        /// <summary>
        ///     Section with file storage folder path
        /// </summary>
        public const string FileStorageFolder = "FileStorage:Folder";
    }
}