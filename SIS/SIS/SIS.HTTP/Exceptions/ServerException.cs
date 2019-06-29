namespace SIS.HTTP.Exceptions
{
    using System;
    public abstract  class ServerException : Exception
    {
        protected ServerException(string message) : base(message) { }
    }
}