using Microsoft.AspNet.SignalR;
using FILM_Sparepart_MVC.Services;
using System.Threading.Tasks;

namespace FILM_Sparepart_MVC.Hubs
{
    public class RfidHub : Hub
    {
        private static readonly RFIDService _rfidService = new RFIDService();
        private static readonly object _lock = new object();

        public void ConnectReader(string ipAddress)
        {
            var connectionId = Context.ConnectionId;
            var result = _rfidService.ConnectReader(ipAddress, connectionId);
            Clients.Client(connectionId).updateReaderStatus(ipAddress, result);
        }

        public void DisconnectReader(string ipAddress)
        {
            var connectionId = Context.ConnectionId;
            var result = _rfidService.DisconnectReader(ipAddress);
            Clients.Client(connectionId).updateReaderStatus(ipAddress, result);
        }

        public void ConnectAll()
        {
            var connectionId = Context.ConnectionId;
            _rfidService.ConnectAll(connectionId);
        }

        public void DisconnectAll()
        {
            var connectionId = Context.ConnectionId;
            _rfidService.DisconnectAll(connectionId);
        }

        public void GetReaderStatuses()
        {
            var connectionId = Context.ConnectionId;
            var statuses = _rfidService.GetReaderStatuses();
            Clients.Client(connectionId).receiveReaderStatuses(statuses);
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            return base.OnDisconnected(stopCalled);
        }
    }
}
