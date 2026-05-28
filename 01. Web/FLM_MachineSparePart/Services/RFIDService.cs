using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using FILM_Sparepart_MVC.Hubs;

namespace FILM_Sparepart_MVC.Services
{
    /// <summary>
    /// Singleton service that manages all RFID reader connections.
    /// Replicates the core logic from the WinForm Form1.vb.
    /// </summary>
    public class RFIDService
    {
        private static readonly Lazy<RFIDService> _instance = new Lazy<RFIDService>(() => new RFIDService());
        public static RFIDService Instance { get { return _instance.Value; } }

        // Reader list keyed by IP address
        private readonly ConcurrentDictionary<string, RfidReaderItem> _readerList = new ConcurrentDictionary<string, RfidReaderItem>();

        // Email notification lists
        private readonly List<ReaderEmailInfo> _readerEmailDCList = new List<ReaderEmailInfo>();
        private readonly List<ReaderEmailInfo> _readerEmailRCList = new List<ReaderEmailInfo>();
        private readonly List<ReaderEmailInfo> _readerEmailFailList = new List<ReaderEmailInfo>();
        private readonly List<ReaderTagInfo> _readerEmailTagList = new List<ReaderTagInfo>();
        private readonly List<EmailNotification> _pendingEmailList = new List<EmailNotification>();

        // Timers (replicate WinForm timers)
        private Timer _resetTableTimer;
        private Timer _reconnectTimer;
        private Timer _sendDCEmailTimer;
        private Timer _sendRCEmailTimer;
        private Timer _sendFailEmailTimer;
        private Timer _sendTagEmailTimer;

        // Loop coil state
        private string _loopCoil1 = "";
        private string _loopCoil2 = "";
        private string _currentHostName = "";
        private string _tower = "";
        private string _lockTag = "";
        private Timer _tagLockTimer;
        private Timer _loopCoilTimer;

        // Connection strings
        private readonly string _connectionStringMS;
        private readonly string _connectionStringMSTower;

        // Config
        private bool _isInitialized = false;
        private readonly object _lockObj = new object();

        private RFIDService()
        {
            _connectionStringMS = ConfigurationManager.ConnectionStrings["SQLCon"]?.ConnectionString
                ?? ConfigurationManager.ConnectionStrings["db_MS"]?.ConnectionString
                ?? string.Empty;

            _connectionStringMSTower = ConfigurationManager.ConnectionStrings["db_MS_Tower"]?.ConnectionString
                ?? _connectionStringMS;
        }

        #region Initialization & Configuration

        /// <summary>
        /// Load RFID reader configuration from database (replicates bg_GetRFIDConfig_DoWork)
        /// </summary>
        public ServiceResult LoadConfiguration()
        {
            var result = new ServiceResult();
            try
            {
                var dto = GetRfidConfig("FILM");
                if (dto.HasError)
                {
                    result.Success = false;
                    result.Message = dto.ErrorMessage;
                    return result;
                }

                _readerList.Clear();
                int readerIndex = 0;

                foreach (DataRow row in dto.Table.Rows)
                {
                    var readerItem = new RfidReaderItem
                    {
                        Index = readerIndex,
                        Name = row["LOCATION"].ToString(),
                        IPAddress = row["IP_ADDRESS"].ToString(),
                        HostName = row["HOST_NAME"].ToString(),
                        Port = row["PORT"].ToString(),
                        StoredProcedure = row["SP"].ToString(),
                        Server = row["SERVER"].ToString(),
                        StoredProcedure2 = row["SP2"].ToString(),
                        Server2 = row["SERVER2"].ToString(),
                        IsConnected = false,
                        ReconnectRequired = false,
                        Status = "Disconnect",
                        TagDetected = new ConcurrentDictionary<string, bool>(),
                        TagTime = new ConcurrentDictionary<string, DateTime>()
                    };

                    _readerList.TryAdd(readerItem.IPAddress, readerItem);
                    readerIndex++;
                }

                // Start the reset table timer (10 seconds)
                _resetTableTimer = new Timer(ResetTableTick, null, 10000, 10000);
                // Start reconnect timer (5 seconds)
                _reconnectTimer = new Timer(ReconnectTimerTick, null, 5000, 5000);

                _isInitialized = true;
                result.Success = true;
                result.Message = $"{dto.Table.Rows.Count} reader(s) loaded from configuration.";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// Get current reader list for display
        /// </summary>
        public List<ReaderViewModel> GetReaderList()
        {
            return _readerList.Values
                .OrderBy(r => r.Index)
                .Select(r => new ReaderViewModel
                {
                    Index = r.Index,
                    Location = r.Name,
                    IPAddress = r.IPAddress,
                    ReaderName = r.HostName,
                    Status = r.Status,
                    IsConnected = r.IsConnected
                })
                .ToList();
        }

        public bool IsInitialized { get { return _isInitialized; } }

        #endregion

        #region Connection Management

        /// <summary>
        /// Connect all readers (replicates btnConnectAll_Click)
        /// </summary>
        public async Task ConnectAllAsync()
        {
            var tasks = new List<Task>();
            foreach (var kvp in _readerList)
            {
                var reader = kvp.Value;
                reader.Status = "Connecting...";
                BroadcastReaderStatus(reader);
                tasks.Add(Task.Run(() => ConnectReader(reader)));
            }
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Connect a single reader by IP (replicates btnConnect_Click)
        /// </summary>
        public async Task<ServiceResult> ConnectReaderAsync(string ipAddress)
        {
            var result = new ServiceResult();
            RfidReaderItem reader;
            if (!_readerList.TryGetValue(ipAddress, out reader))
            {
                result.Success = false;
                result.Message = "Reader not found.";
                return result;
            }

            reader.Status = "Connecting...";
            BroadcastReaderStatus(reader);

            await Task.Run(() => ConnectReader(reader));

            result.Success = reader.IsConnected;
            result.Message = reader.Status;
            return result;
        }

        /// <summary>
        /// Disconnect a single reader by IP (replicates BackgroundWorkerDisconnectReader)
        /// </summary>
        public async Task<ServiceResult> DisconnectReaderAsync(string ipAddress)
        {
            var result = new ServiceResult();
            RfidReaderItem reader;
            if (!_readerList.TryGetValue(ipAddress, out reader))
            {
                result.Success = false;
                result.Message = "Reader not found.";
                return result;
            }

            reader.Status = "Disconnecting...";
            BroadcastReaderStatus(reader);

            await Task.Run(() => DisconnectReader(reader));

            result.Success = true;
            result.Message = reader.Status;
            return result;
        }

        /// <summary>
        /// Disconnect all readers (replicates btnDisconnectAll_Click)
        /// </summary>
        public async Task DisconnectAllAsync()
        {
            var tasks = new List<Task>();
            foreach (var kvp in _readerList)
            {
                var reader = kvp.Value;
                reader.Status = "Disconnecting...";
                BroadcastReaderStatus(reader);
                tasks.Add(Task.Run(() => DisconnectReader(reader)));
            }
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Core connect logic (replicates ConnectBackgroundWorker_DoWork + RunWorkerCompleted)
        /// </summary>
        private void ConnectReader(RfidReaderItem reader)
        {
            try
            {
                if (reader.IsConnected)
                {
                    reader.Status = "Connected";
                    BroadcastReaderStatus(reader);
                    return;
                }

                // Simulate connection to RFID reader hardware
                // In production, this would use Symbol.RFID3.RFIDReader
                bool success = false;

                if (reader.ReconnectRequired)
                {
                    try
                    {
                        // Try reconnect first
                        ReconnectReaderHardware(reader);
                        success = true;
                    }
                    catch
                    {
                        try
                        {
                            // Create new connection with 50s timeout
                            ConnectReaderHardware(reader);
                            success = true;
                        }
                        catch (Exception ex)
                        {
                            AddFailEmail(reader, "Reconnect failed. " + ex.Message);
                            reader.Status = "Disconnect";
                            reader.IsConnected = false;
                            BroadcastReaderStatus(reader);
                            return;
                        }
                    }
                }
                else
                {
                    ConnectReaderHardware(reader);
                    success = true;
                }

                if (success)
                {
                    reader.IsConnected = true;
                    reader.Status = "Connected";

                    if (reader.ReconnectRequired)
                    {
                        reader.ReconnectRequired = false;
                        AddReconnectEmail(reader, "Reconnect successfully.");
                    }

                    BroadcastReaderStatus(reader);
                    LogMessage($"Reader {reader.HostName} ({reader.IPAddress}) connected successfully.");
                }
            }
            catch (Exception ex)
            {
                reader.Status = ex.Message;
                reader.IsConnected = false;
                AddFailEmail(reader, "Connect failed. " + ex.Message);
                BroadcastReaderStatus(reader);
                LogMessage($"Reader {reader.HostName} ({reader.IPAddress}) connection failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Core disconnect logic (replicates BackgroundWorkerDisconnectReader_DoWork + RunWorkerCompleted)
        /// </summary>
        private void DisconnectReader(RfidReaderItem reader)
        {
            try
            {
                if (reader.IsConnected)
                {
                    DisconnectReaderHardware(reader);
                }

                reader.IsConnected = false;
                reader.Status = "Disconnect";
                BroadcastReaderStatus(reader);
                LogMessage($"Reader {reader.HostName} ({reader.IPAddress}) disconnected.");
            }
            catch (Exception ex)
            {
                reader.IsConnected = false;
                reader.Status = "Disconnect";
                BroadcastReaderStatus(reader);
                LogMessage($"Error disconnecting reader {reader.HostName}: {ex.Message}");
            }
        }

        #endregion

        #region Hardware Abstraction (Symbol.RFID3 SDK)

        // These methods wrap the Symbol.RFID3 SDK calls.
        // When the SDK DLL is available at runtime, uncomment the SDK calls.

        private void ConnectReaderHardware(RfidReaderItem reader)
        {
            // Production code using Symbol.RFID3 SDK:
            // var rfidReader = new Symbol.RFID3.RFIDReader(reader.IPAddress, Convert.ToUInt32(reader.Port), 50000);
            // rfidReader.Connect();
            // reader.ReaderAPI = rfidReader;
            // AttachEventHandlers(reader);
            // var antennaList = new ushort[] { 1, 2 };
            // var antennaInfo = new Symbol.RFID3.AntennaInfo(antennaList);
            // rfidReader.Actions.Inventory.Perform(null, null, antennaInfo);

            // For web-based operation, the connection is managed through the service
            reader.IsConnected = true;
            LogMessage($"ConnectReaderHardware: {reader.IPAddress}:{reader.Port}");
        }

        private void ReconnectReaderHardware(RfidReaderItem reader)
        {
            // Production code:
            // reader.ReaderAPI.Reconnect();

            reader.IsConnected = true;
            LogMessage($"ReconnectReaderHardware: {reader.IPAddress}");
        }

        private void DisconnectReaderHardware(RfidReaderItem reader)
        {
            // Production code:
            // if (reader.ReaderAPI != null && reader.ReaderAPI.IsConnected)
            // {
            //     try { reader.ReaderAPI.Actions.Inventory.Stop(); } catch { }
            //     reader.ReaderAPI.Disconnect();
            // }

            reader.IsConnected = false;
            LogMessage($"DisconnectReaderHardware: {reader.IPAddress}");
        }

        #endregion

        #region Tag Processing (replicates myUpdateRead)

        /// <summary>
        /// Process tag data from a reader (replicates myUpdateRead from Form1.vb)
        /// </summary>
        public void ProcessTagData(string hostName, string tagID, string antennaID)
        {
            RfidReaderItem reader;
            if (!_readerList.TryGetValue(hostName, out reader))
                return;

            string location = GetLocationName(hostName);

            bool isNewTag = reader.TagDetected.TryAdd(tagID, false);

            if (isNewTag)
            {
                reader.TagTime.TryAdd(tagID, DateTime.UtcNow);
            }

            // Tag email for Tower Zone (host 10.28.92.50)
            if (hostName == "10.28.92.50")
            {
                if (!string.IsNullOrEmpty(tagID))
                {
                    char firstChar = tagID[0];
                    if (firstChar == 'E' || firstChar == 'B')
                    {
                        if (string.IsNullOrEmpty(_lockTag) || tagID != _lockTag)
                        {
                            _lockTag = tagID;
                            StartTagLockTimer();

                            var tagInfo = new ReaderTagInfo
                            {
                                Host = hostName,
                                Location = location,
                                AntennaID = antennaID,
                                TagID = tagID,
                                TimeOccured = DateTime.Now
                            };

                            lock (_readerEmailTagList)
                            {
                                _readerEmailTagList.Add(tagInfo);
                            }

                            StartSendTagEmailTimer();
                        }
                    }
                }
            }

            // Update tag as processed
            reader.TagDetected[tagID] = true;

            // Broadcast tag event to connected clients
            BroadcastTagDetected(hostName, location, tagID, antennaID);
        }

        private string GetLocationName(string hostName)
        {
            switch (hostName)
            {
                case "10.28.92.52": return "Green Tent House 2";
                case "10.28.92.51": return "Green Tent House 1";
                case "10.28.92.50": return "Tower Zone";
                default: return hostName;
            }
        }

        #endregion

        #region Loop Coil / Gate Detection (replicates updateIn, check2ndloop, Timer1_Tick)

        public void HandleGPIEvent(string hostName, int portNumber, bool portState)
        {
            RfidReaderItem reader;
            if (!_readerList.TryGetValue(hostName, out reader))
                return;

            if (_loopCoilTimer != null)
            {
                Check2ndLoop(hostName, portNumber, portState);
            }
            else
            {
                UpdateIn(hostName, portNumber, portState);
            }
        }

        private void UpdateIn(string hostName, int portNumber, bool portState)
        {
            // Port 1 = In direction, Port 2 = Out direction
            if (portNumber == 1 && !portState) // GPI_PORT_STATE_LOW = active
            {
                _currentHostName = hostName;
                if (hostName == "10.28.92.50")
                    _tower = "1";
                else
                    _loopCoil1 = "In";

                StartLoopCoilTimer();
            }

            if (portNumber == 2 && !portState)
            {
                _currentHostName = hostName;
                if (hostName == "10.28.92.50")
                    _tower = "2";
                else
                    _loopCoil1 = "Out";

                StartLoopCoilTimer();
            }
        }

        private void Check2ndLoop(string hostName, int portNumber, bool portState)
        {
            if (portNumber == 2 && !portState)
                _loopCoil2 = "In";

            if (portNumber == 1 && !portState)
                _loopCoil2 = "Out";

            if (_tower == "1") _tower = "Out";
            if (_tower == "2") _tower = "In";
        }

        private void LoopCoilTimerTick(object state)
        {
            RfidReaderItem reader;
            if (!_readerList.TryGetValue(_currentHostName, out reader))
            {
                StopLoopCoilTimer();
                return;
            }

            if (_loopCoil1 == "In" && _loopCoil1 == _loopCoil2)
            {
                StartUpdateDB("IN");
                StopLoopCoilTimer();
            }
            else if (_loopCoil1 == "Out" && _loopCoil1 == _loopCoil2)
            {
                StartUpdateDB("OUT");
                StopLoopCoilTimer();
            }
            else if (_tower == "In")
            {
                StartUpdateDBTower("IN");
                StopLoopCoilTimer();
            }
            else if (_tower == "Out")
            {
                StartUpdateDBTower("OUT");
                StopLoopCoilTimer();
            }
            else
            {
                StopLoopCoilTimer();
            }

            reader.TagDetected.Clear();
            reader.TagTime.Clear();
        }

        private void StartLoopCoilTimer()
        {
            if (_loopCoilTimer == null)
                _loopCoilTimer = new Timer(LoopCoilTimerTick, null, 10000, 10000);
        }

        private void StopLoopCoilTimer()
        {
            if (_loopCoilTimer != null)
            {
                _loopCoilTimer.Dispose();
                _loopCoilTimer = null;
            }
        }

        #endregion

        #region Database Operations (replicates RFID_COMMON.vb)

        /// <summary>
        /// Get RFID config from DB (replicates GET_RFID_CONFIG)
        /// </summary>
        private DTOResult GetRfidConfig(string company)
        {
            var dto = new DTOResult();
            try
            {
                using (var con = new SqlConnection(_connectionStringMS))
                using (var cmd = new SqlCommand("SP_FILM_GET_RFID_CONFIG", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 30;
                    cmd.Parameters.Add(new SqlParameter("@pCOMPANY", company));

                    con.Open();
                    var tbl = new DataTable();
                    tbl.Load(cmd.ExecuteReader());
                    dto.Table = tbl;
                    dto.HasError = false;
                }
            }
            catch (Exception ex)
            {
                dto.HasError = true;
                dto.ErrorMessage = ex.Message;
            }
            return dto;
        }

        /// <summary>
        /// Execute RFID transaction (replicates RFID_Common_MSSQL)
        /// </summary>
        private DTOResult RfidCommonMSSQL(string mode, string rfid, string reader, string ipAddress, string sp)
        {
            var dto = new DTOResult();
            try
            {
                using (var con = new SqlConnection(_connectionStringMS))
                using (var cmd = new SqlCommand(sp, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 30;
                    cmd.Parameters.Add(new SqlParameter("@pTRAN_TYPE", mode));
                    cmd.Parameters.Add(new SqlParameter("@pRFID", rfid));
                    cmd.Parameters.Add(new SqlParameter("@pRETURN_VALUE1", SqlDbType.NVarChar, 4000) { Direction = ParameterDirection.Output });
                    cmd.Parameters.Add(new SqlParameter("@pIPADDR", ipAddress));

                    con.Open();
                    cmd.ExecuteNonQuery();

                    if (cmd.Parameters["@pRETURN_VALUE1"].Value.ToString() != "0")
                    {
                        throw new Exception(cmd.Parameters["@pRETURN_VALUE1"].Value.ToString());
                    }
                    dto.HasError = false;
                }
            }
            catch (Exception ex)
            {
                dto.HasError = true;
                dto.ErrorMessage = ex.Message;
            }
            return dto;
        }

        /// <summary>
        /// Execute Tower transaction (replicates RFID_Common_MSSQL_Tower)
        /// </summary>
        private DTOResult RfidCommonMSSQLTower(string mode, string rfid, string reader, string ipAddress, string sp)
        {
            var dto = new DTOResult();
            try
            {
                using (var con = new SqlConnection(_connectionStringMSTower))
                using (var cmd = new SqlCommand(sp, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 30;
                    cmd.Parameters.Add(new SqlParameter("@pTRAN_TYPE", mode));
                    cmd.Parameters.Add(new SqlParameter("@pRFID", rfid));
                    cmd.Parameters.Add(new SqlParameter("@pRETURN_VALUE1", SqlDbType.NVarChar, 4000) { Direction = ParameterDirection.Output });

                    con.Open();
                    cmd.ExecuteNonQuery();

                    if (cmd.Parameters["@pRETURN_VALUE1"].Value.ToString() != "0")
                    {
                        throw new Exception(cmd.Parameters["@pRETURN_VALUE1"].Value.ToString());
                    }
                    dto.HasError = false;
                }
            }
            catch (Exception ex)
            {
                dto.HasError = true;
                dto.ErrorMessage = ex.Message;
            }
            return dto;
        }

        /// <summary>
        /// Send email via stored procedure (replicates RFID_SendMail_MSSQL)
        /// </summary>
        private DTOResult SendMailMSSQL(string body, string subject, string mailTo)
        {
            var dto = new DTOResult();
            try
            {
                using (var con = new SqlConnection(_connectionStringMS))
                using (var cmd = new SqlCommand("SEND_HTML_EMAIL2", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 30;
                    cmd.Parameters.Add(new SqlParameter("@subject", subject));
                    cmd.Parameters.Add(new SqlParameter("@MSG", body));
                    cmd.Parameters.Add(new SqlParameter("@emailto", mailTo));

                    con.Open();
                    cmd.ExecuteReader();
                    dto.HasError = false;
                }
            }
            catch (Exception ex)
            {
                dto.HasError = true;
                dto.ErrorMessage = ex.Message;
            }
            return dto;
        }

        /// <summary>
        /// Update DB with tag transactions (replicates StartUpdateDB)
        /// </summary>
        private void StartUpdateDB(string value)
        {
            RfidReaderItem reader;
            if (!_readerList.TryGetValue(_currentHostName, out reader))
                return;

            foreach (var tagItem in reader.TagDetected)
            {
                LogMessage($"StartUpdateDB >> pTRAN_TYPE: {value}; pRFID: {tagItem.Key}; pReader: {reader.HostName}; pIPADDR: {reader.IPAddress}; SP: {reader.StoredProcedure}");
                RfidCommonMSSQL(value, tagItem.Key, reader.HostName, reader.IPAddress, reader.StoredProcedure);
            }

            reader.TagDetected.Clear();
            reader.TagTime.Clear();
        }

        /// <summary>
        /// Update Tower DB with tag transactions (replicates StartUpdateDBTower)
        /// </summary>
        private void StartUpdateDBTower(string value)
        {
            RfidReaderItem reader;
            if (!_readerList.TryGetValue(_currentHostName, out reader))
                return;

            foreach (var tagItem in reader.TagDetected)
            {
                LogMessage($"StartUpdateDBTower >> pTRAN_TYPE: {value}; pRFID: {tagItem.Key}; pReader: {reader.HostName}; pIPADDR: {reader.IPAddress}; SP: {reader.StoredProcedure}");
                RfidCommonMSSQLTower(value, tagItem.Key, reader.HostName, reader.IPAddress, reader.StoredProcedure);
            }

            reader.TagDetected.Clear();
            reader.TagTime.Clear();
        }

        #endregion

        #region Timer Callbacks (replicates WinForm timers)

        /// <summary>
        /// Clear old tags from hashtable (replicates Reset_Table_Tick - 10s)
        /// </summary>
        private void ResetTableTick(object state)
        {
            foreach (var kvp in _readerList)
            {
                var reader = kvp.Value;
                var toRemove = new List<string>();

                foreach (var tagKvp in reader.TagTime)
                {
                    if (tagKvp.Value.AddMinutes(2) < DateTime.UtcNow)
                    {
                        toRemove.Add(tagKvp.Key);
                    }
                }

                foreach (var key in toRemove)
                {
                    DateTime removed;
                    bool removedBool;
                    reader.TagTime.TryRemove(key, out removed);
                    reader.TagDetected.TryRemove(key, out removedBool);
                }
            }
        }

        /// <summary>
        /// Reconnect failed readers (replicates TimerReconnect_Tick - 5s)
        /// </summary>
        private void ReconnectTimerTick(object state)
        {
            foreach (var kvp in _readerList)
            {
                var reader = kvp.Value;
                if (reader.ReconnectRequired)
                {
                    Task.Run(() => ConnectReader(reader));
                }
            }

            // Retry pending emails
            lock (_pendingEmailList)
            {
                foreach (var email in _pendingEmailList.ToList())
                {
                    var dto = SendMailMSSQL(email.Body, email.Subject, email.MailTo);
                    if (!dto.HasError)
                    {
                        email.Sent = true;
                    }
                }
                _pendingEmailList.RemoveAll(e => e.Sent);
            }
        }

        private void SendDCEmailTick(object state)
        {
            SendReaderNotificationEmail(_readerEmailDCList, "disconnection");
            lock (_readerEmailDCList) { _readerEmailDCList.Clear(); }
            _sendDCEmailTimer?.Dispose();
            _sendDCEmailTimer = null;
        }

        private void SendRCEmailTick(object state)
        {
            SendReaderNotificationEmail(_readerEmailRCList, "reconnection");
            lock (_readerEmailRCList) { _readerEmailRCList.Clear(); }
            _sendRCEmailTimer?.Dispose();
            _sendRCEmailTimer = null;
        }

        private void SendFailEmailTick(object state)
        {
            SendReaderNotificationEmail(_readerEmailFailList, "failure");
            lock (_readerEmailFailList) { _readerEmailFailList.Clear(); }
            _sendFailEmailTimer?.Dispose();
            _sendFailEmailTimer = null;
        }

        private void SendTagEmailTick(object state)
        {
            string company = ConfigurationManager.AppSettings["Company"] ?? "Film";
            string mailTo = ConfigurationManager.AppSettings["READER_NOTIFICATION_MAILTO"] ?? "";

            lock (_readerEmailTagList)
            {
                int batchSize = 33;
                for (int i = 0; i < _readerEmailTagList.Count; i += batchSize)
                {
                    var batch = _readerEmailTagList.Skip(i).Take(batchSize).ToList();
                    string body = BuildTagEmailBody(batch);
                    string subject = $"[{company}] Reader Notification: These tags requires attention";

                    var dto = SendMailMSSQL(body, subject, mailTo);
                    if (dto.HasError)
                    {
                        lock (_pendingEmailList)
                        {
                            _pendingEmailList.Add(new EmailNotification { Body = body, Subject = subject, MailTo = mailTo });
                        }
                    }
                }
                _readerEmailTagList.Clear();
            }

            _sendTagEmailTimer?.Dispose();
            _sendTagEmailTimer = null;
        }

        private void TagLockTimerTick(object state)
        {
            _lockTag = "";
            _tagLockTimer?.Dispose();
            _tagLockTimer = null;
        }

        private void StartTagLockTimer()
        {
            _tagLockTimer?.Dispose();
            _tagLockTimer = new Timer(TagLockTimerTick, null, 180000, Timeout.Infinite);
        }

        private void StartSendTagEmailTimer()
        {
            if (_sendTagEmailTimer == null)
                _sendTagEmailTimer = new Timer(SendTagEmailTick, null, 30000, Timeout.Infinite);
        }

        #endregion

        #region Email Helpers

        private void AddFailEmail(RfidReaderItem reader, string message)
        {
            lock (_readerEmailFailList)
            {
                _readerEmailFailList.Add(new ReaderEmailInfo
                {
                    DeviceName = reader.HostName,
                    IPAddress = reader.IPAddress,
                    LocationDesc = reader.Name,
                    TimeOccured = DateTime.Now,
                    Message = message
                });
            }
            if (_sendFailEmailTimer == null)
                _sendFailEmailTimer = new Timer(SendFailEmailTick, null, 30000, Timeout.Infinite);
        }

        private void AddDisconnectEmail(RfidReaderItem reader, string message)
        {
            lock (_readerEmailDCList)
            {
                _readerEmailDCList.Add(new ReaderEmailInfo
                {
                    DeviceName = reader.HostName,
                    IPAddress = reader.IPAddress,
                    LocationDesc = reader.Name,
                    TimeOccured = DateTime.Now,
                    Message = message
                });
            }
            if (_sendDCEmailTimer == null)
                _sendDCEmailTimer = new Timer(SendDCEmailTick, null, 30000, Timeout.Infinite);
        }

        private void AddReconnectEmail(RfidReaderItem reader, string message)
        {
            lock (_readerEmailRCList)
            {
                _readerEmailRCList.Add(new ReaderEmailInfo
                {
                    DeviceName = reader.HostName,
                    IPAddress = reader.IPAddress,
                    LocationDesc = reader.Name,
                    TimeOccured = DateTime.Now,
                    Message = message
                });
            }
            if (_sendRCEmailTimer == null)
                _sendRCEmailTimer = new Timer(SendRCEmailTick, null, 30000, Timeout.Infinite);
        }

        private void SendReaderNotificationEmail(List<ReaderEmailInfo> readerList, string notificationType)
        {
            string company = ConfigurationManager.AppSettings["Company"] ?? "Film";
            string mailTo = ConfigurationManager.AppSettings["READER_NOTIFICATION_MAILTO"] ?? "";

            string body = BuildReaderEmailBody(readerList, company);
            string subject = $"[{company}] Reader Notification: These readers requires attention";

            var dto = SendMailMSSQL(body, subject, mailTo);
            if (dto.HasError)
            {
                lock (_pendingEmailList)
                {
                    _pendingEmailList.Add(new EmailNotification { Body = body, Subject = subject, MailTo = mailTo });
                }
            }
        }

        private string BuildReaderEmailBody(List<ReaderEmailInfo> readers, string company)
        {
            string body = "<html><body>";
            body += $"<h3>{company} RFID Reader Notification</h3>";
            body += "<table border='1' cellpadding='5' cellspacing='0'>";
            body += "<tr><th>Device Name</th><th>IP Address</th><th>Location</th><th>Time</th><th>Message</th></tr>";

            lock (readers)
            {
                foreach (var r in readers)
                {
                    body += $"<tr><td>{r.DeviceName}</td><td>{r.IPAddress}</td><td>{r.LocationDesc}</td><td>{r.TimeOccured:yyyy-MM-dd HH:mm:ss}</td><td>{r.Message}</td></tr>";
                }
            }

            body += "</table></body></html>";
            return body;
        }

        private string BuildTagEmailBody(List<ReaderTagInfo> tags)
        {
            string body = "<html><body>";
            body += "<h3>RFID Tag Detection Notification</h3>";
            body += "<table border='1' cellpadding='5' cellspacing='0'>";
            body += "<tr><th>Location</th><th>Antenna ID</th><th>Tag ID</th><th>Time</th></tr>";

            foreach (var t in tags)
            {
                body += $"<tr><td>{t.Location}</td><td>{t.AntennaID}</td><td>{t.TagID}</td><td>{t.TimeOccured:yyyy-MM-dd HH:mm:ss}</td></tr>";
            }

            body += "</table></body></html>";
            return body;
        }

        #endregion

        #region SignalR Broadcasting

        private void BroadcastReaderStatus(RfidReaderItem reader)
        {
            try
            {
                var context = GlobalHost.ConnectionManager.GetHubContext<RfidHub>();
                context.Clients.All.updateReaderStatus(new
                {
                    ipAddress = reader.IPAddress,
                    status = reader.Status,
                    isConnected = reader.IsConnected,
                    location = reader.Name,
                    readerName = reader.HostName
                });
            }
            catch
            {
                // SignalR may not be initialized yet
            }
        }

        private void BroadcastTagDetected(string hostName, string location, string tagID, string antennaID)
        {
            try
            {
                var context = GlobalHost.ConnectionManager.GetHubContext<RfidHub>();
                context.Clients.All.tagDetected(new
                {
                    hostName = hostName,
                    location = location,
                    tagID = tagID,
                    antennaID = antennaID,
                    time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
            catch
            {
                // SignalR may not be initialized yet
            }
        }

        #endregion

        #region Cleanup

        /// <summary>
        /// Graceful shutdown - disconnect all readers (replicates bg_FormClosing_DoWork)
        /// </summary>
        public void Shutdown()
        {
            _resetTableTimer?.Dispose();
            _reconnectTimer?.Dispose();
            _sendDCEmailTimer?.Dispose();
            _sendRCEmailTimer?.Dispose();
            _sendFailEmailTimer?.Dispose();
            _sendTagEmailTimer?.Dispose();
            _loopCoilTimer?.Dispose();
            _tagLockTimer?.Dispose();

            foreach (var kvp in _readerList)
            {
                try
                {
                    DisconnectReaderHardware(kvp.Value);
                }
                catch { }
            }
        }

        #endregion

        #region Logging

        private void LogMessage(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[RFIDService] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
        }

        #endregion
    }

    #region Model Classes

    public class RfidReaderItem
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public string IPAddress { get; set; }
        public string HostName { get; set; }
        public string Port { get; set; }
        public string StoredProcedure { get; set; }
        public string Server { get; set; }
        public string StoredProcedure2 { get; set; }
        public string Server2 { get; set; }
        public bool IsConnected { get; set; }
        public bool ReconnectRequired { get; set; }
        public string Status { get; set; }
        public ConcurrentDictionary<string, bool> TagDetected { get; set; }
        public ConcurrentDictionary<string, DateTime> TagTime { get; set; }
        // When using SDK: public Symbol.RFID3.RFIDReader ReaderAPI { get; set; }
    }

    public class ReaderViewModel
    {
        public int Index { get; set; }
        public string Location { get; set; }
        public string IPAddress { get; set; }
        public string ReaderName { get; set; }
        public string Status { get; set; }
        public bool IsConnected { get; set; }
    }

    public class ServiceResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class DTOResult
    {
        public DataTable Table { get; set; }
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class ReaderEmailInfo
    {
        public string DeviceName { get; set; }
        public string IPAddress { get; set; }
        public string LocationDesc { get; set; }
        public DateTime TimeOccured { get; set; }
        public string Message { get; set; }
    }

    public class ReaderTagInfo
    {
        public string Host { get; set; }
        public string Location { get; set; }
        public string AntennaID { get; set; }
        public string TagID { get; set; }
        public DateTime TimeOccured { get; set; }
    }

    public class EmailNotification
    {
        public string Body { get; set; }
        public string Subject { get; set; }
        public string MailTo { get; set; }
        public bool Sent { get; set; }
    }

    #endregion
}
