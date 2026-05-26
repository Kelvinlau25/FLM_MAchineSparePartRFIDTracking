using FILM_Sparepart_MVC.Models;
using Microsoft.EntityFrameworkCore;

namespace FILM_Sparepart_MVC.DAL
{
    public class TMS_ITEquiptContext : DbContext
    {
        // Constructor for dependency injection (EF Core 8)
        public TMS_ITEquiptContext(DbContextOptions<TMS_ITEquiptContext> options) : base(options) { }

        // Parameterless constructor for backward compatibility with legacy code
        public TMS_ITEquiptContext() : base() { }

        public DbSet<Registration_Asset_Management> Registration_Asset_Managements { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Only configure if not already configured (for backward compatibility)
            if (!optionsBuilder.IsConfigured)
            {
                // This will be used by legacy code that uses parameterless constructor
                // The connection string should be configured in appsettings.json
                optionsBuilder.UseSqlServer("name=SQLCon");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure entity mappings
            modelBuilder.Entity<Registration_Asset_Management>(entity =>
            {
                entity.ToTable("Registration_Asset_Management");
                entity.HasKey(e => e.ID);
            });
        }
    }
}