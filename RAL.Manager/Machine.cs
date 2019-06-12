using RAL.Collector;
using RAL.Devices.Adam;
using RAL.Repository;
using RAL.Repository.Model;
using RAL.Rules;
using TheColonel2688.Utilities;
using Serilog;
using System;
using System.Collections.Generic;

namespace RAL.Manager
{
    public class Machine : HasLogger, IMachineInfo, IMachineForRules
    {
        public string IPAddress { get; set; }
        public string MAC { get; set; }
        public string Department { get; set; }
        public string Line { get; set; }
        public string Name { get; set; }

        //private ILogger _logger;

        private Adam6051Client _adam6051Client;

        public Adam6051Client Adam6051Client
        {
            get { return _adam6051Client; }
            set
            {
                if (!(_adam6051Client is null))
                {
                    _adam6051Client.StatusReceived -= _adam6051PressClient_StatusReceived;
                    _adam6051Client.LastWillReceived -= _adam6051Client_LastWillReceived;
                    _adam6051Client.Connected -= _adam6051Client_Connected;
                    _adam6051Client.Disconnected -= _adam6051Client_Disconnected;
                }
                _adam6051Client = value;
                _adam6051Client.StatusReceived += _adam6051PressClient_StatusReceived;
                _adam6051Client.LastWillReceived += _adam6051Client_LastWillReceived;
                _adam6051Client.Connected += _adam6051Client_Connected;
                _adam6051Client.Disconnected += _adam6051Client_Disconnected;
            }
        }

        public event EventHandler<StatusChangedArgs<MachineStatusMessage>> StatusChanged;

        public IEnumerable<ISingleMachineRule<Machine>> Rules { get => _rules; }

        private IMachineStatusPayloadConverter Adam6051StatusPayloadConverterForPress;

        private List<ISingleMachineRule<Machine>> _rules = new List<ISingleMachineRule<Machine>>();

        public IMachineRepository Repository { get; private set; }

        //protected override string _classTypeAsString => nameof(Machine);

        private DateTime? whenLastStatusMessageWasReceived;

        private MachineStatusMessage _lastStatus = new MachineStatusMessage();
        
        public override string Description { get => $"Machine {ToString()}"; }

        protected override string ClassTypeAsString => nameof(Machine);

        public Machine(IMachineInfo machineInfo, IMachineRepository repository, Adam6051Client adam6051Client, IMachineStatusPayloadConverter statusPayloadConverter, ILogger logger = null) : base (logger)
        {
            IPAddress = machineInfo.IPAddress;
            MAC = machineInfo.MAC;
            Department = machineInfo.Department;
            Line = machineInfo.Line;
            Name = machineInfo.Name;
            Repository = repository;
            Adam6051StatusPayloadConverterForPress = statusPayloadConverter ?? new Adam6051StatusPayloadConverterForPress();
            Adam6051Client = adam6051Client;

        }

        public void AddRule(ISingleMachineRule<Machine> rule)
        {
            _rules.Add(rule);
        }

        private async void _adam6051Client_LastWillReceived(object sender, Adam6051PayloadReceivedEventArgs e)
        {
            //** TODO Add a converter and get the last IP address used.
            _logger().Debug("Last Will Received");
        }

        private async void _adam6051PressClient_StatusReceived(object sender, Adam6051PayloadReceivedEventArgs e)
        {
            if (MAC == "00-D0-C9-FC-A9-C0")
            {

            }

            var Now = DateTime.Now;
            var tempStatus = Adam6051StatusPayloadConverterForPress.Convert(e.Payload);
            if (StatusIsChanged(tempStatus))
            {
                StatusChanged?.Invoke(this, new StatusChangedArgs<MachineStatusMessage>(tempStatus.Copy()));
                try
                {
                    await Repository.MachineStatusRepo.WriteAsync(new MachineStatusInflux()
                    {
                        IPAddress = IPAddress,
                        MAC = MAC,
                        Department = Department,
                        Line = Line,
                        Name = Name,
                        IsCycling = tempStatus.IsCycling,
                        IsFaulted = tempStatus.IsFaulted,
                        IsInAutomatic = tempStatus.IsInAutomatic,
                        Time = Now.ToUniversalTime()
                    });
                }
                catch (RepositoryConnectionException ex)
                {
                    _logger().Warning(ex, "Attempt to write Repository Failed due to a connection issue.");
                }
                catch (Exception ex)
                {
                    _logger().Warning(ex, "Attempt to write Repository Failed, with unexpected Exception");
                }
                _lastStatus = tempStatus;

                _logger().Debug("Status Changed to {Status}", tempStatus);
            }

            whenLastStatusMessageWasReceived = Now;
        }

        private void _adam6051Client_Connected(object sender, EventArgs e)
        {
            _logger().Information("Connected");
        }

        private void _adam6051Client_Disconnected(object sender, EventArgs e)
        {
            _logger().Information("Disconnected");
        }

        private bool StatusIsChanged(MachineStatusMessage newStatus)
        {
            bool hasChanged = _lastStatus.IsCycling != newStatus.IsCycling;

            return hasChanged;
        }

        public override string ToString()
        {
            return $"{Line}.{Name} with {MAC}";
        }

        /*
        //** Assign default Converter
        private IMachineStatusPayloadConverter _statusPayloadConverter = new Adam6051StatusPayloadConverterForPress();

        
        public IMachineStatusPayloadConverter StatusPayloadConverter
        {
            get { return _statusPayloadConverter; }
            set
            {
                _statusPayloadConverter = value;
                _logger?.Here(nameof(Adam6051Client), MAC).Warning("Overriding default StatusPayloadConverter");
            }
        }
        */
    }

    public class StatusChangedArgs<T> : EventArgs
    {
        public T Status { get; private set; }

        public StatusChangedArgs(T status)
        {
            Status = status;
        }
    }
}
