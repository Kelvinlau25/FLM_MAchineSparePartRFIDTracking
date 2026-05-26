using System;
using System.Collections.Generic;

namespace PAB_NewAquarium.Models
{
    public class Notification
    {
        public List<NotificationList> NotificationListing { get; set; }
    }

    public class NotificationList
    {
        public int NOTIFICATION_ID { get; set; }
        public int COLUMN_ID { get; set; }
        public bool ISSEEN { get; set; }
        public string CATEGORY_NAME { get; set; }
        public string CHOP_NO { get; set; }
        public DateTime CREATED_DATE { get; set; }
    }

    
}