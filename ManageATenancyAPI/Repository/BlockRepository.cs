using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using ManageATenancyAPI.Configuration;
using ManageATenancyAPI.Models.Housing.NHO;
using ManageATenancyAPI.Models.UniversalHousing;
using Microsoft.Extensions.Options;

namespace ManageATenancyAPI.Repository
{
    public class BlockRepository : BaseRepository, IBlockRepository
    {
        public BlockRepository(IOptions<ConnStringConfiguration> connectionStringConfig) : base(connectionStringConfig)
        {
        }

        
        public async Task<IEnumerable<Block>> GetBlocksByEstateId(string estateId)
        {
            using (var connection = GetOpenConnection(_connectionStringConfig.UHWReportingWarehouse))
            {
                var fullResults = connection.Query<UhProperty>(
                    "SELECT * FROM property WHERE major_ref=@EstateId and level_code=3", new { EstateId = estateId });


                var results = new List<Block>();
                foreach (var uhProperty in fullResults)
                {
                    results.Add(Block.FromModel(uhProperty));
                }
                return results;
            }
        }
    }
}
