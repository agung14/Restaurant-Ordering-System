using RestaurantOrdering.Model.Entities;
using RestaurantOrdering.Model.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantOrdering.Infrastructure.Repository.Interface
{
    public interface IOrderRepository
    {
        Task<bool> Update(UpdateCartRequest request);
        Task<Orders> CreateOrderLog(int cartid, string vouchercode);
        Task<int> GetDiscountVoucherByCode(string vouchercode);

    }
}
