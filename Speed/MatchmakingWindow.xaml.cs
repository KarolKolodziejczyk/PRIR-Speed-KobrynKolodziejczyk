using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Speed.Backend;

namespace Speed
{
    public partial class MatchmakingWindow : Window
    {
        Networking networking = new Networking();
        public MatchmakingWindow()
        {
            InitializeComponent();
            LoadOpponentImg();
            LoadOpponentIPTxt();
            startNetwork();
            //networking.MessageReceived += OnMessageReceived;
            LbxLocalIPs.Items.Clear();

        }

        private void startNetwork()
        {
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

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            SpeedGameWindow game = new SpeedGameWindow(LblOpponentIP.ContentStringFormat);
            game.ShowDialog();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.networking.Stop();
            WaitingForHostWindow waitingWindow = new WaitingForHostWindow();
            waitingWindow.Show();
            //this.Close();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            networking.MessageReceived += OnMessageReceived;

        }
        public void OnMessageReceived(string message)
        {
            
                // Ensure the method is called on the UI thread
                Dispatcher.Invoke(() =>
                {
                   //MessageBox.Show($"Odebrano wiadomość !!!: {message}");

                    bool messageExists = false;
                    foreach (var item in LbxLocalIPs.Items)
                    {
                        if (item.ToString() == message)
                        {
                            messageExists = true;
                            break;
                        }
                    }

                    if (!messageExists)
                    {
                        LbxLocalIPs.Items.Add(message);
                    }
                });
            

        }

        private void LbxLocalIPs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LblOpponentIP.Content=LbxLocalIPs.SelectedItem.ToString();
        }
    }
}
