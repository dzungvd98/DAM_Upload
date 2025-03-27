using DAM_Upload.Services.FileService;
using DAM_Upload.Services.FolderService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DAM_Upload.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FolderController : ControllerBase
    {
        public readonly IFolderService folderService;
        public readonly IFileService fileService;

        public FolderController(IFolderService folderService, IFileService fileService)
        {
            this.folderService = folderService;
            this.fileService = fileService;
        }


        [HttpGet]
        public async Task<IActionResult> GetFolderAndFile(int folderId)
        {
            try
            {
                var result = await folderService.GetFolderAndFileAsync(folderId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateFolder(string folderName, int parentId)
        {
            try
            {
                var folderCreated = await folderService.CreateFolder(folderName, parentId);
                return Ok(folderCreated);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("upload-file")]
        public async Task<IActionResult> uploadFileAsync(int? folderId, IFormFile file)
        {
            try
            {
                var fileUploadted = await fileService.UploadFileAsync(folderId, file);
                return Ok(fileUploadted);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{folderId}")]
        public async Task<IActionResult> UpdateFolder(int folderId, string folderName)
        {
            try
            {
                var folderUpdated = await folderService.UpdateFolderNameAsync(folderId, folderName);
                return Ok(folderUpdated);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }  
}
