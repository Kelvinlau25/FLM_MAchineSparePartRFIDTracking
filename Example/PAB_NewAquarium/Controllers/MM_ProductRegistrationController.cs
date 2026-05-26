using Newtonsoft.Json;
using PAB_NewAquarium.DAL;
using PAB_NewAquarium.Filters;
using PAB_NewAquarium.Helpers;
using PAB_NewAquarium.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PAB_NewAquarium.Controllers
{
    public class MM_ProductRegistrationController : BaseController
    {
        readonly MM_ProductRegistration_DAL dal = new MM_ProductRegistration_DAL();
        readonly ReaderRFID readerRFID = new ReaderRFID();
        readonly BlobHelper blob = new BlobHelper();

        [SessionExpire]
        public async Task<ActionResult> MM_ProductRegistration()
        {
            try
            {
                Product_H model = new Product_H();
                model.STATUS_DDL = await common.PSP_COMMON_DROPDOWN("PProductListStatus");
                model.FABRIC_TYPE_DDL = await common.PSP_COMMON_DROPDOWN("PFabricType");
                model.ACTION_LIST = await common.PSP_COMMON_GET_LIST_ITEMS("PFabricListingAction");
                DataTable finishedType = await common.PSP_COMMON_SQL("PSP_GET_FINISHED_TYPE", CommandType.StoredProcedure);
                List<SelectListItem> finishedTypeDdl = new List<SelectListItem>();
                foreach (DataRow row in finishedType.Rows)
                {
                    finishedTypeDdl.Add(new SelectListItem()
                    {
                        Value = row["FINISHED_TYPE"].ToString().Trim(),
                        Text = row["FINISHED_TYPE"].ToString().Trim()
                    });
                }
                model.FINISHED_TYPE_DDL = finishedTypeDdl;

                return View(model);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return RedirectToAction("Error", "Home");
            }
        }

        [SessionExpire]
        public async Task<ActionResult> MM_ProductRegistration_View(int id, string status)
        {

            Product_H_Detail model = await dal.GetProductDetailAsync(id);
            model.STATUS = status;
            model.imageList = JsonConvert.SerializeObject(await dal.GetImage(id));
            dal.DeleteFolder(model.REFERENCE_NO);

            return View(model);
        }

        [SessionExpire]
        [HttpPost]
        public async Task<ActionResult> MM_ProductRegistration_View(Product_H_Detail model, string btn)
        {
            ACL_UserObj aclObj = (Session["AclUser"] as ACL_UserObj);
            string recordTyp = "5";
            Product_H_Detail returnModel = await dal.GetProductDetailAsync(model.PRODUCT_H_ID);
            returnModel.imageList = JsonConvert.SerializeObject(await dal.GetImage(model.PRODUCT_H_ID));

            List<SqlParameter> _pMssql = new List<SqlParameter>();
            _pMssql.Add(new SqlParameter("@pID", model.PRODUCT_H_ID));
            _pMssql.Add(new SqlParameter("@pDeleteID", model.PRODUCT_H_ID));
            _pMssql.Add(new SqlParameter("@pReferenceNo", ""));
            _pMssql.Add(new SqlParameter("@pCategoryStr", ""));
            _pMssql.Add(new SqlParameter("@pProductType1", ""));
            _pMssql.Add(new SqlParameter("@pProductType2", ""));
            _pMssql.Add(new SqlParameter("@pProductType3", ""));
            _pMssql.Add(new SqlParameter("@pProductType4", ""));
            _pMssql.Add(new SqlParameter("@pProductType5", ""));
            _pMssql.Add(new SqlParameter("@pProductType6", ""));
            _pMssql.Add(new SqlParameter("@pMaterialType1", ""));
            _pMssql.Add(new SqlParameter("@pMaterialType2", ""));
            _pMssql.Add(new SqlParameter("@pMaterialType3", ""));
            _pMssql.Add(new SqlParameter("@pMaterialType4", ""));
            _pMssql.Add(new SqlParameter("@pMaterialType5", ""));
            _pMssql.Add(new SqlParameter("@pMaterialType6", ""));
            _pMssql.Add(new SqlParameter("@pRfidTag", ""));
            _pMssql.Add(new SqlParameter("@pHangerLoc", ""));
            _pMssql.Add(new SqlParameter("@pOwnedBy", ""));
            _pMssql.Add(new SqlParameter("@pRecordTyp", recordTyp));
            _pMssql.Add(new SqlParameter("@pCreatedBy", aclObj.EMP_NO + " - " + aclObj.EMP_NAME));
            _pMssql.Add(new SqlParameter("@pCreatedLoc", Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString()));
            _pMssql.Add(new SqlParameter("returnResult", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });

            string result = await common.PSP_COMMON_SQL("PSP_MM_PRODUCT_DETAIL_MAINT", CommandType.StoredProcedure, _pMssql, "", "", true);

            ViewBag.Result = "Success";

            return View(returnModel);
        }

        [SessionExpire]
        public async Task<ActionResult> MM_ProductRegistration_Edit(int id, string status)
        {
            ACL_UserObj aclObj = (Session["AclUser"] as ACL_UserObj);
            ViewBag.Role = aclObj.ROLE_NAME;
            ViewBag.Result = null;
            Product_H_Detail model = await dal.GetProductDetailAsync(id, "Edit");
            model.RFID_READER = await readerRFID.GetReader("RFIDFabricReg");
            model.PRODUCT_TYPE_SELECTED = new List<string>();
            model.MATERIAL_TYPE_SELECTED = new List<string>();
            model.CATEGORY_SELECTED = new List<string>();

            #region Mutiple Dropdown Common Function
            DataTable dataTable = new DataTable()
            {
                Columns = { "TABLE_INDEX", "TABLE_NAME", "TABLE_VALUE", "TABLE_TEXT", "TABLE_CONDITION" }
            };

            dataTable.Rows.Add("1", "MM_PRODUCT_TYPE", "PRODUCT_TYPE", "PRODUCT_TYPE", "");
            dataTable.Rows.Add("2", "MM_PRODUCT_MATERIAL", "MATERIAL_TYPE", "MATERIAL_TYPE", "");
            dataTable.Rows.Add("3", "MM_CATEGORY_H", "CATEGORY_H_ID", "CATEGORY_NAME", "");
            dataTable.Rows.Add("4", "MM_PRODUCT_D", "COLOR_WAY", "COLOR_WAY", "AND PRODUCT_H_ID = " + id + "");
            var obj = new { pDataTable = dataTable };
            List<DropDownModel> ListModel = await common.PSP_COMMON_DAPPER<DropDownModel>("PSP_COMMON_DROPDOWN_MULTIPLE", CommandType.StoredProcedure, obj);

            model.PRODUCT_TYPE_DDL = ListModel.Where(item => item.TABLE_INDEX == "1").Select(item => new SelectListItem { Value = item.TABLE_VALUE, Text = item.TABLE_TEXT }).ToList();
            model.MATERIAL_TYPE_DDL = ListModel.Where(item => item.TABLE_INDEX == "2").Select(item => new SelectListItem { Value = item.TABLE_VALUE, Text = item.TABLE_TEXT }).ToList();
            model.CATEGORY_DDL = ListModel.Where(item => item.TABLE_INDEX == "3").Select(item => new SelectListItem { Value = item.TABLE_VALUE, Text = item.TABLE_TEXT }).ToList();
            model.COLOR_WAY_DDL = ListModel.Where(item => item.TABLE_INDEX == "4").Select(item => new SelectListItem { Value = item.TABLE_VALUE, Text = item.TABLE_TEXT }).ToList();
            #endregion

            model.HANGER_LOC_DDL = await common.PSP_COMMON_DAPPER<SelectListItem>("PSP_GET_LOCATION_DDL", CommandType.StoredProcedure, null);

            if (ConfigurationManager.AppSettings["isTest"].ToUpper().ToString() == "TRUE")
            {
                model.VIDEO_CAT_DDL = await common.PSP_COMMON_DAPPER<SelectListItem>("PSP_GET_VIDEO_CATEGORY_DROPDOWN", CommandType.StoredProcedure, null, "PAB_SampleInv_Test");
            }
            else
            {
                model.VIDEO_CAT_DDL = await common.PSP_COMMON_DAPPER<SelectListItem>("PSP_GET_VIDEO_CATEGORY_DROPDOWN", CommandType.StoredProcedure, null, "PAB_SampleInv");
            }

            model.STATUS = status;
            model.imageList = JsonConvert.SerializeObject(await dal.GetImage(id));

            ViewBag.CategoryNum = model.CATEGORY_DDL.Count;

            if (model.PRODUCT_TYPE1 != null && model.PRODUCT_TYPE1 != "")
                model.PRODUCT_TYPE_SELECTED.Add(model.PRODUCT_TYPE1);

            if (model.PRODUCT_TYPE2 != null && model.PRODUCT_TYPE2 != "")
                model.PRODUCT_TYPE_SELECTED.Add(model.PRODUCT_TYPE2);

            if (model.PRODUCT_TYPE3 != null && model.PRODUCT_TYPE3 != "")
                model.PRODUCT_TYPE_SELECTED.Add(model.PRODUCT_TYPE3);

            if (model.PRODUCT_TYPE4 != null && model.PRODUCT_TYPE4 != "")
                model.PRODUCT_TYPE_SELECTED.Add(model.PRODUCT_TYPE4);

            if (model.PRODUCT_TYPE5 != null && model.PRODUCT_TYPE5 != "")
                model.PRODUCT_TYPE_SELECTED.Add(model.PRODUCT_TYPE5);

            if (model.PRODUCT_TYPE6 != null && model.PRODUCT_TYPE6 != "")
                model.PRODUCT_TYPE_SELECTED.Add(model.PRODUCT_TYPE6);

            if (model.MATERIAL_TYPE1 != null && model.MATERIAL_TYPE1 != "")
                model.MATERIAL_TYPE_SELECTED.Add(model.MATERIAL_TYPE1);

            if (model.MATERIAL_TYPE2 != null && model.MATERIAL_TYPE2 != "")
                model.MATERIAL_TYPE_SELECTED.Add(model.MATERIAL_TYPE2);

            if (model.MATERIAL_TYPE3 != null && model.MATERIAL_TYPE3 != "")
                model.MATERIAL_TYPE_SELECTED.Add(model.MATERIAL_TYPE3);

            if (model.MATERIAL_TYPE4 != null && model.MATERIAL_TYPE4 != "")
                model.MATERIAL_TYPE_SELECTED.Add(model.MATERIAL_TYPE4);

            if (model.MATERIAL_TYPE5 != null && model.MATERIAL_TYPE5 != "")
                model.MATERIAL_TYPE_SELECTED.Add(model.MATERIAL_TYPE5);

            if (model.MATERIAL_TYPE6 != null && model.MATERIAL_TYPE6 != "")
                model.MATERIAL_TYPE_SELECTED.Add(model.MATERIAL_TYPE6);

            if (model.CATEGORY != null && model.CATEGORY != "")
            {
                foreach (string x in model.CATEGORY.Split(','))
                {
                    model.CATEGORY_SELECTED.Add(x);
                }
            }

            return View(model);
        }

        [SessionExpire]
        [HttpPost]
        public async Task<ActionResult> MM_ProductRegistration_Edit(Product_H_Detail model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.CREATED_BY = username;
                    model.CREATED_DATE = DateTime.Now;
                    model.CREATED_LOC = Request.UserHostAddress;

                    bool success = await dal.PSP_INSERT_VIDEO_CATEGORY(model);

                    if (!success)
                    {
                        return RedirectToAction("Error", "Home");
                    }

                    List<SqlParameter> _pMssql = new List<SqlParameter>();
                    for (int i = 0; i < 6; i++)
                    {
                        if (model.PRODUCT_TYPE_SELECTED != null)
                        {
                            if (i <= model.PRODUCT_TYPE_SELECTED.Count - 1)
                                _pMssql.Add(new SqlParameter("@pProductType" + (i + 1), model.PRODUCT_TYPE_SELECTED[i].ToString()));
                            else
                                _pMssql.Add(new SqlParameter("@pProductType" + (i + 1), ""));
                        }
                        else
                        {
                            _pMssql.Add(new SqlParameter("@pProductType" + (i + 1), ""));
                        }
                    }

                    for (int j = 0; j < 6; j++)
                    {
                        if (model.MATERIAL_TYPE_SELECTED != null)
                        {
                            if (j <= model.MATERIAL_TYPE_SELECTED.Count - 1)
                                _pMssql.Add(new SqlParameter("@pMaterialType" + (j + 1), model.MATERIAL_TYPE_SELECTED[j].ToString()));
                            else
                                _pMssql.Add(new SqlParameter("@pMaterialType" + (j + 1), ""));
                        }
                        else
                        {
                            _pMssql.Add(new SqlParameter("@pMaterialType" + (j + 1), ""));
                        }
                    }
                    _pMssql.Add(new SqlParameter("@pID", model.PRODUCT_H_ID));
                    _pMssql.Add(new SqlParameter("@pDeleteID", model.PRODUCT_H_ID));
                    _pMssql.Add(new SqlParameter("@pReferenceNo", model.REFERENCE_NO));
                    _pMssql.Add(new SqlParameter("@pCategoryStr", model.CATEGORY_SELECTED == null ? "" : string.Join(",", model.CATEGORY_SELECTED)));
                    _pMssql.Add(new SqlParameter("@pRfidTag", model.RFID ?? ""));
                    _pMssql.Add(new SqlParameter("@pIsGarment", model.ISGARMENT));
                    _pMssql.Add(new SqlParameter("@pHangerLoc", model.HANGER_LOC ?? ""));
                    _pMssql.Add(new SqlParameter("@pOwnedBy", model.RUNNING_NO));
                    _pMssql.Add(new SqlParameter("@pRecordTyp", 3));
                    _pMssql.Add(new SqlParameter("@pCreatedBy", username));
                    _pMssql.Add(new SqlParameter("@pCreatedLoc", Request.UserHostAddress));
                    _pMssql.Add(new SqlParameter("returnResult", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });

                    // Procedd image
                    List<string> blobFileList = dal.ProcessImageAsync(model);

                    // Save image to blob storage 
                    HttpContext context = System.Web.HttpContext.Current;
                    Dictionary<string, string> fileUpload = await blob.UploadBobAsync(blobFileList, context);
                    for (int i = 0; i < fileUpload.Count; i++)
                    {
                        var kvp = fileUpload.ElementAt(i);

                        if (!kvp.Value.Contains("_Display_"))
                        {
                            var productImage = new
                            {
                                p_HID = model.PRODUCT_H_ID,
                                pReferenceNo = model.REFERENCE_NO,
                                pColorWay = HttpUtility.UrlDecode(kvp.Value.Split('/')[kvp.Value.Split('/').Length - 2]),
                                pImagePath = kvp.Value,
                                pRecordTyp = 3,
                                pCreatedBy = username,
                                pCreatedLoc = Request.UserHostAddress,
                                returnResult = ""
                            };
                            await common.PSP_COMMON_DAPPER_SINGLE<string>("PSP_FABRIC_PRODUCT_IMAGE_MAINT", CommandType.StoredProcedure, productImage);
                        }
                    }

                    // Delete the folder after submit
                    dal.DeleteFolder(model.REFERENCE_NO);

                    string result = await common.PSP_COMMON_SQL("PSP_MM_PRODUCT_DETAIL_MAINT", CommandType.StoredProcedure, _pMssql, "", "", true);

                    ViewBag.Result = "Success";
                    ViewBag.NewID = result;
                }

                model.imageList = JsonConvert.SerializeObject(await dal.GetImage(model.PRODUCT_H_ID));

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
        public async Task<ActionResult> MM_ProductRegistration_DeleteListing(string[] deleteID)
        {
            ACL_UserObj aclObj = (Session["AclUser"] as ACL_UserObj);
            string deleteIDStr = string.Join(",", deleteID);
            Product_H_Detail model = new Product_H_Detail();

            List<SqlParameter> _pMssql = new List<SqlParameter>();
            for (int i = 0; i < 6; i++)
            {
                if (model.PRODUCT_TYPE_SELECTED != null)
                {
                    if (i <= model.PRODUCT_TYPE_SELECTED.Count - 1)
                    {
                        _pMssql.Add(new SqlParameter("@pProductType" + (i + 1), model.PRODUCT_TYPE_SELECTED[i].ToString()));
                    }
                    else
                    {
                        _pMssql.Add(new SqlParameter("@pProductType" + (i + 1), ""));
                    }
                }
                else
                {
                    _pMssql.Add(new SqlParameter("@pProductType" + (i + 1), ""));
                }
            }

            for (int j = 0; j < 6; j++)
            {
                if (model.MATERIAL_TYPE_SELECTED != null)
                {
                    if (j <= model.MATERIAL_TYPE_SELECTED.Count - 1)
                    {
                        _pMssql.Add(new SqlParameter("@pMaterialType" + (j + 1), model.MATERIAL_TYPE_SELECTED[j].ToString()));
                    }
                    else
                    {
                        _pMssql.Add(new SqlParameter("@pMaterialType" + (j + 1), ""));
                    }
                }
                else
                {
                    _pMssql.Add(new SqlParameter("@pMaterialType" + (j + 1), ""));
                }
            }

            _pMssql.Add(new SqlParameter("@pID", ""));
            _pMssql.Add(new SqlParameter("@pDeleteID", deleteIDStr));
            _pMssql.Add(new SqlParameter("@pReferenceNo", ""));
            _pMssql.Add(new SqlParameter("@pCategoryStr", ""));
            _pMssql.Add(new SqlParameter("@pRfidTag", ""));
            _pMssql.Add(new SqlParameter("@pHangerLoc", ""));
            _pMssql.Add(new SqlParameter("@pOwnedBy", ""));
            _pMssql.Add(new SqlParameter("@pRecordTyp", '5'));
            _pMssql.Add(new SqlParameter("@pCreatedBy", aclObj.EMP_NO + " - " + aclObj.EMP_NAME));
            _pMssql.Add(new SqlParameter("@pCreatedLoc", Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString()));
            _pMssql.Add(new SqlParameter("returnResult", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });

            string result = await common.PSP_COMMON_SQL("PSP_MM_PRODUCT_DETAIL_MAINT", CommandType.StoredProcedure, _pMssql, "", "", true);
            string isSuccessStr = "";
            if (result == "Success")
            {
                isSuccessStr = "Success";
            }
            else
            {
                isSuccessStr = "Failed";
            }

            return Json(new { isSuccess = isSuccessStr });
        }

        [SessionExpire]
        public async Task<ActionResult> MM_ProductRegistration_EditCategory(string editID)
        {
            Product_H_Array model = new Product_H_Array();
            model.Product_H_Detail_Category = new List<Product_H_Detail_Category>();
            List<SelectListItem> categoryDdl = new List<SelectListItem>();
            categoryDdl = await dal.PSP_GET_DDL2("MM_CATEGORY_H", "CATEGORY_H_ID", "CATEGORY_NAME");

            var obj = new { pID = editID };
            model.Product_H_Detail_Category = await common.PSP_COMMON_DAPPER<Product_H_Detail_Category>("PSP_GET_MM_PRODUCT_CATEGORY_DETAIL", CommandType.StoredProcedure, obj);

            if (model.Product_H_Detail_Category != null)
            {
                for (int i = 0; i < model.Product_H_Detail_Category.Count; i++)
                {
                    model.Product_H_Detail_Category[i].CATEGORY_DDL = categoryDdl;
                    if (model.Product_H_Detail_Category[i].CATEGORY != null)
                    {
                        model.Product_H_Detail_Category[i].CATEGORY_SELECTED = new List<string>(model.Product_H_Detail_Category[i].CATEGORY.Split(','));
                    }
                }
            }

            ViewBag.CategoryNum = categoryDdl.Count;
            ViewBag.ProductNum = model.Product_H_Detail_Category.Count;

            return View(model);
        }

        [SessionExpire]
        [HttpPost]
        public async Task<ActionResult> MM_ProductRegistration_EditCategory(Product_H_Array model)
        {
            ACL_UserObj aclObj = (Session["AclUser"] as ACL_UserObj);
            List<SqlParameter> _pMssql = new List<SqlParameter>();

            for (int i = 0; i < model.Product_H_Detail_Category.Count; i++)
            {
                _pMssql.Add(new SqlParameter("@pID", model.Product_H_Detail_Category[i].PRODUCT_H_ID));
                _pMssql.Add(new SqlParameter("@pCategoryStr", model.Product_H_Detail_Category[i].CATEGORY_SELECTED != null ? string.Join(",", model.Product_H_Detail_Category[i].CATEGORY_SELECTED) : ""));
                _pMssql.Add(new SqlParameter("@pCreatedBy", aclObj.EMP_NO + " - " + aclObj.EMP_NAME));
                _pMssql.Add(new SqlParameter("@pCreatedLoc", Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString()));
                _pMssql.Add(new SqlParameter("returnResult", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });

                string result = await common.PSP_COMMON_SQL("PSP_MM_PRODUCT_CATEGORY_DETAIL_MAINT", CommandType.StoredProcedure, _pMssql, "", "", true);

                ViewBag.Result = result;
                _pMssql.Clear();
            }

            List<SelectListItem> categoryDdl = new List<SelectListItem>();
            categoryDdl = await dal.PSP_GET_DDL2("MM_CATEGORY_H", "CATEGORY_H_ID", "CATEGORY_NAME");

            //var obj = new { pID = editID };
            //model.Product_H_Detail_Category = await common.PSP_COMMON_DAPPER<Product_H_Detail_Category>("PSP_GET_MM_PRODUCT_CATEGORY_DETAIL", CommandType.StoredProcedure, obj);

            if (model.Product_H_Detail_Category != null)
            {
                for (int i = 0; i < model.Product_H_Detail_Category.Count; i++)
                {
                    model.Product_H_Detail_Category[i].CATEGORY_DDL = categoryDdl;
                    if (model.Product_H_Detail_Category[i].CATEGORY != null)
                    {
                        model.Product_H_Detail_Category[i].CATEGORY_SELECTED = new List<string>(model.Product_H_Detail_Category[i].CATEGORY.Split(','));
                    }
                }
            }

            ViewBag.CategoryNum = categoryDdl.Count;
            ViewBag.ProductNum = model.Product_H_Detail_Category.Count;

            return View(model);
        }

        [SessionExpire]
        public async Task<ActionResult> MM_ProductRegistration_AuditTrail(int id)
        {
            AuditTrailModel auditTrailModel = new AuditTrailModel();

            List<SqlParameter> _pMssql = new List<SqlParameter>();
            _pMssql.Add(new SqlParameter("@TABLE", "MM_PRODUCT_H"));
            _pMssql.Add(new SqlParameter("@KEY_VALUE", id));
            _pMssql.Add(new SqlParameter("@SortColumn", "UPDATED_DATE"));
            _pMssql.Add(new SqlParameter("@SortType", "DESC"));
            auditTrailModel = await AuditTrailHelper.AuditTrailStoreProcedureSqlAsync("PSP_GET_AUDIT_TRAIL", CommandType.StoredProcedure, _pMssql, "PAB_NEW_AQUA");


            ViewBag.JsonResult = auditTrailModel.JsonData;
            ViewBag.KeyNames = auditTrailModel.ListData;
            ViewBag.PRODUCT_H_ID = id;

            return View();
        }

        #region Ajax Function
        [HttpPost]
        public async Task<ActionResult> GetProductListAsync()
        {
            try
            {
                var obj = new { pRole = AclUser.ROLE_NAME };
                List<Product_H_List> model = await common.PSP_COMMON_DAPPER<Product_H_List>("PSP_GET_MM_PRODUCT_LIST", CommandType.StoredProcedure, obj);

                return Json(new { data = model });
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return Json(new { data = new List<Product_H_List>() });
            }
        }

        public async Task<JsonResult> GetProductDetailAjaxAsync(string referenceNo)
        {
            var obj = new { pID = 0, pReferenceNo = referenceNo, pIsEdit = "" };
            Product_H_Detail model = await common.PSP_COMMON_DAPPER_SINGLE<Product_H_Detail>("PSP_GET_MM_PRODUCT_DETAIL", CommandType.StoredProcedure, obj);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public string SaveImageToTemp(string imageData, string referenceNo, string colorWay, string imageName)
        {
            if (imageData != null)
            {
                try
                {
                    Match match = Regex.Match(imageData, @"data:(image/\w+);base64,(.+)");
                    if (match.Success)
                    {
                        string parentFolderPath = AppDomain.CurrentDomain.BaseDirectory + "\\TempImage"; // Specify the path to the parent folder
                        string newFolderName = referenceNo; // Name of the new folder

                        // Combine the parent folder path and the new folder name
                        string newFolderPath = Path.Combine(parentFolderPath, newFolderName);

                        if (!Directory.Exists(parentFolderPath))
                        {
                            return "Something Wrong Happen.";
                        }

                        // Check if the new folder already exists
                        if (!Directory.Exists(newFolderPath))
                        {
                            // Create the new folder
                            Directory.CreateDirectory(newFolderPath);
                        }

                        string anotherNewFolderPath = Path.Combine(newFolderPath, colorWay);

                        if (!Directory.Exists(anotherNewFolderPath))
                        {
                            Directory.CreateDirectory(anotherNewFolderPath);
                        }
                        string base64Data = match.Groups[2].Value;
                        byte[] imageDataByte = Convert.FromBase64String(base64Data);
                        string fileName = imageName.Replace('~', ' '); // Specify the file name
                        string path = Server.MapPath("~/TempImage/" + referenceNo + "/" + colorWay.Trim() + "/" + fileName);
                        System.IO.File.WriteAllBytes(path, imageDataByte);
                        return "Success";

                    }
                    else
                    {
                        return "Something Wrong Happen.";
                    }
                }
                catch (Exception)
                {
                    return "Something Wrong Happen.";
                }
            }
            else
            {
                return "No Image Uploaded.";
            }

        }

        public async Task<string> DeleteImageFromTemp(string referenceNo, string colorWay, string imageName, string url, string id)
        {
            if (referenceNo != null)
            {
                try
                {
                    if (url.Substring(0, 4) == "blob")
                    {
                        char[] charactersToReplace = { '~' };
                        string parentFolderPath = AppDomain.CurrentDomain.BaseDirectory + "\\TempImage"; // Specify the path to the parent folder

                        foreach (char character in charactersToReplace)
                        {
                            colorWay = colorWay.Replace(character, ' ');
                        }
                        // Combine the parent folder path and the new folder name
                        string newFolderPath = Path.Combine(parentFolderPath, referenceNo, colorWay, imageName.Replace('~', ' '));

                        if (!Directory.Exists(parentFolderPath))
                        {
                            return "Something Wrong Happen.";
                        }

                        string path = Server.MapPath("~/TempImage/" + referenceNo + "/" + colorWay + "/" + imageName.Replace('~', ' '));
                        System.IO.File.Delete(path);
                        return "Success";
                    }
                    else
                    {
                        string removeResult = await blob.DeleteBlobAsync(url);
                        var obj = new
                        {
                            pReferenceNo = referenceNo,
                            pColorWay = "",
                            pImagePath = "",
                            pRecordTyp = 5,
                            pCreatedBy = username,
                            pCreatedLoc = Request.UserHostAddress,
                            pIsBlob = id,
                            returnResult = ""
                        };
                        await common.PSP_COMMON_DAPPER_SINGLE<string>("PSP_FABRIC_PRODUCT_IMAGE_MAINT", CommandType.StoredProcedure, obj);

                        return "Success";
                    }


                }
                catch (Exception)
                {
                    return "Something Wrong Happen.";
                }
            }
            else
            {
                return "No Image Uploaded.";
            }
        }

        public async Task<JsonResult> MM_ProductRegistration_ProductActDeact(string type, string[] id)
        {
            ACL_UserObj aclObj = (Session["AclUser"] as ACL_UserObj);
            string IDStr = string.Join(",", id);
            Product_H_Detail model = new Product_H_Detail();

            List<SqlParameter> _pMssql = new List<SqlParameter>();
            _pMssql.Add(new SqlParameter("@pID", IDStr));
            _pMssql.Add(new SqlParameter("@pType", type));
            _pMssql.Add(new SqlParameter("@pRecordTyp", '3'));
            _pMssql.Add(new SqlParameter("@pCreatedBy", aclObj.EMP_NO + " - " + aclObj.EMP_NAME));
            _pMssql.Add(new SqlParameter("@pCreatedLoc", Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString()));
            _pMssql.Add(new SqlParameter("returnResult", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });

            string result = await common.PSP_COMMON_SQL("PSP_MM_PRODUCT_ACTIVATION", CommandType.StoredProcedure, _pMssql, "", "", true);
            string isSuccessStr = "";
            if (result == "Success")
            {
                isSuccessStr = "Success";
            }
            else
            {
                isSuccessStr = "Failed";
            }

            return Json(new { isSuccess = isSuccessStr });
        }
        #endregion
    }
}