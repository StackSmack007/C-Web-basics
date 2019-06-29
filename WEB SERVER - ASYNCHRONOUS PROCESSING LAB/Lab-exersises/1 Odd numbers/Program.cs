using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace _1_Odd_numbers
{
    public class Program
    {
        static void Main()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            int min = 2;
            int max = 100000;
            Console.WriteLine(NumberOfPrimesInInterval(min, max));
            stopWatch.Stop();
            Console.WriteLine(stopWatch.ElapsedMilliseconds);
            //9592 ms
            //1800

            // Thread thread = new Thread(() => PrintEvenNumbers());
            // thread.Start();
            // thread.Join();
            // Console.WriteLine("Thread finished work!");
        }


        public static long NumberOfPrimesInInterval(int min, int max)
        {
            long result = min < 2 ? 1 : 0;
            object obj = new object();
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
            return result;
        }





        public static void PrintEvenNumbers()
        {
            int[] interval = Console.ReadLine().Split().Select(int.Parse).ToArray();
            for (int i = interval[0]; i <= interval[1]; i++)
            {
                if (i % 2 == 0)
                {
                    Console.WriteLine(i);
                }
            }
        }

    }
}
