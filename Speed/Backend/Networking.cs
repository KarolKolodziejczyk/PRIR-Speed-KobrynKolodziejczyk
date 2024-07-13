using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Speed.Backend
{
    public class Networking
    {
        public string status = "nl";
        private TcpListener listener;
        private CancellationTokenSource cancellationTokenSource;
        private Task listenTask;
        private DispatcherTimer broadcastTimer;

        // Zdarzenie do przekazywania wiadomości do klasy nadrzędnej
        public event Action<string> MessageReceived;

        public bool Broadcast()
        {
            listener = new TcpListener(IPAddress.Any, 7077);
            listener.Start();
            cancellationTokenSource = new CancellationTokenSource();
            listenTask = Task.Run(() => ListenForConnections(cancellationTokenSource.Token));
            StartBroadcasting();
            return true;
        }

        private void StartBroadcasting()
        {
            broadcastTimer = new DispatcherTimer();
            broadcastTimer.Interval = TimeSpan.FromSeconds(5); // Wysyłanie wiadomości co 5 sekund
            broadcastTimer.Tick += BroadcastMessage;
            broadcastTimer.Start();
        }

        private async void BroadcastMessage(object sender, EventArgs e)
        {
            var localIPs = GetAllLocalIPv4().ToList();
            if (localIPs.Count == 0)
            {
                MessageBox.Show("Nie można znaleźć lokalnych adresów IP.");
                return;
            }

            var tasks = new List<Task>();
            foreach (var localIP in localIPs)
            {
                var subnet = localIP.Substring(0, localIP.LastIndexOf('.') + 1);
                tasks.AddRange(Enumerable.Range(1, 254).Select(i => SendMessage(subnet + i)));
            }

            await Task.WhenAll(tasks);
        }

        private async Task SendMessage(string ipAddress)
        {
            try
            {
                using (var client = new TcpClient())
                {
                    await client.ConnectAsync(ipAddress, 7077);
                    var stream = client.GetStream();
                    var message = Encoding.UTF8.GetBytes(ipAddress);
                    await stream.WriteAsync(message, 0, message.Length);
                }
            }
            catch (Exception)
            {
                // Obsługa wyjątków
            }
        }

        private async Task ListenForConnections(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    if (listener.Pending())
                    {
                        var client = await listener.AcceptTcpClientAsync();
                        _ = Task.Run(() => HandleClient(client, token));
                    }
                    await Task.Delay(100); // Uniknięcie zajęcia CPU w pętli
                }
            }
            catch (Exception ex)
            {
                // Obsługa wyjątków
            }
        }

        private async Task HandleClient(TcpClient client, CancellationToken token)
        {
            using (client)
            {
                var buffer = new byte[1024];
                var stream = client.GetStream();
                int bytesRead;
                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, token)) != 0)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    MessageReceived?.Invoke(message);
                }
            }
        }

        public async Task<IEnumerable<string>> ScanNetwork()
        {
            var localIPs = GetAllLocalIPv4().ToList();
            if (localIPs.Count == 0)
            {
                MessageBox.Show("Nie można znaleźć lokalnych adresów IP.");
                return Enumerable.Empty<string>();
            }

            var tasks = new List<Task<string>>();
            foreach (var localIP in localIPs)
            {
                var subnet = localIP.Substring(0, localIP.LastIndexOf('.') + 1);
                tasks.AddRange(Enumerable.Range(1, 254).Select(i => PingAndCheckPort(subnet + i)));
            }

            var results = await Task.WhenAll(tasks);

            return results.Where(result => result != null);
        }

        private async Task<string> PingAndCheckPort(string ipAddress)
        {
            try
            {
                using (var ping = new Ping())
                {
                    var reply = await ping.SendPingAsync(ipAddress, 2000);
                    if (reply.Status == IPStatus.Success)
                    {
                        if (await IsPortOpen(ipAddress, 7077, TimeSpan.FromSeconds(2)))
                        {
                            return ipAddress;
                        }
                    }
                }
            }
            catch (Exception) { }

            return null;
        }

        private async Task<bool> IsPortOpen(string host, int port, TimeSpan timeout)
        {
            try
            {
                using (var client = new TcpClient())
                {
                    var result = client.BeginConnect(host, port, null, null);
                    var success = await Task.Run(() => result.AsyncWaitHandle.WaitOne(timeout));
                    if (!success)
                    {
                        return false;
                    }

                    client.EndConnect(result);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static IEnumerable<string> GetAllLocalIPv4()
        {
            List<string> ipAddrList = new List<string>();
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            string ipAddress = ip.Address.ToString();
                            if (!ipAddress.StartsWith("127."))
                            {
                                ipAddrList.Add(ipAddress);
                            }
                        }
                    }
                }
            }
            return ipAddrList;
        }

        public void Stop()
        {
            cancellationTokenSource.Cancel();
            listener.Stop();
            broadcastTimer.Stop();
        }

        public async Task SendRequest(string ipAddress, string request)
        {
            try
            {
                using (var client = new TcpClient())
                {
                    await client.ConnectAsync(ipAddress, 7077);
                    var stream = client.GetStream();
                    string localIp = GetLocalIpAddress();
                    var message = Encoding.UTF8.GetBytes($"{request},{localIp}");
                    await stream.WriteAsync(message, 0, message.Length);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd wysyłania żądania: {ex.Message}");
            }
        }

        private string GetLocalIpAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork && !ip.ToString().StartsWith("127."))
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Brak lokalnego adresu IP.");
        }
    }
}
