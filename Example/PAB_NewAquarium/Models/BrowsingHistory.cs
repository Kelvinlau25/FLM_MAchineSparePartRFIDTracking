using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PAB_NewAquarium.Models
{
    public class BrowsingHistoryList
    {
        public List<SelectListItem> BrowsingHistoryDdl { get; set; }
        public List<SelectListItem> CompanyDdl { get; set; }
        public List<SelectListItem> FabricCategoryDdl { get; set; }
        public List<SelectListItem> ProductTypeDdl { get; set; }
        public List<SelectListItem> SortBy { get; set; }
    }

    public class BrowsingHistoryViewModel
    {
        public List<BrowsingHistory> BrowsingHistory { get; set; }
        public List<BrowsingHistoryCustomer> BrowsingHistoryCustomer { get; set; }
    }

    public class BrowsingHistory
    {
        public string PRODUCT_H_ID { get; set; }
        public string VIEWS { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public string COMPANY_NAME { get; set; }
        public string CATEGORY { get; set; }
        public string CHOP_NO { get; set; }
        public string TRANSACTION_DATE { get; set; }
        public string PRODUCT_IMAGE { get; set; }
    }

    public class BrowsingHistoryCustomer
    {
        public string PRODUCT_H_ID { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public string COMPANY_NAME { get; set; }
        public string TRANSACTION_DATE { get; set; }
    }

}