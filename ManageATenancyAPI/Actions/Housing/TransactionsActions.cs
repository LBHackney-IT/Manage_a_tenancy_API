using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ManageATenancyAPI.Helpers.Housing;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Models;
using ManageATenancyAPI.Models.Housing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ManageATenancyAPI.Actions.Housing
{
    public class TransactionsActions
    {

        private HttpClient _client;
       
        private readonly IHackneyHousingAPICallBuilder _apiCallBuilder;
        private readonly ILoggerAdapter<TransactionsActions> _loggerAdapter;
        private IHackneyHousingAPICall _hackneyLeaseAccountTransactionsApi;
        private IHackneyGetCRM365Token _accessToken;
        private IHackneyHousingAPICallBuilder _hackneyLeaseAccountApiBuilder;
       
        
       public TransactionsActions(ILoggerAdapter<TransactionsActions> logger, IHackneyHousingAPICallBuilder apiCallBuilder,
            IHackneyHousingAPICall apiCall,IHackneyGetCRM365Token accessToken)
        {

            _client = new HttpClient();
            _accessToken = accessToken;
            _hackneyLeaseAccountApiBuilder = apiCallBuilder;
            _hackneyLeaseAccountTransactionsApi = apiCall;
            _loggerAdapter = logger;

        }

        public async Task<object> GetTransactionsByTagReference(string tagReference)
        {

            HttpResponseMessage result = null;
            try
            {

                _loggerAdapter.LogInformation($"Getting Transactions with tagReference {tagReference}  ");


                var accessToken = _accessToken.getCRM365AccessToken().Result;

                _client =  _hackneyLeaseAccountApiBuilder.CreateRequest(accessToken).Result;

                string query =
                    HousingAPIQueryBuilder.getTransactionsByTagReference(tagReference);

                result = _hackneyLeaseAccountTransactionsApi.getHousingAPIResponse(_client, query, tagReference).Result;
                if (result != null)
                {
                    if (!result.IsSuccessStatusCode)
                    {
                        throw new TransactionsServiceException();
                    }
                    JObject jRetrieveResponse = JsonConvert.DeserializeObject<JObject>(result.Content.ReadAsStringAsync().Result);
                    if (jRetrieveResponse != null)
                    {

                        dynamic transactionsResponse = jRetrieveResponse["value"];
                        var transactionsList = new List<Transactions>();
                        foreach (var transaction in transactionsResponse)
                        {
                            transactionsList.Add(buildTransactions(transaction));
                        }

                        return new
                        {
                            results = transactionsList
                        };
                    }
                    return null;
                }
                else
                {
                    _loggerAdapter.LogError($" Data missing for transactions with tagReference  {tagReference} ");
                    throw new TransactionsMissingResultsException();
                }
            }
            catch (Exception ex)
            {
                if (result == null)
                {
                    _loggerAdapter.LogError($" Transactions details missing for tagReference {tagReference} ");
                    throw new TransactionsMissingResultsException();

                }
                else
                {
                    _loggerAdapter.LogError($" Service error getting Transactions details for {tagReference}");
                    throw new TransactionsServiceException();
                }
            }

        }

        private object buildTransactions(dynamic transactionsResponse)
        {
            return new Transactions()
            {

                tagReference = transactionsResponse.housing_tag_ref.ToString().Trim(),
                propertyReference = transactionsResponse.housing_prop_ref.ToString().Trim(),
                transactionSid = transactionsResponse.housing_rtrans_sid,
                houseReference = transactionsResponse.housing_house_ref.ToString().Trim(),
                transactionType = transactionsResponse.housing_trans_type.ToString().Trim(),
                postDate = transactionsResponse.housing_postdate,
                realValue = transactionsResponse.housing_real_value,
                transactionID = transactionsResponse.housing_rtransid.ToString().Trim(),
                debDesc = transactionsResponse.housing_deb_desc.ToString().Trim()

            };
        }
    }
    public class TransactionsServiceException : System.Exception { }
    public class TransactionsMissingResultsException : System.Exception { }
}
