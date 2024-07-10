using Dapper;
using Microsoft.EntityFrameworkCore;
using RestaurantOrdering.Infrastructure.Context;
using RestaurantOrdering.Infrastructure.Repository.Interface;
using RestaurantOrdering.Model.Entities;
using RestaurantOrdering.Model.Request;
using RestaurantOrdering.Model.Response;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantOrdering.Infrastructure.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly MySqlContext _context;
        public CartRepository(MySqlContext context)
        {
            _context = context;
        }

        public async Task<bool> Create(AddDishToCartRequest request, int cartid)
        {
            var query = @"INSERT INTO cartitem(cartid, dishid, qty)
                          VALUES(@cartid, @dishid, @qty);";

            var parameters = new DynamicParameters();
            parameters.Add("cartid", cartid, DbType.Int32);
            parameters.Add("dishid", request.DishId, DbType.Int32);
            parameters.Add("qty", request.Qty, DbType.Int32);

            using var connection = _context.Connect();

            var result = await connection.ExecuteAsync(query, parameters) > 0;

            return result;

        }

        public async Task<bool> CreateNewCart(int customerid)
        {
            var query = @"INSERT INTO cart(customerlogid)
                          VALUES(@customerid);";

            var parameters = new DynamicParameters();
            parameters.Add("customerid", customerid, DbType.Int32);

            using var connection = _context.Connect();

            var result = await connection.ExecuteAsync(query, parameters) > 0;

            return result;

        }

        public async Task<int> GetCartIdByToken(string token)
        {
            var query = @"select crt.cartid from cart crt 
                          join customerlog cust on crt.customerlogid = cust.customerlogid
                          where cust.token = @token and crt.ischeckout = false";

            var parameters = new DynamicParameters();
            parameters.Add("token", token, DbType.String);

            using var connection = _context.Connect();
            var result = await connection.QuerySingleOrDefaultAsync<int>(query, parameters);

            return result;

        }

        public async Task<List<Item>> GetAllDishByCartId(int cartid)
        {
            var query = @"select dis.*, crtm.qty from cartitem crtm
                        join dish dis on crtm.dishid = dis.dishid
                        where crtm.cartid = @cartid ";

            var parameters = new DynamicParameters();
            parameters.Add("cartid", cartid, DbType.Int32);

            using var connection = _context.Connect();
            var result = await connection.QueryAsync<Item>(query, parameters);

            return result.ToList();

        }

        public async Task<bool> UpdateCartItem(int cartid, int dishid, int qty)
        {
            var query =  @"UPDATE cartitem
                           SET qty = @qty
                           WHERE dishid = @dishid 
                           and cartid = @cartid;";

            var parameters = new DynamicParameters();
            parameters.Add("cartid", cartid, DbType.Int32);
            parameters.Add("dishid", dishid, DbType.Int32);
            parameters.Add("qty", qty, DbType.Int32);

            using var connection = _context.Connect();

            var result = await connection.ExecuteAsync(query, parameters) > 0;

            return result;

        }

        public async Task<bool> DeleteCartItem(int cartid, int dishid)
        {

            var query = "DELETE FROM cartitem WHERE dishid = @dishid and cartid = @cartid;";

            var parameters = new DynamicParameters();
            parameters.Add("cartid", cartid, DbType.Int32);
            parameters.Add("dishid", dishid, DbType.Int32);

            using var connection = _context.Connect();

            var result = await connection.ExecuteAsync(query, parameters) > 0;

            return result;

        }
    }
}
