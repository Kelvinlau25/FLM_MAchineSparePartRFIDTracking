using System;
using System.IO;
using System.Windows.Forms;

namespace FILM_RFID_READER_DEPLOY
{
    public class logger
    {
        private readonly string strFile =
            Path.Combine(Application.StartupPath, "Log", "ErrorLog_" + DateTime.Today.ToString("dd-MMM-yyyy") + ".txt");

        public void _LogRead(string tagid, string reader, string indicator, string no_of_rfid)
        {
            EnsureLogDirectory();

            bool fileExists = File.Exists(strFile);
            using (StreamWriter writer = new StreamWriter(strFile, true))
            {
                if (!fileExists)
                {
                    writer.WriteLine("Starting Error Log for today " + DateTime.Now.ToString("dd/MM/yy hh:mm:ss"));
                }

                writer.WriteLine("Read: " + tagid + "-->" + "Reader: " + reader + "-" + DateTime.Now.ToString("dd/MM/yy hh:mm:ss") + " Indicator: " + indicator);
                writer.WriteLine("No of RFID Tag in loop : " + no_of_rfid);
            }
        }

        public void _LogNoOfTag(string no_of_rfid)
        {
            EnsureLogDirectory();

            bool fileExists = File.Exists(strFile);
            using (StreamWriter writer = new StreamWriter(strFile, true))
            {
                if (!fileExists)
                {
                    writer.WriteLine("Starting Error Log for today " + DateTime.Now.ToString("dd/MM/yy hh:mm:ss"));
                }

                writer.WriteLine("No of RFID Tag outside loop : " + no_of_rfid + " - " + DateTime.Now.ToString("dd/MM/yy hh:mm:ss"));
            }
        }

        public void _LogDetect(string status, string reader, string indicator)
        {
            EnsureLogDirectory();

            bool fileExists = File.Exists(strFile);
            using (StreamWriter writer = new StreamWriter(strFile, true))
            {
                if (!fileExists)
                {
                    writer.WriteLine("Starting Error Log for today " + DateTime.Now.ToString("dd/MM/yy hh:mm:ss"));
                }

                writer.WriteLine("Forklift Detected: " + status + "-->" + "Reader: " + reader + "-" + DateTime.Now.ToString("dd/MM/yy hh:mm:ss") + " Indicator: " + indicator);
            }
        }

        public void _LogCheck(string status)
        {
            EnsureLogDirectory();

            bool fileExists = File.Exists(strFile);
            using (StreamWriter writer = new StreamWriter(strFile, true))
            {
                if (!fileExists)
                {
                    writer.WriteLine("Starting Error Log for today " + DateTime.Now.ToString("dd/MM/yy hh:mm:ss"));
                }

                writer.WriteLine("MU Gate Check: " + status + "-" + DateTime.Now.ToString("dd/MM/yy hh:mm:ss"));
            }
        }

        public void _LogTriggerGPI(string action, string value, string hostname, bool eventData)
        {
            EnsureLogDirectory();

            bool fileExists = File.Exists(strFile);
            using (StreamWriter writer = new StreamWriter(strFile, true))
            {
                if (!fileExists)
                {
                    writer.WriteLine("Starting Error Log for today " + DateTime.Now.ToString("dd/MM/yy hh:mm:ss"));
                }

                writer.WriteLine("Trigger GPI Event: - " + DateTime.Now.ToString("dd/MM/yy hh:mm:ss") + " - Action : " + action + " - eventData.GPIEventData.PortNumber : " + value + " - Hostname : " + hostname + " - eventData.GPIEventData.GPIEvent : " + eventData.ToString());
            }
        }

        public void _LogResetMU(string msg)
        {
            EnsureLogDirectory();

            bool fileExists = File.Exists(strFile);
            using (StreamWriter writer = new StreamWriter(strFile, true))
            {
                if (!fileExists)
                {
                    writer.WriteLine("Starting Error Log for today " + DateTime.Now.ToString("dd/MM/yy hh:mm:ss"));
                }

                writer.WriteLine("Message : " + msg + " - " + DateTime.Now.ToString("dd/MM/yy hh:mm:ss"));
            }
        }

        public void _LogGen(string msg)
        {
            EnsureLogDirectory();

            bool fileExists = File.Exists(strFile);
            using (StreamWriter writer = new StreamWriter(strFile, true))
            {
                if (!fileExists)
                {
                    writer.WriteLine("Starting Error Log for today " + DateTime.Now.ToString("dd/MM/yy hh:mm:ss"));
                }

                writer.WriteLine(msg + " - " + DateTime.Now.ToString("dd/MM/yy hh:mm:ss"));
            }
        }

        private void EnsureLogDirectory()
        {
            string logFolder = Path.Combine(Application.StartupPath, "Log");

            if (!Directory.Exists(logFolder))
            {
                Directory.CreateDirectory(logFolder);
            }
        }
    }
}