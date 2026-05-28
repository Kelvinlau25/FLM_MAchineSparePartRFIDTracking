using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FILM_Sparepart_MVC.Models
{
    public class MM_SPARE_PART
    {
        [Key]
        public int SPARE_PART_ID { get; set; }
        public int SPARE_PART_MODEL_ID { get; set; }
        public string SERIES_NO { get; set; }
        public string PART_NO { get; set; }
        public int SPARE_PART_MACHINE_ID { get; set; }
        public int RFID_TYPE_ID { get; set; }
        public string RFID_TAG_ID { get; set; }
        public string RFID_TAG_ID_2 { get; set; }
        public int STORAGE_ID { get; set; }
        public string STORAGE_DESCRIPTION1 { get; set; }
        public string STORAGE_DESCRIPTION2 { get; set; }
        public int MANUFACTURER_ID { get; set; }
        public string STATUS { get; set; }
        public string REMARKS { get; set; }
        public string RECORD_TYPE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string CREATED_LOC { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
        public string UPDATED_LOC { get; set; }
    }
}