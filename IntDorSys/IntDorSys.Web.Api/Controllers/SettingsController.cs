using IntDorSys.Core.Entities;
using IntDorSys.Services.AppSettings;
using IntDorSys.Web.Api.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ouro.CommonUtils.Results;

namespace IntDorSys.Web.Api.Controllers
{
    [Route("settings")]
    public sealed class SettingsController : ProtectedApiController
    {
        [AllowAnonymous]
        [HttpGet("timezone")]
        public async Task<DataResult<string>> GetTimezoneAsync(
            [FromServices] IAppSettingService settings,
            CancellationToken ct)
        {
            var value = await settings.GetValueAsync("TimeZone", ct);
            return new DataResult<string>().WithData(value ?? "+03:00");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<DataResult<List<AppSetting>>> GetAllAsync(
            [FromServices] IAppSettingService settings,
            CancellationToken ct)
        {
            var result = await settings.GetAllEditableAsync(ct);
            return new DataResult<List<AppSetting>>().WithData(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:long}")]
        public async Task<Result> UpdateAsync(
            long id,
            [FromBody] string value,
            [FromServices] IAppSettingService settings,
            CancellationToken ct)
        {
            return await settings.UpdateAsync(id, value, UserId, ct);
        }
    }
}