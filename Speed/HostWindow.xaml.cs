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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Speed
{
    public partial class HostWindow : Window
    {
        public HostWindow()
        {
            InitializeComponent();

            // loading roating image
            Image imgLogotype = new Image();
            var uri = new Uri("pack://application:,,,/cards/loadingIcon.png");
            var bitmap = new BitmapImage(uri);
            imgLogotype.Source = bitmap;
            LblRotIcon.Content = imgLogotype;

            StartRotationAnimation();
        }

        private void StartRotationAnimation()
        {
            RotateTransform rotateTransform = new RotateTransform();
            LblRotIcon.RenderTransform = rotateTransform;
            LblRotIcon.RenderTransformOrigin = new Point(0.5, 0.5);

            // time = 360 degrees / 20 degrees per second
            DoubleAnimation rotateAnimation = new DoubleAnimation
            {
                From = 0,
                To = 360,
                Duration = new Duration(TimeSpan.FromSeconds(360.0 / 60.0)),
                RepeatBehavior = RepeatBehavior.Forever
            };

            rotateTransform.BeginAnimation(RotateTransform.AngleProperty, rotateAnimation);
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
