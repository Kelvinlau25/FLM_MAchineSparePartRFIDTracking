namespace PAB_NewAquarium.Models
{
    public class LogoutRequest
    {
        public string JWTToken { get; set; }
        public string JWTRefreshToken { get; set; }
    }
}