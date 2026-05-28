using System;

namespace FILM_Sparepart_MVC.Models
{
    public class RfidReportTable
    {


        public string partNo { get; set; }
        public string seriesNo { get; set; }
        public string modelNo { get; set; }
        public string model { get; set; }
        public string actualLocation { get; set; }
        private string StorageCode;
        public string storageCode
        {
            get
            {
                return StorageCode;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    StorageCode = rfidAuditStorageCode;
                }
                else
                {
                    StorageCode = value;
                }

            }
        }
        public string rfidAuditStorageCode { get; set; }


        public string manufacturer { get; set; }

        private string STATUS;
        public string status
        {
            get
            {
                return STATUS;
            }
            set
            {
                switch (value)
                {
                    case null:
                        STATUS = "No";
                        break;
                    case "Y":
                        STATUS = "Yes";
                        break;
                    case "N":
                        STATUS = "No";
                        break;
                    default:
                        STATUS = "UNKNOWN";
                        break;
                }
            }
        }
        private string RfidId;
        public string RFID_TAG_CODE
        {
            get
            {
                return RfidId;
            }
            set
            {
                switch (value)
                {
                    case null:
                        break;
                    default:
                        RfidId = value;
                        break;

                }
            }
        }
        public string rfidId
        {
            get
            {
                return RfidId;
            }
            set
            {
                switch (value)
                {
                    case null:
                        break;
                    default:
                        RfidId = value;
                        break;

                }
            }

        }
        public string remarks { get; set; }
        private string CreatedDate;
        public string createdDate
        {
            get
            {
                return CreatedDate;
            }
            set
            {
                CreatedDate = Convert.ToDateTime(value).ToString("dd/MM/yyyy");
            }
        }
        public string createdBy { get; set; }

        public Nullable<int> RFID_AUDIT_ID { get; set; }


    }
}