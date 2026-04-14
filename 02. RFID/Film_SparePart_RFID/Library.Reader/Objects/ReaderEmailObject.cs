using System;

namespace Library.Reader.Objects
{
    public class ReaderEmailObject
    {
        public ReaderEmailObject(string device_name, string ip_address, string location_desc, DateTime time_occured, string message)
        {
            DeviceName = device_name;
            IPAddress = ip_address;
            LocationDesc = location_desc;
            TimeOccured = time_occured;
            Message = message;
        }

        public string Company { get; set; }
        public string DeviceName { get; set; }
        public string IPAddress { get; set; }
        public string LocationDesc { get; set; }
        public DateTime TimeOccured { get; set; }
        public string Message { get; set; }
    }
}
