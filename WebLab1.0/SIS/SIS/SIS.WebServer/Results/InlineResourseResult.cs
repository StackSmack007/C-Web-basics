namespace SIS.WebServer.Results
{
    using SIS.HTTP.Headers;
    using SIS.HTTP.Responses;
    using System.Net;
    public class InlineResourseResult : HttpResponse
    {
        public InlineResourseResult(byte[] content,HttpStatusCode statusCode) : base(statusCode)
        {
            Headers.Add(new HttpHeader(HttpHeader.ContentLengthKey, content.Length.ToString()));
            Headers.Add(new HttpHeader(HttpHeader.ContentDispositionKey, "inline"));
            Content = content;
        }
    }
}