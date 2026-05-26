using System;
using System.Collections.Generic;
using System.Linq;

namespace FILM_Sparepart_MVC.Models
{
    public class RfidEntryRequestModel
    {
        public string rfid { get; set; }
        public string location { get; set; }
        public string username { get; set; }
    }
}