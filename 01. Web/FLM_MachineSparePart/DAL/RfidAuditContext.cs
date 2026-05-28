using FILM_Sparepart_MVC.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace FILM_Sparepart_MVC.DAL
{
    public class RfidAuditContext : DbContext
    {

        public RfidAuditContext() : base("SQLCon") { }
        public DbSet<MM_SP_RFID_Audit> MM_SP_RFID_Audits { get; set; }
        public DbSet<MM_SP_STORAGE> MM_SP_STORAGEs { get; set; }
        public DbSet<MM_SPARE_PART_READER> MM_SPARE_PART_READERs { get; set; }
        public DbSet<MM_SPARE_PART> MM_SPARE_PARTs { get; set; }
        public DbSet<PVIEW_MM_SP_STORAGE> PVIEW_MM_SP_STORAGEs { get; set; }
        public DbSet<PVIEW_MM_SPARE_PART> PVIEW_MM_SPARE_PARTs { get; set; }
        



        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

    }
}