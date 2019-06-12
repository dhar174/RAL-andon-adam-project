using System;

namespace RAL.Manager.Configuration
{
    public enum Shift { First, Second, Third }

    public class EmailReportConfig
    {
        

        public string Department { get; private set; }
        public string[] ToEmailAddressesForHourly { get; private set; }
        public string[] ToEmailAddressesForShiftly { get; private set; }
        public Shift Shift {get; private set; }


        public EmailReportConfig(string department, Shift shift, string[] toEmailAddressesForHourly, string[] toEmailAddressesForShiftly)
        {
            Department = department ?? throw new ArgumentNullException(nameof(department));
            Shift = shift;
            ToEmailAddressesForHourly = toEmailAddressesForHourly ?? throw new ArgumentNullException(nameof(toEmailAddressesForHourly));
            ToEmailAddressesForShiftly = toEmailAddressesForShiftly ?? throw new ArgumentNullException(nameof(toEmailAddressesForShiftly));
        }
    }
}