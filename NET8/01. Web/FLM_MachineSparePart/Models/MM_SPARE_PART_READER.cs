using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FILM_Sparepart_MVC.Models
{
    [Table("MM_SPARE_PART_READER")]
    public class MM_SPARE_PART_READER
    {
        [Key]
        [Column("READER_ID")]
        public int READER_ID { get; set; }

        [Column("STORAGE_CODE")]
        [StringLength(255)]
        public string? STORAGE_CODE { get; set; }

        [Column("READER_NAME")]
        [StringLength(255)]
        public string? READER_NAME { get; set; }

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

        [Column("READER_IP")]
        [StringLength(50)]
        public string? READER_IP { get; set; }
    }
}