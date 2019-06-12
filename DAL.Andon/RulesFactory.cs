using RAL.RulesEngine;
using Serilog;
using System;
using System.Collections.Generic;

namespace RAL.Factory
{
    public class RulesFactory
    {
        private List<Action> listOfLoadOperations = new List<Action>();

        private List<Action> listOfActions = new List<Action>();

        private IMapper mapper = new Mapper(
            new MapperConfiguration(cfg =>
                cfg.CreateMap<MachineConfigDSC, MachineInfo>(MemberList.Source)
                .ForSourceMember(x => x.PayloadConverterType, opt => opt.DoNotValidate())
                .ForSourceMember(x => x.MachineType, opt => opt.DoNotValidate())

            )
            );

        private ILogger _logger;

        private IRepositoryForRules repoToInject;

        private ILogger loggerToInject;

        private IList<IRule> listOfRules = new List<IRule>();

        public RulesFactory(ILogger logger = null)
        {
            _logger = logger;
        }

        /*
        public RulesFactory AddFromStorageType(IEnumerable<MachineConfigDSC> listOfMachineConfigs)
        {
            listOfLoadOperations.Add(() =>
            {
                var listOfStackLights = new List<IStackLight5Light>();

                foreach (var machine in listOfMachineConfigs)
                {
                    IMachineForRules newMachine = mapper.Map<MachineInfo>(machine);

                    foreach (var rule in machine.Rules.Where(x => x.TypeOfRuleInterval == typeof(RuleMachineIsRunning)))
                    {
                        var newRule = new RuleMachineIsRunning(rule.AllowedCycleTime, null, newMachine, repoToInject, loggerToInject);

                        listOfRules.Add(newRule);
                    }

                    foreach (var rule in machine.Rules.Where(x => x.TypeOfRuleInterval == typeof(RuleMachineIsRunningStackLight)))
                    {
                        var existingRule = (RuleMachineIsRunningStackLight)listOfRules.FirstOrDefault(x => x is RuleMachineIsRunningStackLight && ((RuleMachineIsRunningStackLight)x).StackLight.IPAddress == rule.StackLightUri.IP);

                        //var existingStackLight = existingRule.StackLight;

                        IStackLight5Light stackLight;

                        if (existingRule is null)
                        {
                            stackLight = new SignaworksEthernetAsStackLightRYGBW(new SignaworksEthernetStackLight(rule.StackLightUri.IP));
                            listOfStackLights.Add(stackLight);
                        }
                        else
                        {
                            stackLight = existingRule.StackLight;
                        }

                        var newRule = new RuleMachineIsRunningStackLight(machine: newMachine, repository:repoToInject, logger:loggerToInject, stackLight:stackLight, lightNumber: (StackLight5Lights.LightNumber)rule.LightNumber);

                        listOfRules.Add(newRule);
                    }
                }

                _logger.Here(nameof(RulesFactory), "").Debug("{stackLightCount} were Loaded", listOfStackLights.Count());
            });
            return this;
        }
        */
        /*
        public RulesFactory LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Machine Config file not found", filePath);
            }

            List<MachineConfigDSC> machines;

            var ser = new ConfigurationContainer().Create();

            using (XmlReader reader = XmlReader.Create(filePath))
            {
                machines = (List<MachineConfigDSC>)ser.Deserialize(reader);

                AddFromStorageType(machines);
            }

            return this;
        }
        */

        public RulesFactory WithRepository(IRepositoryForRules repository)
        {
            repoToInject = repository;

            return this;
        }

        public RulesFactory WithLogger(ILogger logger)
        {
            loggerToInject = logger;
            return this;
        }

        public IList<IRule> Build()
        {
            if (repoToInject is null)
            {
                throw new InvalidOperationException($"Repository Writer is Required, please call {nameof(WithRepository)} before {nameof(Build)}.");
            }

            foreach (var loadAction in listOfLoadOperations)
            {
                loadAction.Invoke();
            }

            return listOfRules;
        }
    }
}