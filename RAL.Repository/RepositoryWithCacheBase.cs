using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using TheColonel2688.Utilities;
using Vibrant.InfluxDB.Client;

namespace RAL.Repository
{
    public abstract class RepositoryWithCacheBase<T, R> : HasLogger where R : IRepository<T>
    {
        protected readonly R _repository;

        private NonReentrantTimer cacheWriteTimer = new NonReentrantTimer();

        protected ListWithLock<T> measurementCache = new ListWithLock<T>();

        protected ListWithLock<T> measurementsThatNeedToBeWrittenToDB = new ListWithLock<T>();

        private readonly int measurementCacheMaxItems = 5000;

        protected override string ClassTypeAsString => nameof(MachineStatusRepositoryWithCache);

        public RepositoryWithCacheBase(R repository,TimeSpan cacheWriteInterval, ILogger logger = null) : base(logger)
        {
            _repository = repository;

            cacheWriteTimer = new NonReentrantTimer(cacheWriteInterval.TotalMilliseconds);
            cacheWriteTimer.Elapsed += CacheWriteTimer_Elapsed;
            cacheWriteTimer.Start();
        }

        private void TrimCacheIfNeeded()
        {
            if (measurementCache.Count > measurementCacheMaxItems + Convert.ToInt32(measurementCacheMaxItems * 0.20))
            {
                measurementCache.TrimToSize(measurementCacheMaxItems);
            }
        }

        public void Close()
        {
            cacheWriteTimer.Close();
        }

        private void CacheWriteTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            try
            {
                Collection<T> tempToWriteItems = measurementsThatNeedToBeWrittenToDB.Clone();

                //** Stores current measurementsThatNeedToBeWrittenToDB in a temp variable so that we know which ones we are trying to write, (More maybe added while we are processing)

                _repository.WriteAsync(tempToWriteItems).GetAwaiter();

                //** if WriteAsync did not throw an exception we can assume the items were written to the database and we can remove them from measurementsThatNeedToBeWrittenToDB

                measurementsThatNeedToBeWrittenToDB.RemoveMany(tempToWriteItems);

            }
            catch (Exception ex)
            {
                _logger()?.Error(ex, "Issue While Handling cache write");
            }
            finally
            {
                sw.Stop();
                _logger()?.Debug("This Method took {elapsed}", sw.Elapsed);
            }
        }

        public Task WriteAsync(IEnumerable<T> rows)
        {
            measurementsThatNeedToBeWrittenToDB.AddRange(rows);

            measurementCache.AddRange(rows);

            TrimCacheIfNeeded();

            return Task.CompletedTask;
        }


        public Task WriteAsync(T data)
        {
            measurementsThatNeedToBeWrittenToDB.Add(data);

            measurementCache.Add(data);

            TrimCacheIfNeeded();

            return Task.CompletedTask;
        }

    }   
}
