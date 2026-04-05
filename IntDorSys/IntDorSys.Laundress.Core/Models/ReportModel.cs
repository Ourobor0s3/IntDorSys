using FileInfo = IntDorSys.Core.Entities.FileInfo;

namespace IntDorSys.Laundress.Core.Models
{
    public class ReportModel
    {
        /// <summary>
        ///     User id, who created report
        /// </summary>
        required public long UserId { get; set; }

        /// <summary>
        ///     Username, who created report
        /// </summary>
        required public string Username { get; set; }

        /// <summary>
        ///     The ID of the file receiving group
        /// </summary>
        public string? GroupId { get; set; }

        /// <summary>
        ///     Description about report
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        ///     Date created report
        /// </summary>
        required public DateTime CreatedAt { get; set; }

        /// <summary>
        ///     Files
        /// </summary>
        public List<FileInfo>? Files { get; set; }
    }
}