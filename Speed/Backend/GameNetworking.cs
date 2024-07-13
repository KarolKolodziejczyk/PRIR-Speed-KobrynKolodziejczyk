using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Speed.Backend
{
    internal class GameNetworking : IDisposable
    {
        private TcpListener listener;
        private CancellationTokenSource cancellationTokenSource;
        private Task listenTask;

        public string IPPrzeciwnika { get; }

        public event Action<string> MessageReceived;

        public GameNetworking(string ip)
        {
            IPPrzeciwnika = ip;
        }

        public void StartListening(int port)
        {
            listener = new TcpListener(IPAddress.Any, port); // Nasłuchuj na wszystkich interfejsach
            listener.Start();
            cancellationTokenSource = new CancellationTokenSource();
            listenTask = Task.Run(() => ListenForConnections(cancellationTokenSource.Token));
        }

        public async Task SendToOpponent(string message)
        {
            if (!string.IsNullOrEmpty(IPPrzeciwnika))
            {
                await SendMessage(IPPrzeciwnika, message);
            }
            else
            {
                throw new InvalidOperationException("IPPrzeciwnika is not set. Cannot send message.");
            }
        }

        private async Task SendMessage(string ipAddress, string message)
        {
            try
            {
                using (var client = new TcpClient())
                {
                    await client.ConnectAsync(ipAddress, 7078);
                    var stream = client.GetStream();
                    var mess = Encoding.UTF8.GetBytes(message);
                    await stream.WriteAsync(mess, 0, mess.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message to {ipAddress}: {ex.Message}");
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
                Console.WriteLine($"Error while listening for connections: {ex.Message}");
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
                    OnMessageReceived(message); // Wywołanie metody do obsługi otrzymanej wiadomości
                }
            }
        }

        private void OnMessageReceived(string message)
        {
            MessageReceived?.Invoke(message); // Wywołanie zdarzenia MessageReceived
        }

        public void Stop()
        {
            cancellationTokenSource?.Cancel();
            listener?.Stop();
        }

        public void Dispose()
        {
            Stop();
            cancellationTokenSource?.Dispose();
        }
    }
}
