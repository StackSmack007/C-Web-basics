namespace SIS.HTTP.Responses
{
    using SIS.HTTP.Common;
    using SIS.HTTP.Cookies;
    using SIS.HTTP.Cookies.Contracts;
    using SIS.HTTP.Exceptions;
    using SIS.HTTP.Headers;
    using SIS.HTTP.Headers.Contracts;
    using SIS.HTTP.Responses.Contracts;
    using System.Linq;
    using System.Net;
    using System.Text;

    public class HttpResponse : IHttpResponse
    {
        public HttpResponse(HttpStatusCode statusCode)
        {
            Headers = new HttpHeaderCollection();
            Content = new byte[0];
            StatusCode = statusCode;
            Cookies = new HttpCookieCollection();
        }
        public IHttpCookieCollection Cookies { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public IHttpHeaderCollection Headers { get; set; }
        public byte[] Content { get; set; }

        public void AddHeader(HttpHeader header)
        {
            if (header is null)
            {
                throw new InternalServerErrorException();
            }
            Headers.Add(header);
        }

        public override string ToString()
        {
            string httpProtocol = GlobalConstants.HttpOneProtocolFragment;
            int statusCode = (int)StatusCode;
            string statusCodeMessage = StatusCode.ToString();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{httpProtocol} {statusCode} {statusCodeMessage}").AppendLine(Headers.ToString());
            if (Cookies.HasCookies())
            {
                sb.AppendLine(Cookies.ToStringSet());
            }
            sb.AppendLine();
            return sb.ToString();
        }

        public byte[] GetBytes()
        {

            byte[] part1 = Encoding.UTF8.GetBytes(this.ToString());

            byte[] result = part1.Concat(Content).ToArray();

            return result;
        }

        public void AddCookie(HttpCookie cookie)
        {
            this.Cookies.Add(cookie);
        }
    }
}