using System.Collections.Generic;

namespace PAB_NewAquarium.Models
{
    public class AddToCartModel
    {
        public List<CartModel> CART { get; set; }
        public List<GroupCartModel> GROUP_CART { get; set; }
        public AddToCartModel()
        {
            CART = new List<CartModel>();
            GROUP_CART = new List<GroupCartModel>();
        }
    }

    public class CartModel
    {
        public int CART_H_ID { get; set; }
        public string CUST_CODE { get; set; }
        public string COMPANY_NAME { get; set; }
        public string CATEGORY_NAME { get; set; }
        public string CHOP_NO { get; set; }
        public string COLOR_WAY { get; set; }
        public string REFERENCE_NO { get; set; }
        public int QUANTITY { get; set; }
        public int CURRENT_QTY { get; set; }
        public int CART_D_ID { get; set; }
        public string IMAGE_URL { get; set; }
    }

    public class GroupCartModel
    {
        public string COMPANY_NAME { get; set; }
        public List<CartModel> ITEM { get; set; }
    }

    public class DraftCartModel
    {
        public int CART_H_ID { get; set; }
        public string ATTN_TO { get; set; }
        public string REMARK { get; set; }
        public string CUST_CODE { get; set; }
        public string CHARGE { get; set; }
        public string CHARGE_BY { get; set; }
        public int RECORD_TYP { get; set; }
    }

    public class AddHocRequestModel
    {
        public string SAMPLE_REQUEST_NO { get; set; }      
        public string ATTN_TO { get; set; }
        public string REMARK { get; set; }
        public string CHARGE { get; set; }
        public string CHARGE_BY { get; set; }
        public string COMPLETED { get; set; }
        public string PRIORITY { get; set; }
        public string CUST_CODE { get; set; }
        public string CUST_NAME { get; set; }
        public string PHONE_NO { get; set; }
        public string CUST_ADDR_1 { get; set; }
        public string CUST_ADDR_2 { get; set; }
        public string CUST_ADDR_3 { get; set; }
        public string CUST_ADDR_4 { get; set; }
        public string SENT_BY { get; set; }
        public string DATE_SENT { get; set; }
        public string SALES_REGION {  get; set; }
        public int RECORD_TYP { get; set; }
        public string CREATED_BY { get; set; }
        public string CREATED_LOC { get; set; }
        public List<AddHocRequestDetail> Details { get; set; }

        public AddHocRequestModel()
        {
            Details = new List<AddHocRequestDetail>();
        }
    }

    public class AddHocRequestDetail
    {
        public string PO_NO { get; set; }
        public string CHOP_NO { get; set; }
        public string COLOR_DESC { get; set; }
        public string SAMPLE_ROLL_NO { get; set; }
        public string LOCATION { get; set; }
        public string BALANCE_YARDAGE { get; set; }
        public string UOM { get; set; }
        public string COMPOSITION { get; set; }
        public int QUANTITY_REQUEST { get; set; }
        public int QUANTITY_SENT { get; set; }
    }
}