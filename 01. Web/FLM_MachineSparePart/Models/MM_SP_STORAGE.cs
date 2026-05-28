using System;
using System.ComponentModel.DataAnnotations;

namespace FILM_Sparepart_MVC.Models
{
    public class MM_SP_STORAGE
    {
        [Key]
        public int STORAGE_ID { get; set; }
        public string STORAGE_DESC_1 { get; set; }
        public string STORAGE_DESC_2 { get; set; }
        public int READER_ID { get; set; }
        public string REMARKS { get; set; }
        public string RECORD_TYPE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string CREATED_LOC { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
        public string UPDATED_LOC { get; set; }
        public string STORAGE_CODE { get; set; }
    }
}