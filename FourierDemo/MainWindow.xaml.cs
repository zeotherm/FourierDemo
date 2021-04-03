using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using OxyPlot;

namespace FourierDemo {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        System.Timers.Timer timer = new System.Timers.Timer(100);
        private double theta = 0, N = 3;
        private readonly double x_0, y_0, r, deg2rad, zero_point; // Where the origin is
        private XYPoint p;
        private PlotViewModel pvm;
        public MainWindow() {
            InitializeComponent();
            this.pvm = new PlotViewModel();
            this.DataContext = this.pvm; // new PlotViewModel();
            this.Title = "My Plot";
            //this.Points = 

            p = new XYPoint();

            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Enabled = false;
            x_0 = testLine.X1;
            y_0 = testLine.Y1;
            zero_point = y_0;
            r = testLine.X2 - testLine.X1;
            deg2rad = Math.PI / 180;
        }

        private void TimerButton_Click(object sender, RoutedEventArgs e) {
            timer.Enabled = !timer.Enabled;
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                ReDraw();
            }));
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            var old_p = p.Points;
            var old_x1 = old_p[0].X;
            var old_y1 = old_p[0].Y;
            var old_x2 = old_p[1].X;
            var old_y2 = old_p[1].Y;

            var ps = new List<DataPoint>
                              {
                                  new DataPoint(2, 3),
                                  new DataPoint(4, 5)
                              };


            p.Points = new ObservableCollection<DataPoint>(ps);
            p.Title = p.Title + "_bark!";

            return;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e) {
            this.DragMove();
        }

        private void AdvButton_Click(object sender, RoutedEventArgs e) {
            ReDraw();
        }

        private void ReDraw() {
            var xy = ComputeNewCoords();
            testLine.X2 = xy.Item1;
            testLine.Y2 = xy.Item2;
            //mainCanvas.Children.Remove(yVal);
            Canvas.SetTop(yVal, testLine.Y2 - yVal.Height/2);
            pvm.OnNewData(zero_point - xy.Item2);

        }
        private Tuple<double, double> ComputeNewCoords() {
            //theta = (theta - N * deg2rad) % 360;
            theta += N * deg2rad;
            var cosTheta = Math.Cos(theta);
            var sinTheta = Math.Sin(theta);
            var x = x_0 + r * cosTheta;
            var y = y_0 - r * sinTheta;
            return Tuple.Create<double, double>(x, y);
        }

    }

    
}
