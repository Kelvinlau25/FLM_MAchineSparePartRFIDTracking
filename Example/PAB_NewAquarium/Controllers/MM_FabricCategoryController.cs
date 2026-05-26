using Newtonsoft.Json;
using PAB_NewAquarium.DAL;
using PAB_NewAquarium.Filters;
using PAB_NewAquarium.Helpers;
using PAB_NewAquarium.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PAB_NewAquarium.Controllers
{
    public class MM_FabricCategoryController : BaseController
    {
        readonly MM_FABRIC_CATEGORY_DAL dal = new MM_FABRIC_CATEGORY_DAL();

        [SessionExpire]
        public async Task<ActionResult> MM_FabricCategory_List()
        {
            try
            {
                MM_FABRIC_CATEGORY model = new MM_FABRIC_CATEGORY();
                var obj = new { pID = 0 };
                model.LISTING = await common.PSP_COMMON_DAPPER<MM_FABRIC_CATEGORY_LISTING>("PSP_GET_FABRIC_CATEGORY", CommandType.StoredProcedure, obj);
                model.LISTING_JSON = JsonConvert.SerializeObject(model.LISTING);
                return View(model);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return RedirectToAction("Error", "Home");
            }
        }

        [SessionExpire]
        public async Task<ActionResult> MM_FabricCategory_View(int id)
        {
            try
            {
                var obj = new { pID = id.ToString() };
                MM_FABRIC_CATEGORY model = await common.PSP_COMMON_DAPPER_SINGLE<MM_FABRIC_CATEGORY>("PSP_GET_FABRIC_CATEGORY", CommandType.StoredProcedure, obj);
                obj = new { pID = model.PRODUCT_H_ID };
                model.PRODUCT_H = await common.PSP_COMMON_DAPPER<MM_FABRIC_CATEGORY_PRODUCT_LISTING>("PSP_GET_PRODUCT_H_LIST", CommandType.StoredProcedure, obj);

                return View(model);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return RedirectToAction("Error", "Home");
            }
        }

        [SessionExpire]
        public async Task<ActionResult> MM_FabricCategory_Create()
        {
            try
            {
                MM_FABRIC_CATEGORY model = new MM_FABRIC_CATEGORY();
                model.PRODUCT_H = await common.PSP_COMMON_DAPPER<MM_FABRIC_CATEGORY_PRODUCT_LISTING>("PSP_GET_PRODUCT_H_LIST", CommandType.StoredProcedure);
                model.LISTING_JSON = JsonConvert.SerializeObject(model.PRODUCT_H);
                return View(model);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return RedirectToAction("Error", "Home");
            }
        }

        [SessionExpire]
        [HttpPost]
        public async Task<ActionResult> MM_FabricCategory_Create(MM_FABRIC_CATEGORY model)
        {
            try
            {
                ACL_UserObj aclObj = (Session["AclUser"] as ACL_UserObj);
                model.CATEGORY_H_ID = 0;
                model.CATEGORY_H_DELETE_ID = "";
                model.RECORD_TYP = 1;
                model.CREATED_BY = username;
                model.CREATED_DATE = DateTime.Now;
                model.CREATED_LOC = Request.UserHostAddress;
                model = await dal.PSP_FABRIC_CATEGORY_MAINT(model);
                ViewBag.ReturnMessage = (model.RETURN_MESSAGE == "Saved") ? "Success" : "Fail";
                return View(model);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return RedirectToAction("Error", "Home");
            }
        }

        [SessionExpire]
        public async Task<ActionResult> MM_FabricCategory_Edit(int id)
        {
            try
            {
                var obj = new { pID = id };
                MM_FABRIC_CATEGORY model = await common.PSP_COMMON_DAPPER_SINGLE<MM_FABRIC_CATEGORY>("PSP_GET_FABRIC_CATEGORY", CommandType.StoredProcedure, obj);
                model.PRODUCT_H = await common.PSP_COMMON_DAPPER<MM_FABRIC_CATEGORY_PRODUCT_LISTING>("PSP_GET_PRODUCT_H_LIST", CommandType.StoredProcedure);

                #region Filter data
                List<int> productHIds = model.PRODUCT_H_ID.Split(',').Select(int.Parse).ToList();
                Dictionary<int, MM_FABRIC_CATEGORY_PRODUCT_LISTING> productDict = model.PRODUCT_H.ToDictionary(item => item.PRODUCT_H_ID);
                List<MM_FABRIC_CATEGORY_PRODUCT_LISTING> matchingItems = new List<MM_FABRIC_CATEGORY_PRODUCT_LISTING>();
                List<MM_FABRIC_CATEGORY_PRODUCT_LISTING> nonMatchingItems = new List<MM_FABRIC_CATEGORY_PRODUCT_LISTING>();

                foreach (int item in productHIds)
                {
                    if (productDict.ContainsKey(item))
                    {
                        matchingItems.Add(productDict[item]);
                    }
                }

                foreach (var item in model.PRODUCT_H)
                {
                    if (!productHIds.Contains(item.PRODUCT_H_ID))
                    {
                        nonMatchingItems.Add(item);
                    }
                }

                model.PRODUCT_H = matchingItems.Concat(nonMatchingItems).ToList();
                model.LISTING_JSON = JsonConvert.SerializeObject(model.PRODUCT_H);
                #endregion

                return View(model);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return RedirectToAction("Error", "Home");
            }
        }

        [SessionExpire]
        [HttpPost]
        public async Task<ActionResult> MM_FabricCategory_Edit(MM_FABRIC_CATEGORY model)
        {
            try
            {
                model.CATEGORY_H_DELETE_ID = "";
                model.RECORD_TYP = 3;
                model.CREATED_BY = username;
                model.CREATED_DATE = DateTime.Now;
                model.CREATED_LOC = Request.UserHostAddress;
                model = await dal.PSP_FABRIC_CATEGORY_MAINT(model);
                ViewBag.ReturnMessage = (model.RETURN_MESSAGE == "Saved") ? "Success" : "Fail";
                return View(model);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return RedirectToAction("Error", "Home");
            }
        }

        [SessionExpire]
        [HttpPost]
        public async Task<ActionResult> MM_FabricCategory_Delete(string[] id)
        {
            try
            {
                MM_FABRIC_CATEGORY model = new MM_FABRIC_CATEGORY();
                model.CATEGORY_H_DELETE_ID = string.Join(",", id);
                model.CATEGORY_NAME = "";
                model.CATEGORY_DETAIL = "";
                model.PRODUCT_H_ID = "";
                model.RECORD_TYP = 5;
                model.CREATED_BY = username;
                model.CREATED_DATE = DateTime.Now;
                model.CREATED_LOC = Request.UserHostAddress;
                model = await dal.PSP_FABRIC_CATEGORY_MAINT(model);

                return Json("SUCCESS", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return Json(string.Empty, JsonRequestBehavior.AllowGet);
            }
        }

        [SessionExpire]
        public async Task<ActionResult> MM_FabricCategory_AuditTrail(int id)
        {
            try
            {
                AuditTrailModel auditTrailModel = new AuditTrailModel();

                List<SqlParameter> _pMssql = new List<SqlParameter>();
                _pMssql.Add(new SqlParameter("@TABLE", "MM_CATEGORY_A"));
                _pMssql.Add(new SqlParameter("@KEY_VALUE", id));
                _pMssql.Add(new SqlParameter("@SortColumn", "UPDATED_DATE"));
                _pMssql.Add(new SqlParameter("@SortType", "DESC"));
                auditTrailModel = await AuditTrailHelper.AuditTrailStoreProcedureSqlAsync("PSP_GET_AUDIT_TRAIL", CommandType.StoredProcedure, _pMssql, "PAB_NEW_AQUA");

                ViewBag.HistoryID = id;
                ViewBag.JsonResult = auditTrailModel.JsonData;
                ViewBag.KeyNames = auditTrailModel.ListData;

                return View();
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public async Task<ActionResult> MM_FabricCategory_CategoryNameCheck(string categoryID, string categoryName)
        {
            try
            {
                List<SqlParameter> _pMssql = new List<SqlParameter>();
                _pMssql.Add(new SqlParameter("@pCategoryID", categoryID));
                _pMssql.Add(new SqlParameter("@pCategoryName", categoryName));
                _pMssql.Add(new SqlParameter("returnResult", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                string result = await common.PSP_COMMON_SQL("PSP_CHECK_FABRIC_CATEGORY_NAME", CommandType.StoredProcedure, _pMssql, "", "", true);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                throw;
            }
        }
    }
}