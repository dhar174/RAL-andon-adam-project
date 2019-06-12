namespace RAL.Collector
{
    public interface ITopicMessageHandlerForProcessor
    {
        string Topic { get; set; }

        void Process(string payload);
    }
}