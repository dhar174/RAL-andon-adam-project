using System;

namespace TheColonel2688.Utilities
{
    public class ExceptionOccurredEventArgs : EventArgs
    {
        public Exception Exception { get; private set; }

        public ExceptionOccurredEventArgs(Exception exception)
        {
            Exception = exception;
        }
    }
}
