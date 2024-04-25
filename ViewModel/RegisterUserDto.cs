using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceWebSite.Dashboard.ViewModel
{
    public class RegisterUserDto
    {
        [Required]

        [Display(Name = "First Name")]
        public string fName { set; get; }
        [Display(Name = "Last Name")]
        public string lName { set; get; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        [Required]
        public string email { set; get; }
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [Required]
        public string password { set; get; }
        [DataType(DataType.Password)]
        [Display(Name = "Comfirm Password")]
        [Required]
        [Compare("password")]
        public string comfirmPassword { set; get; }
        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { set; get; }
      
    }
}
