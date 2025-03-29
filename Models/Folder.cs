using System.ComponentModel.DataAnnotations.Schema;

namespace DAM_Upload.Models
{
    public class Folder
    {
        public int FolderId { get; set; }
        public string Name { get; set; }  = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int? ParentId {  get; set; }
        public string Path {  get; set; } = string.Empty;

        public int OwnerId { get; set; }

        //public StatusFolder status;

        [ForeignKey("ParentId")]

        public virtual Folder ParentFolder { get; set; }
        public virtual ICollection<Folder> Children { get; set; } = new List<Folder>();
       
        public virtual ICollection<File> Files { get; set; } = new List<File>();

        public ICollection<PermissionFolder> FolderPermissions { get; set; } = new List<PermissionFolder>();
    }

    public enum StatusFolder
    {
        PUBLIC, PRIVATE
    }
}
