
namespace FILM_Sparepart_MVC.Helper_Code.Objects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class UnknownObj
    {
        
        public string RFID_ID { get; set; } 
        public string SERIES_NO { get; set; }
        public string MODEL_NO { get; set; }
        public string PART_NO { get; set; }
        public string MACHINE_MODEL { get; set; }
        public string STORAGE_CODE { get; set; }
        public string STORAGE_DESCRIPTION_1 { get; set; }
        public string STORAGE_DESCRIPTION_2 { get; set; }
        public string MANUFACTURER { get; set; }
        public string TRANSACTION_TYPE { get; set; }
        public string STATUS { get; set; }
        public string CREATED_BY { get; set; }
        public string CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }

      
    }

    public class AdUnknownObj
    {
        public string AD_RFID_ID { get; set; }
    }
}