using RAL.Repository.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Vibrant.InfluxDB.Client;

namespace RAL.Repository
{
    public class MachineIsConnectedRepo : RepositoryAbstract<MachineIsConnectedInflux> //IRepository<MachineIsConnectedInflux>
    {


        public MachineIsConnectedRepo(InfluxClient client, string databaseName) : base(client, databaseName)
        {

            MeasurementName = "IsConnected";
        }

        public override Task<MachineIsConnectedInflux> LastOrDefaultAsync(string line, string name)
        {
            throw new NotImplementedException();
        }

        public override Task WriteAsync(MachineIsConnectedInflux row)
        {

            return _client.WriteAsync(_databaseName, MeasurementName, new[] { row } );
        }

        public override Task WriteAsync(IEnumerable<MachineIsConnectedInflux> rows)
        {
            return _client.WriteAsync(_databaseName, MeasurementName, rows);
        }
    }
}
