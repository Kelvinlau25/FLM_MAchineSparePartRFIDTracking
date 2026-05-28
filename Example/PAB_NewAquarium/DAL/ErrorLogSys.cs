using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace PAB_NewAquarium.DAL
{
    /// <summary>
    /// Application Error Message Collector 
    /// Connection to 
    /// </summary>

    public class ErrorLogSys : Database
    {
        public async Task<Boolean> ErrorLog_Add(string FunctionName,string ErrorMsg, string CreatedBy)
        {
            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_ERROR_LOGS", con))
                    {
                        con.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter("@pFunctionName", FunctionName)).Direction = System.Data.ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pErrorMSG", ErrorMsg)).Direction = System.Data.ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCreateBy", CreatedBy)).Direction = System.Data.ParameterDirection.Input;
                        reader = cmd.ExecuteReader();
                        reader.Close();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message.ToString());
                return false;
            }
        }
        public async Task<Boolean> ErrorLog_Add_V2(string FunctionName, Exception exData, string CreatedBy, string OptnalRef = "")
        {
            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_ERROR_LOGS", con))
                    {
                        con.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter("@pFunctionName", FunctionName)).Direction = System.Data.ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pErrorMSG", OptnalRef + exData.Message.ToString())).Direction = System.Data.ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pSource", exData.Source.ToString())).Direction = System.Data.ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pStackTrace", exData.StackTrace.ToString())).Direction = System.Data.ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pTargetSite", exData.TargetSite.ToString())).Direction = System.Data.ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCreateBy", CreatedBy)).Direction = System.Data.ParameterDirection.Input;
                        reader = cmd.ExecuteReader();
                        reader.Close();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message.ToString());
                return false;
            }
        }
    }
}