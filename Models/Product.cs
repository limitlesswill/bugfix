using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceWebSite.Dashboard.Models
{
    public class Product
    {
        public int id { get; set; }
        public string enName { get; set; }
        public string arName { get; set; }
        public string EnDescription { get; set; }
        public string ArDescription { get; set; }
        public string imgURL { get; set; }
        public int Quantity { get; set; }
        public decimal price { get; set; }
        public int SubCategoryId { get; set; }
    }
}
