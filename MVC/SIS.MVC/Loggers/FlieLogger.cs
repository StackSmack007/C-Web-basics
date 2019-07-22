namespace SIS.MVC.Loggers
{
    using SIS.MVC.Contracts;
    using System;
    using System.IO;
    public class FlieLogger : ILogger
    {
        private static object lockObj = new object();
        private string fileNamePath;
        public FlieLogger()
        {
            fileNamePath = "../../../log.txt";
        }

        public FlieLogger(string path)
        {
            fileNamePath = path;
        }

        public void Log(string message)
        {
            lock (lockObj)
            {
                File.WriteAllText(fileNamePath, $"FileLogger:{message}" + Environment.NewLine);
            }
        }
    }
}