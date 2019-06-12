using Serilog;
using System;
using System.Diagnostics;
using System.Timers;

namespace TheColonel2688.Utilities
{
    public class NonReentrantTimer : HasLogger, IDisposable
    {
        //private readonly ILogger _logger;

        Timer _timer = new Timer();

        public event ElapsedEventHandler Elapsed;

        //private string _discription;

        //protected override string _classTypeAsString => nameof(NonReentrantTimer);

        protected override string ClassTypeAsString => nameof(NonReentrantTimer);

        public override string Description => _getDiscription();

        private Func<string> _getDiscription;

        volatile bool _IsProcessingElapsedMethod = false;

        volatile int _reentranceAttemptsSinceLastEntry = 0;

        public NonReentrantTimer(Func<string> getDiscription, ILogger logger = null) : base(logger)
        {
            _getDiscription = getDiscription;
            _timer = new Timer();
            _timer.Elapsed += _timer_Elapsed;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval">Interval is ms</param>
        /// <param name="discription">Name of class instance</param>
        /// <param name="logger">Logger instance for Class</param>
        public NonReentrantTimer(double interval, ILogger logger = null) : base(logger)
        {
            _timer = new Timer(interval);
            _timer.Elapsed += _timer_Elapsed;
        }

        
        public NonReentrantTimer(ILogger logger = null) : base(logger)
        {
            _timer = new Timer();
            _timer.Elapsed += _timer_Elapsed;
        }
        
        public double Interval { get => _timer.Interval; set { _timer.Interval = value; } }

        public void Start() => _timer.Start();

        public void Stop() => _timer.Stop();

        public void Close() => _timer.Close();

        private DateTime _lastEntrance;

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_IsProcessingElapsedMethod)
            {
                _logger()?.Warning("Re-entrance was attempted and Ignored. {attempts} Attempts, {Elapsed} has Elapsed", ++_reentranceAttemptsSinceLastEntry,  DateTime.Now - _lastEntrance);
                return;
            }
            _logger()?.Verbose("Entering");
            _lastEntrance = DateTime.Now;
            _reentranceAttemptsSinceLastEntry = 0;
            _IsProcessingElapsedMethod = true;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            try
            {
                Elapsed?.Invoke(sender, e);
            }
            finally
            {
                sw.Stop();
                if (sw.ElapsedMilliseconds > _timer.Interval)
                {
                    _logger()?.Warning("Processing Took {ProcessingTimeElapsed} of {IntervalTime}", sw.Elapsed, TimeSpan.FromMilliseconds(_timer.Interval));
                }
            }
            _logger()?.Verbose("Leaving");
            _IsProcessingElapsedMethod = false;
        }

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}
