using Microsoft.AspNet.SignalR;
using FILM_Sparepart_MVC.Hubs;
using FILM_Sparepart_MVC.Models;
using Symbol.RFID3;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace FILM_Sparepart_MVC.Services
{
    public class RFIDService
    {
        private static readonly object _lock = new object();
        private static readonly ConcurrentDictionary<string, ReaderConnection> _readers = new ConcurrentDictionary<string, ReaderConnection>();
        private static bool _isInitialized = false;
        private static List<RFIDModel> _readerConfigs = new List<RFIDModel>();

        // Connection string name for the Tower database (SP2/SERVER2)
        private const string TowerConnectionStringName = "SQLConTower";

        /// <summary>
        /// Loads RFID reader configurations from the database (equivalent to bg_GetRFIDConfig in Form1.vb)
        /// </summary>
        public List<RFIDModel> LoadReaderConfigurations()
        {
            lock (_lock)
            {
                if (_isInitialized && _readerConfigs.Count > 0)
                {
                    // Update connection status from live reader state
                    foreach (var config in _readerConfigs)
                    {
                        ReaderConnection conn;
                        if (_readers.TryGetValue(config.READER_IP, out conn))
                        {
                            config.IsConnected = conn.Reader != null && conn.Reader.IsConnected;
                            config.Status = config.IsConnected ? "Connected" : "Disconnected";
                        }
                        else
                        {
                            config.IsConnected = false;
                            config.Status = "Disconnected";
                        }
                    }
                    return _readerConfigs;
                }

                var configs = new List<RFIDModel>();
                string connectionString = ConfigurationManager.ConnectionStrings["SQLCon"].ConnectionString;

                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SP_FILM_GET_RFID_CONFIG", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.Add(new SqlParameter("@pCOMPANY", "FILM"));

                    con.Open();
                    var tbl = new DataTable();
                    tbl.Load(cmd.ExecuteReader());

                    foreach (DataRow row in tbl.Rows)
                    {
                        configs.Add(new RFIDModel
                        {
                            LOCATION = row["LOCATION"].ToString(),
                            READER_IP = row["IP_ADDRESS"].ToString(),
                            READER_NAME = row["HOST_NAME"].ToString(),
                            READER_PORT = uint.Parse(row["PORT"].ToString()),
                            STORED_PROCEDURE = row["SP"].ToString(),
                            SERVER = row["SERVER"].ToString(),
                            STORED_PROCEDURE2 = row["SP2"].ToString(),
                            SERVER2 = row["SERVER2"].ToString(),
                            IsConnected = false,
                            Status = "Disconnected"
                        });
                    }
                }

                _readerConfigs = configs;
                _isInitialized = true;

                // Update status for any already-connected readers
                foreach (var config in _readerConfigs)
                {
                    ReaderConnection conn;
                    if (_readers.TryGetValue(config.READER_IP, out conn))
                    {
                        config.IsConnected = conn.Reader != null && conn.Reader.IsConnected;
                        config.Status = config.IsConnected ? "Connected" : "Disconnected";
                    }
                }

                return _readerConfigs;
            }
        }

        /// <summary>
        /// Connect a single reader by IP address
        /// </summary>
        public ConnectionResult ConnectReader(string readerIP)
        {
            var result = new ConnectionResult();

            try
            {
                var config = _readerConfigs.FirstOrDefault(r => r.READER_IP == readerIP);
                if (config == null)
                {
                    result.Success = false;
                    result.Message = "Reader configuration not found for IP: " + readerIP;
                    return result;
                }

                ReaderConnection existingConn;
                if (_readers.TryGetValue(readerIP, out existingConn) && existingConn.Reader != null && existingConn.Reader.IsConnected)
                {
                    result.Success = true;
                    result.Message = "Already connected";
                    config.IsConnected = true;
                    config.Status = "Connected";
                    return result;
                }

                var reader = new RFIDReader(readerIP, config.READER_PORT, 50000);
                reader.Connect();

                // Subscribe to events
                reader.Events.NotifyGPIEvent = true;
                reader.Events.NotifyBufferFullEvent = true;
                reader.Events.NotifyBufferFullWarningEvent = true;
                reader.Events.NotifyReaderDisconnectEvent = true;
                reader.Events.NotifyReaderExceptionEvent = true;
                reader.Events.NotifyAccessStartEvent = true;
                reader.Events.NotifyAccessStopEvent = true;
                reader.Events.NotifyInventoryStartEvent = true;
                reader.Events.NotifyInventoryStopEvent = true;

                // Subscribe to read and status events
                reader.Events.AttachTagDataWithReadEvent = false;
                reader.Events.ReadNotify += Events_ReadNotify;
                reader.Events.StatusNotify += Events_StatusNotify;

                // Start inventory on antennas 1 & 2
                var antennaList = new ushort[] { 1, 2 };
                var antennaInfo = new AntennaInfo(antennaList);
                reader.Actions.Inventory.Perform(null, null, antennaInfo);

                var conn = new ReaderConnection
                {
                    Reader = reader,
                    Config = config
                };

                _readers.AddOrUpdate(readerIP, conn, (key, old) =>
                {
                    // Dispose old reader if exists
                    try { old.Reader?.Dispose(); } catch { }
                    return conn;
                });

                config.IsConnected = true;
                config.Status = "Connected";

                result.Success = true;
                result.Message = "Connected successfully";

                // Notify clients via SignalR
                var hubContext = GlobalHost.ConnectionManager.GetHubContext<RfidHub>();
                hubContext.Clients.All.readerStatusChanged(readerIP, "Connected", true);
            }
            catch (OperationFailureException ex)
            {
                result.Success = false;
                result.Message = "Connect failed: " + ex.Result.ToString();

                var config = _readerConfigs.FirstOrDefault(r => r.READER_IP == readerIP);
                if (config != null)
                {
                    config.IsConnected = false;
                    config.Status = result.Message;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Connect failed: " + ex.Message;

                var config = _readerConfigs.FirstOrDefault(r => r.READER_IP == readerIP);
                if (config != null)
                {
                    config.IsConnected = false;
                    config.Status = result.Message;
                }
            }

            return result;
        }

        /// <summary>
        /// Disconnect a single reader by IP address
        /// </summary>
        public ConnectionResult DisconnectReader(string readerIP)
        {
            var result = new ConnectionResult();

            try
            {
                ReaderConnection conn;
                if (_readers.TryGetValue(readerIP, out conn) && conn.Reader != null)
                {
                    if (conn.Reader.IsConnected)
                    {
                        try
                        {
                            if (conn.Reader.Actions.TagAccess.OperationSequence.Length > 0)
                            {
                                conn.Reader.Actions.TagAccess.OperationSequence.StopSequence();
                                conn.Reader.Actions.Inventory.Stop();
                            }
                            else
                            {
                                conn.Reader.Actions.Inventory.Stop();
                            }
                        }
                        catch { }

                        conn.Reader.Events.ReadNotify -= Events_ReadNotify;
                        conn.Reader.Events.StatusNotify -= Events_StatusNotify;
                        conn.Reader.Disconnect();
                    }

                    ReaderConnection removed;
                    _readers.TryRemove(readerIP, out removed);
                }

                var config = _readerConfigs.FirstOrDefault(r => r.READER_IP == readerIP);
                if (config != null)
                {
                    config.IsConnected = false;
                    config.Status = "Disconnected";
                }

                result.Success = true;
                result.Message = "Disconnected successfully";

                // Notify clients via SignalR
                var hubContext = GlobalHost.ConnectionManager.GetHubContext<RfidHub>();
                hubContext.Clients.All.readerStatusChanged(readerIP, "Disconnected", false);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Disconnect failed: " + ex.Message;
            }

            return result;
        }

        /// <summary>
        /// Connect all configured readers
        /// </summary>
        public List<ConnectionResult> ConnectAll()
        {
            var results = new List<ConnectionResult>();
            foreach (var config in _readerConfigs)
            {
                var r = ConnectReader(config.READER_IP);
                r.ReaderIP = config.READER_IP;
                results.Add(r);
            }
            return results;
        }

        /// <summary>
        /// Disconnect all connected readers
        /// </summary>
        public List<ConnectionResult> DisconnectAll()
        {
            var results = new List<ConnectionResult>();
            foreach (var config in _readerConfigs)
            {
                var r = DisconnectReader(config.READER_IP);
                r.ReaderIP = config.READER_IP;
                results.Add(r);
            }
            return results;
        }

        /// <summary>
        /// Get the current status of all readers
        /// </summary>
        public List<RFIDModel> GetReaderStatuses()
        {
            return LoadReaderConfigurations();
        }

        /// <summary>
        /// Handle RFID tag read events (equivalent to Events_ReadNotify / myUpdateRead in Form1.vb).
        /// When a new tag is detected, the stored procedure from RFID_CONFIG is automatically executed,
        /// matching the original WinForm behaviour in StartUpdateDB / StartUpdateDBTower.
        /// </summary>
        private void Events_ReadNotify(object sender, Events.ReadEventArgs e)
        {
            try
            {
                string readerHostName = ((Symbol.RFID3.Events)sender).HostName;

                ReaderConnection conn;
                if (!_readers.TryGetValue(readerHostName, out conn))
                {
                    // Try to find by matching the hostname in configs
                    var config = _readerConfigs.FirstOrDefault(r => r.READER_IP == readerHostName || r.READER_NAME == readerHostName);
                    if (config != null)
                    {
                        _readers.TryGetValue(config.READER_IP, out conn);
                    }
                }

                if (conn == null || conn.Reader == null) return;

                TagData[] tagData = conn.Reader.Actions.GetReadTags(50);
                if (tagData != null)
                {
                    var hubContext = GlobalHost.ConnectionManager.GetHubContext<RfidHub>();

                    foreach (var tag in tagData)
                    {
                        if (tag.OpCode == ACCESS_OPERATION_CODE.ACCESS_OPERATION_NONE ||
                            (tag.OpCode == ACCESS_OPERATION_CODE.ACCESS_OPERATION_READ &&
                             tag.OpStatus == ACCESS_OPERATION_STATUS.ACCESS_SUCCESS))
                        {
                            string tagID = tag.TagID;
                            int antennaID = tag.AntennaID;

                            // --- Tag deduplication (equivalent to ht_TagDetected in Form1.vb) ---
                            bool isNewTag = conn.TagDetected.TryAdd(tagID, DateTime.UtcNow);

                            // Broadcast tag to all connected web clients
                            hubContext.Clients.All.tagRead(readerHostName, tagID, antennaID, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                            // --- Execute stored procedure for NEW tags only (equivalent to StartUpdateDB / StartUpdateDBTower) ---
                            if (isNewTag)
                            {
                                string storedProcedure;
                                string server;

                                if (antennaID == 1)
                                {
                                    storedProcedure = conn.Config.STORED_PROCEDURE;
                                    server = conn.Config.SERVER;
                                }
                                else
                                {
                                    // Antenna 2+: use SP2/SERVER2 if available, otherwise fall back to SP/SERVER
                                    storedProcedure = !string.IsNullOrEmpty(conn.Config.STORED_PROCEDURE2) ? conn.Config.STORED_PROCEDURE2 : conn.Config.STORED_PROCEDURE;
                                    server = !string.IsNullOrEmpty(conn.Config.SERVER2) ? conn.Config.SERVER2 : conn.Config.SERVER;
                                }

                                if (!string.IsNullOrEmpty(storedProcedure) && !string.IsNullOrEmpty(server) && server == "MSSQL")
                                {
                                    bool useTowerConnection = antennaID != 1
                                        && !string.IsNullOrEmpty(conn.Config.SERVER2)
                                        && !string.IsNullOrEmpty(conn.Config.STORED_PROCEDURE2);

                                    var spResult = ExecuteStoredProcedure("IN", tagID, conn.Config.READER_NAME, conn.Config.READER_IP, storedProcedure, useTowerConnection);

                                    // Broadcast stored procedure result to web clients
                                    hubContext.Clients.All.tagProcessed(readerHostName, tagID, antennaID, spResult.Success, spResult.Message);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Silently handle read errors
            }
        }

        /// <summary>
        /// Execute the configured stored procedure for a scanned RFID tag.
        /// Equivalent to RFID_Common_MSSQL / RFID_Common_MSSQL_Tower in Library.Database/RFID_COMMON.vb.
        /// Parameters match the original WinForm: @pTRAN_TYPE, @pRFID, @pIPADDR, @pRETURN_VALUE1 (output).
        /// </summary>
        private StoredProcedureResult ExecuteStoredProcedure(string tranType, string rfidTag, string readerName, string ipAddress, string storedProcedure, bool useTowerConnection)
        {
            var result = new StoredProcedureResult();

            string connectionStringName = useTowerConnection ? TowerConnectionStringName : "SQLCon";
            var connStringSetting = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (connStringSetting == null)
            {
                // Fall back to default connection string if tower connection is not configured
                connStringSetting = ConfigurationManager.ConnectionStrings["SQLCon"];
            }

            string connectionString = connStringSetting.ConnectionString;

            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(storedProcedure, con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;

                cmd.Parameters.Add(new SqlParameter("@pTRAN_TYPE", tranType));
                cmd.Parameters.Add(new SqlParameter("@pRFID", rfidTag));
                cmd.Parameters.Add(new SqlParameter("@pIPADDR", ipAddress));
                cmd.Parameters.Add(new SqlParameter("@pRETURN_VALUE1", SqlDbType.NVarChar, 4000) { Direction = ParameterDirection.Output });

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();

                    string returnValue = cmd.Parameters["@pRETURN_VALUE1"].Value?.ToString() ?? "";
                    if (returnValue != "0")
                    {
                        result.Success = false;
                        result.Message = returnValue;
                    }
                    else
                    {
                        result.Success = true;
                        result.Message = "OK";
                    }
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = ex.Message;
                }
            }

            return result;
        }

        /// <summary>
        /// Handle RFID status events (equivalent to Events_StatusNotify / myUpdateStatus in Form1.vb)
        /// </summary>
        private void Events_StatusNotify(object sender, Events.StatusEventArgs e)
        {
            try
            {
                string readerHostName = ((Symbol.RFID3.Events)sender).HostName;
                var eventData = e.StatusEventData;
                string statusMsg = "";

                switch (eventData.StatusEventType)
                {
                    case Events.STATUS_EVENT_TYPE.INVENTORY_START_EVENT:
                        statusMsg = "Inventory started";
                        break;
                    case Events.STATUS_EVENT_TYPE.INVENTORY_STOP_EVENT:
                        statusMsg = "Inventory stopped";
                        break;
                    case Events.STATUS_EVENT_TYPE.ACCESS_START_EVENT:
                        statusMsg = "Access Operation started";
                        break;
                    case Events.STATUS_EVENT_TYPE.ACCESS_STOP_EVENT:
                        statusMsg = "Access Operation stopped";
                        break;
                    case Events.STATUS_EVENT_TYPE.BUFFER_FULL_WARNING_EVENT:
                        statusMsg = "Buffer full warning";
                        break;
                    case Events.STATUS_EVENT_TYPE.BUFFER_FULL_EVENT:
                        statusMsg = "Buffer full";
                        break;
                    case Events.STATUS_EVENT_TYPE.DISCONNECTION_EVENT:
                        statusMsg = "Disconnection Event " + eventData.DisconnectionEventData.DisconnectEventInfo.ToString();
                        break;
                    case Events.STATUS_EVENT_TYPE.ANTENNA_EVENT:
                        statusMsg = "Antenna Status Update";
                        break;
                    case Events.STATUS_EVENT_TYPE.READER_EXCEPTION_EVENT:
                        statusMsg = "Reader ExceptionEvent " + eventData.ReaderExceptionEventData.ReaderExceptionEventInfo;
                        break;
                    default:
                        statusMsg = "Unhandled Status";
                        break;
                }

                // Broadcast status to all connected web clients
                var hubContext = GlobalHost.ConnectionManager.GetHubContext<RfidHub>();
                hubContext.Clients.All.readerStatusEvent(readerHostName, statusMsg);

                // Handle disconnection - update reader status
                ReaderConnection conn;
                if (_readers.TryGetValue(readerHostName, out conn) && conn.Reader != null && !conn.Reader.IsConnected)
                {
                    var config = _readerConfigs.FirstOrDefault(r => r.READER_IP == readerHostName);
                    if (config != null)
                    {
                        config.IsConnected = false;
                        config.Status = statusMsg;
                    }
                    hubContext.Clients.All.readerStatusChanged(readerHostName, statusMsg, false);
                }
            }
            catch (Exception)
            {
                // Silently handle status errors
            }
        }
    }

    /// <summary>
    /// Holds a reader connection and its configuration
    /// </summary>
    public class ReaderConnection
    {
        public RFIDReader Reader { get; set; }
        public RFIDModel Config { get; set; }

        /// <summary>
        /// Tracks detected tags to avoid duplicate processing (equivalent to ht_TagDetected in Form1.vb).
        /// Key = TagID, Value = first detection time (UTC).
        /// </summary>
        public ConcurrentDictionary<string, DateTime> TagDetected { get; set; } = new ConcurrentDictionary<string, DateTime>();
    }

    /// <summary>
    /// Result of a connection/disconnection operation
    /// </summary>
    public class ConnectionResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ReaderIP { get; set; }
    }

    /// <summary>
    /// Result of executing a stored procedure for a scanned RFID tag
    /// </summary>
    public class StoredProcedureResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
