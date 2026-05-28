namespace PAB_NewAquarium.Models
{
    public class ResetPasswordRequest
    {
        public string EmailAddress { get; set; }
        public string CompanyCode { get; set; }
        public string OTP { get; set; }
        public string NewPass { get; set; }
        public string DatabaseType { get; set; }

    }
}