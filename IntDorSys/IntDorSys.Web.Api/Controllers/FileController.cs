using IntDorSys.Services.FileStorage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using Ouro.WebApiUtils;

namespace IntDorSys.Web.Api.Controllers
{
    [Route("file")]
    [Authorize]
    public sealed class FileController : ApiController
    {
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetAsync(
            Guid id,
            [FromServices] IFileService fileService)
        {
            var file = (await fileService.GetAsync(id, HttpContext.RequestAborted))?.Data;

            if (file == null)
            {
                return NotFound();
            }

            var contentType = MimeTypes.GetMimeType(file.Extension);

            return File(file.Content ?? [], contentType);
        }
    }
}