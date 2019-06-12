using System.Threading.Tasks;
using Serilog;
using RAL.Rules.Core;
using TheColonel2688.Utilities;
using RAL.Repository;
using RAL.Repository.Model;

namespace RAL.Manager.Rules
{
    public class RuleActionWriteIsRunning : RuleActionBase<RuleIsRunningData>
    {
        private ILogger _logger;

        public string Description { get; private set; }

        private IMachineRepository _repository;

        public RuleActionWriteIsRunning(IMachineRepository machineRepository, string description = null, ILogger logger = null)
        {
            Description = description;
            _repository = machineRepository;
            _logger = logger;
        }



        public async override Task ExecuteAsync(bool IsRuleMet, bool RuleIsMetHasChanged, RuleIsRunningData Data)
        {
            if (RuleIsMetHasChanged)
            {
                    await Write(Data, IsRuleMet);                
            }
        }

        /*
        public override async Task EvaluatedAsFalse(RuleIsRunningData data)
        {
            await Write(data, false);
        }

        public override async Task EvaluatedAsTrue(RuleIsRunningData data)
        {
            await Write(data, true);
        }
        */

        private async Task Write(RuleIsRunningData data, bool isRunning)
        {
            try
            {
                MachineIsRunningInflux row = new MachineIsRunningInflux(data.Machine)
                {
                    Time = data.When,
                    IsRunning = isRunning
                };

                await _repository.MachineIsRunningRepo.WriteAsync(row);
            }
            catch (TaskCanceledException ex)
            {
                _logger?.Here(nameof(RuleActionWriteIsRunning), Description).Warning("Attempt to Write IsRunning was Canceled, Value not written.");
            }
        }

        public override Task InitializeAsync()
        {
            //** Do Nothing
            return Task.CompletedTask;
        }

    }
}
