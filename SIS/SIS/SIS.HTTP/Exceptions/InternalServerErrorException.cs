namespace SIS.HTTP.Exceptions
{
    public class InternalServerErrorException : ServerException
    {
        private static string message = "The Server has encountered an error.";
        public InternalServerErrorException() : base(message)
        { }
    }
}