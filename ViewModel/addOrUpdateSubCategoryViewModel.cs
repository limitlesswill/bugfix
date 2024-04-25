using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceWebSite.Dashboard.ViewModel
{
    public class addOrUpdateSubCategoryViewModel
    {
        [Required]
        public int id { get; set; }
        [Required(ErrorMessage = " Arabic name is required.")]
        [MaxLength(50, ErrorMessage = "SubCategory name cannot exceed 50 characters.")]
        public string ArName { get; set; }

        [Required(ErrorMessage = " English name is required.")]
        [MaxLength(50, ErrorMessage = "SubCategory name cannot exceed 50 characters.")]
        public string EnName { get; set; }
        [Required]
        public int categoryid { get; set; }
        
    }
}
