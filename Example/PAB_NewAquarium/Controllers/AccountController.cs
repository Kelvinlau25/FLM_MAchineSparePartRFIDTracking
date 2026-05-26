using PAB_NewAquarium.DAL;
using PAB_NewAquarium.Filters;
using PAB_NewAquarium.Helpers;
using PAB_NewAquarium.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Web.Mvc;
using OfficeOpenXml;
using System.Reflection;

namespace PAB_NewAquarium.Controllers
{
    public class AccountController : BaseController
    {
        readonly MM_ProductRegistration_DAL dal = new MM_ProductRegistration_DAL();
        readonly BlobHelper blobHelper = new BlobHelper();

        [SessionExpire]
        public async Task<ActionResult> Account()
        {
            try
            {
                var obj = new { pCreatedBy = username };
                List<RECOMMENDED_PRODUCTS> model = await common.PSP_COMMON_DAPPER<RECOMMENDED_PRODUCTS>("PSP_GET_SALES_RECENT_REQUESTED_PRODUCTS", CommandType.StoredProcedure, obj);

                foreach (var item in model)
                {
                    item.IMAGE_URL = await blobHelper.SetBlobUrlToken(item.IMAGE_URL);
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
        public async Task<ActionResult> BrowsingHistory()
        {
            try
            {
                BrowsingHistoryList model = new BrowsingHistoryList();
                model.BrowsingHistoryDdl = await common.PSP_COMMON_DROPDOWN("PBrowsingHistory");
                model.CompanyDdl = await dal.PSP_GET_DDL2("MM_CUSTOMER_GROUP_H", "GROUP_H_ID", "COMPANY_NAME");
                model.FabricCategoryDdl = await dal.PSP_GET_DDL2("MM_CATEGORY_H", "CATEGORY_H_ID", "CATEGORY_NAME");
                model.ProductTypeDdl = await dal.PSP_GET_DDL("MM_PRODUCT_TYPE", "PRODUCT_TYPE");
                model.SortBy = await common.PSP_COMMON_DROPDOWN("PBrowseSortBy");
                ViewBag.CompanyNum = model.CompanyDdl.Count;
                ViewBag.FabricCategoryNum = model.FabricCategoryDdl.Count;
                ViewBag.ProductTypeNum = model.ProductTypeDdl.Count;
                ViewBag.CreatedBy = username;
                return View(model);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return RedirectToAction("Error", "Home");
            }
        }

        [SessionExpire]
        public async Task<ActionResult> RequestHistory()
        {
            try
            {
                RequestHistoryList model = new RequestHistoryList();
                model.RequestHistoryDdl = await common.PSP_COMMON_DROPDOWN("PRequestHistory");
                model.CompanyDdl = await dal.PSP_GET_DDL2("MM_CUSTOMER_GROUP_H", "GROUP_H_ID", "COMPANY_NAME");
                model.FabricCategoryDdl = await dal.PSP_GET_DDL2("MM_CATEGORY_H", "CATEGORY_H_ID", "CATEGORY_NAME");
                model.ProductTypeDdl = await dal.PSP_GET_DDL("MM_PRODUCT_TYPE", "PRODUCT_TYPE");
                model.SortBy = await common.PSP_COMMON_DROPDOWN("PRequestSortBy");
                model.SampleRequestNo = await dal.PSP_GET_DDL("MM_REQUEST_H", "SAMPLE_REQUEST_NO");
                ViewBag.CompanyNum = model.CompanyDdl.Count;
                ViewBag.FabricCategoryNum = model.FabricCategoryDdl.Count;
                ViewBag.ProductTypeNum = model.ProductTypeDdl.Count;
                ViewBag.SampleRequestNo = model.SampleRequestNo.Count;
                ViewBag.Date = DateTime.Now.AddMonths(-3);
                return View(model);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return RedirectToAction("Error", "Home");
            }
        }

        #region Ajax Function
        public async Task<JsonResult> GetBrowsingHistory(string customer, string fabricCategory, string productType, string fromDate, string toDate, string sortBy, string createdBy)
        {
            try
            {
                List<SqlParameter> _pMssql = new List<SqlParameter>();
                _pMssql.Add(new SqlParameter("@pCustomer", customer));
                _pMssql.Add(new SqlParameter("@pFabricCategory", fabricCategory));
                _pMssql.Add(new SqlParameter("@pProductType", productType));
                _pMssql.Add(new SqlParameter("@pFromDate", fromDate));
                _pMssql.Add(new SqlParameter("@pToDate", toDate));
                _pMssql.Add(new SqlParameter("@pSortBy", sortBy));
                _pMssql.Add(new SqlParameter("@pCreatedBy", createdBy));
                List<DataTable> result = await common.PSP_COMMON_SQL_RTN_MULTIPLE("PSP_GET_BROWSING_HISTORY", CommandType.StoredProcedure, _pMssql);


                BrowsingHistoryViewModel viewModel = new BrowsingHistoryViewModel();
                viewModel.BrowsingHistory = new List<BrowsingHistory>();
                foreach (DataRow row in result[0].Rows)
                {
                    BrowsingHistory bh = new BrowsingHistory
                    {
                        PRODUCT_H_ID = Convert.ToString(row["PRODUCT_H_ID"]),
                        VIEWS = Convert.ToString(row["VIEWS"]),
                        CUSTOMER_NAME = Convert.ToString(row["CUSTOMER_NAME"]),
                        COMPANY_NAME = Convert.ToString(row["COMPANY_NAME"]),
                        CATEGORY = Convert.ToString(row["CATEGORY"]),
                        CHOP_NO = Convert.ToString(row["CHOP_NO"]),
                        TRANSACTION_DATE = Convert.ToString(row["TRANSACTION_DATE"]),
                        PRODUCT_IMAGE = await blobHelper.SetBlobUrlToken(Convert.ToString(row["PRODUCT_IMAGE"]))
                    };
                    viewModel.BrowsingHistory.Add(bh);
                }

                viewModel.BrowsingHistoryCustomer = new List<BrowsingHistoryCustomer>();
                foreach (DataRow row in result[1].Rows)
                {
                    BrowsingHistoryCustomer bhc = new BrowsingHistoryCustomer
                    {
                        PRODUCT_H_ID = Convert.ToString(row["PRODUCT_H_ID"]),
                        CUSTOMER_NAME = Convert.ToString(row["CUSTOMER_NAME"]),
                        COMPANY_NAME = Convert.ToString(row["COMPANY_NAME"]),
                        TRANSACTION_DATE = Convert.ToString(row["TRANSACTION_DATE"])
                    };
                    viewModel.BrowsingHistoryCustomer.Add(bhc);
                }

                return Json(viewModel, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return Json(new BrowsingHistoryViewModel(), JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<JsonResult> GetBrowsingHistoryExcel(string customer, string fabricCategory, string productType, string fromDate, string toDate, string sortBy, string createdBy)
        {
            try
            {
                List<SqlParameter> _pMssql = new List<SqlParameter>();
                _pMssql.Add(new SqlParameter("@pCustomer", customer));
                _pMssql.Add(new SqlParameter("@pFabricCategory", fabricCategory));
                _pMssql.Add(new SqlParameter("@pProductType", productType));
                _pMssql.Add(new SqlParameter("@pFromDate", fromDate));
                _pMssql.Add(new SqlParameter("@pToDate", toDate));
                _pMssql.Add(new SqlParameter("@pSortBy", sortBy));
                _pMssql.Add(new SqlParameter("@pCreatedBy", createdBy));
                List<DataTable> result = await common.PSP_COMMON_SQL_RTN_MULTIPLE("PSP_GET_BROWSING_HISTORY", CommandType.StoredProcedure, _pMssql);


                BrowsingHistoryViewModel viewModel = new BrowsingHistoryViewModel();
                viewModel.BrowsingHistory = new List<BrowsingHistory>();
                foreach (DataRow row in result[0].Rows)
                {
                    BrowsingHistory bh = new BrowsingHistory
                    {
                        PRODUCT_H_ID = Convert.ToString(row["PRODUCT_H_ID"]),
                        VIEWS = Convert.ToString(row["VIEWS"]),
                        CUSTOMER_NAME = Convert.ToString(row["CUSTOMER_NAME"]),
                        COMPANY_NAME = Convert.ToString(row["COMPANY_NAME"]),
                        CATEGORY = Convert.ToString(row["CATEGORY"]),
                        CHOP_NO = Convert.ToString(row["CHOP_NO"]),
                        TRANSACTION_DATE = Convert.ToString(row["TRANSACTION_DATE"]),
                        PRODUCT_IMAGE = Convert.ToString(row["PRODUCT_IMAGE"])
                    };
                    viewModel.BrowsingHistory.Add(bh);
                }

                viewModel.BrowsingHistoryCustomer = new List<BrowsingHistoryCustomer>();
                foreach (DataRow row in result[1].Rows)
                {
                    BrowsingHistoryCustomer bhc = new BrowsingHistoryCustomer
                    {
                        PRODUCT_H_ID = Convert.ToString(row["PRODUCT_H_ID"]),
                        CUSTOMER_NAME = Convert.ToString(row["CUSTOMER_NAME"]),
                        COMPANY_NAME = Convert.ToString(row["COMPANY_NAME"]),
                        TRANSACTION_DATE = Convert.ToString(row["TRANSACTION_DATE"])
                    };
                    viewModel.BrowsingHistoryCustomer.Add(bhc);
                }

                return Json(viewModel, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return Json(new BrowsingHistoryViewModel(), JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<JsonResult> GetRequestHistory(string historyCategory, string customer, string fabricCategory, string productType, string fromDate, string toDate, string sortBy, string sampleReqNo)
        {
            try
            {
                RequestHistoryViewModel model = new RequestHistoryViewModel();
                historyCategory = historyCategory == "2" ? username : string.Empty;
                var param = new
                {
                    pHistoryCategory = historyCategory,
                    pCustomer = customer,
                    pFabricCategory = fabricCategory,
                    pProductType = productType,
                    pFromDate = fromDate,
                    pToDate = toDate,
                    pSortBy = sortBy,
                    pSampleReqNo = sampleReqNo
                };
                model.RequestHistory = await common.PSP_COMMON_DAPPER<RequestHistory>("PSP_GET_REQUEST_HISTORY", CommandType.StoredProcedure, param);
                
                foreach (var item in model.RequestHistory)
                {
                    item.PRODUCT_IMAGE = await blobHelper.SetBlobUrlToken(item.PRODUCT_IMAGE);
                }

                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return Json(new RequestHistoryViewModel(), JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<ActionResult> GetRequestHistoryExcel(string historyCategory, string customer, string fabricCategory, string productType, string fromDate, string toDate, string sortBy, string sampleReqNo)
        {
            try
            {
                List<SqlParameter> _pMssql = new List<SqlParameter>();
                _pMssql.Add(new SqlParameter("@pHistoryCategory", historyCategory == "2" ? username : ""));
                _pMssql.Add(new SqlParameter("@pCustomer", customer));
                _pMssql.Add(new SqlParameter("@pFabricCategory", fabricCategory));
                _pMssql.Add(new SqlParameter("@pProductType", productType));
                _pMssql.Add(new SqlParameter("@pFromDate", fromDate));
                _pMssql.Add(new SqlParameter("@pToDate", toDate));
                _pMssql.Add(new SqlParameter("@pSortBy", sortBy));
                _pMssql.Add(new SqlParameter("@pSampleReqNo", sampleReqNo));
                DataTable result = await common.PSP_COMMON_SQL("PSP_GET_REQUEST_HISTORY", CommandType.StoredProcedure, _pMssql);

                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                using (ExcelPackage excel = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = excel.Workbook.Worksheets.Add("Sheet1");
                    worksheet.Cells[1, 1].Value = "Company Name";
                    //worksheet.Cells[1, 2].Value = "Category";
                    worksheet.Cells[1, 2].Value = "Reference No";
                    worksheet.Cells[1, 3].Value = "Chop No";
                    worksheet.Cells[1, 4].Value = "Color Way";
                    worksheet.Cells[1, 5].Value = "Quantity Request";
                    worksheet.Cells[1, 6].Value = "Sample Adhoc Request No.";
                    worksheet.Cells[1, 7].Value = "Date Sent";

                    int row = 2;
                    foreach (DataRow item in result.Rows)
                    {
                        worksheet.Cells[row, 1].Value = Convert.ToString(item["COMPANY_NAME"]);
                        //worksheet.Cells[row, 2].Value = Convert.ToString(item["CATEGORY"]);
                        worksheet.Cells[row, 2].Value = Convert.ToString(item["REFERENCE_NO"]);
                        worksheet.Cells[row, 3].Value = Convert.ToString(item["CHOP_NO"]);
                        worksheet.Cells[row, 4].Value = Convert.ToString(item["COLOR_WAY"]);
                        worksheet.Cells[row, 5].Value = Convert.ToString(item["QUANTITY_REQ"]);
                        worksheet.Cells[row, 6].Value = Convert.ToString(item["SAMPLE_REQUEST_NO"]);
                        worksheet.Cells[row, 7].Value = Convert.ToString(item["CREATED_DATE"]);

                        row++;
                    }

                    var range = worksheet.Cells[1, 1, row - 1, 7];
                    var table = worksheet.Tables.Add(range, "TopSalesTable");

                    return File(excel.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Data.xlsx");
                }
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return RedirectToAction("Error", "Home");
            }
        }
        #endregion

        #region Temporally closed due to functional spec changes
        public async Task<ActionResult> ToReturn()
        {
            try
            {
                ToReturnModel model = new ToReturnModel();
                model.RETURN_LIST = await common.PSP_COMMON_DAPPER<ToReturnListModel>("PSP_GET_TO_RETURN_LIST", CommandType.StoredProcedure);
                for (int i = 0; i < model.RETURN_LIST.Count; i++)
                {
                    string directory = Path.GetDirectoryName(model.RETURN_LIST[i].IMAGE_URL);
                    string fileName = Path.GetFileNameWithoutExtension(model.RETURN_LIST[i].IMAGE_URL);
                    string extension = Path.GetExtension(model.RETURN_LIST[i].IMAGE_URL);
                    string modifiedFileName = "_Display_" + fileName;
                    model.RETURN_LIST[i].IMAGE_URL = Path.Combine(directory, modifiedFileName + extension);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<ActionResult> RFIDToReturnList()
        {
            try
            {
                ToReturnModel model = new ToReturnModel();
                model.RETURN_LIST = await common.PSP_COMMON_DAPPER<ToReturnListModel>("PSP_GET_TO_RETURN_LIST", CommandType.StoredProcedure);
                for (int i = 0; i < model.RETURN_LIST.Count; i++)
                {
                    string directory = Path.GetDirectoryName(model.RETURN_LIST[i].IMAGE_URL);
                    string fileName = Path.GetFileNameWithoutExtension(model.RETURN_LIST[i].IMAGE_URL);
                    string extension = Path.GetExtension(model.RETURN_LIST[i].IMAGE_URL);
                    string modifiedFileName = "_Display_" + fileName;
                    model.RETURN_LIST[i].IMAGE_URL = Path.Combine(directory, modifiedFileName + extension);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public async Task<ActionResult> ReturnItem(int[] array)
        {
            try
            {
                foreach (var item in array)
                {
                    var obj = new
                    {
                        pProductID = item,
                        pCreatedBy = username,
                        pCreatedDate = DateTime.Now,
                        pCreatedLoc = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString()
                    };
                    await common.PSP_COMMON_DAPPER<dynamic>("PSP_UPDATE_TO_RETURN", CommandType.StoredProcedure, obj);
                }
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}