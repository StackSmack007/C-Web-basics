using System;
using System.Linq;
using System.Threading;

namespace Lab_exersises
{
    public class Program
    {
        static void Main()
        {

            int[] interval = Console.ReadLine().Split().Select(int.Parse).ToArray();

            Thread thread = new Thread(() => PrintEvenNumbers(interval[0], interval[1]));

            thread.Start();
            //  thread.Join();
            Console.WriteLine("Thread finished work!");

        }


        public static void PrintEvenNumbers(int min, int max)
        {
            Thread.Sleep(10000);
            for (int i = min; i <= max; i++)
            {
                if (i % 2 == 0)
                {
                    Console.WriteLine(i);
                }
            }
        }

    }
}
