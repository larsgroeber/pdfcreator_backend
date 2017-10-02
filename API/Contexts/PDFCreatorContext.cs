using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Contexts
{
    public class PDFCreatorContext: DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<Role> Roles { get; set; }

        public PDFCreatorContext(DbContextOptions<PDFCreatorContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Indexed properties need to be restricted in length!
            modelBuilder.Entity<User>().HasIndex(u => u.Name).IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}