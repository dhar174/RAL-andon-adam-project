using System;
using System.Linq;
using Vibrant.InfluxDB.Client;

namespace RAL.Repository
{
    public abstract class MachineRepositoryBase : IMachineRepository
    {
        protected InfluxClient _client;

        public IMachineStatusRepository MachineStatusRepo { get; protected set; }

        public IMachineIsRunningRepository MachineIsRunningRepo { get; protected set; }

        public MachineIsConnectedRepo MachineIsConnectedRepo { get; protected set; }


        private string _ipaddress;
        private int _port;


        protected MachineRepositoryBase(string ipaddress, string username, string password, string databaseName, TimeSpan timeOut, int port = 8086)
        {
            _ipaddress = ipaddress;
            _port = port;
            var uri = new Uri($"http://{ipaddress}:{port}");
            _client = new InfluxClient(uri, username, password);
            _client.Timeout = timeOut;
        }

        public void ThrowIfCantConnect()
        {
            if (CanConnect() == false)
            {
                throw new RepositoryConnectionException($"Can not connect to Database at {_ipaddress}:{_port}");
            }
        }

        public bool CanConnect()
        {
            try
            {
                var result = _client.PingAsync().Result;
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public void CreateDB(string v)
        {
            var result = _client.CreateDatabaseAsync(v);
            result.Wait();

            if (result.Result.Succeeded != true)
            {
                //** TODO better Exception
                throw new Exception();
            }

        }

        public bool DoesDBExist(string databaseName)
        {
            var results = _client.ShowDatabasesAsync();
            results.Wait();

            var databases = results.Result.Series[0].Rows;

            return databases.Any(x => x.Name == databaseName);
        }

        public void DropDB(string v)
        {
            var result = _client.DropDatabaseAsync(v);
            result.Wait();

            if (result.Result.Succeeded != true)
            {
                //** TODO better Exception
                throw new Exception();
            }
        }
    }
}
