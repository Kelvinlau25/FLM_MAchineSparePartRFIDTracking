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
    public class RFIDDashboardController : BaseController
    {
        readonly ReaderRFID dal = new ReaderRFID();
        readonly MM_ProductDetail_DAL productDtl = new MM_ProductDetail_DAL();
        readonly BlobHelper blobHelper = new BlobHelper();

        #region RFID Dashboard
        [SessionExpire]
        public async Task<ActionResult> RFIDDashboard()
        {
            try
            {
                RFIDDashboardModel model = new RFIDDashboardModel();
                model.RFID_TAG = await common.PSP_COMMON_DAPPER<RFID_TAG>("PSP_GET_RFID_TAG_LIST", CommandType.StoredProcedure);
                model.RFID_READER = await dal.GetReader("RFIDDashboard");
                model.RFID_TAG_JSON = JsonConvert.SerializeObject(model.RFID_TAG);
                ViewBag.RFID_TAG = JsonConvert.SerializeObject(model.RFID_TAG);
                return View(model);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<ActionResult> GetImageCount(int id, string user)
        {
            try
            {
                Random random = new Random();
                var obj = new { pID = id };
                int randomNumber = 0;

                // Base URL to redirect
                string url = "/RFIDDashboard/RFIDProductPage";

                // Get the number of images associated with the product ID
                int count = await common.PSP_COMMON_DAPPER_SINGLE<int>("PSP_GET_PRODUCT_IMAGE_COUNT", CommandType.StoredProcedure, obj);

                // Determine the range of random numbers based on the image count
                if (count == 1)
                {
                    randomNumber = random.Next(1, 5); // Generates 1–4
                }
                else if (count == 2)
                {
                    randomNumber = random.Next(5, 9); // Generates 5–8
                }
                else if (count == 3)
                {
                    randomNumber = random.Next(9, 13); // Generates 9–12
                }

                if (randomNumber == 1)
                {
                    url += "?id=" + id + "&user=" + user;
                }
                else
                {
                    url += randomNumber + "?id=" + id + "&user=" + user;
                }

                return Json(url, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                throw;
            }
        }

        public async Task<ActionResult> GetRFIDImage(string tag)
        {
            try
            {
                var obj = new { pRFID = tag };
                RFID_IMAGE model = await common.PSP_COMMON_DAPPER_SINGLE<RFID_IMAGE>("PSP_GET_IMAGE_RFID_DASHBOARD", CommandType.StoredProcedure, obj);
                model.IMAGE_URL = await blobHelper.SetBlobUrlToken(model.IMAGE_URL);
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return Json(new RFID_IMAGE(), JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<ActionResult> UpdateLocation(string tag)
        {
            try
            {
                ACL_UserObj aclObj = (Session["AclUser"] as ACL_UserObj);
                var obj = new
                {
                    pRFID = tag,
                    pCreatedBy = aclObj.EMP_NO + " - " + aclObj.EMP_NAME,
                    pCreatedDate = DateTime.Now,
                    pCreatedLoc = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString()
                };
                await common.PSP_COMMON_DAPPER<dynamic>("PSP_UPDATE_PRODUCT_LOCATION", CommandType.StoredProcedure, obj);
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                throw;
            }
        }

        [HttpPost]
        public async Task<ActionResult> RFIDStatus(string tag)
        {
            try
            {
                List<SqlParameter> _pMssql = new List<SqlParameter>();
                _pMssql = new List<SqlParameter>();
                _pMssql.Add(new SqlParameter("@pRFIDTag", tag));
                _pMssql.Add(new SqlParameter("returnResult", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                string result = await common.PSP_COMMON_SQL("PSP_GET_AJAX_RFID_TAG_STATUS", CommandType.StoredProcedure, _pMssql, "", "", true);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                throw;
            }
        }
        #endregion

        #region RFID Product Page
        public async Task<ActionResult> RFIDProductPage(int id, string user, string colorWay = "")
        {
            try
            {
                Product_H_Detail model = await GetProductDetailAsync(id);
                List<ProductImageList> productImageList = await GetImage(id);
                model.imageList = JsonConvert.SerializeObject(productImageList);
                model.FabricVideoList = await productDtl.PSP_GET_VIDEO_BY_CATEGORY(id);
                ViewBag.colorWay = colorWay == "" ? productImageList[0].COLOR_WAY : colorWay;
                ViewBag.user = user;
                return View(model);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<ActionResult> RFIDProductPage2(int id, string user, string colorWay = "")
        {
            try
            {
                Product_H_Detail model = await GetProductDetailAsync(id);
                List<ProductImageList> productImageList = await GetImage(id);
                model.imageList = JsonConvert.SerializeObject(productImageList);
                model.FabricVideoList = await productDtl.PSP_GET_VIDEO_BY_CATEGORY(id);
                ViewBag.colorWay = colorWay == "" ? productImageList[0].COLOR_WAY : colorWay;
                ViewBag.user = user;
                return View(model);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<ActionResult> RFIDProductPage3(int id, string user, string colorWay = "")
        {
            try
            {
                Product_H_Detail model = await GetProductDetailAsync(id);
                List<ProductImageList> productImageList = await GetImage(id);
                model.imageList = JsonConvert.SerializeObject(productImageList);
                model.FabricVideoList = await productDtl.PSP_GET_VIDEO_BY_CATEGORY(id);
                ViewBag.colorWay = colorWay == "" ? productImageList[0].COLOR_WAY : colorWay;
                ViewBag.user = user;
                return View(model);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<ActionResult> RFIDProductPage4(int id, string user, string colorWay = "")
        {
            try
            {
                Product_H_Detail model = await GetProductDetailAsync(id);
                List<ProductImageList> productImageList = await GetImage(id);
                model.imageList = JsonConvert.SerializeObject(productImageList);
                model.FabricVideoList = await productDtl.PSP_GET_VIDEO_BY_CATEGORY(id);
                ViewBag.colorWay = colorWay == "" ? productImageList[0].COLOR_WAY : colorWay;
                ViewBag.user = user;
                return View(model);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<ActionResult> RFIDProductPage5(int id, string user, string colorWay = "")
        {
            try
            {
                Product_H_Detail model = await GetProductDetailAsync(id);
                List<ProductImageList> productImageList = await GetImage(id);
                model.imageList = JsonConvert.SerializeObject(productImageList);
                model.FabricVideoList = await productDtl.PSP_GET_VIDEO_BY_CATEGORY(id);
                ViewBag.colorWay = colorWay == "" ? productImageList[0].COLOR_WAY : colorWay;
                ViewBag.user = user;
                return View(model);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<ActionResult> RFIDProductPage6(int id, string user, string colorWay = "")
        {
            try
            {
                Product_H_Detail model = await GetProductDetailAsync(id);
                List<ProductImageList> productImageList = await GetImage(id);
                model.imageList = JsonConvert.SerializeObject(productImageList);
                model.FabricVideoList = await productDtl.PSP_GET_VIDEO_BY_CATEGORY(id);
                ViewBag.colorWay = colorWay == "" ? productImageList[0].COLOR_WAY : colorWay;
                ViewBag.user = user;
                return View(model);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<ActionResult> RFIDProductPage7(int id, string user, string colorWay = "")
        {
            try
            {
                Product_H_Detail model = await GetProductDetailAsync(id);
                List<ProductImageList> productImageList = await GetImage(id);
                model.imageList = JsonConvert.SerializeObject(productImageList);
                model.FabricVideoList = await productDtl.PSP_GET_VIDEO_BY_CATEGORY(id);
                ViewBag.colorWay = colorWay == "" ? productImageList[0].COLOR_WAY : colorWay;
                ViewBag.user = user;
                return View(model);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<ActionResult> RFIDProductPage8(int id, string user, string colorWay = "")
        {
            try
            {
                Product_H_Detail model = await GetProductDetailAsync(id);
                List<ProductImageList> productImageList = await GetImage(id);
                model.imageList = JsonConvert.SerializeObject(productImageList);
                model.FabricVideoList = await productDtl.PSP_GET_VIDEO_BY_CATEGORY(id);
                ViewBag.colorWay = colorWay == "" ? productImageList[0].COLOR_WAY : colorWay;
                ViewBag.user = user;
                return View(model);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<ActionResult> RFIDProductPage9(int id, string user, string colorWay = "")
        {
            try
            {
                Product_H_Detail model = await GetProductDetailAsync(id);
                List<ProductImageList> productImageList = await GetImage(id);
                model.imageList = JsonConvert.SerializeObject(productImageList);
                model.FabricVideoList = await productDtl.PSP_GET_VIDEO_BY_CATEGORY(id);
                ViewBag.colorWay = colorWay == "" ? productImageList[0].COLOR_WAY : colorWay;
                ViewBag.user = user;
                return View(model);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<ActionResult> RFIDProductPage10(int id, string user, string colorWay = "")
        {
            try
            {
                Product_H_Detail model = await GetProductDetailAsync(id);
                List<ProductImageList> productImageList = await GetImage(id);
                model.imageList = JsonConvert.SerializeObject(productImageList);
                model.FabricVideoList = await productDtl.PSP_GET_VIDEO_BY_CATEGORY(id);
                ViewBag.colorWay = colorWay == "" ? productImageList[0].COLOR_WAY : colorWay;
                ViewBag.user = user;
                return View(model);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<ActionResult> RFIDProductPage11(int id, string user, string colorWay = "")
        {
            try
            {
                Product_H_Detail model = await GetProductDetailAsync(id);
                List<ProductImageList> productImageList = await GetImage(id);
                model.imageList = JsonConvert.SerializeObject(productImageList);
                model.FabricVideoList = await productDtl.PSP_GET_VIDEO_BY_CATEGORY(id);
                ViewBag.colorWay = colorWay == "" ? productImageList[0].COLOR_WAY : colorWay;
                ViewBag.user = user;
                return View(model);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<ActionResult> RFIDProductPage12(int id, string user, string colorWay = "")
        {
            try
            {
                Product_H_Detail model = await GetProductDetailAsync(id);
                List<ProductImageList> productImageList = await GetImage(id);
                model.imageList = JsonConvert.SerializeObject(productImageList);
                model.FabricVideoList = await productDtl.PSP_GET_VIDEO_BY_CATEGORY(id);
                ViewBag.colorWay = colorWay == "" ? productImageList[0].COLOR_WAY : colorWay;
                ViewBag.user = user;
                return View(model);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return RedirectToAction("Error", "Home");
            }
        }
        #endregion

        public async Task<Product_H_Detail> GetProductDetailAsync(int id, string isEdit = "")
        {
            try
            {
                var obj = new { pID = id, pReferenceNo = "", pIsEdit = isEdit };
                Product_H_Detail model = await common.PSP_COMMON_DAPPER_SINGLE<Product_H_Detail>("PSP_GET_MM_PRODUCT_DETAIL", CommandType.StoredProcedure, obj);
                model.CHOP_NO = model.CHOP_NO + " - " + model.FINISH_TYPE1.Replace(", ", " - ");
                if (model.CATEGORY != null)
                {
                    model.CATEGORY = model.CATEGORY.Split(',')[0];
                }
                return model;
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                throw;
            }
        }

        public async Task<List<ProductImageList>> GetImage(int id)
        {
            try
            {
                // Fetch image data from DB
                var obj = new { pID = id };
                List<ProductImage> data = await common.PSP_COMMON_DAPPER<ProductImage>("PSP_GET_PRODUCT_IMAGE", CommandType.StoredProcedure, obj);

                // Add blob token to each image URL
                foreach (var item in data)
                {
                    string[] split = item.IMAGE_URL.Split('/');
                    string imageName = split[split.Length - 1];
                    item.IMAGE_URL = await blobHelper.SetBlobUrlToken(item.IMAGE_URL);
                }

                // Prepare final grouped image list
                List<ProductImageList> imageList = new List<ProductImageList>();
                string colorWay = "", colorWayChk = "", imageUrl = "", currentQty = "";

                if (data.Count != 0)
                {
                    colorWay = data[0].COLOR_WAY;
                    List<string> imgNameList = new List<string>();
                    List<string> imgPathList = new List<string>();
                    for (int i = 0; i < data.Count; i++)
                    {
                        colorWayChk = data[i].COLOR_WAY;
                        currentQty = data[i].CURRENT_QTY;
                        imageUrl = data[i].IMAGE_URL;
                        string[] imageName = imageUrl.Split('/');

                        imgNameList.Add(imageName[imageName.Length - 1]); // last part (filename)
                        imgPathList.Add(imageUrl);

                        // Peek ahead to check if next item has different color or is last item
                        if (i + 1 < data.Count)
                        {
                            colorWay = data[i + 1].COLOR_WAY;

                        }
                        if (colorWay != colorWayChk || i == data.Count - 1)
                        {
                            ProductImageList singleColorImgList = new ProductImageList
                            {
                                COLOR_WAY = colorWayChk,
                                CURRENT_QTY = currentQty,
                                imageName = imgNameList,
                                imageFile = imgPathList
                            };
                            // Add images by color
                            imageList.Add(singleColorImgList);

                            // Reset for next group
                            imgNameList = new List<string>();
                            imgPathList = new List<string>();
                        }
                    }
                }

                return imageList;
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                throw;
            }
        }
    }
}