﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantOrdering.Model.Request
{
    public class CartRequest
    {
    }

    public class AddDishToCartRequest
    {
        public int DishId { get; set; }
        public int Qty { get; set; }
    }
}
