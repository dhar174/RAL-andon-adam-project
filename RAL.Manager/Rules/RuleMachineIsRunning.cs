using RAL.Manager;
using RAL.Manager.Rules;
using RAL.Repository;
using RAL.Rules.Exceptions;
using TheColonel2688.Utilities;
using Serilog;
using System;
using System.Threading.Tasks;
using System.Diagnostics;
using RAL.Rules.Core;
using Devices.Core;
using RAL.Repository.Model;

namespace RAL.Rules
{
    public class RuleMachineIsRunning : RuleInterval<RuleIsRunningData>, ISingleMachineRule<Machine>
    {
        protected override string ClassType { get => nameof(RuleMachineIsRunning); }

        public override string Description { get => $"Rule MachineIsRunning for {Machine?.Line}.{Machine?.Name}"; }

        public Machine Machine { get; set; }

        public TimeSpan AllowedCycleTime { get; set; }

        public IMachineRepository Repository;

        private (bool Result, DateTime When) LastResult;

        public RuleMachineIsRunning(Machine machine, TimeSpan allowedCycleInterval, TimeSpan? pollInterval = null, IMachineRepository repository = null, ILogger logger = null) 
            : base(dontBreakOnExceptions: true, logger: logger)
        {
            Initialize(machine, allowedCycleInterval, pollInterval, repository, logger);
        }

        public void Initialize(Machine machine, TimeSpan allowedCycleInterval, TimeSpan? pollInterval = null, IMachineRepository repo = null, ILogger logger = null)
        {
            base.Initialize(pollInterval);

            Machine = machine;

            Repository = repo;

            AllowedCycleTime = allowedCycleInterval;

            _logger = logger;

            base.ExceptionOccurred += RuleMachineIsRunning_ExceptionOccurred;
        }

        private void RuleMachineIsRunning_ExceptionOccurred(object sender, ExceptionOccurredEventArgs e)
        {
            var exception = e.Exception;

            switch (exception)
            {
                case DeviceConnectionException ex:
                    //** Ignoring should have already been logged;
                    break;
                case RepositoryConnectionException ex:
                    //** Ignoring should have already been logged;
                    var blah  = ex.Message;
                    break;
                case RuleEvaluationException ex:
                    //** Ignoring should have already been logged;
                    break;               
                default:
                    _logger.Here(nameof(RuleMachineIsRunning), Description).Error(e.Exception, "Unknown Exception was thrown during evaluation or action.");
                    break;
            }
        }
        /*
        public override Task StartAsync()
        {
            //** QUESTION? Is this naughty??????????

            /Task.Run(() => Start()).Start();

            return Task.CompletedTask;
        }*/

        public async override Task StartAsync()
        {
            Stopwatch sw = Stopwatch.StartNew();
            _logger.Here(nameof(RuleMachineIsRunning), Description).Information("Starting Rule");

            try
            {
                _logger.Here(nameof(RuleMachineIsRunning), Description).Debug("Checking Connection To the Database");
                Stopwatch swsub = new Stopwatch();
                swsub.Start();
                var CanConnect = Repository.CanConnect();
                swsub.Stop();

                _logger.Here(nameof(RuleMachineIsRunning), Description).Debug("{method} returned {value} took {Elapsed} to execute", nameof(Repository.CanConnect), CanConnect, swsub.Elapsed);


                if (CanConnect)
                {
                    swsub.Start();
                    try
                    {

                        (bool Result, DateTime When) lastResult = await GetLastResultOrDefaultAsync();

                        swsub.Stop();

                        if (lastResult == (default, default))
                        {
                            _logger.Here(nameof(RuleMachineIsRunning), Description).Warning("No previous result was found in the database.");
                        }

                        LastResult = lastResult;
                    }
                    catch (Exception ex)
                    {
                        _logger.Here(nameof(RuleMachineIsRunning), Description).Warning(ex, "Exception Thrown when calling {methodName}", nameof(GetLastResultOrDefaultAsync));
                    }
                    finally
                    {
                        swsub.Stop();
                        _logger.Here(nameof(RuleMachineIsRunning), Description).Debug("{method} took {Elapsed} to execute", nameof(GetLastResultOrDefaultAsync), swsub.Elapsed);
                    }
                }
            }
            finally
            {
                await base.StartAsync();
                sw.Stop();
                _logger.Here(nameof(RuleMachineIsRunning), Description).Debug("Rule Starting Completed. Took {elapsed} to Execute", sw.Elapsed);
            }
        }

        private async Task<(bool Result, DateTime When)> GetLastResultOrDefaultAsync()
        {
           var resultsForLastLegitCycle = await Repository.MachineStatusRepo.LastOrDefaultAsync(Machine.Line, Machine.Name);

            if(resultsForLastLegitCycle is null)
            {
                _logger?.Here(nameof(RuleMachineIsRunning), Description).Warning("Could not find a IsCycling TRUE, IsInAutomatic TRUE, IsFaulted FALSE entry for Line.Machine {Line}.{Name}", Machine.Line, Machine.Name);
                return (false, default);
            }

           return GetStatusFromInfluxType(resultsForLastLegitCycle);
        }

        public (bool Result, DateTime When) GetStatusFromInfluxType(MachineStatusInflux status)
        {
            return (status.IsCycling && status.IsInAutomatic && status.IsFaulted == false, status.Time);
        }

        protected async override Task<(bool, bool, RuleIsRunningData)> PerformEvaluateRuleAsync()
        {
            Stopwatch sw = Stopwatch.StartNew();

            try
            {               
                MachineStatusInflux resultsQuery;
                Stopwatch swsub = Stopwatch.StartNew();
                try
                {
                    resultsQuery = await Repository.MachineStatusRepo.LastOrDefaultWhereIsAsync(Machine.Line, Machine.Name, true, true, false);
                }
                finally
                {

                    swsub.Stop();
                    if(swsub.ElapsedMilliseconds > 1000)
                    {
                        _logger.Here(nameof(RuleMachineIsRunning), Description).Warning("{Method} took {elapsed}ms of preferred {Limit}ms",nameof(Repository.MachineStatusRepo.LastOrDefaultWhereIsAsync), swsub.ElapsedMilliseconds, 1000);
                    }
                }

                

                if (resultsQuery is null)
                {
                    _logger?.Here(nameof(RuleMachineIsRunning), Description).Warning("Could not find a IsCycling TRUE, IsInAutomatic TRUE, IsFaulted FALSE entry for Line.Machine {Line}.{Name}", Machine.Line, Machine.Name);
                    throw new RuleEvaluationException($"Could not find a IsCycling TRUE, IsInAutomatic TRUE, IsFaulted FALSE entry for Line.Machine \"{Machine.Line}.{Machine.Name}\"");
                }

                var resultForLastLegitCycle = resultsQuery;

                DateTime now = DateTime.UtcNow;

                TimeSpan limit = AllowedCycleTime;

                TimeSpan sinceLast = now - resultForLastLegitCycle.Time;

                TimeSpan timeOver = sinceLast - limit;

                TimeSpan timeUnder = limit - sinceLast;

                bool IsRuleMet = (sinceLast < limit);

                var HasRuleIsMetChanged = false;

                if (LastResult.Result != IsRuleMet)
                {
                    _logger?.Here(nameof(RuleMachineIsRunning), Description).Verbose("Rule {Description} changed to {IsRuleMet}, time from last change {time}",
                        Description, IsRuleMet, (DateTime.UtcNow - LastResult.When));

                    HasRuleIsMetChanged = true;

                    LastResult = (IsRuleMet, DateTime.UtcNow);
                }

                //_logger?.Here(nameof(RuleMachineIsRunning), Description).Verbose("Last Status {isCyclingName} is {isCycling} && " +
                //"{isInAutomaticName} is {isInAutomatic} && {isFaultedName} is {isFaulted} was {Elapsed}", nameof(resultForLastLegitCycle.IsCycling), resultForLastLegitCycle.IsCycling,
                //nameof(resultForLastLegitCycle.IsInAutomatic), resultForLastLegitCycle.IsInAutomatic, nameof(resultForLastLegitCycle.IsFaulted), resultForLastLegitCycle.IsFaulted, now - resultForLastLegitCycle.Time);
                _logger?.Here(nameof(RuleMachineIsRunning), Description).Verbose("Since last Status {Elapsed}", now - resultForLastLegitCycle.Time);
                var data = new RuleIsRunningData() { When = now, Machine = Machine };

                return (IsRuleMet, HasRuleIsMetChanged, data);
            }
            catch (Exception ex)
            {
                _logger?.Here(nameof(RuleMachineIsRunning), Description).Error(ex, "Rule Evaluation failed");
                throw;
            }
            finally
            {
                sw.Stop();
                _logger?.Here(nameof(RuleMachineIsRunning), Description).Debug("Took {elapsed}", sw.Elapsed);
            }

        }
    }
}
