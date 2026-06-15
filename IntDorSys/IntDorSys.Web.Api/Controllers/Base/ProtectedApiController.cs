using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
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
                if (!TryGetUserId(out var userId))
                {
                    throw new UnauthorizedAccessException("Invalid or missing user claim");
                }
                return userId;
            }
        }

        protected bool TryGetUserId(out long userId)
        {
            if (_userId.HasValue)
            {
                userId = _userId.Value;
                return true;
            }

            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim?.Value) || !long.TryParse(userIdClaim.Value, out userId))
            {
                userId = 0;
                return false;
            }

            _userId = userId;
            return true;
        }

        protected IActionResult UnauthorizedUserId()
        {
            return Unauthorized(new { error = "Invalid or missing user claim" });
        }
    }
}