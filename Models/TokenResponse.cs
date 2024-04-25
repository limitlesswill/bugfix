using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceWebSite.Dashboard.Models
{
    public class TokenResponse
    {
        public string token { get; set; }
        public DateTime expiration { get; set; }
    }
}
