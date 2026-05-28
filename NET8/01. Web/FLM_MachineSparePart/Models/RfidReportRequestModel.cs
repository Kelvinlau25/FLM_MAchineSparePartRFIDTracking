using System;

namespace FILM_Sparepart_MVC.Models
{
    public class RfidReportRequestModel
    {
        public Boolean dateFilter { get; set; }
        public DateTime dateFrom { get; set; }
        public DateTime dateTo { get; set; }
        public String location { get; set; }
        public String reportId { get; set; }
        public String rfid { get; set; }
        private string Status;
        public String status
        {
            get
            {
                return Status;
            }
            set
            {
                switch (value)
                {
                    case "null":
                        Status = null;
                        break;
                    default:
                        Status = value;
                        break;
                }
            }
        }
    }
}