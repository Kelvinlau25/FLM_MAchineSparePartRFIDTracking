using Oracle.ManagedDataAccess.Client;
using PAB_NewAquarium.Helpers;
using PAB_NewAquarium.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;

namespace PAB_NewAquarium.DAL
{
    public class MM_ProductDetail_DAL
    {
        readonly CommonFunction common = new CommonFunction();
        readonly BlobHelper blobHelper = new BlobHelper();

        public async Task<bool> PSP_UPDATE_ADHOC_REQUEST_EDIT(AddHocRequestModel model)
        {
            try
            {
                var param = new
                {
                    model.SAMPLE_REQUEST_NO,
                    model.SALES_REGION,
                    model.ATTN_TO,
                    model.REMARK,
                    model.CHARGE,
                    model.CHARGE_BY,
                    model.PRIORITY,
                    model.CUST_CODE,
                    model.CUST_ADDR_1,
                    model.CUST_ADDR_2,
                    model.RECORD_TYP,
                    model.CREATED_BY,
                    model.CREATED_LOC,
                    DATE_SENT = DateTime.ParseExact(model.DATE_SENT, "dd/MM/yyyy", CultureInfo.InvariantCulture)
                };

                string constr = (ConfigurationManager.AppSettings["isTest"].ToUpper() == "TRUE") ? "PAB_SampleInv_Test" : "PAB_SampleInv";

                // Update SAMPLE_ADHOC_MASTER table
                bool success = await common.PSP_COMMON_DAPPER_SINGLE<bool>("PSP_UPDATE_ADHOC_REQUEST_H", CommandType.StoredProcedure, param, constr);

                // Update SAMPLE_ADHOC_DETAILS table
                if (success)
                {
                    foreach (var detail in model.Details)
                    {
                        var detailParam = new
                        {
                            model.SAMPLE_REQUEST_NO,
                            detail.SAMPLE_ROLL_NO,
                            detail.QUANTITY_REQUEST,
                            model.RECORD_TYP,
                            model.CREATED_BY,
                            model.CREATED_LOC,
                        };

                        success = await common.PSP_COMMON_DAPPER_SINGLE<bool>("PSP_UPDATE_ADHOC_REQUEST_D", CommandType.StoredProcedure, detailParam, constr);

                        if (!success)
                        {
                            break;
                        }
                    }
                }
                
                return success;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> PSP_UPDATE_ADHOC_REQUEST_CANCEL(AddHocRequestModel model)
        {
            try
            {
                var param = new
                {
                    model.SAMPLE_REQUEST_NO,
                    model.RECORD_TYP,
                    model.CREATED_BY,
                    model.CREATED_LOC,
                };

                string constr = (ConfigurationManager.AppSettings["isTest"].ToUpper() == "TRUE") ? "PAB_SampleInv_Test" : "PAB_SampleInv";
                bool success = await common.PSP_COMMON_DAPPER_SINGLE<bool>("PSP_UPDATE_ADHOC_REQUEST_H", CommandType.StoredProcedure, param, constr);

                if (success)
                {
                    success = await common.PSP_COMMON_DAPPER_SINGLE<bool>("PSP_CANCEL_MM_REQUEST", CommandType.StoredProcedure, param);
                }

                return success;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> GetSalesRegion(string userID, string company)
        {
            try
            {
                string salesRegion = string.Empty;
                List<OracleParameter> _pMssql = new List<OracleParameter>();
                _pMssql.Add(new OracleParameter("puserid", userID));
                _pMssql.Add(new OracleParameter("pcoid", company));
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

                return salesRegion;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task SendEmailAsync(string returnFileName, string newSRNo, ACL_UserObj AclUser)
        {
            try
            {
                string filePath = AppDomain.CurrentDomain.BaseDirectory + "pdfFolder\\" + returnFileName;
                if (System.IO.File.Exists(filePath) && AclUser != null)
                {
                    var param1 = new { INDICATOR = "PSmtpServerIP" };
                    var param2 = new { INDICATOR = "PSenderEmail" };
                    var param3 = new { INDICATOR = "PAdHocEmailSubject" };
                    var param4 = new { INDICATOR = "PAdHocEmailContent" };
                    var param5 = new { INDICATOR = "PAdHocEmailCC" };
                    string smtpServer = await common.PSP_COMMON_DAPPER_SINGLE<string>("PSP_GET_MASTER_SETTING_VALUE2", CommandType.StoredProcedure, param1);
                    string senderEmail = await common.PSP_COMMON_DAPPER_SINGLE<string>("PSP_GET_MASTER_SETTING_VALUE2", CommandType.StoredProcedure, param2);
                    string emailSubject = await common.PSP_COMMON_DAPPER_SINGLE<string>("PSP_GET_MASTER_SETTING_VALUE2", CommandType.StoredProcedure, param3);
                    string emailContent = await common.PSP_COMMON_DAPPER_SINGLE<string>("PSP_GET_MASTER_SETTING_VALUE2", CommandType.StoredProcedure, param4);
                    string emailCC = await common.PSP_COMMON_DAPPER_SINGLE<string>("PSP_GET_MASTER_SETTING_VALUE2", CommandType.StoredProcedure, param5);
                    string[] array = smtpServer.Split('-');
                    string serverIP = array[0];
                    string serverPort = array[1];
                    emailSubject = emailSubject.Replace("xxxxxxx", newSRNo);
                    emailContent = emailContent.Replace("xxxxxxx", newSRNo).Replace("Dear Name", "Dear " + AclUser.EMP_NAME + "");

                    using (SmtpClient smtpClient = new SmtpClient(serverIP, Convert.ToInt32(serverPort)))
                    {
                        MailMessage message = new MailMessage(senderEmail, AclUser.USR_EMAIL)
                        {
                            IsBodyHtml = true,
                            Subject = emailSubject,
                            Body = emailContent
                        };
                        if (emailCC != "")
                        {
                            foreach (string item in emailCC.Split(';'))
                            {
                                message.CC.Add(new MailAddress(item.Trim()));
                            }
                        }

                        Attachment attachment = new Attachment(filePath, MediaTypeNames.Application.Octet);
                        message.Attachments.Add(attachment);
                        smtpClient.Send(message);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<FabricVideo>> PSP_GET_VIDEO_BY_CATEGORY(int id = 0)
        {
            try
            {
                object obj = new { PID = id };
                string constr = ConfigurationManager.AppSettings["isTest"].ToUpper().ToString() == "TRUE" ? "PAB_SampleInv_Test" : "PAB_SampleInv";               
                List<FabricVideo> fabricVideos = await common.PSP_COMMON_DAPPER<FabricVideo>("PSP_GET_VIDEO_BY_CATEGORY", CommandType.StoredProcedure, obj, constr);

                return fabricVideos;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<ProductImageList>> GetImageHiRes(int id)
        {
            var obj = new { pID = id };

            List<ProductImageHiRes> data = await common.PSP_COMMON_DAPPER<ProductImageHiRes>("PSP_GET_PRODUCT_IMAGE", CommandType.StoredProcedure, obj);

            foreach (var item in data)
            {
                item.IMAGE_URL = await blobHelper.SetBlobUrlToken(item.IMAGE_URL);
            }

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

                    imgNameList.Add(imageName[imageName.Length - 1]);
                    imgPathList.Add(imageUrl);

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
                        imageList.Add(singleColorImgList);
                        imgNameList = new List<string>();
                        imgPathList = new List<string>();
                    }
                }
            }

            return imageList;
        }

        public async Task<Product_H_Detail> GetProductDetailAsync(int id, string isEdit = "")
        {
            try
            {
                var obj = new { pID = id, pReferenceNo = "", pIsEdit = isEdit };
                Product_H_Detail model = await common.PSP_COMMON_DAPPER_SINGLE<Product_H_Detail>("PSP_GET_MM_PRODUCT_DETAIL", CommandType.StoredProcedure, obj);
                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<ProductComment>> GetProductCommentAsync(int id)
        {
            try
            {
                var obj = new { pID = id };
                List<ProductComment> model = await common.PSP_COMMON_DAPPER<ProductComment>("PSP_GET_A_PRODUCT_COMMENT_LIST", CommandType.StoredProcedure, obj);
                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<ProductComment>> GetProductCommentsAsync(int id)
        {
            try
            {
                var obj = new { pID = id };
                List<ProductComment> model = await common.PSP_COMMON_DAPPER<ProductComment>("PSP_GET_A_PRODUCT_COMMENTS_LIST", CommandType.StoredProcedure, obj);
                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}