using System;

namespace TheColonel2688.Utilities
{ 
    public class ErrorEventArgs : EventArgs
    {
        public string Message;

        public ErrorEventArgs(string msg)
        {
            Message = msg;
        }
    }

}
