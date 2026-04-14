
Imports System.Net
Imports Symbol.RFID3
Imports Symbol.RFID3.Events

#Region "Experimental"

Namespace Objects

    Public Class ReaderObject

        Public Sub New(ByVal index As Integer, ByVal host_name As String, ByVal port As String)

            Me.Index = index

            Me.HostName = host_name

            Me.Port = port

            'HANA Change timeout val from 0 to 50secs
            'Me.MyReader = New RFIDReader(Me.HostName, Me.Port, 0)
            Me.MyReader = New RFIDReader(Me.HostName, Me.Port, 50000)

        End Sub

        Public Sub ConnectReader()
            Dim Status As Boolean = False
            Dim Message As String = String.Empty
            Try

                If Me.ReconnectRequired Then
                    Me.MyReader.Reconnect()
                Else
                    Me.MyReader.Connect()
                End If

                Status = True

                'AddHandler Me.MyReader.Events.ReadNotify, New ReadNotifyHandler(AddressOf Me.Events_ReadNotify)
                'reader.Events.AttachTagDataWithReadEvent = False
                AddHandler Me.MyReader.Events.StatusNotify, New StatusNotifyHandler(AddressOf Me.Events_StatusNotify)
                Me.MyReader.Events.NotifyGPIEvent = True
                Me.MyReader.Events.NotifyReaderDisconnectEvent = True
                Me.MyReader.Events.NotifyAccessStartEvent = True
                Me.MyReader.Events.NotifyAccessStopEvent = True
                Me.MyReader.Events.NotifyInventoryStartEvent = True
                Me.MyReader.Events.NotifyInventoryStopEvent = True

                'Me.OffLight()

            Catch operationException As OperationFailureException

                Message = "Connect Failed : " & operationException.Result

            Catch socketException As Sockets.SocketException

                Message = "Connect Failed [" & Me.HostName & "]: " & socketException.Message

            Catch ex As Exception

                Message = "Connect Failed [" & Me.HostName & "]: " & ex.Message

            End Try

            Me.ConnectResult = New Result(Status, Message)

        End Sub

        Public Sub StartRead()

            'm_ReaderList(index).Actions.Inventory.Perform()

            ' perform inventory on antenna 1 & 2
            Dim antennaList As UShort() = New UShort(1) {1, 2}

            Dim antennaInfo As AntennaInfo = New AntennaInfo(antennaList)

            Me.MyReader.Actions.Inventory.Perform(Nothing, Nothing, antennaInfo)

        End Sub
        Public Property Index As Integer
        Public Property MyReader As RFIDReader
        Public Property ReconnectRequired As Boolean = False
        Public Property TagDetected As Hashtable
        Public Property TagTime As Hashtable
        Public Property ConnectResult As Result
        Public Property Name As String
        Public Property IPAdress As String
        Public Property HostName As String
        Public Property Port As String
        Public Property StoredProcedure As String
        Public Property Server As String
        Public Property StoredProcedure2 As String
        Public Property Server2 As String

        Private m_UpdateReadHandler As UpdateRead = New UpdateRead(AddressOf Me.myUpdateRead)

        Private m_UpdateStatusHandler As UpdateStatus = New UpdateStatus(AddressOf Me.myUpdateStatus)

        Private Delegate Sub UpdateRead(ByVal eventData As ReadEventData)

        Private Delegate Sub UpdateStatus(ByVal eventData As StatusEventData)

        Private Sub Events_ReadNotify(ByVal sender As Object, ByVal readEventArgs As ReadEventArgs)
            Try
                Dim ReaderHostName As String = CType(sender, Symbol.RFID3.Events).HostName
                'MyBase.Invoke(Me.m_UpdateReadHandler, New Object() {ReaderHostName, readEventArgs.ReadEventData.TagData})
                m_UpdateReadHandler.Invoke(readEventArgs.ReadEventData)
            Catch exception1 As Exception
            End Try
        End Sub

        Public Sub Events_StatusNotify(ByVal sender As Object, ByVal statusEventArgs As StatusEventArgs)
            Try
                Dim ReaderHostName As String = CType(sender, Symbol.RFID3.Events).HostName
                'MyBase.Invoke(Me.m_UpdateStatusHandler, New Object() {ReaderHostName, statusEventArgs.StatusEventData})
                m_UpdateStatusHandler.Invoke(statusEventArgs.StatusEventData)
            Catch exception1 As Exception
            End Try
        End Sub

        Private Sub myUpdateRead(ByVal eventData As ReadEventData)
        End Sub

        Private Sub myUpdateStatus(ByVal eventData As Events.StatusEventData)
            Dim StatusMsg As String = ""

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
                    'myUpdateRead(Nothing)
                    Exit Select
                Case Symbol.RFID3.Events.STATUS_EVENT_TYPE.BUFFER_FULL_EVENT
                    StatusMsg = "Buffer full"
                    'myUpdateRead(Nothing)
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
                Case Else
                    StatusMsg = "Unhandled Status"
                    Exit Select

            End Select

            If Not Me.MyReader.IsConnected Then

                Try

                    Me.MyReader.Reconnect()

                    StatusMsg &= ", Reconnect success."

                Catch ex As Exception

                    StatusMsg &= ", Reconnect fail."

                    Me.ReconnectRequired = True


                End Try

            End If

            Console.WriteLine(StatusMsg)

        End Sub

    End Class

    Public Class Result

        Public Sub New(ByVal status As Boolean, ByVal message As String)
            Me.Status = status
            Me.Message = message
        End Sub

        Public Property Status As Boolean = False
        Public Property Message As String
    End Class

End Namespace


#End Region
