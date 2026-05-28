using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using FILM_Sparepart_MVC.Models;

namespace FILM_Sparepart_MVC.Services
{
    /// <summary>
    /// Manages RFID reader configurations and connection state.
    /// Loads reader definitions from the database via SP_FILM_GET_RFID_CONFIG
    /// and executes stored procedures when tags are scanned.
    /// </summary>
    public class RFIDService
    {
        private static readonly Lazy<RFIDService> _instance =
            new Lazy<RFIDService>(() => new RFIDService());

        public static RFIDService Instance
        {
            get { return _instance.Value; }
        }

        private readonly string _connectionString;
        private readonly ConcurrentDictionary<string, ReaderRFIDModel> _readers;
        private readonly ConcurrentDictionary<string, bool> _readerConnected;

        public RFIDService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["SQLCon"].ConnectionString;
            _readers = new ConcurrentDictionary<string, ReaderRFIDModel>(StringComparer.OrdinalIgnoreCase);
            _readerConnected = new ConcurrentDictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

            LoadReadersFromDatabase();
        }

        /// <summary>
        /// Loads reader configurations from the database by calling SP_FILM_GET_RFID_CONFIG.
        /// </summary>
        private void LoadReadersFromDatabase()
        {
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SP_FILM_GET_RFID_CONFIG", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 30;
                    cmd.Parameters.Add(new SqlParameter("@pCOMPANY", "FILM"));

                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        var table = new DataTable();
                        table.Load(reader);

                        int id = 0;
                        foreach (DataRow row in table.Rows)
                        {
                            var model = new ReaderRFIDModel
                            {
                                ID = id++,
                                IP_ADDRESS = row["IP_ADDRESS"].ToString(),
                                HOST_NAME = row["HOST_NAME"].ToString(),
                                PORT = row["PORT"].ToString(),
                                SP = row["SP"].ToString(),
                                SERVER = row["SERVER"].ToString(),
                                SP2 = row.Table.Columns.Contains("SP2") ? row["SP2"].ToString() : string.Empty,
                                SERVER2 = row.Table.Columns.Contains("SERVER2") ? row["SERVER2"].ToString() : string.Empty,
                                COMPANY = row.Table.Columns.Contains("COMPANY") ? row["COMPANY"].ToString() : string.Empty,
                                LOCATION = row.Table.Columns.Contains("LOCATION") ? row["LOCATION"].ToString() : string.Empty,
                                RSSI = row.Table.Columns.Contains("RSSI") ? Convert.ToInt32(row["RSSI"]) : -70
                            };

                            _readers[model.IP_ADDRESS] = model;
                            _readerConnected[model.IP_ADDRESS] = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("RFIDService.LoadReadersFromDatabase error: " + ex.Message);
            }
        }

        /// <summary>
        /// Returns the list of all configured RFID readers.
        /// </summary>
        public List<ReaderRFIDModel> GetDefaultReaders()
        {
            if (_readers.IsEmpty)
            {
                LoadReadersFromDatabase();
            }
            return _readers.Values.ToList();
        }

        /// <summary>
        /// Returns the connection status for all configured readers.
        /// </summary>
        public List<ReaderStatusModel> GetReaderStatuses()
        {
            return _readers.Values.Select(r => new ReaderStatusModel
            {
                IpAddress = r.IP_ADDRESS,
                HostName = r.HOST_NAME,
                Location = r.LOCATION,
                IsConnected = _readerConnected.ContainsKey(r.IP_ADDRESS) && _readerConnected[r.IP_ADDRESS]
            }).ToList();
        }

        /// <summary>
        /// Attempts to connect a reader by IP address.
        /// Calls the reader's stored procedure with a "CONNECT" transaction type.
        /// </summary>
        public Task<ConnectResultModel> ConnectReaderByIpAsync(string ip)
        {
            return Task.Run(() =>
            {
                var result = new ConnectResultModel { IpAddress = ip };

                bool isConnected;
                if (_readerConnected.TryGetValue(ip, out isConnected) && isConnected)
                {
                    result.Status = ConnectStatus.AlreadyConnected;
                    result.Message = "Reader is already connected.";
                    return result;
                }

                ReaderRFIDModel reader;
                if (!_readers.TryGetValue(ip, out reader))
                {
                    result.Status = ConnectStatus.Failed;
                    result.Message = "Reader not found in configuration.";
                    return result;
                }

                try
                {
                    // Execute the stored procedure to register the connection
                    ExecuteReaderStoredProcedure("CONNECT", string.Empty, reader.HOST_NAME, ip, reader.SP);
                    _readerConnected[ip] = true;
                    result.Status = ConnectStatus.Connected;
                    result.Message = "Connected successfully.";
                }
                catch (Exception ex)
                {
                    result.Status = ConnectStatus.Failed;
                    result.Message = "Connect failed: " + ex.Message;
                }

                return result;
            });
        }

        /// <summary>
        /// Disconnects a reader by IP address.
        /// </summary>
        public void DisconnectReaderByIp(string ip)
        {
            _readerConnected[ip] = false;
        }

        /// <summary>
        /// Disconnects all readers.
        /// </summary>
        public void DisconnectAllReaders()
        {
            foreach (var key in _readerConnected.Keys.ToList())
            {
                _readerConnected[key] = false;
            }
        }

        /// <summary>
        /// Connects all configured readers.
        /// </summary>
        public async Task ConnectAllReadersAsync()
        {
            var tasks = _readers.Keys.Select(ip => ConnectReaderByIpAsync(ip));
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Executes the reader's configured stored procedure (same pattern as RFID_COMMON.RFID_Common_MSSQL).
        /// Parameters: @pTRAN_TYPE, @pRFID, @pIPADDR, @pRETURN_VALUE1 (output).
        /// </summary>
        public void ExecuteReaderStoredProcedure(string tranType, string rfid, string hostName, string ipAddress, string storedProcedure)
        {
            if (string.IsNullOrEmpty(storedProcedure))
                return;

            using (var con = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(storedProcedure, con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@pTRAN_TYPE", tranType));
                cmd.Parameters.Add(new SqlParameter("@pRFID", rfid));
                cmd.Parameters.Add(new SqlParameter("@pIPADDR", ipAddress));
                cmd.Parameters.Add(new SqlParameter("@pRETURN_VALUE1", SqlDbType.NVarChar, 4000)
                {
                    Direction = ParameterDirection.Output
                });

                con.Open();
                cmd.ExecuteNonQuery();

                string returnValue = cmd.Parameters["@pRETURN_VALUE1"].Value.ToString();
                if (returnValue != "0")
                {
                    throw new Exception(returnValue);
                }
            }
        }
    }
}
