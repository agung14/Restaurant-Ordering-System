using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantOrdering.Model.Entities
{
    public class Orders
    {
        public int orderid { get; set; } 
        public string VoucherCode { get; set; } 
        public DateTime ordertime { get; set; }
    }
}
