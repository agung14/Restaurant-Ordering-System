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
    public class CustomerRepository : ICustomerRepository
    {
        private readonly MySqlContext _context;
        public CustomerRepository(MySqlContext context)
        {
            _context = context;
        }

        public async Task<bool> Create(CustomerRequest request, string token)
        {
            var query = @"INSERT INTO customerlog(tablecode,name,email,token)
                          VALUES(@tablecode,@name,@email,@token);
                          set @lastinsertid = LAST_INSERT_ID();
                          INSERT INTO cart(customerlogid) VALUES(@lastinsertid);";

            var parameters = new DynamicParameters();
            parameters.Add("tablecode", request.TableCode, DbType.String);
            parameters.Add("name", request.Name, DbType.String);
            parameters.Add("email", request.Email, DbType.String);
            parameters.Add("token", token, DbType.String);

            using var connection = _context.Connect();

            var result = await connection.ExecuteAsync(query, parameters) > 0;

            return result;

        }

        public async Task<int> GetCustIdByToken(string token)
        {
            var query = @"SELECT customerlogid from customerlog where token = @token;";

            var parameters = new DynamicParameters();
            parameters.Add("token", token, DbType.String);

            using var connection = _context.Connect();
            var result = await connection.QuerySingleOrDefaultAsync<int>(query, parameters);

            return result;

        }

        public async Task<List<Dish>> GetAllDish()
        {
            var query = @"SELECT * from dish";

            using var connection = _context.Connect();
            var result = await connection.QueryAsync<Dish>(query);

            return result.ToList();

        }

        public async Task<List<Voucher>> GetAllVoucher()
        {
            var query = @"SELECT * from voucher";

            using var connection = _context.Connect();
            var result = await connection.QueryAsync<Voucher>(query);

            return result.ToList();

        }
    }
}
