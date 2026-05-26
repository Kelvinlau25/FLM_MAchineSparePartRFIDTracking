using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using PAB_NewAquarium.DAL;
using PAB_NewAquarium.Filters;
using PAB_NewAquarium.Helpers;
using PAB_NewAquarium.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Reflection;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using Microsoft.IdentityModel.Logging;

namespace PAB_NewAquarium.Controllers
{
    public class ProductDetailController : BaseController
    {
        readonly MM_ProductRegistration_DAL productReg = new MM_ProductRegistration_DAL();
        readonly MM_ProductDetail_DAL productDtl = new MM_ProductDetail_DAL();
        readonly BlobHelper blobHelper = new BlobHelper();

        [SessionExpire]
        public async Task<ActionResult> ProductDetail(int id, int groupDID)
        {
            try
            {
                List<SqlParameter> _pMssql = new List<SqlParameter>();
                _pMssql.Add(new SqlParameter("@pID", id));
                _pMssql.Add(new SqlParameter("@pGroupDID", groupDID));
                _pMssql.Add(new SqlParameter("@pCreatedBy", username));
                _pMssql.Add(new SqlParameter("returnResult", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });

                string result = await common.PSP_COMMON_SQL("PSP_INSERT_BROWSING_DETAILS", CommandType.StoredProcedure, _pMssql, "", "", true);

                ProductDetailPageModel model = new ProductDetailPageModel();
                model.GROUP_D_ID = groupDID;
                model.Product_H_Detail = await productDtl.GetProductDetailAsync(id);
                model.Product_H_Detail.imageList = JsonConvert.SerializeObject(await productDtl.GetImageHiRes(id));
                model.FabricVideoList = await productDtl.PSP_GET_VIDEO_BY_CATEGORY(model.Product_H_Detail.PRODUCT_H_ID);
                model.Product_H_Detail.CATEGORY = model.Product_H_Detail.CATEGORY.Split(',')[0];
                model.Product_H_Detail.ITEM_CODE = model.Product_H_Detail.CHOP_NO + " - " + model.Product_H_Detail.FINISH_TYPE1.Replace(", ", " - ");
                model.ProductCommentList = await productDtl.GetProductCommentAsync(id);

                var obj = new { pCreatedBy = username, pCurrentProduct = id };
                model.RecentViewList = await common.PSP_COMMON_DAPPER<RecentView>("PSP_GET_SALES_RECENT_VIEW_PRODUCTS", CommandType.StoredProcedure, obj);

                foreach (var item in model.RecentViewList)
                {
                    string[] split = item.IMAGE_URL.Split('/');
                    string imageName = split[split.Length - 1];
                    item.IMAGE_URL = item.IMAGE_URL.Replace(imageName, $"_Display_{imageName}");
                    item.IMAGE_URL = await blobHelper.SetBlobUrlToken(item.IMAGE_URL);
                }

                ViewBag.CompanyName = new SelectList(await productReg.PSP_GET_DDL2("MM_CUSTOMER_GROUP_H", "GROUP_H_ID", "COMPANY_NAME"), "Value", "Text");
                ViewBag.CommentSuggestion = await common.PSP_COMMON_DAPPER<string>("PSP_GET_KEYWORD", CommandType.StoredProcedure);

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
        public async Task<ActionResult> ProductDetail(Product_H_Detail model)
        {
            try
            {
                model.imageList = JsonConvert.SerializeObject(productDtl.GetImageHiRes(model.PRODUCT_H_ID));
                ViewBag.CompanyName = new SelectList(await productReg.PSP_GET_DDL2("MM_CUSTOMER_GROUP_H", "GROUP_H_ID", "COMPANY_NAME"), "Value", "Text");
                ViewBag.CommentSuggestion = await common.PSP_COMMON_DAPPER<string>("PSP_GET_KEYWORD", CommandType.StoredProcedure);
                return View(model);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return RedirectToAction("Error", "Home");
            }
        }

        [SessionExpire]
        public async Task<ActionResult> CustomerReviewFeedback(int id)
        {
            try
            {
                ProductCommentList model = new ProductCommentList();
                model.ProductListComment = await productDtl.GetProductCommentsAsync(id);
                ViewBag.ProductHID = id;
                ViewBag.CommentSuggestion = await common.PSP_COMMON_DAPPER<string>("PSP_GET_KEYWORD", CommandType.StoredProcedure);
                ViewBag.AllComments = JsonConvert.SerializeObject(model.ProductListComment);
                return View(model);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return RedirectToAction("Error", "Home");
            }
        }

        [SessionExpire]
        public async Task<ActionResult> Cart(string id = "")
        {
            try
            {
                var param = new
                {
                    pID = id == "" ? AclUser.EMP_NO : id,
                    pLogin = id == "" ? "Sales" : "Customer"
                };

                AddToCartModel model = new AddToCartModel();
                model.CART = await common.PSP_COMMON_DAPPER<CartModel>("PSP_GET_CART_ITEM", CommandType.StoredProcedure, param);
                model.GROUP_CART = model.CART.GroupBy(c => c.COMPANY_NAME)
                                             .Select(group => new GroupCartModel
                                             {
                                                 COMPANY_NAME = group.Key,
                                                 ITEM = group.ToList()
                                             }).ToList();

                foreach (var item in model.CART)
                {
                    item.IMAGE_URL = await blobHelper.SetBlobUrlToken(item.IMAGE_URL);
                }

                ViewBag.ID = id;
                ViewBag.Username = AclUser.EMP_NAME;

                return View(model);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return RedirectToAction("Error", "Home");
            }
        }


        #region Phase 2 Function
        /* Created By JiaYan for phase 2
         * Function: User can edit or cancel the adhoc request
        */
        public async Task<ActionResult> AdHocRequestEdit(string requestNo)
        {
            try
            {
                var param = new { pSampleRequestNo = requestNo };
                AddHocRequestModel model = await common.PSP_COMMON_DAPPER_SINGLE<AddHocRequestModel>("PSP_GET_ADHOC_REQUEST_H", CommandType.StoredProcedure, param);
                model.Details = await common.PSP_COMMON_DAPPER<AddHocRequestDetail>("PSP_GET_ADHOC_REQUEST_D", CommandType.StoredProcedure, param);
                model.SENT_BY = AclUser.EMP_NAME;

                return View(model);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public async Task<ActionResult> AdHocRequestEdit(AddHocRequestModel model)
        {
            try
            {
                model.RECORD_TYP = 3;
                model.CHARGE_BY = model.CHARGE == "No Charge" ? string.Empty : model.CHARGE_BY;
                model.CREATED_BY = AclUser.EMP_NAME.ToUpper();
                model.CREATED_LOC = Request.UserHostAddress;
                model.SALES_REGION = await productDtl.GetSalesRegion(AclUser.USER_ID, AclUser.COMPANY);

                bool success = await productDtl.PSP_UPDATE_ADHOC_REQUEST_EDIT(model);

                // After edit, send email to user with latest adhoc request PDF
                if (success)
                {
                    ExportPDf.ExportPDf.AjaxExportPdf generatePdf = new ExportPDf.ExportPDf.AjaxExportPdf();
                    string returnFileName = generatePdf.AjaxExportPdf("SAMPLE_REQUEST_NO", model.SAMPLE_REQUEST_NO, "0", "SPL_AdHocRequest", "connPF");

                    await productDtl.SendEmailAsync(returnFileName, model.SAMPLE_REQUEST_NO, AclUser);
                }

                ViewBag.RETURN_MESSAGE = success ? "Success" : "Error";

                return View(model);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public async Task<ActionResult> AdHocRequestCancel(string requestNo)
        {
            try
            {
                AddHocRequestModel model = new AddHocRequestModel();
                model.SAMPLE_REQUEST_NO = requestNo;
                model.RECORD_TYP = 5;
                model.CREATED_BY = AclUser.EMP_NAME.ToUpper();
                model.CREATED_LOC = Request.UserHostAddress;

                bool success = await productDtl.PSP_UPDATE_ADHOC_REQUEST_CANCEL(model);

                return Json(new { success });
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return Json(new { success = false });
            }
        }
        #endregion

        #region Ajax Function
        [HttpPost]
        public async Task<string> SubmitFeedback(int id, string colorWay, int companyID, string customerName, string[] commentSuggestArr, string comment)
        {
            string result = "";

            try
            {
                ACL_UserObj aclObj = (Session["AclUser"] as ACL_UserObj);
                List<SqlParameter> _pMssql = new List<SqlParameter>();
                _pMssql.Add(new SqlParameter("@pID", id));
                _pMssql.Add(new SqlParameter("@pColorWay", colorWay));
                _pMssql.Add(new SqlParameter("@pCompanyID", companyID));
                _pMssql.Add(new SqlParameter("@pCustomerName", customerName));
                _pMssql.Add(new SqlParameter("@pComment", comment));
                _pMssql.Add(new SqlParameter("@pCreatedBy", aclObj.EMP_NO + " - " + aclObj.EMP_NAME));
                _pMssql.Add(new SqlParameter("@pCreatedLoc", Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString()));
                _pMssql.Add(new SqlParameter("returnResult", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });

                string feedbackHID = await common.PSP_COMMON_SQL("PSP_SUBMIT_COMMENT", CommandType.StoredProcedure, _pMssql, "", "", true);


                for (int i = 0; i < commentSuggestArr.Count(); i++)
                {
                    List<SqlParameter> _pMssql2 = new List<SqlParameter>();
                    _pMssql2.Add(new SqlParameter("@pFeedbackHID", feedbackHID));
                    _pMssql2.Add(new SqlParameter("@pKeyword", commentSuggestArr[i]));
                    _pMssql2.Add(new SqlParameter("@pCreatedBy", aclObj.EMP_NO + " - " + aclObj.EMP_NAME));
                    _pMssql2.Add(new SqlParameter("@pCreatedLoc", Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString()));
                    _pMssql2.Add(new SqlParameter("returnResult", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });

                    result = await common.PSP_COMMON_SQL("PSP_SUBMIT_COMMENT_KEYWORD", CommandType.StoredProcedure, _pMssql2, "", "", true);

                }

            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
            }

            return result;
        }

        [HttpPost]
        public async Task<ActionResult> AddToCart(int customerID, int productID, string colorway)
        {
            try
            {
                ACL_UserObj aclObj = (Session["AclUser"] as ACL_UserObj);
                List<SqlParameter> _pMssql = new List<SqlParameter>();
                _pMssql.Add(new SqlParameter("@pCustomerID", customerID));
                _pMssql.Add(new SqlParameter("@pSalesID", aclObj.EMP_NO));
                _pMssql.Add(new SqlParameter("@pProductID", productID));
                _pMssql.Add(new SqlParameter("@pColorWay", colorway));
                _pMssql.Add(new SqlParameter("@pRecordTyp", 1));
                _pMssql.Add(new SqlParameter("@pCreatedBy", aclObj.EMP_NO + " - " + aclObj.EMP_NAME));
                _pMssql.Add(new SqlParameter("@pCreatedDate", DateTime.Now));
                _pMssql.Add(new SqlParameter("@pCreatedLoc", Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString()));
                _pMssql.Add(new SqlParameter("returnResult", SqlDbType.VarChar, 10) { Direction = ParameterDirection.Output });
                string result = await common.PSP_COMMON_SQL("PSP_INSERT_ADD_TO_CART", CommandType.StoredProcedure, _pMssql, "", "", true);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> UpdateCart(int cartID, string action)
        {
            try
            {
                var param = new { pCartID = cartID, pAction = action };
                await common.PSP_COMMON_DAPPER_SINGLE<bool>("PSP_UPDATE_CART", CommandType.StoredProcedure, param);

                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                throw;
            }
        }

        public async Task<ActionResult> GetSimilarityProducts(int id, int groupDID)
        {
            try
            {
                HttpClientHandler httpClientHandler = await proxy.GetHttpClientHandler();
                var param1 = new { INDICATOR = "PMatrixAPIKey" };
                string apiKey = await common.PSP_COMMON_DAPPER_SINGLE<string>("PSP_GET_MASTER_SETTING_VALUE2", CommandType.StoredProcedure, param1);
                string apiUrl = ConfigurationManager.AppSettings["MATRIX_COMPARISON_API"];

                List<SimilarityInputModel> similarityInput = new List<SimilarityInputModel>();
                SimilarityInputModel data = new SimilarityInputModel
                {
                    FABRIC_TYPE = 1,
                    WEAVE_TYPE = 1,
                    WARP_COUNT1 = 1,
                    WEFT_COUNT1 = 1,
                    COMPOSITION = 1
                };
                similarityInput.Add(data);

                var obj = new { pID = id, pUser = (groupDID == 0) ? "" : "customer" };
                List<SimilarityCompareModel> similarityCompare = await common.PSP_COMMON_DAPPER<SimilarityCompareModel>("PSP_MATRIX_COMPARISON", CommandType.StoredProcedure, obj);

                SimilarityRequestModel similarity = new SimilarityRequestModel
                {
                    api_key = "TMS-gt08N0baWAroammL",
                    input = similarityInput,
                    compareList = similarityCompare
                };
                var json = JsonConvert.SerializeObject(similarity);
                var buffer = Encoding.UTF8.GetBytes(json);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpClient client = new HttpClient(handler: httpClientHandler, disposeHandler: true);
                HttpResponseMessage response = client.PostAsync(apiUrl, byteContent).Result;
                var resp = await response.Content.ReadAsStringAsync();
                SimilarityResponseModel respObj = JsonConvert.DeserializeObject<SimilarityResponseModel>(resp);
                respObj.similarityList = respObj.similarityList.Where(arr => arr[0].ToString() != id.ToString()).Select(arr => arr).OrderByDescending(arr => (double)arr[1]).Take(8).ToList();
                var obj2 = new { pid = string.Join(",", respObj.similarityList.Select(arr => arr[0].ToString())), pType = "ALL" };
                List<SimilarProduct> model = await common.PSP_COMMON_DAPPER<SimilarProduct>("PSP_GET_RECOMMENDED_PRODUCT", CommandType.StoredProcedure, obj2);

                foreach (var item in model)
                {
                    string[] split = item.IMAGE_URL.Split('/');
                    string imageName = split[split.Length - 1];
                    item.IMAGE_URL = item.IMAGE_URL.Replace(imageName, $"_Display_{imageName}");
                    item.IMAGE_URL = await blobHelper.SetBlobUrlToken(item.IMAGE_URL);
                }

                if (model.Count == 0)
                {
                    return Json("Empty");
                }
                return PartialView("_Similar_Product", model);
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }
        }

        public async Task<JsonResult> GetCustomerName(int id)
        {
            try
            {
                List<string> model = new List<string>();
                if (id != 0)
                {
                    var obj = new { pID = id };
                    model = await common.PSP_COMMON_DAPPER<string>("PSP_GET_CUSTOMER_NAME_LIST", CommandType.StoredProcedure, obj);
                }

                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return Json(new List<string>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> GetCustomerDetailAsync(string id)
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
                return Json(cust, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return Json(new CustomerDetail(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> GetCustomersListAsync()
        {
            try
            {
                List<CustomerDetail> custList = new List<CustomerDetail>();
                List<OracleParameter> _pMssql = new List<OracleParameter>();
                OracleParameter resultCursor = new OracleParameter
                {
                    ParameterName = "v_output",
                    Direction = ParameterDirection.Output,
                    OracleDbType = OracleDbType.RefCursor
                };
                _pMssql.Add(resultCursor);

                DataTable dt = await common.PSP_COMMON_ORA("PSP_GET_CUSTOMER_DETAILS_LIST", CommandType.StoredProcedure, _pMssql, null, "PAB_SALES_SQL");
                foreach (DataRow row in dt.Rows)
                {
                    CustomerDetail cust = new CustomerDetail();
                    cust.CUST_CODE = Convert.ToString(row["CUST_CODE"]);
                    cust.CUST_NAME = Convert.ToString(row["CUST_NAME"]);
                    cust.CUST_ADD1 = Convert.ToString(row["CUST_ADDR1"]);
                    cust.CUST_ADD2 = Convert.ToString(row["CUST_ADDR2"]);
                    cust.CUST_ADD3 = Convert.ToString(row["CUST_ADDR3"]);
                    cust.CUST_ADD4 = Convert.ToString(row["CUST_ADDR4"]);
                    cust.STATUS = Convert.ToString(row["STATUS"]);

                    custList.Add(cust);
                }
                return Json(custList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return Json(new List<CustomerDetail>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> GetDraftDataAsync(int id)
        {
            try
            {
                object obj = new
                {
                    pCART_H_ID = id,
                    pACTION = "GET"
                };
                DraftCartModel model = await common.PSP_COMMON_DAPPER_SINGLE<DraftCartModel>("PSP_DRAFT_CART_ITEM_MAINT", CommandType.StoredProcedure, obj);

                return Json(new { success = true, data = model });
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return Json(new { success = false });
            }
        }

        [HttpPost]
        public async Task<ActionResult> SaveAsDraftAsync(DraftCartModel model)
        {
            try
            {
                object obj = new
                {
                    pCART_H_ID = model.CART_H_ID,
                    pACTION = "CREATE",
                    pATTN_TO = model.ATTN_TO ?? string.Empty,
                    pREMARK = model.REMARK ?? string.Empty,
                    pCUST_CODE = model.CUST_CODE ?? string.Empty,
                    pCHARGE = model.CHARGE ?? string.Empty,
                    pCHARGE_BY = model.CHARGE_BY ?? string.Empty,
                    pCREATED_BY = username,
                    pLOC = Request.UserHostAddress,
                };

                bool success = await common.PSP_COMMON_DAPPER_SINGLE<bool>("PSP_DRAFT_CART_ITEM_MAINT", CommandType.StoredProcedure, obj);

                return Json(new { success });
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return Json(new { success = false });
            }
        }

        [HttpPost]
        public async Task<ActionResult> GetSampleRollAsync(string id, string requestQty)
        {
            try
            {
                List<SampleRoll> sampleRollList = new List<SampleRoll>();
                SampleAdHocViewModel adHocVM = new SampleAdHocViewModel();
                SampleAdHocMaster adHocMaster = new SampleAdHocMaster();
                string[] cartID = id.Split(',');
                string[] qty = requestQty.Split(',');
                string singleCartID = "";

                DataTable dt = new DataTable();

                for (int i = 0; i < cartID.Length; i++)
                {
                    List<SqlParameter> _pMssql = new List<SqlParameter>();
                    singleCartID = cartID[i];
                    _pMssql.Add(new SqlParameter("@pCartID", singleCartID));
                    dt = await common.PSP_COMMON_SQL("PSP_GET_SAMPLE_ROLL_DETAILS", CommandType.StoredProcedure, _pMssql, "", "", false);

                    foreach (DataRow row in dt.Rows)
                    {
                        SampleRoll sampleRoll = new SampleRoll();

                        sampleRoll.CART_D_ID = Convert.ToInt32(cartID[i]);
                        sampleRoll.PO_NO = Convert.ToString(row["PO_No"]);
                        sampleRoll.COLOR_WAY = Convert.ToString(row["Color_Desc"]);
                        sampleRoll.SAMPLE_ROLL_NO = Convert.ToString(row["Sample_Roll_No"]);
                        sampleRoll.LOCATION = Convert.ToString(row["Location_ID"]);
                        sampleRoll.BALANCE_YARDAGE = Convert.ToInt32(row["Current_Qty"]);
                        sampleRoll.UOM = Convert.ToString(row["UOM"]);
                        sampleRoll.REQUEST_QUANTITY = Convert.ToInt32(qty[i]);
                        sampleRoll.CHOP_NO = Convert.ToString(row["Chop_No"]);
                        sampleRoll.COMPOSITION = Convert.ToString(row["Composition"]);
                        sampleRoll.FINISH_TYPE1 = Convert.ToString(row["FINISH_TYPE1"]);
                        sampleRoll.FINISH_TYPE2 = Convert.ToString(row["FINISH_TYPE2"]);
                        sampleRoll.FINISH_TYPE3 = Convert.ToString(row["FINISH_TYPE3"]);
                        sampleRoll.FINISH_TYPE4 = Convert.ToString(row["FINISH_TYPE4"]);
                        sampleRoll.WARP_DENSITY = Convert.ToString(row["WARP_DENSITY"]);
                        sampleRoll.WARP_DENSITY2 = Convert.ToString(row["WARP_DENSITY2"]);
                        sampleRoll.WEFT_DENSITY = Convert.ToString(row["WEFT_DENSITY"]);
                        sampleRoll.WEFT_DENSITY2 = Convert.ToString(row["WEFT_DENSITY2"]);
                        sampleRoll.WARP_COUNT = Convert.ToString(row["WARP_COUNT"]);
                        sampleRoll.WARP_COUNT2 = Convert.ToString(row["WARP_COUNT2"]);
                        sampleRoll.WARP_COUNT3 = Convert.ToString(row["WARP_COUNT3"]);
                        sampleRoll.WEFT_COUNT = Convert.ToString(row["WEFT_COUNT"]);
                        sampleRoll.WEFT_COUNT2 = Convert.ToString(row["WEFT_COUNT2"]);
                        sampleRoll.WEFT_COUNT3 = Convert.ToString(row["WEFT_COUNT3"]);
                        sampleRollList.Add(sampleRoll);
                    }
                }
                adHocVM.SampleAdHocMaster = adHocMaster;
                adHocVM.sampleRolls = sampleRollList;

                return Json(new { success = true, data = adHocVM });
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return Json(new { success = false });
            }
        }

        [HttpPost]
        public async Task<ActionResult> UpdateRequest(List<CartDUpdate> cartDArray, List<AdHocRequestDetails> adHocRequestDetailArray, SampleAdHocMaster sampleAdHocMaster, string cartIDStr)
        {
            try
            {
                string dateFormat = "dd/MM/yyyy";
                string isTest = ConfigurationManager.AppSettings["isTest"].ToUpper();
                string constr = (isTest == "TRUE") ? "PAB_SampleInv_Test" : "PAB_SampleInv";
                string newSRNo = await common.PSP_COMMON_DAPPER_SINGLE<string>("psp_SAMPLE_ADHOC_NewSRNo", CommandType.StoredProcedure, null, constr);
                string salesRegion = "";
                List<OracleParameter> _pMssql = new List<OracleParameter>();
                _pMssql.Add(new OracleParameter("puserid", AclUser.USER_ID));
                _pMssql.Add(new OracleParameter("pcoid", AclUser.COMPANY));
                OracleParameter resultCursor = new OracleParameter
                {
                    ParameterName = "SREFData",
                    Direction = ParameterDirection.Output,
                    OracleDbType = OracleDbType.RefCursor
                };
                _pMssql.Add(resultCursor);

                //get sales region
                DataTable dt = await common.PSP_COMMON_ORA("SP_GET_USER_DEPT", CommandType.StoredProcedure, _pMssql, null, "PAB_ACL_NET");
                foreach (DataRow row in dt.Rows)
                {
                    salesRegion = Convert.ToString(row["Region"]);

                    break;
                }

                //update cart quantity
                foreach (var x in cartDArray)
                {
                    List<SqlParameter> _pMssql2 = new List<SqlParameter>();
                    _pMssql2.Add(new SqlParameter("@pCartID", x.CART_D_ID));
                    _pMssql2.Add(new SqlParameter("@pQuantity", x.QUANTITY));
                    await common.PSP_COMMON_SQL("PSP_UPDATE_CART_AD_HOC", CommandType.StoredProcedure, _pMssql2, "", "", false);
                }

                //submit ad hoc request (insert customer info to SAMPLE_ADHOC_MASTER)
                List<SqlParameter> _pMssql4 = new List<SqlParameter>();
                _pMssql4.Add(new SqlParameter("@Rec_Type", '1'));
                _pMssql4.Add(new SqlParameter("@Sample_Request_No", newSRNo == null ? "" : newSRNo));
                _pMssql4.Add(new SqlParameter("@Sales_Region", salesRegion == null ? "" : salesRegion));
                _pMssql4.Add(new SqlParameter("@Cust_Code", sampleAdHocMaster.CUST_CODE == null ? "" : sampleAdHocMaster.CUST_CODE));
                _pMssql4.Add(new SqlParameter("@Tel_No", sampleAdHocMaster.TEL_NO == null ? "" : sampleAdHocMaster.TEL_NO));
                _pMssql4.Add(new SqlParameter("@Addr1", sampleAdHocMaster.ADDR1 == null ? "" : sampleAdHocMaster.ADDR1));
                _pMssql4.Add(new SqlParameter("@Addr2", sampleAdHocMaster.ADDR2 == null ? "" : sampleAdHocMaster.ADDR2));
                _pMssql4.Add(new SqlParameter("@City", ""));
                _pMssql4.Add(new SqlParameter("@State", ""));
                _pMssql4.Add(new SqlParameter("@Postal", ""));
                _pMssql4.Add(new SqlParameter("@Country", ""));
                _pMssql4.Add(new SqlParameter("@Attn_To", sampleAdHocMaster.ATTN_TO == null ? "" : sampleAdHocMaster.ATTN_TO));
                _pMssql4.Add(new SqlParameter("@Remark", sampleAdHocMaster.REMARK == null ? "" : sampleAdHocMaster.REMARK));
                _pMssql4.Add(new SqlParameter("@Sent_By", sampleAdHocMaster.SENT_BY == null ? "" : sampleAdHocMaster.SENT_BY));
                _pMssql4.Add(new SqlParameter("@Date_Sent", DateTime.ParseExact(sampleAdHocMaster.DATE_SENT, dateFormat, CultureInfo.InvariantCulture)));
                _pMssql4.Add(new SqlParameter("@Courier_No", sampleAdHocMaster.COURIER_NO == null ? "" : sampleAdHocMaster.COURIER_NO));
                _pMssql4.Add(new SqlParameter("@Charge", sampleAdHocMaster.CHARGE == null ? "" : sampleAdHocMaster.CHARGE));
                _pMssql4.Add(new SqlParameter("@Charge_By", sampleAdHocMaster.CHARGE_BY == null ? "" : sampleAdHocMaster.CHARGE_BY));
                _pMssql4.Add(new SqlParameter("@UserName", AclUser.EMP_NAME));
                _pMssql4.Add(new SqlParameter("@IPAddress", Request.UserHostAddress));
                await common.PSP_COMMON_SQL("psp_SAMPLE_ADHOC_MASTER_Create", CommandType.StoredProcedure, _pMssql4, "", constr, false);

                //submit ad hoc request (insert product info to SAMPLE_ADHOC_DETAILS)
                foreach (var z in adHocRequestDetailArray)
                {
                    if (z.REQUEST_QUANTITY > 0)
                    {
                        List<SqlParameter> _pMssql3 = new List<SqlParameter>();
                        _pMssql3.Add(new SqlParameter("@Rec_Type", '1'));
                        _pMssql3.Add(new SqlParameter("@Sample_Request_No", newSRNo == null ? "" : newSRNo));
                        _pMssql3.Add(new SqlParameter("@PONO", z.PO_NO == null ? "" : z.PO_NO));
                        _pMssql3.Add(new SqlParameter("@Chop_Number", z.CHOP_NO == null ? "" : z.CHOP_NO));
                        _pMssql3.Add(new SqlParameter("@Composition", z.COMPOSITION == null ? "" : z.COMPOSITION));
                        _pMssql3.Add(new SqlParameter("@Color_Desc", z.COLOR_WAY == null ? "" : z.COLOR_WAY));
                        _pMssql3.Add(new SqlParameter("@Finish_Type1", z.FINISH_TYPE1 == null ? "" : z.FINISH_TYPE1));
                        _pMssql3.Add(new SqlParameter("@Finish_Type2", z.FINISH_TYPE2 == null ? "" : z.FINISH_TYPE2));
                        _pMssql3.Add(new SqlParameter("@Finish_Type3", z.FINISH_TYPE3 == null ? "" : z.FINISH_TYPE3));
                        _pMssql3.Add(new SqlParameter("@Finish_Type4", z.FINISH_TYPE4 == null ? "" : z.FINISH_TYPE4));
                        _pMssql3.Add(new SqlParameter("@Warp_Density", z.WARP_DENSITY == null ? "" : z.WARP_DENSITY));
                        _pMssql3.Add(new SqlParameter("@Warp_Density2", z.WARP_DENSITY2 == null ? "" : z.WARP_DENSITY2));
                        _pMssql3.Add(new SqlParameter("@Weft_Density", z.WEFT_DENSITY == null ? "" : z.WEFT_DENSITY));
                        _pMssql3.Add(new SqlParameter("@Weft_Density2", z.WEFT_DENSITY2 == null ? "" : z.WEFT_DENSITY2));
                        _pMssql3.Add(new SqlParameter("@Warp_Count", z.WARP_COUNT == null ? "" : z.WARP_COUNT));
                        _pMssql3.Add(new SqlParameter("@Warp_Count2", z.WARP_COUNT2 == null ? "" : z.WARP_COUNT2));
                        _pMssql3.Add(new SqlParameter("@Warp_Count3", z.WARP_COUNT3 == null ? "" : z.WARP_COUNT3));
                        _pMssql3.Add(new SqlParameter("@Weft_Count", z.WEFT_COUNT == null ? "" : z.WEFT_COUNT));
                        _pMssql3.Add(new SqlParameter("@Weft_Count2", z.WEFT_COUNT2 == null ? "" : z.WEFT_COUNT2));
                        _pMssql3.Add(new SqlParameter("@Weft_Count3", z.WEFT_COUNT3 == null ? "" : z.WEFT_COUNT3));
                        _pMssql3.Add(new SqlParameter("@Sample_Roll_Number", z.SAMPLE_ROLL_NO == null ? "" : z.SAMPLE_ROLL_NO));
                        _pMssql3.Add(new SqlParameter("@Balance_Yardage", z.BALANCE_YARDAGE));
                        _pMssql3.Add(new SqlParameter("@Location", z.LOCATION == null ? "" : z.LOCATION));
                        _pMssql3.Add(new SqlParameter("@Qty_Request", z.REQUEST_QUANTITY));
                        _pMssql3.Add(new SqlParameter("@UOM", z.UOM == null ? "" : z.UOM));
                        _pMssql3.Add(new SqlParameter("@Sent_By", z.SENT_BY == null ? "" : z.SENT_BY));
                        _pMssql3.Add(new SqlParameter("@Date_Sent", DateTime.ParseExact(z.DATE_SENT, dateFormat, CultureInfo.InvariantCulture)));
                        _pMssql3.Add(new SqlParameter("@UserName", AclUser.EMP_NAME));
                        _pMssql3.Add(new SqlParameter("@IPAddress", Request.UserHostAddress));
                        _pMssql3.Add(new SqlParameter("@Qty_Sent", '0'));
                        await common.PSP_COMMON_SQL("PSP_SAMPLE_ADHOC_DETAILS_CREATE_NEW_AQUARIUM", CommandType.StoredProcedure, _pMssql3, "", constr, false);
                    }
                }

                //insert new request
                List<SqlParameter> _pMssql5 = new List<SqlParameter>();
                _pMssql5.Add(new SqlParameter("@pCartID", cartIDStr));
                _pMssql5.Add(new SqlParameter("@pSampleReqNo", newSRNo));
                _pMssql5.Add(new SqlParameter("@pCreatedBy", username));
                _pMssql5.Add(new SqlParameter("@pCreatedDate", DateTime.Now));
                _pMssql5.Add(new SqlParameter("@pCreatedLoc", Request.UserHostAddress));
                await common.PSP_COMMON_SQL("PSP_INSERT_MM_REQUEST", CommandType.StoredProcedure, _pMssql5, "", "", false);

                //clean folder
                string folderPath = AppDomain.CurrentDomain.BaseDirectory + "pdfFolder";
                if (Directory.Exists(folderPath))
                {
                    string[] files = Directory.GetFiles(folderPath);
                    foreach (string file in files)
                    {
                        System.IO.File.Delete(file);
                    }
                }

                //generate PDF
                ExportPDf.ExportPDf.AjaxExportPdf generatePdf = new ExportPDf.ExportPDf.AjaxExportPdf();
                string returnFileName = generatePdf.AjaxExportPdf("SAMPLE_REQUEST_NO", newSRNo, "0", "SPL_AdHocRequest", "connPF");

                await productDtl.SendEmailAsync(returnFileName, newSRNo, AclUser);

                object obj = new
                {
                    pCART_H_ID = sampleAdHocMaster.CART_H_ID,
                    pACTION = "DELETE",
                    pCREATED_BY = username,
                    pLOC = Request.UserHostAddress,
                };

                bool success = await common.PSP_COMMON_DAPPER_SINGLE<bool>("PSP_DRAFT_CART_ITEM_MAINT", CommandType.StoredProcedure, obj);

                return Json(new { success });
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return Json(new { success = false });
            }
        }
        #endregion
    }
}