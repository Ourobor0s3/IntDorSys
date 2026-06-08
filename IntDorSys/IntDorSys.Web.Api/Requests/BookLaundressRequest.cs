using System.ComponentModel.DataAnnotations;

namespace IntDorSys.Web.Api.Requests
{
    public sealed class BookLaundressRequest
    {
        [Required]
        public DateTime TimeWash { get; set; }

        [Required]
        public long UserId { get; set; }
    }
}