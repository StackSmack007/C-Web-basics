namespace MishMashApplication
{
    using SIS.MVC.Contracts;
    using SIS.MVC.Loggers;
    using SIS.WebServer.Routing;
    public class Configurator : IMvcApplication
    {
        public void ConfigureRouting(ServerRoutingTable serverRoutingTable)
        { }

        public void ConfigureServices(IServiceCollection servicecollection)
        {
            servicecollection.AddService<ILogger, ConsoleLogger>();
        }
    }
}