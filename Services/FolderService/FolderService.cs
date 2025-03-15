
using DAM_Upload.DTO;
using DAM_Upload.Models;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace DAM_Upload.Services.FolderService
{
    public class FolderService : IFolderService
    {
        public readonly DamUploadDbContext _context;

        public FolderService(DamUploadDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<FolderDTO> CreateFolder(string folderName, int? parentId)
        {
            var parent = parentId.HasValue ? await _context.Folders.FindAsync(parentId) : null;
            
            string path = parent != null ? Path.Combine(parent.Path, folderName) : folderName;
            int? validParentId = parent?.FolderId;

            bool isDuplicate = await _context.Folders
                .AnyAsync(f => f.Name == folderName && f.ParentId == parentId);

            if (isDuplicate)
            {
                throw new Exception($"A folder with the name '{folderName}' already exists in this directory.");
            }

            var folder = new Folder
            {
                Name = folderName,
                Path = path,
                ParentId = validParentId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            _context.Folders.Add(folder);
            await _context.SaveChangesAsync();
            var result = new FolderDTO
            {
                ParentId = validParentId,
                Path = path,
                FolderName = folderName,
            };
            return result;
        }

        public async Task<List<StorageDTO>> GetFolderAndFileAsync(int folderId)
        {
            var folder = await _context.Folders
                .Include(f => f.Children) 
                .Include(f => f.Files)
                .FirstOrDefaultAsync(f => f.FolderId == folderId);

            if (folder == null)
            {
                throw new Exception("Folder not exists");
            }

            var result = folder.Children
                .Select(child => new StorageDTO
                {
                    Id = child.FolderId,
                    Name = child.Name,
                    Path = child.Path,
                    Format = "Folder"
                })
                .Concat(folder.Files.Select(f => new StorageDTO
                {
                    Id = f.FileId,
                    Name = f.Name,
                    Format = f.Format
                }))
                .ToList();
            return result;
        }
    }
}
