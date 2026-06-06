using System.ComponentModel.DataAnnotations;

namespace IntDorSys.Web.Api.Requests
{
    public sealed class CreateLaundressRequest
    {
        [Required]
        public DateTime TimeWash { get; set; }

        [Required]
        public long CreatedUserId { get; set; }
    }
}
