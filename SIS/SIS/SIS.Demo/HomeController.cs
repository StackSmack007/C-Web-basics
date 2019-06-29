using SIS.HTTP.Responses.Contracts;
using SIS.WebServer.Results;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SIS.Demo
{
  public  class HomeController
    {
        public IHttpResponse Index()
        {
            string content = "<h1>Hello, world!</h1>";
            return new HtmlResult(content, HttpStatusCode.OK);
        }
               
    }
}
