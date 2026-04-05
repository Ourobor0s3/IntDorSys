using Ouro.CommonUtils.Base.Entities;

namespace IntDorSys.Laundress.Core.Models.Filters
{
    public class LaundressFilterModel : BaseFilterModel
    {
        /// <summary>
        ///     Search by specific date
        /// </summary>
        public DateTime SearchDate { get; set; }

        /// <summary>
        ///     Search for unoccupied records
        /// </summary>
        public bool IsUnoccupiedRecords { get; set; }
    }
}