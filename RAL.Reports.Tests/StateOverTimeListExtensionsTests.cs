using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace RAL.Reports.Tests
{
    public class StateOverTimeListExtensionsTests
    {

        [Fact]
        public void TotalTimeWhere()
        {
            List<StateOverTime<bool>> listOfStates = new List<StateOverTime<bool>>()
            {
                new StateOverTime<bool>() { Start = DateTime.MinValue, End = DateTime.MinValue.AddHours(2), State = true },
                new StateOverTime<bool>() { Start = DateTime.MinValue.AddHours(2), End = DateTime.MinValue.AddHours(10), State = false }
            };



            Assert.Equal(TimeSpan.FromHours(2), listOfStates.TotalTimeWhere(x => x.State == true));
            Assert.Equal(TimeSpan.FromHours(8), listOfStates.TotalTimeWhere(x => x.State == false));
        }


        [Fact]
        public void PercentOfTotalWhere()
        {
            List<StateOverTime<bool>> listOfStates = new List<StateOverTime<bool>>()
            {
                new StateOverTime<bool>() { Start = DateTime.MinValue, End = DateTime.MinValue.AddHours(2), State = true },
                new StateOverTime<bool>() { Start = DateTime.MinValue.AddHours(2), End = DateTime.MinValue.AddHours(10), State = false }
            };

            Assert.Equal(0.20, listOfStates.PercentOfTotalWhere(x => x.State == true));
            Assert.Equal(0.80, listOfStates.PercentOfTotalWhere(x => x.State == false));
        }

        [Fact]
        public void Total()
        {
            List<StateOverTime<bool>> listOfStates = new List<StateOverTime<bool>>()
            {
                new StateOverTime<bool>() { Start = DateTime.MinValue, End = DateTime.MinValue.AddHours(2), State = true },
                new StateOverTime<bool>() { Start = DateTime.MinValue.AddHours(2), End = DateTime.MinValue.AddHours(10), State = false }
            };

            Assert.Equal(TimeSpan.FromHours(10), listOfStates.Total());
        }


    }
}
