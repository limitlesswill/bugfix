using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceWebSite.Dashboard.ViewModel
{
    public class OrderUserDto
    {
        public int Id { get; set; }
        public int State { get; set; }

        public string Address { get; set; }

        public string UserName { get; set; }
    }
}
