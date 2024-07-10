using RestaurantOrdering.Model.Entities;
using RestaurantOrdering.Model.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantOrdering.Infrastructure.Repository.Interface
{
    public interface ICustomerRepository
    {
        Task<bool> Create(CustomerRequest request, string token);
        Task<int> GetCustIdByToken(string token);
        Task<List<Dish>> GetAllDish();
        Task<List<Voucher>> GetAllVoucher();
    }
}
