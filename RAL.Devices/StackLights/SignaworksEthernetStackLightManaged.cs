using TheColonel2688.Utilities;
using Serilog;
using System;
using System.Runtime.CompilerServices;
using Trybot;
using static RAL.Devices.StackLights.SignaworksEthernetStackLightUnmanaged;
using Nito.AsyncEx;
using System.Threading.Tasks;
using Devices.Core;

namespace RAL.Devices.StackLights
{

    public class SignaworksEthernetStackLightManaged : ManagedDeviceBase
    {
        private SignaworksEthernetStackLightUnmanaged unmanaged;

        public override string Description => _description;

        protected override string ClassTypeAsString => nameof(SignaworksEthernetStackLightManaged);

        //private AsyncLock _mutex = new AsyncLock();
        private string _description;

        public SignaworksEthernetStackLightManaged(string ipaddress, TimeSpan? connectTimeout = null, TimeSpan? sendReceiveTimeout = null, int port = 20000, ILogger logger = null) : base(logger)
        {
            unmanaged = new SignaworksEthernetStackLightUnmanaged(ipaddress, connectTimeout, sendReceiveTimeout, port, logger);

            _description = $"Managed Client for {unmanaged.ToString()}";


        }

        protected override async Task DoConnectAsync()
        {
            await unmanaged.ConnectAsync();
        }

        protected override async Task DoReconnectAsync()
        {
            await unmanaged.ReconnectAsync();
        }

        protected override async Task DoCloseAsync()
        {
            await unmanaged.CloseAsync();
        }

        public SoundState GetBuzzerStateCached()
        {
            //ThrowExceptionIfNotConnected();

            //using (await _mutex.LockAsync())
            {
                return unmanaged.GetBuzzerStateCached();
            }
            
        }

        public LightState GetLightStateCached(LightColor whichColor)
        {
            //ThrowExceptionIfNotConnected();

            return unmanaged.GetLightStateCached(whichColor);
        }

        /// <summary>
        /// Get the current Light Status from the Device. See returns for details
        /// </summary>
        /// <param name="whichColor"></param>
        /// <returns></returns>
        public async Task<LightState> GetLightStateCurrentAsync(LightColor whichColor)
        {
            //ThrowExceptionIfNotConnected();
            return await RequestAsync(async () => await unmanaged.GetLightStateCurrentAsync(whichColor));
        }


        public async Task BlinkLightAsync(LightColor whichColor)
        {

            //ThrowExceptionIfNotConnected();

            //using (await _mutex.LockAsync())
            {
                await CommandAsync(async() => await unmanaged.BlinkLightAsync(whichColor));
            }
        }


        public async Task TurnLightOffWithConfirmAsync(LightColor whichColor)
        {
            //ThrowExceptionIfNotConnected();


            //using (await _mutex.LockAsync())
            {
                await CommandAsync(async ()  => await unmanaged.TurnLightOffWithConfirmAsync(whichColor));
            }


        }
        /*
        public async Task TurnLightOffAsync(LightColor whichColor)
        {
            //ThrowExceptionIfNotConnected();

            //using (await _mutex.LockAsync())
            {
                await CommandAsync(async () => await unmanaged.TurnLightOffAsync(whichColor));
                
            }

        }
        */

        public async Task TurnLightOnWithConfirmAsync(LightColor whichColor)
        {
            //ThrowExceptionIfNotConnected();

            //using (await _mutex.LockAsync())
            {
                await CommandAsync(async () => await unmanaged.TurnLightOnWithConfirmAsync(whichColor));
            }

        }
        /*
        public async Task TurnLightOnAsync(LightColor whichColor)
        {
            //ThrowExceptionIfNotConnected();

            //using (await _mutex.LockAsync())
            {
                await CommandAsync(async () => await unmanaged.TurnLightOnAsync(whichColor));                
            }
        }
        */
        public override string ToString()
        {
            return $"{IPAddress}:{Port}";
        }



        public int Port
        {
            get
            {
                return unmanaged.Port;
            }

        }

        public string IPAddress
        {
            get
            {
                return unmanaged.IPAddress;
            }

        }


    }
}