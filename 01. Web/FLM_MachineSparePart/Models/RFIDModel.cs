using System;

namespace FILM_Sparepart_MVC.Models
{
    public class RFIDModel
    {
        public string READER_NAME { get; set; }
        public string READER_IP { get; set; }
        public uint READER_PORT { get; set; }
        public string STORED_PROCEDURE { get; set; }
        public string SERVER { get; set; }
        public string STORED_PROCEDURE2 { get; set; }
        public string SERVER2 { get; set; }
        public string LOCATION { get; set; }
        public bool IsConnected { get; set; }
        public string Status { get; set; }
    }
}
