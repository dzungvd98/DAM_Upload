
using AutoMapper;
using DAM_Upload.Config;
using DAM_Upload.DTO;
using DAM_Upload.Models;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
namespace DAM_Upload.Services.FolderService
{
    public class FolderService : IFolderService
    {
        private readonly DamUploadDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FolderService(DamUploadDbContext dbContext, IConfiguration configuration, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _context = dbContext;
            _configuration = configuration;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;

        }

        public async Task<FolderDTO> CreateFolder(string folderName, int? parentId)
        {
            var parent = parentId.HasValue ? await _context.Folders.FindAsync(parentId) : null;
            if(parent == null && parentId != 0)
            {
                throw new Exception("Folder not found!");
            }



            string storageDisk = "Disk";
            string path = parent != null ? Path.Combine(parent.Path, folderName) : Path.Combine(storageDisk, folderName);
            string originalFolderName = folderName;
            string basePath = parent != null ? parent.Path : storageDisk;

            
            int count = 1;
            while (Directory.Exists(path) || await _context.Folders.AnyAsync(f => f.Name == folderName && f.ParentId == parentId))
            {
                folderName = $"{originalFolderName}{count}";
                path = Path.Combine(basePath, folderName);
                count++;
            }

            Directory.CreateDirectory(path);
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
                Name = folderName
            };
            return result;
        }

        

        public async Task<List<StorageDTO>> GetFolderAndFileAsync(int? folderId)
        {
            var folders = await _context.Folders
            .Where(f => folderId == 0 ? f.ParentId == null : f.FolderId == folderId)
            .Select(f => new StorageDTO
            {
                Id = f.FolderId,
                Name = f.Name,
                Path = f.Path,
                Format = "Folder",
            })
            .ToListAsync();

            string hostlink = _configuration["Path:Link"];

            var files = await _context.Files
                .Where(f => folderId == 0 ? f.Folder == null : f.Folder.FolderId == folderId)
                .Select(f => new StorageDTO
                {
                    Id = f.FileId,
                    Name = f.Name,
                    Path = f.Path,
                    Format = f.Format,
                    IconLink = $"{hostlink}/api/icon/{f.Format}"
                })
                .ToListAsync();

            return folders.Concat(files).ToList();
        }


        public async Task<FolderDTO> UpdateFolderNameAsync(int? folderId, string newFolderName)
        {
            var folder = await _context.Folders.FindAsync(folderId);
            ArgumentNullException.ThrowIfNull(folder, "Folder doesn't exist!");

            var parent = folder.ParentId.HasValue ? await _context.Folders.FindAsync(folder.ParentId) : null;
            string basePath = parent != null ? parent.Path : "Disk";
            string newPath = Path.Combine(basePath, newFolderName);

            bool isDuplicate = Directory.Exists(newPath) || await _context.Folders.AnyAsync(f => f.Name == newFolderName && f.ParentId == folder.ParentId);
            if (isDuplicate)
            {
                throw new Exception($"A folder with the name '{newFolderName}' already exists in this directory.");
            }

            if (Directory.Exists(folder.Path))
            {
                Directory.Move(folder.Path, newPath);
            }

            folder.Name = newFolderName;
            folder.Path = newPath;
            folder.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return new FolderDTO
            {
                ParentId = folder.ParentId,
                Path = newPath,
                Name = newFolderName
            };
        }
    }
}
