namespace SIS.MVC.Contracts
{
    public interface IViewEngine
    {
        string GetHtmlImbued(string htmlNotRendered,object model);
    }
}