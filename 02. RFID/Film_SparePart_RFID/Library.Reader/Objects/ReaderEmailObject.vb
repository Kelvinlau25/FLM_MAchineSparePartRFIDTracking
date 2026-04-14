Namespace Objects

    Public Class ReaderEmailObject

        Public Sub New(ByVal device_name As String, ByVal ip_address As String, ByVal location_desc As String, ByVal time_occured As DateTime, ByVal message As String)

            Me.DeviceName = device_name
            Me.IPAddress = ip_address
            Me.LocationDesc = location_desc
            Me.TimeOccured = time_occured
            Me.Message = message

        End Sub

        Public Property Company As String

        Public Property DeviceName As String

        Public Property IPAddress As String

        Public Property LocationDesc As String

        Public Property TimeOccured As DateTime

        Public Property Message As String


    End Class


End Namespace
