using System;
using System.Collections;
using Symbol.RFID3;

namespace Library.Reader.Objects
{
    public class ReaderObject
    {
        private delegate void UpdateRead(Symbol.RFID3.Events.ReadEventData eventData);
        private delegate void UpdateStatus(Symbol.RFID3.Events.StatusEventData eventData);

        private UpdateRead m_UpdateReadHandler;
        private UpdateStatus m_UpdateStatusHandler;

        public ReaderObject()
        {
            m_UpdateReadHandler = myUpdateRead;
            m_UpdateStatusHandler = myUpdateStatus;
        }

        public ReaderObject(int index, string host_name, string port) : this()
        {
            Index = index;
            HostName = host_name;
            Port = port;
            MyReader = new RFIDReader(HostName, Convert.ToUInt32(Port), 50000);
        }

        public int Index { get; set; }
        public RFIDReader MyReader { get; set; }
        public bool ReconnectRequired { get; set; } = false;
        public Hashtable TagDetected { get; set; }
        public Hashtable TagTime { get; set; }
        public Result ConnectResult { get; set; }
        public string Name { get; set; }
        public string IPAdress { get; set; }
        public string HostName { get; set; }
        public string Port { get; set; }
        public string StoredProcedure { get; set; }
        public string Server { get; set; }
        public string StoredProcedure2 { get; set; }
        public string Server2 { get; set; }

        public void ConnectReader()
        {
            bool status = false;
            string message = string.Empty;

            try
            {
                if (ReconnectRequired)
                {
                    MyReader.Reconnect();
                }
                else
                {
                    MyReader.Connect();
                }

                status = true;

                MyReader.Events.ReadNotify += Events_ReadNotify;
                MyReader.Events.StatusNotify += Events_StatusNotify;
                MyReader.Events.NotifyGPIEvent = true;
                MyReader.Events.NotifyReaderDisconnectEvent = true;
                MyReader.Events.NotifyAccessStartEvent = true;
                MyReader.Events.NotifyAccessStopEvent = true;
                MyReader.Events.NotifyInventoryStartEvent = true;
                MyReader.Events.NotifyInventoryStopEvent = true;
            }
            catch (OperationFailureException operationException)
            {
                message = "Connect Failed : " + operationException.Result;
            }
            catch (System.Net.Sockets.SocketException socketException)
            {
                message = "Connect Failed [" + HostName + "]: " + socketException.Message;
            }
            catch (Exception ex)
            {
                message = "Connect Failed [" + HostName + "]: " + ex.Message;
            }

            ConnectResult = new Result(status, message);
        }

        public void StartRead()
        {
            ushort[] antennaList = new ushort[] { 1, 2 };
            AntennaInfo antennaInfo = new AntennaInfo(antennaList);
            MyReader.Actions.Inventory.Perform(null, null, antennaInfo);
        }

        private void Events_ReadNotify(object sender, Symbol.RFID3.Events.ReadEventArgs readEventArgs)
        {
            try
            {
                string readerHostName = ((Symbol.RFID3.Events)sender).HostName;
                m_UpdateReadHandler?.Invoke(readEventArgs.ReadEventData);
            }
            catch
            {
            }
        }

        public void Events_StatusNotify(object sender, Symbol.RFID3.Events.StatusEventArgs statusEventArgs)
        {
            try
            {
                string readerHostName = ((Symbol.RFID3.Events)sender).HostName;
                m_UpdateStatusHandler?.Invoke(statusEventArgs.StatusEventData);
            }
            catch
            {
            }
        }

        private void myUpdateRead(Symbol.RFID3.Events.ReadEventData eventData)
        {
        }

        private void myUpdateStatus(Symbol.RFID3.Events.StatusEventData eventData)
        {
            string statusMsg = string.Empty;

            switch (eventData.StatusEventType)
            {
                case Events.STATUS_EVENT_TYPE.INVENTORY_START_EVENT:
                    statusMsg = "Inventory started";
                    break;
                case Events.STATUS_EVENT_TYPE.INVENTORY_STOP_EVENT:
                    statusMsg = "Inventory stopped";
                    break;
                case Events.STATUS_EVENT_TYPE.ACCESS_START_EVENT:
                    statusMsg = "Access Operation started";
                    break;
                case Events.STATUS_EVENT_TYPE.ACCESS_STOP_EVENT:
                    statusMsg = "Access Operation stopped";
                    break;
                case Events.STATUS_EVENT_TYPE.BUFFER_FULL_WARNING_EVENT:
                    statusMsg = " Buffer full warning";
                    break;
                case Events.STATUS_EVENT_TYPE.BUFFER_FULL_EVENT:
                    statusMsg = "Buffer full";
                    break;
                case Events.STATUS_EVENT_TYPE.DISCONNECTION_EVENT:
                    statusMsg = "Disconnection Event " + eventData.DisconnectionEventData.DisconnectEventInfo;
                    break;
                case Events.STATUS_EVENT_TYPE.ANTENNA_EVENT:
                    statusMsg = "Antenna Status Update";
                    break;
                case Events.STATUS_EVENT_TYPE.NXP_EAS_ALARM_EVENT:
                    break;
                case Events.STATUS_EVENT_TYPE.READER_EXCEPTION_EVENT:
                    statusMsg = "Reader ExceptionEvent " + eventData.ReaderExceptionEventData.ReaderExceptionEventInfo;
                    break;
                default:
                    statusMsg = "Unhandled Status";
                    break;
            }

            if (!MyReader.IsConnected)
            {
                try
                {
                    MyReader.Reconnect();
                    statusMsg += ", Reconnect success.";
                }
                catch
                {
                    statusMsg += ", Reconnect fail.";
                    ReconnectRequired = true;
                }
            }

            Console.WriteLine(statusMsg);
        }
    }

    public class Result
    {
        public Result(bool status, string message)
        {
            Status = status;
            Message = message;
        }

        public bool Status { get; set; } = false;
        public string Message { get; set; }
    }
}