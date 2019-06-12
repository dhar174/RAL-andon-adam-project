using RAL.Repository.Model;
using Serilog;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TheColonel2688.Utilities;
using Vibrant.InfluxDB.Client;

namespace RAL.Repository
{
    public class MachineStatusRepositoryWithCache : HasLogger, IMachineStatusRepository
    {
        readonly MachineStatusRepository repository;

        NonReentrantTimer cacheWriteTimer = new NonReentrantTimer();

        private ListWithLock<MachineStatusInflux> measurementCache = new ListWithLock<MachineStatusInflux>();

        private ListWithLock<MachineStatusInflux> measurementsThatNeedToBeWrittenToDB = new ListWithLock<MachineStatusInflux>();

        private readonly int measurementCacheMaxItems = 5000;

        protected override string ClassTypeAsString => nameof(MachineStatusRepositoryWithCache);

        public MachineStatusRepositoryWithCache(InfluxClient client, string databaseName, TimeSpan cacheWriteInterval, ILogger logger = null) : base(logger)
        {
            repository = new MachineStatusRepository(client, databaseName);

            cacheWriteTimer = new NonReentrantTimer(cacheWriteInterval.TotalMilliseconds);
            cacheWriteTimer.Elapsed += CacheWriteTimer_Elapsed;
            cacheWriteTimer.Start();
        }

        private void TrimCacheIfNeeded()
        {
            if (measurementCache.Count > measurementCacheMaxItems + Convert.ToInt32(measurementCacheMaxItems * 0.20))
            {
                //measurementCache = new ListWithLock<MachineStatusInflux>();
                measurementCache.TrimToSize(measurementCacheMaxItems);
            }
        }

        private void CacheWriteTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            try
            {
                Collection<MachineStatusInflux> tempToWriteItems = measurementsThatNeedToBeWrittenToDB.Clone();

                //** Stores current measurementsThatNeedToBeWrittenToDB in a temp variable so that we know which ones we are trying to write, (More maybe added while we are processing)

                repository.WriteAsync(tempToWriteItems).GetAwaiter();

                //** if WriteAsync did not throw an exception we can assume the items were written to the database and we can remove them from measurementsThatNeedToBeWrittenToDB

                measurementsThatNeedToBeWrittenToDB.RemoveMany(tempToWriteItems);

            }
            catch (Exception ex)
            {
                _logger()?.Error(ex,"Issue While Handling cache write");
            }
            finally
            {
                sw.Stop();
                _logger()?.Debug("This Method took {elapsed}", sw.Elapsed);
            }
            
        }

        public async Task<MachineStatusInflux> LastOrDefaultAsync(string line, string name)
        {
            Stopwatch sw = Stopwatch.StartNew();
            var result = await Task.Run(() => measurementCache.LastOrDefault(x => x.Line == line && x.Name == name));
            sw.Stop();

            if (sw.ElapsedMilliseconds > 1100)
            {
                _logger()?.Warning("{Action} Took {elapsed} of {limit}", "Cache Query", sw.Elapsed, 1100);
            }
            _logger()?.Debug("{Action} Took {elapsed}", "Cache Query", sw.Elapsed);

            if (!(result is null))
            {
                return result;
            }
            sw.Restart();
            result = await repository.LastOrDefaultAsync(line, name);
            sw.Stop();
            if (sw.ElapsedMilliseconds > 1100)
            {
                _logger()?.Warning("{method} Took {elapsed} of {limit}", nameof(repository.LastOrDefaultAsync), sw.Elapsed, 1100);
            }
            _logger()?.Debug("{Action} Took {elapsed}", nameof(repository.LastOrDefaultAsync), sw.Elapsed);

            if (!(result is null))
            {
                measurementCache.Add(result);
            }

            return result;
        }

        public async Task<MachineStatusInflux> LastOrDefaultWhereIsAsync(string line, string name, bool? isCycling = null, bool? isInAutomatic = null, bool? isFaulted = null)
        {
            var whatWasPassed = (isCycling, isInAutomatic, isFaulted);

            MachineStatusInflux result;

            Func<MachineStatusInflux, bool> machinepredicate = x => (x.Line == line && x.Name == name);

            Func<MachineStatusInflux, bool> predicate;

            switch (whatWasPassed)
            {
                case var a when a.isCycling.HasValue && a.isInAutomatic.HasValue && a.isFaulted.HasValue:
                    predicate = x => machinepredicate(x) && x.IsCycling == a.isCycling.Value && x.IsInAutomatic == a.isInAutomatic.Value && x.IsFaulted == a.isFaulted.Value;
                    break;
                case var a when a.isCycling.HasValue && a.isInAutomatic.HasValue:
                    predicate = x => machinepredicate(x) && x.IsCycling == a.isCycling.Value && x.IsInAutomatic == a.isInAutomatic.Value;
                    break;
                case var a when a.isCycling.HasValue && a.isFaulted.HasValue:
                    predicate = x => machinepredicate(x) && x.IsCycling == a.isCycling.Value && x.IsFaulted == a.isFaulted.Value;
                    break;
                //** TODO Implement more Possibilities
                default:
                    throw new ArgumentNullException($"{nameof(isCycling)},{nameof(isInAutomatic)}, and {nameof(isFaulted)}", "One of the 3 Options Must be Used");
            }

            result = await Task.Run(() => measurementCache.LastOrDefault(predicate));

            if (!(result is null))
            {
                return result;
            }

            _logger()?.Debug("No Cache Entry for machine {Line}.{Name} where {isCyclingName} is {isCycling} && " +
                "{isInAutomaticName} is {isInAutomatic} && {isFaultedName} is {isFaulted} ", line, name, nameof(isCycling), isCycling.Value,
                nameof(isInAutomatic), isInAutomatic.Value, nameof(isFaulted), isFaulted.Value);

            result = await repository.LastOrDefaultWhereIsAsync(line, name, isCycling, isInAutomatic, isFaulted);

            if (!(result is null))
            {
                measurementCache.Add(result);
            }

            return result;

        }



        public Task WriteAsync(MachineStatusInflux data)
        {            
            measurementsThatNeedToBeWrittenToDB.Add(data);

            measurementCache.Add(data);

            try
            {
                TrimCacheIfNeeded();
            }
            catch (Exception ex)
            {

            }

            return Task.CompletedTask;
        }


    }
}
