

namespace FILM_Sparepart_MVC.Helper_Code.Objects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    public class MachineObj
    {
        #region Properties

        /// <summary>
        /// Gets or sets User ID property.
        /// </summary>
        public int MACHINE_ID { get; set; }

        /// <summary>
        /// Gets or sets User name property.
        /// </summary>
        public string MACHINE_MODEL { get; set; }

        #endregion
    }

    public class AdMachineObj
    {
        public string AD_MACHINE_ID { get; set; }
    }
}