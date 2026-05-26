using System.Collections;
using Symbol.RFID3;

namespace FILM_Sparepart_MVC.Library.Reader.Objects
{
    /// <summary>
    /// Wrapper around the Symbol RFIDReader for a single physical reader.
    /// Provides connect/disconnect/reconnect and inventory operations.
    /// This is the C# equivalent of Library.Reader.Objects.ReaderObject from the VB.NET RFID project.
    /// </summary>
    public class ReaderObject
    {
        public int Index { get; set; }
        public string HostName { get; set; } = "";
        public string IpAddress { get; set; } = "";
        public string Port { get; set; } = "5084";
        public string Name { get; set; } = "";
        public string StoredProcedure { get; set; } = "";
        public string Server { get; set; } = "MSSQL";
        public string? StoredProcedure2 { get; set; }
        public string? Server2 { get; set; }

        public RFIDReader? Reader { get; set; }
        public bool ReconnectRequired { get; set; }
        public Hashtable TagDetected { get; set; } = new();
        public Hashtable TagTime { get; set; } = new();

        public ReaderObject() { }

        public ReaderObject(int index, string ipAddress, string port)
        {
            Index = index;
            IpAddress = ipAddress;
            Port = port;
            Reader = new RFIDReader(ipAddress, port, 50000);
        }

        /// <summary>
        /// Connects to the reader. If a reconnect is required, attempts reconnect first.
        /// </summary>
        public ConnectResult ConnectReader()
        {
            try
            {
                if (ReconnectRequired && Reader != null)
                {
                    Reader.Reconnect();
                }
                else
                {
                    Reader ??= new RFIDReader(IpAddress, Port, 50000);
                    Reader.Connect();
                }

                return new ConnectResult(true, "");
            }
            catch (OperationFailureException ex)
            {
                return new ConnectResult(false, $"Connect Failed: {ex.Result}");
            }
            catch (Exception ex)
            {
                return new ConnectResult(false, $"Connect Failed [{HostName}]: {ex.Message}");
            }
        }

        /// <summary>
        /// Starts inventory on antennas 1 and 2.
        /// </summary>
        public void StartRead()
        {
            var antennaList = new ushort[] { 1, 2 };
            var antennaInfo = new AntennaInfo(antennaList);
            Reader?.Actions.Inventory.Perform(null, null, antennaInfo);
        }
    }

    public class ConnectResult
    {
        public bool Status { get; set; }
        public string Message { get; set; } = "";

        public ConnectResult(bool status, string message)
        {
            Status = status;
            Message = message;
        }
    }
}