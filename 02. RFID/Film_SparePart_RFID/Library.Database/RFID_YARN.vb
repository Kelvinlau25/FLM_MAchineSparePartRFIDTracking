Imports Oracle.DataAccess.Client
Imports System.Data.SqlClient
Public Class RFID_YARN

    Dim dto As New DTO
    Dim db As New Library.Database.RFID_COMMON
    'Dim ConnectionString_YARN As String = System.Configuration.ConfigurationManager.ConnectionStrings("db_OraYarnM1").ToString()
    'Dim ConnectionString_APPS As String = System.Configuration.ConfigurationManager.ConnectionStrings("db_OraApps").ToString()
    Dim ConnectionString_MS As String = System.Configuration.ConfigurationManager.ConnectionStrings("db_MS").ToString()

    Dim ShipmentNumber As String = String.Empty
    Dim ItemNumber As String = String.Empty
    Dim transQty As Double = 0
    Dim palletQty As Double = 0
    Dim perWeight As Double = 0
    Dim perPallet As Double = 0
    Dim perCone As Double = 0
    Dim suppCode As String = String.Empty
    Public Function MainYarnRFID(RFID As String, Reader As String, IPAddress As String, SP As String) As DTO
        MainYarnRFID = New DTO
        If SP = "SP_M2_YARN_RECEIVING_CHECK" Then 'Yarn Receiving
            'MainYarnRFID = CheckYarnDtl(RFID, Reader, IPAddress, SP)
        ElseIf SP = "SP_M2_YARN_ISSUING_MOV_MAINT" Then 'Yarn Issues Out Godown
            MainYarnRFID = UpdYarnInvMov_Iss(RFID, Reader, IPAddress, SP)
        ElseIf SP = "SP_M2_YARN_ISSUING_PROD_REC" Then 'Yarn Issues Out & Received by Production
            MainYarnRFID = UpdYarnMov_ProdRec(RFID, Reader, IPAddress, SP)
        ElseIf SP = "SP_TEST_RFID" Then
            MainYarnRFID = db.RFID_Common_MSSQL("", RFID, Reader, IPAddress, SP)
        End If

    End Function


    Public Function Get_ImportYarn_Dtl(RFID As String) As DTO
        Get_ImportYarn_Dtl = New DTO
        Dim _dt As New Data.DataTable
        Dim con As New SqlConnection(ConnectionString_MS)
        Dim cmd As New SqlCommand("SP_GET_M2RFID_IMPORT_YARN", con)
        Dim _rdr As SqlDataReader

        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandTimeout = 0
        cmd.Parameters.Clear()
        cmd.Parameters.Add(New SqlParameter("@pRFID", RFID)).Direction = ParameterDirection.Input
        cmd.Parameters.Add(New SqlParameter("@RETURN_VALUE", SqlDbType.VarChar, 1000)).Direction = ParameterDirection.Output

        Try
            con.Open()
            _rdr = cmd.ExecuteReader()
            _dt.Load(_rdr)

            If cmd.Parameters("@RETURN_VALUE").Value.ToString = "0" Then
                Throw New Exception("No Data Found on Import Yarn")
            End If

            'HANA loop change
            Dim InvoiceNo As String
            Dim ITEM_NUMBER As String
            Dim WEIGHT As String
            Dim CONES As String
            Dim SUPP As String

            If _dt.Rows.Count > 0 Then
                For Each _dr As DataRow In _dt.Rows
                    InvoiceNo = _dr("INVOICE_NO").ToString
                    ITEM_NUMBER = _dr("ITEM_CODE").ToString
                    WEIGHT = _dr("WEIGHT").ToString
                    CONES = _dr("UNIT_QTY").ToString
                    SUPP = _dr("SUPPLIER_CODE").ToString

                    ShipmentNumber = InvoiceNo
                    ItemNumber = ITEM_NUMBER
                    perWeight = WEIGHT
                    perCone = CONES
                    suppCode = SUPP
                Next

                Get_ImportYarn_Dtl.Error = False
            Else
                Get_ImportYarn_Dtl.Error = True
            End If


        Catch ex As Exception
            Get_ImportYarn_Dtl.Error = True
            Get_ImportYarn_Dtl.ErrorMessage = ex.Message
        Finally
            con.Close()
            con.Dispose()
        End Try

    End Function



    Public Function UpdYarnInvMov_Rec(RFID As String, Reader As String, IPAddress As String, InvoiceNo As String,
                                     ItemCode As String, Source As String, PltQty As Double, NoOfPlt As Double, TotalCone As Double) As DTO
        UpdYarnInvMov_Rec = New DTO
        dto = New DTO
        Dim con As New SqlConnection(ConnectionString_MS)
        Dim cmd As New SqlCommand("SP_M2_YARN_RECEIVING_MOV_MAINT", con)

        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandTimeout = 0
        cmd.Parameters.Clear()
        cmd.Parameters.Add(New SqlParameter("@pRFID", RFID)).Direction = ParameterDirection.Input
        cmd.Parameters.Add(New SqlParameter("@pREADER", Reader)).Direction = ParameterDirection.Input
        cmd.Parameters.Add(New SqlParameter("@pIPADDR", IPAddress)).Direction = ParameterDirection.Input
        cmd.Parameters.Add(New SqlParameter("@pInvoiceNo", InvoiceNo)).Direction = ParameterDirection.Input
        cmd.Parameters.Add(New SqlParameter("@pITEMCODE", ItemCode)).Direction = ParameterDirection.Input
        cmd.Parameters.Add(New SqlParameter("@pSOURCE", Source)).Direction = ParameterDirection.Input
        cmd.Parameters.Add(New SqlParameter("@pPLTQTY", PltQty)).Direction = ParameterDirection.Input
        cmd.Parameters.Add(New SqlParameter("@pNoOfPlt", NoOfPlt)).Direction = ParameterDirection.Input
        cmd.Parameters.Add(New SqlParameter("@pTOTALCONE", TotalCone)).Direction = ParameterDirection.Input
        cmd.Parameters.Add(New SqlParameter("@RETURN_VALUE", SqlDbType.VarChar, 1000)).Direction = ParameterDirection.Output

        Try
            con.Open()
            cmd.ExecuteReader()

            If cmd.Parameters("@RETURN_VALUE").Value.ToString = "0" Then
                Throw New Exception("Fail Update Yarn Movemnt & Yarn Inventory")
            End If

            UpdYarnInvMov_Rec.Error = False

            Return UpdYarnInvMov_Rec
        Catch ex As Exception
            UpdYarnInvMov_Rec.Error = True
            UpdYarnInvMov_Rec.ErrorMessage = ex.Message
            Return UpdYarnInvMov_Rec
        Finally
            con.Close()
            con.Dispose()
        End Try

    End Function

    Public Function UpdYarnInvMov_Iss(RFID As String, Reader As String, IPAddress As String, SP As String) As DTO
        UpdYarnInvMov_Iss = New DTO

        Dim con As New SqlConnection(ConnectionString_MS)
        Dim cmd As New SqlCommand(SP, con) 'SP_M2_YARN_ISSUING_MOV_MAINT

        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandTimeout = 0
        cmd.Parameters.Clear()
        cmd.Parameters.Add(New SqlParameter("@pRFID", RFID)).Direction = ParameterDirection.Input
        cmd.Parameters.Add(New SqlParameter("@pREADER", Reader)).Direction = ParameterDirection.Input
        cmd.Parameters.Add(New SqlParameter("@pIPADDR", IPAddress)).Direction = ParameterDirection.Input
        cmd.Parameters.Add(New SqlParameter("@RETURN_VALUE", SqlDbType.VarChar, 1000)).Direction = ParameterDirection.Output

        Try
            con.Open()
            cmd.ExecuteReader()

            If cmd.Parameters("@RETURN_VALUE").Value.ToString = "0" Then
                Throw New Exception("Fail Update Yarn Movemnt & Yarn Inventory")
            End If

            UpdYarnInvMov_Iss.Error = False

            Return UpdYarnInvMov_Iss
        Catch ex As Exception
            UpdYarnInvMov_Iss.Error = True
            UpdYarnInvMov_Iss.ErrorMessage = ex.Message
            Return UpdYarnInvMov_Iss
        Finally
            con.Close()
            con.Dispose()
        End Try

    End Function

    Public Function UpdYarnMov_ProdRec(RFID As String, Reader As String, IPAddress As String, SP As String) As DTO
        UpdYarnMov_ProdRec = New DTO

        Dim con As New SqlConnection(ConnectionString_MS)
        Dim cmd As New SqlCommand(SP, con) 'SP_M2_YARN_ISSUING_PROD_REC

        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandTimeout = 0
        cmd.Parameters.Clear()
        cmd.Parameters.Add(New SqlParameter("@pRFID", RFID)).Direction = ParameterDirection.Input
        cmd.Parameters.Add(New SqlParameter("@pREADER", Reader)).Direction = ParameterDirection.Input
        cmd.Parameters.Add(New SqlParameter("@pIPADDR", IPAddress)).Direction = ParameterDirection.Input
        cmd.Parameters.Add(New SqlParameter("@RETURN_VALUE", SqlDbType.VarChar, 1000)).Direction = ParameterDirection.Output

        Try
            con.Open()
            cmd.ExecuteReader()

            If cmd.Parameters("@RETURN_VALUE").Value.ToString = "0" Then
                Throw New Exception("Fail Update Yarn Movemnt & Raw Material Requisition")
            End If

            UpdYarnMov_ProdRec.Error = False

            Return UpdYarnMov_ProdRec
        Catch ex As Exception
            UpdYarnMov_ProdRec.Error = True
            UpdYarnMov_ProdRec.ErrorMessage = ex.Message
            Return UpdYarnMov_ProdRec
        Finally
            con.Close()
            con.Dispose()
        End Try

    End Function

End Class
