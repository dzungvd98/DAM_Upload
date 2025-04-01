
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

        private const int DefaultPageSize = 5;

        public FolderService(DamUploadDbContext dbContext, IConfiguration configuration, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _context = dbContext;
            _configuration = configuration;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;

        }

        public async Task<FolderDTO> CreateFolder(string folderName, int? parentId, int userId)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var parent = parentId.HasValue ? await _context.Folders.FindAsync(parentId) : null;
                    if (parent == null && parentId != 0)
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
                        OwnerId = userId
                    };

                    _context.Folders.Add(folder);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync(); // ✅ Commit nếu không có lỗi

                    return new FolderDTO
                    {
                        ParentId = validParentId,
                        Path = path,
                        Name = folderName
                    };
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync(); // ❌ Rollback nếu có lỗi
                    throw;
                }
            }
        }

        

        public async Task<(List<StorageDTO> Items, bool HasMore)> GetFolderAndFileAsync(int? folderId, int userId, int skip, int take = DefaultPageSize)
        {
            
            skip = Math.Max(0, skip);
            take = Math.Max(1, take);

            // Lấy danh sách Folders
            IQueryable<Folder> folderQuery = _context.Folders
                .Where(f => f.OwnerId == userId); // Lọc theo UserId

            if (folderId.HasValue)
            {
                folderQuery = folderQuery.Where(f => f.ParentId == folderId.Value);
            }


            var folders = await folderQuery
            .Select(f => new StorageDTO
            {
                Id = f.FolderId,
                Name = f.Name,
                Path = f.Path,
                Format = "Folder",
            })
            .ToListAsync();

            string hostlink = _configuration["Path:Link"];

            // Lấy danh sách Files
            IQueryable<DAM_Upload.Models.File> fileQuery = _context.Files
                .Where(f => f.Folder != null && f.Folder.OwnerId == userId); // Lọc theo UserId của Folder

            if (folderId.HasValue)
            {
                fileQuery = fileQuery.Where(f => f.Folder != null && f.Folder.FolderId == folderId.Value);
            }

            var files = await _context.Files
                .Select(f => new StorageDTO
                {
                    Id = f.FileId,
                    Name = f.Name,
                    Path = f.Path,
                    Format = f.Format,
                    IconLink = $"{hostlink}/api/icon/{f.Format}"
                })
                .ToListAsync();
            var combinedList = folders.Concat(files)
                .ToList();

            int totalCount = combinedList.Count;
            bool hasMore = skip + take < totalCount;

            var items = combinedList
               .Skip(skip)
               .Take(take)
               .ToList();

            return (items, hasMore);
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
