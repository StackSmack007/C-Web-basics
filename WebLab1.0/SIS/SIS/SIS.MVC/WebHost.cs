namespace SIS.MVC
{
    using SIS.MVC.Contracts;
    using SIS.WebServer;
    using SIS.WebServer.Routing;
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
            configurator.ConfigureRouting(serverRoutingTable);
            configurator.ConfigureServices();
            Server server = new Server(port, serverRoutingTable);
            server.Run();
        }
    }
}