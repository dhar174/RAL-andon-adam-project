using Serilog;
using System.Runtime.CompilerServices;

namespace TheColonel2688.Utilities
{
    public abstract class HasLogger : HasDescription, IHasDescription
    {

        /// <summary>
        /// Class type name this is used to provide context for logging. It is meant to report the type of the last Descendant
        /// </summary>
        protected abstract string ClassTypeAsString { get; }

        protected ILogger _hiddenlogger;

        public HasLogger(ILogger logger)
        {
            _hiddenlogger = logger;
        }

        protected ILogger _logger([CallerMemberName] string memberName = "")
        {
            if (_hiddenlogger is null)
            {
                return null;
            }
            return _hiddenlogger.Here(ClassTypeAsString, Description, memberName);
        }
    }
}
