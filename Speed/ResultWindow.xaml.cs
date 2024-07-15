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
    public enum ResultType
    {
        Surrender = 0,
        Win = 1,
        Failure = 2 
    }

    public partial class ResultWindow : Window
    {
        public ResultWindow()
        {
            InitializeComponent();
        }

        public ResultWindow(ResultType matchResult)
        {
            InitializeComponent();
            
            switch(matchResult)
            {
                case ResultType.Surrender:
                    break;
                case ResultType.Win:
                    LblResult.Content = "! YOU WON !";
                    break;
                case ResultType.Failure:
                    LblResult.Content = "X ENEMY WON X";
                    break;
                default:
                    LblResult.Content = "ERROR";
                    break;
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
