namespace SIS.HTTP.Exceptions
{
    public class BadRequestException : ServerException
    {
        private static string message = "The Request was malformed or contains unsupported elements.";
        public BadRequestException() : base(message)
        { }
    }
}