using RAL.Collector;
using RAL.Reports.Scheduler;
using RAL.Rules.Core;
using Serilog;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheColonel2688.Utilities;

namespace RAL.Manager
{
    public class Manager : HasLogger
    {
        public CollectorManager CollectorManager { get; set; }
        public RulesManager RulesManager { get; set; }
        public ReportsManager ReportsManager { get; set; }
        public IList<Machine> Machines { get; set; }

        protected override string ClassTypeAsString => nameof(Manager);

        public Manager(ILogger logger) : base (logger)
        {

        }

        public void Start()
        {
            _logger()?.Information("Manager Start Called");
            _logger()?.Information("Starting Collector...");
            CollectorManager.Start();
            _logger()?.Information("Collector Starting Completed");
            _logger()?.Information("Starting Rules...");
            RulesManager.StartAsync().Wait();
            _logger()?.Information("Rules Starting Completed");
            _logger()?.Information("Starting Reports...");
            ReportsManager.Start();
            _logger()?.Information("Rules Starting Completed");
            _logger()?.Information("Manager Start Completed");
        }

        public void WaitForClose()
        {
            Task collectorTask = CollectorManager.WaitForCloseToCompleteAsync();
            Task rulesTask = RulesManager.WaitForCloseToCompleteAsync();

            Task.WaitAll(collectorTask, rulesTask);
        }
    }
}
