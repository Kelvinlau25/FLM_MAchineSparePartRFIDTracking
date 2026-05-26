using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace PAB_NewAquarium.Helpers
{
    public class ImageHelper
    {
        public static async Task<bool> InsertImageStoreProcedureSqlAsync(string sp, CommandType type, List<SqlParameter> ListofParam)
        {
            try
            {
                CommonFunction common = new CommonFunction();
                DataTable result = await common.PSP_COMMON_SQL(sp, type, ListofParam, "", "DEV_MVC");

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static async Task<bool> InsertImageStoreProcedureOracleAsync(string sp, CommandType type, List<OracleParameter> ListofParam)
        {
            try
            {
                CommonFunction common = new CommonFunction();
                DataTable result = await common.PSP_COMMON_ORA(sp, type, ListofParam, "", "DEV_NET");

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}