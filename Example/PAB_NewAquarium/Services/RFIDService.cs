using Microsoft.AspNet.SignalR;
using PAB_NewAquarium.Hubs;
using PAB_NewAquarium.Models;
using Symbol.RFID3;
using System;
using System.Linq;

namespace PAB_NewAquarium.Services
{
    public class RFIDService
    {
        private RFIDReader _reader;
        private RFIDModel _model;
        private string _connectionId;
        private IHubContext hubContext;

        public bool Start(RFIDModel model, string connectionId, string hub)
        {
            try
            {
                InitHubContext(hub);
                _model = model;
                _connectionId = connectionId;
                _reader = new RFIDReader(_model.READER_IP, _model.READER_PORT, 0);
                _reader.Connect();
                _reader.Events.ReadNotify += Events_ReadNotify;
                _reader.Actions.Inventory.Perform();
                hubContext.Clients.Client(_connectionId).broadcastMessage("Success", "RFID Service is ready.");

                return true;
            }
            catch (OperationFailureException ex)
            {
                if (ex.Result == RFIDResults.RFID_COMM_CONNECTION_ALREADY_EXISTS)
                {
                    hubContext.Clients.Client(_connectionId).broadcastMessage("Error", "RFID Service already used by another page. Please try again later.");
                }
                else
                {
                    hubContext.Clients.Client(_connectionId).broadcastMessage("Error", "Failed to start RFID Service.");
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Stop()
        {
            try
            {
                if (_reader != null)
                {
                    _reader.Actions.Inventory.Stop();
                    _reader.Events.ReadNotify -= Events_ReadNotify;
                    _reader.Dispose();
                }
            }
            catch (Exception)
            {
            }
        }

        public void InitHubContext(string hub)
        {
            try
            {
                switch (hub)
                {
                    case "FabricReg":
                        hubContext = GlobalHost.ConnectionManager.GetHubContext<RFIDFabricRegHub>();
                        break;
                    case "Dashbord":
                        hubContext = GlobalHost.ConnectionManager.GetHubContext<RFIDDashboardHub>();
                        break;
                    default:
                        hubContext = GlobalHost.ConnectionManager.GetHubContext<RFIDDashboardHub>();
                        break;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void Events_ReadNotify(object sender, Events.ReadEventArgs e)
        {
            try
            {
                // Fetch up to 1000 recently read tags
                TagData[] tagData = _reader.Actions.GetReadTags(1000);

                if (tagData != null)
                {
                    // Filter tags by signal strength and group by unique TagID
                    var filterTag = tagData.Where(tag => tag.PeakRSSI >= _model.READER_RANGE).GroupBy(tag => tag.TagID);

                    // Broadcast each unique tag to the specific client
                    foreach (var item in filterTag)
                    {
                        hubContext.Clients.Client(_connectionId).broadcastTag(item.Key);
                    }
                }
            }
            catch (Exception ex)
            {
                hubContext.Clients.Client(_connectionId).broadcastTag(ex.Message);
            }
        }
    }
}