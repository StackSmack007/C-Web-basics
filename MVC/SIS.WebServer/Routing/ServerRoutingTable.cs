using SIS.HTTP.Enums;
using SIS.HTTP.Requests.Contracts;
using SIS.HTTP.Responses.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SIS.WebServer.Routing
{
    public class ServerRoutingTable
    {
        private Dictionary<HttpRequestMethod, Dictionary<string, Func<IHttpRequest, IHttpResponse>>> routes;

        public ServerRoutingTable()
        {
            this.routes = new Dictionary<HttpRequestMethod, Dictionary<string, Func<IHttpRequest, IHttpResponse>>>
            {
                [HttpRequestMethod.Get] = new Dictionary<string, Func<IHttpRequest, IHttpResponse>>(),
                [HttpRequestMethod.Post] = new Dictionary<string, Func<IHttpRequest, IHttpResponse>>(),
                [HttpRequestMethod.Put] = new Dictionary<string, Func<IHttpRequest, IHttpResponse>>(),
                [HttpRequestMethod.Delete] = new Dictionary<string, Func<IHttpRequest, IHttpResponse>>(),
            };
        }

        public bool ContainsRoute(HttpRequestMethod method,string path)
        {
            return routes.ContainsKey(method) && routes[method].Keys.Any(x => x.ToLower() == path.ToLower());
        }

        public void RegisterRoute(HttpRequestMethod method, string path, Func<IHttpRequest, IHttpResponse> func)
        {
            routes[method][path] = func;
            Console.WriteLine("registered route at "+ path);
        }

        public Func<IHttpRequest, IHttpResponse> GetFunc(HttpRequestMethod method, string path)
        {
            if (ContainsRoute(method,path))
            {
                return routes[method].FirstOrDefault(x=>x.Key.ToLower()==path.ToLower()).Value;
            }
            return null;
        }
    }
}