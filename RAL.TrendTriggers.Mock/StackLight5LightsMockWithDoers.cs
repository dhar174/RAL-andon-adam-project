using RAL.Devices.StackLights;
using System;
using System.Threading.Tasks;

namespace RAL.Devices.Mocks
{
    public class StackLight5LightsMockWithDoers : IStackLight5Light
    {
        public string IPAddress { get; private set; }

        public delegate bool IsLightDoer(StackLight5Lights.LightNumber number);

        public delegate void TurnLightDoer(StackLight5Lights.LightNumber number);

        //public delegate void IsLightOnDoer(StackLightRYGBWAbstract.LightNumber number);

        //public delegate void TurnLightOffDoer(StackLightRYGBWAbstract.LightNumber number);

        //public delegate void TurnLightOnDoer(StackLightRYGBWAbstract.LightNumber number);

        public IsLightDoer IsLightOffDoer;

        public IsLightDoer IsLightOnDoer;

        public TurnLightDoer TurnLightOffDoer;

        public TurnLightDoer TurnLightOnDoer;

        public void BeginConnect()
        {
            throw new NotImplementedException();
        }


        //public IsLightOffDoer IsLightOffDoer;

        //public IsLightOnDoer IsLightOnDoer;

        //public TurnLightOffDoer TurnLightOffDoer;

        //public TurnLightOnDoer IsLightOffDoer;


        public bool IsLightOffAsync(StackLight5Lights.LightNumber number)
        {
            return IsLightOffDoer.Invoke(number);
        }

        public bool IsLightOnAsync(StackLight5Lights.LightNumber number)
        {
            return IsLightOnDoer.Invoke(number);
        }

        public void TurnLightOffAsync(StackLight5Lights.LightNumber number)
        {
            TurnLightOffDoer(number);
        }

        public void TurnLightOnAsync(StackLight5Lights.LightNumber number)
        {
            TurnLightOnDoer(number);
        }

        Task<bool> IStackLight5Light.IsLightOffAsync(StackLight5Lights.LightNumber number)
        {
            throw new NotImplementedException();
        }

        Task<bool> IStackLight5Light.IsLightOnAsync(StackLight5Lights.LightNumber number)
        {
            throw new NotImplementedException();
        }

        Task IStackLight5Light.TurnLightOffAsync(StackLight5Lights.LightNumber number)
        {
            throw new NotImplementedException();
        }

        Task IStackLight5Light.TurnLightOnAsync(StackLight5Lights.LightNumber number)
        {
            throw new NotImplementedException();
        }

        public Task CloseAsync()
        {
            throw new NotImplementedException();
        }

        public Task ConnectAsync()
        {
            throw new NotImplementedException();
        }

        public void BeingClose()
        {
            throw new NotImplementedException();
        }
    }
}
