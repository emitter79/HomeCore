using System.IO.Ports;
using System.Threading.Tasks;
using CoreHardware.Enumerations;
using CoreHardware.Models;
using RJCP.IO.Ports;

namespace CoreHardware.Interfaces
{
    public interface ISystemNode
    {
        string PortName { get; set; }

        PortSpeeds PortSpeed { get; set; }

        SerialPortStream Port { get; }

        ProcessorTypes ProcessorType { get; }

        bool IsConnected { get; }

        bool Connect();

        bool Connect(string portName, PortSpeeds portSpeed);

        bool Connect(string portName, PortSpeeds portSpeed, ProcessorTypes processorType);

        void SendCommand(ValueCommand command);

        Task SendCommandAsync(ValueCommand command);

        byte[] SendCommand(Command command);
    }
}