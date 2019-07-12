namespace Application
{
    using Application.Configure;
    using SIS.MVC;
    using SIS.MVC.Contracts;

    public class StartUp
    {
        public static void Main()
        {

            IConfiguratorRoutes_Services configurator = new ConfigureRoutesServices();
            IWebHost webhost = new WebHost(2000, configurator);
            webhost.Start();
        }
    }
}