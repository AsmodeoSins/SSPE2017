using ControlPenales;
using ControlPenales.BiometricoServiceReference;
//using LinqKit;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace ControlPenales
{
    public partial class ReporteRelacionInternoViewModel : ValidationViewModelBase
    {
        #region Constructor
        public ReporteRelacionInternoViewModel() { }
        #endregion

        #region Metodos
        private async void SwitchClick(Object obj)
        {
            if (!pConsultar)
            {
                Repositorio.Visibility = Visibility.Collapsed;
                new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                //Repositorio.Visibility = Visibility.Visible;
                return;
            }
                //ReportViewerVisible = Visibility.Collapsed;
                //Repositorio.Visibility = Visibility.Collapsed;
                await StaticSourcesViewModel.CargarDatosMetodoAsync(GenerarReporte);
                //Repositorio.Visibility = Visibility.Visible;
                //ReportViewerVisible = Visibility.Visible;
        }

        private void OnLoad(ReporteRelacionInternoView Window = null)
        {                   
            try
            {
                ConfiguraPermisos();
                Repositorio = Window.WFH;
                Reporte = Window.Report;
                ValidarFiltros();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar pantalla", ex);
            }
        }
        #endregion

        #region Reporte
        private void GenerarReporte()
        {
            try
            {
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    Repositorio.Visibility = Visibility.Collapsed;
                }));
                var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();

                var datosReporte = new List<cReporteDatos>();
                datosReporte.Add(new cReporteDatos()
                {
                    Encabezado1 = Parametro.ENCABEZADO1.Trim(),
                    Encabezado2 = Parametro.ENCABEZADO2.Trim(),
                    Encabezado3 = centro.DESCR.Trim().ToUpper(),
                    Titulo = "RELACIÓN DE INTERNOS",
                    Logo1 = Parametro.REPORTE_LOGO1,
                    Logo2 = Parametro.REPORTE_LOGO2,
                    //Centro = centro.DESCR.Trim().ToUpper(),
                });

                var hoy = Fechas.GetFechaDateServer.Date;

                var lstInternos = new List<cReporteRelacionInternosDetalle>();
                var lista = new cIngreso().ReporteRelacionInterno(GlobalVar.gCentro, OrdenarPor);
                if (lista != null)
                {
                    foreach (var w in lista)
                    {
                        lstInternos.Add(new cReporteRelacionInternosDetalle()
                        {
                            Hoy = hoy,
                            Anio = w.ID_ANIO,
                            Imputado = w.ID_IMPUTADO,
                            Nombre = w.NOMBRE,
                            Paterno = w.PATERNO,
                            Materno = w.MATERNO,
                            FecNacimiento = IncluirEdad ? w.NACIMIENTO_FECHA : null,
                            Fuero = string.IsNullOrEmpty(w.FUERO) ? (w.FUERO == "C" ? "COMUN" : w.FUERO == "F" ? "FEDERAL" : "DESCONOCIDO") : "DESCONOCIDO",
                            Clasificacion = w.CLASIFICACION_JURIDICA,
                            Ubicacion = string.Format("{0}-{1}-{2}-{3}",
                                         !string.IsNullOrEmpty(w.EDIFICIO) ? w.EDIFICIO.Trim() : string.Empty,
                                         !string.IsNullOrEmpty(w.SECTOR) ? w.SECTOR.Trim() : string.Empty,
                                         !string.IsNullOrEmpty(w.CELDA) ? w.CELDA.Trim() : string.Empty,
                                         w.CAMA),
                            Foto = IncluirFoto ? (w.FOTO != null ? w.FOTO : new Imagenes().getImagenPerson()) : null,
                        });
                    }
                }

                #region comentado
                //var ingresos = new cIngreso().ObtenerIngresosActivos(GlobalVar.gCentro).Where(w => w.CAMA != null);//.Where(w => w.ID_UB_CENTRO > 0 && w.ID_UB_SECTOR > 0 && !string.IsNullOrEmpty(w.ID_UB_CELDA) && w.ID_UB_CAMA > 0 && w.CAMA != null);
                //if (ingresos != null)
                //{
                //    switch (OrdenarPor)
                //    {
                //        case 1://expediente
                //            ingresos = ingresos.OrderBy(w => new { w.ID_ANIO, w.ID_IMPUTADO });
                //            break;
                //        case 2://nombre
                //            ingresos = ingresos.OrderBy(w => new { w.IMPUTADO.NOMBRE, w.IMPUTADO.PATERNO, w.IMPUTADO.MATERNO });
                //            break;
                //        case 3://Fecha de ingreso
                //            ingresos = ingresos.OrderBy(w => w.FEC_INGRESO_CERESO);
                //            break;
                //        case 4://Ubicacion
                //            ingresos = ingresos.OrderBy(w => new { w.ID_UB_CENTRO, w.ID_UB_EDIFICIO, w.ID_UB_SECTOR, w.ID_UB_CELDA, w.ID_UB_CAMA });
                //            break;
                //        case 5://Autoridad que interna
                //            ingresos = ingresos.OrderBy(w => w.ID_AUTORIDAD_INTERNA);
                //            break;
                //        case 6://Clasificacion juridica
                //            ingresos = ingresos.OrderBy(w => w.ID_CLASIFICACION_JURIDICA);
                //            break;
                //    }

                    //var lstInternos = ingresos.Select(w => new cReporteRelacionInternosDetalle()
                    //{
                    //    Hoy = hoy,
                    //    Anio = w.ID_ANIO,
                    //    Imputado = w.ID_IMPUTADO,
                    //    Nombre = w.IMPUTADO.NOMBRE,
                    //    Paterno = w.IMPUTADO.PATERNO,
                    //    Materno = w.IMPUTADO.MATERNO,
                    //    FecNacimiento = IncluirEdad ? w.IMPUTADO.NACIMIENTO_FECHA : null,
                    //    CP = w.CAUSA_PENAL.FirstOrDefault(x => x.ID_ESTATUS_CP == 1),
                    //    Clasificacion = w.ID_CLASIFICACION_JURIDICA,
                    //    UEdificio = w.CAMA.CELDA.SECTOR.EDIFICIO.DESCR,
                    //    //USector = w.CAMA.CELDA.SECTOR.DESCR,
                    //    //UCelda = w.CAMA.CELDA.ID_CELDA,
                    //    //UCama = w.CAMA.ID_CAMA,
                    //    IB = IncluirFoto ? w.INGRESO_BIOMETRICO.FirstOrDefault(x => x.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && x.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG) : null,
                    //}).ToList();




                    
                    //if (ingresos != null)
                    //{
                        ////byte[] foto;
                        ////INGRESO_BIOMETRICO biometrico;
                        //foreach (var w in ingresos)
                        //{
                        //    //if(IncluirFoto)
                        //    //{
                        //    //    if (w.INGRESO_BIOMETRICO != null)
                        //    //    {
                        //    //        biometrico = w.INGRESO_BIOMETRICO.FirstOrDefault(x => x.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && x.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG);
                        //    //    }
                        //    //    else
                        //    //        biometrico = null;
                        //    //}
                        //    //else
                        //    //{
                        //    //    biometrico = null;
                        //    //}
                        //    lstInternos.Add(new cReporteRelacionInternosDetalle()
                        //    {
                        //        Hoy = hoy,
                        //        Anio = w.ID_ANIO,
                        //        Imputado = w.ID_IMPUTADO,
                        //        Nombre = w.IMPUTADO.NOMBRE,
                        //        Paterno = w.IMPUTADO.PATERNO,
                        //        Materno = w.IMPUTADO.MATERNO,
                        //        FecNacimiento = IncluirEdad ? w.IMPUTADO.NACIMIENTO_FECHA : null,
                        //        CP = w.CAUSA_PENAL.FirstOrDefault(x => x.ID_ESTATUS_CP == 1),
                        //        Clasificacion = w.ID_CLASIFICACION_JURIDICA,
                        //        //UEdificio = w.CAMA.CELDA.SECTOR.EDIFICIO.DESCR,
                        //        //USector = w.CAMA.CELDA.SECTOR.DESCR,
                        //        //UCelda = w.CAMA.CELDA.ID_CELDA,
                        //        //UCama = w.CAMA.ID_CAMA,
                        //        Ubicacion = string.Format("{0}-{1}-{2}-{3}",
                        //                     w.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim(),
                        //                     w.CAMA.CELDA.SECTOR.DESCR.Trim(),
                        //                     w.CAMA.CELDA.ID_CELDA.Trim(),
                        //                     w.CAMA.ID_CAMA),
                        //        IB = IncluirFoto ? w.INGRESO_BIOMETRICO != null ? w.INGRESO_BIOMETRICO.FirstOrDefault(x => x.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && x.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG) : null : null
                        //    });
                        //}
                //}
                #endregion

                #region Reporte
                Reporte.LocalReport.ReportPath = "Reportes/rRelacionInternosDetalle.rdlc";
                    Reporte.LocalReport.DataSources.Clear();

                    Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds1.Name = "DataSet1";
                    rds1.Value = datosReporte;
                    Reporte.LocalReport.DataSources.Add(rds1);

                    Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds2.Name = "DataSet2";
                    rds2.Value = lstInternos;
                    Reporte.LocalReport.DataSources.Add(rds2);

                    #region Parametros
                    Reporte.LocalReport.SetParameters(new Microsoft.Reporting.WinForms.ReportParameter("MostrarEdad", IncluirEdad ? "N" : "S"));
                    Reporte.LocalReport.SetParameters(new Microsoft.Reporting.WinForms.ReportParameter("MostrarFoto", IncluirFoto ? "N" : "S"));
                    #endregion
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        Reporte.RefreshReport();
                    }));
                    #endregion
                //}
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    Repositorio.Visibility = Visibility.Collapsed;
                }));
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar reporte", ex);
            }

            Application.Current.Dispatcher.Invoke((Action)(delegate
            {
                Repositorio.Visibility = Visibility.Visible;
            }));
        }
        private string SetUbicacion(object value)
        {
            if (value != null)
            {
                var cama = ((CAMA)value);
                if (cama.CELDA == null) return string.Empty;
                if (cama.CELDA.SECTOR == null) return string.Empty;
                if (cama.CELDA.SECTOR.EDIFICIO == null) return string.Empty;
                return string.Format("{0}-{1}-{2}-{3}",
                                              cama.CELDA.SECTOR.EDIFICIO.DESCR.Trim(),
                                              cama.CELDA.SECTOR.DESCR.Trim(),
                                              cama.ID_CELDA.Trim(),
                                              cama.ID_CAMA);

            }
            else
                return string.Empty;
        }


        #endregion

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_LISTADO_GENERAL.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
    }
}
