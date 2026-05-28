using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PAB_NewAquarium.Models
{
    public class WordDetectionModel
    {
        [Required]
        public string api_key { get; set; }
        [Required]
        public string input { get; set; }
        [Required]
        public List<string> compareList { get; set; }
    }
}