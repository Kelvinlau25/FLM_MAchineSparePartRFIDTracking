namespace PAB_NewAquarium.Models
{
    public class LoginRequest
    {
        public string LoginID { get; set; }
        public string LoginPass { get; set; }
        public string SystemName { get; set; }
        public string CompanyCode { get; set; }
        public string DatabaseType { get; set; }
    }
}