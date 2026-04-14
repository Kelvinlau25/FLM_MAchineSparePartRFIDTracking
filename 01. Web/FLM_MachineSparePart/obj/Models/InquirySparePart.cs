using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FILM_Sparepart_MVC.Models
{
    public class InquirySparePart
    {
        public int SPARE_PART_ID { get; set; }
        public string Spare_Part_Model_ID { get; set; }
        public string Series_No { get; set; }
        public string Part_No { get; set; }
        public string Spare_Part_Machine_ID { get; set; }
        public string Storage_ID { get; set; }
        public string Storage_Description_1 { get; set; }
        public string Storage_Description_2 { get; set; }
        public string Manufacturer_ID { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
    }
}