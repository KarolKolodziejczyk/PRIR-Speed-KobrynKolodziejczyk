using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace Speed
{
    public partial class SpeedGameWindow : Window
    {
        private SpeedGameApp APP;
        private int HoverPixelAmount { get; } = 20;

        public SpeedGameWindow()
        {
            InitializeComponent();

            APP = new SpeedGameApp(this);

            // card test
            BtnPlayerCard1.Content = LoadCardImage("heart", "5");
            BtnPlayerCard2.Content = LoadCardImage("spade", "2");
            BtnPlayerCard3.Content = LoadCardImage("diam", "3");
            BtnPlayerCard4.Content = LoadCardImage("club", "7");
            BtnPlayerCard5.Content = LoadCardImage("superSWAP");

            // enemy card setup
            LblEnemyCard1.Content = LoadCardImage("reverse");
            LblEnemyCard2.Content = LoadCardImage("reverse");
            LblEnemyCard3.Content = LoadCardImage("reverse");
            LblEnemyCard4.Content = LoadCardImage("reverse");
            LblEnemyCard5.Content = LoadCardImage("reverse");

            // static icons setup
            LblEnemy.Content = LoadCardImage("opponent");
            LblDeck1.Content = LoadCardImage("reverse");
            LblDeck2.Content = LoadCardImage("reverse");
            LblTableChangeTop.Content = LoadCardImage("tableChangeIcon");
            LblTableChangeBottom.Content = LoadCardImage("tableChangeIcon");
            LblTableChangeTop.Visibility = Visibility.Hidden;
            LblTableChangeBottom.Visibility = Visibility.Hidden;

            //LblTableCard.Content = LoadCardImage("noCard");
            LblTableCard.Content = LoadCardImage("diam6");

            // UI info setup
            LblDeckRemaining.Content = "x40"; // both players should have 5 cards
            SetPlayerPoints(0);
            SetEnemyPoints(0);
        }

        private void SetPlayerPoints(int count)
        {
            LblPlayerPoints.Content = "Points: " + count;
        }

        private void SetEnemyPoints(int count)
        {
            LblEnemyPoints.Content = "Enemy Points: " + count;
        }

        private Image LoadCardImage(string cardSymbol, string cardNumber = "")
        {
            Image image = new Image();
            var uri = new Uri("pack://application:,,,/cards/" + cardSymbol.ToLower() + cardNumber + ".png");
            var bitmap = new BitmapImage(uri);
            image.Source = bitmap;
            return image;
        }

        private void MoveButton(object sender, int pixelAmount)
        {
            Button button = sender as Button;
            if (button != null)
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

        private void AnimateTableThread(object obj)
        {
            if(obj is Label label)
            {
                label.Dispatcher.Invoke(() => label.Visibility = Visibility.Visible);
                Thread.Sleep(1000);
                label.Dispatcher.Invoke(() => label.Visibility = Visibility.Hidden);
            }
        }

        private void TableChangeEffect()
        {
            Thread t1 = new Thread(AnimateTableThread);
            Thread t2 = new Thread(AnimateTableThread);
            t1.Start(LblTableChangeTop);
            t2.Start(LblTableChangeBottom);
        }

        // SURRENDER OPTION

        private void BtnSurrender_Click(object sender, RoutedEventArgs e)
        {
            // OnPlayerSurrender -> safe delete + thread break

            ResultWindow info = new ResultWindow(ResultType.Surrender);
            info.Owner = this;
            info.ShowDialog();

            this.Close();
        }

        // CARD INTERACTIONS - COMMUNICATION

        private void BtnPlayerCard1_Click(object sender, RoutedEventArgs e)
        {
            // test effect
            TableChangeEffect();
            LblTableCard.Content = LoadCardImage("spadeQ");
            // test end

            // OnCard1Pick
        }

        private void BtnPlayerCard2_Click(object sender, RoutedEventArgs e)
        {
            // OnCard2Pick
        }

        private void BtnPlayerCard3_Click(object sender, RoutedEventArgs e)
        {
            // OnCard3Pick
        }

        private void BtnPlayerCard4_Click(object sender, RoutedEventArgs e)
        {
            // OnCard4Pick
        }

        private void BtnPlayerCard5_Click(object sender, RoutedEventArgs e)
        {
            // OnCard5Pick
        }
    }
}
