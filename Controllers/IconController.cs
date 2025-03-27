using DAM_Upload.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DAM_Upload.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IconController : ControllerBase
    {
        [HttpGet("{format}")]
        public IActionResult GetIcon(string format)
        {
            try
            {
                string pathIcon = FileIconHelper.GetFileIcon(format);
                if (!System.IO.File.Exists(pathIcon))
                    return NotFound("Icon not found");
                var fileStream = System.IO.File.OpenRead(pathIcon);
                var contentType = GetContentType(pathIcon);

                Response.Headers.Add("Content-Disposition", $"inline; filename=\"{Path.GetFileName(pathIcon)}\"");
                return File(fileStream, contentType);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private string GetContentType(string path)
        {
            var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(path, out var contentType))
            {
                contentType = "application/octet-stream"; // default 
            }
            return contentType;
        }
    }
}
