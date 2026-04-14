using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Library.Database
{
    public class RFID_YARN
    {
        private DTO dto = new DTO();
        private RFID_COMMON db = new RFID_COMMON();
        // string ConnectionString_YARN = ConfigurationManager.ConnectionStrings["db_OraYarnM1"].ToString();
        // string ConnectionString_APPS = ConfigurationManager.ConnectionStrings["db_OraApps"].ToString();
        private string ConnectionString_MS = ConfigurationManager.ConnectionStrings["db_MS"].ToString();

        private string ShipmentNumber = string.Empty;
        private string ItemNumber = string.Empty;
        private double transQty = 0;
        private double palletQty = 0;
        private double perWeight = 0;
        private double perPallet = 0;
        private double perCone = 0;
        private string suppCode = string.Empty;

        public DTO MainYarnRFID(string RFID, string Reader, string IPAddress, string SP)
        {
            DTO result = new DTO();

            if (SP == "SP_M2_YARN_RECEIVING_CHECK")
            {
                // result = CheckYarnDtl(RFID, Reader, IPAddress, SP);
            }
            else if (SP == "SP_M2_YARN_ISSUING_MOV_MAINT")
            {
                result = UpdYarnInvMov_Iss(RFID, Reader, IPAddress, SP);
            }
            else if (SP == "SP_M2_YARN_ISSUING_PROD_REC")
            {
                result = UpdYarnMov_ProdRec(RFID, Reader, IPAddress, SP);
            }
            else if (SP == "SP_TEST_RFID")
            {
                result = db.RFID_Common_MSSQL(string.Empty, RFID, Reader, IPAddress, SP);
            }

            return result;
        }

        public DTO Get_ImportYarn_Dtl(string RFID)
        {
            DTO result = new DTO();
            DataTable dt = new DataTable();
            SqlConnection con = new SqlConnection(ConnectionString_MS);
            SqlCommand cmd = new SqlCommand("SP_GET_M2RFID_IMPORT_YARN", con);
            SqlDataReader rdr;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;
            cmd.Parameters.Clear();
            cmd.Parameters.Add(new SqlParameter("@pRFID", RFID)).Direction = ParameterDirection.Input;
            cmd.Parameters.Add(new SqlParameter("@RETURN_VALUE", SqlDbType.VarChar, 1000)).Direction = ParameterDirection.Output;

            try
            {
                con.Open();
                rdr = cmd.ExecuteReader();
                dt.Load(rdr);

                if (cmd.Parameters["@RETURN_VALUE"].Value.ToString() == "0")
                {
                    throw new Exception("No Data Found on Import Yarn");
                }

                string InvoiceNo;
                string ITEM_NUMBER;
                string WEIGHT;
                string CONES;
                string SUPP;

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        InvoiceNo = dr["INVOICE_NO"].ToString();
                        ITEM_NUMBER = dr["ITEM_CODE"].ToString();
                        WEIGHT = dr["WEIGHT"].ToString();
                        CONES = dr["UNIT_QTY"].ToString();
                        SUPP = dr["SUPPLIER_CODE"].ToString();

                        ShipmentNumber = InvoiceNo;
                        ItemNumber = ITEM_NUMBER;
                        perWeight = Convert.ToDouble(WEIGHT);
                        perCone = Convert.ToDouble(CONES);
                        suppCode = SUPP;
                    }

                    result.Error = false;
                }
                else
                {
                    result.Error = true;
                }
            }
            catch (Exception ex)
            {
                result.Error = true;
                result.ErrorMessage = ex.Message;
            }
            finally
            {
                con.Close();
                con.Dispose();
            }

            return result;
        }

        public DTO UpdYarnInvMov_Rec(string RFID, string Reader, string IPAddress, string InvoiceNo,
            string ItemCode, string Source, double PltQty, double NoOfPlt, double TotalCone)
        {
            DTO result = new DTO();
            dto = new DTO();
            SqlConnection con = new SqlConnection(ConnectionString_MS);
            SqlCommand cmd = new SqlCommand("SP_M2_YARN_RECEIVING_MOV_MAINT", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;
            cmd.Parameters.Clear();
            cmd.Parameters.Add(new SqlParameter("@pRFID", RFID)).Direction = ParameterDirection.Input;
            cmd.Parameters.Add(new SqlParameter("@pREADER", Reader)).Direction = ParameterDirection.Input;
            cmd.Parameters.Add(new SqlParameter("@pIPADDR", IPAddress)).Direction = ParameterDirection.Input;
            cmd.Parameters.Add(new SqlParameter("@pInvoiceNo", InvoiceNo)).Direction = ParameterDirection.Input;
            cmd.Parameters.Add(new SqlParameter("@pITEMCODE", ItemCode)).Direction = ParameterDirection.Input;
            cmd.Parameters.Add(new SqlParameter("@pSOURCE", Source)).Direction = ParameterDirection.Input;
            cmd.Parameters.Add(new SqlParameter("@pPLTQTY", PltQty)).Direction = ParameterDirection.Input;
            cmd.Parameters.Add(new SqlParameter("@pNoOfPlt", NoOfPlt)).Direction = ParameterDirection.Input;
            cmd.Parameters.Add(new SqlParameter("@pTOTALCONE", TotalCone)).Direction = ParameterDirection.Input;
            cmd.Parameters.Add(new SqlParameter("@RETURN_VALUE", SqlDbType.VarChar, 1000)).Direction = ParameterDirection.Output;

            try
            {
                con.Open();
                cmd.ExecuteReader();

                if (cmd.Parameters["@RETURN_VALUE"].Value.ToString() == "0")
                {
                    throw new Exception("Fail Update Yarn Movemnt & Yarn Inventory");
                }

                result.Error = false;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = true;
                result.ErrorMessage = ex.Message;
                return result;
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
        }

        public DTO UpdYarnInvMov_Iss(string RFID, string Reader, string IPAddress, string SP)
        {
            DTO result = new DTO();

            SqlConnection con = new SqlConnection(ConnectionString_MS);
            SqlCommand cmd = new SqlCommand(SP, con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;
            cmd.Parameters.Clear();
            cmd.Parameters.Add(new SqlParameter("@pRFID", RFID)).Direction = ParameterDirection.Input;
            cmd.Parameters.Add(new SqlParameter("@pREADER", Reader)).Direction = ParameterDirection.Input;
            cmd.Parameters.Add(new SqlParameter("@pIPADDR", IPAddress)).Direction = ParameterDirection.Input;
            cmd.Parameters.Add(new SqlParameter("@RETURN_VALUE", SqlDbType.VarChar, 1000)).Direction = ParameterDirection.Output;

            try
            {
                con.Open();
                cmd.ExecuteReader();

                if (cmd.Parameters["@RETURN_VALUE"].Value.ToString() == "0")
                {
                    throw new Exception("Fail Update Yarn Movemnt & Yarn Inventory");
                }

                result.Error = false;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = true;
                result.ErrorMessage = ex.Message;
                return result;
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
        }

        public DTO UpdYarnMov_ProdRec(string RFID, string Reader, string IPAddress, string SP)
        {
            DTO result = new DTO();

            SqlConnection con = new SqlConnection(ConnectionString_MS);
            SqlCommand cmd = new SqlCommand(SP, con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;
            cmd.Parameters.Clear();
            cmd.Parameters.Add(new SqlParameter("@pRFID", RFID)).Direction = ParameterDirection.Input;
            cmd.Parameters.Add(new SqlParameter("@pREADER", Reader)).Direction = ParameterDirection.Input;
            cmd.Parameters.Add(new SqlParameter("@pIPADDR", IPAddress)).Direction = ParameterDirection.Input;
            cmd.Parameters.Add(new SqlParameter("@RETURN_VALUE", SqlDbType.VarChar, 1000)).Direction = ParameterDirection.Output;

            try
            {
                con.Open();
                cmd.ExecuteReader();

                if (cmd.Parameters["@RETURN_VALUE"].Value.ToString() == "0")
                {
                    throw new Exception("Fail Update Yarn Movemnt & Raw Material Requisition");
                }

                result.Error = false;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = true;
                result.ErrorMessage = ex.Message;
                return result;
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
        }
    }
}
