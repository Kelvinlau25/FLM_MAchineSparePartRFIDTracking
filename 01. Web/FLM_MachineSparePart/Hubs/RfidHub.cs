using System;
using System.Threading.Tasks;
using FILM_Sparepart_MVC.Models;
using FILM_Sparepart_MVC.Services;
using Microsoft.AspNet.SignalR;

namespace FILM_Sparepart_MVC.Hubs
{
    public class RfidHub : Hub
    {
        private readonly RFIDService _rfidService;

        public RfidHub()
        {
            _rfidService = RFIDService.Instance;
        }

        public override Task OnConnected()
        {
            // Replay current state to this newly connected client
            var statuses = _rfidService.GetReaderStatuses();
            foreach (var s in statuses)
            {
                string status  = s.IsConnected ? "Connected"    : "Disconnected";
                string message = s.IsConnected ? "Connected successfully." : "Disconnected.";
                Clients.Caller.OnReaderStatus(s.HostName, status, message);
            }

            return base.OnConnected();
        }

        public void GetReaders()
        {
            var readers = _rfidService.GetDefaultReaders();
            Clients.Caller.OnReadersLoaded(readers);
        }

        public void GetReaderStatuses()
        {
            var statuses = _rfidService.GetReaderStatuses();
            Clients.Caller.OnReaderStatuses(statuses);
        }

        /// <summary>
        /// Attempts to connect a reader. Sends OnConnectResult back to the calling client only.
        /// Result status: "AlreadyConnected" | "Connected" | "Failed"
        /// </summary>
        public async Task ConnectReader(string ip)
        {
            var result = await _rfidService.ConnectReaderByIpAsync(ip);
            Clients.Caller.OnConnectResult(new
            {
                ipAddress = result.IpAddress,
                status = result.Status.ToString(),   // "AlreadyConnected" | "Connected" | "Failed"
                message = result.Message
            });
        }

        public void DisconnectReader(string ip)
        {
            _rfidService.DisconnectReaderByIp(ip);
        }

        public void DisconnectAll()
        {
            _rfidService.DisconnectAllReaders();
        }

        public async Task ConnectAll()
        {
            await _rfidService.ConnectAllReadersAsync();
        }

        public void Ping()
        {
            Clients.Caller.OnPong("MVC-RFID-SERVICE",
                DateTime.Now.ToString("HH:mm:ss"));
        }
    }
}