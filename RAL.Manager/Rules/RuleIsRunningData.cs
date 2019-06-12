using System;

namespace RAL.Manager.Rules
{
    public class RuleIsRunningData
    {
        public DateTime When { get; set; }
        public Machine Machine {get; set;}
    }
}