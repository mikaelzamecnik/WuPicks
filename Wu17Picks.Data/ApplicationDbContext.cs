using Microsoft.EntityFrameworkCore;
using Wu17Picks.Data.Entities;
using Wu17Picks.Data.Models;

namespace Wu17Picks.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<GalleryImage> GalleryImages { get; set; }
        public DbSet<ImageTag> ImageTags { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
