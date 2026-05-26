using System.Collections.Generic;
using System.Web.Mvc;

namespace PAB_NewAquarium.Models
{
    public class SalesDashboardModel
    {
        public List<CAROUSEL_DASHBOARD> CAROUSEL_DASHBOARD { get; set; }
        public List<FABRIC_HOT_SALES> FABRIC_HOT_SALES { get; set; }
        public List<LATEST_PRODUCTS> LATEST_PRODUCTS { get; set; }       
        public List<CUSTOMER_REGISTRATION> CUSTOMER_REGISTRATION { get; set; }
        public List<SelectListItem> COMPANY_DDL { get; set; }

        public SalesDashboardModel()
        {
            FABRIC_HOT_SALES = new List<FABRIC_HOT_SALES>();
            LATEST_PRODUCTS = new List<LATEST_PRODUCTS>();
            CAROUSEL_DASHBOARD = new List<CAROUSEL_DASHBOARD>();
        }
    }

    public class CAROUSEL_DASHBOARD
    {
        public string IMAGE_URL { get; set; }
    }

    public class FABRIC_HOT_SALES
    {
        public string PRODUCT_H_ID { get; set; }       
        public string CATEGORY_NAME { get; set; }
        public string CHOP_NO { get; set; }       
        public string IMAGE_URL { get; set; }
    }

    public class LATEST_PRODUCTS
    {
        public string PRODUCT_H_ID { get; set; }
        public string CATEGORY_NAME { get; set; }
        public string CHOP_NO { get; set; }
        public string IMAGE_URL { get; set; }
    }

    public class CUSTOMER_REGISTRATION
    {
        public string CUSTOMER_NAME { get; set; }
    }

}