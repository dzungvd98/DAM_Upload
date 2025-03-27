namespace DAM_Upload.DTO
{
    public class FolderSavedDTO
    {
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int? ParentId { get; set; }
        public string Path { get; set; } = string.Empty;
        public string Status {  get; set; } = string.Empty;

    }
}
