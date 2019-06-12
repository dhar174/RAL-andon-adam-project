using System;

namespace RAL.ConfigStorageTypes
{
    public class EmailReportDSC
    {
        private Type _typeOfReport;

        public Type TypeOfEmailReport
        {
            get { return _typeOfReport; }
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

                _typeOfReport = value;
            }
        }
        public string Name { get; set; }
        public string[] EmailAddress { get; set; }
        public string Department { get; set; }
        public (int Hours, int Minutes) ReportTimeSpanStartTime { get; set; }
        public (int Hours, int Minutes) ReportTimeSpanEndTime { get; set; }

    }
}