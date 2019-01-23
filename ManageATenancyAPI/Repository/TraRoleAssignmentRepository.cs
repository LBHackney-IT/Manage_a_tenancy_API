using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using ManageATenancyAPI.Configuration;
using ManageATenancyAPI.Models.Housing.NHO;
using Microsoft.Extensions.Options;

namespace ManageATenancyAPI.Repository
{
    public class TraRoleAssignmentRepository : BaseRepository, ITraRoleAssignmentRepository
    {
        public TraRoleAssignmentRepository(IOptions<ConnStringConfiguration> connectionStringConfig) : base(connectionStringConfig)
        {
        }

        public async Task AddRoleAssignment(int traId, string role, string personName)
        {
            using (var connection = GetOpenConnection(_connectionStringConfig.ManageATenancyDatabase))
            {
                var res =
                     connection.Execute(
                         "INSERT INTO TRARoleAssignment (TRAId,Role,PersonName) VALUES(@TraId,@Role,@PersonName)",
                         new { TRAId = traId, Role = role, PersonName = personName });
            }
        }

        public async Task RemoveRoleAssignment(int traId, string role)
        {
            using (var connection = GetOpenConnection(_connectionStringConfig.ManageATenancyDatabase))
            {
                var candidateToDelete = connection.Execute(
                    "DELETE FROM TRARoleAssignment  WHERE traid=@TraId and Role=@role",
                    new { TraId = traId, Role = role });
            }
        }

        public async Task<List<RoleAssignment>> GetRoleAssignmentForTra(int tra)
        {
            using (var connection = GetOpenConnection(_connectionStringConfig.UHWReportingWarehouse))
            {
                var result = connection.Query<RoleAssignment>(
                    "SELECT * FROM TRARoleAssignment  WHERE TraId=@traId", new { TraId = tra });
                return result.ToList();
            }
        }
    }
}
