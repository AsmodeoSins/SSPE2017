using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ControlPenales
{
  public  class TextBoxFocusBehavior
    {
        public static int GetKeepFocus(DependencyObject obj)
        {
            return (int)obj.GetValue(KeepFocusProperty);
        }

        public static void SetKeepFocus(DependencyObject obj, int value)
        {
            obj.SetValue(KeepFocusProperty, value);
        }

        // Using a DependencyProperty as the backing store for KeepFocus.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KeepFocusProperty =
            DependencyProperty.RegisterAttached("KeepFocus", typeof(int), typeof(TextBoxFocusBehavior), new UIPropertyMetadata(0, OnKeepFocusChanged));

        private static void OnKeepFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBox t = d as TextBox;
            if (t != null)
            {
                t.Focus();
            }
        }

    }
}
