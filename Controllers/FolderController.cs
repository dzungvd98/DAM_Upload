using DAM_Upload.Services;
using DAM_Upload.Services.AuthService;
using DAM_Upload.Services.FileService;
using DAM_Upload.Services.FolderService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DAM_Upload.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FolderController : ControllerBase
    {
        public readonly IFolderService folderService;
        public readonly IFileService fileService;
        private readonly IAuthService _authService;

        public FolderController(IFolderService folderService, IFileService fileService, IAuthService authService)
        {
            this.folderService = folderService;
            this.fileService = fileService;
            _authService = authService;
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
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized("Invalid user.");
                }
                var folderCreated = await folderService.CreateFolder(folderName, parentId, userId);
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
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized("Invalid user.");
                }
                var fileUploadted = await fileService.UploadFileAsync(folderId, file, userId);
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
