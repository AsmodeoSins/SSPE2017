using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace GESAL.Views.Controles.HighlightDatesDatePicker
{
    /// <summary>
    /// Datepicker modificado para marcar fechas seleccionadas en el calendario a traves de una lista de fechas.
    /// AlternativeCalendarStyle : Aplica estilo que contiene el xaml para marcar las fechas seleccionadas y el funcionamiento adecuado del control.
    /// DateHighlightBrush: Color de background para marcar las fechas seleccionadas.
    /// ShowDateHighlighting: booleano para decidir mostrar marcadas las fechas seleccionadas
    /// WatermarkText: El texto del watermark.
    /// ShowHighlightedDateText: Siempre en falso, pero si se quiere usar hay que modificar el control para mostrar tooltips de la fecha marcada.
    /// </summary>

    public class HighlightDatesDatePicker:DatePicker
    {
        public HighlightDatesDatePicker()
        {
            HighlightedDateList = new List<DateTime>();
            this.Loaded += ((s, e) => {
                var datePicker = s as HighlightDatesDatePicker;
                if (datePicker == null)
                    return;
                var datePickerTextBox = GetFirstChildOfType<DatePickerTextBox>(datePicker);
                if (datePickerTextBox == null)
                    return;
                var partWatermark = datePickerTextBox.Template.FindName("PART_Watermark", datePickerTextBox) as ContentControl;
                if (partWatermark == null)
                    return;
                if (!string.IsNullOrWhiteSpace(WatermarkText))
                    partWatermark.Content = WatermarkText;
            });
        }

        private static T GetFirstChildOfType<T>(DependencyObject dependencyObject) where T : DependencyObject
        {
            if (dependencyObject == null)
                return null;

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++)
            {
                var child = VisualTreeHelper.GetChild(dependencyObject, i);
                var result = (child as T) ?? GetFirstChildOfType<T>(child);
                if (result != null)
                    return result;
            }

            return null;
        }
        /// <summary>
        /// Allows us to hook into when a new style is applied, so we can call ApplyTemplate()
        /// at the correct time to get the things we need out of the Template
        /// </summary>
        public static readonly DependencyProperty AlternativeCalendarStyleProperty =

            DependencyProperty.Register("AlternativeCalendarStyle",
                typeof(Style), typeof(HighlightDatesDatePicker),
                new FrameworkPropertyMetadata((Style)null,
                    new PropertyChangedCallback(OnAlternativeCalendarStyleChanged)));

        public Style AlternativeCalendarStyle
        {
            get { return (Style)GetValue(AlternativeCalendarStyleProperty); }
            set { SetValue(AlternativeCalendarStyleProperty, value); }
        }


        private static void OnAlternativeCalendarStyleChanged(DependencyObject d,
                DependencyPropertyChangedEventArgs e)
        {
            HighlightDatesDatePicker target = (HighlightDatesDatePicker)d;
            target.ApplyTemplate();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var popup = this.GetTemplateChild("PART_Popup") as Popup;

            if (AlternativeCalendarStyle != null)
            {
                System.Windows.Controls.Calendar calendar = popup.Child as System.Windows.Controls.Calendar;
                calendar.Style = AlternativeCalendarStyle;
                calendar.ApplyTemplate();

            }
        }

        #region Dependency Properties

        // The background brush used for the date highlight.
        public static DependencyProperty DateHighlightBrushProperty = DependencyProperty.Register
             (
                  "DateHighlightBrush",
                  typeof(Brush),
                  typeof(HighlightDatesDatePicker),
                  new PropertyMetadata(new SolidColorBrush(Colors.Red))
             );

        // The list of dates to be highlighted.
        public static DependencyProperty HighlightedDateListProperty = DependencyProperty.Register
            (
                "HighlightedDateList",
                typeof(List<DateTime>),
                typeof(HighlightDatesDatePicker),
                new PropertyMetadata()
            );

        // Whether highlights should be shown.
        public static DependencyProperty ShowDateHighlightingProperty = DependencyProperty.Register
             (
                  "ShowDateHighlighting",
                  typeof(bool),
                  typeof(HighlightDatesDatePicker),
                  new PropertyMetadata(true)
             );

        // Whether tool tips should be shown with highlights.
        public static DependencyProperty ShowHighlightedDateTextProperty = DependencyProperty.Register
             (
                  "ShowHighlightedDateText",
                  typeof(bool),
                  typeof(HighlightDatesDatePicker),
                  new PropertyMetadata(true)
             );
        //Propiedad para watermark
        public static DependencyProperty WatermarkTextProperty = DependencyProperty.Register(
            "WatermarkText",typeof(string),typeof(HighlightDatesDatePicker),new PropertyMetadata("Select a date"));
        #endregion

        #region CLR Properties
        //La diferencia es que el CLR es una propiedad normal

        /// <summary>
        /// The tool tips for highlighted dates.
        /// </summary>
        [Browsable(true)]
        [Category("Highlighting")]
        public List<DateTime> HighlightedDateList
        {
            get { return (List<DateTime>)GetValue(HighlightedDateListProperty); }
            set { SetValue(HighlightedDateListProperty, value); }
        }

        /// <summary>
        /// The background brush used for the date highlight.
        /// </summary>
        [Browsable(true)]
        [Category("Highlighting")]
        public Brush DateHighlightBrush
        {
            get { return (Brush)GetValue(DateHighlightBrushProperty); }
            set { SetValue(DateHighlightBrushProperty, value); }
        }

        /// <summary>
        /// Whether highlights should be shown.
        /// </summary>
        [Browsable(true)]
        [Category("Highlighting")]
        public bool ShowDateHighlighting
        {
            get { return (bool)GetValue(ShowDateHighlightingProperty); }
            set { SetValue(ShowDateHighlightingProperty, value); }
        }

        /// <summary>
        /// Whether tool tips should be shown with highlights.
        /// </summary>
        [Browsable(true)]
        [Category("Highlighting")]
        public bool ShowHighlightedDateText
        {
            get { return (bool)GetValue(ShowHighlightedDateTextProperty); }
            set { SetValue(ShowHighlightedDateTextProperty, value); }
        }
        /// <summary>
        //Propiedad para watermark
        /// </summary>
        [Browsable(true)]
        [Category("Custom")]
        public string WatermarkText
        {
            get { return (string)GetValue(WatermarkTextProperty); }
            set { SetValue(WatermarkTextProperty, value); }
        }

        #endregion
    }
}
