using ControlPenales.BiometricoServiceReference;
using System.Linq;
namespace ControlPenales
{
    public partial class HojaControlLiquidosViewModel : ValidationViewModelBase
    {
        public HojaControlLiquidosViewModel() { }

        private async void PageLoad(HojaControlLiquidosView Window = null)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    InicializaListas();
                    ConfiguraPermisos();
                });

                StaticSourcesViewModel.SourceChanged = false;
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar información.", ex);
            }
        }

        #region [PERMISOS]
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new SSP.Controlador.Catalogo.Justicia.cProcesoUsuario().Obtener(enumProcesos.HOJA_CONTROL_LIQUIDOS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                foreach (var p in permisos)
                {
                    if (p.INSERTAR == 1)
                        PInsertar = MenuGuardarEnabled = true;
                    if (p.EDITAR == 1)
                        PEditar = MenuGuardarEnabled = true;
                    if (p.CONSULTAR == 1)
                        PConsultar = MenuBuscarEnabled = true;
                    if (p.IMPRIMIR == 1)
                        PImprimir = MenuFichaEnabled = MenuReporteEnabled = true;
                };

                var _detalleUsuarioActual = new SSP.Controlador.Catalogo.Justicia.cUsuario().GetData(x => x.ID_USUARIO.Trim() == GlobalVar.gUsr).FirstOrDefault();
                if (_detalleUsuarioActual != null)
                    if (_detalleUsuarioActual.USUARIO_ROL.Any(x => x.ID_ROL == (short)eRolesMedi.ENFERMERO))
                        EsEnfermero = true;
                    else
                    {
                        LimpiaValidacionesLiquidosIngreso();
                        AgregarMenuEnabled = EditarEnabled = EditarMenuEnabled = false;
                        PInsertar = MenuGuardarEnabled = false;
                        PEditar = MenuGuardarEnabled = false;
                        PConsultar = MenuBuscarEnabled = false;
                        PImprimir = MenuFichaEnabled = MenuReporteEnabled = true;
                    };
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }
        #endregion

        private void InicializaListas()
        {
            try
            {
                #region HORAS
                ListHorasLiquidos = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO_HORA>(new SSP.Controlador.Catalogo.Justicia.cLiquidoHora().GetData(x => x.ESTATUS == "S", y => y.ID_LIQHORA.ToString()));
                ListHorasLiquidos.Insert(0, new SSP.Servidor.LIQUIDO_HORA { DESCR = "SELECCIONE", ID_LIQHORA = -1 });
                SelectedHoraLiquidos = -1;

                ListTipoLiquido = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO_TIPO>(new SSP.Controlador.Catalogo.Justicia.cLiquidoTipo().GetData(x => x.ESTATUS == "S"));

                RaisePropertyChanged("ListTipoLiquido");
                RaisePropertyChanged("ListHorasLiquidos");
                OnPropertyChanged("SelectedHoraLiquidos");
                #endregion

                #region LIQUIDOS
                System.Windows.Application.Current.Dispatcher.Invoke((System.Action)(delegate
                {
                    ListLiquidosIngresos = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO>(/*new SSP.Controlador.Catalogo.Justicia.cLiquido().GetData()*/);
                    ListLiquidosIngresos.Insert(0, new SSP.Servidor.LIQUIDO { ID_LIQ = -1, DESCR = "SELECCIONE" });
                    ListTipoLiquido.Insert(0, new SSP.Servidor.LIQUIDO_TIPO { DESCR = "SELECCIONE", ID_LIQTIPO = "-1" });
                }));

                RaisePropertyChanged("ListTipoLiquido");
                RaisePropertyChanged("ListLiquidosIngresos");
                #endregion

                FecSeleccionadaregistro = Fechas.GetFechaDateServer;
                OnPropertyChanged("FecSeleccionadaregistro");
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private async void clickSwitch(System.Object obj)
        {
            switch (obj.ToString())
            {
                case "nueva_busqueda":
                    NombreBuscar = ApellidoPaternoBuscar = ApellidoMaternoBuscar = string.Empty;
                    FolioBuscar = AnioBuscar = new int?();
                    ListExpediente = new RangeEnabledObservableCollection<SSP.Servidor.IMPUTADO>();
                    break;
                case "salir_menu":
                    PrincipalViewModel.SalirMenu();
                    break;
                case "buscar_visible":
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "buscar_salir":
                    NombreBuscar = ApellidoPaternoBuscar = ApellidoMaternoBuscar = string.Empty;
                    AnioBuscar = FolioBuscar = null;
                    ListExpediente = new RangeEnabledObservableCollection<SSP.Servidor.IMPUTADO>();
                    SelectExpediente = null;
                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "limpiar_menu":
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new HojaControlLiquidosView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.HojaControlLiquidosViewModel();
                    break;

                case "buscar_menu":
                    ListExpediente = new RangeEnabledObservableCollection<SSP.Servidor.IMPUTADO>();
                    SelectExpediente = new SSP.Servidor.IMPUTADO();
                    EmptyExpedienteVisible = true;
                    ApellidoPaternoBuscar = ApellidoMaternoBuscar = NombreBuscar = string.Empty;
                    FolioBuscar = AnioBuscar = null;
                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "buscar_seleccionar":
                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un ingreso vigente");
                        return;
                    }

                    foreach (var item in Parametro.ESTATUS_ADMINISTRATIVO_INACT)
                        if (SelectIngreso.ID_ESTATUS_ADMINISTRATIVO == item)
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningún ingreso activo en este imputado.");
                            return;
                        }

                    if (SelectIngreso.ID_UB_CENTRO.HasValue && SelectIngreso.ID_UB_CENTRO != GlobalVar.gCentro)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                        return;
                    }

                    if (SelectIngreso.TRASLADO_DETALLE.Any(a => (a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-Parametro.TOLERANCIA_TRASLADO_EDIFICIO) <= Fechas.GetFechaDateServer))
                    {
                        new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + SelectIngreso.ID_ANIO.ToString() + "/" +
                            SelectIngreso.ID_IMPUTADO.ToString() + "] tiene un traslado próximo y no tiene permitido ningún cambio de información.");
                        return;
                    }

                    SelectedInterno = SelectIngreso.IMPUTADO;
                    if (SelectIngreso != null)
                        SelectIngreso = SelectIngreso;

                    var _validacionHospitalizado = new SSP.Controlador.Catalogo.Justicia.Medico.cHospitalizacion().ObtenerHospitalizacionesPorIngreso(SelectIngreso);
                    if (!_validacionHospitalizado.Any())
                    {
                        SelectExpediente = null;
                        SelectIngreso = null;
                        ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no está hospitalizado.");
                        return;
                    }

                    SeleccionaIngreso();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    StaticSourcesViewModel.SourceChanged = false;
                    break;
                case "guardar_menu":
                    if (SelectExpediente == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un imputado.");
                        break;
                    }

                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un ingreso valido.");
                        break;
                    }

                    ValidacionesSignosVitales();
                    if (HasErrors)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", base.Error);
                        return;
                    };


                    if (ListTipoLiquido == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe agregar al menos un líquido a la hoja de líquidos.");
                        break;
                    }

                    if (ListTipoLiquido.Count == 0)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe agregar al menos un líquido a la hoja de líquidos.");
                        break;
                    }

                    if (SelectedHoraLiquidos == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar una hora.");
                        break;
                    }

                    if (SelectedHoraLiquidos == -1)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar una hora.");
                        break;
                    }

                    if (FecSeleccionadaregistro == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar una fecha.");
                        break;
                    }

                    if (!ValidaCapturaLiquidos())
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Ya existe registro de control de líquidos para la hora y fecha seleccionadas.");
                        break;
                    }

                    if (await StaticSourcesViewModel.OperacionesAsync<bool>("Ingresando hoja de control de líquidos", () => GuardaControlLiquidos()))
                    {
                        LimpiaCamposIngresaLiquido();
                        LimpiaDatosHoja();
                        ListLiquidosIngresoEditar = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO_HOJA_CTRL_DETALLE>();
                        (new Dialogos()).ConfirmacionDialogo("Exito!", "Se ha registrado la hoja de control de líquidos exitosamente");
                        StaticSourcesViewModel.SourceChanged = false;
                    }
                    else
                        (new Dialogos()).ConfirmacionDialogo("Error!", "Surgió un error al ingresar la hoja de control de líquidos");
                    break;

                case "reporte_menu":
                    if (SelectExpediente == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un imputado.");
                        break;
                    }

                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un ingreso valido.");
                        break;
                    }

                    if (FechaInicioBusqueda == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar una fecha para generar el reporte.");
                        break;
                    }

                    if (Opcion == (short)ePosicionActual.CAPTURA)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "No es posible generar el reporte en el apartado de captura.");
                        break;
                    }

                    ImprimeReporteHojaControlLiquidos();
                    break;

                case "consultar_hoja_control_liquidos":
                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un ingreso valido.");
                        break;
                    }

                    if (FechaInicioBusqueda == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Ingrese una fecha para consultar.");
                        break;
                    }

                    ConsultaHistorico();
                    break;

                case "agregar_liquido_hoja_control":
                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un ingreso valido.");
                        break;
                    }

                    if (SelectedTipoLiquido.HasValue && SelectedTipoLiquido != -1)
                        ValidacionesLiquidosIngreso();
                    else
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un líquido para agregar a la lista.");
                        break;
                    }

                    if (HasErrors)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", base.Error);
                        return;
                    };

                    AgregaLiquidoIngreso();
                    break;

                #region CONCENTRADO
                case "agregar_concentrado":
                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un ingreso valido.");
                        break;
                    }

                    InicializaEntornoConcentradoLiquidos();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_CONCENTRADO_HOJA_LIQUIDOS);
                    break;
                case "cancelar_concentrado":
                    StaticSourcesViewModel.SourceChanged = false;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_CONCENTRADO_HOJA_LIQUIDOS);
                    break;

                case "guardar_concentrado":
                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un ingreso valido.");
                        break;
                    }

                    if (!TotalEntradas.HasValue || !TotalSalidas.HasValue || !TotalBalance.HasValue)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe existir al menos un valor registrado para guardar el concentrado.");
                        break;
                    }

                    int result = 0;
                    result = await (new Dialogos()).ConfirmacionTresBotonesDinamico("Guardado de Concentrado", "Una vez guardado NO ES POSIBLE volver a generar el concentrado ¿Desea Continuar?", "SI", 1, "NO", 2, "Cancelar", 0);
                    switch (result)
                    {
                        case 0:
                            StaticSourcesViewModel.SourceChanged = false;
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_CONCENTRADO_HOJA_LIQUIDOS);
                            break;
                        case 1:
                            if (await StaticSourcesViewModel.OperacionesAsync<bool>("Ingresando Concentrado", () => GuardarConcentrado()))
                            {
                                (new Dialogos()).ConfirmacionDialogo("Exito!", "Se ha registrado el concentrado exitosamente");
                                StaticSourcesViewModel.SourceChanged = false;
                                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_CONCENTRADO_HOJA_LIQUIDOS);
                            }
                            else
                                (new Dialogos()).ConfirmacionDialogo("Error!", "Surgió un error al ingresar el concentrado");
                            break;

                        case 2:
                            StaticSourcesViewModel.SourceChanged = false;
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_CONCENTRADO_HOJA_LIQUIDOS);
                            break;

                        default:
                            //no action
                            return;
                    };
                    break;
                #endregion
            }
        }

        private bool ValidaCapturaLiquidos()
        {
            try
            {
                var DetallesHora = new SSP.Controlador.Catalogo.Justicia.cLiquidoHora().GetData().FirstOrDefault(x => x.ID_LIQHORA == SelectedHoraLiquidos);
                if (DetallesHora != null)
                {
                    int _Hora = int.Parse(DetallesHora.DESCR);
                    var fecha_minima = new System.DateTime(
                        FecSeleccionadaregistro.Value.Year,
                        FecSeleccionadaregistro.Value.Month,
                        FecSeleccionadaregistro.Value.Day,
                        FecSeleccionadaregistro.Value.Hour,
                        FecSeleccionadaregistro.Value.Minute,
                        FecSeleccionadaregistro.Value.Second);// FECHA FORMADA CON LA HORA Y LA FECHA SELECCIONADA DESDE EL CONTROL
                    if (new SSP.Controlador.Catalogo.Justicia.cLiquidoHojaCtrl().ObtenerHojaLiquidos(SelectedHospitalizacion, fecha_minima, SelectedHoraLiquidos).Any())
                        return false;
                    else
                        return true;
                }
                else
                    return false;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
        private void ConsultaHistorico()
        {
            try
            {
                LeyendaIngresos = "INGRESOS \n";//SE FORMAN LAS LEYENDAS PARA QUE INTERACTUEN CON EL GRID Y SIRVAN DE DETALLE AL USUARIO
                LeyendaEgresos = "EGRESOS \n";
                LstEspecialConsultaHojaLiquidos = new System.Collections.ObjectModel.ObservableCollection<GridEspecialconsultaHojaLiquidos>();
                var LiquidosExistentes = new SSP.Controlador.Catalogo.Justicia.cLiquido().GetData();
                System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO> listaDetalleIn = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO>(new SSP.Controlador.Catalogo.Justicia.cLiquido().GetData(x => x.ID_LIQTIPO == "1" && x.ID_LIQ_FORMATO == 1));
                System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO> listaDetalleEg = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO>(new SSP.Controlador.Catalogo.Justicia.cLiquido().GetData(x => x.ID_LIQTIPO == "2" && x.ID_LIQ_FORMATO == 1));
                if (listaDetalleIn.Any())
                    foreach (var item in listaDetalleIn)
                        LeyendaIngresos += string.Format("    {0} \n", !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty);

                if (listaDetalleEg.Any())
                    foreach (var item in listaDetalleEg)
                        LeyendaEgresos += string.Format("    {0} \n", !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty);

                //quita espacio restante
                LeyendaIngresos = LeyendaIngresos.TrimEnd('\n');
                LeyendaEgresos = LeyendaEgresos.TrimEnd('\n');
                OnPropertyChanged("LeyendaIngresos");
                OnPropertyChanged("LeyendaEgresos");

                var _DetallesHojaLiquidos = new SSP.Controlador.Catalogo.Justicia.cLiquidoHojaCtrl().ObtenerHojaLiquidos(SelectedHospitalizacion, FechaInicioBusqueda);
                var _CantidadHoras = new SSP.Controlador.Catalogo.Justicia.cLiquidoHora().GetData(x => x.ESTATUS == "S");
                ListaLiquidos = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO>(new SSP.Controlador.Catalogo.Justicia.cLiquido().GetData(x => x.ID_LIQ_FORMATO == 1));
                RaisePropertyChanged("ListaLiquidos");

                System.Collections.Generic.List<ComplementoiquidosEspecial> listaIn = new System.Collections.Generic.List<ComplementoiquidosEspecial>();
                System.Collections.Generic.List<ComplementoiquidosEspecial> listaEg = new System.Collections.Generic.List<ComplementoiquidosEspecial>();

                #region CONCENTRADOS
                System.Collections.Generic.List<SSP.Servidor.LIQUIDO_HOJA_CTRL_CONCEN> ListaConcentrados = new System.Collections.Generic.List<SSP.Servidor.LIQUIDO_HOJA_CTRL_CONCEN>(new SSP.Controlador.Catalogo.Justicia.cLiquidoHojaCtrlConcen().GetData(x => x.ID_HOSPITA == SelectedHospitalizacion && x.CONCENTRADO_FEC.Value.Year == FechaInicioBusqueda.Value.Year && x.CONCENTRADO_FEC.Value.Month == FechaInicioBusqueda.Value.Month && x.CONCENTRADO_FEC.Value.Day == FechaInicioBusqueda.Value.Day));
                if (ListaConcentrados.Any())
                {
                    EntradasMatutino = SalidasMatutino = BalanceMatutino = EntradasVespertino = SalidasVespertino = BalanceVespertino = EntradasNocturno = SalidasNocturno = BalanceNocturno = BalanceTotal = SalidasTotal = EntradasTotal = null;
                    NombreMatutino = NombreVespertino = NombreNocturno = NombreTotal = string.Empty;//LIMPIA ANTES DE CONSULTAR DE NUEVO

                    SSP.Servidor.LIQUIDO_HOJA_CTRL_CONCEN _TurnoMatutino = ListaConcentrados.FirstOrDefault(x => x.ID_CONCENTIPO == (decimal)eTipoConcentrados.MATUTINO);
                    SSP.Servidor.LIQUIDO_HOJA_CTRL_CONCEN _TurnoVespertino = ListaConcentrados.FirstOrDefault(x => x.ID_CONCENTIPO == (decimal)eTipoConcentrados.VESPERTINO);
                    SSP.Servidor.LIQUIDO_HOJA_CTRL_CONCEN _TurnoNocturno = ListaConcentrados.FirstOrDefault(x => x.ID_CONCENTIPO == (decimal)eTipoConcentrados.NOCTURNO);
                    SSP.Servidor.LIQUIDO_HOJA_CTRL_CONCEN _TurnoTotal = ListaConcentrados.FirstOrDefault(x => x.ID_CONCENTIPO == (decimal)eTipoConcentrados.TOTAL);

                    if (_TurnoMatutino != null)
                    {
                        EntradasMatutino = _TurnoMatutino.ENTRADA;
                        SalidasMatutino = _TurnoMatutino.SALIDA;
                        BalanceMatutino = _TurnoMatutino.ENTRADA - _TurnoMatutino.SALIDA;
                        var _detallesUsuario = !string.IsNullOrEmpty(_TurnoMatutino.REGISTRO_USUARIO) ? new SSP.Controlador.Catalogo.Justicia.cUsuario().GetData(x => x.ID_USUARIO.Trim() == _TurnoMatutino.REGISTRO_USUARIO.Trim()).FirstOrDefault() : null;
                        NombreMatutino = _detallesUsuario != null ? string.Format("{0} {1} {2}",
                            _detallesUsuario.EMPLEADO != null ? _detallesUsuario.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(_detallesUsuario.EMPLEADO.PERSONA.NOMBRE) ? _detallesUsuario.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                            _detallesUsuario.EMPLEADO != null ? _detallesUsuario.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(_detallesUsuario.EMPLEADO.PERSONA.PATERNO) ? _detallesUsuario.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                            _detallesUsuario.EMPLEADO != null ? _detallesUsuario.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(_detallesUsuario.EMPLEADO.PERSONA.MATERNO) ? _detallesUsuario.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty : string.Empty : string.Empty) : string.Empty;
                    };

                    if (_TurnoVespertino != null)
                    {
                        EntradasVespertino = _TurnoVespertino.ENTRADA;
                        SalidasVespertino = _TurnoVespertino.SALIDA;
                        BalanceVespertino = _TurnoVespertino.ENTRADA - _TurnoVespertino.SALIDA;
                        var _detallesUsuario = !string.IsNullOrEmpty(_TurnoVespertino.REGISTRO_USUARIO) ? new SSP.Controlador.Catalogo.Justicia.cUsuario().GetData(x => x.ID_USUARIO.Trim() == _TurnoVespertino.REGISTRO_USUARIO.Trim()).FirstOrDefault() : null;
                        NombreVespertino = _detallesUsuario != null ? string.Format("{0} {1} {2}",
                            _detallesUsuario.EMPLEADO != null ? _detallesUsuario.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(_detallesUsuario.EMPLEADO.PERSONA.NOMBRE) ? _detallesUsuario.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                            _detallesUsuario.EMPLEADO != null ? _detallesUsuario.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(_detallesUsuario.EMPLEADO.PERSONA.PATERNO) ? _detallesUsuario.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                            _detallesUsuario.EMPLEADO != null ? _detallesUsuario.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(_detallesUsuario.EMPLEADO.PERSONA.MATERNO) ? _detallesUsuario.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty : string.Empty : string.Empty) : string.Empty;
                    };


                    if (_TurnoNocturno != null)
                    {
                        EntradasNocturno = _TurnoNocturno.ENTRADA;
                        SalidasNocturno = _TurnoNocturno.SALIDA;
                        BalanceNocturno = _TurnoNocturno.ENTRADA - _TurnoNocturno.SALIDA;
                        var _detallesUsuario = !string.IsNullOrEmpty(_TurnoNocturno.REGISTRO_USUARIO) ? new SSP.Controlador.Catalogo.Justicia.cUsuario().GetData(x => x.ID_USUARIO.Trim() == _TurnoNocturno.REGISTRO_USUARIO.Trim()).FirstOrDefault() : null;
                        NombreNocturno = _detallesUsuario != null ? string.Format("{0} {1} {2}",
                            _detallesUsuario.EMPLEADO != null ? _detallesUsuario.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(_detallesUsuario.EMPLEADO.PERSONA.NOMBRE) ? _detallesUsuario.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                            _detallesUsuario.EMPLEADO != null ? _detallesUsuario.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(_detallesUsuario.EMPLEADO.PERSONA.PATERNO) ? _detallesUsuario.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                            _detallesUsuario.EMPLEADO != null ? _detallesUsuario.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(_detallesUsuario.EMPLEADO.PERSONA.MATERNO) ? _detallesUsuario.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty : string.Empty : string.Empty) : string.Empty;
                    };

                    if (_TurnoTotal != null)
                    {
                        EntradasTotal = _TurnoTotal.ENTRADA;
                        SalidasTotal = _TurnoTotal.SALIDA;
                        BalanceTotal = _TurnoTotal.ENTRADA - _TurnoTotal.SALIDA;
                        var _detallesUsuario = !string.IsNullOrEmpty(_TurnoTotal.REGISTRO_USUARIO) ? new SSP.Controlador.Catalogo.Justicia.cUsuario().GetData(x => x.ID_USUARIO.Trim() == _TurnoTotal.REGISTRO_USUARIO.Trim()).FirstOrDefault() : null;
                        NombreTotal = _detallesUsuario != null ? string.Format("{0} {1} {2}",
                            _detallesUsuario.EMPLEADO != null ? _detallesUsuario.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(_detallesUsuario.EMPLEADO.PERSONA.NOMBRE) ? _detallesUsuario.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                            _detallesUsuario.EMPLEADO != null ? _detallesUsuario.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(_detallesUsuario.EMPLEADO.PERSONA.PATERNO) ? _detallesUsuario.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                            _detallesUsuario.EMPLEADO != null ? _detallesUsuario.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(_detallesUsuario.EMPLEADO.PERSONA.MATERNO) ? _detallesUsuario.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty : string.Empty : string.Empty) : string.Empty;
                    };
                }
                else
                {
                    //NO HAY NADA, LIMPIA LOS REGISTROS QUE HABIA ANTES
                    EntradasMatutino = SalidasMatutino = BalanceMatutino = EntradasVespertino = SalidasVespertino = BalanceVespertino = EntradasNocturno = SalidasNocturno = BalanceNocturno = BalanceTotal = SalidasTotal = EntradasTotal = null;
                    NombreMatutino = NombreVespertino = NombreNocturno = NombreTotal = string.Empty;
                }
                #endregion

                if (_CantidadHoras.Any())
                    foreach (var item in _CantidadHoras)
                    {
                        if (_DetallesHojaLiquidos != null && _DetallesHojaLiquidos.Any())
                        {
                            if (_DetallesHojaLiquidos.Any(x => x.ID_LIQHORA == item.ID_LIQHORA))
                            {
                                var _detallesHoraExiste = _DetallesHojaLiquidos.FirstOrDefault(x => x.ID_LIQHORA == item.ID_LIQHORA);
                                if (_detallesHoraExiste != null)
                                {
                                    string a = "\n";
                                    string b = "\n";
                                    var _Ingreso = _detallesHoraExiste.LIQUIDO_HOJA_CTRL_DETALLE != null ? _detallesHoraExiste.LIQUIDO_HOJA_CTRL_DETALLE.Where(x => x.LIQUIDO.ID_LIQTIPO == "1" && x.LIQUIDO.ID_LIQ_FORMATO == 1) : null;
                                    var _Egreso = _detallesHoraExiste.LIQUIDO_HOJA_CTRL_DETALLE != null ? _detallesHoraExiste.LIQUIDO_HOJA_CTRL_DETALLE.Where(x => x.LIQUIDO.ID_LIQTIPO == "2" && x.LIQUIDO.ID_LIQ_FORMATO == 1) : null;
                                    if (listaDetalleIn != null && listaDetalleIn.Any())
                                    {
                                        foreach (var itemX in listaDetalleIn)
                                        {
                                            var _exis = _Ingreso != null ? _Ingreso.FirstOrDefault(x => x.ID_LIQ == itemX.ID_LIQ) : null;
                                            a += string.Format("{0} \n", _exis != null ? _exis.CANT.HasValue ? string.Format("{0} ml.", _exis.CANT.Value.ToString()) : "*" : "*");
                                        };
                                    };

                                    if (listaDetalleEg != null && listaDetalleEg.Any())
                                    {
                                        foreach (var itemY in listaDetalleEg)
                                        {
                                            var _exis = _Egreso != null ? _Egreso.FirstOrDefault(x => x.ID_LIQ == itemY.ID_LIQ) : null;
                                            b += string.Format("{0} \n", _exis != null ? _exis.CANT.HasValue ? string.Format("{0} ml.", _exis.CANT.Value.ToString()) : "*" : "*");
                                        };
                                    };

                                    a = !string.IsNullOrEmpty(a) ? a.TrimEnd('\n') : string.Empty;
                                    b = !string.IsNullOrEmpty(b) ? b.TrimEnd('\n') : string.Empty;
                                    LstEspecialConsultaHojaLiquidos.Add(new GridEspecialconsultaHojaLiquidos
                                        {
                                            FrecuenciaCard = _detallesHoraExiste.FRECUENCIA_CARDIACA,
                                            FrecuenciaRespiratoria = _detallesHoraExiste.FRECUENCIA_RESPIRATORIA,
                                            Hora = !string.IsNullOrEmpty(item.DESCR) ? short.Parse(item.DESCR) : new short(),
                                            Temperatura = _detallesHoraExiste.TEMPERATURA,
                                            TensionArt = _detallesHoraExiste.TENSION_ARTERIAL,
                                            Detalles1 = a,
                                            Detalles2 = b,
                                        });
                                }
                                else
                                    LstEspecialConsultaHojaLiquidos.Add(new GridEspecialconsultaHojaLiquidos
                                    {
                                        FrecuenciaCard = string.Empty,
                                        FrecuenciaRespiratoria = string.Empty,
                                        Hora = !string.IsNullOrEmpty(item.DESCR) ? short.Parse(item.DESCR) : new short(),
                                        Temperatura = string.Empty,
                                        TensionArt = string.Empty,
                                    });
                            }
                            else
                                LstEspecialConsultaHojaLiquidos.Add(new GridEspecialconsultaHojaLiquidos
                                {
                                    FrecuenciaCard = string.Empty,
                                    FrecuenciaRespiratoria = string.Empty,
                                    Hora = !string.IsNullOrEmpty(item.DESCR) ? short.Parse(item.DESCR) : new short(),
                                    Temperatura = string.Empty,
                                    TensionArt = string.Empty,
                                });
                        }
                        else
                            LstEspecialConsultaHojaLiquidos.Add(new GridEspecialconsultaHojaLiquidos
                            {
                                FrecuenciaCard = string.Empty,
                                FrecuenciaRespiratoria = string.Empty,
                                Hora = !string.IsNullOrEmpty(item.DESCR) ? short.Parse(item.DESCR) : new short(),
                                Temperatura = string.Empty,
                                TensionArt = string.Empty,
                            });
                    }
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private void ImprimeReporteHojaControlLiquidos()
        {
            try
            {
                if (SelectIngreso == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un ingreso valido.");
                    return;
                }

                ReportesView View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };

                View.Show();

                var _validacionHospitalizado = new SSP.Controlador.Catalogo.Justicia.Medico.cHospitalizacion().ObtenerHospitalizacionesPorIngreso(SelectIngreso);
                var _UnicaH = _validacionHospitalizado != null ? _validacionHospitalizado.Any() ? _validacionHospitalizado.FirstOrDefault(x => x.ID_HOSPITA == SelectedHospitalizacion) : null : null;
                string Enfermedades = string.Empty;
                string Dietas = string.Empty;

                var _Enfermedades = _UnicaH != null ? _UnicaH.NOTA_MEDICA != null ? _UnicaH.NOTA_MEDICA.NOTA_MEDICA_ENFERMEDAD : null : null;
                if (_Enfermedades != null && _Enfermedades.Any())
                    foreach (var item in _Enfermedades)
                        Enfermedades += string.Format("{0}, ", item.ENFERMEDAD != null ? !string.IsNullOrEmpty(item.ENFERMEDAD.NOMBRE) ? item.ENFERMEDAD.NOMBRE.Trim() : string.Empty : string.Empty);

                var _Dietas = _UnicaH != null ? _UnicaH.NOTA_MEDICA != null ? _UnicaH.NOTA_MEDICA.NOTA_MEDICA_DIETA : null : null;
                if (_Dietas != null && _Dietas.Any())
                    foreach (var item in _Dietas)
                        Dietas += string.Format("{0}, ", item.DIETA != null ? !string.IsNullOrEmpty(item.DIETA.DESCR) ? item.DIETA.DESCR.Trim() : string.Empty : string.Empty);

                var datos = new System.Collections.Generic.List<cHojaControlLiquidosReporte>();
                var _detallesEnfermo = new cHojaControlLiquidosEncabezadoReporte()
                {
                    Nombre = string.Format("{0} {1} {2}",
                    SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty,
                    SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty,
                    SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty),
                    Sexo = SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.SEXO) ? SelectIngreso.IMPUTADO.SEXO == "M" ? "MASCULINO" : SelectIngreso.IMPUTADO.SEXO == "F" ? "FEMENINO" : string.Empty : string.Empty : string.Empty,
                    Edad = SelectIngreso.IMPUTADO != null ? new Fechas().CalculaEdad(SelectIngreso.IMPUTADO.NACIMIENTO_FECHA).ToString() : string.Empty,
                    Fecha = FechaInicioBusqueda != null ? FechaInicioBusqueda.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Cama = _UnicaH != null ? _UnicaH.CAMA_HOSPITAL != null ? !string.IsNullOrEmpty(_UnicaH.CAMA_HOSPITAL.DESCR) ? _UnicaH.CAMA_HOSPITAL.DESCR.Trim() : string.Empty : string.Empty : string.Empty,
                    Peso = _UnicaH != null ? _UnicaH.NOTA_MEDICA != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES != null ? !string.IsNullOrEmpty(_UnicaH.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.PESO) ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.PESO.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                    Talla = _UnicaH != null ? _UnicaH.NOTA_MEDICA != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES != null ? !string.IsNullOrEmpty(_UnicaH.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.TALLA) ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.TALLA.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                    Diagnostico = !string.IsNullOrEmpty(Enfermedades) ? Enfermedades.TrimEnd(',') : string.Empty,
                    Dieta = !string.IsNullOrEmpty(Dietas) ? Dietas.TrimEnd(',') : string.Empty,
                    Exped = string.Format("{0} / {1}", SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO),
                    Glucemia = _UnicaH != null ? _UnicaH.NOTA_MEDICA != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES != null ? !string.IsNullOrEmpty(_UnicaH.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.GLUCEMIA) ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.GLUCEMIA.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty
                };

                var _LiquidosIngreso = new SSP.Controlador.Catalogo.Justicia.cLiquido().GetData(x => x.ID_LIQTIPO == "1" && x.ID_LIQ_FORMATO == 1);
                var _LiquidosEreso = new SSP.Controlador.Catalogo.Justicia.cLiquido().GetData(x => x.ID_LIQTIPO == "2" && x.ID_LIQ_FORMATO == 1);
                string _detalleLiquidosIngreso = "INGRESOS \n";
                string _detalleLiquidosEgreso = "EGRESOS \n";

                if (_LiquidosIngreso.Any())
                    foreach (var item in _LiquidosIngreso)
                        _detalleLiquidosIngreso += string.Format("   {0}\n", !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty);

                if (_LiquidosEreso.Any())
                    foreach (var item in _LiquidosEreso)
                        _detalleLiquidosEgreso += string.Format("   {0}\n", !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty);

                var _HorasDisponibles = new SSP.Controlador.Catalogo.Justicia.cLiquidoHora().GetData(x => x.ESTATUS == "S", y => y.ID_LIQHORA.ToString());
                var _DetallesHojaLiquidos = new SSP.Controlador.Catalogo.Justicia.cLiquidoHojaCtrl().ObtenerHojaLiquidos(SelectedHospitalizacion, FechaInicioBusqueda);
                if (_HorasDisponibles.Any())
                {
                    foreach (var item in _HorasDisponibles)
                    {
                        string CondensadoIngreso = "\n";
                        string CondensadoEgreso = "\n";
                        SSP.Servidor.LIQUIDO_HOJA_CTRL _HojaCapturada = _DetallesHojaLiquidos.Any() ? _DetallesHojaLiquidos.FirstOrDefault(x => x.ID_LIQHORA == item.ID_LIQHORA) : null;
                        if (_LiquidosIngreso.Any())
                        {
                            foreach (var itemX in _LiquidosIngreso)
                            {
                                if (_HojaCapturada != null)
                                    if (_HojaCapturada.LIQUIDO_HOJA_CTRL_DETALLE.Any())
                                    {
                                        var _elegida = _HojaCapturada.LIQUIDO_HOJA_CTRL_DETALLE.FirstOrDefault(x => x.ID_LIQ == itemX.ID_LIQ);
                                        if (_elegida != null)
                                            CondensadoIngreso += string.Format("{0} \n", _elegida.CANT);
                                        else
                                            CondensadoIngreso += "*\n";
                                    }
                                    else
                                        CondensadoIngreso += "*\n";
                                else
                                    CondensadoIngreso += "*\n";
                            };
                        };

                        if (_LiquidosEreso.Any())
                        {
                            foreach (var itemX in _LiquidosEreso)
                            {
                                if (_HojaCapturada != null)
                                    if (_HojaCapturada.LIQUIDO_HOJA_CTRL_DETALLE.Any())
                                    {
                                        var _elegida = _HojaCapturada.LIQUIDO_HOJA_CTRL_DETALLE.FirstOrDefault(x => x.ID_LIQ == itemX.ID_LIQ);
                                        if (_elegida != null)
                                            CondensadoEgreso += string.Format("{0} \n", _elegida.CANT);
                                        else
                                            CondensadoEgreso += "*\n";
                                    }
                                    else
                                        CondensadoEgreso += "*\n";
                                else
                                    CondensadoEgreso += "*\n";
                            };
                        };

                        _detalleLiquidosIngreso = !string.IsNullOrEmpty(_detalleLiquidosIngreso) ? _detalleLiquidosIngreso.TrimEnd('\n') : string.Empty;
                        _detalleLiquidosEgreso = !string.IsNullOrEmpty(_detalleLiquidosEgreso) ? _detalleLiquidosEgreso.TrimEnd('\n') : string.Empty;
                        CondensadoIngreso = !string.IsNullOrEmpty(CondensadoIngreso) ? CondensadoIngreso.TrimEnd('\n') : string.Empty;
                        CondensadoEgreso = !string.IsNullOrEmpty(CondensadoEgreso) ? CondensadoEgreso.TrimEnd('\n') : string.Empty;

                        datos.Add(new cHojaControlLiquidosReporte
                            {
                                Hora = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty,
                                Ingresos = _detalleLiquidosIngreso,
                                Egresos = _detalleLiquidosEgreso,
                                FrecCard = "FREC. CARD.",
                                FrecResp = "FREC. RESP.",
                                Temp = "TEMP.",
                                TensionArt = "TENS. ART.",
                                Generico = _HojaCapturada != null ? !string.IsNullOrEmpty(_HojaCapturada.TENSION_ARTERIAL) ? _HojaCapturada.TENSION_ARTERIAL.Trim() : string.Empty : string.Empty,
                                Generico1 = _HojaCapturada != null ? !string.IsNullOrEmpty(_HojaCapturada.FRECUENCIA_CARDIACA) ? _HojaCapturada.FRECUENCIA_CARDIACA.Trim() : string.Empty : string.Empty,
                                Generico2 = _HojaCapturada != null ? !string.IsNullOrEmpty(_HojaCapturada.FRECUENCIA_RESPIRATORIA) ? _HojaCapturada.FRECUENCIA_RESPIRATORIA.Trim() : string.Empty : string.Empty,
                                Generico3 = _HojaCapturada != null ? !string.IsNullOrEmpty(_HojaCapturada.TEMPERATURA) ? _HojaCapturada.TEMPERATURA.Trim() : string.Empty : string.Empty,
                                Generico4 = CondensadoIngreso,
                                Generico5 = CondensadoEgreso
                            });
                    };
                };

                cControlLiquidosConcentrados Concentrados = new cControlLiquidosConcentrados();
                var _Concentrados = new SSP.Controlador.Catalogo.Justicia.cLiquidoHojaCtrlConcen().GetData(x => x.ID_HOSPITA == SelectedHospitalizacion && x.CONCENTRADO_FEC.Value.Year == FechaInicioBusqueda.Value.Year && x.CONCENTRADO_FEC.Value.Month == FechaInicioBusqueda.Value.Month && x.CONCENTRADO_FEC.Value.Day == FechaInicioBusqueda.Value.Day && x.ID_CENTRO_UBI == GlobalVar.gCentro);
                if (_Concentrados.Any())
                {
                    SSP.Servidor.LIQUIDO_HOJA_CTRL_CONCEN _ConcentradoMatutino = _Concentrados.FirstOrDefault(x => x.ID_CONCENTIPO == (decimal)eTipoConcentrados.MATUTINO);
                    SSP.Servidor.LIQUIDO_HOJA_CTRL_CONCEN _ConcentradoVespertino = _Concentrados.FirstOrDefault(x => x.ID_CONCENTIPO == (decimal)eTipoConcentrados.VESPERTINO);
                    SSP.Servidor.LIQUIDO_HOJA_CTRL_CONCEN _ConcentradoNocturno = _Concentrados.FirstOrDefault(x => x.ID_CONCENTIPO == (decimal)eTipoConcentrados.NOCTURNO);
                    SSP.Servidor.LIQUIDO_HOJA_CTRL_CONCEN _ConcentradoTotal = _Concentrados.FirstOrDefault(x => x.ID_CONCENTIPO == (decimal)eTipoConcentrados.TOTAL);
                    SSP.Servidor.USUARIO _Usuario = null;
                    if (_ConcentradoMatutino != null)
                    {
                        _Usuario = null;//SE ASEGURA DE LIMPIAR EL USUARIO PARA QUE NO SE TOME EL MISMO O SE SOBREESCRIBA
                        if (!string.IsNullOrEmpty(_ConcentradoMatutino.REGISTRO_USUARIO))
                            _Usuario = new SSP.Controlador.Catalogo.Justicia.cUsuario().GetData(x => x.ID_USUARIO.Trim() == _ConcentradoMatutino.REGISTRO_USUARIO.Trim()).FirstOrDefault();

                        Concentrados.BalanceTM = _ConcentradoMatutino.BALANCE;
                        Concentrados.EnfermeroTM = _Usuario != null ? string.Format("{0} {1} {2}",
                            _Usuario.EMPLEADO != null ? _Usuario.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(_Usuario.EMPLEADO.PERSONA.NOMBRE) ? _Usuario.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                            _Usuario.EMPLEADO != null ? _Usuario.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(_Usuario.EMPLEADO.PERSONA.PATERNO) ? _Usuario.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                            _Usuario.EMPLEADO != null ? _Usuario.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(_Usuario.EMPLEADO.PERSONA.MATERNO) ? _Usuario.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty : string.Empty : string.Empty) : string.Empty;
                        Concentrados.EntradaTM = _ConcentradoMatutino.ENTRADA;
                        Concentrados.SalidasTM = _ConcentradoMatutino.SALIDA;
                    };


                    if (_ConcentradoVespertino != null)
                    {
                        _Usuario = null;
                        if (!string.IsNullOrEmpty(_ConcentradoVespertino.REGISTRO_USUARIO))
                            _Usuario = new SSP.Controlador.Catalogo.Justicia.cUsuario().GetData(x => x.ID_USUARIO.Trim() == _ConcentradoVespertino.REGISTRO_USUARIO.Trim()).FirstOrDefault();

                        Concentrados.BalanceTV = _ConcentradoVespertino.BALANCE;
                        Concentrados.EnfermeroTV = _Usuario != null ? string.Format("{0} {1} {2}",
                            _Usuario.EMPLEADO != null ? _Usuario.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(_Usuario.EMPLEADO.PERSONA.NOMBRE) ? _Usuario.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                            _Usuario.EMPLEADO != null ? _Usuario.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(_Usuario.EMPLEADO.PERSONA.PATERNO) ? _Usuario.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                            _Usuario.EMPLEADO != null ? _Usuario.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(_Usuario.EMPLEADO.PERSONA.MATERNO) ? _Usuario.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty : string.Empty : string.Empty) : string.Empty;
                        Concentrados.EntradaTV = _ConcentradoVespertino.ENTRADA;
                        Concentrados.SalidasTV = _ConcentradoVespertino.SALIDA;
                    };


                    if (_ConcentradoNocturno != null)
                    {
                        _Usuario = null;
                        if (!string.IsNullOrEmpty(_ConcentradoNocturno.REGISTRO_USUARIO))
                            _Usuario = new SSP.Controlador.Catalogo.Justicia.cUsuario().GetData(x => x.ID_USUARIO.Trim() == _ConcentradoNocturno.REGISTRO_USUARIO.Trim()).FirstOrDefault();

                        Concentrados.BalanceTN = _ConcentradoNocturno.BALANCE;
                        Concentrados.EnfermeroTN = _Usuario != null ? string.Format("{0} {1} {2}",
                            _Usuario.EMPLEADO != null ? _Usuario.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(_Usuario.EMPLEADO.PERSONA.NOMBRE) ? _Usuario.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                            _Usuario.EMPLEADO != null ? _Usuario.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(_Usuario.EMPLEADO.PERSONA.PATERNO) ? _Usuario.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                            _Usuario.EMPLEADO != null ? _Usuario.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(_Usuario.EMPLEADO.PERSONA.MATERNO) ? _Usuario.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty : string.Empty : string.Empty) : string.Empty;
                        Concentrados.EntradaTN = _ConcentradoNocturno.ENTRADA;
                        Concentrados.SalidasTN = _ConcentradoNocturno.SALIDA;
                    };

                    if (_ConcentradoTotal != null)
                    {
                        _Usuario = null;
                        if (!string.IsNullOrEmpty(_ConcentradoTotal.REGISTRO_USUARIO))
                            _Usuario = new SSP.Controlador.Catalogo.Justicia.cUsuario().GetData(x => x.ID_USUARIO.Trim() == _ConcentradoTotal.REGISTRO_USUARIO.Trim()).FirstOrDefault();

                        Concentrados.BalanceTTL = _ConcentradoTotal.BALANCE;
                        Concentrados.EnfermeroTTL = _Usuario != null ? string.Format("{0} {1} {2}",
                            _Usuario.EMPLEADO != null ? _Usuario.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(_Usuario.EMPLEADO.PERSONA.NOMBRE) ? _Usuario.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                            _Usuario.EMPLEADO != null ? _Usuario.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(_Usuario.EMPLEADO.PERSONA.PATERNO) ? _Usuario.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                            _Usuario.EMPLEADO != null ? _Usuario.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(_Usuario.EMPLEADO.PERSONA.MATERNO) ? _Usuario.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty : string.Empty : string.Empty) : string.Empty;
                        Concentrados.EntradasTTL = _ConcentradoTotal.ENTRADA;
                        Concentrados.SalidasTTL = _ConcentradoTotal.SALIDA;
                    };
                }

                var Encabezado = new cEncabezado();
                Encabezado = new cEncabezado()
                {
                    ImagenDerecha = Parametro.LOGO_ESTADO,
                    ImagenIzquierda = Parametro.REPORTE_LOGO2,
                    TituloDos = Parametro.ENCABEZADO2
                };

                View.Report.LocalReport.ReportPath = "Reportes/rReporteHojaControlLiquidos.rdlc";
                View.Report.LocalReport.DataSources.Clear();

                System.Collections.Generic.List<cEncabezado> ds1 = new System.Collections.Generic.List<cEncabezado>();
                ds1.Add(Encabezado);
                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource() { Name = "DataSet1", Value = ds1 };
                View.Report.LocalReport.DataSources.Add(rds1);

                var ds2 = new System.Collections.Generic.List<cHojaControlLiquidosEncabezadoReporte>();
                ds2.Add(_detallesEnfermo);
                Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource() { Name = "DataSet2", Value = ds2 };
                View.Report.LocalReport.DataSources.Add(rds2);

                Microsoft.Reporting.WinForms.ReportDataSource rds3 = new Microsoft.Reporting.WinForms.ReportDataSource() { Name = "DataSet3", Value = datos };
                View.Report.LocalReport.DataSources.Add(rds3);

                var ds4 = new System.Collections.Generic.List<cControlLiquidosConcentrados>();
                ds4.Add(Concentrados);
                Microsoft.Reporting.WinForms.ReportDataSource rds4 = new Microsoft.Reporting.WinForms.ReportDataSource() { Name = "DataSet4", Value = ds4 };
                View.Report.LocalReport.DataSources.Add(rds4);

                View.Report.RefreshReport();
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private void InicializaEntornoConcentradoLiquidos()
        {
            try
            {
                FechaGeneraConcentrado = Fechas.GetFechaDateServer;
                ListTurnosLiquidos = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO_HOJA_CTRL_CONCEN_TIPO>(new SSP.Controlador.Catalogo.Justicia.cLiquidoHojaCtrlConcenTipo().GetData(x => x.ESTATUS == "S", y => y.DESCR));
                ListTurnosLiquidos.Insert(0, new SSP.Servidor.LIQUIDO_HOJA_CTRL_CONCEN_TIPO { DESCR = "SELECCIONE", ID_CONCENTIPO = -1 });
                RaisePropertyChanged("ListTurnosLiquidos");

                SelectedTurnoLiquidos = -1;
                TotalEntradas = TotalBalance = TotalSalidas = new decimal?();
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private void ConsultaCocentrados()
        {
            try
            {
                if (SelectIngreso == null)
                    return;

                if (SelectedTurnoLiquidos.HasValue)
                    if (SelectedTurnoLiquidos != -1)
                        if (FechaGeneraConcentrado.HasValue)
                        {
                            TotalBalance = TotalEntradas = TotalSalidas = null;
                            var _ValidaConcentradoTotal = new SSP.Controlador.Catalogo.Justicia.cLiquidoHojaCtrlConcen().GetData(x => x.ID_HOSPITA == SelectedHospitalizacion && x.CONCENTRADO_FEC.Value.Year == FechaGeneraConcentrado.Value.Year && x.CONCENTRADO_FEC.Value.Month == FechaGeneraConcentrado.Value.Month && x.CONCENTRADO_FEC.Value.Day == FechaGeneraConcentrado.Value.Day && x.ID_CONCENTIPO == (decimal)eTipoConcentrados.TOTAL).FirstOrDefault();
                            if (_ValidaConcentradoTotal != null)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "Ya fue capturado el concentrado total de esta fecha.");
                                return;
                            }

                            var _ValidaExistenciaConcentradoPadre = new SSP.Controlador.Catalogo.Justicia.cLiquidoHojaCtrlConcen().GetData(x => x.ID_HOSPITA == SelectedHospitalizacion && x.CONCENTRADO_FEC.Value.Year == FechaGeneraConcentrado.Value.Year && x.CONCENTRADO_FEC.Value.Month == FechaGeneraConcentrado.Value.Month && x.CONCENTRADO_FEC.Value.Day == FechaGeneraConcentrado.Value.Day && x.ID_CONCENTIPO == SelectedTurnoLiquidos).FirstOrDefault();
                            if (_ValidaExistenciaConcentradoPadre != null)
                            {
                                TotalBalance = TotalEntradas = TotalSalidas = null;
                                new Dialogos().ConfirmacionDialogo("Validación", "Ya fue capturado un concentrado con esta fecha y turno.");
                                return;
                            }

                            var _detallesConcentrado = new SSP.Controlador.Catalogo.Justicia.cLiquidoHojaCtrlConcenDetalle().ObtenerDetallesConcentrado(SelectedHospitalizacion, GlobalVar.gCentro, FechaGeneraConcentrado, SelectedTurnoLiquidos);
                            if (_detallesConcentrado.Any())
                            {
                                TotalEntradas = _detallesConcentrado.Where(x => x.LIQUIDO != null && x.LIQUIDO.ID_LIQTIPO == "1").Sum(x => x.CANT);
                                TotalSalidas = _detallesConcentrado.Where(x => x.LIQUIDO != null && x.LIQUIDO.ID_LIQTIPO == "2").Sum(x => x.CANT);
                                TotalBalance = TotalEntradas - TotalSalidas;
                            }
                            else
                                if (SelectedTurnoLiquidos == (decimal)eTipoConcentrados.TOTAL)
                                {//ESTE ES EL CONDENSADO DEL TOTAL
                                    var _detallesCondensado = new SSP.Controlador.Catalogo.Justicia.cLiquidoHojaCtrlConcenDetalle().GetData(x => x.ID_HOSPITA == SelectedHospitalizacion && x.ID_CENTRO_UBI == GlobalVar.gCentro && x.LIQUIDO_HOJA_CTRL.FECHA.Value.Year == FechaGeneraConcentrado.Value.Year && x.LIQUIDO_HOJA_CTRL.FECHA.Value.Month == FechaGeneraConcentrado.Value.Month && x.LIQUIDO_HOJA_CTRL.FECHA.Value.Day == FechaGeneraConcentrado.Value.Day);
                                    if (_detallesCondensado.Any())
                                    {
                                        TotalEntradas = _detallesCondensado.Where(x => x.LIQUIDO != null && x.LIQUIDO.ID_LIQTIPO == "1").Sum(x => x.CANT);
                                        TotalSalidas = _detallesCondensado.Where(x => x.LIQUIDO != null && x.LIQUIDO.ID_LIQTIPO == "2").Sum(x => x.CANT);
                                        TotalBalance = TotalEntradas - TotalSalidas;
                                    };
                                };
                        };
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private bool GuardarConcentrado()
        {
            try
            {
                var _Concentrado = new SSP.Servidor.LIQUIDO_HOJA_CTRL_CONCEN()
                {
                    BALANCE = TotalBalance,
                    CONCENTRADO_FEC = FechaGeneraConcentrado,
                    ENTRADA = TotalEntradas,
                    ID_CENTRO_UBI = GlobalVar.gCentro,
                    REGISTRO_FEC = Fechas.GetFechaDateServer,
                    ID_HOSPITA = SelectedHospitalizacion,
                    ID_CONCENTIPO = SelectedTurnoLiquidos,
                    REGISTRO_USUARIO = GlobalVar.gUsr,
                    SALIDA = TotalSalidas
                };

                return new SSP.Controlador.Catalogo.Justicia.cLiquidoHojaCtrlConcen().GuardarConcentrado(_Concentrado);
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }
        private bool GuardaControlLiquidos()
        {
            try
            {
                var DetallesHora = new SSP.Controlador.Catalogo.Justicia.cLiquidoHora().GetData().FirstOrDefault(x => x.ID_LIQHORA == SelectedHoraLiquidos);
                int _Hora = int.Parse(DetallesHora.DESCR);
                var fechaSele = new System.DateTime(
                    FecSeleccionadaregistro.Value.Year,
                    FecSeleccionadaregistro.Value.Month,
                    FecSeleccionadaregistro.Value.Day,
                    FecSeleccionadaregistro.Value.Hour,
                    FecSeleccionadaregistro.Value.Minute,
                    FecSeleccionadaregistro.Value.Second);

                var _HojaPrincipal = new SSP.Servidor.LIQUIDO_HOJA_CTRL()
                {
                    FECHA = fechaSele,
                    FECHA_REGISTRO = Fechas.GetFechaDateServer,
                    TENSION_ARTERIAL = string.Format("{0}/{1}", Arterial1, Arterial2),
                    FRECUENCIA_RESPIRATORIA = FrecuenciaRespiratoria,
                    ID_CENTRO_UBI = GlobalVar.gCentro,
                    ID_USUARIO__REGISTRO = GlobalVar.gUsr,
                    TEMPERATURA = Temperatura,
                    FRECUENCIA_CARDIACA = FrecuenciaCardiaca,
                    ID_LIQHORA = SelectedHoraLiquidos,
                    GLUCEMIA = Glucemia,
                    ID_HOSPITA = SelectedHospitalizacion
                };

                if (ListLiquidosIngresoEditar != null && ListLiquidosIngresoEditar.Any())
                    foreach (var item in ListLiquidosIngresoEditar)
                        _HojaPrincipal.LIQUIDO_HOJA_CTRL_DETALLE.Add(new SSP.Servidor.LIQUIDO_HOJA_CTRL_DETALLE
                            {
                                CANT = item.CANT,
                                ID_HOSPITA = SelectedHospitalizacion,
                                ID_CENTRO_UBI = GlobalVar.gCentro,
                                ID_LIQ = item.ID_LIQ
                            });

                return new SSP.Controlador.Catalogo.Justicia.cLiquidoHojaCtrl().IngresarControlLiquidos(_HojaPrincipal);
            }
            catch (System.Exception exc)
            {
                return false;
            }
        }

        private void AgregaLiquidoIngreso()
        {
            try
            {
                if (ListLiquidosIngresoEditar == null)
                    ListLiquidosIngresoEditar = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO_HOJA_CTRL_DETALLE>();

                if (SelectedLiqIngreso == null)
                    return;

                if (ListLiquidosIngresoEditar.Any(x => x.ID_LIQ == SelectedLiqIngreso))
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "El liquido seleccionado ya se encuentra en la lista.");
                    return;
                };

                var _DetalleLiquido = new SSP.Controlador.Catalogo.Justicia.cLiquido().GetData(x => x.ID_LIQ == SelectedLiqIngreso).FirstOrDefault();
                ListLiquidosIngresoEditar.Add(new SSP.Servidor.LIQUIDO_HOJA_CTRL_DETALLE
                    {
                        CANT = TxtCantidad,
                        ID_CENTRO_UBI = GlobalVar.gCentro,
                        ID_LIQ = SelectedLiqIngreso.Value,
                        LIQUIDO = _DetalleLiquido
                    });

                LimpiaValidacionesLiquidosIngreso();
                LimpiaCamposIngresaLiquido();
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private void LimpiaCamposIngresaLiquido()
        {
            try
            {
                ListLiquidosIngresos = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO>();
                ListLiquidosIngresos.Insert(0, new SSP.Servidor.LIQUIDO { ID_LIQ = -1, DESCR = "SELECCIONE" });
                TxtCantidad = null;
                SelectedTipoLiquido = -1;
                SelectedLiqIngreso = -1;

                RaisePropertyChanged("ListLiquidosIngresos");
                OnPropertyChanged("TxtCantidad");
                OnPropertyChanged("SelectedTipoLiquido");
                OnPropertyChanged("SelectedLiqIngreso");
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private void LimpiaDatosHoja()
        {
            try
            {
                Arterial1 = Arterial2 = FrecuenciaCardiaca = FrecuenciaRespiratoria = Temperatura = Glucemia = string.Empty;
                FecSeleccionadaregistro = Fechas.GetFechaDateServer;
                SelectedHoraLiquidos = -1;
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private async void ModelEnter(System.Object obj)
        {
            try
            {
                if (!MenuBuscarEnabled)
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                    return;
                };

                if (obj != null)
                    if (!obj.GetType().Name.Equals("String"))
                    {
                        var textbox = obj as System.Windows.Controls.TextBox;
                        if (textbox != null)
                            switch (textbox.Name)
                            {
                                case "NombreBuscar":
                                    NombreBuscar = textbox.Text;
                                    NombreD = NombreBuscar;
                                    FolioBuscar = FolioD;
                                    AnioBuscar = AnioD;
                                    break;
                                case "ApellidoPaternoBuscar":
                                    ApellidoPaternoBuscar = textbox.Text;
                                    PaternoD = ApellidoPaternoBuscar;
                                    FolioBuscar = FolioD;
                                    AnioBuscar = AnioD;
                                    break;
                                case "ApellidoMaternoBuscar":
                                    ApellidoMaternoBuscar = textbox.Text;
                                    MaternoD = ApellidoMaternoBuscar;
                                    FolioBuscar = FolioD;
                                    AnioBuscar = AnioD;
                                    break;
                                case "FolioBuscar":
                                    if (!string.IsNullOrEmpty(textbox.Text))
                                        FolioBuscar = int.Parse(textbox.Text);
                                    else
                                        FolioBuscar = null;
                                    AnioBuscar = AnioD;
                                    break;
                                case "AnioBuscar":
                                    if (!string.IsNullOrEmpty(textbox.Text))
                                        AnioBuscar = int.Parse(textbox.Text);
                                    else
                                        AnioBuscar = null;
                                    FolioBuscar = FolioD;
                                    break;
                            }
                    }

                ListExpediente = new RangeEnabledObservableCollection<SSP.Servidor.IMPUTADO>();

                if (string.IsNullOrEmpty(NombreD))
                    NombreBuscar = string.Empty;
                else
                    NombreBuscar = NombreD;

                if (string.IsNullOrEmpty(PaternoD))
                    ApellidoPaternoBuscar = string.Empty;
                else
                    ApellidoPaternoBuscar = PaternoD;

                if (string.IsNullOrEmpty(MaternoD))
                    ApellidoMaternoBuscar = string.Empty;
                else
                    ApellidoMaternoBuscar = MaternoD;

                if (AnioBuscar != null && FolioBuscar != null)
                {
                    ListExpediente = new RangeEnabledObservableCollection<SSP.Servidor.IMPUTADO>();
                    ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                    if (ListExpediente.Count == 1)
                    {
                        if (ListExpediente[0].INGRESO != null && !ListExpediente[0].INGRESO.Any())
                        {
                            SelectExpediente = null;
                            SelectIngreso = null;
                            ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                            new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningún ingreso activo en este imputado.");
                            return;
                        };

                        foreach (var item in Parametro.ESTATUS_ADMINISTRATIVO_INACT)
                        {
                            if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_ESTATUS_ADMINISTRATIVO == item)
                            {
                                SelectExpediente = null;
                                SelectIngreso = null;
                                ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                                new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningún ingreso activo en este imputado.");
                                return;
                            };
                        };

                        if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_UB_CENTRO != GlobalVar.gCentro)
                        {
                            SelectExpediente = null;
                            SelectIngreso = null;
                            ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                            new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                            return;
                        };

                        if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().TRASLADO_DETALLE.Any(a => a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false))
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + ListExpediente[0].ID_ANIO.ToString() + "/" +
                                ListExpediente[0].ID_IMPUTADO.ToString() + "] tiene un traslado próximo y no tiene permitido ningún cambio de información.");
                            return;
                        };


                        SelectExpediente = ListExpediente[0];
                        SelectIngreso = ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                        SelectedInterno = SelectExpediente;
                        var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                        if (SelectIngreso.TRASLADO_DETALLE.Any(a => (a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado) <= Fechas.GetFechaDateServer))
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + SelectIngreso.ID_ANIO.ToString() + "/" +
                                SelectIngreso.ID_IMPUTADO.ToString() + "] tiene un traslado próximo y no tiene permitido ningún cambio de información.");
                            return;
                        }

                        var _validacionHospitalizado = new SSP.Controlador.Catalogo.Justicia.Medico.cHospitalizacion().ObtenerHospitalizacionesPorIngreso(SelectIngreso);
                        if (!_validacionHospitalizado.Any())
                        {
                            SelectExpediente = null;
                            SelectIngreso = null;
                            ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                            new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no esta hospitalizado.");
                            return;
                        }

                        SeleccionaIngreso();
                        StaticSourcesViewModel.SourceChanged = false;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        return;
                    }
                    else
                    {
                        SelectExpediente = null;
                        SelectIngreso = null;
                        ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    }
                }
                else
                {
                    ListExpediente = new RangeEnabledObservableCollection<SSP.Servidor.IMPUTADO>();
                    ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                    if (ListExpediente.Count > 0)//Empty row
                        EmptyExpedienteVisible = false;
                    else
                        EmptyExpedienteVisible = true;

                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                }
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ingresar búsqueda", ex);
            }
        }

        private async void ClickEnter(System.Object obj)
        {
            try
            {
                if (obj != null)
                {
                    var textbox = obj as System.Windows.Controls.TextBox;
                    if (textbox != null)
                        switch (textbox.Name)
                        {
                            case "NombreBuscar":
                                NombreBuscar = textbox.Text;
                                break;
                            case "ApellidoPaternoBuscar":
                                ApellidoPaternoBuscar = textbox.Text;
                                break;
                            case "ApellidoMaternoBuscar":
                                ApellidoMaternoBuscar = textbox.Text;
                                break;
                            case "AnioBuscar":
                                if (!string.IsNullOrEmpty(textbox.Text))
                                    AnioBuscar = int.Parse(textbox.Text);
                                else
                                    AnioBuscar = null;
                                break;
                            case "FolioBuscar":
                                if (!string.IsNullOrEmpty(textbox.Text))
                                    FolioBuscar = int.Parse(textbox.Text);
                                else
                                    FolioBuscar = null;
                                break;
                        }
                }

                ListExpediente = new RangeEnabledObservableCollection<SSP.Servidor.IMPUTADO>();
                ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                if (ListExpediente.Count > 0)//Empty row
                    EmptyExpedienteVisible = false;
                else
                    EmptyExpedienteVisible = true;

                ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ingresar búsqueda", ex);
            }

            return;
        }

        private async System.Threading.Tasks.Task<System.Collections.Generic.List<SSP.Servidor.IMPUTADO>> SegmentarResultadoBusqueda(int _Pag = 1)
        {
            try
            {
                if (string.IsNullOrEmpty(ApellidoPaternoBuscar) && string.IsNullOrEmpty(ApellidoMaternoBuscar) && string.IsNullOrEmpty(NombreBuscar) && !AnioBuscar.HasValue && !FolioBuscar.HasValue)
                    return new System.Collections.Generic.List<SSP.Servidor.IMPUTADO>();

                Pagina = _Pag;
                var result = await StaticSourcesViewModel.CargarDatosAsync<System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.IMPUTADO>>(() => new SSP.Controlador.Catalogo.Justicia.cImputado().ObtenerTodos(ApellidoPaternoBuscar, ApellidoMaternoBuscar, NombreBuscar, AnioBuscar, FolioBuscar, _Pag));
                if (result.Any())
                {
                    Pagina++;
                    SeguirCargando = true;
                }
                else
                    SeguirCargando = false;

                return result.ToList();
            }

            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al segmentar resultados de búsqueda", ex);
                return new System.Collections.Generic.List<SSP.Servidor.IMPUTADO>();
            }
        }


        private void SeleccionaIngreso()
        {
            try
            {
                if (SelectIngreso != null)
                {
                    AnioD = SelectIngreso.ID_ANIO;
                    FolioD = SelectIngreso.ID_IMPUTADO;
                    PaternoD = SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty;
                    MaternoD = SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty;
                    NombreD = SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty;
                    SexoImp = SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.SEXO) ? SelectIngreso.IMPUTADO.SEXO == "M" ? "MASCULINO" : SelectIngreso.IMPUTADO.SEXO == "F" ? "FEMENINO" : string.Empty : string.Empty : string.Empty;
                    EdadImp = SelectIngreso.IMPUTADO != null ? new Fechas().CalculaEdad(SelectIngreso.IMPUTADO.NACIMIENTO_FECHA).ToString() : string.Empty;
                    var _validacionHospitalizado = new SSP.Controlador.Catalogo.Justicia.Medico.cHospitalizacion().ObtenerHospitalizacionesPorIngreso(SelectIngreso);
                    string DatosDiagnostico = string.Empty;
                    string DatosDietas = string.Empty;
                    if (_validacionHospitalizado != null && _validacionHospitalizado.Any())
                    {
                        var _UnicaHospitalizacion = _validacionHospitalizado.FirstOrDefault();
                        SelectedHospitalizacion = _UnicaHospitalizacion.ID_HOSPITA;
                        var _Enfermedades = _UnicaHospitalizacion.NOTA_MEDICA != null ? _UnicaHospitalizacion.NOTA_MEDICA.NOTA_MEDICA_ENFERMEDAD : null;
                        if (_Enfermedades != null && _Enfermedades.Any())
                            foreach (var item in _Enfermedades)
                                DatosDiagnostico += string.Format("{0}, ", item.ENFERMEDAD != null ? !string.IsNullOrEmpty(item.ENFERMEDAD.NOMBRE) ? item.ENFERMEDAD.NOMBRE.Trim() : string.Empty : string.Empty);

                        var _Dietas = _UnicaHospitalizacion.NOTA_MEDICA != null ? _UnicaHospitalizacion.NOTA_MEDICA.NOTA_MEDICA_DIETA : null;
                        if (_Dietas != null && _Dietas.Any())
                            foreach (var item in _Dietas)
                                DatosDietas += string.Format("{0}, ", item.DIETA != null ? !string.IsNullOrEmpty(item.DIETA.DESCR) ? item.DIETA.DESCR.Trim() : string.Empty : string.Empty);

                        DiagnosticoImp = !string.IsNullOrEmpty(DatosDiagnostico) ? DatosDiagnostico.TrimEnd(',') : string.Empty;
                        DietaImp = !string.IsNullOrEmpty(DatosDietas) ? DatosDietas.TrimStart(',') : string.Empty;
                        CamaImp = _UnicaHospitalizacion.CAMA_HOSPITAL != null ? !string.IsNullOrEmpty(_UnicaHospitalizacion.CAMA_HOSPITAL.DESCR) ? _UnicaHospitalizacion.CAMA_HOSPITAL.DESCR.Trim() : string.Empty : string.Empty;
                        PesoImp = _UnicaHospitalizacion.NOTA_MEDICA != null ? _UnicaHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA != null ? _UnicaHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES != null ? !string.IsNullOrEmpty(_UnicaHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.PESO) ? _UnicaHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.PESO.Trim() : string.Empty : string.Empty : string.Empty : string.Empty;
                        TallaImp = _UnicaHospitalizacion.NOTA_MEDICA != null ? _UnicaHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA != null ? _UnicaHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES != null ? !string.IsNullOrEmpty(_UnicaHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.TALLA) ? _UnicaHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.TALLA.Trim() : string.Empty : string.Empty : string.Empty : string.Empty;
                    };

                    if (EsEnfermero)
                        ValidacionesSignosVitales();
                };
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar ingreso", ex);
            }
        }


        private async void OnModelChangedSwitch(object parametro)
        {
            switch (parametro.ToString())
            {
                case "cambio_tipo_liquido":
                    if (SelectedTipoLiquido.HasValue)
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            CargaLiquidos(SelectedTipoLiquido.Value);
                        });
                    break;
                #region Cambio SelectedItem de Busqueda de Expediente
                case "cambio_expediente":
                    if (SelectExpediente != null && (SelectExpediente.INGRESO == null || SelectExpediente.INGRESO.Count == 0))
                    {
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            selectExpediente = new  SSP.Controlador.Catalogo.Justicia.cImputado().Obtener(selectExpediente.ID_IMPUTADO, selectExpediente.ID_ANIO, selectExpediente.ID_CENTRO).First();
                            RaisePropertyChanged("SelectExpediente");
                        });
                        //MUESTRA LOS INGRESOS
                        if (SelectExpediente.INGRESO != null && SelectExpediente.INGRESO.Count > 0)
                        {
                            EmptyIngresoVisible = false;
                            SelectIngreso = SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                        }
                        else
                            EmptyIngresoVisible = true;

                        //OBTENEMOS FOTO DE FRENTE
                        if (SelectIngreso != null)
                        {
                            if (SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                                ImagenImputado = SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                            else
                                ImagenImputado = new Imagenes().getImagenPerson();
                        }
                        else
                            ImagenImputado = new Imagenes().getImagenPerson();
                    }
                    break;
                #endregion
            }
        }

        private void CargaLiquidos(decimal _dato)
        {
            try
            {
                string _dat = _dato.ToString();
                System.Windows.Application.Current.Dispatcher.Invoke((System.Action)(delegate
                {
                    ListLiquidosIngresos = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO>(new SSP.Controlador.Catalogo.Justicia.cLiquido().GetData(x => x.ID_LIQTIPO == _dat && x.ID_LIQ_FORMATO == 1));
                    ListLiquidosIngresos.Insert(0, new SSP.Servidor.LIQUIDO { ID_LIQ = -1, DESCR = "SELECCIONE" });
                }));

                TxtCantidad = null;
                ValidacionesLiquidosIngreso();
                SelectedLiqIngreso = -1;
                RaisePropertyChanged("ListLiquidosIngresos");
                OnPropertyChanged("SelectedLiqIngreso");
                OnPropertyChanged("TxtCantidad");
            }
            catch (System.Exception exc)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la lista de liquidos", exc);
            }
        }
    }
}