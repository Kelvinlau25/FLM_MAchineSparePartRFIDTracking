using FILM_Sparepart_MVC.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;

namespace FILM_Sparepart_MVC.DAL
{
    public class ReaderRFID
    {
        private readonly string _connectionString;

        public ReaderRFID(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SQLCon")
                ?? throw new InvalidOperationException("Connection string 'SQLCon' not found.");
        }

        public ReaderRFIDModel GetReader(string readerName)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_FILM_GET_RFID_CONFIG", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pCOMPANY", SqlDbType.VarChar, 10).Value = "FILM";

                conn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        string location = Convert.ToString(dr["LOCATION"]);
                        string hostName = Convert.ToString(dr["HOST_NAME"]);
                        string ipAddress = Convert.ToString(dr["IP_ADDRESS"]);

                        if (string.Equals(location, readerName, StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(hostName, readerName, StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(ipAddress, readerName, StringComparison.OrdinalIgnoreCase))
                        {
                            return new ReaderRFIDModel
                            {
                                LOCATION   = location,
                                HOST_NAME  = hostName,
                                IP_ADDRESS = ipAddress,
                                PORT       = dr["PORT"] == DBNull.Value ? "5084" : Convert.ToString(dr["PORT"]),
                                SP         = Convert.ToString(dr["SP"]),
                                SERVER     = Convert.ToString(dr["SERVER"]),
                                SP2        = dr["SP2"] == DBNull.Value ? null : Convert.ToString(dr["SP2"]),
                                SERVER2    = dr["SERVER2"] == DBNull.Value ? null : Convert.ToString(dr["SERVER2"]),
                                RSSI       = dr["RSSI"] == DBNull.Value ? -70 : Convert.ToInt32(dr["RSSI"])
                            };
                        }
                    }
                }
            }

            return null;
        }
    }
}