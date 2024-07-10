using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantOrdering.Model.Entities
{
    public class Voucher
    {
        public string VoucherCode { get; set; }
        public int DiscountAmount { get; set; }
    }
}
