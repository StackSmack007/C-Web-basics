namespace SIS.MVC
{
    using SIS.MVC.Contracts;
    using SIS.MVC.Loggers;
    using SIS.MVC.Routing;
    using SIS.MVC.Services;
    using SIS.MVC.ViewEngine;
    using SIS.WebServer;
    using SIS.WebServer.Routing;
    using System.IO;
    using System.Linq;

    /// <summary>
    ///  Every DTO must have constructor (Non empty!) accepting parameters exact number,
    /// <para> types and naming (Case Sensitive) as the dada provided by Views!!! </para>
    /// Overloading of constructors in DTOs is an option.
    /// <para> Nesting of complex types is an option but there must be unique names for every parameter provided by request! </para>
    /// </summary>
    public class WebHost
    {
        private static ServerRoutingTable serverRoutingTable;

        private static string tempLocation = @"SIS.MVC/ViewEngine/GeneratedModels";
        private static IServiceCollection serviceContainer;

        private static ILogger logger;

        static WebHost()
        {
            serverRoutingTable = new ServerRoutingTable();
            serviceContainer = new ServiceCollection();
            serviceContainer.AddService<ILogger, ConsoleLogger>();
            serviceContainer.AddService<IEncrypter, Encrypter>();
            logger = new ConsoleLogger();
            RouterEngine.Logger = logger;
            ClearTempFolder(tempLocation);
        }

        public static IConfiguration Configurations { get; private set; }
        public static IServiceCollection ServiceContainer => serviceContainer;

        private static void ClearTempFolder(string subPath)
        {
            string destinationFolderPath = Locator.GetPathOfFolder(subPath);

            if (Directory.Exists(destinationFolderPath))
            {
                var files = Directory.EnumerateFiles(destinationFolderPath).ToHashSet();
                foreach (var filePath in files)
                {
                    File.Delete(filePath);
                }
            }
        }
        
        public static void Start(int port, IMvcApplication application)
        {
            Configurations = application.SetConfigurations();
            RouterEngine.ConfigureRoutingFromAttributes(serverRoutingTable);
            RouterEngine.ConfigureRoutesByNamesConvention(serverRoutingTable);
            RouterEngine.RegisterStaticFiles(serverRoutingTable);
            application.ConfigureRouting(serverRoutingTable);
            application.ConfigureServices(serviceContainer);
            serviceContainer.AddService<IViewEngine, View_Engine>();
            Server server = new Server(port, serverRoutingTable);
            server.Run();
        }
    }
}