using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RAL.Rules.Core
{
    public interface IRuleAction<T>
    {

        Task ExecuteAsync(bool IsRuleMet, bool HasRuleMetChanged, T Data);

        //Task EvaluatedAsFalse(T data);

        //Task EvaluatedAsTrue(T data);

        Task InitializeAsync();
    }
}
