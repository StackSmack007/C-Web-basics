namespace TorshiaApp
{
    using SIS.MVC.Contracts;
    using SIS.MVC.Services;
    using SIS.WebServer.Routing;
    class Configurator : IMvcApplication
    {
        public void ConfigureRouting(ServerRoutingTable serverRoutingTable)
        { }

        public void ConfigureServices(IServiceCollection servicecollection)
        { }

        public IConfiguration SetConfigurations()
        {
             return  new Configuration();
        }
    }
}