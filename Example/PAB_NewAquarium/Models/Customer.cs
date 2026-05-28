using System.Collections.Generic;

namespace PAB_NewAquarium.Models
{
    public class Customer
    {
        public List<CustomerList> CustomerListing { get; set; }
    }

    public class CustomerList
    {
        public string CUST_ID { get; set; }
        public string CUST_CODE { get; set; }
        public string CUST_NAME { get; set; }
        public string COUNTRY { get; set; }
        public string VIEW { get; set; }
    }

    public class CustomerDetail
    {
        public string CUST_CODE { get; set; }
        public string CUST_NAME { get; set; }
        public string CITY { get; set; }
        public string COUNTRY { get; set; }
        public string CUST_ADD1 { get; set; }
        public string CUST_ADD2 { get; set; }
        public string CUST_ADD3 { get; set; }
        public string CUST_ADD4 { get; set; }
        public string STATUS { get; set; }
        
        
    }

    public class CustomerVisitList
    {
        public string CUSTOMER_NAME { get; set; }
        public string SALES_PIC { get; set; }
        public string UPDATED_DATE { get; set; }
    }
}