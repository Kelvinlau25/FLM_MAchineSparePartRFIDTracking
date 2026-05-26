using Microsoft.AspNet.SignalR;
using PAB_NewAquarium.Models;
using PAB_NewAquarium.Services;

namespace PAB_NewAquarium.Hubs
{
    public class RFIDFabricRegHub : Hub
    {
        private static RFIDService _rfidService = new RFIDService();
        private static readonly object _lock = new object();
        private static bool _isStarted = false;

        public void StartRFID(RFIDModel model, string connectionId, string connectionHub)
        {
            lock (_lock)
            {
                if (!_isStarted)
                {
                    _isStarted = _rfidService.Start(model, connectionId, connectionHub);
                }
                else
                {
                    Clients.Client(connectionId).broadcastMessage("Error", "RFID Service already used by another registration page. Please try again later.");
                }
            }
        }

        public void StopRFID(string connectionId)
        {
            lock (_lock)
            {
                if (_isStarted)
                {
                    _rfidService.Stop();
                    _isStarted = false;
                }
            }
        }
    }
}