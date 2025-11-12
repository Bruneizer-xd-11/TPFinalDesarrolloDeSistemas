using System.Data;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace TPfinal.DapperDapperData
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string? _connectionString; // <- ahora acepta null

        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public IDbConnection CreateConnection()
            => new MySqlConnection(_connectionString ?? string.Empty); // evita null
    }
}
