using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FILM_Sparepart_MVC.Models
{
    [Table("Registration_Asset_Management")]
    public class Registration_Asset_Management
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }

        [Column("Site")]
        [StringLength(255)]
        public string? Site { get; set; }

        [Column("Item_Type")]
        [StringLength(255)]
        public string? Item_Type { get; set; }

        [Column("CEL_Number")]
        [StringLength(255)]
        public string? CEL_Number { get; set; }

        [Column("Status")]
        [StringLength(50)]
        public string? Status { get; set; }

        [Column("Owner")]
        [StringLength(255)]
        public string? Owner { get; set; }

        [Column("Fixed_Asset")]
        [StringLength(255)]
        public string? Fixed_Asset { get; set; }

        [Column("Location")]
        [StringLength(255)]
        public string? Location { get; set; }

        [Column("RFID")]
        [StringLength(255)]
        public string? RFID { get; set; }

        [Column("Department")]
        [StringLength(255)]
        public string? Department { get; set; }

        [Column("Model")]
        [StringLength(255)]
        public string? Model { get; set; }

        [Column("Purchase_Date")]
        public DateTime Purchase_Date { get; set; }

        [Column("Manufacturer")]
        [StringLength(255)]
        public string? Manufacturer { get; set; }

        [Column("Waranty_expiry")]
        public DateTime Waranty_expiry { get; set; }

        [Column("OS_version")]
        [StringLength(255)]
        public string? OS_version { get; set; }

        [Column("EOL_Support")]
        public DateTime EOL_Support { get; set; }

        [Column("Serial_Number")]
        [StringLength(255)]
        public string? Serial_Number { get; set; }

        [Column("Description")]
        [StringLength(500)]
        public string? Description { get; set; }

        [Column("Asset_Type")]
        [StringLength(255)]
        public string? Asset_Type { get; set; }

        [Column("Record_Type")]
        [StringLength(1)]
        public string? Record_Type { get; set; }

        [Column("Created_By")]
        [StringLength(255)]
        public string? Created_By { get; set; }

        [Column("Created_Date")]
        public DateTime Created_Date { get; set; }

        [Column("Created_Loc")]
        [StringLength(255)]
        public string? Created_Loc { get; set; }

        [Column("Update_By")]
        [StringLength(255)]
        public string? Update_By { get; set; }

        [Column("Update_Date")]
        public DateTime Update_Date { get; set; }

        [Column("Update_Loc")]
        [StringLength(255)]
        public string? Update_Loc { get; set; }

        [Column("IP_Address")]
        [StringLength(50)]
        public string? IP_Address { get; set; }

        [Column("MAC_Address")]
        [StringLength(50)]
        public string? MAC_Address { get; set; }

        [Column("Host_Name")]
        [StringLength(255)]
        public string? Host_Name { get; set; }

        [Column("System_Application")]
        [StringLength(255)]
        public string? System_Application { get; set; }

        [Column("Installation_Date")]
        public DateTime Installation_Date { get; set; }
    }
}