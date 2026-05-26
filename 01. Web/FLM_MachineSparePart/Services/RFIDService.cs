using System.Collections;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using System.Data;
using FILM_Sparepart_MVC.Hubs;
using FILM_Sparepart_MVC.Models;
using Symbol.RFID3;
using Symbol.RFID3.Events;

namespace FILM_Sparepart_MVC.Services
{
    /// <summary>
    /// Background service that manages RFID reader connections, tag reading,
    /// GPI loop-coil events, database transactions, reconnection logic,
    /// and email notifications — replicating the WinForm (02. RFID) behaviour
    /// entirely within the ASP.NET Core web application.
    /// </summary>
    public class RFIDService : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<RFIDService> _logger;
        private readonly IHubContext<RfidHub> _hubContext;
        private readonly IServiceProvider _serviceProvider;

        // Reader management
        private readonly ConcurrentDictionary<string, RfidReaderItem> _readerList = new();

        // Loop-coil state (per-reader tracked via RfidReaderItem)
        private readonly object _loopCoilLock = new();
        private string _loopCoil1 = "";
        private string _loopCoil2 = "";
        private string _activeHostName = "";
        private string _towerDirection = "";

        // Timers
        private Timer? _reconnectTimer;
        private Timer? _resetTableTimer;
        private Timer? _loopCoilTimer;

        // Email lists (thread-safe)
        private readonly ConcurrentBag<ReaderEmailInfo> _dcEmailList = new();
        private readonly ConcurrentBag<ReaderEmailInfo> _rcEmailList = new();
        private readonly ConcurrentBag<ReaderEmailInfo> _failEmailList = new();
        private Timer? _dcEmailTimer;
        private Timer? _rcEmailTimer;
        private Timer? _failEmailTimer;

        // Tag lock (mirrors WinForm lockTag logic)
        private string _lockTag = "";
        private Timer? _tagLockTimer;

        public RFIDService(
            IConfiguration configuration,
            ILogger<RFIDService> logger,
            IHubContext<RfidHub> hubContext,
            IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _logger = logger;
            _hubContext = hubContext;
            _serviceProvider = serviceProvider;
        }

        #region BackgroundService Lifecycle

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("RFIDService starting...");

            // Load reader config from appsettings.json
            LoadReadersFromConfig();

            // Start periodic timers
            _reconnectTimer = new Timer(ReconnectTimerCallback, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
            _resetTableTimer = new Timer(ResetTableTimerCallback, null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));

            // Auto-connect on startup if configured
            var autoConnect = _configuration.GetValue<bool>("RFIDService:AutoConnectOnStartup");
            if (autoConnect)
            {
                _logger.LogInformation("AutoConnectOnStartup enabled — connecting all readers...");
                // Small delay to allow the app to fully start
                await Task.Delay(2000, stoppingToken);
                await ConnectAllReadersAsync();
            }

            // Keep running until shutdown
            try
            {
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Expected on shutdown
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("RFIDService stopping — disconnecting all readers...");
            await DisconnectAllReadersAsync();

            _reconnectTimer?.Dispose();
            _resetTableTimer?.Dispose();
            _loopCoilTimer?.Dispose();
            _dcEmailTimer?.Dispose();
            _rcEmailTimer?.Dispose();
            _failEmailTimer?.Dispose();
            _tagLockTimer?.Dispose();

            await base.StopAsync(cancellationToken);
        }

        #endregion

        #region Configuration

        /// <summary>
        /// Loads reader definitions from the "RfidReaders" section of appsettings.json.
        /// </summary>
        private void LoadReadersFromConfig()
        {
            var readers = _configuration.GetSection("RfidReaders").Get<List<RfidReaderConfig>>();
            if (readers == null || readers.Count == 0)
            {
                _logger.LogWarning("No RFID readers configured in appsettings.json");
                return;
            }

            int index = 0;
            foreach (var cfg in readers)
            {
                var item = new RfidReaderItem
                {
                    Index = index++,
                    Name = cfg.Name ?? cfg.HostName,
                    IpAddress = cfg.IpAddress,
                    HostName = cfg.HostName,
                    Port = cfg.Port ?? "5084",
                    StoredProcedure = cfg.StoredProcedure ?? "SP_FILM_UPDATE_TRAN",
                    Server = cfg.Server ?? "MSSQL",
                    StoredProcedure2 = cfg.StoredProcedure2,
                    Server2 = cfg.Server2,
                    Status = ReaderConnectionStatus.Disconnected,
                    TagDetected = new Hashtable(),
                    TagTime = new Hashtable()
                };

                _readerList.TryAdd(cfg.IpAddress, item);
                _logger.LogInformation("Configured reader: {Name} ({Ip}:{Port})", item.Name, item.IpAddress, item.Port);
            }
        }

        /// <summary>
        /// Returns the list of configured readers for use by controllers (e.g., RfidConnectController).
        /// </summary>
        public List<ReaderRFIDModel> GetDefaultReaders()
        {
            return _readerList.Values.Select(r => new ReaderRFIDModel
            {
                IP_ADDRESS = r.IpAddress,
                HOST_NAME = r.HostName,
                PORT = r.Port,
                SP = r.StoredProcedure,
                SERVER = r.Server,
                SP2 = r.StoredProcedure2,
                SERVER2 = r.Server2,
                LOCATION = r.Name
            }).ToList();
        }

        /// <summary>
        /// Returns the live connection status of every reader for SignalR clients.
        /// </summary>
        public List<ReaderStatusDto> GetReaderStatuses()
        {
            return _readerList.Values.Select(r => new ReaderStatusDto
            {
                IpAddress = r.IpAddress,
                HostName = r.HostName,
                Name = r.Name,
                Status = r.Status.ToString(),
                ReconnectRequired = r.ReconnectRequired
            }).ToList();
        }

        #endregion

        #region Connect / Disconnect

        public async Task ConnectAllReadersAsync()
        {
            var tasks = _readerList.Values.Select(r => ConnectReaderAsync(r.IpAddress));
            await Task.WhenAll(tasks);
        }

        public async Task DisconnectAllReadersAsync()
        {
            var tasks = _readerList.Values.Select(r => DisconnectReaderAsync(r.IpAddress));
            await Task.WhenAll(tasks);
        }

        public Task ConnectReaderAsync(string ipAddress)
        {
            return Task.Run(async () =>
            {
                if (!_readerList.TryGetValue(ipAddress, out var reader))
                {
                    _logger.LogWarning("ConnectReader: reader {Ip} not found", ipAddress);
                    return;
                }

                if (reader.ReaderApi != null && reader.ReaderApi.IsConnected)
                {
                    reader.Status = ReaderConnectionStatus.Connected;
                    await BroadcastReaderStatus(reader);
                    return;
                }

                reader.Status = ReaderConnectionStatus.Connecting;
                await BroadcastReaderStatus(reader);

                try
                {
                    if (reader.ReconnectRequired && reader.ReaderApi != null)
                    {
                        try
                        {
                            reader.ReaderApi.Reconnect();
                        }
                        catch
                        {
                            // Reconnect failed — create a new instance
                            reader.ReaderApi = new RFIDReader(reader.IpAddress, reader.Port, 50000);
                            reader.ReaderApi.Connect();
                        }
                    }
                    else
                    {
                        reader.ReaderApi = new RFIDReader(reader.IpAddress, reader.Port, 50000);
                        reader.ReaderApi.Connect();
                    }

                    // Register event handlers
                    reader.ReaderApi.Events.ReadNotify += (sender, args) => OnReadNotify(sender, args, reader);
                    reader.ReaderApi.Events.StatusNotify += (sender, args) => OnStatusNotify(sender, args, reader);
                    reader.ReaderApi.Events.AttachTagDataWithReadEvent = false;
                    reader.ReaderApi.Events.NotifyGPIEvent = true;
                    reader.ReaderApi.Events.NotifyBufferFullEvent = true;
                    reader.ReaderApi.Events.NotifyBufferFullWarningEvent = true;
                    reader.ReaderApi.Events.NotifyReaderDisconnectEvent = true;
                    reader.ReaderApi.Events.NotifyReaderExceptionEvent = true;
                    reader.ReaderApi.Events.NotifyAccessStartEvent = true;
                    reader.ReaderApi.Events.NotifyAccessStopEvent = true;
                    reader.ReaderApi.Events.NotifyInventoryStartEvent = true;
                    reader.ReaderApi.Events.NotifyInventoryStopEvent = true;

                    // Start inventory on antenna 1 & 2
                    var antennaList = new ushort[] { 1, 2 };
                    var antennaInfo = new AntennaInfo(antennaList);
                    reader.ReaderApi.Actions.Inventory.Perform(null, null, antennaInfo);

                    // If this was a reconnect, send reconnect email
                    if (reader.ReconnectRequired)
                    {
                        reader.ReconnectRequired = false;
                        _rcEmailList.Add(new ReaderEmailInfo(reader.HostName, reader.IpAddress, reader.Name, DateTime.Now, "Reconnect successfully."));
                        EnsureEmailTimer(ref _rcEmailTimer, SendRCEmail);
                    }

                    reader.Status = ReaderConnectionStatus.Connected;
                    _logger.LogInformation("Reader {Name} ({Ip}) connected successfully", reader.Name, reader.IpAddress);
                }
                catch (OperationFailureException ex)
                {
                    reader.Status = ReaderConnectionStatus.Error;
                    _logger.LogError("Connect failed for {Ip}: {Msg}", reader.IpAddress, ex.StatusDescription);
                    AddFailEmail(reader, $"Connect failed: {ex.StatusDescription}");
                }
                catch (Exception ex)
                {
                    reader.Status = ReaderConnectionStatus.Error;
                    _logger.LogError(ex, "Connect failed for {Ip}", reader.IpAddress);
                    AddFailEmail(reader, $"Connect failed: {ex.Message}");
                }

                await BroadcastReaderStatus(reader);
            });
        }

        public Task DisconnectReaderAsync(string ipAddress)
        {
            return Task.Run(async () =>
            {
                if (!_readerList.TryGetValue(ipAddress, out var reader))
                    return;

                if (reader.ReaderApi == null || !reader.ReaderApi.IsConnected)
                {
                    reader.Status = ReaderConnectionStatus.Disconnected;
                    await BroadcastReaderStatus(reader);
                    return;
                }

                try
                {
                    try
                    {
                        if (reader.ReaderApi.Actions.TagAccess.OperationSequence.Length > 0)
                            reader.ReaderApi.Actions.TagAccess.OperationSequence.StopSequence();
                    }
                    catch { /* ignore */ }

                    try
                    {
                        reader.ReaderApi.Actions.Inventory.Stop();
                    }
                    catch { /* ignore */ }

                    reader.ReaderApi.Disconnect();
                    reader.Status = ReaderConnectionStatus.Disconnected;
                    _logger.LogInformation("Reader {Name} ({Ip}) disconnected", reader.Name, reader.IpAddress);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error disconnecting reader {Ip}", reader.IpAddress);
                    reader.Status = ReaderConnectionStatus.Error;
                }

                await BroadcastReaderStatus(reader);
            });
        }

        #endregion

        #region RFID Event Handlers

        /// <summary>
        /// Handles tag read notifications from the reader — equivalent to WinForm's myUpdateRead.
        /// </summary>
        private void OnReadNotify(object sender, ReadEventArgs readEventArgs, RfidReaderItem reader)
        {
            try
            {
                var tagData = reader.ReaderApi?.Actions.GetReadTags(50);
                if (tagData == null) return;

                foreach (var tag in tagData)
                {
                    if (tag.OpCode != ACCESS_OPERATION_CODE.ACCESS_OPERATION_NONE &&
                        !(tag.OpCode == ACCESS_OPERATION_CODE.ACCESS_OPERATION_READ &&
                          tag.OpStatus == ACCESS_OPERATION_STATUS.ACCESS_SUCCESS))
                        continue;

                    var tagId = tag.TagID;

                    lock (reader.TagDetected.SyncRoot)
                    {
                        if (!reader.TagDetected.ContainsKey(tagId))
                        {
                            reader.TagDetected.Add(tagId, false);
                            lock (reader.TagTime.SyncRoot)
                            {
                                reader.TagTime.Add(tagId, DateTime.UtcNow);
                            }

                            _logger.LogDebug("Tag detected: {TagId} on reader {Name}", tagId, reader.Name);

                            // Broadcast to web clients
                            _ = _hubContext.Clients.All.SendAsync("TagDetected", new
                            {
                                reader.IpAddress,
                                reader.HostName,
                                reader.Name,
                                TagId = tagId,
                                AntennaId = tag.AntennaID,
                                Rssi = tag.PeakRSSI,
                                Time = DateTime.Now
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnReadNotify for reader {Ip}", reader.IpAddress);
            }
        }

        /// <summary>
        /// Handles status notifications from the reader — equivalent to WinForm's myUpdateStatus.
        /// Processes GPI (loop coil) events, disconnections, reconnections, etc.
        /// </summary>
        private void OnStatusNotify(object sender, StatusEventArgs statusEventArgs, RfidReaderItem reader)
        {
            try
            {
                var eventData = statusEventArgs.StatusEventData;
                string statusMsg;

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
                        statusMsg = "Disconnection Event " + eventData.DisconnectionEventData.DisconnectEventInfo;
                        break;
                    case STATUS_EVENT_TYPE.ANTENNA_EVENT:
                        statusMsg = "Antenna Status Update";
                        break;
                    case STATUS_EVENT_TYPE.READER_EXCEPTION_EVENT:
                        statusMsg = "Reader Exception: " + eventData.ReaderExceptionEventData.ReaderExceptionEventInfo;
                        break;
                    case STATUS_EVENT_TYPE.GPI_EVENT:
                        HandleGpiEvent(reader, eventData);
                        return;
                    default:
                        statusMsg = "Unhandled Status";
                        break;
                }

                _logger.LogInformation("Reader {Name} ({Ip}): {Status}", reader.Name, reader.IpAddress, statusMsg);

                // Broadcast status to web clients
                _ = _hubContext.Clients.All.SendAsync("ReaderStatusEvent", new
                {
                    reader.IpAddress,
                    reader.HostName,
                    reader.Name,
                    Message = statusMsg,
                    Time = DateTime.Now
                });

                // Check if reader disconnected
                if (reader.ReaderApi != null && !reader.ReaderApi.IsConnected)
                {
                    reader.ReconnectRequired = true;
                    reader.Status = ReaderConnectionStatus.Disconnected;

                    _dcEmailList.Add(new ReaderEmailInfo(reader.HostName, reader.IpAddress, reader.Name, DateTime.Now, statusMsg));
                    EnsureEmailTimer(ref _dcEmailTimer, SendDCEmail);

                    _ = BroadcastReaderStatus(reader);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnStatusNotify for reader {Ip}", reader.IpAddress);
            }
        }

        #endregion

        #region GPI / Loop Coil Logic

        /// <summary>
        /// Handles GPI (loop coil) events — determines IN/OUT direction and triggers DB update.
        /// Replicates updateIn / check2ndloop / Timer1_Tick from WinForm.
        /// </summary>
        private void HandleGpiEvent(RfidReaderItem reader, StatusEventData eventData)
        {
            lock (_loopCoilLock)
            {
                _logger.LogInformation("GPI event on reader {Name} ({Ip}), Port: {Port}",
                    reader.Name, reader.IpAddress, eventData.GPIEventData.PortNumber);

                // If loop coil timer is already running, this is the 2nd coil trigger
                if (_loopCoilTimer != null)
                {
                    // Check 2nd loop
                    if (reader.ReaderApi!.Config.GPI[2].PortState == GPI_PORT_STATE.GPI_PORT_STATE_LOW)
                        _loopCoil2 = "In";
                    if (reader.ReaderApi!.Config.GPI[1].PortState == GPI_PORT_STATE.GPI_PORT_STATE_LOW)
                        _loopCoil2 = "Out";
                    if (_towerDirection == "1") _towerDirection = "Out";
                    if (_towerDirection == "2") _towerDirection = "In";
                }
                else
                {
                    // First coil trigger
                    if (reader.ReaderApi!.Config.GPI[1].PortState == GPI_PORT_STATE.GPI_PORT_STATE_LOW)
                    {
                        _activeHostName = reader.IpAddress;
                        if (reader.IpAddress == "10.28.92.50")
                            _towerDirection = "1";
                        else
                            _loopCoil1 = "In";
                    }

                    if (reader.ReaderApi!.Config.GPI[2].PortState == GPI_PORT_STATE.GPI_PORT_STATE_LOW)
                    {
                        _activeHostName = reader.IpAddress;
                        if (reader.IpAddress == "10.28.92.50")
                            _towerDirection = "2";
                        else
                            _loopCoil1 = "Out";
                    }

                    // Start loop coil timer (180 seconds, same as WinForm Timer1)
                    _loopCoilTimer = new Timer(LoopCoilTimerCallback, null, TimeSpan.FromSeconds(180), Timeout.InfiniteTimeSpan);
                }
            }
        }

        /// <summary>
        /// Fires after the loop coil timer expires — evaluates direction and writes to DB.
        /// Mirrors WinForm Timer1_Tick logic.
        /// </summary>
        private void LoopCoilTimerCallback(object? state)
        {
            lock (_loopCoilLock)
            {
                try
                {
                    if (!_readerList.TryGetValue(_activeHostName, out var reader))
                        return;

                    if (_loopCoil1 == "In" && _loopCoil1 == _loopCoil2)
                    {
                        StartUpdateDB(reader, "IN");
                    }
                    else if (_loopCoil1 == "Out" && _loopCoil1 == _loopCoil2)
                    {
                        StartUpdateDB(reader, "OUT");
                    }
                    else if (_towerDirection == "In")
                    {
                        StartUpdateDBTower(reader, "IN");
                    }
                    else if (_towerDirection == "Out")
                    {
                        StartUpdateDBTower(reader, "OUT");
                    }

                    // Clear tags
                    reader.TagDetected.Clear();
                    reader.TagTime.Clear();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in LoopCoilTimerCallback");
                }
                finally
                {
                    // Reset state
                    _loopCoil1 = "";
                    _loopCoil2 = "";
                    _towerDirection = "";
                    _activeHostName = "";
                    _loopCoilTimer?.Dispose();
                    _loopCoilTimer = null;
                }
            }
        }

        #endregion

        #region Database Operations

        /// <summary>
        /// Executes the RFID transaction stored procedure for each detected tag.
        /// Mirrors WinForm StartUpdateDB.
        /// </summary>
        private void StartUpdateDB(RfidReaderItem reader, string tranType)
        {
            var connectionString = _configuration.GetConnectionString("db_MS");
            if (string.IsNullOrEmpty(connectionString))
            {
                _logger.LogError("Connection string 'db_MS' not found");
                return;
            }

            lock (reader.TagDetected.SyncRoot)
            {
                foreach (DictionaryEntry tagItem in reader.TagDetected)
                {
                    var tagId = tagItem.Key?.ToString() ?? "";
                    _logger.LogInformation("StartUpdateDB >> pTRAN_TYPE: {TranType}; pRFID: {Rfid}; pReader: {Reader}; pIPADDR: {Ip}; SP: {Sp}",
                        tranType, tagId, reader.HostName, reader.IpAddress, reader.StoredProcedure);

                    ExecuteRfidStoredProcedure(connectionString, reader.StoredProcedure, tranType, tagId, reader.IpAddress);
                }
            }

            reader.TagDetected.Clear();
            reader.TagTime.Clear();
        }

        /// <summary>
        /// Executes the RFID transaction stored procedure using the Tower connection string.
        /// Mirrors WinForm StartUpdateDBTower.
        /// </summary>
        private void StartUpdateDBTower(RfidReaderItem reader, string tranType)
        {
            var connectionString = _configuration.GetConnectionString("db_MS_Tower");
            if (string.IsNullOrEmpty(connectionString))
            {
                _logger.LogError("Connection string 'db_MS_Tower' not found");
                return;
            }

            lock (reader.TagDetected.SyncRoot)
            {
                foreach (DictionaryEntry tagItem in reader.TagDetected)
                {
                    var tagId = tagItem.Key?.ToString() ?? "";
                    _logger.LogInformation("StartUpdateDBTower >> pTRAN_TYPE: {TranType}; pRFID: {Rfid}; pReader: {Reader}; pIPADDR: {Ip}; SP: {Sp}",
                        tranType, tagId, reader.HostName, reader.IpAddress, reader.StoredProcedure);

                    ExecuteRfidStoredProcedure(connectionString, reader.StoredProcedure, tranType, tagId, reader.IpAddress);
                }
            }

            reader.TagDetected.Clear();
            reader.TagTime.Clear();
        }

        /// <summary>
        /// Executes the RFID stored procedure — equivalent to RFID_Common_MSSQL from Library.Database.
        /// </summary>
        private void ExecuteRfidStoredProcedure(string connectionString, string storedProcedure, string tranType, string rfid, string ipAddress)
        {
            try
            {
                using var conn = new SqlConnection(connectionString);
                using var cmd = new SqlCommand(storedProcedure, conn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 0
                };

                cmd.Parameters.Add(new SqlParameter("@pTRAN_TYPE", tranType));
                cmd.Parameters.Add(new SqlParameter("@pRFID", rfid));
                cmd.Parameters.Add(new SqlParameter("@pIPADDR", ipAddress));
                cmd.Parameters.Add(new SqlParameter("@pRETURN_VALUE1", SqlDbType.NVarChar, 4000)
                {
                    Direction = ParameterDirection.Output
                });

                conn.Open();
                cmd.ExecuteNonQuery();

                var returnValue = cmd.Parameters["@pRETURN_VALUE1"].Value?.ToString() ?? "";
                if (returnValue != "0" && !string.IsNullOrEmpty(returnValue))
                {
                    _logger.LogWarning("SP {Sp} returned: {ReturnValue} for RFID {Rfid}", storedProcedure, returnValue, rfid);
                }

                // Broadcast DB result to web clients
                _ = _hubContext.Clients.All.SendAsync("DbTransactionResult", new
                {
                    IpAddress = ipAddress,
                    TranType = tranType,
                    Rfid = rfid,
                    Success = string.IsNullOrEmpty(returnValue) || returnValue == "0",
                    Message = returnValue,
                    Time = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing SP {Sp} for RFID {Rfid}", storedProcedure, rfid);
            }
        }

        #endregion

        #region Reconnection Timer

        /// <summary>
        /// Periodically checks for disconnected readers and attempts to reconnect.
        /// Mirrors WinForm TimerReconnect_Tick (5-second interval).
        /// </summary>
        private void ReconnectTimerCallback(object? state)
        {
            foreach (var kvp in _readerList)
            {
                var reader = kvp.Value;
                if (!reader.ReconnectRequired) continue;

                _logger.LogInformation("Attempting reconnect for reader {Name} ({Ip})", reader.Name, reader.IpAddress);
                _ = ConnectReaderAsync(reader.IpAddress);
            }
        }

        #endregion

        #region Tag Cleanup Timer

        /// <summary>
        /// Periodically removes tags that are older than 2 minutes from the hashtables.
        /// Mirrors WinForm Reset_Table_Tick (10-second interval).
        /// </summary>
        private void ResetTableTimerCallback(object? state)
        {
            foreach (var kvp in _readerList)
            {
                var reader = kvp.Value;
                var toRemove = new List<string>();

                lock (reader.TagTime.SyncRoot)
                {
                    foreach (string key in reader.TagTime.Keys)
                    {
                        if (reader.TagTime[key] is DateTime startTime && startTime.AddMinutes(2) < DateTime.UtcNow)
                        {
                            toRemove.Add(key);
                        }
                    }
                }

                foreach (var key in toRemove)
                {
                    lock (reader.TagTime.SyncRoot) { reader.TagTime.Remove(key); }
                    lock (reader.TagDetected.SyncRoot) { reader.TagDetected.Remove(key); }
                    _logger.LogDebug("Tag {Tag} removed from {Reader} (expired)", key, reader.Name);
                }
            }
        }

        #endregion

        #region Email Notifications

        private void AddFailEmail(RfidReaderItem reader, string message)
        {
            _failEmailList.Add(new ReaderEmailInfo(reader.HostName, reader.IpAddress, reader.Name, DateTime.Now, message));
            EnsureEmailTimer(ref _failEmailTimer, SendFailEmail);
        }

        private void EnsureEmailTimer(ref Timer? timer, TimerCallback callback)
        {
            if (timer == null)
            {
                timer = new Timer(callback, null, TimeSpan.FromSeconds(30), Timeout.InfiniteTimeSpan);
            }
        }

        private void SendDCEmail(object? state)
        {
            SendEmailBatch(_dcEmailList, "Disconnection");
            _dcEmailTimer?.Dispose();
            _dcEmailTimer = null;
        }

        private void SendRCEmail(object? state)
        {
            SendEmailBatch(_rcEmailList, "Reconnection");
            _rcEmailTimer?.Dispose();
            _rcEmailTimer = null;
        }

        private void SendFailEmail(object? state)
        {
            SendEmailBatch(_failEmailList, "Connection Failure");
            _failEmailTimer?.Dispose();
            _failEmailTimer = null;
        }

        /// <summary>
        /// Sends notification emails via the SEND_HTML_EMAIL2 stored procedure.
        /// Mirrors WinForm Mailer.SendDBEmailMSSQL.
        /// </summary>
        private void SendEmailBatch(ConcurrentBag<ReaderEmailInfo> emailList, string eventType)
        {
            if (emailList.IsEmpty) return;

            var mailTo = _configuration["RfidEmail:MailTo"] ?? "";
            if (string.IsNullOrEmpty(mailTo))
            {
                _logger.LogWarning("RfidEmail:MailTo not configured — skipping email");
                return;
            }

            var connectionString = _configuration.GetConnectionString("db_MS");
            if (string.IsNullOrEmpty(connectionString))
            {
                _logger.LogError("Connection string 'db_MS' not found for email");
                return;
            }

            try
            {
                // Build HTML email body
                var items = new List<ReaderEmailInfo>();
                while (emailList.TryTake(out var item))
                    items.Add(item);

                if (items.Count == 0) return;

                var body = $"<h3>RFID Reader {eventType} Notification</h3>" +
                           "<table border='1' cellpadding='4' cellspacing='0'>" +
                           "<tr><th>Reader</th><th>IP Address</th><th>Location</th><th>Time</th><th>Message</th></tr>";

                foreach (var item in items)
                {
                    body += $"<tr><td>{item.HostName}</td><td>{item.IpAddress}</td><td>{item.Name}</td>" +
                            $"<td>{item.Time:yyyy-MM-dd HH:mm:ss}</td><td>{item.Message}</td></tr>";
                }

                body += "</table>";

                var subject = $"Film MSP - RFID Reader {eventType} Notification";

                using var conn = new SqlConnection(connectionString);
                using var cmd = new SqlCommand("SEND_HTML_EMAIL2", conn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 0
                };

                cmd.Parameters.Add(new SqlParameter("@subject", subject));
                cmd.Parameters.Add(new SqlParameter("@MSG", body));
                cmd.Parameters.Add(new SqlParameter("@emailto", mailTo));

                conn.Open();
                cmd.ExecuteNonQuery();

                _logger.LogInformation("Sent {EventType} email to {MailTo} with {Count} reader(s)", eventType, mailTo, items.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send {EventType} email", eventType);
            }
        }

        #endregion

        #region SignalR Broadcasting

        private Task BroadcastReaderStatus(RfidReaderItem reader)
        {
            return _hubContext.Clients.All.SendAsync("ReaderStatusChanged", new ReaderStatusDto
            {
                IpAddress = reader.IpAddress,
                HostName = reader.HostName,
                Name = reader.Name,
                Status = reader.Status.ToString(),
                ReconnectRequired = reader.ReconnectRequired
            });
        }

        #endregion
    }

    #region Supporting Types

    /// <summary>
    /// Configuration model bound from appsettings.json "RfidReaders" section.
    /// </summary>
    public class RfidReaderConfig
    {
        public string IpAddress { get; set; } = "";
        public string HostName { get; set; } = "";
        public string Name { get; set; } = "";
        public string? Port { get; set; }
        public string? StoredProcedure { get; set; }
        public string? Server { get; set; }
        public string? StoredProcedure2 { get; set; }
        public string? Server2 { get; set; }
    }

    /// <summary>
    /// Represents the state of a single RFID reader managed by the service.
    /// Equivalent to RFID_Reader_Item in the WinForm project.
    /// </summary>
    public class RfidReaderItem
    {
        public int Index { get; set; }
        public string Name { get; set; } = "";
        public string IpAddress { get; set; } = "";
        public string HostName { get; set; } = "";
        public string Port { get; set; } = "5084";
        public string StoredProcedure { get; set; } = "";
        public string Server { get; set; } = "MSSQL";
        public string? StoredProcedure2 { get; set; }
        public string? Server2 { get; set; }
        public RFIDReader? ReaderApi { get; set; }
        public bool ReconnectRequired { get; set; }
        public ReaderConnectionStatus Status { get; set; } = ReaderConnectionStatus.Disconnected;
        public Hashtable TagDetected { get; set; } = new();
        public Hashtable TagTime { get; set; } = new();
    }

    public enum ReaderConnectionStatus
    {
        Disconnected,
        Connecting,
        Connected,
        Error
    }

    /// <summary>
    /// DTO sent to SignalR clients representing reader status.
    /// </summary>
    public class ReaderStatusDto
    {
        public string IpAddress { get; set; } = "";
        public string HostName { get; set; } = "";
        public string Name { get; set; } = "";
        public string Status { get; set; } = "";
        public bool ReconnectRequired { get; set; }
    }

    /// <summary>
    /// Data object for reader email notifications.
    /// Mirrors WinForm's ReaderEmailObject.
    /// </summary>
    public record ReaderEmailInfo(string HostName, string IpAddress, string Name, DateTime Time, string Message);

    #endregion
}
