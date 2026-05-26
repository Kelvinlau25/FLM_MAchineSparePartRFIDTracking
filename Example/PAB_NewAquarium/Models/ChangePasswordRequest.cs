namespace PAB_NewAquarium.Models
{
    public class ChangePasswordRequest
    {
        public object LoginID { get; set; }
        public string OldPass { get; set; }
        public string NewPass { get; set; }
        public string CompanyCode { get; set; }
        public string DatabaseType { get; set; }
    }
}