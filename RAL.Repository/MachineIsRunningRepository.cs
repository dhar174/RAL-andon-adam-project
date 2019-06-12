using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using Vibrant.InfluxDB.Client;
using RAL.Repository.Model;
using Newtonsoft.Json;
using System.Text;

namespace RAL.Repository
{
    public class MachineIsRunningRepository : RepositoryAbstract<MachineIsRunningInflux>, IMachineIsRunningRepository
    {

        public MachineIsRunningRepository(InfluxClient client, string databaseName) : base(client, databaseName)
        {
            MeasurementName = "IsRunning";
        }


        public async Task<IList<MachineIsRunningInflux>> LastNOrDefaultBeforeAsync(int number, string line, string name, DateTime before)
        {
            string query = InfluxDataHelper.BuildQueryLimitDesc(number, MeasurementName, line, name, before.ToUniversalTime());
            InfluxResultSet<MachineIsRunningInflux> resultSet;

            try
            {
                resultSet = await _client.ReadAsync<MachineIsRunningInflux>(_databaseName, query);

            }
            catch (Exception ex)
            {
                throw new RepositoryConnectionException("Could not ReadAsync", ex);
            }

            if (resultSet is null)
            {
                return null;
            }

            var result = resultSet.Results[0];

            if (result.Series.Count < 1)
            {
                return null;
            }

            var serie = result.Series[0];

            return serie.Rows;
        }


        public async Task<IList<MachineIsRunningInflux>> LastNOrDefaultAsync(int number, string line, string name)
        {
            string query = InfluxDataHelper.BuildQueryLimitDesc(number, MeasurementName, line, name);
            InfluxResultSet<MachineIsRunningInflux> resultSet;

            try
            {
                resultSet = await _client.ReadAsync<MachineIsRunningInflux>(_databaseName, query);

            }
            catch (Exception ex)
            {
                throw new RepositoryConnectionException("Could not ReadAsync", ex);
            }

            if (resultSet is null)
            {
                return null;
            }

            var result = resultSet.Results[0];

            if (result.Series.Count < 1)
            {
                return null;
            }

            var serie = result.Series[0];

            return serie.Rows;
        }

        private T ParseSerieKey<T>(string key)
        {

            try
            {
                var keySplit = key.Split(',');

                var builder = new StringBuilder();

                builder.Append("{");

                //** skip first item in split
                for (int i = 1; i < keySplit.Length; i++)
                {
                    var withBackSlashRemoved = keySplit[i].Replace("\\", "");
                    var variableAndValue = withBackSlashRemoved.Split('=');
                    builder.Append("\"");
                    builder.Append(variableAndValue[0]);
                    builder.Append("\"");
                    builder.Append(":");
                    builder.Append("\"");
                    builder.Append(variableAndValue[1]);
                    builder.Append("\"");
                    if (i + 1 < keySplit.Length)
                    {
                        builder.Append(",");
                    }
                }

                builder.Append("}");

                string serializedTags = builder.ToString();

                var deserializedTags = JsonConvert.DeserializeObject<T>(serializedTags);

                return deserializedTags;
            }
            catch (Exception ex)
            {
                throw new FormatException("Error parsing Serie Key. See inner exception for details", ex);
            }
        }

        public async Task<IList<MachineInfoInflux>> GetAllMachinesWithRecordsAsync(string department)
        {
            List<MachineInfoInflux> machines = new List<MachineInfoInflux>();

            var results = await _client.ShowSeriesAsync(_databaseName, MeasurementName);

            var result = results.Series[0];
            
            foreach (Vibrant.InfluxDB.Client.Rows.ShowSeriesRow row in result.Rows)
            {
                machines.Add(ParseSerieKey<MachineInfoInflux>(row.Key));        
            }
            
            return machines;
        }

        
        public async Task<IList<MachineIsRunningInflux>> GetLastForManyAsync((string Line, string Name)[] machines)
        {
            var results = new List<MachineIsRunningInflux>();

            var tasks = new List<Task<MachineIsRunningInflux>>();

            foreach(var machine in machines)
            {
                tasks.Add(LastOrDefaultAsync(machine.Line, machine.Name));
            }

            while (tasks.Count > 0)
            {
                Task<MachineIsRunningInflux> task = await Task.WhenAny(tasks);

                tasks.Remove(task);

                results.Add(await task);
            }

            return results;
        }
        

        public async Task<IList<MachineIsRunningInflux>> GetDataForTimeRangeReportAsync(DateTime start, DateTime end, string department)
        {
            string query = $"SELECT * FROM \"{MeasurementName}\" WHERE \"{Tag.Name.Department}\" = '{department}' AND time >= '{start.ToUniversalTime():yyyy-MM-ddTHH:mm:ss.ffffZ}' AND time <= '{end.ToUniversalTime():yyyy-MM-ddTHH:mm:ss.ffffZ}'";



            InfluxResult<MachineIsRunningInflux> result;

            try
            {
                result = (await _client.ReadAsync<MachineIsRunningInflux>(_databaseName, query)).Results[0];
            }
            catch (Exception ex)
            {
                throw new RepositoryConnectionException("Could not WriteAsync", ex);
            }

            if (result.Series.Count < 1)
            {
                return null;
            }

            var serie = result.Series[0];

            return serie.Rows;
        }

        public override async Task WriteAsync(MachineIsRunningInflux isRunning)
        {
            try
            { 
                await _client.WriteAsync(_databaseName, MeasurementName, new[] { isRunning });
            }
            catch (Exception ex)
            {
                throw new RepositoryConnectionException("Could not WriteAsync", ex);
            }
        }

        public override async Task WriteAsync(IEnumerable<MachineIsRunningInflux> isRunnings)
        {
            try
            { 
                await _client.WriteAsync(_databaseName, MeasurementName, isRunnings);
            }
            catch (Exception ex)
            {
                throw new RepositoryConnectionException("Could not WriteAsync", ex);
            }
        }

        public async Task<MachineIsRunningInflux> LastOrDefaultBeforeAsync(string line, string name, DateTime before)
        {
            var results = await LastNOrDefaultBeforeAsync(1, line, name, before);

            if(results is null || results.Count() < 1)
            {
                return null;
            }

            return results[0];
        }


        public override async Task<MachineIsRunningInflux> LastOrDefaultAsync(string line, string name)
        {
            var results = await LastNOrDefaultAsync(1, line, name);

            if (results is null || results.Count() < 1)
            {
                return null;
            }

            return results[0];
        }
    }
}
