using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FILM_Sparepart_MVC.Models
{
    public class MM_SPARE_PART_READER
    {
        [Key]
        public int READER_ID { get; set; }
        public string STORAGE_CODE { get; set; }
        public string READER_NAME { get; set; }
        public string REMARKS { get; set; }
        public string RECORD_TYPE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string CREATED_LOC { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
        public string UPDATED_LOC { get; set; }
        public string READER_IP { get; set; }
    }
}