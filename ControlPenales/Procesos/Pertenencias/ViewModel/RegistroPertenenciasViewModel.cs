using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ControlPenales
{
    partial class RegistroPertenenciasViewModel : ValidationViewModelBase
    {
        #region constructor
        public RegistroPertenenciasViewModel() { }
        #endregion

        #region [metodos]

        private async void clickSwitch(object obj)
        {
            try
            {
                switch (obj.ToString())
                {
                    case "guardar_menu":
                        Guardar();
                        //GuardarPertenencia();
                        break;
                    case "buscar_menu":
                        EnterExpediente(obj);
                        break;
                    case "limpiar_menu":
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new RegistroPertenenciasView();
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.RegistroPertenenciasViewModel();
                        break;
                    case "reporte_menu":
                        break;
                    case "ficha_menu":
                        break;
                    case "ayuda_menu":
                        break;
                    case "salir_menu":
                        PrincipalViewModel.SalirMenu();
                        break;
                    case "cancelar_tomar_foto_senas":
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.FOTOSSENIASPARTICULAES);
                        if (CamaraWeb != null)
                        {
                            await CamaraWeb.ReleaseVideoDevice();
                            CamaraWeb = null;
                        }
                        break;
                    case "aceptar_tomar_foto_senas":
                        ImagenObjeto = ImagesToSave.Any() ? ImagesToSave.FirstOrDefault().ImageCaptured : new Imagenes().ConvertByteToBitmap(new Imagenes().getImagenObjetos());
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.FOTOSSENIASPARTICULAES);
                        if (CamaraWeb != null)
                        {
                            await CamaraWeb.ReleaseVideoDevice();
                            CamaraWeb = null;
                        }
                        break;
                    case "guardar_objeto":
                        
                        if (SelectedClasificacionObjeto == -1)
                        {
                            await new Dialogos().ConfirmacionDialogoReturn("Registro Pertenencia", "Debe seleccionar una clasificación");
                            break;
                        }
                        if (string.IsNullOrEmpty(DescrObjeto))
                        {
                            await new Dialogos().ConfirmacionDialogoReturn("Registro Pertenencia", "Debe ingresar una descripción");
                            break;
                        }
                        if (ImagesToSave == null)
                        {
                            await new Dialogos().ConfirmacionDialogoReturn("Registro Pertenencia", "Debe de tomar una foto al objeto");
                            break;
                        }
                        if (ImagesToSave.Count <= 0)
                        {
                            await new Dialogos().ConfirmacionDialogoReturn("Registro Pertenencia", "Debe de tomar una foto al objeto");
                            break;
                        }

                        if (ImagesToSave.Any(x => x.ImageCaptured == null))
                        {
                            await new Dialogos().ConfirmacionDialogoReturn("Registro Pertenencia", "Debe de tomar una foto al objeto");
                            break; 
                        }

                        if (!PInsertar && !PEditar)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                            return;
                        }

                        var encoder = new JpegBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(ImagesToSave.FirstOrDefault().ImageCaptured));
                        encoder.QualityLevel = 100;
                        var bit = new byte[0];
                        using (MemoryStream stream = new MemoryStream())
                        {
                            encoder.Frames.Add(BitmapFrame.Create(ImagesToSave.FirstOrDefault().ImageCaptured));
                            encoder.Save(stream);
                            bit = stream.ToArray();
                            stream.Close();
                        }

                        var entity = new INGRESO_PERTENENCIA_DET()
                        {
                            DESCR = DescrObjeto,
                            ID_ANIO = SelectImputadoIngreso.ID_ANIO,
                            ID_CENTRO = SelectImputadoIngreso.ID_CENTRO,
                            ID_CONSEC = 0,
                            ID_IMPUTADO = SelectImputadoIngreso.ID_IMPUTADO,
                            ID_INGRESO = SelectImputadoIngreso.ID_INGRESO,
                            ID_OBJETO_TIPO = SelectedClasificacionObjeto,
                            IMAGEN = bit
                        };

                        if (new cIngresoPertenenciasDet().Insertar(entity))
                        {
                            StaticSourcesViewModel.Mensaje("Registro Pertenencia", "Pertenencia Registrada con Éxito", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                            SelectedClasificacionObjeto = -1;
                            DescrObjeto = string.Empty;
                            ImagesToSave = null;
                            ImagenObjeto = new Imagenes().ConvertByteToBitmap(new Imagenes().getImagenObjetos());
                            ListObjetoImputado = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<INGRESO_PERTENENCIA_DET>>(() => new ObservableCollection<INGRESO_PERTENENCIA_DET>(new cIngresoPertenenciasDet().GetData().Where(w => w.ID_CENTRO == SelectImputadoIngreso.ID_CENTRO && w.ID_ANIO == SelectImputadoIngreso.ID_ANIO && w.ID_IMPUTADO == SelectImputadoIngreso.ID_IMPUTADO && w.ID_INGRESO == SelectImputadoIngreso.ID_INGRESO).ToList()));
                        }
                        else
                            StaticSourcesViewModel.Mensaje("Registro Pertenencia", "Ocurrió un Problema al Registrar la Pertenencia", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                        
                        break;
                    case "Ingresa_Pertenencia":
                        if (!FechaIngresoResponsable.HasValue)
                        {
                            await new Dialogos().ConfirmacionDialogoReturn("Validación", "Debe de seleccionar una fecha");
                            break;
                        }

                        if (string.IsNullOrEmpty(TextIngresoPersonasAutorizadas))
                        {
                            await new Dialogos().ConfirmacionDialogoReturn("Validación", "Debe de ingresar a las personas autorizadas");
                            break;
                        }

                        if (ListObjetoImputado == null || ListObjetoImputado.Count == 0)
                        {
                            await new Dialogos().ConfirmacionDialogoReturn("Validación", "Favor de agregar pertenencia");
                            break;
                        }

                        if (!PInsertar && !PEditar)//Sin privilegios de editar o insertar pertenencias
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                            return;
                        }

                        var IngresoPertenencia = new cIngresoPertenencia().Obtener(SelectImputadoIngreso.ID_CENTRO, SelectImputadoIngreso.ID_ANIO, SelectImputadoIngreso.ID_IMPUTADO, SelectImputadoIngreso.ID_INGRESO).FirstOrDefault();
                        if (IngresoPertenencia != null)
                        {
                            if (IngresoPertenencia.ING_FECHA != null)
                            {
                                if (await new Dialogos().ConfirmarEliminar("Validación", "Ya hay un ingreso de pertenencias registrado, ¿Quiere actualizar la información?") != 1)
                                    break;
                            }

                            if (new cIngresoPertenencia().Actualizar(new INGRESO_PERTENENCIA()
                            {
                                ID_CENTRO = SelectImputadoIngreso.ID_CENTRO,
                                ID_ANIO = SelectImputadoIngreso.ID_ANIO,
                                ID_IMPUTADO = SelectImputadoIngreso.ID_IMPUTADO,
                                ID_INGRESO = SelectImputadoIngreso.ID_INGRESO,
                                ING_DOCUMENTO = null,
                                ING_FECHA = FechaIngresoResponsable,
                                ING_PER_AUTORIZADAS = TextIngresoPersonasAutorizadas,
                                ING_RESPONSABLE = StaticSourcesViewModel.UsuarioLogin.Username,

                                EGR_DOCUMENTO = IngresoPertenencia.EGR_DOCUMENTO,
                                EGR_FECHA = IngresoPertenencia.EGR_FECHA,
                                EGR_PER_AUTORIZADAS = IngresoPertenencia.EGR_PER_AUTORIZADAS,
                                EGR_RESPONSABLE = IngresoPertenencia.EGR_RESPONSABLE
                            }))
                            {
                                //StaticSourcesViewModel.Mensaje("Registro Pertenencia", "Ingreso de Pertenencias Registrado con Éxito", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                                StaticSourcesViewModel.Mensaje("Éxito", "Las pertenencias fueron ingresadas correctamente", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                                ImprimirReportePertenencia(1);
                            }
                            else
                                //StaticSourcesViewModel.Mensaje("Registro Pertenencia", "Ocurrió un Problema al Ingresar Pertenencias", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                                StaticSourcesViewModel.Mensaje("Error", "Ocurrió un problema al ingresar pertenencias", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                        }
                        else
                            if (new cIngresoPertenencia().Insertar(new INGRESO_PERTENENCIA()
                                {
                                    ID_CENTRO = SelectImputadoIngreso.ID_CENTRO,
                                    ID_ANIO = SelectImputadoIngreso.ID_ANIO,
                                    ID_IMPUTADO = SelectImputadoIngreso.ID_IMPUTADO,
                                    ID_INGRESO = SelectImputadoIngreso.ID_INGRESO,
                                    ING_DOCUMENTO = null,
                                    ING_FECHA = FechaIngresoResponsable,
                                    ING_PER_AUTORIZADAS = TextIngresoPersonasAutorizadas,
                                    ING_RESPONSABLE = StaticSourcesViewModel.UsuarioLogin.Username,

                                    EGR_DOCUMENTO = null,
                                    EGR_FECHA = null,
                                    EGR_PER_AUTORIZADAS = null,
                                    EGR_RESPONSABLE = null
                                }))
                            {
                                //StaticSourcesViewModel.Mensaje("Registro Pertenencia", "Ingreso de Pertenencias Registrado con Éxito", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                                StaticSourcesViewModel.Mensaje("Éxito", "Las pertenencias fueron ingresadas correctamente", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                                ImprimirReportePertenencia(1);
                            }
                            else
                                //StaticSourcesViewModel.Mensaje("Registro Pertenencia", "Ocurrió un Problema al Ingresar Pertenencias", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                                StaticSourcesViewModel.Mensaje("Error", "Ocurrió un problema al ingresar pertenencias", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                        break;
                    case "Egresar_Pertenencia":
                        if (!FechaEgresoResponsable.HasValue)
                        {
                            await new Dialogos().ConfirmacionDialogoReturn("Validación", "Debe de seleccionar una fecha");
                            break;
                        }

                        if (string.IsNullOrEmpty(TextEgresoPersonasAutorizadas))
                        {
                            await new Dialogos().ConfirmacionDialogoReturn("Validación", "Debe de ingresar a las personas autorizadas");
                            break;
                        }

                        if (ListObjetoImputado == null || ListObjetoImputado.Count == 0)
                        {
                            await new Dialogos().ConfirmacionDialogoReturn("Validación", "Favor de agregar pertenencia");
                            break;
                        }
                        if (!PEditar)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                            return;
                        }
                        var EgresoPertenencia = new cIngresoPertenencia().Obtener(SelectImputadoIngreso.ID_CENTRO, SelectImputadoIngreso.ID_ANIO, SelectImputadoIngreso.ID_IMPUTADO, SelectImputadoIngreso.ID_INGRESO).FirstOrDefault();
                        if (EgresoPertenencia != null)
                        {
                            if (EgresoPertenencia.EGR_FECHA != null)
                            {
                                if (await new Dialogos().ConfirmarEliminar("Validación", "Ya hay un egreso de pertenencias registrado, ¿Quiere actualizar la información?") != 1)
                                    break;
                            }

                            if (new cIngresoPertenencia().Actualizar(new INGRESO_PERTENENCIA()
                            {
                                ID_CENTRO = SelectImputadoIngreso.ID_CENTRO,
                                ID_ANIO = SelectImputadoIngreso.ID_ANIO,
                                ID_IMPUTADO = SelectImputadoIngreso.ID_IMPUTADO,
                                ID_INGRESO = SelectImputadoIngreso.ID_INGRESO,
                                EGR_DOCUMENTO = null,
                                EGR_FECHA = FechaEgresoResponsable,
                                EGR_PER_AUTORIZADAS = TextEgresoPersonasAutorizadas,
                                EGR_RESPONSABLE = StaticSourcesViewModel.UsuarioLogin.Username,

                                ING_DOCUMENTO = EgresoPertenencia.ING_DOCUMENTO,
                                ING_FECHA = EgresoPertenencia.ING_FECHA,
                                ING_PER_AUTORIZADAS = EgresoPertenencia.ING_PER_AUTORIZADAS,
                                ING_RESPONSABLE = EgresoPertenencia.ING_RESPONSABLE
                            }))
                            {
                                //StaticSourcesViewModel.Mensaje("Registro Pertenencia", "Egreso de Pertenencias Registrado con Exito", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                                StaticSourcesViewModel.Mensaje("Éxito", "Las pertenencias fueron ingresadas correctamente", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                                ImprimirReportePertenencia(2);
                            }
                            else
                                //StaticSourcesViewModel.Mensaje("Registro Pertenencia", "Ocurrio un Problema al Egresar Pertenencias", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                                StaticSourcesViewModel.Mensaje("Error", "Ocurrió un problema al ingresar pertenencias", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                        }
                        else
                            await new Dialogos().ConfirmacionDialogoReturn("Validación", "Debe de tener un ingreso de pertenencias");
                        break;
                    case "buscar_seleccionar":
                        if (SelectExpediente == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Debes seleccionar un imputado.");
                            return;
                        }
                        if (SelectIngreso == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Debes seleccionar un ingreso valido.");
                            return;
                        }
                        var EstatusInactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                        foreach (var item in EstatusInactivos)
                        {
                            if (SelectIngreso.ID_ESTATUS_ADMINISTRATIVO == item)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no esta activo.");
                                return;
                            }
                        }
                        if (SelectIngreso.ID_UB_CENTRO != GlobalVar.gCentro)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                            return;
                        }
                        var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                        if (SelectIngreso.TRASLADO_DETALLE.Any(a => (a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado) <= Fechas.GetFechaDateServer))
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + SelectIngreso.ID_ANIO.ToString() + "/" +
                                SelectIngreso.ID_IMPUTADO.ToString() + "] tiene un traslado próximo y no puede recibir visitas.");
                            return;
                        }
                        ApellidoPaternoBuscar = SelectExpediente.PATERNO;
                        ApellidoMaternoBuscar = SelectExpediente.MATERNO;
                        NombreBuscar = SelectExpediente.NOMBRE;
                        AnioD = SelectExpediente.ID_ANIO.ToString();
                        FolioD = SelectExpediente.ID_IMPUTADO.ToString();
                        AnioBuscar = SelectExpediente.ID_ANIO;
                        FolioBuscar = SelectExpediente.ID_IMPUTADO;
                        SelectImputadoIngreso = SelectIngreso;
                        GetDatosIngresoImputadoSeleccionado();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        break;
                    case "buscar_salir":
                        AnioD = string.Empty;
                        FolioD = string.Empty;
                        AnioBuscar = null;
                        FolioBuscar = null;
                        NombreBuscar = string.Empty;
                        ApellidoPaternoBuscar = string.Empty;
                        ApellidoMaternoBuscar = string.Empty;
                        ImagenIngreso = new Imagenes().getImagenPerson();
                        ImagenImputado = new Imagenes().getImagenPerson();
                        ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();

                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        break;
                    case "nueva_busqueda":
                        AnioD = string.Empty;
                        FolioD = string.Empty;
                        AnioBuscar = null;
                        FolioBuscar = null;
                        NombreBuscar = string.Empty;
                        ApellidoPaternoBuscar = string.Empty;
                        ApellidoMaternoBuscar = string.Empty;
                        ImagenIngreso = new Imagenes().getImagenPerson();
                        ImagenImputado = new Imagenes().getImagenPerson();
                        ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el flujo del modulo", ex);
            }
        }

        private void EnterExpediente(object obj)
        {
            try
            {
                if (obj != null)
                {
                    if (!PConsultar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        return;
                    }

                    if (obj.ToString() != "buscar_visible" && obj.ToString() != "buscar_menu")
                    {
                        //cuando es boton no se hace nada porque solamente existe el de buscar, si hay otro habra que castearlos a button y hacer la comparacion
                        var textbox = obj as TextBox;
                        if (textbox != null)
                        {
                            switch (textbox.Name)
                            {
                                case "NombreBuscar":
                                    NombreBuscar = NombreD = textbox.Text;
                                    AnioBuscar = !string.IsNullOrEmpty(AnioD) ? int.Parse(AnioD) : new Nullable<int>();
                                    FolioBuscar = !string.IsNullOrEmpty(FolioD) ? int.Parse(FolioD) : new Nullable<int>();
                                    ApellidoMaternoBuscar = MaternoD;
                                    ApellidoPaternoBuscar = PaternoD;
                                    break;
                                case "ApellidoPaternoBuscar":
                                    ApellidoPaternoBuscar = PaternoD = textbox.Text;
                                    AnioBuscar = !string.IsNullOrEmpty(AnioD) ? int.Parse(AnioD) : new Nullable<int>();
                                    FolioBuscar = !string.IsNullOrEmpty(FolioD) ? int.Parse(FolioD) : new Nullable<int>();
                                    ApellidoMaternoBuscar = MaternoD;
                                    NombreBuscar = NombreD;
                                    break;
                                case "ApellidoMaternoBuscar":
                                    ApellidoMaternoBuscar = MaternoD = textbox.Text;
                                    AnioBuscar = !string.IsNullOrEmpty(AnioD) ? int.Parse(AnioD) : new Nullable<int>();
                                    FolioBuscar = !string.IsNullOrEmpty(FolioD) ? int.Parse(FolioD) : new Nullable<int>();
                                    ApellidoPaternoBuscar = PaternoD;
                                    NombreBuscar = NombreD;
                                    break;
                                case "FolioBuscar":
                                    FolioBuscar = !string.IsNullOrEmpty(textbox.Text) ? int.Parse(textbox.Text) : new Nullable<int>();
                                    AnioBuscar = !string.IsNullOrEmpty(AnioD) ? int.Parse(AnioD) : new Nullable<int>();
                                    ApellidoMaternoBuscar = MaternoD;
                                    NombreBuscar = NombreD;
                                    ApellidoPaternoBuscar = PaternoD;
                                    break;
                                case "AnioBuscar":
                                    AnioBuscar = !string.IsNullOrEmpty(textbox.Text) ? int.Parse(textbox.Text) : new Nullable<int>();
                                    FolioBuscar = !string.IsNullOrEmpty(FolioD) ? int.Parse(FolioD) : new Nullable<int>();
                                    ApellidoMaternoBuscar = MaternoD;
                                    NombreBuscar = NombreD;
                                    ApellidoPaternoBuscar = PaternoD;
                                    break;
                            }
                        }

                    }
                    else
                    {
                        NombreBuscar = NombreD;
                        ApellidoPaternoBuscar = PaternoD;
                        ApellidoMaternoBuscar = MaternoD;
                    }

                    BuscarImputado(NombreD, PaternoD, MaternoD);
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la busqueda de imputados.", ex);
            }
        }

        private async void BuscarImputado(string nombre, string paterno, string materno)
        {
            try
            {
                ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                if (ListExpediente.Count > 0)
                {
                    SelectExpediente = ListExpediente.OrderBy(o => o.ID_ANIO).ThenBy(t => t.ID_IMPUTADO).FirstOrDefault();
                }
                if (ListExpediente.Count == 1)
                {
                    var EstatusInactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                    foreach (var item in EstatusInactivos)
                    {
                        if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_ESTATUS_ADMINISTRATIVO == item)
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "El ingreso seleccionado no esta activo.");
                            return;
                        }
                    }
                    if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_UB_CENTRO != GlobalVar.gCentro)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                        return;
                    }
                    var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                    if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().TRASLADO_DETALLE.Any(a => (a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado) <= Fechas.GetFechaDateServer))
                    {
                        new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + SelectIngreso.ID_ANIO.ToString() + "/" +
                            SelectIngreso.ID_IMPUTADO.ToString() + "] tiene un traslado proximo y no puede recibir visitas.");
                        return;
                    }
                    SelectImputadoIngreso = ListExpediente[0].INGRESO.Where(w => w.ID_ESTATUS_ADMINISTRATIVO != Parametro.ID_ESTATUS_ADMVO_LIBERADO).FirstOrDefault();
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() => GetDatosIngresoImputadoSeleccionado());
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    return;
                }
                else if (ListExpediente.Count == 0)
                {
                    if (!string.IsNullOrEmpty(NombreD) || !string.IsNullOrEmpty(PaternoD) || !string.IsNullOrEmpty(MaternoD))
                        new Dialogos().ConfirmacionDialogo("Notificacion!", "No se encontro ningun imputado con esos datos.");
                }

                if (ListExpediente.Count > 0)//Empty row
                    EmptyExpedienteVisible = false;
                else
                    EmptyExpedienteVisible = true;
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al hacer la busqueda.", ex);
            }
        }

        private void GetDatosIngresoImputadoSeleccionado()
        {
            try
            {
                if (SelectImputadoIngreso != null)
                {
                    AnioD = SelectImputadoIngreso.ID_ANIO.ToString();
                    FolioD = SelectImputadoIngreso.ID_IMPUTADO.ToString();
                    NombreD = string.IsNullOrEmpty(SelectImputadoIngreso.IMPUTADO.NOMBRE) ? string.Empty : SelectImputadoIngreso.IMPUTADO.NOMBRE.Trim();
                    PaternoD = string.IsNullOrEmpty(SelectImputadoIngreso.IMPUTADO.PATERNO) ? string.Empty : SelectImputadoIngreso.IMPUTADO.PATERNO.Trim();
                    MaternoD = string.IsNullOrEmpty(SelectImputadoIngreso.IMPUTADO.MATERNO) ? string.Empty : SelectImputadoIngreso.IMPUTADO.MATERNO.Trim();
                    IngresosD = SelectImputadoIngreso.IMPUTADO.INGRESO.Count.ToString();
                    UbicacionD = SelectImputadoIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() + "-" + SelectImputadoIngreso.CAMA.CELDA.SECTOR.DESCR.Trim() + "" +
                                 SelectImputadoIngreso.CAMA.CELDA.ID_CELDA.ToString().Trim() + "-" + SelectImputadoIngreso.CAMA.ID_CAMA;
                    TipoSeguridadD = SelectImputadoIngreso.TIPO_SEGURIDAD.DESCR;
                    FecIngresoD = SelectImputadoIngreso.FEC_INGRESO_CERESO == null ? null : SelectImputadoIngreso.FEC_INGRESO_CERESO.ToString();
                    ClasificacionJuridicaD = SelectImputadoIngreso.CLASIFICACION_JURIDICA.DESCR;
                    EstatusD = SelectImputadoIngreso.ESTATUS_ADMINISTRATIVO.DESCR;
                    ImagenIngreso = SelectImputadoIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any() ?
                        SelectImputadoIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO :
                            SelectImputadoIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any() ?
                                SelectImputadoIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO :
                                    new Imagenes().getImagenPerson();

                    SelectedClasificacionObjeto = -1;
                    DescrObjeto = string.Empty;
                    ImagesToSave = null;
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ImagenObjeto = new Imagenes().ConvertByteToBitmap(new Imagenes().getImagenObjetos());
                    }));
                    
                    //ListObjetoImputado = new ObservableCollection<INGRESO_PERTENENCIA_DET>(new cIngresoPertenenciasDet().GetData().Where(w => w.ID_CENTRO == SelectImputadoIngreso.ID_CENTRO && w.ID_ANIO == SelectImputadoIngreso.ID_ANIO && w.ID_IMPUTADO == SelectImputadoIngreso.ID_IMPUTADO && w.ID_INGRESO == SelectImputadoIngreso.ID_INGRESO).ToList());
                    //ListTipoObjeto = ListTipoObjeto ?? new ObservableCollection<OBJETO_TIPO>(new cObjetoTipo().ObtenerTodos().ToList());

                    //TextIngresoResponsable = StaticSourcesViewModel.UsuarioLogin.Username;
                    //TextEgresoResponsable = StaticSourcesViewModel.UsuarioLogin.Username;

                    #region Pertenencia
                    var pertenencias = SelectIngreso.INGRESO_PERTENENCIA;
                    if (pertenencias != null)
                    {
                        //Reponsable
                        if (!string.IsNullOrEmpty(pertenencias.ING_RESPONSABLE))
                            TextIngresoResponsable = pertenencias.ING_RESPONSABLE;//StaticSourcesViewModel.UsuarioLogin.Username;
                        else
                            TextIngresoResponsable = StaticSourcesViewModel.UsuarioLogin.Username;

                        if (!string.IsNullOrEmpty(pertenencias.EGR_RESPONSABLE))
                            TextEgresoResponsable = pertenencias.EGR_RESPONSABLE; //StaticSourcesViewModel.UsuarioLogin.Username;
                        else
                            TextEgresoResponsable = StaticSourcesViewModel.UsuarioLogin.Username;

                        TextIngresoPersonasAutorizadas = pertenencias.ING_PER_AUTORIZADAS;
                        TextEgresoPersonasAutorizadas = pertenencias.EGR_PER_AUTORIZADAS;

                        DocumentoIngreso = pertenencias.ING_DOCUMENTO;
                        DocumentoEgreso = pertenencias.EGR_DOCUMENTO;
                    }
                    else
                    {
                        TextIngresoResponsable = StaticSourcesViewModel.UsuarioLogin.Username;
                        TextEgresoResponsable = StaticSourcesViewModel.UsuarioLogin.Username;
                    }
                    ListObjetoImputado = new ObservableCollection<INGRESO_PERTENENCIA_DET>();
                    var pertenenciasDetalle = SelectIngreso.INGRESO_PERTENENCIA_DET;
                    if (pertenenciasDetalle != null)
                    {
                        ListObjetoImputado = new ObservableCollection<INGRESO_PERTENENCIA_DET>(pertenenciasDetalle);
                    }
                    #endregion

                    EnableGroupBoxes = true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos del visitante.", ex);
            }
        }

        private async Task<List<IMPUTADO>> SegmentarResultadoBusqueda(int _Pag = 1)
        {
            try
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
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al querer guardar.", ex);
                return new List<IMPUTADO>();
            }
        }

        private async void EnterBuscar(Object obj)
        {
            try
            {
                if (obj != null)
                {
                    if (obj.ToString() != "buscar_visible")
                    {
                        //cuando es boton no se hace nada porque solamente existe el de buscar, si hay otro habra que castearlos a button y hacer la comparacion
                        var textbox = obj as TextBox;
                        if (textbox != null)
                        {
                            switch (textbox.Name)
                            {
                                case "NombreBuscar":
                                    NombreD = NombreBuscar = textbox.Text;
                                    AnioD = AnioBuscar.HasValue ? AnioBuscar.ToString() : string.Empty;
                                    FolioD = FolioBuscar.HasValue ? FolioBuscar.ToString() : string.Empty;
                                    MaternoD = ApellidoMaternoBuscar;
                                    PaternoD = ApellidoPaternoBuscar;
                                    break;
                                case "ApellidoPaternoBuscar":
                                    PaternoD = ApellidoPaternoBuscar = textbox.Text;
                                    AnioD = AnioBuscar.HasValue ? AnioBuscar.ToString() : string.Empty;
                                    FolioD = FolioBuscar.HasValue ? FolioBuscar.ToString() : string.Empty;
                                    MaternoD = ApellidoMaternoBuscar;
                                    NombreD = NombreBuscar;
                                    break;
                                case "ApellidoMaternoBuscar":
                                    MaternoD = ApellidoMaternoBuscar = textbox.Text;
                                    AnioD = AnioBuscar.HasValue ? AnioBuscar.ToString() : string.Empty;
                                    FolioD = FolioBuscar.HasValue ? FolioBuscar.ToString() : string.Empty;
                                    PaternoD = ApellidoPaternoBuscar;
                                    NombreD = NombreBuscar;
                                    break;
                                case "FolioBuscar":
                                    FolioD = textbox.Text;
                                    if (!string.IsNullOrWhiteSpace(textbox.Text))
                                        FolioBuscar = int.Parse(textbox.Text);
                                    else
                                        FolioBuscar = null;
                                    AnioD = AnioBuscar.HasValue ? AnioBuscar.ToString() : string.Empty;
                                    MaternoD = ApellidoMaternoBuscar;
                                    NombreD = NombreBuscar;
                                    PaternoD = ApellidoPaternoBuscar;
                                    break;
                                case "AnioBuscar":
                                    AnioD = textbox.Text;
                                    if (!string.IsNullOrWhiteSpace(textbox.Text))
                                        AnioBuscar = int.Parse(textbox.Text);
                                    else
                                        AnioBuscar = null;
                                    FolioD = FolioBuscar.HasValue ? FolioBuscar.ToString() : string.Empty;
                                    MaternoD = ApellidoMaternoBuscar;
                                    NombreD = NombreBuscar;
                                    PaternoD = ApellidoPaternoBuscar;
                                    break;
                            }
                        }
                    }
                    else
                    {
                        NombreBuscar = NombreD;
                        ApellidoPaternoBuscar = PaternoD;
                        ApellidoMaternoBuscar = MaternoD;
                    }

                    ImagenIngreso = new Imagenes().getImagenPerson();
                    ImagenImputado = new Imagenes().getImagenPerson();
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                    ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                    EmptyExpedienteVisible = !(ListExpediente.Count > 0);
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la busqueda de imputados.", ex);
            }
        }

        private async void Load_Window(RegistroPertenenciasView Window) 
        {
            try
            {
               await StaticSourcesViewModel.CargarDatosMetodoAsync(() => { ConfiguraPermisos(); });
            }
            catch (Exception exc)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la ventana.", exc);
            }
        }

        private async void FotoObjeto(object obj)
        {
            try
            {
                var Window = PopUpsViewModels.MainWindow.FotosSenasView;
                if (Window == null)
                    return;
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.FOTOSSENIASPARTICULAES);
                if (!((System.Windows.UIElement)(Window.TomarFotoSenaParticularWindow)).IsVisible) return;
                CamaraWeb = new WebCam(new WindowInteropHelper(Application.Current.Windows[0]).Handle);
                await CamaraWeb.InitializeWebCam(new List<System.Windows.Controls.Image> { Window.ImgSenaParticular });
                ImagesToSave = new List<ImageSourceToSave>();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar popup.", ex);
            }
        }

        private void CapturarFoto(System.Windows.Controls.Image Picture)
        {
            try
            {
                if (Processing)
                    return;
                Processing = true;
                ImagesToSave = ImagesToSave ?? new List<ImageSourceToSave>();
                if (CamaraWeb.ImageControls != null && CamaraWeb.ImageControls.Where(w => w.Name == Picture.Name).Any())
                {
                    Picture.Source = CamaraWeb.TomarFoto(Picture);
                    ImagesToSave.Add(new ImageSourceToSave { FrameName = Picture.Name, ImageCaptured = (BitmapSource)Picture.Source });
                    StaticSourcesViewModel.Mensaje(Picture.Name == "ImgSenaParticular" ? "Visita" : "Camara", "Foto Capturada", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION, 1);
                }
                else
                {
                    CamaraWeb.QuitarFoto(Picture);
                    ImagesToSave.Remove(ImagesToSave.Where(wm => wm.FrameName == Picture.Name).SingleOrDefault());
                }
                Processing = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al tomar una foto.", ex);
                Processing = false;
            }
        }

        private void OpenSetting(string obj)
        {
            try
            {
                CamaraWeb.AdvanceSetting();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar configuracion de la camara.", ex);
            }
        }

        private void ImprimirReportePertenencia(int op)
        {
            try
            {
                var centro = new cCentro().Obtener(GlobalVar.gCentro).SingleOrDefault();
                var datosReporte = new List<cReporteDatos>();
                datosReporte.Add(new cReporteDatos()
                {
                    Encabezado1 = Parametro.ENCABEZADO1,
                    Encabezado2 = Parametro.ENCABEZADO2,
                    Encabezado3 = Parametro.ENCABEZADO3,
                    Titulo = op == 1 ? "REGISTRO DE INGRESO DE PERTENENCIA" : "REGISTRO DE SALIDA DE PERTENENCIA",
                    Logo1 = Parametro.REPORTE_LOGO1,
                    Logo2 = Parametro.REPORTE_LOGO2,
                    Centro = centro.DESCR.ToUpper(),
                });

                var reporte = new List<cPertenenciasReporte>();
                var fecha = DateTime.Now.ToShortDateString();
                reporte.Add(new cPertenenciasReporte()
                {
                    Titulo = op == 1 ? "REGISTRO DE INGRESO DE PERTENENCIA" : "REGISTRO DE SALIDA DE PERTENENCIA",
                    Expediente = string.Format("{0}/{1}", SelectImputadoIngreso.ID_ANIO, SelectImputadoIngreso.ID_IMPUTADO),
                    Nombre = string.Format("{0} {1} {2}", SelectImputadoIngreso.IMPUTADO.NOMBRE.Trim(), SelectImputadoIngreso.IMPUTADO.PATERNO.Trim(), SelectImputadoIngreso.IMPUTADO.MATERNO.Trim()),
                    FechaRegistro = fecha,
                    Texto1 = op == 1 ? "Se hace constar que se depositan en el area de resguardo de la institución las siguientes pertenencias:" : "Se hace constar que se entregan en el area de resguardo de esta institucion las siguientes pertenencias, en el mismo estado en que se recibieron:",
                    Texto2 = op == 1 ? "Se autoriza la entrega de estas pertenencias a las siguientes personas:" : string.Empty,
                    //Texto3 = string.Format("En Mexicali Baja California, a {0}", fecha),
                    Texto3 = string.Format("En {0} {1}, a {2}",centro.MUNICIPIO.MUNICIPIO1,centro.MUNICIPIO.ENTIDAD.DESCR,fecha),
                    PersonasAutorizadas = op == 1 ? TextIngresoPersonasAutorizadas : string.Empty,
                    Header = null,
                    Foto = ImagenIngreso
                });

                var objetos = new List<cObjetosPertenenciaReporte>();
                foreach (var o in ListObjetoImputado)
                {
                    objetos.Add(new cObjetosPertenenciaReporte()
                    {
                        Clasificacion = o.OBJETO_TIPO.DESCR,
                        Descripcion = o.DESCR,
                        Imagen = o.IMAGEN
                    });
                }

                var view = new ReportesView();
                view.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                //view.Show();

                //ARMAMOS EL REPORTE
                view.Report.LocalReport.ReportPath = "Reportes/rPertenencias.rdlc";
                view.Report.LocalReport.DataSources.Clear();

                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds1.Name = "DataSet1";
                rds1.Value = reporte;
                view.Report.LocalReport.DataSources.Add(rds1);

                Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds2.Name = "DataSet2";
                rds2.Value = objetos;
                view.Report.LocalReport.DataSources.Add(rds2);

                Microsoft.Reporting.WinForms.ReportDataSource rds3 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds3.Name = "DataSet3";
                rds3.Value = datosReporte;
                view.Report.LocalReport.DataSources.Add(rds3);

                if (op == 1)
                {
                    view.Report.LocalReport.SetParameters(new Microsoft.Reporting.WinForms.ReportParameter("NombreRecibio", TextIngresoResponsable));
                    view.Report.LocalReport.SetParameters(new Microsoft.Reporting.WinForms.ReportParameter("NombreEntrego", string.Format("{0} {1} {2}", SelectImputadoIngreso.IMPUTADO.NOMBRE.Trim(), SelectImputadoIngreso.IMPUTADO.PATERNO.Trim(), SelectImputadoIngreso.IMPUTADO.MATERNO.Trim())));
                }
                else
                {
                    view.Report.LocalReport.SetParameters(new Microsoft.Reporting.WinForms.ReportParameter("NombreRecibio", TextEgresoPersonasAutorizadas));
                    view.Report.LocalReport.SetParameters(new Microsoft.Reporting.WinForms.ReportParameter("NombreEntrego", TextEgresoResponsable));
                }
                

                view.Report.RefreshReport();

                /********************************************/
                byte[] bytes = view.Report.LocalReport.Render("WORD");
                var tc = new TextControlView();
                tc.editor.Loaded += (s, e) =>
                {
                    try
                    {
                        tc.editor.Load(bytes, TXTextControl.BinaryStreamType.MSWord);
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                    }
                };
                tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Show();
                /********************************************/
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al imprimir reporte.", ex);
            }
        }

        private async void Guardar()
        {
            try
            {
                if (SelectIngreso == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación","Favor de seleccionar un interno");
                    return;
                }
                else
                        if (SelectedClasificacionObjeto == -1)
                        {
                            await new Dialogos().ConfirmacionDialogoReturn("Validación", "Debe seleccionar una clasificación");
                            return;
                        }
                        if (string.IsNullOrEmpty(DescrObjeto))
                        {
                            await new Dialogos().ConfirmacionDialogoReturn("Validación", "Debe ingresar una descripción");
                            return;
                        }
                        if (ImagesToSave == null)
                        {
                            await new Dialogos().ConfirmacionDialogoReturn("Validación", "Debe de tomar una foto al objeto");
                            return;
                        }
                        if (ImagesToSave.Count <= 0)
                        {
                            await new Dialogos().ConfirmacionDialogoReturn("Validación", "Debe de tomar una foto al objeto");
                            return;
                        }

                        if (ImagesToSave.FirstOrDefault().ImageCaptured == null)
                        {
                            await new Dialogos().ConfirmacionDialogoReturn("Validación", "Debe de tomar una foto al objeto");
                            return;
                        }

                        if (!PInsertar && !PEditar)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                            return;
                        }

                        var encoder = new JpegBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(ImagesToSave.FirstOrDefault().ImageCaptured));
                        encoder.QualityLevel = 100;
                        var bit = new byte[0];
                        using (MemoryStream stream = new MemoryStream())
                        {
                            encoder.Frames.Add(BitmapFrame.Create(ImagesToSave.FirstOrDefault().ImageCaptured));
                            encoder.Save(stream);
                            bit = stream.ToArray();
                            stream.Close();
                        }

                        var entity = new INGRESO_PERTENENCIA_DET()
                        {
                            DESCR = DescrObjeto,
                            ID_ANIO = SelectImputadoIngreso.ID_ANIO,
                            ID_CENTRO = SelectImputadoIngreso.ID_CENTRO,
                            ID_CONSEC = 0,
                            ID_IMPUTADO = SelectImputadoIngreso.ID_IMPUTADO,
                            ID_INGRESO = SelectImputadoIngreso.ID_INGRESO,
                            ID_OBJETO_TIPO = SelectedClasificacionObjeto,
                            IMAGEN = bit
                        };

                        if (new cIngresoPertenenciasDet().Insertar(entity))
                        {
                            //StaticSourcesViewModel.Mensaje("Éxito", "Pertenencia Registrada con Éxito", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                            StaticSourcesViewModel.Mensaje("Éxito", "Las pertenencias fueron guardadas correctamente", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                            SelectedClasificacionObjeto = -1;
                            DescrObjeto = string.Empty;
                            ImagesToSave = null;
                            ImagenObjeto = new Imagenes().ConvertByteToBitmap(new Imagenes().getImagenObjetos());
                            ListObjetoImputado = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<INGRESO_PERTENENCIA_DET>>(() => new ObservableCollection<INGRESO_PERTENENCIA_DET>(new cIngresoPertenenciasDet().GetData().Where(w => w.ID_CENTRO == SelectImputadoIngreso.ID_CENTRO && w.ID_ANIO == SelectImputadoIngreso.ID_ANIO && w.ID_IMPUTADO == SelectImputadoIngreso.ID_IMPUTADO && w.ID_INGRESO == SelectImputadoIngreso.ID_INGRESO).ToList()));
                        }
                        else
                            StaticSourcesViewModel.Mensaje("Registro Pertenencia", "Ocurrió un Problema al Registrar la Pertenencia", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar pertenencias.", ex);
            }
        }
        #endregion

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {

                ListTipoObjeto = ListTipoObjeto ?? new ObservableCollection<OBJETO_TIPO>(new cObjetoTipo().ObtenerTodos(string.Empty,"S"));
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    ListTipoObjeto.Insert(0, new OBJETO_TIPO() { ID_OBJETO_TIPO = -1, DESCR = "SELECCIONE" });
                }));
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.PERTENENCIAS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
                        //if (p.IMPRIMIR == 1)
                        //    PImprimir = MenuFichaEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }
        #endregion

        #region Pertenencias
        private void GuardarPertenencia() {
            try
            {
                if (SelectIngreso == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un ingreso");
                    return;
                }
                if (!PInsertar && !PEditar)//Sin privilegios de editar o insertar pertenencias
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                    return;
                }
                if (base.HasErrors)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar los campos obligatorios");
                    return;
                }
                if (ListObjetoImputado == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar pertenencias");
                    return;
                }
                if (ListObjetoImputado.Count == 0)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar pertenencias");
                    return;
                }
                var p = new INGRESO_PERTENENCIA();
                p.ID_CENTRO = SelectIngreso.ID_CENTRO;
                p.ID_ANIO = SelectIngreso.ID_ANIO;
                p.ID_IMPUTADO = SelectIngreso.ID_IMPUTADO;
                p.ID_INGRESO = SelectIngreso.ID_INGRESO;
                p.ING_FECHA = FechaIngresoResponsable;
                p.ING_RESPONSABLE = TextIngresoResponsable;
                p.ING_PER_AUTORIZADAS = TextIngresoPersonasAutorizadas;
                p.ING_DOCUMENTO = DocumentoIngreso;
                p.EGR_FECHA = FechaEgresoResponsable;
                p.EGR_RESPONSABLE =TextEgresoResponsable; 
                p.EGR_PER_AUTORIZADAS = TextEgresoPersonasAutorizadas;
                p.EGR_DOCUMENTO = DocumentoEgreso;

                var lst = new List<INGRESO_PERTENENCIA_DET>();
                if (ListObjetoImputado != null)
                {
                    short index = 1;
                    foreach (var l in ListObjetoImputado)
                    { 
                        lst.Add(new INGRESO_PERTENENCIA_DET(){
                        ID_CENTRO = SelectIngreso.ID_CENTRO,
                        ID_ANIO = SelectIngreso.ID_ANIO,
                        ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                        ID_INGRESO = SelectIngreso.ID_INGRESO,
                        ID_CONSEC = index,
                        ID_OBJETO_TIPO = l.ID_OBJETO_TIPO,
                        IMAGEN = l.IMAGEN,
                        DESCR = l.DESCR
                        });
                        index++;
                    }
                }

                if (new cIngresoPertenencia().Insertar(p, lst))
                    new Dialogos().ConfirmacionDialogo("ÉXITO", "La información se guardo correctamente");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar el ingreso.", ex);
            }
        }
        #endregion

        #region Cambio SelectedItem de Busqueda de Expediente
        private async void OnModelChangedSwitch(object parametro)
        {
            if (parametro != null)
            {
                switch (parametro.ToString())
                {
                    case "cambio_expediente":
                        if (SelectExpedienteAsignacion != null && (SelectExpedienteAsignacion.INGRESO == null || SelectExpedienteAsignacion.INGRESO.Count == 0))
                        {
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {
                                SelectExpedienteAsignacion = new cImputado().Obtener(SelectExpedienteAsignacion.ID_IMPUTADO, SelectExpedienteAsignacion.ID_ANIO, SelectExpedienteAsignacion.ID_CENTRO).First();
                                RaisePropertyChanged("SelectExpediente");
                            });
                            //MUESTRA LOS INGRESOS
                            if (SelectExpedienteAsignacion.INGRESO != null && SelectExpedienteAsignacion.INGRESO.Count > 0)
                            {
                                EmptyIngresoVisible = false;
                                SelectIngreso = SelectExpedienteAsignacion.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
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
                }
            }
        }
        #endregion
    }
}
