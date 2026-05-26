using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PAB_NewAquarium.Models
{
    public class MM_FABRIC_CATEGORY
    {
        public int CATEGORY_H_ID { get; set; }
        public string CATEGORY_H_DELETE_ID { get; set; }
        public string PRODUCT_H_ID { get; set; }
        [Required(ErrorMessage = "Category Name is required")]
        public string CATEGORY_NAME { get; set; }
        [Required(ErrorMessage = "Category Detail is required.")]
        public string CATEGORY_DETAIL { get; set; }
        public int RECORD_TYP { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string CREATED_LOC { get; set; }
        public string RETURN_ID { get; set; }
        public string RETURN_MESSAGE { get; set; }
        public string LISTING_JSON { get; set; }
        public List<MM_FABRIC_CATEGORY_LISTING> LISTING { get; set; }
        public List<MM_FABRIC_CATEGORY_PRODUCT_LISTING> PRODUCT_H { get; set; }
    }

    public class MM_FABRIC_CATEGORY_LISTING
    {
        public string ID { get; set; }
        public string CATEGORY_NAME { get; set; }
        public string CATEGORY_DETAIL { get; set; }
    }

    public class MM_FABRIC_CATEGORY_PRODUCT_LISTING
    {
        public int PRODUCT_H_ID { get; set; }
        public string REFERENCE_NO { get; set; }
        public string CHOP_NO { get; set; }
        public string RUNNING_NO { get; set; }
        public string FABRIC_TYPE { get; set; }
        public string PRODUCT_TYPE { get; set; }
        public string WEAVE_TYPE { get; set; }
        public string WIDTH { get; set; }
        public string FAB_WEIGHT_GSM { get; set; }
        public string HANGER_LOC { get; set; }
    }
}