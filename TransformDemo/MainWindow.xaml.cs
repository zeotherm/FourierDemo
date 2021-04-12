using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace TransformDemo {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private double x0, y0, x, y;
        private double r0 = 50;
        System.Timers.Timer timer = new System.Timers.Timer(100);
        private double dTheta = 3, theta = 0;
        public MainWindow() {
            InitializeComponent();
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Enabled = false;

            x0 = this.CircleCanvas.Width / 2;
            y0 = this.CircleCanvas.Height / 2;
            x = x0;
            y = y0;
            //double theta = 25;
            //for (var i = 0; i < 3; i++) {
            //    var prev_x = x;
            //    var prev_y = y;
            //    int n = 2 * i + 1;
            //    var radius = r0 * (4.0 / (n * Math.PI));
            //    DrawCircle(radius, prev_x, prev_y);
            //    x += radius * Math.Cos(n * theta * Math.PI / 180.0);
            //    y += radius * Math.Sin(n * theta * Math.PI / 180.0);
            //    DrawLine(prev_x, prev_y, x, y);
            //}
            //DrawPoint(x, y);
            // draw a circle
            //DrawCircle(r0, x0, y0);
            //var x1 = x0 + 4/Math.PI * r0 * Math.Cos(45 * Math.PI / 180);
            //var y1 = y0 + 4 / Math.PI * r0 * Math.Sin(45 * Math.PI / 180);
            //DrawLine(x0, y0, x1, y1);
            //DrawCircle(r1, x1, y1);
            //var x2 = x1 + r1 * Math.Cos(120 * Math.PI / 180);
            //var y2 = y1 + r1 * Math.Sin(120 * Math.PI / 180);
            //DrawLine(x1, y1, x2, y2);
            //DrawCircle(r2, x2, y2);
        }
        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                ReDraw();
            }));
        }

        private void ReDraw() {
            this.CircleCanvas.Children.Clear();
            x = x0;
            y = y0;
            theta += dTheta;
            for (var i = 0; i < _n_circles; i++) {
                var prev_x = x;
                var prev_y = y;
                int n = 2 * i + 1;
                var radius = r0 * (4.0 / (n * Math.PI));
                DrawCircle(radius, prev_x, prev_y);
                x += radius * Math.Cos(n * theta * Math.PI / 180.0);
                y += radius * Math.Sin(n * theta * Math.PI / 180.0);
                DrawLine(prev_x, prev_y, x, y);
            }
            DrawPoint(x, y);

        }

        private void DrawCircle(double r, double c_x, double c_y) {
            Ellipse c1 = new Ellipse();
            c1.Height = 2 * r;
            c1.Width = 2 * r;
            c1.StrokeThickness = 2;
            c1.Stroke = Brushes.Black;
            double y_range = this.CircleCanvas.Height;
            
            Canvas.SetTop(c1, y_range- (c_y+r));
            Canvas.SetLeft(c1, c_x-r);
            this.CircleCanvas.Children.Add(c1);
        }

        private void StartButton_Click(object sender, RoutedEventArgs e) {
            timer.Enabled = true;
        }

        private void Step_Click(object sender, RoutedEventArgs e) {
            ReDraw();
        }

        private void DrawPoint(double c_x, double c_y) {
            Ellipse c1 = new Ellipse();
            c1.Height = 5;
            c1.Width = 5;
            c1.StrokeThickness = 2;
            c1.Stroke = Brushes.Red;
            c1.Fill = Brushes.Red;
            double y_range = this.CircleCanvas.Height;

            Canvas.SetTop(c1, y_range - (c_y + 2.5));
            Canvas.SetLeft(c1, c_x - 2.5);
            this.CircleCanvas.Children.Add(c1);

        }

        private void DrawLine(double x0, double y0, double x1, double y1) {
            double y_range = this.CircleCanvas.Height;
            Line l1 = new Line { X1 = x0, Y1 = y_range-y0, X2 = x1, Y2 = y_range-y1, Stroke=Brushes.Black, StrokeThickness=1};
            this.CircleCanvas.Children.Add(l1);
        }

        private void NCircleBox_TextChanged(object sender, TextChangedEventArgs e) {
            TextBox textBox = sender as TextBox;
            if (textBox != null) {
                string theText = textBox.Text;
                CircleText = theText;
            }
            var s = 0;
        }

        private void MySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            Slider thisSlider = sender as Slider;
            if (thisSlider != null) {
                int v = (int)thisSlider.Value;
                CircleText = v.ToString();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private int _n_circles = 2;
        public string CircleText
        {
            get { return _n_circles.ToString(); }
            set { _n_circles = Int32.Parse(value); NotifyPropertyChanged("CircleText"); }
        }



        private void NotifyPropertyChanged(String propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
