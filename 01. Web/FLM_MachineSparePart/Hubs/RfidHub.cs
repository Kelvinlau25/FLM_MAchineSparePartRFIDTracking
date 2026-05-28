using FILM_Sparepart_MVC.Models;
using FILM_Sparepart_MVC.Services;
using Microsoft.AspNetCore.SignalR;

namespace FILM_Sparepart_MVC.Hubs
{
    public class RfidHub : Hub
    {
        private readonly RFIDService _rfidService;

        public RfidHub(RFIDService rfidService)
        {
            _rfidService = rfidService;
        }

        public override async Task OnConnectedAsync()
        {
            // Replay current state to this newly connected client
            var statuses = _rfidService.GetReaderStatuses();
            foreach (var s in statuses)
            {
                string status  = s.IsConnected ? "Connected"    : "Disconnected";
                string message = s.IsConnected ? "Connected successfully." : "Disconnected.";
                await Clients.Caller.SendAsync("OnReaderStatus", s.HostName, status, message);
            }

            await base.OnConnectedAsync();
        }

        public async Task GetReaders()
        {
            var readers = _rfidService.GetDefaultReaders();
            await Clients.Caller.SendAsync("OnReadersLoaded", readers);
        }

        public async Task GetReaderStatuses()
        {
            var statuses = _rfidService.GetReaderStatuses();
            await Clients.Caller.SendAsync("OnReaderStatuses", statuses);
        }

        /// <summary>
        /// Attempts to connect a reader. Sends OnConnectResult back to the calling client only.
        /// Result status: "AlreadyConnected" | "Connected" | "Failed"
        /// </summary>
        public async Task ConnectReader(string ip)
        {
            var result = await _rfidService.ConnectReaderByIpAsync(ip);
            await Clients.Caller.SendAsync("OnConnectResult", new
            {
                ipAddress = result.IpAddress,
                status = result.Status.ToString(),   // "AlreadyConnected" | "Connected" | "Failed"
                message = result.Message
            });
        }

        public async Task DisconnectReader(string ip)
        {
            _rfidService.DisconnectReaderByIp(ip);
            await Task.CompletedTask;
        }

        public async Task DisconnectAll()
        {
            _rfidService.DisconnectAllReaders();
            await Task.CompletedTask;
        }

        public async Task ConnectAll()
        {
            await _rfidService.ConnectAllReadersAsync();
        }

        public async Task Ping()
        {
            await Clients.Caller.SendAsync("OnPong", "MVC-RFID-SERVICE",
                DateTime.Now.ToString("HH:mm:ss"));
        }
    }
}