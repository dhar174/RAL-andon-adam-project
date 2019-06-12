using System;
using RAL.RulesEngine;
using RAL.rules.StackLights;

namespace RAL.ConfigStorageTypes
{
    public class RuleIntervalDSC
    {
        public TimeSpan pollTime { get; set; }

        public TimeSpan AllowedCycleTime { get; set; }

        public (string IP, string port) StackLightUri { get; set; }

        public StackLight5Lights.LightNumber LightNumber;

        private Type _typeOfRuleInterval;
        public Type TypeOfRuleInterval
        {
            get { return _typeOfRuleInterval; }
            set {

                /*
                 * 
                //** TODO Needs fixed
                if (value.GetType().IsSubclassOf(typeof(RuleInterval)))
                {
                    _typeOfRuleInterval = value.GetType();
                }
                else
                {
                    throw new ArgumentOutOfRangeException($"{nameof(TypeOfRuleInterval)} must be a subclass of {nameof(RuleInterval)}");
                }
                */
                _typeOfRuleInterval = value;
            }
        }
    }
}