using Microsoft.AspNetCore.Http;
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
        [Required(ErrorMessage = " English name is required.")]
        public string enName { get; set; }
        [Required(ErrorMessage = " English image is required.")]
        public IFormFile imgURL { get; set; }
        [Required(ErrorMessage = " Arabic name is required.")]
        public string arName { get; set; }
        [Required(ErrorMessage = "Arabic image is required.")]
        public IFormFile imgURLAr { get; set; }
    }
}

