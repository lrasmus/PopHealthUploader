using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;

namespace PopHealthAPI
{
    public class Patient : ApiBase
    {
        public Patient(string username, string password, string baseUrl) : base(username, password, baseUrl)
        {
        }

        public void UploadArchive(string file)
        {
            var client = new RestClient(BaseUrl);
            client.Authenticator = new HttpBasicAuthenticator(Username, Password);

            var request = new RestRequest("api/admin/patients.json", Method.POST);
            request.AddFile("file", file);

            try
            {
                var response = client.Execute(request);
                if (response.ResponseStatus != ResponseStatus.Completed)
                {
                    var responseMessage = string.Format("The API call failed.\r\n  Status: {0}\r\n  Message: {1}",
                        response.ResponseStatus, response.Content);
                    throw new Exception(responseMessage);
                }
            }
            catch (Exception exc)
            {
                throw new Exception("Exception when executing API request", exc);
            }
            
        }

        // Using this requires you to add the following to api/patients_controller.rb.  
        //     skip_before_action :verify_authenticity_token, :only => [:create]
        //
        // This is probably a bad idea, since it exposes the normal user API (sans CSRF 
        // protection).  Instead we're going to require files be zipped up.
        //public void UploadFile(string file)
        //{
        //    var client = new RestClient(BaseUrl);
        //    client.Authenticator = new HttpBasicAuthenticator(Username, Password);

        //    var request = new RestRequest("api/patients.xml", Method.POST);
        //    request.AddFile("file", file);

        //    var response = client.Execute(request);
        //}
    }
}
