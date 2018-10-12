using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Models;
using System.Text;
using System.Configuration;
using System.Data;
using System.Collections.Generic;
using LBH.Utils;
using System;
using System.Data.SqlClient;
using ManageATenancyAPI.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;

namespace ManageATenancyAPI.Repository
{
    public class CitizenIndexRepository : ICitizenIndexRepository
    {
        private readonly IDBAccessRepository _genericRepository;
        private readonly ConnStringConfiguration _configuration;
        private readonly AppConfiguration _appConfiguration;
        private readonly string _strCrmConnString;

        public CitizenIndexRepository(IDBAccessRepository genericRepository, IOptions<ConnStringConfiguration> config, IOptions<AppConfiguration> appConfig)
        {
            _genericRepository = genericRepository;
            _configuration = config?.Value;
            _appConfiguration = appConfig?.Value;
            _strCrmConnString = _configuration.CIDatabase;

        }
        public List<CIPerson> SearchCitizenIndex(string firstname, string surname, string addressSearch, string postcode)
        {
            SqlParameter[] sqlParams = new SqlParameter[5];

            sqlParams[0] = new SqlParameter("@FirstName", SqlDbType.VarChar, 200) {Value = firstname == string.Empty ? null : firstname };
            sqlParams[1] = new SqlParameter("@Surname", SqlDbType.VarChar, 50) {Value = surname == string.Empty ? null : surname };
            sqlParams[2] = new SqlParameter("@Postcode", SqlDbType.VarChar, 20) {Value = postcode == string.Empty ? null : postcode };
            sqlParams[3] = new SqlParameter("@AddressString", SqlDbType.VarChar, 100) {Value = addressSearch == string.Empty ? null : addressSearch };
            sqlParams[4] = new SqlParameter("@ResultsCount", SqlDbType.SmallInt) { Value = int.Parse(_appConfiguration.MaxCISearchResults) };

            return BuildContactPerson(_genericRepository.ExecuteReader(_strCrmConnString, CommandType.StoredProcedure, "usp_SearchCitizenIndex", sqlParams));
        }

        public List<CIPerson> BuildContactPerson(IDataReader dataReader)
        {
            var results = new List<CIPerson>();
            while (dataReader.Read())
            {
                var sbAddress = new StringBuilder();

                if (Utils.NullToString(dataReader["ADDR_SUBBNAME"]) != "")
                {
                    sbAddress.Append(Utils.NullToString(dataReader["ADDR_SUBBNAME"]) + " ");
                }

                if (Utils.NullToString(dataReader["ADDR_BNUM"]) != "")
                {
                    sbAddress.Append(Utils.NullToString(dataReader["ADDR_BNUM"]) + " ");
                }
                if (Utils.NullToString(dataReader["ADDR_BUILDING"]) != "")
                {
                    sbAddress.Append(Utils.NullToString(dataReader["ADDR_BUILDING"]));
                }

                var addressline1 = sbAddress.ToString().TrimEnd();

                if (Utils.NullToString(dataReader["ADDR_BUILDING"]) != "" &&
                    Utils.NullToString(dataReader["ADDR_STREET"]) != "")
                {
                    sbAddress.Append(" " + Utils.NullToString(dataReader["ADDR_STREET"]) + " ");
                }
                else
                {
                    sbAddress.Append(Utils.NullToString(dataReader["ADDR_STREET"]) + " ");
                }

                if (Utils.NullToString(dataReader["ADDR_LOCALITY"]) != "")
                {
                    sbAddress.Append(Utils.NullToString(dataReader["ADDR_LOCALITY"]) + " ");
                }

                if (Utils.NullToString(dataReader["ADDR_TOWN"]) != "")
                {
                    sbAddress.Append(Utils.NullToString(dataReader["ADDR_TOWN"]) + " ");
                }

                if (Utils.NullToString(dataReader["ADDR_COUNTY"]) != "")
                {
                    sbAddress.Append(Utils.NullToString(dataReader["ADDR_COUNTY"]) + " ");
                }
                if (Utils.NullToString(dataReader["ADDR_POSTCODE"]) != "")
                {
                    sbAddress.Append(Utils.NullToString(dataReader["ADDR_POSTCODE"]));
                }
                var dateofbirth = string.Empty;
                if (Utils.NullToString(dataReader["DOB_VALUE"]) != "")
                {
                    dateofbirth = Convert.ToDateTime(dataReader["DOB_VALUE"]).ToString($"yyyy-MM-dd").Trim();
                }
                var addresSearch = new StringBuilder();

                if (sbAddress.Length > 0)
                {
                    var straraddressSearch = sbAddress.ToString().Split(' ');

                    foreach (var searchciteria in straraddressSearch)
                    {
                        if (!string.IsNullOrEmpty(searchciteria) && !string.IsNullOrEmpty(searchciteria))
                            ;               {
                            addresSearch = addresSearch.Append(searchciteria);
                        }

                    }
                }

                var item = new CIPerson
                {
                    HackneyhomesId = Utils.NullToString(dataReader["HACKNEYHOMES"]),
                    Title = Utils.NullToString(dataReader["NAME_TITLE"]),
                    FirstName = Utils.NullToString(dataReader["NAME_CUSTOM_1"]),
                    Surname = Utils.NullToString(dataReader["NAME_SURNAME"]),
                    DateOfBirth = dateofbirth,
                    FullAddressDisplay = sbAddress.ToString(),
                    FullAddressSearch = addresSearch.ToString(),
                    Address = sbAddress.ToString(),
                    AddressLine1 = addressline1,
                    AddressLine2 = Utils.NullToString(dataReader["ADDR_STREET"]),
                    AddressLine3 = Utils.NullToString(dataReader["ADDR_LOCALITY"]),
                    AddressCity = Utils.NullToString(dataReader["ADDR_TOWN"]),
                    AddressCountry = Utils.NullToString(dataReader["ADDR_COUNTY"]),
                    PostCode = Utils.NullToString(dataReader["ADDR_POSTCODE"]),
                    SystemName = "CitizenIndex",
                    USN = Utils.NullToString(dataReader["USN"]),
                    UPRN = Utils.NullToString(dataReader["UPRN"]),
                    LARN = Utils.NullToString(dataReader["LARN"]),
                    IsActiveTenant = false,
                    HouseholdId = null,
                    Accounttype = null
                };

                if (!string.IsNullOrEmpty(Utils.NullToString(dataReader["CRM"])))
                {
                    if (Guid.TryParse(dataReader["CRM"].ToString(), out var crmContactId))
                    {
                        item.CrmContactId = crmContactId;
                    }

                }

                results.Add(item);
            }
            return results;
        }
        
    }
}
