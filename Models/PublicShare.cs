namespace DAM_Upload.Models
{
    public class PublicShare
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public int OwnerId { get; set; }
        public User Owner { get; set; } = null!;
        public int? FolderId { get; set; }
        public Folder? Folder { get; set; }
        public int? FileId { get; set; }
        public File? File { get; set; }
        public PermissionType PermissionType { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset ExpiresAt { get; set; }
    }
}
