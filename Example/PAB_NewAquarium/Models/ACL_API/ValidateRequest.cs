namespace PAB_NewAquarium.Models
{
    public class ValidateRequest
    {
        public string JWTToken { get; set; }
        public string JWTRefreshToken { get; set; }
        public string DatabaseType { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
    }
}