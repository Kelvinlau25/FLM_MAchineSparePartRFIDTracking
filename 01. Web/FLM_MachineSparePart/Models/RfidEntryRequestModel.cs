using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FILM_Sparepart_MVC.Models
{
    public class RfidEntryRequestModel
    {
        public string rfid { get; set; }
        public string location { get; set; }
    }
}