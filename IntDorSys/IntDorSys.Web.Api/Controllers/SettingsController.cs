using System.Text.Json;
using IntDorSys.Core.Entities;
using IntDorSys.Services.AppSettings;
using IntDorSys.Web.Api.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ouro.CommonUtils.Results;

namespace IntDorSys.Web.Api.Controllers
{
    [Route("settings")]
    [Authorize(Roles = "Admin")]
    public sealed class SettingsController : ProtectedApiController
    {
        [HttpGet]
        public async Task<DataResult<List<AppSetting>>> GetAllAsync(
            [FromServices] IAppSettingService settings,
            CancellationToken ct)
        {
            var result = await settings.GetAllEditableAsync(ct);
            return new DataResult<List<AppSetting>>().WithData(result);
        }

        [HttpPut("{id:long}")]
        public async Task<Result> UpdateAsync(
            long id,
            [FromBody] JsonElement body,
            [FromServices] IAppSettingService settings,
            CancellationToken ct)
        {
            var value = body.GetProperty("value").GetString();
            if (string.IsNullOrEmpty(value))
                return new Result().WithError("Value is required");

            try
            {
                await settings.UpdateAsync(id, value, UserId, ct);
            }
            catch (InvalidOperationException ex)
            {
                return new Result().WithError(ex.Message);
            }

            return new Result();
        }
    }
}
