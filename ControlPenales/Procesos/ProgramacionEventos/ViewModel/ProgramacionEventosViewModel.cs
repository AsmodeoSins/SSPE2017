using ControlPenales;

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using ControlPenales.Clases;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Windows.Interop;
using System.IO;
using System.Windows.Controls;

namespace ControlPenales
{
    partial class ProgramacionEventosViewModel : ValidationViewModelBase
    {
        #region constructor
        public ProgramacionEventosViewModel() {}
        #endregion

        #region metodos
        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "salir_menu":
                    PrincipalViewModel.SalirMenu();
                    break;
                case "buscar_menu":
                    LimpiarBuscar();
                    LstEventos = new RangeEnabledObservableCollection<EVENTO>();
                    BEventosEmpty = Visibility.Visible;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_EVENTO);
                    break;
                case "reporte_menu":
                    ImpresionEvento();
                    break;
                case "buscar_evento_pop":
                    LstEventos = new RangeEnabledObservableCollection<EVENTO>();
                    LstEventos.InsertRange(await SegmentarBusqueda(1));
                    BEventosEmpty = LstEventos.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                    break;
                case "seleccionar_evento":
                    if (SelectedEvento != null)
                    {
                        if (StaticSourcesViewModel.SourceChanged)
                        {
                            var respuesta = await new Dialogos().ConfirmarEliminar("Advertencia", "Hay cambios sin guardar,¿Seguro que desea continuar?");
                            if (respuesta == 1)
                            {
                                ObtenerEvento();
                                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_EVENTO);
                            }
                        }
                        else
                        {
                            ObtenerEvento();
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_EVENTO);
                        }
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validacion", "Favor de seleccionar un evento");
                    break;
                case "guardar_menu":
                    if (!base.HasErrors)
                    {
                        if (ValidarExistenProgramas())
                        {
                            if (ValidarExistenPresidium())
                            {
                                if (ValidarExistenInformacionTecnica())
                                {
                                    if (Guardar())
                                        new Dialogos().ConfirmacionDialogo("Éxito", "El evento se guardo correctamente.");
                                    else
                                        new Dialogos().ConfirmacionDialogo("Error", "Ocurrio un problema al guardar el evento.");
                                }
                                else
                                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de agregar informacion técnica.");
                            }
                            else
                                new Dialogos().ConfirmacionDialogo("Validación", "Favor de agregar presidium.");
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de agregar programa.");
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar los campos requeridos: ."+base.Error);
                    break;
                case "cancelar_evento":
                    SelectedEvento = null;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_EVENTO);
                    break;
                case "add_programa":
                    EProgramasTitulo = "Agregar Programa";
                    SelectedEventoPrograma = null;
                    ValidacionPrograma();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_EVENTO_PROGRAMA);
                    break;
                case "edit_programa":
                    EProgramasTitulo = "Editar Programa";
                    if (SelectedEventoPrograma != null)
                    {
                        ObtenerPrograma();
                        ValidacionPrograma();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_EVENTO_PROGRAMA);
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validacion", "Favor de seleccionar un programa");
                    break;
                case "del_programa":
                    if (SelectedEventoPrograma != null)
                    {
                        var respuesta = await new Dialogos().ConfirmarSalida("Validacion", "¿Confirma la eliminación de este programa?");
                        if (respuesta == 1)
                        {
                            QuitarPrograma();
                        }
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validacion", "Favor de seleccionar un programa");
                    break;
                case "agregar_programa":
                    if (!base.HasErrors)
                    {
                        AgregarPrograma();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_EVENTO_PROGRAMA);
                        ValidacionEvento();
                    }
                    break;
                case "cancelar_programa":
                    LimpiarPrograma();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_EVENTO_PROGRAMA);
                    ValidacionEvento();
                    break;
                case "programa_up":
                    UpPrograma();
                    break;
                case "programa_down":
                    DownPrograma();
                    break;
                case "add_presidium":
                    EPresidiumTitulo = "Agregar Presidium";
                    ValidacionPresidium();
                    SelectedEventoPresidium = null;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_EVENTO_PRESIDIUM);
                    break;
                case "edit_presidium":
                    EPresidiumTitulo = "Editar Presidium";
                    if (SelectedEventoPresidium != null)
                    {
                        ObtenerPresidium();
                        ValidacionPresidium();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_EVENTO_PRESIDIUM);
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validacion", "Favor de seleccionar un integrante del presidium");
                    break;
                case "del_presidium":
                    if (SelectedEventoPresidium != null)
                    {
                        var respuesta = await new Dialogos().ConfirmarSalida("Validacion", "¿Confirma la eliminación de este miembro del presidium?");
                        if (respuesta == 1)
                        {
                            QuitarPresidium();
                        }
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validacion", "Favor de seleccionar un integrante del presidium");
                    break;
                case "agregar_presidium":
                    if (!base.HasErrors)
                    {
                        AgregarPresidium();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_EVENTO_PRESIDIUM);
                        ValidacionEvento();
                    }
                    break;
                case "cancelar_presidium":
                    LimpiarPresidium();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_EVENTO_PRESIDIUM);
                    ValidacionEvento();
                    break;
                case "presidium_up":
                    UpPresidium();
                    break;
                case "presidium_down":
                    DownPresidium();
                    break;
                case "add_inf_tecnica":
                    EInfTecnicaTitulo = "Agregar Información Técnica";
                    ValidacionInfTecnica();
                    SelectedEventoInfTecnica = null;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_EVENTO_INF_TECNICA);
                    break;
                case "edit_inf_tecnica":
                    EInfTecnicaTitulo = "Editar Información Técnica";
                    if (SelectedEventoInfTecnica != null)
                    {
                        ObtenerInformacionTecnica();
                        ValidacionInfTecnica();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_EVENTO_INF_TECNICA);
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validacion", "Favor de seleccionar información técnica");
                    break;
                case "del_inf_tecnica":
                    if (SelectedEventoInfTecnica != null)
                    {
                        var respuesta = await new Dialogos().ConfirmarSalida("Validacion", "¿Confirma la eliminación de esta información técnica?");
                        if (respuesta == 1)
                        {
                            QuitarInformacionTecnica();
                        }
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validacion", "Favor de seleccionar un integrante del presidium");
                    break;
                case "agregar_inf_tecnica":
                    if (!base.HasErrors)
                    {
                        AgregarInformacionTecnica();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_EVENTO_INF_TECNICA);
                        ValidacionEvento();
                    }
                    break;
                case "cancelar_inf_tecnica":
                    LimpiarPresidium();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_EVENTO_INF_TECNICA);
                    ValidacionEvento();
                    break;
                case "limpiar_menu":
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new ProgramacionEventosView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ProgramacionEventosViewModel();
                    break;
                case "buscar_imputado":
                    LstIngresos = new RangeEnabledObservableCollection<cTrasladoIngreso>();
                    await ObtenerTodosActivos();
                    break;
                case "agregar_interno":
                    AgregarInterno();
                    break;
                case "quitar_interno":
                    QuitarInterno();
                    break;
            }
        }

        private async void OnLoad(ProgramacionEventosView obj = null)
        {
            try
            {
                estatus_inactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                await StaticSourcesViewModel.CargarDatosMetodoAsync(CargarListas);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar decomisos", ex);
            }

        }

        private async void ClickEnter(Object obj)
        {
            try
            {
                if (obj != null)
                {
                    var textbox = (TextBox)obj;
                    switch (textbox.Name)
                    {
                        case "NombreEvento":
                            BNombre = textbox.Text;
                            break;
                    }
                    ObtenerTodo();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ingresar busqueda", ex);
            }
        }

        private void CargarListas()
        {
            try
            {
                LstEventoTipo = new ObservableCollection<EVENTO_TIPO>(new cEventoTipo().ObtenerTodos());
                LstEntidades = new ObservableCollection<ENTIDAD>(new cEntidad().Obtener(82));
                LstMunicipios = new ObservableCollection<MUNICIPIO>();
                LstCentros = new ObservableCollection<CENTRO>(new cCentro().ObtenerTodos());
                LstEventoImpacto = new ObservableCollection<EVENTO_IMPACTO>(new cEventoImpacto().ObtenerTodos());
                LstEventoVestimenta = new ObservableCollection<EVENTO_VESTIMENTA>(new cEventoVestimenta().ObtenerTodos());
                LstEventoEstatus = new ObservableCollection<EVENTO_ESTATUS>(new cEventoEstatus().ObtenerTodos());
                ListResponsables = new ObservableCollection<EMPLEADO>(new cEmpleado().ObtenerTodos(GlobalVar.gCentro).OrderBy(o => o.PERSONA.PATERNO).ThenBy(t => t.PERSONA.MATERNO).ThenBy(t => t.PERSONA.NOMBRE));

                System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    LstEventoTipo.Insert(0, new EVENTO_TIPO() { ID_TIPO = -1, DESCR = "SELECCIONE" });
                    LstEntidades.Insert(0, new ENTIDAD() { ID_ENTIDAD = -1, DESCR = "SELECCIONE" });
                    LstMunicipios.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                    LstCentros.Insert(0, new CENTRO() { ID_CENTRO = -1, DESCR = "SELECCIONE" });
                    LstEventoImpacto.Insert(0, new EVENTO_IMPACTO() { ID_IMPACTO = -1, DESCR = "SELECCIONE" });
                    LstEventoVestimenta.Insert(0, new EVENTO_VESTIMENTA() { ID_VESTIMENTA = -1, DESCR = "SELECCIONE" });
                    LstEventoEstatus.Insert(0, new EVENTO_ESTATUS() { ID_ESTATUS = -1, DESCR = "SELECCIONE" });
                    ListResponsables.Insert(0, new EMPLEADO() { ID_EMPLEADO = -1, PERSONA = new PERSONA { NOMBRE = "SELECCIONE" } });
                    ValidacionEvento();
                    ConfiguraPermisos();
                    StaticSourcesViewModel.SourceChanged = false;

                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar listados", ex);
            }
        }
        #endregion

        #region ProgramacionEventos
        private void ObtenerTodo()
        {
            //LstEventos = new ObservableCollection<EVENTO>(new cEvento().ObtenerTodos(BNombre,BTipo,BFecha,GlobalVar.gCentro));
            //BEventosEmpty = LstEventos.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        private async Task<List<EVENTO>> SegmentarBusqueda(int _Pag = 1)
        {
            try
            {
                PaginaEvento = _Pag;
                var result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<EVENTO>>(() =>
                             new ObservableCollection<EVENTO>(new cEvento().ObtenerTodos(BNombre, BTipo, BFecha, GlobalVar.gCentro, _Pag)));
                if (result.Any())
                {
                    PaginaEvento++;
                    SeguirCargandoEventos = true;
                }
                else
                    SeguirCargandoEventos = false;
                return result.ToList();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al consultar internos.", ex);
                return new List<EVENTO>();
            }
        }

        private void ObtenerEvento()
        {
            if (SelectedEvento != null)
            {
                ENombre = SelectedEvento.NOMBRE;
                EEventoTipo = SelectedEvento.ID_EVENTO_TIPO.Value;
                ECentro = SelectedEvento.ID_CENTRO != null ? SelectedEvento.ID_CENTRO.Value : (short)-1;
                ELugar = SelectedEvento.LUGAR;
                EDireccion = SelectedEvento.LUGAR_DIRECCION;
                EEstado = SelectedEvento.LUGAR_ENTIDAD != null ? SelectedEvento.LUGAR_ENTIDAD.Value : (short)-1;
                EMunicipio = SelectedEvento.LUGAR_MUNICIPIO != null ? SelectedEvento.LUGAR_MUNICIPIO.Value : (short)-1;
                EDuracionHrs = SelectedEvento.HOR_DURACION;
                EDuracionMin = SelectedEvento.MIN_DURACION != null ? SelectedEvento.MIN_DURACION : null;
                EFecha = SelectedEvento.EVENTO_FEC;
                EHoraInvitados = SelectedEvento.HORA_INVITACION;
                EHoraPresidium = SelectedEvento.HORA_ARRIBO;
                ETelefono = SelectedEvento.TELEFONO;
                EDependencia = SelectedEvento.PROMOVENTE;
                EPerfilInvitados = SelectedEvento.PERFIL_ASISTENTES;
                EObjetivo = SelectedEvento.OBJETIVO;
                EMaestroCeremonias = SelectedEvento.MAESTRO;
                EComite = SelectedEvento.COMITE == "S" ? true : false;
                EImpactoEvento = SelectedEvento.ID_IMPACTO.Value;
                EVestimentaSugerida = SelectedEvento.ID_VESTIMENTA.Value;
                EConvocartoriaMedios = SelectedEvento.MEDIOS == "S" ? true : false;
                EObjetivoGral = SelectedEvento.OBJETIVO_GENERAL;
                EObservacion = SelectedEvento.OBSERV;
                if (SelectedEvento.ID_ESTATUS != null)
                    EEstatus = SelectedEvento.ID_ESTATUS.Value;
                SelectResponsable = SelectedEvento.ID_PERSONA_RESPONSABLE != null ? SelectedEvento.ID_PERSONA_RESPONSABLE : -1;
                //Programa
                LstEventoPrograma = new ObservableCollection<EVENTO_PROGRAMA>(SelectedEvento.EVENTO_PROGRAMA.OrderBy(w => w.ID_CONSEC));
                EProgramaVisible = LstEventoPrograma.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                //Presidium
                LstEventoPresidium = new ObservableCollection<EVENTO_PRESIDIUM>(SelectedEvento.EVENTO_PRESIDIUM.OrderBy(w => w.ID_CONSEC));
                EPresidiumVisible = LstEventoPresidium.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                //Informacion Tecnica
                LstEventoInfTecnica = new ObservableCollection<EVENTO_INF_TECNICA>(SelectedEvento.EVENTO_INF_TECNICA);
                EInfTecnicaVisible = LstEventoInfTecnica.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                //Internos
                LstIngresosSeleccionados = new ObservableCollection<INGRESO>();
                if (SelectedEvento.EVENTO_INGRESO != null)
                {
                    foreach (var i in SelectedEvento.EVENTO_INGRESO)
                    {
                        LstIngresosSeleccionados.Add(i.INGRESO);
                    }
                }

                if (SelectedEvento.ID_ESTATUS == (short)enumEventoEstatus.REALIZADO)
                    EEstatusEnabled = false;
                else
                    EEstatusEnabled = true;
                StaticSourcesViewModel.SourceChanged = false;
            }
            else
                new Dialogos().ConfirmacionDialogo("Validacion", "Favor de seleccionar un evento");
        }

        private bool Guardar()
        {
            var obj = new EVENTO();

            obj.ID_EVENTO_TIPO = EEventoTipo;
            obj.NOMBRE = ENombre;
            if (ECentro != -1)
            {
                obj.ID_CENTRO = ECentro;
                obj.LUGAR = string.Empty;
            }
            else
            {
                obj.LUGAR = ELugar;
            }
            obj.LUGAR_DIRECCION = EDireccion;
            obj.LUGAR_ENTIDAD = EEstado;
            obj.LUGAR_MUNICIPIO = EMunicipio;

            obj.ID_IMPACTO = EImpactoEvento;
            obj.ID_VESTIMENTA = EVestimentaSugerida;
            obj.HOR_DURACION = EDuracionHrs;
            obj.MIN_DURACION = EDuracionMin;
            obj.EVENTO_FEC = EFecha;
            obj.HORA_INVITACION = EHoraInvitados;
            obj.HORA_ARRIBO = EHoraPresidium;
            obj.TELEFONO = ETelefono.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
            obj.PROMOVENTE = EDependencia;
            obj.PERFIL_ASISTENTES = EPerfilInvitados;
            obj.OBJETIVO = EObjetivo;
            obj.MAESTRO = EMaestroCeremonias;
            obj.COMITE = EComite ? "S" : "N";
            obj.OBSERV = EObservacion;
            obj.MEDIOS = EConvocartoriaMedios ? "S" : "N";
            obj.OBJETIVO_GENERAL = EObjetivoGral;
            obj.ID_ESTATUS = EEstatus;
            obj.CENTRO_REGISTRA = GlobalVar.gCentro;
            obj.ID_PERSONA_RESPONSABLE = SelectResponsable;
            if (SelectedEvento == null)//INSERT
            {
                var programa = new List<EVENTO_PROGRAMA>(LstEventoPrograma == null ? null : LstEventoPrograma.Select((w, i) => 
                    new EVENTO_PROGRAMA() { ID_EVENTO = 0, ID_CONSEC = Convert.ToInt16(i + 1), DESCR = w.DESCR, DURACION = w.DURACION }));
                obj.EVENTO_PROGRAMA = programa;
                var presidium = new List<EVENTO_PRESIDIUM>(LstEventoPresidium == null ? null : LstEventoPresidium.Select((w, i) => 
                    new EVENTO_PRESIDIUM() { ID_EVENTO = 0, ID_CONSEC = Convert.ToInt16(i + 1), NOMBRE = w.NOMBRE, PUESTO = w.PUESTO }));
                obj.EVENTO_PRESIDIUM = presidium;
                var infTecnica = new List<EVENTO_INF_TECNICA>(LstEventoInfTecnica == null ? null : LstEventoInfTecnica.Select((w, i) => 
                    new EVENTO_INF_TECNICA() { ID_EVENTO = 0, ID_CONSEC = Convert.ToInt16(i + 1), DESCR = w.DESCR }));
                obj.EVENTO_INF_TECNICA = infTecnica;
                var internos = new List<EVENTO_INGRESO>(LstIngresosSeleccionados == null ? null : LstIngresosSeleccionados.Select((w, i) => 
                    new EVENTO_INGRESO() { ID_EVENTO = 0, ID_CENTRO = w.ID_CENTRO, ID_ANIO = w.ID_ANIO, ID_IMPUTADO = w.ID_IMPUTADO, ID_INGRESO = w.ID_INGRESO }));
                obj.EVENTO_INGRESO = internos;
                obj.ID_EVENTO = new cEvento().Insertar(obj);
                if (obj.ID_EVENTO > 0)
                {
                    SelectedEvento = obj;
                    StaticSourcesViewModel.SourceChanged = false;
                    return true;
                }
            }
            else
            {
                obj.ID_EVENTO = SelectedEvento.ID_EVENTO;
                var programa = new List<EVENTO_PROGRAMA>(LstEventoPrograma == null ? null : LstEventoPrograma.Select((w, i) => 
                    new EVENTO_PROGRAMA() { ID_EVENTO = obj.ID_EVENTO, ID_CONSEC = Convert.ToInt16(i + 1), DESCR = w.DESCR, DURACION = w.DURACION }));
                var presidium = new List<EVENTO_PRESIDIUM>(LstEventoPresidium == null ? null : LstEventoPresidium.Select((w, i) => 
                    new EVENTO_PRESIDIUM() { ID_EVENTO = obj.ID_EVENTO, ID_CONSEC = Convert.ToInt16(i + 1), NOMBRE = w.NOMBRE, PUESTO = w.PUESTO }));
                var informacion = new List<EVENTO_INF_TECNICA>(LstEventoInfTecnica == null ? null : LstEventoInfTecnica.Select((w, i) => 
                    new EVENTO_INF_TECNICA() { ID_EVENTO = obj.ID_EVENTO, ID_CONSEC = Convert.ToInt16(i + 1), DESCR = w.DESCR }));
                var internos = new List<EVENTO_INGRESO>(LstIngresosSeleccionados == null ? null : LstIngresosSeleccionados == null ? null : LstIngresosSeleccionados.Select(w => 
                    new EVENTO_INGRESO() { ID_EVENTO = SelectedEvento.ID_EVENTO, ID_CENTRO = w.ID_CENTRO, ID_ANIO = w.ID_ANIO, ID_IMPUTADO = w.ID_IMPUTADO, ID_INGRESO = w.ID_INGRESO }));
                if (new cEvento().Actualizar(obj, programa, presidium, informacion, internos))
                {
                    if (obj.ID_ESTATUS == (short)enumEventoEstatus.REALIZADO)
                        EEstatusEnabled = false;
                    StaticSourcesViewModel.SourceChanged = false;
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Evento Programa
        private void LimpiarPrograma()
        {
            EPDescripcion = EPDuracion = string.Empty;
            SelectedEventoPrograma = null;
        }

        private void ObtenerPrograma()
        {
            if (SelectedEventoPrograma != null)
            {
                EPDescripcion = SelectedEventoPrograma.DESCR;
                EPDuracion = SelectedEventoPrograma.DURACION;
            }
            else
                new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un programa");

        }

        private void AgregarPrograma()
        {
            if (LstEventoPrograma == null)
                LstEventoPrograma = new ObservableCollection<EVENTO_PROGRAMA>();
            if (SelectedEventoPrograma == null)
                LstEventoPrograma.Add(new EVENTO_PROGRAMA() { DESCR = EPDescripcion, DURACION = EPDuracion });
            else
            {
                SelectedEventoPrograma.DESCR = EPDescripcion;
                SelectedEventoPrograma.DURACION = EPDuracion;
                LstEventoPrograma = new ObservableCollection<EVENTO_PROGRAMA>(LstEventoPrograma);
            }
            EProgramaVisible = LstEventoPrograma.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            LimpiarPrograma();
        }

        private bool QuitarPrograma()
        {
            if (LstEventoPrograma != null)
            {
                if (LstEventoPrograma.Remove(SelectedEventoPrograma))
                {
                    LstEventoPrograma = new ObservableCollection<EVENTO_PROGRAMA>(LstEventoPrograma);
                    EProgramaVisible = LstEventoPrograma.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                    return true;
                }
            }
            return false;
        }

        private bool GuardarEventoPrograma(int Id)
        {
            if (LstEventoPrograma != null)
            {
                var programa = new List<EVENTO_PROGRAMA>(LstEventoPrograma.Select((w, i) => new EVENTO_PROGRAMA() { ID_EVENTO = Id, ID_CONSEC = Convert.ToInt16(i + 1), DESCR = w.DESCR, DURACION = w.DURACION }));
                //short index = 1;
                //foreach (var p in LstEventoPrograma)
                //{
                //    programa.Add(new EVENTO_PROGRAMA() { ID_EVENTO = Id, ID_CONSEC = index, DESCR = p.DESCR, DURACION = p.DURACION });
                //    index++;
                //}
                if (new cEventoPrograma().Insertar(Id, programa))
                    return true;
            }
            return false;
        }

        private void UpPrograma()
        {
            if (SelectedEventoPrograma != null)
            {
                if (PIndex > 0)
                {
                    var i = PIndex;
                    var x = SelectedEventoPrograma;
                    LstEventoPrograma.Remove(SelectedEventoPrograma);
                    LstEventoPrograma.Insert(i - 1, x);
                    SelectedEventoPrograma = x;
                }
            }
            else
                new Dialogos().ConfirmacionDialogo("Validacion", "Favor de seleccionar programa a mover");
        }

        private void DownPrograma()
        {
            if (SelectedEventoPrograma != null)
            {
                if (PIndex < LstEventoPrograma.Count - 1)
                {
                    var i = PIndex;
                    var x = SelectedEventoPrograma;
                    LstEventoPrograma.Remove(SelectedEventoPrograma);
                    LstEventoPrograma.Insert(i + 1, x);
                    SelectedEventoPrograma = x;
                }
            }
            else
                new Dialogos().ConfirmacionDialogo("Validacion", "Favor de seleccionar programa a mover");
        }
        #endregion

        #region Evento Presidium
        private void LimpiarPresidium()
        {
            EPNombre = EPPuesto = string.Empty;
            SelectedEventoPresidium = null;
        }

        private void ObtenerPresidium()
        {
            if (SelectedEventoPresidium != null)
            {
                EPNombre = SelectedEventoPresidium.NOMBRE;
                EPPuesto = SelectedEventoPresidium.PUESTO;
            }
            else
                new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar a un interante del presidium");
        }

        private void AgregarPresidium()
        {
            if (LstEventoPresidium == null)
                LstEventoPresidium = new ObservableCollection<EVENTO_PRESIDIUM>();
            if (SelectedEventoPresidium == null)
                LstEventoPresidium.Add(new EVENTO_PRESIDIUM() { NOMBRE = EPNombre, PUESTO = EPPuesto });
            else
            {
                SelectedEventoPresidium.NOMBRE = EPNombre;
                SelectedEventoPresidium.PUESTO = EPPuesto;
                LstEventoPresidium = new ObservableCollection<EVENTO_PRESIDIUM>(LstEventoPresidium);
            }
            EPresidiumVisible = LstEventoPresidium.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            LimpiarPresidium();
        }

        private bool QuitarPresidium()
        {
            if (LstEventoPresidium != null)
            {
                if (LstEventoPresidium.Remove(SelectedEventoPresidium))
                {
                    LstEventoPresidium = new ObservableCollection<EVENTO_PRESIDIUM>(LstEventoPresidium);
                    EPresidiumVisible = LstEventoPresidium.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                    return true;
                }
            }
            return false;
        }

        private bool GuardarEventoPresidium(int Id)
        {
            if (LstEventoPresidium != null)
            {
                var presidium = new List<EVENTO_PRESIDIUM>(LstEventoPresidium.Select((w, i) => new EVENTO_PRESIDIUM() { ID_EVENTO = Id, ID_CONSEC = Convert.ToInt16(i + 1), NOMBRE = w.NOMBRE, PUESTO = w.PUESTO }));
                //short index = 1;
                //foreach (var p in LstEventoPresidium)
                //{
                //    presidium.Add(new EVENTO_PRESIDIUM() { ID_EVENTO = Id, ID_CONSEC = index, NOMBRE = p.NOMBRE, PUESTO = p.PUESTO });
                //    index++;
                //}
                if (new cEventoPresidium().Insertar(Id, presidium))
                    return true;
            }
            return false;
        }

        private void UpPresidium()
        {
            if (SelectedEventoPresidium != null)
            {
                if (PRIndex > 0)
                {
                    var i = PRIndex;
                    var x = SelectedEventoPresidium;
                    LstEventoPresidium.Remove(SelectedEventoPresidium);
                    LstEventoPresidium.Insert(i - 1, x);
                    SelectedEventoPresidium = x;
                }
            }
            else
                new Dialogos().ConfirmacionDialogo("Validacion", "Favor de seleccionar programa a mover");
        }

        private void DownPresidium()
        {
            if (SelectedEventoPresidium != null)
            {
                if (PRIndex < LstEventoPresidium.Count - 1)
                {
                    var i = PRIndex;
                    var x = SelectedEventoPresidium;
                    LstEventoPresidium.Remove(SelectedEventoPresidium);
                    LstEventoPresidium.Insert(i + 1, x);
                    SelectedEventoPresidium = x;
                }
            }
            else
                new Dialogos().ConfirmacionDialogo("Validacion", "Favor de seleccionar programa a mover");
        }
        #endregion

        #region Evento Informacion Tecnica
        private void LimpiarInfTecnica()
        {
            ETDescripcion = string.Empty;
            SelectedEventoInfTecnica = null;
        }

        private void ObtenerInformacionTecnica()
        {
            if (SelectedEventoInfTecnica != null)
            {
                ETDescripcion = SelectedEventoInfTecnica.DESCR;
            }
            else
                new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar información tecnica");
        }

        private void AgregarInformacionTecnica()
        {
            if (LstEventoInfTecnica == null)
                LstEventoInfTecnica = new ObservableCollection<EVENTO_INF_TECNICA>();
            if (SelectedEventoInfTecnica == null)
                LstEventoInfTecnica.Add(new EVENTO_INF_TECNICA() { DESCR = ETDescripcion });
            else
            {
                SelectedEventoInfTecnica.DESCR = ETDescripcion;
                LstEventoInfTecnica = new ObservableCollection<EVENTO_INF_TECNICA>(LstEventoInfTecnica);
            }
            EInfTecnicaVisible = LstEventoInfTecnica.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            LimpiarInfTecnica();
        }

        private bool QuitarInformacionTecnica()
        {
            if (LstEventoInfTecnica != null)
            {
                if (LstEventoInfTecnica.Remove(SelectedEventoInfTecnica))
                {
                    LstEventoInfTecnica = new ObservableCollection<EVENTO_INF_TECNICA>(LstEventoInfTecnica);
                    EInfTecnicaVisible = LstEventoInfTecnica.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                    return true;
                }
            }
            return false;
        }

        private bool GuardarEventoInformacionTecnica(int Id)
        {
            if (LstEventoInfTecnica != null)
            {
                var informacion = new List<EVENTO_INF_TECNICA>(LstEventoInfTecnica.Select((w, i) => new EVENTO_INF_TECNICA() { ID_EVENTO = Id, ID_CONSEC = Convert.ToInt16(i + 1), DESCR = w.DESCR }));
                //short index = 1;
                //foreach (var p in LstEventoInfTecnica)
                //{
                //    informacion.Add(new EVENTO_INF_TECNICA() { ID_EVENTO = Id, ID_CONSEC = index, DESCR = p.DESCR });
                //    index++;
                //}
                if (new cEventoInfTecnica().Insertar(Id, informacion))
                    return true;
            }
            return false;
        }
        #endregion

        #region Buscar
        private void LimpiarBuscar()
        {
            BNombre = string.Empty;
            BTipo = -1;
            BFecha = null;
        }
        #endregion

        #region Impresion Evento
        private void ImpresionEvento()
        {
            if (SelectedEvento != null)
            {
                SelectedEvento = new cEvento().Obtener(SelectedEvento.ID_EVENTO).FirstOrDefault();

                var parametros = new Dictionary<string, string>();
                parametros.Add("<<nombre_evento>>", SelectedEvento.NOMBRE.Trim());
                if (SelectedEvento.CENTRO != null)//CENTRO
                {
                    parametros.Add("<<lugar>>", SelectedEvento.CENTRO.DESCR.Trim());
                }
                else//EXTERNO
                {
                    parametros.Add("<<lugar>>", SelectedEvento.LUGAR.Trim());
                }
                parametros.Add("<<direccion>>", SelectedEvento.LUGAR_DIRECCION);
                parametros.Add("<<ciudad>>", string.Format("{0},{1}", SelectedEvento.MUNICIPIO.MUNICIPIO1.Trim(), SelectedEvento.MUNICIPIO.ENTIDAD.DESCR.Trim()));
                parametros.Add("<<fecha>>", Fechas.fechaLetra(SelectedEvento.EVENTO_FEC.Value, false));
                parametros.Add("<<duracion>>", string.Format("{0} hrs {1} min", SelectedEvento.HOR_DURACION, SelectedEvento.MIN_DURACION));
                parametros.Add("<<hora_invitados>>", SelectedEvento.HORA_INVITACION.Value.ToString("hh:mm tt"));
                parametros.Add("<<hora_presidium>>", SelectedEvento.HORA_ARRIBO.Value.ToString("hh:mm tt"));
                parametros.Add("<<telefono>>", new Converters().MascaraTelefono(SelectedEvento.TELEFONO));
                parametros.Add("<<promovente>>", SelectedEvento.PROMOVENTE);
                parametros.Add("<<perfil_asistentes>>", SelectedEvento.PERFIL_ASISTENTES);
                parametros.Add("<<objetivo_evento>>", SelectedEvento.OBJETIVO);
                parametros.Add("<<maestro_ceremonias>>", SelectedEvento.MAESTRO);
                short index = 1;
                string programas = string.Empty;
                if (SelectedEvento.EVENTO_PROGRAMA != null)
                {
                    foreach (var p in SelectedEvento.EVENTO_PROGRAMA)
                    {
                        if (!string.IsNullOrEmpty(programas))
                            programas = string.Format("{0}\n", programas);
                        programas = string.Format("{0}{1}{2} ({3})", programas, index, p.DESCR, p.DURACION);
                        index++;
                    }
                }
                parametros.Add("<<programa>>", programas);
                index = 1;
                string presidium = string.Empty;
                if (SelectedEvento.EVENTO_PRESIDIUM != null)
                {
                    foreach (var p in SelectedEvento.EVENTO_PRESIDIUM)
                    {
                        if (!string.IsNullOrEmpty(presidium))
                            presidium = string.Format("{0}\n", presidium);
                        presidium = string.Format("{0}{1}{2} -{3}", presidium, index, p.NOMBRE, p.PUESTO);
                        index++;
                    }
                }
                parametros.Add("<<presidium>>", presidium);
                parametros.Add("<<comite>>", SelectedEvento.COMITE == "S" ? "SI" : "NO");
                parametros.Add("<<impacto_evento>>", SelectedEvento.EVENTO_IMPACTO != null ? !string.IsNullOrEmpty(SelectedEvento.EVENTO_IMPACTO.DESCR) ? SelectedEvento.EVENTO_IMPACTO.DESCR.Trim() : string.Empty : string.Empty);
                parametros.Add("<<vestimenta>>", SelectedEvento.EVENTO_VESTIMENTA != null ? !string.IsNullOrEmpty(SelectedEvento.EVENTO_VESTIMENTA.DESCR) ? SelectedEvento.EVENTO_VESTIMENTA.DESCR.Trim() : string.Empty : string.Empty);
                parametros.Add("<<medios>>", SelectedEvento.MEDIOS == "S" ? "SI" : "NO");
                parametros.Add("<<observaciones>>", SelectedEvento.OBSERV ?? string.Empty);
                parametros.Add("<<objetivo_general>>", SelectedEvento.OBJETIVO_GENERAL);
                index = 1;
                string informacionTecnica = string.Empty;
                if (SelectedEvento.EVENTO_INF_TECNICA != null)
                {
                    foreach (var p in SelectedEvento.EVENTO_INF_TECNICA)
                    {
                        if (!string.IsNullOrEmpty(informacionTecnica))
                            informacionTecnica = string.Format("{0}\n", informacionTecnica);
                        informacionTecnica = string.Format("{0}{1}{2}", programas, index, p.DESCR);
                        index++;
                    }
                }
                parametros.Add("<<informacion_tecnica>>", informacionTecnica);

                var documento = new cImputadoTipoDocumento().Obtener((short)enumTipoDocumentoImputado.FORMATO_EVENTO);//var doc = File.ReadAllBytes(@"C:\libertades\E.doc");
                if (documento == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "No se encontro la plantilla del documento");
                    return;
                }
                var bytes = new cWord().FillFieldsDocx(documento.DOCUMENTO, parametros);//FillFields(documento.DOCUMENTO, parametros);
                if (bytes == null)
                    return;
                var tc = new TextControlView();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.editor.Loaded += (s, e) =>
                {
                    try
                    {
                        switch (documento.ID_FORMATO)
                        {
                            case (int)enumFormatoDocumento.DOCX:
                                tc.editor.Load(bytes, TXTextControl.BinaryStreamType.WordprocessingML);
                                break;
                            case (int)enumFormatoDocumento.PDF:
                                tc.editor.Load(bytes, TXTextControl.BinaryStreamType.AdobePDF);
                                break;
                            case (int)enumFormatoDocumento.DOC:
                                tc.editor.Load(bytes, TXTextControl.BinaryStreamType.MSWord);
                                break;
                            default:
                                new Dialogos().ConfirmacionDialogo("Validación", string.Format("El formato {0} del documento no es valido", documento.FORMATO_DOCUMENTO.DESCR));
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                    }
                };
                tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Show();
            }
            else
                new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un evento.");

        }
        #endregion

        #region Ingresos
        private async Task ObtenerTodosActivos(int _Pag = 1)
        {
            var ingresos = await SegmentarIngresoBusqueda(_Pag);
            var temp = new ObservableCollection<cTrasladoIngreso>();
            foreach (var x in ingresos)
            {
                if (LstIngresosSeleccionados == null)
                    temp.Add(new cTrasladoIngreso() { Ingreso = x, Seleccionado = false });
                else
                {
                    if (LstIngresosSeleccionados.FirstOrDefault(w => w.ID_IMPUTADO == x.ID_IMPUTADO && w.ID_ANIO == x.ID_ANIO && w.ID_CENTRO == x.ID_CENTRO && w.ID_INGRESO == w.ID_INGRESO) == null)
                        temp.Add(new cTrasladoIngreso() { Ingreso = x, Seleccionado = false });
                    else
                        temp.Add(new cTrasladoIngreso() { Ingreso = x, Seleccionado = true });
                }
            }
            LstIngresos.InsertRange(temp);
        }

        private async Task<List<INGRESO>> SegmentarIngresoBusqueda(int _Pag = 1)
        {
            try
            {
                Pagina = _Pag;
                var result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<INGRESO>>(() =>
                             new ObservableCollection<INGRESO>(new cIngreso().ObtenerIngresosActivosFiltrados(estatus_inactivos,IAnio, IFolio, INombre, IPaterno, IMaterno, GlobalVar.gCentro, _Pag)));
                if (result.Any())
                {
                    Pagina++;
                    SeguirCargandoIngresos = true;
                }
                else
                    SeguirCargandoIngresos = false;
                return result.ToList();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al consultar internos.", ex);
                return new List<INGRESO>();
            }
        }

        private void AgregarInterno()
        {
            if (SelectedIngreso != null)
            {
                if (!SelectedIngreso.Seleccionado)
                {
                    if (LstIngresosSeleccionados == null)
                        LstIngresosSeleccionados = new ObservableCollection<INGRESO>();
                    SelectedIngreso.Seleccionado = true;
                    LstIngresosSeleccionados.Add(SelectedIngreso.Ingreso);
                    var temp = LstIngresos;
                    LstIngresos = new RangeEnabledObservableCollection<cTrasladoIngreso>();
                    LstIngresos.InsertRange(temp);
                    SelectedIngreso = null;
                }
            }
            else
                new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un ingreso");
        }

        private void QuitarInterno()
        {
            if (SelectedIngresoSeleccionado != null)
            {
                var tmp = SelectedIngresoSeleccionado;
                if (LstIngresosSeleccionados.Remove(SelectedIngresoSeleccionado))
                {
                    LstIngresosSeleccionados = new ObservableCollection<INGRESO>(LstIngresosSeleccionados);
                    var ingreso = LstIngresos.Where(x => x.Ingreso.ID_CENTRO == tmp.ID_CENTRO && x.Ingreso.ID_ANIO == tmp.ID_ANIO && x.Ingreso.ID_IMPUTADO == tmp.ID_IMPUTADO && x.Ingreso.ID_INGRESO == tmp.ID_INGRESO).SingleOrDefault();
                    if (ingreso != null)
                    {
                        ingreso.Seleccionado = false;
                        var temp = LstIngresos;
                        LstIngresos = new RangeEnabledObservableCollection<cTrasladoIngreso>();
                        LstIngresos.InsertRange(temp);
                    }
                }
            }
            else
                new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un ingreso");
        }

        private bool GuardarInterno()
        {
            try
            {
                var internos = new List<EVENTO_INGRESO>(LstIngresosSeleccionados == null ? null : LstIngresosSeleccionados.Select(w => new EVENTO_INGRESO() { ID_EVENTO = SelectedEvento.ID_EVENTO, ID_CENTRO = w.ID_CENTRO, ID_ANIO = w.ID_ANIO, ID_IMPUTADO = w.ID_IMPUTADO, ID_INGRESO = w.ID_INGRESO }));
                //if (LstIngresosSeleccionados != null)
                //{
                //    foreach (var i in LstIngresosSeleccionados)
                //    {
                //        internos.Add(new EVENTO_INGRESO() { ID_EVENTO = SelectedEvento.ID_EVENTO, ID_CENTRO = i.ID_CENTRO, ID_ANIO = i.ID_ANIO, ID_IMPUTADO = i.ID_IMPUTADO, ID_INGRESO = i.ID_INGRESO });
                //    }
                //}
                if (new cEventoIngreso().Insertar(internos, SelectedEvento.ID_EVENTO))
                    return true;

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar internos", ex);
            }
            return false;
        }

        #endregion

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.EVENTO.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                foreach (var p in permisos)
                {
                    if (p.INSERTAR == 1)
                        PInsertar = true;
                    if (p.EDITAR == 1)
                        PEditar = true;
                    if (p.CONSULTAR == 1)
                        PConsultar = true;
                    if (p.IMPRIMIR == 1)
                        PImprimir = true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }
        #endregion
    }
}
