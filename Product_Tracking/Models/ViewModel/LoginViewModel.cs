using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Product_Tracking.Models.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username Required!")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password Required!")]
        public string Password { get; set; }
        //public bool RememberMe { get; set; }

    }
}