using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using ManageATenancyAPI.Configuration;
using ManageATenancyAPI.Models.Housing.NHO;
using Microsoft.Extensions.Options;

namespace ManageATenancyAPI.Repository
{
    public class TraRoleRepository : BaseRepository, ITraRoleRepository
    {
        public TraRoleRepository(IOptions<ConnStringConfiguration> connectionStringConfig) : base(connectionStringConfig)
        {
        }

        public async Task<List<TraRole>> List()
        {
            using (var connection = GetOpenConnection(_connectionStringConfig.UHWReportingWarehouse))
            {
                var result = connection.Query<TraRole>(
                    "SELECT * FROM TraRole");
                return result.ToList();
            }
        }
    }
}
