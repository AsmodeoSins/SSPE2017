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
using System.Globalization;
using System.IO;
using Novacode;
using ControlPenales.BiometricoServiceReference;

namespace ControlPenales
{
    partial class HistorialAtencionRecibidaViewModel : ValidationViewModelBase, IPageViewModel
    {
        #region constructor
        public HistorialAtencionRecibidaViewModel() { }
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

                case "buscar_seleccionar":
                    if (!pConsultar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        return;
                    }
                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validacion!", "Favor de seleccionar un ingreso");
                        return;
                    }
                    var EstatusInactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                    foreach (var item in EstatusInactivos)
                    {
                        if (SelectIngreso.ID_ESTATUS_ADMINISTRATIVO == item)
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "El ingreso seleccionado no esta activo.");
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
                            SelectIngreso.ID_IMPUTADO.ToString() + "] tiene un traslado proximo y no puede imprimirse el documento.");
                        return;
                    }
                    SelectedIngreso = SelectIngreso;
                    ReportViewerVisible = Visibility.Visible;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    CargarHistorialAtencionesRecibidas();
                    break;
                case "buscar_salir":
                    if (SelectedIngreso != null)
                    {
                        SelectIngreso = SelectedIngreso;
                    }
                    else
                        ImagenIngreso = new Imagenes().getImagenPerson();
                    ReportViewerVisible = Visibility.Visible;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;

                case "buscar_interno":
                    if (!pConsultar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        return;
                    }
                    if (base.HasErrors)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar una área técnica");
                        break;
                    }
                    FolioBuscar = null;
                    AnioBuscar = null;
                    NombreBuscar = ApellidoMaternoBuscar = ApellidoPaternoBuscar = string.Empty;
                    ListExpediente = null;
                    SelectExpediente = null;
                    ImagenIngreso = new Imagenes().getImagenPerson();
                    ReportViewerVisible = Visibility.Collapsed;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;

                case "imprimir":
                    if (!pImprimir)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        return;
                    }
                    if (SelectIngreso != null)
                        CargarHistorialAtencionesRecibidas();
                    else
                        new Dialogos().ConfirmacionDialogo("VALIDACION!", "Favor de seleccionar un ingreso");
                    break;
                case "limpiar_menu":
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new HistorialAtencionRecibidaView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new HistorialAtencionRecibidaViewModel();
                    break;
                case "salir_menu":
                    PrincipalViewModel.SalirMenu();
                    break;
            }
        }

        private async void ClickEnter(Object obj)
        {
            try
            {
                if (!pConsultar)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                    return;
                }
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
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ingresar búsqueda", ex);
            }
        }

        private async void ModelEnter(Object obj)
        {
            try
            {
                //if (obj != null)
                //{
                //    if (!obj.GetType().Name.Equals("String"))
                //    {

                //        var textbox = (TextBox)obj;
                //        switch (textbox.Name)
                //        {
                //            case "NombreBuscar":
                //                NombreBuscar = textbox.Text;
                //                NombreD = NombreBuscar;
                //                FolioBuscar = FolioD;
                //                AnioBuscar = AnioD;
                //                break;
                //            case "ApellidoPaternoBuscar":
                //                ApellidoPaternoBuscar = textbox.Text;
                //                PaternoD = ApellidoPaternoBuscar;
                //                FolioBuscar = FolioD;
                //                AnioBuscar = AnioD;
                //                break;
                //            case "ApellidoMaternoBuscar":
                //                ApellidoMaternoBuscar = textbox.Text;
                //                MaternoD = ApellidoMaternoBuscar;
                //                FolioBuscar = FolioD;
                //                AnioBuscar = AnioD;
                //                break;
                //            case "FolioBuscar":
                //                if (!string.IsNullOrEmpty(textbox.Text))
                //                    FolioBuscar = int.Parse(textbox.Text);
                //                else
                //                    FolioBuscar = null;
                //                AnioBuscar = AnioD;
                //                break;
                //            case "AnioBuscar":
                //                if (!string.IsNullOrEmpty(textbox.Text))
                //                    AnioBuscar = int.Parse(textbox.Text);
                //                else
                //                    AnioBuscar = null;
                //                FolioBuscar = FolioD;
                //                break;
                //        }
                //    }
                //}
                //ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();

                //if (string.IsNullOrEmpty(NombreD))
                //    NombreBuscar = string.Empty;
                //else
                //    NombreBuscar = NombreD;

                //if (string.IsNullOrEmpty(PaternoD))
                //    ApellidoPaternoBuscar = string.Empty;
                //else
                //    ApellidoPaternoBuscar = PaternoD;

                //if (string.IsNullOrEmpty(MaternoD))
                //    ApellidoMaternoBuscar = string.Empty;
                //else
                //    ApellidoMaternoBuscar = MaternoD;

                //if (AnioBuscar != null && FolioBuscar != null)
                //{
                //    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                //    ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                //    if (ListExpediente.Count == 1)
                //    {
                //        if (ListExpediente[0].INGRESO.Count > 0)
                //        {
                //            foreach (var item in ListExpediente[0].INGRESO)
                //            {
                //                if (item.ID_ESTATUS_ADMINISTRATIVO != Parametro.ID_ESTATUS_ADMVO_LIBERADO)
                //                {
                //                    SelectExpediente = ListExpediente[0];
                //                    SelectIngreso = item;
                //                    //TabVisible = true;
                //                    //this.SeleccionaIngreso();
                //                    //this.ViewModelArbol();
                //                    //EdificioI = SelectIngreso.ID_UB_EDIFICIO;
                //                    ObtenerIngreso();
                //                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                //                    break;
                //                }
                //                else
                //                {
                //                    SelectExpediente = null;
                //                    SelectIngreso = null;
                //                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                //                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                //                    //TabVisible = false;
                //                }
                //            }
                //        }
                //        else
                //        {
                //            SelectExpediente = null;
                //            SelectIngreso = null;
                //            ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                //            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                //            //TabVisible = false;
                //        }
                //    }
                //    else
                //    {
                //        ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                //        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                //        //TabVisible = false;
                //    }
                //}
                //else
                //{
                //    //ListExpediente = (new cImputado()).ObtenerTodos(ApellidoPaternoBuscar, ApellidoMaternoBuscar, NombreBuscar, AnioBuscar,FolioBuscar);
                //    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                //    ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                //    if (ListExpediente.Count > 0)//Empty row
                //        EmptyExpedienteVisible = false;
                //    else
                //        EmptyExpedienteVisible = true;
                //    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                //    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                //    //TabVisible = false;
                //}
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ingresar búsqueda", ex);
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
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al segmentar resultados de búsqueda", ex);
                return new List<IMPUTADO>();
            }
        }

        private void OnLoad(HistorialAtencionRecibidaView Window = null)
        {
            try
            {
                ConfiguraPermisos();
                ReporteHeight = (Window.ActualHeight / 1.5);
                Reporte = Window.Report;
                ReportViewerVisible = Visibility.Hidden;
                LstAreaTecnica = new ObservableCollection<AREA_TECNICA>(new cAreaTecnica().ObtenerTodo(string.Empty, "S").OrderBy(w => w.DESCR));
                LstAreaTecnica.Insert(0, new AREA_TECNICA() { ID_TECNICA = -1, DESCR = "SELECCIONE" });
                base.AddRule(() => AreaTecnica, () => AreaTecnica != -1, "OFICIO AUTORIZACION ES REQUERIDO!");
                RaisePropertyChanged("AreaTecnica");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la entrevista inicial", ex);
            }
        }

        #endregion

        #region Metodos Buscar
        private void LimpiarBusqueda()
        {
            AnioBuscar = FolioBuscar = null;
            NombreBuscar = ApellidoPaternoBuscar = ApellidoMaternoBuscar = string.Empty;
            ListExpediente = null;
            SelectExpediente = null;
            ImagenIngreso = new Imagenes().getImagenPerson();
        }

        private void ObtenerIngreso()
        {

        }
        #endregion

        #region Metodos Reporte
        private void CargarHistorialAtencionesRecibidas()
        {
            try
            {
                if (SelectIngreso != null)
                {
                    var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();
                    var reporte = new List<cReporte>();
                    reporte.Add(new cReporte()
                    {
                        Encabezado1 = Parametro.ENCABEZADO1,
                        Encabezado2 = Parametro.ENCABEZADO2,
                        Encabezado3 = centro.DESCR.Trim(),
                        Encabezado4 = "Historial de Atención de Citas del Área " + SelectedAreaTecnica.DESCR.Trim(),
                        Logo1 = Parametro.REPORTE_LOGO1,
                        Logo2 = Parametro.REPORTE_LOGO2

                    });

                    var generales = new List<cHistorialAtencionCitasGenerales>();
                    
                        generales.Add(new cHistorialAtencionCitasGenerales()
                        {
                            Expediente = string.Format("{0}/{1}", SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO),
                            Nombre = string.Format("{0} {1} {2}",
                            SelectIngreso.IMPUTADO.NOMBRE.Trim(),
                            !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty,
                            !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty),
                        });
                    
                    var historial = new List<cHistorialAtencionCitas>();
                    var historico = new cAtencionRecibida().ObtenerTodoHistorico(SelectIngreso.ID_CENTRO, SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO,AreaTecnica).OrderBy(w => w.ATENCION_FEC);
                    if (historico != null)
                    {
                        foreach (var h in historico)
                        {
                            historial.Add(new cHistorialAtencionCitas() { Fecha = h.ATENCION_FEC.Value.ToString("dd/MM/yyyy"), Atencion = h.ATENCION_RECIBIDA_TXT });
                        }
                    }

                    Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds1.Name = "DataSet1";
                    rds1.Value = historial;
                    Reporte.LocalReport.DataSources.Add(rds1);

                    Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds2.Name = "DataSet2";
                    rds2.Value = reporte;
                    Reporte.LocalReport.DataSources.Add(rds2);

                    Microsoft.Reporting.WinForms.ReportDataSource rds3 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds3.Name = "DataSet3";
                    rds3.Value = generales;
                    Reporte.LocalReport.DataSources.Add(rds3);

                    Reporte.LocalReport.ReportPath = "Reportes/rAtencionCitaHistorial.rdlc";
                    Reporte.RefreshReport();
                }
                else
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar a un interno");

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar el ingreso.", ex);
            }
        }
        #endregion

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.HISTORIAL_CITAS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                foreach (var p in permisos)
                {
                    if (p.INSERTAR == 1)
                        pInsertar = true;
                    if (p.EDITAR == 1)
                        pEditar = true;
                    if (p.CONSULTAR == 1)
                        pConsultar = true;
                    if (p.IMPRIMIR == 1)
                        pImprimir = true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
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
                        if (SelectExpediente != null && (SelectExpediente.INGRESO == null || SelectExpediente.INGRESO.Count == 0))
                        {
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {
                                selectExpediente = new cImputado().Obtener(selectExpediente.ID_IMPUTADO, selectExpediente.ID_ANIO, selectExpediente.ID_CENTRO).First();
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
                }
            }
        }
        #endregion
    }
}
