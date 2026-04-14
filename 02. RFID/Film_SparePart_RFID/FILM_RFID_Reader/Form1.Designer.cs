using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace FILM_RFID_READER_DEPLOY
{
    partial class Form1
    {
        private IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new Container();
            this.lv_reader = new ListView();
            this.ColumnHeader1 = new ColumnHeader();
            this.ColumnHeader2 = new ColumnHeader();
            this.ColumnHeader3 = new ColumnHeader();
            this.ColumnHeader4 = new ColumnHeader();
            this.btnConnectAll = new Button();
            this.btnDisconnectAll = new Button();
            this.btnConnect = new Button();
            this.lbl_error = new Label();
            this.bg_GetRFIDConfig = new BackgroundWorker();
            this.bg_FormClosing = new BackgroundWorker();
            this.Reset_Table = new Timer(this.components);
            this.Button1 = new Button();
            this.TimerSendDCEmail = new Timer(this.components);
            this.TimerReconnect = new Timer(this.components);
            this.TimerTagReconnect = new Timer(this.components);
            this.TimerSendRCEmail = new Timer(this.components);
            this.TimerSendFailEmail = new Timer(this.components);
            this.TimerSendTagEmail = new Timer(this.components);
            this.ReconnectBackgroundWorker = new BackgroundWorker();
            this.Timer1 = new Timer(this.components);
            this.Timer2 = new Timer(this.components);
            this.Timer3 = new Timer(this.components);
            this.TimerTagLock = new Timer(this.components);
            this.SuspendLayout();
            // 
            // lv_reader
            // 
            this.lv_reader.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.lv_reader.BackColor = SystemColors.Window;
            this.lv_reader.Columns.AddRange(new ColumnHeader[] {
                this.ColumnHeader1,
                this.ColumnHeader2,
                this.ColumnHeader3,
                this.ColumnHeader4
            });
            this.lv_reader.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.lv_reader.FullRowSelect = true;
            this.lv_reader.GridLines = true;
            this.lv_reader.HideSelection = false;
            this.lv_reader.Location = new Point(15, 15);
            this.lv_reader.Margin = new Padding(4, 4, 4, 4);
            this.lv_reader.MultiSelect = false;
            this.lv_reader.Name = "lv_reader";
            this.lv_reader.Size = new Size(1214, 343);
            this.lv_reader.TabIndex = 0;
            this.lv_reader.UseCompatibleStateImageBehavior = false;
            this.lv_reader.View = View.Details;
            // 
            // ColumnHeader1
            // 
            this.ColumnHeader1.Text = "Location";
            this.ColumnHeader1.Width = 150;
            // 
            // ColumnHeader2
            // 
            this.ColumnHeader2.Text = "IP Address";
            this.ColumnHeader2.Width = 150;
            // 
            // ColumnHeader3
            // 
            this.ColumnHeader3.Text = "Reader Name";
            this.ColumnHeader3.Width = 270;
            // 
            // ColumnHeader4
            // 
            this.ColumnHeader4.Text = "Connection Status";
            this.ColumnHeader4.Width = 404;
            // 
            // btnConnectAll
            // 
            this.btnConnectAll.BackColor = Color.Lime;
            this.btnConnectAll.Font = new Font("Lucida Bright", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.btnConnectAll.Location = new Point(19, 366);
            this.btnConnectAll.Margin = new Padding(4, 4, 4, 4);
            this.btnConnectAll.Name = "btnConnectAll";
            this.btnConnectAll.Size = new Size(164, 45);
            this.btnConnectAll.TabIndex = 1;
            this.btnConnectAll.Text = "Connect All";
            this.btnConnectAll.UseVisualStyleBackColor = false;
            // 
            // btnDisconnectAll
            // 
            this.btnDisconnectAll.BackColor = Color.Red;
            this.btnDisconnectAll.Font = new Font("Lucida Bright", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.btnDisconnectAll.Location = new Point(190, 366);
            this.btnDisconnectAll.Margin = new Padding(4, 4, 4, 4);
            this.btnDisconnectAll.Name = "btnDisconnectAll";
            this.btnDisconnectAll.Size = new Size(164, 45);
            this.btnDisconnectAll.TabIndex = 2;
            this.btnDisconnectAll.Text = "Disconnect All";
            this.btnDisconnectAll.UseVisualStyleBackColor = false;
            // 
            // btnConnect
            // 
            this.btnConnect.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.btnConnect.BackColor = Color.Lime;
            this.btnConnect.Font = new Font("Lucida Bright", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.btnConnect.Location = new Point(1066, 366);
            this.btnConnect.Margin = new Padding(4, 4, 4, 4);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new Size(164, 45);
            this.btnConnect.TabIndex = 3;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = false;
            // 
            // lbl_error
            // 
            this.lbl_error.AutoSize = true;
            this.lbl_error.ForeColor = Color.Red;
            this.lbl_error.Location = new Point(15, 430);
            this.lbl_error.Margin = new Padding(4, 0, 4, 0);
            this.lbl_error.Name = "lbl_error";
            this.lbl_error.Size = new Size(54, 17);
            this.lbl_error.TabIndex = 4;
            this.lbl_error.Text = "lblError";
            // 
            // Reset_Table
            // 
            this.Reset_Table.Interval = 10000;
            // 
            // Button1
            // 
            this.Button1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.Button1.Font = new Font("Lucida Bright", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.Button1.Location = new Point(880, 366);
            this.Button1.Margin = new Padding(4, 4, 4, 4);
            this.Button1.Name = "Button1";
            this.Button1.Size = new Size(164, 45);
            this.Button1.TabIndex = 7;
            this.Button1.Text = "Testing";
            this.Button1.UseVisualStyleBackColor = true;
            // 
            // TimerSendDCEmail
            // 
            this.TimerSendDCEmail.Interval = 30000;
            // 
            // TimerReconnect
            // 
            this.TimerReconnect.Interval = 5000;
            // 
            // TimerTagReconnect
            // 
            this.TimerTagReconnect.Interval = 5000;
            // 
            // TimerSendRCEmail
            // 
            this.TimerSendRCEmail.Interval = 30000;
            // 
            // TimerSendFailEmail
            // 
            this.TimerSendFailEmail.Interval = 30000;
            // 
            // TimerSendTagEmail
            // 
            this.TimerSendTagEmail.Interval = 30000;
            // 
            // Timer1
            // 
            this.Timer1.Interval = 10000;
            // 
            // Timer2
            // 
            this.Timer2.Interval = 180000;
            // 
            // Timer3
            // 
            this.Timer3.Interval = 180000;
            // 
            // TimerTagLock
            // 
            this.TimerTagLock.Interval = 180000;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new SizeF(120F, 120F);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.ClientSize = new Size(1245, 430);
            this.Controls.Add(this.Button1);
            this.Controls.Add(this.lbl_error);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.btnDisconnectAll);
            this.Controls.Add(this.btnConnectAll);
            this.Controls.Add(this.lv_reader);
            this.Margin = new Padding(4, 4, 4, 4);
            this.Name = "Form1";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Film RFID Reader";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        internal ListView lv_reader;
        internal ColumnHeader ColumnHeader1;
        internal ColumnHeader ColumnHeader2;
        internal ColumnHeader ColumnHeader3;
        internal ColumnHeader ColumnHeader4;
        internal Button btnConnectAll;
        internal Button btnDisconnectAll;
        internal Button btnConnect;
        internal Label lbl_error;
        internal BackgroundWorker bg_GetRFIDConfig;
        internal BackgroundWorker bg_FormClosing;
        internal Timer Reset_Table;
        internal Button Button1;
        internal Timer TimerSendDCEmail;
        internal Timer TimerReconnect;
        internal Timer TimerTagReconnect;
        internal Timer TimerSendRCEmail;
        internal Timer TimerSendFailEmail;
        internal Timer TimerSendTagEmail;
        internal BackgroundWorker ReconnectBackgroundWorker;
        internal Timer Timer1;
        internal Timer Timer2;
        internal Timer Timer3;
        internal Timer TimerTagLock;
    }
}