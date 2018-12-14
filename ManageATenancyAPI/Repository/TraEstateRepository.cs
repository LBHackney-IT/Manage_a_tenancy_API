using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using ManageATenancyAPI.Configuration;
using ManageATenancyAPI.Models.Housing.NHO;
using ManageATenancyAPI.Models.UniversalHousing;
using Microsoft.Extensions.Options;

namespace ManageATenancyAPI.Repository
{
    public class TraEstateRepository : BaseRepository, ITraEstateRepository
    {
        public TraEstateRepository(IOptions<ConnStringConfiguration> connectionStringConfig) : base(connectionStringConfig)
        {
        }

        public IList<TraEstate> GetEstatesByTraId(int traId)
        {

            using (var connection = GetOpenConnection(_connectionStringConfig.ManageATenancyDatabase))
            {
                var fullResults = connection.Query<TraEstate>(
                    "SELECT * FROM TRAEstate WHERE TraId=@TraId", new { TraId = traId });

                return fullResults.ToList();
            }
        }

        public IList<string> GetAllUsedEstateRefs()
        {
            using (var connection = GetOpenConnection(_connectionStringConfig.ManageATenancyDatabase))
            {
                var fullResults = connection.Query<string>("SELECT EstateUHRef FROM TRAEstate");
                return fullResults.ToList();
            }
        }

        public void AddEstateToTra(int traId, string estateId, string estateName)
        {
            var traEstate = new TraEstate() { EstateName = estateName, EstateUHRef = estateId, TRAId = traId };
            using (var connection = GetOpenConnection(_connectionStringConfig.ManageATenancyDatabase))
            {
                var fullResults = connection.Execute("INSERT INTO TraEstate (TraId,EstateUHRef,EstateName) VALUES (@TraId,@EstateUHRef,@EstateName)", traEstate);
            }
        }

        public void RemoveEstateFromTra(int traId, string estateId)
        {
            using (var connection = GetOpenConnection(_connectionStringConfig.ManageATenancyDatabase))
            {
                var candidateToDelete = connection.QuerySingleOrDefault<TraEstate>(
                      "select * from TraEstate WHERE traid=@TraId and EstateUHRef=@EstateUHRef",
                      new { TraId = traId, EstateUHRef = estateId });

                if (candidateToDelete != null)
                {
                    connection.Delete(candidateToDelete);
                }
            }
        }
    }
}
