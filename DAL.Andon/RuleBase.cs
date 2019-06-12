using Serilog;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using RAL.Rules;
using System;
using TheColonel2688.Utilities;
using Nito.AsyncEx;

namespace RAL.Rules.Core
{
    public abstract class RuleBase<T> : IRule
    {
        protected abstract string ClassType { get; }

        protected CancellationTokenSource cts = new CancellationTokenSource();
        protected CancellationToken ct;

        protected AsyncManualResetEvent StoppedEvent = new AsyncManualResetEvent();

        public abstract string Description { get; }

        protected ILogger _logger { get; set; }

        protected event EventHandler<ExceptionOccurredEventArgs> ExceptionOccurred;

        private bool _ThrowOnExceptions;

        public RuleBase(bool BreakOnExceptions = false)
        {
            _ThrowOnExceptions = BreakOnExceptions;
            ct = cts.Token;
        }

        protected abstract Task<(bool RuleIsMet, bool HasRuleIsMetChanged, T data)> PerformEvaluateRuleAsync();

        protected async Task EvaluateRuleAndExecuteActionsAsync()
        {

            try
            {
                //_logger?.Here(ClassType, Description).Debug("Started");

                var result = await PerformEvaluateRuleAsync();

                var (RuleIsMet, HasRuleIsMetChanged, data) = result;

                var tasks = new List<Task>();

                foreach (var ruleAction in ruleActions)
                {
                    tasks.Add(ruleAction.ExecuteAsync(RuleIsMet, HasRuleIsMetChanged, data));
                }
                await Task.WhenAll(tasks.ToArray());
            }
            catch (Exception ex)
            {
                if (ExceptionOccurred is null)
                {
                    if (!_ThrowOnExceptions)
                    {
                        _logger?.Here(ClassType, Description).Warning(ex, "Unknown Exception was thrown during evaluation. No Exception EventHandler is subscribed, Exception Was ignored.");
                    }
                    else
                    {
                        _logger?.Here(ClassType, Description).Error(ex, "Unknown Exception was thrown during evaluation. No Exception EventHandler is subscribed. Re-throwing");
                        throw;
                    }
                }
                else
                {
                    _logger?.Here(ClassType, Description).Debug("Invoking Exception event handler");
                    ExceptionOccurred.Invoke(this, new ExceptionOccurredEventArgs(ex));                 
                }
            }
        }

        protected List<IRuleAction<T>> ruleActions = new List<IRuleAction<T>>();

        public void AddAction(IRuleAction<T> action)
        {
            ruleActions.Add(action);
        }

        public virtual async Task StartAsync()
        {
            var tasks = new List<Task>();

            foreach(var ruleAction in ruleActions)
            {
                tasks.Add(ruleAction.InitializeAsync());
            }

            await Task.WhenAll(tasks.ToArray());
        }

        public virtual void Start()
        {
            StartAsync().GetAwaiter().GetResult();
        }

        public virtual Task StopAsync()
        {

            StoppedEvent.Set();
            return Task.CompletedTask;
        }

        public virtual void Stop()
        {
            StopAsync().GetAwaiter().GetResult();
        }

        public virtual void Close()
        {
            CloseAsync().GetAwaiter().GetResult();
        }
        
        public virtual async Task CloseAsync()
        {
            await StopAsync();
            cts.Cancel();
        }

        public void WaitForClose()
        {
            WaitHandle.WaitAll(new[] { ct.WaitHandle });
        }

        public async void WaitForCloseAsync()
        {
            await Task.Run(() => WaitForClose());
        }

        public WaitHandle GetCloseWaitHandle()
        {
            return ct.WaitHandle;
        }
    }
}
