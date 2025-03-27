using DAM_Upload.Config;
using DAM_Upload.Services.FileService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DAM_Upload.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly DamUploadDbContext _context;

        public FileController(IFileService fileService, DamUploadDbContext context)
        {
            _fileService = fileService;
            _context = context;
        }


        [HttpGet("{fileId}/open")]
        public async Task<IActionResult> GetFile(int fileId)
        {
            try
            {
                var file = await _context.Files.FirstOrDefaultAsync(f => f.FileId == fileId);
                if (file == null)
                    return NotFound("File not found!");

                if (!System.IO.File.Exists(file.Path))
                    return NotFound("File does not exist on server!");

                var fileStream = System.IO.File.OpenRead(file.Path);
                var contentType = GetContentType(file.Path);

                Response.Headers.Add("Content-Disposition", $"inline; filename=\"{Path.GetFileName(file.Path)}\"");
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
                contentType = "application/octet-stream"; // default nếu không xác định được
            }
            return contentType;
        }

        

    }
}
