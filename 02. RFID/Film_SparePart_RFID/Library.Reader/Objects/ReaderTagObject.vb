Namespace Objects
    Public Class ReaderTagObject
        Public Sub New(ByVal hostName As String, ByVal location As String, ByVal antennaID As String, ByVal tagID As String, ByVal time_occured As DateTime)

            Me.Host = hostName
            Me.Location = location
            Me.AntennaID = antennaID
            Me.TagID = tagID
            Me.TimeOccured = time_occured

        End Sub

        Public Property Host As String

        Public Property Location As String

        Public Property AntennaID As String

        Public Property TagID As String

        Public Property TimeOccured As DateTime

        'Public Property Message As String
    End Class
End Namespace
