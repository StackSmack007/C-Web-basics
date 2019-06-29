namespace SIS.HTTP.Extensions
{
    using System;
    using System.Linq;
    using System.Net;

    public static class HttpResponseStatusExtensions
    {
        public static string GetResponseLine(this HttpStatusCode statCode)
        {
            int[] allowedCodes = { 200, 201, 302, 303, 400, 401, 403, 404, 500 };
            if (allowedCodes.Contains((int)statCode))
            {
                return string.Format("{0} {1}", (int)statCode, statCode.ToString());
            }
            return "Code not recognised NSH";
        }
            }
}