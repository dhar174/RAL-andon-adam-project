using System;
using System.Diagnostics;
using System.Threading.Tasks;
using TheColonel2688.Utilities;
using Xunit;

namespace TheColonel1026.Tests
{
    public class TimeOutTests
    {

        [Fact]
        public async void TestTimeOutExceptionThrow()
        {
            //Stopwatch sw = Stopwatch.StartNew();
            var Thrown = false;
            try
            {
                await TimeOut.Execute(() => Task.Delay(5000), TimeSpan.FromMilliseconds(1000));
            }
            catch (OperationCanceledException)
            {
                Thrown = true;
            }
            //sw.Stop();

            //Assert.True(sw.Elapsed >= TimeSpan.FromMilliseconds(1000));

            Assert.True(Thrown);
        }

        [Fact]
        public async void TestTimeOutWithReturnExceptionThrown()
        {
            //Stopwatch sw = Stopwatch.StartNew();
            var Thrown = false;
            try
            {
                await TimeOut.Execute(async () => {
                    await Task.Delay(5000);
                    return true;
                    }, TimeSpan.FromMilliseconds(1000));
            }
            catch (OperationCanceledException)
            {
                Thrown = true;
            }
            //sw.Stop();

            //Assert.True(sw.Elapsed >= TimeSpan.FromMilliseconds(1000));

            Assert.True(Thrown);
        }



    }
}
