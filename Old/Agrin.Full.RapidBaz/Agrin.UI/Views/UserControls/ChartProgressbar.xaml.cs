using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Agrin.UI.Views.UserControls
{
    /// <summary>
    /// Interaction logic for ChartProgressbar.xaml
    /// </summary>
    public partial class ChartProgressbar : UserControl
    {
        public ChartProgressbar()
        {
            try
            {
                InitializeComponent();
            }
            catch
            {

            }
            //if (MainWindow.This != null)
            //{
            //    MaximumProgressbar = 100;
            //    AddPoint(40);
            //    AddPoint(60);
            //    AddPoint(50);
            //    AddPoint(80);
            //    AddPoint(90);
            //    AddPoint(50);
            //    AddPoint(40);
            //    AddPoint(30);
            //    AddPoint(20);
            //    AddPoint(80);
            //    AddPoint(90);
            //    AddPoint(20);
            //    Value = 60;
            //}
        }

        public static readonly DependencyProperty MaximumProgressbarProperty = DependencyProperty.Register("MaximumProgressbar", typeof(double), typeof(ChartProgressbar), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (sender, e) =>
        {
            ((ChartProgressbar)sender).MaximumProgressbar = (double)e.NewValue;
        }));
        public static readonly DependencyProperty AddPointValueProperty = DependencyProperty.Register("AddPointValue", typeof(double), typeof(ChartProgressbar), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (sender, e) =>
        {
            ((ChartProgressbar)sender).AddPointValue = (double)e.NewValue;
        }));
        public static readonly DependencyProperty ListPointValuesProperty = DependencyProperty.Register("ListPointValues", typeof(List<double>), typeof(ChartProgressbar), new FrameworkPropertyMetadata(new List<double>(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (sender, e) =>
        {
            ((ChartProgressbar)sender).ListPointValues = (List<double>)e.NewValue;
            ((ChartProgressbar)sender).Value = ((ChartProgressbar)sender).Value;
        }));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(ChartProgressbar), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (sender, e) =>
        {
            ((ChartProgressbar)sender).Value = (double)e.NewValue;
        }));
        public double MaximumProgressbar
        {
            get { return (double)this.GetValue(MaximumProgressbarProperty); }
            set
            {
                this.SetValue(MaximumProgressbarProperty, value);
            }
        }

        public double Value
        {
            get { return (double)this.GetValue(ValueProperty); }
            set
            {
                this.SetValue(ValueProperty, value);
                ValueChanged();
            }
        }
        public double AddPointValue
        {
            get { return (double)this.GetValue(AddPointValueProperty); ; }
            set
            {
                this.SetValue(AddPointValueProperty, value);
                AddPoint(value);
            }
        }
        public List<double> ListPointValues
        {
            get { return (List<double>)this.GetValue(ListPointValuesProperty); ; }
            set
            {
                ClearPoints();
                this.SetValue(ListPointValuesProperty, value);
                AddRangePoint(value);
                //UserControl_SizeChanged(null, null);
            }
        }
        List<double> points = new List<double>();
        List<double> averagePoints = new List<double>();
        List<double> drawPoints = new List<double>();
        double maximumPoint;
        double maximumAveragePoint;

        void Get100LastItem()
        {
            if (points.Count <= 100)
            {
                drawPoints.Clear();
                drawPoints.AddRange(points);
                averagePoints = drawPoints;
                return;
            }
            List<double> items = points.Skip(points.Count - 100).ToList();
            //if (items.First() != 0)
            //items.Insert(0, items.First());
            averagePoints = drawPoints = items;
            maximumPoint = items.Max();
        }

        void Get100Miangin()
        {
            int jam = points.Count / 100;
            if (jam <= 1)
            {
                drawPoints.Clear();
                drawPoints.AddRange(points);
                averagePoints = drawPoints;
                return;
            }

            List<double> mm = new List<double>();
            int i = 0;
            double mainPoint = 0;
            foreach (var item in points)
            {
                i++;
                mainPoint += item;
                if (i >= jam)
                {
                    mm.Add(mainPoint / jam);
                    i = 0;
                    mainPoint = 0;
                }
            }
            averagePoints = drawPoints = mm;
            maximumAveragePoint = maximumPoint = mm.Max();
        }
        void HeightPointChanges()
        {
            List<double> mm = new List<double>();
            double dahdarsad = maximumPoint / 10;
            double height = gridProgress.ActualHeight / (maximumPoint + dahdarsad);
            foreach (var item in averagePoints)
            {
                mm.Add(item * height);
            }
            drawPoints = mm;
        }

        void ValueChanged()
        {
            if (points.Count == 0)
                return;
            DoubleAnimation animation;
            if (Value == 0 || MaximumProgressbar < 0)
            {
                if (gridProgress.Width != 0 && !double.IsNaN(gridProgress.Width))
                {
                    animation = new DoubleAnimation(0.0, new Duration(TimeSpan.FromMilliseconds(300)));
                    animation.DecelerationRatio = 1;
                    gridProgress.BeginAnimation(Grid.WidthProperty, animation);
                }
                return;
            }
            double newValue = MaximumProgressbar / Value;
            newValue = mainGrid.ActualWidth / newValue;
            //gridProgress.Width = newValue;
            if (double.IsNaN(newValue) || double.IsInfinity(newValue))
                return;
            if (double.IsNaN(gridProgress.Width))
                gridProgress.Width = 0.0;
            animation = new DoubleAnimation(newValue, new Duration(TimeSpan.FromMilliseconds(300)));
            animation.DecelerationRatio = 1;
            animation.CurrentTimeInvalidated += animation_CurrentTimeInvalidated;
            animation.Completed += animation_CurrentTimeInvalidated;
            gridProgress.BeginAnimation(Grid.WidthProperty, animation);
            //gridProgress.Width = newValue;
            //maximumAveragePoint = gridProgress.ActualHeight;
            //RefreshData();
        }

        void animation_CurrentTimeInvalidated(object sender, EventArgs e)
        {
            maximumAveragePoint = gridProgress.ActualHeight;
            RefreshData();
        }
        object lockobj = new object();
        public void RefreshData()
        {
            maximumAveragePoint = gridProgress.ActualHeight;
            Get100LastItem();
            HeightPointChanges();
            mainLines.Segments.Clear();
            mainLines.StartPoint = new Point(0, maximumAveragePoint);
            double jam = (gridProgress.Width - 1) / (drawPoints.Count - 1);
            double letPoint = 0;
            if (drawPoints.Count > 0)
            {
                mainLines.Segments.Add(new LineSegment(new Point(0, maximumAveragePoint - drawPoints.First()), true) { IsSmoothJoin = true });

                //StringBuilder bbbb = new StringBuilder();
                foreach (var item in drawPoints.Skip(1))
                {
                    letPoint += jam;
                    double value = maximumAveragePoint - item;
                    BezierSegment seg = new BezierSegment(new Point(letPoint, value), new Point(letPoint, value), new Point(letPoint, value), true) { IsSmoothJoin = true };
                    mainLines.Segments.Add(seg);
                    //bbbb.AppendLine("<BezierSegment Point1=\"" + letPoint + "," + (valueGhable + normal) + "\" Point2=\"" + (letPoint + 2) + "," + (value - normal) + "\" Point3=\"" + (letPoint + 4) + "," + value + "\"/>");
                }
            }

            //string q = bbbb.ToString().Replace("/",".");
            mainLines.Segments.Add(new LineSegment(new Point(letPoint, maximumAveragePoint), true) { IsSmoothJoin = true });
        }

        double heightGhabl = 0;
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (points.Count == 0)
                return;
            if (heightGhabl != this.ActualHeight && averagePoints.Count > 0)
                SetCurrentPoint(averagePoints.Last());
            heightGhabl = this.ActualHeight;
            ValueChanged();
            //maximumAveragePoint = gridProgress.ActualHeight;
            //RefreshData();
        }

        void SetCurrentPoint(double current)
        {
            double dahdarsad = maximumPoint / 10;
            if (current == 0)
            {
                current = this.ActualHeight;
                return;
            }
            else
                current = this.ActualHeight - ((this.ActualHeight / (maximumPoint + dahdarsad)) * current);
            if (double.IsNaN(current) || double.IsInfinity(current))
                return;
            DoubleAnimation animation = new DoubleAnimation(current, new Duration(TimeSpan.FromMilliseconds(500)));
            currentPoint.BeginAnimation(Line.Y1Property, animation);
            ThicknessAnimation tAnimation = new ThicknessAnimation(new Thickness(0, current - txt_currentPoint.ActualHeight, 0, 0), new Duration(TimeSpan.FromMilliseconds(500)));
            txt_currentPoint.BeginAnimation(TextBlock.MarginProperty, tAnimation);
        }

        public void AddPoint(double newValue)
        {
            txt_currentPoint.Visibility = System.Windows.Visibility.Visible;
            if (newValue > maximumPoint)
                maximumPoint = newValue;
            points.Add(newValue);
            drawPoints.Clear();
            drawPoints.AddRange(points);
            RefreshData();
            SetCurrentPoint(averagePoints.Last());
        }

        public void AddRangePoint(List<double> newValues)
        {
            txt_currentPoint.Visibility = System.Windows.Visibility.Visible;
            foreach (var item in newValues)
            {
                if (item > maximumPoint)
                    maximumPoint = item;
            }
            points.AddRange(newValues);
            drawPoints.Clear();
            drawPoints.AddRange(points);
            RefreshData();
            if (averagePoints.Count > 0)
                SetCurrentPoint(averagePoints.Last());
        }

        public void ClearPoints()
        {
            txt_currentPoint.Visibility = System.Windows.Visibility.Collapsed;
            points.Clear();
            averagePoints.Clear();
            drawPoints.Clear();
            maximumPoint = 0;
        }
    }
}
