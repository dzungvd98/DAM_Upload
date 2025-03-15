using DAM_Upload.Services.FileService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DAM_Upload.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }


        [HttpPost("upload")]
        public async Task<IActionResult> CreateFile(int folderId, IFormFile file)
        {
            try
            {
                var fileUploadted = await _fileService.UploadFileAsync(folderId, file);
                return Ok(fileUploadted);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
