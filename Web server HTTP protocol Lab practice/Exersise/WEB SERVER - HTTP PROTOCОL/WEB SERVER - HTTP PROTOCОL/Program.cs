using System;
using WEB_SERVER___HTTP_PROTOCОL.Tasks;

namespace WEB_SERVER___HTTP_PROTOCОL
{
 public   class Program
    {
        static void Main(string[] args)
        {
            string url = Console.ReadLine();
            //  string result = Methods.DecodeUrl(url);
            //  Console.WriteLine(result);


            //  string result = Methods.UrlParser(url);
            //  Console.WriteLine(result);

            Console.WriteLine(Methods.WebServer());

        }
    }
}
