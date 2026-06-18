using IntDorSys.Core.Constants;
using IntDorSys.Core.Entities.Users;
using IntDorSys.Core.Enums;
using IntDorSys.Core.Models;
using IntDorSys.Services.Users;
using IntDorSys.Web.Api.Builders;
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

        [HttpGet("{userId:long}")]
        public async Task<DataResult<UserInfoModel>> GetByIdAsync(
            long userId,
            [FromServices] IUserInfoBuilder builder,
            [FromServices] IUserQueryService userService)
        {
            var result = new DataResult<UserInfoModel>();
            var user = await userService.GetAsync(userId, HttpContext.RequestAborted);

            if (!user.IsSuccess)
                return result.WithErrors(user.Errors);

            result.Data = await builder.BuildAsync(user.Data, HttpContext.RequestAborted);
            return result;
        }

        [HttpPut("change-status/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<DataResult<bool>> ChangeStatusAsync(
            long userId,
            [FromBody] UserStatus newStatus,
            [FromServices] IUserCommandService service)
        {
            return await service.ChangeUserStatus(userId, newStatus, UserId, HttpContext.RequestAborted);
        }

        [HttpPut("confirm/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<DataResult<UserInfo>> ConfirmUserAsync(
            long userId,
            [FromServices] IUserCommandService service,
            [FromQuery] string roleKey = UserRoleKeys.Student)
        {
            return await service.ConfirmUserWithRoleAsync(userId, roleKey, UserId, HttpContext.RequestAborted);
        }

        [HttpPut("remove-role/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<DataResult<bool>> RemoveRoleAsync(
            long userId,
            [FromQuery] string roleKey,
            [FromServices] IUserCommandService service)
        {
            return await service.RemoveRoleAsync(userId, roleKey, UserId, HttpContext.RequestAborted);
        }
    }
}