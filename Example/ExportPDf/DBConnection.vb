Imports System.Threading.Tasks
Imports Microsoft.VisualBasic
Imports System.Net.Http
Imports Newtonsoft.Json

Public Class DBConnection

    Public Async Function OpenAclConnection(ByVal parameter As String) As Task(Of String)

        Try
            Dim connectionName As ConnectionString = New ConnectionString()
            Dim connectionResult As ConnectionString = New ConnectionString()

            connectionName.ConnectionStringDBName = parameter

            Dim client = New HttpClient()
            Dim clientbodystr As ByteArrayContent = New StringContent(JsonConvert.SerializeObject(connectionName), Encoding.UTF8, "application/json")
            Dim respone As HttpResponseMessage = client.PostAsync("http://cld-tgm-app001.toray.my:140/api/hello/GetConnectionByParams", clientbodystr).Result


            respone.EnsureSuccessStatusCode()

            If (respone.IsSuccessStatusCode) Then
                Dim readTask = Await respone.Content.ReadAsStringAsync()
                connectionResult = JsonConvert.DeserializeObject(Of ConnectionString)(readTask)

                Return connectionResult.ConnectionStringDBResult
            Else
                Return Nothing
            End If
        Catch ex As Exception
            Return ex.ToString
        End Try
    End Function

End Class
