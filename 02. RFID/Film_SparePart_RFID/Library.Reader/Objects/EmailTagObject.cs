using System;
using System.Collections.Generic;

namespace Library.Reader.Objects
{
    public class EmailTagObject
    {
        public EmailTagObject(string company, string mail_to, string mail_cc, string mail_bcc, bool send_flag, List<ReaderTagObject> reader_object_list)
        {
            Company = company;
            MailTo = mail_to;
            MailCc = mail_cc;
            MailBcc = mail_bcc;
            SendFlag = send_flag;

            for (int i = 0; i <= reader_object_list.Count - 1; i++)
            {
                Body += string.Format("<h3>{3}. Location : {1} ({2}) </h3>{0}", Environment.NewLine, reader_object_list[i].Location, reader_object_list[i].Host, i + 1);
                Body += string.Format("<p>Antenna ID : <strong>{1}</strong></p>{0}", Environment.NewLine, reader_object_list[i].AntennaID);
                Body += string.Format("<p>Tag ID : <strong>{1}</strong></p>{0}", Environment.NewLine, reader_object_list[i].TagID);
                Body += string.Format("<p>Time Occured : <strong>{1}</strong></p>{0}", Environment.NewLine, reader_object_list[i].TimeOccured.ToString());
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
