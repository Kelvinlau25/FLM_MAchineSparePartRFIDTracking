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

namespace ReportModel
{
    public class MovementRpt
    {
        [Required(ErrorMessage = "* Please choose your report type.")]
        public int selectedId { get; set; }

        [Required(ErrorMessage = "* Please choose location.")]
        public int selectedLocId { get; set; }

        //public System.Web.Mvc.SelectList ReportName;
        public List<SelectListItem> DropdownReportName { get; set; }

        public List<SelectListItem> DropdownLocationName { get; set; }
        //[Required(ErrorMessage = "Date from is required")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        //public DateTime? DateFrom { get; set; }

        //[Required(ErrorMessage = "Date to is required")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        //public DateTime? DateTo { get; set; }
        public DataTable tableParent { get; set; }

        public string DateFrom { get; set; }
        public string DateTo { get; set; }

    }

    public class UnknownClass
    {
        public string RFID_ID { get; set; }
        public string SERIES_NO { get; set; }
        public string MODEL_NO { get; set; }
        public string PART_NO { get; set; }
        public string MACHINE_MODEL { get; set; }
        public string STORAGE_CODE { get; set; }
        public string STORAGE_DESCRIPTION_1 { get; set; }
        public string STORAGE_DESCRIPTION_2 { get; set; }
        public string MANUFACTURER { get; set; }
        public string TRANSACTION_TYPE { get; set; }
        public string STATUS { get; set; }
        public string CREATED_BY { get; set; }
        public string CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }

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
 
        public DataTable getReport(Int64 type, DateTime DateFrom, DateTime DateTo)
        {
     
            string sp = " ";

            if (type == 1)
            {
                sp = "SP_FILM_ERROR_RPT";
            }
            else if (type == 2)
            {
                sp = "SP_FILM_MOVEMENT_RPT";
            }
            else
            {
                sp = "SP_FILM_ERROR_LOG_RPT";
            }
            OpenConnection();
            command.CommandText = sp;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add(new SqlParameter("@dateFrom", DateFrom)).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@dateto", DateTo)).Direction = System.Data.ParameterDirection.Input;
            reader = command.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(reader);

            CloseReader();
            CloseConnection();

            return dt;
        }


    }
}