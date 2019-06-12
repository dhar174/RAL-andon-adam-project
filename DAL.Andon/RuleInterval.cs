using TheColonel2688.Utilities;
using Serilog;
using System;
using System.Diagnostics;
using System.Timers;
using Timer = System.Timers.Timer;
using System.Threading.Tasks;

namespace RAL.Rules.Core
{
    public abstract class RuleInterval<T> : RuleBase<T>
    {
        //protected virtual new string ClassType { get => nameof(RuleInterval<T>); }

        public TimeSpan PollInterval { get => TimeSpan.FromMilliseconds(_timer.Interval); set => _timer.Interval = value.TotalMilliseconds; }

        protected NonReentrantTimer _timer;

        private volatile bool IsProcessing = false;

        //private class TimerState {}

        public RuleInterval(bool dontBreakOnExceptions = false, ILogger logger = null) : base(dontBreakOnExceptions)
        {
            _timer = new NonReentrantTimer(() => Description, logger);
            _timer.Elapsed += _timer_Elapsed;
        }

        public virtual void Initialize(TimeSpan? pollInterval = null)
        {
            if (pollInterval == null)
            {
                pollInterval = TimeSpan.FromMilliseconds(500);
            }

            PollInterval = pollInterval.Value;
        }

        protected virtual void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            EvaluateRuleAndExecuteActionsAsync().GetAwaiter().GetResult();
        }

        public async override Task StartAsync()
        {
            _timer.Start();
            await base.StartAsync();
        }


        public async override Task StopAsync()
        {
            _timer.Stop();
            await StopAsync();
        }

    }
}
