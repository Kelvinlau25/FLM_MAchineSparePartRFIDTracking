

namespace FILM_Sparepart_MVC.Helper_Code.Objects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    public class ModelObj
    {
        #region Properties

        /// <summary>
        /// Gets or sets User ID property.
        /// </summary>
        public int MODEL_ID { get; set; }

        /// <summary>
        /// Gets or sets User name property.
        /// </summary>
        public string MODEL_NAME { get; set; }

        #endregion
    }

    public class AdModelObj
    {
        public string AD_Model_ID { get; set; }
    }

}