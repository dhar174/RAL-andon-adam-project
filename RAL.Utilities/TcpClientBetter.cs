using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace TheColonel2688.Utilities
{
    public class TcpClientBetter : IDisposable, ITcpClientBetter
    {
        private TcpClient _client;

        //private int _defaultReceiveBufferSize = 2000;

        private TimeSpan _connectTimeout = TimeSpan.FromSeconds(2);

        //private TimeSpan _reconnectTimeout;

        private TimeSpan _sendReceiveTimeout = TimeSpan.FromSeconds(2);

        public string Hostname { get; private set; }

        public int Port { get; private set; }

        public bool Connected => _client is null ? false : _client.Connected;

        public TcpClientBetter(string hostname, int port, TimeSpan? connectTimeOut = null, TimeSpan? sendReceiveTimeOut = null)
        {
            if(!(connectTimeOut is null))
            {
                _connectTimeout = connectTimeOut.Value;
            }

            if (!(sendReceiveTimeOut is null))
            {
                _sendReceiveTimeout = sendReceiveTimeOut.Value;
                
            }

            Hostname = hostname;
            Port = port;

        }

        public async Task ReconnectAsync()
        {
            try
            {
                var blah = _client.Connected;
                await CloseAsync();
            }
            catch (ObjectDisposedException)
            {
                //** Ignore this is so we can
                //** only call close() if Client is not disposed
            }

            InstantiationTcpClient();

            await ConnectAsync();
        }

        private void InstantiationTcpClient()
        {
            _client = new TcpClient();
            _client.Client.Blocking = true;
            _client.LingerState.Enabled = true;
            _client.LingerState.LingerTime = 0;
            _client.Client.ReceiveTimeout = Convert.ToInt32(_sendReceiveTimeout.TotalMilliseconds);
            _client.Client.SendTimeout = Convert.ToInt32(_sendReceiveTimeout.TotalMilliseconds);
        }

        public async Task ConnectAsync()
        {
            using (var connectedWait = new ManualResetEvent(false))
            {
                IAsyncResult result = null;
                try
                {
                    InstantiationTcpClient();
                    await TimeOut.Execute(async () =>
                    {
                        //** if use ConnectAsync you cannot dispose of the client if it fails, because the await will return and throw a disposed object exception.
                        result = _client.BeginConnect(Hostname, Port, (blah) => connectedWait.Set(), null);
                        await connectedWait.WaitOneAsync();

                    }, _connectTimeout);

                    if (!_client.Connected)
                    {
                        //throw new InvalidOperationException();
                    }
                }
                catch (OperationCanceledException)
                {
                    /*if (!(result is null)){
                        try
                        {
                            _client.EndConnect(result);
                        }
                        catch (Exception ex)
                        {
                            //** Ignore it
                        }
                    }*/
                    _client.Close();
                    throw new TimeoutException("Timed out while trying to Connect");
                }
            }
        }


        public async Task CloseAsync()
        {
            await Task.Run(() => _client.Close());
        }

        private async Task FlushOSSocketReceiveBufferAsync(int flushSize = 2000)
        {
            await Task.Run(
                () => 
                {
                    var dump = new byte[1];
                    while (true)
                    {
                        byte[] info = new byte[2];
                        byte[] outval = new byte[flushSize];
                        _client.Client.IOControl(IOControlCode.DataToRead, info, outval);
                        uint bytesAvailable = BitConverter.ToUInt32(outval, 0);
                        if (bytesAvailable != 0)
                        {
                            int len = _client.Client.Receive(outval); //Flush buffer
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                );
        }

        public async Task SendAsync(byte[] sendBuffer, bool shouldFlushBuffer = true)
        {
            //if (shouldFlushBuffer)
            //{
            //    await FlushOSSocketReceiveBufferAsync();
            //}            

            try
            {
                //await TimeOut(() => _client.Client.Send(sendBuffer), _connectTimeout);
                await Task.Run(() => _client.Client.Send(sendBuffer));

            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException("Timed out while trying to Send");
            }

        }

        public async Task ReceiveAsync(byte[] tempBufferIn)
        {
            /*
            int bufferSize;
            if(receiveBufferSize == 0)
            {
                bufferSize = _defaultReceiveBufferSize;
            }
            else
            {
                bufferSize = receiveBufferSize;
            }
            
            byte[] tempBufferIn = new byte[bufferSize];
            */


            //await TimeOut(() => _client.Client.Receive(tempBufferIn), _sendReceiveTimeout);
            await Task.Run(() => _client.Client.Receive(tempBufferIn));

        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
