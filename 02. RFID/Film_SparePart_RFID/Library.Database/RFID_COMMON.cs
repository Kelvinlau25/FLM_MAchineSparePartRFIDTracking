using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Library.Database
{
    public class RFID_COMMON
    {
        // string ConnectionString_Ora = ConfigurationManager.ConnectionStrings["db_ora"].ToString();
        private string ConnectionString_MS = ConfigurationManager.ConnectionStrings["db_MS"].ToString();
        private string ConnectionString_MS_Tower = ConfigurationManager.ConnectionStrings["db_MS_Tower"].ToString();
        private DTO dto = new DTO();

        // Use for MS SQL TRANS
        private string pSP_MS = string.Empty; // Outdated

        public DTO RFID_Common_MSSQL(string mode, string RFID, string Reader, string IPAddress, string SP)
        {
            dto = new DTO();
            SqlConnection con = new SqlConnection(ConnectionString_MS);
            SqlCommand cmd = new SqlCommand(SP, con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;
            cmd.Parameters.Clear();
            cmd.Parameters.Add(new SqlParameter("@pTRAN_TYPE", mode)).Direction = ParameterDirection.Input;
            cmd.Parameters.Add(new SqlParameter("@pRFID", RFID)).Direction = ParameterDirection.Input;
            cmd.Parameters.Add(new SqlParameter("@pRETURN_VALUE1", SqlDbType.NVarChar, 4000)).Direction = ParameterDirection.Output;
            // cmd.Parameters.Add(new SqlParameter("@pRFID", RFID)).Direction = ParameterDirection.Input;
            // cmd.Parameters.Add(new SqlParameter("@pREADER", Reader)).Direction = ParameterDirection.Input;
            cmd.Parameters.Add(new SqlParameter("@pIPADDR", IPAddress)).Direction = ParameterDirection.Input;
            // cmd.Parameters.Add(new SqlParameter("@RETURN_ERROR", SqlDbType.VarChar, 1000)).Direction = ParameterDirection.Output;

            try
            {
                con.Open();
                cmd.ExecuteNonQuery();

                if (cmd.Parameters["@pRETURN_VALUE1"].Value.ToString() != "0")
                {
                    throw new Exception(cmd.Parameters["@pRETURN_VALUE1"].Value.ToString());
                }

                dto.Error = false;
                return dto;
            }
            catch (Exception ex)
            {
                dto.Error = true;
                dto.ErrorMessage = ex.Message;
                return dto;
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
        }

        public DTO RFID_Common_MSSQL_Tower(string mode, string RFID, string Reader, string IPAddress, string SP)
        {
            dto = new DTO();
            SqlConnection con = new SqlConnection(ConnectionString_MS_Tower);
            SqlCommand cmd = new SqlCommand(SP, con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;
            cmd.Parameters.Clear();
            cmd.Parameters.Add(new SqlParameter("@pTRAN_TYPE", mode)).Direction = ParameterDirection.Input;
            cmd.Parameters.Add(new SqlParameter("@pRFID", RFID)).Direction = ParameterDirection.Input;
            cmd.Parameters.Add(new SqlParameter("@pRETURN_VALUE1", SqlDbType.NVarChar, 4000)).Direction = ParameterDirection.Output;
            // cmd.Parameters.Add(new SqlParameter("@pRFID", RFID)).Direction = ParameterDirection.Input;
            // cmd.Parameters.Add(new SqlParameter("@pREADER", Reader)).Direction = ParameterDirection.Input;
            // cmd.Parameters.Add(new SqlParameter("@pIPADDR", IPAddress)).Direction = ParameterDirection.Input;
            // cmd.Parameters.Add(new SqlParameter("@RETURN_ERROR", SqlDbType.VarChar, 1000)).Direction = ParameterDirection.Output;

            try
            {
                con.Open();
                cmd.ExecuteNonQuery();

                if (cmd.Parameters["@pRETURN_VALUE1"].Value.ToString() != "0")
                {
                    throw new Exception(cmd.Parameters["@pRETURN_VALUE1"].Value.ToString());
                }

                dto.Error = false;
                return dto;
            }
            catch (Exception ex)
            {
                dto.Error = true;
                dto.ErrorMessage = ex.Message;
                return dto;
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
        }

        public DTO RFID_Common_MSSQL2(string mode, string RFID, string SP)
        {
            dto = new DTO();
            SqlConnection con = new SqlConnection(ConnectionString_MS);
            SqlCommand cmd = new SqlCommand(SP, con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;
            cmd.Parameters.Clear();
            cmd.Parameters.Add(new SqlParameter("@pTRAN_TYPE", mode)).Direction = ParameterDirection.Input;
            cmd.Parameters.Add(new SqlParameter("@pRFID", RFID)).Direction = ParameterDirection.Input;
            cmd.Parameters.Add(new SqlParameter("@pRETURN_VALUE1", SqlDbType.NVarChar, 4000)).Direction = ParameterDirection.Output;

            try
            {
                con.Open();
                cmd.ExecuteNonQuery();

                if (cmd.Parameters["@pRETURN_VALUE1"].Value.ToString() != "0")
                {
                    throw new Exception(cmd.Parameters["@pRETURN_VALUE1"].Value.ToString());
                }

                dto.Error = false;
                return dto;
            }
            catch (Exception ex)
            {
                dto.Error = true;
                dto.ErrorMessage = ex.Message;
                return dto;
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
        }

        public DTO RFID_SendMail_MSSQL(string MailBody, string MailSubject, string MailTo)
        {
            dto = new DTO();

            SqlConnection con = new SqlConnection(ConnectionString_MS);
            SqlCommand cmd = new SqlCommand("SEND_HTML_EMAIL2", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;

            cmd.Parameters.Clear();
            cmd.Parameters.Add(new SqlParameter("@subject", MailSubject)).Direction = ParameterDirection.Input;
            cmd.Parameters.Add(new SqlParameter("@MSG", MailBody)).Direction = ParameterDirection.Input;
            cmd.Parameters.Add(new SqlParameter("@emailto", MailTo)).Direction = ParameterDirection.Input;

            try
            {
                con.Open();
                cmd.ExecuteReader();

                dto.Error = false;
                return dto;
            }
            catch (Exception ex)
            {
                dto.Error = true;
                dto.ErrorMessage = ex.Message;
                return dto;
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
        }

        public DTO GET_RFID_CONFIG(string vCompany)
        {
            dto = new DTO();
            SqlConnection con = new SqlConnection(ConnectionString_MS);
            SqlCommand cmd = new SqlCommand("SP_FILM_GET_RFID_CONFIG", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;
            cmd.Parameters.Clear();
            cmd.Parameters.Add(new SqlParameter("@pCOMPANY", vCompany)).Direction = ParameterDirection.Input;

            try
            {
                con.Open();
                DataTable tbl = new DataTable();
                tbl.Load(cmd.ExecuteReader());
                dto.Table = tbl;
                dto.Error = false;
                return dto;
            }
            catch (Exception ex)
            {
                dto.Error = true;
                dto.ErrorMessage = ex.Message;
                return dto;
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
        }
    }
}
