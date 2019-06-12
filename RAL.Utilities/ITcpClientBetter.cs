using System.Threading.Tasks;

namespace TheColonel2688.Utilities
{
    public interface ITcpClientBetter
    {
        bool Connected { get; }
        string Hostname { get; }
        int Port { get; }

        Task CloseAsync();
        Task ConnectAsync();
        void Dispose();
        Task ReceiveAsync(byte[] tempBufferIn);
        Task SendAsync(byte[] sendBuffer, bool shouldFlushBuffer = true);
    }
}