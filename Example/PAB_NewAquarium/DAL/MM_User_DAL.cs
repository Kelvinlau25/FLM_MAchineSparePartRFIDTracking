using PAB_NewAquarium.Helpers;
using PAB_NewAquarium.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PAB_NewAquarium.DAL
{
    public class MM_User_DAL
    {
        readonly CommonFunction common = new CommonFunction();

        public async Task<List<SelectListItem>> PSP_GET_EMPLOYEE_ID()
        {
            List<SelectListItem> ddlList = new List<SelectListItem>();
            List<string> employeeIDList = await common.PSP_COMMON_DAPPER<string>("PSP_AQUA_GET_EMPLOYEE_NO", CommandType.StoredProcedure, null, "PAB_ACL_MVC");

            foreach (string item in employeeIDList)
            {
                SelectListItem selectListItem = new SelectListItem
                {
                    Text = item,
                    Value = item
                };
                ddlList.Add(selectListItem);
            }

            return ddlList;
        }

        public async Task<string> PSP_GET_EMPLOYEE_NAME(string employeeID)
        {
            var obj = new { pEmpNo = employeeID };
            string employeeName = await common.PSP_COMMON_DAPPER_SINGLE<string>("PSP_AQUA_GET_EMPLOYEE_NAME", CommandType.StoredProcedure, obj, "PAB_ACL_MVC");

            return employeeName;
        }
    }
}