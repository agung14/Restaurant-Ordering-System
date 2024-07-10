using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantOrdering.Model.Request
{
    public class CustomerRequest
    {
        public string? TableCode { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
    }
}
