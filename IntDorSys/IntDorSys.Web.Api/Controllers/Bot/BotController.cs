using IntDorSys.Core.Models;
using IntDorSys.Web.Api.Bot;
using IntDorSys.Web.Api.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ouro.CommonUtils.Results;

namespace IntDorSys.Web.Api.Controllers.Bot
{
    [Route("[controller]")]
    [Authorize]
    public sealed class BotController : ProtectedApiController
    {
        private readonly BotStatus _botStatus;
        private readonly IBotControlService _botControl;

        public BotController(BotStatus botStatus, IBotControlService botControl)
        {
            _botStatus = botStatus;
            _botControl = botControl;
        }

        [HttpGet("status")]
        public DataResult<BotStatusDto> GetStatus()
        {
            return new DataResult<BotStatusDto>().WithData(new BotStatusDto
            {
                Running = _botStatus.IsRunning,
                Username = _botStatus.BotUsername,
                LastStarted = _botStatus.LastStartedAt,
            });
        }

        [HttpPost("restart")]
        [Authorize(Roles = "Admin")]
        public async Task<DataResult<bool>> Restart()
        {
            await _botControl.RestartAsync();
            return new DataResult<bool>().WithData(true);
        }
    }
}