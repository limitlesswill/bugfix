using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceWebSite.Dashboard.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please enter your email")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "Please enter your password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        //[Display(Name = "Remember Me")]
        //public bool RememberMe { get; set; }
    }
}
