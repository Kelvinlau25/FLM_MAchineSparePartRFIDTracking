using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACL_System.Models
{
    public class SimilarityRequestModel
    {
        public SimilarityRequestModel(string input, List<string> compareList) {
            this.input = input;
            this.compareList = compareList;
            this.API_Key = "";
        }
        public String input { get; set; }
        public List<String> compareList { get; set; }
        public string API_Key { get; set; }
    }
}