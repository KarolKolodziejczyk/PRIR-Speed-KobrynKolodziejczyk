using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Speed.Backend;

namespace Speed
{
    public partial class SpeedGameWindow : Window
    {
        private SpeedGameApp APP;
        private int HoverPixelAmount { get; } = 20;

        public SpeedGameWindow(string IP)
        {
            
            InitializeComponent();
            Aktualizuj("1");
            APP = new SpeedGameApp(IP);

            // Testowe karty
            BtnPlayerCard1.Content = LoadCardImage("heart", "5");
            BtnPlayerCard2.Content = LoadCardImage("spade", "2");
            BtnPlayerCard3.Content = LoadCardImage("diam", "3");
            BtnPlayerCard4.Content = LoadCardImage("club", "7");
            BtnPlayerCard5.Content = LoadCardImage("superSWAP");
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
            MessageBox.Show("Wysylam");
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

                //MessageBox.Show($"Otrzymano wiadomość w Window: {message}");

        }
        public void Aktualizuj(string message)
        {
            LblTest.Content = message;
        }
    }
}
