using System.Security.Claims;
using Ouro.WebApiUtils;

namespace IntDorSys.Web.Api.Controllers.Base
{
    public class ProtectedApiController : ApiController
    {
        protected long UserId
        {
            get
            {
                // claim 'sub' means 'nameidentifier'
                var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    throw new InvalidOperationException("Unable to get claim 'sub'");
                }

                return long.Parse(userId);
            }
        }
    }
}