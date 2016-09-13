using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PopHealthAPI.Model;
using RestSharp;
using RestSharp.Authenticators;

namespace PopHealthAPI
{
    public class QueryApi : ApiBase
    {
        public QueryApi(string username, string password, string baseUrl) : base(username, password, baseUrl)
        {
        }

        public void Add(Query query)
        {
            var client = new RestClient(BaseUrl) { Authenticator = new HttpBasicAuthenticator(Username, Password) };

            var request = new RestRequest("api/admin/queries.json", Method.POST);
            //request.AddParameter("effective_date", query.EffectiveDate);
            //request.AddParameter("effective_start_date", query.EffectiveStartDate);
            //request.AddParameter("measure_id", query.MeasureId);
            //if (!string.IsNullOrWhiteSpace(query.SubId))
            //{
            //    request.AddParameter("sub_id", query.SubId);
            //}
            //request.AddParameter("providers", query.Providers);
            //foreach (var provider in query.Providers)
            //{
            //    request.AddParameter("providers", provider);
            //}
            //request.AddParameter("providers", JsonConvert.SerializeObject(query.Providers));
            //request.AddJsonBody(query);

            var json = JsonConvert.SerializeObject(query);
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            var response = client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                var responseMessage = string.Format("The API call failed.\r\n  Status: {0} {1}\r\n  Message: {2}",
                    response.StatusCode, response.StatusDescription, response.Content);
                throw new Exception(responseMessage);
            }
        }
    }
}
