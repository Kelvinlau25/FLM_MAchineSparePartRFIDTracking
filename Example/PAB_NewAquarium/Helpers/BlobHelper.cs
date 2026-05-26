using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Azure.Core.Pipeline;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using PAB_NewAquarium.Models;
using System.IO;
using System.Data;
using System.Configuration;

namespace PAB_NewAquarium.Helpers
{
    public class BlobHelper
    {
        readonly CommonFunction common = new CommonFunction();

        public async Task<AzureBlobModel> GetAzureBlob()
        {
            try
            {
                string name = ConfigurationManager.AppSettings["isTest"].ToString().ToUpper().Trim() == "TRUE" ? ConfigurationManager.AppSettings["GENERAL_QR_DEV"].ToString() : ConfigurationManager.AppSettings["GENERAL_QR_LIVE"].ToString();
                string blobIndicator = ConfigurationManager.AppSettings["isTest"].ToString().ToUpper().Trim() == "TRUE" ? "PAB_NEW_AQUARIUM_DEV" : "PAB_NEW_AQUARIUM_PROD";
                var param = new { pIndicator = blobIndicator };
                AzureBlobModel model = await common.PSP_COMMON_DAPPER_SINGLE<AzureBlobModel>("PSP_GET_BLOB_INFO", CommandType.StoredProcedure, param, name);

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Dictionary<string, string>> UploadBobAsync(List<string> filepath, HttpContext context)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            try
            {
                AzureBlobModel azureBlob = await GetAzureBlob();
                BlobServiceClient blobServiceClient = GetBlobServiceClient(azureBlob);

                // Get a reference to a container
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(azureBlob.CONTAINER);
                if (await containerClient.ExistsAsync() == false)
                {
                    //create container
                    containerClient = await blobServiceClient.CreateBlobContainerAsync(azureBlob.CONTAINER);
                }

                foreach (string file in filepath)
                {
                    string fileName = Path.GetFileName(file);
                    string physicalPath = context.Server.MapPath(file);
                    // Get a reference to a blob
                    string finalPath = azureBlob.DIRECTORY + file.Replace("~/TempImage/", "");
                    BlobClient blobClient = containerClient.GetBlobClient(finalPath);
                    using (FileStream uploadFileStream = File.OpenRead(physicalPath))
                    {
                        await blobClient.UploadAsync(uploadFileStream, true);
                    }
                    dic.Add(fileName, blobClient.Uri.AbsoluteUri);
                }

                return dic;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> DeleteBlobAsync(string uri)
        {
            try
            {
                AzureBlobModel azureBlob = await GetAzureBlob();
                BlobServiceClient blobServiceClient = GetBlobServiceClient(azureBlob);

                Uri blobUri = new Uri(uri);
                var blobUriParts = new BlobUriBuilder(blobUri);

                string containerName = blobUriParts.BlobContainerName;
                string blobName = blobUriParts.BlobName;

                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                BlobClient blobClient = containerClient.GetBlobClient(blobName);

                await blobClient.DeleteIfExistsAsync();

                return "Delete Success";
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> SetBlobUrlToken(string url)
        {
            try
            {
                if (url.Contains("../") || url.Contains("~/")) { return url; }
                AzureBlobModel azureBlob = await GetAzureBlob();
                BlobServiceClient blobServiceClient = GetBlobServiceClient(azureBlob);

                string originalUrl = HttpUtility.UrlDecode(url);

                // Get a reference to a container
                string container = url.Replace(azureBlob.END_POINT, "").Split('/')[0];
                string blobName = originalUrl.Replace(azureBlob.END_POINT + container + "/", "");

                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(container);
                BlobClient blobClient = containerClient.GetBlobClient(blobName);
                string fileName = originalUrl.Split('/').Last();
                // Generate the SAS token
                BlobSasBuilder sasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = blobClient.BlobContainerName,
                    BlobName = blobClient.Name,
                    Resource = "b",
                    StartsOn = DateTimeOffset.UtcNow,
                    ExpiresOn = DateTimeOffset.UtcNow.AddDays(365),
                };

                sasBuilder.SetPermissions(BlobSasPermissions.Read);
                sasBuilder.ContentDisposition = "inline";
                sasBuilder.ContentType = getContentType(fileName);

                string sasToken = sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(azureBlob.AZURE_ACC_NAME, azureBlob.AZURE_KEY)).ToString();
                UriBuilder sasUriBuilder = new UriBuilder(blobClient.Uri)
                {
                    Query = sasToken
                };

                return sasUriBuilder.Uri.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string getContentType(string fileName)
        {
            try
            {
                string extension = Path.GetExtension(fileName)?.ToLower();
                switch (extension)
                {
                    case ".pdf":
                        return "application/pdf";
                    case ".mp4":
                        return "video/mp4";
                    case ".jpg":
                    case ".jpeg":
                        return "image/jpeg";
                    case ".png":
                        return "image/png";
                    // Add more cases as needed for other file types
                    default:
                        return "application/octet-stream"; // fallback to binary content type
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public BlobServiceClient GetBlobServiceClient(AzureBlobModel model)
        {
            string isTest = ConfigurationManager.AppSettings["isTest"].ToUpper();
            string proxyIP = ConfigurationManager.AppSettings["ProxyIP"];

            #region Pass proxy server in development
            if (isTest == "TRUE")
            {
                var proxy = new WebProxy
                {
                    Address = new Uri(proxyIP),
                    BypassProxyOnLocal = false,
                    UseDefaultCredentials = false,

                    Credentials = new NetworkCredential(
                    userName: model.CREDENTIAL_ID,
                    password: model.CREDENTIAL_PASS)
                };

                var httpClientHandler = new HttpClientHandler
                {
                    Proxy = proxy,
                    UseProxy = true
                };
                return new BlobServiceClient(
                    new Uri(model.END_POINT),
                    new StorageSharedKeyCredential(model.AZURE_ACC_NAME, model.AZURE_KEY),
                    new BlobClientOptions { Transport = new HttpClientTransport(httpClientHandler) }
                );
            }
            else
            {
                return new BlobServiceClient(
                    new Uri(model.END_POINT),
                    new StorageSharedKeyCredential(model.AZURE_ACC_NAME, model.AZURE_KEY)
                );
            }
            #endregion
        }
    }
}