namespace SIS.MVC.Services
{
    using SIS.MVC.Contracts;
    public class Configuration : IConfiguration
    {
        public string DefaultAuthorisedRedirectAdress { get; set; } = "/Users/Login";
        public string LayoutsFolderPath { get; set; } = "/Views/Layouts/";
        public string DefaultLayoutName { get; set; } = "_importLayout.html";
        public string KeywordForInsertingBodyInImportLayout { get; set; } = "@BodyContent@";
        public string LocationOfViewsFolder { get; set; } = "/Views/";
        public string LocationOfRootFolder { get; set; } = "/Root/";
    }
}