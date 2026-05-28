using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FILM_Sparepart_MVC.Models
{

    public class InquiryTable
    {
        public string SPARE_PART_MACHINE_ID { get; set; }
        public string SPARE_PART_MODEL_ID { get; set; }
        public string MODEL_NO { get; set; }
        public string SERIES_NO { get; set; }
        public string PART_NO { get; set; }
        public string MACHINE_MODEL { get; set; }
        public string STORAGE_CODE { get; set; }
        public string STORAGE_ID { get; set; }
        public string MANUFACTURER_ID { get; set; }
        public string MANUFACTURER_CODE { get; set; }
        private string status;
        public string STATUS
        {
            get
            {
                return status;
            }
            set
            {
                status = changeStatus(value);
            }
        }
        public string REMARKS { get; set; }
        public int TRANSACTION_ID { get; set; }
        private string changeStatus(string value)
        {
            switch (value)
            {
                case "DONE":
                    return "OFF";
                case "NEW":
                    return "ON";
                default:
                    return value;

            }
        }
    }
}