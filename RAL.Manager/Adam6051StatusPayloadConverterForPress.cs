using Newtonsoft.Json;
using RAL.Collector;

namespace RAL.Devices.Adam
{

    public class Adam6051StatusPayloadConverterForPress : IMachineStatusPayloadConverter
    {

        public MachineStatusMessage Convert(string payload)
        {
            var newPressStatusData = new MachineStatusMessage() { };

            Adam6051DataPayloadRaw adamData = JsonConvert.DeserializeObject<Adam6051DataPayloadRaw>(payload);

            //** ATTENTION This is Inverting
            newPressStatusData.IsCycling = !adamData.di1;
            newPressStatusData.IsInAutomatic = !adamData.di2;
            newPressStatusData.IsFaulted = !adamData.di4;

            return newPressStatusData;
        }

    }
}
