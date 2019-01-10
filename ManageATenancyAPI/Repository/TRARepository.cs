using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using LBH.Utils;
using ManageATenancyAPI.Configuration;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Models;
using ManageATenancyAPI.Models.UniversalHousing;
using Microsoft.Extensions.Options;

namespace ManageATenancyAPI.Repository
{
    public class TRARepository : BaseRepository, ITRARepository
    {
        private readonly IDBAccessRepository _genericRepository;
        private readonly ConnStringConfiguration _configuration;
        private readonly AppConfiguration _appConfiguration;
        private ILoggerAdapter<TRARepository> _logger;
        private readonly string _strCrmConnString;

        public TRARepository(ILoggerAdapter<TRARepository> logger, IDBAccessRepository genericRepository, IOptions<ConnStringConfiguration> config, IOptions<AppConfiguration> appConfig) : base(config)
        {
            _genericRepository = genericRepository;
            _configuration = config?.Value;
            _appConfiguration = appConfig?.Value;
            _strCrmConnString = _configuration.ManageATenancyDatabase;
            _logger = logger;
        }
        public List<TRA> FindTRAsForPatch(string patchId)
        {
            SqlParameter[] sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter("@PatchCRMId", SqlDbType.VarChar, 200) { Value = patchId == string.Empty ? null : patchId };
            try
            {
                List<TRA> result = BuildListOfTRAs(_genericRepository.ExecuteReader(_strCrmConnString, CommandType.StoredProcedure, "get_tra_for_patch", sqlParams));
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("An error has occurred while retriving data for FindTRAsForPatch method: " + ex.InnerException);
                throw ex;
            }
        }

        public TRAInformation FindTRAInformation(int TRAId)
        {
            SqlParameter[] sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter("@TRAId", SqlDbType.VarChar, 200) { Value = TRAId == 0 ? 0 : TRAId };
            try
            {
                TRAInformation result = BuildTRAInformation(_genericRepository.ExecuteReader(_strCrmConnString, CommandType.StoredProcedure, "get_tra_information", sqlParams));
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("An error has occurred while retriving data for FindTRAsForPatch method: " + ex.InnerException);
                throw ex;
            }
        }

        public Task<bool> Exists(string traName)
        {
            using (var connection = GetOpenConnection(_connectionStringConfig.ManageATenancyDatabase))
            {
                var result = connection.ExecuteScalar<int>(
                    "SELECT COUNT(*) FROM TRA WHERE NAME=@Name",
                    new { Name = traName });
                return Task.FromResult(result > 0);
            }
        }

        public void UpdateNotes(int traId, string notes)
        {
            using (var connection = GetOpenConnection(_connectionStringConfig.ManageATenancyDatabase))
            {
                connection.ExecuteScalar<int>(
                    "Update TRA SET Notes=@Notes WHERE TraId=@traId", new { Notes = notes, TraId = traId });

            }
        }
        public void UpdateEmail(int traId, string email)
        {
            using (var connection = GetOpenConnection(_connectionStringConfig.ManageATenancyDatabase))
            {
                connection.ExecuteScalar<int>(
                    "Update TRA SET Email=@Email WHERE TraId=@traId", new { Email = email, TraId = traId });

            }
        }


        public Task<TRA> Create(string name, string notes,string email, int areaId, Guid patchId)
        {

            var tra = new TRA() { Name = name,Notes=notes, AreaId = areaId,PatchId= patchId, Email = email };
            using (var connection = GetOpenConnection(_connectionStringConfig.ManageATenancyDatabase))
            {
                var traId = connection.Insert(tra);
                return Task.FromResult(connection.Get<TRA>(traId));
            }
        }

        public List<TRA> BuildListOfTRAs(IDataReader dataReader)
        {
          
            var results = new List<TRA>();
            while (dataReader.Read())
            {
                var item = new TRA
                {
                    TRAId = (int)dataReader["TRAId"],
                    Name = Utils.NullToString(dataReader["Name"]),
                    AreaId = (int)dataReader["AreaId"]
                };
                results.Add(item);
            }
            return results;
        }

        public TRAInformation BuildTRAInformation(IDataReader dataReader)
        {
            var estates = new List<TRAEstate>();
            var roles = new List<TRARolesAssignment>();
            var result = new TRAInformation();
            while (dataReader.Read())
            {
                result.TRAId = (int) dataReader["TRAId"];
                result.Email = dataReader["Email"].ToString();
                result.AreaId = (int) dataReader["AreaId"];
                result.Name = Utils.NullToString(dataReader["Name"]);
                result.PatchId = dataReader["PatchCRMId"].ToString();
                result.Notes = dataReader["Notes"].ToString();

                estates.Add(new TRAEstate()
                {
                    TRAId = (int)dataReader["TRAId"],
                    EstateName = dataReader["EstateName"].ToString(),
                    EstateUHReference = dataReader["EstateUHRef"].ToString()
                });

                roles.Add(new TRARolesAssignment()
                {
                    PersonName = dataReader["PersonName"].ToString(),
                    Role = dataReader["Role"].ToString()
                });
            }

            List<TRAEstate> estatesList = (from estate in estates
                                group estate by new
                                {
                                    estate.EstateName,
                                    estate.EstateUHReference
                                } into grp
                                select new TRAEstate
                                {
                                    EstateName = grp.Key.EstateName,
                                   EstateUHReference = grp.Key.EstateUHReference
                                }).ToList();

            List<TRARolesAssignment> rolesList = (from role in roles
                group role by new
                {
                   role.RoleName,
                   role.Role,
                   role.PersonName
                } into grp
                select new TRARolesAssignment
                {
                    RoleName = grp.Key.RoleName,
                    Role = grp.Key.Role,
                    PersonName = grp.Key.PersonName
                }).ToList();

            result.ListOfEstates = estatesList;
            result.ListOfRoles = rolesList;
            
            return result;
        }
    }
}