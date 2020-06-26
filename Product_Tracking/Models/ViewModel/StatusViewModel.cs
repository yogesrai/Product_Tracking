using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Product_Tracking.Models.ViewModel
{
    public class StatusViewModel
    {
        public int StatusId { get; set; }
        [Required(ErrorMessage = "StatusName Required!")]
        public string StatusName { get; set; }
    }
}