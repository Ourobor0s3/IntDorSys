using System.Security.Claims;
using Ouro.WebApiUtils;

namespace IntDorSys.Web.Api.Controllers.Base
{
    public class ProtectedApiController : ApiController
    {
        private long? _userId;
        
        protected long UserId
        {
            get
            {
                if (_userId.HasValue)
                {
                    return _userId.Value;
                }

                var userIdClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
                
                if (string.IsNullOrEmpty(userIdClaim?.Value) || !long.TryParse(userIdClaim.Value, out var userId))
                {
                    throw new UnauthorizedAccessException("Invalid or missing user claim");
                }

                _userId = userId;
                return userId;
            }
        }
    }
}