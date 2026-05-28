using System;
using System.ComponentModel.DataAnnotations;

namespace FILM_Sparepart_MVC.Models
{
    public class MM_SP_RFID_Audit
    {
        [Key]
        public int RFID_AUDIT_ID { get; set; }
        // Y = Yes, N = No
        public string RFID_FLAG { get; set; }
        public string RFID_TAG_CODE { get; set; }
        // 1 = INSERT, 3 = UPDATE, 5 = DELETE
        public string RECORD_TYPE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string CREATED_LOC { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
        public string UPDATED_LOC { get; set; }
        public int ID_LOCATION { get; set; }
    }


}