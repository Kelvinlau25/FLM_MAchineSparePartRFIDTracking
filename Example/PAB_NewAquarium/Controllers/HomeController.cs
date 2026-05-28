using PAB_NewAquarium.Filters;
using PAB_NewAquarium.Models;
using PAB_NewAquarium.Helpers;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Configuration;
using System.Data;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Data.SqlClient;
using System;

namespace PAB_NewAquarium.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult AccessDenied()
        {
            return View();
        }

        public ActionResult SessionExpired()
        {
            return View();
        }

        public ActionResult NotFoundPage()
        {
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }

        [SessionExpire]
        public ActionResult ChangePassword()
        {
            HttpContext.Session["parts"] = "Inspection" ?? " ";
            ViewBag.Parts = "Inspection" ?? " ";
            ViewBag.butParts = "hide";
            return View();
        }

        [HttpPost]
        [SessionExpire]
        public async Task<ActionResult> ChangePassword(ChangePasswordModel ChangePasswordModel)
        {
            try
            {
                if (ChangePasswordModel.NEW_PASSWORD.ToString() != ChangePasswordModel.CONFIRM_NEW_PASSWORD.ToString())
                {
                    ModelState.Clear();
                    ViewBag.Validate = "Fail";
                    ViewData["Message"] = "Passwords don't match.";
                    ViewData["MessageType"] = "E";
                    return View();
                }

                string aclUser = (Session["AclUser"] as ACL_UserObj).USER_ID;
                string systemName = ConfigurationManager.AppSettings["SystemName_ACL"];
                string companyCode = ConfigurationManager.AppSettings["CompanyCode_ACL"];
                string DatabaseType = ConfigurationManager.AppSettings["DBTYPE"];

                ChangePasswordRequest changepasswordRequest = new ChangePasswordRequest();
                changepasswordRequest.LoginID = aclUser;
                changepasswordRequest.NewPass = ChangePasswordModel.NEW_PASSWORD;
                changepasswordRequest.OldPass = ChangePasswordModel.OLD_PASSWORD;
                changepasswordRequest.CompanyCode = companyCode;
                changepasswordRequest.DatabaseType = DatabaseType;

                ByteArrayContent clientbodystr = new StringContent(JsonConvert.SerializeObject(changepasswordRequest), Encoding.UTF8, "application/json");

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.PostAsync(ConfigurationManager.AppSettings["ACL_API"] + "/api/v1/Password/ChangePassword", clientbodystr);

                if (response.IsSuccessStatusCode == true)
                {
                    TempData["Message"] = "SuccessChange";
                    return RedirectToAction("Login", "ACL");
                }
                else
                {
                    ViewBag.Validate = "Fail"; // Or "Error"
                    ViewData["Message"] = "Password change failed."; // Optional error message
                    ViewData["MessageType"] = "E";
                    return View();
                }
            }
            catch
            {
                ViewBag.Validate = "Error";
                ViewData["Message"] = "Invalid!";
                return View();
            }
        }

        public async Task<ActionResult> Notification()
        {
            CommonFunction common = new CommonFunction();
            ACL_UserObj aclObj = (Session["AclUser"] as ACL_UserObj);
            Notification result = new Notification();
            result.NotificationListing = new List<NotificationList>();

            var obj = new { pCreatedBy = aclObj.EMP_NO + " - " + aclObj.EMP_NAME };
            result.NotificationListing = await common.PSP_COMMON_DAPPER<NotificationList>("PSP_GET_NOTIFICATION_LIST", CommandType.StoredProcedure, obj);

            return View(result);
        }

        [HttpPost]
        public async Task<string> Notification(string[] noti_ID)
        {
            CommonFunction common = new CommonFunction();
            string notiStr = string.Join(",", noti_ID);

            List<SqlParameter> _pMssql = new List<SqlParameter>();
            _pMssql.Add(new SqlParameter("@pNotiID", notiStr));

            await common.PSP_COMMON_SQL("PSP_UPDATE_NOTIFICATION_ISSEEN", CommandType.StoredProcedure, _pMssql, "", "", false);

            return "";
        }

        [HttpPost]
        public JsonResult CheckSessionTimeout()
        {
            try
            {
                if (Session["AclUser"] == null)
                {
                    return Json(new { sessionExpired = true }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { sessionExpired = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { sessionExpired = true }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}