using System;

namespace Library.Reader.Objects
{
    public class ReaderTagObject
    {
        public ReaderTagObject(string hostName, string location, string antennaID, string tagID, DateTime time_occured)
        {
            Host = hostName;
            Location = location;
            AntennaID = antennaID;
            TagID = tagID;
            TimeOccured = time_occured;
        }

        public string Host { get; set; }
        public string Location { get; set; }
        public string AntennaID { get; set; }
        public string TagID { get; set; }
        public DateTime TimeOccured { get; set; }
    }
}
