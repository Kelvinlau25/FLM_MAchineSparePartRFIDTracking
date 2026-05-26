using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FILM_Sparepart_MVC.Models
{
    [Table("MM_SP_RFID_Audit")]
    public class MM_SP_RFID_Audit
    {
        [Key]
        [Column("RFID_AUDIT_ID")]
        public int RFID_AUDIT_ID { get; set; }

        // Y = Yes, N = No
        [Column("RFID_FLAG")]
        [StringLength(1)]
        public string? RFID_FLAG { get; set; }

        [Column("RFID_TAG_CODE")]
        [StringLength(255)]
        [Required] // Database column does not allow NULL
        public string RFID_TAG_CODE { get; set; } = string.Empty;

        // 1 = INSERT, 3 = UPDATE, 5 = DELETE
        [Column("RECORD_TYPE")]
        [StringLength(1)]
        public string? RECORD_TYPE { get; set; }

        [Column("CREATED_BY")]
        [StringLength(255)]
        public string? CREATED_BY { get; set; }

        [Column("CREATED_DATE")]
        public DateTime CREATED_DATE { get; set; }

        [Column("CREATED_LOC")]
        [StringLength(255)]
        public string? CREATED_LOC { get; set; }

        [Column("UPDATED_BY")]
        [StringLength(255)]
        public string? UPDATED_BY { get; set; }

        [Column("UPDATED_DATE")]
        public DateTime UPDATED_DATE { get; set; }

        [Column("UPDATED_LOC")]
        [StringLength(255)]
        public string? UPDATED_LOC { get; set; }

        [Column("ID_LOCATION")]
        public int ID_LOCATION { get; set; }
    }
}