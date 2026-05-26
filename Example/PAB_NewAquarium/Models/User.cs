using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PAB_NewAquarium.Models
{
    public class User
    {
        public List<UserList> UserListing { get; set; }
    }

    public class UserList
    {
        public int USER_ID { get; set; }
        public string EMPLOYEE_ID { get; set; }
        public string EMPLOYEE_NAME { get; set; }
        public string VIEW_ID { get; set; }
    }

    public class UserDetail
    {
        public int USER_ID { get; set; }
        [Required(ErrorMessage = "Employee ID is required")]
        public string EMPLOYEE_ID { get; set; }
        public string EMPLOYEE_NAME { get; set; }
        [Required(ErrorMessage = "RFID Tag is required")]
        public string RFID_TAG { get; set; }
        public string RECORD_TYP { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string CREATED_LOC { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
        public string UPDATED_LOC { get; set; }
        public List<SelectListItem> EmployeeIDDdl { get; set; }
        public RFIDModel RFID_READER { get; set; }
    }
}