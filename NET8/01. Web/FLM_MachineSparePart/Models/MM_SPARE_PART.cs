using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FILM_Sparepart_MVC.Models
{
    [Table("MM_SPARE_PART")]
    public class MM_SPARE_PART
    {
        [Key]
        [Column("SPARE_PART_ID")]
        public int SPARE_PART_ID { get; set; }

        [Column("SPARE_PART_MODEL_ID")]
        public int SPARE_PART_MODEL_ID { get; set; }

        [Column("SERIES_NO")]
        [StringLength(255)]
        public string? SERIES_NO { get; set; }

        [Column("PART_NO")]
        [StringLength(255)]
        public string? PART_NO { get; set; }

        [Column("SPARE_PART_MACHINE_ID")]
        public int SPARE_PART_MACHINE_ID { get; set; }

        [Column("RFID_TYPE_ID")]
        public int RFID_TYPE_ID { get; set; }

        [Column("RFID_TAG_ID")]
        [StringLength(255)]
        public string? RFID_TAG_ID { get; set; }

        [Column("RFID_TAG_ID_2")]
        [StringLength(255)]
        public string? RFID_TAG_ID_2 { get; set; }

        [Column("STORAGE_ID")]
        public int STORAGE_ID { get; set; }

        [Column("STORAGE_DESCRIPTION1")]
        [StringLength(255)]
        public string? STORAGE_DESCRIPTION1 { get; set; }

        [Column("STORAGE_DESCRIPTION2")]
        [StringLength(255)]
        public string? STORAGE_DESCRIPTION2 { get; set; }

        [Column("MANUFACTURER_ID")]
        public int MANUFACTURER_ID { get; set; }

        [Column("STATUS")]
        [StringLength(50)]
        public string? STATUS { get; set; }

        [Column("REMARKS")]
        [StringLength(500)]
        public string? REMARKS { get; set; }

        [Column("RECORD_TYPE")]
        [StringLength(1)]
        public string? RECORD_TYPE { get; set; }

        [Column("CREATED_BY")]
        [StringLength(255)]
        public string? CREATED_BY { get; set; }

        [Column("CREATED_DATE")]
        public DateTime? CREATED_DATE { get; set; }

        [Column("CREATED_LOC")]
        [StringLength(255)]
        public string? CREATED_LOC { get; set; }

        [Column("UPDATED_BY")]
        [StringLength(255)]
        public string? UPDATED_BY { get; set; }

        [Column("UPDATED_DATE")]
        public DateTime? UPDATED_DATE { get; set; }

        [Column("UPDATED_LOC")]
        [StringLength(255)]
        public string? UPDATED_LOC { get; set; }
    }
}