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
    public class PracticeApi : ApiBase
    {
        public PracticeApi(string username, string password, string baseUrl) : base(username, password, baseUrl)
        {
        }

        public List<Practice> SearchForPracticesByAlternateId(string alternateId)
        {
            var client = new RestClient(BaseUrl);
            client.Authenticator = new HttpBasicAuthenticator(Username, Password);

            var request = new RestRequest(string.Format("api/admin/practices?alternate_id={0}", alternateId));
            var response = client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                var responseMessage = string.Format("The practice search API call failed for {0}.\r\n  Status: {1} {2}\r\n  Message: {3}",
                    alternateId, response.StatusCode, response.StatusDescription, response.Content);
                throw new Exception(responseMessage);
            }

            return JsonConvert.DeserializeObject<List<Practice>>(response.Content);
        }

        public Practice Get(string practiceId)
        {
            var client = new RestClient(BaseUrl);
            client.Authenticator = new HttpBasicAuthenticator(Username, Password);

            var request = new RestRequest(string.Format("api/admin/practices/{0}", practiceId));
            var response = client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                var responseMessage = string.Format("The practice lookup API call failed for {0}.\r\n  Status: {1} {2}\r\n  Message: {3}",
                    practiceId, response.StatusCode, response.StatusDescription, response.Content);
                throw new Exception(responseMessage);
            }
            
            var practice = JsonConvert.DeserializeObject<Practice>(response.Content);
            if (practice == null)
            {
                var responseMessage = string.Format("Failed to deserialize practice.\r\n  Content: {0}",
                    response.Content);
                throw new Exception(responseMessage);
            }

            return practice;
        }
    }
}
