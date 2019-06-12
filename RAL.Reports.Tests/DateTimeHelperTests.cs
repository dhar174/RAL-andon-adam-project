using System;
using Xunit;
using RAL.Reports.Base;

namespace RAL.Reports.Tests
{
    public class DateTimeHelperTests
    {

        public static TheoryData<
            (
                ((int Hour, int Minute) AsTuple, DateTime AsDateTime) Start,
                ((int Hour, int Minute) AsTuple, DateTime AsDateTime) End,
                DateTime EndDate
            )> Data
        {
            get
            {
                var data = new TheoryData<(((int Hour, int Minute) AsTuple, DateTime AsDateTime) Start, ((int Hour, int Minute) AsTuple, DateTime AsDateTime) End, DateTime EndDate)>()
                {
                    (
                        (
                            (7,00), new DateTime(2019, 2, 16, 7, 0, 0)
                        ),
                        (
                            (8,00), new DateTime(2019, 2, 16, 8, 0, 0)
                        ),
                        new DateTime(2019, 2, 16)
                    ),
                    (
                        (
                            (7,26), new DateTime(2019, 2, 16, 7, 26, 0)
                        ),
                        (
                            (8,26), new DateTime(2019, 2, 16, 8, 26, 0)
                        ),
                        new DateTime(2019, 2, 16)
                    ),
                    (
                        (
                            (7,26), new DateTime(2019, 2, 16, 7, 26, 0)
                        ),
                        (
                            (8,27), new DateTime(2019, 2, 16, 8, 27, 0)
                        ),
                        new DateTime(2019, 2, 16)
                    ),
                    (
                        (
                            (23,26), new DateTime(2019, 2, 15, 23, 26, 0)
                        ),
                        (
                            (00,27), new DateTime(2019, 2, 16, 00, 27, 0)
                        ),
                        new DateTime(2019, 2, 16)
                    ),
                    (
                        (
                            (23,26), new DateTime(2019, 2, 28, 23, 26, 0)
                        ),
                        (
                            (00,27), new DateTime(2019, 3, 01, 00, 27, 0)
                        ),
                        new DateTime(2019, 3, 1)
                    ),
                    (
                        (
                            (23,26), new DateTime(2020, 12, 31, 23, 26, 0)
                        ),
                        (
                            (00,27), new DateTime(2021, 01, 01, 00, 27, 0)
                        ),
                        new DateTime(2021, 1, 1)
                    ),
                    (
                        (
                            (7,00), new DateTime(2019, 2, 15, 7, 00, 0)
                        ),
                        (
                            (7,00), new DateTime(2019, 2, 16, 7, 00, 0)
                        ),
                        new DateTime(2019, 2, 16)
                    ),
                    (
                        (
                            (7,15), new DateTime(2019, 2, 15, 7, 15, 0)
                        ),
                        (
                            (7,15), new DateTime(2019, 2, 16, 7, 15, 0)
                        ),
                        new DateTime(2019, 2, 16)
                    ),
                    (
                        (
                            (00,00), new DateTime(2019, 2, 15, 00, 00, 0)
                        ),
                        (
                            (00,00), new DateTime(2019, 2, 16, 00, 00, 0)
                        ),
                        new DateTime(2019, 2, 16)
                    ),

                };

                return data;
            }
        }


        [Theory]
        [MemberData(nameof(Data))]
        public void GetStartAndEndDateTimeTest((((int Hour, int Minute) AsTuple, DateTime AsDateTime) Start, ((int Hour, int Minute) AsTuple, DateTime AsDateTime) End, DateTime EndDate) testData)
        {
            var (startDateTime, endDateTime) = DateTimeHelper.GetStartAndEndDateTimeUsingEndDate(testData.Start.AsTuple, testData.End.AsTuple, testData.EndDate);

            Assert.Equal(testData.Start.AsDateTime, startDateTime);
            Assert.Equal(testData.End.AsDateTime, endDateTime);
        }

    }
}
