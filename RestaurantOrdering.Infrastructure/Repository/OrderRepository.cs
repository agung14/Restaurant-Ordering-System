using Dapper;
using RestaurantOrdering.Infrastructure.Context;
using RestaurantOrdering.Infrastructure.Repository.Interface;
using RestaurantOrdering.Model.Entities;
using RestaurantOrdering.Model.Request;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantOrdering.Infrastructure.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly MySqlContext _context;
        public OrderRepository(MySqlContext context)
        {
            _context = context;
        }

        public async Task<bool> Update(UpdateCartRequest request)
        {
            var query = @" UPDATE cart
                           SET totalprice = @totalprice, totalitem = @totalitem, ischeckout = @ischeckout
                           WHERE cartid = @cartid;";

            var parameters = new DynamicParameters();
            parameters.Add("cartid", request.cartid, DbType.Int32);
            parameters.Add("totalprice", request.totalprice, DbType.Int32);
            parameters.Add("totalitem", request.totalitem, DbType.Int32);
            parameters.Add("ischeckout", request.ischeckout, DbType.Int32);

            using var connection = _context.Connect();

            var result = await connection.ExecuteAsync(query, parameters) > 0;

            return result;

        }

        public async Task<Orders> CreateOrderLog(int cartid, string vouchercode)
        {
            var query = @"INSERT INTO orders(cartid, vouchercode, ordertime)
                          VALUES(@cartid, @vouchercode, @ordertime);
                          set @lastinsertid = LAST_INSERT_ID();
                          SELECT * from orders where orderid = @lastinsertid";

            var parameters = new DynamicParameters();
            parameters.Add("cartid", cartid, DbType.Int32);
            parameters.Add("vouchercode", vouchercode, DbType.String);
            parameters.Add("ordertime", DateTime.Now.ToLocalTime(), DbType.DateTime);

            using var connection = _context.Connect();

            var result = await connection.QuerySingleOrDefaultAsync<Orders>(query, parameters);

            return result;

        }

        public async Task<int> GetDiscountVoucherByCode(string vouchercode)
        {
            var query = @"SELECT discountamount FROM voucher WHERE vouchercode = @vouchercode";

            var parameters = new DynamicParameters();
            parameters.Add("vouchercode", vouchercode, DbType.String);

            using var connection = _context.Connect();

            var result = await connection.QuerySingleOrDefaultAsync<int>(query, parameters);

            return result;

        }
    }
}
