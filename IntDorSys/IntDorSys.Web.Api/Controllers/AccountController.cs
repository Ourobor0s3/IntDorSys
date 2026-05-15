using IntDorSys.Security.Models;
using IntDorSys.Security.Services;
using IntDorSys.Web.Api.Controllers.Base;
using IntDorSys.Web.Api.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ouro.CommonUtils.Results;

namespace IntDorSys.Web.Api.Controllers
{
    [Route("account")]
    public sealed class AccountController : ProtectedApiController
    {
        /// <summary>
        ///     Registration user
        /// </summary>
        /// <param name="request">Registration data</param>
        /// <param name="authService">Registration service</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<Result> Post(
            [FromBody] NewUserRequest request,
            [FromServices] IAuthService authService)
        {
            var result = new Result();

            var registrationData = new RegistrationData
            {
                TelegramId = request.TelegramId ?? 0,
                Email = request.Email,
                Password = request.Password,
                FullName = request.FullName,
                NumGroup = request.NumGroup,
                NumRoom = request.NumRoom,
            };

            var registerResult = await authService.RegisterUserAsync(
                registrationData,
                HttpContext.RequestAborted);

            return !registerResult.IsSuccess
                ? result.WithErrors(registerResult.Errors)
                : result;
        }
    }
}