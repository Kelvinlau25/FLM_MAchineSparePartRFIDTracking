using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using PAB_NewAquarium.Models;

namespace PAB_NewAquarium.Helpers
{
    public class WordDetectionHelper
    {
        public async Task<SimilarityResponseModel> RetrieveWordResponseAsync(WordDetectionModel model)
        {
            try
            {
                HttpClientHelper httpClientHelper = new HttpClientHelper();
                AutoKeyHelper autoKeyHelper = new AutoKeyHelper();
                AIAutoKeyResult autoKeyResult = await autoKeyHelper.AutoApiKeyRegistration("Word Detection API");
                var httpClientHandler = await httpClientHelper.GetHttpClientHandlerAsync();


                model.api_key = autoKeyResult.APIKey;
                var json = JsonConvert.SerializeObject(model);
                var buffer = Encoding.UTF8.GetBytes(json);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                // Create HttpClient with handler
                HttpClient client = new HttpClient(handler: httpClientHandler, disposeHandler: true);
                HttpResponseMessage response = await client.PostAsync(autoKeyResult.APIURL, byteContent);
                if (response.IsSuccessStatusCode)
                {
                    var wordDetectionAPIResponse = await response.Content.ReadAsStringAsync();
                    var wordDetectionAPIResult = JsonConvert.DeserializeObject<SimilarityResponseModel>(wordDetectionAPIResponse);

                    return wordDetectionAPIResult;
                }
                else
                {
                    return new SimilarityResponseModel()
                    {
                        score = -1
                    };
                }
            }
            catch (Exception)
            {
                return new SimilarityResponseModel()
                {
                    score = -1
                };
            }
        }
    }
}