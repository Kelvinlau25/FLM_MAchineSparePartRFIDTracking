using System;
using System.Net.Mail;
using Library.Reader.Objects;
using Symbol.RFID3;

namespace Library.Reader.Helpers
{
    public class Mailer
    {
        internal RFIDReader m_ReaderAPI;

        public static bool SendEmail(ref EmailObject _email)
        {
            bool result = false;

            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    string subject = string.Format("{0} Reader Notification : These reader requires your attention.", _email.Company);
                    string body = _email.Body;

                    SmtpClient smtpServer = new SmtpClient("10.200.1.1", 25);
                    mail.IsBodyHtml = true;
                    mail.From = new MailAddress("apps_notification@toray.com.my");

                    foreach (string receiver in _email.MailTo.Split(','))
                    {
                        mail.To.Add(receiver);
                    }

                    mail.Subject = subject;
                    mail.Body = body;

                    smtpServer.Send(mail);

                    _email.SendFlag = true;
                    result = true;
                }
            }
            catch (Exception)
            {
            }

            return result;
        }

        public static bool SendDBEmailMSSQL(ref EmailObject _email)
        {
            bool result = false;

            string subject = string.Format("{0} Reader Notification : These readers requires your attention.", "Film");
            string body = @"Dear Sir/Madam,<br/><br/>
                                  Please refer to the following SPARE PART RFID details.<br/>
                                  <br/>" + _email.Body + @"<br/>
                                  Thank You. ";

            string mailTo = _email.MailTo;
            string mailFrom = "appsnotification.tms.mb@mail.toray";
            string smtpHostname = "10.252.130.24";
            string smptPort = "25";
            int Count = 1;

            Library.Database.RFID_COMMON db = new Library.Database.RFID_COMMON();
            Library.Database.DTO dto = new Library.Database.DTO();

            dto = db.RFID_SendMail_MSSQL(body, subject, mailTo);

            if (!dto.Error)
            {
                result = true;
            }

            return result;
        }

        public static bool SendDBFailEmailMSSQL(ref EmailObject _email)
        {
            bool result = false;

            string subject = string.Format("{0} Reader Notification : These readers have failed to connect.", "Film");
            string body = @"Dear Sir/Madam,<br/><br/>
                                  Please be informed that the following SPARE PART RFID has failed to connect.<br/>
                                  <br/>" + _email.Body + @"<br/>
                                  Thank You. ";

            string mailTo = _email.MailTo;
            string mailFrom = "appsnotification.tms.mb@mail.toray";
            string smtpHostname = "10.252.130.24";
            string smptPort = "25";
            int Count = 1;

            Library.Database.RFID_COMMON db = new Library.Database.RFID_COMMON();
            Library.Database.DTO dto = new Library.Database.DTO();

            dto = db.RFID_SendMail_MSSQL(body, subject, mailTo);

            if (!dto.Error)
            {
                result = true;
            }

            return result;
        }

        public static bool SendDBTagEmailMSSQL(ref EmailTagObject _email)
        {
            bool result = false;

            string subject = string.Format("{0} Reader Notification : These tags requires your attention.", "Film");
            string body = @"Dear Sir/Madam,<br/><br/>
                                  Please refer to the following SPARE PART RFID tag details.<br/>
                                  <br/>" + _email.Body + @"<br/>
                                  Thank You. ";

            string mailTo = _email.MailTo;
            string mailFrom = "appsnotification.tms.mb@mail.toray";
            string smtpHostname = "10.252.130.24";
            string smptPort = "25";
            int Count = 1;

            Library.Database.RFID_COMMON db = new Library.Database.RFID_COMMON();
            Library.Database.DTO dto = new Library.Database.DTO();

            dto = db.RFID_SendMail_MSSQL(body, subject, mailTo);

            if (!dto.Error)
            {
                result = true;
            }

            return result;
        }
    }
}
