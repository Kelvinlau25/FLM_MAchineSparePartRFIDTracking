namespace PAB_NewAquarium.Models
{
    public class ChangePasswordModel
    {
        public string OLD_PASSWORD { get; set; }
        public string NEW_PASSWORD { get; set; }
        public string CONFIRM_NEW_PASSWORD { get; set; }
    }
}