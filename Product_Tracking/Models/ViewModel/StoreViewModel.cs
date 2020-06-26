using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Product_Tracking.Models.ViewModel
{
    public class StoreViewModel
    {
        public int StoreId { get; set; }
        [Required(ErrorMessage = "StoreName Required!")]
        public string StoreName { get; set; }
        [Required(ErrorMessage = "StoreLocation Required!")]
        public string StoreLocation { get; set; }
        [Required(ErrorMessage = "StoreCapacity Required!")]
        public string StoreCapacity { get; set; }
    }
}