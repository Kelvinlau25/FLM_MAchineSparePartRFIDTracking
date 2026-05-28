using FILM_Sparepart_MVC.Models;
using Microsoft.EntityFrameworkCore;

namespace FILM_Sparepart_MVC.DAL
{
    public class RfidAuditContext : DbContext
    {
        // Constructor for dependency injection (EF Core 8)
        public RfidAuditContext(DbContextOptions<RfidAuditContext> options) : base(options) { }

        // Parameterless constructor for backward compatibility with legacy code
        public RfidAuditContext() : base() { }

        public DbSet<MM_SP_RFID_Audit> MM_SP_RFID_Audits { get; set; }
        public DbSet<MM_SP_STORAGE> MM_SP_STORAGEs { get; set; }
        public DbSet<MM_SPARE_PART_READER> MM_SPARE_PART_READERs { get; set; }
        public DbSet<MM_SPARE_PART> MM_SPARE_PARTs { get; set; }
        public DbSet<PVIEW_MM_SP_STORAGE> PVIEW_MM_SP_STORAGEs { get; set; }
        public DbSet<PVIEW_MM_SPARE_PART> PVIEW_MM_SPARE_PARTs { get; set; }

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
            modelBuilder.Entity<MM_SP_RFID_Audit>(entity =>
            {
                entity.ToTable("MM_SP_RFID_Audit");
                entity.HasKey(e => e.RFID_AUDIT_ID);
            });

            modelBuilder.Entity<MM_SP_STORAGE>(entity =>
            {
                entity.ToTable("MM_SP_STORAGE");
                entity.HasKey(e => e.STORAGE_ID);
            });

            modelBuilder.Entity<MM_SPARE_PART_READER>(entity =>
            {
                entity.ToTable("MM_SPARE_PART_READER");
                entity.HasKey(e => e.READER_ID);
            });

            modelBuilder.Entity<MM_SPARE_PART>(entity =>
            {
                entity.ToTable("MM_SPARE_PART");
                entity.HasKey(e => e.SPARE_PART_ID);
            });

            modelBuilder.Entity<PVIEW_MM_SP_STORAGE>(entity =>
            {
                entity.ToTable("PVIEW_MM_SP_STORAGE");
                entity.HasKey(e => e.STORAGE_ID);
            });

            modelBuilder.Entity<PVIEW_MM_SPARE_PART>(entity =>
            {
                entity.ToTable("PVIEW_MM_SPARE_PART");
                entity.HasKey(e => e.SPARE_PART_ID);
            });
        }
    }
}