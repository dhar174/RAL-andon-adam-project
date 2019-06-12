using RAL.Repository.Model;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Vibrant.InfluxDB.Client;

namespace RAL.Repository
{
    public class MachineIsRunningRepositoryWithCache : RepositoryWithCacheBase<MachineIsRunningInflux, MachineIsRunningRepository>, IMachineIsRunningRepository
    {
        public MachineIsRunningRepositoryWithCache(InfluxClient client, string databaseName, TimeSpan cacheWriteInterval, ILogger logger = null)
            : base(new MachineIsRunningRepository(client, databaseName), cacheWriteInterval, logger)
        {

        }

        public Task<IList<MachineInfoInflux>> GetAllMachinesWithRecordsAsync(string department)
        {
            return _repository.GetAllMachinesWithRecordsAsync(department);
        }


        /// <summary>
        /// Get Data for Time Range Report
        /// </summary>
        /// <remarks>
        /// Always Calls to the DB
        /// </remarks>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="department"></param>
        /// <returns></returns>
        public async Task<IList<MachineIsRunningInflux>> GetDataForTimeRangeReportAsync(DateTime start, DateTime end, string department)
        {

            //var result = measurementCache.Where(x=> x.Time >= start && x.Time < end && x.Department == department);

            //if(!(result is null))
            //{
                //return result.ToList();
            //}

            return await _repository.GetDataForTimeRangeReportAsync(start,end,department);
        }

        public async Task<IList<MachineIsRunningInflux>> LastNOrDefaultAsync(int number, string line, string name)
        {
            Stopwatch sw = Stopwatch.StartNew();
            var resultFromCache = await Task.Run(() => measurementCache.Where(x => x.Line == line && x.Name == name).Reverse().Take(number).Reverse());
            if(!(resultFromCache is null) && resultFromCache.Count() == number)
            {
                return resultFromCache.ToList();
            }
            sw.Stop();

            if(sw.ElapsedMilliseconds > 1100)
            {
                _logger()?.Warning("{Action} Took {elapsed} of {limit}", "Cache Query", sw.Elapsed, 1100);
            }
            _logger()?.Debug("{Action} Took {elapsed}", "Cache Query", sw.Elapsed);


            sw.Restart();

            var resultFromDB  = await _repository.LastNOrDefaultAsync(number, line, name);

            sw.Stop();

            if (sw.ElapsedMilliseconds > 1100)
            {
                _logger()?.Warning("{method} Took {elapsed} of {limit}", nameof(_repository.LastNOrDefaultAsync), sw.Elapsed, 1100);
            }

            if (!(resultFromDB is null))
            {
                measurementCache.AddRange(resultFromDB);
            }

            return resultFromDB;

        }

        public async Task<IList<MachineIsRunningInflux>> LastNOrDefaultBeforeAsync(int number, string line, string name, DateTime before)
        {
            var resultFromCache = await Task.Run(() => measurementCache.Where(x => x.Line == line && x.Name == name && x.Time < before).Reverse().Take(number).Reverse());
            if (!(resultFromCache is null) && resultFromCache.Count() == number)
            {
                return resultFromCache.ToList();
            }

            var resultFromDB = await _repository.LastNOrDefaultBeforeAsync(number, line, name, before);

            if (!(resultFromDB is null))
            {
                measurementCache.AddRange(resultFromDB);
            }

            return resultFromDB;
        }

        public async Task<MachineIsRunningInflux> LastOrDefaultBeforeAsync(string line, string name, DateTime before)
        {
            var result = await LastNOrDefaultBeforeAsync(1, line, name, before);

            if(result is null)
            {
                return null;
            }
            return result.FirstOrDefault();
        }

        public async Task<MachineIsRunningInflux> LastOrDefaultAsync(string line, string name)
        {
            var result = await LastNOrDefaultAsync(1, line, name);

            if (result is null)
            {
                return null;
            }
            return result.FirstOrDefault();
        }
    }
}
