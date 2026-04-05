using IntDorSys.Core.Enums;
using IntDorSys.Core.Models;
using IntDorSys.Services.Builders;
using IntDorSys.Services.Users;
using IntDorSys.Web.Api.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using Ouro.CommonUtils.Results;

namespace IntDorSys.Web.Api.Controllers
{
    [Route("users-info")]
    public sealed class UsersInfoController : ProtectedApiController
    {
        [HttpGet]
        public Task<DataResult<List<UserInfoModel>>> GetAsync([FromServices] IUserInfoBuilder builder)
        {
            return builder.BuildAsync(HttpContext.RequestAborted);
        }

        [HttpPut("change-status/{userId}")]
        public Task<DataResult<bool>> ChangeStatus(
            long userId,
            [FromBody] UserStatus newStatus,
            [FromServices] IUserService service)
        {
            return service.ChangeUserStatus(userId, newStatus, HttpContext.RequestAborted);
        }
    }
}