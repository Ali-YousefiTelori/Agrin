using Agrin.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Agrin.Windows.UI.Views.Controls
{
    /// <summary>
    /// Interaction logic for NumbericUpDown.xaml
    /// </summary>
    public partial class NumbericUpDown : UserControl
    {
        public NumbericUpDown()
        {
            InitializeComponent();
            this.Loaded += TimePicker_Loaded;
            txtMain.Text = Value.ToString();
        }

        private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            NumbericUpDown c = sender as NumbericUpDown;
            if (c != null)
            {
                c.Value = (int)e.NewValue;
            }
        }

        private static void OnPropertyChanged2(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            NumbericUpDown c = sender as NumbericUpDown;
            if (c != null)
            {
                if (e.Property == MinimumProperty)
                {
                    c.Minimum = (int)e.NewValue;
                }
                else if (e.Property == MaximumProperty)
                {
                    c.Maximum = (int)e.NewValue;
                }
            }
        }

        public NumbericUpDown SumNumbricUpDown
        {
            get { return (NumbericUpDown)GetValue(SumNumbricUpDownProperty); }
            set { SetValue(SumNumbricUpDownProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SumNumbricUpDown.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SumNumbricUpDownProperty =
            DependencyProperty.Register("SumNumbricUpDown", typeof(NumbericUpDown), typeof(NumbericUpDown), new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for Maximum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(int), typeof(NumbericUpDown), new FrameworkPropertyMetadata(int.MaxValue, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPropertyChanged2));

        // Using a DependencyProperty as the backing store for Minimum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(int), typeof(NumbericUpDown), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPropertyChanged2));

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int), typeof(NumbericUpDown), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPropertyChanged));


        public int Maximum
        {
            get { return (int)GetValue(MaximumProperty); }
            set
            {
                SetValue(MaximumProperty, value);
            }
        }

        public int Minimum
        {
            get { return (int)GetValue(MinimumProperty); }
            set
            {
                SetValue(MinimumProperty, value);

                //if (Value > Maximum)
                //{
                //    Value = Maximum;
                //}
                
                //if (Value < Minimum)
                //{
                //    Value = Minimum;
                //}
            }
        }

        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set
            {
                if (value > Maximum)
                {
                    value = Minimum;
                    if (SumNumbricUpDown != null)
                        SumNumbricUpDown.Value--;
                }
                if (value < Minimum)
                {
                    value = Maximum;
                    if (SumNumbricUpDown != null)
                        SumNumbricUpDown.Value++;
                }

                SetValue(ValueProperty, value);
                string strValue = Value.ToString();
                if (txtMain.Text != strValue)
                    txtMain.Text = strValue;
            }
        }

        void TimePicker_Loaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window == null)
                return;
            MouseMove -= TimePicker_MouseMove;
            window.Closing -= window_Closing;
            window.Deactivated -= window_Deactivated;

            MouseMove += TimePicker_MouseMove;
            window.Closing += window_Closing;
            window.Deactivated += window_Deactivated;
        }

        void window_Deactivated(object sender, EventArgs e)
        {
            CloseCaptureMouse();
        }

        void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CloseCaptureMouse();
        }

        double lastX = 0;
        void TimePicker_MouseMove(object sender, MouseEventArgs e)
        {
            if (!canMove)
                return;
            var p = e.GetPosition(this);
            var newVal = p.Y - lastX;
            if (newVal == 0)
                return;
            if (lastX == 0)
            {
                lastX = p.Y;
                return;
            }
            if (newVal <= 0)
                Value++;
            else
                Value--;
            lastX = p.Y;
        }



        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            int newValue = 0;
            if (int.TryParse(txtMain.Text, out newValue))
                Value = newValue;
            else
                txtMain.Text = Value.ToString();
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab || Keyboard.Modifiers == ModifierKeys.Control || e.Key == Key.Back)
                return;
            if ((int)e.Key < 74 || (int)e.Key > 83)
            {
                if (Keyboard.Modifiers == ModifierKeys.Shift || (int)e.Key < 34 || (int)e.Key > 43)
                    e.Handled = true;
                else
                    e.Handled = false;
            }
            else
                e.Handled = false;
            if (e.Key == Key.Down)
            {
                Value--;
            }
            else if (e.Key == Key.Up)
            {
                Value++;
            }
        }

        bool isUp = true;
        private void Button_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isUp = false;
            isCapture = false;
            Value++;
            AsyncActions.Action(() =>
            {
                System.Threading.Thread.Sleep(500);
                while (!isUp)
                {
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        Value++;
                    }));
                    System.Threading.Thread.Sleep(50);
                }
            });
        }

        private void txtMain_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                Value++;
            }
            else if (e.Delta < 0)
            {
                Value--;
            }
        }

        ManualResetEvent resetEvent = new ManualResetEvent(false);
        private void Button_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            isUp = false;
            isCapture = false;
            Value--;
            AsyncActions.Action(() =>
            {
                resetEvent.WaitOne(500);
                resetEvent.Reset();
                while (!isUp)
                {
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        Value--;
                    }));
                    System.Threading.Thread.Sleep(50);
                }
            });
        }

        bool isCapture = false;
        bool canMove = false;

        private void UserControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isCapture = true;
            AsyncActions.Action(() =>
            {
                System.Threading.Thread.Sleep(100);
                if (isCapture)
                {
                    canMove = true;
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        this.CaptureMouse();
                        this.Cursor = Cursors.SizeNS;
                    }));
                }
            });
        }

        private void UserControl_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CloseCaptureMouse();
        }

        void CloseCaptureMouse()
        {
            isUp = true;
            resetEvent.Reset();
            lastX = 0;
            isCapture = false;
            canMove = false;
            this.ReleaseMouseCapture();
            this.Cursor = Cursors.Arrow;
        }

        private void txtMain_GotFocus(object sender, RoutedEventArgs e)
        {
            txtMain.SelectAll();
        }

    }
}