namespace PAB_NewAquarium.Models
{
    public class AIAutoKey
    {
        public string APIName { get; set; }
        public string SystemName { get; set; }
        public string CompanyName { get; set; }
    }

    public class AIAutoKeyResult
    {
        public string msg { get; set; }
        public string APIName { get; set; }
        public string APIKey { get; set; }
        public string APIURL { get; set; }
    }
}