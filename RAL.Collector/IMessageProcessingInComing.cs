namespace RAL.Collector
{
    public interface IMessageProcessorInComing
    {
        void ClientConnected(string ClientID);
        void ClientDisconnected(string ClientID);
        void AddMessage(MQTTMessage message);
        void Start();
    }
}