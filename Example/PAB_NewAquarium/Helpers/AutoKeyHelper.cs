using Newtonsoft.Json;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System;
using PAB_NewAquarium.Models;
using System.Web;

namespace PAB_NewAquarium.Helpers
{
    public class AutoKeyHelper
    {
        public async Task<AIAutoKeyResult> AutoApiKeyRegistration(string APIName)
        {
            AIAutoKeyResult autoKeyAPIResult = new AIAutoKeyResult();

            try
            {
                autoKeyAPIResult = HttpContext.Current.Session["AIAutoKeyResult"] as AIAutoKeyResult;

                if (autoKeyAPIResult == null)
                {
                    string apiUrl = "";
                    if (ConfigurationManager.AppSettings["isTest"].ToLower() == "true")
                        apiUrl = ConfigurationManager.AppSettings["AUTO_KEY_TEST"];
                    else
                        apiUrl = ConfigurationManager.AppSettings["AUTO_KEY_LIVE"];

                    string companyCode = ConfigurationManager.AppSettings["CompanyCode_ACL"];
                    string systemName = ConfigurationManager.AppSettings["SystemName"];
                    AIAutoKey autoKey = new AIAutoKey
                    {
                        APIName = APIName,
                        SystemName = systemName,
                        CompanyName = companyCode
                    };
                    string jsonData = System.Text.Json.JsonSerializer.Serialize(autoKey);
                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    using (var apiUrlRequest = new HttpClient())
                    {
                        HttpResponseMessage autoKeyResponse = await apiUrlRequest.PostAsync(apiUrl, content);
                        if (autoKeyResponse.IsSuccessStatusCode)
                        {
                            var autoKeyAPIResponse = await autoKeyResponse.Content.ReadAsStringAsync();
                            autoKeyAPIResult = JsonConvert.DeserializeObject<AIAutoKeyResult>(autoKeyAPIResponse);
                            HttpContext.Current.Session["AIAutoKeyResult"] = autoKeyAPIResult;
                        }
                    }
                }

                return autoKeyAPIResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}