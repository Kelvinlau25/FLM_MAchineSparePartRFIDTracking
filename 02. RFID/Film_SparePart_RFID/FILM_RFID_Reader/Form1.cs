using Library.Reader.Helpers;
using Library.Reader.Objects;
using Symbol.RFID3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using static Symbol.RFID3.Events;

namespace FILM_RFID_READER_DEPLOY
{
    public partial class Form1 : Form
    {
        private string lockTag = string.Empty;
        private System.Threading.Timer btnConnectAllTimer;

        private class returnResult
        {
            internal int rowIndex;
            internal string Result;
            internal bool SkipThisConnect;
        }

        private RFID_Reader_Item _r_dtl;
        private readonly Dictionary<string, RFID_Reader_Item> m_reader_list = new Dictionary<string, RFID_Reader_Item>();
        private string selected_reader = string.Empty;
        internal RFIDReader m_ReaderAPI;

        private TagData m_ReadTag = null;
        internal string m_SelectedTagID = null;
        private Hashtable m_TagTable;
        private uint m_TagTotalCount;
        private DateTime m_LastSent = new DateTime();
        private UpdateRead m_UpdateReadHandler = null;
        private UpdateStatus m_UpdateStatusHandler = null;
        private DateTime m_emptyTagTime;
        private string m_ReaderName;

        private List<EmailObject> PendingEmailList = new List<EmailObject>();
        private List<EmailTagObject> PendingTagEmailList = new List<EmailTagObject>();
        private List<ReaderEmailObject> ReaderEmailDCList = new List<ReaderEmailObject>();
        private List<ReaderEmailObject> ReaderEmailRCList = new List<ReaderEmailObject>();
        private List<ReaderEmailObject> ReaderEmailFailList = new List<ReaderEmailObject>();
        private List<ReaderTagObject> ReaderEmailTagList = new List<ReaderTagObject>();

        private Library.Database.DTO dto = new Library.Database.DTO();
        private Library.Database.RFID_COMMON db = new Library.Database.RFID_COMMON();
        private string mode = "NEW";

        private int int_reset;
        private int int_towerlight_reset;
        private int int_towerlight_current;
        private int int_towerlight_mode;

        private readonly logger _logger = new logger();

        private string loopCoil1 = string.Empty;
        private string loopCoil2 = string.Empty;
        private string hostn = string.Empty;
        private string tower = string.Empty;

        private string _Error = string.Empty;

        private delegate void UpdateRead(string hostName, TagData[] eventData);
        private delegate void UpdateStatus(string hostName, Symbol.RFID3.Events.StatusEventData eventData);

        internal class AccessOperationResult
        {
            public ACCESS_OPERATION_CODE m_OpCode = ACCESS_OPERATION_CODE.ACCESS_OPERATION_READ;
            public string m_VendorMessage = string.Empty;
            public string m_StatusDescription = string.Empty;
            public RFIDResults m_Result = RFIDResults.RFID_NO_ACCESS_IN_PROGRESS;
        }

        public Form1()
        {
            try
            {
                InitializeComponent();

                Load += Form1_Load;
                FormClosing += AppForm_FormClosing;

                btnConnectAll.Click += btnConnectAll_Click;
                btnDisconnectAll.Click += btnDisconnectAll_Click;
                btnConnect.Click += btnConnect_Click;
                Button1.Click += Button1_Click;
                lv_reader.SelectedIndexChanged += lv_reader_SelectedIndexChanged;

                bg_GetRFIDConfig.DoWork += bg_GetRFIDConfig_DoWork;
                bg_GetRFIDConfig.RunWorkerCompleted += bg_GetRFIDConfig_RunWorkerCompleted;
                bg_FormClosing.DoWork += bg_FormClosing_DoWork;
                bg_FormClosing.RunWorkerCompleted += bg_FormClosing_RunWorkerCompleted;
                ReconnectBackgroundWorker.DoWork += ReconnectBackgroundWorker_DoWork;
                ReconnectBackgroundWorker.RunWorkerCompleted += ReconnectBackgroundWorker_RunWorkerCompleted;

                Reset_Table.Tick += Reset_Table_Tick;
                Timer1.Tick += Timer1_Tick;
                Timer2.Tick += Timer2_Tick;
                Timer3.Tick += Timer3_Tick;
                TimerReconnect.Tick += TimerReconnect_Tick;
                TimerTagReconnect.Tick += TimerTagReconnect_Tick;
                TimerSendDCEmail.Tick += TimerSendDCEmail_Tick;
                TimerSendRCEmail.Tick += TimerSendRCEmail_Tick;
                TimerSendFailEmail.Tick += TimerSendFailEmail_Tick;
                TimerSendTagEmail.Tick += TimerSendTagEmail_Tick;
                TimerTagLock.Tick += TimerTagLock_Tick;

                m_UpdateStatusHandler = myUpdateStatus;
                m_UpdateReadHandler = myUpdateRead;

                m_TagTable = new Hashtable();
                m_TagTotalCount = 0;

                Loading frm = new Loading();
                bg_GetRFIDConfig.RunWorkerAsync(frm);
                frm.ShowDialog();

                btnConnectAllTimer = new System.Threading.Timer(TimerCallback, null, 1000, Timeout.Infinite);

                _logger._LogGen("New() >> Init done");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void TimerCallback(object state)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => btnConnectAll_Click(this, EventArgs.Empty)));
            }
            else
            {
                btnConnectAll_Click(this, EventArgs.Empty);
            }

            btnConnectAllTimer?.Dispose();
        }

        private void myUpdateStatus(string hostName, Symbol.RFID3.Events.StatusEventData eventData)
        {
            string statusMsg = string.Empty;
            int running = 0;
            RFID_Reader_Item reader = m_reader_list[hostName];

            switch (eventData.StatusEventType)
            {
                case Symbol.RFID3.Events.STATUS_EVENT_TYPE.INVENTORY_START_EVENT:
                    statusMsg = "Inventory started";
                    break;
                case Symbol.RFID3.Events.STATUS_EVENT_TYPE.INVENTORY_STOP_EVENT:
                    statusMsg = "Inventory stopped";
                    break;
                case Symbol.RFID3.Events.STATUS_EVENT_TYPE.ACCESS_START_EVENT:
                    statusMsg = "Access Operation started";
                    break;
                case Symbol.RFID3.Events.STATUS_EVENT_TYPE.ACCESS_STOP_EVENT:
                    statusMsg = "Access Operation stopped";
                    break;
                case Symbol.RFID3.Events.STATUS_EVENT_TYPE.BUFFER_FULL_WARNING_EVENT:
                    statusMsg = " Buffer full warning";
                    break;
                case Symbol.RFID3.Events.STATUS_EVENT_TYPE.BUFFER_FULL_EVENT:
                    statusMsg = "Buffer full";
                    break;
                case Symbol.RFID3.Events.STATUS_EVENT_TYPE.DISCONNECTION_EVENT:
                    statusMsg = "Disconnection Event " + eventData.DisconnectionEventData.DisconnectEventInfo;
                    break;
                case Symbol.RFID3.Events.STATUS_EVENT_TYPE.ANTENNA_EVENT:
                    statusMsg = "Antenna Status Update";
                    break;
                case Symbol.RFID3.Events.STATUS_EVENT_TYPE.NXP_EAS_ALARM_EVENT:
                    break;
                case Symbol.RFID3.Events.STATUS_EVENT_TYPE.READER_EXCEPTION_EVENT:
                    statusMsg = "Reader ExceptionEvent " + eventData.ReaderExceptionEventData.ReaderExceptionEventInfo;
                    break;
                case Symbol.RFID3.Events.STATUS_EVENT_TYPE.GPI_EVENT:
                    if (Timer1.Enabled)
                    {
                        check2ndloop("CHECK", eventData.GPIEventData.PortNumber.ToString(), hostName, eventData.GPIEventData.GPIEvent, 0);
                    }
                    else
                    {
                        updateIn("CHECK", eventData.GPIEventData.PortNumber.ToString(), hostName, eventData.GPIEventData.GPIEvent, 0);
                    }
                    break;
                default:
                    statusMsg = "Unhandled Status";
                    break;
            }

            if (!reader.m_ReaderAPI.IsConnected)
            {
                reader.bool_ReconnectRequired = true;

                if (!ReconnectBackgroundWorker.IsBusy)
                {
                    ReconnectBackgroundWorker.RunWorkerAsync(reader.str_IPAdress + "-" + reader.int_Index);
                }

                ReaderEmailObject readerObject = new ReaderEmailObject(reader.str_HostName, reader.str_IPAdress, reader.str_Name, DateTime.Now, statusMsg);
                ReaderEmailDCList.Add(readerObject);

                if (!TimerSendDCEmail.Enabled)
                {
                    TimerSendDCEmail.Start();
                }
            }
        }

        private void check2ndloop(string action, string value, string hostname, bool eventData, int mode)
        {
            RFID_Reader_Item reader = m_reader_list[hostname];
            int indicator = 0;

            if ((int)m_reader_list[hostname].m_ReaderAPI.Config.GPI[2].PortState == 0)
            {
                loopCoil2 = "In";
            }

            if ((int)m_reader_list[hostname].m_ReaderAPI.Config.GPI[1].PortState == 0)
            {
                loopCoil2 = "Out";
            }

            if (tower == "1")
            {
                tower = "Out";
            }

            if (tower == "2")
            {
                tower = "In";
            }
        }

        private void updateIn(string action, string value, string hostname, bool eventData, int mode)
        {
            RFID_Reader_Item reader = m_reader_list[hostname];
            int indicator = 0;

            if ((int)m_reader_list[hostname].m_ReaderAPI.Config.GPI[1].PortState == 0)
            {
                hostn = hostname;

                if (hostn == "10.28.92.50")
                {
                    tower = "1";
                }
                else
                {
                    loopCoil1 = "In";
                }

                Timer1.Start();
                Timer1.Enabled = true;
            }

            if ((int)m_reader_list[hostname].m_ReaderAPI.Config.GPI[2].PortState == 0)
            {
                hostn = hostname;

                if (hostn == "10.28.92.50")
                {
                    tower = "2";
                }
                else
                {
                    loopCoil1 = "Out";
                }

                Timer1.Start();
                Timer1.Enabled = true;
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            RFID_Reader_Item reader = m_reader_list[hostn];

            if (loopCoil1 == "In")
            {
                if (loopCoil1 == loopCoil2)
                {
                    StartUpdateDB("IN");
                    Timer1.Stop();
                    Timer1.Enabled = false;
                }

                reader.ht_TagDetected.Clear();
                reader.ht_TagTime.Clear();
            }
            else if (loopCoil1 == "Out")
            {
                if (loopCoil1 == loopCoil2)
                {
                    StartUpdateDB("OUT");
                    Timer1.Stop();
                    Timer1.Enabled = false;
                }

                reader.ht_TagDetected.Clear();
                reader.ht_TagTime.Clear();
            }
            else if (tower == "In")
            {
                StartUpdateDBTower("IN");
                Timer1.Stop();
                Timer1.Enabled = false;
            }
            else if (tower == "Out")
            {
                StartUpdateDBTower("OUT");
                Timer1.Stop();
                Timer1.Enabled = false;
            }
            else
            {
                Timer1.Stop();
                Timer1.Enabled = false;
                reader.ht_TagDetected.Clear();
                reader.ht_TagTime.Clear();
            }
        }

        private void StartUpdateDB(string value)
        {
            RFID_Reader_Item reader = m_reader_list[hostn];

            foreach (DictionaryEntry tagItem in reader.ht_TagDetected)
            {
                _logger._LogGen("StartUpdateDB() >> pTRAN_TYPE: " + value + "; pRFID: " + tagItem.Key.ToString() + "; pReader: " + reader.str_HostName + "; pIPADDR: " + reader.str_IPAdress + "; SP: " + reader.str_Stored_Procedure);
                dto = db.RFID_Common_MSSQL(value, tagItem.Key.ToString(), reader.str_HostName, reader.str_IPAdress, reader.str_Stored_Procedure);
            }

            reader.ht_TagDetected.Clear();
            reader.ht_TagTime.Clear();
        }

        private void StartUpdateDBTower(string value)
        {
            RFID_Reader_Item reader = m_reader_list[hostn];

            foreach (DictionaryEntry tagItem in reader.ht_TagDetected)
            {
                _logger._LogGen("StartUpdateDBTower() >> pTRAN_TYPE: " + value + "; pRFID: " + tagItem.Key.ToString() + "; pReader: " + reader.str_HostName + "; pIPADDR: " + reader.str_IPAdress + "; SP: " + reader.str_Stored_Procedure);
                dto = db.RFID_Common_MSSQL_Tower(value, tagItem.Key.ToString(), reader.str_HostName, reader.str_IPAdress, reader.str_Stored_Procedure);
            }

            reader.ht_TagDetected.Clear();
            reader.ht_TagTime.Clear();
        }

        private void myUpdateRead(string hostName, TagData[] eventData)
        {
            int index = 0;
            string strFoundTag = string.Empty;
            string sendtag = string.Empty;
            string location = string.Empty;

            RFID_Reader_Item reader = m_reader_list[hostName];
            string antennaID = "1";

            if (hostName == "10.28.92.52")
            {
                location = "Green Tent House 2";
            }
            else if (hostName == "10.28.92.51")
            {
                location = "Green Tent House 1";
            }
            else if (hostName == "10.28.92.50")
            {
                location = "Tower Zone";
            }

            Symbol.RFID3.TagData[] tagData = reader.m_ReaderAPI.Actions.GetReadTags(50);
            List<object> updateTag = new List<object>();

            if (tagData != null)
            {
                List<TagDetails> tagDtl = new List<TagDetails>();

                for (int nIndex = 0; nIndex < tagData.Length; nIndex++)
                {
                    if (tagData[nIndex].OpCode == ACCESS_OPERATION_CODE.ACCESS_OPERATION_NONE ||
                        (tagData[nIndex].OpCode == ACCESS_OPERATION_CODE.ACCESS_OPERATION_READ &&
                         tagData[nIndex].OpStatus == ACCESS_OPERATION_STATUS.ACCESS_SUCCESS))
                    {
                        Symbol.RFID3.TagData tag = tagData[nIndex];
                        antennaID = tag.AntennaID.ToString();
                        string tagID = tag.TagID;
                        bool isFoundTag = false;

                        lock (reader.ht_TagDetected)
                        {
                            isFoundTag = reader.ht_TagDetected.ContainsKey(tagID);
                        }

                        if (!isFoundTag)
                        {
                            lock (reader.ht_TagDetected)
                            {
                                reader.ht_TagDetected.Add(tagID, false);
                            }

                            lock (reader.ht_TagTime)
                            {
                                reader.ht_TagTime.Add(tagID, DateTime.UtcNow);
                            }
                        }

                        tagDtl.Add(new TagDetails
                        {
                            hostName = hostName,
                            location = location,
                            antennaID = antennaID,
                            tagID = tagID,
                            tagTime = DateTime.Now
                        });
                    }
                }

                foreach (DictionaryEntry tagItem in reader.ht_TagDetected)
                {
                    if (!(bool)tagItem.Value)
                    {
                        if (antennaID == "1")
                        {
                            if (reader.str_SERVER == "ORACLE")
                            {
                            }
                            else if (reader.str_SERVER == "MSSQL")
                            {
                            }
                        }
                        else
                        {
                            if (reader.str_SERVER2 != null && reader.str_Stored_Procedure2 != null)
                            {
                                if (reader.str_SERVER2 == "ORACLE")
                                {
                                }
                                else if (reader.str_SERVER2 == "MSSQL")
                                {
                                }
                            }
                        }

                        if (!dto.Error)
                        {
                            TowerLight_ON(1, reader.str_IPAdress);
                        }
                        else
                        {
                            TowerLight_ON(2, reader.str_IPAdress);
                        }

                        updateTag.Add(tagItem.Key);
                    }
                }

                foreach (object tagItemKey in updateTag)
                {
                    reader.ht_TagDetected[tagItemKey] = true;
                }

                if (hostName == "10.28.92.50" && tagDtl.Count > 0)
                {
                    TagDetails tagDt = tagDtl.First();
                    char firstChar = tagDt.tagID.First();

                    if (firstChar == 'E' || firstChar == 'B')
                    {
                        if (lockTag == string.Empty || tagDt.tagID != lockTag)
                        {
                            lockTag = tagDt.tagID;
                            TimerTagLock.Start();

                            ReaderTagObject readerObject = new ReaderTagObject(tagDt.hostName, tagDt.location, tagDt.antennaID, tagDt.tagID, tagDt.tagTime);
                            ReaderEmailTagList.Add(readerObject);

                            if (!TimerSendTagEmail.Enabled)
                            {
                                TimerSendTagEmail.Start();
                            }

                            _logger._LogGen("myUpdateRead() >> Emailing >> hostName: " + tagDt.hostName + "; location: " + tagDt.location + "; antennaID: " + tagDt.antennaID + "; RFID: " + tagDt.tagID + "; tagTime: " + tagDt.tagTime);
                        }
                    }
                }
            }
            else
            {
                TimeSpan dd = DateTime.Now - m_emptyTagTime;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                lbl_error.Text = string.Empty;
                m_emptyTagTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Events_ReadNotify(object sender, Symbol.RFID3.Events.ReadEventArgs readEventArgs)
        {
            try
            {
                string readerHostName = ((Symbol.RFID3.Events)sender).HostName;
                Invoke(m_UpdateReadHandler, new object[] { readerHostName, readEventArgs.ReadEventData.TagData });
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
                Invoke(m_UpdateStatusHandler, new object[] { readerHostName, statusEventArgs.StatusEventData });
            }
            catch
            {
            }
        }

        private void ConnectBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string[] arry = e.Argument.ToString().Split('-');
            RFID_Reader_Item readerItem = m_reader_list[arry[0]];
            bool reconnectRequired = readerItem.bool_ReconnectRequired;
            bool success = false;
            returnResult result = new returnResult();

            if (readerItem.m_ReaderAPI.IsConnected)
            {
                result.SkipThisConnect = true;
                result.rowIndex = Convert.ToInt32(arry[1]);
                e.Result = result;
                return;
            }

            try
            {
                if (reconnectRequired)
                {
                    try
                    {
                        readerItem.m_ReaderAPI.Reconnect();
                        success = true;
                    }
                    catch
                    {
                        try
                        {
                            readerItem.m_ReaderAPI = new RFIDReader(readerItem.str_IPAdress, Convert.ToUInt32(readerItem.str_Port), 50000);
                            readerItem.m_ReaderAPI.Connect();
                            success = true;
                        }
                        catch
                        {
                            ReaderEmailObject readerObject = new ReaderEmailObject(readerItem.str_HostName, readerItem.str_IPAdress, readerItem.str_Name, DateTime.Now, "Reconnect failed.");
                            ReaderEmailFailList.Add(readerObject);

                            if (!TimerSendFailEmail.Enabled)
                            {
                                TimerSendFailEmail.Start();
                            }

                            result.rowIndex = Convert.ToInt32(arry[1]);
                            result.Result = "Disconnect";
                            e.Result = result;
                        }
                    }
                }
                else
                {
                    readerItem.m_ReaderAPI.Connect();
                    success = true;
                }

                if (success)
                {
                    result.rowIndex = Convert.ToInt32(arry[1]);
                    result.Result = "Connect Succeed";
                    e.Result = result;
                }
            }
            catch (OperationFailureException operationException)
            {
                result.rowIndex = Convert.ToInt32(arry[1]);
                result.Result = operationException.StatusDescription + " : " + arry[0];
                e.Result = result;

                ReaderEmailObject readerObject = new ReaderEmailObject(readerItem.str_HostName, readerItem.str_IPAdress, readerItem.str_Name, DateTime.Now, "Connect failed. " + e.Result);
                ReaderEmailFailList.Add(readerObject);

                if (!TimerSendFailEmail.Enabled)
                {
                    TimerSendFailEmail.Start();
                }
            }
            catch (Exception ex)
            {
                result.rowIndex = Convert.ToInt32(arry[1]);
                result.Result = ex.Message;
                e.Result = result;

                ReaderEmailObject readerObject = new ReaderEmailObject(readerItem.str_HostName, readerItem.str_IPAdress, readerItem.str_Name, DateTime.Now, "Connect failed. " + e.Result);
                ReaderEmailFailList.Add(readerObject);

                if (!TimerSendFailEmail.Enabled)
                {
                    TimerSendFailEmail.Start();
                }
            }
        }

        private void ConnectBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        }

        private void ConnectBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            returnResult returnResult = (returnResult)e.Result;
            int rowIndex = returnResult.rowIndex;
            string result = returnResult.Result;
            string hostName = lv_reader.Items[rowIndex].SubItems[1].Text;
            RFID_Reader_Item readerItem = m_reader_list[hostName];
            ushort[] antennaList = new ushort[] { 1, 2 };
            AntennaInfo antennaInfo = new AntennaInfo(antennaList);

            if (returnResult.SkipThisConnect)
            {
                returnResult.SkipThisConnect = false;
                lv_reader.Items[rowIndex].SubItems[3].Text = "Connected";
                lv_reader.Items[rowIndex].ForeColor = Color.Green;
                return;
            }

            if (result == "Connect Succeed")
            {
                if (!Properties.Settings.Default.TEST_ENVIRONMENT)
                {
                    readerItem.m_ReaderAPI.Events.ReadNotify += new ReadNotifyHandler(Events_ReadNotify);
                    readerItem.m_ReaderAPI.Events.AttachTagDataWithReadEvent = false;
                }

                readerItem.m_ReaderAPI.Events.StatusNotify += new StatusNotifyHandler(Events_StatusNotify);
                readerItem.m_ReaderAPI.Events.NotifyGPIEvent = true;
                readerItem.m_ReaderAPI.Events.NotifyBufferFullEvent = true;
                readerItem.m_ReaderAPI.Events.NotifyBufferFullWarningEvent = true;
                readerItem.m_ReaderAPI.Events.NotifyReaderDisconnectEvent = true;
                readerItem.m_ReaderAPI.Events.NotifyReaderExceptionEvent = true;
                readerItem.m_ReaderAPI.Events.NotifyAccessStartEvent = true;
                readerItem.m_ReaderAPI.Events.NotifyAccessStopEvent = true;
                readerItem.m_ReaderAPI.Events.NotifyInventoryStartEvent = true;
                readerItem.m_ReaderAPI.Events.NotifyInventoryStopEvent = true;

                try
                {
                    readerItem.m_ReaderAPI.Actions.Inventory.Perform(null, null, antennaInfo);
                }
                catch (OperationFailureException operationException)
                {
                    Console.WriteLine(operationException.Result);
                }

                if (readerItem.bool_ReconnectRequired)
                {
                    readerItem.bool_ReconnectRequired = false;

                    ReaderEmailObject readerObject = new ReaderEmailObject(readerItem.str_HostName, readerItem.str_IPAdress, readerItem.str_Name, DateTime.Now, "Reconnect successfully.");
                    ReaderEmailRCList.Add(readerObject);

                    if (!TimerSendRCEmail.Enabled)
                    {
                        TimerSendRCEmail.Start();
                    }
                }

                lv_reader.Items[rowIndex].SubItems[3].Text = "Connected";
                lv_reader.Items[rowIndex].ForeColor = Color.Green;
                btnConnect.Text = "Disconnect";
                btnConnect.BackColor = Color.Red;
            }
            else
            {
                lv_reader.Items[rowIndex].SubItems[3].Text = result;
                lv_reader.Items[rowIndex].ForeColor = Color.Red;

                ReaderEmailObject readerObject = new ReaderEmailObject(readerItem.str_HostName, readerItem.str_IPAdress, readerItem.str_Name, DateTime.Now, "Reconnect failed.");
                ReaderEmailFailList.Add(readerObject);

                if (!TimerSendFailEmail.Enabled)
                {
                    TimerSendFailEmail.Start();
                }

                btnConnect.Text = "Connect";
                btnConnect.BackColor = Color.Lime;
            }
        }

        private void AppForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Loading frm = new Loading();
            bg_FormClosing.RunWorkerAsync(frm);
            frm.ShowDialog();
        }

        private void btnConnectAll_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < lv_reader.Items.Count; i++)
                {
                    string passHostName = lv_reader.Items[i].SubItems[1].Text;
                    lv_reader.Items[i].SubItems[3].Text = "Connecting...";
                    lv_reader.Items[i].ForeColor = Color.Green;

                    BackgroundWorker worker = new BackgroundWorker();
                    worker.DoWork += ConnectBackgroundWorker_DoWork;
                    worker.RunWorkerCompleted += ConnectBackgroundWorker_RunWorkerCompleted;

                    worker.RunWorkerAsync(passHostName + "-" + i);
                }
            }
            catch (Exception ex)
            {
                lbl_error.Text = ex.Message;
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (lv_reader.SelectedItems.Count > 0)
            {
                try
                {
                    if (btnConnect.Text == "Connect")
                    {
                        int rowIndex = lv_reader.SelectedIndices[0];
                        string passHostName = lv_reader.Items[rowIndex].SubItems[1].Text;

                        lv_reader.Items[rowIndex].SubItems[3].Text = "Connecting...";
                        lv_reader.Items[rowIndex].ForeColor = Color.Green;

                        BackgroundWorker worker = new BackgroundWorker();
                        worker.DoWork += ConnectBackgroundWorker_DoWork;
                        worker.RunWorkerCompleted += ConnectBackgroundWorker_RunWorkerCompleted;

                        worker.RunWorkerAsync(passHostName + "-" + rowIndex);
                    }
                    else if (btnConnect.Text == "Disconnect")
                    {
                        int rowIndex = lv_reader.SelectedIndices[0];
                        string passHostName = lv_reader.Items[rowIndex].SubItems[1].Text;

                        lv_reader.Items[rowIndex].SubItems[3].Text = "Disconnecting...";
                        lv_reader.Items[rowIndex].ForeColor = Color.Red;

                        BackgroundWorker worker = new BackgroundWorker();
                        worker.DoWork += BackgroundWorkerDisconnectReader_DoWork;
                        worker.RunWorkerCompleted += BackgroundWorkerDisconnectReader_RunWorkerCompleted;

                        worker.RunWorkerAsync(passHostName + "-" + rowIndex);
                        btnConnect.Text = "Connect";
                    }
                }
                catch (Exception ex)
                {
                    lbl_error.Text = ex.Message;
                }
            }
        }

        private void BackgroundWorkerDisconnectReader_DoWork(object sender, DoWorkEventArgs e)
        {
            string[] arry = e.Argument.ToString().Split('-');
            RFIDReader currentReader = m_reader_list[arry[0]].m_ReaderAPI;

            returnResult result = new returnResult
            {
                rowIndex = Convert.ToInt32(arry[1])
            };

            if (currentReader.IsConnected)
            {
                try
                {
                    if (currentReader.Actions.TagAccess.OperationSequence.Length > 0)
                    {
                        currentReader.Actions.TagAccess.OperationSequence.StopSequence();
                        currentReader.Actions.Inventory.Stop();
                    }
                    else
                    {
                        currentReader.Actions.Inventory.Stop();
                    }

                    currentReader.Disconnect();
                }
                catch (Exception ex)
                {
                    _Error = ex.Message;
                }
            }
            else
            {
                result.SkipThisConnect = true;
            }

            e.Result = result;
        }

        private void BackgroundWorkerDisconnectReader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            returnResult returnResult = (returnResult)e.Result;
            int rowIndex = returnResult.rowIndex;
            string hostName = lv_reader.Items[rowIndex].SubItems[1].Text;
            RFIDReader currentReader = m_reader_list[hostName].m_ReaderAPI;

            if (returnResult.SkipThisConnect)
            {
                returnResult.SkipThisConnect = false;
            }

            lv_reader.Items[rowIndex].SubItems[3].Text = "Disconnect";
            lv_reader.Items[rowIndex].ForeColor = Color.Red;

            if (btnConnect.Text == "Connect")
            {
                btnConnect.BackColor = Color.Lime;
            }
            else
            {
                btnConnect.BackColor = Color.Red;
            }
        }

        private void btnDisconnectAll_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < lv_reader.Items.Count; i++)
                {
                    string passHostName = lv_reader.Items[i].SubItems[1].Text;
                    lv_reader.Items[i].SubItems[3].Text = "Disconnecting...";
                    lv_reader.Items[i].ForeColor = Color.Red;

                    BackgroundWorker worker = new BackgroundWorker();
                    worker.DoWork += BackgroundWorkerDisconnectReader_DoWork;
                    worker.RunWorkerCompleted += BackgroundWorkerDisconnectReader_RunWorkerCompleted;

                    worker.RunWorkerAsync(passHostName + "-" + i);
                }
            }
            catch (Exception ex)
            {
                lbl_error.Text = ex.Message;
            }
        }

        private void lv_reader_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lv_reader.SelectedItems.Count > 0)
            {
                try
                {
                    int rowIndex = lv_reader.SelectedIndices[0];
                    string hostName = lv_reader.Items[rowIndex].SubItems[1].Text;
                    RFIDReader currentReader = m_reader_list[hostName].m_ReaderAPI;

                    if (currentReader.IsConnected)
                    {
                        btnConnect.Text = "Disconnect";
                        btnConnect.BackColor = Color.Red;
                    }
                    else
                    {
                        btnConnect.Text = "Connect";
                        btnConnect.BackColor = Color.Lime;
                    }
                }
                catch (Exception ex)
                {
                    lbl_error.Text = ex.Message;
                }
            }
        }

        private void bg_GetRFIDConfig_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = e.Argument;
            dto = db.GET_RFID_CONFIG("FILM");
        }

        private void bg_GetRFIDConfig_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (dto.Error)
                {
                    throw new Exception(dto.ErrorMessage);
                }

                ListViewItem item;
                ListViewItem.ListViewSubItem subitem;
                int readerIndex = 0;

                foreach (DataRow row in dto.Table.Rows)
                {
                    _r_dtl = new RFID_Reader_Item();
                    _r_dtl.int_Index = readerIndex;
                    _r_dtl.m_ReaderAPI = new RFIDReader(
                        row["IP_ADDRESS"].ToString(),
                        Convert.ToUInt32(row["PORT"].ToString()),
                        50000);
                    _r_dtl.ht_TagDetected = new Hashtable();
                    _r_dtl.ht_TagTime = new Hashtable();
                    _r_dtl.str_Name = row["LOCATION"].ToString();
                    _r_dtl.str_IPAdress = row["IP_ADDRESS"].ToString();
                    _r_dtl.str_HostName = row["HOST_NAME"].ToString();
                    _r_dtl.str_Port = row["PORT"].ToString();
                    _r_dtl.str_Stored_Procedure = row["SP"].ToString();
                    _r_dtl.str_SERVER = row["SERVER"].ToString();
                    _r_dtl.str_Stored_Procedure2 = row["SP2"].ToString();
                    _r_dtl.str_SERVER2 = row["SERVER2"].ToString();

                    m_reader_list.Add(row["IP_ADDRESS"].ToString(), _r_dtl);

                    item = new ListViewItem(row["LOCATION"].ToString());

                    subitem = new ListViewItem.ListViewSubItem(item, row["IP_ADDRESS"].ToString());
                    item.SubItems.Add(subitem);

                    subitem = new ListViewItem.ListViewSubItem(item, row["HOST_NAME"].ToString());
                    item.SubItems.Add(subitem);

                    subitem = new ListViewItem.ListViewSubItem(item, "Disconnect");
                    item.SubItems.Add(subitem);

                    item.ForeColor = Color.Red;
                    lv_reader.Items.Add(item);
                    readerIndex++;
                }

                Reset_Table.Enabled = true;
                TimerReconnect.Start();

                Loading frm = e.Result as Loading;
                if (frm != null)
                {
                    frm.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                Loading frm = e.Result as Loading;
                if (frm != null)
                {
                    frm.Close();
                }
            }
        }

        private void bg_FormClosing_DoWork(object sender, DoWorkEventArgs e)
        {
            foreach (KeyValuePair<string, RFID_Reader_Item> item in m_reader_list)
            {
                RFIDReader currentReader = item.Value.m_ReaderAPI;

                if (currentReader.IsConnected)
                {
                    try
                    {
                        if (currentReader.Actions.TagAccess.OperationSequence.Length > 0)
                        {
                            currentReader.Actions.TagAccess.OperationSequence.StopSequence();
                            currentReader.Actions.Inventory.Stop();
                        }
                        else
                        {
                            currentReader.Actions.Inventory.Stop();
                        }
                    }
                    catch
                    {
                    }
                }
            }

            e.Result = e.Argument;
        }

        private void bg_FormClosing_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Loading frm = e.Result as Loading;
            if (frm != null)
            {
                frm.Close();
            }
        }

        private void GPIO_ligthing_Control(string lightOption, string hostName)
        {
            if (lightOption.ToLower().Trim().Equals("error"))
            {
                m_reader_list[hostName].m_ReaderAPI.Config.GPO[1].PortState = Symbol.RFID3.GPOs.GPO_PORT_STATE.TRUE;
                m_reader_list[hostName].m_ReaderAPI.Config.GPO[2].PortState = Symbol.RFID3.GPOs.GPO_PORT_STATE.FALSE;
                m_reader_list[hostName].m_ReaderAPI.Config.GPO[3].PortState = Symbol.RFID3.GPOs.GPO_PORT_STATE.FALSE;
            }
            else if (lightOption.ToLower().Trim().Equals("reset"))
            {
                m_reader_list[hostName].m_ReaderAPI.Config.GPO[1].PortState = Symbol.RFID3.GPOs.GPO_PORT_STATE.FALSE;
                m_reader_list[hostName].m_ReaderAPI.Config.GPO[2].PortState = Symbol.RFID3.GPOs.GPO_PORT_STATE.FALSE;
                m_reader_list[hostName].m_ReaderAPI.Config.GPO[3].PortState = Symbol.RFID3.GPOs.GPO_PORT_STATE.FALSE;
            }
            else
            {
                m_reader_list[hostName].m_ReaderAPI.Config.GPO[1].PortState = Symbol.RFID3.GPOs.GPO_PORT_STATE.FALSE;
                m_reader_list[hostName].m_ReaderAPI.Config.GPO[2].PortState = Symbol.RFID3.GPOs.GPO_PORT_STATE.TRUE;
                m_reader_list[hostName].m_ReaderAPI.Config.GPO[3].PortState = Symbol.RFID3.GPOs.GPO_PORT_STATE.FALSE;
            }
        }

        private void TowerLight_ON(int mode, string hostName)
        {
            System.Windows.Forms.Timer tmr = new System.Windows.Forms.Timer();
            tmr.Interval = 4500;
            tmr.Tag = hostName;
            tmr.Tick += TowerLight_Timer_Tick;

            if (mode == 1)
            {
                GPIO_ligthing_Control("ok", hostName);
            }
            else if (mode == 2)
            {
                GPIO_ligthing_Control("error", hostName);
            }

            tmr.Enabled = true;
        }

        private void TowerLight_Timer_Tick(object sender, EventArgs e)
        {
            System.Windows.Forms.Timer tmr = sender as System.Windows.Forms.Timer;
            string hostName = tmr.Tag?.ToString();

            GPIO_ligthing_Control("reset", hostName);
            tmr.Enabled = false;
            tmr.Dispose();
        }

        private void Reset_Table_Tick(object sender, EventArgs e)
        {
            List<string> toRemove;

            foreach (KeyValuePair<string, RFID_Reader_Item> item in m_reader_list)
            {
                toRemove = new List<string>();

                foreach (object vKey in item.Value.ht_TagTime.Keys)
                {
                    DateTime starttime = (DateTime)item.Value.ht_TagTime[vKey];
                    if (starttime.AddMinutes(2) < DateTime.UtcNow)
                    {
                        toRemove.Add(vKey.ToString());
                    }
                }

                foreach (string key in toRemove)
                {
                    item.Value.ht_TagTime.Remove(key);
                    item.Value.ht_TagDetected.Remove(key);
                }
            }
        }

        private void TimerSendDCEmail_Tick(object sender, EventArgs e)
        {
            string company = Properties.Settings.Default.COMPANY.ToString();
            string mailTo = Properties.Settings.Default.TEST_ENVIRONMENT
                ? "nurfarhanah@maxsys.com.my"
                : Properties.Settings.Default.READER_NOTIFICATION_MAILTO.ToString();
            string mailCc = Properties.Settings.Default.READER_NOTIFICATION_MAILCC.ToString();
            string mailBcc = Properties.Settings.Default.READER_NOTIFICATION_MAILBCC.ToString();

            EmailObject emailObject = new EmailObject(company, mailTo, mailCc, mailBcc, false, ReaderEmailDCList);

            if (!Mailer.SendDBEmailMSSQL(ref emailObject))
            {
                PendingEmailList.Add(emailObject);
            }

            ReaderEmailDCList.Clear();
            TimerSendDCEmail.Stop();
        }

        private void TimerSendRCEmail_Tick(object sender, EventArgs e)
        {
            string company = Properties.Settings.Default.COMPANY.ToString();
            string mailTo = Properties.Settings.Default.TEST_ENVIRONMENT
                ? "nurfarhanah@maxsys.com.my"
                : Properties.Settings.Default.READER_NOTIFICATION_MAILTO.ToString();
            string mailCc = Properties.Settings.Default.READER_NOTIFICATION_MAILCC.ToString();
            string mailBcc = Properties.Settings.Default.READER_NOTIFICATION_MAILBCC.ToString();

            EmailObject emailObject = new EmailObject(company, mailTo, mailCc, mailBcc, false, ReaderEmailRCList);

            if (!Mailer.SendDBEmailMSSQL(ref emailObject))
            {
                PendingEmailList.Add(emailObject);
            }

            ReaderEmailRCList.Clear();
            TimerSendRCEmail.Stop();
        }

        private void TimerSendFailEmail_Tick(object sender, EventArgs e)
        {
            string company = Properties.Settings.Default.COMPANY.ToString();
            string mailTo = Properties.Settings.Default.TEST_ENVIRONMENT
                ? "nurfarhanah@maxsys.com.my"
                : Properties.Settings.Default.READER_NOTIFICATION_MAILTO.ToString();
            string mailCc = Properties.Settings.Default.READER_NOTIFICATION_MAILCC.ToString();
            string mailBcc = Properties.Settings.Default.READER_NOTIFICATION_MAILBCC.ToString();

            EmailObject emailObject = new EmailObject(company, mailTo, mailCc, mailBcc, false, ReaderEmailFailList);

            if (!Mailer.SendDBEmailMSSQL(ref emailObject))
            {
                PendingEmailList.Add(emailObject);
            }

            ReaderEmailFailList.Clear();
            TimerSendFailEmail.Stop();
        }

        private void TimerSendTagEmail_Tick(object sender, EventArgs e)
        {
            string company = Properties.Settings.Default.COMPANY.ToString();
            string mailTo = Properties.Settings.Default.TEST_ENVIRONMENT
                ? "nurfarhanah@maxsys.com.my"
                : Properties.Settings.Default.READER_NOTIFICATION_MAILTO.ToString();
            string mailCc = Properties.Settings.Default.READER_NOTIFICATION_MAILCC.ToString();
            string mailBcc = Properties.Settings.Default.READER_NOTIFICATION_MAILBCC.ToString();

            int batchSize = 33;
            for (int i = 0; i < ReaderEmailTagList.Count; i += batchSize)
            {
                List<ReaderTagObject> batch = ReaderEmailTagList.Skip(i).Take(batchSize).ToList();
                EmailTagObject emailObject = new EmailTagObject(company, mailTo, mailCc, mailBcc, false, batch);

                if (!Mailer.SendDBTagEmailMSSQL(ref emailObject))
                {
                    PendingTagEmailList.Add(emailObject);
                }
            }

            ReaderEmailTagList.Clear();
            TimerSendTagEmail.Stop();
        }

        private void TimerTagReconnect_Tick(object sender, EventArgs e)
        {
            int rowIndex = 0;

            foreach (KeyValuePair<string, RFID_Reader_Item> kvp in m_reader_list)
            {
                RFID_Reader_Item reader = kvp.Value;
                if (reader.bool_ReconnectRequired)
                {
                    if (!ReconnectBackgroundWorker.IsBusy)
                    {
                        ReconnectBackgroundWorker.RunWorkerAsync(reader.str_IPAdress + "-" + rowIndex);
                    }
                }

                rowIndex++;
            }

            for (int i = 0; i < PendingTagEmailList.Count; i++)
            {
                EmailTagObject email = PendingTagEmailList[i];
                Mailer.SendDBTagEmailMSSQL(ref email);
                PendingTagEmailList[i] = email;
            }

            PendingTagEmailList = PendingTagEmailList.Where(n => n.SendFlag == false).ToList();
        }

        private void TimerReconnect_Tick(object sender, EventArgs e)
        {
            int rowIndex = 0;

            foreach (KeyValuePair<string, RFID_Reader_Item> kvp in m_reader_list)
            {
                RFID_Reader_Item reader = kvp.Value;
                if (reader.bool_ReconnectRequired)
                {
                    if (!ReconnectBackgroundWorker.IsBusy)
                    {
                        ReconnectBackgroundWorker.RunWorkerAsync(reader.str_IPAdress + "-" + rowIndex);
                    }
                }

                rowIndex++;
            }

            for (int i = 0; i < PendingEmailList.Count; i++)
            {
                EmailObject email = PendingEmailList[i];
                Mailer.SendDBEmailMSSQL(ref email);
                PendingEmailList[i] = email;
            }

            PendingEmailList = PendingEmailList.Where(n => n.SendFlag == false).ToList();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            dto = db.RFID_Common_MSSQL("", "A17000000000000000012991", "TestRdr", "127.0.0.1", "SP_TEST_RFID");

            if (!dto.Error)
            {
                MessageBox.Show("Connect MSSQL Success, Transaction Success.");
                TowerLight_ON(1, "10.28.92.50");
            }
            else
            {
                MessageBox.Show("Connect MSSQL Success, Transaction Fail.");
                TowerLight_ON(2, "10.28.92.50");
            }
        }

        private void ReconnectBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string[] arry = e.Argument.ToString().Split('-');
            RFID_Reader_Item readerItem = m_reader_list[arry[0]];
            bool success = false;
            returnResult result = new returnResult();

            try
            {
                readerItem.m_ReaderAPI.Reconnect();
                success = true;
            }
            catch (Exception ex)
            {
                try
                {
                    readerItem.m_ReaderAPI = new RFIDReader(readerItem.str_IPAdress, Convert.ToUInt32(readerItem.str_Port), 50000);
                    readerItem.m_ReaderAPI.Connect();
                    success = true;
                }
                catch
                {
                    ReaderEmailObject readerObject = new ReaderEmailObject(readerItem.str_HostName, readerItem.str_IPAdress, readerItem.str_Name, DateTime.Now, "Reconnect failed.");
                    ReaderEmailFailList.Add(readerObject);

                    if (!TimerSendFailEmail.Enabled)
                    {
                        TimerSendFailEmail.Start();
                    }

                    result.rowIndex = Convert.ToInt32(arry[1]);
                    result.Result = ex.Message;
                    e.Result = result;
                }
            }

            if (success)
            {
                result.rowIndex = Convert.ToInt32(arry[1]);
                result.Result = "Connect Succeed";
                e.Result = result;
            }
        }

        private void ReconnectBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            returnResult returnResult = (returnResult)e.Result;
            int rowIndex = returnResult.rowIndex;
            string result = returnResult.Result;
            string hostName = lv_reader.Items[rowIndex].SubItems[1].Text;
            RFID_Reader_Item readerItem = m_reader_list[hostName];
            ushort[] antennaList = new ushort[] { 1, 2 };
            AntennaInfo antennaInfo = new AntennaInfo(antennaList);

            if (result == "Connect Succeed")
            {
                if (!Properties.Settings.Default.TEST_ENVIRONMENT)
                {
                    readerItem.m_ReaderAPI.Events.ReadNotify += new ReadNotifyHandler(Events_ReadNotify);
                    readerItem.m_ReaderAPI.Events.AttachTagDataWithReadEvent = false;
                }

                readerItem.m_ReaderAPI.Events.StatusNotify += new StatusNotifyHandler(Events_StatusNotify);
                readerItem.m_ReaderAPI.Events.NotifyGPIEvent = true;
                readerItem.m_ReaderAPI.Events.NotifyBufferFullEvent = true;
                readerItem.m_ReaderAPI.Events.NotifyBufferFullWarningEvent = true;
                readerItem.m_ReaderAPI.Events.NotifyReaderDisconnectEvent = true;
                readerItem.m_ReaderAPI.Events.NotifyReaderExceptionEvent = true;
                readerItem.m_ReaderAPI.Events.NotifyAccessStartEvent = true;
                readerItem.m_ReaderAPI.Events.NotifyAccessStopEvent = true;
                readerItem.m_ReaderAPI.Events.NotifyInventoryStartEvent = true;
                readerItem.m_ReaderAPI.Events.NotifyInventoryStopEvent = true;

                try
                {
                    readerItem.m_ReaderAPI.Actions.Inventory.Perform(null, null, antennaInfo);
                }
                catch (OperationFailureException operationException)
                {
                    Console.WriteLine(operationException.Result);
                }

                readerItem.bool_ReconnectRequired = false;

                ReaderEmailObject readerObject = new ReaderEmailObject(readerItem.str_HostName, readerItem.str_IPAdress, readerItem.str_Name, DateTime.Now, "Reconnect successfully.");
                ReaderEmailRCList.Add(readerObject);

                if (!TimerSendRCEmail.Enabled)
                {
                    TimerSendRCEmail.Start();
                }

                lv_reader.Items[rowIndex].SubItems[3].Text = "Connected";
                lv_reader.Items[rowIndex].ForeColor = Color.Green;
                btnConnect.Text = "Disconnect";
                btnConnect.BackColor = Color.Red;
            }
            else
            {
                lv_reader.Items[rowIndex].SubItems[3].Text = "Reconnecting";
                lv_reader.Items[rowIndex].ForeColor = Color.Red;

                ReaderEmailObject readerObject = new ReaderEmailObject(readerItem.str_HostName, readerItem.str_IPAdress, readerItem.str_Name, DateTime.Now, "Connect failed. Reconnecting");
                ReaderEmailFailList.Add(readerObject);

                if (!TimerSendFailEmail.Enabled)
                {
                    TimerSendFailEmail.Start();
                }

                btnConnect.Text = "Connect";
                btnConnect.BackColor = Color.Lime;
            }
        }

        private void resetTimer3()
        {
            m_TagTable.Clear();
            Timer3.Enabled = false;
        }

        private void resetTimer2()
        {
            m_TagTable.Clear();
            Timer2.Enabled = false;
        }

        private void Timer2_Tick(object sender, EventArgs e)
        {
            resetTimer2();
        }

        private void Timer3_Tick(object sender, EventArgs e)
        {
            resetTimer3();
        }

        private void TimerTagLock_Tick(object sender, EventArgs e)
        {
            lockTag = string.Empty;
        }
    }

    public class TagDetails
    {
        public string hostName = string.Empty;
        public string location = string.Empty;
        public string antennaID = string.Empty;
        public string tagID = string.Empty;
        public DateTime tagTime;
    }

    public class RFID_Reader_Item
    {
        internal Form1.AccessOperationResult m_AccessOpResult;
        internal RFIDReader m_ReaderAPI;
        internal bool bool_ReconnectRequired = false;
        internal int int_Index;
        internal Hashtable ht_TagDetected;
        internal Hashtable ht_TagTime;

        internal string str_Name;
        internal string str_IPAdress;
        internal string str_HostName;
        internal string str_Port;

        internal string str_Stored_Procedure;
        internal string str_SERVER;
        internal string str_Stored_Procedure2;
        internal string str_SERVER2;
        internal int int_RSSI;
    }
}
