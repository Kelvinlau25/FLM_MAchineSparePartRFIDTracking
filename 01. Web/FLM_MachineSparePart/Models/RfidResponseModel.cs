using System;
using System.Collections.Generic;

namespace FILM_Sparepart_MVC.Models
{
    public class RfidResponseModel
    {
        public List<InquiryTable> inquiryTables { get; set; }
        public List<RfidReportTable> rfidReportTables { get; set; }

        public Dictionary<int, String> locations {get;set; }
        public Dictionary<int, String> reports { get; set; }
        
    }
}