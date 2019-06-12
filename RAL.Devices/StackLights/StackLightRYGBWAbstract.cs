using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static RAL.Devices.StackLights.StackLight5Lights;

namespace RAL.Devices.StackLights
{

    public abstract class StackLightRYGBWAbstract : IStackLightRYGBW
    {
        public abstract string IPAddress { get; }

        public abstract Task<bool> IsLightOnAsync(LightNumber number);

        public abstract Task<bool> IsLightOffAsync(LightNumber number);

        public abstract Task TurnLightOnAsync(LightNumber number);

        public abstract Task TurnLightOffAsync(LightNumber number);

        public abstract bool IsRedLightOn { get; set; }

        public abstract bool IsRedLightOff { get; set; }

        public abstract bool IsYellowLightOn { get; set; }

        public abstract bool IsYellowLightOff { get; set; }

        public abstract bool IsGreenLightOn { get; set; }

        public abstract bool IsGreenLightOff { get; set; }

        public abstract bool IsBlueLightOn { get; set; }

        public abstract bool IsBlueLightOff { get; set; }

        public abstract bool IsWhiteLightOn { get; set; }

        public abstract bool IsWhiteLightOff { get; set; }

        public abstract void BeginConnect();

        public abstract Task ConnectAsync();

        public abstract void BeingClose();

        public abstract Task CloseAsync();
    }
}
