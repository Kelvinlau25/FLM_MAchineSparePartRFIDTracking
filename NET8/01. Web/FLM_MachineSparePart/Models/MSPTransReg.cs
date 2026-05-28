using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using FILM_Sparepart_MVC.Helper_Code;
using FILM_Sparepart_MVC.Helper_Code.Objects;

namespace MSPTransReg
{
    public class Transaction
    {
        public int TRANSACTION_ID { get; set; }
        public int SPARE_PART_MODEL_ID { get; set; }
        [Required(ErrorMessage = "* Please Select Model No.")]
        public string Model_No { get; set; }
        public string Series_No { get; set; }
        [Required(ErrorMessage = "* Please Enter Reason.")]
        public string Trans_Reason { get; set; }
        public string Trans_Flow { get; set; }
        public int Indicators { get; set; }
        public string Status { get; set; }
        public DateTime Created_Date { get; set; }
        public string Record_Type { get; set; }
        [Required(ErrorMessage = "* Please Select Transaction Type.")]
        public int Trans_Type_ID { get; set; }
        public string Trans_Name { get; set; }
        public string Part_No { get; set; }
        public int Spare_Part_Machine_ID { get; set; }
        public string Machine_Model { get; set; }
        public string RFID_Type { get; set; }
        public string RFID_Tag_ID { get; set; }
        public int Storage_ID { get; set; }
        public string Storage_Code { get; set; }
        public string Storage_Desc_1 { get; set; }
        public string Storage_Desc_2 { get; set; }
        public int Manufacturer_ID { get; set; }
        public string Manufacturer_Code { get; set; }
        public string Remarks { get; set; }
        public string Created_By { get; set; }
        public string Created_Loc { get; set; }
        public string Updated_By { get; set; }
        public DateTime Updated_Date { get; set; }
        public string Updated_Loc { get; set; }
        public string SEARCH_VALUE { get; set; }

        public string QTY_ON_HAND { get; set; }
        public string QTY_ON_REQUEST { get; set; }       
        public string QTY_TRANS { get; set; }

        public List<SelectListItem> DropdownTransType { get; set; }
    }

    public class SparePartModel
    {
        public int SPARE_PART_MODEL_ID { get; set; }        
        public string Model_No { get; set; }
        public string Remarks { get; set; }
        public string Record_Type { get; set; }
        public string Created_By { get; set; }
        public DateTime Created_Date { get; set; }
        public string Created_Loc { get; set; }
        public string Updated_By { get; set; }
        public DateTime Updated_Date { get; set; }
        public string Updated_Loc { get; set; }
        public string SEARCH_VALUE { get; set; }

    }

    public class Qty
    {
        public string OnHand { get; set; }
        public string OnRequest { get; set; }
    }

    public class Database
    {
        protected string sql { get; set; }
        protected SqlCommand command;
        protected SqlConnection c;
        public SqlDataReader reader;
        protected SqlTransaction tran;
        protected SqlDataAdapter sqladp;
        protected string Message;

        // constructor
        public void database()
        {
        }
        // method
        public void OpenConnection()
        {
            command = new SqlCommand();
            c = new SqlConnection(AppConfig.GetConnectionString("SQLCon"));
            command.Connection = c;
            c.Open();
        }
        public string ExecuteNonQuery()
        {
            string i = null;
            try
            {
                command.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                i = e.Message;
            }
            return i;
        }
        public void ExecuteReader()
        {
            try
            {
                reader = command.ExecuteReader();
            }
            catch (SqlException e)
            {
                Message = e.Message;
            }
        }
        public void CloseReader()
        {
            reader.Close();
            reader.Dispose();
            reader = null;
        }
        public void CloseConnection()
        {
            c.Close();
            c.Dispose();
            c = null;
        }
    }
    public class DB : Database
    {
        public DB()
        {

        }

        public DataTable List(string Table, string TableID, string Search,
        string Value, string SortField, string Direction,
        string FrmRowno, string ToRowno, string Deleted)
        {
            OpenConnection();
            command.CommandText = "PSP_COMMON_LIST";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add(new SqlParameter("@Table", Table)).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@TableID", TableID)).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@Search", Search)).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@Value", Value)).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@SortField", SortField)).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@Direction", Direction)).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@FrmRowno", FrmRowno)).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@ToRowno", ToRowno)).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@Deleted", Deleted)).Direction = System.Data.ParameterDirection.Input;
            reader = command.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(reader);

            CloseReader();
            CloseConnection();

            return dt;
        }

        public DataTable ListModelNo(string Value)
        {
            OpenConnection();
            command.CommandText = " select * from PVIEW_MM_SP_MODEL WHERE " + Value + " AND RECORD_TYPE <> 5";
            command.CommandType = CommandType.Text;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
         
            reader = command.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(reader);

            CloseReader();
            CloseConnection();

            return dt;
        }

        public Transaction getTransactionData(string id)
        {
            OpenConnection();
            command.CommandText = "SP_FILM_SP_TRAN_REG_SEL";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add(new SqlParameter("@pID", id)).Direction = System.Data.ParameterDirection.Input;
            reader = command.ExecuteReader();

            Transaction Transaction = null;

            while (reader.Read())
            {
                Transaction = new Transaction();
                Transaction.TRANSACTION_ID = Convert.ToInt32(reader["TRANSACTION_ID"]);
                Transaction.SPARE_PART_MODEL_ID = Convert.ToInt32(reader["SPARE_PART_MODEL_ID"]);
                Transaction.Model_No = reader["MODEL_NO"].ToString();
                Transaction.Series_No = reader["SERIES_NO"].ToString();
                Transaction.Trans_Reason = reader["TRANS_REASON"].ToString();
                Transaction.Trans_Flow = reader["TRANS_FLOW"].ToString();
                Transaction.Indicators = Convert.ToInt32(reader["INDICATORS"]);
                Transaction.Trans_Name = reader["TRANS_NAME"].ToString();
                Transaction.Trans_Type_ID = Convert.ToInt32(reader["TRANS_TYPE_ID"]);
                Transaction.Status = reader["STATUS"].ToString();
                Transaction.Remarks = reader["REMARKS"].ToString();
                Transaction.Created_By = reader["CREATED_BY"].ToString();
                Transaction.Created_Date = Convert.ToDateTime(reader["CREATED_DATE"]);
                Transaction.Created_Loc = reader["CREATED_LOC"].ToString();
                Transaction.Updated_By = reader["UPDATED_BY"].ToString();
                Transaction.Updated_Date = Convert.ToDateTime(reader["UPDATED_DATE"]);
                Transaction.Updated_Loc = reader["UPDATED_LOC"].ToString();
            }

            CloseReader();
            CloseConnection();

            return Transaction;

        }
        public string TransactionRegMaint(Transaction model, String record_typ, String loc, String pc)
        {
            string result = "";
            string para5 = "";
            string para6 = "";
            string para7 = "";
            string para8 = "";
            string return_value = "0";

            OpenConnection();
            try
            {
                command.CommandText = "SP_FILM_TRAN_REG_MAINT";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@para1", model.TRANSACTION_ID)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para2", model.SPARE_PART_MODEL_ID)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para3", model.Trans_Type_ID)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para4", model.Trans_Reason)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para5", para5)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para6", para6)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para7", para7)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para8", para8)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@REMARKS", model.Remarks)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@PRECTYPE", record_typ)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@PCREATEDBY", pc)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@PCREATEDLOC", loc)).Direction = System.Data.ParameterDirection.Input;
                command.ExecuteScalar();
                return_value = "1";

                //return_value = command.ExecuteScalar().ToString();
                if (Convert.ToInt32(return_value) > 0)
                {
                    if (record_typ == "5")
                    {
                        result = "Data is successfully deleted.";
                    }
                    else
                    {
                        result = "Data is successfully saved.";
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                return result = ex.Message;
            }
            finally
            {

                CloseConnection();
            }

        }

        public DataTable getTransTypeLst()
        {
            OpenConnection();
            command.CommandText = "SP_FILM_GET_TRAN_CAT";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            reader = command.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(reader);

            CloseReader();
            CloseConnection();

            return dt;

        }

        public String GetQty(string modelID, string transID, Qty model)
        {
            string result = "";
            OpenConnection();
            command.CommandText = "SP_FILM_GET_MODEL_QUANTITY";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add(new SqlParameter("@pModelID", modelID)).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@pTranTID", transID)).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@pRETURN_VALUE1", SqlDbType.NVarChar, 50)).Direction = System.Data.ParameterDirection.Output;
            command.Parameters.Add(new SqlParameter("@pRETURN_VALUE2", SqlDbType.NVarChar, 50)).Direction = System.Data.ParameterDirection.Output;
            command.ExecuteNonQuery();
            result = "1";            

            model.OnHand = command.Parameters["@pRETURN_VALUE1"].Value.ToString() ;
            model.OnRequest = command.Parameters["@pRETURN_VALUE2"].Value.ToString();

            //result = command.Parameters["@pRETURN_VALUE1"].Value.ToString();            
            CloseConnection();

            return result;            
        }
    }
}