namespace SIS.MVC.Contracts
{
    using SIS.WebServer.Routing;
    public interface IConfiguratorRoutes_Services
    {

        void ConfigureRouting(ServerRoutingTable serverRoutingTable);


        void ConfigureServices();

    }
}
