using DAM_Upload.Models;
using DAM_Upload.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAM_Upload.Services
{
    public class SearchService
    {
        private readonly DamUploadDbContext _context;
        private const int DefaultPageSize = 5;
        private readonly IConfiguration _configuration;

        public SearchService(DamUploadDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<(List<StorageDTO> Items, bool HasMore)> SearchAsync(int userId, SearchCriteriaDTO criteria, int skip = 0, int take = DefaultPageSize)
        {
            // Đảm bảo skip và take hợp lệ
            skip = Math.Max(0, skip);
            take = Math.Max(1, take);

            // Lấy danh sách Folders
            IQueryable<Folder> folderQuery = _context.Folders
                .Where(f => f.OwnerId == userId); // Lọc theo UserId

            // Lấy danh sách Files
            IQueryable<DAM_Upload.Models.File> fileQuery = _context.Files
                .Where(f => f.Folder != null && f.Folder.OwnerId == userId); // Lọc theo UserId của Folder

            // Áp dụng các tiêu chí tìm kiếm
            if (!string.IsNullOrWhiteSpace(criteria.Name))
            {
                var nameLower = criteria.Name.ToLower();
                folderQuery = folderQuery.Where(f => f.Name.ToLower().Contains(nameLower));
                fileQuery = fileQuery.Where(f => f.Name.ToLower().Contains(nameLower));
            }

            if (!string.IsNullOrWhiteSpace(criteria.Format))
            {
                if (criteria.Format.ToLower() == "folder")
                {
                    fileQuery = fileQuery.Where(f => false); // Không lấy file
                }
                else if (criteria.Format.ToLower() == "file")
                {
                    folderQuery = folderQuery.Where(f => false); // Không lấy folder
                }
            }


            if (!string.IsNullOrWhiteSpace(criteria.Format))
            {
                var formatLower = criteria.Format.ToLower();
                fileQuery = fileQuery.Where(f => f.Format.ToLower() == formatLower);
            }

            // Chuyển đổi Folders thành FolderFileDto
            var folders = await folderQuery
                .Select(f => new StorageDTO
                {
                    Id = f.FolderId,
                    Format = "Folder",
                    Name = f.Name,
                    Path = f.Path
                })
                .ToListAsync();

            string hostlink = _configuration["Path:Link"];
            // Chuyển đổi Files thành FolderFileDto
            var files = await fileQuery
                .Select(f => new StorageDTO
                {
                    Id = f.FileId,
                    Format = "Folder",
                    Name = f.Name,
                    Path = f.Path,
                    IconLink = $"{hostlink}/api/icon/{f.Format}"
                })
                .ToListAsync();

            // Kết hợp danh sách Folders và Files, sắp xếp theo CreatedAt (mới nhất trước)
            var combinedList = folders.Concat(files)
                .ToList();

            // Tính toán số lượng mục còn lại
            int totalCount = combinedList.Count;
            bool hasMore = skip + take < totalCount;

            // Lấy dữ liệu cho lần tải hiện tại
            var items = combinedList
                .Skip(skip)
                .Take(take)
                .ToList();

            return (items, hasMore);
        }
    }
}