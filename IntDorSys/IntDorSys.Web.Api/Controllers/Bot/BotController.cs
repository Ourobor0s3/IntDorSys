using IntDorSys.Web.Api.Bot;
using IntDorSys.Web.Api.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntDorSys.Web.Api.Controllers.Bot
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class BotController : ProtectedApiController
    {
        private readonly BotStatus _botStatus;
        private readonly IBotControlService _botControl;

        public BotController(BotStatus botStatus, IBotControlService botControl)
        {
            _botStatus = botStatus;
            _botControl = botControl;
        }

        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            return Ok(new
            {
                running = _botStatus.IsRunning,
                username = _botStatus.BotUsername,
                lastStarted = _botStatus.LastStartedAt,
            });
        }

        [HttpPost("restart")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Restart()
        {
            try
            {
                await _botControl.RestartAsync();
                return Ok(new { message = "Bot restarted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Bot restart failed", error = ex.Message });
            }
        }
    }
}