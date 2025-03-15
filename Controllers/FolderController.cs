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

        public FolderController(IFolderService folderService)
        {
            this.folderService = folderService;
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

    }  
}
