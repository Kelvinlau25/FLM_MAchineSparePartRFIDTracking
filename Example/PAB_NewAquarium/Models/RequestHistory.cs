using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PAB_NewAquarium.Models
{
    public class RequestHistoryList
    {
        public List<SelectListItem> RequestHistoryDdl { get; set; }
        public List<SelectListItem> CompanyDdl { get; set; }
        public List<SelectListItem> FabricCategoryDdl { get; set; }
        public List<SelectListItem> ProductTypeDdl { get; set; }
        public List<SelectListItem> SortBy { get; set; }
        public List<SelectListItem> SampleRequestNo { get; set; }
    }

    public class RequestHistoryViewModel
    {
        public List<RequestHistory> RequestHistory { get; set; }
    }

    public class RequestHistory
    {
        public string PRODUCT_H_ID { get; set; }
        public string COMPANY_NAME { get; set; }
        public string CATEGORY { get; set; }
        public string CHOP_NO { get; set; }
        public string CREATED_DATE { get; set; }
        public string PRODUCT_IMAGE { get; set; }
        public string COLOR_WAY { get; set; }
        public string REFERENCE_NO { get; set; }
        public string QUANTITY_REQ { get; set; }
        public string SAMPLE_REQ_NO { get; set; }
        public string REMARK { get; set; }

    }

}