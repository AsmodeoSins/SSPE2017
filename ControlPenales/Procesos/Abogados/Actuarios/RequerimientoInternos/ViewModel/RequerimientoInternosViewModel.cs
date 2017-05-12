using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Controlador.Catalogo.Justicia;
using System.Collections.ObjectModel;
using SSP.Servidor;
using System.Windows;
using ControlPenales.Clases;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Threading;
using System.Windows.Controls;
using Cogent.Biometrics;
using System.Runtime.InteropServices;
using DPUruNet;
using System.IO;
using System.Drawing;
using System.Threading.Tasks;
using WPFPdfViewer;
using ControlPenales.BiometricoServiceReference;

namespace ControlPenales
{
    partial class RequerimientoInternosViewModel : FingerPrintScanner
    {
        public RequerimientoInternosViewModel() { }

        private async void Load_Window(RequerimientoInternosView Window)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(CargarDatos);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos iniciales.", ex);
            }
        }

        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                #region MENU
                case "guardar_menu":
                    if (!PInsertar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        break;
                    }
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(async () =>
                    {
                        if (SelectPersona == null ? true : SelectPersona.ABOGADO == null)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar a un actuario.");
                            }));
                            return;
                        }
                        if (SelectPersona.ABOGADO.ID_ABOGADO_TITULO != Parametro.ID_ABOGADO_TITULAR_ACTUARIO)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                (new Dialogos()).ConfirmacionDialogo("Advertencia!", "El actuario seleccionado no tiene un registro correcto.");
                            }));
                            return;
                        }
                        if (ListInternosSeleccionadosPorNotificar == null ? true : ListInternosSeleccionadosPorNotificar.Count <= 0)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                (new Dialogos()).ConfirmacionDialogo("Advertencia!", "La lista de internos esta vacia.");
                            }));
                            return;
                        }
                        var hoy = Fechas.GetFechaDateServer;
                        if (!SelectFechaAsignacion.HasValue)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar una fecha.");
                            }));
                            return;
                        }
                        if (SelectFechaAsignacion.Value <= hoy.AddDays(-1))
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar una fecha mayor o igual al dia de hoy.");
                            }));
                            return;
                        }
                        /*if (new cActuarioLista().GetData().Any(a => SelectFechaAsignacion.HasValue ?
                            a.CAPTURA_FEC.HasValue ?
                                (a.CAPTURA_FEC.Value.Day == SelectFechaAsignacion.Value.Day && a.CAPTURA_FEC.Value.Month == SelectFechaAsignacion.Value.Month && a.CAPTURA_FEC.Value.Year == SelectFechaAsignacion.Value.Year)
                            : false
                        : false))
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                (new Dialogos()).ConfirmacionDialogo("Advertencia!", "La fecha seleccionada ya contiene .");
                            }));
                            return;
                        }*/
                        await GuardarListaPorNotificar();
                    });

                    break;
                case "limpiar_menu":
                    base.ClearRules();
                    LimpiarCampos();
                    break;
                case "buscar_menu":
                    if (!PConsultar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        return;
                    }
                    TextNombre = TextNombreAbogado;
                    TextMaterno = TextMaternoAbogado;
                    TextPaterno = TextPaternoAbogado;
                    var pers1 = SelectPersona;
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(BuscarPersonasSinCodigo);
                    SelectPersonaAuxiliar = pers1;
                    LimpiarBusqueda();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                    break;
                case "ayuda_menu":
                    break;
                case "salir_menu":
                    PrincipalViewModel.SalirMenu();
                    break;
                #endregion

                #region BUSCAR_ABOGADOS
                case "nueva_busqueda_visitante":
                    TextPaternoAbogado = TextMaternoAbogado = TextNombreAbogado = TextPaterno = TextMaterno = TextNombre = string.Empty;
                    ListPersonasAuxiliar = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                    ListPersonas = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                    break;
                case "buscar_abogado":
                    if (!PConsultar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        return;
                    }
                    TextNombre = TextNombreAbogado;
                    TextMaterno = TextMaternoAbogado;
                    TextPaterno = TextPaternoAbogado;
                    var pers3 = SelectPersona;
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(BuscarPersonasSinCodigo);
                    SelectPersonaAuxiliar = pers3;
                    LimpiarBusqueda();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                    #region Comentado
                    //if (!PConsultar)
                    //{
                    //    (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                    //    return;
                    //}

                    //if (TextPaterno == null)
                    //    TextPaterno = string.Empty;
                    //if (TextMaterno == null)
                    //    TextMaterno = string.Empty;
                    //if (TextNombre == null)
                    //    TextNombre = string.Empty;
                    //if (TextCodigo == null)
                    //    TextCodigo = string.Empty;
                    //var pers3 = SelectPersona;
                    //SelectPersonaAuxiliar = pers3;
                    //if (string.IsNullOrEmpty(TextCodigoAbogado))
                    //    BuscarPersonasSinCodigo();
                    //else
                    //    BuscarPersonas();
                    #endregion
                    break;
                case "seleccionar_buscar_persona":
                    if (SelectPersona == null)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar a una persona.");
                        break;
                    }
                    if (StaticSourcesViewModel.SourceChanged)
                    {
                        if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                            "Existen cambios sin guardar, esta seguro que desea seleccionar a otra persona?") != 1)
                            return;
                    }
                    //SelectPersona = ListPersonas.FirstOrDefault();
                    if (SelectPersona.ABOGADO != null)
                    {
                        if (SelectPersona.ABOGADO.ID_ABOGADO_TITULO == Parametro.ID_ABOGADO_TITULAR_ACTUARIO)
                        {
                            GetDatosPersonaSeleccionada();
                            StaticSourcesViewModel.SourceChanged = false;
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                        }
                        else
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "La persona seleccionada no es un actuario.");
                    }
                    else
                    {
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "La persona seleccionada no es un actuario.");
                    }
                    break;
                case "cancelar_buscar_persona":
                    var pers = SelectPersonaAuxiliar;
                    SelectPersona = pers;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                    break;
                case "buscar_por_huella":
                    break;
                #endregion
            }
        }

        private async Task GuardarListaPorNotificar()
        {
            try
            {
                var hoy = Fechas.GetFechaDateServer;
                var ActuarioLista = new ACTUARIO_LISTA();
                var ListaPorNotificar = new List<ACTUARIO_INGRESO>();
                var Existencias = SelectPersona.ABOGADO.ACTUARIO_LISTA.Where(w => w.CAPTURA_FEC.HasValue ?
                    (w.CAPTURA_FEC.Value.Day == SelectFechaAsignacion.Value.Day && w.CAPTURA_FEC.Value.Month == SelectFechaAsignacion.Value.Month && w.CAPTURA_FEC.Value.Year == SelectFechaAsignacion.Value.Year) : false);
                if (Existencias.Any())
                {
                    ActuarioLista = new ACTUARIO_LISTA
                    {
                        ID_ABOGADO = Existencias.FirstOrDefault().ID_ABOGADO,
                        ID_LISTA = Existencias.FirstOrDefault().ID_LISTA,
                        CAPTURA_FEC = Existencias.FirstOrDefault().CAPTURA_FEC
                    };
                    if (Existencias.FirstOrDefault().ACTUARIO_INGRESO.Count > 0)
                        foreach (var itm in ListInternosSeleccionadosPorNotificar.Where(w => w.ELEGIDO))
                            foreach (var exis in Existencias)
                                foreach (var item in exis.ACTUARIO_INGRESO.Where(w => w.ID_LISTA == ActuarioLista.ID_LISTA))
                                {
                                    if (itm.INGRESO.ID_CENTRO == item.ID_CENTRO && itm.INGRESO.ID_ANIO == item.ID_ANIO && itm.INGRESO.ID_IMPUTADO == item.ID_IMPUTADO && itm.INGRESO.ID_INGRESO == item.ID_INGRESO) { }
                                    else if (!ListaPorNotificar.Any(a => a.ID_CENTRO == itm.INGRESO.ID_CENTRO && a.ID_ANIO == itm.INGRESO.ID_ANIO && a.ID_IMPUTADO == itm.INGRESO.ID_IMPUTADO && a.ID_INGRESO == itm.INGRESO.ID_INGRESO))
                                        ListaPorNotificar.Add(new ACTUARIO_INGRESO
                                        {
                                            ID_ANIO = itm.INGRESO.ID_ANIO,
                                            ID_CENTRO = itm.INGRESO.ID_CENTRO,
                                            ID_IMPUTADO = itm.INGRESO.ID_IMPUTADO,
                                            ID_INGRESO = itm.INGRESO.ID_INGRESO,
                                            ID_LISTA = ActuarioLista.ID_LISTA,
                                        });
                                }
                    else
                        foreach (var item in ListInternosSeleccionadosPorNotificar.Where(w => w.ELEGIDO))
                            ListaPorNotificar.Add(new ACTUARIO_INGRESO
                            {
                                ID_ANIO = item.INGRESO.ID_ANIO,
                                ID_CENTRO = item.INGRESO.ID_CENTRO,
                                ID_IMPUTADO = item.INGRESO.ID_IMPUTADO,
                                ID_INGRESO = item.INGRESO.ID_INGRESO,
                                ID_LISTA = ActuarioLista.ID_LISTA,
                            });
                }
                else
                {
                    ActuarioLista = new ACTUARIO_LISTA()
                    {
                        ID_ABOGADO = SelectPersona.ID_PERSONA,
                        CAPTURA_FEC = SelectFechaAsignacion,
                    };
                    if (new cActuarioLista().Insertar(ActuarioLista))
                        foreach (var item in ListInternosSeleccionadosPorNotificar.Where(w => w.ELEGIDO))
                            ListaPorNotificar.Add(new ACTUARIO_INGRESO
                            {
                                ID_ANIO = item.INGRESO.ID_ANIO,
                                ID_CENTRO = item.INGRESO.ID_CENTRO,
                                ID_IMPUTADO = item.INGRESO.ID_IMPUTADO,
                                ID_INGRESO = item.INGRESO.ID_INGRESO,
                                ID_LISTA = ActuarioLista.ID_LISTA
                            });
                    else
                    {
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "No se pudo guardar la información.");
                        }));
                        return;
                    }
                }
                if (ListaPorNotificar.Count > 0)
                    if (!new cActuarioIngreso().InsertarListaTransaccion(ListaPorNotificar, ActuarioLista.ID_LISTA))
                    {
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "No se pudo guardar la información.");
                        }));
                        return;
                    }
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    SelectPersona = new cPersona().ObtenerPersonaXID(ActuarioLista.ID_ABOGADO).FirstOrDefault();
                    GetDatosPersonaSeleccionada();
                    StaticSourcesViewModel.SourceChanged = false;
                });

                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    (new Dialogos()).ConfirmacionDialogo("Éxito!", "INFORMACIÓN GRABADA EXITOSAMENTE.");
                }));
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar la información.", ex);
                }));
            }
        }

        private void CargarDatos()
        {
            ListJuzgado = new ObservableCollection<JUZGADO>(new cJuzgado().ObtenerTodos());
            var estatus = new cEstatusVisita().ObtenerTodos();
            ConfiguraPermisos();
            Application.Current.Dispatcher.Invoke((Action)(delegate
            {
                ListEstatus = new ObservableCollection<ESTATUS_VISITA>();
                foreach (var item in Parametro.ESTATUS_ABOGADOS)
                {
                    var _id_estatus_visita = short.Parse(item);
                    if (estatus.Where(w => w.ID_ESTATUS_VISITA == _id_estatus_visita).Any())
                    {
                        ListEstatus.Add(estatus.Where(w => w.ID_ESTATUS_VISITA == _id_estatus_visita).FirstOrDefault());
                    }
                }
                ListEstatus.Insert(0, new ESTATUS_VISITA { ID_ESTATUS_VISITA = -1, DESCR = "SELECCIONE" });
                ListJuzgado.Insert(0, new JUZGADO { ID_JUZGADO = -1, DESCR = "SELECCIONE" });
                SelectJuzgadoItem = ListJuzgado.Where(w => w.ID_JUZGADO == -1).FirstOrDefault();
                SelectJuzgado = SelectJuzgadoItem.ID_JUZGADO;
                SelectSexo = "M";
                SelectEstatusVisita = -1;
            }));
        }

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REQUERIMIENTO_INTERNOS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                if (permisos.Any())
                {
                    foreach (var p in permisos)
                    {
                        if (p.INSERTAR == 1)
                            PInsertar = true;
                        if (p.EDITAR == 1)
                            PEditar = true;
                        if (p.CONSULTAR == 1)
                            PConsultar = true;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }
        #endregion

        private void EnterAbogados(Object obj)
        {
            try
            {
                if (!PConsultar)
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                    return;
                }
                BuscarPersonas();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la búsqueda.", ex);
            }
        }

        private void EnterPersonas(Object obj)
        {
            try
            {
                if (!PConsultar)
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                    return;
                }
                BuscarPersonasSinCodigo();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la búsqueda.", ex);
            }
        }

        private async Task<List<SSP.Servidor.PERSONA>> SegmentarPersonasBusqueda(int _Pag = 1)
        {
            try
            {
                if (string.IsNullOrEmpty(TextPaternoAbogado) && string.IsNullOrEmpty(TextMaternoAbogado) && string.IsNullOrEmpty(TextNombreAbogado) && string.IsNullOrEmpty(TextCodigoAbogado))
                    return new List<SSP.Servidor.PERSONA>();
                Pagina = _Pag;
                var legal = short.Parse(Parametro.ID_TIPO_PERSONA_LEGAL);
                var result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<SSP.Servidor.PERSONA>>(() =>
                        new ObservableCollection<SSP.Servidor.PERSONA>(new cPersona().ObtenerAbogadosXTipoTitular(TextNombreAbogado,
                            TextPaternoAbogado, TextMaternoAbogado, 0, Parametro.ID_ABOGADO_TITULAR_ACTUARIO, _Pag)
                             .OrderByDescending(o => o.ABOGADO != null).ThenByDescending(t => t.ID_TIPO_PERSONA == legal)));
                if (result.Any())
                {
                    Pagina++;
                    SeguirCargandoPersonas = true;
                }
                else
                    SeguirCargandoPersonas = false;
                return result.ToList();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al querer guardar.", ex);
                return new List<SSP.Servidor.PERSONA>();
            }
        }

        private async void BuscarPersonas()
        {
            try
            {
                ListPersonasAuxiliar = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                ListPersonas = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                if (string.IsNullOrEmpty(TextCodigoAbogado))
                {
                    var persona = SelectPersona;
                    ListPersonas.InsertRange(await SegmentarPersonasBusqueda());
                    ListPersonasAuxiliar.InsertRange(ListPersonas);
                    TextNombre = TextNombreAbogado;
                    TextPaterno = TextPaternoAbogado;
                    TextMaterno = TextMaternoAbogado;
                    SelectPersonaAuxiliar = persona;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                }
                else
                {
                    var legal = short.Parse(Parametro.ID_TIPO_PERSONA_LEGAL);
                    var persona = SelectPersona;
                    ListPersonas.InsertRange(await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<SSP.Servidor.PERSONA>>(() =>
                       new ObservableCollection<SSP.Servidor.PERSONA>(new cPersona().ObtenerTodos(TextNombreAbogado, TextPaternoAbogado, TextMaternoAbogado, int.Parse(TextCodigoAbogado))
                             .OrderByDescending(o => o.ABOGADO != null).ThenByDescending(t => t.ID_TIPO_PERSONA == legal))));
                    ListPersonasAuxiliar.InsertRange(ListPersonas);
                    SelectPersonaAuxiliar = persona;
                    if (ListPersonas.Count == 1)
                    {
                        if (StaticSourcesViewModel.SourceChanged)
                        {
                            if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                                "Existen cambios sin guardar, esta segúro que desea seleccionar a otra persona?") != 1)
                                return;
                        }
                        var persons = ListPersonas;
                        LimpiarCampos();
                        ListPersonas = persons;
                        SelectPersona = ListPersonas.FirstOrDefault();
                        if (SelectPersona.ABOGADO != null)
                        {
                            if (SelectPersona.ABOGADO.ID_ABOGADO_TITULO == Parametro.ID_ABOGADO_TITULAR_ACTUARIO)
                            {
                                GetDatosPersonaSeleccionada();
                                StaticSourcesViewModel.SourceChanged = false;
                                LimpiarBusqueda();
                                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                            }
                            else
                                (new Dialogos()).ConfirmacionDialogo("Advertencia!", "La persona seleccionada no es un actuario.");
                        }
                        else
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "La persona seleccionada no es un actuario.");
                        }
                    }
                    else
                    {
                        TextNombre = TextNombreAbogado;
                        TextPaterno = TextPaternoAbogado;
                        TextMaterno = TextMaternoAbogado;
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                    }
                }
                if (ListPersonas != null)
                    EmptyBuscarRelacionInternoVisible = !(ListPersonas.Count > 0);
                else
                    EmptyBuscarRelacionInternoVisible = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al traer datos del visitante.", ex);
            }
        }

        private void BuscarPersonasSinCodigo()
        {
            try
            {
                TextNombreAbogado = TextNombre == null ? (TextNombre = string.Empty) : TextNombre;
                TextPaternoAbogado = TextPaterno == null ? (TextPaterno = string.Empty) : TextPaterno;
                TextMaternoAbogado = TextMaterno == null ? (TextMaterno = string.Empty) : TextMaterno;
                var legal = short.Parse(Parametro.ID_TIPO_PERSONA_LEGAL);
                Application.Current.Dispatcher.Invoke((System.Action)(async delegate
                {
                    var person = SelectPersona;
                    ListPersonasAuxiliar = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                    ListPersonas = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                    ListPersonas.InsertRange(await SegmentarPersonasBusqueda());
                    ListPersonasAuxiliar.InsertRange(ListPersonas);
                    if (PopUpsViewModels.VisibleBuscarPersonasExistentes == Visibility.Collapsed)
                    {
                        SelectPersonaAuxiliar = person;
                        LimpiarBusqueda();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                    }
                    if (ListPersonas != null)
                        EmptyBuscarRelacionInternoVisible = !(ListPersonas.Count > 0);
                    else
                        EmptyBuscarRelacionInternoVisible = false;

                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al traer datos del visitante.", ex);
            }
        }

        private void GetDatosPersonaSeleccionada()
        {
            try
            {
                //var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                TextCodigoAbogado = SelectPersona.ID_PERSONA.ToString();
                TextPaternoAbogado = SelectPersona.PATERNO.Trim();
                TextMaternoAbogado = SelectPersona.MATERNO.Trim();
                TextNombreAbogado = SelectPersona.NOMBRE.Trim();
                SelectSexo = SelectPersona.SEXO;
                var hoy = Fechas.GetFechaDateServer;
                SelectFechaNacimiento = SelectPersona.FEC_NACIMIENTO.HasValue ? SelectPersona.FEC_NACIMIENTO.Value : hoy;
                TextCurp = SelectPersona.CURP;
                TextRfc = SelectPersona.RFC;
                TextTelefonoFijo = SelectPersona.TELEFONO;
                TextTelefonoMovil = SelectPersona.TELEFONO_MOVIL;
                TextCorreo = SelectPersona.CORREO_ELECTRONICO;
                TextIne = SelectPersona.IFE;
                ImagenAbogado = SelectPersona.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any() ?
                    SelectPersona.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO :
                        new Imagenes().getImagenPerson();
                if (SelectPersona.ABOGADO != null)
                {
                    TextCedulaCJF = SelectPersona.ABOGADO.CJF;
                    if (SelectPersona.ABOGADO.ALTA_FEC.HasValue)
                        SelectFechaAlta = SelectPersona.ABOGADO.ALTA_FEC.Value.ToString("dd/MM/yyyy");
                    else
                        SelectFechaAlta = string.Empty;

                    //SelectFechaAlta = SelectPersona.ABOGADO.ALTA_FEC.HasValue ? SelectPersona.ABOGADO.ALTA_FEC.Value.ToString("dd/MM/yyyy") : hoy.ToString("dd/MM/yyyy");
                    SelectEstatusVisita = SelectPersona.ABOGADO.ID_ESTATUS_VISITA.HasValue ? SelectPersona.ABOGADO.ID_ESTATUS_VISITA.Value : (short)-1;
                    SelectJuzgadoItem = SelectPersona.ABOGADO.ID_JUZGADO.HasValue ? SelectPersona.ABOGADO.JUZGADO : ListJuzgado.Where(w => w.ID_JUZGADO == -1).FirstOrDefault();
                    SelectJuzgado = SelectJuzgadoItem.ID_JUZGADO;
                    Credencializado = SelectPersona.ABOGADO.CREDENCIALIZADO == "S";
                    ListFechasGuardadas = ListFechasGuardadas ?? new ObservableCollection<string>();
                    ListFechasGuardadas = new ObservableCollection<string>(SelectPersona.ABOGADO.ACTUARIO_LISTA.Where(w => (w.CAPTURA_FEC.Value.Day >= hoy.Day && w.CAPTURA_FEC.Value.Month >= hoy.Month && w.CAPTURA_FEC.Value.Year >= hoy.Year)
                        && w.CAPTURA_FEC.HasValue).OrderBy(o => o.CAPTURA_FEC.Value).Select(s => s.CAPTURA_FEC.Value.ToString("dd/MM/yyyy")));
                    ListFechasGuardadas.Insert(0, "NUEVO");
                    SelectFechaGuardada = ListFechasGuardadas.Any() ? ListFechasGuardadas.FirstOrDefault() : "NUEVO";
                }
                if (SelectPersona.PERSONA_NIP != null ? SelectPersona.PERSONA_NIP.Count > 0 : false)
                {
                    var VisitaLegal = Parametro.ID_TIPO_VISITA_LEGAL;
                    TextNip = SelectPersona.PERSONA_NIP.Where(w => w.ID_CENTRO == GlobalVar.gCentro && w.ID_TIPO_VISITA == VisitaLegal).FirstOrDefault().NIP.HasValue ?
                        SelectPersona.PERSONA_NIP.Where(w => w.ID_CENTRO == GlobalVar.gCentro && w.ID_TIPO_VISITA == VisitaLegal).FirstOrDefault().NIP.Value.ToString() : string.Empty;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al traer datos del visitante.", ex);
            }
        }

        private void LimpiarCampos()
        {
            TextCodigo = TextPaterno = TextMaterno = TextNombre = TextCodigoAbogado = TextPaternoAbogado = TextMaternoAbogado = TextNombreAbogado =
                TextCurp = TextRfc = TextTelefonoFijo = TextTelefonoMovil = TextCorreo = TextIne = TextCedulaCJF = string.Empty;
            SelectFechaNacimiento = Fechas.GetFechaDateServer;
            SelectFechaAlta = string.Empty;//SelectFechaNacimiento.ToString("dd/MM/yyyy");
            SelectSexo = "M";
            SelectEstatusVisita = -1;
            SelectJuzgadoItem = ListJuzgado.Where(w => w.ID_JUZGADO == -1).FirstOrDefault();
            SelectJuzgado = SelectJuzgadoItem.ID_JUZGADO;
            ImagenAbogado = new Imagenes().getImagenPerson();
            SelectPersona = null;
            StaticSourcesViewModel.SourceChanged = Credencializado = false;
            HuellasCapturadas = null;
            ListPersonas = null;
            ListPersonasAuxiliar = null;
            ListInternosSeleccionadosPorNotificar = ListInternosSeleccionadosPorNotificarAuxiliar = null;
            ListInternosPorNotificar = null;
            SeleccionarTodosEnable = SeleccionarTodoInternos = BanderaSelectAll = false;
            ListFechasGuardadas = new ObservableCollection<string>();
        }

        private void CheckBoxInternoSelecccionado(object SelectedItem)
        {
            try
            {
                if (SelectedItem is InternoPorNotificar)
                {
                    var obj = (InternoPorNotificar)SelectedItem;
                    if (obj.ELEGIDO)
                    {
                        if (ListInternosSeleccionadosPorNotificar == null)
                            ListInternosSeleccionadosPorNotificar = new ObservableCollection<InternoPorNotificar>();
                        ListInternosSeleccionadosPorNotificar.Select(s => new InternoPorNotificar
                            {
                                INGRESO = s.INGRESO,
                                ELEGIDO = true
                            });
                    }
                    else
                    {
                        if (ListInternosSeleccionadosPorNotificar == null)
                            ListInternosSeleccionadosPorNotificar = new ObservableCollection<InternoPorNotificar>();
                        ListInternosSeleccionadosPorNotificar.Select(s => new InternoPorNotificar
                            {
                                INGRESO = obj.INGRESO,
                                ELEGIDO = false
                            });
                        //var o = ListInternosSeleccionadosPorNotificar.Where(w => w.ELEGIDO == obj.ELEGIDO).FirstOrDefault() ?? null;
                        //if (o != null)
                        //    ListInternosSeleccionadosPorNotificar.Remove(o);
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar", ex);
            }
            //try
            //{
            //    if ((((object[])(SelectedItem))[0]) is CheckBox)
            //    {
            //        var checkbox = (CheckBox)(((object[])(SelectedItem))[0]);
            //        if (BanderaSelectAll)
            //        {
            //            if (ListInternosSeleccionadosPorNotificar.Any())
            //                ListInternosSeleccionadosPorNotificar = new ObservableCollection<InternoPorNotificar>(ListInternosSeleccionadosPorNotificar.Select(s => new InternoPorNotificar
            //               {
            //                   INGRESO = s.INGRESO,
            //                   ELEGIDO = checkbox.IsChecked.HasValue ? checkbox.IsChecked.Value : false
            //               }));
            //            StaticSourcesViewModel.SourceChanged = false;

            //        }
            //    }
            //    if ((((object[])(SelectedItem))[0]) is InternoPorNotificar)
            //    {
            //        var causaPenal = (InternoPorNotificar)(((object[])(SelectedItem))[0]);
            //        var checkBox = (CheckBox)(((object[])(SelectedItem))[1]);
            //        causaPenal.ELEGIDO = checkBox.IsChecked.HasValue ? checkBox.IsChecked.Value : false;
            //        if (checkBox.IsChecked.HasValue)
            //        {
            //            if (SeleccionarTodoInternos)
            //            {
            //                if (!checkBox.IsChecked.Value)
            //                {
            //                    BanderaSelectAll = false;
            //                    SeleccionarTodoInternos = false;
            //                    BanderaSelectAll = true;
            //                }
            //            }
            //            else
            //                if (checkBox.IsChecked.Value)
            //                {
            //                    BanderaSelectAll = false;
            //                    SeleccionarTodoInternos = ListInternosSeleccionadosPorNotificar.Where(w => w.ELEGIDO).Count() == ListInternosSeleccionadosPorNotificar.Count;
            //                    BanderaSelectAll = true;
            //                }
            //        }
            //        StaticSourcesViewModel.SourceChanged = true;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar", ex);
            //}
        }

        private async void HeaderSort(Object obj)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    if (obj != null ? obj.ToString() == "Tipo visita" : false)
                    {
                        ListPersonas = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                        switch (HeaderSortin)
                        {
                            case true:
                                ListPersonas.InsertRange(ListPersonasAuxiliar.OrderByDescending(o => o.ABOGADO != null)
                                    .ThenByDescending(t => t.ABOGADO != null ? t.ABOGADO.ID_ABOGADO_TITULO == Parametro.ID_ABOGADO_TITULAR_ACTUARIO : t.ABOGADO != null));
                                HeaderSortin = false;
                                break;
                            case false:
                                ListPersonas.InsertRange(ListPersonasAuxiliar.OrderByDescending(o => o.ABOGADO == null));
                                HeaderSortin = true;
                                break;
                        }
                    }
                    if (obj != null ? obj.ToString() == "Nombre de Interno" : false)
                    {
                        switch (HeaderSortin)
                        {
                            case true:
                                ListInternosPorNotificar = new ObservableCollection<ACTUARIO_INGRESO>(ListInternosPorNotificar
                                    .OrderByDescending(o => o.INGRESO.IMPUTADO.PATERNO)
                                        .ThenByDescending(o => o.INGRESO.IMPUTADO.MATERNO)
                                            .ThenByDescending(o => o.INGRESO.IMPUTADO.NOMBRE));
                                HeaderSortin = false;
                                break;
                            case false:
                                ListInternosPorNotificar = new ObservableCollection<ACTUARIO_INGRESO>(ListInternosPorNotificar
                                    .OrderBy(o => o.INGRESO.IMPUTADO.PATERNO)
                                        .ThenBy(o => o.INGRESO.IMPUTADO.MATERNO)
                                            .ThenBy(o => o.INGRESO.IMPUTADO.NOMBRE));
                                HeaderSortin = true;
                                break;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ordenar la busqueda.", ex);
            }
        }

        #region HUELLAS DIGITALES

        private void ShowIdentification(object obj = null)
        {
            ShowPopUp = Visibility.Visible;
            ShowFingerPrint = Visibility.Hidden;
            var Initial442 = new Thread((Init) =>
            {
                try
                {
                    var nRet = 0;
                    CLSFPCaptureDllWrapper.CLS_Initialize();
                    CLSFPCaptureDllWrapper.CLS_SetLanguage(CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_RESOURCE.ENGLISH);
                    nRet = CLSFPCaptureDllWrapper.CLS_CaptureFP(CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_TYPE.IDFLATS);
                    if (nRet == 0)
                    {
                        ScannerMessage = "Procesando...";
                        ShowFingerPrint = Visibility.Visible;
                        ShowLine = Visibility.Visible;
                        ShowOk = Visibility.Hidden;
                        Thread.Sleep(300);
                        HuellasCapturadas = new List<PlantillaBiometrico>();
                        var SaveFingerPrints = new Thread((saver) =>
                        {
                            try
                            {
                                #region [Huellas]
                                for (short i = 1; i <= 10; i++)
                                {
                                    var pBuffer = IntPtr.Zero;
                                    var nBufferLength = 0;
                                    var nNFIQ = 0;
                                    CLSFPCaptureDllWrapper.CLS_GetImage(CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_IMPRESSION_TYPE.PLAIN, (CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i, CLSFPCaptureDllWrapper.IMG_TYPE.BMP, ref pBuffer, ref nBufferLength);
                                    var bufferBMP = new byte[nBufferLength];
                                    if (pBuffer != IntPtr.Zero)
                                        Marshal.Copy(pBuffer, bufferBMP, 0, nBufferLength);
                                    CLSFPCaptureDllWrapper.CLS_GetImage(CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_IMPRESSION_TYPE.PLAIN, (CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i, CLSFPCaptureDllWrapper.IMG_TYPE.WSQ, ref pBuffer, ref nBufferLength);
                                    var bufferWSQ = new byte[nBufferLength];
                                    if (pBuffer != IntPtr.Zero)
                                        Marshal.Copy(pBuffer, bufferWSQ, 0, nBufferLength);
                                    CLSFPCaptureDllWrapper.CLS_GetImageNFIQ(((CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i), ref nNFIQ, CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_IMPRESSION_TYPE.PLAIN);
                                    Fmd FMD = null;
                                    if (bufferBMP.Length != 0)
                                    {
                                        GuardaHuella = CreateBitmapSourceFromBitmap(new MemoryStream(bufferBMP));
                                        FMD = ExtractFmdfromBmp(new Bitmap(new MemoryStream(bufferBMP)).Clone(new Rectangle(0, 0, 357, 392), System.Drawing.Imaging.PixelFormat.Format8bppIndexed)).Data;
                                    }
                                    Thread.Sleep(200);
                                    switch ((CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i)
                                    {
                                        #region [Pulgar Derecho]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_THUMB:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        #region [Indice Derecho]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_INDEX:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        #region [Medio Derecho]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_MIDDLE:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        #region [Anular Derecho]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_RING:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        #region [Meñique Derecho]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_LITTLE:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        #region [Pulgar Izquierdo]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_THUMB:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        #region [Indice Izquierdo]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_INDEX:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        #region [Medio Izquierdo]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_MIDDLE:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        #region [Anular Izquierdo]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_RING:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        #region [Meñique Izquierdo]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_LITTLE:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        default:
                                            break;
                                    }
                                }
                                #endregion
                                ScannerMessage = "Huellas Capturadas Correctamente";
                            }
                            catch (Exception ex)
                            {
                                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al procesar huellas.", ex);
                            }
                        });
                        SaveFingerPrints.Start();
                        SaveFingerPrints.Join();
                        ShowLine = Visibility.Hidden;
                        Thread.Sleep(1500);
                    }
                    ShowPopUp = Visibility.Hidden;
                    CLSFPCaptureDllWrapper.CLS_Terminate();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.HUELLAS);
                }
                catch
                {
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ShowPopUp = Visibility.Hidden;
                        (new Dialogos()).ConfirmacionDialogo("Error", "Revise que el escanner este bien configurado.");
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.HUELLAS);
                    }));
                }
            });

            Initial442.Start();
        }
        private async void OnBuscarPorHuella(string obj = "")
        {
            try
            {
                if (!PConsultar)
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                    return;
                }
                await Task.Factory.StartNew(() => PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO));
                await TaskEx.Delay(400);
                var nRet = -1;
                var bandera = true;
                var requiereGuardarHuellas = Parametro.GuardarHuellaEnBusquedaPadronVisita;
                if (requiereGuardarHuellas)
                    try
                    {
                        nRet = CLSFPCaptureDllWrapper.CLS_Initialize();
                    }
                    catch
                    {
                        bandera = false;
                    }
                else
                    bandera = false;

                var windowBusqueda = new BusquedaHuella();
                windowBusqueda.DataContext = new BusquedaHuellaViewModel(enumTipoPersona.PERSONA_ABOGADO, nRet == 0, requiereGuardarHuellas);

                if (nRet != 0 ? ((ControlPenales.Clases.FingerPrintScanner)(windowBusqueda.DataContext)).Readers.Count == 0 : false)
                {
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.HUELLAS);
                    StaticSourcesViewModel.Mensaje("ADVERTENCIA", "ASEGURESE DE CONECTAR SU LECTOR DE HUELLA DIGITAL", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION, 5);
                    return;
                }

                windowBusqueda.Owner = PopUpsViewModels.MainWindow;
                windowBusqueda.KeyDown += (s, e) => { if (e.Key == System.Windows.Input.Key.Escape)windowBusqueda.Close(); };
                windowBusqueda.Closed += async (s, e) =>
                {
                    HuellasCapturadas = ((BusquedaHuellaViewModel)windowBusqueda.DataContext).HuellasCapturadas;
                    if (bandera == true)
                        CLSFPCaptureDllWrapper.CLS_Terminate();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    var huella = ((BusquedaHuellaViewModel)windowBusqueda.DataContext);
                    if (!huella.IsSucceed)
                        return;
                    if (huella.ScannerMessage == "HUELLA NO ENCONTRADA")
                    {
                        var persn = SelectPersona;
                        ListPersonasAuxiliar = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                        ListPersonas = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                        LimpiarBusqueda();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                        SelectPersonaAuxiliar = persn;
                        return;
                    }
                    if (huella.SelectRegistro != null ? huella.SelectRegistro.Persona == null : null == null)
                        return;
                    if (StaticSourcesViewModel.SourceChanged)
                    {
                        if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                            "Existen cambios sin guardar, esta seguro que desea seleccionar a otra persona?") != 1)
                            return;
                    }
                    SelectPersona = huella.SelectRegistro.Persona;
                    if (SelectPersona.ABOGADO != null)
                    {
                        if (SelectPersona.ABOGADO.ID_ABOGADO_TITULO == Parametro.ID_ABOGADO_TITULAR_ACTUARIO)
                        {
                            GetDatosPersonaSeleccionada();
                            StaticSourcesViewModel.SourceChanged = false;
                            LimpiarBusqueda();
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                        }
                        else
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "La persona seleccionada no es un actuario.");
                    }
                    else
                    {
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "La persona seleccionada no es un actuario.");
                    }

                };
                windowBusqueda.ShowDialog();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar por huella.", ex);
            }
        }

        #endregion

        private void LimpiarBusqueda()
        {
            TextPaterno = TextMaterno = TextNombre = string.Empty;
            ListPersonas = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
            EmptyBuscarRelacionInternoVisible = false;
            ImagenPersona = new Imagenes().getImagenPerson();
        }
    }
}