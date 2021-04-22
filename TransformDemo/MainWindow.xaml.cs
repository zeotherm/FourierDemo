using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TransformDemo {
    public enum CurveSelection {
        SquareWave,
        TriangleWave,
        SawtoothWave
    }



    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private double x0, y0, x, y;
        private double r0 = 50;
        Queue<double> yPoints, epiPoints;
        List<double> xPoints;
        private CurveSelection curveSelection;
        private double graphY;
        private double circlesY;

        int counter = 0;
        System.Timers.Timer timer = new System.Timers.Timer(100);
        private double dTheta = 3, theta = 0, y_range;
        public MainWindow() {
            InitializeComponent();
            timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
            timer.Enabled = false;
            y_range = this.EpiCycleCanvas.Height;

            x0 = this.EpiCycleCanvas.Width / 2;
            y0 = this.EpiCycleCanvas.Height / 2;
            x = x0;
            y = y0;
            ReDraw();

            var xs = Enumerable.Range(0, 360).ToList();
            var ys = Enumerable.Range(0, 360).ToList(); 
            xPoints = new List<double>();
            for( int i = 0; i < 720; i += (int)dTheta) {
                xPoints.Add(i*0.5);
            }
            epiPoints = new Queue<double>();
            yPoints = new Queue<double>();
        }

        public CurveSelection CurveSelection
        {
            get { return this.curveSelection; }
            set
            {
                this.curveSelection = value;
                this.NotifyPropertyChanged("CurveSelection");
                ReDraw();
                ReDrawGraph();
            }
        }


        void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                ReDraw();
                ReDrawGraph();
            }));
        }

        private void ReDrawGraph() {
            LinePlotCanvas.Children.Clear();
            if (yPoints.Count > xPoints.Count) yPoints.Dequeue();
            if (epiPoints.Count > xPoints.Count) epiPoints.Dequeue();
            
            epiPoints.Enqueue(y_range - y);
            counter += (int)dTheta;
            double yprime;
            switch (curveSelection)
            {
                case CurveSelection.SquareWave: 
                {
                    if (counter % 360 < 180)
                        yPoints.Enqueue(y_range - (y0 + r0));
                    else
                        yPoints.Enqueue(y_range - (y0 - r0));

                    break;
                }
                case CurveSelection.TriangleWave:
                {
                    var x = counter % 360;
                    if( x <= 90) {
                        yprime = x / 90.0 * r0;
                    } else if (x > 90 && x <= 270) {
                        yprime = (1.0 + (90 - x) / 90.0)*r0;
                    } else {
                        yprime = ((x - 270.0) / 90.0 - 1.0)* r0;
                    }
                    yPoints.Enqueue(y_range - (y0 + yprime));
                    break;
                }
                case CurveSelection.SawtoothWave: 
                {
                    // x == -180, y == -r0
                    // x == 179, y = r0

                    // y = (r0- (-r0))/(360)*(x - -180)
                    // 2r0/360*(x+180)
                    // x == 180, y = -r0
                    // x == 359, y ~= 0
                    var x = counter % 360;
                    if (x < 180) {
                        yprime = 2 * r0 / 360.0 * (x + 180.0) - r0;
                    } else if (x == 180) {
                        yprime = -r0;
                    } else {
                        yprime = r0 / 180.0 * (x - 180) - r0;
                    }
                    yPoints.Enqueue(y_range - (y0 + yprime));
                    break;
                }
            }
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

            for (var i = 0; i < _numCircles; i++) {
                var prev_x = x;
                var prev_y = y;
                switch (curveSelection) {
                    case CurveSelection.SquareWave:
                    {
                        int n = 2 * i + 1;
                        var radius = r0 * (4.0 / (n * Math.PI));
                        DrawCircle(radius, prev_x, prev_y);
                        x += radius * Math.Cos(n * theta * Math.PI / 180.0);
                        y += radius * Math.Sin(n * theta * Math.PI / 180.0);
                        DrawLine(prev_x, prev_y, x, y);
                        break;
                    }
                    case CurveSelection.TriangleWave: {
                        int n = 2 * i + 1;
                        var radius = r0 * (8 / (Math.PI * Math.PI * n * n));
                        DrawCircle(radius, prev_x, prev_y);
                        var prefix = Math.Pow(-1, (n - 1) / 2);
                        x += prefix * radius * Math.Cos(n * theta * Math.PI / 180.0);
                        y += prefix * radius * Math.Sin(n * theta * Math.PI / 180.0);
                        DrawLine(prev_x, prev_y, x, y);
                        break;
                    }
                    case CurveSelection.SawtoothWave: {
                        int n = i+1;
                        var radius = 2 * r0 / (Math.PI * n);
                        DrawCircle(radius, prev_x, prev_y);
                        var prefix = Math.Pow(-1.0, n+1);
                        x += prefix * radius * Math.Cos(n * theta * Math.PI / 180.0);
                        y += prefix * radius * Math.Sin(n * theta * Math.PI / 180.0);
                        DrawLine(prev_x, prev_y, x, y);
                        break;
                    }
                }
            }
            DrawPoint(x, y);
            theta += dTheta;
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

        private int _numCircles = 1;
        public double NumberOfCircles
        {
            get { return 1.0*_numCircles; }
            set { _numCircles = (int)value; NotifyPropertyChanged("CircleText"); }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e) {
            timer.Enabled = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
