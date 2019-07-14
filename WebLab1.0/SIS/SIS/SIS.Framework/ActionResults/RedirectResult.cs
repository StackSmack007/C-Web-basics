namespace SIS.Framework.ActionResults
{
using SIS.Framework.ActionResults.Contracts;
    public class RedirectResult : IRedirectable
    {
        public string RedirectUrl { get; set; }

        public RedirectResult(string url)
        {
            RedirectUrl = url;
        }

        public string Invoke()
        {
            return this.RedirectUrl;
        }
    }
}