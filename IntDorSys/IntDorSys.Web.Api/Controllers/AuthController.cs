using IntDorSys.Security.Models;
using IntDorSys.Security.Services;
using IntDorSys.Web.Api.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Ouro.CommonUtils.Results;
using Ouro.WebApiUtils;

namespace IntDorSys.Web.Api.Controllers
{
    [Route("token")]
    [EnableRateLimiting("AuthLimit")]
    public sealed class AuthController : ApiController
    {
        /// <summary>
        ///     Authenticate user by login and password
        /// </summary>
        /// <param name="request">Authentication data</param>
        /// <param name="authService">Authentication service</param>
        /// <param name="tokenService">Generates token by user data</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DataResult<AuthToken>> Post(
            [FromBody] TokenRequest request,
            [FromServices] IAuthService authService,
            [FromServices] ITokenService tokenService)
        {
            var result = new DataResult<AuthToken>();

            var authResult = await authService.AuthenticateUser(
                new AuthData
                {
                    Login = request.Login,
                    Password = request.Password,
                },
                HttpContext.RequestAborted);
            return !authResult.IsSuccess
                ? result.WithErrors(authResult.Errors)
                : result.WithData(tokenService.IssueToken(authResult.Data));
        }
    }
}