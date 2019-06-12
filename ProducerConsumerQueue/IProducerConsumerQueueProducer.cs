namespace RAL.Utilities
{
    public interface IProducerConsumerQueueProducer<T>
    {
        void Add(T item);
    }
}