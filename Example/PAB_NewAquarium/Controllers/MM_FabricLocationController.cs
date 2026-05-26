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
    public class MM_FabricLocationController : BaseController
    {
        [SessionExpire]
        public async Task<ActionResult> MM_FabricLocation()
        {
            try
            {
                FabricLocation model = new FabricLocation();
                model.FabricLocationListing = new List<FabricLocationList>();
                model.FabricLocationListing = await common.PSP_COMMON_DAPPER<FabricLocationList>("PSP_GET_FABRIC_LOCATION_LIST", CommandType.StoredProcedure);
                
                return View(model);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return RedirectToAction("Error", "Home");
            }
        }

        [SessionExpire]
        public async Task<ActionResult> MM_FabricLocation_View(string id)
        {
            try
            {
                FabricLocationDetail model = await GetLocationDetailAsync(id);
                model.LocationTypeDdl = await common.PSP_COMMON_DROPDOWN("PLocType");

                return View(model);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return RedirectToAction("Error", "Home");
            }
        }

        [SessionExpire]
        public async Task<ActionResult> MM_FabricLocation_Create()
        {
            try
            {
                ViewBag.Result = null;
                FabricLocationDetail model = new FabricLocationDetail();
                model.LocationTypeDdl = await common.PSP_COMMON_DROPDOWN("PLocType");

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
        public async Task<ActionResult> MM_FabricLocation_Create(FabricLocationDetail model, string btn)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    List<SqlParameter> _pMssql = new List<SqlParameter>();
                    _pMssql.Add(new SqlParameter("@pID", model.LOC_CODE));
                    _pMssql.Add(new SqlParameter("@pDeleteID", ""));
                    _pMssql.Add(new SqlParameter("@pLocationDescription", model.LOC_DESC));
                    _pMssql.Add(new SqlParameter("@pLocationType", model.LOC_TYPE));
                    _pMssql.Add(new SqlParameter("@pRFIDTag", model.RFID_TAG == null ? "" : model.RFID_TAG));
                    _pMssql.Add(new SqlParameter("@pRecordTyp", "1"));
                    _pMssql.Add(new SqlParameter("@pCreatedBy", username));
                    _pMssql.Add(new SqlParameter("@pCreatedLoc", Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString()));
                    _pMssql.Add(new SqlParameter("returnResult", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });

                    string result = await common.PSP_COMMON_SQL("PSP_FABRIC_LOCATION_DETAIL_MAINT", CommandType.StoredProcedure, _pMssql, "", "", true);

                    ViewBag.Result = result;
                }
                model.LocationTypeDdl = await common.PSP_COMMON_DROPDOWN("PLocType");

                return View(model);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return RedirectToAction("Error", "Home");
            }
        }

        [SessionExpire]
        public async Task<ActionResult> MM_FabricLocation_Edit(string id)
        {
            try
            {
                ViewBag.Result = null;
                FabricLocationDetail model = await GetLocationDetailAsync(id);
                model.LocationTypeDdl = await common.PSP_COMMON_DROPDOWN("PLocType");

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
        public async Task<ActionResult> MM_FabricLocation_Edit(FabricLocationDetail model, string btn)
        {
            try
            {
                model.LOC_CODE = model.LOC_CODE_ID;
                if (ModelState.IsValid)
                {
                    string recordTyp = "3";

                    List<SqlParameter> _pMssql = new List<SqlParameter>();
                    _pMssql.Add(new SqlParameter("@pID", model.LOC_CODE_ID));
                    _pMssql.Add(new SqlParameter("@pDeleteID", ""));
                    _pMssql.Add(new SqlParameter("@pLocationDescription", model.LOC_DESC));
                    _pMssql.Add(new SqlParameter("@pLocationType", model.LOC_TYPE));
                    _pMssql.Add(new SqlParameter("@pRFIDTag", model.RFID_TAG == null ? "" : model.RFID_TAG));
                    _pMssql.Add(new SqlParameter("@pRecordTyp", recordTyp));
                    _pMssql.Add(new SqlParameter("@pCreatedBy", username));
                    _pMssql.Add(new SqlParameter("@pCreatedLoc", Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString()));
                    _pMssql.Add(new SqlParameter("returnResult", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });

                    string result = await common.PSP_COMMON_SQL("PSP_FABRIC_LOCATION_DETAIL_MAINT", CommandType.StoredProcedure, _pMssql, "", "", true);

                    ViewBag.Result = result;
                }
                model.LocationTypeDdl = await common.PSP_COMMON_DROPDOWN("PLocType");

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
        public async Task<ActionResult> MM_FabricLocation_DeleteListing(string[] id)
        {
            try
            {
                string deleteIDStr = string.Join(",", id);

                List<SqlParameter> _pMssql = new List<SqlParameter>();
                _pMssql.Add(new SqlParameter("@pID", ""));
                _pMssql.Add(new SqlParameter("@pDeleteID", deleteIDStr));
                _pMssql.Add(new SqlParameter("@pLocationDescription", ""));
                _pMssql.Add(new SqlParameter("@pLocationType", ""));
                _pMssql.Add(new SqlParameter("@pRFIDTag", ""));
                _pMssql.Add(new SqlParameter("@pRecordTyp", "5"));
                _pMssql.Add(new SqlParameter("@pCreatedBy", username));
                _pMssql.Add(new SqlParameter("@pCreatedLoc", Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString()));
                _pMssql.Add(new SqlParameter("returnResult", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });

                string result = await common.PSP_COMMON_SQL("PSP_FABRIC_LOCATION_DETAIL_MAINT", CommandType.StoredProcedure, _pMssql, "", "", true);
                bool isSuccessStr = true;
                if (result == "Success")
                {
                    isSuccessStr = true;
                }
                else
                {
                    isSuccessStr = false;
                }

                return Json(new { valid = isSuccessStr }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return Json(new { valid = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [SessionExpire]
        public async Task<ActionResult> MM_FabricLocation_AuditTrail(string id)
        {
            try
            {
                AuditTrailModel auditTrailModel = new AuditTrailModel();

                List<SqlParameter> _pMssql = new List<SqlParameter>();
                _pMssql.Add(new SqlParameter("@TABLE", "MM_LOCATION"));
                _pMssql.Add(new SqlParameter("@KEY_VALUE", id));
                _pMssql.Add(new SqlParameter("@SortColumn", "UPDATED_DATE"));
                _pMssql.Add(new SqlParameter("@SortType", "DESC"));
                auditTrailModel = await AuditTrailHelper.AuditTrailStoreProcedureSqlAsync("PSP_GET_AUDIT_TRAIL", CommandType.StoredProcedure, _pMssql, "PAB_NEW_AQUA");


                ViewBag.JsonResult = auditTrailModel.JsonData;
                ViewBag.KeyNames = auditTrailModel.ListData;
                ViewBag.LOC_CODE = id;

                return View();
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<FabricLocationDetail> GetLocationDetailAsync(string id)
        {
            try
            {
                var obj = new { pID = id };
                FabricLocationDetail model = await common.PSP_COMMON_DAPPER_SINGLE<FabricLocationDetail>("PSP_GET_FABRIC_LOCATION_DETAIL", CommandType.StoredProcedure, obj);
                return model;
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                throw ex;
            }
        }
    }
}