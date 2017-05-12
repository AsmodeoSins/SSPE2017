using ControlPenales.BiometricoServiceReference;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace ControlPenales
{
    public partial class ManejoEmpalmesViewModel
    {
        /* [descripcion de clase]
         * modulo de manejo de empalmes
         * 
         * esta clase se refiere al manejo de las horas cruzadas que se puedan dar en la calendarizacion de los grupos o horas aniadidas
         * 
         * las horas empalmadas se deben de mostrar al seleccionar a un imputado de la lista que se carga al entrar al modulo, si esta se ecuentra vacia no hay empalmes
         * para la seleccion de la hora asignada de las horas empalmadas se seleccionara mediante un radio button donde solo puede ver una hora seleccionada por dia
         * la hora marcada por default es la hora cual se registro primero
         * 
         */

        #region [PERMISOS]
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.MANEJO_EMPALMES.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                foreach (var p in permisos)
                {
                    if (p.INSERTAR == 1)
                    {
                        AgregarMenuEnabled = true;
                    }
                    if (p.CONSULTAR == 1)
                    {
                        ListaEmpalmeEnabled = true;
                        banderaConsulta = true;
                    }
                    if (p.EDITAR == 1)
                        EditarMenuEnabled = true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }
        #endregion

        /// <summary>
        /// metodo para la interaccion con el menu
        /// </summary>
        /// <param name="obj">valor del menu seleccioinado</param>
        private async void clickSwitch(Object obj)
        {
            try
            {
                switch (obj.ToString())
                {
                    case "menu_cancelar":
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = null;
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = null;
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new ManejoEmpalmesView();
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ManejoEmpalmesViewModel();
                        break;
                    case "menu_guardar":
                        // proceso para guardar las deciciones de los empalmes
                        if (SelectedParticipante == null)
                            return;

                        if (await new Dialogos().ConfirmarEliminar("Manejo de Empalmes", "Esta Seguro De Guardar?") == 0)
                            break;

                        var result = await StaticSourcesViewModel.OperacionesAsync<bool>("Guardando...", () =>
                         {
                             var dalGA = new cGrupoAsistencia();
                             foreach (var item in SelectedParticipante.PorHora)
                             {
                                 var entitUpdate = dalGA.GetData().Where(w => w.ID_CENTRO == item.Entity.ID_CENTRO && w.ID_TIPO_PROGRAMA == item.Entity.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == item.Entity.ID_ACTIVIDAD && w.ID_GRUPO == item.Entity.ID_GRUPO && w.ID_CONSEC == item.Entity.ID_CONSEC && w.ID_GRUPO_HORARIO == item.Entity.ID_GRUPO_HORARIO && w.EMPALME == item.Entity.EMPALME && w.EMP_COORDINACION == 1 && w.ESTATUS == 1).AsEnumerable().Select(s => new GRUPO_ASISTENCIA
                                 {
                                     ASISTENCIA = s.ASISTENCIA,
                                     EMP_APROBADO = item.Check ? 1 : 0,
                                     EMP_COORDINACION = 2,
                                     EMP_FECHA = Fechas.GetFechaDateServer,
                                     EMPALME = s.EMPALME,
                                     ESTATUS = s.ESTATUS,
                                     FEC_REGISTRO = item.Check ? Fechas.GetFechaDateServer : s.FEC_REGISTRO,
                                     ID_ACTIVIDAD = s.ID_ACTIVIDAD,
                                     ID_CENTRO = s.ID_CENTRO,
                                     ID_CONSEC = s.ID_CONSEC,
                                     ID_GRUPO = s.ID_GRUPO,
                                     ID_GRUPO_HORARIO = s.ID_GRUPO_HORARIO,
                                     ID_TIPO_PROGRAMA = s.ID_TIPO_PROGRAMA
                                 }).FirstOrDefault();

                                 dalGA.Update(entitUpdate);
                             }
                             return true;
                         });
                        if (result)
                            await new Dialogos().ConfirmacionDialogoReturn("Manejo de Empalmes", "Grabado Exitosamente");
                        ManejoEmpalmeLoad(null);
                        break;
                    case "menu_salir":
                        PrincipalViewModel.SalirMenu();
                        break;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el flujo del modulo.", ex);
            }
        }

        /// <summary>
        /// metodo para cargar la informacion que se muestra al entrar al modulo
        /// </summary>
        /// <param name="obj">usercontrol del modulo</param>
        private async void ManejoEmpalmeLoad(ManejoEmpalmesView obj)
        {
            try
            {
                ConfiguraPermisos();
                if (obj != null)
                {
                    DynamicWrapPanel = obj.DynamicWrapPanel;
                    DynamicGrid = obj.DynamicGrid;
                }

                // se limpian valores del control empalme
                DynamicGrid.Children.Clear();
                DynamicGrid.RowDefinitions.Clear();
                DynamicGrid.ColumnDefinitions.Clear();
                DynamicWrapPanel.Children.Clear();

                // se carga la lista de los imputados con empalme no resuelto
                var list = new List<MaestroEmpalme>();
                if (banderaConsulta)
                {
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                    {
                        try
                        {
                            foreach (var item in new cGrupoAsistencia().GetData().Where(w => w.EMPALME != 0 && w.EMP_COORDINACION == 1).GroupBy(g => g.GRUPO_PARTICIPANTE.INGRESO))
                            {
                                list.Add(new MaestroEmpalme()
                                {
                                    Entity = item.Key,
                                    NOMBRE = item.Key.IMPUTADO.NOMBRE,
                                    PATERNO = item.Key.IMPUTADO.PATERNO,
                                    MATERNO = item.Key.IMPUTADO.MATERNO,
                                    ID_CENTRO = item.Key.ID_CENTRO,
                                    ID_ANIO = item.Key.ID_ANIO,
                                    ID_IMPUTADO = item.Key.ID_IMPUTADO,
                                    ID_INGRESO = item.Key.ID_INGRESO,
                                    UBICACION = item.Key.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() + "-" + item.Key.CAMA.CELDA.SECTOR.DESCR.Trim() + item.Key.CAMA.CELDA.ID_CELDA.Trim() + "-" + (string.IsNullOrEmpty(item.Key.CAMA.DESCR) ? item.Key.CAMA.ID_CAMA.ToString().Trim() : item.Key.CAMA.ID_CAMA + " " + item.Key.CAMA.DESCR.Trim()),
                                    PLANIMETRIA = item.Key.CAMA != null ? item.Key.CAMA.SECTOR_OBSERVACION_CELDA != null ? item.Key.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION != null ? item.Key.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION.SECTOR_CLASIFICACION != null ? item.Key.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION.SECTOR_CLASIFICACION.POBLACION : string.Empty : string.Empty : string.Empty : string.Empty,
                                    PLANIMETRIACOLOR = item.Key.CAMA != null ? item.Key.CAMA.SECTOR_OBSERVACION_CELDA != null ? item.Key.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION != null ? item.Key.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION.SECTOR_CLASIFICACION != null ? item.Key.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION.SECTOR_CLASIFICACION.COLOR : string.Empty : string.Empty : string.Empty : string.Empty,
                                    DELITO = item.Key.INGRESO_DELITO.DESCR,
                                    RESTANTE = CalcularSentencia(item.Key),
                                    SENTENCIA = varauxSentencia,
                                    ImagenParticipante = item.Key.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any() ? item.Key.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO : item.Key.INGRESO_BIOMETRICO.Any(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG) ? item.Key.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO : new Imagenes().getImagenPerson(),

                                    PorHora = item.Where(w => w.GRUPO_PARTICIPANTE.GRUPO != null).Select(s => new DetalleEmpalmeHora()
                                    {
                                        Entity = s,
                                        ID_EJE = s.GRUPO_HORARIO.GRUPO.ID_EJE.Value,
                                        ID_GRUPO = s.ID_GRUPO,
                                        GRUPO = s.GRUPO_HORARIO.GRUPO.DESCR,
                                        HORA_INICIO = s.GRUPO_HORARIO.HORA_INICIO,
                                        HORA_TERMINO = s.GRUPO_HORARIO.HORA_TERMINO,
                                        FEC_REGISTRO = s.FEC_REGISTRO,
                                        PRIORIDAD = s.GRUPO_HORARIO.GRUPO.ACTIVIDAD.PRIORIDAD,
                                        DEPARTAMENTO = s.GRUPO_HORARIO.GRUPO.CENTRO_DEPARTAMENTO.DEPARTAMENTO.DESCR,
                                        PROGRAMA = s.GRUPO_HORARIO.GRUPO.ACTIVIDAD.TIPO_PROGRAMA.NOMBRE,
                                        ACTIVIDAD = s.GRUPO_HORARIO.GRUPO.ACTIVIDAD.DESCR,
                                        IDGRUPOHORARIO = s.ID_GRUPO_HORARIO
                                    }).ToList()
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener participantes.", ex);
                        }
                    });
                }
                ListEmpalmesParticipantes = new ObservableCollection<MaestroEmpalme>(list);

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener participantes.", ex);
            }
        }

        /// <summary>
        /// metodo para calcular la sentencia restante y sentencia
        /// </summary>
        /// <param name="ingres">ingreso del interno seleccionado</param>
        /// <returns>sentencia restante[sentencia mendiante propiedad]</returns>
        private string CalcularSentencia(INGRESO ingres)
        {
            try
            {
                if (ingres != null)
                {
                    int anios = 0, meses = 0, dias = 0, anios_abono = 0, meses_abono = 0, dias_abono = 0;
                    DateTime? FechaInicioCompurgacion = null, FechaFinCompurgacion = null;
                    if (ingres.CAUSA_PENAL != null)
                    {
                        foreach (var cp in ingres.CAUSA_PENAL)
                        {
                            var segundaInstancia = false;
                            if (cp.SENTENCIA != null)
                            {
                                if (cp.SENTENCIA.Count > 0)
                                {
                                    //BUSCAMOS SI TIENE 2DA INSTANCIA
                                    if (cp.RECURSO.Count > 0)
                                    {
                                        var r = cp.RECURSO.Where(w => w.SENTENCIA_ANIOS > 0 || w.SENTENCIA_MESES > 0 || w.SENTENCIA_DIAS > 0);
                                        if (r != null)
                                        {
                                            var res = r.FirstOrDefault();
                                            if (res != null)
                                            {
                                                //SENTENCIA
                                                anios = anios + (res.SENTENCIA_ANIOS != null ? res.SENTENCIA_ANIOS.Value : 0);
                                                meses = meses + (res.SENTENCIA_MESES != null ? res.SENTENCIA_MESES.Value : 0);
                                                dias = dias + (res.SENTENCIA_DIAS != null ? res.SENTENCIA_DIAS.Value : 0);

                                                segundaInstancia = true;
                                            }
                                        }
                                    }
                                    var s = cp.SENTENCIA.FirstOrDefault();
                                    if (s != null)
                                    {
                                        if (FechaInicioCompurgacion == null)
                                        {
                                            FechaInicioCompurgacion = s.FEC_INICIO_COMPURGACION;
                                        }
                                        else
                                        {
                                            if (FechaInicioCompurgacion > s.FEC_INICIO_COMPURGACION)
                                                FechaInicioCompurgacion = s.FEC_INICIO_COMPURGACION;
                                        }

                                        //SENTENCIA
                                        if (!segundaInstancia)
                                        {
                                            anios = anios + (s.ANIOS != null ? s.ANIOS.Value : 0);
                                            meses = meses + (s.MESES != null ? s.MESES.Value : 0);
                                            dias = dias + (s.DIAS != null ? s.DIAS.Value : 0);
                                        }

                                        //ABONO
                                        anios_abono = anios_abono + (s.ANIOS_ABONADOS != null ? s.ANIOS_ABONADOS.Value : 0);
                                        meses_abono = meses_abono + (s.MESES_ABONADOS != null ? s.MESES_ABONADOS.Value : 0);
                                        dias_abono = dias_abono + (s.DIAS_ABONADOS != null ? s.DIAS_ABONADOS.Value : 0);
                                    }
                                }
                            }
                        }
                    }

                    while (dias > 29)
                    {
                        meses++;
                        dias = dias - 30;
                    }
                    while (meses > 11)
                    {
                        anios++;
                        meses = meses - 12;
                    }

                    if (FechaInicioCompurgacion != null)
                    {
                        FechaFinCompurgacion = FechaInicioCompurgacion;
                        FechaFinCompurgacion = FechaFinCompurgacion.Value.AddYears(anios);
                        FechaFinCompurgacion = FechaFinCompurgacion.Value.AddMonths(meses);
                        FechaFinCompurgacion = FechaFinCompurgacion.Value.AddDays(dias);
                        //
                        FechaFinCompurgacion = FechaFinCompurgacion.Value.AddYears(-anios_abono);
                        FechaFinCompurgacion = FechaFinCompurgacion.Value.AddMonths(-meses_abono);
                        FechaFinCompurgacion = FechaFinCompurgacion.Value.AddDays(-dias_abono);

                        int a = 0, m = 0, d = 0;
                        new Fechas().DiferenciaFechas(Fechas.GetFechaDateServer.Date, FechaInicioCompurgacion.Value.Date, out a, out  m, out d);
                        a = m = d = 0;
                        new Fechas().DiferenciaFechas(FechaFinCompurgacion.Value.Date, Fechas.GetFechaDateServer.Date, out a, out  m, out d);

                        //propiedad de la sentencia
                        varauxSentencia = anios + (anios == 1 ? " Año " : " Años ") + meses + (meses == 1 ? " Mes " : " Meses ") + dias + (dias == 1 ? " Dia" : " Dias");
                        //valor de sentencia restante
                        return a + (a == 1 ? " Año " : " Años ") + m + (m == 1 ? " Mes " : " Meses ") + d + (d == 1 ? " Dia" : " Dias");
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al calcular sentencia", ex);
            }
            return string.Empty;
        }

        /// <summary>
        /// metodo que carga los registros de empalme al control de empalmes
        /// </summary>
        /// <param name="DynamicWrapPanel">control wrap panel de la vista</param>
        /// <param name="ListHoras">lista horas del imputado seleccionado</param>
        /// <param name="ListGrupos">lista grupo de imputado seleccionado</param>
        private void LoadEmpalmes(WrapPanel DynamicWrapPanel, List<DetalleEmpalmeHora> ListHoras, List<GRUPO> ListGrupos)
        {
            try
            {
                #region [Crear Grid]
                //declaracion del grid
                DynamicGrid.Children.Clear();
                DynamicGrid.RowDefinitions.Clear();
                DynamicGrid.ColumnDefinitions.Clear();

                //declaracoin de renglon
                DynamicGrid.RowDefinitions.Add(new RowDefinition());

                //declaracion de columna
                DynamicGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(.2, GridUnitType.Star) });
                DynamicGrid.ColumnDefinitions.Add(new ColumnDefinition());
                DynamicGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(.4, GridUnitType.Star) });
                DynamicGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(.4, GridUnitType.Star) });
                DynamicGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(.4, GridUnitType.Star) });

                //declaracion de encabezados
                var arrayHeader = new string[] { "PRIORIDAD", "ACTIVIDAD", "% DE ASISTENCIA", "HORAS ASIGNADAS", "% DE HORAS EMPALMADAS" }.ToList();
                var addTextBlock = new TextBlock();
                foreach (var item in arrayHeader)
                {
                    addTextBlock.Text = item;
                    addTextBlock.FontSize = 14;
                    addTextBlock.Margin = new Thickness(5);
                    addTextBlock.Foreground = Brushes.Black;
                    addTextBlock.FontWeight = FontWeights.Bold;

                    Grid.SetRow(addTextBlock, 0);
                    Grid.SetColumn(addTextBlock, arrayHeader.IndexOf(item));
                    DynamicGrid.Children.Add(addTextBlock);
                    addTextBlock = new TextBlock();
                }

                //declaracion de las actividades empalmadas en forma de listado
                var addTextBox = new TextBox();
                foreach (var item in ListGrupos)
                {
                    DynamicGrid.RowDefinitions.Add(new RowDefinition());
                    foreach (var itemHeader in arrayHeader)
                    {
                        addTextBox.Name = "TB_" + item.ID_GRUPO + "_" + arrayHeader.IndexOf(itemHeader);
                        addTextBox.IsReadOnly = true;
                        addTextBox.Text = itemHeader == "PRIORIDAD" ? (item.ACTIVIDAD.PRIORIDAD.HasValue ? item.ACTIVIDAD.PRIORIDAD.Value.ToString() : string.Empty) : itemHeader == "ACTIVIDAD" ? item.ACTIVIDAD.DESCR : itemHeader == "% DE ASISTENCIA" ? ObtenerAsistencia(item, item.ID_GRUPO, SelectedParticipante) : itemHeader == "HORAS ASIGNADAS" ? ObtenerHorasAsignadas(item, SelectedParticipante) : string.Empty;
                        Grid.SetRow(addTextBox, (ListGrupos.IndexOf(item) + 1));
                        Grid.SetColumn(addTextBox, arrayHeader.IndexOf(itemHeader));
                        DynamicGrid.Children.Add(addTextBox);
                        addTextBox = new TextBox();
                    }
                }

                #endregion
                #region [Crear WrapPanel]
                // limpieza de wrappanel
                foreach (var item in DynamicWrapPanel.Children.Cast<Grid>().ToList())
                    item.Children.Clear();

                DynamicWrapPanel.Children.Clear();

                //rendereo de grids de fechas empalmadas

                //definimos grid
                var addGrid = new Grid();
                //definicon de renglon
                addGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                addGrid.RowDefinitions.Add(new RowDefinition());

                //declaramos el datagrid y las columnas del datagrid y bindiamos
                var addDataGrid = new DataGrid();
                addDataGrid.Columns.Add(new DataGridTextColumn() { Binding = new Binding("HORA_INICIO") { StringFormat = "dd/MM/yyyy" }, Header = "FECHA", CanUserSort = false, IsReadOnly = true, FontSize = 12, MaxWidth = 150, ElementStyle = new Style(typeof(TextBlock)) { Setters = { new Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap) } } });

                foreach (var item in SelectedParticipante.PorHora.GroupBy(g => new { g.DEPARTAMENTO, g.PROGRAMA, g.ACTIVIDAD, g.ID_EJE }).OrderBy(o => o.FirstOrDefault().FEC_REGISTRO))
                {
                    var RadioFactory = new FrameworkElementFactory(typeof(RadioButton));
                    RadioFactory.SetValue(RadioButton.GroupNameProperty, new Binding("HORA_INICIO"));
                    RadioFactory.AddHandler(RadioButton.CheckedEvent, new RoutedEventHandler(chkSelect_Checked));
                    RadioFactory.AddHandler(RadioButton.UncheckedEvent, new RoutedEventHandler(chkSelect_Unchecked));
                    RadioFactory.SetValue(RadioButton.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                    RadioFactory.SetValue(RadioButton.VerticalAlignmentProperty, VerticalAlignment.Center);
                    RadioFactory.SetValue(RadioButton.TagProperty, item.FirstOrDefault().ID_GRUPO);

                    RadioFactory.SetValue(RadioButton.IsCheckedProperty, new Binding("HORA_INICIO") { Converter = new IsCheckedAssistConverter(), ConverterParameter = new object[] { SelectedParticipante.PorHora, item.FirstOrDefault().ID_GRUPO } });
                    RadioFactory.SetValue(RadioButton.VisibilityProperty, new Binding("HORA_INICIO") { Converter = new IsVisibleConverter(), ConverterParameter = new object[] { SelectedParticipante.PorHora, item.FirstOrDefault().ID_GRUPO } });

                    var triggerEnabled = new DataTrigger { Binding = new Binding("HORA_INICIO") { Converter = new IsEnabledDateConverter() }, Value = true };
                    triggerEnabled.Setters.Add(new Setter { Property = RadioButton.IsEnabledProperty, Value = false });

                    var template = new DataTemplate { VisualTree = RadioFactory };
                    template.Triggers.Add(triggerEnabled);

                    addDataGrid.Columns.Add(new DataGridTemplateColumn() { Header = "DEPARTAMENTO: " + item.Key.DEPARTAMENTO + "\n" + "PROGRAMA: " + item.Key.PROGRAMA + "\n" + "ACTIVIDAD: " + item.Key.ACTIVIDAD + "\n" + "GRUPO: " + item.FirstOrDefault().GRUPO, CellTemplate = template });
                }

                // configuramos propiedades del datagrid
                addDataGrid.Name = "ContenedorGrid";
                addDataGrid.FontSize = 14;
                addDataGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
                addDataGrid.VerticalAlignment = VerticalAlignment.Top;
                addDataGrid.Margin = new Thickness(5);
                addDataGrid.AutoGenerateColumns = false;


                //var style = new Style(typeof(DataGrid), (Style)Application.Current.FindResource("MetroDataGrid"));
                //style.Setters.Add(new Setter(DataGrid.GridLinesVisibilityProperty, DataGridGridLinesVisibility.Horizontal));
                //Resources.Add(typeof(TextBlock), style);

                addDataGrid.Style = (Style)Application.Current.FindResource("MetroDataGrid");
                addDataGrid.CanUserAddRows = false;
                addDataGrid.CanUserDeleteRows = false;
                addDataGrid.BeginningEdit += (s, e) =>
                {
                    e.Cancel = true;
                };
                addDataGrid.VerticalGridLinesBrush = Brushes.Black;
                addDataGrid.GridLinesVisibility = DataGridGridLinesVisibility.Vertical;
                addDataGrid.SelectionMode = DataGridSelectionMode.Single;
                addDataGrid.AutoGenerateColumns = false;
                //igualamos el datasource a la lista de horas empalmadas
                addDataGrid.ItemsSource = ListHoras.GroupBy(g => new { g.HORA_INICIO.Value.Date }).OrderBy(o => o.FirstOrDefault().HORA_INICIO);

                //agregamos el datagrid
                Grid.SetRow(addDataGrid, 1);
                Grid.SetColumn(addDataGrid, 0);

                //agregamos el texbox y el datagrid al grid
                addGrid.Children.Add(addTextBlock);
                addGrid.Children.Add(addDataGrid);

                //agregamos el grid al wrappanel
                DynamicWrapPanel.Children.Add(addGrid);
                #endregion
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la información", ex);
            }
        }

        /// <summary>
        /// metodo que obtiene el calculo en porcentaje para el porcentaje de empalmes
        /// </summary>
        /// <param name="SelectedParticipante">imputado seleccionado</param>
        private void ObtenerEmpalmes(MaestroEmpalme SelectedParticipante)
        {
            try
            {
                var TotalHoras = 0.0;
                var AsistenciaHoras = 0.0;
                foreach (var item3 in SelectedParticipante.PorHora.GroupBy(g => g.Entity.GRUPO_PARTICIPANTE.GRUPO).Select(s => s.Key).OrderBy(o => o.ACTIVIDAD.PRIORIDAD).ToList())
                {
                    TotalHoras = SelectedParticipante.Entity.GRUPO_PARTICIPANTE.Where(w => w.ID_GRUPO == item3.ID_GRUPO).FirstOrDefault().GRUPO_ASISTENCIA.Where(w => w.ESTATUS == 1).Count();
                    AsistenciaHoras = SelectedParticipante.Entity.GRUPO_PARTICIPANTE.Where(w => w.ID_GRUPO == item3.ID_GRUPO).FirstOrDefault().GRUPO_ASISTENCIA.Where(w => w.ESTATUS == 1).Count() - Convert.ToDouble((Convert.ToInt32(DynamicGrid.Children.Cast<UIElement>().Where(w => w is TextBox).Cast<TextBox>().Where(w => w.Name.Equals("TB_" + item3.ID_GRUPO + "_3")).FirstOrDefault().Text)));

                    if (double.IsNaN((AsistenciaHoras / TotalHoras)))
                    {
                        DynamicGrid.Children.Cast<UIElement>().Where(w => w is TextBox).Cast<TextBox>().Where(w => w.Name.Equals("TB_" + item3.ID_GRUPO + "_4")).FirstOrDefault().Text = string.Empty;
                        continue;
                    }
                    DynamicGrid.Children.Cast<UIElement>().Where(w => w is TextBox).Cast<TextBox>().Where(w => w.Name.Equals("TB_" + item3.ID_GRUPO + "_4")).FirstOrDefault().Text = String.Format("{0:P2}", (AsistenciaHoras / TotalHoras));
                }

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al calcular el porcentaje de horas empalmadas del participante.", ex);
            }
        }

        /// <summary>
        /// metodo que obtiene las horas asignadas a la actividad
        /// </summary>
        /// <param name="item">grupo</param>
        /// <param name="SelectedParticipante">imputado seleccionado</param>
        /// <returns>cantidad de horas asignadas</returns>
        private string ObtenerHorasAsignadas(GRUPO item, MaestroEmpalme SelectedParticipante)
        {
            try
            {
                return SelectedParticipante.Entity.GRUPO_PARTICIPANTE.Where(w => w.ID_GRUPO == item.ID_GRUPO).FirstOrDefault().GRUPO_ASISTENCIA.Where(w => w.EMPALME == 0 && w.ESTATUS == 1).Count().ToString();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al calcular horas asignadas del participante.", ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// metodo que obtiene el porcentaje de asistencia para la actividad
        /// </summary>
        /// <param name="item">objeto grupo</param>
        /// <param name="idGrupo">id grupo seleccionado</param>
        /// <param name="SelectedParticipante">imputado seleccionado</param>
        /// <returns>porcentaje de asistencia</returns>
        private string ObtenerAsistencia(object item, short idGrupo, MaestroEmpalme SelectedParticipante)
        {
            try
            {
                var TotalHoras = 0.0;
                var AsistenciaHoras = 0.0;
                if (item is GRUPO)
                    TotalHoras = ((GRUPO)item).GRUPO_HORARIO.Where(w => w.ID_GRUPO == idGrupo && w.ESTATUS == 1).Count();
                else
                    TotalHoras = Convert.ToDouble(item);
                AsistenciaHoras = SelectedParticipante.Entity.GRUPO_PARTICIPANTE.Where(w => w.ID_GRUPO == idGrupo).FirstOrDefault().GRUPO_ASISTENCIA.Where(w => w.GRUPO_HORARIO.ESTATUS == 1 && (w.ESTATUS == 1 || w.ESTATUS == 3) && SelectedParticipante.Entity.GRUPO_PARTICIPANTE.Where(wh => wh.GRUPO != null && wh.GRUPO.GRUPO_HORARIO.Where(whe => whe.ESTATUS == 1).Any()).Contains(w.GRUPO_PARTICIPANTE) && w.ASISTENCIA == 1).Count();

                if (double.IsNaN((AsistenciaHoras / TotalHoras)))
                    return string.Empty;

                return string.Format("{0:P2}", (AsistenciaHoras / TotalHoras));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la asistencia del participante.", ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// metodo que ejecuta el comando de desmarcar y recalcula asistencia, horas asignadas y porcentaje de empalme
        /// </summary>
        /// <param name="sender">control radiobutton</param>
        /// <param name="e">event args</param>
        private void chkSelect_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                var ID_GRUPO = Convert.ToInt16(((FrameworkElement)(sender)).Tag);

                ((IEnumerable<DetalleEmpalmeHora>)((((ContentPresenter)(((FrameworkElement)(sender)).TemplatedParent))).Content)).FirstOrDefault().Check = ((RadioButton)sender).IsChecked.Value;
                var HoraAsignadas = (Convert.ToInt32(DynamicGrid.Children.Cast<UIElement>().Where(w => w is TextBox).Cast<TextBox>().Where(w => w.Name.Equals("TB_" + ID_GRUPO + "_3")).FirstOrDefault().Text) - 1);
                DynamicGrid.Children.Cast<UIElement>().Where(w => w is TextBox).Cast<TextBox>().Where(w => w.Name.Equals("TB_" + ID_GRUPO + "_2")).FirstOrDefault().Text = ObtenerAsistencia(HoraAsignadas, ID_GRUPO, SelectedParticipante);
                DynamicGrid.Children.Cast<UIElement>().Where(w => w is TextBox).Cast<TextBox>().Where(w => w.Name.Equals("TB_" + ID_GRUPO + "_3")).FirstOrDefault().Text = HoraAsignadas.ToString();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al desseleccionar actividad", ex);
            }
        }

        /// <summary>
        /// metodo que ejecuta el comando de marcar y recalcula asistencia, horas asignadas y porcentaje de empalme
        /// </summary>
        /// <param name="sender">control radiobutton</param>
        /// <param name="e">event args</param>
        private void chkSelect_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                var ID_GRUPO = Convert.ToInt16(((FrameworkElement)(sender)).Tag);

                ((IEnumerable<DetalleEmpalmeHora>)((((ContentPresenter)(((FrameworkElement)(sender)).TemplatedParent))).Content)).FirstOrDefault().Check = ((RadioButton)sender).IsChecked.Value;
                var HoraAsignadas = (Convert.ToInt32(DynamicGrid.Children.Cast<UIElement>().Where(w => w is TextBox).Cast<TextBox>().Where(w => w.Name.Equals("TB_" + ID_GRUPO + "_3")).FirstOrDefault().Text) + 1);
                DynamicGrid.Children.Cast<UIElement>().Where(w => w is TextBox).Cast<TextBox>().Where(w => w.Name.Equals("TB_" + ID_GRUPO + "_2")).FirstOrDefault().Text = ObtenerAsistencia(HoraAsignadas, ID_GRUPO, SelectedParticipante);
                DynamicGrid.Children.Cast<UIElement>().Where(w => w is TextBox).Cast<TextBox>().Where(w => w.Name.Equals("TB_" + ID_GRUPO + "_3")).FirstOrDefault().Text = HoraAsignadas.ToString();
                ObtenerEmpalmes(SelectedParticipante);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar actividad", ex);
            }
        }

        /// <summary>
        /// metodo que muestra el total de horas y cursos del imputado
        /// </summary>
        /// <param name="collection">lista de grupos pertenecientes al imputado</param>
        /// <param name="TotalActividades">total de actividades del imputado</param>
        private void ObtenerCursosAprovadosTotalHoras(ICollection<GRUPO_PARTICIPANTE> collection, int TotalActividades)
        {
            var acreditados = 0;
            try
            {
                var HorasAsistencia = 0;
                var TotalAsistencia = 0;
                foreach (var item in collection)
                {
                    acreditados = acreditados + item.NOTA_TECNICA.Where(w => w.ESTATUS == 1).Count();
                    TotalAsistencia = TotalAsistencia + (item.ID_GRUPO.HasValue ? item.GRUPO.GRUPO_HORARIO.Where(w => w.ID_GRUPO == item.ID_GRUPO && w.ID_GRUPO == item.ID_GRUPO && w.ESTATUS == 1).Count() : 0);
                    HorasAsistencia = HorasAsistencia + item.GRUPO_ASISTENCIA.Where(w => w.GRUPO_HORARIO.ESTATUS == 1 && (w.ESTATUS == 1 || w.ESTATUS == 3) && collection.Where(wh => wh.GRUPO != null && wh.GRUPO.GRUPO_HORARIO.Where(whe => whe.ESTATUS == 1).Any()).Contains(w.GRUPO_PARTICIPANTE) && w.ASISTENCIA == 1).Count();
                }
                MaxValueProBar = TotalActividades == 0 ? 1 : TotalActividades;
                CantidadActividadesAprovadas = acreditados;

                HorasTratamiento = HorasAsistencia + "/" + TotalAsistencia;
                AvanceTratamiento = acreditados + "/" + TotalActividades;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener avances del participante.", ex);
            }
        }
    }

    public class IsEnabledDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((DateTime)value) < Fechas.GetFechaDateServer;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("Nope.");
        }
    }
    class IsCheckedAssistConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var param = (object[])parameter;
                var listhora = (IEnumerable<DetalleEmpalmeHora>)param[0];
                var idgrupo = (short)param[1];
                var HORA_INICIO = (DateTime)value;

                if (listhora.Where(w => w.HORA_INICIO == HORA_INICIO && w.Entity.ASISTENCIA == 1).Any())
                    return listhora.Where(w => w.HORA_INICIO == HORA_INICIO && w.Entity.ASISTENCIA == 1).OrderBy(o => o.FEC_REGISTRO).FirstOrDefault().ID_GRUPO == idgrupo;
                else
                    return listhora.Where(w => w.HORA_INICIO == HORA_INICIO).OrderBy(o => o.FEC_REGISTRO).FirstOrDefault().ID_GRUPO == idgrupo;
            }
            catch
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("Nope.");
        }
    }
    class IsVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var param = (object[])parameter;
                var listhora = (IEnumerable<DetalleEmpalmeHora>)param[0];
                var idgrupo = (short)param[1];
                var HORA_INICIO = (DateTime)value;

                return listhora.Where(w => w.ID_GRUPO == idgrupo && w.HORA_INICIO.Value.Date == HORA_INICIO.Date).Any() ? Visibility.Visible : Visibility.Collapsed;
            }
            catch
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("Nope.");
        }
    }
}
