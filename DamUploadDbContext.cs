using DAM_Upload.Models;
using Microsoft.EntityFrameworkCore;

namespace DAM_Upload
{
    public class DamUploadDbContext : DbContext
    {
        public DamUploadDbContext(DbContextOptions<DamUploadDbContext> options) : base(options) { }
        
        public DbSet<Folder> Folders { get; set; }
        public DbSet<Models.File> Files { get; set; }
        public DbSet<Icon> Icons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
