using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using PAB_NewAquarium.Helpers;
using PAB_NewAquarium.Models;
using System.Data;

namespace PAB_NewAquarium.Controllers
{
    public class ACLController : Controller
    {
        private readonly CommonFunction common = new CommonFunction();

        #region ACL

        public PartialViewResult OpenPartial(string PageName)
        {
            if(PageName == "TriggerForgetPassword")
            {
                return PartialView("TriggerForgetPassword");
            }
            else if(PageName == "VerifyOTP")
            {
                return PartialView("VerifyOTP");
            }
            else
            {
                return PartialView("ForgetPassword");
            }
        }

        public ActionResult Login(string ReturnUrl = "")
        {
            string message = TempData["Message"] as string;
            TempData.Remove("Message");
            string ResetSuccess = TempData["ResetSuccess"] as string;

            ViewBag.ResetSuccess = ResetSuccess;
            ViewBag.Message = message;

            AuthenticatorModel model = new AuthenticatorModel();
            model.RETURN_URL = ReturnUrl;
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(AuthenticatorModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string systemName = ConfigurationManager.AppSettings["SystemName_ACL"];
                    string companyCode = ConfigurationManager.AppSettings["CompanyCode_ACL"];
                    string DatabaseType = ConfigurationManager.AppSettings["DBTYPE"];
                    string ReturnURL = model.RETURN_URL;

                    LoginRequest loginmodel = new LoginRequest();
                    loginmodel.LoginID = model.LOGIN_ID;
                    loginmodel.LoginPass = model.PASSWORD;
                    loginmodel.SystemName = systemName;
                    loginmodel.CompanyCode = companyCode;
                    loginmodel.DatabaseType = DatabaseType;

                    ByteArrayContent clientbodystr = new StringContent(JsonConvert.SerializeObject(loginmodel), Encoding.UTF8, "application/json");

                    HttpClient client = new HttpClient();
                    HttpResponseMessage response = await client.PostAsync(ConfigurationManager.AppSettings["ACL_API"] + "/api/v1/login/login", clientbodystr);
                    if (response.IsSuccessStatusCode == true)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<LoginResponse>(responseBody);

                        Response.Cookies["JWTToken"].Value = result.JWTToken;
                        Response.Cookies["JWTToken"].Expires = DateTime.Now.AddMinutes(15);
                        Response.Cookies["JWTRefreshToken"].Value = result.JWTRefreshToken;
                        Response.Cookies["JWTRefreshToken"].Expires = DateTime.Now.AddMinutes(60);
                        HttpContext.Session["AclUser"] = ACLHelper.JWTTokenToACLObj(result.JWTToken);
                        int roleID = (Session["AclUser"] as ACL_UserObj).ID_ACL_ROLE;
                        var obj = new { ID_ACL_ROLE = roleID, pSystemName = systemName };
                        (Session["AclUser"] as ACL_UserObj).SIDEBAR_CONTENT = await common.PSP_COMMON_DAPPER<SideBarContent>("PSP_ACL_SIDEBAR_PERMISSION", CommandType.StoredProcedure, obj, "PAB_ACL_MVC");
                        (Session["AclUser"] as ACL_UserObj).ACCESS_LIST = await common.PSP_COMMON_DAPPER<AccessList>("PSP_AQUA_GET_ACL_PERMISSION", CommandType.StoredProcedure, obj, "PAB_ACL_MVC");
                        
                        ViewBag.Validate = true;

                        return View(model);
                    }
                    else
                    {
                        ViewBag.Validate = false;
                        return View(model);
                    }
                }
                catch
                {
                    ViewBag.Validate = false;
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("", "Error in saving data");
                return View();
            }
        }

        public async Task<ActionResult> Logout()
        {
            try
            {
                if (Request.Cookies["JWTToken"] == null && Request.Cookies["JWTRefreshToken"] == null)
                    return RedirectToAction("Login", "ACL");

                LogoutRequest request = new LogoutRequest
                {
                    JWTToken = Request.Cookies["JWTToken"] != null ? Request.Cookies["JWTToken"].Value : "",
                    JWTRefreshToken = Request.Cookies["JWTRefreshToken"] != null ? Request.Cookies["JWTRefreshToken"].Value : "",
                };

                ByteArrayContent clientbodystr = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.PostAsync(ConfigurationManager.AppSettings["ACL_API"] + "/api/v1/logout/logout", clientbodystr);

                if (Request.Cookies["JWTToken"] != null)
                {
                    Response.Cookies["JWTToken"].Expires = DateTime.Now.AddDays(-1);
                }

                if (Request.Cookies["ASP.NET_SessionId"] != null)
                {
                    Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddDays(-1);
                }

                return RedirectToAction("Login", "ACL");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "ACL", new { alertMessage = "Error: " + ex.Message });
            }
        }

        public ActionResult TriggerForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> TriggerForgetPassword(TriggerForgetPasswordViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string companyCode = ConfigurationManager.AppSettings["CompanyCode_ACL"];
                    string DatabaseType = ConfigurationManager.AppSettings["DBTYPE"];
                    string systemName = ConfigurationManager.AppSettings["SystemName_ACL"];

                    TriggerResetPasswordRequest request = new TriggerResetPasswordRequest()
                    {
                        CompanyCode = companyCode,
                        EmailAddress = viewModel.EmailAddress,
                        ResetPasswordURL = Request.Url.GetLeftPart(UriPartial.Authority) + Url.Action("VerifyOTP", "ACL") + $"?EmailAddress={viewModel.EmailAddress}&OTP=",
                        DatabaseType = DatabaseType,
                        SystemName = systemName
                    };

                    ByteArrayContent clientbodystr = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

                    HttpClient client = new HttpClient();
                    HttpResponseMessage response = await client.PostAsync(ConfigurationManager.AppSettings["ACL_API"] + "/api/v1/Password/TriggerResetPassword", clientbodystr);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode == true)
                    {
                        ViewBag.Validate = true;
                        TempData["EmailAddress"] = viewModel.EmailAddress;
                        return Json(new { success = true });
                    }
                    else
                    {
                        ViewBag.Validate = false;
                        return Json(new { success = false, errorMessage = "Fail: " + responseBody });
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Validate = false;
                    return Json(new { success = false, errorMessage = "Error: " + ex.Message });
                }
            }
            else
            {
                return Json(new { success = false, errorMessage = "Invalid data." });
            }
        }

        public ActionResult VerifyOTP(string EmailAddress = "", string OTP = "")
        {
            VerifyOTPViewModel viewModel = new VerifyOTPViewModel
            {
                EmailAddress = EmailAddress,
                OTP = OTP,
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> VerifyOTP(VerifyOTPViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string companyCode = ConfigurationManager.AppSettings["CompanyCode_ACL"];
                    string DatabaseType = ConfigurationManager.AppSettings["DBTYPE"];

                    VerifyOTPRequest request = new VerifyOTPRequest()
                    {
                        CompanyCode = companyCode,
                        EmailAddress = viewModel.EmailAddress,
                        OTP = viewModel.OTP,
                        DatabaseType = DatabaseType,
                    };

                    ByteArrayContent clientbodystr = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

                    HttpClient client = new HttpClient();
                    HttpResponseMessage response = await client.PostAsync(ConfigurationManager.AppSettings["ACL_API"] + "/api/v1/Password/VerifyOTP", clientbodystr);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode == true)
                    {
                        ViewBag.Validate = true;
                        TempData["EmailAddress"] = viewModel.EmailAddress;
                        TempData["OTP"] = viewModel.OTP;
                        return Json(new { success = true });
                    }
                    else
                    {
                        ViewBag.Validate = false;
                        return Json(new { success = false, errorMessageOTP = responseBody });
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Validate = false;
                    return Json(new { success = false, errorMessageOTP = "Error: " + ex.Message });
                }
            }
            else
            {
                if ((viewModel.EmailAddress == "" || viewModel.EmailAddress == null) && (viewModel.OTP == "" || viewModel.OTP == null))
                {
                    return Json(new { success = false, errorMessageOTP = "Invalid OTP!", errorMessageEmail = "Please enter email!" });
                }

                if ((viewModel.EmailAddress == "" || viewModel.EmailAddress == null) && (viewModel.OTP != "" || viewModel.OTP != null))
                {
                    return Json(new { success = false, errorMessageEmail = "Please enter email!" });
                }

                if ((viewModel.EmailAddress != "" || viewModel.EmailAddress != null) && (viewModel.OTP == "" || viewModel.OTP == null))
                {
                    return Json(new { success = false, errorMessageOTP = "Invalid OTP!"});
                }

                return Json(new { success = false, errorMessage = "Error: Missing fields" });
            }
        }

        public ActionResult ForgetPassword(string EmailAddress = "", string OTP = "")
        {
            if (EmailAddress == "" || OTP == "")
            {
                return RedirectToAction("Login", "ACL", new { AlertMessage = "Error: Missing fields" });
            }

            ForgetPasswordViewModel viewModel = new ForgetPasswordViewModel
            {
                EmailAddress = EmailAddress,
                OTP = OTP,
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> ForgetPassword(ForgetPasswordViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (viewModel.NewPass != viewModel.ConfirmNewPass)
                        return RedirectToAction("ForgetPassword", "ACL", new { AlertMessage = "Fail: Password do not match" });
                    else if (!ACLHelper.ValidatePassword(viewModel.NewPass))
                        return RedirectToAction("ForgetPassword", "ACL", new { AlertMessage = "Fail: Password doesn't meet requirement" });

                    string companyCode = ConfigurationManager.AppSettings["CompanyCode_ACL"];
                    string DatabaseType = ConfigurationManager.AppSettings["DBTYPE"];

                    ResetPasswordRequest request = new ResetPasswordRequest()
                    {
                        CompanyCode = companyCode,
                        EmailAddress = viewModel.EmailAddress,
                        OTP = viewModel.OTP,
                        NewPass = viewModel.NewPass,
                        DatabaseType = DatabaseType,
                    };

                    ByteArrayContent clientbodystr = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

                    HttpClient client = new HttpClient();
                    HttpResponseMessage response = await client.PostAsync(ConfigurationManager.AppSettings["ACL_API"] + "/api/v1/Password/ResetPassword", clientbodystr);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode == true)
                    {
                        TempData["ResetSuccess"] = "Password reset successfully! Please re-login.";
                        ViewBag.Validate = true;
                        return Json(new { success = true });
                    }
                    else
                    {
                        ViewBag.Validate = false;
                        return Json(new { success = false, errorMessage = "Fail!" });
                    }
                }
                catch (Exception)
                {
                    ViewBag.Validate = false;
                    return Json(new { success = false, errorMessage = "Error occured." });
                }
            }
            else
            {
                return Json(new { success = false, errorMessage = "Missing Field." });
            }
        }

        #endregion
    }
}