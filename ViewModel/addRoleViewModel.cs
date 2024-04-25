using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceWebSite.Dashboard.ViewModel
{
    public class addRoleViewModel
    {
        [Required(ErrorMessage = "The Role Required ")]
        [Display(Name = "Enter The Role Name")]
        public string RoleName { set; get; }
    }
}
