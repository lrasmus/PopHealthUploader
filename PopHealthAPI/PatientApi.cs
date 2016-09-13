using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PopHealthAPI.Model;
using RestSharp;
using RestSharp.Authenticators;

namespace PopHealthAPI
{
    public class PatientApi : ApiBase
    {
        public PatientApi(string username, string password, string baseUrl) : base(username, password, baseUrl)
        {
        }

        public void UploadArchive(string file, Practice practice)
        {
            var client = new RestClient(BaseUrl) {Authenticator = new HttpBasicAuthenticator(Username, Password)};

            var request = new RestRequest("api/admin/patients.json", Method.POST);
            request.AddParameter("practice_id", practice.Id);
            request.AddParameter("practice_name", practice.Name);
            request.AddFile("file", file);

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
