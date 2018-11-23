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
    public class TraEstatesRepository : BaseRepository, ITraEstatesRepository
    {
        public TraEstatesRepository(IOptions<ConnStringConfiguration> connectionStringConfig) : base(connectionStringConfig)
        {
        }

        public IList<TraEstate> GetEstatesByTraId(int traId)
        {

            using (var connection = GetOpenConnection(_connectionStringConfig.ManageATenancyDatabase))
            {
                var fullResults = connection.Query<TraEstate>(
                    "SELECT * FROM TRAEstates WHERE TraId=@TraId", new { TraId = traId });

                return fullResults.ToList();
            }
        }
    }
}
