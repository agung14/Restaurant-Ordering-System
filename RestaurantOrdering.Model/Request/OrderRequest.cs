using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantOrdering.Model.Request
{
    public class OrderRequest
    {
        public string VoucherCode { get; set; }
    }

    public class UpdateCartRequest
    {
        public int cartid { get; set; }
        public int totalprice { get; set; }
        public int totalitem { get; set; }
        public bool ischeckout { get; set; }
    }
}
