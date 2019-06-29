using SIS.HTTP.Exceptions;
using SIS.HTTP.Extensions;
using System;
using System.Linq;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            string test = "imalo edno vreme"+Environment.NewLine+" edin golqm bair";


            int[] num1 = { 1, 2, 3, 4 };
            int[] num2 = { 5, 6, 7, 8 };
            int[] num12 = num1.Concat(num2).ToArray();

            // InterpretateRequestString(test);
            // var ex = new BadRequestException();
            // var ex1 = new InternalServerErrorException();
            // Console.WriteLine("mustafARi".Capitalize());
            // Console.WriteLine(ex.Message);
            // Console.WriteLine(ex1.Message);



            ParseQueryParameters("http://mysite.com:8080/demo/index.php?id=27&lang=en#lectures");

        }


        private static void ParseQueryParameters(string Url)
        {
            int legth = Url.Length;
            int startIndex = Url.IndexOf("?")+1;
            if (startIndex < 0) return;
            int endIndex = Url.IndexOf('#');
            if (endIndex < 0)
            {
                endIndex = Url.Length - 1;
            }
            var Queries1 = Url.Substring(startIndex, endIndex- startIndex);

            string[] Queries = Queries1.Split('&');


        }


        private static void InterpretateRequestString(string str)
        {
            string[] lines = str.Split(Environment.NewLine);







        }

    }
}
