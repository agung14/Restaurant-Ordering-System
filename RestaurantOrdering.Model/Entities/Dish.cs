using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantOrdering.Model.Entities
{
    public class Dish
    {
        public int DishId { get; set; }
        public string DishName { get; set; }
        public int Price { get; set; }
        public string Category { get; set; }
    }
}
