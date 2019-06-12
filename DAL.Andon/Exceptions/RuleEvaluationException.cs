using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace RAL.Rules.Exceptions
{
    public class RuleEvaluationException : Exception
    {
        ///private HttpRequestException ex;

        public RuleEvaluationException(string message) : base(message)
        {
        }

        public RuleEvaluationException(string message, Exception inner)
        : base(message, inner)
        {

        }

    }
}
