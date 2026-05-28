

namespace FILM_Sparepart_MVC.Helper_Code.Objects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    public class ManufacturerObj
    {
        #region Properties

        /// <summary>
        /// Gets or sets User ID property.
        /// </summary>
        public int MANUFACTURER_ID { get; set; }

        /// <summary>
        /// Gets or sets User name property.
        /// </summary>
        public string MANUFACTURER_CODE { get; set; }

        #endregion
    }

    public class AdManufacturerObj
    {
        public string AD_MANUFACTURER_ID { get; set; }
    }
}