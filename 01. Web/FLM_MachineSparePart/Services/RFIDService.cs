using FILM_Sparepart_MVC.Hubs;
using Microsoft.AspNet.SignalR;
using Symbol.RFID3;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using static Symbol.RFID3.Events;

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

                // Force-disconnect any existing readers before reloading
                // This handles the case where a previous debug session was killed abruptly
                foreach (var kvp in _readerList)
                {
                    try { DisconnectReaderHardware(kvp.Value); }
                    catch { }
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

                    uint port = 0;
                    if (!uint.TryParse(readerItem.Port != null ? readerItem.Port.Trim() : "", out port) || port == 0)
                        port = 5084;

                    readerItem.ReaderAPI = new RFIDReader(readerItem.IPAddress, port, 50000);

                    _readerList.TryAdd(readerItem.IPAddress, readerItem);
                    readerIndex++;
                }

                _resetTableTimer = new Timer(ResetTableTick, null, 10000, 10000);
                _reconnectTimer = new Timer(ReconnectTimerTick, null, 5000, 5000);

                _isInitialized = true;
                result.Success = true;
                result.Message = string.Format("{0} reader(s) loaded from configuration.", dto.Table.Rows.Count);
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
                // Guard against concurrent connect calls only
                if (reader.IsConnected && reader.ReaderAPI != null && reader.ReaderAPI.IsConnected)
                {
                    reader.Status = "Connected";
                    BroadcastReaderStatus(reader);
                    return;
                }

                bool success = false;

                if (reader.ReconnectRequired)
                {
                    try
                    {
                        ReconnectReaderHardware(reader);
                        success = true;
                    }
                    catch
                    {
                        try
                        {
                            uint port = 0;
                            if (!uint.TryParse(reader.Port != null ? reader.Port.Trim() : "", out port) || port == 0)
                                port = 5084;

                            reader.ReaderAPI = new RFIDReader(reader.IPAddress, port, 50000);
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
                    LogMessage(string.Format("Reader {0} ({1}) connected successfully.", reader.HostName, reader.IPAddress));
                }
            }
            catch (OperationFailureException ofe)
            {
                reader.Status = ofe.StatusDescription + " : " + reader.IPAddress;
                reader.IsConnected = false;
                AddFailEmail(reader, "Connect failed. " + ofe.StatusDescription);
                BroadcastReaderStatus(reader);
                LogMessage(string.Format("Reader {0} ({1}) OperationFailure: {2}", reader.HostName, reader.IPAddress, ofe.StatusDescription));
            }
            catch (Exception ex)
            {
                reader.Status = ex.Message;
                reader.IsConnected = false;
                AddFailEmail(reader, "Connect failed. " + ex.Message);
                BroadcastReaderStatus(reader);
                LogMessage(string.Format("Reader {0} ({1}) connection failed: {2}", reader.HostName, reader.IPAddress, ex.Message));
            }
        }

        /// <summary>
        /// Core disconnect logic (replicates BackgroundWorkerDisconnectReader_DoWork + RunWorkerCompleted)
        /// </summary>
        private void DisconnectReader(RfidReaderItem reader)
        {
            try
            {
                if (reader.ReaderAPI != null && reader.ReaderAPI.IsConnected)
                {
                    DisconnectReaderHardware(reader);
                }

                reader.IsConnected = false;
                reader.Status = "Disconnect";
                BroadcastReaderStatus(reader);
                LogMessage(string.Format("Reader {0} ({1}) disconnected.", reader.HostName, reader.IPAddress));
            }
            catch (Exception ex)
            {
                reader.IsConnected = false;
                reader.Status = "Disconnect";
                BroadcastReaderStatus(reader);
                LogMessage(string.Format("Error disconnecting reader {0}: {1}", reader.HostName, ex.Message));
            }
        }

        #endregion

        #region Hardware Abstraction (Symbol.RFID3 SDK)

        /// <summary>
        /// Replicates: readerItem.m_ReaderAPI.Connect() + ConnectBackgroundWorker_RunWorkerCompleted event wiring
        /// </summary>
        private void ConnectReaderHardware(RfidReaderItem reader)
        {
            reader.ReaderAPI.Connect();
            AttachEventHandlers(reader);

            // Replicates: readerItem.m_ReaderAPI.Actions.Inventory.Perform(Nothing, Nothing, antennaInfo)
            var antennaList = new ushort[] { 1, 2 };
            var antennaInfo = new AntennaInfo(antennaList);
            try
            {
                reader.ReaderAPI.Actions.Inventory.Perform(null, null, antennaInfo);
            }
            catch (OperationFailureException ex)
            {
                LogMessage(string.Format("Inventory.Perform failed for {0}: {1}", reader.IPAddress, ex.Result));
            }

            LogMessage(string.Format("ConnectReaderHardware: {0}:{1}", reader.IPAddress, reader.Port));
        }

        /// <summary>
        /// Replicates: readerItem.m_ReaderAPI.Reconnect() in ReconnectBackgroundWorker_DoWork
        /// </summary>
        private void ReconnectReaderHardware(RfidReaderItem reader)
        {
            reader.ReaderAPI.Reconnect();
            LogMessage(string.Format("ReconnectReaderHardware: {0}", reader.IPAddress));
        }

        /// <summary>
        /// Replicates: BackgroundWorkerDisconnectReader_DoWork disconnect sequence
        /// </summary>
        private void DisconnectReaderHardware(RfidReaderItem reader)
        {
            if (reader.ReaderAPI == null)
                return;

            try
            {
                if (reader.ReaderAPI.IsConnected)
                {
                    try
                    {
                        if (reader.ReaderAPI.Actions.TagAccess.OperationSequence.Length > 0)
                        {
                            reader.ReaderAPI.Actions.TagAccess.OperationSequence.StopSequence();
                            reader.ReaderAPI.Actions.Inventory.Stop();
                        }
                        else
                        {
                            reader.ReaderAPI.Actions.Inventory.Stop();
                        }
                    }
                    catch { }

                    reader.ReaderAPI.Disconnect();
                    LogMessage(string.Format("DisconnectReaderHardware: {0}", reader.IPAddress));
                }
            }
            catch (Exception ex)
            {
                LogMessage(string.Format("DisconnectReaderHardware error [{0}]: {1}", reader.IPAddress, ex.Message));
            }
            finally
            {
                // Always recreate the RFIDReader object to fully reset SDK state
                // This prevents "connection already exists" on next connect attempt
                uint port = 0;
                if (!uint.TryParse(reader.Port != null ? reader.Port.Trim() : "", out port) || port == 0)
                    port = 5084;

                reader.ReaderAPI = new RFIDReader(reader.IPAddress, port, 50000);
                LogMessage(string.Format("ReaderAPI recreated for {0}", reader.IPAddress));
            }
        }

        /// <summary>
        /// Attach SDK event handlers (replicates ConnectBackgroundWorker_RunWorkerCompleted wiring)
        /// </summary>
        private void AttachEventHandlers(RfidReaderItem reader)
        {
            bool isTestEnv = string.Equals(
                ConfigurationManager.AppSettings["TEST_ENVIRONMENT"],
                "true",
                StringComparison.OrdinalIgnoreCase);

            // Replicates: If Not My.Settings.TEST_ENVIRONMENT Then ... AddHandler ReadNotify
            if (!isTestEnv)
            {
                reader.ReaderAPI.Events.ReadNotify += (sender, e) => OnReadNotify(reader, e);
                reader.ReaderAPI.Events.AttachTagDataWithReadEvent = false;
            }

            reader.ReaderAPI.Events.StatusNotify += (sender, e) => OnStatusNotify(reader, e);
            reader.ReaderAPI.Events.NotifyGPIEvent = true;
            reader.ReaderAPI.Events.NotifyBufferFullEvent = true;
            reader.ReaderAPI.Events.NotifyBufferFullWarningEvent = true;
            reader.ReaderAPI.Events.NotifyReaderDisconnectEvent = true;
            reader.ReaderAPI.Events.NotifyReaderExceptionEvent = true;
            reader.ReaderAPI.Events.NotifyAccessStartEvent = true;
            reader.ReaderAPI.Events.NotifyAccessStopEvent = true;
            reader.ReaderAPI.Events.NotifyInventoryStartEvent = true;
            reader.ReaderAPI.Events.NotifyInventoryStopEvent = true;
        }

        #endregion

        #region SDK Event Handlers

        /// <summary>
        /// Replicates: Events_ReadNotify -> myUpdateRead in Form1.vb
        /// </summary>
        private void OnReadNotify(RfidReaderItem reader, ReadEventArgs e)
        {
            try
            {
                TagData[] tagData = reader.ReaderAPI.Actions.GetReadTags(50);
                if (tagData == null)
                    return;

                var updateList = new List<string>();

                for (int i = 0; i < tagData.Length; i++)
                {
                    TagData tag = tagData[i];

                    if (tag.OpCode != ACCESS_OPERATION_CODE.ACCESS_OPERATION_NONE &&
                        !(tag.OpCode == ACCESS_OPERATION_CODE.ACCESS_OPERATION_READ &&
                          tag.OpStatus == ACCESS_OPERATION_STATUS.ACCESS_SUCCESS))
                        continue;

                    string tagID = tag.TagID;
                    string antennaID = tag.AntennaID.ToString();

                    bool isNewTag = reader.TagDetected.TryAdd(tagID, false);
                    if (isNewTag)
                        reader.TagTime.TryAdd(tagID, DateTime.UtcNow);

                    // Replicates the tag email block in myUpdateRead for Tower Zone host
                    string location = GetLocationName(reader.IPAddress);
                    if (reader.IPAddress == "10.28.92.50" && !string.IsNullOrEmpty(tagID))
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
                                    Host = reader.IPAddress,
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

                    if (!reader.TagDetected[tagID])
                        updateList.Add(tagID);

                    BroadcastTagDetected(reader.IPAddress, location, tagID, antennaID);
                }

                // Replicates: mark tags as processed outside the loop to avoid collection exception
                foreach (var key in updateList)
                    reader.TagDetected[key] = true;
            }
            catch (Exception ex)
            {
                LogMessage(string.Format("OnReadNotify error [{0}]: {1}", reader.IPAddress, ex.Message));
            }
        }

        /// <summary>
        /// Replicates: Events_StatusNotify -> myUpdateStatus in Form1.vb
        /// </summary>
        private void OnStatusNotify(RfidReaderItem reader, StatusEventArgs e)
        {
            try
            {
                string statusMsg = "";
                StatusEventData eventData = e.StatusEventData;

                switch (eventData.StatusEventType)
                {
                    case STATUS_EVENT_TYPE.INVENTORY_START_EVENT:
                        statusMsg = "Inventory started";
                        break;

                    case STATUS_EVENT_TYPE.INVENTORY_STOP_EVENT:
                        statusMsg = "Inventory stopped";
                        break;

                    case STATUS_EVENT_TYPE.ACCESS_START_EVENT:
                        statusMsg = "Access Operation started";
                        break;

                    case STATUS_EVENT_TYPE.ACCESS_STOP_EVENT:
                        statusMsg = "Access Operation stopped";
                        break;

                    case STATUS_EVENT_TYPE.BUFFER_FULL_WARNING_EVENT:
                        statusMsg = "Buffer full warning";
                        break;

                    case STATUS_EVENT_TYPE.BUFFER_FULL_EVENT:
                        statusMsg = "Buffer full";
                        break;

                    case STATUS_EVENT_TYPE.DISCONNECTION_EVENT:
                        // Replicates: reader.bool_ReconnectRequired = True + run ReconnectBackgroundWorker
                        statusMsg = "Disconnection Event " + eventData.DisconnectionEventData.DisconnectEventInfo.ToString();
                        reader.ReconnectRequired = true;
                        reader.IsConnected = false;
                        reader.Status = "Disconnect";
                        BroadcastReaderStatus(reader);
                        AddDisconnectEmail(reader, statusMsg);
                        LogMessage(string.Format("Reader {0} disconnected: {1}", reader.IPAddress, statusMsg));
                        break;

                    case STATUS_EVENT_TYPE.ANTENNA_EVENT:
                        statusMsg = "Antenna Status Update";
                        break;

                    case STATUS_EVENT_TYPE.NXP_EAS_ALARM_EVENT:
                        break;

                    case STATUS_EVENT_TYPE.READER_EXCEPTION_EVENT:
                        statusMsg = "Reader ExceptionEvent " + eventData.ReaderExceptionEventData.ReaderExceptionEventInfo;
                        break;

                    case STATUS_EVENT_TYPE.GPI_EVENT:
                        // Replicates: Timer1.Enabled check -> updateIn or check2ndloop
                        HandleGPIEvent(
                            reader.IPAddress,
                            eventData.GPIEventData.PortNumber,
                            eventData.GPIEventData.GPIEvent);
                        break;

                    default:
                        statusMsg = "Unhandled Status";
                        break;
                }

                // Replicates: If Not reader.m_ReaderAPI.IsConnected Then ... Reconnect()
                if (reader.ReaderAPI != null && !reader.ReaderAPI.IsConnected &&
                    eventData.StatusEventType != STATUS_EVENT_TYPE.DISCONNECTION_EVENT)
                {
                    try
                    {
                        reader.ReaderAPI.Reconnect();
                        statusMsg += ", Reconnect success.";
                        reader.IsConnected = true;
                        reader.Status = "Connected";
                        BroadcastReaderStatus(reader);
                    }
                    catch
                    {
                        statusMsg += ", Reconnect fail.";
                        reader.ReconnectRequired = true;
                    }
                }

                if (!string.IsNullOrEmpty(statusMsg))
                    LogMessage(string.Format("[{0}] {1}", reader.IPAddress, statusMsg));
            }
            catch (Exception ex)
            {
                LogMessage(string.Format("OnStatusNotify error [{0}]: {1}", reader.IPAddress, ex.Message));
            }
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

            reader.TagDetected[tagID] = true;

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
            // GPI_PORT_STATE_LOW (false) = active, replicates: Config.GPI.Item(1).PortState = 0
            if (portNumber == 1 && !portState)
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
                        throw new Exception(cmd.Parameters["@pRETURN_VALUE1"].Value.ToString());

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
                        throw new Exception(cmd.Parameters["@pRETURN_VALUE1"].Value.ToString());

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

        private void StartUpdateDB(string value)
        {
            RfidReaderItem reader;
            if (!_readerList.TryGetValue(_currentHostName, out reader))
                return;

            foreach (var tagItem in reader.TagDetected)
            {
                LogMessage(string.Format("StartUpdateDB >> pTRAN_TYPE: {0}; pRFID: {1}; pReader: {2}; pIPADDR: {3}; SP: {4}",
                    value, tagItem.Key, reader.HostName, reader.IPAddress, reader.StoredProcedure));
                RfidCommonMSSQL(value, tagItem.Key, reader.HostName, reader.IPAddress, reader.StoredProcedure);
            }

            reader.TagDetected.Clear();
            reader.TagTime.Clear();
        }

        private void StartUpdateDBTower(string value)
        {
            RfidReaderItem reader;
            if (!_readerList.TryGetValue(_currentHostName, out reader))
                return;

            foreach (var tagItem in reader.TagDetected)
            {
                LogMessage(string.Format("StartUpdateDBTower >> pTRAN_TYPE: {0}; pRFID: {1}; pReader: {2}; pIPADDR: {3}; SP: {4}",
                    value, tagItem.Key, reader.HostName, reader.IPAddress, reader.StoredProcedure));
                RfidCommonMSSQLTower(value, tagItem.Key, reader.HostName, reader.IPAddress, reader.StoredProcedure);
            }

            reader.TagDetected.Clear();
            reader.TagTime.Clear();
        }

        #endregion

        #region Timer Callbacks (replicates WinForm timers)

        private void ResetTableTick(object state)
        {
            foreach (var kvp in _readerList)
            {
                var reader = kvp.Value;
                var toRemove = new List<string>();

                foreach (var tagKvp in reader.TagTime)
                {
                    if (tagKvp.Value.AddMinutes(2) < DateTime.UtcNow)
                        toRemove.Add(tagKvp.Key);
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

            lock (_pendingEmailList)
            {
                foreach (var email in _pendingEmailList.ToList())
                {
                    var dto = SendMailMSSQL(email.Body, email.Subject, email.MailTo);
                    if (!dto.HasError)
                        email.Sent = true;
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
                    string subject = string.Format("[{0}] Reader Notification: These tags requires attention", company);

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
            string subject = string.Format("[{0}] Reader Notification: These readers requires attention", company);

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
            body += string.Format("<h3>{0} RFID Reader Notification</h3>", company);
            body += "<table border='1' cellpadding='5' cellspacing='0'>";
            body += "<tr><th>Device Name</th><th>IP Address</th><th>Location</th><th>Time</th><th>Message</th></tr>";

            lock (readers)
            {
                foreach (var r in readers)
                {
                    body += string.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3:yyyy-MM-dd HH:mm:ss}</td><td>{4}</td></tr>",
                        r.DeviceName, r.IPAddress, r.LocationDesc, r.TimeOccured, r.Message);
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
                body += string.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3:yyyy-MM-dd HH:mm:ss}</td></tr>",
                    t.Location, t.AntennaID, t.TagID, t.TimeOccured);
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
            catch { }
        }

        /// <summary>
        /// Returns the actual SDK-level connection state, independent of the tracked IsConnected flag.
        /// Useful for diagnosing stale connection states.
        /// </summary>
        public bool IsReaderSDKConnected(string ipAddress)
        {
            RfidReaderItem reader;
            if (!_readerList.TryGetValue(ipAddress, out reader))
                return false;

            try
            {
                return reader.ReaderAPI != null && reader.ReaderAPI.IsConnected;
            }
            catch
            {
                return false;
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
            catch { }
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
                try { DisconnectReaderHardware(kvp.Value); }
                catch { }
            }
        }

        #endregion

        #region Logging

        private void LogMessage(string message)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("[RFIDService] {0:yyyy-MM-dd HH:mm:ss} - {1}", DateTime.Now, message));
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
        // Replicates: Friend m_ReaderAPI As RFIDReader
        public Symbol.RFID3.RFIDReader ReaderAPI { get; set; }
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