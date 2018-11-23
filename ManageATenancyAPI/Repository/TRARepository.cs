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

    }
}
