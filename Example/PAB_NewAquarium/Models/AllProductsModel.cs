using PAB_NewAquarium.Helpers;
using System.Collections.Generic;

namespace PAB_NewAquarium.Models
{
    public class AllProductsModel
    {
        public string ALL_PRODUCTS_JSON { get; set; }
        public string RECOMMENDED_PRODUCTS_JSON { get; set; }
        public string FABRIC_TYPE_JSON { get; set; }
        public string PRODUCT_TYPE_JSON { get; set; }
        public string MATERIAL_TYPE_JSON { get; set; }
        public string COMPOSITION_JSON { get; set; }
        public string WEIGHT_JSON { get; set; }
        public List<ALL_PRODUCTS> ALL_PRODUCTS { get; set; }
        public List<RECOMMENDED_PRODUCTS> RECOMMENDED_PRODUCTS { get; set; }
        public List<FABRIC_TYPE_LIST> FABRIC_TYPE_LIST { get; set; }
        public List<PRODUCT_TYPE_LIST> PRODUCT_TYPE_LIST { get; set; }
        public List<MATERIAL_TYPE_LIST> MATERIAL_TYPE_LIST { get; set; }
        public List<WEAVE_TYPE_LIST> WEAVE_TYPE_LIST { get; set; }
        public List<WEIGHT_LIST> WEIGHT_LIST { get; set; }

        public AllProductsModel()
        {
            ALL_PRODUCTS = new List<ALL_PRODUCTS>();
            RECOMMENDED_PRODUCTS = new List<RECOMMENDED_PRODUCTS>();
            FABRIC_TYPE_LIST = new List<FABRIC_TYPE_LIST>();
            PRODUCT_TYPE_LIST = new List<PRODUCT_TYPE_LIST>();
            MATERIAL_TYPE_LIST = new List<MATERIAL_TYPE_LIST>();
            WEAVE_TYPE_LIST = new List<WEAVE_TYPE_LIST>();
            WEIGHT_LIST = new List<WEIGHT_LIST>();
        }
    }

    public class ALL_PRODUCTS
    {
        #region Property for implement custom logic
        private string _PRODUCT_TYPE;
        private string _MATERIAL_TYPE;
        private string _WEAVE_TYPE;
        #endregion

        public int PRODUCT_H_ID { get; set; }
        public string IMAGE_URL { get; set; }
        public string CHOP_NO { get; set; }
        public string CATEGORY_NAME { get; set; }
        public string FABRIC_TYPE { get; set; }      
        public string PRODUCT_TYPE
        {
            get
            {
                return CommonMethod.ToTitleCase(_PRODUCT_TYPE);
            }
            set
            {
                _PRODUCT_TYPE = value;
            }
        }       
        public string MATERIAL_TYPE
        {
            get
            {
                return CommonMethod.ToTitleCase(_MATERIAL_TYPE);
            }
            set
            {
                _MATERIAL_TYPE = value;
            }
        }        
        public string WEAVE_TYPE
        {
            get
            {
                return CommonMethod.ToTitleCase(_WEAVE_TYPE);
            }
            set
            {
                _WEAVE_TYPE = value;
            }
        }
        public string FAB_WEIGHT_GSM { get; set; }
        public string CURRENT_QTY { get; set; }
        public string ISGARMENT { get; set; }
    }

    public class RECOMMENDED_PRODUCTS
    {
        public int PRODUCT_H_ID { get; set; }
        public string IMAGE_URL { get; set; }
        public string CATEGORY_NAME { get; set; }
        public string CHOP_NO { get; set; }
    }

    public class FABRIC_TYPE_LIST
    {
        public string FABRIC_TYPE { get; set; }
    }

    public class PRODUCT_TYPE_LIST
    {
        private string _PRODUCT_TYPE;
        public string PRODUCT_TYPE
        {
            get
            {
                return CommonMethod.ToTitleCase(_PRODUCT_TYPE);
            }
            set
            {
                _PRODUCT_TYPE = value;
            }
        }
    }

    public class MATERIAL_TYPE_LIST
    {
        private string _MATERIAL_TYPE;
        public string MATERIAL_TYPE
        {
            get
            {
                return CommonMethod.ToTitleCase(_MATERIAL_TYPE);
            }
            set
            {
                _MATERIAL_TYPE = value;
            }
        }
    }

    public class WEAVE_TYPE_LIST
    {
        private string _WEAVE_TYPE;
        public string WEAVE_TYPE
        {
            get
            {
                return CommonMethod.ToTitleCase(_WEAVE_TYPE);
            }
            set
            {
                _WEAVE_TYPE = value;
            }
        }
    }

    public class WEIGHT_LIST
    {
        public string FAB_WEIGHT_GSM { get; set; }
    }
}