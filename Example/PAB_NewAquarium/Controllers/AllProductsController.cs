using Newtonsoft.Json;
using PAB_NewAquarium.Filters;
using PAB_NewAquarium.Helpers;
using PAB_NewAquarium.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PAB_NewAquarium.Controllers
{
    public class AllProductsController : BaseController
    {
        readonly BlobHelper blobHelper = new BlobHelper();

        [SessionExpire]
        public async Task<ActionResult> AllProducts(string type = "", string user = "", string search = "")
        {
            try
            {
                // Fetch the product data based on user type, search keyword, and type (e.g., HotSales)
                AllProductsModel model = await GetAllProducts(user, search, type);
                ViewBag.LOGIN_USER = (user == "customer") ? "customer" : "";
                ViewBag.Title = (type == "HotSales") ? "Fabric Hot Sales" : "All Products";

                return View(model);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<AllProductsModel> GetAllProducts(string user, string search, string type)
        {
            try
            {
                // Initialize the model to store all fetched data
                AllProductsModel model = new AllProductsModel();

                // Set up parameters based on product type
                var obj = type == "HotSales"
                    ? new { pType = "MOM", pUser = user, pSearch = "" } // MOM: for HotSales
                    : new { pType = "ALL", pUser = user, pSearch = (search == "") ? "" : search }; // ALL: for general search

                // Call stored procedure to get all products with optional filters
                model.ALL_PRODUCTS = await common.PSP_COMMON_DAPPER<ALL_PRODUCTS>("PSP_GET_IMAGE_ALL_PRODUCTS", CommandType.StoredProcedure, obj);

                // Call stored procedure to get all filter lists using a multiple result set
                var (objModelA, objModelB, objModelC, objModelD, objModelE) = await common.PSP_COMMON_DAPPER_MULTIPLE<FABRIC_TYPE_LIST, PRODUCT_TYPE_LIST, MATERIAL_TYPE_LIST, WEAVE_TYPE_LIST, WEIGHT_LIST>("PSP_GET_PRODUCT_FILTER", CommandType.StoredProcedure, obj);

                // Assign filter data to model
                model.FABRIC_TYPE_LIST = objModelA;
                model.PRODUCT_TYPE_LIST = objModelB;
                model.MATERIAL_TYPE_LIST = objModelC;
                model.WEAVE_TYPE_LIST = objModelD;
                model.WEIGHT_LIST = objModelE;

                // Process each product's image URL to include '_Display_' prefix and secure it with a blob token
                foreach (var item in model.ALL_PRODUCTS)
                {
                    string[] split = item.IMAGE_URL.Split('/');
                    string imageName = split[split.Length - 1];
                    item.IMAGE_URL = item.IMAGE_URL.Replace(imageName, $"_Display_{imageName}");
                    item.IMAGE_URL = await blobHelper.SetBlobUrlToken(item.IMAGE_URL);
                }

                // Serialize product and filter data into JSON strings for use in front-end
                model.ALL_PRODUCTS_JSON = JsonConvert.SerializeObject(model.ALL_PRODUCTS);
                model.RECOMMENDED_PRODUCTS_JSON = JsonConvert.SerializeObject(model.RECOMMENDED_PRODUCTS);
                model.FABRIC_TYPE_JSON = JsonConvert.SerializeObject(model.FABRIC_TYPE_LIST);
                model.PRODUCT_TYPE_JSON = JsonConvert.SerializeObject(model.PRODUCT_TYPE_LIST);
                model.MATERIAL_TYPE_JSON = JsonConvert.SerializeObject(model.MATERIAL_TYPE_LIST);
                model.COMPOSITION_JSON = JsonConvert.SerializeObject(model.WEAVE_TYPE_LIST);
                model.WEIGHT_JSON = JsonConvert.SerializeObject(model.WEIGHT_LIST);

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ActionResult> GetSimilarityProducts(string user, string type)
        {
            try
            {
                // Get a configured HttpClient handler with proxy settings
                HttpClientHandler httpClientHandler = await proxy.GetHttpClientHandler();

                // Retrieve API key for the similarity matrix service from the database
                var param1 = new { INDICATOR = "PMatrixAPIKey" };
                string apiKey = await common.PSP_COMMON_DAPPER_SINGLE<string>("PSP_GET_MASTER_SETTING_VALUE2", CommandType.StoredProcedure, param1);

                // Read the API endpoint URL from config
                string apiUrl = ConfigurationManager.AppSettings["MATRIX_COMPARISON_API"];

                // Prepare the input data for similarity comparison (static for now, can be dynamic)
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

                // Get the list of items to compare against from the database
                var obj = new { pID = "", pUser = user };
                List<SimilarityCompareModel> similarityCompare = await common.PSP_COMMON_DAPPER<SimilarityCompareModel>("PSP_MATRIX_COMPARISON", CommandType.StoredProcedure, obj);

                // Proceed only if we have comparison data
                if (similarityCompare.Count != 0)
                {
                    // Build the full request model for the similarity API
                    SimilarityRequestModel similarity = new SimilarityRequestModel
                    {
                        api_key = apiKey,
                        input = similarityInput,
                        compareList = similarityCompare
                    };

                    // Serialize request to JSON and prepare HTTP content
                    var json = JsonConvert.SerializeObject(similarity);
                    var buffer = Encoding.UTF8.GetBytes(json);
                    var byteContent = new ByteArrayContent(buffer);
                    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    // Send POST request to the similarity API
                    HttpClient client = new HttpClient(handler: httpClientHandler, disposeHandler: true);
                    HttpResponseMessage response = client.PostAsync(apiUrl, byteContent).Result;

                    // Read and parse response
                    var resp = await response.Content.ReadAsStringAsync();
                    SimilarityResponseModel respObj = JsonConvert.DeserializeObject<SimilarityResponseModel>(resp);

                    // Sort similarity list by score descending and take top 10
                    respObj.similarityList = respObj.similarityList.Select(arr => arr).OrderByDescending(arr => (double)arr[1]).Take(10).ToList();

                    // Prepare DB call to fetch recommended product details
                    var obj2 = new { pID = string.Join(",", respObj.similarityList.Select(arr => arr[0].ToString())), pType = type };
                    List<RECOMMENDED_PRODUCTS> model = await common.PSP_COMMON_DAPPER<RECOMMENDED_PRODUCTS>("PSP_GET_RECOMMENDED_PRODUCT", CommandType.StoredProcedure, obj2);

                    // Format image URLs to secure blob URLs with display prefix
                    foreach (var item in model)
                    {
                        string[] split = item.IMAGE_URL.Split('/');
                        string imageName = split[split.Length - 1];
                        item.IMAGE_URL = item.IMAGE_URL.Replace(imageName, $"_Display_{imageName}");
                        item.IMAGE_URL = await blobHelper.SetBlobUrlToken(item.IMAGE_URL);
                    }

                    // Return the recommended products as JSON
                    return Json(model, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    // No comparison data found
                    return Json("Failed", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return Json("Failed", JsonRequestBehavior.AllowGet);
            }
        }
    }
}