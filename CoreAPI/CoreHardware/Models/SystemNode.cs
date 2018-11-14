using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using CoreHardware.Interfaces;
using CoreHardware.Models;
using CoreHardware.Enumerations;
using RJCP.IO.Ports;

namespace CoreHardware.Models
{
    public class SystemNode : ISystemNode, IDisposable
    {
        private const int PORT_NOT_FOUND = -2146232800;
        private const int BUFFER_SIZE = 64;
        private static bool ENABLE_FLUSH = true;
        
        private List<byte> _data = new List<byte>();

        private SerialPortStream _port;

        public string PortName { get; set; }

        public PortSpeeds PortSpeed { get; set; }

        public ProcessorTypes ProcessorType { get; set; }

        public virtual SerialPortStream Port
        {
            get => _port;
        }

        public bool IsConnected
        {
            get => _port.IsOpen;
        }

        #region constructors

        public SystemNode()
        {
            _tryConnect();
        }

        public SystemNode(string portName, PortSpeeds portSpeed)
        {
            PortName = portName;
            PortSpeed = portSpeed;
            _tryConnect();
        }

        public SystemNode(string portName, PortSpeeds portSpeed, ProcessorTypes processorType)
        {
            PortName = portName;
            PortSpeed = portSpeed;
            ProcessorType = processorType;
            _tryConnect();
        }

        #endregion

        #region overloaded Connect() methods

        public bool Connect()
        {
            return _tryConnect();
        }

        public bool Connect(string portName, PortSpeeds portSpeed)
        {
            PortName = portName;
            PortSpeed = portSpeed;
            return _tryConnect();
        }

        public bool Connect(string portName, PortSpeeds portSpeed, ProcessorTypes processorType)
        {
            PortName = portName;
            PortSpeed = portSpeed;
            ProcessorType = processorType;
            return _tryConnect();
        }

        #endregion

        #region internal methods

        private int _convertPortSpeed(PortSpeeds speed)
        {
            int.TryParse(speed.ToString().Replace("_", string.Empty), out int _speed);
            return _speed;
        }

        private void _destroyPort()
        {
            if (_port?.IsOpen == true)
            {
                // try to close the socket if open.
                try { _port.Close(); }
                catch { }
            }
            // destroy the instance and free the resources
            if (_port != null)
            {
                try
                {
                    _port?.Dispose();
                    _port = null;
                    GC.Collect();
                }
                catch { }
            }
        }

        private bool _tryConnect()
        {
            // destroy the port and it's resources
            _destroyPort();
            // try to connect with the current parameters
            int _speed = _convertPortSpeed(PortSpeed);
            if (PortName.Length > 0 && _speed > 0)
            {
                _port = new SerialPortStream(PortName, _speed);
                try
                {
                    _port.Open();
                    return _port.IsOpen;
                }
                catch (Exception e)
                {
                    if (e.HResult == PORT_NOT_FOUND)
                    {
                        Console.WriteLine("::> Error connecting to primary system node.");
                    }
                    else Console.WriteLine("::> Error: " + e);
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region public methods

        public void SendCommand(ValueCommand command)
        {
            if (_port != null && _port?.IsOpen == true)
            {
                var data = command.ToByteArray();
                _port.Write(data, 0, data.Length);
                if (ENABLE_FLUSH) _port.Flush();
            }
        }

        public async Task SendCommandAsync(ValueCommand command)
        {
            if (_port != null && _port?.IsOpen == true) await Task.Run(() => {
                _port.Write(command.ToByteArray(), 0, 4);
                if (ENABLE_FLUSH) _port.Flush();
            });
        }

        public byte[] SendCommand(Command command)
        {
            byte index = 0;
            if (_port != null && _port?.IsOpen == true)
            {
                _data.Clear();
                var data = command.ToByteArray();
                _port.Write(data, 0, data.Length);
                _port.Flush();
                System.Threading.Thread.Sleep(100);
                byte b = (byte)_port.ReadByte();
                while ((b != 13) && (index < 255) && _port.CanRead)
                {
                    _data.Add(b);
                    index++;
                    b = (byte)_port.ReadByte();
                }
            }
            return _data.ToArray();
        }

        #endregion

        public void Dispose()
        {
            _destroyPort();
        }
    }
}
