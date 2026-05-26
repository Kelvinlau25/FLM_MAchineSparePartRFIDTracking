using Microsoft.AspNetCore.SignalR;
using FILM_Sparepart_MVC.Services;

namespace FILM_Sparepart_MVC.Hubs
{
    /// <summary>
    /// SignalR hub for real-time RFID reader status updates.
    /// Web clients connect here to receive live reader statuses,
    /// tag detections, and connection events.
    /// </summary>
    public class RfidHub : Hub
    {
        private readonly RFIDService _rfidService;
        private readonly ILogger<RfidHub> _logger;

        public RfidHub(RFIDService rfidService, ILogger<RfidHub> logger)
        {
            _rfidService = rfidService;
            _logger = logger;
        }

        /// <summary>
        /// Called when a client connects. Sends the current reader statuses immediately.
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("SignalR client connected: {ConnectionId}", Context.ConnectionId);
            var statuses = _rfidService.GetReaderStatuses();
            await Clients.Caller.SendAsync("ReceiveReaderStatuses", statuses);
            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation("SignalR client disconnected: {ConnectionId}", Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Client requests to connect a specific reader by its IP address.
        /// </summary>
        public async Task ConnectReader(string ipAddress)
        {
            _logger.LogInformation("Client requested connect for reader: {IpAddress}", ipAddress);
            await _rfidService.ConnectReaderAsync(ipAddress);
        }

        /// <summary>
        /// Client requests to disconnect a specific reader by its IP address.
        /// </summary>
        public async Task DisconnectReader(string ipAddress)
        {
            _logger.LogInformation("Client requested disconnect for reader: {IpAddress}", ipAddress);
            await _rfidService.DisconnectReaderAsync(ipAddress);
        }

        /// <summary>
        /// Client requests to connect all configured readers.
        /// </summary>
        public async Task ConnectAllReaders()
        {
            _logger.LogInformation("Client requested connect all readers");
            await _rfidService.ConnectAllReadersAsync();
        }

        /// <summary>
        /// Client requests to disconnect all readers.
        /// </summary>
        public async Task DisconnectAllReaders()
        {
            _logger.LogInformation("Client requested disconnect all readers");
            await _rfidService.DisconnectAllReadersAsync();
        }

        /// <summary>
        /// Client requests the current status of all readers.
        /// </summary>
        public async Task GetReaderStatuses()
        {
            var statuses = _rfidService.GetReaderStatuses();
            await Clients.Caller.SendAsync("ReceiveReaderStatuses", statuses);
        }
    }
}
