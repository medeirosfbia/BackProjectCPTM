using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace ApiOracle.Data
{
    public class OracleConnectionFactory
    {
        private readonly IConfiguration _config;

        public OracleConnectionFactory(IConfiguration config)
        {
            _config = config;
        }

        public IDbConnection CreateConnection()
        {
            var conn = new OracleConnection(_config.GetConnectionString("Oracle"));
            conn.Open();
            return conn;
        }
    }
}