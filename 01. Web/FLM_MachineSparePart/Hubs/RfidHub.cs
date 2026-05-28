using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using FILM_Sparepart_MVC.Services;

namespace FILM_Sparepart_MVC.Hubs
{
    /// <summary>
    /// SignalR Hub for RFID reader real-time communication.
    /// Provides methods for the frontend to control RFID readers
    /// and receives real-time status updates.
    /// </summary>
    public class RfidHub : Hub
    {
        private readonly RFIDService _rfidService = RFIDService.Instance;

        /// <summary>
        /// Load RFID configuration from database
        /// </summary>
        public object LoadConfig()
        {
            var result = _rfidService.LoadConfiguration();
            return new
            {
                success = result.Success,
                message = result.Message,
                readers = _rfidService.GetReaderList()
            };
        }

        /// <summary>
        /// Get current reader list
        /// </summary>
        public object GetReaders()
        {
            return new
            {
                isInitialized = _rfidService.IsInitialized,
                readers = _rfidService.GetReaderList()
            };
        }

        /// <summary>
        /// Connect all readers
        /// </summary>
        public async Task ConnectAll()
        {
            await _rfidService.ConnectAllAsync();
        }

        /// <summary>
        /// Disconnect all readers
        /// </summary>
        public async Task DisconnectAll()
        {
            await _rfidService.DisconnectAllAsync();
        }

        /// <summary>
        /// Connect a single reader by IP address
        /// </summary>
        public async Task<object> ConnectReader(string ipAddress)
        {
            var result = await _rfidService.ConnectReaderAsync(ipAddress);
            return new
            {
                success = result.Success,
                message = result.Message
            };
        }

        /// <summary>
        /// Disconnect a single reader by IP address
        /// </summary>
        public async Task<object> DisconnectReader(string ipAddress)
        {
            var result = await _rfidService.DisconnectReaderAsync(ipAddress);
            return new
            {
                success = result.Success,
                message = result.Message
            };
        }

        public override Task OnConnected()
        {
            System.Diagnostics.Debug.WriteLine($"[RfidHub] Client connected: {Context.ConnectionId}");
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            System.Diagnostics.Debug.WriteLine($"[RfidHub] Client disconnected: {Context.ConnectionId}");
            return base.OnDisconnected(stopCalled);
        }
    }
}
