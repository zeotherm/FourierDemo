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
        Queue<double> yPoints, epiPoints;
        List<int> xPoints;

        int counter = 0;
        System.Timers.Timer timer = new System.Timers.Timer(100);
        private double dTheta = 3, theta = 0, y_range;
        public MainWindow() {
            InitializeComponent();
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Enabled = false;
            y_range = this.EpiCycleCanvas.Height;

            x0 = this.EpiCycleCanvas.Width / 2;
            y0 = this.EpiCycleCanvas.Height / 2;
            x = x0;
            y = y0;
            ReDraw();

            var xs = Enumerable.Range(0, 360).ToList();
            var ys = Enumerable.Range(0, 360).ToList(); // new List<double> { };
            //PointCollection ps = new PointCollection();
            //point_q = new Queue<Point>();
            //foreach (var i in xs) {
            //    if (i < 90 || i >= 270) {
            //        point_q.Enqueue(new Point(i, y0 - r0));
            //        //ps.Add(new Point(i, y0-r0));
            //    } else
            //        point_q.Enqueue(new Point(i, y0 + r0));
            //        //ps.Add(new Point(i, y0+r0));
            //}
            xPoints = new List<int>();
            for( int i = 0; i < 360; i += (int)dTheta) {
                xPoints.Add(i);
            }
            //yPoints = new Queue<double>();
            //foreach( int i in xPoints) {
            //    if (i < 180 ) {
            //        yPoints.Enqueue(y0 - r0);
            //    } else
            //        yPoints.Enqueue(y0 + r0);
            //}
            //var points = xPoints.Zip(yPoints, (x, y) => new Point(x, y));
            ////for ( int i = 0; i < 360; i++) {
            ////    ps.Add(new Point(i, 20));
            ////}
            //Polyline polyline = new Polyline
            //{
            //    StrokeThickness = 1,
            //    Stroke = Brushes.Black,
            //    Points = new PointCollection(points)
            //};

            //LinePlotCanvas.Children.Add(polyline);
            epiPoints = new Queue<double>();
            yPoints = new Queue<double>();
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                ReDraw();
                ReDrawGraph();
            }));
        }

        private void ReDrawGraph() {
            LinePlotCanvas.Children.Clear();
            if(yPoints.Count > xPoints.Count) yPoints.Dequeue();
            if (epiPoints.Count > xPoints.Count) epiPoints.Dequeue();
            epiPoints.Enqueue(y_range - y);
            counter += (int)dTheta;
            if (counter % 360 < 180)
                yPoints.Enqueue(y_range - (y0 + r0));
            else
                yPoints.Enqueue(y_range - (y0 - r0));

            Polyline polyline = new Polyline
            {
                StrokeThickness = 1,
                Stroke = Brushes.Black,
                Points = new PointCollection(xPoints.Zip(yPoints, (x, y) => new Point(x, y)))
            };
            LinePlotCanvas.Children.Add(polyline);
            Polyline epiOutput = new Polyline
            {
                StrokeThickness = 2,
                Stroke = Brushes.Red,
                Points = new PointCollection(xPoints.Zip(epiPoints, (x, y) => new Point(x, y)))
            };
            LinePlotCanvas.Children.Add(epiOutput);
        }

        private void ReDraw() {
            EpiCycleCanvas.Children.Clear();
            x = x0;
            y = y0;
            theta += dTheta;
            for (var i = 0; i < _numCircles; i++) {
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
            Ellipse c1 = new Ellipse
            {
                Height = 2 * r,
                Width = 2 * r,
                StrokeThickness = 2,
                Stroke = Brushes.Black
            };
            
            Canvas.SetTop(c1, y_range- (c_y+r));
            Canvas.SetLeft(c1, c_x-r);
            this.EpiCycleCanvas.Children.Add(c1);
        }

        private void StartButton_Click(object sender, RoutedEventArgs e) {
            timer.Enabled = true;
        }

        private void Step_Click(object sender, RoutedEventArgs e) {
            ReDraw();
        }

        private void DrawPoint(double c_x, double c_y) {
            Ellipse c1 = new Ellipse
            {
                Height = 5,
                Width = 5,
                StrokeThickness = 2,
                Stroke = Brushes.Red,
                Fill = Brushes.Red
            };

            Canvas.SetTop(c1, y_range - (c_y + 2.5));
            Canvas.SetLeft(c1, c_x - 2.5);
            this.EpiCycleCanvas.Children.Add(c1);
        }

        private void DrawLine(double x0, double y0, double x1, double y1) {
            Line l1 = new Line { X1 = x0, Y1 = y_range-y0, X2 = x1, Y2 = y_range-y1, Stroke=Brushes.Black, StrokeThickness=1};
            this.EpiCycleCanvas.Children.Add(l1);
        }

        private int _numCircles = 2;
        public double NumberOfCircles
        {
            get { return 1.0*_numCircles; }
            set { _numCircles = (int)value; NotifyPropertyChanged("CircleText"); }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e) {
            timer.Enabled = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
