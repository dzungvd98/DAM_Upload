using DAM_Upload.DTO;
using DAM_Upload.Models;
using PdfiumViewer;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Diagnostics;

namespace DAM_Upload.Services.FileService
{
    public class FileService : IFileService
    {
        private readonly DamUploadDbContext _context;
        public FileService(DamUploadDbContext context)
        {
            _context = context;
        }

        public async Task<StorageDTO> UploadFileAsync(int? folderId, IFormFile file, int userId)
        {
            ArgumentNullException.ThrowIfNull(file, "File invalid!");

            string folderPath;
            Folder folder = null;

            if (folderId.HasValue)
            {
                folder = await _context.Folders.FindAsync(folderId.Value);
                ArgumentNullException.ThrowIfNull(folder, "Folder doesn't exist!");
                folderPath = folder.Path;
            }
            else
            {
                // Lưu vào thư mục mặc định (ví dụ: D:/Uploads)
                folderPath = Path.Combine("Disk");
                Directory.CreateDirectory(folderPath); // Tạo nếu chưa tồn tại
            }

            string originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
            string extension = Path.GetExtension(file.FileName).ToLower();
            string newFileName = $"{originalFileName}{extension}";
            string filePath = Path.Combine(folderPath, newFileName);

            // Handle duplicate file
            int count = 1;
            while (System.IO.File.Exists(filePath))
            {
                newFileName = $"{originalFileName}_{count}{extension}";
                filePath = Path.Combine(folderPath, newFileName);
                count++;
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var newFile = new Models.File
            {
                Name = newFileName,
                Format = extension,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Folder = folder,
                Size = (int)file.Length,
                Path = filePath,
                OwnerId = userId
            };

            _context.Files.Add(newFile);
            await _context.SaveChangesAsync();

            return new StorageDTO
            {
                Id = newFile.FileId,
                Name = newFile.Name,
                Format = newFile.Format,
                Path = filePath
            };
        }

        
    }
}
