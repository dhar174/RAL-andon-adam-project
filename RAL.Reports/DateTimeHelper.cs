using System;
using System.Collections.Generic;
using System.Text;

namespace RAL.Reports.Base
{
    public static class DateTimeHelper
    {
        public static (DateTime StartDateTime, DateTime endDateTime) GetStartAndEndDateTimeUsingEndDate((int Hour, int Minute) startTime, (int Hour, int Minute) endTime, DateTime endDate)
        {
            DateTime startDateTime;
            DateTime endDateTime;

            DateTime endDateTimeStripped = endDate.Date;

            if (startTime.Hour < endTime.Hour)
            {
                startDateTime = endDateTimeStripped.AddHours(startTime.Hour).AddMinutes(startTime.Minute);
                endDateTime = endDateTimeStripped.AddHours(endTime.Hour).AddMinutes(endTime.Minute);
            }
            else
            {
                startDateTime = endDateTimeStripped.AddDays(-1).AddHours(startTime.Hour).AddMinutes(startTime.Minute);
                endDateTime = endDateTimeStripped.AddHours(endTime.Hour).AddMinutes(endTime.Minute);
            }
            return (startDateTime, endDateTime);
        }
    }
}
