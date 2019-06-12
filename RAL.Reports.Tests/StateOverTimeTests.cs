using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace RAL.Reports.Tests
{
    public class StateOverTimeTests
    {

        [Fact]
        public void TimeSpanTest()
        {
            var blah = new StateOverTime<bool>() { Start = DateTime.MinValue, End = DateTime.MinValue.AddMilliseconds(0.001), State = false};

            Assert.Equal(TimeSpan.FromMilliseconds(0.001),blah.TimeSpan);
        }

        [Fact]
        public void StartTest()
        {
            var blah = new StateOverTime<bool>() { Start = DateTime.MinValue.AddHours(500), End = DateTime.MinValue.AddHours(500).AddMilliseconds(0.001), State = false };

            Assert.Equal(DateTime.MinValue.AddHours(500), blah.Start);
        }

        [Fact]
        public void EndTest()
        {
            var blah = new StateOverTime<bool>() { Start = DateTime.MinValue.AddHours(500), End = DateTime.MinValue.AddHours(500).AddMilliseconds(0.001), State = false };

            Assert.Equal(DateTime.MinValue.AddHours(500).AddMilliseconds(0.001), blah.End);
        }


    }
}
