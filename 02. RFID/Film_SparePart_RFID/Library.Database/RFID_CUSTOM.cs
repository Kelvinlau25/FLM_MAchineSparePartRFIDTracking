using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Library.Database
{
    public class RFID_CUSTOM
    {
        // string ConnectionString_Ora = ConfigurationManager.ConnectionStrings["db_ora"].ToString();
        private string ConnectionString_MS = ConfigurationManager.ConnectionStrings["db_MS"].ToString();
        private DTO dto = new DTO();

        // Use for MS SQL TRANS
        private string pSP_MS = string.Empty; // Outdated

        public bool Check_M2_Grey_RFID(string RFID)
        {
            SqlConnection con = new SqlConnection(ConnectionString_MS);
            SqlCommand cmd = new SqlCommand("SP_CHK_M2_GREY_RFID", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;
            cmd.Parameters.Clear();
            cmd.Parameters.Add(new SqlParameter("@pRFID", RFID)).Direction = ParameterDirection.Input;
            cmd.Parameters.Add(new SqlParameter("@RETURN_VALUE", SqlDbType.VarChar, 1000)).Direction = ParameterDirection.Output;

            try
            {
                con.Open();
                cmd.ExecuteReader();

                if (cmd.Parameters["@RETURN_VALUE"].Value.ToString() == "1")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
        }

        public bool RFID_Filter(string RFID)
        {
            SqlConnection con = new SqlConnection(ConnectionString_MS);
            SqlCommand cmd = new SqlCommand("SP_CHK_FILTER_RFID", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;
            cmd.Parameters.Clear();
            cmd.Parameters.Add(new SqlParameter("@pRFID", RFID)).Direction = ParameterDirection.Input;
            cmd.Parameters.Add(new SqlParameter("@RETURN_VALUE", SqlDbType.VarChar, 1000)).Direction = ParameterDirection.Output;

            try
            {
                con.Open();
                cmd.ExecuteReader();

                if (cmd.Parameters["@RETURN_VALUE"].Value.ToString() == "1")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
        }
    }
}
