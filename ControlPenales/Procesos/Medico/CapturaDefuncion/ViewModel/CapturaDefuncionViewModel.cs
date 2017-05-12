using System.Threading.Tasks;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ControlPenales.BiometricoServiceReference;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;
using SSP.Controlador.Catalogo.Justicia;
using MahApps.Metro.Controls;
using System.IO;
using Novacode;


namespace ControlPenales
{
    public partial class CapturaDefuncionViewModel:ValidationViewModelBase
    {
        

        #region Constructores
        public CapturaDefuncionViewModel()
        {

        }

        public CapturaDefuncionViewModel(INGRESO _ingreso, enumProcesos _proceso_origen)
        {
            selectedauxIngreso = _ingreso;
            proceso_origen = _proceso_origen;
            modo_seleccionado = MODO_OPERACION.Insertar;
        }

        #endregion

        #region Generales

        private async void ClickSwitch(object parametro)
        {
            if (parametro != null && !string.IsNullOrWhiteSpace(parametro.ToString()))
                switch (parametro.ToString())
                {
                    case "regresar_pantalla":
                        if (HasErrors)
                        {
                            if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                                String.Format("Falta capturar los campos obligatorios. \n\n {0} \n\n ¿Esta seguro de regresar a la pantalla anterior sin guardar la informacion del deceso?", Error)) == 1)
                            {
                                var metro = Application.Current.Windows[0] as MetroWindow;
                                GC.Collect();
                                ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).Content = new BitacoraIngresosEgresosHospitalizacionView();
                                ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext = new BitacoraIngresosEgresosHospitalizacionViewModel(enumProcesos.CAPTURADEFUNCION, Fechas.GetFechaDateServer, enumResultadoOperacion.CANCELADA);
                                return;
                            }
                            else return;
                        }
                        await Guardar();
                        if (!proceso_origen.HasValue && errorGuardar==false)
                        {
                            new Dialogos().ConfirmacionDialogo("EXITO!", "La tarjeta informativa de deceso fue guardada con exito");
                        }
                        else if (errorGuardar==false)
                        {
                            var metro = Application.Current.Windows[0] as MetroWindow;
                            GC.Collect();
                            ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).Content = new BitacoraIngresosEgresosHospitalizacionView();
                            ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext = new BitacoraIngresosEgresosHospitalizacionViewModel(enumProcesos.CAPTURADEFUNCION, FechaDeceso.Value,enumResultadoOperacion.EXITO);
                        }
                        break;
                    case "eliminar_enfermedad":
                        if (SelectEnfermedad==null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar una enfermedad de la lista");
                            return;
                        }
                        ListEnfermedades.Remove(ListEnfermedades.FirstOrDefault(w => w.ID_ENFERMEDAD == SelectEnfermedad.ID_ENFERMEDAD));
                        ListEnfermedades = new ObservableCollection<ENFERMEDAD>(ListEnfermedades);
                        IsEnfermedadValida = false;
                        break;
                    case "agregar_menu":
                        LimpiarBuscar();
                        modo_seleccionado = MODO_OPERACION.Insertar;
                        PopUpsViewModels.ShowPopUp(this,PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        break;
                    case "buscar_menu":
                        LimpiarBuscar();
                        modo_seleccionado = MODO_OPERACION.Reporte;
                        PopUpsViewModels.ShowPopUp(this,PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        break;
                    case "buscar_salir":
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        break;
                    case "nueva_busqueda":
                        LimpiarBuscar();
                        break;
                    case "buscar_seleccionar":
                        if (SelectIngreso == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar un ingreso de la lista");
                            return;
                        }
                        if (modo_seleccionado==MODO_OPERACION.Insertar)
                        {
                            if (SelectIngreso.NOTA_DEFUNCION != null)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "El ingreso ya cuenta con una tarjeta informativa de deceso");
                                return;
                            }
                            if (StaticSourcesViewModel.SourceChanged)
                            {
                                if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                                    "Existen cambios sin guardar,¿desea seleccionar otro ingreso?") != 1)
                                    return;
                            }
                            LimpiarCapturaDeceso();
                            selectedauxIngreso = SelectIngreso;
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {
                                CargarDatosIngreso(selectedauxIngreso);
                            });
                            IsCapturaDefuncionEnabled = true;
                            MenuGuardarEnabled = true;
                            MenuReporteEnabled = false;
                            menuBuscarEnabled = false;
                            setValidacionesCapturaDefuncion();
                            IsFechaDecesoValida = false;
                            IsEnfermedadValida = false;
                        }
                        else if (modo_seleccionado==MODO_OPERACION.Reporte)
                        {
                            if (SelectIngreso.NOTA_DEFUNCION == null)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "El ingreso no cuenta con una tarjeta informativa de deceso");
                                return;
                            }
                            if (StaticSourcesViewModel.SourceChanged)
                            {
                                if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                                    "Existen cambios sin guardar,¿desea seleccionar otro ingreso?") != 1)
                                    return;
                            }
                            LimpiarCapturaDeceso();
                            selectedauxIngreso = SelectIngreso;
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {
                                CargarDatosIngreso(selectedauxIngreso);
                                
                            });
                            FechaDeceso = selectedauxIngreso.NOTA_DEFUNCION.FECHA_DECESO;
                            TextLugarDeceso = selectedauxIngreso.NOTA_DEFUNCION.LUGAR;
                            ListEnfermedades = new ObservableCollection<ENFERMEDAD>( new List<ENFERMEDAD>{selectedauxIngreso.NOTA_DEFUNCION.ENFERMEDAD});
                            TextHechosDeceso = selectedauxIngreso.NOTA_DEFUNCION.HECHOS;
                            MenuGuardarEnabled = false;
                            MenuReporteEnabled = true;
                            MenuBuscarEnabled = true;
                            MenuAgregarEnabled = false;
                            IsEnfermedadValida = true;
                            ClearRules();
                        }
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        break;
                    case "guardar_menu":
                        if (HasErrors)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", String.Format("Falta capturar los campos obligatorios. \n\n {0}",Error));
                            return;
                        }
                        await Guardar();
                        if (!errorGuardar)
                            new Dialogos().ConfirmacionDialogo("EXITO!", "La tarjeta informativa de deceso fue guardada con exito");
                        MenuGuardarEnabled = false;
                        MenuReporteEnabled = true;
                        break;
                    case "limpiar_menu":
                        if (StaticSourcesViewModel.SourceChanged)
                    {
                        if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                            "Existen cambios sin guardar,¿desea limpiar la pantalla?") != 1)
                            return;
                    }
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new CapturaDefuncionView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new CapturaDefuncionViewModel();
                        break;
                    case "reporte_menu":
                        if (modo_seleccionado == MODO_OPERACION.Reporte)
                            reporte = selectedauxIngreso.NOTA_DEFUNCION.TARJETA_DECESO;
                        var tc = new TextControlView();
                        tc.Closed += (s, e) =>
                        {
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        };
                        tc.editor.Loaded += (s, e) =>
                        {
                            
                            //DOCX
                            tc.editor.EditMode = TXTextControl.EditMode.ReadOnly;
                            TXTextControl.LoadSettings _settings = new TXTextControl.LoadSettings();
                            tc.editor.Load(reporte, TXTextControl.BinaryStreamType.WordprocessingML, _settings);
                        };
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        tc.Owner = PopUpsViewModels.MainWindow;
                        tc.Show();
                        break;
                }
        }

        private void LimpiarCapturaDeceso()
        {
            selectedauxIngreso = null;
            TextAnioImputado = string.Empty;
            TextFolioImputado = string.Empty;
            TextPaternoImputado = string.Empty;
            TextMaternoImputado = string.Empty;
            TextNombreImputado = string.Empty;
            TextAliasImputado = string.Empty;
            TextEdadImputado = string.Empty;
            TextSexoImputado = string.Empty;
            TextOriginarioImputado = string.Empty;
            TextFechaIngreso = string.Empty;
            TextUltimaVisita = string.Empty;
            TextUltimaVisitaFecha = string.Empty;
            LstCausasPenales = null;
            TextCertMedDiagnostico = string.Empty;
            FechaDeceso = null;
            IsFechaDecesoValida = true;
            TextLugarDeceso = string.Empty;
            SelectEnfermedad = null;
            ListEnfermedades = null;
            TextHechosDeceso = string.Empty;
            selectedauxIngreso = null;
        }

        #region Buscar Imputado
        private void LimpiarBuscar()
        {
            SelectIngreso = null;
            ApellidoPaternoBuscar = string.Empty;
            ApellidoMaternoBuscar = string.Empty;
            NombreBuscar = string.Empty;
            AnioBuscar = null;
            FolioBuscar = null;
            ImagenImputado = null;
            EmptyExpedienteVisible = true;
            EmptyIngresoVisible = true;
            ListExpediente = null;
            SelectExpediente = null;
        }

        private void ClickBuscarInterno(object parametro)
        {
            buscarImputadoInterno(parametro);
        }

        private async void buscarImputadoInterno(Object obj = null)
        {
            ImagenIngreso = ImagenImputado = new Imagenes().getImagenPerson();
            ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
            ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
            SelectIngresoEnabled = false;
            if (ListExpediente != null)
                EmptyExpedienteVisible = ListExpediente.Count < 0;
            else
                EmptyExpedienteVisible = true;
        }


        private async Task<List<IMPUTADO>> SegmentarResultadoBusqueda(int _Pag = 1)
        {
            if (string.IsNullOrEmpty(ApellidoPaternoBuscar) && string.IsNullOrEmpty(ApellidoMaternoBuscar) && string.IsNullOrEmpty(NombreBuscar) && !AnioBuscar.HasValue && !FolioBuscar.HasValue)
                return new List<IMPUTADO>();

            Pagina = _Pag;
            var result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<IMPUTADO>>(() => new cImputado().ObtenerTodos(ApellidoPaternoBuscar, ApellidoMaternoBuscar, NombreBuscar, AnioBuscar, FolioBuscar, _Pag));
            if (result.Any())
            {
                Pagina++;
                SeguirCargando = true;
            }
            else
                SeguirCargando = false;

            return result.ToList();
        }
        #endregion
        private async Task Guardar()
        {
            try
            {
                var _fecha_servidor = Fechas.GetFechaDateServer;
                var _id_persona_coord = new cDepartamentosAcceso().ObtenerCoordinadorPorCentro(5, GlobalVar.gCentro).FirstOrDefault();
                if (_id_persona_coord == null)
                    throw new Exception("No se encuentra configurado el coordinador para la dirección médica del centro");
                if (await StaticSourcesViewModel.OperacionesAsync<bool>("Guardando solicitud", () =>
                    {
                        var _nota_defuncion = new NOTA_DEFUNCION {
                            FECHA_DECESO=fechaDeceso,
                            HECHOS=textHechosDeceso.Trim(),
                            ID_ANIO = selectedauxIngreso.ID_ANIO,
                            ID_CENTRO = selectedauxIngreso.ID_CENTRO,
                            ID_IMPUTADO = selectedauxIngreso.ID_IMPUTADO,
                            ID_INGRESO = selectedauxIngreso.ID_INGRESO,
                            LUGAR = textLugarDeceso,
                            ID_ENFERMEDAD=ListEnfermedades.First().ID_ENFERMEDAD,
                            ID_EMPLEADO_COORDINADOR_MED=_id_persona_coord.USUARIO.ID_PERSONA.Value,
                            FECHA_REGISTRO=_fecha_servidor,
                            USUARIO_REGISTRO=GlobalVar.gUsr
                        };
                        reporte= _nota_defuncion.TARJETA_DECESO = CrearTarjetaInformativaDeceso(_nota_defuncion, selectedauxIngreso);
                        var _isAltaHospitalizacion = selectedauxIngreso.ATENCION_MEDICA.Any(w => w.NOTA_MEDICA.HOSPITALIZACION.Any(w2 => w2.ID_HOSEST == 1));
                        new cNota_Defuncion().Insertar(_nota_defuncion,_fecha_servidor,(short)enumMensajeTipo.TARJETA_INFORMATIVA_DECESO, (short) enumMensajeTipo.TARJETA_INFORMATIVA_DECESO_ESTATAL,
                            _isAltaHospitalizacion, _isAltaHospitalizacion?(short?)enumMotivoEgreso.DEFUNCION:null);
                        return true;
                    }))
                {
                    errorGuardar = false;
                    return;
                }
                
            }
            catch (Exception ex)
            {
                errorGuardar = true;
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar la tarjeta informativa de deceso", ex);
            }
        }

        private async void CapturaDefuncionOnLoading(CapturaDefuncionView Window)
        {
            estatus_inactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
            await StaticSourcesViewModel.CargarDatosMetodoAsync(() => {
                #region AutoCompletes
                AutoCompleteTB = Window.AutoCompleteTB;
                AutoCompleteLB = AutoCompleteTB.Template.FindName("PART_ListBox", Window.AutoCompleteTB) as ListBox;
                AutoCompleteTB.PreviewMouseDown += new MouseButtonEventHandler(listBox_MouseUp);
                AutoCompleteTB.KeyDown += listBox_KeyDown;
                #endregion
                if (selectedauxIngreso != null)
                {
                    IsCapturaDefuncionEnabled = true;
                    IsMenuEnabled = false;
                    BotonRegresarVisible = Visibility.Visible;
                    CargarDatosIngreso(selectedauxIngreso);
                }
            });
            if (selectedauxIngreso!=null)
                setValidacionesCapturaDefuncion();
            StaticSourcesViewModel.SourceChanged = false;
        }

        private void CargarDatosIngreso(INGRESO _ingreso)
        {
            #region datos del ingreso
            textAnioImputado = _ingreso.ID_ANIO.ToString();
            RaisePropertyChanged("TextAnioImputado");
            textFolioImputado = _ingreso.ID_IMPUTADO.ToString();
            RaisePropertyChanged("TextFolioImputado");
            textMaternoImputado = _ingreso.IMPUTADO.MATERNO;
            RaisePropertyChanged("TextMaternoImputado");
            textNombreImputado = _ingreso.IMPUTADO.NOMBRE;
            RaisePropertyChanged("TextNombreImputado");
            var _strbuilder = new StringBuilder();
            _strbuilder.Append(selectedauxIngreso.IMPUTADO.MUNICIPIO.MUNICIPIO1.Trim()).Append(", ").Append(selectedauxIngreso.IMPUTADO.MUNICIPIO.ENTIDAD.DESCR.Trim()).Append(", ")
                .Append(selectedauxIngreso.IMPUTADO.MUNICIPIO.ENTIDAD.PAIS_NACIONALIDAD.PAIS.Trim());
            textOriginarioImputado= _strbuilder.ToString();
            RaisePropertyChanged("TextOriginarioImputado");
            textPaternoImputado = _ingreso.IMPUTADO.PATERNO;
            RaisePropertyChanged("TextPaternoImputado");
            textSexoImputado = _ingreso.IMPUTADO.SEXO == "M" ? "MASCULINO" : "FEMENINO";
            RaisePropertyChanged("TextSexoImputado");
            textEdadImputado = new Fechas().CalculaEdad(_ingreso.IMPUTADO.NACIMIENTO_FECHA.Value).ToString();
            RaisePropertyChanged("TextEdadImputado");
            _strbuilder.Clear();
            foreach (var item in _ingreso.IMPUTADO.ALIAS)
            {
                if (_strbuilder.Length != 0)
                    _strbuilder.Append(", ");
                _strbuilder.Append(item.NOMBRE.Trim());
                if (!string.IsNullOrWhiteSpace(item.PATERNO))
                    _strbuilder.Append(" ").Append(item.PATERNO.Trim());
                if (!string.IsNullOrWhiteSpace(item.MATERNO))
                    _strbuilder.Append(" ").Append(item.MATERNO.Trim());
            }
            foreach(var item in _ingreso.IMPUTADO.APODO)
            {
                if (_strbuilder.Length != 0)
                    _strbuilder.Append(", ");
                _strbuilder.Append(item.APODO1);
            }
            textAliasImputado = _strbuilder.ToString();
            RaisePropertyChanged("TextAliasImputado");
            imagenIngresoDeceso = _ingreso.INGRESO_BIOMETRICO.Any(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG) ?
                _ingreso.INGRESO_BIOMETRICO.First(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).BIOMETRICO :
                _ingreso.INGRESO_BIOMETRICO.Any(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG) ?
                _ingreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO :
                new Imagenes().getImagenPerson();
            RaisePropertyChanged("ImagenIngresoDeceso");
            #endregion
            #region datos legales
            textFechaIngreso = _ingreso.FEC_INGRESO_CERESO.Value.ToShortDateString();
            RaisePropertyChanged("TextFechaIngreso");
            var _ultima_visita = _ingreso.ADUANA_INGRESO.Where(w => w.ADUANA.ID_TIPO_PERSONA == (short)enumTipoPersona.PERSONA_VISITA).OrderByDescending(o => o.ID_ADUANA).FirstOrDefault();
            if (_ultima_visita != null)
            {
                _strbuilder.Clear();
                _strbuilder.Append(_ultima_visita.ADUANA.PERSONA.NOMBRE.Trim());
                if (!string.IsNullOrWhiteSpace(_ultima_visita.ADUANA.PERSONA.PATERNO))
                {
                    _strbuilder.Append(" ");
                    _strbuilder.Append(_ultima_visita.ADUANA.PERSONA.PATERNO.Trim());
                }
                if (!string.IsNullOrWhiteSpace(_ultima_visita.ADUANA.PERSONA.MATERNO))
                {
                    _strbuilder.Append(" ");
                    _strbuilder.Append(_ultima_visita.ADUANA.PERSONA.MATERNO.Trim());
                }
                textUltimaVisita = _strbuilder.ToString();
                RaisePropertyChanged("TextUltimaVisita");
                textUltimaVisitaFecha = _ultima_visita.ENTRADA_FEC.Value.ToShortDateString();
                RaisePropertyChanged("TextUltimaVisitaFecha");

            }
            lstCausasPenales = new ObservableCollection<EXT_DELITOS>(_ingreso.CAUSA_PENAL.Select(s => new EXT_DELITOS
            {
                CP_ANIO = s.CP_ANIO.ToString(),
                CP_FOLIO = s.CP_ANIO.ToString(),
                CP_BIS = s.CP_BIS,
                DELITOS = s.CAUSA_PENAL_DELITO.ToList()
            }));
            RaisePropertyChanged("LstCausasPenales");
            #endregion

            #region datos del deceso
            _strbuilder.Clear();
            if (_ingreso.ATENCION_MEDICA.Any(w => w.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CERTIFICADO_NUEVO_INGRESO))
                foreach (var item in _ingreso.ATENCION_MEDICA.First(w => w.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CERTIFICADO_NUEVO_INGRESO).NOTA_MEDICA.NOTA_MEDICA_ENFERMEDAD)
                {
                    if (_strbuilder.Length > 0)
                        _strbuilder.Append(", ");
                    _strbuilder.Append(item.ENFERMEDAD.NOMBRE.Trim());
                }
            if (_strbuilder.Length > 0)
            {
                textCertMedDiagnostico = _strbuilder.ToString();
                RaisePropertyChanged("TextCertMedDiagnostico");
            }
            #endregion
        }

        private async void OnModelChangedSwitch(object parametro)
        {
            switch (parametro.ToString())
            {
                case "cambio_fecha_defuncion":
                    if (!fechaDeceso.HasValue)
                        IsFechaDecesoValida = false;
                    else
                        IsFechaDecesoValida = true;
                    break;
                #region Cambio SelectedItem de Busqueda de Expediente
                case "cambio_expediente":
                    if (SelectExpediente != null && (SelectExpediente.INGRESO == null || SelectExpediente.INGRESO.Count == 0))
                    {
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            selectExpediente = new cImputado().Obtener(selectExpediente.ID_IMPUTADO, selectExpediente.ID_ANIO, selectExpediente.ID_CENTRO).First();
                            RaisePropertyChanged("SelectExpediente");
                        });
                        //MUESTRA LOS INGRESOS
                        if (selectExpediente.INGRESO != null && selectExpediente.INGRESO.Count > 0)
                        {
                            EmptyIngresoVisible = false;
                            SelectIngreso = selectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                        }
                        else
                        {
                            SelectIngreso = null;
                            EmptyIngresoVisible = true;
                        }


                        //OBTENEMOS FOTO DE FRENTE
                        if (SelectIngreso != null)
                        {
                            if (SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                                ImagenImputado = SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                            else
                                ImagenImputado = new Imagenes().getImagenPerson();
                            TextBotonSeleccionarIngreso = "aceptar";
                            SelectIngresoEnabled = true;


                            if (( modo_seleccionado==MODO_OPERACION.Insertar && estatus_inactivos != null && SelectIngreso != null && estatus_inactivos.Contains(SelectIngreso.ID_ESTATUS_ADMINISTRATIVO)) || (modo_seleccionado==MODO_OPERACION.Reporte && estatus_inactivos !=null && SelectIngreso!=null && SelectIngreso.NOTA_DEFUNCION==null && SelectIngreso.ID_UB_CENTRO==GlobalVar.gCentro))
                            {
                                TextBotonSeleccionarIngreso = "seleccionar ingreso";
                                SelectIngresoEnabled = false;
                            }
                        }
                    }
                    break;
                #endregion
            }
        }

        #region ENFERMEDAD

        private void listBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var popup = AutoCompleteTB.Template.FindName("PART_Popup", AutoCompleteTB) as System.Windows.Controls.Primitives.Popup;
                AutoCompleteLB = AutoCompleteTB.Template.FindName("PART_ListBox", AutoCompleteTB) as ListBox;
                var dep = (DependencyObject)e.OriginalSource;
                while ((dep != null) && !(dep is ListBoxItem))
                    dep = VisualTreeHelper.GetParent(dep);
                if (dep == null) return;
                var item = AutoCompleteLB.ItemContainerGenerator.ItemFromContainer(dep);
                if (item == null) return;
                //new ControlPenales.Controls.AutoCompleteTextBox().SetTextValueBySelection(item, false);
                if (item is ENFERMEDAD)
                {
                    ListEnfermedades = ListEnfermedades ?? new ObservableCollection<ENFERMEDAD>();
                    if (ListEnfermedades.Count==0 && !ListEnfermedades.Any(a => a.ID_ENFERMEDAD == ((ENFERMEDAD)item).ID_ENFERMEDAD))
                    {
                        ListEnfermedades.Insert(0, (ENFERMEDAD)item);
                        AutoCompleteTB.Text = string.Empty;
                        IsEnfermedadValida = true;
                    }
                    else if (ListEnfermedades.Count>0)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Solo puede agregar una enfermedad como posible causa");
                        popup.IsOpen = false;
                        return;
                    }
                        
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar una enfermedad.", ex);
            }
        }

        private void listBox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter || e.Key == Key.Return)
                {
                    var popup = AutoCompleteTB.Template.FindName("PART_Popup", AutoCompleteTB) as System.Windows.Controls.Primitives.Popup;
                    AutoCompleteLB = AutoCompleteTB.Template.FindName("PART_ListBox", AutoCompleteTB) as ListBox;
                    var dep = (DependencyObject)e.OriginalSource;
                    while ((dep != null) && !(dep is ListBoxItem))
                        dep = VisualTreeHelper.GetParent(dep);
                    if (dep == null) return;
                    var item = AutoCompleteLB.ItemContainerGenerator.ItemFromContainer(dep);
                    if (item == null) return;
                    if (item is ENFERMEDAD)
                    {
                        ListEnfermedades = ListEnfermedades ?? new ObservableCollection<ENFERMEDAD>();
                        if (ListEnfermedades.Count == 0 && !ListEnfermedades.Any(a => a.ID_ENFERMEDAD == ((ENFERMEDAD)item).ID_ENFERMEDAD))
                        {
                            ListEnfermedades.Insert(0, (ENFERMEDAD)item);
                            AutoCompleteTB.Text = string.Empty;
                            AutoCompleteTB.Focus();
                            IsEnfermedadValida = true;
                        }
                        else if (ListEnfermedades.Count > 0)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Solo puede agregar una enfermedad como posible causa");
                            popup.IsOpen = false;
                            return;
                        }
                        else
                            popup.IsOpen = false;
                    }
                }
                else if (e.Key == Key.Tab) { }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar una enfermedad.", ex);
            }
        }

        #endregion

        #region Documento
        private byte[] CrearTarjetaInformativaDeceso(NOTA_DEFUNCION _nota_defuncion, INGRESO _ingreso)
        {
            MemoryStream stream = new MemoryStream();
            using (DocX document = DocX.Create(stream))
            {
                var font_documento = new System.Drawing.FontFamily("Arial");
                const double font_size_body = 10D;
                const double font_size_ccp = 7D;
                const float indentation_left = 1.5F;
                const float indentation_right = 1.5F;
                #region Configuracion del Documento
                document.MarginLeft = 40;
                document.MarginRight = 40;
                document.AddHeaders();
                #endregion
                #region Header
                Header header_default = document.Headers.odd;
                // Insert a Paragraph into the default Header.
                Novacode.Table th = header_default.InsertTable(1,3);
                th.Design = TableDesign.None;
                float[] _witdhs = { 100, 500, 100 };
                th.SetWidths(_witdhs);
                MemoryStream ms_imagen = new MemoryStream(Parametro.REPORTE_LOGO2);
                Novacode.Image _logo2 = document.AddImage(ms_imagen);
                var pic1 = _logo2.CreatePicture();
                pic1.Width = 97;
                pic1.Height = 97;
                th.Rows[0].Cells[0].Paragraphs.First().InsertPicture(pic1);
                Novacode.Paragraph p1 = th.Rows[0].Cells[1].Paragraphs.First();
                p1.Alignment = Alignment.center;
                p1.AppendLine("Secretaria de Seguridad Publica").Bold().FontSize(11D).Font(font_documento);
                p1.AppendLine("Subsecretaria del Sistema Estatal Penitenciario").Bold().FontSize(11D).Font(font_documento);
                p1.AppendLine("Dirección de Programas de Reinserción Social").Bold().FontSize(11D).Font(font_documento);
                p1.AppendLine(_ingreso.CENTRO.DESCR.Trim()).Bold().FontSize(11D).Font(font_documento);
                p1.AppendLine("TARJETA INFORMATIVA DE DECESOS").Bold().FontSize(11D).Font(font_documento);
                ms_imagen.Close();
                ms_imagen = new MemoryStream(Parametro.REPORTE_LOGO1);
                Novacode.Image _logo1 = document.AddImage(ms_imagen);
                p1 = th.Rows[0].Cells[2].Paragraphs.First();
                p1.Alignment = Alignment.right;
                p1.InsertPicture(_logo1.CreatePicture());
                ms_imagen.Close();
                #endregion
                Novacode.Paragraph pbody = document.InsertParagraph();
                pbody.Alignment = Alignment.right;
                pbody.IndentationBefore = indentation_left;
                pbody.IndentationAfter = indentation_right;
                pbody.AppendLine();
                pbody.InsertText("FECHA Y LUGAR: ", false, new Formatting()
                {
                    Size=9D,
                    Bold=true,
                    FontFamily = font_documento
                });
                pbody.InsertText(string.Format("{0}, {1}, A {2}", _ingreso.CENTRO.MUNICIPIO.MUNICIPIO1.Trim(), _ingreso.CENTRO.MUNICIPIO.ENTIDAD.DESCR.Trim(),
                    Fechas.fechaLetra(Fechas.GetFechaDateServer, false).ToUpper()),false, new Formatting() {
                        Size = 9D,
                        Bold=true,
                        FontFamily = font_documento
                    });
                pbody.AppendLine();
                pbody = document.InsertParagraph();
                pbody.IndentationBefore = indentation_left;
                pbody.IndentationAfter = indentation_right;
                pbody.AppendLine(_ingreso.CENTRO.DIRECTOR.Trim()).Bold().FontSize(font_size_body).Font(font_documento);
                pbody.AppendLine("DIRECTOR DEL CENTRO DE REINSERCIÓN SOCIAL").Bold().FontSize(font_size_body).Font(font_documento);
                pbody.AppendLine("BC. SECRETARIA DE SEGURIDAD PUBLICA").Bold().FontSize(font_size_body).Font(font_documento);
                pbody.AppendLine("P R E S E N T E.").Bold().FontSize(font_size_body).Font(font_documento);
                pbody.AppendLine();
                pbody = document.InsertParagraph();
                pbody.Alignment = Alignment.left;
                pbody.IndentationBefore = indentation_left;
                pbody.IndentationAfter = indentation_right;
                pbody.Append("NOMBRE DEL INTERNO: ").Bold().UnderlineStyle(UnderlineStyle.singleLine).FontSize(font_size_body).Font(font_documento);
                StringBuilder _strbuilder = new StringBuilder();
                _strbuilder.Append(_ingreso.IMPUTADO.NOMBRE.Trim());
                if (!string.IsNullOrWhiteSpace(_ingreso.IMPUTADO.PATERNO))
                    _strbuilder.Append(" ").Append(_ingreso.IMPUTADO.PATERNO.Trim());
                if (!string.IsNullOrWhiteSpace(_ingreso.IMPUTADO.MATERNO))
                    _strbuilder.Append(" ").Append(_ingreso.IMPUTADO.MATERNO.Trim());
                pbody.Append(_strbuilder.ToString()).FontSize(font_size_body).Font(font_documento);
                _strbuilder.Clear();
                foreach (var item in _ingreso.IMPUTADO.ALIAS)
                {
                    if (_strbuilder.Length != 0)
                        _strbuilder.Append(", ");
                    _strbuilder.Append(item.NOMBRE.Trim());
                    if (!string.IsNullOrWhiteSpace(item.PATERNO))
                        _strbuilder.Append(" ").Append(item.PATERNO.Trim());
                    if (!string.IsNullOrWhiteSpace(item.MATERNO))
                        _strbuilder.Append(" ").Append(item.MATERNO.Trim());
                }
                foreach (var item in _ingreso.IMPUTADO.APODO)
                {
                    if (_strbuilder.Length != 0)
                        _strbuilder.Append(", ");
                    _strbuilder.Append(item.APODO1.Trim());
                }
                pbody = document.InsertParagraph();
                pbody.Alignment = Alignment.both;
                pbody.IndentationBefore = indentation_left;
                pbody.IndentationAfter = indentation_right;
                pbody.Append("ALIAS: ").Bold().FontSize(font_size_body).Font(font_documento);
                pbody.Append(_strbuilder.ToString()).FontSize(font_size_body).Font(font_documento);
                int contador = 1;
                foreach (var item in lstCausasPenales)
                {
                    pbody = document.InsertParagraph();
                    pbody.IndentationBefore = indentation_left;
                    pbody.IndentationAfter = indentation_right;
                    pbody.Append(string.Format("{0}.- DELITO: ", contador.ToString())).Bold().FontSize(8D).Font(font_documento);
                    pbody.Append(item.DELITOS_DESCR).FontSize(8D).Font(font_documento);
                    pbody = document.InsertParagraph();
                    pbody.IndentationBefore = indentation_left;
                    pbody.IndentationAfter = indentation_right;
                    pbody.Append("CAUSA PENAL: ").Bold().FontSize(8D).Font(font_documento);
                    pbody.Append(item.CAUSA_PENAL).FontSize(8D).Font(font_documento);
                    contador += 1;
                }
                Novacode.Table tbody = document.InsertTable(1, 2);
                tbody.Design = TableDesign.None;
                tbody.Alignment = Alignment.center;
                tbody.SetWidths(new float[] { 414, 200 });
                var doc_cell = tbody.Rows[0].Cells[0];
                doc_cell.MarginRight = 0;
                doc_cell.MarginRight = 0;
                pbody = doc_cell.Paragraphs.First();
                pbody.Alignment = Alignment.left;
                pbody.Append("INGRESO AL CERESO: ").Bold().FontSize(font_size_body).Font(font_documento);
                pbody.Append(Fechas.fechaLetra(_ingreso.FEC_INGRESO_CERESO.Value, false).ToUpper()).FontSize(font_size_body).Font(font_documento);
                pbody = tbody.Rows[0].Cells[1].Paragraphs.First();
                pbody.Alignment = Alignment.left;
                pbody.Append("A LAS: ").Bold().FontSize(font_size_body).Font(font_documento); ;
                pbody.Append(_ingreso.FEC_INGRESO_CERESO.Value.TimeOfDay.ToString(@"hh\:mm\:ss").ToUpper()).FontSize(font_size_body).Font(font_documento);
                pbody.Append(" HRS").FontSize(font_size_body).Font(font_documento); 
                tbody = document.InsertTable(1, 3);
                tbody.Alignment = Alignment.center;
                tbody.Design = TableDesign.None;
                tbody.SetWidths(new float[] { 110, 130, 374 });
                doc_cell = tbody.Rows[0].Cells[0];
                doc_cell.MarginRight = 0;
                doc_cell.MarginRight = 0;
                pbody = doc_cell.Paragraphs.First();
                pbody.Alignment = Alignment.left;
                pbody.Append("EDAD: ").Bold().FontSize(font_size_body).Font(font_documento);
                pbody.Append(new Fechas().CalculaEdad(_ingreso.IMPUTADO.NACIMIENTO_FECHA.Value).ToString()).FontSize(font_size_body).Font(font_documento);
                pbody.Append(" AÑOS").FontSize(font_size_body).Font(font_documento);
                doc_cell = tbody.Rows[0].Cells[1];
                doc_cell.MarginRight = 0;
                doc_cell.MarginRight = 0;
                pbody = doc_cell.Paragraphs.First();
                pbody.Alignment = Alignment.left;
                pbody.Append("SEXO: ").Bold().FontSize(font_size_body).Font(font_documento);
                pbody.Append(_ingreso.IMPUTADO.SEXO == "M" ? "MASCULINO" : "FEMENINO").FontSize(font_size_body).Font(font_documento);
                doc_cell = tbody.Rows[0].Cells[2];
                doc_cell.MarginRight = 0;
                doc_cell.MarginRight = 0;
                pbody = doc_cell.Paragraphs.First(); 
                pbody.Alignment = Alignment.left;
                pbody.Append("ORIGINARIO: ").Bold().FontSize(font_size_body).Font(font_documento); 
                _strbuilder.Clear();
                _strbuilder.Append(_ingreso.IMPUTADO.MUNICIPIO.MUNICIPIO1.Trim()).Append(", ").Append(_ingreso.IMPUTADO.MUNICIPIO.ENTIDAD.DESCR.Trim()).Append(", ")
                    .Append(_ingreso.IMPUTADO.MUNICIPIO.ENTIDAD.PAIS_NACIONALIDAD.PAIS.Trim());
                pbody.Append(_strbuilder.ToString()).FontSize(font_size_body).Font(font_documento);
                pbody = document.InsertParagraph();
                pbody.IndentationBefore = indentation_left;
                pbody.IndentationAfter = indentation_right;
                pbody.Alignment = Alignment.left;
                pbody.Append("VISITA FAMILIAR: ").Bold().FontSize(font_size_body).Font(font_documento);
                var _ultima_visita = _ingreso.ADUANA_INGRESO.Where(w => w.ADUANA.ID_TIPO_PERSONA == (short)enumTipoPersona.PERSONA_VISITA).OrderByDescending(o => o.ID_ADUANA).FirstOrDefault();
                if (_ultima_visita != null)
                {
                    _strbuilder.Clear();
                    _strbuilder.Append(_ultima_visita.ADUANA.PERSONA.NOMBRE.Trim());
                    if (!string.IsNullOrWhiteSpace(_ultima_visita.ADUANA.PERSONA.PATERNO))
                    {
                        _strbuilder.Append(" ");
                        _strbuilder.Append(_ultima_visita.ADUANA.PERSONA.PATERNO.Trim());
                    }
                    if (!string.IsNullOrWhiteSpace(_ultima_visita.ADUANA.PERSONA.MATERNO))
                    {
                        _strbuilder.Append(" ");
                        _strbuilder.Append(_ultima_visita.ADUANA.PERSONA.MATERNO.Trim());
                    }
                    pbody = document.InsertParagraph();
                    pbody.IndentationBefore = indentation_left;
                    pbody.IndentationAfter = indentation_right;
                    pbody.Alignment = Alignment.left;
                    pbody.Append(_strbuilder.ToString()).Bold().FontSize(font_size_body).Font(font_documento);
                    _strbuilder.Clear();
                    _strbuilder.Append(" (");
                    _strbuilder.Append(_ultima_visita.ADUANA.PERSONA.VISITANTE.VISITANTE_INGRESO.FirstOrDefault(w => w.ID_ANIO == _ingreso.ID_ANIO && w.ID_CENTRO == _ingreso.ID_CENTRO
                        && w.ID_IMPUTADO == _ingreso.ID_IMPUTADO && w.ID_INGRESO == _ingreso.ID_INGRESO).TIPO_REFERENCIA.DESCR.Trim());
                    _strbuilder.Append(")");
                    pbody.Append(_strbuilder.ToString()).FontSize(font_size_body).Font(font_documento);
                    pbody.Append(" ULTIMA VISITA: ").Bold().FontSize(font_size_body).Font(font_documento);
                    pbody.Append("EL ").FontSize(font_size_body).Font(font_documento);
                    pbody.Append(Fechas.fechaLetra(_ultima_visita.ENTRADA_FEC.Value)).FontSize(font_size_body).Font(font_documento);
                }
                pbody = document.InsertParagraph();
                pbody.IndentationBefore = indentation_left;
                pbody.IndentationAfter = indentation_right;
                pbody.Alignment = Alignment.left;
                pbody.Append("CERTIFICADO MEDICO DE NUEVO INGRESO REPORTA: ").Bold().FontSize(font_size_body).Font(font_documento);
                _strbuilder.Clear();
                if (_ingreso.ATENCION_MEDICA.Any(w => w.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CERTIFICADO_NUEVO_INGRESO))
                    foreach (var item in _ingreso.ATENCION_MEDICA.First(w => w.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CERTIFICADO_NUEVO_INGRESO).NOTA_MEDICA.NOTA_MEDICA_ENFERMEDAD)
                    {
                        if (_strbuilder.Length > 0)
                            _strbuilder.Append(", ");
                        _strbuilder.Append(item.ENFERMEDAD.NOMBRE.Trim());
                    }
                if (_strbuilder.Length > 0)
                {
                    pbody.Append(_strbuilder.ToString()).FontSize(font_size_body).Font(font_documento);
                }
                pbody = document.InsertParagraph();
                pbody.Alignment = Alignment.left;
                pbody.IndentationBefore = indentation_left;
                pbody.IndentationAfter = indentation_right;
                pbody.Append("FECHA Y LUGAR DEL DECESO: ").Bold().FontSize(font_size_body).Font(font_documento);
                pbody.Append(Fechas.fechaLetra(_nota_defuncion.FECHA_DECESO.Value, false).ToUpper()).FontSize(font_size_body).Font(font_documento);
                pbody.Append(" EN ").FontSize(font_size_body).Font(font_documento);
                pbody.Append(_nota_defuncion.LUGAR).FontSize(font_size_body).Font(font_documento);
                pbody = document.InsertParagraph();
                pbody.IndentationBefore = indentation_left;
                pbody.IndentationAfter = indentation_right;
                pbody.Alignment = Alignment.left;
                pbody.Append("CAUSA APARENTE DEL DECESO: ").Bold().FontSize(font_size_body).Font(font_documento);
                pbody.Append(ListEnfermedades.First().NOMBRE).FontSize(font_size_body).Font(font_documento);
                pbody = document.InsertParagraph();
                pbody.Alignment = Alignment.center;
                pbody.AppendLine();
                pbody.AppendLine();
                pbody.AppendLine();
                pbody.AppendLine("HECHOS:").Bold().FontSize(font_size_body).Font(font_documento);
                pbody = document.InsertParagraph();
                pbody.Alignment = Alignment.both;
                pbody.IndentationBefore = indentation_left;
                pbody.IndentationAfter = indentation_right;
                pbody.AppendLine(_nota_defuncion.HECHOS).FontSize(font_size_body).Font(font_documento);
                document.InsertParagraph();
                document.InsertParagraph();
                pbody = document.InsertParagraph();
                pbody.Alignment = Alignment.center;
                pbody.Append("ATENTAMENTE").Bold().FontSize(11D).Font(font_documento);
                document.InsertParagraph();
                document.InsertParagraph();
                pbody = document.InsertParagraph();
                pbody.Alignment = Alignment.center;
                var _coordinador = new cPersona().Obtener(_nota_defuncion.ID_EMPLEADO_COORDINADOR_MED.Value).FirstOrDefault();
                _strbuilder.Clear();
                _strbuilder.Append(_coordinador.NOMBRE.Trim());
                if (!string.IsNullOrWhiteSpace(_coordinador.PATERNO))
                    _strbuilder.Append(" ").Append(_coordinador.PATERNO.Trim());
                if (!string.IsNullOrWhiteSpace(_coordinador.MATERNO))
                    _strbuilder.Append(" ").Append(_coordinador.MATERNO.Trim());
                pbody.AppendLine(string.Format("DR. {0}", _strbuilder.ToString())).Bold().FontSize(11D).Font(font_documento);
                pbody.AppendLine("COORDINADOR MEDICO").Bold().FontSize(11D).Font(font_documento);
                pbody.AppendLine(_ingreso.CENTRO.DESCR.Trim()).Bold().FontSize(11D).Font(font_documento);
                var cedula=!string.IsNullOrWhiteSpace(_coordinador.EMPLEADO.CEDULA)?_coordinador.EMPLEADO.CEDULA:string.Empty;
                pbody.AppendLine(string.Format("CED. PROF. {0}", cedula)).Bold().FontSize(11D).Font(font_documento);
                document.InsertParagraph();
                pbody = document.InsertParagraph();
                pbody.IndentationBefore = indentation_left;
                pbody.IndentationAfter = indentation_right;
                var algo = pbody.AppendLine(string.Format("C.C.P.- {0}.- DIRECTOR DE {1}. PARA SU SUPERIOR CONOCIMIENTO.", _ingreso.CENTRO.DIRECTOR.Trim(), _ingreso.CENTRO.DESCR.Trim())).FontSize(font_size_ccp).Font(font_documento);
                pbody.AppendLine(string.Format("C.C.P.- {0}.- DIRECTOR DE PROGRAMAS DE REINSERCIÓN SOCIAL. PARA SU SUPERIOR CONOCIMIENTO.", Parametro.DIR_PROGRAMAS_ESTATAL.Trim())).FontSize(font_size_ccp).Font(font_documento);
                pbody.AppendLine(string.Format("C.C.P.- {0}.- JEFE DE PROGRAMAS DE REINSERCIÓN. PARA SU CONOCIMIENTO", Parametro.JEFE_PROGRAMAS_ESTATAL.Trim())).FontSize(font_size_ccp).Font(font_documento);
                pbody.AppendLine(string.Format("C.C.P.- {0}.- COORDINADOR MÉDICO ESTATAL. PARA SU CONOCIMIENTO", Parametro.JEFE_PROGRAMAS_ESTATAL.Trim())).FontSize(font_size_ccp).Font(font_documento);
                var departamento_acceso = new cDepartamentosAcceso().ObtenerCoordinadorPorCentro((short)enumDepartamentos.COORDINACION_TECNICA,_ingreso.ID_UB_CENTRO.Value).FirstOrDefault();
                _strbuilder.Clear();
                _strbuilder.Append(departamento_acceso.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim());
                if (!string.IsNullOrWhiteSpace(departamento_acceso.USUARIO.EMPLEADO.PERSONA.PATERNO))
                    _strbuilder.Append(" ").Append(departamento_acceso.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim());
                if (!string.IsNullOrWhiteSpace(departamento_acceso.USUARIO.EMPLEADO.PERSONA.MATERNO))
                    _strbuilder.Append(" ").Append(departamento_acceso.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim());
                pbody.AppendLine(string.Format("C.C.P.- {0}.- COORDINADOR DE AREAS TECNICAS DEL {1}. PARA SU CONOCIMIENTO", _strbuilder.ToString(), _ingreso.CENTRO.DESCR.Trim())).FontSize(font_size_ccp).Font(font_documento);
                pbody.AppendLine("C.C.P.- ARCHIVO").FontSize(font_size_ccp).Font(font_documento);
                document.Save();
            }
            var _bytes = stream.ToArray();
            stream.Close();
            return _bytes;
        }  
        #endregion
        #endregion

        private enum MODO_OPERACION
        {
            Insertar=1,
            Reporte=2
        }
    }
}
