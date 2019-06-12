using TheColonel2688.Utilities;
using Serilog;
using System;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Devices.Core;
using Nito.AsyncEx;

namespace RAL.Devices.StackLights
{

    public class SignaworksEthernetStackLightUnmanaged
    {
        private enum _bufferIndex
        {
            Command = 0,
            SoundID = 1,
            RedLight = 2,
            YellowLight = 3,
            GreenLight = 4,
            BlueLight = 5,
            WhiteLight = 6,
            SoundCommand = 7,
            Spare0 = 8,
            Spare1 = 9
        }

        public enum LightState
        {
            Off = 0,
            On = 1,
            LightBlink = 2
        }

        public enum SoundState
        {
            SoundOff = 0,
            SoundOn = 1
        }

        public enum LightColor
        {
            Red = 2,
            Yellow = 3,
            Green = 4,
            Blue = 5,
            White = 6
        }



        private const int MaxLights = 5;
        private const byte WriteCommand = 0x57; // 87 W
        private const byte ReadCommand = 0x52;  // 82 R
        private const byte RequestAck = 0x41;   // 65 A
        private const int StatusToBufferOffset = 2;
        private const int MessageDwellInMs = 50;

        private const int TransmitBufferSize = 10;
        private const int MaximumPort = short.MaxValue;
        private TimeSpan _sendReceiveTimeout;
        private TimeSpan _connectTimeout;

        private TcpClientBetter _tcpClientBetter;


        private AsyncLock _mutex = new AsyncLock();

        private volatile byte[] _lightStatus = new byte[TransmitBufferSize-4];

        private bool _isDebug = false;

        private string _ipaddress;
        private int _port;
        private bool _tryingToConnect;
        private readonly ILogger _logger;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="connectTimeout">Default is 5000ms</param>
        /// <param name="sendReceiveTimeOut">Default is 2000ms</param>
        /// <param name="port"></param>
        /// <param name="logger">Default is none</param>
        public SignaworksEthernetStackLightUnmanaged(string ipAddress, TimeSpan? connectTimeout = null, TimeSpan? sendReceiveTimeOut = null, int port = 20000, ILogger logger = null)
        {
            _isDebug = logger.IsEnabled(Serilog.Events.LogEventLevel.Debug);

            _sendReceiveTimeout = sendReceiveTimeOut ?? TimeSpan.FromMilliseconds(2000);
            _connectTimeout = connectTimeout ?? TimeSpan.FromMilliseconds(5000);
            _ipaddress = ipAddress;
            _port = port;
            //** Assign injected Logger
            this._logger = logger;
            _tcpClientBetter = new TcpClientBetter(_ipaddress, _port, _connectTimeout, _sendReceiveTimeout);
        }

        public Task CloseAsync()
        {
            return _tcpClientBetter.CloseAsync();
        }


        public void Close()
        {
            _tcpClientBetter.CloseAsync().GetAwaiter().GetResult();
        }

        public bool Connected => _tcpClientBetter is null ? false : _tcpClientBetter.Connected;

        public async Task ConnectAsync()
        {
            using (await _mutex.LockAsync())
            {
                try
                {
                    _logger?.Here(nameof(SignaworksEthernetStackLightUnmanaged), ToString()).Debug("Attempting to Connect...");

                    await _tcpClientBetter.ConnectAsync();

                    _logger?.Here(nameof(SignaworksEthernetStackLightUnmanaged), ToString()).Information("Connected");
                }
                catch (TimeoutException ex)
                {
                    _logger?.Here(nameof(SignaworksEthernetStackLightUnmanaged), ToString()).Error("Could not Connect to Signaworks Stack Light at {ipaddress}:{port}", _ipaddress, _port);
                    throw new DeviceConnectionException($"Could not Connect to Signaworks Stack Light at {_ipaddress}:{_port}", ex);
                }
                catch (SocketException ex)
                {
                    _logger?.Here(nameof(SignaworksEthernetStackLightUnmanaged), ToString()).Error("Could not Connect to Signaworks Stack Light at {ipaddress}:{port}", _ipaddress, _port);
                    throw new DeviceConnectionException($"Could not Connect to Signaworks Stack Light at {_ipaddress}:{_port}", ex);
                }
                catch (Exception ex)
                {
                    _logger?.Here(nameof(SignaworksEthernetStackLightUnmanaged), ToString()).Error(ex, "Unknown Exception Encountered. Could not Connect to Signaworks Stack Light at {ipaddress}:{port}", _ipaddress, _port);
                    throw new Exception($"Unknown Exception: Could not Connect to Signaworks Stack Light at {_ipaddress}:{_port} see inner exception", ex);
                }

                try
                {
                    ResetStatus();
                    await SendBufferWithConfirmAsync(_lightStatus);
                    _logger?.Here(nameof(SignaworksEthernetStackLightUnmanaged), ToString()).Information("Light States Reset");
                }
                catch (SocketException ex)
                {
                    _logger?.Here(nameof(SignaworksEthernetStackLightUnmanaged), ToString()).Error("Could not send command to reset Stack Light at {ipaddress}:{port}", _ipaddress, _port);
                    throw new DeviceConnectionException($"Could not Connect to Signaworks Stack Light at {_ipaddress}:{_port}", ex);
                }
                catch (Exception ex)
                {
                    _logger?.Here(nameof(SignaworksEthernetStackLightUnmanaged), ToString()).Error(ex, "Unknown Exception Encountered. Could not send command to reset Signaworks Stack Light at {ipaddress}:{port}", _ipaddress, _port);
                    throw new Exception($"Unknown Exception: Could not Connect to Signaworks Stack Light at {_ipaddress}:{_port} see inner exception", ex);
                }
            }
        }

        public void Connect()
        {
            ConnectAsync().GetAwaiter().GetResult();
        }

        public async Task ReconnectAsync()
        {
            if (_tryingToConnect == true)
            {
                _logger?.Here(nameof(SignaworksEthernetStackLightUnmanaged)).Fatal($"Called while already running");
                throw new InvalidOperationException($"{nameof(Reconnect)} called while already running");
            }

            _tryingToConnect = true;

            try
            {
                await _tcpClientBetter.ReconnectAsync();
                _logger?.Here(nameof(SignaworksEthernetStackLightUnmanaged), ToString()).Information("Reconnected");
            }
            catch (TimeoutException ex)
            {
                _logger?.Here(nameof(SignaworksEthernetStackLightUnmanaged), ToString()).Error("Could not reconnect to Signaworks Stack Light at {ipaddress}:{port}", _ipaddress, _port);
                throw new DeviceConnectionException($"Could not reconnect to Signaworks Stack Light at {_ipaddress}:{_port}", ex);
            }
            catch (SocketException ex)
            {
                _logger?.Here(nameof(SignaworksEthernetStackLightUnmanaged), ToString()).Error("Could not reconnect to Signaworks Stack Light at {ipaddress}:{port}", _ipaddress, _port);
                throw new DeviceConnectionException($"Could not reconnect to Signaworks Stack Light at {_ipaddress}:{_port}", ex);
            }
            catch (Exception ex)
            {
                _logger?.Here(nameof(SignaworksEthernetStackLightUnmanaged), ToString()).Error(ex, "Unknown Exception Encountered. Could not Reconnect to Signaworks Stack Light at {ipaddress}:{port}", _ipaddress, _port);
                throw new Exception($"Unknown Exception: Could not reconnect to Signaworks Stack Light at {_ipaddress}:{_port} see inner exception", ex);
            }
            finally
            {
                _tryingToConnect = false;
            }

            try
            {
                ResetStatus();
                await SendBufferWithConfirmAsync(_lightStatus);
                _logger?.Here(nameof(SignaworksEthernetStackLightUnmanaged), ToString()).Information("Light States Reset");
            }
            catch (SocketException ex)
            {
                _logger?.Here(nameof(SignaworksEthernetStackLightUnmanaged), ToString()).Error(ex, "Could not Reset Light Status");
                throw new DeviceConnectionException($"Could not Reset Light Status", ex);
            }
        }

        /// <summary>
        /// Call Reconnect Synchronously
        /// </summary>
        public void Reconnect()
        {
            ReconnectAsync().GetAwaiter().GetResult();
        
        }

        public SoundState GetBuzzerStateCached()
        {
                return (SoundState)_lightStatus[(int)_bufferIndex.SoundCommand - StatusToBufferOffset];
        }
        
        public LightState GetLightStateCached(LightColor whichColor)
        {
            return (LightState)_lightStatus[(int)whichColor - StatusToBufferOffset];
        }

        /// <summary>
        /// Get the current Light Status from the Device. See returns for details
        /// </summary>
        /// <param name="whichColor"></param>
        /// <returns>If Connected, returns current State From the Device. If CommandsShouldBeIgnoredWhenNotConnected is Enabled and it is not connected this returns last known state.</returns>
        public async Task<LightState> GetLightStateCurrentAsync(LightColor whichColor)
        {
            await UpdateStatusFromLightAsync();

            return GetLightStateCached(whichColor);
        }        

        public async Task UpdateStatusFromLightAsync()
        {
            try
            {
                await _tcpClientBetter.SendAsync(new byte[TransmitBufferSize] { ReadCommand, 0, 0, 0, 0, 0, 0, 0, 0, 0});
            }
            catch (SocketException ex)
            {
                _logger?.Here(nameof(SignaworksEthernetStackLightUnmanaged), ToString()).Error(ex, "While Sending");
                throw;
            }

            var receiveBuffer = new byte[TransmitBufferSize];
            try
            {
                await _tcpClientBetter.ReceiveAsync(receiveBuffer);
            }
            catch (SocketException ex)
            {
                _logger?.Here(nameof(SignaworksEthernetStackLightUnmanaged), ToString()).Error(ex, "Receiving");

                throw;
            }

            Array.Copy(receiveBuffer, StatusToBufferOffset, _lightStatus, 0, _lightStatus.Length);
        }
       
        
        public async Task BlinkLightAsync(LightColor lightColor)
        {
            using (await _mutex.LockAsync())
            {
                var tempStatus = _lightStatus;
                tempStatus[(int)lightColor - StatusToBufferOffset] = (byte)LightState.LightBlink;

                await SendBufferWithConfirmAsync(tempStatus);

                _lightStatus = tempStatus;
            }
        }
        

        public async Task TurnLightOffWithConfirmAsync(LightColor lightColor)
        {
            using (await _mutex.LockAsync())
            {
                var tempStatus = _lightStatus;
                tempStatus[(int)lightColor - StatusToBufferOffset] = (byte)LightState.Off;
                try
                {
                    await SendBufferWithConfirmAsync(tempStatus);
                }
                catch (Exception ex)
                {
                    if (_isDebug) _logger?.Here(nameof(SignaworksEthernetStackLightUnmanaged), ToString()).Warning(ex,"Could Not Set {LightColor} OFF", lightColor);

                    throw;
                }

                _lightStatus = tempStatus;
            }
            _logger?.Here(nameof(SignaworksEthernetStackLightUnmanaged), ToString()).Debug("{LightColor} OFF", lightColor);
        }
        /*
        public async Task TurnLightOffAsync(LightColor lightColor)
        {
            using (await _mutex.LockAsync())
            {
                var tempStatus = _lightStatus;
                tempStatus[(int)lightColor - StatusToBufferOffset] = (byte)LightState.Off;
                await SendBufferAsync(tempStatus);

                _lightStatus = tempStatus;
            }
            
        }
        */
        public async Task TurnLightOnWithConfirmAsync(LightColor lightColor)
        {
            using (await _mutex.LockAsync())
            {
                var tempStatus = _lightStatus;
                tempStatus[(int)lightColor - StatusToBufferOffset] = (byte)LightState.On;
                await SendBufferWithConfirmAsync(tempStatus);

                _lightStatus = tempStatus;
            }
            _logger?.Here(nameof(SignaworksEthernetStackLightUnmanaged), ToString()).Debug("{LightColor} ON", lightColor);
        }
        /*
        public async Task TurnLightOnAsync(LightColor lightColor)
        {
            using (await _mutex.LockAsync())
            {
                var tempStatus = _lightStatus;
                tempStatus[(int)lightColor - StatusToBufferOffset] = (byte)LightState.On;
                await SendBufferAsync(tempStatus);

                _lightStatus = tempStatus;
            }
        }
        */
        private void ResetStatus()
        {
            //Set them all to 0
            for(int i = 0; i < _lightStatus.Length; i++)
            {
                _lightStatus[i] = 0;
            }
            
        }

        private async Task SendBufferWithConfirmAsync(byte[] buffer)
        {
            try
            {
                await SendBufferInternalAsync(buffer, false);

                var tempBufferIn = new byte[TransmitBufferSize];

                await _tcpClientBetter.ReceiveAsync(tempBufferIn);

                if (tempBufferIn[0] != RequestAck)
                {
                    throw new DeviceConnectionException($"{nameof(SendBufferWithConfirmAsync)} did not receive acknowledgment from Stack Light");
                }

                //_logger?.Verbose("Pausing After Send with confirm");
                await Task.Delay(MessageDwellInMs);
                //_logger?.Verbose("Continuing After Send with confirm");
            }
            catch (SocketException ex)
            {
                throw new DeviceConnectionException($"{nameof(SendBufferWithConfirmAsync)}Exception thrown, seen inner Exception", ex);
            }
        }

        private async Task SendBufferAsync(byte[] buffer)
        {
            await SendBufferInternalAsync(buffer, false);
        }

        /// <summary>
        /// Needs to be called from within a lock
        /// </summary>
        private async Task SendBufferInternalAsync(byte[] buffer, bool dwell = true)
        {
            try
            {
                var tempBufferOut = new byte[TransmitBufferSize];
                Array.Copy(buffer, 0, tempBufferOut, StatusToBufferOffset, buffer.Length);
                tempBufferOut[(int)_bufferIndex.Command] = WriteCommand;

                await _tcpClientBetter.SendAsync(tempBufferOut);
                if (dwell)
                {
                    //_logger?.Verbose("Pausing After Sending");
                    await Task.Delay(MessageDwellInMs);
                    //_logger?.Verbose("Continuing After Sending");
                }
            }
            catch (SocketException ex)
            {
                throw new DeviceConnectionException($"{nameof(SendBufferInternalAsync)} Exception thrown, see inner Exception", ex);
            }
        }

        private void ValidAddressAndPort(string ipAddress, int port)
        {
            if (!LooksLikeIP(ipAddress))
            {
                var ex = new ArgumentException($"IP address \"{ipAddress}\" is not a valid address");
                _logger?.Here(nameof(SignaworksEthernetStackLightUnmanaged), ToString()).Error(ex, "Invalid IP Address");
                throw ex;
            }

            if (port <= 0)
            {
                var ex = new ArgumentException($"Port number \"{port}\" is not a valid port ({port} <= 0)");
                _logger?.Here(nameof(SignaworksEthernetStackLightUnmanaged), ToString()).Error(ex, "Invalid Port");
                throw ex;                
            }

            if (port >= MaximumPort)
            {
                var ex = new ArgumentException($"Port number \"{port}\" is not a valid port ({port} > {MaximumPort})");
                _logger?.Here(nameof(SignaworksEthernetStackLightUnmanaged), ToString()).Error(ex, "Invalid Port");
                throw ex;
            }

            _ipaddress = ipAddress;

            _port = port;

            _logger?.Here(nameof(SignaworksEthernetStackLightUnmanaged), ToString()).Debug("IP and Port are valid");

        }

        public override string ToString()
        {
            return $"{IPAddress}:{Port}";
        }

        private static bool LooksLikeIP(string str)
        {     
            //** I didn't write this method....
            const string ipv4Regex = "((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])";
            return Regex.IsMatch(str, ipv4Regex);
        }

        public int Port
        {
            get
            {
                return _port;
            }
            
        }

        public string IPAddress
        {
            get
            {
                return _ipaddress;
            }

        }
    }
}