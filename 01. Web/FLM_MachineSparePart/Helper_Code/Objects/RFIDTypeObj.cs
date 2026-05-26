

namespace FILM_Sparepart_MVC.Helper_Code.Objects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class RFIDTypeObj
    {
        #region Properties

        /// <summary>
        /// Gets or sets User ID property.
        /// </summary>
        public int RFID_TYPE_ID { get; set; }

        /// <summary>
        /// Gets or sets User name property.
        /// </summary>
        public string RFID_TYPE { get; set; }

        #endregion
    }

    public class AdRFIDTypeObj
    {
        public string AD_RFID_TYPE_ID { get; set; }
    }
}