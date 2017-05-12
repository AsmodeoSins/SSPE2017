using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WPFPdfViewer;

namespace ControlPenales
{
    partial class ConsultaUnificadaAdminViewModel : ValidationViewModelBase
    {
        #region Constructor
        public ConsultaUnificadaAdminViewModel() { }
        #endregion

        #region Metodos
        private async void ClickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "buscar":
                    Buscar();
                    break;
                case "menu_editar":
                    if (SelectedItem != null)
                    {
                        Nuevo = false;
                        HeaderAgregar = "Editar Documento para Consulta Unificada";
                        EditarVisible = true;
                        NuevoVisible = false;
                        GuardarMenuEnabled = true;
                        AgregarMenuEnabled = false;
                        EliminarMenuEnabled = false;
                        EditarMenuEnabled = false;
                        CancelarMenuEnabled = true;
                        AyudaMenuEnabled = true;
                        SalirMenuEnabled = true;
                        ExportarMenuEnabled = false;
                        AgregarVisible = true;
                        bandera_editar = true;
                        FocusText = true;
                        #region Obtener Valores
                        setValidationRules();
                        GetConsultaUnificada();
                        StaticSourcesViewModel.SourceChanged = false;
                        SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == SelectedItem.ESTATUS).SingleOrDefault();
                        #endregion
                    }
                    else
                    {
                        bandera_editar = false;
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar una opción.");
                    }
                    break;
                case "menu_agregar":
                    Nuevo = true;
                    HeaderAgregar = "Agregar Documento para Consulta Unificada";
                    EditarVisible = false;
                    NuevoVisible = true;
                    GuardarMenuEnabled = true;
                    AgregarMenuEnabled = false;
                    EliminarMenuEnabled = false;
                    EditarMenuEnabled = false;
                    CancelarMenuEnabled = true;
                    AyudaMenuEnabled = true;
                    SalirMenuEnabled = true;
                    ExportarMenuEnabled = false;
                    AgregarVisible = true;
                    bandera_editar = false;
                    FocusText = true;
                    /********************************/
                    setValidationRules();
                    Limpiar();
                    StaticSourcesViewModel.SourceChanged = false;
                    /********************************/
                    break;
                case "menu_guardar":
                    //if (!string.IsNullOrEmpty(Descripcion))
                    //{
                    if (base.HasErrors)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar los campos obligatorios. " + base.Error);
                        break;
                    }
                    EditarVisible = false;
                    NuevoVisible = false;
                    GuardarMenuEnabled = false;
                    AgregarMenuEnabled = true;
                    EliminarMenuEnabled = false;
                    EditarMenuEnabled = false;
                    CancelarMenuEnabled = false;
                    AyudaMenuEnabled = true;
                    SalirMenuEnabled = true;
                    ExportarMenuEnabled = true;
                    AgregarVisible = false;
                    /**********************************/
                    if (Guardar())
                    {
                        Buscar();
                        base.ClearRules();
                        StaticSourcesViewModel.SourceChanged = false;
                    }
                    /**********************************/
                    //}
                    //else
                    //    FocusText = true;
                    break;
                case "menu_cancelar":
                    NuevoVisible = false;
                    GuardarMenuEnabled = false;
                    AgregarMenuEnabled = true;
                    EliminarMenuEnabled = false;
                    EditarMenuEnabled = false;
                    CancelarMenuEnabled = false;
                    AyudaMenuEnabled = true;
                    SalirMenuEnabled = true;
                    ExportarMenuEnabled = true;
                    AgregarVisible = false;
                    /****************************************/
                    Limpiar();
                    base.ClearRules();
                    StaticSourcesViewModel.SourceChanged = false;
                    /****************************************/
                    break;
                case "menu_eliminar":
                    //var metro = Application.Current.Windows[0] as MetroWindow;
                    //if (SelectedItem != null)
                    //{
                    //    var mySettings = new MetroDialogSettings()
                    //    {
                    //        AffirmativeButtonText = "Aceptar",
                    //        NegativeButtonText = "Cancelar",
                    //        AnimateShow = true,
                    //        AnimateHide = false
                    //    };
                    //    //var result = await metro.ShowMessageAsync("Borrar", "¿Está seguro que desea borrar esto? [ " + SelectedItem.POBLACION.Trim() + " ]", MessageDialogStyle.AffirmativeAndNegative, mySettings);
                    //    //if (result == MessageDialogResult.Affirmative)
                    //    //{
                    //    //    EliminarSectorClasificacion();
                    //    //    var dialog = (BaseMetroDialog)metro.Resources["ConfirmacionDialog"];
                    //    //    await metro.ShowMetroDialogAsync(dialog);
                    //    //    await TaskEx.Delay(1500);
                    //    //    await metro.HideMetroDialogAsync(dialog);
                    //    //}
                    //}
                    //else
                    //    await metro.ShowMessageAsync("Validación", "Debe seleccionar una opción");
                    //SeleccionIndice = -1;
                    break;
                case "menu_exportar":
                    break;
                case "menu_ayuda":
                    break;
                case "menu_salir":
                    PrincipalViewModel.SalirMenu();
                    break;
                case "documento":
                    if (!CuTipo)
                        ElegirArchivoGuardar();
                    else
                    {
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.DIGITALIZAR_DOCUMENTO);
                    }
                    break;
                case "Cancelar_digitalizar_documentos":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.DIGITALIZAR_DOCUMENTO);
                    //escaner.Hide();
                    //DocumentoDigitalizado = null;
                    break;
                case "guardar_documento":
                    GuardarDocumento();
                    break;
            }
        }

        private void ClickEnter(Object obj)
        {
            if (obj != null)
            {
                Busqueda = ((System.Windows.Controls.TextBox)(obj)).Text;
                Buscar();
            }
        }

        private async void PageLoad(ConsultaUnificadaAdminView Window = null)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    LstClasificacion = new System.Collections.Generic.List<CLASIFICACION_DOCUMENTO>(new cClasificacionDocumento().ObtenerTodos(string.Empty));
                    ListItems = new ObservableCollection<CONSULTA_UNIFICADA>(new cConsultaUnificada().ObtenerTodos(string.Empty, string.Empty));
                    ListTipoDocumento = new ObservableCollection<TipoDocumento>();
                    ListTipoDocumento.Add(new TipoDocumento() { ID_TIPO_DOCUMENTO = 0, DESCR = "DOCUMENTO PDF" });

                    Lista_Sources = escaner.Sources();
                    if (Lista_Sources.Count > 0) SelectedSource = Lista_Sources.Where(w => w.Default).SingleOrDefault();
                    HojasMaximo = string.Format("Escaneo permitido: {0} documentos máximo.", escaner.HojasMaximo);

                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstClasificacion.Insert(0, new CLASIFICACION_DOCUMENTO() { ID_CLASIFICACION = -1, DESCR = "SELECCIONE", });
                    }));
                    //HeaderAgregar = "Agregar Nueva Clasificacion";
                    //LLENAR 
                    EditarVisible = false;
                    NuevoVisible = false;
                    AgregarVisible = false;
                    GuardarMenuEnabled = false;
                    EliminarMenuEnabled = false;
                    EditarMenuEnabled = false;
                    CancelarMenuEnabled = false;
                    AyudaMenuEnabled = true;
                    SalirMenuEnabled = true;
                    ExportarMenuEnabled = true;
                    EmptyVisible = false;
                    ConfiguraPermisos();
                });

                escaner.EscaneoCompletado += delegate 
                {
                    DatePickCapturaDocumento = Fechas.GetFechaDateServer;
                    //ObservacionDocumento = string.Empty;
                    DocumentoDigitalizado = escaner.ScannedDocument;
                    if (AutoGuardado)
                        if (DocumentoDigitalizado != null)
                            GuardarDocumento();

                    escaner.Dispose();
                };
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos.", ex);
            }
        }
        #endregion

        #region [PERMISOS]
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CAT_CONSULTA_UNIFICADA.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                foreach (var p in permisos)
                {
                    if (p.INSERTAR == 1)
                        AgregarMenuEnabled = true;
                    if (p.CONSULTAR == 1)
                    {
                        BuscarHabilitado = true;
                        TextoHabilitado = true;
                    }
                    if (p.EDITAR == 1)
                        EditarEnabled = true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }
        #endregion

        #region Scanner
        private async void Scan(PdfViewer obj)
        {
            try
            {
                await Task.Factory.StartNew(async () =>
                {
                    await escaner.Scann(Duplex, SelectedSource, obj);
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al escanear el documento.", ex);
            }
        }

        private async void AbrirDocumento(PdfViewer obj)
        {
            try
            {
                if (SelectedTipoDocumento == null)
                {
                    escaner.Hide();
                    await new Dialogos().ConfirmacionDialogoReturn("Digitalización", "Elija El Tipo De Documento A Digitalizar");
                    return;
                }
                escaner.Show();

                if (CuDocumentoScanner == null)
                {
                    StaticSourcesViewModel.Mensaje("Digitalización", "Documento No Digitalizado", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION);
                    obj.Visibility = Visibility.Collapsed;
                    DocumentoDigitalizado = null;
                    return;
                }
                DocumentoDigitalizado = CuDocumentoScanner;
                ObservacionDocumento = string.Empty;
                DatePickCapturaDocumento = null;

                if (DocumentoDigitalizado == null)
                    return;
                await Task.Factory.StartNew(() =>
                {
                    var fileNamepdf = Path.GetTempPath() + Path.GetRandomFileName().Split('.')[0] + ".pdf";
                    File.WriteAllBytes(fileNamepdf, DocumentoDigitalizado);
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        obj.LoadFile(fileNamepdf);
                        obj.Visibility = Visibility.Visible;
                    }));
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al abrir documento.", ex);
            }
        }

        private void GuardarDocumento()
        {
            try
            {
                Application.Current.Dispatcher.Invoke((System.Action)(async delegate
                {
                    escaner.Hide();
                    if (SelectedTipoDocumento == null)
                    {
                        await new Dialogos().ConfirmacionDialogoReturn("Digitalización", "Elija el tipo de documento a digitalizar");
                        return;
                    }
                    if (DocumentoDigitalizado == null)
                    {
                        await new Dialogos().ConfirmacionDialogoReturn("Digitalización", "Digitalice un documento para guardar");
                        return;
                    }
                    if (DocumentoDigitalizado.Length <= 0)
                    {
                        await new Dialogos().ConfirmacionDialogoReturn("Digitalización", "Digitalice un documento para guardar");
                        return;
                    }
                    var hoy = Fechas.GetFechaDateServer;
                    try
                    {
                        CuDocumentoScanner = DocumentoDigitalizado;
                        StaticSourcesViewModel.Mensaje("Digitalización", "Documento digitalizado exitosamente", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                        ListTipoDocumento[0].DIGITALIZADO = true;
                        ListTipoDocumento = new ObservableCollection<TipoDocumento>(ListTipoDocumento);
                        #region ABOGADO
                        //await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        //{
                        //var Documentos = new cAbogadoDocumento().ObtenerTodos(/*4*/GlobalVar.gCentro, 3, SelectPersona.ID_PERSONA, 0).Where(w =>
                        //    w.ID_TIPO_DOCUMENTO == SelectedTipoDocumento.ID_TIPO_DOCUMENTO && w.ID_TIPO_VISITA == SelectedTipoDocumento.ID_TIPO_VISITA).FirstOrDefault();

                        //if (Documentos == null)
                        //    if (new cAbogadoDocumento().Insertar(new ABOGADO_DOCUMENTO
                        //    {
                        //        ID_ABOGADO = SelectPersona.ID_PERSONA,
                        //        DOCUMENTO = DocumentoDigitalizado,
                        //        ID_CENTRO = GlobalVar.gCentro,//4,
                        //        ID_TIPO_DOCUMENTO = SelectedTipoDocumento.ID_TIPO_DOCUMENTO,
                        //        ID_TIPO_VISITA = SelectedTipoDocumento.ID_TIPO_VISITA,
                        //        OBSERVACIONES = ObservacionDocumento,
                        //        ALTA_FEC = hoy
                        //    }))
                        //    {
                        //        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                        //        {
                        //            StaticSourcesViewModel.Mensaje("Digitalización", "Documento guardado exitosamente", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                        //        }));
                        //    }
                        //    else
                        //    {
                        //        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                        //        {
                        //            StaticSourcesViewModel.Mensaje("Digitalización", "Ocurrió un error al grabar el documento", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                        //        }));
                        //    }
                        //else
                        //    if (new cAbogadoDocumento().Actualizar(new ABOGADO_DOCUMENTO
                        //    {
                        //        ID_ABOGADO = SelectPersona.ID_PERSONA,
                        //        DOCUMENTO = DocumentoDigitalizado,
                        //        ID_CENTRO = GlobalVar.gCentro,//4,
                        //        ID_TIPO_DOCUMENTO = SelectedTipoDocumento.ID_TIPO_DOCUMENTO,
                        //        ID_TIPO_VISITA = SelectedTipoDocumento.ID_TIPO_VISITA,
                        //        OBSERVACIONES = ObservacionDocumento,
                        //        ALTA_FEC = hoy
                        //    }))
                        //    {
                        //        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                        //        {
                        //            StaticSourcesViewModel.Mensaje("Digitalización", "Documento actualizado exitosamente", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                        //        }));
                        //    }
                        //    else
                        //    {
                        //        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                        //        {
                        //            StaticSourcesViewModel.Mensaje("Digitalización", "Ocurrió un error al actualizar el documento", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                        //        }));
                        //    }
                        //CargarListaTipoDocumentoDigitalizado();
                        //});
                        #endregion

                        if (AutoGuardado)
                            escaner.Show();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al digitalizar documento.", ex);
                    }
                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al digitalizar documento.", ex);
            }
        }
        #endregion

        #region Documentos
        private void ElegirArchivoGuardar()
        {
            var op = new System.Windows.Forms.OpenFileDialog();
            op.Title = "Seleccione un  documento";
            op.Filter = "Archivos PDF (*.pdf)|*.pdf";
            if (op.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (new System.IO.FileInfo(op.FileName).Length > 5000000)
                    StaticSourcesViewModel.Mensaje("Archivo no soportado", "El archivo debe ser de menos de 5 Mb", StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA, 5);
                else
                {
                    CuDocumentoArchivo = System.IO.File.ReadAllBytes(op.FileName);
                }
            }
        }
        #endregion

        #region Consulta Unificada
        private void GetConsultaUnificada()
        {
            try
            {
                if (SelectedItem != null)
                {
                    CuClasificacion = SelectedItem.ID_CLASIFICACION;
                    CuNombre = SelectedItem.NOMBRE;
                    CuEstatus = SelectedItem.ESTATUS;
                    CuTipo = SelectedItem.TIPO == 0 ? false : true;
                    if (CuTipo)
                    {
                        CuDocumentoScanner = SelectedItem.DOCUMENTO;
                        ListTipoDocumento = new ObservableCollection<TipoDocumento>();
                        ListTipoDocumento.Add(new TipoDocumento() { ID_TIPO_DOCUMENTO = 0, DESCR = "DOCUMENTO PDF", DIGITALIZADO = true });
                    }
                    else
                        CuDocumentoArchivo = SelectedItem.DOCUMENTO;
                }
                else
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un documento.");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar información.", ex);
            }
        }

        private bool Guardar()
        {
            try
            {
                var obj = new CONSULTA_UNIFICADA();
                obj.ID_CLASIFICACION = CuClasificacion;
                obj.NOMBRE = CuNombre;
                obj.ESTATUS = CuEstatus;
                obj.TIPO = CuTipo ? 1 : 0;
                if (obj.TIPO == 0)
                    obj.DOCUMENTO = CuDocumentoArchivo;
                else
                    obj.DOCUMENTO = CuDocumentoScanner;
                if (SelectedItem == null)//insert
                {
                    obj.REGISTRO_FEC = Fechas.GetFechaDateServer;
                    obj.ID_CONSULTA = new cConsultaUnificada().Insertar(obj);
                    if (obj.ID_CONSULTA > 0)
                    {
                        new Dialogos().ConfirmacionDialogo("Éxito", "La información se guardo correctamente");
                        return true;
                    }
                }
                else//update
                {
                    obj.REGISTRO_FEC = SelectedItem.REGISTRO_FEC;
                    obj.ID_CONSULTA = SelectedItem.ID_CONSULTA;
                    if (new cConsultaUnificada().Actualizar(obj))
                    {
                        new Dialogos().ConfirmacionDialogo("Éxito", "La información se guardo correctamente");
                        return true;
                    }
                }

                new Dialogos().ConfirmacionDialogo("Error", "Ocurrió un error al guardar la información");
                return false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar información.", ex);
                return false;
            }
        }

        private void Buscar()
        {
            try
            {
                ListItems = new ObservableCollection<CONSULTA_UNIFICADA>(new cConsultaUnificada().ObtenerTodos(Busqueda, string.Empty));
                EmptyVisible = ListItems.Count > 0 ? false : true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al consultar información.", ex);
            }
        }

        private void Limpiar()
        {
            try
            {
                SelectedItem = null;
                CuClasificacion = -1;
                CuNombre = string.Empty;
                CuEstatus = "S";
                CuDocumentoArchivo = CuDocumentoScanner = null;
                CuTipo = false;
                ListTipoDocumento = new ObservableCollection<TipoDocumento>();
                ListTipoDocumento.Add(new TipoDocumento() { ID_TIPO_DOCUMENTO = 0, DESCR = "DOCUMENTO PDF" });
                DocumentoDigitalizado = null;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar.", ex);
            }
        }
        #endregion
    }
}