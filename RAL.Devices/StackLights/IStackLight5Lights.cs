using System.Threading.Tasks;

namespace RAL.Devices.StackLights
{
    public interface IStackLight5Light
    {
        string IPAddress { get; }

        Task<bool> IsLightOffAsync(StackLight5Lights.LightNumber number);
        Task<bool> IsLightOnAsync(StackLight5Lights.LightNumber number);
        Task TurnLightOffAsync(StackLight5Lights.LightNumber number);
        Task TurnLightOnAsync(StackLight5Lights.LightNumber number);

        Task CloseAsync();

        Task ConnectAsync();

        void BeginConnect();

        void BeingClose();
    }
}