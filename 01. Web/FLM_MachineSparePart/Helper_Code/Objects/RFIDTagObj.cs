

namespace FILM_Sparepart_MVC.Helper_Code.Objects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    public class RFIDTagObj
    {
        #region Properties

        /// <summary>
        /// Gets or sets User ID property.
        /// </summary>
        public int RFID_TAG_ID { get; set; }

        /// <summary>
        /// Gets or sets User name property.
        /// </summary>
        public string RFID_TAG { get; set; }

        #endregion
    }
    public class AdRFIDTagObj
    {
        public string AD_RFID_TAG_ID { get; set; }
    }
}