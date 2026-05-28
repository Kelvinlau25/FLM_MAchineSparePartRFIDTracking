using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using FILM_Sparepart_MVC.Helper_Code;

namespace DatabaseModel
{
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
            var connectionString = AppConfig.GetConnectionString("DBAccess");
            
            // Log connection string for debugging (remove password for security)
            var logConnectionString = connectionString;
            if (!string.IsNullOrEmpty(logConnectionString) && logConnectionString.Contains("Password="))
            {
                var parts = logConnectionString.Split(';');
                logConnectionString = string.Join(";", parts.Where(p => !p.Trim().StartsWith("Password=", StringComparison.OrdinalIgnoreCase)));
            }
            Console.WriteLine($"[DatabaseModel.Database] Connecting with: {logConnectionString}");
            
            c = new SqlConnection(connectionString);
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
    public class Database1
    {
        protected string sql { get; set; }
        public SqlCommand command;
        protected SqlConnection c;
        public SqlDataReader reader;
        protected SqlTransaction tran;
        protected SqlDataAdapter sqladp;
        protected string Message;

        // constructor
        public void database1()
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
}