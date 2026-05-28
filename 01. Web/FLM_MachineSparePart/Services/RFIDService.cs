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
using System.Threading;

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

        // Tower zone reader IP (same hardcoded value as Form1.vb)
        private const string TowerZoneReaderIP = "10.28.92.50";

        // GPI direction detection timer interval in milliseconds (equivalent to Timer1.Interval = 10000 in Form1.Designer.vb)
        private const int GpiTimerIntervalMs = 10000;

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
                    // Dispose GPI timer if active
                    conn.GpiTimer?.Dispose();
                    conn.GpiTimer = null;

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
        /// Tags are collected into TagDetected for later processing when the GPI direction is confirmed.
        /// The stored procedure is NOT called here — it is called from the GPI timer callback.
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
                            // Tags accumulate here until GPI direction is confirmed and StartUpdateDB processes them
                            conn.TagDetected.TryAdd(tagID, DateTime.UtcNow);

                            // Broadcast tag to all connected web clients
                            hubContext.Clients.All.tagRead(readerHostName, tagID, antennaID, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
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
        /// Batch-execute the stored procedure for all accumulated tags on a reader.
        /// Equivalent to StartUpdateDB in Form1.vb — iterates ht_TagDetected and calls RFID_Common_MSSQL for each tag.
        /// </summary>
        private void StartUpdateDB(string tranType, ReaderConnection conn)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<RfidHub>();
            var tagsSnapshot = conn.TagDetected.Keys.ToList();

            foreach (var tagID in tagsSnapshot)
            {
                var spResult = ExecuteStoredProcedure(tranType, tagID, conn.Config.READER_NAME, conn.Config.READER_IP, conn.Config.STORED_PROCEDURE, false);
                hubContext.Clients.All.tagProcessed(conn.Config.READER_IP, tagID, 0, spResult.Success, spResult.Message);
            }

            conn.TagDetected.Clear();
        }

        /// <summary>
        /// Batch-execute the stored procedure for all accumulated tags on a Tower reader.
        /// Equivalent to StartUpdateDBTower in Form1.vb — uses the Tower database connection.
        /// </summary>
        private void StartUpdateDBTower(string tranType, ReaderConnection conn)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<RfidHub>();
            var tagsSnapshot = conn.TagDetected.Keys.ToList();

            foreach (var tagID in tagsSnapshot)
            {
                var spResult = ExecuteStoredProcedure(tranType, tagID, conn.Config.READER_NAME, conn.Config.READER_IP, conn.Config.STORED_PROCEDURE, true);
                hubContext.Clients.All.tagProcessed(conn.Config.READER_IP, tagID, 0, spResult.Success, spResult.Message);
            }

            conn.TagDetected.Clear();
        }

        /// <summary>
        /// Handle the first GPI event — determines initial direction.
        /// Equivalent to updateIn() in Form1.vb.
        /// GPI Port 1 LOW → "In" direction (or tower="1" for Tower zone reader).
        /// GPI Port 2 LOW → "Out" direction (or tower="2" for Tower zone reader).
        /// Starts the GPI timer to wait for the second loop coil confirmation.
        /// </summary>
        private void HandleFirstGpiEvent(ReaderConnection conn)
        {
            bool isTowerZone = conn.Config.READER_IP == TowerZoneReaderIP;

            // Check GPI Port 1 state (PortState 0 = LOW = triggered)
            if (conn.Reader.Config.GPI.Item(1).PortState == 0)
            {
                if (isTowerZone)
                {
                    conn.GpiTower = "1";
                }
                else
                {
                    conn.GpiLoopCoil1 = "In";
                }
                StartGpiTimer(conn);
            }

            // Check GPI Port 2 state
            if (conn.Reader.Config.GPI.Item(2).PortState == 0)
            {
                if (isTowerZone)
                {
                    conn.GpiTower = "2";
                }
                else
                {
                    conn.GpiLoopCoil1 = "Out";
                }
                StartGpiTimer(conn);
            }
        }

        /// <summary>
        /// Handle a second GPI event while timer is running — confirms direction.
        /// Equivalent to check2ndloop() in Form1.vb.
        /// </summary>
        private void HandleSecondGpiEvent(ReaderConnection conn)
        {
            bool isTowerZone = conn.Config.READER_IP == TowerZoneReaderIP;

            // Check GPI Port 2 state → "In"
            if (conn.Reader.Config.GPI.Item(2).PortState == 0)
            {
                conn.GpiLoopCoil2 = "In";
            }

            // Check GPI Port 1 state → "Out"
            if (conn.Reader.Config.GPI.Item(1).PortState == 0)
            {
                conn.GpiLoopCoil2 = "Out";
            }

            // For tower zone, translate numeric tower to direction
            if (isTowerZone)
            {
                if (conn.GpiTower == "1")
                {
                    conn.GpiTower = "Out";
                }

                if (conn.GpiTower == "2")
                {
                    conn.GpiTower = "In";
                }
            }
        }

        /// <summary>
        /// Start or restart the GPI direction resolution timer for a reader.
        /// </summary>
        private void StartGpiTimer(ReaderConnection conn)
        {
            // Dispose existing timer if any
            conn.GpiTimer?.Dispose();

            conn.GpiTimer = new System.Threading.Timer(GpiTimerCallback, conn, GpiTimerIntervalMs, Timeout.Infinite);

            // Notify web clients that GPI event was detected
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<RfidHub>();
            hubContext.Clients.All.gpiEventDetected(conn.Config.READER_IP, conn.GpiLoopCoil1, conn.GpiTower);
        }

        /// <summary>
        /// Timer callback — equivalent to Timer1_Tick in Form1.vb.
        /// Resolves GPI direction and executes the stored procedure for all accumulated tags.
        /// </summary>
        private void GpiTimerCallback(object state)
        {
            var conn = (ReaderConnection)state;

            try
            {
                // Dispose the timer (one-shot, equivalent to Timer1.Stop() + Timer1.Enabled = False)
                conn.GpiTimer?.Dispose();
                conn.GpiTimer = null;

                var hubContext = GlobalHost.ConnectionManager.GetHubContext<RfidHub>();
                bool isTowerZone = conn.Config.READER_IP == TowerZoneReaderIP;

                if (!isTowerZone)
                {
                    // Non-tower readers: direction confirmed only if both loop coils agree
                    if (conn.GpiLoopCoil1 == "In" && conn.GpiLoopCoil1 == conn.GpiLoopCoil2)
                    {
                        StartUpdateDB("IN", conn);
                        hubContext.Clients.All.gpiDirectionResolved(conn.Config.READER_IP, "IN");
                    }
                    else if (conn.GpiLoopCoil1 == "Out" && conn.GpiLoopCoil1 == conn.GpiLoopCoil2)
                    {
                        StartUpdateDB("OUT", conn);
                        hubContext.Clients.All.gpiDirectionResolved(conn.Config.READER_IP, "OUT");
                    }
                    else
                    {
                        // Direction not confirmed — clear tags without processing
                        conn.TagDetected.Clear();
                        hubContext.Clients.All.gpiDirectionResolved(conn.Config.READER_IP, "UNCONFIRMED");
                    }
                }
                else
                {
                    // Tower zone reader: use tower direction
                    if (conn.GpiTower == "In")
                    {
                        StartUpdateDBTower("IN", conn);
                        hubContext.Clients.All.gpiDirectionResolved(conn.Config.READER_IP, "IN (Tower)");
                    }
                    else if (conn.GpiTower == "Out")
                    {
                        StartUpdateDBTower("OUT", conn);
                        hubContext.Clients.All.gpiDirectionResolved(conn.Config.READER_IP, "OUT (Tower)");
                    }
                    else
                    {
                        // Direction not confirmed — clear tags without processing
                        conn.TagDetected.Clear();
                        hubContext.Clients.All.gpiDirectionResolved(conn.Config.READER_IP, "UNCONFIRMED (Tower)");
                    }
                }

                // Reset GPI state for next detection cycle
                conn.GpiLoopCoil1 = "";
                conn.GpiLoopCoil2 = "";
                conn.GpiTower = "";
            }
            catch (Exception)
            {
                // Silently handle timer errors
            }
        }

        /// <summary>
        /// Handle RFID status events (equivalent to Events_StatusNotify / myUpdateStatus in Form1.vb).
        /// Includes GPI_EVENT handling for loop coil direction detection.
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
                    case Events.STATUS_EVENT_TYPE.GPI_EVENT:
                        // GPI event triggered when loop coil detects forklift presence
                        // Equivalent to GPI_EVENT handling in myUpdateStatus in Form1.vb
                        statusMsg = "GPI Event Port " + eventData.GPIEventData.PortNumber + " State " + eventData.GPIEventData.GPIEvent;
                        ReaderConnection gpiConn;
                        if (!_readers.TryGetValue(readerHostName, out gpiConn))
                        {
                            var gpiConfig = _readerConfigs.FirstOrDefault(r => r.READER_IP == readerHostName || r.READER_NAME == readerHostName);
                            if (gpiConfig != null)
                            {
                                _readers.TryGetValue(gpiConfig.READER_IP, out gpiConn);
                            }
                        }
                        if (gpiConn != null)
                        {
                            if (gpiConn.GpiTimer != null)
                            {
                                // Timer already running — this is the 2nd GPI event (equivalent to check2ndloop)
                                HandleSecondGpiEvent(gpiConn);
                            }
                            else
                            {
                                // First GPI event (equivalent to updateIn)
                                HandleFirstGpiEvent(gpiConn);
                            }
                        }
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

        // --- GPI Loop Coil Direction State (equivalent to Form1.vb fields) ---

        /// <summary>First loop coil direction: "In" or "Out" (equivalent to loopCoil1 in Form1.vb)</summary>
        public string GpiLoopCoil1 { get; set; } = "";

        /// <summary>Second loop coil direction for confirmation: "In" or "Out" (equivalent to loopCoil2 in Form1.vb)</summary>
        public string GpiLoopCoil2 { get; set; } = "";

        /// <summary>Tower zone direction state: "1", "2", "In", or "Out" (equivalent to tower in Form1.vb)</summary>
        public string GpiTower { get; set; } = "";

        /// <summary>
        /// Timer for GPI direction resolution (equivalent to Timer1 in Form1.vb).
        /// After the first GPI event, waits for a second event to confirm direction before executing the SP.
        /// </summary>
        public System.Threading.Timer GpiTimer { get; set; }
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
