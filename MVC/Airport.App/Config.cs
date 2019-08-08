namespace Airport.App
{
    using SIS.MVC.Contracts;
    using SIS.MVC.Services;
    using SIS.WebServer.Routing;
    public class Config : IMvcApplication
    {
        public void ConfigureRouting(ServerRoutingTable serverRoutingTable)
        { }

        public void ConfigureServices(IServiceCollection servicecollection)
        { }

        public IConfiguration SetConfigurations()
        {
            IConfiguration cfg = new Configuration();
            cfg.DefaultAuthorisedRedirectAdress = "/Users/Login";
            cfg.DefaultLayoutName = "defaultLayout.html";
            cfg.LayoutsFolderPath = "/Views/Shared/";
            return cfg;
        }
    }
}