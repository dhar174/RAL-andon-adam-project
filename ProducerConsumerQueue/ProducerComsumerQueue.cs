using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace RAL.Utilities
{
    public class ProducerConsumerQueue<T> : IProducerConsumerQueueProducer<T>, IProducerConsumerQueueConsumer<T>
    {
        BlockingCollection<T> theQueue = new BlockingCollection<T>();

        public void Add(T item)
        {
            theQueue.Add(item);
        }

        public T Take()
        {
            return theQueue.Take();
        }

    }
}
