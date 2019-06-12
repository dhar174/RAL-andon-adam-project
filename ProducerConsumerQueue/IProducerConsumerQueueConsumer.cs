namespace RAL.Utilities
{
    public interface IProducerConsumerQueueConsumer<T>
    {
        T Take();
    }
}