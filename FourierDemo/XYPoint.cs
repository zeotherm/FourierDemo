using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;

namespace FourierDemo {
    public class XYPoint {
        private double x_pos;

        public double X
        {
            get { return x_pos; }
            set { x_pos = value; }
        }
        private double y_pos;

        public double Y
        {
            get { return y_pos; }
            set { y_pos = value; }
        }
        public ObservableCollection<DataPoint> Points {
            get;
            set;
        }

        public string Title { get; set; }
        public XYPoint() {
            this.Title = "my attempt";
            var x = new List<DataPoint>
            {
                new DataPoint(0,0),
                new DataPoint(4,4)
            };

            this.Points = new ObservableCollection<DataPoint>(x);
        }
    }
}
