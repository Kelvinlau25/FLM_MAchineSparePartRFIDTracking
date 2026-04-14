Imports Symbol.RFID3
Imports Symbol.RFID3.Events
Imports Symbol.RFID3.GPI_PORT_STATE
Imports Symbol.RFID3.GPIs
Imports System.Net.Mail
Imports System.IO
Imports Library.Reader.Objects



Namespace Helpers
    Public Class Mailer
        Friend m_ReaderAPI As RFIDReader

        Public Shared Function SendEmail(ByRef _email As EmailObject) As Boolean

            SendEmail = False

            Try
                Using mail As New MailMessage()

                    Dim subject As String = String.Format("{0} Reader Notification : These reader requires your attention.", _email.Company)
                    Dim body As String = _email.Body

                    Dim SmtpServer As New SmtpClient("10.200.1.1", 25)
                    mail.IsBodyHtml = True

                    mail.From = New MailAddress("apps_notification@toray.com.my")

                    For Each _receiver As String In _email.MailTo.Split(",")
                        mail.To.Add(_receiver)
                    Next

                    mail.Subject = subject
                    mail.Body = body

                    SmtpServer.Send(mail)

                    _email.SendFlag = True
                    SendEmail = True

                End Using


            Catch ex As Exception

            End Try

        End Function


        Public Shared Function SendDBEmailMSSQL(ByRef _email As EmailObject) As Boolean
            SendDBEmailMSSQL = False
            'Dim hostname As String = m_ReaderAPI.HostName()

            Dim subject As String = String.Format("{0} Reader Notification : These readers requires your attention.", "Film")
            'Dim subject As String = String.Format("{0} Reader Notification : These readers requires your attention.", "Film")
            'Dim body As String = "SPART PART RFID has connected successfully"
            'HANA 17112023 changing email format

            Dim body As String = "Dear Sir/Madam,<br/><br/>
                                  Please refer to the following SPARE PART RFID details.<br/>
                                  <br/>" + _email.Body + "<br/>
                                  Thank You. "

            'HANA Added mailto again
            Dim mailTo As String = _email.MailTo
            Dim mailFrom As String = "appsnotification.tms.mb@mail.toray"
            'Dim mailCc As String = _email.MailCc
            'Dim mailBcc As String = _email.MailBcc
            'Dim mailText As String = String.Empty
            Dim smtpHostname As String = "10.252.130.24"
            Dim smptPort As String = "25"
            Dim Count As Integer = 1

            Dim db As New Library.Database.RFID_COMMON
            Dim dto As New Library.Database.DTO

            'dto = db.RFID_SendMail_MSSQL(mailTo, mailFrom, mailCc, mailBcc, subject, mailText, body, smtpHostname, smptPort)
            dto = db.RFID_SendMail_MSSQL(body, subject, mailTo)

            If Not dto.Error Then
                '_email.SendFlag = True
                SendDBEmailMSSQL = True
            End If
        End Function

        Public Shared Function SendDBFailEmailMSSQL(ByRef _email As EmailObject) As Boolean
            SendDBFailEmailMSSQL = False
            'Dim hostname As String = m_ReaderAPI.HostName()


            Dim subject As String = String.Format("{0} Reader Notification : These readers have failed to connect.", "Film")
            'Dim body As String = "SPART PART RFID has connected successfully"
            'HANA 17112023 changing email format

            Dim body As String = "Dear Sir/Madam,<br/><br/>
                                  Please be informed that the following SPARE PART RFID has failed to connect.<br/>
                                  <br/>" + _email.Body + "<br/>
                                  Thank You. "


            'HANA Added mailto again
            Dim mailTo As String = _email.MailTo
            Dim mailFrom As String = "appsnotification.tms.mb@mail.toray"
            'Dim mailCc As String = _email.MailCc
            'Dim mailBcc As String = _email.MailBcc
            'Dim mailText As String = String.Empty
            Dim smtpHostname As String = "10.252.130.24"
            Dim smptPort As String = "25"
            Dim Count As Integer = 1

            Dim db As New Library.Database.RFID_COMMON
            Dim dto As New Library.Database.DTO

            'dto = db.RFID_SendMail_MSSQL(mailTo, mailFrom, mailCc, mailBcc, subject, mailText, body, smtpHostname, smptPort)
            dto = db.RFID_SendMail_MSSQL(body, subject, mailTo)

            If Not dto.Error Then
                '_email.SendFlag = True
                SendDBFailEmailMSSQL = True
            End If
        End Function

        Public Shared Function SendDBTagEmailMSSQL(ByRef _email As EmailTagObject) As Boolean
            SendDBTagEmailMSSQL = False
            'Dim hostname As String = m_ReaderAPI.HostName()

            Dim subject As String = String.Format("{0} Reader Notification : These tags requires your attention.", "Film")
            'Dim subject As String = String.Format("{0} Reader Notification : These readers requires your attention.", "Film")
            'Dim body As String = "SPART PART RFID has connected successfully"
            'HANA 17112023 changing email format

            Dim body As String = "Dear Sir/Madam,<br/><br/>
                                  Please refer to the following SPARE PART RFID tag details.<br/>
                                  <br/>" + _email.Body + "<br/>
                                  Thank You. "

            'HANA Added mailto again
            Dim mailTo As String = _email.MailTo
            Dim mailFrom As String = "appsnotification.tms.mb@mail.toray"
            'Dim mailCc As String = _email.MailCc
            'Dim mailBcc As String = _email.MailBcc
            'Dim mailText As String = String.Empty
            Dim smtpHostname As String = "10.252.130.24"
            Dim smptPort As String = "25"
            Dim Count As Integer = 1

            Dim db As New Library.Database.RFID_COMMON
            Dim dto As New Library.Database.DTO

            'dto = db.RFID_SendMail_MSSQL(mailTo, mailFrom, mailCc, mailBcc, subject, mailText, body, smtpHostname, smptPort)
            dto = db.RFID_SendMail_MSSQL(body, subject, mailTo)

            If Not dto.Error Then
                '_email.SendFlag = True
                SendDBTagEmailMSSQL = True
            End If
        End Function
    End Class

End Namespace

