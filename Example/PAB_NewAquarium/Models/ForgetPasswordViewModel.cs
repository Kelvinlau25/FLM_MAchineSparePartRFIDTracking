using System.ComponentModel.DataAnnotations;

namespace PAB_NewAquarium.Models
{
    public class ForgetPasswordViewModel
    {
        [Required]
        public string EmailAddress { get; set; }
        [Required]
        public string OTP { get; set; }
        [Required]
        public string NewPass { get; set; }
        [Required]
        public string ConfirmNewPass { get; set; }
    }
}