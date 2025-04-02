using DAM_Upload.DTO;
using DAM_Upload.Services;
using DAM_Upload.Services.AuthService;
using DAM_Upload.Services.FileService;
using DAM_Upload.Services.FolderService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DAM_Upload.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FolderController : ControllerBase
    {
        private readonly IFolderService folderService;
        private readonly IFileService fileService;
        private readonly IAuthService _authService;
        private readonly SearchService searchService;


        public FolderController(IFolderService folderService, IFileService fileService, IAuthService authService, SearchService searchService)
        {
            this.folderService = folderService;
            this.fileService = fileService;
            _authService = authService;
            this.searchService = searchService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetFolderAndFile(int? folderId, int skip = 0)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized("Invalid user.");
                }
                var result = await folderService.GetFolderAndFileAsync(folderId, userId, skip);
                return Ok(result.Items);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
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

        [Authorize]
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

        [Authorize]
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

        [Authorize]
        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] SearchCriteriaDTO criteria = null, int skip = 0)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized("Invalid user.");
                }

                criteria ??= new SearchCriteriaDTO();

                var (items, hasMore) = await searchService.SearchAsync(userId, criteria, skip);
                return Ok(new
                {
                    Items = items,
                    HasMore = hasMore,
                    Skip = skip,
                    Take = 5
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }  
}
