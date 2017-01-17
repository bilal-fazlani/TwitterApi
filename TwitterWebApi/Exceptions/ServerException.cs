using System;

namespace TwitterWebApi.Exceptions
{
    public class ServerException : Exception
    {
        public ServerException(string message, Exception innerException):base(message, innerException)
        {

        }
    }
}