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
    public partial class SpeedGameWindow : Window
    {
        private SpeedGameApp APP;

        public SpeedGameWindow()
        {
            InitializeComponent();

            APP = new SpeedGameApp(this);
        }

        private void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            // communication
        }
    }
}
