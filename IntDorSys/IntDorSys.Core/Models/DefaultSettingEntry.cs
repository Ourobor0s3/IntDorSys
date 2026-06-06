namespace IntDorSys.Core.Models
{
    public sealed class DefaultSettingEntry
    {
        required public string Key { get; init; }
        required public string Value { get; init; }
        public bool IsEditable { get; init; }
    }
}
