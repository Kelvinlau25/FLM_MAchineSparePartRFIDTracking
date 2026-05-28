using FILM_Sparepart_MVC.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace FILM_Sparepart_MVC.DAL
{
    public class TMS_ITEquiptContext : DbContext
    {
        public TMS_ITEquiptContext() : base("SQLCon") { }
        public DbSet<Registration_Asset_Management> Registration_Asset_Managements { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

    }
}