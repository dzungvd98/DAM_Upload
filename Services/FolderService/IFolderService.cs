using DAM_Upload.DTO;
using DAM_Upload.Models;

namespace DAM_Upload.Services.FolderService
{
    public interface IFolderService
    {
        Task<FolderDTO> CreateFolder(string folderNamem, int? parentId);
        Task<List<StorageDTO>> GetFolderAndFileAsync(int? folderId);
        Task<FolderDTO> UpdateFolderNameAsync(int? folderId, string newFolderName);
    }
}
