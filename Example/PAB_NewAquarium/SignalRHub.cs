using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace PAB_NewAquarium
{
    public class SignalRHub : Hub
    {
        public static string rfidConnectionID = "";

        public void ReceiveClientConnectionID(string rfidConnectionId)
        {
            rfidConnectionID = rfidConnectionId;
        }

        public void GetRFIDConnectionID(string webConnectionID)
        {
            Clients.Client(webConnectionID).getRFIDConnectionID(rfidConnectionID);
        }

        public void ConnectRFID(string webConnectionID, string readerName, string readerSignal)
        {
            if (rfidConnectionID != "")
            {
                Clients.Client(rfidConnectionID).connectRFID(webConnectionID, readerName, readerSignal);
            }
        }

        public void DisconnectRFID(string webConnectionID)
        {
            if (rfidConnectionID != "")
            {
                Clients.Client(rfidConnectionID).disconnectRFID(webConnectionID);
            }
        }

        public void ReceiveMessage(string webConnectionID, string message)
        {
            Clients.Client(webConnectionID).receiveMessage(message);
        }

        public void GetTags(string webConnectionID, string tag)
        {
            Clients.Client(webConnectionID).getTags(tag);
        }

        public override Task OnConnected()
        {
            var connectedClientId = Context.ConnectionId;            
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var disconnectedClientId = Context.ConnectionId;
            if (rfidConnectionID != "")
            {
                Clients.Client(rfidConnectionID).disconnectRFID(disconnectedClientId);
            }
            if (disconnectedClientId == rfidConnectionID)
            {
                rfidConnectionID = "";
            }
            
            return base.OnDisconnected(stopCalled);
        }
    }
}