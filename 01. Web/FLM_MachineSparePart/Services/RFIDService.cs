using Microsoft.AspNet.SignalR;
using FILM_Sparepart_MVC.Hubs;
using FILM_Sparepart_MVC.Models;
using Symbol.RFID3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace FILM_Sparepart_MVC.Services
{
    public class RFIDReaderItem
    {
        public RFIDReader ReaderAPI { get; set; }
        public Hashtable TagDetected { get; set; }
        public Hashtable TagTime { get; set; }
        public bool ReconnectRequired { get; set; }
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
    }

    public class RFIDService
    {
        private static readonly Dictionary<string, RFIDReaderItem> _readerList = new Dictionary<string, RFIDReaderItem>();
        private static readonly object _lock = new object();
        private static bool _configLoaded = false;
        private IHubContext _hubContext;

        public RFIDService()
        {
            _hubContext = GlobalHost.ConnectionManager.GetHubContext<RfidHub>();
        }

        public List<RFIDReaderConfigModel> GetRFIDConfig()
        {
            var configList = new List<RFIDReaderConfigModel>();
            string connectionString = ConfigurationManager.ConnectionStrings["SQLCon"].ConnectionString;

            using (var con = new SqlConnection(connectionString))
            {
                var cmd = new SqlCommand("SP_FILM_GET_RFID_CONFIG", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@pCOMPANY", "FILM"));

                con.Open();
                var tbl = new DataTable();
                tbl.Load(cmd.ExecuteReader());

                foreach (DataRow row in tbl.Rows)
                {
                    var config = new RFIDReaderConfigModel
                    {
                        LOCATION = row["LOCATION"].ToString(),
                        IP_ADDRESS = row["IP_ADDRESS"].ToString(),
                        HOST_NAME = row["HOST_NAME"].ToString(),
                        PORT = row["PORT"].ToString(),
                        SP = row["SP"].ToString(),
                        SERVER = row["SERVER"].ToString(),
                        SP2 = row["SP2"].ToString(),
                        SERVER2 = row["SERVER2"].ToString()
                    };
                    configList.Add(config);
                }
            }

            lock (_lock)
            {
                if (!_configLoaded)
                {
                    int index = 0;
                    foreach (var config in configList)
                    {
                        if (!_readerList.ContainsKey(config.IP_ADDRESS))
                        {
                            var readerItem = new RFIDReaderItem
                            {
                                ReaderAPI = new RFIDReader(config.IP_ADDRESS, Convert.ToUInt32(config.PORT), 50000),
                                TagDetected = new Hashtable(),
                                TagTime = new Hashtable(),
                                ReconnectRequired = false,
                                Index = index,
                                Name = config.LOCATION,
                                IPAddress = config.IP_ADDRESS,
                                HostName = config.HOST_NAME,
                                Port = config.PORT,
                                StoredProcedure = config.SP,
                                Server = config.SERVER,
                                StoredProcedure2 = config.SP2,
                                Server2 = config.SERVER2,
                                IsConnected = false
                            };
                            _readerList.Add(config.IP_ADDRESS, readerItem);
                            index++;
                        }
                    }
                    _configLoaded = true;
                }
            }

            return configList;
        }

        public string ConnectReader(string ipAddress, string connectionId)
        {
            lock (_lock)
            {
                if (!_readerList.ContainsKey(ipAddress))
                    return "Reader not found in configuration.";

                var readerItem = _readerList[ipAddress];

                try
                {
                    if (readerItem.ReaderAPI.IsConnected)
                        return "Already connected.";

                    readerItem.ReaderAPI.Connect();
                    readerItem.ReaderAPI.Events.ReadNotify += Events_ReadNotify;
                    readerItem.ReaderAPI.Events.StatusNotify += Events_StatusNotify;

                    readerItem.ReaderAPI.Events.NotifyBufferFullWarningEvent = true;
                    readerItem.ReaderAPI.Events.NotifyBufferFullEvent = true;
                    readerItem.ReaderAPI.Events.NotifyInventoryStartEvent = true;
                    readerItem.ReaderAPI.Events.NotifyInventoryStopEvent = true;
                    readerItem.ReaderAPI.Events.NotifyAccessStartEvent = true;
                    readerItem.ReaderAPI.Events.NotifyAccessStopEvent = true;
                    readerItem.ReaderAPI.Events.NotifyAntennaEvent = true;
                    readerItem.ReaderAPI.Events.NotifyReaderDisconnectEvent = true;
                    readerItem.ReaderAPI.Events.NotifyReaderExceptionEvent = true;
                    readerItem.ReaderAPI.Events.NotifyGPIEvent = true;

                    readerItem.ReaderAPI.Actions.Inventory.Perform();
                    readerItem.IsConnected = true;

                    return "Connected";
                }
                catch (OperationFailureException ex)
                {
                    return "Connection failed: " + ex.StatusDescription;
                }
                catch (Exception ex)
                {
                    return "Connection failed: " + ex.Message;
                }
            }
        }

        public string DisconnectReader(string ipAddress)
        {
            lock (_lock)
            {
                if (!_readerList.ContainsKey(ipAddress))
                    return "Reader not found in configuration.";

                var readerItem = _readerList[ipAddress];

                try
                {
                    if (readerItem.ReaderAPI != null && readerItem.ReaderAPI.IsConnected)
                    {
                        readerItem.ReaderAPI.Actions.Inventory.Stop();
                        readerItem.ReaderAPI.Events.ReadNotify -= Events_ReadNotify;
                        readerItem.ReaderAPI.Events.StatusNotify -= Events_StatusNotify;
                        readerItem.ReaderAPI.Disconnect();
                    }
                    readerItem.IsConnected = false;
                    return "Disconnected";
                }
                catch (Exception ex)
                {
                    readerItem.IsConnected = false;
                    return "Disconnect error: " + ex.Message;
                }
            }
        }

        public string ConnectAll(string connectionId)
        {
            var results = new List<string>();
            List<string> keys;

            lock (_lock)
            {
                keys = _readerList.Keys.ToList();
            }

            foreach (var ip in keys)
            {
                var result = ConnectReader(ip, connectionId);
                results.Add(ip + ": " + result);
                BroadcastReaderStatus(ip, result, connectionId);
            }

            return string.Join("; ", results);
        }

        public string DisconnectAll(string connectionId)
        {
            var results = new List<string>();
            List<string> keys;

            lock (_lock)
            {
                keys = _readerList.Keys.ToList();
            }

            foreach (var ip in keys)
            {
                var result = DisconnectReader(ip);
                results.Add(ip + ": " + result);
                BroadcastReaderStatus(ip, "Disconnected", connectionId);
            }

            return string.Join("; ", results);
        }

        public List<object> GetReaderStatuses()
        {
            var statuses = new List<object>();
            lock (_lock)
            {
                foreach (var kvp in _readerList)
                {
                    var item = kvp.Value;
                    bool connected = false;
                    try
                    {
                        connected = item.ReaderAPI != null && item.ReaderAPI.IsConnected;
                    }
                    catch
                    {
                        connected = false;
                    }
                    item.IsConnected = connected;

                    statuses.Add(new
                    {
                        ipAddress = item.IPAddress,
                        location = item.Name,
                        hostName = item.HostName,
                        status = connected ? "Connected" : "Disconnected",
                        isConnected = connected
                    });
                }
            }
            return statuses;
        }

        private void BroadcastReaderStatus(string ipAddress, string status, string connectionId)
        {
            try
            {
                _hubContext.Clients.Client(connectionId).updateReaderStatus(ipAddress, status);
            }
            catch { }
        }

        private void Events_ReadNotify(object sender, Events.ReadEventArgs e)
        {
            try
            {
                string hostName = ((Events)sender).HostName;
                RFIDReaderItem reader;

                lock (_lock)
                {
                    if (!_readerList.ContainsKey(hostName))
                        return;
                    reader = _readerList[hostName];
                }

                TagData[] tagData = reader.ReaderAPI.Actions.GetReadTags(50);
                if (tagData != null)
                {
                    foreach (var tag in tagData)
                    {
                        if (tag.OpCode == ACCESS_OPERATION_CODE.ACCESS_OPERATION_NONE ||
                            (tag.OpCode == ACCESS_OPERATION_CODE.ACCESS_OPERATION_READ &&
                             tag.OpStatus == ACCESS_OPERATION_STATUS.ACCESS_SUCCESS))
                        {
                            string tagID = tag.TagID;
                            lock (reader.TagDetected)
                            {
                                if (!reader.TagDetected.ContainsKey(tagID))
                                {
                                    reader.TagDetected.Add(tagID, false);
                                    reader.TagTime.Add(tagID, DateTime.UtcNow);
                                }
                            }
                        }
                    }
                }
            }
            catch { }
        }

        private void Events_StatusNotify(object sender, Events.StatusEventArgs e)
        {
            try
            {
                string hostName = ((Events)sender).HostName;
                RFIDReaderItem reader;

                lock (_lock)
                {
                    if (!_readerList.ContainsKey(hostName))
                        return;
                    reader = _readerList[hostName];
                }

                if (!reader.ReaderAPI.IsConnected)
                {
                    reader.IsConnected = false;
                    reader.ReconnectRequired = true;
                    _hubContext.Clients.All.updateReaderStatus(hostName, "Disconnected");
                }
            }
            catch { }
        }
    }
}
