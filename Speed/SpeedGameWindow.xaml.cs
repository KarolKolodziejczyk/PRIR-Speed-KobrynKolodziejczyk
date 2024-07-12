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

        public SpeedGameWindow(string IP)
        {
            InitializeComponent();

            APP = new SpeedGameApp(this, IP);

            // card test
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
        }

        private Image LoadCardImage(string cardSymbol, string cardNumber = "")
        {
            Image image = new Image();
            var uri = new Uri("pack://application:,,,/cards/" + cardSymbol.ToLower() + cardNumber + ".png");
            var bitmap = new BitmapImage(uri);
            image.Source = bitmap;
            return image;
        }

        private void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            // communication
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

        private void BtnPlayerCard1_Click(object sender, RoutedEventArgs e)
        {
            //
        }
    }
}
