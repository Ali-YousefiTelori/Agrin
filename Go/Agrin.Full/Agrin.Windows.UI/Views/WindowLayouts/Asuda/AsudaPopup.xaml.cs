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
using System.Windows.Shapes;

namespace Agrin.Windows.UI.Views.WindowLayouts.Asuda
{
    public enum AsudaPopupTypeEnum
    {
        Complete,
        Error,
        Warning,
        Information
    }

    public class AsudaPopupInfo
    {
        public Action OpenLocationAction { get; set; }
        public Action OpenFileAction { get; set; }
        public Action RetryAction { get; set; }

        public object Data { get; set; }

        public AsudaPopupTypeEnum Mode { get; set; }
        public string Message { get; set; }

    }
    /// <summary>
    /// Interaction logic for AsudaPopup.xaml
    /// </summary>
    public partial class AsudaPopup : Window
    {
        public AsudaPopup()
        {
            InitializeComponent();
        }

        double width = 185, height = 30;
        static AsudaPopup currentWindow = null;
        public static void ShowPopup(AsudaPopupInfo popUp)
        {
            if (currentWindow == null)
            {
                currentWindow = new AsudaPopup();
                //currentWindow.Activated += (s, e) =>
                //{
                //    BasketReceiverWindow.currentPinnedWindow.Activate();
                //};
            }
            //currentWindow.popUpGrid.Margin = new Thickness(0, SystemParameters.WorkArea.Height / 2 + 86, 40, 0);
            currentWindow.popUpGrid.Margin = new Thickness(0, SystemParameters.WorkArea.Height / 2 + 51, 16, 0); ;
            currentWindow.line.X2 = currentWindow.line.X1 = 16;
            currentWindow.line.Y2 = currentWindow.line.Y1 = SystemParameters.WorkArea.Height / 2 + 51;

            //currentWindow.line.X2 = currentWindow.popUpGrid.Margin.Right + 5;
            //currentWindow.line.Y2 = currentWindow.popUpGrid.Margin.Top + 4;

            currentWindow.Show();
            currentWindow.ShowPopupAnimating();
        }

        public void RefreshLine()
        {
            currentWindow.line.X2 = currentWindow.popUpGrid.Margin.Right + 5 - ((TranslateTransform)popUpGrid.RenderTransform).X;
            currentWindow.line.Y2 = currentWindow.popUpGrid.Margin.Top + 4 + ((TranslateTransform)popUpGrid.RenderTransform).Y;
        }

        public void ShowPopupAnimating()
        {
            canMove = false;
            Duration duration = new Duration(new TimeSpan(0, 0, 0, 0, 300));
            var marginTop = SystemParameters.WorkArea.Height / 2 + 86;
            var marginRight = 40.0;
            ThicknessAnimation anim = new ThicknessAnimation(new Thickness(0, marginTop, marginRight, 0), duration);
            anim.DecelerationRatio = 1;
            anim.Completed += (s, ee) =>
            {
                popUpGrid.ApplyAnimationClock(Grid.MarginProperty, null);
                popUpGrid.Margin = new Thickness(0, marginTop, marginRight, 0);
            };
            popUpGrid.BeginAnimation(Grid.MarginProperty, anim);

            var X2 = marginRight + 5;
            var Y2 = marginTop + 4;

            var x2anim = new DoubleAnimation(X2, duration);
            var y2anim = new DoubleAnimation(Y2, duration);
            x2anim.DecelerationRatio = 1;
            y2anim.DecelerationRatio = 1;

            x2anim.Completed += (s, ee) =>
            {
                line.ApplyAnimationClock(Line.X2Property, null);
                line.X2 = X2;
            };
            y2anim.Completed += (s, ee) =>
            {
                line.ApplyAnimationClock(Line.Y2Property, null);
                line.Y2 = Y2;
                var widthanim = new DoubleAnimation(width, duration);
                var heightanim = new DoubleAnimation(height, duration);
                widthanim.DecelerationRatio = 1;
                heightanim.DecelerationRatio = 1;
                widthanim.Completed += (ss, eee) =>
                {
                    popUpGrid.ApplyAnimationClock(Grid.WidthProperty, null);
                    popUpGrid.Width = width;
                };
                heightanim.Completed += (ss, eee) =>
                {
                    popUpGrid.ApplyAnimationClock(Grid.HeightProperty, null);
                    popUpGrid.Height = height;
                    canMove = true;
                };
                popUpGrid.BeginAnimation(Grid.WidthProperty, widthanim);
                popUpGrid.BeginAnimation(Grid.HeightProperty, heightanim);
            };
            line.BeginAnimation(Line.X2Property, x2anim);
            line.BeginAnimation(Line.Y2Property, y2anim);

        }

        public void HidePopupAnimating()
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void popUpGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!canMove)
                return;
            anchorPoint = e.GetPosition(null);
            popUpGrid.CaptureMouse();
            isInDrag = true;
            e.Handled = true;
        }

        Point anchorPoint;
        Point currentPoint;
        bool isInDrag = false;

        private TranslateTransform transform = new TranslateTransform();

        private void popUpGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (isInDrag)
            {
                currentPoint = e.GetPosition(null);

                transform.X += currentPoint.X - anchorPoint.X;
                transform.Y += (currentPoint.Y - anchorPoint.Y);
                popUpGrid.RenderTransform = transform;
                anchorPoint = currentPoint;
                RefreshLine();
            }
        }

        bool canMove = true;
        private void popUpGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isInDrag)
            {
                popUpGrid.ReleaseMouseCapture();
                isInDrag = false;
                e.Handled = true;
                canMove = false;
                var T = ((TranslateTransform)popUpGrid.RenderTransform);
                Duration duration = new Duration(new TimeSpan(0, 0, 0, 0, 500));
                DoubleAnimation anim = new DoubleAnimation(0, duration);
                anim.DecelerationRatio = 1;
                anim.Completed += (s, ee) =>
                {
                    T.ApplyAnimationClock(TranslateTransform.XProperty, null);
                    T.ApplyAnimationClock(TranslateTransform.YProperty, null);
                    T.X = 0;
                    T.Y = 0;
                    canMove = true;
                };
                T.BeginAnimation(TranslateTransform.XProperty, anim);
                T.BeginAnimation(TranslateTransform.YProperty, anim);

                var X2 = currentWindow.popUpGrid.Margin.Right + 5;
                var Y2 = currentWindow.popUpGrid.Margin.Top + 4;

                var x2anim = new DoubleAnimation(X2, duration);
                var y2anim = new DoubleAnimation(Y2, duration);
                x2anim.DecelerationRatio = 1;
                y2anim.DecelerationRatio = 1;

                x2anim.Completed += (s, ee) =>
                {
                    line.ApplyAnimationClock(Line.X2Property, null);
                    line.X2 = X2;
                };
                y2anim.Completed += (s, ee) =>
                {
                    line.ApplyAnimationClock(Line.Y2Property, null);
                    line.Y2 = Y2;
                };
                line.BeginAnimation(Line.X2Property, x2anim);
                line.BeginAnimation(Line.Y2Property, y2anim);

            }
        }
    }
}
