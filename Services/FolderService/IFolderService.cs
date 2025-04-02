using DAM_Upload.DTO;
using DAM_Upload.Models;

namespace DAM_Upload.Services.FolderService
{
    public interface IFolderService
    {
        Task<FolderDTO> CreateFolder(string folderNamem, int? parentId, int userId);
        Task<(List<StorageDTO> Items, bool HasMore)> GetFolderAndFileAsync(int? folderId, int userId, int skip, int take = 10);
        Task<FolderDTO> UpdateFolderNameAsync(int? folderId, string newFolderName);
    }
}
