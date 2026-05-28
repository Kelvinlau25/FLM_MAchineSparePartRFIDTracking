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
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PAB_NewAquarium.Controllers
{
    public class MM_ProductMaterialController : BaseController
    {
        readonly MM_PRODUCT_MATERIAL_DAL dal = new MM_PRODUCT_MATERIAL_DAL();

        [SessionExpire]
        public async Task<ActionResult> MM_ProductMaterial_List()
        {
            try
            {
                MM_PRODUCT_MATERIAL model = new MM_PRODUCT_MATERIAL();
                var obj = new { pID = 0 };
                model.LISTING = await common.PSP_COMMON_DAPPER<MM_PRODUCT_MATERIAL_LISTING>("PSP_GET_PRODUCT_MATERIAL", CommandType.StoredProcedure, obj);
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
        public async Task<ActionResult> MM_ProductMaterial_View(int id)
        {
            try
            {
                var obj = new { pID = id };
                MM_PRODUCT_MATERIAL model = await common.PSP_COMMON_DAPPER_SINGLE<MM_PRODUCT_MATERIAL>("PSP_GET_PRODUCT_MATERIAL", CommandType.StoredProcedure, obj);
                return View(model);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return RedirectToAction("Error", "Home");
            }
        }

        [SessionExpire]
        public async Task<ActionResult> MM_ProductMaterial_Create()
        {
            try
            {
                MM_PRODUCT_MATERIAL model = new MM_PRODUCT_MATERIAL();
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
        public async Task<ActionResult> MM_ProductMaterial_Create(MM_PRODUCT_MATERIAL model)
        {
            try
            {
                ACL_UserObj aclObj = (Session["AclUser"] as ACL_UserObj);
                model.PRODUCT_MATERIAL_ID = 0;
                model.PRODUCT_MATERIAL_DELETE_ID = "";
                model.RECORD_TYP = 1;
                model.CREATED_BY = aclObj.EMP_NO + " - " + aclObj.EMP_NAME;
                model.CREATED_DATE = DateTime.Now;
                model.CREATED_LOC = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString();
                model = await dal.PSP_PRODUCT_MATERIAL_MAINT(model);
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
        public async Task<ActionResult> MM_ProductMaterial_Edit(int id)
        {
            try
            {
                var obj = new { pID = id };
                MM_PRODUCT_MATERIAL model = await common.PSP_COMMON_DAPPER_SINGLE<MM_PRODUCT_MATERIAL>("PSP_GET_PRODUCT_MATERIAL", CommandType.StoredProcedure, obj);
                return View(model);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return RedirectToAction("Error", "Home");
            }        }

        [SessionExpire]
        [HttpPost]
        public async Task<ActionResult> MM_ProductMaterial_Edit(MM_PRODUCT_MATERIAL model)
        {
            try
            {
                ACL_UserObj aclObj = (Session["AclUser"] as ACL_UserObj);
                model.PRODUCT_MATERIAL_DELETE_ID = "";
                model.RECORD_TYP = 3;
                model.CREATED_BY = aclObj.EMP_NO + " - " + aclObj.EMP_NAME;
                model.CREATED_DATE = DateTime.Now;
                model.CREATED_LOC = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString();
                model = await dal.PSP_PRODUCT_MATERIAL_MAINT(model);
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
        public async Task<ActionResult> MM_ProductMaterial_Delete(string[] id)
        {
            try
            {
                ACL_UserObj aclObj = (Session["AclUser"] as ACL_UserObj);
                MM_PRODUCT_MATERIAL model = new MM_PRODUCT_MATERIAL();
                model.PRODUCT_MATERIAL_DELETE_ID = string.Join(",", id);
                model.MATERIAL_TYPE = "";
                model.MATERIAL_DESC = "";
                model.RECORD_TYP = 5;
                model.CREATED_BY = aclObj.EMP_NO + " - " + aclObj.EMP_NAME;
                model.CREATED_DATE = DateTime.Now;
                model.CREATED_LOC = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString();
                model = await dal.PSP_PRODUCT_MATERIAL_MAINT(model);

                return Json(new { valid = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return Json(new { valid = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [SessionExpire]
        public async Task<ActionResult> MM_ProductMaterial_AuditTrail(int id)
        {
            try
            {
                AuditTrailModel auditTrailModel = new AuditTrailModel();

                List<SqlParameter> _pMssql = new List<SqlParameter>();
                _pMssql.Add(new SqlParameter("@TABLE", "MM_PRODUCT_MATERIAL_A"));
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
    }
}