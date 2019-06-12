using System;
using System.Runtime.Serialization;

namespace RAL.Repository
{
    public class RepositoryConnectionException : Exception
    {
        public RepositoryConnectionException()
        {


        }

        public RepositoryConnectionException(string message) : base (message)
        {

        }

        public RepositoryConnectionException(string message, Exception innerException) : base (message, innerException)
        {

        }

        protected RepositoryConnectionException(SerializationInfo info, StreamingContext context) : base (info, context)
        {

        }
    }
}
