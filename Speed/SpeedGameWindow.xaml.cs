using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using Speed.Backend;

namespace Speed
{
    public partial class SpeedGameWindow : Window
    {
        private SpeedGameApp APP;
        private int HoverPixelAmount { get; } = 20;
        private bool CzyHost = false;
        private List<Button> playerCardButtons;

        public SpeedGameWindow(string IP, bool s)
        {

            playerCardButtons = new List<Button> {BtnPlayerCard1, BtnPlayerCard2, BtnPlayerCard3, BtnPlayerCard4, BtnPlayerCard5 };

            InitializeComponent();
            //Aktualizuj("1");
            CzyHost = s;
            APP = new SpeedGameApp(IP);
            if (CzyHost)
            {
               // Aktualizuj("JESTEM HOSTEM");
                APP.game.Init();
                for(int i=0; i !=5;i++)
                    APP.network.SendToOpponent("[TG]?" + this.APP.game.RękaPrzeciwnika[i].SerializeToJson());

                aktualizujKarty();
            }
            // Testowe karty
            // Aktualizuj(this.APP.game.RękaGracza[0].ImagePath);
     
            LblEnemyCard1.Content = LoadCardImage("reverse");
            LblEnemyCard2.Content = LoadCardImage("reverse");
            LblEnemyCard3.Content = LoadCardImage("reverse");
            LblEnemyCard4.Content = LoadCardImage("reverse");
            LblEnemyCard5.Content = LoadCardImage("reverse");
            LblEnemy.Content = LoadCardImage("opponent");

            this.APP.network.MessageReceived += OnMessageReceived; // Subskrybuj zdarzenie odbioru wiadomości
        }

        private Image LoadCardImage(string cardSymbol, string cardNumber = "")
        {
            Image image = new Image();
            var uri = new Uri($"pack://application:,,,/cards/{cardSymbol.ToLower()}{cardNumber}.png");
            var bitmap = new BitmapImage(uri);
            image.Source = bitmap;
            return image;
        }

        private async void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Wysylam");
            await APP.network.SendToOpponent("Wiadomość: hehe");
        }

        private void MoveButton(object sender, int pixelAmount)
        {
            if (sender is Button button)
            {
                button.Margin = new Thickness(button.Margin.Left, button.Margin.Top - pixelAmount, button.Margin.Right, button.Margin.Bottom + pixelAmount);
            }
        }

        private void BtnPlayerCard_MouseEnter(object sender, MouseEventArgs e)
        {
            MoveButton(sender, HoverPixelAmount);
        }

        private void BtnPlayerCard_MouseLeave(object sender, MouseEventArgs e)
        {
            MoveButton(sender, -HoverPixelAmount);
        }

        private void BtnPlayerCard1_Click(object sender, RoutedEventArgs e)
        {
            // Obsługa kliknięcia karty gracza
        }

        private void OnMessageReceived(string message)
        {

            // Podział wiadomości na części na podstawie separatora [TG]
            string[] parts = message.Split(new string[] { "?" }, StringSplitOptions.None);

            if (parts.Length == 2 && parts[0] == "[TG]")
            {
                // Rekonstrukcja tali na nowo
                try
                {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {

                    Karta karta = new Karta(parts[1]);
                    //MessageBox.Show($"Otrzymano wiadomość w Window: {karta.SerializeToJson()}");
                    this.APP.game.RękaGracza.Add(karta);

                    if (this.APP.game.RękaGracza.Count() == 5) aktualizujKarty();
                    //MessageBox.Show("Talia otrzymana i zrekonstruowana na nowo.");

                });                    

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd podczas deserializacji tali: {ex.Message}");
                }
            }
            else
            {
               MessageBox.Show($"Otrzymano wiadomość w Window: {message}");
            }
        }
        public void Aktualizuj(string message)
        {
            LblTest.Content = message;
        }
        private void aktualizujKarty()
        {
            BtnPlayerCard1.Content = LoadCardImage(this.APP.game.RękaGracza[0].ImagePath);
            BtnPlayerCard2.Content = LoadCardImage(this.APP.game.RękaGracza[1].ImagePath);
            BtnPlayerCard3.Content = LoadCardImage(this.APP.game.RękaGracza[2].ImagePath);
            BtnPlayerCard4.Content = LoadCardImage(this.APP.game.RękaGracza[3].ImagePath);
            BtnPlayerCard5.Content = LoadCardImage(this.APP.game.RękaGracza[4].ImagePath);
        }
    }
}
