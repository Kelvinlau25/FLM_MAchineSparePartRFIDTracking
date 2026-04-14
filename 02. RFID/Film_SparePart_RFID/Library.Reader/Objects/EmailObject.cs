using System;
using System.Collections.Generic;

namespace Library.Reader.Objects
{
    public class EmailObject
    {
        public EmailObject(string company, string mail_to, string mail_cc, string mail_bcc, bool send_flag, List<ReaderEmailObject> reader_object_list)
        {
            Company = company;
            MailTo = mail_to;
            MailCc = mail_cc;
            MailBcc = mail_bcc;
            SendFlag = send_flag;

            for (int i = 0; i <= reader_object_list.Count - 1; i++)
            {
                Body += string.Format("<h3>{3}. [{1}] at {2}</h3>{0}", Environment.NewLine, reader_object_list[i].DeviceName, reader_object_list[i].LocationDesc, i + 1);
                Body += string.Format("<p>Device Name : <strong>{1}</strong></p>{0}", Environment.NewLine, reader_object_list[i].DeviceName);
                Body += string.Format("<p>IP Address : <strong>{1}</strong></p>{0}", Environment.NewLine, reader_object_list[i].IPAddress);
                Body += string.Format("<p>Time Occured : <strong>{1}</strong></p>{0}", Environment.NewLine, reader_object_list[i].TimeOccured.ToString());
                Body += string.Format("<p>Message : <strong>{1}</strong></p>{0}", Environment.NewLine, reader_object_list[i].Message);
                Body += string.Format("{0}{0}", "<br/>");
            }
        }

        public string Company { get; set; }
        public string MailTo { get; set; }
        public string MailCc { get; set; }
        public string MailBcc { get; set; }
        public bool SendFlag { get; set; }
        public string Body { get; set; }
    }
}
