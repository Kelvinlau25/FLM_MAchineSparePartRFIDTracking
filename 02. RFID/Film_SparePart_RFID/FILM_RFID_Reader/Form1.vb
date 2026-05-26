Imports Symbol.RFID3
Imports Symbol.RFID3.Events
Imports Symbol.RFID3.GPI_PORT_STATE
Imports Symbol.RFID3.GPIs
Imports Symbol.RFID3.GPOs
Imports System
Imports System.ComponentModel
Imports System.Threading
Imports Library.Reader.Objects
Imports Library.Reader.Helpers

Public Class Form1
    Inherits Form
    Dim lockTag As String = "" 'HANA 220324 Creating lock for tag
    'Added by Teng Yong Ping on 12/11/2024 to auto connect all when app executed
    Private btnConnectAllTimer As Timer

    Public Sub New()
        Try
            InitializeComponent()

            Me.m_UpdateStatusHandler = New UpdateStatus(AddressOf Me.myUpdateStatus)
            Me.m_UpdateReadHandler = New UpdateRead(AddressOf Me.myUpdateRead)

            Me.m_TagTable = New Hashtable
            Me.m_TagTotalCount = 0

            'txt_log.AppendText($"{Date.Now.ToShortTimeString()} - Application Started")
            'txt_log.ScrollToCaret()

            Dim frm As New Loading
            Me.bg_GetRFIDConfig.RunWorkerAsync(frm)
            frm.ShowDialog()

            'Added by Teng Yong Ping on 12/11/2024 to auto connect all when app executed
            btnConnectAllTimer = New System.Threading.Timer(AddressOf TimerCallback, Nothing, 1000, Timeout.Infinite)

            logger._LogGen("New() >> Init done")
        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString)
        End Try


    End Sub

    'Added by Teng Yong Ping on 12/11/2024 to auto connect all when app executed
    Private Sub TimerCallback(state As Object)
        ' Make sure the button click runs on the UI thread
        If Me.InvokeRequired Then
            Me.BeginInvoke(New Action(Sub() btnConnectAll_Click(Me, EventArgs.Empty)))
        Else
            btnConnectAll_Click(Me, EventArgs.Empty)
        End If

        ' Dispose of the timer as we only need it once
        btnConnectAllTimer.Dispose()
    End Sub

    Private Class returnResult
        Friend rowIndex As Integer
        Friend Result As String
        Friend SkipThisConnect As Boolean
    End Class

    Dim _r_dtl As RFID_Reader_Item
    Private m_reader_list As New Dictionary(Of String, RFID_Reader_Item)
    Private selected_reader As String = String.Empty
    Friend m_ReaderAPI As RFIDReader

    Private m_ReadTag As TagData = Nothing
    Friend m_SelectedTagID As String = Nothing
    Private m_TagTable As Hashtable
    Private m_TagTotalCount As UInt32
    Private m_LastSent As DateTime = New DateTime()
    Private m_UpdateReadHandler As UpdateRead = Nothing
    Private m_UpdateStatusHandler As UpdateStatus = Nothing
    Private m_emptyTagTime As DateTime
    Private m_ReaderName As String

    Private PendingEmailList As New List(Of EmailObject)
    'HANA 09112023 Adding email for tag
    Private PendingTagEmailList As New List(Of EmailTagObject)
    Private ReaderEmailDCList As New List(Of ReaderEmailObject)
    Private ReaderEmailRCList As New List(Of ReaderEmailObject)
    'HANA 09112023 Adding email for failed connection
    Private ReaderEmailFailList As New List(Of ReaderEmailObject)
    'HANA 10012024 Adding email for tag
    Private ReaderEmailTagList As New List(Of ReaderTagObject)

    'db
    Dim dto As New Library.Database.DTO
    Dim db As New Library.Database.RFID_COMMON
    Dim mode As String = "NEW"

    'Tower Light
    Private int_reset As Integer
    Private int_towerlight_reset As Integer
    Private int_towerlight_current As Integer
    Private int_towerlight_mode As Integer  ' 1 - ok , 2 - error 

    'for log
    Private logger As New FILM_RFID_READER_DEPLOY.logger

    Dim loopCoil1 As String = ""
    Dim loopCoil2 As String = ""
    Dim hostn As String = ""
    Dim tower As String = ""


    'To Handle Data Passing/Error
    Private _Error As String = String.Empty

    Private Delegate Sub UpdateRead(ByVal HostName As String, ByVal eventData As ReadEventData)

    Private Delegate Sub UpdateStatus(ByVal hostName As String, ByVal eventData As StatusEventData)

    Friend Class AccessOperationResult
        ' Fields
        Public m_OpCode As ACCESS_OPERATION_CODE = ACCESS_OPERATION_CODE.ACCESS_OPERATION_READ
        Public m_VendorMessage As String = String.Empty
        Public m_StatusDescription As String = String.Empty
        Public m_Result As RFIDResults = RFIDResults.RFID_NO_ACCESS_IN_PROGRESS
    End Class

    Private Sub myUpdateStatus(ByVal HostName As String, ByVal eventData As Events.StatusEventData)

        Dim StatusMsg As String = ""
        Dim running As Integer = 0
        Dim reader As RFID_Reader_Item = m_reader_list(HostName)

        Select Case eventData.StatusEventType
            Case Symbol.RFID3.Events.STATUS_EVENT_TYPE.INVENTORY_START_EVENT
                StatusMsg = "Inventory started"
                Exit Select
            Case Symbol.RFID3.Events.STATUS_EVENT_TYPE.INVENTORY_STOP_EVENT
                StatusMsg = "Inventory stopped"
                Exit Select
            Case Symbol.RFID3.Events.STATUS_EVENT_TYPE.ACCESS_START_EVENT
                StatusMsg = "Access Operation started"
                Exit Select
            Case Symbol.RFID3.Events.STATUS_EVENT_TYPE.ACCESS_STOP_EVENT
                StatusMsg = "Access Operation stopped"

                'If Me.m_SelectedTagID = String.Empty Then
                '    Dim successCount As UInteger, failureCount As UInteger
                '    successCount = 0
                '    failureCount = 0
                '    m_ReaderAPI.Actions.TagAccess.GetLastAccessResult(successCount, failureCount)
                '    Me.functionCallStatusLabel.Text = String.Concat(New String() {"Access completed - Successs Count: ", successCount.ToString, ", Failure Count: ", failureCount.ToString})
                'End If
                Exit Select
            Case Symbol.RFID3.Events.STATUS_EVENT_TYPE.BUFFER_FULL_WARNING_EVENT
                StatusMsg = " Buffer full warning"
                'myUpdateRead(HostName, Nothing)
                Exit Select
            Case Symbol.RFID3.Events.STATUS_EVENT_TYPE.BUFFER_FULL_EVENT
                StatusMsg = "Buffer full"
                'myUpdateRead(HostName, Nothing)
                Exit Select
            Case Symbol.RFID3.Events.STATUS_EVENT_TYPE.DISCONNECTION_EVENT
                StatusMsg = "Disconnection Event " & eventData.DisconnectionEventData.DisconnectEventInfo.ToString()
                'connectBackgroundWorker.RunWorkerAsync("Disconnect")
                'sendErrorMSG(functionCallStatusLabel.Text)
                'Application.Restart()
                Exit Select
            Case Symbol.RFID3.Events.STATUS_EVENT_TYPE.ANTENNA_EVENT
                StatusMsg = "Antenna Status Update"
                Exit Select
            Case Symbol.RFID3.Events.STATUS_EVENT_TYPE.NXP_EAS_ALARM_EVENT
                Exit Select
            'Case Symbol.RFID3.Events.STATUS_EVENT_TYPE.TEMPERATURE_ALARM_EVENT
            '    StatusMsg = String.Concat(New String() {"Temperature Alarm ", eventData.TemperatureAlarmEventData.SourceName.ToString, " Temperature ", eventData.TemperatureAlarmEventData.CurrentTemperature.ToString, " Level ", eventData.TemperatureAlarmEventData.AlarmLevel.ToString})
            '    'sendErrorMSG(Me.functionCallStatusLabel.Text)
            '    Exit Select
            Case Symbol.RFID3.Events.STATUS_EVENT_TYPE.READER_EXCEPTION_EVENT
                StatusMsg = "Reader ExceptionEvent " & eventData.ReaderExceptionEventData.ReaderExceptionEventInfo
                'sendErrorMSG(functionCallStatusLabel.Text)
                Exit Select
                'add for loopcoil 
            Case Symbol.RFID3.Events.STATUS_EVENT_TYPE.GPI_EVENT
                'Event triggered when loopcoil detect Forklift presence
                'logger._LogTriggerGPI("CHECK", eventData.GPIEventData.PortNumber.ToString(), HostName, eventData.GPIEventData.GPIEvent)

                If Timer1.Enabled = True Then
                    check2ndloop("CHECK", eventData.GPIEventData.PortNumber.ToString(), HostName, eventData.GPIEventData.GPIEvent, 0)
                Else
                    updateIn("CHECK", eventData.GPIEventData.PortNumber.ToString(), HostName, eventData.GPIEventData.GPIEvent, 0)
                End If


                'If Timer2.Enabled Then
                '    check2ndloopout("CHECK", eventData.GPIEventData.PortNumber.ToString(), HostName, eventData.GPIEventData.GPIEvent, 0)
                'Else
                '    updateOut("CHECK", eventData.GPIEventData.PortNumber.ToString(), HostName, eventData.GPIEventData.GPIEvent, 0)
                'End If
                'If m_reader_list(HostName).m_ReaderAPI.Config.GPI.Item(1).PortState = 0 Then
                '    updateIn("CHECK", eventData.GPIEventData.PortNumber.ToString(), HostName, eventData.GPIEventData.GPIEvent, 0)
                'End If

                'If m_reader_list(HostName).m_ReaderAPI.Config.GPI.Item(2).PortState = 0 Then
                '    updateOut("CHECK", eventData.GPIEventData.PortNumber.ToString(), HostName, eventData.GPIEventData.GPIEvent, 0)
                'End If

                Exit Select
            Case Else
                StatusMsg = "Unhandled Status"
                Exit Select

        End Select

        If Not reader.m_ReaderAPI.IsConnected Then

            reader.bool_ReconnectRequired = True

            If Not ReconnectBackgroundWorker.IsBusy Then
                ReconnectBackgroundWorker.RunWorkerAsync(reader.str_IPAdress + "-" + reader.int_Index.ToString())
            End If

            'add disconnected reader into list, after 1 minute will trigger TimerSendEmail to send Reader Notification Email
            Dim _reader_object As New ReaderEmailObject(reader.str_HostName, reader.str_IPAdress, reader.str_Name, Now(), StatusMsg)
            ReaderEmailDCList.Add(_reader_object)

            'send out email after 1 minute
            If Not TimerSendDCEmail.Enabled Then
                TimerSendDCEmail.Start()
            End If

        End If

    End Sub

    Private Sub check2ndloop(ByVal action As String, ByVal value As String, ByVal hostname As String, ByVal eventData As Boolean, ByVal mode As Integer)

        Dim reader As RFID_Reader_Item = m_reader_list(hostname)

        Dim indicator As Integer = 0

        If m_reader_list(hostname).m_ReaderAPI.Config.GPI.Item(2).PortState = 0 Then
            loopCoil2 = "In"
        End If

        If m_reader_list(hostname).m_ReaderAPI.Config.GPI.Item(1).PortState = 0 Then
            loopCoil2 = "Out"
        End If

        If tower = "1" Then
            tower = "Out"
        End If

        If tower = "2" Then
            tower = "In"
        End If

    End Sub

    Private Sub updateIn(ByVal action As String, ByVal value As String, ByVal hostname As String, ByVal eventData As Boolean, ByVal mode As Integer)

        Dim reader As RFID_Reader_Item = m_reader_list(hostname)

        Dim indicator As Integer = 0

        If m_reader_list(hostname).m_ReaderAPI.Config.GPI.Item(1).PortState = 0 Then

            hostn = hostname

            If hostn = "10.28.92.50" Then

                tower = "1"
            Else

                loopCoil1 = "In"
            End If

            Timer1.Start()
            Timer1.Enabled = True
        End If

        If m_reader_list(hostname).m_ReaderAPI.Config.GPI.Item(2).PortState = 0 Then
            hostn = hostname
            If hostn = "10.28.92.50" Then

                tower = "2"
            Else

                loopCoil1 = "Out"
            End If

            Timer1.Start()
            Timer1.Enabled = True
        End If



        'If m_ReaderAPI.Config.GPI.Item(1).PortState = GPIs.GPI_PORT_STATE.GPI_PORT_STATE_HIGH And m_ReaderAPI.Config.GPI.Item(2).PortState = GPIs.GPI_PORT_STATE.GPI_PORT_STATE_LOW Then
        '    hostn = hostname
        '    tower = "2"
        '    Timer1.Start()
        '    Timer1.Enabled = True
        'End If

        'If m_ReaderAPI.Config.GPI.Item(2).PortState = GPIs.GPI_PORT_STATE.GPI_PORT_STATE_HIGH And m_ReaderAPI.Config.GPI.Item(1).PortState = GPIs.GPI_PORT_STATE.GPI_PORT_STATE_LOW Then
        '    hostn = hostname
        '    tower = "1"
        '    Timer1.Start()
        '    Timer1.Enabled = True
        'End If
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick

        Dim reader As RFID_Reader_Item = m_reader_list(hostn)

        If loopCoil1 = "In" Then

            If loopCoil1 = loopCoil2 Then

                StartUpdateDB("IN")

                Timer1.Stop()
                Timer1.Enabled = False
            End If
            'End If
            reader.ht_TagDetected.Clear()
            reader.ht_TagTime.Clear()

        ElseIf loopCoil1 = "Out" Then

            If loopCoil1 = loopCoil2 Then

                StartUpdateDB("OUT")

                Timer1.Stop()
                Timer1.Enabled = False
            End If
            reader.ht_TagDetected.Clear()
            reader.ht_TagTime.Clear()

        ElseIf tower = "In" Then

            StartUpdateDBTower("IN")

            Timer1.Stop()
            Timer1.Enabled = False

        ElseIf tower = "Out" Then

            StartUpdateDBTower("OUT")

            Timer1.Stop()
            Timer1.Enabled = False
        Else
            Timer1.Stop()
            Timer1.Enabled = False
            reader.ht_TagDetected.Clear()
            reader.ht_TagTime.Clear()
        End If

    End Sub

    Private Sub StartUpdateDB(ByVal value As String)

        Dim reader As RFID_Reader_Item = m_reader_list(hostn)
        'Dim tagRSSI As Integer
        Dim ReaderRSSI As Integer = -70

        'Dim tagData As Symbol.RFID3.TagData() = reader.m_ReaderAPI.Actions.GetReadTags(50)

        'If tagData IsNot Nothing Then
        '    For nIndex As Integer = 0 To tagData.Length - 1
        '        If tagData(nIndex).OpCode = ACCESS_OPERATION_CODE.ACCESS_OPERATION_NONE OrElse (tagData(nIndex).OpCode = ACCESS_OPERATION_CODE.ACCESS_OPERATION_READ AndAlso tagData(nIndex).OpStatus = ACCESS_OPERATION_STATUS.ACCESS_SUCCESS) Then
        '            Dim tag As Symbol.RFID3.TagData = tagData(nIndex)
        '            Dim tagID As String = tag.TagID
        '            tagRSSI = tag.PeakRSSI

        '        End If
        '    Next

        '    If tagRSSI >= ReaderRSSI Then
        '        'For Each TagItem As DictionaryEntry In reader.ht_TagDetected
        '        '    dto = db.RFID_Common_MSSQL(value, TagItem.Key, reader.str_HostName, reader.str_IPAdress, reader.str_Stored_Procedure)
        '        'Next
        '    End If
        'End If

        For Each TagItem As DictionaryEntry In reader.ht_TagDetected
            'If (reader.str_Stored_Procedure = "SP_FILM_UPDATE_TRAN") Then
            '    logger._LogGen("StartUpdateDB() >> pTRAN_TYPE: " & value & "; pRFID: " & TagItem.Key & "; pReader: " & reader.str_HostName & "; pIPADDR: " & reader.str_IPAdress & "; SP: SP_FILM_UPDATE_TRAN")
            'End If
            logger._LogGen("StartUpdateDB() >> pTRAN_TYPE: " & value & "; pRFID: " & TagItem.Key & "; pReader: " & reader.str_HostName & "; pIPADDR: " & reader.str_IPAdress & "; SP: " & reader.str_Stored_Procedure)
            dto = db.RFID_Common_MSSQL(value, TagItem.Key, reader.str_HostName, reader.str_IPAdress, reader.str_Stored_Procedure)
        Next

        reader.ht_TagDetected.Clear()
        reader.ht_TagTime.Clear()

    End Sub

    Private Sub StartUpdateDBTower(ByVal value As String)

        Dim reader As RFID_Reader_Item = m_reader_list(hostn)

        For Each TagItem As DictionaryEntry In reader.ht_TagDetected
            logger._LogGen("StartUpdateDBTower() >> pTRAN_TYPE: " & value & "; pRFID: " & TagItem.Key & "; pReader: " & reader.str_HostName & "; pIPADDR: " & reader.str_IPAdress & "; SP: " & reader.str_Stored_Procedure)

            dto = db.RFID_Common_MSSQL_Tower(value, TagItem.Key, reader.str_HostName, reader.str_IPAdress, reader.str_Stored_Procedure)

        Next

        reader.ht_TagDetected.Clear()
        reader.ht_TagTime.Clear()

    End Sub

    Private Sub myUpdateRead(ByVal hostName As String, ByVal eventData As ReadEventData)

        Dim index As Integer = 0
        'Dim item As ListViewItem
        'Dim FoundTag As New List(Of String)
        Dim strFoundTag As String = ""
        Dim sendtag As String = ""
        Dim location As String = ""

        Dim reader As RFID_Reader_Item = m_reader_list(hostName)
        Dim antennaID As String = 1

        'HANA 10012024 adding hostName-location mapping for the tag email
        If (hostName = "10.28.92.52") Then
            location = "Green Tent House 2"
        ElseIf (hostName = "10.28.92.51") Then
            location = "Green Tent House 1"
        ElseIf (hostName = "10.28.92.50") Then
            location = "Tower Zone"
        End If


        Dim tagData As Symbol.RFID3.TagData() = reader.m_ReaderAPI.Actions.GetReadTags(50)
        Dim updateTag As New List(Of Object)
        If tagData IsNot Nothing Then
            Dim tagDtl As New List(Of TagDetails)

            For nIndex As Integer = 0 To tagData.Length - 1
                If tagData(nIndex).OpCode = ACCESS_OPERATION_CODE.ACCESS_OPERATION_NONE OrElse (tagData(nIndex).OpCode = ACCESS_OPERATION_CODE.ACCESS_OPERATION_READ AndAlso tagData(nIndex).OpStatus = ACCESS_OPERATION_STATUS.ACCESS_SUCCESS) Then
                    Dim tag As Symbol.RFID3.TagData = tagData(nIndex)
                    antennaID = tag.AntennaID.ToString
                    Dim tagID As String = tag.TagID
                    Dim isFound As Boolean = False
                    Dim isFoundTag As Boolean = False

                    SyncLock reader.ht_TagDetected
                        isFoundTag = reader.ht_TagDetected.ContainsKey(tagID)
                        'MessageBox.Show("Tag found ? " + isFound.ToString())
                    End SyncLock

                    If Not isFoundTag Then
                        ' New tag detected
                        SyncLock reader.ht_TagDetected
                            reader.ht_TagDetected.Add(tagID, False)
                        End SyncLock

                        SyncLock reader.ht_TagTime
                            reader.ht_TagTime.Add(tagID, DateTime.UtcNow)
                        End SyncLock
                    End If 'isFound

                    'HANA 10012024 adding the tag email
                    tagDtl.Add(New TagDetails() With {.hostName = hostName, .location = location, .antennaID = antennaID, .tagID = tagID, .tagTime = Now()})
                End If
            Next

            For Each TagItem As DictionaryEntry In reader.ht_TagDetected
                If TagItem.Value = False Then

                    'test
                    'txt_log.AppendText($"{vbNewLine}{Date.Now.ToShortTimeString()} - RFID Tag {TagItem.Key} updated in {reader.str_Name}")
                    'txt_log.ScrollToCaret()

                    'check loopcoils detected:

                    'If m_reader_list(hostName).m_ReaderAPI.Config.GPI.Item(1).PortState = 0 Then
                    '    If m_reader_list(hostName).m_ReaderAPI.Config.GPI.Item(2).PortState = 0 Then
                    '        mode = "IN"
                    '    End If
                    'End If

                    'If m_reader_list(hostName).m_ReaderAPI.Config.GPI.Item(2).PortState = 0 Then
                    '    If m_reader_list(hostName).m_ReaderAPI.Config.GPI.Item(1).PortState = 0 Then
                    '        mode = "OUT"
                    '    End If
                    'End If


                    'If m_reader_list(hostName).m_ReaderAPI.Config.GPI.Item(1).PortState = GPIs.GPI_PORT_STATE.GPI_PORT_STATE_LOW Then
                    '    If m_reader_list(hostName).m_ReaderAPI.Config.GPI.Item(2).PortState = GPIs.GPI_PORT_STATE.GPI_PORT_STATE_LOW Then
                    '        mode = "IN"
                    '    End If
                    'ElseIf m_reader_list(hostName).m_ReaderAPI.Config.GPI.Item(2).PortState = GPIs.GPI_PORT_STATE.GPI_PORT_STATE_LOW Then
                    '    If m_reader_list(hostName).m_ReaderAPI.Config.GPI.Item(1).PortState = GPIs.GPI_PORT_STATE.GPI_PORT_STATE_LOW Then
                    '        mode = "OUT"
                    '    End If
                    'Else
                    '    mode = ""
                    'End If


                    If antennaID = 1 Then
                        'Update Status Here (DB)
                        If reader.str_SERVER = "ORACLE" Then
                            'dto = db.RFID_Common_ORA(TagItem.Key, reader.str_HostName, reader.str_IPAdress, reader.str_Stored_Procedure)
                        ElseIf reader.str_SERVER = "MSSQL" Then
                            'dto = db.RFID_Common_MSSQL(mode, TagItem.Key, reader.str_HostName, reader.str_IPAdress, reader.str_Stored_Procedure)
                        End If
                    Else
                        If reader.str_SERVER2 IsNot Nothing And reader.str_Stored_Procedure2 IsNot Nothing Then
                            If reader.str_SERVER2 = "ORACLE" Then
                                'dto = db.RFID_Common_ORA(TagItem.Key, reader.str_HostName, reader.str_IPAdress, reader.str_Stored_Procedure2)
                            ElseIf reader.str_SERVER2 = "MSSQL" Then
                                'dto = db.RFID_Common_MSSQL(mode, TagItem.Key, reader.str_HostName, reader.str_IPAdress, reader.str_Stored_Procedure2)
                            End If
                        End If
                    End If


                    If dto.Error = False Then
                        'txt_log.AppendText($"{vbNewLine}{Date.Now.ToShortTimeString()} - RFID Tag {TagItem.Key} updated in {reader.str_Name}")
                        'txt_log.ScrollToCaret()
                        TowerLight_ON(1, reader.str_IPAdress)
                    Else
                        'txt_log.AppendText($"{vbNewLine}{Date.Now.ToShortTimeString()} - Error Occur in {reader.str_Name} : {dto.ErrorMessage}")
                        'txt_log.ScrollToCaret()
                        TowerLight_ON(2, reader.str_IPAdress)
                    End If

                    updateTag.Add(TagItem.Key)
                    'HANA 10112023 Change to move Key = True to outside the loop due to exception caused
                    'reader.ht_TagDetected(TagItem.Key) = True
                    ' TagItem.Key(tagData)

                End If
            Next

            For Each tagItemKey In updateTag
                reader.ht_TagDetected(tagItemKey) = True

            Next

            'HANA 10012024 adding the tag email. Tag email is only for host 50
            If (hostName = "10.28.92.50") Then
                'Removing loop to avoid duplicates
                'Dim tagDt = tagDtl.GroupBy(Function(x) x.tagID).[Select](Function(x) x.First()).ToList()
                Dim tagDt = tagDtl.First()
                Dim firstChar = tagDt.tagID.First()
                If (firstChar = "E" Or firstChar = "B") Then
                    If (lockTag = "" Or tagDt.tagID <> lockTag) Then 'HANA 220324 Add lock check
                        lockTag = tagDt.tagID
                        TimerTagLock.Start()

                        'For i As Integer = 0 To tagDt.Count - 1
                        Dim _reader_object As New ReaderTagObject(tagDt.hostName, tagDt.location, tagDt.antennaID, tagDt.tagID, tagDt.tagTime)
                        ReaderEmailTagList.Add(_reader_object)
                        If Not TimerSendTagEmail.Enabled Then
                            TimerSendTagEmail.Start()
                        End If

                        logger._LogGen("myUpdateRead() >> Emailing >> hostName: " & tagDt.hostName & "; location: " & tagDt.location & "; antennaID: " & tagDt.antennaID & "; RFID: " & tagDt.tagID & "; tagTime: " & tagDt.tagTime)
                    End If
                End If
                ' Next
            End If

            'Catch ex As Exception
            '    m_LastSent = DateTime.Now
            '    sendtag = ""
            '    strFoundTag = ""
            'End Try
            ' tag checking end
        Else
                Dim dd As TimeSpan

            dd = DateTime.Now - m_emptyTagTime

        End If


    End Sub

    'Dim _reader_object As New ReaderEmailObject(readerItem.str_HostName, readerItem.str_IPAdress, readerItem.str_Name, Now(), "Reconnect failed.")
    '                    ReaderEmailFailList.Add(_reader_object)
    '                    If Not TimerSendFailEmail.Enabled Then
    '                        TimerSendFailEmail.Start()
    '                    End If


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            '''''START'''''
            'Using _thread As New CustomBgWorker
            '    AddHandler _thread.DoWork, AddressOf BgTestLoading_DoWork
            '    _thread.Run()

            '    If _Error <> "" Then
            '        Throw New Exception(_Error)
            '    End If
            'End Using

            'MessageBox.Show("Success")
            ''''''END''''''

            'LoadConnection()

            lbl_error.Text = ""
            m_emptyTagTime = DateTime.Now

        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString())
        End Try
    End Sub

    Private Sub Events_ReadNotify(ByVal sender As Object, ByVal readEventArgs As ReadEventArgs)
        Try
            Dim ReaderHostName As String = CType(sender, Symbol.RFID3.Events).HostName
            MyBase.Invoke(Me.m_UpdateReadHandler, New Object() {ReaderHostName, readEventArgs.ReadEventData.TagData})
        Catch exception1 As Exception
        End Try
    End Sub

    Public Sub Events_StatusNotify(ByVal sender As Object, ByVal statusEventArgs As StatusEventArgs)
        Try
            Dim ReaderHostName As String = CType(sender, Symbol.RFID3.Events).HostName
            MyBase.Invoke(Me.m_UpdateStatusHandler, New Object() {ReaderHostName, statusEventArgs.StatusEventData})
        Catch exception1 As Exception
        End Try
    End Sub

    Private Sub ConnectBackgroundWorker_DoWork(sender As Object, e As DoWorkEventArgs)

        Dim arry() As String = e.Argument.ToString().Split("-")
        Dim readerItem As RFID_Reader_Item = m_reader_list(arry(0))
        'Dim currentReader As RFIDReader = readerItem.m_ReaderAPI
        Dim ReconnectRequired As Boolean = readerItem.bool_ReconnectRequired
        Dim _Success As Boolean = False
        Dim _Result As New returnResult

        If readerItem.m_ReaderAPI.IsConnected Then
            _Result.SkipThisConnect = True
            _Result.rowIndex = Convert.ToInt32(arry(1))
            e.Result = _Result
            Exit Sub

        End If

        Try
            If ReconnectRequired Then
                Try
                    readerItem.m_ReaderAPI.Reconnect()
                    _Success = True
                Catch ex As Exception
                    Try
                        'HANA Change timeout val from 0 to 50secs
                        'readerItem.m_ReaderAPI = New RFIDReader(readerItem.str_IPAdress, readerItem.str_Port, 0)
                        readerItem.m_ReaderAPI = New RFIDReader(readerItem.str_IPAdress, readerItem.str_Port, 50000)
                        readerItem.m_ReaderAPI.Connect()
                        _Success = True
                    Catch exed As Exception
                        'HANA 09112023 Adding email for failed connection
                        Dim _reader_object As New ReaderEmailObject(readerItem.str_HostName, readerItem.str_IPAdress, readerItem.str_Name, Now(), "Reconnect failed.")
                        ReaderEmailFailList.Add(_reader_object)
                        If Not TimerSendFailEmail.Enabled Then
                            TimerSendFailEmail.Start()
                        End If

                        _Result.rowIndex = Convert.ToInt32(arry(1))
                        _Result.Result = "Disconnect"
                        e.Result = _Result
                    End Try
                End Try

            Else
                readerItem.m_ReaderAPI.Connect()
                _Success = True
            End If

            If _Success Then
                _Result.rowIndex = Convert.ToInt32(arry(1))
                _Result.Result = "Connect Succeed"
                e.Result = _Result
            End If

        Catch operationException As OperationFailureException
            _Result.rowIndex = Convert.ToInt32(arry(1))
            _Result.Result = operationException.StatusDescription + " : " + arry(0)
            e.Result = _Result

            'HANA 09112023 Adding email for failed connection
            Dim _reader_object As New ReaderEmailObject(readerItem.str_HostName, readerItem.str_IPAdress, readerItem.str_Name, Now(), "Connect failed. " & e.Result.ToString)
            ReaderEmailFailList.Add(_reader_object)
            If Not TimerSendFailEmail.Enabled Then
                TimerSendFailEmail.Start()
            End If
            'sendErrorMSG(workEventArgs.Result)c

        Catch ex As Exception
            _Result.rowIndex = Convert.ToInt32(arry(1))
            _Result.Result = ex.Message
            e.Result = _Result

            'HANA 09112023 Adding email for failed connection
            Dim _reader_object As New ReaderEmailObject(readerItem.str_HostName, readerItem.str_IPAdress, readerItem.str_Name, Now(), "Connect failed. " & e.Result.ToString)
            ReaderEmailFailList.Add(_reader_object)
            If Not TimerSendFailEmail.Enabled Then
                TimerSendFailEmail.Start()
            End If
            'sendErrorMSG(workEventArgs.Result)
        End Try


        'Try
        '    Me.m_ReaderAPI.Disconnect()
        '    Me.m_IsConnected = False
        '    _Result = "Disconnect succeed"
        'Catch ofe As OperationFailureException
        '    _Result = ofe.Result
        'End Try

        'Thread.Sleep(5000)
    End Sub

    Private Sub ConnectBackgroundWorker_ProgressChanged(sender As Object, e As ProgressChangedEventArgs)

    End Sub

    Private Sub ConnectBackgroundWorker_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs)

        Dim _ReturnResult As returnResult = e.Result
        Dim rowIndex As Integer = _ReturnResult.rowIndex
        Dim Result As String = _ReturnResult.Result
        Dim hostName As String = lv_reader.Items(rowIndex).SubItems(1).Text.ToString()
        Dim readerItem As RFID_Reader_Item = m_reader_list(hostName)
        'Dim currentReader As RFIDReader = readerItem.m_ReaderAPI
        Dim antennaList As UShort() = New UShort(1) {1, 2}

        Dim antennaInfo As AntennaInfo = New AntennaInfo(antennaList)


        If _ReturnResult.SkipThisConnect Then

            _ReturnResult.SkipThisConnect = False

            lv_reader.Items(rowIndex).SubItems(3).Text = "Connected"
            lv_reader.Items(rowIndex).ForeColor = Color.Green

            Exit Sub

        End If

        If (Result = "Connect Succeed") Then

            If Not My.Settings.TEST_ENVIRONMENT Then
                AddHandler readerItem.m_ReaderAPI.Events.ReadNotify, New ReadNotifyHandler(AddressOf Me.Events_ReadNotify)
                readerItem.m_ReaderAPI.Events.AttachTagDataWithReadEvent = False
            End If
            AddHandler readerItem.m_ReaderAPI.Events.StatusNotify, New StatusNotifyHandler(AddressOf Me.Events_StatusNotify)
            readerItem.m_ReaderAPI.Events.NotifyGPIEvent = True
            readerItem.m_ReaderAPI.Events.NotifyBufferFullEvent = True
            readerItem.m_ReaderAPI.Events.NotifyBufferFullWarningEvent = True
            readerItem.m_ReaderAPI.Events.NotifyReaderDisconnectEvent = True
            readerItem.m_ReaderAPI.Events.NotifyReaderExceptionEvent = True
            readerItem.m_ReaderAPI.Events.NotifyAccessStartEvent = True
            readerItem.m_ReaderAPI.Events.NotifyAccessStopEvent = True
            readerItem.m_ReaderAPI.Events.NotifyInventoryStartEvent = True
            readerItem.m_ReaderAPI.Events.NotifyInventoryStopEvent = True
            '.Events.NotifyTemperatureAlarmEvent = True

            Try

                readerItem.m_ReaderAPI.Actions.Inventory.Perform(Nothing, Nothing, antennaInfo)

            Catch operationException As OperationFailureException

                Console.WriteLine(operationException.Result)

            End Try

            If readerItem.bool_ReconnectRequired Then

                readerItem.bool_ReconnectRequired = False

                Dim _reader_object As New ReaderEmailObject(readerItem.str_HostName, readerItem.str_IPAdress, readerItem.str_Name, Now(), "Reconnect successfully.")
                ReaderEmailRCList.Add(_reader_object)

                If Not TimerSendRCEmail.Enabled Then
                    TimerSendRCEmail.Start()
                End If

            End If

            lv_reader.Items(rowIndex).SubItems(3).Text = "Connected"
            lv_reader.Items(rowIndex).ForeColor = Color.Green

            'txt_log.AppendText($"{vbNewLine}{Date.Now.ToShortTimeString()} - {lv_reader.Items(rowIndex).Text} connect successfully.")
            'txt_log.ScrollToCaret()

            btnConnect.Text = "Disconnect"
            btnConnect.BackColor = Color.Red

        Else
            'Error Handle
            lv_reader.Items(rowIndex).SubItems(3).Text = Result.ToString()
            lv_reader.Items(rowIndex).ForeColor = Color.Red

            'HANA 09112023 Adding email for failed connection
            Dim _reader_object As New ReaderEmailObject(readerItem.str_HostName, readerItem.str_IPAdress, readerItem.str_Name, Now(), "Reconnect failed.")
            ReaderEmailFailList.Add(_reader_object)
            If Not TimerSendFailEmail.Enabled Then
                TimerSendFailEmail.Start()
            End If
            'txt_log.AppendText($"{vbNewLine}{Date.Now.ToShortTimeString()} - Error occur while connect to {lv_reader.Items(rowIndex).Text} : {Result.ToString()}")
            'txt_log.ScrollToCaret()

            btnConnect.Text = "Connect"
            btnConnect.BackColor = Color.Lime
        End If

    End Sub

    Private Sub AppForm_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs) Handles MyBase.FormClosing

        Dim frm As New Loading
        Me.bg_FormClosing.RunWorkerAsync(frm)
        frm.ShowDialog()

    End Sub

    Private Sub btnConnectAll_Click(sender As Object, e As EventArgs) Handles btnConnectAll.Click
        Try
            For i As Integer = 0 To lv_reader.Items.Count - 1
                Dim _PassHostName As String

                _PassHostName = lv_reader.Items(i).SubItems(1).Text
                lv_reader.Items(i).SubItems(3).Text = "Connecting..."
                lv_reader.Items(i).ForeColor = Color.Green

                Dim worker As New BackgroundWorker
                AddHandler worker.DoWork, AddressOf ConnectBackgroundWorker_DoWork
                AddHandler worker.RunWorkerCompleted, AddressOf ConnectBackgroundWorker_RunWorkerCompleted

                worker.RunWorkerAsync(_PassHostName + "-" + i.ToString())

            Next
        Catch ex As Exception
            Me.lbl_error.Text = ex.Message
        End Try
    End Sub

    Private Sub btnConnect_Click(sender As Object, e As EventArgs) Handles btnConnect.Click
        If lv_reader.SelectedItems.Count > 0 Then
            Try
                If btnConnect.Text = "Connect" Then
                    Dim _PassHostName As String
                    Dim rowIndex As Integer = lv_reader.SelectedIndices(0)

                    _PassHostName = lv_reader.Items(rowIndex).SubItems(1).Text
                    lv_reader.Items(rowIndex).SubItems(3).Text = "Connecting..."
                    lv_reader.Items(rowIndex).ForeColor = Color.Green

                    Dim worker As New BackgroundWorker
                    AddHandler worker.DoWork, AddressOf ConnectBackgroundWorker_DoWork
                    AddHandler worker.RunWorkerCompleted, AddressOf ConnectBackgroundWorker_RunWorkerCompleted

                    worker.RunWorkerAsync(_PassHostName + "-" + rowIndex.ToString())
                    'Me.ConnectBackgroundWorker.RunWorkerAsync(rowIndex)


                ElseIf btnConnect.Text = "Disconnect" Then
                    Dim rowIndex As Integer = lv_reader.SelectedIndices(0)
                    Dim _PassHostName As String

                    _PassHostName = lv_reader.Items(rowIndex).SubItems(1).Text
                    lv_reader.Items(rowIndex).SubItems(3).Text = "Disconnecting..."
                    lv_reader.Items(rowIndex).ForeColor = Color.Red

                    Dim worker As New BackgroundWorker
                    AddHandler worker.DoWork, AddressOf BackgroundWorkerDisconnectReader_DoWork
                    AddHandler worker.RunWorkerCompleted, AddressOf BackgroundWorkerDisconnectReader_RunWorkerCompleted

                    worker.RunWorkerAsync(_PassHostName + "-" + rowIndex.ToString())

                    'Me.BackgroundWorkerDisconnectReader.RunWorkerAsync(rowIndex)

                    btnConnect.Text = "Connect"

                End If
            Catch ex As Exception
                Me.lbl_error.Text = ex.Message
            End Try

        End If
    End Sub

    Private Sub BackgroundWorkerDisconnectReader_DoWork(sender As Object, e As DoWorkEventArgs)

        Dim arry() As String = e.Argument.ToString().Split("-")
        Dim currentReader As RFIDReader = m_reader_list(arry(0)).m_ReaderAPI

        Dim _Result As New returnResult
        _Result.rowIndex = Convert.ToInt32(arry(1))

        If currentReader.IsConnected Then
            Try
                If (currentReader.Actions.TagAccess.OperationSequence.Length > 0) Then
                    currentReader.Actions.TagAccess.OperationSequence.StopSequence()
                    currentReader.Actions.Inventory.Stop()
                Else
                    currentReader.Actions.Inventory.Stop()
                End If
                currentReader.Disconnect()
            Catch ex As Exception
                _Error = ex.Message.ToString
            End Try

        Else
            _Result.SkipThisConnect = True
        End If
        e.Result = _Result
    End Sub

    Private Sub BackgroundWorkerDisconnectReader_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs)
        Dim _ReturnResult As returnResult = e.Result

        Dim rowIndex As Integer = _ReturnResult.rowIndex
        Dim hostName As String = lv_reader.Items(rowIndex).SubItems(1).Text.ToString()
        Dim currentReader As RFIDReader = m_reader_list(hostName).m_ReaderAPI

        If _ReturnResult.SkipThisConnect Then

            _ReturnResult.SkipThisConnect = False

        Else
            'RemoveHandler currentReader.Events.ReadNotify, AddressOf Me.Events_ReadNotify
            'RemoveHandler currentReader.Events.StatusNotify, AddressOf Me.Events_StatusNotify

            'txt_log.AppendText($"{vbNewLine}{Date.Now.ToShortTimeString()} - {lv_reader.Items(rowIndex).Text} disconnect successfully.")
            'txt_log.ScrollToCaret()

        End If

        lv_reader.Items(rowIndex).SubItems(3).Text = "Disconnect"
        lv_reader.Items(rowIndex).ForeColor = Color.Red
        If btnConnect.Text = "Connect" Then
            btnConnect.BackColor = Color.Lime
        Else
            btnConnect.BackColor = Color.Red
        End If


    End Sub

    Private Sub btnDisconnectAll_Click(sender As Object, e As EventArgs) Handles btnDisconnectAll.Click
        Try
            For i As Integer = 0 To lv_reader.Items.Count - 1
                Dim _PassHostName As String

                _PassHostName = lv_reader.Items(i).SubItems(1).Text
                lv_reader.Items(i).SubItems(3).Text = "Disconnecting..."
                lv_reader.Items(i).ForeColor = Color.Red

                Dim worker As New BackgroundWorker
                AddHandler worker.DoWork, AddressOf BackgroundWorkerDisconnectReader_DoWork
                AddHandler worker.RunWorkerCompleted, AddressOf BackgroundWorkerDisconnectReader_RunWorkerCompleted

                worker.RunWorkerAsync(_PassHostName + "-" + i.ToString())
            Next
        Catch ex As Exception
            Me.lbl_error.Text = ex.Message
        End Try
    End Sub

    Private Sub lv_reader_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lv_reader.SelectedIndexChanged
        If lv_reader.SelectedItems.Count > 0 Then
            Try
                Dim rowIndex As Integer = lv_reader.SelectedIndices(0)
                Dim hostName As String = lv_reader.Items(rowIndex).SubItems(1).Text.ToString()
                Dim currentReader As RFIDReader = m_reader_list(hostName).m_ReaderAPI

                If currentReader.IsConnected Then
                    btnConnect.Text = "Disconnect"
                    btnConnect.BackColor = Color.Red
                Else
                    btnConnect.Text = "Connect"
                    btnConnect.BackColor = Color.Lime
                End If
            Catch ex As Exception
                Me.lbl_error.Text = ex.Message
            End Try

        End If
    End Sub

    Private Sub bg_GetRFIDConfig_DoWork(sender As Object, e As DoWorkEventArgs) Handles bg_GetRFIDConfig.DoWork

        'Thread.Sleep(1500)

        e.Result = e.Argument

        dto = db.GET_RFID_CONFIG("FILM")

    End Sub

    Private Sub bg_GetRFIDConfig_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles bg_GetRFIDConfig.RunWorkerCompleted
        Try
            If dto.Error = True Then
                Throw New Exception(dto.ErrorMessage)
            End If

            Dim item As New ListViewItem
            Dim subitem As New ListViewItem.ListViewSubItem
            Dim ReaderIndex As Integer = 0
            For Each row In dto.Table.Rows
                _r_dtl = New RFID_Reader_Item
                _r_dtl.int_Index = ReaderIndex
                'HANA Change timeout val from 0 to 50secs
                '_r_dtl.m_ReaderAPI = New RFIDReader(row("IP_ADDRESS").ToString, row("PORT").ToString, 0)
                _r_dtl.m_ReaderAPI = New RFIDReader(row("IP_ADDRESS").ToString, row("PORT").ToString, 50000)
                _r_dtl.ht_TagDetected = New Hashtable
                _r_dtl.ht_TagTime = New Hashtable
                _r_dtl.str_Name = row("LOCATION").ToString()
                _r_dtl.str_IPAdress = row("IP_ADDRESS").ToString()
                _r_dtl.str_HostName = row("HOST_NAME").ToString()
                _r_dtl.str_Port = row("PORT").ToString()
                _r_dtl.str_Stored_Procedure = row("SP").ToString()
                _r_dtl.str_SERVER = row("SERVER").ToString()
                _r_dtl.str_Stored_Procedure2 = row("SP2").ToString()
                _r_dtl.str_SERVER2 = row("SERVER2").ToString()
                m_reader_list.Add(row("IP_ADDRESS").ToString(), _r_dtl)

                item = New ListViewItem(row("LOCATION").ToString())

                'Location
                subitem = New ListViewItem.ListViewSubItem(item, row("IP_ADDRESS").ToString())
                item.SubItems.Add(subitem)

                'Reader Name
                subitem = New ListViewItem.ListViewSubItem(item, row("HOST_NAME").ToString())
                item.SubItems.Add(subitem)

                'Status
                subitem = New ListViewItem.ListViewSubItem(item, "Disconnect")
                item.SubItems.Add(subitem)

                item.ForeColor = Color.Red

                lv_reader.Items.Add(item)
                ReaderIndex += 1
            Next

            'txt_log.Text += vbNewLine
            'txt_log.AppendText($"{Date.Now.ToShortTimeString()} - {dto.Table.Rows.Count.ToString()} connection found.")
            'txt_log.ScrollToCaret()

            Reset_Table.Enabled = True
            TimerReconnect.Start()

            Dim frm As Loading = TryCast(e.Result, Loading)
            If frm IsNot Nothing Then
                frm.Close()
            End If

        Catch ex As Exception
            MessageBox.Show(ex.Message)

            Dim frm As Loading = TryCast(e.Result, Loading)
            If frm IsNot Nothing Then
                frm.Close()
            End If
        End Try
    End Sub

    Private Sub bg_FormClosing_DoWork(sender As Object, e As DoWorkEventArgs) Handles bg_FormClosing.DoWork
        For Each Item As KeyValuePair(Of String, RFID_Reader_Item) In m_reader_list

            Dim currentReader As RFIDReader = Item.Value.m_ReaderAPI

            If currentReader.IsConnected Then

                Try
                    If (currentReader.Actions.TagAccess.OperationSequence.Length > 0) Then
                        currentReader.Actions.TagAccess.OperationSequence.StopSequence()
                        currentReader.Actions.Inventory.Stop()
                    Else
                        currentReader.Actions.Inventory.Stop()
                    End If

                Catch ex As Exception

                End Try

            End If

        Next

        e.Result = e.Argument

    End Sub

    Private Sub bg_FormClosing_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles bg_FormClosing.RunWorkerCompleted
        Dim frm As Loading = TryCast(e.Result, Loading)
        If frm IsNot Nothing Then
            frm.Close()
        End If
    End Sub

    Private Sub GPIO_ligthing_Control(ByVal pStr_LightOption As String, ByVal HostName As String)
        If pStr_LightOption.ToLower.Trim.Equals("error") = True Then
            ''Error
            m_reader_list(HostName).m_ReaderAPI.Config.GPO.Item(1).PortState = GPO_PORT_STATE.TRUE
            m_reader_list(HostName).m_ReaderAPI.Config.GPO.Item(2).PortState = GPO_PORT_STATE.FALSE
            m_reader_list(HostName).m_ReaderAPI.Config.GPO.Item(3).PortState = GPO_PORT_STATE.FALSE

            'txt_log.Text += $"{vbNewLine}{Date.Now.ToShortTimeString()} - {m_reader_list(HostName).str_Name}'s Tower Light - Red"
        ElseIf pStr_LightOption.ToLower.Trim.Equals("reset") = True Then
            ''Reset
            m_reader_list(HostName).m_ReaderAPI.Config.GPO.Item(1).PortState = GPO_PORT_STATE.FALSE
            m_reader_list(HostName).m_ReaderAPI.Config.GPO.Item(2).PortState = GPO_PORT_STATE.FALSE
            m_reader_list(HostName).m_ReaderAPI.Config.GPO.Item(3).PortState = GPO_PORT_STATE.FALSE

        Else
            ''OK
            m_reader_list(HostName).m_ReaderAPI.Config.GPO.Item(1).PortState = GPO_PORT_STATE.FALSE
            m_reader_list(HostName).m_ReaderAPI.Config.GPO.Item(2).PortState = GPO_PORT_STATE.TRUE
            m_reader_list(HostName).m_ReaderAPI.Config.GPO.Item(3).PortState = GPO_PORT_STATE.FALSE

            'txt_log.Text += $"{vbNewLine}{Date.Now.ToShortTimeString()} - {m_reader_list(HostName).str_Name}'s Tower Light - Green"
        End If
    End Sub

    Private Sub TowerLight_ON(Mode As Integer, HostName As String)

        Dim tmr As New Windows.Forms.Timer

        tmr.Interval = 4500
        tmr.Tag = HostName
        AddHandler tmr.Tick, AddressOf TowerLight_Timer_Tick

        If Mode = 1 Then
            GPIO_ligthing_Control("ok", HostName)
        ElseIf Mode = 2 Then
            GPIO_ligthing_Control("error", HostName)
        End If

        tmr.Enabled = True

    End Sub

    Private Sub TowerLight_Timer_Tick(sender As Object, e As EventArgs)

        Dim tmr As Windows.Forms.Timer = TryCast(sender, Windows.Forms.Timer)
        Dim HostName As String = tmr.Tag

        GPIO_ligthing_Control("reset", HostName)
        tmr.Enabled = False
        tmr.Dispose()

    End Sub

    Private Sub Reset_Table_Tick(sender As Object, e As EventArgs) Handles Reset_Table.Tick
        Dim toRemove As List(Of String)
        For Each Item As KeyValuePair(Of String, RFID_Reader_Item) In m_reader_list

            toRemove = New List(Of String)

            For Each vKey In Item.Value.ht_TagTime.Keys
                Dim starttime As DateTime = Item.Value.ht_TagTime.Item(vKey)
                'If Tag is more than 5 mins, clear it from hashtable
                If starttime.AddMinutes(2) < DateTime.UtcNow Then
                    toRemove.Add(vKey)
                End If
            Next

            For Each key In toRemove
                Item.Value.ht_TagTime.Remove(key)
                Item.Value.ht_TagDetected.Remove(key)
                'txt_log.Text += vbNewLine + key + " removed from " + Item.Value.str_Name
            Next

        Next

    End Sub

    Private Sub TimerSendDCEmail_Tick(sender As Object, e As EventArgs) Handles TimerSendDCEmail.Tick

        Dim Company As String = My.Settings.COMPANY.ToString
        'HANA 08112023 : Changing mailto values
        'Dim MailTo As String = If(My.Settings.TEST_ENVIRONMENT, "rosmieza@maxsys.com.my", My.Settings.READER_NOTIFICATION_MAILTO.ToString)
        Dim MailTo As String = If(My.Settings.TEST_ENVIRONMENT, "nurfarhanah@maxsys.com.my", My.Settings.READER_NOTIFICATION_MAILTO.ToString)
        Dim MailCc As String = My.Settings.READER_NOTIFICATION_MAILCC.ToString
        Dim MailBcc As String = My.Settings.READER_NOTIFICATION_MAILBCC.ToString

        Dim _email_object As New EmailObject(Company, MailTo, MailCc, MailBcc, False, ReaderEmailDCList)

        'dto = db.GET_RFID_CONFIG("FILM")
        'Dim ipAddress As String = String.Empty

        'For Each row In dto.Table.Rows

        '    ipAddress = row("IP_ADDRESS").ToString()

        'Next

        'if fail to send email, then add into pending list
        'If Not Mailer.SendDBEmailMSSQL(ipAddress) Then
        If Not Mailer.SendDBEmailMSSQL(_email_object) Then
            PendingEmailList.Add(_email_object)
        End If

        ReaderEmailDCList.Clear()

        TimerSendDCEmail.Stop()

    End Sub

    Private Sub TimerSendRCEmail_Tick(sender As Object, e As EventArgs) Handles TimerSendRCEmail.Tick

        Dim Company As String = My.Settings.COMPANY.ToString
        Dim MailTo As String = If(My.Settings.TEST_ENVIRONMENT, "nurfarhanah@maxsys.com.my", My.Settings.READER_NOTIFICATION_MAILTO.ToString)
        Dim MailCc As String = My.Settings.READER_NOTIFICATION_MAILCC.ToString
        Dim MailBcc As String = My.Settings.READER_NOTIFICATION_MAILBCC.ToString

        Dim _email_object As New EmailObject(Company, MailTo, MailCc, MailBcc, False, ReaderEmailRCList)

        'dto = db.GET_RFID_CONFIG("FILM")
        'Dim ipAddress As String = String.Empty

        'For Each row In dto.Table.Rows

        '    ipAddress = row("IP_ADDRESS").ToString()

        'Next

        'if fail to send email, then add into pending list
        'If Not Mailer.SendDBEmailMSSQL(ipAddress) Then
        If Not Mailer.SendDBEmailMSSQL(_email_object) Then
            PendingEmailList.Add(_email_object)
        End If

        ReaderEmailRCList.Clear()

        TimerSendRCEmail.Stop()
    End Sub

    'HANA 09112023 Add email for fail connection
    Private Sub TimerSendFailEmail_Tick(sender As Object, e As EventArgs) Handles TimerSendFailEmail.Tick

        Dim Company As String = My.Settings.COMPANY.ToString
        Dim MailTo As String = If(My.Settings.TEST_ENVIRONMENT, "nurfarhanah@maxsys.com.my", My.Settings.READER_NOTIFICATION_MAILTO.ToString)
        Dim MailCc As String = My.Settings.READER_NOTIFICATION_MAILCC.ToString
        Dim MailBcc As String = My.Settings.READER_NOTIFICATION_MAILBCC.ToString

        Dim _email_object As New EmailObject(Company, MailTo, MailCc, MailBcc, False, ReaderEmailFailList)

        'dto = db.GET_RFID_CONFIG("FILM")
        'Dim ipAddress As String = String.Empty

        'For Each row In dto.Table.Rows

        '    ipAddress = row("IP_ADDRESS").ToString()

        'Next

        'if fail to send email, then add into pending list
        'If Not Mailer.SendDBEmailMSSQL(ipAddress) Then
        If Not Mailer.SendDBEmailMSSQL(_email_object) Then
            PendingEmailList.Add(_email_object)
        End If

        ReaderEmailFailList.Clear()

        TimerSendFailEmail.Stop()
    End Sub

    'HANA 10012024 Add email for tag
    Private Sub TimerSendTagEmail_Tick(sender As Object, e As EventArgs) Handles TimerSendTagEmail.Tick

        Dim Company As String = My.Settings.COMPANY.ToString
        Dim MailTo As String = If(My.Settings.TEST_ENVIRONMENT, "nurfarhanah@maxsys.com.my", My.Settings.READER_NOTIFICATION_MAILTO.ToString)
        Dim MailCc As String = My.Settings.READER_NOTIFICATION_MAILCC.ToString
        Dim MailBcc As String = My.Settings.READER_NOTIFICATION_MAILBCC.ToString

        'Dim _email_object As New EmailTagObject(Company, MailTo, MailCc, MailBcc, False, ReaderEmailTagList)

        'If Not Mailer.SendDBTagEmailMSSQL(_email_object) Then
        '    PendingTagEmailList.Add(_email_object)
        'End If

        ' Process items in groups of 33 (Max in one email before it is truncated)
        Dim batchSize As Integer = 33
        For i As Integer = 0 To ReaderEmailTagList.Count - 1 Step batchSize
            Dim batch = ReaderEmailTagList.Skip(i).Take(batchSize).ToList()

            Dim _email_object As New EmailTagObject(Company, MailTo, MailCc, MailBcc, False, batch)
            If Not Mailer.SendDBTagEmailMSSQL(_email_object) Then
                PendingTagEmailList.Add(_email_object)
            End If
        Next

        ReaderEmailTagList.Clear()
        TimerSendTagEmail.Stop()
    End Sub

    Private Sub TimerTagReconnect_Tick(sender As Object, e As EventArgs) Handles TimerTagReconnect.Tick
        Dim RowIndex As Integer = 0
        For Each kvp As KeyValuePair(Of String, RFID_Reader_Item) In m_reader_list
            Dim _reader As RFID_Reader_Item = kvp.Value
            If _reader.bool_ReconnectRequired Then
                If Not ReconnectBackgroundWorker.IsBusy Then
                    ReconnectBackgroundWorker.RunWorkerAsync(_reader.str_IPAdress + "-" + RowIndex.ToString())
                End If
            End If
            RowIndex += 1
        Next

        'Attempt to resend email
        For Each _email As EmailTagObject In PendingTagEmailList

            'Mailer.SendDBEmailMSSQL(ipAddress)
            Mailer.SendDBTagEmailMSSQL(_email)

        Next

        PendingTagEmailList = PendingTagEmailList.Where(Function(n) n.SendFlag = False).ToList

    End Sub

    Private Sub TimerReconnect_Tick(sender As Object, e As EventArgs) Handles TimerReconnect.Tick
        Dim RowIndex As Integer = 0
        For Each kvp As KeyValuePair(Of String, RFID_Reader_Item) In m_reader_list
            Dim _reader As RFID_Reader_Item = kvp.Value
            If _reader.bool_ReconnectRequired Then
                If Not ReconnectBackgroundWorker.IsBusy Then
                    ReconnectBackgroundWorker.RunWorkerAsync(_reader.str_IPAdress + "-" + RowIndex.ToString())
                End If
            End If
            RowIndex += 1
        Next

        'dto = db.GET_RFID_CONFIG("FILM")
        'Dim ipAddress As String = String.Empty

        'For Each row In dto.Table.Rows
        '    ipAddress = row("IP_ADDRESS").ToString()
        'Next

        'Attempt to resend email
        For Each _email As EmailObject In PendingEmailList

            'Mailer.SendDBEmailMSSQL(ipAddress)
            Mailer.SendDBEmailMSSQL(_email)

        Next

        PendingEmailList = PendingEmailList.Where(Function(n) n.SendFlag = False).ToList

    End Sub


    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        dto = db.RFID_Common_MSSQL("", "A17000000000000000012991", "TestRdr", "127.0.0.1", "SP_TEST_RFID")

        If dto.Error = False Then
            MsgBox("Connect MSSQL Success, Transaction Success.")
            TowerLight_ON(1, "10.28.92.50")
        Else
            MsgBox("Connect MSSQL Success, Transaction Fail.")
            TowerLight_ON(2, "10.28.92.50")
        End If

    End Sub

    Private Sub ReconnectBackgroundWorker_DoWork(sender As Object, e As DoWorkEventArgs) Handles ReconnectBackgroundWorker.DoWork
        Dim arry() As String = e.Argument.ToString().Split("-")
        Dim readerItem As RFID_Reader_Item = m_reader_list(arry(0))
        'Dim currentReader As RFIDReader = readerItem.m_ReaderAPI
        Dim _Success As Boolean = False
        Dim _Result As New returnResult
        Try
            readerItem.m_ReaderAPI.Reconnect()
            _Success = True
        Catch ex As Exception

            Try
                'HANA Change timeout val from 0 to 50secs
                'readerItem.m_ReaderAPI = New RFIDReader(readerItem.str_IPAdress, readerItem.str_Port, 0)
                readerItem.m_ReaderAPI = New RFIDReader(readerItem.str_IPAdress, readerItem.str_Port, 50000)
                readerItem.m_ReaderAPI.Connect()
                _Success = True
            Catch exed As Exception
                'HANA 09112023 Adding email for failed connection
                Dim _reader_object As New ReaderEmailObject(readerItem.str_HostName, readerItem.str_IPAdress, readerItem.str_Name, Now(), "Reconnect failed.")
                ReaderEmailFailList.Add(_reader_object)
                If Not TimerSendFailEmail.Enabled Then
                    TimerSendFailEmail.Start()
                End If

                _Result.rowIndex = Convert.ToInt32(arry(1))
                _Result.Result = ex.Message
                e.Result = _Result
            End Try

        End Try

        If _Success Then
            _Result.rowIndex = Convert.ToInt32(arry(1))
            _Result.Result = "Connect Succeed"
            e.Result = _Result
        End If

    End Sub

    Private Sub ReconnectBackgroundWorker_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles ReconnectBackgroundWorker.RunWorkerCompleted
        Dim _ReturnResult As returnResult = e.Result
        Dim rowIndex As Integer = _ReturnResult.rowIndex
        Dim Result As String = _ReturnResult.Result
        Dim hostName As String = lv_reader.Items(rowIndex).SubItems(1).Text.ToString()
        Dim readerItem As RFID_Reader_Item = m_reader_list(hostName)
        'Dim currentReader As RFIDReader = readerItem.m_ReaderAPI
        Dim antennaList As UShort() = New UShort(1) {1, 2}

        Dim antennaInfo As AntennaInfo = New AntennaInfo(antennaList)

        If (Result = "Connect Succeed") Then

            If Not My.Settings.TEST_ENVIRONMENT Then
                AddHandler readerItem.m_ReaderAPI.Events.ReadNotify, New ReadNotifyHandler(AddressOf Me.Events_ReadNotify)
                readerItem.m_ReaderAPI.Events.AttachTagDataWithReadEvent = False
            End If
            AddHandler readerItem.m_ReaderAPI.Events.StatusNotify, New StatusNotifyHandler(AddressOf Me.Events_StatusNotify)
            readerItem.m_ReaderAPI.Events.NotifyGPIEvent = True
            readerItem.m_ReaderAPI.Events.NotifyBufferFullEvent = True
            readerItem.m_ReaderAPI.Events.NotifyBufferFullWarningEvent = True
            readerItem.m_ReaderAPI.Events.NotifyReaderDisconnectEvent = True
            readerItem.m_ReaderAPI.Events.NotifyReaderExceptionEvent = True
            readerItem.m_ReaderAPI.Events.NotifyAccessStartEvent = True
            readerItem.m_ReaderAPI.Events.NotifyAccessStopEvent = True
            readerItem.m_ReaderAPI.Events.NotifyInventoryStartEvent = True
            readerItem.m_ReaderAPI.Events.NotifyInventoryStopEvent = True
            '.Events.NotifyTemperatureAlarmEvent = True

            Try

                readerItem.m_ReaderAPI.Actions.Inventory.Perform(Nothing, Nothing, antennaInfo)

            Catch operationException As OperationFailureException

                Console.WriteLine(operationException.Result)

            End Try

            readerItem.bool_ReconnectRequired = False

            Dim _reader_object As New ReaderEmailObject(readerItem.str_HostName, readerItem.str_IPAdress, readerItem.str_Name, Now(), "Reconnect successfully.")
            ReaderEmailRCList.Add(_reader_object)

            If Not TimerSendRCEmail.Enabled Then
                TimerSendRCEmail.Start()
            End If

            lv_reader.Items(rowIndex).SubItems(3).Text = "Connected"
            lv_reader.Items(rowIndex).ForeColor = Color.Green

            'txt_log.AppendText($"{vbNewLine}{Date.Now.ToShortTimeString()} - {lv_reader.Items(rowIndex).Text} connect successfully.")
            'txt_log.ScrollToCaret()

            btnConnect.Text = "Disconnect"
            btnConnect.BackColor = Color.Red

        Else
            'Error Handle
            lv_reader.Items(rowIndex).SubItems(3).Text = "Reconnecting"
            lv_reader.Items(rowIndex).ForeColor = Color.Red

            'txt_log.AppendText($"{vbNewLine}{Date.Now.ToShortTimeString()} - Error occur while connect to {lv_reader.Items(rowIndex).Text} : {Result.ToString()}")
            'txt_log.ScrollToCaret()

            'HANA 09112023 Adding email for failed connection
            Dim _reader_object As New ReaderEmailObject(readerItem.str_HostName, readerItem.str_IPAdress, readerItem.str_Name, Now(), "Connect failed. Reconnecting")
            ReaderEmailFailList.Add(_reader_object)
            If Not TimerSendFailEmail.Enabled Then
                TimerSendFailEmail.Start()
            End If

            btnConnect.Text = "Connect"
            btnConnect.BackColor = Color.Lime
        End If
    End Sub


    'Private Sub Timer3_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
    '    resetTimer1()
    'End Sub

    'Private Sub Timer2_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '    resetTimer2()
    'End Sub

    Private Sub resetTimer3()
        Me.m_TagTable.Clear()
        Timer3.Enabled = False
    End Sub

    Private Sub resetTimer2()
        Me.m_TagTable.Clear()
        Timer2.Enabled = False
    End Sub

    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
        resetTimer2()
    End Sub

    Private Sub Timer3_Tick(sender As Object, e As EventArgs) Handles Timer3.Tick
        resetTimer3()
    End Sub

    'HANA 22032024 Releasing locked tag
    Private Sub TimerTagLock_Tick(sender As Object, e As EventArgs) Handles TimerTagLock.Tick
        lockTag = ""
    End Sub

End Class

Public Class TagDetails
    Public hostName As String = String.Empty
    Public location As String = String.Empty
    Public antennaID As String = String.Empty
    Public tagID As String = String.Empty
    Public tagTime As DateTime
End Class


Public Class RFID_Reader_Item

    Friend m_AccessOpResult As AccessOperationResult
    Friend m_ReaderAPI As RFIDReader
    Friend bool_ReconnectRequired As Boolean = False
    Friend int_Index As Integer
    Friend ht_TagDetected As Hashtable
    Friend ht_TagTime As Hashtable
    'Friend ht_TagTable As Hashtable

    Friend str_Name As String
    Friend str_IPAdress As String
    Friend str_HostName As String
    Friend str_Port As String

    Friend str_Stored_Procedure As String
    Friend str_SERVER As String
    Friend str_Stored_Procedure2 As String
    Friend str_SERVER2 As String
    Friend int_RSSI As Integer
End Class
