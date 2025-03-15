using DAM_Upload.Models;
namespace DAM_Upload.Services.FileService
{
    public interface IFileService
    {
        Task<Models.File> UploadFileAsync(int folderId, IFormFile file);
    }
}
