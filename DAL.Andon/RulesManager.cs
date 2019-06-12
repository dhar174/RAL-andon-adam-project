using TheColonel2688.Utilities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RAL.Rules.Core
{
    public class RulesManager
    {
        public IList<IRule> Rules = new List<IRule>();

        ILogger _logger;

        public RulesManager(ILogger logger = null)
        {
            _logger = logger;
        }

        public async Task StartAsync()
        {
            _logger?.Here(nameof(RulesManager)).Information("Rules Manager Starting");
            if (Rules.Count() < 1)
            {
                throw new InvalidOperationException($"{nameof(StartAsync)} Called with no rules added.");
            }
            _logger?.Here(nameof(RulesManager)).Information("Starting Rules Manager with {rulesCount}", Rules.Count());


            IList<Task> tasks = new List<Task>();
            foreach (var rule in Rules)
            {
                tasks.Add(rule.StartAsync());
            }

            await Task.WhenAll(tasks);
            _logger?.Here(nameof(RulesManager)).Information("Rules Manager start Completed.");
        }

        public async Task StopAsync()
        {
            _logger?.Here(nameof(RulesManager)).Information("Rules Manager Requested Stop");


            IList<Task> tasks = new List<Task>();
            foreach(var rule in Rules)
            {
                tasks.Add(rule.StopAsync());
            }

            await Task.WhenAll(tasks);
        }

        public async Task CloseAsync()
        {
            _logger?.Here(nameof(RulesManager)).Information("Rules Manager Requested Close");
            //** Will this run asynchronously??
            IList<Task> tasks = new List<Task>();
            foreach (var rule in Rules)
            {
                tasks.Add(rule.CloseAsync());
            }

            await Task.WhenAll(tasks);
        }

        public async Task WaitForCloseToCompleteAsync()
        {
            if (Rules.Count() < 1)
            {
                throw new InvalidOperationException($"{nameof(WaitForCloseToCompleteAsync)} Called with no rules added.");
            }

            _logger?.Here(nameof(RulesManager)).Information("Asynchronously Waiting For Rules Manager to Stop");
            var waitHandles = Rules.Select(x => x.GetCloseWaitHandle().WaitOneAsync()).ToArray();

            await Task.WhenAll(waitHandles);

        }
    }
}
