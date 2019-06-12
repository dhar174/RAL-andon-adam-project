using System.Collections.Generic;
using System.Threading.Tasks;
using RAL.Repository.Model;
using System;
using Vibrant.InfluxDB.Client;

namespace RAL.Repository
{
    public class MachineStatusRepository : RepositoryAbstract<MachineStatusInflux>, IMachineStatusRepository
    {

        public MachineStatusRepository(InfluxClient client, string databaseName) : base (client, databaseName)
        {
            MeasurementName = "Status";
        }

        public override async Task<MachineStatusInflux> LastOrDefaultAsync(string line, string name)
        {

            string query = $"SELECT * FROM \"Status\" WHERE \"Line\" = '{line}' AND \"Name\" = '{name}' ";

            query = $"{query} ORDER BY time DESC LIMIT {1}";


            InfluxResultSet<MachineStatusInflux> resultsSet;
            try
            {
                 resultsSet = await _client.ReadAsync<MachineStatusInflux>(_databaseName, query);
            }
            catch (Exception ex)
            {
                throw new RepositoryConnectionException($"Could not ReadAsync", ex);
            }

            var result = resultsSet.Results[0];

            if (result.Series.Count < 1)
            {
                return null;
            }

            var serie = result.Series[0];

            if(serie.Rows.Count > 1)
            {
                //** TODO
                throw new Exception();
            }

            return serie.Rows[0];

        }

        public async Task<MachineStatusInflux> LastOrDefaultWhereIsAsync(string line, string name, bool? isCycling = null, bool? isInAutomatic = null, bool? isFaulted = null)
        {
            if(isCycling is null && isInAutomatic is null && isFaulted is null)
            {
                throw new ArgumentNullException($"{nameof(isCycling)},{nameof(isInAutomatic)}, and {nameof(isFaulted)}", "All three option arguments can't be null");
            }


            IList<MachineStatusInflux> machineStatuses = new List<MachineStatusInflux>();

            string query = $"SELECT * FROM \"{MeasurementName}\" WHERE \"Line\" = '{line}' AND \"Name\" = '{name}'";

            if(isCycling.HasValue)
            {
                query = $"{query} AND \"IsCycling\" = ";

                if (isCycling.Value)
                {
                    query = $"{query}True";
                }
                else
                {
                    query = $"{query}False";
                }
            }

            if (isInAutomatic.HasValue)
            {
                query = $"{query} AND \"IsInAutomatic\" = ";

                if (isInAutomatic.Value)
                {
                    query = $"{query}True";
                }
                else
                {
                    query = $"{query}False";
                }
            }

            if (isFaulted.HasValue)
            {
                query = $"{query} AND \"IsFaulted\" = ";

                if (isFaulted.Value)
                {
                    query = $"{query}True";
                }
                else
                {
                    query = $"{query}False";
                }
            }



            query = $"{query} ORDER BY time DESC LIMIT {1}";

            InfluxResultSet<MachineStatusInflux> resultsSet;
            try
            {
                resultsSet = await _client.ReadAsync<MachineStatusInflux>(_databaseName, query);
            }
            catch (Exception ex)
            {
                throw new RepositoryConnectionException("Could not ReadAsync", ex);
            }

            var result = resultsSet.Results[0];


            if (result.Series.Count > 1)
            {
                //** TODO Better Logging and Exceptions
                throw new Exception("Series count should be 1");
            }
            else if(result.Series.Count < 1)
            {
                return null;
            }

            var serie = result.Series[0];

            if (serie.Rows.Count > 1)
            {
                //** TODO Better Logging and Exceptions
                throw new Exception("Rows Count Should be > 1");
            }


            return serie.Rows[0];
        }

        public override async Task WriteAsync(IEnumerable<MachineStatusInflux> data)
        {
            try
            {
                await _client.WriteAsync(_databaseName, MeasurementName, data );
            }
            catch (Exception ex)
            {
                throw new RepositoryConnectionException("Could not WriteAsync", ex);
            }
        }

        public override async Task WriteAsync(MachineStatusInflux data)
        {
            try
            { 
                await _client.WriteAsync(_databaseName, MeasurementName, new[] { data });
            }
            catch (Exception ex)
            {
                throw new RepositoryConnectionException("Could not WriteAsync", ex);
            }
        }
    }
}
