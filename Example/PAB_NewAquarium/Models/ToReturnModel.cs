using System.Collections.Generic;

namespace PAB_NewAquarium.Models
{
    public class ToReturnModel
    {
        public List<ToReturnListModel> RETURN_LIST { get; set; }
    }

    public class ToReturnListModel
    {
        public int PRODUCT_H_ID { get; set; }
        public string REFERENCE_NO { get; set; }
        public string CHOP_NO { get; set; }
        public string CATEGORY_NAME { get; set; }
        public string HANGER_LOC { get; set; }
        public string CURRENT_LOC { get; set; }
        public string DATE_TAKEN { get; set; }
        public string IMAGE_URL { get; set; }
    }
}