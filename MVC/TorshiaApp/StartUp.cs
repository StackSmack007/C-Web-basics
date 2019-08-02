namespace TorshiaApp
{
    using SIS.MVC;
    public class StartUp
    {
        static void Main()
        {
            WebHost.Start(2001, new Configurator());
        }
    }
}