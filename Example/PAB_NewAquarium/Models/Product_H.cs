using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web.Mvc;

namespace PAB_NewAquarium.Models
{
    public class Product_H
    {
        public List<Product_H_List> Product_H_Listing { get; set; }
        public List<SelectListItem> STATUS_DDL { get; set; }
        public List<SelectListItem> FABRIC_TYPE_DDL { get; set; }
        public List<SelectListItem> FINISHED_TYPE_DDL { get; set; }
        public List<string> ACTION_LIST { get; set; }

        public Product_H()
        {
            ACTION_LIST = new List<string>();
        }
    }

    public class Product_H_Array
    {
        public List<Product_H_Detail_Category> Product_H_Detail_Category { get; set; }
    }

    public class Product_H_List
    {
        public int PRODUCT_H_ID { get; set; }
        public string MATERIAL_TYPE { get; set; }
        public string CATEGORY_NAME { get; set; }
        public string REFERENCE_NO { get; set; }
        public string CHOP_NO { get; set; }
        public string RUNNING_NO { get; set; }
        public string FABRIC_TYPE { get; set; }
        public string PRODUCT_TYPE { get; set; }
        public string WEAVE_TYPE { get; set; }
        public string WIDTH { get; set; }
        public string FAB_WEIGHT_GSM { get; set; }
        public string HANGER_LOC { get; set; }
        public string ACTIVESTATUS { get; set; }
        public string STATUS { get; set; }
        public string ASSIGNEE_COL { get; set; }
        public string WARP_COUNT { get; set; }
        public string WEFT_COUNT { get; set; }
        public string COMPOSITION { get; set; }
        public string WARP_DENSITY { get; set; }
        public string WEFT_DENSITY { get; set; }
        public string FIN_WARP_DENSITY { get; set; }
        public string FIN_WEFT_DENSITY { get; set; }
        public string FINISH_WIDTH1 { get; set; }
        public string FINISH_WIDTH2 { get; set; }
        public string FINISH_TYPE { get; set; }
    }

    public class Product_H_Detail
    {
        public int PRODUCT_H_ID { get; set; }
        public string REFERENCE_NO { get; set; }
        public string CHOP_NO { get; set; }
        public bool RUNNING_NO { get; set; }
        public string FABRIC_TYPE { get; set; }
        public string FINISH_TYPE { get; set; }
        public string PRODUCT_TYPE1 { get; set; }
        public string PRODUCT_TYPE2 { get; set; }
        public string PRODUCT_TYPE3 { get; set; }
        public string PRODUCT_TYPE4 { get; set; }
        public string PRODUCT_TYPE5 { get; set; }
        public string PRODUCT_TYPE6 { get; set; }
        public string MATERIAL_TYPE1 { get; set; }
        public string MATERIAL_TYPE2 { get; set; }
        public string MATERIAL_TYPE3 { get; set; }
        public string MATERIAL_TYPE4 { get; set; }
        public string MATERIAL_TYPE5 { get; set; }
        public string MATERIAL_TYPE6 { get; set; }
        public string CATEGORY { get; set; }
        public string WEAVE_TYPE { get; set; }
        public string FINISH_TYPE1 { get; set; }
        public string FINISH_TYPE2 { get; set; }
        public string FINISH_TYPE3 { get; set; }
        public string FINISH_TYPE4 { get; set; }
        public string FINISH_TYPE5 { get; set; }
        public string FINISH_TYPE6 { get; set; }
        public int WARP_DENSITY { get; set; }
        public int WEFT_DENSITY { get; set; }
        public int FIN_WARP_DENSITY { get; set; }
        public int FIN_WEFT_DENSITY { get; set; }
        public string WARP_COUNT1 { get; set; }
        public string WARP_COUNT2 { get; set; }
        public string WARP_COUNT3 { get; set; }
        public string WEFT_COUNT1 { get; set; }
        public string WEFT_COUNT2 { get; set; }
        public string WEFT_COUNT3 { get; set; }
        public string COMPOSITION { get; set; }
        public float FAB_WEIGHT_GSM { get; set; }
        public float WIDTH { get; set; }
        public float FINISH_WIDTH1 { get; set; }
        public float FINISH_WIDTH2 { get; set; }
        public string END_USAGE { get; set; }
        public bool ISGARMENT { get; set; }
        public string HANGER_LOC { get; set; }
        public string CURR_LOC { get; set; }
        //[Required(ErrorMessage = "RFID Tag is required")]
        public string RFID { get; set; }
        public string RFID_STATUS { get; set; }
        public string UOM { get; set; }
        public string RECORD_TYP { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string CREATED_LOC { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
        public string UPDATED_LOC { get; set; }
        public List<SelectListItem> PRODUCT_TYPE_DDL { get; set; }
        public List<SelectListItem> MATERIAL_TYPE_DDL { get; set; }
        public List<SelectListItem> CATEGORY_DDL { get; set; }
        public List<SelectListItem> REFERENCE_NO_DDL { get; set; }
        public List<SelectListItem> HANGER_LOC_DDL { get; set; }
        public List<SelectListItem> COLOR_WAY_DDL { get; set; }
        public List<SelectListItem> VIDEO_CAT_DDL { get; set; }
        public List<string> PRODUCT_TYPE_SELECTED { get; set; }
        public List<string> MATERIAL_TYPE_SELECTED { get; set; }
        public List<string> CATEGORY_SELECTED { get; set; }
        public List<int> VIDEO_CAT_SELECTED { get; set; }
        public string STATUS { get; set; }
        public string COLOR { get; set; }
        public string imageList { get; set; }
        public string ITEM_CODE { get; set; }
        public string VIDEO_CAT_NAME { get; set; }
        public RFIDModel RFID_READER { get; set; }
        public List<FabricVideo> FabricVideoList { get; set; }
        public Product_H_Detail()
        {
            PRODUCT_TYPE_DDL = new List<SelectListItem>();
            MATERIAL_TYPE_DDL = new List<SelectListItem>();
            CATEGORY_DDL = new List<SelectListItem>();
            REFERENCE_NO_DDL = new List<SelectListItem>();
            HANGER_LOC_DDL = new List<SelectListItem>();
            COLOR_WAY_DDL = new List<SelectListItem>();
            VIDEO_CAT_DDL = new List<SelectListItem>();
            PRODUCT_TYPE_SELECTED = new List<string>();
            MATERIAL_TYPE_SELECTED = new List<string>();
            CATEGORY_SELECTED = new List<string>();
            VIDEO_CAT_SELECTED = new List<int>();
            FabricVideoList = new List<FabricVideo>();
        }
    }

    public class Product_H_Detail_Category
    {
        public int PRODUCT_H_ID { get; set; }
        public string REFERENCE_NO { get; set; }
        public string CATEGORY { get; set; }
        public List<SelectListItem> CATEGORY_DDL { get; set; }
        public List<string> CATEGORY_SELECTED { get; set; }
    }

    public class ProductImageList
    {
        public string COLOR_WAY { get; set; }
        public string CURRENT_QTY { get; set; }
        public List<string> imageName { get; set; }
        public List<string> imageFile { get; set; }
        public List<int> imageID { get; set; }
    }

    public class ProductImage
    {
        public string COLOR_WAY { get; set; }
        public string CURRENT_QTY { get; set; }
        public int IMAGE_ID { get; set; }
        public string IMAGE_URL { get; set; }
    }

    public class ProductImageHiRes
    {
        public string COLOR_WAY { get; set; }
        public string CURRENT_QTY { get; set; }
        public int IMAGE_ID { get; set; }
        public string IMAGE_URL { get; set; }
    }

    public class ProductComment
    {
        public string CUSTOMER_NAME { get; set; }
        public string COLOR_WAY { get; set; }
        public string FEEDBACK_DESC { get; set; }
        public string KEYWORD_STR { get; set; }
        public DateTime CREATED_DATE { get; set; }
    }

    public class ProductCommentList
    {
        public List<ProductComment> ProductListComment { get; set; }
    }

    public class ProductDetailPageModel
    {
        public int GROUP_D_ID { get; set; }
        public Product_H_Detail Product_H_Detail { get; set; }
        public List<ProductComment> ProductCommentList { get; set; }
        public List<SimilarProduct> SimilarProductList { get; set; }
        public List<RecentView> RecentViewList { get; set; }
        public List<FabricVideo> FabricVideoList { get; set; }
        public string SIMILAR_PRODUCT_JSON { get; set; }
        public string RECENT_VIEW_PRODUCT_JSON { get; set; }
    }

    public class FabricVideo
    {
        private string _VIDEO_NAME;
        public string VIDEO_NAME
        {
            get => _VIDEO_NAME;
            set
            {
                _VIDEO_NAME = value;
                VIDEO_URL = ConfigurationManager.AppSettings["PAB_QR_API"] + value.Replace(" ", "%20");
            }
        }

        public string VIDEO_URL { get; set; }
    }

    public class SampleRoll
    {
        public int CART_D_ID { get; set; }
        public string PO_NO { get; set; }
        public string COLOR_WAY { get; set; }
        public string SAMPLE_ROLL_NO { get; set; }
        public string LOCATION { get; set; }
        public int BALANCE_YARDAGE { get; set; }
        public string UOM { get; set; }
        public int REQUEST_QUANTITY { get; set; }
        public string CHOP_NO { get; set; }
        public string COMPOSITION { get; set; }
        public string FINISH_TYPE1 { get; set; }
        public string FINISH_TYPE2 { get; set; }
        public string FINISH_TYPE3 { get; set; }
        public string FINISH_TYPE4 { get; set; }
        public string WARP_DENSITY { get; set; }
        public string WARP_DENSITY2 { get; set; }
        public string WEFT_DENSITY { get; set; }
        public string WEFT_DENSITY2 { get; set; }
        public string WARP_COUNT { get; set; }
        public string WARP_COUNT2 { get; set; }
        public string WARP_COUNT3 { get; set; }
        public string WEFT_COUNT { get; set; }
        public string WEFT_COUNT2 { get; set; }
        public string WEFT_COUNT3 { get; set; }
    }

    public class CartDUpdate
    {
        public int CART_D_ID { get; set; }
        public int QUANTITY { get; set; }
    }

    public class AdHocRequestDetails
    {
        public string PO_NO { get; set; }
        public string CHOP_NO { get; set; }
        public string COMPOSITION { get; set; }
        public string COLOR_WAY { get; set; }
        public string FINISH_TYPE1 { get; set; }
        public string FINISH_TYPE2 { get; set; }
        public string FINISH_TYPE3 { get; set; }
        public string FINISH_TYPE4 { get; set; }
        public string WARP_DENSITY { get; set; }
        public string WARP_DENSITY2 { get; set; }
        public string WEFT_DENSITY { get; set; }
        public string WEFT_DENSITY2 { get; set; }
        public string WARP_COUNT { get; set; }
        public string WARP_COUNT2 { get; set; }
        public string WARP_COUNT3 { get; set; }
        public string WEFT_COUNT { get; set; }
        public string WEFT_COUNT2 { get; set; }
        public string WEFT_COUNT3 { get; set; }
        public string SAMPLE_ROLL_NO { get; set; }
        public int BALANCE_YARDAGE { get; set; }
        public string LOCATION { get; set; }
        public int REQUEST_QUANTITY { get; set; }
        public string UOM { get; set; }
        public string SENT_BY { get; set; }
        public string DATE_SENT { get; set; }
    }

    public class SampleAdHocMaster
    {
        public int CART_H_ID { get; set; }
        public string CUST_CODE { get; set; }
        public string TEL_NO { get; set; }
        public string ADDR1 { get; set; }
        public string ADDR2 { get; set; }
        public string ATTN_TO { get; set; }
        public string REMARK { get; set; }
        public string SENT_BY { get; set; }
        public string DATE_SENT { get; set; }
        public string COURIER_NO { get; set; }
        public string CHARGE { get; set; }
        public string CHARGE_BY { get; set; }
        public int PRIORITY { get; set; }
        public int QTY_SENT { get; set; }
    }

    public class SampleAdHocViewModel
    {
        public SampleAdHocMaster SampleAdHocMaster { get; set; }
        public List<SampleRoll> sampleRolls { get; set; }

    }

    public class SimilarProduct
    {
        public string PRODUCT_H_ID { get; set; }
        public string IMAGE_URL { get; set; }
        public string CATEGORY_NAME { get; set; }
        public string CHOP_NO { get; set; }
    }

    public class RecentView
    {
        public string PRODUCT_H_ID { get; set; }
        public string IMAGE_URL { get; set; }
        public string CATEGORY_NAME { get; set; }
        public string CHOP_NO { get; set; }
    }
}