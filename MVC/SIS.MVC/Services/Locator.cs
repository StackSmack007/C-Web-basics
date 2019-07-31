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
            return GetPathOfFolder(destination) + "/" + filename;
        }

        public static string GetPathOfFile(string subPath)
        {
            subPath = subPath.Replace("\\", "/");
            int indexOfLastLine = subPath.LastIndexOf("/");
            if (indexOfLastLine==-1)
            {
                return GetPathOfFile("/", subPath);
            }
            string path = subPath.Substring(0, indexOfLastLine + 1);
            string fileName = subPath.Replace(path, "");
            return GetPathOfFile(path, fileName);
        }

        public static string GetPathOfFolder(string destination)
        {
            destination = destination.Trim('\\', '/');
            string[] foldersToBeContained = destination.Split('/', StringSplitOptions.RemoveEmptyEntries);
            string currentPath = Path.GetFullPath(Path.Combine(CurrentLocation, $"../"));

            while (!FoldersAreContained(currentPath, foldersToBeContained))
            {
                currentPath = Path.GetFullPath(Path.Combine(currentPath, $"../"));
            }
            return Path.Combine(currentPath, destination);
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