using System.ComponentModel.DataAnnotations;

namespace PAB_NewAquarium.Models
{
    public class VerifyOTPViewModel
    {
        [Required]
        public string EmailAddress { get; set; }
        [Required]
        public string OTP { get; set; }
    }
}