using RAL.Repository.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace RAL.Collector.Machine
{
    public interface IMachineForTopicBuilder : IMachineInfo
    {
        IList<string> SubscribableTopics { get; }
    }
}
