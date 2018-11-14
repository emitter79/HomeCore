using System.Net;
using System.Net.Sockets;

namespace CoreAPI
{
    public static class Configuration
    {
        private static int _port = 80;

        public static string Hostname
        {
            get => Dns.GetHostName();
        }

        public static string EthernetIP
        {
            get => _getLocalIP();
        }

        public static int Port
        {
            get => _port;
            set => _port = value;
        }

        public static string BaseAddress
        {
            get => _createBaseAddress();
        }

        private static string _getLocalIP()
        {
            string _ip = string.Empty;
            IPHostEntry hostEntry = Dns.GetHostEntry(Hostname);
            IPAddress[] IPs = hostEntry.AddressList;
            foreach (var ip in IPs)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    _ip = ip.ToString();
                    break;
                }
            }
            return _ip;
        }

        private static string _createBaseAddress()
        {
            var base_addr = "http://" + _getLocalIP();
            if (_port != 80) base_addr += ":" + _port;
            return base_addr + "/";
        }
    }
}
