using DAM_Upload.DTO;
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

        public async Task<StorageDTO> UploadFileAsync(int folderId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File không hợp lệ");

            var folder = await _context.Folders.FindAsync(folderId);
            if (folder == null)
                throw new KeyNotFoundException("Thư mục không tồn tại");

            string folderPath = folder.Path;
            Directory.CreateDirectory(folderPath);

            string originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
            string extension = Path.GetExtension(file.FileName).ToLower();
            string newFileName = $"{Guid.NewGuid()}{extension}";
            string filePath = Path.Combine(folderPath, newFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Xử lý tên file trùng
            int count = 1;
            while (System.IO.File.Exists(filePath))
            {
                newFileName = $"{originalFileName}_{count:D2}{extension}";
                filePath = Path.Combine(folderPath, newFileName);
                count++;
            }

            var newFile = new Models.File
            {
                Name = newFileName,
                Format = extension,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Folder = folder,
                Size = (int)file.Length / 1048576,
                Path = filePath
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
