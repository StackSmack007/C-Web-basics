using System;

namespace Airport.App
{
    using SIS.MVC;
    public class StartUp
    {
        public static void Main()
        {
            WebHost webHost = new WebHost();
            WebHost.Start(2003, new Config());
        }
    }
}