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
using ControlPenales.BiometricoServiceReference;
using Cogent.Biometrics;
using TXTextControl;
using Microsoft.Reporting.WinForms;
using Novacode;



namespace ControlPenales
{
    partial class AtencionCitaViewModel : ValidationViewModelBase
    {
        #region Constructor
        public AtencionCitaViewModel() { }
        public AtencionCitaViewModel(ATENCION_CITA ac) {
            SelectedAtencionCita = ac;
        }
        #endregion

        #region Funciones Eventos
        private async void clickSwitch(Object obj)
        {
            try
            {
                switch (obj.ToString())
                {
                    case "regresar":
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new AtencionCitaListaView();
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new AtencionCitaListaViewModel();
                        break;
                    case "ver_historico":
                        if (!PConsultar)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                            break;
                        }
                        VerHistorico();
                        break;
                    case "guardar_menu":
                        if (!PInsertar)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                            break;
                        }
                        Guardar();
                        break;
                    case "limpiar_menu":
                        StaticSourcesViewModel.SourceChanged = false;
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new AtencionCitaView();
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new AtencionCitaViewModel(SelectedAtencionCita);
                        break;
                    case "salir_menu":
                        PrincipalViewModel.SalirMenu();
                        break;

                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionaropción", ex);
            }
        }

        private async void WindowLoad(AtencionCitaView obj)
        {
            try
            {
                if (obj != null)
                {
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(ObtenerIngreso);
                    Reporte = obj.Report;

                    //obj.editor.Loaded += (s, e) => { };
                    //Editor = obj.editor;
                    //GenerarWordAtencionesRecibidas();
                    //CargarPlantilla();
                    CargarHistorialAtencionesRecibidas();
                    SetValidaciones();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar pantalla.", ex);
            }
        }
        #endregion

        #region Listado
        private void ObtenerIngreso() {
            try 
            {
                ConfiguraPermisos();

                if (SelectedAtencionCita != null)
                {
                    if (SelectedAtencionCita.INGRESO != null)
                    {
                        Anio = SelectedAtencionCita.INGRESO.ID_ANIO;
                        Folio = SelectedAtencionCita.INGRESO.ID_IMPUTADO;
                        Paterno = SelectedAtencionCita.INGRESO.IMPUTADO.PATERNO;
                        Materno = SelectedAtencionCita.INGRESO.IMPUTADO.MATERNO;
                        Nombre = SelectedAtencionCita.INGRESO.IMPUTADO.NOMBRE;
                        if (SelectedAtencionCita.INGRESO.INGRESO_BIOMETRICO != null)
                        { 
                            var obj = SelectedAtencionCita.INGRESO.INGRESO_BIOMETRICO.Where((w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG)).FirstOrDefault();
                            if (obj != null)
                                ImagenIngreso = obj.BIOMETRICO;
                        }
                        
                        //Obtener Historico
                        //LstAtencionRecibidaHistorico = new ObservableCollection<ATENCION_RECIBIDA>(new cAtencionRecibida().ObtenerTodoHistorico(SelectedAtencionCita.INGRESO.ID_CENTRO, SelectedAtencionCita.INGRESO.ID_ANIO, SelectedAtencionCita.INGRESO.ID_IMPUTADO));
                        //HistoricoVisible = LstAtencionRecibidaHistorico.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                    }
                    //CargarPlantilla();
                    //GenerarReporte();
                }
            }
            catch(Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener ingreso.", ex);
            }
        }
        #endregion

        #region Huellas
        private async void OnBuscarPorHuella(string obj = "")
        {
            await Task.Factory.StartNew(() => PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO));

            await TaskEx.Delay(400);

            var nRet = -1;
            var bandera = true;
            var requiereGuardarHuellas = Parametro.GuardarHuellaEnBusquedaEstatusAdministrativo;
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
            windowBusqueda.DataContext = new BusquedaHuellaViewModel(enumTipoPersona.IMPUTADO, nRet == 0, requiereGuardarHuellas);

            if (nRet != 0 ? ((ControlPenales.Clases.FingerPrintScanner)(windowBusqueda.DataContext)).Readers.Count == 0 : false)
            {
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.HUELLAS);
                StaticSourcesViewModel.Mensaje("ADVERTENCIA", "ASEGURESE DE CONECTAR SU LECTOR DE HUELLA DIGITAL", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION, 5);
                return;
            }

            windowBusqueda.Owner = PopUpsViewModels.MainWindow;
            windowBusqueda.KeyDown += (s, e) =>
            {
                try
                {
                    if (e.Key == System.Windows.Input.Key.Escape) windowBusqueda.Close();
                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar", ex);
                }
            };
            windowBusqueda.Closed += (s, e) =>
            {
                try
                {
                    HuellasCapturadas = ((BusquedaHuellaViewModel)windowBusqueda.DataContext).HuellasCapturadas;
                    if (bandera == true)
                        CLSFPCaptureDllWrapper.CLS_Terminate();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);

                    if (!((BusquedaHuellaViewModel)windowBusqueda.DataContext).IsSucceed)
                        return;

                    Imputado = ((BusquedaHuellaViewModel)windowBusqueda.DataContext).SelectRegistro != null ? ((BusquedaHuellaViewModel)windowBusqueda.DataContext).SelectRegistro.Imputado : null;

                    if (Imputado == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de autentificar al interno por medio de huella");
                        return;
                    }
                    else
                    { 
                        if(SelectedAtencionCita != null)
                        {
                            if (SelectedAtencionCita.INGRESO != null)
                            {
                                if (Imputado.ID_CENTRO == SelectedAtencionCita.INGRESO.ID_CENTRO && Imputado.ID_ANIO == SelectedAtencionCita.INGRESO.ID_ANIO && Imputado.ID_IMPUTADO == SelectedAtencionCita.ID_IMPUTADO)
                                {
                                    TabControlEnabled = MenuGuardarEnabled = true;
                                    BHuellasEnabled = false;
                                }
                                else
                                    new Dialogos().ConfirmacionDialogo("Validación", "El interno no coincide con el solicitado");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cerrar busqueda", ex);
                }
            };
            windowBusqueda.ShowDialog();
            //AceptarBusquedaHuellaFocus = true;
        }
        #endregion

        #region Atencion Recibida
        private void Guardar() 
        {
            try
            {
                if (SelectedAtencionCita != null)
                {
                    var obj = new ATENCION_RECIBIDA();
                    obj.ID_CENTRO_UBI = GlobalVar.gCentro;
                    obj.ID_CITA = SelectedAtencionCita.ID_CITA;
                    obj.ID_USUARIO = GlobalVar.gUsr;
                    obj.ATENCION_FEC = Fechas.GetFechaDateServer;
                    //byte[] data;
                    //Editor.Save(out data, BinaryStreamType.WordprocessingML);
                    //obj.ATENCION_RECIBIDA1 = data;
                    obj.ATENCION_RECIBIDA_TXT = AtencionTxt;

                    if (SelectedAtencionRecibida == null)
                    {
                        if (!PInsertar)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                            return;
                        }
                        if (new cAtencionRecibida().Agregar(obj))
                        {
                            SelectedAtencionRecibida = obj;
                            MenuGuardarEnabled = false;
                            #region Historico
                            LstAtencionRecibidaHistorico = new ObservableCollection<ATENCION_RECIBIDA>(new cAtencionRecibida().ObtenerTodoHistorico(SelectedAtencionCita.INGRESO.ID_CENTRO, SelectedAtencionCita.INGRESO.ID_ANIO, SelectedAtencionCita.INGRESO.ID_IMPUTADO));
                            HistoricoVisible = LstAtencionRecibidaHistorico.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                            #endregion
                            new Dialogos().ConfirmacionDialogo("Éxito", "La información se ha guardado correctamente");
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Error", "Ocurrio unj error al guardar la información");
                    }
                    else
                    {
                        if (!PEditar)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                            return;
                        }
                        if (new cAtencionRecibida().Actualizar(obj))
                        {
                            SelectedAtencionRecibida = obj;
                            MenuGuardarEnabled = false;
                            #region Historico
                            LstAtencionRecibidaHistorico = new ObservableCollection<ATENCION_RECIBIDA>(new cAtencionRecibida().ObtenerTodoHistorico(SelectedAtencionCita.INGRESO.ID_CENTRO, SelectedAtencionCita.INGRESO.ID_ANIO, SelectedAtencionCita.INGRESO.ID_IMPUTADO));
                            HistoricoVisible = LstAtencionRecibidaHistorico.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                            #endregion
                            new Dialogos().ConfirmacionDialogo("Éxito", "La información se ha guardado correctamente");
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Error", "Ocurrio unj error al guardar la información");
                    }
                }
                else
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar una cita (Regrese al listado de citas por atender.)");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar", ex);
            }
        }
        #endregion

        #region Historico
        private void VerHistorico()
        {
            try
            {
                if (SelectedAtencionRecibidaHistorico != null)
                {
                    var tc = new TextControlView();
                    tc.Closed += (s, e) =>
                    {
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    };
                    tc.editor.Loaded += (s, e) =>
                    {
                       //DOCX
                        tc.editor.Load(SelectedAtencionRecibidaHistorico.ATENCION_RECIBIDA1, TXTextControl.BinaryStreamType.WordprocessingML);
                    };
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    tc.Owner = PopUpsViewModels.MainWindow;
                    tc.Show();
                }
                else
                    new Dialogos().ConfirmacionDialogo("Validación","Favor de seleccionar la atencion recibida a visualizar");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar información historica.", ex);
            }
        }

        private void CargarHistorialAtencionesRecibidas() 
        {
            try
            {
                if (SelectedAtencionCita != null)
                {
                    var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();
                    var reporte = new List<cReporte>();
                    reporte.Add(new cReporte()
                    {
                        Encabezado1 = Parametro.ENCABEZADO1,
                        Encabezado2 = Parametro.ENCABEZADO2,
                        Encabezado3 = centro.DESCR.Trim(),
                        Encabezado4 = "Historial de Atención de Citas del Área " + SelectedAtencionCita.ATENCION_SOLICITUD.AREA_TECNICA.DESCR.Trim(),
                        Logo1 = Parametro.REPORTE_LOGO1,
                        Logo2 = Parametro.REPORTE_LOGO2

                    });

                    var generales = new List<cHistorialAtencionCitasGenerales>();
                    if (SelectedAtencionCita.INGRESO != null)
                    {
                        generales.Add(new cHistorialAtencionCitasGenerales()
                        {
                            Expediente = string.Format("{0}/{1}", SelectedAtencionCita.INGRESO.ID_ANIO, SelectedAtencionCita.INGRESO.ID_IMPUTADO),
                            Nombre = string.Format("{0} {1} {2}", 
                            SelectedAtencionCita.INGRESO.IMPUTADO.NOMBRE.Trim(),
                            !string.IsNullOrEmpty(SelectedAtencionCita.INGRESO.IMPUTADO.PATERNO) ? SelectedAtencionCita.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty,
                            !string.IsNullOrEmpty(SelectedAtencionCita.INGRESO.IMPUTADO.MATERNO) ? SelectedAtencionCita.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty),
                        });
                    }

                    var historial = new List<cHistorialAtencionCitas>();
                    var historico = new cAtencionRecibida().ObtenerTodoHistorico(
                        SelectedAtencionCita.INGRESO.ID_CENTRO, 
                        SelectedAtencionCita.INGRESO.ID_ANIO,
                        SelectedAtencionCita.INGRESO.ID_IMPUTADO, 
                        SelectedAtencionCita.ATENCION_SOLICITUD.ID_TECNICA).OrderBy(w => w.ATENCION_FEC);
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
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar la cita a atender");

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar el ingreso.", ex);
            }
        }
        //private void GenerarWordAtencionesRecibidas() 
        //{
        //    try
        //    {
        //        MemoryStream stream = new MemoryStream();
        //        using (DocX document = DocX.Create(stream))
        //        {
        //            #region Configuracion del Documento
        //            document.MarginLeft = 40;
        //            document.MarginRight = 40;
        //            #endregion

        //            #region Header
        //            document.AddHeaders();
        //            document.AddFooters();
        //            // Get the default Header for this document.
        //            Header header_default = document.Headers.odd;
        //            // Insert a Paragraph into the default Header.
        //            var headLineFormat = new Formatting();
        //            headLineFormat.FontFamily = new System.Drawing.FontFamily("Arial Black");

        //            Novacode.Paragraph p1 = header_default.InsertParagraph();
        //            p1.Alignment = Alignment.right;
        //            p1.Append("Secretaria de Seguridad Publica").Bold();
        //            #endregion

        //            #region Body
        //            var historico = new cAtencionRecibida().ObtenerTodoHistorico(SelectedAtencionCita.INGRESO.ID_CENTRO, SelectedAtencionCita.INGRESO.ID_ANIO, SelectedAtencionCita.INGRESO.ID_IMPUTADO).OrderBy(w => w.ATENCION_FEC);
        //            if (historico != null)
        //            {
        //                Novacode.Paragraph pb = document.InsertParagraph();
        //                pb.AppendLine();
        //                Novacode.Table t = document.AddTable(2, 1);
        //                float[] x = { 350, 350 };
        //                t.SetWidths(x);
        //                t.Alignment = Alignment.center;
        //                t.Design = TableDesign.TableNormal;
        //                int i = 0;

        //                foreach (var h in historico)
        //                {
        //                        t.Rows[i].Cells[0].Paragraphs.First().AppendLine(h.ATENCION_FEC.Value.ToString("dd/MM/yyyy")).Bold();
        //                        t.Rows[i].Cells[0].TextDirection = TextDirection.right;
        //                        i++;
        //                        t.InsertRow();
        //                        t.Rows[i].Cells[0].Paragraphs.First().AppendLine(h.ATENCION_RECIBIDA_TXT);
        //                        i++;
        //                        t.InsertRow();
        //                }
        //                document.InsertTable(t);
        //            }
        //            #endregion

        //            #region Footer

        //            Footer footer_default = document.Footers.odd;
        //            Novacode.Paragraph p7 = footer_default.InsertParagraph();
        //            //p7.Append("pie de pagina").Bold();
        //            //p6.AppendPageCount(PageNumberFormat.normal);
        //            //p6.AppendPageNumber(PageNumberFormat.normal);
        //            //p6.Append().Bold();
        //            #endregion
        //            document.Save();

        //            byte[] bytes = stream.ToArray();
        //            Editor.Load(bytes, TXTextControl.BinaryStreamType.WordprocessingML);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error generar historico de atenciones recibidas.", ex);
        //    }
        //}
        #endregion

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.ATENCION_CITA.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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

        #region Reporte
        private void CargarPlantilla() 
        {
            try
            {
                var diccionario = new Dictionary<string, string>();
                diccionario.Add("<<encabezado1>>", encabezado1);
                diccionario.Add("<<encabezado2>>", encabezado2);
                diccionario.Add("<<encabezado3>>", "Atención de Interno");
                diccionario.Add("<<expediente>>",string.Format("{0}/{1}", SelectedAtencionCita.ID_ANIO,selectedAtencionCita.ID_IMPUTADO));
                var imputado = SelectedAtencionCita.INGRESO.IMPUTADO;
                diccionario.Add("<<nombre>>",string.Format("{0} {1} {2}",imputado.NOMBRE.Trim(),!string.IsNullOrEmpty(imputado.PATERNO) ? imputado.PATERNO.Trim() : string.Empty,!string.IsNullOrEmpty(imputado.MATERNO) ? imputado.MATERNO.Trim() : string.Empty));
                var documento = new cImputadoTipoDocumento().Obtener((short)enumTipoDocumentoImputado.ATENCION_INTERNO);
                if (documento == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "No se encontro la plantilla del documento");
                    return;
                }

                var d = new cWord().FillFieldsDocx(documento.DOCUMENTO, diccionario);
                //var ib = selectedAtencionCita.INGRESO.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).FirstOrDefault();
                //var d = new cWord().FillFields(Parametro.PLANTILLA_ATENCION_INTERNO, diccionario);//.FillFields(Parametro.PLANTILLA_ATENCION_INTERNO, diccionario, logo1, logo2, ib.BIOMETRICO);
                Editor.Load(d, BinaryStreamType.WordprocessingML);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar reporte de atención recibida", ex);
            }
        }
        #endregion
    }
}
