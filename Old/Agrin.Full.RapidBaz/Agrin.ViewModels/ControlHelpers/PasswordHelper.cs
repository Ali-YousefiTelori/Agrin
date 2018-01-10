using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Agrin.ViewModels.ControlHelpers
{
    public class PasswordHelper
    {
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached("Password",
            typeof(object), typeof(PasswordHelper),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPasswordPropertyChanged));

        private static readonly DependencyProperty IsUpdatingProperty =
           DependencyProperty.RegisterAttached("IsUpdating", typeof(bool),
           typeof(PasswordHelper));

        public static string GetPassword(DependencyObject dp)
        {
            return (string)dp.GetValue(PasswordProperty);
        }

        public static void SetPassword(DependencyObject dp, object value)
        {
            dp.SetValue(PasswordProperty, value);
        }

        private static bool GetIsUpdating(DependencyObject dp)
        {
            return (bool)dp.GetValue(IsUpdatingProperty);
        }

        private static void SetIsUpdating(DependencyObject dp, bool value)
        {
            dp.SetValue(IsUpdatingProperty, value);
        }

        private static void OnPasswordPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            passwordBox.PasswordChanged -= PasswordChanged;

            if (!(bool)GetIsUpdating(passwordBox))
            {
                passwordBox.Password = (string)e.NewValue;
            }
            passwordBox.PasswordChanged += PasswordChanged;
            ChangedPassword(passwordBox);
        }

        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            SetIsUpdating(passwordBox, true);
            SetPassword(passwordBox, passwordBox.Password);
            SetIsUpdating(passwordBox, false);
            ChangedPassword(passwordBox);
        }

        static void ChangedPassword(PasswordBox passwordBox)
        {
            TextBlock textBlock = null;
            if (passwordBox.Template == null)
            {
                passwordBox.Loaded -= passwordBox_Loaded;
                passwordBox.Loaded += passwordBox_Loaded;
            }

            

            if (passwordBox.Template != null)
            {
                passwordBox.ApplyTemplate();
                textBlock = passwordBox.Template.FindName("txtCue", passwordBox) as TextBlock;
                if (textBlock == null)
                {

                }
            }
            if (textBlock == null)
                return;
            passwordBox.IsKeyboardFocusedChanged -= passwordBox_IsKeyboardFocusedChanged;
            passwordBox.IsKeyboardFocusedChanged += passwordBox_IsKeyboardFocusedChanged;
            if (string.IsNullOrEmpty(passwordBox.Password) && !passwordBox.IsKeyboardFocused)
            {
                textBlock.Visibility = Visibility.Visible;
            }
            else
            {
                textBlock.Visibility = Visibility.Collapsed;
            }
        }

        static void passwordBox_IsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            ChangedPassword(passwordBox);
        }

        static void passwordBox_Loaded(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            ChangedPassword(passwordBox);
            passwordBox.Loaded -= passwordBox_Loaded;
        }
    }
}
