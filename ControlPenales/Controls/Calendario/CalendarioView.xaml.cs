using MVVMShared.Converters;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace ControlPenales.Controls.Calendario
{
    /// <summary>
    /// Interaction logic for Calendario.xaml
    /// </summary>
    public partial class CalendarioView : UserControl
    {
        #region Variable Liberados
        private bool EsLiberados = false;
        #endregion
        public CalendarioView()
        {
            InitializeComponent();
            this.SetBinding(BindDiasAgendadosPropiedad, new Binding("BindDiasAgendadosProperty") { Mode = BindingMode.TwoWay });
            this.SetBinding(BindAnioMinimoPropiedad, new Binding("BindAnioMinimoProperty") { Mode = BindingMode.TwoWay });
            this.SetBinding(BindAnioMaximoPropiedad, new Binding("BindAnioMaximoProperty") { Mode = BindingMode.TwoWay });
            this.SetBinding(BindCmdDayClickPropiedad, new Binding("BindCmdDayClick") { Mode = BindingMode.TwoWay });
            this.SetBinding(BindSelectedMesPropiedad, new Binding("BindSelectedMesProperty") { Mode = BindingMode.TwoWay });
            this.SetBinding(BindSelectedAnioPropiedad, new Binding("BindSelectedAnioProperty") { Mode = BindingMode.TwoWay });

        }

        #region Grupo Horario
        public void CrearCalendario(DateTime INICIO, DateTime FIN, IList<GRUPO_HORARIO> agenda)
        {
            try
            {
                AnioMaximo = FIN.Year;
                AnioMinimo = INICIO.Year;

                MesMaximo = FIN.Month;
                MesMinimo = INICIO.Month;

                SelectedMes = SelectedMes == 0 ? Fechas.GetFechaDateServer.Month : SelectedMes;
                SelectedAnio = SelectedAnio == 0 ? Fechas.GetFechaDateServer.Year : SelectedAnio;

                btnMesSiguiente.IsEnabled = true;
                btnMesAnterior.IsEnabled = true;
                if (SelectedMes == MesMaximo)
                    btnMesSiguiente.IsEnabled = false;
                if (SelectedMes == MesMinimo)
                    btnMesAnterior.IsEnabled = false;
                MesString.Text = (SelectedMes == 1 ? "ENERO" : SelectedMes == 2 ? "FEBRERO" : SelectedMes == 3 ? "MARZO" : SelectedMes == 4 ? "ABRIL" : SelectedMes == 5 ? "MAYO" : SelectedMes == 6 ? "JUNIO" : SelectedMes == 7 ? "JULIO" : SelectedMes == 8 ? "AGOSTO" : SelectedMes == 9 ? "SEPTIEMBRE" : SelectedMes == 10 ? "OCTUBRE" : SelectedMes == 11 ? "NOVIEMBRE" : "DICIEMBRE") + " " + SelectedAnio;

                GenerarCalendarioUI(SelectedMes, SelectedAnio, new ObservableCollection<GRUPO_HORARIO>(agenda));
            }
            catch
            {

            }
        }
        public void GenerarCalendarioUI(int mes, int anio, ObservableCollection<GRUPO_HORARIO> agenda)
        {
            try
            {
                var hoy = Fechas.GetFechaDateServer;
                var weekRow = new WeekRowView();
                var diasMes = DateTime.DaysInMonth(anio, mes);
                var fechaInicio = new DateTime(anio, mes, 1).Date;
                var semana = 1;
                var dia = Convert.ToInt32(fechaInicio.DayOfWeek);
                if (gridCalendario.Children.Count > 7) //si ya hay algo mas agregado al grid aparte del titulo
                {
                    LimpiarCalendario();
                }
                SetRowDefinitions(diasMes, dia);
                for (var diaCorriente = 1; diaCorriente <= diasMes; diaCorriente++)
                {
                    var daybox = new DayBoxView();
                    daybox.DayNumberLabel.Content = diaCorriente.ToString();
                    var dateCorriente = new DateTime(anio, mes, diaCorriente);

                    if (agenda.Select(s => s.HORA_INICIO.Value.Date).Contains(dateCorriente))
                        daybox.DayAppointmentsStack.Background = Brushes.LightSkyBlue;
                    else
                        daybox.DayAppointmentsStack.Background = Brushes.White;

                    var ObjGrupoHorario = agenda.Where(w => w.HORA_INICIO.Value.Date.Equals(dateCorriente.Date)).FirstOrDefault();
                    if (!(agenda.Select(s => s.HORA_INICIO.Value.Date).Contains(dateCorriente) || dateCorriente >= hoy.Date))
                    {
                        if ((agenda.Select(s => s.HORA_INICIO.Value.Date).Contains(dateCorriente)))
                            daybox.DayAppointmentsStack.Background = Brushes.LightGray;
                        daybox.DayBox.IsEnabled = false;
                        daybox.DayBox.SetBinding(Button.CommandProperty, new Binding(BindCmdDay) { Source = this.DataContext });
                    }
                    else
                    {

                        if ((agenda.Select(s => s.HORA_INICIO.Value.Date).Contains(dateCorriente)) && (dateCorriente < hoy.Date) && !EsLiberados)
                        {
                            daybox.DayAppointmentsStack.Background = Brushes.LightGray;
                            daybox.DayBox.IsEnabled = false;
                        }
                        else
                        {
                            if (ObjGrupoHorario != null)
                                daybox.DayBox.CommandParameter = ObjGrupoHorario;
                            else
                                daybox.DayBox.CommandParameter = dateCorriente.Date;
                            daybox.DayBox.SetBinding(Button.CommandProperty, new Binding(BindCmdDay) { Source = this.DataContext });
                        }
                    }

                    if (ObjGrupoHorario != null)
                    {
                        daybox.TimeAppointmentsLabel.Text = ObjGrupoHorario.HORA_INICIO.Value.ToShortTimeString() + " - " + ObjGrupoHorario.HORA_TERMINO.Value.ToShortTimeString();
                        daybox.AreaAppointmentsLabel.Text = ObjGrupoHorario.AREA != null ? ObjGrupoHorario.AREA.DESCR : string.Empty;
                        daybox.EstatusAppointmentsLabel.Text = ObjGrupoHorario.GRUPO_HORARIO_ESTATUS != null ? ObjGrupoHorario.GRUPO_HORARIO_ESTATUS.DESCR : string.Empty;
                        if (ObjGrupoHorario.ESTATUS == 1)
                            if (daybox.DayBox.IsEnabled)
                                daybox.DayAppointmentsStack.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#00D100"));
                        if (ObjGrupoHorario.ESTATUS == 2)
                            if (daybox.DayBox.IsEnabled)
                                daybox.DayAppointmentsStack.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#C35F57"));
                    }

                    Grid.SetColumn(daybox, dia);
                    weekRow.gridWeekRow.Children.Add(daybox);
                    dia += 1;
                    if (dia == 7 || diaCorriente == diasMes)
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

        public void PintarCalendario(ObservableCollection<GRUPO_HORARIO> agenda)
        {
            if (gridCalendario.Children.Count > 7) //si ya hay algo mas agregado al grid aparte del titulo
            {
                var hoy = Fechas.GetFechaDateServer.Date;
                for (var x = 7; x < gridCalendario.Children.Count; x++)
                {
                    var item = (WeekRowView)gridCalendario.Children[x];
                    var uiCollection = item.gridWeekRow.Children;
                    foreach (var daybox in uiCollection)
                    {
                        var diacorriente = int.Parse(((DayBoxView)daybox).DayNumberLabel.Content.ToString());
                        var fechacorriente = new DateTime(SelectedAnio, SelectedMes, diacorriente);
                        if (agenda.Select(s => s.HORA_INICIO.Value.Date).Contains(fechacorriente))
                        {
                            if (fechacorriente < hoy)
                            {
                                if (EsLiberados)
                                {
                                    ((DayBoxView)daybox).DayAppointmentsStack.Background = Brushes.LightSkyBlue;
                                    ((DayBoxView)daybox).DayBox.IsEnabled = true;
                                }
                                else
                                {
                                    ((DayBoxView)daybox).DayBox.IsEnabled = false;
                                    if (agenda.Select(s => s.HORA_INICIO.Value.Date).Contains(fechacorriente))
                                        ((DayBoxView)daybox).DayAppointmentsStack.Background = Brushes.LightGray;
                                }
                            }
                            else
                            {
                                ((DayBoxView)daybox).DayAppointmentsStack.Background = Brushes.LightSkyBlue;
                                ((DayBoxView)daybox).DayBox.IsEnabled = true;
                            }

                            if (EsLiberados)
                            {
                                var entityestatus = agenda.Where(w => w.HORA_INICIO.Value.Date == fechacorriente.Date).FirstOrDefault();
                                if (entityestatus != null)
                                {
                                    switch ((short)entityestatus.ID_TIPO_PROGRAMA)
                                    {
                                        case (short)enumAsistencia.Asistencia:
                                             ((DayBoxView)daybox).DayAppointmentsStack.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#317A2E"));
                                            break;
                                        case (short)enumAsistencia.Falta:
                                             ((DayBoxView)daybox).DayAppointmentsStack.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#DD5448"));
                                            break;
                                        case (short)enumAsistencia.Falta_Justificada:
                                            ((DayBoxView)daybox).DayAppointmentsStack.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF29D"));
                                            break;
                                            
                                    }
                                }
                            }
                            else
                            {
                                var entityestatus = agenda.Where(w => w.HORA_INICIO.Value.Date == fechacorriente.Date).FirstOrDefault();
                                if (entityestatus != null)
                                    if (entityestatus.ESTATUS == 2)
                                        if (((DayBoxView)daybox).DayBox.IsEnabled)
                                            ((DayBoxView)daybox).DayAppointmentsStack.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#C35F57"));

                            }
                        }
                        else
                        {
                            ((DayBoxView)daybox).DayAppointmentsStack.Background = Brushes.White;
                            if (fechacorriente < hoy)
                                ((DayBoxView)daybox).DayBox.IsEnabled = false;
                        }
                    }
                }
            }
        }
        #endregion

        #region Agenda Libertad Detalle
        public void CrearCalendarioLiberado(DateTime INICIO, DateTime FIN, IList<GRUPO_HORARIO> agenda)
        {
            try
            {
                EsLiberados = true;
                AnioMaximo = FIN.Year;
                AnioMinimo = INICIO.Year;

                MesMaximo = FIN.Month;
                MesMinimo = INICIO.Month;

                SelectedMes = SelectedMes == 0 ? Fechas.GetFechaDateServer.Month : SelectedMes;
                SelectedAnio = SelectedAnio == 0 ? Fechas.GetFechaDateServer.Year : SelectedAnio;

                btnMesSiguiente.IsEnabled = true;
                btnMesAnterior.IsEnabled = true;
                if (SelectedMes == MesMaximo && SelectedAnio == AnioMaximo )
                    btnMesSiguiente.IsEnabled = false;
                if (SelectedMes == MesMinimo && SelectedAnio == AnioMinimo)
                    btnMesAnterior.IsEnabled = false;
                MesString.Text = (SelectedMes == 1 ? "ENERO" : SelectedMes == 2 ? "FEBRERO" : SelectedMes == 3 ? "MARZO" : SelectedMes == 4 ? "ABRIL" : SelectedMes == 5 ? "MAYO" : SelectedMes == 6 ? "JUNIO" : SelectedMes == 7 ? "JULIO" : SelectedMes == 8 ? "AGOSTO" : SelectedMes == 9 ? "SEPTIEMBRE" : SelectedMes == 10 ? "OCTUBRE" : SelectedMes == 11 ? "NOVIEMBRE" : "DICIEMBRE") + " " + SelectedAnio;

                GenerarCalendarioUILiberado(SelectedMes, SelectedAnio, new ObservableCollection<GRUPO_HORARIO>(agenda));
            }
            catch
            {

            }
        }

        public void GenerarCalendarioUILiberado(int mes, int anio, ObservableCollection<GRUPO_HORARIO> agenda)
        {
            try
            {
                var hoy = Fechas.GetFechaDateServer;
                var weekRow = new WeekRowView();
                var diasMes = DateTime.DaysInMonth(anio, mes);
                var fechaInicio = new DateTime(anio, mes, 1).Date;
                var semana = 1;
                var dia = Convert.ToInt32(fechaInicio.DayOfWeek);
                if (gridCalendario.Children.Count > 7) //si ya hay algo mas agregado al grid aparte del titulo
                {
                    LimpiarCalendario();
                }
                SetRowDefinitions(diasMes, dia);
                for (var diaCorriente = 1; diaCorriente <= diasMes; diaCorriente++)
                {
                    var daybox = new DayBoxView();
                    daybox.DayNumberLabel.Content = diaCorriente.ToString();
                    var dateCorriente = new DateTime(anio, mes, diaCorriente);

                    if (agenda.Select(s => s.HORA_INICIO.Value.Date).Contains(dateCorriente))
                        daybox.DayAppointmentsStack.Background = Brushes.LightSkyBlue;
                    else
                        daybox.DayAppointmentsStack.Background = Brushes.White;

                    var ObjGrupoHorario = agenda.Where(w => w.HORA_INICIO.Value.Date.Equals(dateCorriente.Date)).FirstOrDefault();
                    if (!(agenda.Select(s => s.HORA_INICIO.Value.Date).Contains(dateCorriente) || dateCorriente >= hoy.Date))
                    {
                        //if ((agenda.Select(s => s.HORA_INICIO.Value.Date).Contains(dateCorriente)))
                        //    daybox.DayAppointmentsStack.Background = Brushes.LightGray;
                        //daybox.DayBox.IsEnabled = false;
                        if (ObjGrupoHorario != null)
                            daybox.DayBox.CommandParameter = ObjGrupoHorario;
                        else
                            daybox.DayBox.CommandParameter = dateCorriente.Date;
                        daybox.DayBox.SetBinding(Button.CommandProperty, new Binding(BindCmdDay) { Source = this.DataContext });
                    }
                    else
                    {
                        if ((agenda.Select(s => s.HORA_INICIO.Value.Date).Contains(dateCorriente)) && (dateCorriente < hoy.Date))
                        {
                            //daybox.DayAppointmentsStack.Background = Brushes.LightGray;
                            //daybox.DayBox.IsEnabled = false;
                            if (ObjGrupoHorario != null)
                                daybox.DayBox.CommandParameter = ObjGrupoHorario;
                            else
                                daybox.DayBox.CommandParameter = dateCorriente.Date;
                            daybox.DayBox.SetBinding(Button.CommandProperty, new Binding(BindCmdDay) { Source = this.DataContext });
                        }
                        else
                        {
                            if (ObjGrupoHorario != null)
                                daybox.DayBox.CommandParameter = ObjGrupoHorario;
                            else
                                daybox.DayBox.CommandParameter = dateCorriente.Date;
                            daybox.DayBox.SetBinding(Button.CommandProperty, new Binding(BindCmdDay) { Source = this.DataContext });
                        }
                    }

                    if (ObjGrupoHorario != null)
                    {
                        daybox.TimeAppointmentsLabel.Text = ObjGrupoHorario.HORA_INICIO.Value.ToShortTimeString() + " - " + ObjGrupoHorario.HORA_TERMINO.Value.ToShortTimeString();
                        switch ((short)ObjGrupoHorario.ID_TIPO_PROGRAMA)
                        { 
                            case (short)enumAsistencia.Asistencia:
                                if (daybox.DayBox.IsEnabled)
                                    daybox.DayAppointmentsStack.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#317A2E"));
                                break;
                            case (short)enumAsistencia.Falta:
                                if (daybox.DayBox.IsEnabled)
                                    daybox.DayAppointmentsStack.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#DD5448"));
                                break;
                            case (short)enumAsistencia.Falta_Justificada:
                                if (daybox.DayBox.IsEnabled)
                                    daybox.DayAppointmentsStack.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF29D"));
                                break;
                        }
                    }

                    Grid.SetColumn(daybox, dia);
                    weekRow.gridWeekRow.Children.Add(daybox);
                    dia += 1;
                    if (dia == 7 || diaCorriente == diasMes)
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
        #endregion
        void LimpiarCalendario()
        {
            if (gridCalendario.RowDefinitions.Count > 1)
                gridCalendario.RowDefinitions.RemoveRange(1, gridCalendario.RowDefinitions.Count - 1);
            gridCalendario.Children.RemoveRange(7, gridCalendario.Children.Count - 7);
        }



        public void DisposeCalendario()
        {
            LimpiarCalendario();
            AnioMaximo = AnioMinimo = SelectedAnio = Fechas.GetFechaDateServer.Year;
            MesMaximo = MesMinimo = SelectedMes = Fechas.GetFechaDateServer.Month;

            btnMesSiguiente.IsEnabled = true;
            btnMesAnterior.IsEnabled = true;
            if (SelectedMes == MesMaximo)
                btnMesSiguiente.IsEnabled = false;
            if (SelectedMes == MesMinimo)
                btnMesAnterior.IsEnabled = false;
            MesString.Text = (SelectedMes == 1 ? "ENERO" : SelectedMes == 2 ? "FEBRERO" : SelectedMes == 3 ? "MARZO" : SelectedMes == 4 ? "ABRIL" : SelectedMes == 5 ? "MAYO" : SelectedMes == 6 ? "JUNIO" : SelectedMes == 7 ? "JULIO" : SelectedMes == 8 ? "AGOSTO" : SelectedMes == 9 ? "SEPTIEMBRE" : SelectedMes == 10 ? "OCTUBRE" : SelectedMes == 11 ? "NOVIEMBRE" : "DICIEMBRE") + " " + SelectedAnio;
        }

        private void SetRowDefinitions(int diasMes, int diaSemanaInicial)
        {
            var rowHeight = new System.Windows.GridLength(1, GridUnitType.Star);
            if (diaSemanaInicial != 0)
            {
                var rowDefinition = new RowDefinition();
                rowDefinition.Height = rowHeight;
                gridCalendario.RowDefinitions.Add(rowDefinition);
                diasMes -= (7 - diaSemanaInicial);
            }
            var semanasCompletas = diasMes / 7;
            for (var x = 1; x <= diasMes / 7; x++)
            {
                var rowDefinition = new RowDefinition();
                rowDefinition.Height = rowHeight;
                gridCalendario.RowDefinitions.Add(rowDefinition);
            }
            if (diasMes % 7 != 0)
            {
                var rowDefinition = new RowDefinition();
                rowDefinition.Height = rowHeight;
                gridCalendario.RowDefinitions.Add(rowDefinition);
            }
        }

        private void btnMesSiguiente_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedMes == MesMaximo && SelectedAnio == AnioMaximo)
            {
                btnMesSiguiente.IsEnabled = false;
                return;
            }

            SelectedMes += 1;
            if (SelectedMes == 13)
            {
                SelectedMes = 1;
                SelectedAnio += 1;
            }

            GenerarCalendarioUI(SelectedMes, SelectedAnio, DiasAgendados);

            MesString.Text = (SelectedMes == 1 ? "ENERO" : SelectedMes == 2 ? "FEBRERO" : SelectedMes == 3 ? "MARZO" : SelectedMes == 4 ? "ABRIL" : SelectedMes == 5 ? "MAYO" : SelectedMes == 6 ? "JUNIO" : SelectedMes == 7 ? "JULIO" : SelectedMes == 8 ? "AGOSTO" : SelectedMes == 9 ? "SEPTIEMBRE" : SelectedMes == 10 ? "OCTUBRE" : SelectedMes == 11 ? "NOVIEMBRE" : "DICIEMBRE") + " " + SelectedAnio;
            if (SelectedMes == 12 && SelectedAnio + 1 > AnioMaximo)
                btnMesSiguiente.IsEnabled = false;

            if (SelectedMes == MesMaximo)
                btnMesSiguiente.IsEnabled = false;
        }

        private void btnMesAnterior_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedMes == MesMinimo && SelectedAnio == AnioMinimo)
            {
                btnMesAnterior.IsEnabled = false;
                return;
            }

            SelectedMes -= 1;
            if (SelectedMes == 0)
            {
                SelectedMes = 12;
                SelectedAnio -= 1;
            }

            GenerarCalendarioUI(SelectedMes, SelectedAnio, DiasAgendados);

            MesString.Text = (SelectedMes == 1 ? "ENERO" : SelectedMes == 2 ? "FEBRERO" : SelectedMes == 3 ? "MARZO" : SelectedMes == 4 ? "ABRIL" : SelectedMes == 5 ? "MAYO" : SelectedMes == 6 ? "JUNIO" : SelectedMes == 7 ? "JULIO" : SelectedMes == 8 ? "AGOSTO" : SelectedMes == 9 ? "SEPTIEMBRE" : SelectedMes == 10 ? "OCTUBRE" : SelectedMes == 11 ? "NOVIEMBRE" : "DICIEMBRE") + " " + SelectedAnio;
            if (SelectedMes == 1 && SelectedAnio - 1 < AnioMinimo)
                btnMesAnterior.IsEnabled = false;


            if (SelectedMes == MesMinimo)
                btnMesAnterior.IsEnabled = false;
        }

        public static readonly DependencyProperty BindCmdDayClickPropiedad = DependencyProperty.Register("BindCmdDay", typeof(string),
            typeof(CalendarioView), null);

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
                //obj.cbMes.SetBinding(ComboBox.SelectedItemProperty, new Binding(e.NewValue.ToString())
                //{
                //    Converter = new InttoMesMarkUpExtension(),
                //    ConverterCulture = CultureInfo.CurrentCulture,
                //    Mode = BindingMode.TwoWay,
                //    NotifyOnTargetUpdated = true,
                //    Source = obj.DataContext
                //});
                //binding hacia dependencia
                obj.SetBinding(SelectedMesPropiedad, new Binding(e.NewValue.ToString())
                {
                    Mode = BindingMode.TwoWay,
                    Source = obj.DataContext
                });
            }
        }


        public string BindSelectedMes
        {
            get { return (string)GetValue(BindSelectedMesPropiedad); }
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
            typeof(CalendarioView), new FrameworkPropertyMetadata(new PropertyChangedCallback(BindAnioMaximoProperty_Changed)));

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
            get { return (string)GetValue(BindAnioMaximoPropiedad); }
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
                //obj.cbAnio.SetBinding(ComboBox.SelectedItemProperty, new Binding(e.NewValue.ToString())
                //{
                //    Mode = BindingMode.TwoWay,
                //    NotifyOnTargetUpdated = true,
                //    Source = obj.DataContext
                //});
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
            //if (e.NewValue != null && obj.SelectedAnio > 0 && obj.DiasAgendados != null)
            //    obj.GenerarCalendarioUI((int)e.NewValue, obj.SelectedAnio, obj.DiasAgendados);
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
            //if (e.NewValue != null && obj.SelectedMes > 0 && obj.DiasAgendados != null)
            //    obj.GenerarCalendarioUI(obj.SelectedMes, (int)e.NewValue, obj.DiasAgendados);
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

        public static readonly DependencyProperty AnioMaximoPropiedad = DependencyProperty.Register("AnioMaximo", typeof(int),
            typeof(CalendarioView), null);

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

        public static readonly DependencyProperty MesMaximoPropiedad = DependencyProperty.Register("MesMaximo", typeof(int),
            typeof(CalendarioView), null);

        public int MesMaximo
        {
            get { return (int)GetValue(MesMaximoPropiedad); }
            set { SetValue(MesMaximoPropiedad, value); }
        }

        public static readonly DependencyProperty MesMinimoPropiedad = DependencyProperty.Register("MesMinimo", typeof(int),
            typeof(CalendarioView), null);

        public int MesMinimo
        {
            get { return (int)GetValue(MesMinimoPropiedad); }
            set { SetValue(MesMinimoPropiedad, value); }
        }

        public static readonly DependencyProperty DiasAgendadosPropiedad = DependencyProperty.Register("DiasAgendados", typeof(ObservableCollection<GRUPO_HORARIO>),
            typeof(CalendarioView), new FrameworkPropertyMetadata(new PropertyChangedCallback(DiasAgendadosProperty_Changed)));

        private static void DiasAgendadosProperty_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var obj = (CalendarioView)sender;
            if (e.NewValue != null && obj.SelectedMes > 0 && obj.SelectedAnio > 0)
                obj.PintarCalendario((ObservableCollection<GRUPO_HORARIO>)e.NewValue);
        }

        public ObservableCollection<GRUPO_HORARIO> DiasAgendados
        {
            get { return (ObservableCollection<GRUPO_HORARIO>)GetValue(DiasAgendadosPropiedad); }
            set { SetValue(DiasAgendadosPropiedad, value); }
        }
    }


    public enum enumAsistencia 
    {
        Programado = 0,
        Asistencia = 1,
        Falta = 2,
        Falta_Justificada = 3
    }
}
