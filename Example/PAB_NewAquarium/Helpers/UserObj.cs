using PAB_NewAquarium.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PAB_NewAquarium.Helpers
{

    public class ACL_UserObj
    {
        public string JWT_TOKEN { get; set; }
        public string JWT_REFRESH_TOKEN { get; set; }
        public int ID_MM_EMPLOYEE { get; set; }
        public int ID_ACL_USER { get; set; }
        public int ID_ACL_ROLE { get; set; }
        public int ID_ACL_RESOURCE { get; set; }
        public string USER_ID { get; set; }
        public string USR_EMAIL { get; set; }
        public string COMPANY { get; set; }
        public string EMP_NO { get; set; }
        public string EMP_NAME { get; set; }
        public string ROLE_NAME { get; set; }
        public string ROLE_DESC { get; set; }
        public string RESOURCE_NAME { get; set; }
        public string RESOURCE_DESC { get; set; }
        public string TITLE_MODULE { get; set; }
        public List<SideBarContent> SIDEBAR_CONTENT { get; set; }
        public List<AccessList> ACCESS_LIST { get; set; }
    }
}