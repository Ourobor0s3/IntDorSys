using Ouro.DatabaseUtils.Entities.Impl;

namespace IntDorSys.Core.Entities
{
    public sealed class AppSetting : SoftDeletableEntity<long>
    {
        required public string Key { get; set; }
        required public string Value { get; set; }
        public bool IsEditable { get; set; }
    }
}