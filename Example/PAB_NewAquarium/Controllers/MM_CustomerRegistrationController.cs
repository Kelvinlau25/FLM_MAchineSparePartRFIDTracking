using Oracle.ManagedDataAccess.Client;
using PAB_NewAquarium.Filters;
using PAB_NewAquarium.Helpers;
using PAB_NewAquarium.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Mvc;


namespace PAB_NewAquarium.Controllers
{
    public class MM_CustomerRegistrationController : BaseController
    {
        [SessionExpire]
        public async Task<ActionResult> MM_CustomerRegistration()
        {
            try
            {
                Customer model = new Customer();
                model.CustomerListing = new List<CustomerList>();
                List<OracleParameter> _pMssql = new List<OracleParameter>();
                OracleParameter resultCursor = new OracleParameter
                {
                    ParameterName = "v_output",
                    Direction = ParameterDirection.Output,
                    OracleDbType = OracleDbType.RefCursor
                };
                _pMssql.Add(resultCursor);

                DataTable dt = await common.PSP_COMMON_ORA("PSP_GET_CUSTOMER_LIST", CommandType.StoredProcedure, _pMssql, null, "PAB_SALES_SQL");
                foreach (DataRow row in dt.Rows)
                {
                    CustomerList cust = new CustomerList();

                    cust.CUST_ID = Convert.ToString(row["CUST_CODE"]);
                    cust.CUST_CODE = Convert.ToString(row["CUST_CODE"]);
                    cust.CUST_NAME = Convert.ToString(row["CUST_NAME"]);
                    cust.COUNTRY = Convert.ToString(row["COUNTRY"]);
                    cust.VIEW = Convert.ToString(row["CUST_CODE"]);

                    model.CustomerListing.Add(cust);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return RedirectToAction("Error", "Home");
            }
        }

        [SessionExpire]
        public async Task<ActionResult> MM_CustomerRegistration_View(string id)
        {
            try
            {
                var obj = new { pCustCode = id };
                ViewBag.CustomerRecords = await common.PSP_COMMON_DAPPER<CustomerVisitList>("PSP_GET_CUSTOMER_VISIT_LIST", CommandType.StoredProcedure, obj);
                return View(await GetCustomerDetailAsync(id));
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<CustomerDetail> GetCustomerDetailAsync(string id)
        {
            try
            {
                CustomerDetail cust = new CustomerDetail();
                List<OracleParameter> _pMssql = new List<OracleParameter>();
                _pMssql.Add(new OracleParameter("v_cust_code", id));
                OracleParameter resultCursor = new OracleParameter
                {
                    ParameterName = "v_output",
                    Direction = ParameterDirection.Output,
                    OracleDbType = OracleDbType.RefCursor
                };
                _pMssql.Add(resultCursor);

                DataTable dt = await common.PSP_COMMON_ORA("PSP_GET_CUSTOMER_DETAIL", CommandType.StoredProcedure, _pMssql, null, "PAB_SALES_SQL");
                foreach (DataRow row in dt.Rows)
                {


                    cust.CUST_CODE = Convert.ToString(row["CUST_CODE"]);
                    cust.CUST_NAME = Convert.ToString(row["CUST_NAME"]);
                    cust.CITY = Convert.ToString(row["CITY"]);
                    cust.COUNTRY = Convert.ToString(row["COUNTRY"]);
                    cust.CUST_ADD1 = Convert.ToString(row["CUST_ADDR1"]);
                    cust.CUST_ADD2 = Convert.ToString(row["CUST_ADDR2"]);
                    cust.CUST_ADD3 = Convert.ToString(row["CUST_ADDR3"]);
                    cust.CUST_ADD4 = Convert.ToString(row["CUST_ADDR4"]);

                    break;
                }
                return cust;
            }
            catch (Exception)
            {
                throw;
            }        }
    }
}