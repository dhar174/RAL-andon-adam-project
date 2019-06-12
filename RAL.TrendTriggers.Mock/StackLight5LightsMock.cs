using RAL.Devices.StackLights;
using System;
using System.Threading.Tasks;

namespace RAL.Devices.Mocks
{
    public class StackLight5LightsMock : IStackLight5Light
    {
        public string IPAddress { get; set; }


        public bool IsLightOffAsync(StackLight5Lights.LightNumber number)
        {
            throw new NotImplementedException();
        }

        public bool IsLightOnAsync(StackLight5Lights.LightNumber number)
        {
            throw new NotImplementedException();
        }

        public void TurnLightOffAsync(StackLight5Lights.LightNumber number)
        {
            Console.WriteLine($"Stack Light - {IPAddress}, Light # {(int)number} turned off.");
        }

        public void TurnLightOnAsync(StackLight5Lights.LightNumber number)
        {
            Console.WriteLine($"Stack Light - {IPAddress}, Light # {(int)number} turned on.");
        }

        public void BeginConnect()
        {
            throw new NotImplementedException();
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
