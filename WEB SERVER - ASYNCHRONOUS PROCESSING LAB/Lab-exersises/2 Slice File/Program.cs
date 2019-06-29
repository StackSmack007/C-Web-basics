namespace _2_Slice_File
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

   public class Program
    {
        static void Main(string[] args)
        {
            string sourse = "./Sourse/Dark.Matter.S01E10.HDTV.x264-KILLERS.mp4";
            string destination = "./Destination/movie.mp4";
            byte parts = 10;
            Task<Queue<string>> sliceTask = new Task<Queue<string>>(() => SliceFile(sourse, destination, parts));
            sliceTask.Start();
            sliceTask.ContinueWith((t) => JoinFile(t.Result, "./Destination/wholeEpisode.mp4"))
                      .ContinueWith((task) => Console.WriteLine("Work Done!"));

            while (true)
            {
                string action = Console.ReadLine();
                if (action.ToLower() == "exit")
                {
                   Environment.Exit(0);
                }
                Console.WriteLine($"You typed {action}");
            }

        }
        public static Queue<string> SliceFile(string sourse, string destination, byte parts)
        {
            Queue<string> partsPaths = new Queue<string>();
            using (FileStream fsr = new FileStream(sourse, FileMode.Open))
            {
                byte[] buffer = new byte[1024 * 4];
                ulong partLength = (ulong)(fsr.Length / parts);
                ulong progress = 0;
                for (byte i = 1; i <= parts; i++)
                {
                    int indexOflastDot = destination.LastIndexOf('.');
                    string destinationFile = destination.Insert(indexOflastDot, $"_part_{i}");
                    partsPaths.Enqueue(destinationFile);
                    Thread.Sleep(1200);
                    using (FileStream fsw = new FileStream(destinationFile, FileMode.Create))
                    {
                        while (progress < partLength * i)
                        {
                            int readBytes = fsr.Read(buffer, 0, buffer.Length);
                            progress += (ulong)readBytes;
                            fsw.Write(buffer, 0, readBytes);
                        }
                    }
                }
            }
            Console.WriteLine("Slice Complete!");
            return partsPaths;
        }

        public static void JoinFile(Queue<string> sourses, string destination)
        {

            using (FileStream fsw = new FileStream(destination, FileMode.Create))
            {
                byte[] buffer = new byte[1024];
                foreach (string soursePath in sourses)
                {
                    using (FileStream fsr = new FileStream(soursePath, FileMode.Open))
                    {
                        int readBytes = fsr.Read(buffer, 0, buffer.Length);
                        while (readBytes != 0)
                        {
                            fsw.Write(buffer, 0, readBytes);
                            readBytes = fsr.Read(buffer, 0, buffer.Length);
                        }
                    }
                }
            }
            Console.WriteLine("Merge Complete!");
        }
    }
}