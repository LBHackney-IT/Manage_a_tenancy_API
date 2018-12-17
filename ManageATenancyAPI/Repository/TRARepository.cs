using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LBH.Utils;
using ManageATenancyAPI.Configuration;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Models;
using Microsoft.Extensions.Options;

namespace ManageATenancyAPI.Repository
{
    public class TRARepository : ITRARepository
    {
        private readonly IDBAccessRepository _genericRepository;
        private readonly ConnStringConfiguration _configuration;
        private readonly AppConfiguration _appConfiguration;
        private ILoggerAdapter<TRARepository> _logger;
        private readonly string _strCrmConnString;

        public TRARepository(ILoggerAdapter<TRARepository> logger,IDBAccessRepository genericRepository, IOptions<ConnStringConfiguration> config, IOptions<AppConfiguration> appConfig)
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

        public List<TRA> BuildListOfTRAs(IDataReader dataReader)
        {
          
            var results = new List<TRA>();
            while (dataReader.Read())
            { 
                var item = new TRA
                {
                    TRAId =  (int) dataReader["TRAId"],
                    Name = Utils.NullToString(dataReader["Name"]),
                    AreaId = (int) dataReader["AreaId"]
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
            var item = new TRA();
            while (dataReader.Read())
            {
                item.TRAId = (int) dataReader["TRAId"];
                item.TRAEmail = dataReader["TRAEmail"].ToString();
                item.AreaId = (int) dataReader["AreaId"];
                item.Name = Utils.NullToString(dataReader["Name"]);
                result.PatchId = dataReader["PatchCRMId"].ToString();

                estates.Add(new TRAEstate()
                {
                    EstateName = dataReader["EstateName"].ToString(),
                    EstateUHReference = dataReader["EstateUHRef"].ToString()
                });

                roles.Add(new TRARolesAssignment()
                {
                  
                    RoleName = dataReader["RoleName"].ToString(),
                    PersonName = dataReader["PersonName"].ToString(),
                    RoleId = Utils.NullToInteger(dataReader["RoleId"])
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
                   role.RoleId,
                   role.PersonName
                } into grp
                select new TRARolesAssignment
                {
                    RoleName = grp.Key.RoleName,
                    RoleId = grp.Key.RoleId,
                    PersonName = grp.Key.PersonName
                }).ToList();

            result.TRA = item;
            result.ListOfEstates = estatesList;
            result.ListOfRoles = rolesList;
            
            return result;
        }
    }
}