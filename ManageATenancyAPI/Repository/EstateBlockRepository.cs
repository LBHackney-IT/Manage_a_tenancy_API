using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using ManageATenancyAPI.Configuration;
using ManageATenancyAPI.Models.Housing.NHO;
using Microsoft.Extensions.Options;

namespace ManageATenancyAPI.Repository
{
    public class EstateBlockRepository : IEstateBlockRepository
    {
        private ConnStringConfiguration _connectionStringConfig;
        public EstateBlockRepository(IOptions<ConnStringConfiguration> connectionStringConfig)
        {
            _connectionStringConfig = connectionStringConfig.Value;
        }


        protected DbConnection GetOpenConnection()
        {
            var connection = new SqlConnection(_connectionStringConfig.ManageATenancyDatabase);
            connection.Open();

            return connection;
        }
        public async Task<IEnumerable<EstateBlock>> GetBlocksByEstateId(int estateId)
        {
            using (var connection = GetOpenConnection())
            {
                var results = await connection.QueryAsync<EstateBlock>("Select * From Block where EstateId=@EstateId",
                    new {EstateId = estateId});
                return results;
            }
        }
    }
}
