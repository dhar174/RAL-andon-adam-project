using Serilog;

namespace TheColonel2688.Utilities
{
    public abstract class HasThreadHelper : HasLogger
    {
        protected ThreadHelper _threadHelper;

        public HasThreadHelper(ThreadHelper thread, ILogger logger = null) : base (logger)
        {
            _threadHelper = thread;
        }
    }
}
