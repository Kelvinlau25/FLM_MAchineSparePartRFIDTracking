<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.lv_reader = New System.Windows.Forms.ListView()
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader2 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader3 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader4 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.btnConnectAll = New System.Windows.Forms.Button()
        Me.btnDisconnectAll = New System.Windows.Forms.Button()
        Me.btnConnect = New System.Windows.Forms.Button()
        Me.lbl_error = New System.Windows.Forms.Label()
        Me.bg_GetRFIDConfig = New System.ComponentModel.BackgroundWorker()
        Me.bg_FormClosing = New System.ComponentModel.BackgroundWorker()
        Me.Reset_Table = New System.Windows.Forms.Timer(Me.components)
        Me.Button1 = New System.Windows.Forms.Button()
        Me.TimerSendDCEmail = New System.Windows.Forms.Timer(Me.components)
        Me.TimerReconnect = New System.Windows.Forms.Timer(Me.components)
        Me.TimerTagReconnect = New System.Windows.Forms.Timer(Me.components)
        Me.TimerSendRCEmail = New System.Windows.Forms.Timer(Me.components)
        Me.TimerSendFailEmail = New System.Windows.Forms.Timer(Me.components)
        Me.TimerSendTagEmail = New System.Windows.Forms.Timer(Me.components)
        Me.ReconnectBackgroundWorker = New System.ComponentModel.BackgroundWorker()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.Timer2 = New System.Windows.Forms.Timer(Me.components)
        Me.Timer3 = New System.Windows.Forms.Timer(Me.components)
        Me.TimerTagLock = New System.Windows.Forms.Timer(Me.components)
        Me.SuspendLayout()
        '
        'lv_reader
        '
        Me.lv_reader.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lv_reader.BackColor = System.Drawing.SystemColors.Window
        Me.lv_reader.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1, Me.ColumnHeader2, Me.ColumnHeader3, Me.ColumnHeader4})
        Me.lv_reader.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lv_reader.FullRowSelect = True
        Me.lv_reader.GridLines = True
        Me.lv_reader.HideSelection = False
        Me.lv_reader.Location = New System.Drawing.Point(15, 15)
        Me.lv_reader.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.lv_reader.MultiSelect = False
        Me.lv_reader.Name = "lv_reader"
        Me.lv_reader.Size = New System.Drawing.Size(1214, 343)
        Me.lv_reader.TabIndex = 0
        Me.lv_reader.UseCompatibleStateImageBehavior = False
        Me.lv_reader.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "Location"
        Me.ColumnHeader1.Width = 150
        '
        'ColumnHeader2
        '
        Me.ColumnHeader2.Text = "IP Address"
        Me.ColumnHeader2.Width = 150
        '
        'ColumnHeader3
        '
        Me.ColumnHeader3.Text = "Reader Name"
        Me.ColumnHeader3.Width = 270
        '
        'ColumnHeader4
        '
        Me.ColumnHeader4.Text = "Connection Status"
        Me.ColumnHeader4.Width = 404
        '
        'btnConnectAll
        '
        Me.btnConnectAll.BackColor = System.Drawing.Color.Lime
        Me.btnConnectAll.Font = New System.Drawing.Font("Lucida Bright", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnConnectAll.Location = New System.Drawing.Point(19, 366)
        Me.btnConnectAll.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.btnConnectAll.Name = "btnConnectAll"
        Me.btnConnectAll.Size = New System.Drawing.Size(164, 45)
        Me.btnConnectAll.TabIndex = 1
        Me.btnConnectAll.Text = "Connect All"
        Me.btnConnectAll.UseVisualStyleBackColor = False
        '
        'btnDisconnectAll
        '
        Me.btnDisconnectAll.BackColor = System.Drawing.Color.Red
        Me.btnDisconnectAll.Font = New System.Drawing.Font("Lucida Bright", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnDisconnectAll.Location = New System.Drawing.Point(190, 366)
        Me.btnDisconnectAll.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.btnDisconnectAll.Name = "btnDisconnectAll"
        Me.btnDisconnectAll.Size = New System.Drawing.Size(164, 45)
        Me.btnDisconnectAll.TabIndex = 2
        Me.btnDisconnectAll.Text = "Disconnect All"
        Me.btnDisconnectAll.UseVisualStyleBackColor = False
        '
        'btnConnect
        '
        Me.btnConnect.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnConnect.BackColor = System.Drawing.Color.Lime
        Me.btnConnect.Font = New System.Drawing.Font("Lucida Bright", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnConnect.Location = New System.Drawing.Point(1066, 366)
        Me.btnConnect.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.btnConnect.Name = "btnConnect"
        Me.btnConnect.Size = New System.Drawing.Size(164, 45)
        Me.btnConnect.TabIndex = 3
        Me.btnConnect.Text = "Connect"
        Me.btnConnect.UseVisualStyleBackColor = False
        '
        'lbl_error
        '
        Me.lbl_error.AutoSize = True
        Me.lbl_error.ForeColor = System.Drawing.Color.Red
        Me.lbl_error.Location = New System.Drawing.Point(15, 430)
        Me.lbl_error.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lbl_error.Name = "lbl_error"
        Me.lbl_error.Size = New System.Drawing.Size(54, 17)
        Me.lbl_error.TabIndex = 4
        Me.lbl_error.Text = "lblError"
        '
        'bg_GetRFIDConfig
        '
        '
        'bg_FormClosing
        '
        '
        'Reset_Table
        '
        Me.Reset_Table.Interval = 10000
        '
        'Button1
        '
        Me.Button1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Button1.Font = New System.Drawing.Font("Lucida Bright", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button1.Location = New System.Drawing.Point(880, 366)
        Me.Button1.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(164, 45)
        Me.Button1.TabIndex = 7
        Me.Button1.Text = "Testing"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'TimerSendDCEmail
        '
        Me.TimerSendDCEmail.Interval = 30000
        '
        'TimerReconnect
        '
        Me.TimerReconnect.Interval = 5000
        '
        Me.TimerTagReconnect.Interval = 5000
        '
        '
        'TimerSendRCEmail
        '
        Me.TimerSendRCEmail.Interval = 30000
        '
        'TimerSendFailEmail
        '
        Me.TimerSendFailEmail.Interval = 30000
        '
        'TimerSendTagEmail
        '
        Me.TimerSendTagEmail.Interval = 30000
        '
        'ReconnectBackgroundWorker
        '
        '
        'Timer1
        '
        Me.Timer1.Interval = 10000
        '
        'Timer2
        '
        Me.Timer2.Interval = 180000
        '
        'Timer3
        '
        Me.Timer3.Interval = 180000
        '
        'Tag Lock timer
        Me.TimerTagLock.Interval = 180000

        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(120.0!, 120.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(1245, 430)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.lbl_error)
        Me.Controls.Add(Me.btnConnect)
        Me.Controls.Add(Me.btnDisconnectAll)
        Me.Controls.Add(Me.btnConnectAll)
        Me.Controls.Add(Me.lv_reader)
        Me.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.Name = "Form1"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Film RFID Reader"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lv_reader As ListView
    Friend WithEvents ColumnHeader1 As ColumnHeader
    Friend WithEvents ColumnHeader2 As ColumnHeader
    Friend WithEvents ColumnHeader3 As ColumnHeader
    Friend WithEvents ColumnHeader4 As ColumnHeader
    Friend WithEvents btnConnectAll As Button
    Friend WithEvents btnDisconnectAll As Button
    Friend WithEvents btnConnect As Button
    Friend WithEvents lbl_error As Label
    Friend WithEvents bg_GetRFIDConfig As System.ComponentModel.BackgroundWorker
    Friend WithEvents bg_FormClosing As System.ComponentModel.BackgroundWorker
    Friend WithEvents Reset_Table As Timer
    Friend WithEvents Button1 As Button
    Friend WithEvents TimerSendDCEmail As Timer
    Friend WithEvents TimerReconnect As Timer
    Friend WithEvents TimerTagReconnect As Timer
    Friend WithEvents TimerSendRCEmail As Timer
    Friend WithEvents TimerSendFailEmail As Timer
    Friend WithEvents TimerSendTagEmail As Timer
    Friend WithEvents ReconnectBackgroundWorker As System.ComponentModel.BackgroundWorker
    Friend WithEvents Timer1 As Timer
    Friend WithEvents Timer2 As Timer
    Friend WithEvents Timer3 As Timer
    Friend WithEvents TimerTagLock As Timer
End Class
