using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FILM_Sparepart_MVC.Model
{
    public class InquiryRequestModel
    {
        public DateTime dateFrom { get; set; }
        public DateTime dateTo { get; set; }
        public string location { get; set; }
        public string rfid { get; set; }
    }
}