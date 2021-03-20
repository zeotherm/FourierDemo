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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace FourierDemo {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        System.Timers.Timer timer = new System.Timers.Timer(1000);
        private double theta = 0;
        private readonly double x_0, y_0, r, deg2rad; // Where the origin is
        public MainWindow() {
            InitializeComponent();
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Enabled = false;
            x_0 = testLine.X1;
            y_0 = testLine.Y1;
            r = testLine.X2 - testLine.X1;
            deg2rad = Math.PI / 180;
        }
        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                //secondHand.Angle = DateTime.Now.Second * 6;
                //minuteHand.Angle = DateTime.Now.Minute * 6;
                //hourHand.Angle = (DateTime.Now.Hour * 30) + (DateTime.Now.Minute * 0.5);
            }));
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e) {
            this.DragMove();
        }

        private void AdvButton_Click(object sender, RoutedEventArgs e) {
            theta = (theta-10*deg2rad) % 360;
            var x = x_0 + r * Math.Cos(theta);
            var y = y_0 + r * Math.Sin(theta);
            testLine.X2 = x;
            testLine.Y2 = y;

        }
    }
}
