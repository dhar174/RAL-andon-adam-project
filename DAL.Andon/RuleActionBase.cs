using RAL.Rules;
using System.Threading.Tasks;

namespace RAL.Rules.Core
{
    public abstract class RuleActionBase<T> : IRuleAction<T>
    {
        public abstract Task ExecuteAsync(bool IsRuleMet, bool HasRuleMetChanged, T Data);

        //public abstract Task EvaluatedAsFalse(T data);

        //public abstract Task EvaluatedAsTrue(T data);

        public abstract Task InitializeAsync();
    }
}
