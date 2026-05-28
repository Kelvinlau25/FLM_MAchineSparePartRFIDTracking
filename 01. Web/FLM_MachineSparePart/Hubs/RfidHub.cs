using Microsoft.AspNet.SignalR;
using FILM_Sparepart_MVC.Services;
using System.Collections.Generic;

namespace FILM_Sparepart_MVC.Hubs
{
    public class RfidHub : Hub
    {
        private static readonly RFIDService _rfidService = new RFIDService();

        /// <summary>
        /// Load reader configurations and return current status
        /// </summary>
        public void LoadReaders()
        {
            var readers = _rfidService.LoadReaderConfigurations();
            Clients.Caller.loadReadersResult(readers);
        }

        /// <summary>
        /// Connect a single reader by IP
        /// </summary>
        public void ConnectReader(string readerIP)
        {
            Clients.Caller.readerStatusChanged(readerIP, "Connecting...", false);
            var result = _rfidService.ConnectReader(readerIP);
            Clients.Caller.connectResult(result);
        }

        /// <summary>
        /// Disconnect a single reader by IP
        /// </summary>
        public void DisconnectReader(string readerIP)
        {
            Clients.Caller.readerStatusChanged(readerIP, "Disconnecting...", false);
            var result = _rfidService.DisconnectReader(readerIP);
            Clients.Caller.disconnectResult(result);
        }

        /// <summary>
        /// Connect all configured readers
        /// </summary>
        public void ConnectAll()
        {
            var results = _rfidService.ConnectAll();
            Clients.Caller.connectAllResult(results);
        }

        /// <summary>
        /// Disconnect all connected readers
        /// </summary>
        public void DisconnectAll()
        {
            var results = _rfidService.DisconnectAll();
            Clients.Caller.disconnectAllResult(results);
        }

        /// <summary>
        /// Get current status of all readers
        /// </summary>
        public void GetReaderStatuses()
        {
            var statuses = _rfidService.GetReaderStatuses();
            Clients.Caller.readerStatuses(statuses);
        }
    }
}
