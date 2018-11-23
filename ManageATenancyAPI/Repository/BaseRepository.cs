using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Configuration;
using Microsoft.Extensions.Options;

namespace ManageATenancyAPI.Repository
{
    public abstract class BaseRepository
    {
        protected ConnStringConfiguration _connectionStringConfig;
        public BaseRepository(IOptions<ConnStringConfiguration> connectionStringConfig)
        {
            _connectionStringConfig = connectionStringConfig.Value;
        }


        protected DbConnection GetOpenConnection(string connectionString)
        {
            var connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }
    }
}
