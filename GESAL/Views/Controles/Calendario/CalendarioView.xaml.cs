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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GESAL.Clases.Misc;
using MVVMShared.Converters;
using System.Globalization;
using System.Collections.ObjectModel;
using System.ComponentModel;
namespace GESAL.Views.Controles.Calendario
{
    /// <summary>
    /// Interaction logic for Calendario.xaml
    /// </summary>
    public partial class CalendarioView : UserControl
    {
        public CalendarioView()
        {
            InitializeComponent();
            this.SetBinding(BindDiasAgendadosPropiedad, new Binding("BindDiasAgendadosProperty") { Mode = BindingMode.TwoWay });
            this.SetBinding(BindAnioMinimoPropiedad, new Binding("BindAnioMinimoProperty") { Mode=BindingMode.TwoWay });
            this.SetBinding(BindAnioMaximoPropiedad, new Binding("BindAnioMaximoProperty") { Mode = BindingMode.TwoWay });
            this.SetBinding(BindCmdDayClickPropiedad, new Binding("BindCmdDayClick") { Mode = BindingMode.TwoWay });
            this.SetBinding(BindSelectedMesPropiedad, new Binding("BindSelectedMesProperty") { Mode = BindingMode.TwoWay });
            this.SetBinding(BindSelectedAnioPropiedad, new Binding("BindSelectedAnioProperty") { Mode = BindingMode.TwoWay });
            
        }

        public void GenerarCalendarioUI(int mes, int anio, ObservableCollection<DateTime> agenda)
        {
            try
            {
                var weekRow = new WeekRowView();
                var diasMes = DateTime.DaysInMonth(anio, mes);
                var fechaInicio = new DateTime(anio, mes, 1).Date;
                var semana = 1;
                var dia = Convert.ToInt32(fechaInicio.DayOfWeek);
                if (gridCalendario.Children.Count>7) //si ya hay algo mas agregado al grid aparte del titulo
                {
                    LimpiarCalendario();
                }
                SetRowDefinitions(diasMes, dia);
                for (var diaCorriente = 1; diaCorriente <= diasMes; diaCorriente++)
                {
                    var daybox = new DayBoxView();
                    daybox.DayNumberLabel.Content = diaCorriente.ToString();
                    var dateCorriente = new DateTime(anio, mes, diaCorriente);

                    if (agenda.Contains(dateCorriente))
                        daybox.DayAppointmentsStack.Background = Brushes.LightSkyBlue;
                    else
                        daybox.DayAppointmentsStack.Background = Brushes.White;
                    daybox.DayBox.CommandParameter = dateCorriente.Date;
                    daybox.DayBox.SetBinding(Button.CommandProperty, new Binding(BindCmdDay) { Source = this.DataContext });
                    if (!(agenda.Contains(dateCorriente) || dateCorriente >= DateTime.Now.Date))
                    {
                        daybox.DayBox.IsEnabled = false;
                    }
                        
                    Grid.SetColumn(daybox, dia);
                    weekRow.gridWeekRow.Children.Add(daybox);
                    dia += 1;
                    if (dia == 7 || diaCorriente==diasMes)
                    {
                        Grid.SetRow(weekRow, semana);
                        Grid.SetColumnSpan(weekRow, 7);
                        gridCalendario.Children.Add(weekRow);
                        weekRow = new WeekRowView();
                        semana += 1;
                        dia = 0;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void PintarCalendario(ObservableCollection<DateTime> agenda)
        {
            if (gridCalendario.Children.Count > 7) //si ya hay algo mas agregado al grid aparte del titulo
            {
                for(var x=7;x<gridCalendario.Children.Count;x++)
                {
                    var item = (WeekRowView) gridCalendario.Children[x];
                    var uiCollection = item.gridWeekRow.Children;
                    foreach(var daybox in uiCollection)
                    {
                        var diacorriente = int.Parse(((DayBoxView)daybox).DayNumberLabel.Content.ToString());
                        var fechacorriente = new DateTime(SelectedAnio, SelectedMes, diacorriente);
                        if (agenda.Contains(fechacorriente))
                        {
                            ((DayBoxView)daybox).DayAppointmentsStack.Background = Brushes.LightSkyBlue;
                            ((DayBoxView)daybox).DayBox.IsEnabled = true;
                        }
                            
                        else
                        {
                            if (fechacorriente < DateTime.Now.Date)
                                ((DayBoxView)daybox).DayBox.IsEnabled = false;
                            ((DayBoxView)daybox).DayAppointmentsStack.Background = Brushes.White;
                        }
                            
                    }
                }
            }
        }

        private void LimpiarCalendario()
        {
            gridCalendario.RowDefinitions.RemoveRange(1, gridCalendario.RowDefinitions.Count - 1);
            gridCalendario.Children.RemoveRange(7, gridCalendario.Children.Count - 7);
        }

        private void SetRowDefinitions(int diasMes,int diaSemanaInicial)
        {
            var rowHeight=new System.Windows.GridLength(1,GridUnitType.Star);
            if(diaSemanaInicial!=0)
            {
                var rowDefinition=new RowDefinition();
                rowDefinition.Height=rowHeight;
                gridCalendario.RowDefinitions.Add(rowDefinition);
                diasMes -=( 7 - diaSemanaInicial);
            }
            var semanasCompletas = diasMes / 7;
            for (var x = 1; x <= diasMes / 7; x++)
            {
                var rowDefinition = new RowDefinition();
                rowDefinition.Height = rowHeight;
                gridCalendario.RowDefinitions.Add(rowDefinition);
            }
            if (diasMes%7!=0)
            {
                var rowDefinition = new RowDefinition();
                rowDefinition.Height = rowHeight;
                gridCalendario.RowDefinitions.Add(rowDefinition);
            }
        }

        private void GenerarAnios(int anioMinimo, int anioMaximo)
        {
            cbAnio.Items.Clear();
            for (var x = anioMinimo; x <= anioMaximo; x++)
                cbAnio.Items.Add(x);
        }

        private void btnMesSiguiente_Click(object sender, RoutedEventArgs e)
        {
            var mes=SelectedMes;
            mes+=1;
            if (mes == 13)
            {
                mes = 1;
                SelectedAnio += 1;
            }
            SelectedMes = mes;
            if (SelectedMes == 12 && SelectedAnio + 1 > AnioMaximo)
                btnMesSiguiente.IsEnabled = false;

        }

        private void btnMesAnterior_Click(object sender, RoutedEventArgs e)
        {
            var mes = SelectedMes;
            mes -= 1;
            if (mes == 0)
            {
                mes = 12;
                SelectedAnio -= 1;
            }
            SelectedMes = mes;
            if (SelectedMes == 1 && SelectedAnio - 1 < AnioMinimo)
                btnMesAnterior.IsEnabled = false;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            GenerarAnios(AnioMinimo, AnioMaximo);
        }

        public static readonly DependencyProperty BindCmdDayClickPropiedad = DependencyProperty.Register("BindCmdDay", typeof(string),
            typeof(CalendarioView),null);

        public string BindCmdDay
        {
            get { return (string)GetValue(BindCmdDayClickPropiedad); }
            set { SetValue(BindCmdDayClickPropiedad, value); }
        }

        
        public static readonly DependencyProperty BindSelectedMesPropiedad = DependencyProperty.Register("BindSelectedMes", typeof(string),
            typeof(CalendarioView), new FrameworkPropertyMetadata(new PropertyChangedCallback(BindSelectedMesProperty_Changed)));

        private static void BindSelectedMesProperty_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var obj = (CalendarioView)sender;
            if (!string.IsNullOrWhiteSpace((string)e.NewValue))
            {
                //binding hacia componente
                obj.cbMes.SetBinding(ComboBox.SelectedItemProperty, new Binding(e.NewValue.ToString()) {
                    Converter= new InttoMesMarkUpExtension(),
                    ConverterCulture=CultureInfo.CurrentCulture,
                    Mode= BindingMode.TwoWay,
                    NotifyOnTargetUpdated = true,
                    Source = obj.DataContext
                });
                //binding hacia dependencia
                obj.SetBinding(SelectedMesPropiedad, new Binding(e.NewValue.ToString()) {
                    Mode=BindingMode.TwoWay,
                    Source=obj.DataContext
                });
            }
        }


        public string BindSelectedMes
        {
            get { return (string )GetValue(BindSelectedMesPropiedad); }
            set { SetValue(BindSelectedMesPropiedad, value); }
        }

        public static readonly DependencyProperty BindAnioMinimoPropiedad = DependencyProperty.Register("BindAnioMinimo", typeof(string),
            typeof(CalendarioView), new FrameworkPropertyMetadata(new PropertyChangedCallback(BindAnioMinimoProperty_Changed)));

        private static void BindAnioMinimoProperty_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var obj = (CalendarioView)sender;
            if (!string.IsNullOrWhiteSpace((string)e.NewValue))
            {
                obj.SetBinding(AnioMinimoPropiedad, new Binding(e.NewValue.ToString())
                {
                    Mode = BindingMode.TwoWay,
                    Source = obj.DataContext
                });
            }
        }

        public string BindAnioMinimo
        {
            get { return (string)GetValue(BindAnioMinimoPropiedad); }
            set { SetValue(BindAnioMinimoPropiedad, value); }
        }


        public static readonly DependencyProperty BindAnioMaximoPropiedad = DependencyProperty.Register("BindAnioMaximo", typeof(string),
            typeof(CalendarioView),new FrameworkPropertyMetadata(new PropertyChangedCallback(BindAnioMaximoProperty_Changed)));

        private static void BindAnioMaximoProperty_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var obj = (CalendarioView)sender;
            if (!string.IsNullOrWhiteSpace((string)e.NewValue))
            {
                obj.SetBinding(AnioMaximoPropiedad, new Binding(e.NewValue.ToString())
                {
                    Mode = BindingMode.TwoWay,                    
                    Source = obj.DataContext
                });
            }
        }

        public string BindAnioMaximo
        {
            get { return (string) GetValue(BindAnioMaximoPropiedad); }
            set { SetValue(BindAnioMaximoPropiedad, value); }
        }


        public static readonly DependencyProperty BindDiasAgendadosPropiedad = DependencyProperty.Register("BindDiasAgendados", typeof(string),
    typeof(CalendarioView), new FrameworkPropertyMetadata(new PropertyChangedCallback(BindDiasAgendadosProperty_Changed)));

        private static void BindDiasAgendadosProperty_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var obj = (CalendarioView)sender;
            if (!string.IsNullOrWhiteSpace((string)e.NewValue))
            {
                obj.SetBinding(DiasAgendadosPropiedad, new Binding(e.NewValue.ToString())
                {
                    Mode = BindingMode.TwoWay,
                    Source = obj.DataContext,
                    NotifyOnTargetUpdated = true
                });
            }
        }

        public string BindDiasAgendados
        {
            get { return (string)GetValue(BindAnioMaximoPropiedad); }
            set { SetValue(BindAnioMaximoPropiedad, value); }
        }
        
        public static readonly DependencyProperty BindSelectedAnioPropiedad = DependencyProperty.Register("BindSelectedAnio", typeof(string),
            typeof(CalendarioView), new FrameworkPropertyMetadata(new PropertyChangedCallback(BindSelectedAnioProperty_Changed)));

        private static void BindSelectedAnioProperty_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var obj = (CalendarioView)sender;
            if (!string.IsNullOrWhiteSpace((string)e.NewValue))
            {
                obj.cbAnio.SetBinding(ComboBox.SelectedItemProperty, new Binding(e.NewValue.ToString())
                {
                    Mode=BindingMode.TwoWay,
                    NotifyOnTargetUpdated=true,
                    Source=obj.DataContext 
                });
                obj.SetBinding(SelectedAnioPropiedad, new Binding(e.NewValue.ToString())
                {
                    Mode = BindingMode.TwoWay,
                    Source = obj.DataContext
                });
            }
        }

        public string BindSelectedAnio
        {
            get { return (string)GetValue(BindSelectedAnioPropiedad); }
            set { SetValue(BindSelectedAnioPropiedad, value); }
        }

        public static readonly DependencyProperty SelectedMesPropiedad = DependencyProperty.Register("SelectedMes", typeof(int),
            typeof(CalendarioView), new FrameworkPropertyMetadata(new PropertyChangedCallback(SelectedMesProperty_Changed)));

        private static void SelectedMesProperty_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var obj = (CalendarioView)sender;
            if (e.NewValue != null && obj.SelectedAnio > 0 && obj.DiasAgendados != null)
                obj.GenerarCalendarioUI((int)e.NewValue, obj.SelectedAnio, obj.DiasAgendados);
            if (Convert.ToInt16(e.NewValue) == 12 && obj.SelectedAnio + 1 > obj.AnioMaximo)
                obj.btnMesSiguiente.IsEnabled = false;
            else
                obj.btnMesSiguiente.IsEnabled = true;
            if (Convert.ToInt16(e.NewValue) == 1 && obj.SelectedAnio - 1 < obj.AnioMinimo)
                obj.btnMesAnterior.IsEnabled = false;
            else
                obj.btnMesAnterior.IsEnabled = true;
        }

        public int SelectedMes
        {
            get { return (int)GetValue(SelectedMesPropiedad); }
            set { SetValue(SelectedMesPropiedad, value); }
        }

        public static readonly DependencyProperty SelectedAnioPropiedad = DependencyProperty.Register("SelectedAnio", typeof(int),
    typeof(CalendarioView), new FrameworkPropertyMetadata(new PropertyChangedCallback(SelectedAnioProperty_Changed)));

        private static void SelectedAnioProperty_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var obj = (CalendarioView)sender;
            if (e.NewValue != null && obj.SelectedMes > 0 && obj.DiasAgendados != null)
                obj.GenerarCalendarioUI(obj.SelectedMes, (int)e.NewValue, obj.DiasAgendados);
            if (obj.SelectedMes == 12 && Convert.ToInt16(e.NewValue) + 1 > obj.AnioMaximo)
                obj.btnMesSiguiente.IsEnabled = false;
            else
                obj.btnMesSiguiente.IsEnabled = true;
            if (obj.SelectedMes == 1 && Convert.ToInt16(e.NewValue) - 1 < obj.AnioMinimo)
                obj.btnMesAnterior.IsEnabled = false;
            else
                obj.btnMesAnterior.IsEnabled = true;
        }

        public int SelectedAnio
        {
            get { return (int)GetValue(SelectedAnioPropiedad); }
            set { SetValue(SelectedAnioPropiedad, value); }
        }

        public static readonly DependencyProperty AnioMaximoPropiedad= DependencyProperty.Register("AnioMaximo",typeof(int),
            typeof(CalendarioView),null);

        public int AnioMaximo
        {
            get { return (int)GetValue(AnioMaximoPropiedad); }
            set { SetValue(AnioMaximoPropiedad, value); }
        }

        public static readonly DependencyProperty AnioMinimoPropiedad = DependencyProperty.Register("AnioMinimo", typeof(int),
            typeof(CalendarioView), null);

        public int AnioMinimo
        {
            get { return (int)GetValue(AnioMinimoPropiedad); }
            set { SetValue(AnioMinimoPropiedad, value); }
        }

        public static readonly DependencyProperty DiasAgendadosPropiedad = DependencyProperty.Register("DiasAgendados", typeof(ObservableCollection<DateTime>),
            typeof(CalendarioView), new FrameworkPropertyMetadata(new PropertyChangedCallback(DiasAgendadosProperty_Changed)));

        private static void DiasAgendadosProperty_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var obj = (CalendarioView)sender;
            if (e.NewValue != null && obj.SelectedMes > 0 && obj.SelectedAnio > 0)
                obj.PintarCalendario((ObservableCollection<DateTime>)e.NewValue);
        }

        public ObservableCollection<DateTime> DiasAgendados
        {
            get { return (ObservableCollection<DateTime>)GetValue(DiasAgendadosPropiedad); }
            set { SetValue(DiasAgendadosPropiedad, value); }
        }
    }
}
