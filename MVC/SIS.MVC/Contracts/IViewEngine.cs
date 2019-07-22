namespace SIS.MVC.Contracts
{
using System.Collections.Generic;
    public interface IViewEngine
    {
        string GetHtmlImbued(string htmlNotRendered, IDictionary<string, object> viewData);
    }
}