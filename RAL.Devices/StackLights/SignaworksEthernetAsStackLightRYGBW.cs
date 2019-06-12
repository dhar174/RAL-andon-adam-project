using Serilog;
using System;
using System.Threading.Tasks;
using static RAL.Devices.StackLights.SignaworksEthernetStackLightUnmanaged;
using static RAL.Devices.StackLights.StackLight5Lights;

namespace RAL.Devices.StackLights
{

    public class SignaworksEthernetAsStackLightRYGBW : StackLightRYGBWAbstract
    {
        private SignaworksEthernetStackLightManaged _light;

        public override string IPAddress { get => _light.IPAddress; }


        public SignaworksEthernetAsStackLightRYGBW(string IPAddress, TimeSpan? connectTimeout = null, TimeSpan? sendReceiveTimeout = null, ILogger logger = null)
        {
            _light = new SignaworksEthernetStackLightManaged(IPAddress, connectTimeout, sendReceiveTimeout, logger: logger);
        }

        public override void BeginConnect()
        {
            if (_light is null)
            {
                throw new InvalidOperationException($"{nameof(BeginConnect)} called before {nameof(_light)} was assigned.");
            }
            _light.BeginConnect();
        }

        public async override Task ConnectAsync()
        {
            //throw new NotImplementedException();
            if (_light is null)
            {
                throw new InvalidOperationException($"{nameof(BeginConnect)} called before {nameof(_light)} was assigned.");
            }
            await _light.ConnectAsync();
        }

        public override void BeingClose()
        {
            if (_light is null)
            {
                throw new InvalidOperationException($"{nameof(BeingClose)} called before {nameof(_light)} was assigned.");
            }
            _light.BeginClose();
        }

        public async override Task CloseAsync()
        {
            //throw new NotImplementedException();
            if (_light is null)
            {
                throw new InvalidOperationException($"{nameof(CloseAsync)} called before {nameof(_light)} was assigned.");
            }
            await _light.CloseAsync();
        }


        public override bool IsRedLightOn
        {
            get
            {
                return _light.GetLightStateCurrentAsync(LightColor.Red).GetAwaiter().GetResult() == LightState.On;
            }
            set
            {
                _light.TurnLightOnWithConfirmAsync(LightColor.Red).GetAwaiter().GetResult();
            }
        }

        public override bool IsRedLightOff
        {
            get
            {
                return _light.GetLightStateCurrentAsync(LightColor.Red).GetAwaiter().GetResult() == LightState.Off;
            }
            set
            {
                _light.TurnLightOffWithConfirmAsync(LightColor.Red).GetAwaiter().GetResult();
            }
        }

        public override bool IsYellowLightOn
        {
            get
            {
                return _light.GetLightStateCurrentAsync(LightColor.Yellow).GetAwaiter().GetResult() == LightState.On;
            }
            set
            {
                _light.TurnLightOnWithConfirmAsync(LightColor.Yellow).GetAwaiter().GetResult();
            }
        }

        public override bool IsYellowLightOff
        {
            get
            {
                return _light.GetLightStateCurrentAsync(LightColor.Yellow).GetAwaiter().GetResult() == LightState.Off;
            }
            set
            {
                _light.TurnLightOffWithConfirmAsync(LightColor.Yellow).GetAwaiter().GetResult();
            }
        }

        public override bool IsGreenLightOn
        {
            get
            {
                return _light.GetLightStateCurrentAsync(LightColor.Green).GetAwaiter().GetResult() == LightState.On;
            }
            set
            {
                _light.TurnLightOnWithConfirmAsync(LightColor.Green).GetAwaiter().GetResult();
            }
        }

        public override bool IsGreenLightOff
        {
            get
            {
                return _light.GetLightStateCurrentAsync(LightColor.Green).GetAwaiter().GetResult() == LightState.Off;
            }
            set
            {
                _light.TurnLightOffWithConfirmAsync(LightColor.Green).GetAwaiter().GetResult();
            }
        }

        public override bool IsBlueLightOn
        {
            get
            {
                return _light.GetLightStateCurrentAsync(LightColor.Blue).GetAwaiter().GetResult() == LightState.On;
            }
            set
            {
                _light.TurnLightOnWithConfirmAsync(LightColor.Blue).GetAwaiter().GetResult();
            }
        }

        public override bool IsBlueLightOff
        {
            get
            {
                return _light.GetLightStateCurrentAsync(LightColor.Blue).GetAwaiter().GetResult() == LightState.Off;
            }
            set
            {
                _light.TurnLightOffWithConfirmAsync(LightColor.Blue).GetAwaiter().GetResult();
            }
        }


        public override bool IsWhiteLightOn
        {
            get
            {
                return _light.GetLightStateCurrentAsync(LightColor.White).GetAwaiter().GetResult() == LightState.On;
            }
            set
            {
                _light.TurnLightOnWithConfirmAsync(LightColor.White).GetAwaiter().GetResult();
            }
        }

        public override bool IsWhiteLightOff
        {
            get
            {
                return _light.GetLightStateCurrentAsync(LightColor.White).GetAwaiter().GetResult() == LightState.Off;
            }
            set
            {
                _light.TurnLightOffWithConfirmAsync(LightColor.White).GetAwaiter().GetResult();
            }
        }

        

        private LightColor ConvertLightNumberToColor(LightNumber number)
        {
            switch (number)
            {
                case var n when (n == LightNumber.Light0):
                    return LightColor.Red;
                case var n when (n == LightNumber.Light1):
                    return LightColor.Yellow;
                case var n when (n == LightNumber.Light2):
                    return LightColor.Green;
                case var n when (n == LightNumber.Light3):
                    return LightColor.Blue;
                case var n when (n == LightNumber.Light4):
                    return LightColor.White;
                default:
                    //** TODO Better Logging and exceptions
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async override Task<bool> IsLightOffAsync(LightNumber number)
        {
            return await _light.GetLightStateCurrentAsync(ConvertLightNumberToColor(number)) == LightState.Off;
        }

        public async override Task<bool> IsLightOnAsync(LightNumber number)
        {
            return await _light.GetLightStateCurrentAsync(ConvertLightNumberToColor(number)) == LightState.On;        
        }

        public async override Task TurnLightOffAsync(LightNumber number)
        {
            await _light.TurnLightOffWithConfirmAsync(ConvertLightNumberToColor(number));
        }

        public async override Task TurnLightOnAsync(LightNumber number)
        {
             await _light.TurnLightOnWithConfirmAsync(ConvertLightNumberToColor(number));
        }

        
    }
}
