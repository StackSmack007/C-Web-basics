namespace Chushka.App
{
    using SIS.MVC.Contracts;
    using SIS.MVC.Services;
    using SIS.WebServer.Routing;
    internal class Config : IMvcApplication
    {
        public void ConfigureRouting(ServerRoutingTable serverRoutingTable)
        {

        }

        public void ConfigureServices(IServiceCollection servicecollection)
        {

        }

        public IConfiguration SetConfigurations()
        {
            IConfiguration cfg = new Configuration();

            cfg.DefaultAuthorisedRedirectAdress = "/Home/Index";
            cfg.DefaultLayoutName = "defLayout.html";
            cfg.LayoutsFolderPath = "/Views/Shared/";
            return cfg;
        }
    }
}