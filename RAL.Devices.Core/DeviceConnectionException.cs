using System;

namespace Devices.Core
{
    public class DeviceConnectionException : Exception
    {
        public bool HasBeenLogged { get; set; }

        public DeviceConnectionException(bool hasBeenLogged = false)
        {
            HasBeenLogged = hasBeenLogged;
        }

        public DeviceConnectionException(string message, bool hasBeenLogged = false)
            : base(message)
        {
            HasBeenLogged = hasBeenLogged;
        }

        public DeviceConnectionException(string message, Exception inner, bool hasBeenLogged = false)
            : base(message, inner)
        {
            HasBeenLogged = hasBeenLogged;
        }

        
    }
}