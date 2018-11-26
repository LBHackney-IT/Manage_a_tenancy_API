using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using ManageATenancyAPI.Configuration;
using ManageATenancyAPI.Models.Housing.NHO;
using ManageATenancyAPI.Models.UniversalHousing;
using Microsoft.Extensions.Options;

namespace ManageATenancyAPI.Repository
{
    public class EstateRepository : BaseRepository, IEstateRepository
    {
        public EstateRepository(IOptions<ConnStringConfiguration> connectionStringConfig) : base(connectionStringConfig)
        {
        }
        public Task<List<Estate>> GetEstates(IList<string> estateIds)
        {
            using (var connection = GetOpenConnection(_connectionStringConfig.UHWReportingWarehouse))
            {
                var fullResults = connection.Query<UhProperty>(
                    "SELECT * FROM property WHERE prop_ref in @estateIds and level_code=2", new { EstateIds = estateIds });

                var results = new List<Estate>();
                foreach (var uhProperty in fullResults)
                {
                    results.Add(Estate.FromModel(uhProperty));
                }
                return Task.FromResult<List<Estate>>(results);
            }
        }

        public Task<List<Estate>> GetEstatesNotInList(IList<string> usedEstates)
        {
            using (var connection = GetOpenConnection(_connectionStringConfig.UHWReportingWarehouse))
            {
                var fullResults = connection.Query<UhProperty>(
                    "SELECT * FROM property WHERE prop_ref NOT IN @estateIds and level_code=2", new { EstateIds = usedEstates });

                var results = new List<Estate>();
                foreach (var uhProperty in fullResults)
                {
                    results.Add(Estate.FromModel(uhProperty));
                }
                return Task.FromResult(results);
            }
        }
    }
}
