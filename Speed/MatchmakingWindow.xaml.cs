using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Speed.Backend;

namespace Speed
{
    public partial class MatchmakingWindow : Window
    {
        Networking networking = new Networking();
        private HashSet<string> localIPs;

        public MatchmakingWindow()
        {
            InitializeComponent();
            LoadOpponentImg();
            LoadOpponentIPTxt();
            startNetwork();
            LbxLocalIPs.Items.Clear();
        }

        private void startNetwork()
        {
            localIPs = new HashSet<string>(Networking.GetAllLocalIPv4());
            networking.Broadcast();
        }

        private void LoadOpponentImg()
        {
            Image imgOpp = new Image();
            var uri = new Uri("pack://application:,,,/cards/spade2.png");
            var bitmap = new BitmapImage(uri);
            imgOpp.Source = bitmap;
            LblOpponent.Content = imgOpp;
        }

        private void LoadOpponentIPTxt()
        {
            string addr = TxtOpponentIP.Text;
            LblOpponentIP.Content = "[ " + addr + " ]";
        }

        private async void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            string opponentIP = LblOpponentIP.Content.ToString().Trim('[', ']', ' ');

            if (!string.IsNullOrEmpty(opponentIP))
            {
                // Wysyłanie żądania gry do przeciwnika
                await networking.SendRequest(opponentIP, "REQUEST_GAME");
            }
            else
            {
                MessageBox.Show("Wybierz adres IP przeciwnika.");
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            networking.Stop();
            networking.MessageReceived -= OnMessageReceived;
            this.Close();
        }

        private async void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            networking.MessageReceived += OnMessageReceived;
            try
            {
                IEnumerable<string> results = await networking.ScanNetwork();

                // Clear the existing items in ListBox
                LbxLocalIPs.Items.Clear();

                // Add the new results to the ListBox
                foreach (var result in results)
                {
                    if (!localIPs.Contains(result))
                    {
                        LbxLocalIPs.Items.Add(result);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas skanowania sieci: {ex.Message}");
            }
        }

        public void OnMessageReceived(string message)
        {
            Dispatcher.Invoke(() =>
            {
                string[] parts = message.Split(',');
                string command = parts[0];
                string senderIp = parts.Length > 1 ? parts[1] : null;

                if (command == "REQUEST_GAME")
                {
                    // Wyświetlanie okna dialogowego z zapytaniem o przyjęcie gry
                    MessageBoxResult result = MessageBox.Show($"Czy chcesz grać z przeciwnikiem o IP {senderIp}?", "Żądanie gry", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        // Akceptacja gry - wyślij odpowiedź i uruchom grę
                        networking.SendRequest(senderIp, "ACCEPT_GAME");
                        SpeedGameWindow game = new SpeedGameWindow(senderIp, true);
                        game.Title = "Host";
                        networking.Stop();
                        this.Close();
                        game.ShowDialog();
                    }
                    else
                    {
                        // Odrzucenie żądania gry
                        networking.SendRequest(senderIp, "REJECT_GAME");
                    }
                }
                else if (command == "ACCEPT_GAME")
                {
                    SpeedGameWindow game = new SpeedGameWindow(LblOpponentIP.Content.ToString(), false);
                    //MessageBox.Show(LblOpponentIP.Content.ToString());
                    game.Title = "Guest";
                    networking.Stop();
                    this.Close();
                    game.ShowDialog();

                }
                else if (command == "REJECT_GAME")
                {
                    // Możesz dodać kod do obsługi odrzuconego żądania gry
                }
                else
                {
                    if (!localIPs.Contains(message) && !LbxLocalIPs.Items.Contains(message))
                    {
                        LbxLocalIPs.Items.Add(message);
                    }
                }
            });
        }

        private void LbxLocalIPs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LblOpponentIP.Content = LbxLocalIPs.SelectedItem.ToString();
        }

        private void TxtOpponentIP_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtOpponentIP.IsFocused)
            {
                string ipAddress = TxtOpponentIP.Text;
                string pattern = @"^((25[0-5]|2[0-4][0-9]|[0-1]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[0-1]?[0-9][0-9]?)$";
                Regex regex = new Regex(pattern);

                if (regex.IsMatch(ipAddress))
                {
                    LblOpponentIP.Content = ipAddress;
                }
                else
                {
                    LblOpponentIP.Content = "0.0.0.0";
                }
            }
        }
    }
}
