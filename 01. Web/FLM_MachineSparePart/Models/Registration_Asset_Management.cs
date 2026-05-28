using System;

namespace FILM_Sparepart_MVC.Models
{
    public class Registration_Asset_Management
    {
        public int ID { get; set; }
        public string Site { get; set; }
        public string Item_Type { get; set; }
        public string CEL_Number { get; set; }
        public string Status { get; set; }
        public string Owner { get; set; }
        public string Fixed_Asset { get; set; }
        public string Location { get; set; }
        public string RFID { get; set; }
        public string Department { get; set; }
        public string Model { get; set; }
        public DateTime Purchase_Date { get; set; }
        public string Manufacturer { get; set; }
        public DateTime Waranty_expiry { get; set; }
        public string OS_version { get; set; }
        public DateTime EOL_Support { get; set; }
        public string Serial_Number { get; set; }
        public string Description { get; set; }
        public string Asset_Type { get; set; }
        public string Record_Type { get; set; }
        public string Created_By { get; set; }
        public DateTime Created_Date { get; set; }
        public string Created_Loc { get; set; }
        public string Update_By { get; set; }
        public DateTime Update_Date { get; set; }
        public string Update_Loc { get; set; }
        public string IP_Address { get; set; }
        public string MAC_Address { get; set; }
        public string Host_Name { get; set; }
        public string System_Application { get; set; }
        public DateTime Installation_Date { get; set; }
    }
}