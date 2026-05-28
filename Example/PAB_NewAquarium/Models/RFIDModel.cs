namespace PAB_NewAquarium.Models
{
    public class RFIDModel
    {
        public string READER_NAME { get; set; }
        public string READER_BRAND { get; set; }
        public string READER_IP { get; set; }
        public uint READER_PORT { get; set; }
        public sbyte READER_RANGE { get; set; }
    }
}