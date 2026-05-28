using System;
using System.Collections.Generic;
using System.Linq;

namespace FILM_Sparepart_MVC.Models
{
    public class ReaderRFIDModel
    {
        public int ID { get; set; }
        public string IP_ADDRESS { get; set; }
        public string HOST_NAME { get; set; }
        public string PORT { get; set; }
        public string SP { get; set; }
        public string SERVER { get; set; }
        public string SP2 { get; set; }
        public string SERVER2 { get; set; }
        public string COMPANY { get; set; }
        public string LOCATION { get; set; }
        public int RSSI { get; set; }
    }
}