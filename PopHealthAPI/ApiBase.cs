using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;

namespace PopHealthAPI
{
    public class ApiBase
    {
        protected string Username { get; set; }
        protected string Password { get; set; }
        protected string BaseUrl { get; set; }

        public ApiBase(string username, string password, string baseUrl)
        {
            Username = username;
            Password = password;
            BaseUrl = baseUrl;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }
    }
}
