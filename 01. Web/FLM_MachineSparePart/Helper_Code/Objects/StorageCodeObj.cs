

namespace FILM_Sparepart_MVC.Helper_Code.Objects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    public class StorageCodeObj
    {
        #region Properties

        /// <summary>
        /// Gets or sets User ID property.
        /// </summary>
        public int READER_ID { get; set; }

        /// <summary>
        /// Gets or sets User name property.
        /// </summary>
        public string STORAGE_CODE { get; set; }

        #endregion
    }
    public class AdStorageCodeObj
    {
        public string AD_READER_ID { get; set; }
    }
}