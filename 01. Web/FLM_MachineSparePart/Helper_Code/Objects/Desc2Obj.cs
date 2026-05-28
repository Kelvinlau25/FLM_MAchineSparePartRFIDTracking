

namespace ACL_System.Helper_Code.Objects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    public class Desc2Obj
    {
        #region Properties

        /// <summary>
        /// Gets or sets User ID property.
        /// </summary>
        public int STORAGE_ID { get; set; }

        /// <summary>
        /// Gets or sets User name property.
        /// </summary>
        public string STORAGE_DESC_1 { get; set; }
        public string STORAGE_DESC_2 { get; set; }

        #endregion
    }

    public class AdDesc2Obj
    {
        public string AD_STORAGE_ID { get; set; }
    }
}