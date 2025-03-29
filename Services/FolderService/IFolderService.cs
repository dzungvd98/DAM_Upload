using DAM_Upload.DTO;
using DAM_Upload.Models;

namespace DAM_Upload.Services.FolderService
{
    public interface IFolderService
    {
        Task<FolderDTO> CreateFolder(string folderNamem, int? parentId, int userId);
        Task<List<StorageDTO>> GetFolderAndFileAsync(int? folderId, int userId);
        Task<FolderDTO> UpdateFolderNameAsync(int? folderId, string newFolderName);
    }
}
