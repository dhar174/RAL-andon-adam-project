// Copyright (c) 2019 Ben Newman
using System;
using System.Collections.Concurrent;
using System.Threading;
using Serilog;

namespace TheColonel2688.Utilities
{
    public class ThreadHelper : HasLogger
    {
        private Thread _taskThread;

        protected volatile object _lock = false;

        private Timer threadWatchDog;

        public event EventHandler<UnhandledExceptionEventArgs> UnhandledException;

        //public CancellationToken CancellationToken => CTStopRequested;

        //private readonly ILogger _logger;

        ///<summary>
        /// This is the stop flag that signifies that the thread should end
        ///</summary>
        //public bool StopRequested { get; private set; }

        private CancellationTokenSource _ctsStopRequested;
        protected CancellationToken CTStopRequested { get; set; }

        //private ConcurrentQueue<Action> _taskQueue = new ConcurrentQueue<Action>();

        private BlockingCollection<Action> _taskCollection = new BlockingCollection<Action>();

        protected bool IsQueueEmpty
        {
            get
            {
                lock (_lock)
                {
                    return _taskCollection.Count > 0 ? true : false;
                }               
            }
        }

        protected override string ClassTypeAsString => nameof(ThreadHelper);

        /*protected void ClearQueue()
        {
            _taskQueue.Cl
        }
        */

        ///<summary>
        /// This signifies when the thread is about to close
        ///</summary>
        public event EventHandler<EventArgs> ThreadFinished;

        ///<summary>
        /// Thread Started is called on the Thread, before the consumer loop is started.
        ///</summary>
        protected event EventHandler<object> ThreadStarted;

        ///<summary>
        ///Triggered On an error
        ///</summary>
        public event EventHandler<ErrorEventArgs> ErrorEvent;

        /// <summary>
        /// Raise event, this is so it is accessible in a derived Class
        /// </summary>
        protected virtual void RaiseErrorEvent(ErrorEventArgs e)
        {
            _logger().Error("Error message that was raised was: {Message}", e.Message);
            ErrorEvent?.Invoke(this, e);
        }

        private TimeSpan? WatchDogTimerInterval;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="watchDogTimerInterval"> If null there is no time out</param>
        /// <param name="description"></param>
        /// <param name="logger">optional logger</param>
        public ThreadHelper(string description, TimeSpan? watchDogTimerInterval = null,  ILogger logger = null) : base (logger)
        {
            /*
            TimeSpan wdTimeSpan;
            if(watchDogTimerInterval is null)
            {
                wdTimeSpan = TimeSpan.FromSeconds(3);
            }
            else
            {
                wdTimeSpan = watchDogTimerInterval.Value;
            }

            WatchDogTimerInterval = wdTimeSpan;
            */

            WatchDogTimerInterval = watchDogTimerInterval;

            Description = description;
        }


        ///<summary>
        /// Start Thread
        ///</summary>
        protected void Initialize_Threading()
        {
            //ThreadIsDone = false;
            _ctsStopRequested = new CancellationTokenSource();
            CTStopRequested = _ctsStopRequested.Token;
            _taskThread = new Thread(TaskDoer) { Name = $"{Description}_TaskThread" };
            _taskThread.Start();
            //Logger?.Information("Thread {Description}: Thread has Started", Description);
        }


        public volatile bool Waiting = false;

        private void WatchDogElapsed(object s)
        {
            
            if(_taskThread.ThreadState != ThreadState.Running && _taskThread.ThreadState != ThreadState.WaitSleepJoin)
            {

                _logger()?.Error("Thread {ThreadName}: Thread is not running, current Thread State is {state}", _taskThread.Name, _taskThread.ThreadState);
            }
        }

        ///<summary>
        /// Messaging system that runs tasks on the logic thread.
        ///</summary>
        private void TaskDoer()
        {
            ThreadStarted?.Invoke(this,null);

            TimerCallback watchDogCallBack = new TimerCallback(WatchDogElapsed);
            if(!(WatchDogTimerInterval is null))
            {
                threadWatchDog = new Timer(watchDogCallBack, null, 0, Convert.ToInt32(WatchDogTimerInterval.Value.TotalMilliseconds));
            }

            while (!CTStopRequested.IsCancellationRequested)
            {
                try
                {
                    if (_taskCollection.IsCompleted)
                    {
                        Waiting = true;
                        _logger()?.Debug("Waiting for next action");
                    }

                    Action currentAction = _taskCollection.Take(); //** This is Thread Blocking
                    Waiting = false;

                    currentAction.Invoke();
                    _logger()?.Verbose("General Invoke Done");

                    if (_taskCollection.Count > 2)
                    {
                        _logger()?.Warning("Task Queue count has exceeded 2. The count is now at {count}", _taskCollection.Count);
                    }

                }
                catch (Exception ex)
                {
                    if(UnhandledException != null)
                    {
                        _logger()?.Error(ex,"Unhandled Exception Last Chance Catch. {NewLine} {Message}", Environment.NewLine, ex.Message);
                        if(ex.StackTrace is null)
                        {
                            _logger()?.Error("Stack Trace was null");
                        }

                        UnhandledException(this, new UnhandledExceptionEventArgs(ex, false));
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            _logger()?.Debug("Thread loop has been closed");
            threadWatchDog.Change(Timeout.Infinite, Timeout.Infinite);
            _logger()?.Debug("Watch dog timer stop requested.");
            //** trigger ThreadClosed Event
            ThreadFinished?.Invoke(this, null);
            
        }

        ///<summary>
        ///Starts the Consuming Thread and queue   
        ///</summary>
        ///<remarks>
        ///This runs on a Manager Thread
        ///</remarks>
        public virtual void Initialize()
        {
            if (_taskCollection.Count > 0)
            {
                var ex = new InvalidOperationException($"{Description}:{nameof(ThreadHelper)}.{nameof(Initialize)}: Called when there are pending tasks in the queue");

                _logger()?.Error("Initialize requested when there are pending tasks in the queue");

                //** TODO this needs tested for a memory leak, (Note no evidence of one just a precaution)
                //** Clear it. 
                _taskCollection = new BlockingCollection<Action>();
                
                throw ex;
            }

            if(_taskThread == null)
            {
                this.Initialize_Threading();
            }
            
        }



        ///<summary>
        ///Request to Close After Pending Tasks are done
        ///</summary>
        public void InvokeClose()
        {
            //Logger?.Information("{Name} Close Requested.", Name);
            _taskCollection.Add(CloseNow);
        }
        ///<summary>
        ///Close thread As Soon as Possible
        ///</summary>
        public void CloseNow()
        {
            //Logger?.Information("Closing {name} ...", Name);
            _ctsStopRequested.Cancel();
        }

        ///<summary>
        ///Waits Thread to Stop
        ///</summary>
        public void WaitForThreadToClose()
        {
            _taskThread.Join();
        }

        ///<summary>
        ///Requests Any action to run on the Task Thread.
        ///</summary>
        ///<remarks>
        ///This runs on the Logic Thread
        ///</remarks>
        public void Invoke(Action action)
        {
            _taskCollection.Add(action);
        }
    }
}
