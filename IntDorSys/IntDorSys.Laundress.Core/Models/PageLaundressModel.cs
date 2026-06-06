namespace IntDorSys.Laundress.Core.Models
{
    public class PageLaundressModel
    {
        /// <summary>
        ///     Date washing
        /// </summary>
        public string? Date { get; set; }

        /// <summary>
        ///     List laundress model by date washing
        /// </summary>
        public List<LaundModel>? LaundModels { get; set; }
    }
}