using RestaurantOrdering.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantOrdering.Model.Response
{

    public class Item
    {
        public int DishId { get; set; }
        public string DishName { get; set; }
        public int Price { get; set; }
        public string Category { get; set; }
        public int Qty { get; set; }
    }

    public class CartItemResponse
    {
        public List<Item> CartItem { get; set; }
    }

    public class ActiveCartResponse
    {
        public int CartId { get; set; }
    }
}
