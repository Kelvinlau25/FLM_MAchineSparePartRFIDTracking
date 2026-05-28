using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACL_System.Models
{
    public class SimilarityResponseModel
    {
        
        public String info { get; set; }
        public int score { get; set; }
        public List<Object[]> similarityList { get; set; }
        public Object test { get; set; }
        
    }
    
}