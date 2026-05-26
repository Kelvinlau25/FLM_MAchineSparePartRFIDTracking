using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using HomeModel;
using FILM_Sparepart_MVC.Helper_Code.Objects;
using FILM_Sparepart_MVC.Filters;
using FILM_Sparepart_MVC.Models;
using System.Security.Cryptography;
using System.Text.Json;

namespace FILM_Sparepart_MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IConfiguration configuration, ILogger<HomeController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        public static class CommonMethod
        {
            public static List<T> ConvertToList<T>(DataTable dt)
            {
                var columnNames = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName.ToLower()).ToList();
                var properties = typeof(T).GetProperties();
                return dt.AsEnumerable().Select(row => {
                    var objT = Activator.CreateInstance<T>();
                    foreach (var pro in properties)
                    {
                        if (columnNames.Contains(pro.Name.ToLower()))
                        {
                            try
                            {
                                if (pro.PropertyType.Name.Equals("Boolean"))
                                {
                                    if (row[pro.Name].ToString().ToUpper().Equals("TRUE")) { pro.SetValue(objT, true); }
                                    else { pro.SetValue(objT, false); }
                                }
                                else { pro.SetValue(objT, row[pro.Name]); }
                            }
                            catch (Exception ex) { }
                        }
                    }
                    return objT;
                }).ToList();
            }

            public static string ConvertSearchValue(string[] Scol, string str)
            {
                var val = "'%" + str + "%'";
                str = "";
                var additonal = "";
                foreach (var col in Scol)
                {
                    string[] c = col.Split(new Char[] { '/' });
                    str += (additonal + " UPPER(" + c[1] + ") " + "LIKE" + " UPPER(" + val + ") ");
                    additonal = " OR";
                }
                return str;
            }

        }
        public IActionResult Index()
        {
            string userAD = string.Empty;
            string vUserAD = string.Empty;
            string systemName = string.Empty;

            AuthenticatorModel model = new AuthenticatorModel();
            //userAD = Environment.UserName;
            userAD = User.Identity?.Name ?? string.Empty;
            systemName = _configuration["AppSettings:SystemName"];
            string[] splitWords = userAD.Split('\\');
            vUserAD = splitWords[splitWords.Length - 1];

            _logger.LogInformation("Index page accessed by user {UserAD}", vUserAD);

            DB db = new DB();
            model = db.ValidateUserInfo(vUserAD, systemName);            
            ViewBag.Validate = model.VALID_USER;
            ViewBag.userAD = userAD;
            ViewBag.userAD2 = vUserAD;
            if (model.VALID_USER == true)
            {
                _logger.LogInformation("User {UserAD} validated successfully", vUserAD);
                HttpContext.Session.SetString("AclUser", JsonSerializer.Serialize(new ACL_UserObj
                {
                    ID_ACL_USER = model.ID_ACL_USER,
                    ID_ACL_ROLE = model.ID_ACL_ROLE,
                    ID_ACL_RESOURCE = model.ID_ACL_RESOURCE,
                    USER_ID = model.USER_ID,
                    USR_EMAIL = model.USR_EMAIL,
                    COMPANY = model.COMPANY,
                    EMP_NO = model.EMP_NO,
                    EMP_NAME = model.EMP_NAME,
                    ROLE_NAME = model.ROLE_NAME,
                    ROLE_DESC = model.ROLE_DESC,
                    RESOURCE_NAME = model.RESOURCE_NAME,
                    RESOURCE_DESC = model.RESOURCE_DESC
                }));
            }
            else
            {
                _logger.LogWarning("User {UserAD} validation failed", vUserAD);
            }

            return View(model);
           
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            _logger.LogInformation("Login page accessed");
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult Login(AuthenticatorModel model)
        {
            if (ModelState.IsValid)  //checking model is valid or not
            {
                string systemName = string.Empty;
                systemName = _configuration["AppSettings:SystemName"];
                string passwordTMP = model.PASSWORD;
                DB db = new DB();
                _logger.LogInformation("[Login] Attempting ValidateUserInfo — LoginID: '{LoginID}', SystemName: '{SystemName}'", model.LOGIN_ID, systemName);
                model = db.ValidateUserInfo(model.LOGIN_ID, systemName);
                _logger.LogInformation("[Login] ValidateUserInfo result — VALID_USER: {Valid}, USER_ID: '{UserId}', EMP_NAME: '{EmpName}', ID_ACL_USER: {AclUser}, ID_ACL_ROLE: {AclRole}, ROLE_NAME: '{RoleName}', PASSWORD_is_null: {PwdNull}",
                    model.VALID_USER, model.USER_ID, model.EMP_NAME, model.ID_ACL_USER, model.ID_ACL_ROLE, model.ROLE_NAME, string.IsNullOrEmpty(model.PASSWORD));

                bool passwordMatch = false;
                try
                {
                    passwordMatch = !string.IsNullOrEmpty(model.PASSWORD) && VerifyHashedPassword(model.PASSWORD, passwordTMP);
                    _logger.LogInformation("[Login] Password verification result: {PasswordMatch}", passwordMatch);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[Login] Exception during VerifyHashedPassword for user '{LoginID}'", model.LOGIN_ID);
                    passwordMatch = false;
                }

                if (passwordMatch)
                {
                    model.VALID_USER = true;
                }
                else
                {
                    model.VALID_USER = false;
                }

                ViewBag.Validate = model.VALID_USER;

                ModelState.Clear();
                if (model.VALID_USER == true)
                {
                    _logger.LogInformation("[Login] User {UserId} logged in successfully — saving session AclUser. RoleID: {RoleID}", model.USER_ID, model.ID_ACL_ROLE);
                    HttpContext.Session.SetString("AclUser", JsonSerializer.Serialize(new ACL_UserObj
                    {
                        ID_ACL_USER = model.ID_ACL_USER,
                        ID_ACL_ROLE = model.ID_ACL_ROLE,
                        ID_ACL_RESOURCE = model.ID_ACL_RESOURCE,
                        USER_ID = model.USER_ID,
                        USR_EMAIL = model.USR_EMAIL,
                        COMPANY = model.COMPANY,
                        EMP_NO = model.EMP_NO,
                        EMP_NAME = model.EMP_NAME,
                        ROLE_NAME = model.ROLE_NAME,
                        ROLE_DESC = model.ROLE_DESC,
                        RESOURCE_NAME = model.RESOURCE_NAME,
                        RESOURCE_DESC = model.RESOURCE_DESC
                    }));
                    return RedirectToAction("Menu", model);
                }
                else
                {
                    _logger.LogWarning("Failed login attempt for user {LoginId}", model.LOGIN_ID);
                    return View(model);
                }

            }
            else
            {
                _logger.LogWarning("Login validation failed");
                ModelState.AddModelError("", "Please enter username and password");
                return View(model);
            }

        }
       
        [SessionExpire]
        public IActionResult Menu()
        {
            return View();
        }

        public IActionResult SideBar()
        {
            try
            {
                DB db = new DB();
                var aclUserJson = HttpContext.Session.GetString("AclUser");

                if (string.IsNullOrEmpty(aclUserJson))
                {
                    _logger.LogError("[SideBar] Session 'AclUser' is NULL or EMPTY. User may not be logged in or session expired.");
                    return View(new List<SideBarContent>());
                }

                var aclUser = JsonSerializer.Deserialize<ACL_UserObj>(aclUserJson);
                int roleID = aclUser.ID_ACL_ROLE;
                string systemName = _configuration["AppSettings:SystemName"];

                _logger.LogInformation("[SideBar] Querying sidebar — RoleID: {RoleID}, SystemName: '{SystemName}'", roleID, systemName);

                var dt = db.sideBarDB(roleID, systemName);

                _logger.LogInformation("[SideBar] Rows returned from PSP_ACL_SIDEBAR_FILM: {RowCount}", dt.Rows.Count);

                if (dt.Rows.Count == 0)
                {
                    _logger.LogWarning("[SideBar] No sidebar rows returned. Check that SystemName '{SystemName}' and RoleID {RoleID} exist in pfrACL database.", systemName, roleID);
                }
                else
                {
                    foreach (System.Data.DataRow row in dt.Rows)
                    {
                        _logger.LogInformation("[SideBar] Row — ID: {ID}, Name: {Name}, Layer: {Layer}, Controller: {Controller}, View: {View}",
                            row["ID_ACL_RESOURCE"],
                            row["RESOURCE_name"],
                            row["LAYER"],
                            row["RESOURCE_CONTROLLER"],
                            row["RESOURCE_VIEW"]);
                    }
                }

                List<SideBarContent> SideBarModel = CommonMethod.ConvertToList<SideBarContent>(dt);
                return View(SideBarModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SideBar] Exception occurred while loading sidebar.");
                return View(new List<SideBarContent>());
            }
        }

        public IActionResult ChangePassword()
        {

            return View();

        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordModel ChangePasswordModel)
        {
            try
            {                
                var aclUserJson = HttpContext.Session.GetString("AclUser");
                var aclUser = JsonSerializer.Deserialize<ACL_UserObj>(aclUserJson);
                DB db = new DB();
                DataTable dt = db.oldPassword(aclUser.ID_ACL_USER);
                //DataTable dt = db.oldPassword(25);
                bool a = VerifyHashedPassword(dt.Rows[0][0].ToString(), ChangePasswordModel.OLD_PASSWORD);
                if (VerifyHashedPassword(dt.Rows[0][0].ToString(), ChangePasswordModel.OLD_PASSWORD))
                {
                    if (db.NewPassWord(aclUser.ID_ACL_USER, HashPassword(ChangePasswordModel.NEW_PASSWORD)) == "Y")
                    {
                        _logger.LogInformation("Password changed successfully for user {UserId}", aclUser.ID_ACL_USER);
                        ViewData["Message"] = "Password is successfully changed. Please relogin again.";
                        ViewData["MessageType"] = "Y";                      
                    }
                    else
                    {
                        _logger.LogWarning("Password change failed for user {UserId}", aclUser.ID_ACL_USER);
                        ViewData["Message"] = "Failed change your password. Please try again.";
                        ViewData["MessageType"] = "E";                      
                    }
                }
                else
                {
                    _logger.LogWarning("Incorrect old password for user {UserId}", aclUser.ID_ACL_USER);
                    ViewData["Message"] = "Incorrect old password. Please try again.";
                    ViewData["MessageType"] = "E";                  
                }              
                ModelState.Clear();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                Console.Write(ex);
                ViewData["Message"] = ex.Message;
                ViewData["MessageType"] = "E";                
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            _logger.LogError("Error page accessed with RequestId: {RequestId}", Activity.Current?.Id ?? HttpContext.TraceIdentifier);
            return View(new ErrorViewModel 
            { 
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier 
            });
        }

        public string HashPassword(string password)
        {
            byte[] salt;
            byte[] buffer2;
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, 0x10, 0x3e8))
            {
                salt = bytes.Salt;
                buffer2 = bytes.GetBytes(0x20);
            }
            byte[] dst = new byte[0x31];
            Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
            Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
            return Convert.ToBase64String(dst);
        }

        public bool VerifyHashedPassword(string hashedPassword, string password)
        {
            byte[] buffer4;
            if (hashedPassword == null)
            {
                return false;
            }
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            byte[] src = Convert.FromBase64String(hashedPassword);
            //if ((src.Length != 0x31) || (src[0] != 0))
            //{
            //    return false;
            //}
            byte[] dst = new byte[0x10];
            Buffer.BlockCopy(src, 1, dst, 0, 0x10);
            byte[] buffer3 = new byte[0x20];
            Buffer.BlockCopy(src, 0x11, buffer3, 0, 0x20);
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, dst, 0x3e8))
            {
                buffer4 = bytes.GetBytes(0x20);
            }

            return buffer3.SequenceEqual(buffer4);
        }

    }
}