using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HomeModel;
using FILM_Sparepart_MVC.Helper_Code.Objects;
using FILM_Sparepart_MVC.Filters;

using System.Security.Cryptography;

namespace FILM_Sparepart_MVC.Controllers
{
    public class HomeController : Controller
    {
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
        public ActionResult Index()
        {
            string userAD = string.Empty;
            string vUserAD = string.Empty;
            string systemName = string.Empty;

            AuthenticatorModel model = new AuthenticatorModel();
            //userAD = Environment.UserName;
            userAD = System.Web.HttpContext.Current.User.Identity.Name;
            systemName = ConfigurationManager.AppSettings["SystemName"];
            string[] splitWords = userAD.Split('\\');
            vUserAD = splitWords[splitWords.Length - 1];

            DB db = new DB();
            model = db.ValidateUserInfo(vUserAD, systemName);            
            ViewBag.Validate = model.VALID_USER;
            ViewBag.userAD = userAD;
            ViewBag.userAD2 = vUserAD;
            if (model.VALID_USER == true)
            {
                Session["AclUser"] = new ACL_UserObj
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
                };
            }

            return View(model);
           
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(AuthenticatorModel model)
        {
            if (ModelState.IsValid)  //checking model is valid or not
            {
                string systemName = string.Empty;
                systemName = ConfigurationManager.AppSettings["SystemName"];
                string passwordTMP = model.PASSWORD;
                DB db = new DB();
                model = db.ValidateUserInfo(model.LOGIN_ID, systemName);

                if (VerifyHashedPassword(model.PASSWORD, passwordTMP))
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
                    Session["AclUser"] = new ACL_UserObj
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
                    };
                    return RedirectToAction("Menu", model);
                }
                else
                {
                    return View(model);
                }

            }
            else
            {
                ModelState.AddModelError("", "Error in saving data");
                return View();
            }

        }
       
        [SessionExpire]
        public ActionResult Menu()
        {
            return View();
        }

        public ActionResult SideBar()
        {
            DB db = new DB();
            int roleID = (Session["AclUser"] as ACL_UserObj).ID_ACL_ROLE;
            string systemName = ConfigurationManager.AppSettings["SystemName"];
            var dt = db.sideBarDB(roleID, systemName);
            List<SideBarContent> SideBarModel = CommonMethod.ConvertToList<SideBarContent>(dt);
            return View(SideBarModel);
        }

        public ActionResult ChangePassword()
        {

            return View();

        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel ChangePasswordModel)
        {
            try
            {                
                var aclUser = Session["AclUser"];
                DB db = new DB();
                DataTable dt = db.oldPassword((aclUser as ACL_UserObj).ID_ACL_USER);
                //DataTable dt = db.oldPassword(25);
                bool a = VerifyHashedPassword(dt.Rows[0][0].ToString(), ChangePasswordModel.OLD_PASSWORD);
                if (VerifyHashedPassword(dt.Rows[0][0].ToString(), ChangePasswordModel.OLD_PASSWORD))
                {
                    if (db.NewPassWord((aclUser as ACL_UserObj).ID_ACL_USER, HashPassword(ChangePasswordModel.NEW_PASSWORD)) == "Y")
                    {
                        ViewData["Message"] = "Password is successfully changed. Please relogin again.";
                        ViewData["MessageType"] = "Y";                      
                    }
                    else
                    {
                        ViewData["Message"] = "Failed change your password. Please try again.";
                        ViewData["MessageType"] = "E";                      
                    }
                }
                else
                {
                    ViewData["Message"] = "Incorrect old password. Please try again.";
                    ViewData["MessageType"] = "E";                  
                }              
                ModelState.Clear();
            }
            catch (Exception ex)
            {
                // Info
                Console.Write(ex);
                ViewData["Message"] = ex.Message;
                ViewData["MessageType"] = "E";                
            }
            return View();
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