using Microsoft.Extensions.Options;
using SalesOrder.Models;
using System.Data;
using System.Data.SqlClient;

namespace SalesOrder.Repositories
{
    public class BaseRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly AppSettings _appSettings;

        public BaseRepository(IConfiguration configuration, IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            _configuration = configuration;
            _connectionString = appSettings.Value.DatabaseConnection;
        }

        protected IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
