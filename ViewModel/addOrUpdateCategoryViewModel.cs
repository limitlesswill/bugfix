using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceWebSite.Dashboard.ViewModel
{
    public class addOrUpdateCategoryViewModel
    {
        [Required]
        public int id { get; set; }
        [Required(ErrorMessage = " Arabic name is required.")]
        public string ArName { get; set; }
        [Required(ErrorMessage = " English name is required.")]
        public string EnName { get; set; }
        [Required(ErrorMessage = " imgURL is required.")]
        public string imgURL { get; set; }
        [Required(ErrorMessage = "imgURLAr is required.")]
        public string imgURLAr { get; set; }
    }
}

