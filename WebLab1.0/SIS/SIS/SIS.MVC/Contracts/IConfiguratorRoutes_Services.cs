namespace SIS.MVC.Contracts
{
    using SIS.WebServer.Routing;
    public interface IConfiguratorRoutes_Services
    {
        /// <summary>
        /// Routing must be given in cases one is not provided by HttpAttribute in controllers!
        /// Routes given here with same path and RequestMethod will be overwriten by Attributes!
        /// Routes with different paths can be more than one and they will overload! 
        /// </summary>
        /// <param name="serverRoutingTable"></param>
        void ConfigureRouting(ServerRoutingTable serverRoutingTable);

        void ConfigureServices();
    }
}
