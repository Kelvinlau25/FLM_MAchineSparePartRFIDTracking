using Microsoft.AspNet.SignalR.Client;
using Symbol.RFID3;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PAB_New_Aquarium_RFID
{
    public partial class RFID : Form
    {
        private IHubProxy hubProxy;
        private HubConnection hubConnection;
        private RFIDReader reader;
        private ListView listView1;
        private ListView listView2;
        private string rfidConnectionId = "";
        private string connectionString = "";
        private string hubUrl = "";
        private string dev = "";
        private System.Timers.Timer checkTimer;
        private System.Timers.Timer retryTimer;
        List<ConnectionRFID> connectionList = new List<ConnectionRFID>();

        public class ConnectionRFID
        {
            public string WebConnectionID { get; set; }
        }

        public RFID()
        {
            InitializeComponent();
            InitializeListView();
            InitializeEnvironment();
            InitializeSignalRConnection();
        }

        private void InitializeListView()
        {
            listView1 = new ListView();
            listView1.Location = new Point(10, 10);
            listView1.Size = new Size(400, 100);
            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.Columns.Add("No", 50);
            listView1.Columns.Add("Reader Name", 100);
            listView1.Columns.Add("Web Connection ID", 250);

            listView2 = new ListView();
            listView2.Dock = DockStyle.Right;
            listView2.Size = new Size(300, ClientSize.Height);
            listView2.View = View.Details;
            listView2.FullRowSelect = true;
            listView2.GridLines = true;
            listView2.Columns.Add("No", 50);
            listView2.Columns.Add("Tag ID", 200);
            listView2.Columns.Add("RSSI", 50);

            Controls.Add(listView1);
            Controls.Add(listView2);
        }

        private void InitializeEnvironment()
        {
            dev = ConfigurationSettings.AppSettings["DEV"];
            if (dev == "LIVE")
            {
                hubUrl = "http://cld-pab-app001.toray.my:191/signalr";
                connectionString = ConfigurationSettings.AppSettings["LIVE_DB"];
            }
            else
            {
                hubUrl = "http://10.200.0.81:812/signalr";
                connectionString = ConfigurationSettings.AppSettings["TEST_DB"];
            }
        }

        private void InitializeSignalRConnection()
        {
            try
            {
                hubConnection = new HubConnection(hubUrl);
                hubProxy = hubConnection.CreateHubProxy("SignalRHub");
                hubConnection.Start().ContinueWith(task =>
                {
                    rfidConnectionId = hubConnection.ConnectionId;
                }).Wait();

                #region Check Connection
                if (hubConnection.State == Microsoft.AspNet.SignalR.Client.ConnectionState.Connected)
                {
                    SendConnectionIDtoWebApp(rfidConnectionId);
                    InitializeRFIDConnection();
                    CheckConnectionTimer();
                }
                else
                {
                    retryTimer = new System.Timers.Timer(30000);
                    retryTimer.Elapsed += (sender, e) => RetryConnection();
                    retryTimer.Start();
                }
                #endregion

                #region Display Connection ID
                if (FormLabel.IsHandleCreated)
                {
                    if (rfidConnectionId == null)
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            FormLabel.Text = "Hub is disconnect";
                        });
                    }
                    else
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            FormLabel.Text = "Connection ID: " + rfidConnectionId;
                        });
                    }
                }
                else
                {
                    if (rfidConnectionId == null)
                    {
                        FormLabel.Text = "Hub is disconnect";
                    }
                    else
                    {
                        FormLabel.Text = "Connection ID: " + rfidConnectionId;
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void InitializeRFIDConnection()
        {
            hubProxy.On<string, string, string>("connectRFID", (webConnectionID, readerName, readerSignal) =>
            {
                bool rfidExists = connectionList.Count != 0;

                if (rfidExists)
                {
                    hubProxy.Invoke("ReceiveMessage", webConnectionID, "RFID reader has been used by other page. Please close it before continue.");
                }
                else
                {
                    ConnectionRFID connection = new ConnectionRFID
                    {
                        WebConnectionID = webConnectionID,
                    };
                    connectionList.Add(connection);

                    InitializeRFIDReader(webConnectionID, readerName, readerSignal, "Connect");

                    #region Developer display
                    if (dev == "TEST")
                    {
                        ListViewItem existingItem = null;
                        int index = listView1.Items.Count + 1;

                        if (listView1.InvokeRequired)
                        {
                            listView1.Invoke(new MethodInvoker(delegate {
                                existingItem = listView1.FindItemWithText(readerName);
                            }));
                            if (existingItem == null)
                            {
                                ListViewItem newItem = new ListViewItem(new[] { index.ToString(), readerName, webConnectionID });

                                listView1.Invoke(new MethodInvoker(delegate {
                                    listView1.Items.Add(newItem);
                                }));
                            }
                        }
                        else
                        {
                            existingItem = listView1.FindItemWithText(readerName);
                            if (existingItem == null)
                            {
                                ListViewItem newItem = new ListViewItem(new[] { index.ToString(), readerName, webConnectionID });
                                listView1.Items.Add(newItem);
                            }
                        }
                    }
                    #endregion
                }
            });

            hubProxy.On<string>("disconnectRFID", (webConnectionID) =>
            {
                bool rfidExists = connectionList.Count != 0;

                if (rfidExists)
                {
                    for (int i = 0; i < connectionList.Count; i++)
                    {
                        if (connectionList[i].WebConnectionID == webConnectionID)
                        {
                            InitializeRFIDReader(webConnectionID, "", "", "Disconnect");
                            if (connectionList.Count != 0)
                            {
                                connectionList.RemoveAt(i);
                            }

                            #region Developer display
                            if (dev == "TEST")
                            {
                                ListViewItem existingItem = null;

                                listView1.Invoke(new MethodInvoker(delegate {
                                    existingItem = listView1.FindItemWithText(webConnectionID);
                                }));

                                if (existingItem != null)
                                {
                                    listView1.Invoke(new MethodInvoker(delegate {
                                        listView1.Items.Remove(existingItem);
                                    }));
                                }
                            }
                            #endregion
                            break;
                        }
                    }
                }
            });
        }

        private void InitializeRFIDReader(string webConnectionID, string readerName, string readerSignal, string connect)
        {
            try
            {
                if (connect == "Connect")
                {
                    string readerBrand = "";
                    string readerIP = "";
                    string readerPort = "";

                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("PSP_GET_RFID_CONFIG", con))
                        {
                            con.Open();
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandTimeout = 0;
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter("@READER_NAME", readerName)).Direction = ParameterDirection.Input;
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    readerBrand = reader["READER_BRAND"].ToString();
                                    readerIP = reader["READER_IP"].ToString();
                                    readerPort = reader["READER_PORT"].ToString();
                                }
                            }
                        }
                    }

                    reader = new RFIDReader(readerIP, uint.Parse(readerPort), 0);
                    reader.Connect();
                    reader.Events.ReadNotify += (sender, e) => Events_ReadNotify(sender, e, webConnectionID, readerSignal);
                    reader.Events.AttachTagDataWithReadEvent = true;
                    reader.Actions.PurgeTags();
                    reader.Actions.Inventory.Perform();
                    hubProxy.Invoke("ReceiveMessage", webConnectionID, "RFID reader is connected.");
                }
                else
                {
                    reader.Actions.Inventory.Stop();
                    reader.Dispose();
                }
            }
            catch (Exception ex)
            {
                for (int i = 0; i < connectionList.Count; i++)
                {
                    if (connectionList[i].WebConnectionID == webConnectionID)
                    {
                        connectionList.RemoveAt(i);
                        break;
                    }
                }
                hubProxy.Invoke("ReceiveMessage", webConnectionID, "RFID reader is disconnected.");
                throw ex;
            }
        }

        private void Events_ReadNotify(object sender, Events.ReadEventArgs e, string webConnectionID, string readerSignal)
        {
            try
            {
                TagData[] tagData = reader.Actions.GetReadTags(1000);

                if (tagData != null)
                {
                    var filterTag = tagData.Where(tag => tag.PeakRSSI >= sbyte.Parse(readerSignal)).GroupBy(tag => tag.TagID);

                    foreach (var item in filterTag)
                    {
                        hubProxy.Invoke("GetTags", webConnectionID, item.Key);
                    }

                    #region Developer display
                    if (dev == "TEST")
                    {
                        foreach (var item in tagData)
                        {
                            ListViewItem existingItem = null;
                            int index = listView2.Items.Count + 1;

                            if (listView2.InvokeRequired)
                            {
                                listView2.Invoke(new MethodInvoker(delegate {
                                    existingItem = listView2.FindItemWithText(item.TagID);
                                }));
                                if (existingItem != null)
                                {
                                    listView2.Invoke(new MethodInvoker(delegate {
                                        existingItem.SubItems[2].Text = item.PeakRSSI.ToString();
                                    }));

                                }
                                else
                                {
                                    ListViewItem newItem = new ListViewItem(new[] { index.ToString(), item.TagID, item.PeakRSSI.ToString() });

                                    listView2.Invoke(new MethodInvoker(delegate {
                                        listView2.Items.Add(newItem);
                                    }));
                                }
                            }
                            else
                            {
                                existingItem = listView2.FindItemWithText(item.TagID);
                                if (existingItem != null)
                                {
                                    existingItem.SubItems[2].Text = item.PeakRSSI.ToString();
                                }
                                else
                                {
                                    ListViewItem newItem = new ListViewItem(new[] { index.ToString(), item.TagID, item.PeakRSSI.ToString() });
                                    listView2.Items.Add(newItem);
                                }
                            }
                        }
                    }
                    #endregion
                }
            }
            catch (InvalidUsageException ex)
            {
                throw ex;
            }
            catch (OperationFailureException ex)
            {
                throw ex;
            }
        }

        private void SendConnectionIDtoWebApp(string rfidConnectionId)
        {
            if (rfidConnectionId != null)
            {
                hubProxy.Invoke("ReceiveClientConnectionID", rfidConnectionId);
            }
        }

        private void CheckConnectionTimer()
        {
            checkTimer = new System.Timers.Timer(30000);
            checkTimer.Elapsed += (sender, e) => CheckConnection();
            checkTimer.Start();
        }

        private void CheckConnection()
        {
            if (hubConnection.State == Microsoft.AspNet.SignalR.Client.ConnectionState.Connected)
            {
                hubProxy.Invoke("ReceiveClientConnectionID", rfidConnectionId);
            }
            else if (hubConnection.State == Microsoft.AspNet.SignalR.Client.ConnectionState.Disconnected)
            {
                checkTimer.Stop();
                InitializeSignalRConnection();
            }
        }

        private void RetryConnection()
        {
            retryTimer.Stop();
            InitializeSignalRConnection();
        }

        private void FormButton_Click(object sender, EventArgs e)
        {
            Hide();
        }
    }
}
