using FILM_Sparepart_MVC.Hubs;
using FILM_Sparepart_MVC.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Symbol.RFID3;
using System.Collections.Concurrent;
using static Symbol.RFID3.Events;

namespace FILM_Sparepart_MVC.Services
{
    public class RFIDService : BackgroundService
    {
        // ── Per-reader runtime state ──────────────────────────────────────────
        private class ReaderState
        {
            public RFIDReader Reader { get; set; }
            public ReaderRFIDModel Model { get; set; }
            public bool ReconnectRequired { get; set; }
            public bool ManuallyDisconnected { get; set; }  

            // Tag dedup (replaces Hashtable)
            public ConcurrentDictionary<string, bool> TagDetected { get; } = new();
            public ConcurrentDictionary<string, DateTime> TagTime { get; } = new();

            // Loop-coil debounce state
            public string LoopCoil1 { get; set; } = string.Empty;
            public string LoopCoil2 { get; set; } = string.Empty;
            public string Tower { get; set; } = string.Empty;
            public System.Threading.Timer DebounceTimer { get; set; }
            public bool DebounceActive { get; set; }
            public readonly object DebounceLock = new();
        }

        // ── Service-level state ───────────────────────────────────────────────
        private readonly ConcurrentDictionary<string, ReaderState> _readers = new(StringComparer.OrdinalIgnoreCase);

        // Tag email lock (Tower Zone only)
        private string _lockTag = string.Empty;
        private readonly object _lockTagLock = new();
        private System.Threading.Timer _tagLockTimer;

        // Pending email retry queues
        private readonly List<ReaderEmailInfo> _pendingDCEmails = new();
        private readonly List<ReaderEmailInfo> _pendingRCEmails = new();
        private readonly List<ReaderEmailInfo> _pendingFailEmails = new();
        private readonly List<TagEmailInfo> _pendingTagEmails = new();
        private readonly object _emailLock = new();

        private readonly IHubContext<RfidHub> _hubContext;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RFIDService> _logger;

        private string ConnStrMain => _configuration.GetConnectionString("db_MS");
        private string ConnStrTower => _configuration.GetConnectionString("db_MS_Tower");
        private string MailTo => _configuration["RfidEmail:MailTo"] ?? string.Empty;

        public RFIDService(
            IConfiguration configuration,
            IHubContext<RfidHub> hubContext,
            ILogger<RFIDService> logger)
        {
            _configuration = configuration;
            _hubContext = hubContext;
            _logger = logger;
        }

        // ── BackgroundService entry point ─────────────────────────────────────
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            // ── Architecture diagnostic ───────────────────────────────────────────
            _logger.LogInformation("[RFIDService] Process architecture: {Arch} | OS: {OS} | PID: {PID}",
                Environment.Is64BitProcess ? "x64 ✗" : "x86 ✓",
                Environment.Is64BitOperatingSystem ? "64-bit OS" : "32-bit OS",
                Environment.ProcessId);
            _logger.LogInformation("[RFIDService] Executable: {Exe}", Environment.ProcessPath);
            // ─────────────────────────────────────────────────────────────────────

            _logger.LogInformation("[RFIDService] Starting...");

            // 1. Load config & connect all readers (only if enabled)
            bool autoConnect = _configuration.GetValue<bool>("RFIDService:AutoConnectOnStartup", true);
            if (autoConnect)
            {
                await InitReadersAsync();
            }
            else
            {
                _logger.LogInformation("[RFIDService] Auto-connect on startup is disabled.");
            }

            // 2. Periodic: purge stale tags every 30 s
            var staleTagTimer = new PeriodicTimer(TimeSpan.FromSeconds(30));

            // 3. Periodic: reconnect dropped readers + retry emails every 30 s
            var reconnectTimer = new PeriodicTimer(TimeSpan.FromSeconds(30));

            var staleTask = RunPeriodicAsync(staleTagTimer, PurgeStaleTagsAsync, stoppingToken);
            var reconnectTask = RunPeriodicAsync(reconnectTimer, ReconnectAndRetryEmailsAsync, stoppingToken);

            await Task.WhenAll(staleTask, reconnectTask);

            // Cleanup on shutdown
            _logger.LogInformation("[RFIDService] Stopping — disconnecting all readers...");
            foreach (var kvp in _readers)
            {
                SafeDisconnect(kvp.Value);
            }
        }

        private static async Task RunPeriodicAsync(PeriodicTimer timer, Func<Task> action, CancellationToken ct)
        {
            try
            {
                while (await timer.WaitForNextTickAsync(ct))
                {
                    await action();
                }
            }
            catch (OperationCanceledException) { }
        }

        // ── Initialise: load config & connect ────────────────────────────────
        private async Task InitReadersAsync()
        {
            try
            {
                var configs = LoadReaderConfigs();
                _logger.LogInformation("[RFIDService] Found {Count} reader(s) in config.", configs.Count);

                var tasks = configs.Select(cfg => Task.Run(() => ConnectReader(cfg))).ToList();
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[RFIDService] InitReadersAsync failed.");
            }
        }

        private List<ReaderRFIDModel> LoadReaderConfigs()
        {
            var list = new List<ReaderRFIDModel>();

            using var con = new SqlConnection(ConnStrMain);
            using var cmd = new SqlCommand("SP_FILM_GET_RFID_CONFIG", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@pCOMPANY", "FILM");
            con.Open();

            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                list.Add(new ReaderRFIDModel
                {
                    ID       = dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"]),
                    IP_ADDRESS = dr["IP_ADDRESS"].ToString(),
                    HOST_NAME  = dr["HOST_NAME"].ToString(),
                    PORT       = dr["PORT"]?.ToString() ?? "5084",
                    SP         = dr["SP"]?.ToString() ?? string.Empty,
                    SERVER     = dr["SERVER"]?.ToString() ?? string.Empty,
                    SP2        = dr["SP2"]?.ToString() ?? string.Empty,
                    SERVER2    = dr["SERVER2"]?.ToString() ?? string.Empty,
                    COMPANY    = dr["COMPANY"]?.ToString() ?? string.Empty,
                    LOCATION   = dr["LOCATION"]?.ToString() ?? string.Empty,
                    RSSI       = dr["RSSI"] == DBNull.Value ? -40 : Convert.ToInt32(dr["RSSI"])
                });
            }

            return list;
        }

        // ── Connect a single reader ───────────────────────────────────────────
        // ── Connect a single reader ───────────────────────────────────────────
        private void ConnectReader(ReaderRFIDModel model)
        {
            string key = model.IP_ADDRESS;

            try
            {
                _logger.LogInformation("[{Key}] Connecting — ID={ID} PORT={Port} RSSI={RSSI}",
                    key, model.ID, model.PORT, model.RSSI);
                BroadcastReaderStatus(key, "Connecting", "Connecting...");

                // Pre-check: is the port reachable at all?
                bool reachable = IsPortReachableAsync(model.IP_ADDRESS, model.PORT).GetAwaiter().GetResult();
                if (!reachable)
                {
                    _logger.LogWarning("[{Key}] Port unreachable — reader offline or wrong network.", key);
                    BroadcastReaderStatus(key, "Disconnected", "Connect failed: Reader unreachable (check network/IP).");
                    QueueFailEmail(model, "Connect failed: Reader unreachable.");
                    return;
                }


                // In ConnectReader:
                var rfid = ConnectWithTimeoutAsync(model, TimeSpan.FromSeconds(60)).GetAwaiter().GetResult();

                if (rfid == null)
                {
                    _logger.LogWarning("[{Key}] Connect timed out — reader may be occupied by another system.", key);
                    BroadcastReaderStatus(key, "Disconnected", "Connect failed: Reader busy or occupied by another system.");
                    QueueFailEmail(model, "Connect failed: Reader busy or occupied by another system.");
                    return;
                }

                var state = new ReaderState { Reader = rfid, Model = model };
                _readers[key] = state;

                _logger.LogInformation("[{Key}] Connected.", key);
                BroadcastReaderStatus(key, "Connected", "Connected successfully.");
            }
            catch (OperationFailureException ex)
            {
                _logger.LogWarning("[{Key}] Connect failed: {Msg}", key, ex.StatusDescription);
                BroadcastReaderStatus(key, "Disconnected", $"Connect failed: {ex.StatusDescription}");
                QueueFailEmail(model, $"Connect failed: {ex.StatusDescription}");
            }
            catch (Exception ex)
            {
                _logger.LogWarning("[{Key}] Connect failed: {Msg}", key, ex.Message);
                BroadcastReaderStatus(key, "Disconnected", $"Connect failed: {ex.Message}");
                QueueFailEmail(model, $"Connect failed: {ex.Message}");
            }
        }

        // ── RFID Events ───────────────────────────────────────────────────────
        private void Events_ReadNotify(object sender, ReadEventArgs e)
        {
            try
            {
                if (sender is not RFIDReader rfid) return;
                var state = FindState(rfid);
                if (state == null) return;

                ProcessReadTags(state);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[RFIDService] Events_ReadNotify error.");
            }
        }

        private void Events_StatusNotify(object sender, StatusEventArgs e)
        {
            try
            {
                if (sender is not RFIDReader rfid) return;
                var state = FindState(rfid);
                if (state == null) return;

                string key = state.Model.IP_ADDRESS;

                switch (e.StatusEventData.StatusEventType)
                {
                    case STATUS_EVENT_TYPE.GPI_EVENT:
                        HandleGpiEvent(state,
                            e.StatusEventData.GPIEventData.PortNumber,
                            e.StatusEventData.GPIEventData.GPIEvent);
                        break;

                    case STATUS_EVENT_TYPE.DISCONNECTION_EVENT:
                        string msg = $"Disconnection: {e.StatusEventData.DisconnectionEventData.DisconnectEventInfo}";
                        _logger.LogWarning("[{Key}] {Msg}", key, msg);
                        BroadcastReaderStatus(key, "Disconnected", msg);
                        state.ReconnectRequired = true;
                        QueueDcEmail(state.Model, msg);
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[RFIDService] Events_StatusNotify error.");
            }
        }

        // ── Tag processing ────────────────────────────────────────────────────
        private void ProcessReadTags(ReaderState state)
        {
            TagData[] tags = state.Reader.Actions.GetReadTags(50);
            if (tags == null || tags.Length == 0) return;

            string ipAddress = state.Model.IP_ADDRESS;
            string location = ResolveLocation(ipAddress);

            foreach (var tag in tags)
            {
                if (tag.OpCode != ACCESS_OPERATION_CODE.ACCESS_OPERATION_NONE &&
                    !(tag.OpCode == ACCESS_OPERATION_CODE.ACCESS_OPERATION_READ &&
                      tag.OpStatus == ACCESS_OPERATION_STATUS.ACCESS_SUCCESS))
                {
                    continue;
                }

                string tagId = tag.TagID;
                string antenna = tag.AntennaID.ToString();

                bool isNew = state.TagDetected.TryAdd(tagId, false);
                if (isNew)
                {
                    state.TagTime[tagId] = DateTime.UtcNow;
                }

                // Broadcast tag read to all dashboard clients
                BroadcastTagRead(state.Model.HOST_NAME, tagId, location, antenna);

                // Tower Zone: tag email lock logic (only tags starting with E or B)
                if (ipAddress == "10.28.92.50" && isNew)
                {
                    char first = tagId.Length > 0 ? tagId[0] : '\0';
                    if (first == 'E' || first == 'B')
                    {
                        bool shouldEmail = false;
                        lock (_lockTagLock)
                        {
                            if (_lockTag == string.Empty || tagId != _lockTag)
                            {
                                _lockTag = tagId;
                                shouldEmail = true;
                                // Clear lock after interval
                                _tagLockTimer?.Dispose();
                                _tagLockTimer = new System.Threading.Timer(_ =>
                                {
                                    lock (_lockTagLock) { _lockTag = string.Empty; }
                                }, null, TimeSpan.FromSeconds(30), Timeout.InfiniteTimeSpan);
                            }
                        }

                        if (shouldEmail)
                        {
                            QueueTagEmail(state.Model, location, antenna, tagId);
                        }
                    }
                }
            }
        }

        // ── GPI / Loop-coil direction logic ──────────────────────────────────
        private void HandleGpiEvent(ReaderState state, int portNumber, bool portState)
        {
            string ip = state.Model.IP_ADDRESS;
            _logger.LogDebug("[{IP}] GPI Port={Port} State={State}", ip, portNumber, portState);

            if (ip == "10.28.92.50")
            {
                // Tower Zone: GPI 1 = tower "1" (→ OUT), GPI 2 = tower "2" (→ IN)
                HandleTowerGpi(state, portNumber);
            }
            else
            {
                // Loop coil readers
                HandleLoopCoilGpi(state, portNumber);
            }
        }

        private void HandleLoopCoilGpi(ReaderState state, int portNumber)
        {
            lock (state.DebounceLock)
            {
                if (!state.DebounceActive)
                {
                    // First GPI: set loopCoil1 and start debounce timer
                    int gpi1Port = (int)state.Reader.Config.GPI[1].PortState;
                    int gpi2Port = (int)state.Reader.Config.GPI[2].PortState;

                    state.LoopCoil1 = gpi1Port == 0 ? "In" : (gpi2Port == 0 ? "Out" : string.Empty);
                    state.DebounceActive = true;

                    state.DebounceTimer?.Dispose();
                    state.DebounceTimer = new System.Threading.Timer(_ =>
                    {
                        Check2ndLoopAndUpdate(state);
                    }, null, TimeSpan.FromMilliseconds(500), Timeout.InfiniteTimeSpan);
                }
                else
                {
                    // Second GPI: check2ndloop
                    int gpi1Port = (int)state.Reader.Config.GPI[1].PortState;
                    int gpi2Port = (int)state.Reader.Config.GPI[2].PortState;
                    state.LoopCoil2 = gpi2Port == 0 ? "In" : (gpi1Port == 0 ? "Out" : string.Empty);
                }
            }
        }

        private void HandleTowerGpi(ReaderState state, int portNumber)
        {
            lock (state.DebounceLock)
            {
                if (!state.DebounceActive)
                {
                    // First GPI: set initial tower value ("1" or "2"), start debounce
                    state.Tower = portNumber == 1 ? "1" : portNumber == 2 ? "2" : string.Empty;
                    state.DebounceActive = true;

                    state.DebounceTimer?.Dispose();
                    state.DebounceTimer = new System.Threading.Timer(_ =>
                    {
                        Check2ndLoopAndUpdate(state);
                    }, null, TimeSpan.FromMilliseconds(500), Timeout.InfiniteTimeSpan);
                }
                else
                {
                    // Second GPI: convert "1"→"Out", "2"→"In" (same port confirms direction)
                    if (state.Tower == "1") state.Tower = "Out";
                    else if (state.Tower == "2") state.Tower = "In";
                }
            }
        }

        private void Check2ndLoopAndUpdate(ReaderState state)
        {
            string direction = string.Empty;
            bool isTower = state.Model.IP_ADDRESS == "10.28.92.50";

            lock (state.DebounceLock)
            {
                if (isTower)
                {
                    // Only update if second GPI confirmed the direction
                    direction = state.Tower == "In" ? "IN" : state.Tower == "Out" ? "OUT" : string.Empty;
                    state.Tower = string.Empty;
                }
                else
                {
                    if (state.LoopCoil1 == state.LoopCoil2 && !string.IsNullOrEmpty(state.LoopCoil1))
                    {
                        direction = state.LoopCoil1 == "In" ? "IN" : "OUT";
                    }

                    state.LoopCoil1 = string.Empty;
                    state.LoopCoil2 = string.Empty;
                }

                state.DebounceActive = false;
            }

            if (string.IsNullOrEmpty(direction))
            {
                state.TagDetected.Clear();
                state.TagTime.Clear();
                return;
            }

            if (isTower)
            {
                StartUpdateDbTower(state, direction);
            }
            else
            {
                StartUpdateDb(state, direction);
            }
        }

        // ── DB updates ────────────────────────────────────────────────────────
        private void StartUpdateDb(ReaderState state, string tranType)
        {
            foreach (var kv in state.TagDetected)
            {
                string tagId = kv.Key;
                _logger.LogInformation("[{IP}] SP={SP} TRAN={Tran} TAG={Tag}",
                    state.Model.IP_ADDRESS, state.Model.SP, tranType, tagId);

                var dto = ExecuteSp(ConnStrMain, tranType, tagId,
                    state.Model.HOST_NAME, state.Model.IP_ADDRESS, state.Model.SP);

                bool success = !dto.Error;
                BroadcastTransaction(tagId, tranType, success, dto.ErrorMessage);
                TowerLightOn(state, success ? 2 : 1); // 2=Green, 1=Red
            }

            state.TagDetected.Clear();
            state.TagTime.Clear();
        }

        private void StartUpdateDbTower(ReaderState state, string tranType)
        {
            foreach (var kv in state.TagDetected)
            {
                string tagId = kv.Key;
                _logger.LogInformation("[Tower] SP={SP} TRAN={Tran} TAG={Tag}",
                    state.Model.SP, tranType, tagId);

                var dto = ExecuteSpTower(ConnStrTower, tranType, tagId,
                    state.Model.HOST_NAME, state.Model.IP_ADDRESS, state.Model.SP);

                bool success = !dto.Error;
                BroadcastTransaction(tagId, tranType, success, dto.ErrorMessage);
                TowerLightOn(state, success ? 2 : 1);
            }

            state.TagDetected.Clear();
            state.TagTime.Clear();
        }

        private (bool Error, string ErrorMessage) ExecuteSp(
            string connStr, string tranType, string rfid,
            string readerName, string ipAddress, string sp)
        {
            try
            {
                using var con = new SqlConnection(connStr);
                using var cmd = new SqlCommand(sp, con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.AddWithValue("@pTRAN_TYPE", tranType);
                cmd.Parameters.AddWithValue("@pRFID", rfid);
                cmd.Parameters.AddWithValue("@pIPADDR", ipAddress);
                var ret = new SqlParameter("@pRETURN_VALUE1", System.Data.SqlDbType.NVarChar, 4000)
                { Direction = System.Data.ParameterDirection.Output };
                cmd.Parameters.Add(ret);

                con.Open();
                cmd.ExecuteNonQuery();

                string retVal = ret.Value?.ToString() ?? string.Empty;
                if (!string.IsNullOrEmpty(retVal) && retVal != "0" && retVal != "Success")
                    throw new Exception(retVal);

                return (false, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SP] {SP} failed for tag {Tag}", sp, rfid);
                return (true, ex.Message);
            }
        }

        private (bool Error, string ErrorMessage) ExecuteSpTower(
            string connStr, string tranType, string rfid,
            string readerName, string ipAddress, string sp)
        {
            // Tower SP does not pass @pIPADDR per original code
            try
            {
                using var con = new SqlConnection(connStr);
                using var cmd = new SqlCommand(sp, con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.AddWithValue("@pTRAN_TYPE", tranType);
                cmd.Parameters.AddWithValue("@pRFID", rfid);
                var ret = new SqlParameter("@pRETURN_VALUE1", System.Data.SqlDbType.NVarChar, 4000)
                { Direction = System.Data.ParameterDirection.Output };
                cmd.Parameters.Add(ret);

                con.Open();
                cmd.ExecuteNonQuery();

                string retVal = ret.Value?.ToString() ?? string.Empty;
                if (!string.IsNullOrEmpty(retVal) && retVal != "0" && retVal != "Success")
                    throw new Exception(retVal);

                return (false, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Tower SP] {SP} failed for tag {Tag}", sp, rfid);
                return (true, ex.Message);
            }
        }

        // ── Tower light control ───────────────────────────────────────────────
        // GPO 1 = Red (error), GPO 2 = Green (success) — ON for 4.5 s then reset
        private void TowerLightOn(ReaderState state, int mode)
        {
            try
            {
                string color = mode == 2 ? "green" : "red";
                BroadcastTowerLight(state.Model.HOST_NAME, color);

                if (mode == 2)
                {
                    state.Reader.Config.GPO[1].PortState = GPOs.GPO_PORT_STATE.FALSE;
                    state.Reader.Config.GPO[2].PortState = GPOs.GPO_PORT_STATE.TRUE;
                }
                else
                {
                    state.Reader.Config.GPO[1].PortState = GPOs.GPO_PORT_STATE.TRUE;
                    state.Reader.Config.GPO[2].PortState = GPOs.GPO_PORT_STATE.FALSE;
                }

                state.Reader.Config.GPO[3].PortState = GPOs.GPO_PORT_STATE.FALSE;

                // Auto-reset after 4.5 s
                Task.Delay(4500).ContinueWith(_ =>
                {
                    try
                    {
                        state.Reader.Config.GPO[1].PortState = GPOs.GPO_PORT_STATE.FALSE;
                        state.Reader.Config.GPO[2].PortState = GPOs.GPO_PORT_STATE.FALSE;
                        state.Reader.Config.GPO[3].PortState = GPOs.GPO_PORT_STATE.FALSE;
                        BroadcastTowerLight(state.Model.HOST_NAME, "off");
                    }
                    catch { }
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[{IP}] TowerLightOn failed.", state.Model.IP_ADDRESS);
            }
        }

        // ── Periodic: purge stale tags (> 2 min) ─────────────────────────────
        private Task PurgeStaleTagsAsync()
        {
            foreach (var kvp in _readers)
            {
                var state = kvp.Value;
                var toRemove = state.TagTime
                    .Where(t => t.Value.AddMinutes(2) < DateTime.UtcNow)
                    .Select(t => t.Key)
                    .ToList();

                foreach (var key in toRemove)
                {
                    state.TagTime.TryRemove(key, out _);
                    state.TagDetected.TryRemove(key, out _);
                }
            }

            return Task.CompletedTask;
        }

        // ── Periodic: reconnect dropped readers + retry emails ────────────────
        private async Task ReconnectAndRetryEmailsAsync()
        {
            foreach (var kvp in _readers.ToList())
            {
                var state = kvp.Value;
                if (!state.ReconnectRequired || state.ManuallyDisconnected) continue;

                _logger.LogInformation("[{IP}] Attempting reconnect...", state.Model.IP_ADDRESS);
                BroadcastReaderStatus(state.Model.HOST_NAME, "Reconnecting", "Reconnecting...");

                try
                {
                    state.Reader.Reconnect();
                    state.Reader.Actions.Inventory.Perform();

                    state.ReconnectRequired = false;
                    BroadcastReaderStatus(state.Model.HOST_NAME, "Connected", "Reconnected successfully.");
                    _logger.LogInformation("[{IP}] Reconnect succeeded.", state.Model.IP_ADDRESS);
                    QueueRcEmail(state.Model, "Reconnected successfully.");
                }
                catch
                {
                    // Hard reconnect: recreate SDK object with timeout guard
                    try
                    {
                        // In ReconnectAndRetryEmailsAsync:
                        var newRfid = await ConnectWithTimeoutAsync(state.Model, TimeSpan.FromSeconds(60));

                        if (newRfid == null)
                        {
                            _logger.LogWarning("[{IP}] Hard reconnect timed out.", state.Model.IP_ADDRESS);
                            BroadcastReaderStatus(state.Model.HOST_NAME, "Disconnected", "Reconnect timed out.");
                            QueueFailEmail(state.Model, "Reconnect timed out.");
                        }
                        else
                        {
                            state.Reader = newRfid;
                            state.ReconnectRequired = false;
                            BroadcastReaderStatus(state.Model.HOST_NAME, "Connected", "Reconnected (hard) successfully.");
                            QueueRcEmail(state.Model, "Reconnected successfully.");
                        }
                    }
                    catch (Exception ex2)
                    {
                        _logger.LogWarning("[{IP}] Reconnect failed: {Msg}", state.Model.IP_ADDRESS, ex2.Message);
                        BroadcastReaderStatus(state.Model.HOST_NAME, "Disconnected", $"Reconnect failed: {ex2.Message}");
                        QueueFailEmail(state.Model, $"Reconnect failed: {ex2.Message}");
                    }
                }
            }

            await RetryPendingEmailsAsync();
        }

        // ── Connect with timeout guard (isolates blocking native SDK call) ────
        // ── Connect with timeout guard (isolates blocking native SDK call) ────
        // ── Connect with timeout guard (isolates blocking native SDK call) ────
        private async Task<RFIDReader?> ConnectWithTimeoutAsync(ReaderRFIDModel model, TimeSpan timeout)
        {
            const int maxAttempts = 3;
            const int retryDelaySeconds = 10;
            uint port = uint.TryParse(model.PORT, out var p) ? p : 5084u;
            string ip = model.IP_ADDRESS;

            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                _logger.LogInformation("[{IP}] Connect attempt {Attempt}/{Max}...", ip, attempt, maxAttempts);

                var tcs = new TaskCompletionSource<RFIDReader?>(TaskCreationOptions.RunContinuationsAsynchronously);
                RFIDReader? pendingRfid = null;

                var thread = new Thread(() =>
                {
                    try
                    {
                        var rfid = new RFIDReader(ip, port, 50000u);
                        pendingRfid = rfid;

                        rfid.Connect();
                        _logger.LogInformation("[{IP}] TCP connected — registering events...", ip);

                        rfid.Events.ReadNotify                  += Events_ReadNotify;
                        rfid.Events.StatusNotify                += Events_StatusNotify;
                        rfid.Events.NotifyGPIEvent               = true;
                        rfid.Events.NotifyReaderDisconnectEvent  = true;
                        rfid.Events.NotifyBufferFullEvent        = true;
                        rfid.Events.NotifyBufferFullWarningEvent = true;
                        rfid.Events.NotifyReaderExceptionEvent   = true;
                        rfid.Events.AttachTagDataWithReadEvent   = false;

                        var antennaList = new ushort[] { 1, 2 };
                        var antennaInfo = new AntennaInfo(antennaList);
                        rfid.Actions.Inventory.Perform(null, null, antennaInfo);
                        _logger.LogInformation("[{IP}] Inventory started successfully.", ip);

                        tcs.TrySetResult(rfid);
                    }
                    catch (Exception ex)
                    {
                        // ── Full diagnostic dump ──────────────────────────────────
                        Console.WriteLine($"[RFID FULL EX] IP        = {ip}");
                        Console.WriteLine($"[RFID FULL EX] Type      = {ex.GetType().FullName}");
                        Console.WriteLine($"[RFID FULL EX] Message   = {ex.Message}");
                        Console.WriteLine($"[RFID FULL EX] Process   = {(Environment.Is64BitProcess ? "64-bit" : "32-bit")}");
                        Console.WriteLine($"[RFID FULL EX] StackTrace=");
                        Console.WriteLine(ex.StackTrace);
                        if (ex.InnerException != null)
                        {
                            Console.WriteLine($"[RFID FULL EX] Inner.Type   = {ex.InnerException.GetType().FullName}");
                            Console.WriteLine($"[RFID FULL EX] Inner.Msg    = {ex.InnerException.Message}");
                            Console.WriteLine($"[RFID FULL EX] Inner.Stack  =");
                            Console.WriteLine(ex.InnerException.StackTrace);
                        }
                        Console.WriteLine($"[RFID FULL EX] ─────────────────────────────────────────");
                        // ─────────────────────────────────────────────────────────

                        _logger.LogWarning("[{IP}] Connect exception ({Type}): {Msg}", ip, ex.GetType().Name, ex.Message);
                        tcs.TrySetException(ex);
                    }
                });

                thread.SetApartmentState(ApartmentState.STA);
                thread.IsBackground = true;
                thread.Start();

                RFIDReader? result = await tcs.Task
                    .WaitAsync(TimeSpan.FromSeconds(70))
                    .ContinueWith(t =>
                    {
                        if (!t.IsCompletedSuccessfully)
                        {
                            _ = tcs.Task.ContinueWith(
                                inner => { _ = inner.Exception; },
                                TaskContinuationOptions.OnlyOnFaulted);

                            try { pendingRfid?.Actions.Inventory.Stop(); } catch { }
                            try { pendingRfid?.Disconnect(); } catch { }
                            try { pendingRfid?.Dispose(); } catch { }

                            _logger.LogWarning("[{IP}] Cleaned up after failure/timeout on attempt {Attempt}.", ip, attempt);
                            return (RFIDReader?)null;
                        }
                        return t.Result;
                    });

                if (result != null)
                    return result;

                if (attempt < maxAttempts)
                {
                    _logger.LogWarning("[{IP}] Attempt {Attempt}/{Max} failed — retrying in {Delay}s...",
                        ip, attempt, maxAttempts, retryDelaySeconds);
                    BroadcastReaderStatus(ip, "Connecting", $"Retrying... (attempt {attempt + 1}/{maxAttempts})");
                    await Task.Delay(TimeSpan.FromSeconds(retryDelaySeconds));
                }
            }

            _logger.LogError("[{IP}] All {Max} connect attempts failed.", ip, maxAttempts);
            return null;
        }

        private void TrySet(Action setter, string name, string ip)
        {
            try { setter(); }
            catch (Exception ex)
            {
                _logger.LogWarning("[{IP}] Could not set {Name}: {Msg} — continuing.", ip, name, ex.Message);
            }
        }

        private Task RetryPendingEmailsAsync()
        {
            lock (_emailLock)
            {
                foreach (var e in _pendingDCEmails.ToList())
                {
                    if (SendEmailViaDb(BuildReaderEmailBody(e), "Reader Disconnection Notification", e.MailTo))
                        _pendingDCEmails.Remove(e);
                }

                foreach (var e in _pendingRCEmails.ToList())
                {
                    if (SendEmailViaDb(BuildReaderEmailBody(e), "Reader Reconnection Notification", e.MailTo))
                        _pendingRCEmails.Remove(e);
                }

                foreach (var e in _pendingFailEmails.ToList())
                {
                    if (SendEmailViaDb(BuildReaderEmailBody(e), "Reader Connection Failure", e.MailTo))
                        _pendingFailEmails.Remove(e);
                }

                // Tag emails: batch of 33
                foreach (var batch in _pendingTagEmails.ToList()
                    .Chunk(33)
                    .Select(c => c.ToList()))
                {
                    if (SendEmailViaDb(BuildTagEmailBody(batch), "Tag Detection Notification", MailTo))
                    {
                        foreach (var item in batch)
                            _pendingTagEmails.Remove(item);
                    }
                }
            }

            return Task.CompletedTask;
        }

        // ── Email helpers ─────────────────────────────────────────────────────
        private void QueueDcEmail(ReaderRFIDModel model, string msg)
        {
            var info = new ReaderEmailInfo(model, msg, MailTo);
            if (!SendEmailViaDb(BuildReaderEmailBody(info), "Reader Disconnection Notification", MailTo))
            {
                lock (_emailLock) { _pendingDCEmails.Add(info); }
            }
        }

        private void QueueRcEmail(ReaderRFIDModel model, string msg)
        {
            var info = new ReaderEmailInfo(model, msg, MailTo);
            if (!SendEmailViaDb(BuildReaderEmailBody(info), "Reader Reconnection Notification", MailTo))
            {
                lock (_emailLock) { _pendingRCEmails.Add(info); }
            }
        }

        private void QueueFailEmail(ReaderRFIDModel model, string msg)
        {
            var info = new ReaderEmailInfo(model, msg, MailTo);
            if (!SendEmailViaDb(BuildReaderEmailBody(info), "Reader Connection Failure", MailTo))
            {
                lock (_emailLock) { _pendingFailEmails.Add(info); }
            }
        }

        private void QueueTagEmail(ReaderRFIDModel model, string location, string antenna, string tagId)
        {
            var batch = new List<TagEmailInfo>
            {
                new TagEmailInfo(model.HOST_NAME, location, antenna, tagId, DateTime.Now)
            };

            // Collect more tags before sending (send on next timer if batch not full)
            lock (_emailLock) { _pendingTagEmails.Add(batch[0]); }

            if (_pendingTagEmails.Count >= 33)
            {
                _ = RetryPendingEmailsAsync();
            }
        }

        private bool SendEmailViaDb(string body, string subject, string mailTo)
        {
            if (string.IsNullOrWhiteSpace(mailTo)) return true; // silently skip

            try
            {
                string fullBody = $@"Dear Sir/Madam,<br/><br/>
Please refer to the following SPARE PART RFID details.<br/><br/>
{body}<br/>Thank You.";

                using var con = new SqlConnection(ConnStrMain);
                using var cmd = new SqlCommand("SEND_HTML_EMAIL2", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.AddWithValue("@subject", subject);
                cmd.Parameters.AddWithValue("@MSG", fullBody);
                cmd.Parameters.AddWithValue("@emailto", mailTo);

                con.Open();
                cmd.ExecuteReader();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[Email] SendEmailViaDb failed.");
                return false;
            }
        }

        private static string BuildReaderEmailBody(ReaderEmailInfo e) =>
            $"<b>Reader:</b> {e.HostName} ({e.IpAddress})<br/>" +
            $"<b>Name:</b> {e.Name}<br/>" +
            $"<b>Time:</b> {e.EventTime:dd/MM/yyyy HH:mm:ss}<br/>" +
            $"<b>Message:</b> {e.Message}";

        private static string BuildTagEmailBody(List<TagEmailInfo> tags)
        {
            var rows = string.Join("", tags.Select(t =>
                $"<tr><td>{t.TagId}</td><td>{t.Location}</td><td>{t.AntennaId}</td><td>{t.TagTime:dd/MM/yyyy HH:mm:ss}</td></tr>"));

            return $@"<table border='1' cellpadding='4'>
<tr><th>Tag ID</th><th>Location</th><th>Antenna</th><th>Time</th></tr>
{rows}</table>";
        }

        // ── SignalR broadcast helpers ─────────────────────────────────────────
        private void BroadcastTagRead(string hostName, string tagId, string location, string antenna)
        {
            _ = _hubContext.Clients.All.SendAsync("OnTagRead", hostName, tagId, location, antenna);
        }

        private void BroadcastReaderStatus(string hostName, string status, string message)
        {
            _ = _hubContext.Clients.All.SendAsync("OnReaderStatus", hostName, status, message);
        }

        private void BroadcastTransaction(string tagId, string direction, bool success, string message)
        {
            _ = _hubContext.Clients.All.SendAsync("OnTransaction", tagId, direction, success, message);
        }

        private void BroadcastTowerLight(string hostName, string color)
        {
            _ = _hubContext.Clients.All.SendAsync("OnTowerLight", hostName, color);
        }

        // ── Utility ───────────────────────────────────────────────────────────
        private ReaderState FindState(RFIDReader rfid) =>
            _readers.Values.FirstOrDefault(s => s.Reader == rfid);

        private static string ResolveLocation(string ipAddress) => ipAddress switch
        {
            "10.28.92.50" => "Tower Zone",
            "10.28.92.51" => "Green Tent House 1",
            "10.28.92.52" => "Green Tent House 2",
            _ => ipAddress
        };

        private static void SafeDisconnect(ReaderState state)
        {
            try { state.Reader?.Actions.Inventory.Stop(); } catch { }
            try { state.Reader?.Disconnect(); } catch { }
            try { state.Reader?.Dispose(); } catch { }
            state.DebounceTimer?.Dispose();
        }

        // Public method kept for controller use (e.g. dropdown population)
        public List<ReaderRFIDModel> GetDefaultReaders() => LoadReaderConfigs();

        // ── Public reader control (called from Hub) ───────────────────────────
        public record ReaderStatusInfo(string HostName, string IpAddress, bool IsConnected);

        public List<ReaderStatusInfo> GetReaderStatuses() =>
            _readers.Values.Select(s => new ReaderStatusInfo(
                s.Model.HOST_NAME,
                s.Model.IP_ADDRESS,
                !s.ReconnectRequired && !s.ManuallyDisconnected
            )).ToList();

        public enum ConnectResultStatus { AlreadyConnected, Connected, Failed, ExternallyOccupied }

        public record ConnectReaderResult(string IpAddress, ConnectResultStatus Status, string Message);

        public async Task<ConnectReaderResult> ConnectReaderByIpAsync(string ip)
        {
            // Reader already active — connected by service auto-start or another session
            if (_readers.TryGetValue(ip, out var existing)
                && !existing.ReconnectRequired
                && !existing.ManuallyDisconnected)
            {
                string msg = $"Connection already existed — reader {existing.Model.HOST_NAME} " +
                             $"({ip}) is currently active. No action taken.";
                _logger.LogInformation("[{IP}] ConnectReader called but reader is already connected.", ip);
                return new ConnectReaderResult(ip, ConnectResultStatus.AlreadyConnected, msg);
            }

            return await Task.Run(() =>
            {
                try
                {
                    if (_readers.TryGetValue(ip, out var old))
                        SafeDisconnect(old);

                    var configs = LoadReaderConfigs();
                    var cfg = configs.FirstOrDefault(c => c.IP_ADDRESS == ip);
                    if (cfg == null)
                        return new ConnectReaderResult(ip, ConnectResultStatus.Failed,
                            $"No reader config found for IP {ip}.");

                    ConnectReader(cfg);

                    // Check if ConnectReader actually succeeded by verifying it's in _readers
                    if (!_readers.TryGetValue(ip, out var newState))
                        return new ConnectReaderResult(ip, ConnectResultStatus.Failed,
                            $"Reader {cfg.HOST_NAME} ({ip}) failed to connect.");

                    newState.ManuallyDisconnected = false;

                    return new ConnectReaderResult(ip, ConnectResultStatus.Connected,
                        $"Reader {cfg.HOST_NAME} ({ip}) connected successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[{IP}] ConnectReaderByIpAsync failed.", ip);
                    return new ConnectReaderResult(ip, ConnectResultStatus.Failed, ex.Message);
                }
            });
        }

        public Task ConnectAllReadersAsync()
        {
            return Task.Run(async () =>
            {
                var configs = LoadReaderConfigs();
                var tasks = configs.Select(cfg => Task.Run(() =>
                {
                    if (_readers.TryGetValue(cfg.IP_ADDRESS, out var old))
                        SafeDisconnect(old);

                    ConnectReader(cfg);

                    if (_readers.TryGetValue(cfg.IP_ADDRESS, out var newState))
                        newState.ManuallyDisconnected = false;
                })).ToList();

                await Task.WhenAll(tasks);
            });
        }

        public void DisconnectReaderByIp(string ip)
        {
            if (!_readers.TryGetValue(ip, out var state)) return;

            SafeDisconnect(state);
            state.ReconnectRequired = false;
            state.ManuallyDisconnected = true;
            BroadcastReaderStatus(state.Model.HOST_NAME, "Disconnected", "Manually disconnected.");
            _logger.LogInformation("[{IP}] Manually disconnected.", ip);
        }

        public void DisconnectAllReaders()
        {
            foreach (var kvp in _readers)
            {
                SafeDisconnect(kvp.Value);
                kvp.Value.ReconnectRequired = false;
                kvp.Value.ManuallyDisconnected = true;
                BroadcastReaderStatus(kvp.Value.Model.HOST_NAME, "Disconnected", "Manually disconnected.");
            }
            _logger.LogInformation("[RFIDService] All readers manually disconnected.");
        }

        private static async Task<bool> IsPortReachableAsync(string ip, string port, int timeoutMs = 3000)
        {
            try
            {
                int portNum = int.TryParse(port, out var p) ? p : 5084;
                using var client = new System.Net.Sockets.TcpClient();
                var ct = new CancellationTokenSource(timeoutMs);
                await client.ConnectAsync(ip, portNum, ct.Token);
                return true;
            }
            catch
            {
                return false;
            }
        }





        // ── Internal DTOs ─────────────────────────────────────────────────────
        private record ReaderEmailInfo(
            ReaderRFIDModel Model, string Message, string MailTo)
        {
            public string HostName => Model.HOST_NAME;
            public string IpAddress => Model.IP_ADDRESS;
            public string Name => Model.LOCATION;
            public DateTime EventTime { get; } = DateTime.Now;
        }

        private record TagEmailInfo(
            string HostName, string Location, string AntennaId,
            string TagId, DateTime TagTime);
    }

}