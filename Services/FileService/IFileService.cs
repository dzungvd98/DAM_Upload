using DAM_Upload.DTO;
using DAM_Upload.Models;
namespace DAM_Upload.Services.FileService
{
    public interface IFileService
    {
        Task<StorageDTO> UploadFileAsync(int? folderId, IFormFile file, int userId);
    }
}
