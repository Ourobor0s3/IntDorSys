using System.ComponentModel.DataAnnotations;

namespace IntDorSys.Web.Api.Requests
{
    public sealed class CreateLaundressRangeRequest
    {
        [Required]
        public DateTime Date { get; set; }

        [Required]
        [Range(0, 23)]
        public int StartHour { get; set; }

        [Required]
        [Range(0, 23)]
        public int EndHour { get; set; }
    }
}
