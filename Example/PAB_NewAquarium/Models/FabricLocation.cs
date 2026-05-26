using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PAB_NewAquarium.Models
{
    public class FabricLocation
    {
        public List<FabricLocationList> FabricLocationListing { get; set; }
    }

    public class FabricLocationList
    {
        public string ID { get; set; }
        public string LOC_CODE { get; set; }
        public string LOC_DESC { get; set; }
        public string LOC_TYPE { get; set; }
        public string RFID_TAG { get; set; }
        public string VIEW_ID { get; set; }
    }

    public class FabricLocationDetail
    {
        public string LOC_CODE_ID { get; set; }
        [Required(ErrorMessage = "Location Code is required")]
        public string LOC_CODE { get; set; }
        [Required(ErrorMessage = "Description is required")]
        public string LOC_DESC { get; set; }
        [Required(ErrorMessage = "Location Type is required")]
        public string LOC_TYPE { get; set; }
        public string RFID_TAG { get; set; }
        public string RECORD_TYP { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string CREATED_LOC { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
        public string UPDATED_LOC { get; set; }
        public List<SelectListItem> LocationTypeDdl { get; set; }
    }
}