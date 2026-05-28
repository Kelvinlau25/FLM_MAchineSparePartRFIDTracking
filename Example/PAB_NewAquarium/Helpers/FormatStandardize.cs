using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using PAB_NewAquarium.Models;

namespace PAB_NewAquarium.Helpers
{
    public class FormatStandardize
    {
        public static DateFormatResponse dateFormatResponseModel = new DateFormatResponse();
        public static async Task<bool> dateFormatResponseAsync()
        {
            HttpClient client = new HttpClient();
            ByteArrayContent clientbodystr = new StringContent(JsonConvert.SerializeObject(dateFormatResponseModel), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(ConfigurationManager.AppSettings["ACL_API_2"] + "/api/v1/FormatStandardize/DateFormat", clientbodystr);
            if (response.IsSuccessStatusCode == true)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<DateFormatResponse>(responseBody);
                dateFormatResponseModel.DateFormat = result.DateFormat;
                dateFormatResponseModel.TimeFormat = result.TimeFormat;
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string convertDateFormat(string dateString)
        {
            string dateFormat = dateFormatResponseModel.DateFormat;
            string result = Convert.ToDateTime(dateString).ToString(dateFormat);
            return result;
        }

        public static string convertTimeFormat(string timeString)
        {
            string timeFormat = dateFormatResponseModel.TimeFormat;
            string result = Convert.ToDateTime(timeString).ToString(timeFormat);
            return result;
        }
    }
}