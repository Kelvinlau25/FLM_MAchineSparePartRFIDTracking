using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PAB_NewAquarium.Models
{
    public class MM_PRODUCT_MATERIAL
    {
        public int PRODUCT_MATERIAL_ID { get; set; }
        public string PRODUCT_MATERIAL_DELETE_ID { get; set; }
        [Required(ErrorMessage = "Material Type is required")]
        public string MATERIAL_TYPE { get; set; }
        [Required(ErrorMessage = "Description is required")]
        public string MATERIAL_DESC { get; set; }
        public int RECORD_TYP { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string CREATED_LOC { get; set; }
        public string RETURN_MESSAGE { get; set; }
        public string LISTING_JSON { get; set; }
        public List<MM_PRODUCT_MATERIAL_LISTING> LISTING { get; set; }
    }

    public class MM_PRODUCT_MATERIAL_LISTING
    {
        public string ID { get; set; }
        public string MATERIAL_TYPE { get; set; }
        public string MATERIAL_DESC { get; set; }
    }
}