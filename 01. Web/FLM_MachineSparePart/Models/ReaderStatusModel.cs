using System;

namespace FILM_Sparepart_MVC.Models
{
    public class ReaderStatusModel
    {
        public string IpAddress { get; set; }
        public string HostName { get; set; }
        public string Location { get; set; }
        public bool IsConnected { get; set; }
    }
}
