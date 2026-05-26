Imports Oracle.DataAccess.Client
Imports System.Data.SqlClient

Public Class RFID_CUSTOM

    'Dim ConnectionString_Ora As String = System.Configuration.ConfigurationManager.ConnectionStrings("db_ora").ToString()
    Dim ConnectionString_MS As String = System.Configuration.ConfigurationManager.ConnectionStrings("db_MS").ToString()
    Dim dto As New DTO

    'Use for MS SQL TRANS
    Private pSP_MS As String = String.Empty 'Outdated

    'Public Function RFID_Custom_ORA(RFID As String, Reader As String, IPAddress As String, SP As String) As DTO

    '    dto = New DTO

    '    'DB START HERE
    '    Dim con As New OracleConnection(ConnectionString_Ora)
    '    Dim cmd As New OracleCommand(SP, con)
    '    Dim rdr As OracleDataReader

    '    cmd.CommandType = CommandType.StoredProcedure
    '    cmd.CommandTimeout = 0

    '    cmd.Parameters.Clear()
    '    cmd.Parameters.Add(New OracleParameter("pRFID", RFID)).Direction = ParameterDirection.Input
    '    cmd.Parameters.Add(New OracleParameter("pREADER", Reader)).Direction = ParameterDirection.Input
    '    cmd.Parameters.Add(New OracleParameter("pIPADDR", IPAddress)).Direction = ParameterDirection.Input
    '    cmd.Parameters.Add(New OracleParameter("RETURN_VALUE", Oracle.DataAccess.Client.OracleDbType.Int64, 20)).Direction = ParameterDirection.Output
    '    cmd.Parameters.Add(New OracleParameter("MSG", Oracle.DataAccess.Client.OracleDbType.Varchar2, 1000)).Direction = ParameterDirection.Output
    '    cmd.Parameters.Add(New OracleParameter("SREFData", Oracle.DataAccess.Client.OracleDbType.RefCursor)).Direction = ParameterDirection.Output

    '    Try
    '        con.Open()

    '        rdr = cmd.ExecuteReader()
    '        dto.Table = New DataTable
    '        dto.Table.Load(rdr)

    '        dto.Int = CInt(cmd.Parameters("RETURN_VALUE").Value.ToString)
    '        dto.Str = cmd.Parameters("MSG").Value.ToString

    '        If dto.Int = 5 Then
    '            Throw New Exception(cmd.Parameters("MSG").Value.ToString())
    '        End If

    '        Return dto
    '    Catch ex As Exception
    '        dto.Error = True
    '        dto.ErrorMessage = ex.Message
    '        Return dto
    '    Finally
    '        con.Close()
    '        con.Dispose()
    '    End Try

    'End Function

    'Public Function RFID_Custom_ORA2(Dr As DataRow, Reader As String, IPAddress As String, SP As String) As DTO

    '    dto = New DTO

    '    'DB START HERE
    '    Dim con As New OracleConnection(ConnectionString_Ora)
    '    Dim cmd As New OracleCommand(SP, con)
    '    Dim rdr As OracleDataReader

    '    cmd.CommandType = CommandType.StoredProcedure
    '    cmd.CommandTimeout = 0

    '    cmd.Parameters.Clear()

    '    For Each _col As DataColumn In Dr.Table.Columns

    '        cmd.Parameters.Add(New OracleParameter("p" & _col.ColumnName, Dr(_col.ColumnName).ToString)).Direction = ParameterDirection.Input

    '    Next


    '    cmd.Parameters.Add(New OracleParameter("RETURN_VALUE", Oracle.DataAccess.Client.OracleDbType.Int64, 20)).Direction = ParameterDirection.Output
    '    cmd.Parameters.Add(New OracleParameter("MSG", Oracle.DataAccess.Client.OracleDbType.Varchar2, 1000)).Direction = ParameterDirection.Output
    '    cmd.Parameters.Add(New OracleParameter("SREFData", Oracle.DataAccess.Client.OracleDbType.RefCursor)).Direction = ParameterDirection.Output

    '    Try
    '        con.Open()

    '        rdr = cmd.ExecuteReader()
    '        dto.Table = New DataTable
    '        dto.Table.Load(rdr)

    '        dto.Int = CInt(cmd.Parameters("RETURN_VALUE").Value.ToString)
    '        dto.Str = cmd.Parameters("MSG").Value.ToString

    '        If dto.Int = 5 Then
    '            Throw New Exception(cmd.Parameters("MSG").Value.ToString())
    '        End If

    '        Return dto
    '    Catch ex As Exception
    '        dto.Error = True
    '        dto.ErrorMessage = ex.Message
    '        Return dto
    '    Finally
    '        con.Close()
    '        con.Dispose()
    '    End Try

    'End Function

    Public Function Check_M2_Grey_RFID(RFID As String) As Boolean

        Dim con As New SqlConnection(ConnectionString_MS)
        Dim cmd As New SqlCommand("SP_CHK_M2_GREY_RFID", con)

        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandTimeout = 0
        cmd.Parameters.Clear()
        cmd.Parameters.Add(New SqlParameter("@pRFID", RFID)).Direction = ParameterDirection.Input
        cmd.Parameters.Add(New SqlParameter("@RETURN_VALUE", SqlDbType.VarChar, 1000)).Direction = ParameterDirection.Output

        Try
            con.Open()
            cmd.ExecuteReader()

            If cmd.Parameters("@RETURN_VALUE").Value.ToString = "1" Then
                Check_M2_Grey_RFID = True
            Else
                Check_M2_Grey_RFID = False
            End If

            Return Check_M2_Grey_RFID
        Catch ex As Exception
            Return Check_M2_Grey_RFID = False
        Finally
            con.Close()
            con.Dispose()
        End Try

    End Function

    Public Function RFID_Filter(RFID As String) As Boolean

        Dim con As New SqlConnection(ConnectionString_MS)
        Dim cmd As New SqlCommand("SP_CHK_FILTER_RFID", con)

        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandTimeout = 0
        cmd.Parameters.Clear()
        cmd.Parameters.Add(New SqlParameter("@pRFID", RFID)).Direction = ParameterDirection.Input
        cmd.Parameters.Add(New SqlParameter("@RETURN_VALUE", SqlDbType.VarChar, 1000)).Direction = ParameterDirection.Output

        Try
            con.Open()
            cmd.ExecuteReader()

            If cmd.Parameters("@RETURN_VALUE").Value.ToString = "1" Then
                RFID_Filter = True
            Else
                RFID_Filter = False
            End If

            Return RFID_Filter
        Catch ex As Exception
            Return RFID_Filter = False
        Finally
            con.Close()
            con.Dispose()
        End Try

    End Function

End Class
