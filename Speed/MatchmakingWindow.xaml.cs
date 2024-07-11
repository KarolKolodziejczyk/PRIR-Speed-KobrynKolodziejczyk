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

namespace Speed
{
    public partial class MatchmakingWindow : Window
    {
        public MatchmakingWindow()
        {
            InitializeComponent();
            LoadOpponentImg();
            LoadOpponentIPTxt();
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
            SpeedGameWindow game = new SpeedGameWindow();
            game.ShowDialog();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
