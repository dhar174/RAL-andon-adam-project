using Nito.AsyncEx;
using Stateless;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TheColonel2688.Utilities
{
    public class StateMachineNonBlockingProcessing<S, T>
    {
        private StateMachine<S, T> _sm;

        //public object Semaphore { get; private set; } 

        private AsyncLock _mutex = new AsyncLock();

        private Task CurrentProcessingTask = null;

        //ManualResetEvent currentTaskWaitHandle = new ManualResetEvent(true);

        public StateMachine<S, T>.StateConfiguration Configure(S state)
        {
            return _sm.Configure(state);
        }

        public StateMachineNonBlockingProcessing(S initalState)
        {
            _sm = new StateMachine<S, T>(initalState);
        }

        public void ProcessInsideAsync(Func<Task> userCode)
        {
            //** TODO this should be called from inside the StateMachineNonBlocking, I just haven't gotten that far yet
            if (CurrentProcessingTask != null)
            {
                CurrentProcessingTask.Wait();
            }
            CurrentProcessingTask = Task.Run(() => 
            {
                    userCode.Invoke();
            });          
        }

        public void WaitUntilCurrentTaskDone()
        {
            CurrentProcessingTask.Wait();
        }

        public void FireWithNoLock(T trigger)
        {
            _sm.Fire(trigger);
        }

        public void BeginFire(T trigger)
        {
            using (_mutex.Lock())
            {
                /*
                if (CurrentProcessingTask != null)
                {
                    CurrentProcessingTask.Wait();
                }*/
                _sm.Fire(trigger);
            }
        }

        public bool BeginFireIfPermitted(T trigger)
        {
            using (_mutex.Lock())
            {
                if (_sm.CanFire(trigger))
                {
                    /*
                    if (CurrentProcessingTask != null)
                    {
                        CurrentProcessingTask.Wait();
                    }*/
                    _sm.Fire(trigger);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        public bool FireIfPermittedNoLock(T trigger)
        {
                if (_sm.CanFire(trigger))
                {
                    _sm.Fire(trigger);
                    return true;
                }
                else
                {
                    return false;
                }
        }


        private IEnumerable<T> GetPermittedTriggers()
        {
            using (_mutex.Lock())
            {
                return _sm.PermittedTriggers;
            }
        }

        public S GetCurrentState()
        {
            using (_mutex.Lock())
            {
                return _sm.State;
            }
        }

        public bool IsInState(S state)
        {
            using (_mutex.Lock())
            {
                return _sm.IsInState(state);
            }
        }

        public bool CanFire(T trigger)
        {
            using (_mutex.Lock())
            {
                return _sm.CanFire(trigger);
            }
        }
    }
}
