using SIS.HTTP.Requests.Contracts;
using SIS.HTTP.Responses;
using SIS.HTTP.Responses.Contracts;
using SIS.WebServer.Api.Contracts;
using SIS.WebServer.Results;
using SIS.WebServer.Routing;
namespace SIS.WebServer.Api
{
    using System.IO;
    using System.Net;
    public class HttpHandler : IHttpHandler
    {
        private ServerRoutingTable serverRoutingTable;

        public HttpHandler(ServerRoutingTable serverRoutingTable)
        {
            this.serverRoutingTable = serverRoutingTable;
        }

        public IHttpResponse Handle(IHttpRequest httpRequest)
        {
              if (!this.serverRoutingTable.ContainsRoute(httpRequest.RequestMethod,httpRequest.Path))
                {
                    return ReturnIfResource(httpRequest.Path);

                }
            return this.serverRoutingTable.GetFunc(httpRequest.RequestMethod, httpRequest.Path).Invoke(httpRequest);
        }

        private IHttpResponse ReturnIfResource(string path)
        {   
            path = "../../../.." + path;
            if (File.Exists(path))
            {
                byte[] content = File.ReadAllBytes(path);
                var response = new InlineResourseResult(content, HttpStatusCode.OK);
                return response;
            }
            return new HttpResponse(HttpStatusCode.NotFound);
        }
    }
}