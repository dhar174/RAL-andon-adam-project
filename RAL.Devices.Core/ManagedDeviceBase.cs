using Nito.AsyncEx;
using Serilog;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TheColonel2688.Utilities;
using Trybot;

namespace Devices.Core
{
    public enum State
    {
        InitalNotConnected,
        TryingToConnect,
        Connected,
        ErrorEncounteredRetrying,
        TryingToReconnect,
        Closing,
        Closed
    }

    public enum Trigger
    {
        ConnectRequested,
        ConnectAttemptSucceeded,
        ConnectAttemptFailed,
        ErrorOccurred,
        ErrorWasRecoveredFrom,
        CloseRequested,
        ErrorWasUnrecoverable,
        CloseCompleted
    }

    public abstract class ManagedDeviceBase : HasLogger
    {

        private StateMachineNonBlockingProcessing<State, Trigger> _sm;

        private IBotPolicy ReconnectPolicy;

        private IBotPolicy CommandOrRequestPolicy;

        /// <summary>
        /// Lock to make this thread safe
        /// </summary>
        public AsyncLock _mutex { get; private set; } = new AsyncLock();

        private readonly List<Task> listOfCommandsOrRequestTasks = new List<Task>();

        private CancellationTokenSource _ctsConnectedFailed = new CancellationTokenSource();
        private CancellationToken _ctConnectedFailed;

        private CancellationTokenSource _ctsCloseFailed = new CancellationTokenSource();
        private CancellationToken _ctCloseFailed;

        private CancellationTokenSource _ctsKillCommands = new CancellationTokenSource();
        private CancellationToken _ctKillCommandsRequested;

        public bool IsConnected => _sm.IsInState(State.Connected);

        private Task _connectTask;
        private Task _reconnectTask;
        private Task _closeTask;

        private AsyncManualResetEvent _connectDoneWait;

        private AsyncManualResetEvent _reconnectDoneWait;

        private AsyncManualResetEvent _closeDoneWait;


        public ManagedDeviceBase(ILogger logger = null) : base (logger)
        {
            _connectDoneWait = new AsyncManualResetEvent();

            _reconnectDoneWait = new AsyncManualResetEvent();

            _closeDoneWait = new AsyncManualResetEvent();

            _ctKillCommandsRequested = _ctsKillCommands.Token;

            _ctConnectedFailed = _ctsConnectedFailed.Token;

            _ctCloseFailed = _ctsCloseFailed.Token;

            ConfigureStateMachineAndTryBot();
        }

        private void ConfigureStateMachineAndTryBot()
        {
            _sm = new StateMachineNonBlockingProcessing<State, Trigger>(State.InitalNotConnected);

            _sm.Configure(State.InitalNotConnected)
                .Permit(Trigger.ConnectRequested, State.TryingToConnect);

            _sm.Configure(State.TryingToConnect)
                .OnEntry(() => {
                    _sm.ProcessInsideAsync(async () =>
                    {
                        try
                        {
                            _connectTask = DoConnectAsync();
                            await _connectTask;
                            _sm.FireWithNoLock(Trigger.ConnectAttemptSucceeded);
                            _connectDoneWait.Set();
                        }
                        catch
                        {
                            _ctsConnectedFailed.Cancel();
                            _connectDoneWait.Set();
                            _sm.FireWithNoLock(Trigger.ConnectAttemptFailed);
                            
                        }
                    });
                    Console.WriteLine($"Entered State {nameof(State.TryingToConnect)}");
                }
                 )
                .Permit(Trigger.ConnectAttemptSucceeded, State.Connected)
                .Permit(Trigger.ConnectAttemptFailed, State.TryingToReconnect);

            _sm.Configure(State.TryingToReconnect)
                .OnEntry(() => 
                {
                    _sm.ProcessInsideAsync(async () =>
                    {
                        try
                        {
                            _connectDoneWait.Reset();
                            _reconnectDoneWait.Reset();
                            await ReconnectPolicy.ExecuteAsync(async () => await DoReconnectAsync());
                            _sm.FireWithNoLock(Trigger.ConnectAttemptSucceeded);
                            _connectDoneWait.Set();
                            _reconnectDoneWait.Set();
                        }
                        catch (Exception ex)
                        {
                            //** FatalW
                        }
                    });
                    Console.WriteLine($"Entered State {nameof(State.TryingToReconnect)}");
                })
                .Permit(Trigger.ConnectAttemptSucceeded, State.Connected)
                .Permit(Trigger.CloseRequested, State.Closing);

            _sm.Configure(State.Connected)
                .Permit(Trigger.ErrorOccurred, State.ErrorEncounteredRetrying)
                .Permit(Trigger.CloseRequested, State.Closing);

            _sm.Configure(State.ErrorEncounteredRetrying)
                .Ignore(Trigger.ErrorOccurred)
                .Permit(Trigger.CloseRequested, State.Closing)
                .Permit(Trigger.ErrorWasRecoveredFrom, State.Connected)
                .Permit(Trigger.ErrorWasUnrecoverable, State.TryingToReconnect);
            _sm.Configure(State.Closing)
                .OnEntry(() =>
                {
                    DoCloseAsync();
                    _sm.FireWithNoLock(Trigger.CloseCompleted);
                    _closeDoneWait.Set();
                })
                .Permit(Trigger.CloseCompleted, State.Closed);


            ReconnectPolicy = new BotPolicy(policy =>
            policy.Configure(config => config
                    .Retry(retryConfig => retryConfig
                            .WhenExceptionOccurs(exception => exception is DeviceConnectionException)
                            .RetryIndefinitely())
            ));

            const int retryLimit = 5;

            CommandOrRequestPolicy = new BotPolicy(policy =>
            policy.Configure(config => config
               .Retry(retryConfig => retryConfig
                    .WhenExceptionOccurs(exception => exception is DeviceConnectionException)
                    .WithMaxAttemptCount(retryLimit)
                    .OnRetry((ex, context) =>
                    {
                        //_ctKillCommandsRequested.ThrowIfCancellationRequested();
                        _sm.FireWithNoLock(Trigger.ErrorOccurred);
                    })
                    .OnRetrySucceeded((context) =>
                    {
                        _sm.FireIfPermittedNoLock(Trigger.ErrorWasRecoveredFrom);
                    })
                    )
               .Fallback(fallbackConfig => fallbackConfig
                    .WhenExceptionOccurs((exception) => exception is Trybot.Retry.Exceptions.MaxRetryAttemptsReachedException)
                    .OnFallback((exception, executionContext) =>
                    {

                        CancelAllCommandsAndWait().Wait();


                        _sm.FireWithNoLock(Trigger.ErrorWasUnrecoverable);

                        
                        void ThrowException()
                        {
                            throw new DeviceConnectionException($"Retry Count of {retryLimit} Exceeded", exception);
                        }

                        ThrowException();
                    }))

           ));
        }

        public void BeginConnect()
        {
            using (_mutex.Lock())
            {
                _sm.BeginFireIfPermitted(Trigger.ConnectRequested);
            }
            
        }

        public async Task ConnectAsync()
        {
            BeginConnect();

            await WaitForConnectedAsync();

        }

        /// <summary>
        /// Wait for Client to Connect
        /// </summary>
        public async Task WaitForConnectedAsync()
        {

            await _connectDoneWait.WaitAsync();

            if (_ctConnectedFailed.IsCancellationRequested)
            {
                throw _connectTask.Exception;
            }
        }

        private async Task CancelAllCommandsAndWait()
        {
            
            _ctsKillCommands.Cancel();
            try
            {
                await Task.WhenAll(listOfCommandsOrRequestTasks.ToArray());
            }
            catch
            {
                //** ATTENTION: Ignored On Purpose Exceptions are handled elsewhere.
            }
            var temp = listOfCommandsOrRequestTasks;
        }
       
        protected abstract Task DoConnectAsync();

        protected abstract Task DoReconnectAsync();

        protected abstract Task DoCloseAsync();

        protected async Task CommandAsync(Func<Task> command)
        {

            ThrowExceptionIfNotConnected();
     
            Task tempTask = null;

            try
            {                
                await CommandOrRequestPolicy.ExecuteAsync(async () =>
                {
                    tempTask = command.Invoke();
                    listOfCommandsOrRequestTasks.Add(tempTask);

                    await tempTask;
                }
                , _ctKillCommandsRequested
                );

            }
            finally
            {
                if (tempTask != null)
                {
                    listOfCommandsOrRequestTasks.Remove(tempTask);
                }
            }
        }

        protected async Task<T> RequestAsync<T>(Func<Task<T>> request)
        {
            ThrowExceptionIfNotConnected();

            Task<T> tempTask = null;
            
            try
            {
                T result = default;
                await CommandOrRequestPolicy.ExecuteAsync(async () => 
                {
                    result = await (tempTask = Task.Run(() => request.Invoke()));
                    listOfCommandsOrRequestTasks.Add(tempTask);
                }, _ctKillCommandsRequested);

                return result;
                //return await tempTask.GetAwaiter().GetResult();
            }
            finally
            {
                if (tempTask != null)
                {
                    listOfCommandsOrRequestTasks.Remove(tempTask);
                }                
            }
        }

        /// <summary>
        /// Wait for Client to Connect
        /// </summary>
        public async Task WaitForReconnectedAsync()
        {
            using (await _mutex.LockAsync())
            {
                await _reconnectDoneWait.WaitAsync();
            }
        }


        

        public void BeginClose()
        {
            using (_mutex.Lock())
            {
                _sm.BeginFireIfPermitted(Trigger.CloseRequested);
            }
        }

        public async Task CloseAsync()
        {
            BeginClose();           
            await WaitForCloseAsync();
        }

        /// <summary>
        /// Wait for Client to Connect
        /// </summary>
        public async Task WaitForCloseAsync()
        {

            await _closeDoneWait.WaitAsync();
            if (_ctCloseFailed.IsCancellationRequested)
            {
                throw _closeTask.Exception;
            }
        }

        protected void ThrowExceptionIfNotConnected([CallerMemberName] string memberName = "")
        {           
            if (_sm.IsInState(State.Connected))
            {
                return;
            }

            var ConnectHasBeenCalled = _sm.IsInState(State.InitalNotConnected);

            if (ConnectHasBeenCalled)
            {
                throw new DeviceConnectionException($"You must call connect before, before you can call {memberName}");
            }

            //_logger.Here(nameof(SignaworksEthernetStackLightManaged), Description).Error("{memberName} was called while not connected. Current State is {State}", memberName, _sm.GetCurrentState());
            throw new DeviceConnectionException($"{memberName} was called while {ClassTypeAsString}[{ToString()}] was not connected");

        }
    }
}
