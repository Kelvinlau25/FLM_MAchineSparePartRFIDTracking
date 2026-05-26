using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace PAB_NewAquarium.Models
{
    public class MM_STOCK_INVENTORY
    {
        public string REFERENCE_NO { get; set; }
        public string CHOP_NO { get; set; }
        public string COLOR_WAY { get; set; }
        public string STORE_LOC { get; set; }
        public string RFID_STOCK_IN { get; set; }
        public string RFID_STOCK_OUT { get; set; }
        public string STOCK_TYPE { get; set; }
        public string RETURN_MESSAGE { get; set; }
        public int RECORD_TYP { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string CREATED_LOC { get; set; }
        public List<SelectListItem> COLOR_WAY_DDL { get; set; }
        public List<string> STOCK_IN_REFERENCE_NO { get; set; }
        public List<string> STOCK_IN_CHOP_NO { get; set; }
        public List<string> STOCK_IN_COLOR_WAY { get; set; }
        public List<string> STOCK_IN_STORE_LOC { get; set; }
        public List<string> STOCK_IN_RFID { get; set; }
        public List<string> STOCK_OUT_ID { get; set; }

        public List<MM_STOCK_IN_LISTING> STOCK_IN_LISTING { get; set; }
        public List<MM_STOCK_OUT_LISTING> STOCK_OUT_LISTING { get; set; }
    }

    public class MM_STOCK_IN_LISTING
    {
        public string PRODUCT_I_ID { get; set; }
        public string REFERENCE_NO { get; set; }
        public string CHOP_NO { get; set; }
        public string RFID { get; set; }
        public string COLOR_WAY { get; set; }
        public string STORE_LOC { get; set; }
        public string STORE_DATE { get; set; }
        public string STORE_STATUS { get; set; }
        public string SALES_PIC { get; set; }
    }
    public class MM_STOCK_OUT_LISTING
    {
        public string PRODUCT_I_ID { get; set; }
        public string REFERENCE_NO { get; set; }
        public string CHOP_NO { get; set; }
        public string RFID { get; set; }
        public string COLOR_WAY { get; set; }
        public string STORE_LOC { get; set; }
        public string STORE_DATE { get; set; }
        public string STORE_STATUS { get; set; }
        public string SALES_PIC { get; set; }
        public string CUSTOMER_NAME { get; set; }
    }

}