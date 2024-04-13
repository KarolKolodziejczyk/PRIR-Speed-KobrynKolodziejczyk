using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
//Tutaj biorę z swojego starego projektu funkcje jak coś
namespace SpeedBackend
{
    internal class ListaGraczy : IPlansza
    {
        List<String> ListaIp = new List<string>();


        void SearchForPlayers() {
            NetworkInterface[] interfejsy = NetworkInterface.GetAllNetworkInterfaces();
            Parallel.ForEach(interfejsy, networkInterface =>
            {
                if (networkInterface.OperationalStatus == OperationalStatus.Up)
                {
                    IPInterfaceProperties interfaceProperties = networkInterface.GetIPProperties();
                    foreach (UnicastIPAddressInformation ipAddressInfo in interfaceProperties.UnicastAddresses)
                    {
                        if (ipAddressInfo.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            IPAddress subnetMask = ipAddressInfo.IPv4Mask;
                            IPAddress ipAddress = ipAddressInfo.Address;

                            if (!IPAddress.IsLoopback(ipAddress))
                            {
                                IEnumerable<string> pingableIps = GetPingableIPs(ipAddress, subnetMask);

                                lock (ListaIp)
                                {
                                    ListaIp.AddRange(pingableIps);
                                }

                            }
                        }
                    }
                }
            });


        }
        private static IEnumerable<string> GetPingableIPs(IPAddress ipAddress, IPAddress subnetMask)
        {
            List<string> pingableIPs = new List<string>();

            byte[] ipBytes = ipAddress.GetAddressBytes();
            byte[] subnetBytes = subnetMask.GetAddressBytes();

            for (int i = 0; i < ipBytes.Length; i++)
            {
                ipBytes[i] &= subnetBytes[i];
            }

            IPAddress networkAddress = new IPAddress(ipBytes);

            for (int i = 1; i < 255; i++)
            {
                ipBytes[ipBytes.Length - 1] = (byte)i;
                IPAddress pingIP = new IPAddress(ipBytes);

                Ping ping = new Ping();
                PingReply reply = ping.Send(pingIP, 3);

                if (reply.Status == IPStatus.Success)
                {
                    pingableIPs.Add(pingIP.ToString());
                }
            }

            return pingableIPs;
        }
    }
 

}

