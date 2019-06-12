using Serilog;
using Stateless;
using System;
using System.Collections.Generic;

namespace TheColonel2688.Utilities
{
    public class StateMachineWithThreadHelper<S, T> : HasThreadHelper
    {
        private StateMachine<S, T> _sm;

        protected override string ClassTypeAsString => throw new NotImplementedException();
        

        public StateMachine<S, T>.StateConfiguration Configure(S state)
        {
            return _sm.Configure(state);
        }

        public StateMachineWithThreadHelper(S initalState, string description, TimeSpan? threadWatchDogElapsed = null, bool autoStartThread = true, ILogger logger = null) : base(new ThreadHelper($"ThreadHelper For {description}", threadWatchDogElapsed, logger))
        {
            _sm = new StateMachine<S, T>(initalState);
            if (autoStartThread)
            {
                Start();
            }

            Description = description;
        }

        public void InvokeForThreadHelperIShouldBeHidden(Action action)
        {
            _threadHelper.Invoke(action);
        }

        public void Start()
        {
            _threadHelper.Initialize();
        }

        public void Close()
        {
            _threadHelper.CloseNow();
        }

        public void WaitForCloseToComplete()
        {
            _threadHelper.WaitForThreadToClose();
        }

        public void Fire(T trigger)
        {
            lock (_sm)
            {
                _sm.Fire(trigger);
            }

            /*
            if (CanFireThreadSafe(trigger))
            {
                _threadHelper.Invoke(() => _sm.Fire(trigger));
            }
            else
            {
                throw new Exception($"Trigger {trigger} can not be fired right now current state is {GetCurrentStateThreadSafe()} permitted triggers are {GetPermittedTriggersThreadSafe().ConvertToString()}");
            }
            */
        }

        public bool FireIfPermited(T trigger)
        {
            lock (_sm)
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
        }

        private IEnumerable<T> GetPermittedTriggers()
        {
            lock (_sm)
            {
                return _sm.PermittedTriggers;
            }
        }

        public S GetCurrentState()
        {
            lock (_sm)
            {
                return _sm.State;
            }
        }

        public bool IsInState(S state)
        {
            lock (_sm)
            {
                return _sm.IsInState(state);
            }
        }

        public bool CanFire(T trigger)
        {
            lock (_sm)
            {
                return _sm.CanFire(trigger);
            }
        }    
    }
}
