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
using System.Windows.Controls;
using SSP.Servidor;
using SSP.Controlador.Catalogo.Justicia;
using ControlPenales;

namespace ControlPenales
{
    partial class TrasladoIndividualViewModel : ValidationViewModelBase, IPageViewModel
    {
        #region constructor
        public TrasladoIndividualViewModel() {}
        #endregion

        #region Generales
        void IPageViewModel.inicializa()
        { }

        private void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "nueva_busqueda":
                    LimpiarBusqueda();
                    break;
                case "buscar_menu":
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "salir_menu":
                    BuscarVisible = false;
                    break;
                case "buscar_seleccionar":
                    if (SelectIngreso != null)
                    {
                        ObtenerIngreso();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    }
                    break;
                case "ampliar_justificacion":
                    TituloHeaderExpandirDescripcion = "Justificacion";
                    TextAmpliarDescripcion = DTJustificacion;
                    MaxLengthAmpliarDescripcion = 1000;
                    Justificacion = true;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AMPLIAR_DESCRIPCION_GENERICO);
                    break;
                case "ampliar_folio":
                    TituloHeaderExpandirDescripcion = "No.Oficio de Autorizacion";
                    TextAmpliarDescripcion = DTNoOficio;
                    MaxLengthAmpliarDescripcion = 50;
                    Justificacion = false;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AMPLIAR_DESCRIPCION_GENERICO);
                    break;
                case "guardar_ampliar_descripcion":
                    if(Justificacion)
                        DTJustificacion = TextAmpliarDescripcion;
                    else
                        DTNoOficio = TextAmpliarDescripcion;
                    TextAmpliarDescripcion = string.Empty;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AMPLIAR_DESCRIPCION_GENERICO);
                    break;
                case "cancelar_ampliar_descripcion":
                    TextAmpliarDescripcion = string.Empty;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AMPLIAR_DESCRIPCION_GENERICO);
                    break;
                case "addTraslado":
                    SetValidacionesTraslados();
                    Limpiar();
                    break;
                case "editTraslado":
                    SetValidacionesTraslados();
                    Limpiar();
                    if (SelectedTraslado != null)
                        Obtener();
                    break;
                case "delTraslado":
                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("VALIDACION", "FAVOR DE SELECCIONAR UN INGRESO.");
                    }
                    else
                        Elimimar();
                    break;
                case "limpiar_menu":
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new TrasladoIndividualView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.TrasladoIndividualViewModel();
                    break;
                case "guardar_menu":
                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("VALIDACION", "FAVOR DE SELECCIONAR UN INGRESO.");
                    }
                    else
                    if (Guardar())
                    {
                        new Dialogos().ConfirmacionDialogo("EXITO", "Informaci\u00F3n registrada correctamente.");
                        ObtenerTodo();
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("ERROR", "No se registr\u00F3 la informaci\u00F3n.");
                    break;
                case "buscar_salir":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
            }
        }

        private async void ClickEnter(Object obj)
        {
            try
            {
                if (obj != null)
                {
                    //cuando es boton no se hace nada porque solamente existe el de buscar, si hay otro habra que castearlos a button y hacer la comparacion
                    var textbox = obj as TextBox;
                    if (textbox != null)
                    {
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
                }
                //TabVisible = false;
                ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                if (ListExpediente.Count > 0)//Empty row
                    EmptyExpedienteVisible = false;
                else
                    EmptyExpedienteVisible = true;
                ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ingresar busqueda", ex); 
            }
        }
        
        private async void ModelEnter(Object obj)
        {
            try
            {
                if (obj != null)
                {
                    if (!obj.GetType().Name.Equals("String"))
                    {

                        //cuando es boton no se hace nada porque solamente existe el de buscar, si hay otro habra que castearlos a button y hacer la comparacion
                        var textbox = obj as TextBox;
                        if (textbox != null)
                        {
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
                       
                    }
                }
                ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();

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
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                    ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                    if (ListExpediente.Count == 1)
                    {
                        if (ListExpediente[0].INGRESO.Count > 0)
                        {
                            foreach (var item in ListExpediente[0].INGRESO)
                            {
                                if (item.ID_ESTATUS_ADMINISTRATIVO != Parametro.ID_ESTATUS_ADMVO_LIBERADO)
                                {
                                    SelectExpediente = ListExpediente[0];
                                    SelectIngreso = item;
                                    //TabVisible = true;
                                    //this.SeleccionaIngreso();
                                    //this.ViewModelArbol();
                                    //EdificioI = SelectIngreso.ID_UB_EDIFICIO;
                                    ObtenerIngreso();
                                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                                    break;
                                }
                                else
                                {
                                    SelectExpediente = null;
                                    SelectIngreso = null;
                                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                                    //TabVisible = false;
                                }
                            }
                        }
                        else
                        {
                            SelectExpediente = null;
                            SelectIngreso = null;
                            ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                            //TabVisible = false;
                        }
                    }
                    else
                    {
                        ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        //TabVisible = false;
                    }
                }
                else
                {
                    //ListExpediente = (new cImputado()).ObtenerTodos(ApellidoPaternoBuscar, ApellidoMaternoBuscar, NombreBuscar, AnioBuscar,FolioBuscar);
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                    ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                    if (ListExpediente.Count > 0)//Empty row
                        EmptyExpedienteVisible = false;
                    else
                        EmptyExpedienteVisible = true;
                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    //TabVisible = false;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ingresar busqueda", ex);
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
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al segmentar resultados de busqueda", ex);
                return new List<IMPUTADO>();
            }
        }

        private /*async*/ void TrasladoIndivdualLoad(TrasladoIndividualView Window = null)
        {
            try
            {
                //await StaticSourcesViewModel.CargarDatosMetodoAsync(PrepararListas);
                PrepararListas();
                SetValidacionesTraslados();
                
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el traslado", ex);
            }
        }

        private void PrepararListas() 
        {
            if (LstMotivo == null)
            {
                LstMotivo = new ObservableCollection<TRASLADO_MOTIVO>(new cTrasladoMotivo().ObtenerTodos());
                LstMotivo.Insert(0, new TRASLADO_MOTIVO() { ID_MOTIVO = -1, DESCR = "SELECCIONE" });
            }
            if (LstCentro == null)
            {
                LstCentro = new ObservableCollection<CENTRO>(new cCentro().ObtenerTodos());
                LstCentro.Insert(0, new CENTRO() { ID_CENTRO = -1, DESCR = "SELECCIONE" });
            }
            if (LstAutoridadSalida == null)
            {
                LstAutoridadSalida = new ObservableCollection<TIPO_AUTORIDAD_SALIDA>(new cTipoAutoridadSalida().ObtenerTodo());
                LstAutoridadSalida.Insert(0, new TIPO_AUTORIDAD_SALIDA() { ID_AUTORIDAD_SALIDA = -1, DESCR = "SELECCIONE" });
            }
            if (LstEmpleado == null)
            {
                var lista = new ObservableCollection<EMPLEADO>(new cEmpleado().ObtenerTodos());
                LstEmpleado = new ObservableCollection<cAuxiliar>();
                foreach (var e in lista)
                {
                    LstEmpleado.Add(new cAuxiliar() { Id = e.ID_EMPLEADO, Descr = string.Format("{0} {1} {2}",!string.IsNullOrEmpty(e.PERSONA.NOMBRE) ? e.PERSONA.NOMBRE.Trim() : string.Empty, !string.IsNullOrEmpty(e.PERSONA.PATERNO) ? e.PERSONA.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(e.PERSONA.MATERNO) ? e.PERSONA.MATERNO.Trim() : string.Empty) });
                }
                LstEmpleado.Insert(0, new cAuxiliar() { Id = -1, Descr = "SELECCIONE" });
            }
            if (LstMotivoSalida == null)
            {
                LstMotivoSalida = new ObservableCollection<TRASLADO_MOTIVO_SALIDA>(new cTrasladoMotivoSalida().ObtenerTodo());
                LstMotivoSalida.Insert(0, new TRASLADO_MOTIVO_SALIDA() { ID_MOTIVO_SALIDA = -1, DESCR = "SELECCIONE" });
            }
            Limpiar();
        }
        #endregion

        #region Metodos Buscar
        private void LimpiarBusqueda() 
        { 
            AnioBuscar = FolioBuscar = FolioBuscar = null;
            NombreBuscar = ApellidoPaternoBuscar = ApellidoMaternoBuscar = string.Empty;
            ListExpediente = null;
        }
        
        private void ObtenerIngreso() 
        {
            if (SelectIngreso != null)
            {
                AnioD = SelectIngreso.ID_ANIO;
                FolioD = SelectIngreso.ID_IMPUTADO;
                PaternoD = SelectIngreso.IMPUTADO.PATERNO;
                MaternoD = SelectIngreso.IMPUTADO.MATERNO;
                NombreD = SelectIngreso.IMPUTADO.NOMBRE;
                IngresosD = SelectIngreso.ID_INGRESO;
                //NoControlD = SelectIngreso
                if (SelectIngreso.CAMA != null)
                {
                    UbicacionD = string.Format("{0}-{1}{2}-{3}",
                                               SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim(),
                                               SelectIngreso.CAMA.CELDA.SECTOR.DESCR.Trim(),
                                               SelectIngreso.CAMA.CELDA.ID_CELDA.Trim(),
                                               SelectIngreso.ID_UB_CAMA);
                }
                else
                {
                    UbicacionD = string.Empty;
                }
                TipoSeguridadD = SelectIngreso.TIPO_SEGURIDAD.DESCR;
                FecIngresoD = SelectIngreso.FEC_INGRESO_CERESO.Value;
                ClasificacionJuridicaD = SelectIngreso.CLASIFICACION_JURIDICA.DESCR;
                if (SelectIngreso.ESTATUS_ADMINISTRATIVO != null)
                    EstatusD = SelectIngreso.ESTATUS_ADMINISTRATIVO.DESCR;
                else
                    EstatusD = string.Empty;
                this.ObtenerTodo();
            }
        }
        #endregion

        #region Metodos Traslado
        private void Limpiar() 
        { 
            //Datos Traslado
            DTFecha = null;
            DTMotivo = DTCentroDestino = -1;
            DTJustificacion = DTNoOficio = string.Empty;
            DTAutorizado = -1;
            //Datos Egreso
            DEFecha = null;
            DENoOficio = string.Empty;
            DEAutoridad = DEMotivo  = - 1;
        }

        private void Obtener() {
            if (SelectedTraslado != null)
            {
                DTFecha = SelectedTraslado.TRASLADO_FEC;
                DTMotivo = SelectedTraslado.ID_MOTIVO;
                DTJustificacion = SelectedTraslado.JUSTIFICACION;
                DTCentroDestino = SelectedTraslado.CENTRO_DESTINO;
                DTNoOficio = SelectedTraslado.OFICIO_AUTORIZACION;
                ///TODO: cambios en el modelo
                //DTAutorizado = SelectedTraslado.AUTORIZA_EMPLEADO;
                //DEAutoridad = SelectedTraslado.AUTORIDAD_SALIDA;
                //DEFecha = SelectedTraslado.EGRESO_FEC;
                DENoOficio = SelectedTraslado.OFICIO_SALIDA;
                DEMotivo = SelectedTraslado.ID_MOTIVO_SALIDA;
            }
        }

        private void ObtenerTodo() {
            if (SelectIngreso != null)
                LstTraslados = new ObservableCollection<TRASLADO>(new cTraslado().ObtenerTodos(SelectIngreso.ID_CENTRO, SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO));
            else
                LstTraslados = new ObservableCollection<TRASLADO>();
        }

        private bool Guardar() {
            var obj = new TRASLADO();
            obj.ID_CENTRO = SelectIngreso.ID_CENTRO;
            obj.ID_ANIO = SelectIngreso.ID_ANIO;
            obj.ID_IMPUTADO = SelectIngreso.ID_IMPUTADO;
            obj.ID_INGRESO = SelectIngreso.ID_INGRESO;

            obj.TRASLADO_FEC = DTFecha.Value;
            obj.ID_MOTIVO = DTMotivo;
            obj.JUSTIFICACION = DTJustificacion;
            obj.CENTRO_DESTINO = DTCentroDestino;
            obj.OFICIO_AUTORIZACION = DTNoOficio;
            obj.AUTORIZA_EMPLEADO = DTAutorizado;
            
            obj.EGRESO_FEC = DEFecha;
            obj.OFICIO_SALIDA = DENoOficio;
            obj.AUTORIDAD_SALIDA = DEAutoridad;
            obj.ID_MOTIVO_SALIDA = DEMotivo;

            if (SelectedTraslado == null)//INSERT
            {
                obj.ID_TRASLADO = new cTraslado().Insertar(obj);
                if (obj.ID_TRASLADO > 0)
                {
                    base.ClearRules();
                    Limpiar();
                    return true; 
                }
            }
            else//UPDATE
            {
                obj.ID_TRASLADO = SelectedTraslado.ID_TRASLADO;
                if (new cTraslado().Actualizar(obj))
                {
                    base.ClearRules();
                    Limpiar();
                    return true; 
                }
            }


            return false;
        }

        private bool Elimimar() {
            if (SelectedTraslado != null)
            {
                var obj = new TRASLADO();
                obj.ID_CENTRO = SelectedTraslado.ID_CENTRO;
                obj.ID_ANIO = SelectedTraslado.ID_ANIO;
                obj.ID_IMPUTADO = SelectedTraslado.ID_IMPUTADO;
                obj.ID_INGRESO = SelectedTraslado.ID_INGRESO;
                obj.ID_TRASLADO = SelectedTraslado.ID_TRASLADO;
                if (new cTraslado().Eliminar(obj))
                {
                    new Dialogos().ConfirmacionDialogo("EXITO!", "El traslado ha sido eliminado.");
                    base.ClearRules();
                    Limpiar();
                    ObtenerTodo();
                    return true; 
                }
            }
            else
                new Dialogos().ConfirmacionDialogo("VALIDACION", "Favor de seleccionar un traslado.");
            return false;
        }
        #endregion
    }
}
