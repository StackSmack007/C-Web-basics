namespace MishMashApplication
{//17:00
    using SIS.MVC;
    public class StartUp
    {
        static void Main()
        {
            WebHost.Start(1500, new Configurator());
        }
    }
}