using System;
using System.Collections.Generic;
using System.Text;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace TheColonel2688.Utilities
{
    
    public class LoggerAdapterSerilog : IRALLogger
    {
        
        private ILogger _log;

        public LoggerAdapterSerilog(ILogger logger)
        {
            _log = logger;
        }

        void IRALLogger.Debug(string messageTemplate)
        {
            _log.Debug(messageTemplate);
        }

        void IRALLogger.Debug(string messageTemplate, params object[] propertyValues)
        {
            _log.Debug(messageTemplate, propertyValues);
        }

        void IRALLogger.Debug(Exception exception, string messageTemplate)
        {
            _log.Debug(exception, messageTemplate);
        }
       

        void IRALLogger.Debug(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            _log.Debug(exception, messageTemplate, propertyValues);
        }

        void IRALLogger.Error(string messageTemplate)
        {
            _log.Error(messageTemplate);
        }

        void IRALLogger.Error(string messageTemplate, params object[] propertyValues) => _log.Error(messageTemplate, propertyValues);

        void IRALLogger.Error(Exception exception, string messageTemplate)
        {
            _log.Error(exception, messageTemplate);
        }

        void IRALLogger.Error(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            _log.Error(exception, messageTemplate, propertyValues);
        }

        void IRALLogger.Fatal(string messageTemplate)
        {
            throw new NotImplementedException();
        }

        void IRALLogger.Fatal(string messageTemplate, params object[] propertyValues)
        {
            throw new NotImplementedException();
        }

        void IRALLogger.Fatal(Exception exception, string messageTemplate)
        {
            throw new NotImplementedException();
        }

        void IRALLogger.Fatal(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            throw new NotImplementedException();
        }

        void IRALLogger.Information(string messageTemplate)
        {
            throw new NotImplementedException();
        }

        void IRALLogger.Information(string messageTemplate, params object[] propertyValues)
        {
            throw new NotImplementedException();
        }

        void IRALLogger.Information(Exception exception, string messageTemplate)
        {
            throw new NotImplementedException();
        }

        void IRALLogger.Information(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            throw new NotImplementedException();
        }

        bool IRALLogger.IsEnabled(LogEventLevel level)
        {
            throw new NotImplementedException();
        }

        void IRALLogger.Verbose(string messageTemplate)
        {
            throw new NotImplementedException();
        }

        void IRALLogger.Verbose(string messageTemplate, params object[] propertyValues)
        {
            throw new NotImplementedException();
        }

        void IRALLogger.Verbose(Exception exception, string messageTemplate)
        {
            throw new NotImplementedException();
        }

        void IRALLogger.Verbose(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            throw new NotImplementedException();
        }

        void IRALLogger.Warning(string messageTemplate)
        {
            throw new NotImplementedException();
        }

        void IRALLogger.Warning(string messageTemplate, params object[] propertyValues)
        {
            throw new NotImplementedException();
        }

        void IRALLogger.Warning(Exception exception, string messageTemplate)
        {
            throw new NotImplementedException();
        }

        void IRALLogger.Warning(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            throw new NotImplementedException();
        }
        
    }
    
}
