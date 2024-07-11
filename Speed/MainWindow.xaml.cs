using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Speed
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // loading logotype
            Image imgLogotype = new Image();
            var uri = new Uri("pack://application:,,,/cards/logotype.png");
            var bitmap = new BitmapImage(uri);
            imgLogotype.Source = bitmap;
            LblLogotype.Content = imgLogotype;
        }

        private void BtnQuit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnRules_Click(object sender, RoutedEventArgs e)
        {
            // show rules window
            // content: available normal cards
            //          special cards
            //          game rules - how it works

            this.Hide();
            RulesWindow rules = new RulesWindow();
            rules.ShowDialog();
            this.Show();
        }

        private void BtnOptions_Click(object sender, RoutedEventArgs e)
        {
            // Karol chciales wiec ci zostawiam to !!!

            this.Hide();
            OptionsWindow options = new OptionsWindow();
            options.ShowDialog();
            this.Show();
        }

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            MatchmakingWindow game = new MatchmakingWindow();
            game.ShowDialog();
            this.Show();
        }
    }
}