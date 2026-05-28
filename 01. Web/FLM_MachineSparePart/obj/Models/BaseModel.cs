using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FILM_Sparepart_MVC.Models
{
    public class BaseModel
    {
        public string createdBy { get; set; }
        public DateTime createdDate { get; set; }
        public string updatedBy { get; set; }
        public DateTime updatedDate { get; set; }
    }
}