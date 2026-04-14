Namespace Objects
    Public Class EmailTagObject
        Public Sub New(ByVal company As String, ByVal mail_to As String, ByVal mail_cc As String, ByVal mail_bcc As String, ByVal send_flag As Boolean, ByVal reader_object_list As List(Of ReaderTagObject))

            Me.Company = company
            Me.MailTo = mail_to
            Me.MailCc = mail_cc
            Me.MailBcc = mail_bcc
            Me.SendFlag = send_flag

            For i As Integer = 0 To reader_object_list.Count - 1

                Me.Body &= String.Format("<h3>{3}. Location : {1} ({2}) </h3>{0}", Environment.NewLine, reader_object_list(i).Location, reader_object_list(i).Host, i + 1)
                Me.Body &= String.Format("<p>Antenna ID : <strong>{1}</strong></p>{0}", Environment.NewLine, reader_object_list(i).AntennaID)
                Me.Body &= String.Format("<p>Tag ID : <strong>{1}</strong></p>{0}", Environment.NewLine, reader_object_list(i).TagID)
                Me.Body &= String.Format("<p>Time Occured : <strong>{1}</strong></p>{0}", Environment.NewLine, reader_object_list(i).TimeOccured.ToString)
                'Me.Body &= String.Format("<p>Message : <strong>{1}</strong></p>{0}", Environment.NewLine, reader_object_list(i).Message)

                Me.Body &= String.Format("{0}{0}", "<br/>")

            Next

        End Sub

        Public Property Company As String

        Public Property MailTo As String

        Public Property MailCc As String

        Public Property MailBcc As String

        Public Property SendFlag As Boolean

        Public Property Body As String
    End Class

End Namespace

