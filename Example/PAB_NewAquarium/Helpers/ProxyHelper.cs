using System;
using System.Configuration;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace PAB_NewAquarium.Helpers
{
    public class ProxyHelper
    {
        readonly CommonFunction common = new CommonFunction();

        public async Task<HttpClientHandler> GetHttpClientHandler()
        {
            try
            {
                HttpClientHandler httpClientHandler = new HttpClientHandler();
                string isTest = ConfigurationManager.AppSettings["isTest"];

                if (isTest.ToUpper().ToString() == "TRUE")
                {
                    var param = new { INDICATOR = "PProxyEmail" };
                    string proxyEmail = await common.PSP_COMMON_DAPPER_SINGLE<string>("PSP_GET_MASTER_SETTING_VALUE2", CommandType.StoredProcedure, param);
                    string[] array = proxyEmail.Split('-');
                    string credentialUser = array[0];
                    string credentialPassword = array[1];

                    if (!String.IsNullOrEmpty(credentialUser) && !String.IsNullOrEmpty(credentialPassword))
                    {
                        var proxy = new WebProxy
                        {
                            Address = new Uri($"http://10.252.133.21:8080"),
                            BypassProxyOnLocal = false,
                            UseDefaultCredentials = false,

                            Credentials = new NetworkCredential(
                            userName: credentialUser,
                            password: credentialPassword)
                        };

                        httpClientHandler = new HttpClientHandler
                        {
                            Proxy = proxy,
                        };
                    }
                }

                return httpClientHandler;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}