using Oracle.ManagedDataAccess.Client;
using System;
using System.Configuration;
using System.Data.SqlClient;
using PAB_NewAquarium.Helpers.DataModel;
using System.Threading.Tasks;
using System.Collections.Concurrent;


namespace PAB_NewAquarium.DAL
{
    /// <summary>
    /// Database Connection String Retrieve Module
    /// Sample Module to connect database connection 
    /// </summary>

    public class Database
    {
        protected string sql { get; set; }
        protected SqlCommand command;
        protected SqlConnection c;
        public SqlDataReader reader;
        protected SqlTransaction tran;
        protected SqlDataAdapter sqladp;
        protected OracleCommand commandORA;
        protected OracleConnection cORA;
        public OracleDataReader readerORA;
        protected OracleTransaction tranORA;
        protected OracleDataAdapter sqladpORA;
        protected string Message;
        protected string ConnectionName;

        private static readonly ConcurrentDictionary<string, string> dic = new ConcurrentDictionary<string, string>();

        public async Task<string> GetConnectionStringAsync(bool isACL = false, string dbName = null)
        {
            try
            {
                dbName = string.IsNullOrWhiteSpace(dbName) ? GetSystemName(isACL) : dbName;
                string connectionString = GetConnectionString(key: dbName);

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    DatabaseConnectionString dbConnectionString = new DatabaseConnectionString();
                    connectionString = await dbConnectionString.OpenAclConnection(dbName);
                    AddConnectionString(key: dbName, value: connectionString);
                }

                return connectionString;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private string GetSystemName(bool isACL)
        {
            try
            {
                if (isACL)
                {
                    return ConfigurationManager.AppSettings["ACL"];
                }
                else
                {
                    string isTest = ConfigurationManager.AppSettings["isTest"];

                    if (string.Equals(isTest, "TRUE", StringComparison.OrdinalIgnoreCase))
                    {
                        return ConfigurationManager.AppSettings["DEV"];
                    }
                    else
                    {
                        return ConfigurationManager.AppSettings["LIVE"];
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetConnectionString(string key)
        {
            try
            {
                if (dic.TryGetValue(key, out string value))
                {
                    return value;
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AddConnectionString(string key, string value)
        {
            try
            {
                dic.TryAdd(key, value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}