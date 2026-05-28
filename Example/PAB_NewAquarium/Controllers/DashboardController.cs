using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using PAB_NewAquarium.Filters;
using PAB_NewAquarium.Helpers;
using PAB_NewAquarium.Models;

namespace PAB_NewAquarium.Controllers
{
    public class DashboardController : BaseController
    {
        readonly BlobHelper blobHelper = new BlobHelper();

        [SessionExpire]
        public async Task<ActionResult> Index()
        {
            try
            {
                SalesDashboardModel model = new SalesDashboardModel();
                var param = new { pUser = string.Empty };
                var (objModelA, objModelB, objModelC) = await common.PSP_COMMON_DAPPER_MULTIPLE<FABRIC_HOT_SALES, LATEST_PRODUCTS, CAROUSEL_DASHBOARD>("PSP_GET_IMAGE_SALES_DASHBOARD", CommandType.StoredProcedure, param);
                model.FABRIC_HOT_SALES = objModelA;
                model.LATEST_PRODUCTS = objModelB;
                model.CAROUSEL_DASHBOARD = objModelC;

                // Loop each image and set secure token
                foreach (var item in model.FABRIC_HOT_SALES)
                {
                    item.IMAGE_URL = await blobHelper.SetBlobUrlToken(item.IMAGE_URL);
                }
                foreach (var item in model.LATEST_PRODUCTS)
                {
                    item.IMAGE_URL = await blobHelper.SetBlobUrlToken(item.IMAGE_URL);
                }

                return View(model);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return RedirectToAction("Error", "Home");
            }
        }

        #region Ajax Function
        [HttpPost]
        public async Task<ActionResult> GetAddToCart(string customerID = "")
        {
            try
            {
                var obj = new { pID = customerID == "" ? AclUser.EMP_NO : customerID, pLogin = customerID == "" ? "Sales" : "Customer" };
                int count = await common.PSP_COMMON_DAPPER_SINGLE<int>("PSP_GET_AJAX_ADD_TO_CART", CommandType.StoredProcedure, obj);
                return Json(count, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> GetCustomerList(string companyCode)
        {
            try
            {
                var obj = new { pCompanyCode = companyCode };
                List<string> stringList = await common.PSP_COMMON_DAPPER<string>("PSP_GET_AJAX_CUSTOMER_NAME", CommandType.StoredProcedure, obj);
                string jsonData = JsonConvert.SerializeObject(stringList);
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return Json(string.Empty, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> InsertCustomerName(string companyCode, string companyName, string customerName)
        {
            try
            {
                ACL_UserObj aclObj = (Session["AclUser"] as ACL_UserObj);
                List<SqlParameter> _pMssql = new List<SqlParameter>();
                _pMssql.Add(new SqlParameter("@pCompanyCode", companyCode));
                _pMssql.Add(new SqlParameter("@pCompanyName", companyName));
                _pMssql.Add(new SqlParameter("@pCustomerName", customerName.ToUpper()));
                _pMssql.Add(new SqlParameter("@pCreatedBy", aclObj.EMP_NO + " - " + aclObj.EMP_NAME));
                _pMssql.Add(new SqlParameter("@pCreatedDate", DateTime.Now));
                _pMssql.Add(new SqlParameter("@pCreatedLoc", Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString()));
                _pMssql.Add(new SqlParameter("returnResult", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });

                string customerID = await common.PSP_COMMON_SQL("PSP_USER_REGISTRATION", CommandType.StoredProcedure, _pMssql, "", "", true);
                return Json(customerID, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return Json(string.Empty, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<ActionResult> GetOverallSearchData()
        {
            try
            {
                List<string> stringList = await common.PSP_COMMON_DAPPER<string>("PSP_GET_AJAX_OVERALL_SEARCH_DATA", CommandType.StoredProcedure);
                string jsonData = JsonConvert.SerializeObject(stringList);
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return Json(string.Empty, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<ActionResult> GetCompanyList()
        {
            try
            {
                List<SelectListItem> selectList = new List<SelectListItem>();
                List<OracleParameter> _pMssql = new List<OracleParameter>();
                OracleParameter resultCursor = new OracleParameter
                {
                    ParameterName = "v_output",
                    Direction = ParameterDirection.Output,
                    OracleDbType = OracleDbType.RefCursor
                };
                _pMssql.Add(resultCursor);

                DataTable dt = await common.PSP_COMMON_ORA("PSP_GET_CUSTOMER_LIST", CommandType.StoredProcedure, _pMssql, null, "PAB_SALES_SQL");

                foreach (DataRow x in dt.Rows)
                {
                    SelectListItem item = new SelectListItem
                    {
                        Text = (string)x["CUST_NAME"],
                        Value = (string)x["CUST_CODE"]
                    };
                    selectList.Add(item);
                }

                string jsonData = JsonConvert.SerializeObject(selectList);
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return Json(string.Empty, JsonRequestBehavior.AllowGet);
            }
        }      
        #endregion
    }
}