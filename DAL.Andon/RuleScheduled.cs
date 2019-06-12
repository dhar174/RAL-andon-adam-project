using Serilog;
using FluentScheduler;
using System.Threading.Tasks;

namespace RAL.Rules.Core
{
    public abstract class RuleScheduled : RuleBase<object>
    {
        private int _hour;
        public int Hour { get { return _hour; } set { updateHour(value); } }
        
        private int _minute;
        public int Minute { get { return _minute; } set { updateMinute(value); } }

        private IDayRestrictableUnit _job;

        public RuleScheduled() : base() { }

        private void updateHour(int hour)
        {

            if (_job == null)
            {
                var register = new Registry();
                _job = register.Schedule(() => EvaluateRuleAndExecuteActionsAsync()).NonReentrant().ToRunEvery(0).Days().At(hour, 0);
            }
            else
            {
                _job.Schedule.ToRunEvery(0).Days().At(hour, _minute);
            }
            _hour = hour;
        }

        private void updateMinute(int minute)
        {
            if (_job == null)
            {
                var register = new Registry();
                _job = register.Schedule(() => EvaluateRuleAndExecuteActionsAsync()).NonReentrant().ToRunEvery(0).Days().At(0, minute);
            }
            else
            {
                _job.Schedule.ToRunEvery(0).Days().At(_hour, minute);
            }
            _minute = minute;
        }

        public async override Task StartAsync()
        {
            _job.Schedule.Enable();
            await base.StartAsync();
        }

        public async override Task StopAsync()
        {
            _job.Schedule.Disable();
            await base.StopAsync();
        }

    }
}
