using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantOrdering.Infrastructure.Context
{
    public class MySqlContext : DbContext
    {

        private readonly IConfiguration _configuration;
        private readonly string _connetionString;
        public MySqlContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connetionString = _configuration.GetConnectionString("DefaultConnection")!;
        }

        public IDbConnection Connect()
        {
            return new MySqlConnection(_connetionString);
        }
    }
}
