using PAB_NewAquarium.Helpers;
using System;
using System.Configuration;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace PAB_NewAquarium.Helpers
{
    public class HttpClientHelper : BaseController
    {
        public async Task<HttpClientHandler> GetHttpClientHandlerAsync()
        {
            if (ConfigurationManager.AppSettings["isTest"].ToLower() == "true")
            {
                object obj = new { pIndicator = "ProxyID", };
                string credentialUser = await common.PSP_COMMON_DAPPER_SINGLE<string>("PSP_GET_MASTER_SETTING_VALUE_NOTXML", CommandType.StoredProcedure, obj, "StdTemplate_DEV");
                object obj2 = new { pIndicator = "ProxyPass", };
                string credentialPassword = await common.PSP_COMMON_DAPPER_SINGLE<string>("PSP_GET_MASTER_SETTING_VALUE_NOTXML", CommandType.StoredProcedure, obj2, "StdTemplate_DEV");

                if (!String.IsNullOrEmpty(credentialUser) && !String.IsNullOrEmpty(credentialPassword))
                {
                    var proxy = new WebProxy
                    {
                        Address = new Uri($"http://10.252.133.21:8080"),
                        BypassProxyOnLocal = false,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(credentialUser, credentialPassword)
                    };

                    var httpClientHandler = new HttpClientHandler
                    {
                        Proxy = proxy,
                    };

                    return httpClientHandler;
                }
                else
                {
                    return new HttpClientHandler();
                }
            }
            else
            {
                return new HttpClientHandler();
            }
        }
    }
}