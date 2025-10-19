using Microsoft.EntityFrameworkCore;
using ThumbnailWorker.Entity;
namespace ThumbnailWorker.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Image> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Image>(entity =>
            {
                entity.ToTable("Image");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id");

                entity.Property(e => e.ImagePath)
                    .HasColumnName("ImagePath")
                    .HasMaxLength(500)
                    .IsRequired();

                entity.Property(e => e.ThumbnailPath)
                    .HasColumnName("ThumbnailPath")
                    .HasMaxLength(500);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
