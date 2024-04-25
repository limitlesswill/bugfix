using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceWebSite.Dashboard.ViewModel
{
    public class addOrUpdateProductViewModel
    {
        public int id { get; set; }
        [Required(ErrorMessage = " English name is required."), MaxLength(50, ErrorMessage = "Product name cannot exceed 50 characters.")]
        public string enName { get; set; }
        [Required(ErrorMessage = " Arabic name is required."), MaxLength(50, ErrorMessage = "Product name cannot exceed 50 characters.")]
        public string arName { get; set; }
        [Required(ErrorMessage = "EnDescription is required.")]
        public string EnDescription { get; set; }
        [Required(ErrorMessage = "ArDescription is required.")]
        public string ArDescription { get; set; }
        public string imgURL { get; set; }
        [Required(ErrorMessage = "Quantity is required."), Range(0, int.MaxValue, ErrorMessage = "Quantity must be a positive number.")]
        public int Quantity { get; set; }
        [Required(ErrorMessage = "Price is required."), Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number.")]
        public decimal price { get; set; }
        [Required(ErrorMessage = "SubCategoryId is required.")]
        public int SubCategoryId { get; set; }
    }
}
