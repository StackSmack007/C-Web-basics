using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncExperiment
{
    public class Program
    {
        static void Main()
        {
            NumberOfPrimesInInterval(0, 100000);
            while (true)
            {
                Console.ReadLine();
            }
        }





        public async static void NumberOfPrimesInInterval(int min, int max)
        {
            long result = min < 2 ? 1 : 0;
            object obj = new object();
            await Task.Run(() =>
{
    Parallel.For(min, max, i =>
     {
         bool isPrime = true;
         for (int j = 2; j < i; j++)
         {
             if (i % j == 0)
             {
                 isPrime = false;
                 break;
             }
         }
         if (isPrime)
         {
             lock (obj)
             {
                 result++;
             }
         }
     });
});
            Console.WriteLine(result);
            // return result;
        }
    }
}
