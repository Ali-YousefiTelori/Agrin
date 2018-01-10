using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Agrin.Views.Controls
{
    public partial class LinkProgressBar : ContentView
    {
        public LinkProgressBar()
        {
            //InitializeComponent();
            //backgroundFrame.SizeChanged += LinkProgressBar_SizeChanged;
        }

        private void LinkProgressBar_SizeChanged(object sender, EventArgs e)
        {
            //OnMaximumChanged(this, null, Maximum);
        }

        //public static readonly BindableProperty ProgressProperty = BindableProperty.Create("Progress", typeof(double), typeof(LinkProgressBar), 0.0, propertyChanged: new BindableProperty.BindingPropertyChangedDelegate(OnProgressChanged));
        //public static readonly BindableProperty MaximumProperty = BindableProperty.Create("Maximum", typeof(double), typeof(LinkProgressBar), 100.0, propertyChanged: new BindableProperty.BindingPropertyChangedDelegate(OnMaximumChanged));
        //public static readonly BindableProperty ColorProperty = BindableProperty.Create("Color", typeof(Color), typeof(LinkProgressBar), Color.Red, propertyChanged: new BindableProperty.BindingPropertyChangedDelegate(OnColorChanged));

        //private static void OnColorChanged(BindableObject bindable, object oldValue, object newValue)
        //{
        //    LinkProgressBar control = (LinkProgressBar)(object)bindable;
        //    control.progressFrame.BackgroundColor = (Color)newValue;
        //    control.backgroundFrame.BackgroundColor = (Color)newValue;
        //}

        //private static void OnMaximumChanged(BindableObject bindable, object oldValue, object newValue)
        //{
        //    LinkProgressBar control = (LinkProgressBar)(object)bindable;
        //    OnProgressChanged(bindable, null, control.Progress);
        //}

        //static double minimumWidth = 10;
        //private static void OnProgressChanged(BindableObject bindable, object oldValue, object newValue)
        //{
        //    LinkProgressBar control = (LinkProgressBar)(object)bindable;
        //    double value = (double)newValue;
        //    if (control.Maximum <= 0 || control.Maximum == control.Progress)
        //        control.progressFrame.WidthRequest = control.backgroundFrame.Width;
        //    else if (value == 0)
        //        control.progressFrame.WidthRequest = minimumWidth;
        //    else
        //    {
        //        var t = (control.Maximum / value);
        //        var ctrlValue = t == 0 ? 0 :  100 / t;
        //        control.progressFrame.WidthRequest = (((control.backgroundFrame.Width - minimumWidth) / 100) * ctrlValue) + minimumWidth;

        //    }
        //}

        //public double Progress
        //{
        //    get
        //    {
        //        return (double)GetValue(ProgressProperty);
        //    }
        //    set
        //    {
        //        SetValue(ProgressProperty, value);
        //    }
        //}

        //public double Maximum
        //{
        //    get
        //    {
        //        return (double)GetValue(MaximumProperty);
        //    }
        //    set
        //    {
        //        base.SetValue(MaximumProperty, value);
        //    }
        //}

        //public Color Color
        //{
        //    get
        //    {
        //        return (Color)GetValue(ColorProperty);
        //    }
        //    set
        //    {
        //        base.SetValue(ColorProperty, value);
        //    }
        //}
    }
}
