namespace Panda.App
{
using SIS.MVC;
    public  class StartUp
    {
        public static void Main()
        {
            WebHost webHost = new WebHost();
            WebHost.Start(2005, new Config());
        }
    }
}