
namespace DAM_Upload.Services.FileService
{
    public class FileService : IFileService
    {
        private readonly DamUploadDbContext _context;
        public FileService(DamUploadDbContext context)
        {
            _context = context;
        }

        public async Task<Models.File> UploadFileAsync(int folderId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File không hợp lệ");

            var folder = await _context.Folders.FindAsync(folderId);
            if (folder == null)
                throw new KeyNotFoundException("Thư mục không tồn tại");

            string folderPath = folder.Path;
            Directory.CreateDirectory(folderPath);

            string extension = Path.GetExtension(folderPath);
            string filePath = Path.Combine(folderPath, file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var newFile = new Models.File
            {
                Name = file.Name,
                Format = extension,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Folder = folder,
                Size = (int) file.Length/1048576,
                ThumpnailUrl = "A"

            };

            _context.Files.Add(newFile);
            await _context.SaveChangesAsync();
            return newFile;
        }
    }
}
