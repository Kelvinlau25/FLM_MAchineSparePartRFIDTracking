using System.ComponentModel.DataAnnotations;

namespace PAB_NewAquarium.Models
{
    public class TriggerForgetPasswordViewModel
    {
        [Required]
        public string EmailAddress { get; set; }
    }
}