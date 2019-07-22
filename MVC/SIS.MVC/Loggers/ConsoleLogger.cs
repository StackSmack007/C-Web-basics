namespace SIS.MVC.Loggers
{
    using SIS.MVC.Contracts;
    using System;
    public class ConsoleLogger : ILogger
    {
        public ConsoleLogger() { }

        public ConsoleLogger(string initialCreatingMessage)
        {
            Console.WriteLine($"Display this: {initialCreatingMessage}");
        }

        public void Log(string message)
        {
            Console.WriteLine($"ConsoleLogger: {message}");
        }
    }
}