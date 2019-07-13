namespace SIS.MVC
{
    using SIS.HTTP.Enums;
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses.Contracts;
    using SIS.MVC.Attributes;
    using SIS.MVC.Contracts;
    using SIS.WebServer;
    using SIS.WebServer.Routing;
    using System;
    using System.Linq;
    using System.Reflection;

    public class WebHost : IWebHost
    {
        private int port;
        private ServerRoutingTable serverRoutingTable;

        IConfiguratorRoutes_Services configurator;

        public WebHost(int port, IConfiguratorRoutes_Services configurator)
        {
            serverRoutingTable = new ServerRoutingTable();

            this.port = port;
            this.configurator = configurator;
        }

        public void Start()
        {
            ConfigureRoutingWithAttributes();
            configurator.ConfigureRouting(serverRoutingTable);
            configurator.ConfigureServices();
            Server server = new Server(port, serverRoutingTable);
            server.Run();
        }

        private void ConfigureRoutingWithAttributes()
        {
            Type[] controllerClasses = Assembly.GetEntryAssembly().GetTypes().Where(x => x.IsSubclassOf(typeof(Controller)) && !x.IsAbstract).ToArray();

            foreach (Type controller in controllerClasses)
            {
                MethodInfo[] actionMethods = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                                                       .Where(x => x.GetCustomAttributes<HttpAttribute>().Any()).ToArray();

                foreach (MethodInfo methodInfo in actionMethods)
                {
                    HttpAttribute[] attributes = methodInfo.GetCustomAttributes<HttpAttribute>().ToArray();

                    foreach (HttpAttribute attribute in attributes)
                    {
                        HttpRequestMethod methodType = attribute.MethodType;
                        string path = attribute.Path;
                        EnlistRoute(methodType, path, controller, methodInfo);

                    }
                }
            }
        }

        private void EnlistRoute(HttpRequestMethod methodType, string path, Type controllerType, MethodInfo methodInfo)
        {
            this.serverRoutingTable.Routes[methodType][path] = (IHttpRequest request) =>
             {
                 var controllerInstance = Activator.CreateInstance(controllerType);
                 PropertyInfo requestProperty = controllerType.GetProperty("Request");
                 requestProperty.SetValue(controllerInstance, request);

                 //TODO what happens if action method is not parameterless?
                 //More reflection to find out what are the parameters and reach to the container to make them...
                 return (IHttpResponse)methodInfo.Invoke(controllerInstance, new object[0]);
             };
        }
    }
}