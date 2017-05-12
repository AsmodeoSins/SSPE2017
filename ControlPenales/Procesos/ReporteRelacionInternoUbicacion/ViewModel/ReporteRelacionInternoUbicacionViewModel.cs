using ControlPenales;
using ControlPenales.BiometricoServiceReference;
using LinqKit;
using Microsoft.Reporting.WinForms;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace ControlPenales
{
    public partial class ReporteRelacionInternoUbicacionViewModel : ValidationViewModelBase
    {
        #region Constructor
        public ReporteRelacionInternoUbicacionViewModel() { }
        #endregion

        #region Metodos
        private async void SwitchClick(Object obj)
        {
            try
            {
                if (!pConsultar)
                {
                    Repositorio.Visibility = Visibility.Collapsed;       
                    new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                    return;
                }
                await StaticSourcesViewModel.CargarDatosMetodoAsync(GenerarReporte);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar reporte", ex);
            }
        }

        private async void OnLoad(ReporteRelacionInternoUbicacionView Window = null)
        {                   
            try
            {
                ConfiguraPermisos();
                await StaticSourcesViewModel.CargarDatosMetodoAsync(PopulateFiltros);
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

        #region Filtros
        private void PopulateFiltros() 
        {
            try 
            {
                LstEdificios = new ObservableCollection<EDIFICIO>(new cEdificio().ObtenerTodos(string.Empty,0,GlobalVar.gCentro,"S"));
                LstSectores = new ObservableCollection<SECTOR>();
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    LstEdificios.Insert(0, new EDIFICIO() { ID_EDIFICIO = -1, DESCR = "TODOS" });
                    LstSectores.Insert(0, new SECTOR() { ID_SECTOR = -1, DESCR = "TODOS" });
                    FEdificio = FSector = -1;
                }));
                
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar filtros", ex);
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
                    Encabezado3 = Parametro.ENCABEZADO3.Trim(),
                    Titulo = "RELACIÓN DE INTERNOS",
                    Logo1 = Parametro.REPORTE_LOGO1,
                    Logo2 = Parametro.REPORTE_LOGO2,
                    Centro = centro.DESCR.Trim().ToUpper(),
                });

                var hoy = Fechas.GetFechaDateServer.Date;
                List<cReporteEdificio> lEdificio = new List<cReporteEdificio>();
                List<cReporteSector> lSector = new List<cReporteSector>();
                if (FEdificio == -1)
                {
                    lEdificio = new List<cReporteEdificio>(new cEdificio().ObtenerTodos(string.Empty, 0, GlobalVar.gCentro, "S").Select(w => new cReporteEdificio { Descr = w.DESCR, IdCentro = w.ID_CENTRO, IdEdificio = w.ID_EDIFICIO }));
                }
                else
                {
                    lEdificio.Add(new cReporteEdificio { Descr = SelectedEdificio.DESCR, IdCentro = SelectedEdificio.ID_CENTRO, IdEdificio = SelectedEdificio.ID_EDIFICIO });
                }

                if (FSector == -1)
                {
                    lSector = new List<cReporteSector>( new cSector().ObtenerTodos(string.Empty,null,GlobalVar.gCentro,null).Select(w => new cReporteSector() { Descr = w.DESCR, IdCentro = w.ID_CENTRO, IdEdificio = w.ID_EDIFICIO, IdSector = w.ID_SECTOR }));
                }
                else
                {
                    lSector.Add(new cReporteSector() { Descr = SelectedSector.DESCR, IdCentro = SelectedSector.ID_CENTRO, IdEdificio = SelectedSector.ID_EDIFICIO, IdSector = SelectedSector.ID_SECTOR });
                }

                var lstInternos = new List<cReporteRelacionInternosDetalle>();
                var reportes = new cIngreso().ReporteRelacionInternoUbicacion(GlobalVar.gCentro, FEdificio != -1 ? FEdificio : null, FSector != -1 ? FSector : null);
                if (reportes != null)
                {
                    foreach(var w in reportes)
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
                        IdCentro = w.ID_UB_CENTRO.Value,
                        IdEdificio = w.ID_UB_EDIFICIO.Value,
                        IdSector = w.ID_UB_SECTOR.Value,
                        UEdificio = w.EDIFICIO,
                        USector = w.SECTOR,
                        UCelda = w.ID_UB_CELDA,
                        UCama = w.ID_UB_CAMA
                    });
                }

                #region Comentado
                //var ingresos = new cIngreso().ObtenerIngresosActivos(GlobalVar.gCentro, FEdificio != -1 ? FEdificio : null, FSector != -1 ? FSector : null).Where(w => w.CAMA != null);//.Where(w => w.ID_UB_CENTRO > 0 && w.ID_UB_SECTOR > 0 && !string.IsNullOrEmpty(w.ID_UB_CELDA) && w.ID_UB_CAMA > 0);
                //if (ingresos != null)
                //{
                    
                //    foreach (var w in ingresos)
                //    {
                //        lstInternos.Add(new cReporteRelacionInternosDetalle()
                //        {
                //            Hoy = hoy,
                //            Anio = w.ID_ANIO,
                //            Imputado = w.ID_IMPUTADO,
                //            Nombre = w.IMPUTADO.NOMBRE,
                //            Paterno = w.IMPUTADO.PATERNO,
                //            Materno = w.IMPUTADO.MATERNO,
                //            FecNacimiento = IncluirEdad ? w.IMPUTADO.NACIMIENTO_FECHA : null,
                //            CP = w.CAUSA_PENAL.FirstOrDefault(x => x.ID_ESTATUS_CP == 1),
                //            Clasificacion = w.ID_CLASIFICACION_JURIDICA,
                //            Ubicacion = string.Format("{0}-{1}-{2}-{3}",
                //                            w.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim(),
                //                            w.CAMA.CELDA.SECTOR.DESCR.Trim(),
                //                            w.CAMA.CELDA.ID_CELDA.Trim(),
                //                            w.CAMA.ID_CAMA),
                //            IB = IncluirFoto ? w.INGRESO_BIOMETRICO.FirstOrDefault(x => x.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && x.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG) : null,

                //            IdCentro = w.ID_UB_CENTRO.Value,
                //            IdEdificio = w.ID_UB_EDIFICIO.Value,
                //            IdSector = w.ID_UB_SECTOR.Value
                //        });
                //    }
                    #endregion
                    
                    #region Reporte
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        Reporte.Reset();
                    }));
                    Reporte.LocalReport.ReportPath = "Reportes/rRelacionInternosUbicacion.rdlc";
                    Reporte.LocalReport.DataSources.Clear();

                    Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds1.Name = "DataSet1";
                    rds1.Value = datosReporte;
                    Reporte.LocalReport.DataSources.Add(rds1);

                    Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds2.Name = "DataSet2";
                    rds2.Value = lEdificio;
                    Reporte.LocalReport.DataSources.Add(rds2);

                    #region Parametros
                    Reporte.LocalReport.SetParameters(new Microsoft.Reporting.WinForms.ReportParameter("MostrarFoto", !IncluirFoto ? "S" : "N"));
                    #endregion

                    #region Subreporte
                    Reporte.LocalReport.SubreportProcessing += (s, e) =>
                    {

                        if (e.ReportPath.Equals("srSectorInternosUbicacion", StringComparison.InvariantCultureIgnoreCase))
                        {
                            ReportDataSource ds = new ReportDataSource("DataSet1", lSector);
                            e.DataSources.Add(ds);
                        }
                        else
                        {
                            ReportDataSource ds2 = new ReportDataSource("DataSet1", lstInternos);
                            e.DataSources.Add(ds2);
                        }
                    };
                    #endregion

                
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                   {
                       Reporte.Refresh();
                       Reporte.RefreshReport();
                   }));
                   
                    #endregion

                //}

                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    Repositorio.Visibility = Visibility.Visible;
                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar reporte", ex);
            }
        }
        #endregion

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_INTERNOS_UBICACION.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
