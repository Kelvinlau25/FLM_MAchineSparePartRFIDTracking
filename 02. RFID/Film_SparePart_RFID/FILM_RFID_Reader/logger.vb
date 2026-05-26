Imports System.IO
Public Class logger
    Private strFile As String = Application.StartupPath & "\Log\ErrorLog_" & DateTime.Today.ToString("dd-MMM-yyyy") & ".txt"

    Public Sub _LogRead(ByVal tagid As String, ByVal reader As String, ByVal indicator As String, ByVal no_of_rfid As String)
        Dim fileExists As Boolean = File.Exists(strFile)
        Using writer As New StreamWriter(strFile, True)
            If Not fileExists Then
                writer.WriteLine("Starting Error Log for today " & DateTime.Now.ToString("dd/MM/yy hh:mm:ss"))
            End If
            writer.WriteLine("Read: " & tagid & "-->" & "Reader: " & reader & "-" & DateTime.Now.ToString("dd/MM/yy hh:mm:ss") & " Indicator: " & indicator)
            writer.WriteLine("No of RFID Tag in loop : " & no_of_rfid)

        End Using
    End Sub

    Public Sub _LogNoOfTag(ByVal no_of_rfid As String)
        Dim fileExists As Boolean = File.Exists(strFile)
        Using writer As New StreamWriter(strFile, True)
            If Not fileExists Then
                writer.WriteLine("Starting Error Log for today " & DateTime.Now.ToString("dd/MM/yy hh:mm:ss"))
            End If
            writer.WriteLine("No of RFID Tag outside loop : " & no_of_rfid & " - " & DateTime.Now.ToString("dd/MM/yy hh:mm:ss"))

        End Using
    End Sub

    Public Sub _LogDetect(ByVal status As String, ByVal reader As String, ByVal indicator As String)
        Dim fileExists As Boolean = File.Exists(strFile)
        Using writer As New StreamWriter(strFile, True)
            If Not fileExists Then
                writer.WriteLine("Starting Error Log for today " & DateTime.Now.ToString("dd/MM/yy hh:mm:ss"))
            End If
            writer.WriteLine("Forklift Detected: " & status & "-->" & "Reader: " & reader & "-" & DateTime.Now.ToString("dd/MM/yy hh:mm:ss") & " Indicator: " & indicator)

        End Using
    End Sub

    Public Sub _LogCheck(ByVal status As String)
        Dim fileExists As Boolean = File.Exists(strFile)
        Using writer As New StreamWriter(strFile, True)
            If Not fileExists Then
                writer.WriteLine("Starting Error Log for today " & DateTime.Now.ToString("dd/MM/yy hh:mm:ss"))
            End If
            writer.WriteLine("MU Gate Check: " & status & "-" & DateTime.Now.ToString("dd/MM/yy hh:mm:ss"))

        End Using
    End Sub

    Public Sub _LogTriggerGPI(ByVal action As String, ByVal value As String, ByVal hostname As String, ByVal eventData As Boolean)
        Dim fileExists As Boolean = File.Exists(strFile)
        Using writer As New StreamWriter(strFile, True)
            If Not fileExists Then
                writer.WriteLine("Starting Error Log for today " & DateTime.Now.ToString("dd/MM/yy hh:mm:ss"))
            End If
            writer.WriteLine("Trigger GPI Event: - " & DateTime.Now.ToString("dd/MM/yy hh:mm:ss") & " - Action : " & action & " - eventData.GPIEventData.PortNumber : " & value & " - Hostname : " & hostname & " - eventData.GPIEventData.GPIEvent : " & eventData.ToString)

        End Using
    End Sub

    Public Sub _LogResetMU(ByVal msg As String)
        Dim fileExists As Boolean = File.Exists(strFile)
        Using writer As New StreamWriter(strFile, True)
            If Not fileExists Then
                writer.WriteLine("Starting Error Log for today " & DateTime.Now.ToString("dd/MM/yy hh:mm:ss"))
            End If
            writer.WriteLine("Message : " & msg & " - " & DateTime.Now.ToString("dd/MM/yy hh:mm:ss"))

        End Using
    End Sub

    'HANA 05062024 Adding in additional logs.
    Public Sub _LogGen(ByVal msg As String)
        Dim fileExists As Boolean = File.Exists(strFile)
        Using writer As New StreamWriter(strFile, True)
            If Not fileExists Then
                writer.WriteLine("Starting Error Log for today " & DateTime.Now.ToString("dd/MM/yy hh:mm:ss"))
            End If
            writer.WriteLine(msg & " - " & DateTime.Now.ToString("dd/MM/yy hh:mm:ss"))
        End Using
    End Sub

    'END HANA 05062024
End Class
