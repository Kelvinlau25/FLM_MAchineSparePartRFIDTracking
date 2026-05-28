using System;
using System.Collections.Generic;

namespace PAB_NewAquarium.Models
{
    public class SimilarityRequestModel
    {
        public int idMmMainDropdown { get; set; }
        public List<SimilarityInputModel> input { get; set; }
        public List<SimilarityCompareModel> compareList { get; set; }
        public String api_key { get; set; }
    }

    public class SimilarityResponseModel
    {
        public String info { get; set; }
        public int score { get; set; }
        public List<Object[]> similarityList { get; set; }
        public Object test { get; set; }
    }

    public class SimilarityInputModel
    {
        public int FABRIC_TYPE { get; set; }
        public int WEAVE_TYPE { get; set; }
        public int WARP_COUNT1 { get; set; }
        public int WEFT_COUNT1 { get; set; }
        public int COMPOSITION { get; set; }
    }

    public class SimilarityCompareModel
    {
        public int id { get; set; }
        public int FABRIC_TYPE { get; set; }
        public int WEAVE_TYPE { get; set; }
        public int WARP_COUNT1 { get; set; }
        public int WEFT_COUNT1 { get; set; }
        public int COMPOSITION { get; set; }
    }
}