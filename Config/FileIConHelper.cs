namespace DAM_Upload.Config
{
    public static class FileIconHelper
    {
        private static readonly Dictionary<string, string> FileIcons = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            // Document
            { ".doc", "Assets/icons/word.png" },
            { ".docx", "Assets/icons/word.png" },
            { ".xls", "Assets/icons/excel.png" },
            { ".xlsx", "Assets/icons/excel.png" },
            { ".ppt", "Assets/icons/powerpoint.png" },
            { ".pptx", "Assets/icons/powerpoint.png" },
            { ".pdf", "Assets/icons/pdf.png" },
            { ".txt", "Assets/icons/txt.png" },
            { ".csv", "Assets/icons/csv.png" },

            // Image
            { ".jpg", "Assets/icons/jpeg.png" },
            { ".jpeg", "Assets/icons/jpeg.png" },
            { ".png", "Assets/icons/jpeg.png" },
            { ".gif", "Assets/icons/jpeg.png" },
            { ".svg", "Assets/icons/jpeg.png" },
            { ".bmp", "Assets/icons/jpeg.png" },
            { ".webp", "Assets/icons/jpeg.png" },

            // Audio
            { ".mp3", "Assets/icons/audio.png" },
            { ".wav", "Assets/icons/audio.png" },
            { ".ogg", "Assets/icons/audio.png" },
            { ".aac", "Assets/icons/audio.png" },
            { ".flac", "Assets/icons/audio.png" },

            // Video
            { ".mp4", "Assets/icons/video.png" },
            { ".mov", "Assets/icons/video.png" },
            { ".avi", "Assets/icons/video.png" },
            { ".mkv", "Assets/icons/video.png" },
            { ".webm", "Assets/icons/video.png" },
            { ".flv", "Assets/icons/video.png" },

            // Archive
            { ".zip", "Assets/icons/zip.png" },
            { ".rar", "Assets/icons/zip.png" },
            { ".7z", "Assets/icons/zip.png" },
            { ".tar.gz", "Assets/icons/zip.png" },

            // Database
            { ".sql", "Assets/icons/file.png" },
            { ".db", "Assets/icons/file.png" },

            // Code
            { ".html", "Assets/icons/script.png" },
            { ".css", "Assets/icons/script.png" },
            { ".js", "Assets/icons/script.png" },
            { ".java", "Assets/icons/script.png" },
            { ".py", "Assets/icons/script.png" },
            { ".cs", "Assets/icons/script.png" },
            { ".cpp", "Assets/icons/script.png" },

            // Executable
            { ".exe", "Assets/icons/file.png" },
            { ".msi", "Assets/icons/file.png" },
            { ".sh", "Assets/icons/file.png" },

            // Default
            { "default", "Assets/icons/file.png" }
        };

        public static string GetFileIcon(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            return FileIcons.TryGetValue(extension, out var iconPath) ? iconPath : FileIcons["default"];
        }
    }

}
