using System.Threading.Tasks;

namespace RAL.Devices.StackLights
{
    public abstract class StackLight5Lights : IStackLight5Light
    {
        public abstract string IPAddress { get; }

        public enum LightNumber { Light0, Light1, Light2, Light3, Light4 }

        public abstract Task<bool> IsLightOnAsync(LightNumber number);

        public abstract Task<bool> IsLightOffAsync(LightNumber number);

        public abstract Task TurnLightOnAsync(LightNumber number);

        public abstract Task TurnLightOffAsync(LightNumber number);

        public abstract void BeginConnect();

      
        public Task ConnectAsync()
        {
            throw new System.NotImplementedException();
        }

        public abstract void BeingClose();

        public Task CloseAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
