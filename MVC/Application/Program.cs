namespace Application
{
    using SIS.MVC;
    using SIS.MVC.Contracts;

    public class Program
    {
        public static void Main()
        {
            IMvcApplication configurator = new StartUp();
            WebHost.Start(2000, configurator);
        }
    }
}