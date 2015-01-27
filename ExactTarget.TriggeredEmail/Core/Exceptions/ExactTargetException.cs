using System;

namespace ExactTarget.TriggeredEmail.Core.Exceptions
{
    public class ExactTargetException : Exception
    {
        public ExactTargetException()
        {
        }

        public ExactTargetException(string message) : base(message)
        {
        }

        public ExactTargetException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
