namespace SIS.MVC.Services
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    public class Locator
    {
        static Locator()
        {
            CurrentLocation = Assembly.GetAssembly(typeof(Locator)).Location;
        }

        public static string CurrentLocation { get; set; }

        public static string GetPathOfFile(string destination, string filename)
        {
            return GetPathOfFolder(destination) + "\\" + filename;
        }

        public static string GetPathOfFolder(string destination)
        {
            destination = destination.TrimEnd('\\', '/');
            string[] foldersToBeContained = destination.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            string currentPath = Path.GetFullPath(Path.Combine(CurrentLocation, $"../"));

            while (!FoldersAreContained(currentPath, foldersToBeContained))
            {
                currentPath = Path.GetFullPath(Path.Combine(currentPath, $"../"));
            }
            currentPath = Path.GetFullPath(Path.Combine(currentPath, destination));
            return currentPath;
        }

        private static bool FoldersAreContained(string path, string[] foldersToBeContained)
        {
            bool result = foldersToBeContained.Any();
            foreach (string folder in foldersToBeContained)
            {
                path = Path.GetFullPath(Path.Combine(path, $"./{folder}"));
                if (!Directory.Exists(path))
                {
                    result = false;
                    break;
                }
            }
            return result;
        }
    }
}