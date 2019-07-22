namespace Application
{
    using SIS.MVC.Contracts;
    using SIS.MVC.Loggers;
    using SIS.MVC.Services;
    using SIS.WebServer.Routing;
    public class StartUp : IMvcApplication
    {
        public void ConfigureRouting(ServerRoutingTable serverRoutingTable)  { }
      

        public void ConfigureServices(IServiceCollection servicecollection)
        {
            servicecollection.AddService<IEncrypter, Encrypter>();
            servicecollection.AddService<ILogger, ConsoleLogger>();
        }
    }
}