namespace SIS.MVC.ViewEngine
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Emit;
    using SIS.MVC.Contracts;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Loader;
    using System.Text;
    using System.Text.RegularExpressions;

    public interface IView
    {
        string GetView();
    }

    public class View_Engine : IViewEngine
    {
        private Assembly newAssembly;
        private static HashSet<string> usedClassNames = new HashSet<string>();

        public string GetHtmlImbued(string htmlNotRendered, IDictionary<string, object> viewData)
        {
            IView instance = MakeViewInstance(htmlNotRendered, viewData);
            return instance.GetView();
        }

        private IView MakeViewInstance(string htmlNotRendered, IDictionary<string, object> viewData)
        {
            string className = GetNewUnusedName();
            string ViewModelClassName = className.Replace('a', 'b');

            string classContent =
            #region C#ClassAsString implementing IViewEngine 
@"namespace SIS.MVC.ViewEngine.GeneratedModels
{
     using System.Linq;
     using System.Text;"

    + $"{GenerateViewDataClassString(ViewModelClassName, viewData)}" + @"
    
    class " + $"{className}" + @" : IView
    {
       private " + ViewModelClassName + @" model;
       
       public " + $"{className}" + @"(System.Collections.Generic.IDictionary<string, object> viewData)
       {
           this.model =new " + ViewModelClassName + @"(viewData);
       }

       private " + ViewModelClassName + @" Model =>this.model;

       public string GetView()
       {
           StringBuilder sb = new StringBuilder();
           " + ConvertRawHTMLToCode(htmlNotRendered) + @"
           return sb.ToString().Trim();
}   }  }}";
            #endregion

            ConjureClass(classContent, className, viewData);

            var type = newAssembly.GetTypes().FirstOrDefault(x => x.Name == className);

            return (IView)Activator.CreateInstance(type, new object[] { viewData });
        }

        private string GenerateViewDataClassString(string modelClassName, IDictionary<string, object> viewData)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"internal class {modelClassName}");
            sb.AppendLine("{");
            foreach (var kvp in viewData)
            {
                string propertyName = kvp.Key;
                string propertyType = kvp.Value is null ? typeof(string).FullName : kvp.Value.GetType().FullName.Replace("+", ".");
                sb.AppendLine($"internal {propertyType} {propertyName} " + "{get;set;}");
            }
            sb.AppendLine($"internal {modelClassName} (System.Collections.Generic.IDictionary<string, object> modelData)");
            sb.AppendLine("{");
            foreach (var kvp in viewData)
            {
                string propertyName = kvp.Key;
                if (kvp.Value is null)
                {
                    sb.AppendLine($"this.{propertyName}= null;");
                    continue;
                }
                string propertyType = kvp.Value.GetType().FullName.Replace("+", ".");
                sb.AppendLine($"this.{propertyName}=({propertyType}) modelData[\"{propertyName}\"];");
            }
            sb.AppendLine("}");
            return sb.ToString().Trim();
        }

        private void ConjureClass(string classContent, string className, IDictionary<string, object> viewData)
        {
            var dotnetCoreDirectory = Path.GetDirectoryName(typeof(object).GetTypeInfo().Assembly.Location);

            #region Configuring new class's references
            CSharpCompilation compilation = CSharpCompilation.Create(className)
                                  .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                                  .AddReferences(
                                  MetadataReference.CreateFromFile(typeof(IView).GetTypeInfo().Assembly.Location),
                                  MetadataReference.CreateFromFile(typeof(Console).GetTypeInfo().Assembly.Location),
                                  MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
                                  MetadataReference.CreateFromFile(typeof(string).GetTypeInfo().Assembly.Location),
                                  MetadataReference.CreateFromFile(Path.Combine(dotnetCoreDirectory, "System.Runtime.dll")),
                                  MetadataReference.CreateFromFile(Path.Combine(dotnetCoreDirectory, "System.Collections.dll")),
                                  MetadataReference.CreateFromFile(Path.Combine(dotnetCoreDirectory, "System.Text.RegularExpressions.dll")),
                                  MetadataReference.CreateFromFile(Path.Combine(dotnetCoreDirectory, "System.Linq.dll")));

            foreach (object obj in viewData.Values.Where(x=>x!=null))
            {
                compilation = compilation
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(MetadataReference.CreateFromFile(obj.GetType().GetTypeInfo().Assembly.Location));
            }

            compilation = compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(classContent));
            #endregion

            string fullPath = GetPathOfTempFile(Assembly.GetAssembly(typeof(View_Engine)).Location, @"SIS.MVC/ViewEngine/GeneratedModels", className + ".dll");

            EmitResult emitResult = compilation.Emit(fullPath);
            if (emitResult.Success)
            {
                newAssembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.GetFullPath(fullPath));
            }
            else
            {
                foreach (var compilerMessage in compilation.GetDiagnostics())
                {
                    Console.WriteLine(compilerMessage);
                }
            }
        }

        private string GetPathOfTempFile(string innerPath, string destination, string filename)
        {
            string[] foldersToBeContained = destination.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            string currentPath = Path.GetFullPath(Path.Combine(innerPath, $"../"));

            while (!FoldersAreContained(currentPath, foldersToBeContained))
            {
                currentPath = Path.GetFullPath(Path.Combine(currentPath, $"../"));
            }
            currentPath = Path.GetFullPath(Path.Combine(currentPath, destination)) + "\\" + filename;
            return currentPath;
        }

        #region Obtaining TempFolder
        private bool FoldersAreContained(string path, string[] foldersToBeContained)
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

        private static string GetNewUnusedName()
        {
            string className = ("TempClass" + Guid.NewGuid()).Replace("-", "");
            while (usedClassNames.Contains(className))
            {
                className = className = ("TempClass" + Guid.NewGuid()).Replace("-", "");
            }
            usedClassNames.Add(className);
            return className;
        }
        #endregion

        private string ConvertRawHTMLToCode(string htmlNotRendered)
        {
            htmlNotRendered = htmlNotRendered.Replace("\n\n", "\r\n");
            htmlNotRendered = htmlNotRendered.Replace("\n", "\r\n");

            string patternOfInnerDataCode = @"@[^><\""\\\s]+\.ToString\(\\\"".+?\\\""\)|@[^><\""\\\s]+";
            string[] commandsPosibleRow = { "{", "}", "@if", "@else", "@for", "@while", "@when" };
            string[] rawHtmlRows = htmlNotRendered.Split(Environment.NewLine)
                                                  .Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x))
                                                  .Select(x => x.Replace("\"", "\\\"").Trim()).ToArray();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < rawHtmlRows.Length; i++)
            {
                if (commandsPosibleRow.Any(x => rawHtmlRows[i].StartsWith(x)))
                {
                    sb.AppendLine(RemoveAtFirst(rawHtmlRows[i]));
                }
                else
                {
                    string row = rawHtmlRows[i];

                    MatchCollection matchCollection = Regex.Matches(rawHtmlRows[i], patternOfInnerDataCode);

                    foreach (Match match in matchCollection)
                    {
                        string replacement = $"\"+{RemoveAtFirst(match.Value)}+\"";
                        rawHtmlRows[i] = rawHtmlRows[i].Replace(match.Value, replacement);
                    }
                    sb.AppendLine("sb.AppendLine(\"" + rawHtmlRows[i] + "\");");
                }
            }
            return sb.ToString().Trim();
        }

        private string RemoveAtFirst(string row)
        {

            if (!row.Contains('@'))
            {
                return row;
            }
            int indexOfFirstAt = row.IndexOf('@');
            string result = row.Substring(0, indexOfFirstAt) + row.Substring(indexOfFirstAt + 1).Replace("\\\"", "\"");
            return result;
        }
    }
}