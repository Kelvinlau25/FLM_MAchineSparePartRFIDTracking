using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using PAB_NewAquarium.Models;

namespace PAB_NewAquarium.Helpers
{
    public class AuditTrailHelper
    {
        public static async Task<AuditTrailModel> AuditTrailStoreProcedureSqlAsync(string sp, CommandType type, List<SqlParameter> ListofParam, string DBName)
        {
            CommonFunction common = new CommonFunction();
            DataTable result = await common.PSP_COMMON_SQL(sp, type, ListofParam, "", DBName);

            List<string> keyNames = result.Columns.Cast<DataColumn>()
            .Select(column => column.ColumnName)
            .ToList();

            string jsonResult = JsonConvert.SerializeObject(result);

            AuditTrailModel dataModel = new AuditTrailModel
            {
                JsonData = jsonResult,
                ListData = keyNames
            };

            return dataModel;
        }
    }
}