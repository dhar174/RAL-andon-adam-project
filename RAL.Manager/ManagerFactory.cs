using FluentScheduler;
using RAL.Collector;
using RAL.Reports;
using RAL.Reports.Scheduler;
using RAL.Repository;
using RAL.Rules;
using RAL.Rules.Core;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RAL.Manager
{
    public class ManagerFactory
    {

        IList<Machine> listOfMachines = new List<Machine>();

        IList<IRule> listOfRules = new List<IRule>();

        IList<(IReport rpeort, Action<Schedule> schdule)> listOfReportsWithSchedule = new List<(IReport rpeort, Action<Schedule> schdule)>();

        //IList<IReportDaily> listOfReports = new List<IReportDaily>();

        List<Action> listOfMachineLoadOperations = new List<Action>();

        List<Action> listOfRuleLoadOperations = new List<Action>();

        List<Action> listOfReportsLoadOperations = new List<Action>();

        
        //ReportsManager reportsManager = new ReportsManager();

        MachineRepository repoToInject;

        private ILogger _loggerToInject;

        private ILogger _loggerForManagerFactor;
        private ILogger _managerLoggerToInject;
        private ILogger _rulesManagerLoggerToInject;
        private ILogger _collectorManagerLoggerToInject;
        private ILogger _reportManagerLoggerToInject;

        public ManagerFactory(ILogger logger = null)
        {
            _loggerForManagerFactor = logger;
        }

        public void WithLoggerForCollectorManager(ILogger logger)
        {
            _collectorManagerLoggerToInject = logger;
        }

        public void WithLoggerForReportManager(ILogger logger)
        {
            _reportManagerLoggerToInject = logger;
        }

        public void WithLoggerForRulesManager(ILogger logger)
        {
            _rulesManagerLoggerToInject = logger;
        }

        public void WithLoggerForManager(ILogger logger)
        {
            _managerLoggerToInject = logger;
        }

        /// <summary>
        /// Add a Machine instance
        /// </summary>
        /// <param name="machine"></param>
        public ManagerFactory AddMachine(Machine machine)
        {
            addMachine(machine);

            return this;
        }

        /// <summary>
        /// Add a multiple Machine instances
        /// </summary>
        /// <param name="machine"></param>
        public ManagerFactory AddMachines(IList<Machine> machines)
        {
            foreach(var machine in machines)
            {
                addMachine(machine);
            }

            return this;
        }


        private void addMachine(Machine machine)
        {
            //** this is added to a list of load operations that are run with Build() is called
            listOfMachineLoadOperations.Add(() =>
            {
                //machine.Initialize(logger: loggerToInject, repository: repoToInject);
                listOfMachines.Add(machine);
            }
            );
        }

        /// <summary>
        /// Add General Rules which are for more than one machine
        /// </summary>
        public ManagerFactory AddRule(IRule rule)
        {
            addRule(rule);
            return this;
        }


        private void addRule(IRule rule)
        {
            if (listOfRules.Any(x => x == rule))
            {
                throw new InvalidOperationException($"Attempting to add rule which is already added. Rules added to {nameof(Machine)} instances will be added automatically.");
            }

            listOfRuleLoadOperations.Add(() =>
                {
                    //rule.Repository = new RepositoryForRules(repoToInject);
                    //rule.Logger = loggerToInject;
                    listOfRules.Add(rule);
                });
        }

        public ManagerFactory AddReport(IReportDaily report, Action<Schedule> schedule)
        {

            addReport(report, schedule);
            return this;
        }

        private void addReport(IReportDaily report, Action<Schedule> schedule)
        {
            listOfReportsLoadOperations.Add(() =>
            {
                listOfReportsWithSchedule.Add((report,schedule)); ;
            });
        }


        //** Call Before Build RulesManager
        private CollectorManager BuildCollectorManager()
        {
            foreach(var machineLoad in listOfMachineLoadOperations)
            {
                machineLoad.Invoke();
            }

            IList<IClientForMQTTCollector> listOfClients = listOfMachines.Select(x => (IClientForMQTTCollector)x.Adam6051Client).ToList();
            var collectorManager = new CollectorManager(listOfClients, _collectorManagerLoggerToInject);

            return collectorManager;
        }


        //** Call After Build CollectorManager
        private RulesManager BuildRulesManager()
        {
            //var rules = new List<IRule>();

            foreach (var machine in listOfMachines)
            {
                foreach (var rule in machine.Rules)
                {
                    addRule(rule);
                }
            }

            foreach (var ruleLoad in listOfRuleLoadOperations)
            {
                ruleLoad.Invoke();
            }

            var rulesManager = new RulesManager(_rulesManagerLoggerToInject) { Rules =  listOfRules};
            return rulesManager;
        }

        private ReportsManager BuildReportManager()
        {
            foreach(var reportLoad in listOfReportsLoadOperations)
            {
                reportLoad.Invoke();
            }
            return new ReportsManager(listOfReportsWithSchedule, _reportManagerLoggerToInject);
        }

        public Manager Build()
        {
            var CollectorManager = BuildCollectorManager();

            var RulesManager = BuildRulesManager();

            var ReportsManager = BuildReportManager();

            var Manager = new Manager(_managerLoggerToInject) { CollectorManager = CollectorManager, RulesManager = RulesManager, ReportsManager = ReportsManager, Machines = listOfMachines };

            return Manager;
        }
    }
}
