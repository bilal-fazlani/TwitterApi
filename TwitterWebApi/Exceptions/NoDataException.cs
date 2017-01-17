using System;

namespace TwitterWebApi.Exceptions
{
    public class NoDataException:Exception
    {
        public NoDataException(string message):base(message)
        {

        }
    }
}