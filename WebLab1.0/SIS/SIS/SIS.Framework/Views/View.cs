namespace SIS.Framework.Views
{
    using SIS.Framework.ActionResults.Contracts;
    using System;
    using System.IO;
    public class View : IRenderable
    {
        private readonly string fullyQualifiedTemplateName;

        public View(string fullyQualifiedTemplateName)
        {
            this.fullyQualifiedTemplateName = fullyQualifiedTemplateName;
        }

        private string ReadFile()
        {
            if (File.Exists(this.fullyQualifiedTemplateName))
            {
                return File.ReadAllText(fullyQualifiedTemplateName);
            }
            throw new FileNotFoundException($"File at {fullyQualifiedTemplateName} not found!");
        }

        public string Render()
        {
            return this.ReadFile();
        }
    }
}