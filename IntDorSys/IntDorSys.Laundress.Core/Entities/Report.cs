using IntDorSys.Core.Entities.Users;
using Ouro.DatabaseUtils.Entities.Impl;

namespace IntDorSys.Laundress.Core.Entities
{
    public class Report : SoftDeletableEntity<long>
    {
        /// <summary>
        ///     User id, who created report
        /// </summary>
        required public long UserId { get; set; }

        /// <summary>
        ///     User info, who created report
        /// </summary>
        public UserInfo? User { get; set; }

        /// <summary>
        ///     The ID of the file receiving group
        /// </summary>
        public string? GroupId { get; set; }

        /// <summary>
        ///     Description about report
        /// </summary>
        public string? Description { get; set; }
    }
}