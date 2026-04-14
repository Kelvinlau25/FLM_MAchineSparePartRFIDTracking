Imports Oracle.DataAccess.Client
Imports System.Data.SqlClient

Public Class RFID_COMMON

    'Dim ConnectionString_Ora As String = System.Configuration.ConfigurationManager.ConnectionStrings("db_ora").ToString()
    Dim ConnectionString_MS As String = System.Configuration.ConfigurationManager.ConnectionStrings("db_MS").ToString()
    Dim ConnectionString_MS_Tower As String = System.Configuration.ConfigurationManager.ConnectionStrings("db_MS_Tower").ToString()
    Dim dto As New DTO

    'Use for MS SQL TRANS
    Private pSP_MS As String = String.Empty 'Outdated

    Public Function RFID_Common_MSSQL(mode As String, RFID As String, Reader As String, IPAddress As String, SP As String) As DTO

        dto = New DTO
        Dim con As New SqlConnection(ConnectionString_MS)
        Dim cmd As New SqlCommand(SP, con)

        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandTimeout = 0
        cmd.Parameters.Clear()
        cmd.Parameters.Add(New SqlParameter("@pTRAN_TYPE", mode)).Direction = ParameterDirection.Input
        cmd.Parameters.Add(New SqlParameter("@pRFID", RFID)).Direction = ParameterDirection.Input
        cmd.Parameters.Add(New SqlParameter("@pRETURN_VALUE1", SqlDbType.NVarChar, 4000)).Direction = ParameterDirection.Output
        'cmd.Parameters.Add(New SqlParameter("@pRFID", RFID)).Direction = ParameterDirection.Input
        'cmd.Parameters.Add(New SqlParameter("@pREADER", Reader)).Direction = ParameterDirection.Input
        cmd.Parameters.Add(New SqlParameter("@pIPADDR", IPAddress)).Direction = ParameterDirection.Input
        'cmd.Parameters.Add(New SqlParameter("@RETURN_ERROR", SqlDbType.VarChar, 1000)).Direction = ParameterDirection.Output

        Try
            con.Open()
            cmd.ExecuteNonQuery()

            If cmd.Parameters("@pRETURN_VALUE1").Value.ToString <> "0" Then
                Throw New Exception(cmd.Parameters("@pRETURN_VALUE1").Value.ToString())
            End If

            dto.Error = False
            Return dto
        Catch ex As Exception
            dto.Error = True
            dto.ErrorMessage = ex.Message
            Return dto
        Finally
            con.Close()
            con.Dispose()
        End Try

    End Function

    Public Function RFID_Common_MSSQL_Tower(mode As String, RFID As String, Reader As String, IPAddress As String, SP As String) As DTO

        dto = New DTO
        Dim con As New SqlConnection(ConnectionString_MS_Tower)
        Dim cmd As New SqlCommand(SP, con)

        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandTimeout = 0
        cmd.Parameters.Clear()
        cmd.Parameters.Add(New SqlParameter("@pTRAN_TYPE", mode)).Direction = ParameterDirection.Input
        cmd.Parameters.Add(New SqlParameter("@pRFID", RFID)).Direction = ParameterDirection.Input
        cmd.Parameters.Add(New SqlParameter("@pRETURN_VALUE1", SqlDbType.NVarChar, 4000)).Direction = ParameterDirection.Output
        'cmd.Parameters.Add(New SqlParameter("@pRFID", RFID)).Direction = ParameterDirection.Input
        'cmd.Parameters.Add(New SqlParameter("@pREADER", Reader)).Direction = ParameterDirection.Input
        'cmd.Parameters.Add(New SqlParameter("@pIPADDR", IPAddress)).Direction = ParameterDirection.Input
        'cmd.Parameters.Add(New SqlParameter("@RETURN_ERROR", SqlDbType.VarChar, 1000)).Direction = ParameterDirection.Output

        Try
            con.Open()
            cmd.ExecuteNonQuery()

            If cmd.Parameters("@pRETURN_VALUE1").Value.ToString <> "0" Then
                Throw New Exception(cmd.Parameters("@pRETURN_VALUE1").Value.ToString())
            End If

            dto.Error = False
            Return dto
        Catch ex As Exception
            dto.Error = True
            dto.ErrorMessage = ex.Message
            Return dto
        Finally
            con.Close()
            con.Dispose()
        End Try

    End Function


    Public Function RFID_Common_MSSQL2(mode As String, RFID As String, SP As String) As DTO

        dto = New DTO
        Dim con As New SqlConnection(ConnectionString_MS)
        Dim cmd As New SqlCommand(SP, con)

        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandTimeout = 0
        cmd.Parameters.Clear()
        cmd.Parameters.Add(New SqlParameter("@pTRAN_TYPE", mode)).Direction = ParameterDirection.Input
        cmd.Parameters.Add(New SqlParameter("@pRFID", RFID)).Direction = ParameterDirection.Input
        cmd.Parameters.Add(New SqlParameter("@pRETURN_VALUE1", SqlDbType.NVarChar, 4000)).Direction = ParameterDirection.Output
        'cmd.Parameters.Add(New SqlParameter("@pRFID", RFID)).Direction = ParameterDirection.Input
        'cmd.Parameters.Add(New SqlParameter("@pREADER", Reader)).Direction = ParameterDirection.Input
        'cmd.Parameters.Add(New SqlParameter("@pIPADDR", IPAddress)).Direction = ParameterDirection.Input
        'cmd.Parameters.Add(New SqlParameter("@RETURN_ERROR", SqlDbType.VarChar, 1000)).Direction = ParameterDirection.Output

        Try
            con.Open()
            cmd.ExecuteNonQuery()

            If cmd.Parameters("@pRETURN_VALUE1").Value.ToString <> "0" Then
                Throw New Exception(cmd.Parameters("@pRETURN_VALUE1").Value.ToString())
            End If

            dto.Error = False
            Return dto
        Catch ex As Exception
            dto.Error = True
            dto.ErrorMessage = ex.Message
            Return dto
        Finally
            con.Close()
            con.Dispose()
        End Try

    End Function


    Public Function RFID_SendMail_MSSQL(ByVal MailBody As String, ByVal MailSubject As String, ByVal MailTo As String) As DTO

        dto = New DTO

        'DB START HERE
        Dim con As New SqlConnection(ConnectionString_MS)
        Dim cmd As New SqlCommand("SEND_HTML_EMAIL2", con)

        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandTimeout = 0

        cmd.Parameters.Clear()
        cmd.Parameters.Add(New SqlParameter("@subject", MailSubject)).Direction = Data.ParameterDirection.Input
        cmd.Parameters.Add(New SqlParameter("@MSG", MailBody)).Direction = Data.ParameterDirection.Input
        cmd.Parameters.Add(New SqlParameter("@emailto", MailTo)).Direction = Data.ParameterDirection.Input
        'cmd.Parameters.Add(New SqlParameter("@p_from", MailFrom)).Direction = Data.ParameterDirection.Input
        'cmd.Parameters.Add(New SqlParameter("@p_smtp_hostname", SmtpHostName)).Direction = Data.ParameterDirection.Input
        'cmd.Parameters.Add(New SqlParameter("@p_smtp_portnum", SmtpPort)).Direction = Data.ParameterDirection.Input

        Try
            con.Open()
            cmd.ExecuteReader()

            dto.Error = False
            Return dto
        Catch ex As Exception
            dto.Error = True
            dto.ErrorMessage = ex.Message
            Return dto
        Finally
            con.Close()
            con.Dispose()
        End Try

    End Function

    Public Function GET_RFID_CONFIG(vCompany As String) As DTO

        dto = New DTO
        Dim con As New SqlConnection(ConnectionString_MS)
        Dim cmd As New SqlCommand("SP_FILM_GET_RFID_CONFIG", con)

        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandTimeout = 0
        cmd.Parameters.Clear()
        cmd.Parameters.Add(New SqlParameter("@pCOMPANY", vCompany)).Direction = ParameterDirection.Input

        Try
            con.Open()
            Dim tbl As New DataTable
            tbl.Load(cmd.ExecuteReader)
            dto.Table = tbl
            dto.Error = False
            Return dto
        Catch ex As Exception
            dto.Error = True
            dto.ErrorMessage = ex.Message
            Return dto
        Finally
            con.Close()
            con.Dispose()
        End Try

    End Function

End Class
