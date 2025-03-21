﻿using System.ComponentModel.DataAnnotations.Schema;

namespace DAM_Upload.Models
{
    public class Folder
    {
        public int FolderId { get; set; }
        public string Name { get; set; }  
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int? ParentId {  get; set; }
        public string Path {  get; set; }

        [ForeignKey("ParentId")]
        public virtual Folder ParentFolder { get; set; }
        public virtual ICollection<Folder> Children { get; set; } = new List<Folder>();
        
        public virtual ICollection<File> Files { get; set; } = new List<File>();
    }
}
