using TheColonel2688.Utilities;
using System.Collections.Generic;
using System.Threading;
using Xunit;
using static RAL.Devices.StackLights.SignaworksEthernetStackLightUnmanaged;
using System.Threading.Tasks;
using System;
using System.Diagnostics;

namespace RAL.Devices.StackLights.Tests
{
    public class SignaWorksEthernetStackLightTests
    {

        public const string StackLightIPForTest = "172.16.28.151";


        /*
        [Fact]
        public void CycleColorsOn1Second()
        {
            var pause = new ManualResetEvent(false);

            SignaworksEthernetStackLight light = new SignaworksEthernetStackLight(StackLightIPForTest, 20000);

            light.Connect();

            Assert.True(light.Connected);

            foreach (var color in EnumIterator.GetValues<LightColor>())
            {
                light.TurnLightOn(color);
                Assert.Equal(LightState.On, light.GetLightStateCurrent(color));
                pause.WaitOne(1000);
                light.TurnLightOff(color);
                Assert.Equal(LightState.Off, light.GetLightStateCurrent(color));
            }

            light.CloseConnection();

            Assert.False(light.Connected);
        }
        */

        /*
                //** I have no Idea what the point of this test was.........
                [Fact]
                public async void TurnAllColorsOnOff1SecondWithNoConfirmCheckCache()
                {
                    var pause = new ManualResetEvent(false);

                    SignaworksEthernetStackLightUnmanaged light = new SignaworksEthernetStackLightUnmanaged(StackLightIPForTest, 20000);

                    light.Connect();

                    Assert.True(light.Connected);

                    //light.TurnAllLightsOffWithConfirm();

                    foreach (var color in EnumIterator.GetValues<LightColor>())
                    {                
                        await light.TurnLightOnAsync(color);
                    }

                    var cachedLightStates = new List<(LightColor color, LightState state)>();

                    foreach (var color in EnumIterator.GetValues<LightColor>())
                    {
                        cachedLightStates.Add((color, light.GetLightStateCached(color)));
                    }

                    await light.UpdateStatusFromLightAsync();

                    foreach (var colorPair in cachedLightStates)
                    {
                        Assert.Equal(colorPair.state, await light.GetLightStateCurrentAsync(colorPair.color));
                    }

                    pause.WaitOne(1000);
                    pause.Reset();

                    foreach (var color in EnumIterator.GetValues<LightColor>())
                    {                
                        await light.TurnLightOffAsync(color);
                    }

                    cachedLightStates = new List<(LightColor, LightState)>();

                    foreach (var color in EnumIterator.GetValues<LightColor>())
                    {
                        cachedLightStates.Add((color, light.GetLightStateCached(color)));
                    }

                    await light.UpdateStatusFromLightAsync();

                    foreach (var colorPair in cachedLightStates)
                    {
                        Assert.Equal(colorPair.state, await light.GetLightStateCurrentAsync(colorPair.color));
                    }

                    light.CloseConnection();

                    Assert.False(light.Connected);
                }
                */
        /*
        [Fact]
        public void CycleColorsOn1SecondWithConfirm()
        {
            var pause = new ManualResetEvent(false);

            SignaworksEthernetStackLight light = new SignaworksEthernetStackLight(StackLightIPForTest, 20000);

            light.Connect();

            Assert.True(light.Connected);

            foreach (var color in EnumIterator.GetValues<LightColor>())
            {
                light.TurnLightOnWithConfirm(color);
                pause.WaitOne(1000);
                light.TurnLightOffWithConfirm(color);
            }
            light.CloseConnection();

            Assert.False(light.Connected);
        }
        */

        //** Need to add Assert that the Light has actually changed.
        [Fact]
        public async void TurnAllColorsOnOff1SecondWithConfirm()
        {

            SignaworksEthernetStackLightUnmanaged light = new SignaworksEthernetStackLightUnmanaged(StackLightIPForTest, port:20000);

            light.Connect();

            Assert.True(light.Connected);

            foreach (var color in EnumIterator.GetValues<LightColor>())
            {
                await light.TurnLightOnWithConfirmAsync(color);
            }

            await Task.Delay(1000);

            foreach (var color in EnumIterator.GetValues<LightColor>())
            {
                await light.TurnLightOffWithConfirmAsync(color);
            }

            light.Close();

            Assert.False(light.Connected);
        }


        [Theory]
        [InlineData(LightColor.Red)]
        [InlineData(LightColor.Yellow)]
        [InlineData(LightColor.Green)]
        [InlineData(LightColor.Blue)]
        [InlineData(LightColor.White)]
        public async void TurnOnSingleLightWithConfirm(LightColor color)
        {

            SignaworksEthernetStackLightUnmanaged light = new SignaworksEthernetStackLightUnmanaged(StackLightIPForTest, port:20000);

            light.Connect();

            Assert.True(light.Connected);

            await light.TurnLightOnWithConfirmAsync(color);

            Assert.Equal(LightState.On, await light.GetLightStateCurrentAsync(color));

            Task.Delay(1000).Wait();

            await light.TurnLightOffWithConfirmAsync(color);

            light.Close();

            Assert.False(light.Connected);
        }

        /*
        [Theory]
        [InlineData(LightColor.Red)]
        [InlineData(LightColor.Yellow)]
        [InlineData(LightColor.Green)]
        [InlineData(LightColor.Blue)]
        [InlineData(LightColor.White)]
        public async void TurnOnSingleLight(LightColor color)
        {

            SignaworksEthernetStackLightUnmanaged light = new SignaworksEthernetStackLightUnmanaged(StackLightIPForTest, 20000);

            light.Connect();

            Assert.True(light.Connected);

            await light.TurnLightOnAsync(color);

            Assert.Equal(LightState.On, await light.GetLightStateCurrentAsync(color));

            Task.Delay(1000).Wait();

            await light.TurnLightOffAsync(color);

            Assert.Equal(LightState.Off, await light.GetLightStateCurrentAsync(color));

            light.CloseConnection();

            Assert.False(light.Connected);
        }
        */
        /*
        [Fact]
        public void TurnOnRedWithConfirm()
        {
            var pause = new ManualResetEvent(false);

            SignaworksEthernetStackLight light = new SignaworksEthernetStackLight(StackLightIPForTest, 20000);

            light.Connect();

            Assert.True(light.Connected);

            light.TurnLightOnWithConfirm(LightColor.Red);

            pause.WaitOne(1000);

            light.TurnLightOffWithConfirm(LightColor.Red);

            light.CloseConnection();

            Assert.False(light.Connected);
        }

        [Fact]
        public void TurnOnYellowWithConfirm()
        {
            var pause = new ManualResetEvent(false);

            SignaworksEthernetStackLight light = new SignaworksEthernetStackLight(StackLightIPForTest, 20000);

            light.Connect();

            Assert.True(light.Connected);

            light.TurnLightOnWithConfirm(LightColor.Yellow);

            pause.WaitOne(1000);
            light.TurnLightOffWithConfirm(LightColor.Yellow);

            light.CloseConnection();

            Assert.False(light.Connected);
        }

        [Fact]
        public void TurnOnGreenWithConfirm()
        {
            var pause = new ManualResetEvent(false);

            SignaworksEthernetStackLight light = new SignaworksEthernetStackLight(StackLightIPForTest, 20000);

            light.Connect();

            Assert.True(light.Connected);

            light.TurnLightOnWithConfirm(LightColor.Green);

            pause.WaitOne(1000);

            light.TurnLightOffWithConfirm(LightColor.Green);

            light.CloseConnection();

            Assert.False(light.Connected);
        }


        [Fact]
        public void TurnOnBlueWithConfirm()
        {
            var pause = new ManualResetEvent(false);

            SignaworksEthernetStackLight light = new SignaworksEthernetStackLight(StackLightIPForTest, 20000);

            light.Connect();

            Assert.True(light.Connected);

            light.TurnLightOnWithConfirm(LightColor.Blue);

            pause.WaitOne(1000);

            light.TurnLightOffWithConfirm(LightColor.Blue);

            light.CloseConnection();

            Assert.False(light.Connected);
        }

        [Fact]
        public void TurnOnOffWhiteWithConfirm()
        {
            var pause = new ManualResetEvent(false);

            SignaworksEthernetStackLight light = new SignaworksEthernetStackLight(StackLightIPForTest, 20000);

            light.Connect();

            Assert.True(light.Connected);

            light.TurnLightOnWithConfirm(LightColor.White);

            pause.WaitOne(1000);

            light.TurnLightOffWithConfirm(LightColor.White);

            light.CloseConnection();

            Assert.False(light.Connected);
        }
    */
    }
}
