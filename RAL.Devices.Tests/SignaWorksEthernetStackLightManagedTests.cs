using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using TheColonel2688.Utilities;
using Xunit;
using static RAL.Devices.StackLights.SignaworksEthernetStackLightUnmanaged;

namespace RAL.Devices.StackLights.Tests
{
    public class SignaWorksEthernetStackLightManagedTests
    {
        readonly string IPForTest = "172.16.28.151";


        [Fact]
        public async void ConnectAsyncTimeoutTest()
        {
            var client = new SignaworksEthernetStackLightManaged("192.168.254.254", TimeSpan.FromMilliseconds(1000));

            Stopwatch sw = Stopwatch.StartNew();
            var thrown = false;
            try
            {
                await client.ConnectAsync();
            }
            catch (Exception ex)
            {
                thrown = true;
            }

            sw.Stop();

            Assert.True(thrown);

            Assert.True(sw.Elapsed < TimeSpan.FromSeconds(2));

        }

        [Fact]
        public async void ConcurrentBeginConnectTest()
        {
            var client = new SignaworksEthernetStackLightManaged(IPForTest, TimeSpan.FromMilliseconds(1000));

            //List<Task> tasks = new List<Task>();

            for(int i = 0; i < 10; i++)
            {
                client.BeginConnect();
            }

            await client.WaitForConnectedAsync();

            Assert.True(client.IsConnected);

            await client.CloseAsync();

            Assert.False(client.IsConnected);

            //await Task.WhenAll(tasks.ToArray());

        }

        [Fact]
        public async void ConcurrentConnectAsyncTest()
        {
            var client = new SignaworksEthernetStackLightManaged(IPForTest, TimeSpan.FromMilliseconds(1000));

            List<Task> tasks = new List<Task>();

            for (int i = 0; i < 10; i++)
            {
                tasks.Add(client.ConnectAsync());
            }

            await Task.WhenAll(tasks.ToArray());
           
            await client.WaitForConnectedAsync();

            Assert.True(client.IsConnected);

            await client.CloseAsync();

            Assert.False(client.IsConnected);

            //await Task.WhenAll(tasks.ToArray());

        }

        [Fact]
        public async void TurnAllLightsOnAndOff()
        {
            var client = new SignaworksEthernetStackLightManaged(IPForTest, TimeSpan.FromMilliseconds(1000));
            await client.ConnectAsync();

            Assert.True(client.IsConnected);

            foreach (var color in EnumIterator.GetValues<LightColor>())
            {
                await client.TurnLightOnWithConfirmAsync(color);
            }

            foreach (var color in EnumIterator.GetValues<LightColor>())
            {
                Assert.True(await client.GetLightStateCurrentAsync(color) == LightState.On);
            }
            
            await Task.Delay(1000);

            foreach (var color in EnumIterator.GetValues<LightColor>())
            {
                await client.TurnLightOffWithConfirmAsync(color);
            }

            foreach (var color in EnumIterator.GetValues<LightColor>())
            {
                Assert.True(await client.GetLightStateCurrentAsync(color) == LightState.Off);
            }

            await client.CloseAsync();

            Assert.False(client.IsConnected);
        }


    }
}
