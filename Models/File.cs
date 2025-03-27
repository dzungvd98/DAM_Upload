namespace DAM_Upload.Models
{
    public class File
    {
        public int FileId { get; set; }
        public string Name { get; set; }
        public string Format { get; set; }
        public int Size { get; set; }
        public Folder? Folder { get; set; }

        public string? Path { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
