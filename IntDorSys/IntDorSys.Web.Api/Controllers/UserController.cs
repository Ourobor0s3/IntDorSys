using IntDorSys.Core.Models;
using IntDorSys.Services.Builders;
using IntDorSys.Services.Users;
using IntDorSys.Web.Api.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using Ouro.CommonUtils.Results;

namespace IntDorSys.Web.Api.Controllers
{
    [Route("user")]
    public class UserController : ProtectedApiController
    {
        [HttpGet]
        public async Task<DataResult<UserInfoModel>> Get(
            [FromServices] IUserService userService,
            [FromServices] IUserInfoBuilder builder)
        {
            var res = new DataResult<UserInfoModel>();
            var user = await userService.GetByUserIdAsync(UserId, HttpContext.RequestAborted);

            return !user.IsSuccess
                ? res.WithErrors(user.Errors)
                : res.WithData(builder.Build(user.Data));
        }
    }
}