using IntDorSys.Core.Enums;
using IntDorSys.Core.Models;
using IntDorSys.Services.Audit;
using IntDorSys.Web.Api.Builders;
using IntDorSys.Services.Users;
using IntDorSys.Web.Api.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ouro.CommonUtils.Results;

namespace IntDorSys.Web.Api.Controllers
{
    [Route("users-info")]
    [Authorize]
    public sealed class UsersInfoController : ProtectedApiController
    {
        [HttpGet]
        public Task<DataResult<List<UserInfoModel>>> GetAsync([FromServices] IUserInfoBuilder builder)
        {
            return builder.BuildAsync(HttpContext.RequestAborted);
        }

        [HttpPut("change-status/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<DataResult<bool>> ChangeStatusAsync(
            long userId,
            [FromBody] UserStatus newStatus,
            [FromServices] IUserCommandService service,
            [FromServices] IAuditService audit)
        {
            var result = await service.ChangeUserStatus(userId, newStatus, HttpContext.RequestAborted);
            if (result.IsSuccess)
            {
                await audit.RecordAsync(UserId, "ChangeUserStatus", "UserInfo",
                    userId.ToString(), $"New status: {newStatus}");
            }
            return result;
        }
    }
}