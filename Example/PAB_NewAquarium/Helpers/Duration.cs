using System;
using System.Reflection;

namespace PAB_NewAquarium.Helpers
{
    public class Duration
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int Second { get; set; }
        public int Millisecond { get; set; }

        public Duration()
        {
            this.Year = 0;
            this.Month = 0;
            this.Day = 0;
            this.Hour = 0;
            this.Minute = 0;
            this.Second = 0;
            this.Millisecond = 0;
        }

        public Duration(string date)
        {
            char[] seperator = new char[] { '-', ' ', ':', '.' };
            string[] items = date.Split(seperator);

            this.Year = Convert.ToInt32(items[0]);
            this.Month = Convert.ToInt32(items[1]);
            this.Day = Convert.ToInt32(items[2]);
            this.Hour = Convert.ToInt32(items[3]);
            this.Minute = Convert.ToInt32(items[4]);
            this.Second = Convert.ToInt32(items[5]);
            this.Millisecond = Convert.ToInt32(items[6]);
        }

        public Duration(int year = 0, int month = 0, int day = 0, int hour = 0, int minute = 0, int second = 0, int millisecond = 0)
        {
            this.Year = year;
            this.Month = month;
            this.Day = day;
            this.Hour = hour;
            this.Minute = minute;
            this.Second = second;
            this.Millisecond = millisecond;
        }

        public Duration ParseString(string date)
        {
            return new Duration(date);
        }

        public override string ToString()
        {
            PropertyInfo[] properties = this.GetType().GetProperties();
            string text = "";
            int count = 0;

            foreach (PropertyInfo property in properties)
            {
                int value = Convert.ToInt32(property.GetValue(this));
                string word = value + " " + property.Name.ToLower();

                if (value > 1)
                {
                    word = value + " " + property.Name.ToLower() + "s";
                }

                if (count < (properties.Length - 1)) text += word + ", "; else text += word;
                count++;
            }

            return text;
        }

        public string ToString(string format)
        {
            char pad = '0';

            string text = format.Replace("yyyy", this.Year.ToString().PadLeft(4, pad));
            text = text.Replace("MM", this.Month.ToString().PadLeft(2, pad));
            text = text.Replace("dd", this.Day.ToString().PadLeft(2, pad));
            text = text.Replace("HH", this.Hour.ToString().PadLeft(2, pad));
            text = text.Replace("mm", this.Minute.ToString().PadLeft(2, pad));
            text = text.Replace("ss", this.Second.ToString().PadLeft(2, pad));
            text = text.Replace("fff", this.Millisecond.ToString().PadLeft(3, pad));

            return text;
        }

        public static Duration Add(Duration a, Duration b)
        {
            int millisecond = a.Millisecond + b.Millisecond;
            int second = a.Second + b.Second;
            int minute = a.Minute + b.Minute;
            int hour = a.Hour + b.Hour;
            int day = a.Day + b.Day;
            int month = a.Month + b.Month;
            int year = a.Year + b.Year;

            if (millisecond >= 1000)
            {
                second += (millisecond - (millisecond % 1000)) / 1000;
                millisecond = millisecond % 1000;
            }

            if (second >= 60)
            {
                minute += (second - (second % 60)) / 60;
                second = second % 60; 
            }

            if (minute >= 60)
            {
                hour += (minute - (minute % 60)) / 60;
                minute = minute % 60;
            }

            if (hour >= 24)
            {
                day += (hour - (hour % 24)) / 24;
                hour = hour % 24;
            }

            if (day >= 30)
            {
                month += (day - (day % 30)) / 30;
                day = day % 30;
            }

            if (month >= 12)
            {
                year += (month - (month % 12)) / 12;
                month = month % 12;
            }

            return new Duration
            {
                Millisecond = millisecond,
                Second = second,
                Minute = minute,
                Hour = hour,
                Day = day,
                Month = month,
                Year = year
            };
        }

        public static double ToYears(Duration duration)
        {
            PropertyInfo[] properties = duration.GetType().GetProperties();
            double total = 0;

            foreach (PropertyInfo property in properties)
            {
                double value = Convert.ToDouble(property.GetValue(duration));
                double converted = 0;
                string field = property.Name;

                switch (field.ToLower())
                {
                    case "year":
                        converted = value;
                        break;
                    case "month":
                        converted = value / 12;
                        break;
                    case "day":
                        converted = value / 365;
                        break;
                    case "hour":
                        converted = value / 8760;
                        break;
                    case "minute":
                        converted = value / 525600;
                        break;
                    case "second":
                        converted = value / (3.154 * Math.Pow(10, 7));
                        break;
                    case "millisecond":
                        converted = value / (3.154 * Math.Pow(10, 10));
                        break;
                }

                total += converted;
            }

            return Math.Round(total, 4);
        }

        public static double ToMonths(Duration duration)
        {
            PropertyInfo[] properties = duration.GetType().GetProperties();
            double total = 0;

            foreach (PropertyInfo property in properties)
            {
                double value = Convert.ToDouble(property.GetValue(duration));
                double converted = 0;
                string field = property.Name;

                switch (field.ToLower())
                {
                    case "year":
                        converted = value * 12;
                        break;
                    case "month":
                        converted = value;
                        break;
                    case "day":
                        converted = value / 30;
                        break;
                    case "hour":
                        converted = value / 730;
                        break;
                    case "minute":
                        converted = value / 43800;
                        break;
                    case "second":
                        converted = value / (2.628 * Math.Pow(10, 6));
                        break;
                    case "millisecond":
                        converted = value / (2.628 * Math.Pow(10, 9));
                        break;
                }

                total += converted;
            }

            return Math.Round(total, 4);
        }

        public static double ToDays(Duration duration)
        {
            PropertyInfo[] properties = duration.GetType().GetProperties();
            double total = 0;

            foreach (PropertyInfo property in properties)
            {
                double value = Convert.ToDouble(property.GetValue(duration));
                double converted = 0;
                string field = property.Name;

                switch (field.ToLower())
                {
                    case "year":
                        converted = value * 365;
                        break;
                    case "month":
                        converted = value * 30;
                        break;
                    case "day":
                        converted = value;
                        break;
                    case "hour":
                        converted = value / 24;
                        break;
                    case "minute":
                        converted = value / 1440;
                        break;
                    case "second":
                        converted = value / 86400;
                        break;
                    case "millisecond":
                        converted = value / (8.64 * Math.Pow(10, 7));
                        break;
                }

                total += converted;
            }

            return Math.Round(total, 4);
        }

        public static double ToHours(Duration duration)
        {
            PropertyInfo[] properties = duration.GetType().GetProperties();
            double total = 0;

            foreach (PropertyInfo property in properties)
            {
                double value = Convert.ToDouble(property.GetValue(duration));
                double converted = 0;
                string field = property.Name;

                switch (field.ToLower())
                {
                    case "year":
                        converted = value * 8760;
                        break;
                    case "month":
                        converted = value * 730;
                        break;
                    case "day":
                        converted = value * 24;
                        break;
                    case "hour":
                        converted = value;
                        break;
                    case "minute":
                        converted = value / 60; 
                        break;
                    case "second":
                        converted = value / 3600;
                        break;
                    case "millisecond":
                        converted = value / (3.6 * Math.Pow(10, 6));
                        break;
                }

                total += converted;
            }

            return Math.Round(total, 4);
        }

        public static double ToMinutes(Duration duration)
        {
            PropertyInfo[] properties = duration.GetType().GetProperties();
            double total = 0;

            foreach (PropertyInfo property in properties)
            {
                double value = Convert.ToDouble(property.GetValue(duration));
                double converted = 0;
                string field = property.Name;

                switch (field.ToLower())
                {
                    case "year":
                        converted = value * 525600;
                        break;
                    case "month":
                        converted = value * 43800;
                        break;
                    case "day":
                        converted = value * 1440;
                        break;
                    case "hour":
                        converted = value * 60;
                        break;
                    case "minute":
                        converted = value;
                        break;
                    case "second":
                        converted = value / 60;
                        break;
                    case "millisecond":
                        converted = value / 60000;
                        break;
                }

                total += converted;
            }

            return Math.Round(total, 4);
        }
        public static bool isDateCorrect(string date) {
            DateTime dateTime;
            DateTime? dat = null;
            if (DateTime.TryParse(date, out dateTime) == false)
            {
                return false;
            }
            else {
                dat = dateTime;
            }

            if (!dat.HasValue)
            {
                //unassigned
                return true;
            }
            else {
                return false;
            }
        }
        public static DateTime? ConvertToDate(string date)
        {
            DateTime dateTime;
            DateTime? dat = null;
            if (DateTime.TryParse(date, out dateTime) == false)
            {
                return null;
            }
            else
            {
                dat = dateTime;
            }
            return dat;
        }
        public static double ToSeconds(Duration duration)
        {
            PropertyInfo[] properties = duration.GetType().GetProperties();
            double total = 0;

            foreach (PropertyInfo property in properties)
            {
                double value = Convert.ToDouble(property.GetValue(duration));
                double converted = 0;
                string field = property.Name;

                switch (field.ToLower())
                {
                    case "year":
                        converted = value * (3.154 * Math.Pow(10, 7));
                        break;
                    case "month":
                        converted = value * (2.628 * Math.Pow(10, 6));
                        break;
                    case "day":
                        converted = value * 86400;
                        break;
                    case "hour":
                        converted = value * 3600;
                        break;
                    case "minute":
                        converted = value * 60;
                        break;
                    case "second":
                        converted = value;
                        break;
                    case "millisecond":
                        converted = value / 1000;
                        break;
                }

                total += converted;
            }

            return Math.Round(total, 4);
        }
    }
}