namespace Application
{
    using SIS.MVC;
    using SIS.MVC.Contracts;
    using System.Collections.Generic;

    public class Program
    {
        public static void Main()
        {

            IMvcApplication configurator = new StartUp();
            WebHost.Start(2000, configurator);
        }
    }
}