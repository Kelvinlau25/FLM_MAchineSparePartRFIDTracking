<%@ WebHandler Language="VB" Class="ExportMain" %>

'Imports System
'Imports System.Web
'Imports System.Data
'Imports System.Data.SqlClient
'Imports System.Configuration
'Imports CrystalDecisions.CrystalReports.Engine
'Imports CrystalDecisions.Shared
''Imports Oracle.DataAccess.Client
'Imports System.Web.SessionState
'Imports System.Web.Configuration
'Imports ThoughtWorks.QRCode.Codec
'Imports System.Drawing
'Imports System.Drawing.Drawing2D
'Imports System.IO
'Imports iTextSharp.text
'Imports iTextSharp.text.pdf
'Imports iTextSharp.tool.xml
'Imports iTextSharp.tool.xml.css
'Imports iTextSharp.tool.xml.html
'Imports iTextSharp.tool.xml.parser
'Imports iTextSharp.tool.xml.pipeline
'Imports iTextSharp.tool.xml.pipeline.css
'Imports iTextSharp.tool.xml.pipeline.end
'Imports iTextSharp.tool.xml.pipeline.html
'Imports Oracle.ManagedDataAccess.Client

'''' <summary>
'''' Generate the Report
'''' --------------------------------------------
'''' 22 Nov 2011  C.C.Yeon Initial Version
'''' </summary>
'''' <remarks></remarks>
'Public Class ExportMain : Implements IHttpHandler, System.Web.SessionState.IRequiresSessionState
'    ''' <summary>
'    ''' Collect the data from the URL and generate the report
'    ''' </summary>
'    Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
'        Dim _Parameter As String = context.Request.QueryString("P")
'        Dim _Value As String = context.Request.QueryString("V")
'        Dim _Type As String = context.Request.QueryString("T")
'        Dim _Name As String = context.Request.QueryString("N")
'        Dim _conName = context.Request.QueryString("C")
'        context.Response.ContentType = "application/octet-stream"

'        Print_Barcode(_Value)
'        'printReport(context, _Parameter, _Value, _Type, _conName, _Name) ' Commented by ANAS on 27/06/2020 (Replaced by printHTML5Report method)
'        printHTML5Report(context, _Parameter, _Value, _Type, _Name)
'    End Sub

'    Private Sub Print_Barcode(ByVal barcode As String)
'        Dim myconnection As New System.Data.OleDb.OleDbConnection("Provider=SQLOLEDB.1;Persist Security Info=False;User ID=PAB_SampInv;password=Sam58Invent#;Initial Catalog=PAB_SampleInv;Data Source=10.201.1.5,49818;")

'        If barcode.Length > 0 Then
'            Dim value1 As String = ""
'            Dim value2 As String = ""
'            Dim value3 As String = ""
'            'Dim reader As StreamReader = New StreamReader(My.Application.Info.DirectoryPath & "\barcode_value.txt")
'            'value1 = reader.ReadLine
'            'value2 = reader.ReadLine
'            'value3 = reader.ReadLine
'            'reader.Close()


'            Dim dll As QRCodeEncoder = New QRCodeEncoder
'            Dim image As System.Drawing.Image
'            image = dll.Encode(barcode)
'            Dim sqlquery As String = ""
'            Dim myCommand As System.Data.OleDb.OleDbCommand
'            Dim image_data1 As Byte() = convertPicBoxImageToByte(image)
'            Dim request As String = ""
'            myconnection.Open()

'            'sqlquery = "DELETE From Temp_QR_Code where created_date < '" & DateTime.Now.AddMinutes(-1).ToString("yyyy-MM-dd HH:mm") & "' "
'            'myCommand = New System.Data.OleDb.OleDbCommand(sqlquery, myconnection)
'            'myCommand.CommandText = sqlquery
'            'myCommand.ExecuteNonQuery()
'            'myCommand.Dispose()

'            request = barcode.ToString

'            sqlquery = "update sample_adhoc_master  set QR = ? where sample_request_no = '" & request & "'"
'            myCommand = New System.Data.OleDb.OleDbCommand(sqlquery, myconnection)
'            myCommand.CommandText = sqlquery
'            myCommand.Parameters.Clear()
'            myCommand.Parameters.Add("?P1", System.Data.OleDb.OleDbType.Binary)
'            myCommand.Parameters("?P1").Value = image_data1
'            myCommand.ExecuteNonQuery()
'            myCommand.Dispose()
'            myconnection.Dispose()
'            image_data1 = Nothing
'        End If
'    End Sub

'    Private Function convertPicBoxImageToByte(ByVal image As System.Drawing.Image) As Byte()
'        Dim ms As MemoryStream = New MemoryStream
'        image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg)
'        Return (ms.ToArray)
'    End Function

'    ''' <summary>
'    ''' Generate The Report
'    ''' </summary>
'    Public Sub printReport(ByVal context As HttpContext, ByVal _Parameter As String, ByVal _Value As String, ByVal _Type As String, ByVal _ConnectionStringName As String, ByVal _Name As String)
'        Dim cr As ReportDocument
'        Dim _reportname As String = String.Empty
'        Dim _DiskOpts As New DiskFileDestinationOptions
'        Dim _data As New TableLogOnInfo
'        Dim _datestr As String = Now.ToString("ddMMMyyyyhhmmss")
'        'Dim _conInfo As New SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings(_ConnectionStringName).ConnectionString)

'        Dim Parameter As String() = Nothing
'        Dim Value As String() = Nothing

'        context.Response.Buffer = False

'        If (cr IsNot Nothing) Then
'            cr.Close()
'            cr.Dispose()
'        Else
'            cr = New ReportDocument
'        End If

'        'If _Name.ToLower.IndexOf("rptweaverdefectsummary") >= 0 Then
'        '    Dim Value2 As String() = Nothing
'        '    Dim Y As Integer = 0
'        '    For Each word As String In _Value.Split("|")
'        '        ReDim Preserve Value2(Y)
'        '        Value2(Y) = word.ToString
'        '        Y += 1
'        '    Next
'        '    rptWeaverDefectSummary(Value2(0), Value2(1), ConfigurationManager.ConnectionStrings(_ConnectionStringName).ConnectionString)
'        'ElseIf _Name.ToLower.IndexOf("rptdyerdefectanalysis") >= 0 Then
'        '    Dim Value2 As String() = Nothing
'        '    Dim Y As Integer = 0
'        '    For Each word As String In _Value.Split("|")
'        '        ReDim Preserve Value2(Y)
'        '        Value2(Y) = word.ToString
'        '        Y += 1
'        '    Next
'        '    rptDyerDefectAnalysis(Value2(0), Value2(1), ConfigurationManager.ConnectionStrings(_ConnectionStringName).ConnectionString)
'        'End If

'        Using cr
'            'load the report template
'            cr.Load(HttpContext.Current.Server.MapPath(_Name))
'            _reportname = _datestr

'            'Bind the connection into report
'            '_data.ConnectionInfo.UserID = _conInfo.UserID
'            '_data.ConnectionInfo.Password = _conInfo.Password
'            '_data.ConnectionInfo.ServerName = _conInfo.DataSource

'            'If _conInfo.InitialCatalog <> String.Empty Then
'            '    _data.ConnectionInfo.DatabaseName = _conInfo.InitialCatalog
'            'End If

'            _data.ConnectionInfo.ServerName = "CLD-TGM-MDB01\PABDB,49818" '"PAB_EBIZDBA"
'            _data.ConnectionInfo.UserID = "PAB_SampInv"
'            _data.ConnectionInfo.Password = "Sam58Invent#"
'            _data.ConnectionInfo.DatabaseName = "PAB_SampleInv"

'            'For intcounter = 0 To cr.Database.Tables.Count - 1
'            '    cr.Database.Tables(intcounter).ApplyLogOnInfo(_data)
'            'Next

'            Dim CrTables As Tables = cr.Database.Tables
'            Dim crtablelogoninfo As New TableLogOnInfo

'            For Each CrTable As Table In CrTables

'                crtablelogoninfo = CrTable.LogOnInfo
'                crtablelogoninfo.ConnectionInfo = _data.ConnectionInfo
'                CrTable.ApplyLogOnInfo(crtablelogoninfo)

'            Next



'            '    foreach(CrystalDecisions.CrystalReports.Engine.Table CrTable In CrTables)
'            '{
'            '    crtablelogoninfo = CrTable.LogOnInfo;
'            '    crtablelogoninfo.ConnectionInfo = crconnectioninfo;
'            '    CrTable.ApplyLogOnInfo(crtablelogoninfo);
'            '}

'            'Get the parameter
'            Dim v As Integer = 0
'            For Each word As String In _Parameter.Split("|")
'                ReDim Preserve Parameter(v)
'                Parameter(v) = word.ToString
'                v += 1
'            Next

'            'Get the value
'            Dim Y As Integer = 0
'            For Each word As String In _Value.Split("|")
'                ReDim Preserve Value(Y)
'                Value(Y) = word.ToString
'                Y += 1
'            Next

'            'Set the parameter and the value into report
'            'If _Parameter.Trim <> "" Then
'            '    For i As Integer = 0 To Parameter.Length - 1

'            '        cr.ParameterFields.Item(Parameter(i)).CurrentValues.Clear()
'            '        cr.ParameterFields.Item(Parameter(i)).CurrentValues.AddValue(Value(i).ToString)
'            '    Next
'            'End If

'            If _Parameter.Trim <> "" Then
'                For i As Integer = 0 To Parameter.Length - 1

'                    For j As Integer = 0 To Parameter.Length - 1

'                        If cr.ParameterFields.Item(j).Name.ToUpper().Equals(Parameter(i).ToUpper()) Then
'                            cr.ParameterFields.Item(j).CurrentValues.Clear()
'                            cr.ParameterFields.Item(j).CurrentValues.AddValue(Value(i).ToString)
'                        End If


'                    Next

'                    'cr.ParameterFields.Item(Parameter(i)).CurrentValues.Clear()
'                    'cr.ParameterFields.Item(Parameter(i)).CurrentValues.AddValue(Value(i).ToString)
'                Next
'            End If

'            'cr.ParameterFields.Item(0).CurrentValues.Clear()
'            'cr.ParameterFields.Item(0).CurrentValues.AddValue(Value(0).ToString)



'            'export the report according the selected type(PDF or Excel)
'            Dim str = New System.IO.MemoryStream
'            Dim bool As Boolean = False
'            'export the report according the selected type(PDF or Excel)
'            If _Type = 0 Then
'                'cr.ExportToHttpResponse(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, context.Response, True, _reportname)
'                context.Response.ContentType = "application/pdf"

'                context.Response.AddHeader("Content-disposition", "attachment;filename=" & _reportname & ".pdf")

'                str = cr.ExportToStream(ExportFormatType.PortableDocFormat)
'                bool = True
'            Else
'                'cr.ExportToHttpResponse(CrystalDecisions.Shared.ExportFormatType.Excel, context.Response, False, _reportname)
'                context.Response.ContentType = "application/xls"

'                context.Response.AddHeader("Content-disposition", "filename=" & _reportname & ".xls")

'                str = cr.ExportToStream(ExportFormatType.Excel)
'                bool = True
'            End If

'            cr.Database.Dispose()
'            cr.Dispose()

'            If bool = True Then

'                Dim b(str.Length) As Byte

'                str.Read(b, 0, CType(str.Length, Integer))

'                context.Response.BinaryWrite(b)
'                'MsgBox(HttpContext.Current.Session("me").ToString)
'                context.Session("refresh") = "2"
'                'context.Response.Redirect(HttpContext.Current.Session("me").ToString)

'                HttpContext.Current.ApplicationInstance.CompleteRequest()
'                context.Response.Flush()

'                context.Response.Close()

'            End If
'        End Using
'    End Sub

'    'Private Function rptWeaverDefectSummary(ByVal datefrom As String, ByVal dateto As String, ByVal connectionString As String) As Integer
'    '    Dim _conn As OracleConnection = New OracleConnection(connectionString)
'    '    Dim cmd As OracleCommand = New OracleCommand()
'    '    Using _conn
'    '        _conn.Open()
'    '        cmd.Connection = _conn
'    '        cmd.CommandType = Data.CommandType.StoredProcedure
'    '        cmd.CommandText = "psp_WeaverDefectSummary"
'    '        cmd.Parameters.Add("P_FromDate", OracleDbType.Varchar2, Data.ParameterDirection.Input).Value = datefrom
'    '        cmd.Parameters.Add("P_ToDate", OracleDbType.Varchar2, Data.ParameterDirection.Input).Value = dateto
'    '        cmd.ExecuteNonQuery()
'    '        cmd.Dispose()
'    '    End Using
'    'End Function

'    'Private Function rptDyerDefectAnalysis(ByVal datefrom As String, ByVal dateto As String, ByVal connectionString As String) As Integer
'    '    Dim _conn As OracleConnection = New OracleConnection(connectionString)
'    '    Dim cmd As OracleCommand = New OracleCommand()
'    '    Using _conn
'    '        _conn.Open()
'    '        cmd.Connection = _conn
'    '        cmd.CommandType = Data.CommandType.StoredProcedure
'    '        cmd.CommandText = "PSP_DyerDefectAnalysis"
'    '        cmd.Parameters.Add("P_FromDate", OracleDbType.Varchar2, Data.ParameterDirection.Input).Value = datefrom
'    '        cmd.Parameters.Add("P_ToDate", OracleDbType.Varchar2, Data.ParameterDirection.Input).Value = dateto
'    '        cmd.ExecuteNonQuery()
'    '        cmd.Dispose()
'    '    End Using
'    'End Function

'    Private Sub printHTML5Report(ByVal context As HttpContext, ByVal _Parameter As String, ByVal _Value As String, ByVal _Type As String, ByVal _Name As String)
'        Dim _paramName As String() = _Parameter.Split("|")
'        Dim _paramValue As String() = _Value.Split("|")
'        Dim _whereClause As String = String.Empty
'        Dim _currDate As Date = Now
'        Dim _dateStr As String = _currDate.ToString("yyyyMMdd")
'        Dim _timeStr As String = _currDate.ToString("hhmmtt")

'        For i As Integer = 0 To _paramName.Length - 1
'            If i > 0 Then
'                _whereClause += " AND "
'            End If

'            _whereClause += _paramName(i).ToUpper + " = '" + _paramValue(i) + "'"
'        Next

'        Dim dtMainReport As Dictionary(Of String, Object) = RetrieveMainReportData(_Name, _whereClause)
'        Dim dtSubReport As System.Data.DataTable = RetrieveSubreportData()
'        Dim dtLst As New List(Of Object)
'        dtLst.Add(dtMainReport)
'        dtLst.Add(dtSubReport)

'        BuildReport(context, dtLst, _Name, _dateStr, _timeStr)
'    End Sub

'    ' Added by ANAS on 27/06/2020
'    Private Function RetrieveMainReportData(ByVal rptName As String, ByVal WhereClauseString As String) As Dictionary(Of String, Object)
'        RetrieveMainReportData = New Dictionary(Of String, Object)
'        'Dim _sqlConn As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("eBizSQL_OLD").ConnectionString)
'        Dim _sqlConn As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("eBizSQL").ConnectionString)
'        Dim _sqlCmd As New SqlCommand
'        With _sqlCmd
'            .Connection = _sqlConn
'            .CommandType = System.Data.CommandType.Text
'            .CommandTimeout = 0

'            If rptName = "SPL_AdHocRequest" Then
'                '.CommandText = "SELECT SAMPLE_REQUEST_NO, CREATED_BY, CREATED_DATE, REGION, TEL_NO, CUST_NAME, " +
'                '               "SENT_BY, DATE_SENT, COURIER_NO, CHARGE, CHARGE_BY, SAMPLE_ROLL_NUMBER, QTY_REQUEST, " +
'                '               "UOM, COLOR_DESC, FINISH_DESC2, FINISH_DESC3, FINISH_DESC4, PONO, CHOP_NUMBER, LOCATION, " +
'                '               "YARN_COUNT, ATTN_TO, REC_TYPE, CUST_ADDR1, CUST_ADDR2, CUST_ADDR3, CUST_ADDR4, SALES_DESC1, " +
'                '               "SALES_DESC2, SALES_DESC3, SALES_DESC4, REMARK, QR " +
'                '               "FROM DBO.PVIEW_SPL_ADHOCREQUEST WHERE " +
'                '               WhereClauseString
'                .CommandText = "SELECT * FROM DBO.PVIEW_SPL_ADHOCREQUEST WHERE " + WhereClauseString
'            ElseIf rptName = "SPL_ItemDeliver" Then
'                .CommandText = "SELECT A.REQUEST_NO, A.REC_TYPE, A.CREATED_BY, B.REGION_DESC, A.ATTN_TO, " +
'                               "A.REQUEST_DESC, A.REF_DESC, A.COMPANY_NAME, A.ADDR1, A.ADDR2, A.TEL_NO, A.SENT_BY, " +
'                               "A.CREATED_DATE, A.COURIER_NO, A.CHARGE, A.CHARGE_BY, A.DATE_SENT, A.ADDR3, A.ADDR4, A.REMARK " +
'                               "FROM DBO.SAMPBULLETIN_MASTER A " +
'                               "LEFT OUTER JOIN DBO.SAMPBULLENTIN_SECT B ON A.SECTION = B.REGION_CODE WHERE " +
'                               WhereClauseString
'            End If

'            .Parameters.Clear()
'        End With

'        Try
'            _sqlConn.Open()
'            Dim _sqlrdr As SqlDataReader = _sqlCmd.ExecuteReader()
'            Dim _dt As New System.Data.DataTable
'            _dt.Load(_sqlrdr)
'            RetrieveMainReportData.Add("DATA", _dt)
'        Catch ex As Exception
'            Dim errMsg As String = ex.Message
'        End Try

'        If rptName = "SPL_AdHocRequest" Then
'            ' QR Code
'            _sqlCmd = New SqlCommand
'            With _sqlCmd
'                .Connection = _sqlConn
'                .CommandType = System.Data.CommandType.Text
'                .CommandTimeout = 0
'                .CommandText = "SELECT TOP (1) QR " +
'                               "FROM DBO.PVIEW_SPL_ADHOCREQUEST WHERE " +
'                               WhereClauseString
'                .Parameters.Clear()
'            End With

'            Try
'                Dim qrCodeData As Byte() = DirectCast(_sqlCmd.ExecuteScalar(), Byte())
'                RetrieveMainReportData.Add("QR", QRCode(qrCodeData))
'                _sqlConn.Close()
'                _sqlConn.Dispose()
'            Catch ex As Exception
'                Dim errMsg As String = ex.Message
'            End Try

'            ' Getting the rest of data
'            Dim _dt1 As System.Data.DataTable = RetrieveMainReportData.Item("DATA")
'            Dim custCode As String = IIf(IsDBNull(_dt1.DefaultView.ToTable(True, "CUST_CODE").Rows(0)("CUST_CODE")), "NULL", _dt1.DefaultView.ToTable(True, "CUST_CODE").Rows(0)("CUST_CODE"))
'            Dim finishTypeDt As New System.Data.DataTable
'            Dim tmpDt As New System.Data.DataTable
'            finishTypeDt = _dt1.DefaultView.ToTable(True, "FINISH_TYPE1", "FINISH_TYPE2", "FINISH_TYPE3", "FINISH_TYPE4")
'            Dim finishTypeQuery As String = String.Empty
'            tmpDt = finishTypeDt.Clone
'            tmpDt.Rows.Clear()

'            Dim x As Integer = 0
'            For i As Integer = 0 To finishTypeDt.Rows.Count - 1
'                If Not IsDBNull(finishTypeDt.Rows(i)("FINISH_TYPE1")) Then
'                    If Not String.IsNullOrWhiteSpace(finishTypeDt.Rows(i)("FINISH_TYPE1")) Then
'                        tmpDt.Rows.Add(tmpDt.NewRow())
'                        tmpDt.Rows(x).ItemArray = finishTypeDt.Rows(i).ItemArray
'                        x += 1
'                    End If


'                End If
'            Next

'            For i As Integer = 0 To tmpDt.Rows.Count - 1
'                If i > 0 Then
'                    finishTypeQuery += " UNION "
'                End If
'                ' Modified by ANAS on 10/08/2020 (Remarks: Added TRIM function to the query statement)
'                finishTypeQuery += String.Format("SELECT TRIM({0}) AS FINISH_TYPE1, TRIM({1}) AS FINISH_TYPE2, TRIM({2}) AS FINISH_TYPE3, TRIM({3}) AS FINISH_TYPE4 FROM DUAL",
'                    IIf(IsDBNull(tmpDt.Rows(i)("FINISH_TYPE1")), "''", "'" + tmpDt.Rows(i)("FINISH_TYPE1") + "'"),
'                    IIf(IsDBNull(tmpDt.Rows(i)("FINISH_TYPE2")), "''", "'" + tmpDt.Rows(i)("FINISH_TYPE2") + "'"),
'                    IIf(IsDBNull(tmpDt.Rows(i)("FINISH_TYPE3")), "''", "'" + tmpDt.Rows(i)("FINISH_TYPE3") + "'"),
'                    IIf(IsDBNull(tmpDt.Rows(i)("FINISH_TYPE4")), "''", "'" + tmpDt.Rows(i)("FINISH_TYPE4") + "'")
'                )
'            Next

'            ' Customer data
'            Dim _dt2 As New System.Data.DataTable
'            Dim _conn As OracleConnection = New OracleConnection(ConfigurationManager.ConnectionStrings("PABSALESSQL").ConnectionString)
'            Dim _cmd As New OracleCommand
'            With _cmd
'                .Connection = _conn
'                .CommandType = System.Data.CommandType.StoredProcedure
'                .CommandTimeout = 0
'                .CommandText = "SP_SAMPROOM_CUSTOMER"
'                .Parameters.Clear()
'                .Parameters.Add("pCUST_CODE", OracleDbType.Varchar2, System.Data.ParameterDirection.Input).Value = IIf(custCode = "NULL", DBNull.Value, custCode)
'                .Parameters.Add("SREFData", OracleDbType.RefCursor, System.Data.ParameterDirection.Output)
'            End With

'            Try
'                _conn.Open()
'                Dim _rdr As OracleDataReader = _cmd.ExecuteReader()
'                _dt2.Load(_rdr)
'            Catch ex As Exception
'                Dim errMsg As String = ex.Message
'            End Try
'            ' End of customer data

'            ' Finish type data
'            Dim _dt3 As New System.Data.DataTable
'            With _cmd
'                .Connection = _conn
'                .CommandType = System.Data.CommandType.Text
'                .CommandTimeout = 0
'                .CommandText = "SELECT A.FINISH_TYPE1, A.FINISH_TYPE2, A.FINISH_TYPE3, A.FINISH_TYPE4, " +
'                               "TRIM(F1.FINISH_DESC) AS FINISH_DESC1, TRIM(F1.SALES_DESC) AS SALES_DESC1, " +
'                               "TRIM(F2.FINISH_DESC) AS FINISH_DESC2, TRIM(F2.SALES_DESC) AS SALES_DESC2, " +
'                               "TRIM(F3.FINISH_DESC) AS FINISH_DESC3, TRIM(F3.SALES_DESC) AS SALES_DESC3, " +
'                               "TRIM(F4.FINISH_DESC) AS FINISH_DESC4, TRIM(F4.SALES_DESC) AS SALES_DESC4 " +
'                               "FROM (" +
'                               finishTypeQuery + ") A " +
'                               "LEFT JOIN V_MSO_FIN_DESC F1 ON F1.FINISH_CODE = A.FINISH_TYPE1 " +
'                               "LEFT JOIN V_MSO_FIN_DESC F2 ON F2.FINISH_CODE = A.FINISH_TYPE2 " +
'                               "LEFT JOIN V_MSO_FIN_DESC F3 ON F3.FINISH_CODE = A.FINISH_TYPE3 " +
'                               "LEFT JOIN V_MSO_FIN_DESC F4 ON F4.FINISH_CODE = A.FINISH_TYPE4"
'                .Parameters.Clear()
'            End With

'            Try
'                Dim _rdr As OracleDataReader = _cmd.ExecuteReader()
'                _dt3.Load(_rdr)
'                _conn.Close()
'                _conn.Dispose()
'            Catch ex As Exception
'                Dim errMsg As String = ex.Message
'            End Try
'            ' End of finish type data

'            ' Join all tables
'            Dim query As IEnumerable(Of ReportObject.SPL_AdHocRequest) =
'                From a In _dt1.AsEnumerable
'                Group Join b In _dt2.AsEnumerable On a("CUST_CODE") Equals b("CUST_CODE")
'                Into bGrp = Group
'                Let b = bGrp.FirstOrDefault
'                Group Join c1 In _dt3.AsEnumerable On a("FINISH_TYPE1") Equals c1("FINISH_TYPE1")
'                Into c1Grp = Group
'                Let c1 = c1Grp.FirstOrDefault
'                Group Join c2 In _dt3.AsEnumerable On a("FINISH_TYPE2") Equals c2("FINISH_TYPE2")
'                Into c2Grp = Group
'                Let c2 = c2Grp.FirstOrDefault
'                Group Join c3 In _dt3.AsEnumerable On a("FINISH_TYPE3") Equals c3("FINISH_TYPE3")
'                Into c3Grp = Group
'                Let c3 = c3Grp.FirstOrDefault
'                Group Join c4 In _dt3.AsEnumerable On a("FINISH_TYPE4") Equals c4("FINISH_TYPE4")
'                Into c4Grp = Group
'                Let c4 = c4Grp.FirstOrDefault
'                Select New ReportObject.SPL_AdHocRequest With {
'                    .RecType = If(a Is Nothing Or a("REC_TYPE").Equals(DBNull.Value), String.Empty, a("REC_TYPE")),
'                    .SampleRequestNo = If(a Is Nothing Or a("SAMPLE_REQUEST_NO").Equals(DBNull.Value), String.Empty, a("SAMPLE_REQUEST_NO")),
'                    .Region = If(a Is Nothing Or a("REGION").Equals(DBNull.Value), String.Empty, a("REGION")),
'                    .TelephoneNo = If(a Is Nothing Or a("TEL_NO").Equals(DBNull.Value), String.Empty, a("TEL_NO")),
'                    .AttentionTo = If(a Is Nothing Or a("ATTN_TO").Equals(DBNull.Value), String.Empty, a("ATTN_TO")),
'                    .Remark = If(a Is Nothing Or a("REMARK").Equals(DBNull.Value), String.Empty, a("REMARK")),
'                    .SentBy = If(a Is Nothing Or a("SENT_BY").Equals(DBNull.Value), String.Empty, a("SENT_BY")),
'                    .DateSent = a.Field(Of Date)("DATE_SENT"),
'                    .CourierNo = If(a Is Nothing Or a("COURIER_NO").Equals(DBNull.Value), String.Empty, a("COURIER_NO")),
'                    .Charge = If(a Is Nothing Or a("CHARGE").Equals(DBNull.Value), String.Empty, a("CHARGE")),
'                    .ChargeBy = If(a Is Nothing Or a("CHARGE_BY").Equals(DBNull.Value), String.Empty, a("CHARGE_BY")),
'                    .CreatedBy = If(a Is Nothing Or a("CREATED_BY").Equals(DBNull.Value), String.Empty, a("CREATED_BY")),
'                    .CreatedDate = a.Field(Of Date)("CREATED_DATE"),
'                    .PoNo = If(a Is Nothing Or a("PONO").Equals(DBNull.Value), String.Empty, a("PONO")),
'                    .ChopNo = If(a Is Nothing Or a("CHOP_NUMBER").Equals(DBNull.Value), String.Empty, a("CHOP_NUMBER")),
'                    .Composition = If(a Is Nothing Or a("COMPOSITION").Equals(DBNull.Value), String.Empty, a("COMPOSITION")),
'                    .ColorDescription = If(a Is Nothing Or a("COLOR_DESC").Equals(DBNull.Value), String.Empty, a("COLOR_DESC")),
'                    .WarpDensity = If(a Is Nothing Or a("WARP_DENSITY").Equals(DBNull.Value), 0, a("WARP_DENSITY")),
'                    .WarpDensity2 = If(a Is Nothing Or a("WARP_DENSITY2").Equals(DBNull.Value), 0, a("WARP_DENSITY2")),
'                    .WeftDensity = If(a Is Nothing Or a("WEFT_DENSITY").Equals(DBNull.Value), 0, a("WEFT_DENSITY")),
'                    .WeftDensity2 = If(a Is Nothing Or a("WEFT_DENSITY2").Equals(DBNull.Value), 0, a("WEFT_DENSITY2")),
'                    .SampleRollNo = If(a Is Nothing Or a("SAMPLE_ROLL_NUMBER").Equals(DBNull.Value), String.Empty, a("SAMPLE_ROLL_NUMBER")),
'                    .QtyRequest = a.Field(Of Double)("QTY_REQUEST"),
'                    .UOM = If(a Is Nothing Or a("UOM").Equals(DBNull.Value), String.Empty, a("UOM")),
'                    .Location = If(a Is Nothing Or a("LOCATION").Equals(DBNull.Value), String.Empty, a("LOCATION")),
'                    .YarnCount = If(a Is Nothing Or a("YARN_COUNT").Equals(DBNull.Value), String.Empty, a("YARN_COUNT")),
'                    .QRCode = a.Field(Of Byte())("QR"),
'                    .RecTypeDesc = If(a Is Nothing Or a("RECORDTYPE").Equals(DBNull.Value), String.Empty, a("RECORDTYPE")),
'                    .CustomerCode = If(a Is Nothing Or a("CUST_CODE").Equals(DBNull.Value), String.Empty, a("CUST_CODE")),
'                    .CustomerAddress1 = If(b Is Nothing Or b("CUST_ADDR1").Equals(DBNull.Value), String.Empty, b("CUST_ADDR1")),
'                    .CustomerAddress2 = If(b Is Nothing Or b("CUST_ADDR2").Equals(DBNull.Value), String.Empty, b("CUST_ADDR2")),
'                    .CustomerAddress3 = If(b Is Nothing Or b("CUST_ADDR3").Equals(DBNull.Value), String.Empty, b("CUST_ADDR3")),
'                    .CustomerAddress4 = If(b Is Nothing Or b("CUST_ADDR4").Equals(DBNull.Value), String.Empty, b("CUST_ADDR4")),
'                    .CustomerName = If(b Is Nothing Or b("CUST_NAME").Equals(DBNull.Value), String.Empty, b("CUST_NAME")),
'                    .FinishType1 = If(a Is Nothing Or a("FINISH_TYPE1").Equals(DBNull.Value), String.Empty, a("FINISH_TYPE1")),
'                    .FinishType2 = If(a Is Nothing Or a("FINISH_TYPE2").Equals(DBNull.Value), String.Empty, a("FINISH_TYPE2")),
'                    .FinishType3 = If(a Is Nothing Or a("FINISH_TYPE3").Equals(DBNull.Value), String.Empty, a("FINISH_TYPE3")),
'                    .FinishType4 = If(a Is Nothing Or a("FINISH_TYPE4").Equals(DBNull.Value), String.Empty, a("FINISH_TYPE4")),
'                    .FinishTypeDesc1 = If(c1 Is Nothing, String.Empty, c1("FINISH_DESC1")),
'                    .SalesDesc1 = If(c1 Is Nothing, String.Empty, c1("SALES_DESC1")),
'                    .FinishTypeDesc2 = If(c2 Is Nothing, String.Empty, c2("FINISH_DESC2")),
'                    .SalesDesc2 = If(c2 Is Nothing, String.Empty, c2("SALES_DESC2")),
'                    .FinishTypeDesc3 = If(c3 Is Nothing, String.Empty, c3("FINISH_DESC3")),
'                    .SalesDesc3 = If(c3 Is Nothing, String.Empty, c3("SALES_DESC3")),
'                    .FinishTypeDesc4 = If(c4 Is Nothing, String.Empty, c4("FINISH_DESC4")),
'                    .SalesDesc4 = If(c4 Is Nothing, String.Empty, c4("SALES_DESC4")),
'                    .Country = If(a Is Nothing Or a("COUNTRY").Equals(DBNull.Value), String.Empty, a("COUNTRY"))
'                }

'            Dim _joinedDt As System.Data.DataTable = ConvertToDataTable(Of ReportObject.SPL_AdHocRequest)(query.ToList)
'            RetrieveMainReportData.Item("DATA") = _joinedDt

'        ElseIf rptName = "SPL_ItemDeliver" Then
'            Try
'                _sqlConn.Close()
'                _sqlConn.Dispose()
'            Catch ex As Exception
'                Dim errMsg As String = ex.Message
'            End Try
'        End If
'    End Function

'    ' Added by ANAS on 27/06/2020
'    Private Function RetrieveSubreportData() As System.Data.DataTable
'        RetrieveSubreportData = New System.Data.DataTable
'        'Dim _sqlConn As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("eBizSQL_OLD").ConnectionString)
'        Dim _sqlConn As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("eBizSQL").ConnectionString)
'        Dim _sqlCmd As New SqlCommand
'        With _sqlCmd
'            .Connection = _sqlConn
'            .CommandType = System.Data.CommandType.Text
'            .CommandTimeout = 0
'            .CommandText = "SELECT COMPANY_NAME, ADDR2, TELNO, FAXNO, ADDR1, ADDR3, ADDR4 " +
'                           "FROM DBO.PVIEW_PABCONAME"
'            .Parameters.Clear()
'        End With

'        Try
'            _sqlConn.Open()
'            Dim _sqlrdr As SqlDataReader = _sqlCmd.ExecuteReader()
'            RetrieveSubreportData.Load(_sqlrdr)
'            _sqlConn.Close()
'            _sqlConn.Dispose()
'        Catch ex As Exception
'            Dim errMsg As String = ex.Message
'        End Try
'    End Function

'    Private Sub BuildReport(ByVal context As HttpContext, ByVal dtLst As List(Of Object), ByVal rptName As String, ByVal dateStr As String, ByVal timeStr As String)
'        Dim sbHeader As New StringBuilder
'        Dim sbFooter As New StringBuilder
'        Dim sbData As New StringBuilder
'        Dim finalHtmlCode As String = String.Empty
'        Dim dividerHtmlCode As String = "<tr><td class=""custom-border-top-dotted"">&nbsp;</td></tr>"
'        Dim dataDict As Dictionary(Of String, Object) = dtLst(0)
'        Dim dtData As DataTable = dataDict("DATA")
'        Dim dtAddress As System.Data.DataTable = dtLst(1)

'        If Not IsNothing(dtData) And dtData.Rows.Count > 0 Then
'            If rptName = "SPL_AdHocRequest" Then
'                ' Separate distinct data to another datatable
'                Dim dtDistinct As System.Data.DataTable = dtData.DefaultView.ToTable(
'                    True, "SAMPLEREQUESTNO", "CREATEDBY", "CREATEDDATE", "REGION", "TELEPHONENO", "CUSTOMERNAME",
'                        "CUSTOMERADDRESS1", "CUSTOMERADDRESS2", "CUSTOMERADDRESS3", "CUSTOMERADDRESS4", "SENTBY", "DATESENT",
'                        "RECTYPE", "QRCODE", "ATTENTIONTO", "REMARK", "CHARGE", "CHARGEBY", "COURIERNO")
'                Dim recType As String = String.Empty

'                If dtDistinct.Rows(0)("RECTYPE") = "0" Then
'                    recType = "( Draft )"
'                ElseIf dtDistinct.Rows(0)("RECTYPE") = "5" Then
'                    recType = "( Cancel )"
'                End If

'                ' Some calculations
'                Dim rowIndex As Integer = 0
'                Dim totalPages As Integer = 0
'                Dim firstPageRecords As Integer = 4
'                Dim otherPageRecordsPerPage As Integer = 6
'                Dim lastPageRecords As Integer = (dtData.Rows.Count - firstPageRecords) Mod otherPageRecordsPerPage

'                If dtData.Rows.Count <= firstPageRecords Then
'                    totalPages = 1
'                Else
'                    If lastPageRecords = 0 Then
'                        totalPages = ((dtData.Rows.Count - firstPageRecords) / otherPageRecordsPerPage) + 1
'                    Else
'                        totalPages = ((dtData.Rows.Count - firstPageRecords - lastPageRecords) / otherPageRecordsPerPage) + 2
'                    End If
'                End If

'                ' Header
'                sbHeader.Append("<html>")
'                sbHeader.Append("<head>")
'                sbHeader.Append("<style type=""text/css"">")
'                sbHeader.Append(".pageBreak { font-size: 1px; page-break-before: always; }")
'                sbHeader.Append(".txt-center-top { text-align: center; vertical-align: top; }")
'                sbHeader.Append(".txt-left-top { text-align: left; vertical-align: top; }")
'                sbHeader.Append(".txt-right-top { text-align: right; vertical-align: top; }")
'                sbHeader.Append(".txt-right-bottom { text-align: right; vertical-align: bottom; }")
'                sbHeader.Append("body, table, tr, td, span { font-family: ""Times New Roman""; font-size: 9pt; }")
'                sbHeader.Append("table { border: 0px solid black; border-collapse: collapse; width: 100%; }")
'                sbHeader.Append("table > tr > td.colon { width: 1%; }")
'                sbHeader.Append("div.spacing { width: 100%; }")
'                sbHeader.Append("table#mainRpt > tr > td.leftSpace, table#mainRpt > tr > td.rightSpace { width: 5%; }")
'                sbHeader.Append("table#mainRpt > tr > td.middleSpace { width: 90%; }")
'                sbHeader.Append("table.tblAddress > tr > td { color: blue; }")
'                sbHeader.Append("table.tblContent > tr > td.leftCompartment, table.tblContent > tr > td.rightCompartment { width: 22%; }")
'                sbHeader.Append("table.tblContent > tr > td.dataCompartment { font-family: Tahoma; font-size: 8pt; width: 55%; }")
'                sbHeader.Append("table.tblContent2 > tr > td { font-family: Tahoma; font-size: 8pt; }")
'                sbHeader.Append("table.tblContent2 > tr > td.infoIndex { width: 4%; }")
'                sbHeader.Append("table.tblContent2 > tr > td.infoData1 { width: 24%; }")
'                sbHeader.Append("table.tblContent2 > tr > td.infoData2 { width: 20%; }")
'                sbHeader.Append("table.tblContent2 > tr > td.infoData3 { width: 26%; }")
'                sbHeader.Append("table.tblContent2 > tr > td.infoData4 { width: 26%; }")
'                sbHeader.Append("table.tblContent > tr > td.label { width: 22%; }")
'                sbHeader.Append("table.tblContent > tr > td.detail { font-family: Tahoma; font-size: 8pt; width: 27%; }")
'                sbHeader.Append(".address { color: blue; font-size: 8pt; }")
'                sbHeader.Append(".companyName { font-family: ""Arial Black""; font-size: 12pt; }")
'                sbHeader.Append("</style>")
'                sbHeader.Append("</head>")
'                sbHeader.Append("<body>")
'                sbHeader.Append("<table id=""mainRpt"">")

'                Dim pageRecords As Integer
'                Dim currentPage As Integer
'                For currentPage = 1 To totalPages
'                    ' Page break control
'                    If currentPage > 1 Then
'                        sbData.Append("<tr>")
'                        sbData.Append("<td colspan=""3""><p class=""pageBreak"">&nbsp;</p></td>")
'                        sbData.Append("</tr>")
'                    End If
'                    ' End of page break control

'                    ' Data (Report Content)
'                    sbData.Append("<tr>")
'                    sbData.Append("<td class=""leftSpace""><div class=""spacing"">&nbsp;</div></td>")
'                    sbData.Append("<td class=""txt-center-top middleSpace"">")

'                    ' Address section
'                    sbData.Append("<table class=""tblAddress"">")
'                    sbData.Append(String.Format("<tr><td class=""txt-center-top companyName"">{0}</td></tr>", IIf(IsDBNull(dtAddress.Rows(0)("COMPANY_NAME")), "", dtAddress.Rows(0)("COMPANY_NAME"))))
'                    sbData.Append("<tr><td class=""address txt-center-top"">")
'                    sbData.Append(String.Format("{0} {1}<br />", IIf(IsDBNull(dtAddress.Rows(0)("ADDR1")), "", Trim(dtAddress.Rows(0)("ADDR1"))), IIf(IsDBNull(dtAddress.Rows(0)("ADDR2")), "", Trim(dtAddress.Rows(0)("ADDR2")))))
'                    sbData.Append(String.Format("{0} {1}<br />", IIf(IsDBNull(dtAddress.Rows(0)("ADDR3")), "", Trim(dtAddress.Rows(0)("ADDR3"))), IIf(IsDBNull(dtAddress.Rows(0)("ADDR4")), "", Trim(dtAddress.Rows(0)("ADDR4")))))
'                    sbData.Append(String.Format("TEL: {0}<br />", IIf(IsDBNull(dtAddress.Rows(0)("TELNO")), "", Trim(dtAddress.Rows(0)("TELNO")))))
'                    sbData.Append(String.Format("FAX: {0} / 04-3987366 / 04-3909595<br />", IIf(IsDBNull(dtAddress.Rows(0)("FAXNO")), "", Trim(dtAddress.Rows(0)("FAXNO")))))
'                    sbData.Append("</td></tr>")
'                    sbData.Append("</table>")
'                    ' End of Address section

'                    sbData.Append("</td>")
'                    sbData.Append("<td class=""rightSpace""><div class=""spacing"">&nbsp;</div></td>")
'                    sbData.Append("</tr>")
'                    sbData.Append("<tr>")
'                    sbData.Append("<td class=""leftSpace""><div class=""spacing"">&nbsp;</div></td>")
'                    sbData.Append("<td class=""txt-center-top"">")

'                    ' First section
'                    sbData.Append("<table class=""tblContent"">")
'                    sbData.Append("<tr>")
'                    sbData.Append("<td class=""txt-left-top leftCompartment""><b>SAMPLE REQUEST NO</b></td>")
'                    sbData.Append("<td class=""txt-left-top colon""><b>:</b></td>")
'                    sbData.Append(String.Format("<td class=""txt-left-top dataCompartment"">{0} {1}</td>", IIf(IsDBNull(dtDistinct.Rows(0)("SAMPLEREQUESTNO")), "", Trim(dtDistinct.Rows(0)("SAMPLEREQUESTNO"))), recType))
'                    sbData.Append(String.Format("<td class=""txt-right-top rightCompartment"" rowspan=""5""><img width=""100px"" height=""100px"" src=""data:image/png;base64,{0}"" /></td>", dataDict("QR")))
'                    sbData.Append("</tr>")
'                    sbData.Append("<tr>")
'                    sbData.Append("<td class=""txt-left-top leftCompartment""><b>CREATOR</b></td>")
'                    sbData.Append("<td class=""txt-left-top colon""><b>:</b></td>")
'                    sbData.Append(String.Format("<td class=""txt-left-top dataCompartment"">{0}</td>", IIf(IsDBNull(dtDistinct.Rows(0)("CREATEDBY")), "", Trim(dtDistinct.Rows(0)("CREATEDBY")))))
'                    sbData.Append("</tr>")
'                    sbData.Append("<tr>")
'                    sbData.Append("<td class=""txt-left-top leftCompartment""><b>DATE</b></td>")
'                    sbData.Append("<td class=""txt-left-top colon""><b>:</b></td>")
'                    sbData.Append(String.Format("<td class=""txt-left-top dataCompartment"">{0}</td>", Date.Parse(dtDistinct.Rows(0)("CREATEDDATE")).ToString("dd-MMM-yyyy")))
'                    sbData.Append("</tr>")
'                    sbData.Append("<tr>")
'                    sbData.Append("<td class=""txt-left-top leftCompartment""><b>TIME</b></td>")
'                    sbData.Append("<td class=""txt-left-top colon""><b>:</b></td>")
'                    sbData.Append(String.Format("<td class=""txt-left-top dataCompartment"">{0}</td>", Date.Parse(dtDistinct.Rows(0)("CREATEDDATE")).ToString("hh:mm tt")))
'                    sbData.Append("</tr>")
'                    sbData.Append("<tr>")
'                    sbData.Append("<td class=""txt-left-top leftCompartment""><b>SALES REGION</b></td>")
'                    sbData.Append("<td class=""txt-left-top colon""><b>:</b></td>")
'                    sbData.Append(String.Format("<td class=""txt-left-top dataCompartment"">{0}</td>", IIf(IsDBNull(dtDistinct.Rows(0)("REGION")), "", Trim(dtDistinct.Rows(0)("REGION")))))
'                    sbData.Append("</tr>")
'                    sbData.Append("</table>")
'                    ' End of First section

'                    sbData.Append("</td>")
'                    sbData.Append("<td class=""rightSpace""><div class=""spacing"">&nbsp;</div></td>")
'                    sbData.Append("</tr>")

'                    ' Divider #1
'                    sbData.Append("<tr>")
'                    sbData.Append("<td class=""leftSpace""><div class=""spacing"">&nbsp;</div></td>")
'                    sbData.Append("<td class=""middleSpace custom-border-top-dashed"">&nbsp;</td>")
'                    sbData.Append("<td class=""rightSpace""><div class=""spacing"">&nbsp;</div></td>")
'                    sbData.Append("</tr>")
'                    ' End of Divider #1

'                    If currentPage = 1 Then
'                        sbData.Append("<tr>")
'                        sbData.Append("<td class=""leftSpace""><div class=""spacing"">&nbsp;</div></td>")
'                        sbData.Append("<td class=""txt-center-top"">")

'                        ' Second section
'                        sbData.Append("<table class=""tblContent"">")
'                        sbData.Append("<tr>")
'                        sbData.Append("<td class=""txt-left-top leftCompartment""><b>FOR THE ATTENTION OF</b></td>")
'                        sbData.Append("<td class=""txt-left-top colon""><b>:</b></td>")
'                        sbData.Append(String.Format("<td colspan=""2"" class=""txt-left-top dataCompartment"">{0}</td>", System.Net.WebUtility.HtmlEncode(IIf(IsDBNull(dtDistinct.Rows(0)("ATTENTIONTO")), "", Trim(dtDistinct.Rows(0)("ATTENTIONTO")))))) ' Modified by ANAS on 08/07/2020 (To encode HTML special characters)
'                        sbData.Append("</tr>")
'                        sbData.Append("<tr>")
'                        sbData.Append("<td class=""txt-left-top leftCompartment""><b>COMPANY NAME</b></td>")
'                        sbData.Append("<td class=""txt-left-top colon""><b>:</b></td>")
'                        sbData.Append(String.Format("<td colspan=""2"" class=""txt-left-top dataCompartment"">{0}</td>", System.Net.WebUtility.HtmlEncode(IIf(IsDBNull(dtDistinct.Rows(0)("CUSTOMERNAME")), "", Trim(dtDistinct.Rows(0)("CUSTOMERNAME")))))) ' Modified by ANAS on 08/07/2020 (To encode HTML special characters)
'                        sbData.Append("</tr>")
'                        sbData.Append("<tr>")
'                        sbData.Append("<td class=""txt-left-top leftCompartment""><b>COMPANY ADDRESS</b></td>")
'                        sbData.Append("<td class=""txt-left-top colon""><b>:</b></td>")
'                        sbData.Append(String.Format("<td colspan=""2"" class=""txt-left-top dataCompartment"">{0}</td>", System.Net.WebUtility.HtmlEncode(IIf(IsDBNull(dtDistinct.Rows(0)("CUSTOMERADDRESS1")), "", Trim(dtDistinct.Rows(0)("CUSTOMERADDRESS1")))))) ' Modified by ANAS on 08/07/2020 (To encode HTML special characters)
'                        sbData.Append("</tr>")
'                        sbData.Append("<tr>")
'                        sbData.Append("<td class=""txt-left-top leftCompartment"">&nbsp;</td>")
'                        sbData.Append("<td class=""txt-left-top colon"">&nbsp;</td>")
'                        sbData.Append(String.Format("<td colspan=""2"" class=""txt-left-top dataCompartment"">{0}</td>", System.Net.WebUtility.HtmlEncode(IIf(IsDBNull(dtDistinct.Rows(0)("CUSTOMERADDRESS2")), "", Trim(dtDistinct.Rows(0)("CUSTOMERADDRESS2")))))) ' Modified by ANAS on 08/07/2020 (To encode HTML special characters)
'                        sbData.Append("</tr>")
'                        sbData.Append("<tr>")
'                        sbData.Append("<td class=""txt-left-top leftCompartment"">&nbsp;</td>")
'                        sbData.Append("<td class=""txt-left-top colon"">&nbsp;</td>")
'                        sbData.Append(String.Format("<td colspan=""2"" class=""txt-left-top dataCompartment"">{0}</td>", System.Net.WebUtility.HtmlEncode(IIf(IsDBNull(dtDistinct.Rows(0)("CUSTOMERADDRESS3")), "", Trim(dtDistinct.Rows(0)("CUSTOMERADDRESS3")))))) ' Modified by ANAS on 08/07/2020 (To encode HTML special characters)
'                        sbData.Append("</tr>")
'                        sbData.Append("<tr>")
'                        sbData.Append("<td class=""txt-left-top leftCompartment"">&nbsp;</td>")
'                        sbData.Append("<td class=""txt-left-top colon"">&nbsp;</td>")
'                        sbData.Append(String.Format("<td colspan=""2"" class=""txt-left-top dataCompartment"">{0}</td>", System.Net.WebUtility.HtmlEncode(IIf(IsDBNull(dtDistinct.Rows(0)("CUSTOMERADDRESS4")), "", Trim(dtDistinct.Rows(0)("CUSTOMERADDRESS4")))))) ' Added by ANAS on 16/07/2020 (Missed out column)
'                        sbData.Append("</tr>")
'                        sbData.Append("<tr>")
'                        sbData.Append("<td class=""txt-left-top leftCompartment""><b>TELEPHONE NO.</b></td>")
'                        sbData.Append("<td class=""txt-left-top colon""><b>:</b></td>")
'                        sbData.Append(String.Format("<td colspan=""2"" class=""txt-left-top dataCompartment"">{0}</td>", IIf(IsDBNull(dtDistinct.Rows(0)("TELEPHONENO")), "", Trim(dtDistinct.Rows(0)("TELEPHONENO")))))
'                        sbData.Append("</tr>")
'                        sbData.Append("</table>")
'                        ' End of Second section

'                        sbData.Append("</td>")
'                        sbData.Append("<td class=""rightSpace""><div class=""spacing"">&nbsp;</div></td>")
'                        sbData.Append("</tr>")

'                        ' Divider #2
'                        sbData.Append("<tr>")
'                        sbData.Append("<td class=""leftSpace""><div class=""spacing"">&nbsp;</div></td>")
'                        sbData.Append("<td class=""middleSpace custom-border-bottom-dashed"">&nbsp;</td>")
'                        sbData.Append("<td class=""rightSpace""><div class=""spacing"">&nbsp;</div></td>")
'                        sbData.Append("</tr>")
'                        ' End of Divider #2
'                    End If

'                    ' Third section
'                    sbData.Append("<tr>")
'                    sbData.Append("<td class=""leftSpace""><div class=""spacing"">&nbsp;</div></td>")
'                    If currentPage = 1 Then
'                        sbData.Append("<td class=""middleSpace txt-left-top""><br /><b>ADDITIONAL INFORMATION:</b></td>")
'                    Else
'                        sbData.Append("<td class=""middleSpace txt-left-top""><b>ADDITIONAL INFORMATION:</b></td>")
'                    End If
'                    sbData.Append("<td class=""rightSpace""><div class=""spacing"">&nbsp;</div></td>")
'                    sbData.Append("</tr>")

'                    pageRecords = 0
'                    If currentPage = 1 Then
'                        sbData.Append("<tr>")
'                        sbData.Append("<td class=""leftSpace""><div class=""spacing"">&nbsp;</div></td>")
'                        sbData.Append("<td class=""middleSpace"">")
'                        sbData.Append("<table class=""tblContent2"">")
'                        For idx As Integer = rowIndex To firstPageRecords - 1
'                            sbData.Append("<tr>")
'                            sbData.Append(String.Format("<td class=""txt-left-top infoIndex"">{0}</td>", rowIndex + 1))

'                            Dim poNo As String = IIf(IsDBNull(dtData.Rows().Item(rowIndex)("PONO")), "", dtData.Rows().Item(rowIndex)("PONO"))
'                            If Not String.IsNullOrEmpty(poNo) Then
'                                poNo = Left(poNo, 5) + "/" + Mid(poNo, 6, 4) + "/" + Mid(poNo, 10, 7)
'                            End If
'                            sbData.Append(String.Format("<td class=""txt-left-top infoData1"">{0}</td>", poNo))
'                            sbData.Append(String.Format("<td class=""txt-left-top infoData2"">{0}</td>", IIf(IsDBNull(dtData.Rows().Item(rowIndex)("CHOPNO")), "", dtData.Rows().Item(rowIndex)("CHOPNO"))))
'                            sbData.Append(String.Format("<td class=""txt-left-top infoData3"">{0}</td>", IIf(IsDBNull(dtData.Rows().Item(rowIndex)("YARNCOUNT")), "", dtData.Rows().Item(rowIndex)("YARNCOUNT"))))
'                            sbData.Append(String.Format("<td class=""txt-left-top infoData4"">{0}</td>", IIf(IsDBNull(dtData.Rows().Item(rowIndex)("COLORDESCRIPTION")), "", dtData.Rows().Item(rowIndex)("COLORDESCRIPTION"))))
'                            sbData.Append("</tr>")
'                            sbData.Append("<tr>")
'                            sbData.Append(String.Format("<td class=""txt-left-top infoIndex"">{0}</td>", "&nbsp;"))
'                            sbData.Append(String.Format("<td colspan=""4"" class=""txt-left-top infoData1"">{0} {1} - {2} - {3}</td>",
'                                                        IIf(IsDBNull(dtData.Rows().Item(rowIndex)("QTYREQUEST")), "", dtData.Rows().Item(rowIndex)("QTYREQUEST")),
'                                                        IIf(IsDBNull(dtData.Rows().Item(rowIndex)("UOM")), "", dtData.Rows().Item(rowIndex)("UOM")),
'                                                        IIf(IsDBNull(dtData.Rows().Item(rowIndex)("LOCATION")), "", dtData.Rows().Item(rowIndex)("LOCATION")),
'                                                        IIf(IsDBNull(dtData.Rows().Item(rowIndex)("SAMPLEROLLNO")), "", dtData.Rows().Item(rowIndex)("SAMPLEROLLNO"))))
'                            sbData.Append("</tr>")
'                            sbData.Append("<tr>")
'                            sbData.Append(String.Format("<td class=""txt-left-top infoIndex"">{0}</td>", "&nbsp;"))
'                            Dim salesDesc As String = String.Empty
'                            If Not IsDBNull(dtData.Rows().Item(rowIndex)("SALESDESC1")) Then
'                                salesDesc = dtData.Rows().Item(rowIndex)("SALESDESC1")
'                                For i As Integer = 2 To 4
'                                    If Not IsDBNull(dtData.Rows().Item(rowIndex)("FINISHTYPEDESC" + CStr(i))) Then
'                                        If Not String.IsNullOrWhiteSpace(dtData.Rows().Item(rowIndex)("FINISHTYPEDESC" + CStr(i))) Then
'                                            salesDesc += ", " + dtData.Rows().Item(rowIndex)("SALESDESC" + CStr(i))
'                                        End If
'                                    End If
'                                Next
'                            End If
'                            salesDesc = System.Net.WebUtility.HtmlEncode(salesDesc) ' Added by ANAS on 08/07/2020 (To encode HTML special characters)
'                            sbData.Append(String.Format("<td colspan=""4"" class=""txt-left-top infoData1"">{0}</td>", salesDesc))
'                            sbData.Append("</tr>")
'                            rowIndex += 1
'                            pageRecords += 1
'                            If rowIndex = dtData.Rows.Count Then
'                                Exit For
'                            End If
'                        Next
'                        sbData.Append("</table>")
'                        sbData.Append("</td>")
'                        sbData.Append("<td class=""rightSpace""><div class=""spacing"">&nbsp;</div></td>")
'                        sbData.Append("</tr>")
'                    Else
'                        sbData.Append("<tr>")
'                        sbData.Append("<td class=""leftSpace""><div class=""spacing"">&nbsp;</div></td>")
'                        sbData.Append("<td class=""middleSpace"">")
'                        sbData.Append("<table class=""tblContent2"">")
'                        For idx As Integer = rowIndex To rowIndex + otherPageRecordsPerPage - 1
'                            sbData.Append("<tr>")
'                            sbData.Append(String.Format("<td class=""txt-left-top infoIndex"">{0}</td>", rowIndex + 1))

'                            Dim poNo As String = IIf(IsDBNull(dtData.Rows().Item(rowIndex)("PONO")), "", dtData.Rows().Item(rowIndex)("PONO"))
'                            If Not String.IsNullOrEmpty(poNo) Then
'                                poNo = Left(poNo, 5) + "/" + Mid(poNo, 6, 4) + "/" + Mid(poNo, 10, 7)
'                            End If
'                            sbData.Append(String.Format("<td class=""txt-left-top infoData1"">{0}</td>", poNo))
'                            sbData.Append(String.Format("<td class=""txt-left-top infoData2"">{0}</td>", IIf(IsDBNull(dtData.Rows().Item(rowIndex)("CHOPNO")), "", dtData.Rows().Item(rowIndex)("CHOPNO"))))
'                            sbData.Append(String.Format("<td class=""txt-left-top infoData3"">{0}</td>", IIf(IsDBNull(dtData.Rows().Item(rowIndex)("YARNCOUNT")), "", dtData.Rows().Item(rowIndex)("YARNCOUNT"))))
'                            sbData.Append(String.Format("<td class=""txt-left-top infoData4"">{0}</td>", IIf(IsDBNull(dtData.Rows().Item(rowIndex)("COLORDESCRIPTION")), "", dtData.Rows().Item(rowIndex)("COLORDESCRIPTION"))))
'                            sbData.Append("</tr>")
'                            sbData.Append("<tr>")
'                            sbData.Append(String.Format("<td class=""txt-left-top infoIndex"">{0}</td>", "&nbsp;"))
'                            sbData.Append(String.Format("<td colspan=""4"" class=""txt-left-top infoData1"">{0} {1} - {2} - {3}</td>",
'                                                        IIf(IsDBNull(dtData.Rows().Item(rowIndex)("QTYREQUEST")), "", dtData.Rows().Item(rowIndex)("QTYREQUEST")),
'                                                        IIf(IsDBNull(dtData.Rows().Item(rowIndex)("UOM")), "", dtData.Rows().Item(rowIndex)("UOM")),
'                                                        IIf(IsDBNull(dtData.Rows().Item(rowIndex)("LOCATION")), "", dtData.Rows().Item(rowIndex)("LOCATION")),
'                                                        IIf(IsDBNull(dtData.Rows().Item(rowIndex)("SAMPLEROLLNO")), "", dtData.Rows().Item(rowIndex)("SAMPLEROLLNO"))))
'                            sbData.Append("</tr>")
'                            sbData.Append("<tr>")
'                            sbData.Append(String.Format("<td class=""txt-left-top infoIndex"">{0}</td>", "&nbsp;"))
'                            Dim salesDesc As String = String.Empty
'                            If Not IsDBNull(dtData.Rows().Item(rowIndex)("SALESDESC1")) Then
'                                salesDesc = dtData.Rows().Item(rowIndex)("SALESDESC1")
'                                For i As Integer = 2 To 4
'                                    If Not IsDBNull(dtData.Rows().Item(rowIndex)("FINISHTYPEDESC" + CStr(i))) Then
'                                        If Not String.IsNullOrWhiteSpace(dtData.Rows().Item(rowIndex)("FINISHTYPEDESC" + CStr(i))) Then
'                                            salesDesc += ", " + dtData.Rows().Item(rowIndex)("SALESDESC" + CStr(i))
'                                        End If
'                                    End If
'                                Next
'                            End If
'                            salesDesc = System.Net.WebUtility.HtmlEncode(salesDesc) ' Added by ANAS on 08/07/2020 (To encode HTML special characters)
'                            sbData.Append(String.Format("<td colspan=""4"" class=""txt-left-top infoData1"">{0}</td>", salesDesc))
'                            sbData.Append("</tr>")
'                            rowIndex += 1
'                            pageRecords += 1
'                            If rowIndex = dtData.Rows.Count Then
'                                Exit For
'                            End If
'                        Next
'                        sbData.Append("</table>")
'                        sbData.Append("</td>")
'                        sbData.Append("<td class=""rightSpace""><div class=""spacing"">&nbsp;</div></td>")
'                        sbData.Append("</tr>")
'                    End If
'                    ' End of Third section

'                    If currentPage = totalPages Then
'                        ' Divider #3 (Remarks)
'                        sbData.Append("<tr>")
'                        sbData.Append("<td class=""leftSpace""><div class=""spacing"">&nbsp;</div></td>")
'                        sbData.Append("<td class=""middleSpace custom-border-bottom-dashed"">&nbsp;</td>")
'                        sbData.Append("<td class=""rightSpace""><div class=""spacing"">&nbsp;</div></td>")
'                        sbData.Append("</tr>")
'                        ' End of Divider #3

'                        sbData.Append("<tr>")
'                        sbData.Append("<td class=""leftSpace""><div class=""spacing"">&nbsp;</div></td>")
'                        sbData.Append("<td class=""txt-center-top"">")

'                        ' Fourth section (Remarks)
'                        sbData.Append("<table class=""tblContent"">")
'                        sbData.Append("<tr>")
'                        sbData.Append("<td class=""txt-left-top leftCompartment""><b>REMARK</b></td>")
'                        sbData.Append("<td class=""txt-left-top colon""><b>:</b></td>")
'                        sbData.Append(String.Format("<td class=""txt-left-top dataCompartment"">{0}</td>", System.Net.WebUtility.HtmlEncode(IIf(IsDBNull(dtDistinct.Rows(0)("REMARK")), "", Trim(dtDistinct.Rows(0)("REMARK")))).Replace(vbCrLf, "<br />"))) ' Modified by ANAS on 08/07/2020 (To encode HTML special characters; Replace CRLF with HTML br tag (09/07/2020))
'                        sbData.Append("<td class=""txt-left-top rightCompartment"">&nbsp;</td>")
'                        sbData.Append("</tr>")
'                        sbData.Append("</table>")
'                        ' End of Fourth section

'                        sbData.Append("</td>")
'                        sbData.Append("<td class=""rightSpace""><div class=""spacing"">&nbsp;</div></td>")
'                        sbData.Append("</tr>")
'                    End If

'                    ' Spacing
'                    If currentPage = 1 Then
'                        Dim remarkTotalLines As Integer = Regex.Matches(System.Net.WebUtility.HtmlEncode(dtDistinct.Rows(0)("REMARK")), vbCrLf).Count ' Added by ANAS on 09/07/2020 (Remarks: To count how many linebreak in the REMARK value)
'                        sbData.Append("<tr>")
'                        sbData.Append(String.Format("<td colspan=""3"" class=""leftSpace""><div style=""height: {0}px;"">&nbsp;</div></td>", (((2 + firstPageRecords) - pageRecords) * 27) - (remarkTotalLines * 20))) ' Modified by ANAS on 10/08/2020 (Remarks: To calculate proper value for spacing between REMARK section and the footer section)
'                        sbData.Append("</tr>")
'                    ElseIf currentPage = totalPages Then
'                        sbData.Append("<tr>")
'                        sbData.Append(String.Format("<td colspan=""3"" class=""leftSpace""><div style=""height: {0}px;"">&nbsp;</div></td>", (((4 + otherPageRecordsPerPage) - pageRecords) * 27) - 30))
'                        sbData.Append("</tr>")
'                    Else
'                        sbData.Append("<tr>")
'                        sbData.Append(String.Format("<td colspan=""3"" class=""leftSpace""><div style=""height: {0}px;"">&nbsp;</div></td>", ((2 + otherPageRecordsPerPage) - pageRecords) * 27))
'                        sbData.Append("</tr>")
'                    End If
'                    ' End of Spacing

'                    ' Divider #4
'                    sbData.Append("<tr>")
'                    sbData.Append("<td class=""leftSpace""><div class=""spacing"">&nbsp;</div></td>")
'                    sbData.Append("<td class=""middleSpace custom-border-bottom-dashed"">&nbsp;</td>")
'                    sbData.Append("<td class=""rightSpace""><div class=""spacing"">&nbsp;</div></td>")
'                    sbData.Append("</tr>")
'                    ' End of Divider #4

'                    sbData.Append("<tr>")
'                    sbData.Append("<td class=""leftSpace""><div class=""spacing"">&nbsp;</div></td>")
'                    sbData.Append("<td class=""middleSpace"">")

'                    ' Last section
'                    sbData.Append("<table class=""tblContent"">")
'                    sbData.Append("<tr>")
'                    sbData.Append("<td class=""txt-left-top label""><b>SENT BY</b></td>")
'                    sbData.Append("<td class=""txt-left-top colon""><b>:</b></td>")
'                    sbData.Append(String.Format("<td class=""txt-left-top detail"">{0}</td>", IIf(IsDBNull(dtDistinct.Rows(0)("SENTBY")), "", dtDistinct.Rows(0)("SENTBY"))))
'                    sbData.Append("<td class=""txt-left-top label""><b>CHARGE</b></td>")
'                    sbData.Append("<td class=""txt-left-top colon""><b>:</b></td>")
'                    sbData.Append(String.Format("<td class=""txt-left-top detail"">{0}</td>", IIf(IsDBNull(dtDistinct.Rows(0)("CHARGE")), "", dtDistinct.Rows(0)("CHARGE"))))
'                    sbData.Append("</tr>")
'                    sbData.Append("<tr>")
'                    sbData.Append("<td class=""txt-left-top label""><b>DATE</b></td>")
'                    sbData.Append("<td class=""txt-left-top colon""><b>:</b></td>")
'                    sbData.Append(String.Format("<td class=""txt-left-top detail"">{0}</td>", IIf(IsDBNull(dtDistinct.Rows(0)("CREATEDDATE")), "", Date.Parse(dtDistinct.Rows(0)("CREATEDDATE")).ToString("dd-MMM-yyyy"))))
'                    sbData.Append("<td class=""txt-left-top label""><b>TO CHARGE BY</b></td>")
'                    sbData.Append("<td class=""txt-left-top colon""><b>:</b></td>")
'                    sbData.Append(String.Format("<td class=""txt-left-top detail"">{0}</td>", IIf(IsDBNull(dtDistinct.Rows(0)("CHARGEBY")), "", dtDistinct.Rows(0)("CHARGEBY"))))
'                    sbData.Append("</tr>")
'                    sbData.Append("<tr>")
'                    sbData.Append("<td class=""txt-left-top label""><b>COURIER NO.</b></td>")
'                    sbData.Append("<td class=""txt-left-top colon""><b>:</b></td>")
'                    sbData.Append(String.Format("<td class=""txt-left-top detail"">{0}</td>", IIf(IsDBNull(dtDistinct.Rows(0)("COURIERNO")), "", dtDistinct.Rows(0)("COURIERNO"))))
'                    sbData.Append("<td class=""txt-left-top label"">&nbsp;</td>")
'                    sbData.Append("<td class=""txt-left-top colon"">&nbsp;</td>")
'                    sbData.Append("<td class=""txt-left-top detail"">&nbsp;</td>")
'                    sbData.Append("</tr>")
'                    sbData.Append("<tr>")
'                    sbData.Append("<td class=""txt-left-top label""><b>DATE SENT</b></td>")
'                    sbData.Append("<td class=""txt-left-top colon""><b>:</b></td>")
'                    sbData.Append(String.Format("<td class=""txt-left-top detail"">{0}</td>", IIf(IsDBNull(dtDistinct.Rows(0)("DATESENT")), "", IIf(Date.Parse(dtDistinct.Rows(0)("DATESENT")).ToString("dd-MMM-yyyy").Equals("01-Jan-1900"), "", Date.Parse(dtDistinct.Rows(0)("DATESENT")).ToString("dd-MMM-yyyy")))))
'                    sbData.Append("<td class=""txt-left-top label"">&nbsp;</td>")
'                    sbData.Append("<td class=""txt-left-top colon"">&nbsp;</td>")
'                    sbData.Append("<td class=""txt-left-top detail"">&nbsp;</td>")
'                    sbData.Append("</tr>")
'                    sbData.Append("</table>")
'                    ' End of Last section

'                    sbData.Append("</td>")
'                    sbData.Append("<td class=""rightSpace""><div class=""spacing"">&nbsp;</div></td>")
'                    sbData.Append("</tr>")
'                Next

'                ' Footer
'                sbFooter.Append("</table>")
'                sbFooter.Append("</body>")
'                sbFooter.Append("</html>")
'            ElseIf rptName = "SPL_ItemDeliver" Then
'                Dim recType As String = String.Empty

'                If dtData.Rows(0)("REC_TYPE") = "0" Then
'                    recType = "( Draft )"
'                ElseIf dtData.Rows(0)("REC_TYPE") = "1" Then
'                    recType = "( New )"
'                ElseIf dtData.Rows(0)("REC_TYPE") = "3" Then
'                    recType = "( Amended )"
'                End If

'                ' Header
'                sbHeader.Append("<html>")
'                sbHeader.Append("<head>")
'                sbHeader.Append("<style type=""text/css"">")
'                sbHeader.Append(".pageBreak { font-size: 1px; page-break-before: always; }")
'                sbHeader.Append(".txt-center-top { text-align: center; vertical-align: top; }")
'                sbHeader.Append(".txt-left-top { text-align: left; vertical-align: top; }")
'                sbHeader.Append(".txt-right-top { text-align: right; vertical-align: top; }")
'                sbHeader.Append(".txt-right-bottom { text-align: right; vertical-align: bottom; }")
'                sbHeader.Append("body, table, tr, td, span { font-family: ""Times New Roman""; font-size: 9pt; }")
'                sbHeader.Append("table { border: 0px solid black; border-collapse: collapse; width: 100%; }")
'                sbHeader.Append("table > tr > td.colon { width: 1%; vertical-align: top; }")
'                sbHeader.Append("div.spacing { width: 100%; }")
'                sbHeader.Append("table#mainRpt > tr > td.leftSpace, table#mainRpt > tr > td.rightSpace { width: 5%; }")
'                sbHeader.Append("table#mainRpt > tr > td.middleSpace { width: 90%; }")
'                sbHeader.Append("table.tblAddress > tr > td { color: blue; }")
'                sbHeader.Append("table.tblContent > tr > td.leftCompartment { width: 25%; }")
'                sbHeader.Append("table.tblContent > tr > td.dataCompartment { font-family: Arial; font-size: 9pt; width: 74%; }")
'                sbHeader.Append("table.tblContent > tr > td.label { width: 25%; }")
'                sbHeader.Append("table.tblContent > tr > td.detail { font-family: Arial; font-size: 9pt; width: 24%; }")
'                sbHeader.Append(".address { color: blue; font-size: 8pt; }")
'                sbHeader.Append(".companyName { font-family: ""Arial Black""; font-size: 12pt; }")
'                sbHeader.Append("</style>")
'                sbHeader.Append("</head>")
'                sbHeader.Append("<body>")
'                sbHeader.Append("<table id=""mainRpt"">")

'                ' Data (Report Content)
'                sbData.Append("<tr>")
'                sbData.Append("<td class=""leftSpace""><div class=""spacing"">&nbsp;</div></td>")
'                sbData.Append("<td class=""txt-center-top middleSpace"">")

'                ' Address section
'                sbData.Append("<table class=""tblAddress"">")
'                sbData.Append(String.Format("<tr><td class=""txt-center-top companyName"">{0}</td></tr>", IIf(IsDBNull(dtAddress.Rows(0)("COMPANY_NAME")), "", dtAddress.Rows(0)("COMPANY_NAME"))))
'                sbData.Append("<tr><td class=""address txt-center-top"">")
'                sbData.Append(String.Format("{0} {1}<br />", IIf(IsDBNull(dtAddress.Rows(0)("ADDR1")), "", Trim(dtAddress.Rows(0)("ADDR1"))), IIf(IsDBNull(dtAddress.Rows(0)("ADDR2")), "", Trim(dtAddress.Rows(0)("ADDR2")))))
'                sbData.Append(String.Format("{0} {1}<br />", IIf(IsDBNull(dtAddress.Rows(0)("ADDR3")), "", Trim(dtAddress.Rows(0)("ADDR3"))), IIf(IsDBNull(dtAddress.Rows(0)("ADDR4")), "", Trim(dtAddress.Rows(0)("ADDR4")))))
'                sbData.Append(String.Format("TEL: {0}<br />", IIf(IsDBNull(dtAddress.Rows(0)("TELNO")), "", Trim(dtAddress.Rows(0)("TELNO")))))
'                sbData.Append(String.Format("FAX: {0} / 04-3987366 / 04-3909595<br />", IIf(IsDBNull(dtAddress.Rows(0)("FAXNO")), "", Trim(dtAddress.Rows(0)("FAXNO")))))
'                sbData.Append("</td></tr>")
'                sbData.Append("</table>")
'                ' End of Address section

'                sbData.Append("</td>")
'                sbData.Append("<td class=""rightSpace""><div class=""spacing"">&nbsp;</div></td>")
'                sbData.Append("</tr>")

'                sbData.Append("<tr>")
'                sbData.Append("<td class=""leftSpace""><div class=""spacing"">&nbsp;</div></td>")
'                sbData.Append("<td class=""txt-center-top"">")

'                ' First section
'                sbData.Append("<table class=""tblContent"">")
'                sbData.Append("<tr>")
'                sbData.Append("<td class=""txt-left-top leftCompartment""><b>REQUEST NO</b></td>")
'                sbData.Append("<td class=""txt-left-top colon""><b>:</b></td>")
'                sbData.Append(String.Format("<td class=""txt-left-top dataCompartment"">{0} {1}</td>", IIf(IsDBNull(dtData.Rows(0)("REQUEST_NO")), "", dtData.Rows(0)("REQUEST_NO")), recType))
'                sbData.Append("</tr>")
'                sbData.Append("<tr>")
'                sbData.Append("<td class=""txt-left-top leftCompartment""><b>CREATOR</b></td>")
'                sbData.Append("<td class=""txt-left-top colon""><b>:</b></td>")
'                sbData.Append(String.Format("<td class=""txt-left-top dataCompartment"">{0}</td>", IIf(IsDBNull(dtData.Rows(0)("CREATED_BY")), "", dtData.Rows(0)("CREATED_BY"))))
'                sbData.Append("</tr>")
'                sbData.Append("<tr>")
'                sbData.Append("<td class=""txt-left-top leftCompartment""><b>DATE</b></td>")
'                sbData.Append("<td class=""txt-left-top colon""><b>:</b></td>")
'                sbData.Append(String.Format("<td class=""txt-left-top dataCompartment"">{0}</td>", IIf(IsDBNull(dtData.Rows(0)("CREATED_DATE")), "", Date.Parse(dtData.Rows(0)("CREATED_DATE")).ToString("dd/MM/yyyy"))))
'                sbData.Append("</tr>")
'                sbData.Append("<tr>")
'                sbData.Append("<td class=""txt-left-top leftCompartment""><b>SECTION</b></td>")
'                sbData.Append("<td class=""txt-left-top colon""><b>:</b></td>")
'                sbData.Append(String.Format("<td class=""txt-left-top dataCompartment"">{0}</td>", IIf(IsDBNull(dtData.Rows(0)("REGION_DESC")), "", dtData.Rows(0)("REGION_DESC"))))
'                sbData.Append("</tr>")
'                sbData.Append("</table>")
'                ' End of First section

'                sbData.Append("</td>")
'                sbData.Append("<td class=""rightSpace""><div class=""spacing"">&nbsp;</div></td>")
'                sbData.Append("</tr>")

'                ' Divider #1
'                sbData.Append("<tr>")
'                sbData.Append("<td class=""leftSpace""><div class=""spacing"">&nbsp;</div></td>")
'                sbData.Append("<td class=""middleSpace custom-border-top-dashed"">&nbsp;</td>")
'                sbData.Append("<td class=""rightSpace""><div class=""spacing"">&nbsp;</div></td>")
'                sbData.Append("</tr>")
'                ' End of Divider #1

'                sbData.Append("<tr>")
'                sbData.Append("<td class=""leftSpace""><div class=""spacing"">&nbsp;</div></td>")
'                sbData.Append("<td class=""txt-center-top"">")

'                ' Second section
'                sbData.Append("<table class=""tblContent"">")
'                sbData.Append("<tr>")
'                sbData.Append("<td class=""txt-left-top leftCompartment""><b>FOR THE ATTENTION OF</b></td>")
'                sbData.Append("<td class=""txt-left-top colon""><b>:</b></td>")
'                sbData.Append(String.Format("<td class=""txt-left-top dataCompartment"">{0}</td>", IIf(IsDBNull(dtData.Rows(0)("ATTN_TO")), "", dtData.Rows(0)("ATTN_TO"))))
'                sbData.Append("</tr>")
'                sbData.Append("<tr>")
'                sbData.Append("<td class=""txt-left-top leftCompartment""><b>REQUESTED BY / THROUGH</b></td>")
'                sbData.Append("<td class=""txt-left-top colon""><b>:</b></td>")
'                sbData.Append(String.Format("<td class=""txt-left-top dataCompartment"">{0}</td>", IIf(IsDBNull(dtData.Rows(0)("REQUEST_DESC")), "", dtData.Rows(0)("REQUEST_DESC"))))
'                sbData.Append("</tr>")
'                sbData.Append("<tr>")
'                sbData.Append("<td class=""txt-left-top leftCompartment""><b>PER YOUR REFERENCE</b></td>")
'                sbData.Append("<td class=""txt-left-top colon""><b>:</b></td>")
'                sbData.Append(String.Format("<td class=""txt-left-top dataCompartment"">{0}</td>", IIf(IsDBNull(dtData.Rows(0)("REF_DESC")), "", dtData.Rows(0)("REF_DESC"))))
'                sbData.Append("</tr>")
'                sbData.Append("<tr>")
'                sbData.Append("<td class=""txt-left-top leftCompartment""><b>COMPANY NAME</b></td>")
'                sbData.Append("<td class=""txt-left-top colon""><b>:</b></td>")
'                sbData.Append(String.Format("<td class=""txt-left-top dataCompartment"">{0}</td>", IIf(IsDBNull(dtData.Rows(0)("COMPANY_NAME")), "", dtData.Rows(0)("COMPANY_NAME"))))
'                sbData.Append("</tr>")
'                sbData.Append("<tr>")
'                sbData.Append("<td class=""txt-left-top leftCompartment""><b>COMPANY ADDRESS</b></td>")
'                sbData.Append("<td class=""txt-left-top colon""><b>:</b></td>")
'                sbData.Append(String.Format("<td class=""txt-left-top dataCompartment"">{0}</td>", IIf(IsDBNull(dtData.Rows(0)("ADDR1")), "", dtData.Rows(0)("ADDR1"))))
'                sbData.Append("</tr>")
'                sbData.Append("<tr>")
'                sbData.Append("<td class=""txt-left-top leftCompartment"">&nbsp;</td>")
'                sbData.Append("<td class=""txt-left-top colon"">&nbsp;</td>")
'                sbData.Append(String.Format("<td class=""txt-left-top dataCompartment"">{0}</td>", IIf(IsDBNull(dtData.Rows(0)("ADDR2")), "", dtData.Rows(0)("ADDR2"))))
'                sbData.Append("</tr>")
'                sbData.Append("<tr>")
'                sbData.Append("<td class=""txt-left-top leftCompartment"">&nbsp;</td>")
'                sbData.Append("<td class=""txt-left-top colon"">&nbsp;</td>")
'                sbData.Append(String.Format("<td class=""txt-left-top dataCompartment"">{0}</td>", IIf(IsDBNull(dtData.Rows(0)("ADDR3")), "", dtData.Rows(0)("ADDR3"))))
'                sbData.Append("</tr>")
'                sbData.Append("<tr>")
'                sbData.Append("<td class=""txt-left-top leftCompartment"">&nbsp;</td>")
'                sbData.Append("<td class=""txt-left-top colon"">&nbsp;</td>")
'                sbData.Append(String.Format("<td class=""txt-left-top dataCompartment"">{0}</td>", IIf(IsDBNull(dtData.Rows(0)("ADDR4")), "", dtData.Rows(0)("ADDR4"))))
'                sbData.Append("</tr>")
'                sbData.Append("<tr>")
'                sbData.Append("<td class=""txt-left-top leftCompartment""><b>TELEPHONE NO.</b></td>")
'                sbData.Append("<td class=""txt-left-top colon""><b>:</b></td>")
'                sbData.Append(String.Format("<td class=""txt-left-top dataCompartment"">{0}</td>", IIf(IsDBNull(dtData.Rows(0)("TEL_NO")), "", dtData.Rows(0)("TEL_NO"))))
'                sbData.Append("</tr>")
'                sbData.Append("</table>")
'                ' End of Second section

'                sbData.Append("</td>")
'                sbData.Append("<td class=""rightSpace""><div class=""spacing"">&nbsp;</div></td>")
'                sbData.Append("</tr>")
'                sbData.Append("<tr>")
'                sbData.Append("<td class=""leftSpace""><div class=""spacing"">&nbsp;</div></td>")
'                sbData.Append("<td class=""middleSpace""><div class=""spacing"">&nbsp;</div></td>")
'                sbData.Append("<td class=""rightSpace""><div class=""spacing"">&nbsp;</div></td>")
'                sbData.Append("</tr>")

'                ' Divider #2
'                sbData.Append("<tr>")
'                sbData.Append("<td class=""leftSpace""><div class=""spacing"">&nbsp;</div></td>")
'                sbData.Append("<td class=""middleSpace custom-border-top-dashed"">&nbsp;</td>")
'                sbData.Append("<td class=""rightSpace""><div class=""spacing"">&nbsp;</div></td>")
'                sbData.Append("</tr>")
'                ' End of Divider #2

'                sbData.Append("<tr>")
'                sbData.Append("<td class=""leftSpace""><div class=""spacing"">&nbsp;</div></td>")
'                sbData.Append("<td class=""txt-center-top"">")

'                ' Third section
'                sbData.Append("<table class=""tblContent"">")
'                sbData.Append("<tr>")
'                sbData.Append("<td class=""txt-left-top leftCompartment""><b>REMARK</b></td>")
'                sbData.Append("<td class=""txt-left-top colon""><b>:</b></td>")
'                sbData.Append(String.Format("<td class=""txt-left-top dataCompartment"">{0}</td>", IIf(IsDBNull(dtData.Rows(0)("REMARK")), "", System.Net.WebUtility.HtmlEncode(dtData.Rows(0)("REMARK")).Replace(vbCrLf, "<br />")))) 'MODIFIED BY CWTAN 20200714 To encode HTML special characters; Replace CRLF with HTML br tag
'                sbData.Append("</tr>")
'                sbData.Append("<tr>")
'                sbData.Append("<td colspan=""3"" class=""txt-left-top""><div class=""spacing"" style=""height: 120px;"">&nbsp;</div></td>")
'                sbData.Append("</tr>")
'                sbData.Append("</table>")
'                ' End of Third section

'                sbData.Append("</td>")
'                sbData.Append("<td class=""rightSpace""><div class=""spacing"">&nbsp;</div></td>")
'                sbData.Append("</tr>")

'                ' Divider #3
'                sbData.Append("<tr>")
'                sbData.Append("<td class=""leftSpace""><div class=""spacing"">&nbsp;</div></td>")
'                sbData.Append("<td class=""middleSpace custom-border-top-dashed"">&nbsp;</td>")
'                sbData.Append("<td class=""rightSpace""><div class=""spacing"">&nbsp;</div></td>")
'                sbData.Append("</tr>")
'                ' End of Divider #3

'                sbData.Append("<tr>")
'                sbData.Append("<td class=""leftSpace""><div class=""spacing"">&nbsp;</div></td>")
'                sbData.Append("<td class=""middleSpace"">")

'                ' Last section
'                sbData.Append("<table class=""tblContent"">")
'                sbData.Append("<tr>")
'                sbData.Append("<td class=""txt-left-top label""><b>SENT BY</b></td>")
'                sbData.Append("<td class=""txt-left-top colon""><b>:</b></td>")
'                sbData.Append(String.Format("<td class=""txt-left-top detail"">{0}</td>", IIf(IsDBNull(dtData.Rows(0)("SENT_BY")), "", dtData.Rows(0)("SENT_BY"))))
'                sbData.Append("<td class=""txt-left-top label""><b>CHARGE</b></td>")
'                sbData.Append("<td class=""txt-left-top colon""><b>:</b></td>")
'                sbData.Append(String.Format("<td class=""txt-left-top detail"">{0}</td>", IIf(IsDBNull(dtData.Rows(0)("CHARGE")), "", dtData.Rows(0)("CHARGE"))))
'                sbData.Append("</tr>")
'                sbData.Append("<tr>")
'                sbData.Append("<td class=""txt-left-top label""><b>DATE</b></td>")
'                sbData.Append("<td class=""txt-left-top colon""><b>:</b></td>")
'                sbData.Append(String.Format("<td class=""txt-left-top detail"">{0}</td>", IIf(IsDBNull(dtData.Rows(0)("CREATED_DATE")), "", Date.Parse(dtData.Rows(0)("CREATED_DATE")).ToString("dd/MM/yyyy"))))
'                sbData.Append("<td class=""txt-left-top label""><b>TO CHARGE BY</b></td>")
'                sbData.Append("<td class=""txt-left-top colon""><b>:</b></td>")
'                sbData.Append(String.Format("<td class=""txt-left-top detail"">{0}</td>", IIf(IsDBNull(dtData.Rows(0)("CHARGE_BY")), "", dtData.Rows(0)("CHARGE_BY"))))
'                sbData.Append("</tr>")
'                sbData.Append("<tr>")
'                sbData.Append("<td class=""txt-left-top label""><b>COURIER NO.</b></td>")
'                sbData.Append("<td class=""txt-left-top colon""><b>:</b></td>")
'                sbData.Append(String.Format("<td class=""txt-left-top detail"">{0}</td>", IIf(IsDBNull(dtData.Rows(0)("COURIER_NO")), "", dtData.Rows(0)("COURIER_NO"))))
'                sbData.Append("<td class=""txt-left-top label"">&nbsp;</td>")
'                sbData.Append("<td class=""txt-left-top colon"">&nbsp;</td>")
'                sbData.Append("<td class=""txt-left-top detail"">&nbsp;</td>")
'                sbData.Append("</tr>")
'                sbData.Append("<tr>")
'                sbData.Append("<td class=""txt-left-top label""><b>DATE SENT</b></td>")
'                sbData.Append("<td class=""txt-left-top colon""><b>:</b></td>")
'                sbData.Append(String.Format("<td class=""txt-left-top detail"">{0}</td>", IIf(IsDBNull(dtData.Rows(0)("DATE_SENT")), "", (IIf(Date.Parse(dtData.Rows(0)("DATE_SENT")).ToString("dd/MM/yyyy").Equals("01/01/1900"), "", Date.Parse(dtData.Rows(0)("DATE_SENT")).ToString("dd/MM/yyyy"))))))
'                sbData.Append("<td class=""txt-left-top label"">&nbsp;</td>")
'                sbData.Append("<td class=""txt-left-top colon"">&nbsp;</td>")
'                sbData.Append("<td class=""txt-left-top detail"">&nbsp;</td>")
'                sbData.Append("</tr>")
'                sbData.Append("</table>")
'                ' End of Last section

'                sbData.Append("</td>")
'                sbData.Append("<td class=""rightSpace""><div class=""spacing"">&nbsp;</div></td>")
'                sbData.Append("</tr>")

'                ' Footer
'                sbFooter.Append("</table>")
'                sbFooter.Append("</body>")
'                sbFooter.Append("</html>")
'            End If

'            ' Finalize the html code
'            finalHtmlCode += sbHeader.ToString
'            finalHtmlCode += sbData.ToString
'            finalHtmlCode += sbFooter.ToString

'            ExportAsPDF(context, rptName + "_" + dateStr + timeStr, finalHtmlCode)
'        End If

'    End Sub

'    ' Added by ANAS on 27/06/2020
'    Private Sub ExportAsPDF(ByVal context As HttpContext, ByVal fileName As String, ByVal rptContent As String)
'        Dim pdfDoc As New Document(PageSize.A4, 15.0F, 15.0F, 25.0F, 25.0F) '(PageSize.A4, 30.0F, 10.0F, 50.0F, 0.0F)
'        Dim writer As PdfWriter = PdfWriter.GetInstance(pdfDoc, context.Response.OutputStream)
'        writer.PageEvent = New ExportPDf.PageNumberFooter(New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 8.0F, iTextSharp.text.Font.NORMAL, BaseColor.BLACK), Element.ALIGN_RIGHT)

'        Using writer
'            pdfDoc.Open()

'            Dim tagProcessorFactory As ITagProcessorFactory = Tags.GetHtmlTagProcessorFactory()
'            tagProcessorFactory.AddProcessor(New ExportPDf.TableDataProcessor, New String() {HTML.Tag.TD})
'            tagProcessorFactory.AddProcessor(New ExportPDf.ImageTagProcessor, New String() {HTML.Tag.IMG})

'            Dim cssFiles As New CssFilesImpl
'            cssFiles.Add(XMLWorkerHelper.GetInstance().GetDefaultCSS)
'            Dim cssResolver As New StyleAttrCSSResolver(cssFiles)

'            Dim htmlContext As New HtmlPipelineContext(New CssAppliersImpl(New XMLWorkerFontProvider()))
'            htmlContext.SetAcceptUnknown(True).SetTagFactory(tagProcessorFactory)

'            Dim htmlPipeline As New HtmlPipeline(htmlContext, New PdfWriterPipeline(pdfDoc, writer))
'            Dim cssPipeline As New CssResolverPipeline(cssResolver, htmlPipeline)

'            Dim worker As New XMLWorker(cssPipeline, True)
'            Dim xmlParser As New XMLParser(True, worker, Encoding.UTF8)
'            xmlParser.Parse(New StringReader(rptContent))

'            pdfDoc.Close()
'        End Using

'        context.Response.ContentType = "application/pdf"
'        context.Response.AddHeader("content-disposition", "attachment;filename=" & fileName & ".pdf")
'        context.Response.Cache.SetCacheability(HttpCacheability.NoCache)
'        context.Response.Write(pdfDoc)
'        context.Response.End()
'    End Sub

'    ' Added by ANAS on 27/06/2020
'    Private Function QRCode(ByVal qrCodeByte() As Byte) As String
'        QRCode = Convert.ToBase64String(qrCodeByte)
'    End Function

'    ' Added by ANAS on 27/06/2020
'    Private Function ConvertToDataTable(Of T)(ByVal list As IList(Of T)) As System.Data.DataTable
'        Dim table As New System.Data.DataTable()
'        Dim props() As Reflection.PropertyInfo = GetType(T).GetProperties
'        For Each prop As Reflection.PropertyInfo In props
'            table.Columns.Add(prop.Name.ToUpper, prop.PropertyType)
'        Next
'        For Each item As T In list
'            Dim row As DataRow = table.NewRow()
'            For Each prop As Reflection.PropertyInfo In props
'                row(prop.Name) = prop.GetValue(item)
'            Next
'            table.Rows.Add(row)
'        Next
'        Return table
'    End Function

'    Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
'        Get
'            Return False
'        End Get
'    End Property

'End Class