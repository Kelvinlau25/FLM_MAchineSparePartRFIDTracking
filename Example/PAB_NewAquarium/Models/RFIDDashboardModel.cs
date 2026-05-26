using System.Collections.Generic;

namespace PAB_NewAquarium.Models
{
    public class RFIDDashboardModel
    {
        public string RFID_TAG_JSON { get; set; }
        public List<RFID_TAG> RFID_TAG { get; set; }
        public List<RFID_IMAGE> RFID_IMAGE { get; set; }
        public RFIDModel RFID_READER { get; set; }
    }

    public class RFID_TAG
    {
        public string RFID { get; set; }
    }

    public class RFID_IMAGE
    {
        public int PRODUCT_H_ID { get; set; }
        public string CATEGORY_NAME { get; set; }
        public string CHOP_NO { get; set; }
        public string RFID { get; set; }
        public string IMAGE_URL { get; set; }
    }
}