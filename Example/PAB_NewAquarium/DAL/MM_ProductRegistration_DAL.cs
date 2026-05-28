using PAB_NewAquarium.Helpers;
using PAB_NewAquarium.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.IdentityModel.Logging;

namespace PAB_NewAquarium.DAL
{
    public class MM_ProductRegistration_DAL
    {
        readonly CommonFunction common = new CommonFunction();
        readonly BlobHelper blobHelper = new BlobHelper();

        public async Task<bool> PSP_INSERT_VIDEO_CATEGORY(Product_H_Detail model)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("PRODUCT_H_ID", typeof(int));
                dt.Columns.Add("PRODUCT_CAT_H_ID", typeof(int));
                dt.Columns.Add("CREATED_BY", typeof(string));
                dt.Columns.Add("CREATED_DATE", typeof(DateTime));
                dt.Columns.Add("CREATED_LOC", typeof(string));

                foreach (var item in model.VIDEO_CAT_SELECTED) { 
                    dt.Rows.Add(model.PRODUCT_H_ID, item, model.CREATED_BY, model.CREATED_DATE, model.CREATED_LOC);
                }

                var param = new { DataTable = dt };
                bool success = await common.PSP_COMMON_DAPPER_SINGLE<bool>("PSP_MM_VIDEO_CATEGORY_MAINT", CommandType.StoredProcedure, param);

                return success;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<SelectListItem>> PSP_GET_DDL(string table, string column, string condition = "")
        {
            List<SelectListItem> ddlList = new List<SelectListItem>();
            List<SqlParameter> _pMssql = new List<SqlParameter>();
            _pMssql.Add(new SqlParameter("@pTable", table));
            _pMssql.Add(new SqlParameter("@pColumn", column));
            _pMssql.Add(new SqlParameter("@pCondition", condition));

            DataTable dt = await common.PSP_COMMON_SQL("PSP_GET_COMMON_LIST_DROPDOWN", CommandType.StoredProcedure, _pMssql, "", "", false);
            foreach (DataRow x in dt.Rows)
            {
                SelectListItem item = new SelectListItem
                {
                    Text = (string)x[column],
                    Value = (string)x[column]
                };
                ddlList.Add(item);
            }

            return ddlList;
        }

        public async Task<List<SelectListItem>> PSP_GET_DDL2(string table, string column, string column2, string condition = "")
        {
            List<SelectListItem> ddlList = new List<SelectListItem>();
            List<SqlParameter> _pMssql = new List<SqlParameter>();
            _pMssql.Add(new SqlParameter("@pTable", table));
            _pMssql.Add(new SqlParameter("@pColumn", column));
            _pMssql.Add(new SqlParameter("@pColumn2", column2));
            _pMssql.Add(new SqlParameter("@pCondition", condition));

            DataTable dt = await common.PSP_COMMON_SQL("PSP_GET_COMMON_LIST_DROPDOWN2", CommandType.StoredProcedure, _pMssql, "", "", false);
            foreach (DataRow x in dt.Rows)
            {
                SelectListItem item = new SelectListItem
                {
                    Value = x[column].ToString(),
                    Text = (string)x[column2]
                };
                ddlList.Add(item);
            }

            return ddlList;
        }      

        public async Task<Product_H_Detail> GetProductDetailAsync(int id, string isEdit = "")
        {
            try
            {
                var obj = new { pID = id, pReferenceNo = "", pIsEdit = isEdit };
                Product_H_Detail model = await common.PSP_COMMON_DAPPER_SINGLE<Product_H_Detail>("PSP_GET_MM_PRODUCT_DETAIL", CommandType.StoredProcedure, obj);
                var param = new { pID = id };
                model.VIDEO_CAT_SELECTED = await common.PSP_COMMON_DAPPER<int>("PSP_GET_VIDEO_CATEGORY", CommandType.StoredProcedure, param);
                model.VIDEO_CAT_NAME = await common.PSP_COMMON_DAPPER_SINGLE<string>("PSP_GET_VIDEO_CATEGORY_NAME", CommandType.StoredProcedure, param);
                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }      

        public List<string> ProcessImageAsync(Product_H_Detail model)
        {
            try
            {
                string rootFolder = $"{AppDomain.CurrentDomain.BaseDirectory}TempImage";
                string referenceFolder = $"{rootFolder}\\{model.REFERENCE_NO}";
                List<string> blobFileList = new List<string>();

                if (Directory.Exists(referenceFolder))
                {
                    string[] colorFolder = Directory.GetDirectories(referenceFolder);

                    foreach (var imageFolder in colorFolder)
                    {
                        string[] imageFiles = Directory.GetFiles(imageFolder);
                        string[] color = imageFolder.Split('\\');


                        foreach (var image in imageFiles)
                        {
                            string[] imageName = image.Split('\\');
                            string fileName = imageName[imageName.Length - 1].Replace('~', ' ');
                            string filePath = $"~/TempImage/{model.REFERENCE_NO}/{color[color.Length - 1]}/{fileName}";

                            #region Clone a smaller size of image
                            string compressFilePath = $"~/TempImage/{model.REFERENCE_NO}/{color[color.Length - 1]}/_Display_{fileName}";
                            string savePath = $"{referenceFolder}\\{color[color.Length - 1]}\\_Display_{fileName}";
                            ResizeImage(image, savePath);
                            #endregion

                            blobFileList.Add(filePath);
                            blobFileList.Add(compressFilePath);
                        }
                    }
                }
                return blobFileList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<ProductImageList>> GetImage(int id)
        {
            var obj = new { pID = id };

            List<ProductImage> data = await common.PSP_COMMON_DAPPER<ProductImage>("PSP_GET_PRODUCT_IMAGE", CommandType.StoredProcedure, obj);

            foreach (var item in data)
            {
                string[] split = item.IMAGE_URL.Split('/');
                string imageName = split[split.Length - 1];
                item.IMAGE_URL = item.IMAGE_URL.Replace(imageName, $"_Display_{imageName}");
                item.IMAGE_URL = await blobHelper.SetBlobUrlToken(item.IMAGE_URL);
            }

            List<ProductImageList> imageList = new List<ProductImageList>();
            string colorWay = "", colorWayChk = "", imageUrl = "";

            if (data.Count != 0)
            {
                colorWay = data[0].COLOR_WAY;
                List<string> imgNameList = new List<string>();
                List<string> imgPathList = new List<string>();
                List<int> imgID = new List<int>();
                for (int i = 0; i < data.Count; i++)
                {
                    colorWayChk = data[i].COLOR_WAY;
                    imageUrl = data[i].IMAGE_URL; ;
                    string[] imageName = imageUrl.Split('/');

                    imgNameList.Add(imageName[imageName.Length - 1]);
                    imgPathList.Add(imageUrl);
                    imgID.Add(data[i].IMAGE_ID);

                    if (i + 1 < data.Count)
                    {
                        colorWay = data[i + 1].COLOR_WAY;

                    }
                    if (colorWay != colorWayChk || i == data.Count - 1)
                    {
                        ProductImageList singleColorImgList = new ProductImageList
                        {
                            COLOR_WAY = colorWayChk,
                            imageName = imgNameList,
                            imageFile = imgPathList,
                            imageID = imgID
                        };
                        imageList.Add(singleColorImgList);
                        imgNameList = new List<string>();
                        imgPathList = new List<string>();
                    }
                }
            }

            return imageList;
        }

        public void DeleteFolder(string referenceNo)
        {
            try
            {
                string referenceFolder = $"{AppDomain.CurrentDomain.BaseDirectory}TempImage\\{referenceNo}";

                if (Directory.Exists(referenceFolder))
                {
                    Directory.Delete(referenceFolder, true);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        static void ResizeImage(string imagePath, string compressPath)
        {
            try
            {
                using (Bitmap originalImage = new Bitmap(imagePath))
                {
                    // Correct the orientation based on EXIF data
                    RotateImageBasedOnExifOrientation(originalImage);

                    int resizePercentage = 100;
                    int width = originalImage.Width;
                    int height = originalImage.Height;

                    if (width > 300 && width < 500)
                    {
                        resizePercentage = 50;
                    }
                    else if (width > 500 && width < 1000)
                    {
                        resizePercentage = 30;
                    }
                    else if (width > 1000 && width < 3000)
                    {
                        resizePercentage = 10;
                    }
                    else if (width > 3000)
                    {
                        resizePercentage = 5;
                    }

                    int newWidth = (int)(originalImage.Width * (resizePercentage / 100.0));
                    int newHeight = (int)(originalImage.Height * (resizePercentage / 100.0));

                    using (Bitmap resizedImage = new Bitmap(newWidth, newHeight))
                    {
                        using (Graphics g = Graphics.FromImage(resizedImage))
                        {
                            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            g.DrawImage(originalImage, 0, 0, newWidth, newHeight);
                        }

                        resizedImage.Save(compressPath, ImageFormat.Jpeg); // Adjust the format as needed
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        static void RotateImageBasedOnExifOrientation(Bitmap image)
        {
            const int ExifOrientationId = 0x112; // Exif ID for Orientation

            if (image.PropertyIdList.Contains(ExifOrientationId))
            {
                var prop = image.GetPropertyItem(ExifOrientationId);
                int orientationValue = BitConverter.ToUInt16(prop.Value, 0);

                RotateFlipType rotateFlipType = RotateFlipType.RotateNoneFlipNone;

                switch (orientationValue)
                {
                    case 2:
                        rotateFlipType = RotateFlipType.RotateNoneFlipX;
                        break;
                    case 3:
                        rotateFlipType = RotateFlipType.Rotate180FlipNone;
                        break;
                    case 4:
                        rotateFlipType = RotateFlipType.Rotate180FlipX;
                        break;
                    case 5:
                        rotateFlipType = RotateFlipType.Rotate90FlipX;
                        break;
                    case 6:
                        rotateFlipType = RotateFlipType.Rotate90FlipNone;
                        break;
                    case 7:
                        rotateFlipType = RotateFlipType.Rotate270FlipX;
                        break;
                    case 8:
                        rotateFlipType = RotateFlipType.Rotate270FlipNone;
                        break;
                }

                if (rotateFlipType != RotateFlipType.RotateNoneFlipNone)
                {
                    image.RotateFlip(rotateFlipType);
                    image.RemovePropertyItem(ExifOrientationId); // Remove the orientation property after correcting
                }
            }
        }
    }
}