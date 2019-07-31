namespace SIS.MVC.Contracts
{
    public interface IConfiguration
    {
        string DefaultAuthorisedRedirectAdress { get; set; }
        string LayoutsFolderPath { get; set; }
        string KeywordForInsertingBodyInImportLayout { get; set; }
        string LocationOfViewsFolder { get; set; }
        string LocationOfRootFolder { get; set; }
    }
}