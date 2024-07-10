using RestaurantOrdering.Model.Entities;
using RestaurantOrdering.Model.Request;
using RestaurantOrdering.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantOrdering.Infrastructure.Repository.Interface
{
    public interface ICartRepository
    {
        Task<bool> Create(AddDishToCartRequest request, int cartid);
        Task<int> GetCartIdByToken(string token);
        Task<List<Item>> GetAllDishByCartId(int cartid);
        Task<bool> CreateNewCart(int customerid);
        Task<bool> UpdateCartItem(int cartid, int dishid, int qty);
        Task<bool> DeleteCartItem(int cartid, int dishid);
    }
}
