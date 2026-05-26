namespace PAB_NewAquarium.Models
{
    public class LoginResponse
    {
        public string JWTToken { get; set; }
        public string JWTRefreshToken { get; set; }
    }
}