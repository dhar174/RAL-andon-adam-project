using RAL.Devices.StackLights;
using RAL.Rules.Core;
using System.Threading.Tasks;

namespace RAL.Manager.Rules
{
    public class RuleActionStackLightOnWhenIsRunningFalse : RuleActionBase<RuleIsRunningData>
    {
        public IStackLight5Light StackLight { get; private set; }

        public StackLight5Lights.LightNumber LightNumber { get; private set; }

        public RuleActionStackLightOnWhenIsRunningFalse(IStackLight5Light stackLight, StackLight5Lights.LightNumber lightNumber)
        {
            StackLight = stackLight;
            LightNumber = lightNumber;
        }

        public override Task ExecuteAsync(bool IsRuleMet, bool RuleIsMetHasChanged, RuleIsRunningData Data)
        {
            if (RuleIsMetHasChanged)
            {
                if (IsRuleMet)
                {
                    return StackLight.TurnLightOffAsync(LightNumber);
                }
                else
                {
                    return StackLight.TurnLightOnAsync(LightNumber);
                }
            }

            return Task.CompletedTask;
        }

        /*

        public override Task EvaluatedAsFalse(RuleIsRunningData data)
        {
            return Task.Run(() => StackLight.TurnLightOn(LightNumber));
        }

        public override Task EvaluatedAsTrue(RuleIsRunningData data)
        {
            return Task.Run(() => StackLight.TurnLightOff(LightNumber));
        }
        */



        public override Task InitializeAsync()
        {
            StackLight.BeginConnect();

            return Task.CompletedTask;
        }


    }
}
