namespace SIS.Framework.Controllers
{
    using SIS.Framework.ActionResults;
    using SIS.Framework.ActionResults.Contracts;
    using SIS.Framework.Utilities;
    using SIS.Framework.Views;
    using SIS.HTTP.Requests.Contracts;
    using System.Runtime.CompilerServices;

    public abstract class Controller
    {
        protected Controller() { }

        public IHttpRequest Request { get; set; }

        protected IViewable View([CallerMemberName] string viewName = "")
        {
            string controllerNameClean = ControllerUtilities.GetControllerName(this);

            string fullyQualifiedName = ControllerUtilities.GetViewFullQualifiedName(controllerNameClean, viewName);

            IRenderable view = new View(fullyQualifiedName);

            return new ViewResult(view);
        }
    }
}