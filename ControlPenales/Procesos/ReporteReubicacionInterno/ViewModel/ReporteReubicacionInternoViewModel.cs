using ControlPenales;
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
    public partial class ReporteReubicacionInternoViewModel : ValidationViewModelBase
    {
        #region Constructor
        public ReporteReubicacionInternoViewModel() { }
        #endregion

        #region Metodos
        private async void SwitchClick(Object obj)
        {
            if (!pConsultar)
            {
                ReportViewerVisible = Visibility.Collapsed;
                new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                return;
            }
            if (!base.HasErrors)
                await StaticSourcesViewModel.CargarDatosMetodoAsync(GenerarReporte);
            else
            {
                ReportViewerVisible = Visibility.Collapsed;
                await new Dialogos().ConfirmacionDialogoReturn("Validación", base.Error);
                ReportViewerVisible = Visibility.Visible;
            }
        }

        private void OnLoad(ReporteReubicacionInternoView Window = null)
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
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la bitacora de recepción legal", ex);
            }
        }
        #endregion

        #region Bitacora Recepcion Legal
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
                    Titulo = "Reporte de Reubicaciones",
                    Logo1 = Parametro.REPORTE_LOGO1,
                    Logo2 = Parametro.REPORTE_LOGO2,
                    Centro = centro.DESCR.Trim().ToUpper(),
                });

                #region Reporte
                List<string> termsList = new List<string>();
                var ri = new List<cReubicacionInterno>();
                var rid = new List<cReubicacionInternoDetalle>();
                #endregion

                var lstIngreso = new List<cReubicacionInterno>();
                var lstReubicacion = new cIngresoUbicacionAnterior().ObtenerTodos(GlobalVar.gCentro, FFechaInicio.Value.Date, FFechaFin.Value.Date).Where(w => w.ID_UB_CENTRO == GlobalVar.gCentro);

                if (lstReubicacion != null)
                {
                    lstIngreso = new List<cReubicacionInterno>(lstReubicacion.Select(w => new cReubicacionInterno()
                    {
                        Centro = w.ID_CENTRO,
                        Anio = w.ID_ANIO,
                        Imputado = w.ID_IMPUTADO,
                        IdIngreso = w.ID_INGRESO,
                        Ingreso = w,
                    }));
                }
                foreach (var item in lstIngreso)
                {
                    var consulta_interno = item.Ingreso.INGRESO;
                    var edificio = consulta_interno.CAMA.CELDA.SECTOR.EDIFICIO;
                    var sector = consulta_interno.CAMA.CELDA.SECTOR;
                    var celda = consulta_interno.CAMA.CELDA;
                    var cama = consulta_interno.CAMA;
                    item.Expediente = string.Format("{0}/{1}", item.Ingreso.ID_ANIO, item.Ingreso.ID_IMPUTADO);
                    item.Nombre = item.Ingreso.INGRESO.IMPUTADO.NOMBRE.Trim() + " " + (string.IsNullOrEmpty(item.Ingreso.INGRESO.IMPUTADO.PATERNO) ? string.Empty : item.Ingreso.INGRESO.IMPUTADO.PATERNO.Trim()) + " " + (string.IsNullOrEmpty(item.Ingreso.INGRESO.IMPUTADO.MATERNO) ? string.Empty : item.Ingreso.INGRESO.IMPUTADO.MATERNO.Trim());
                    item.UbicacionActual = string.Format("{0}-{1}-{2}-{3}",
                        edificio != null ? edificio.DESCR.TrimEnd() : string.Empty,
                        sector != null ? sector.DESCR.TrimEnd() : string.Empty,
                        celda != null ? celda.ID_CELDA.TrimEnd() : string.Empty,
                        cama != null ? cama.ID_CAMA : 0);
                }
                lstIngreso = new List<cReubicacionInterno>(lstIngreso.OrderByDescending(o => o.Ingreso.ID_CONSEC));
                var DistinctItems = lstIngreso.GroupBy(x => new { x.Imputado }).Select(y => y.First());
                foreach (var n in DistinctItems)
                {
                    var consulta_interno = n.Ingreso.INGRESO;
                    var edificio = consulta_interno.CAMA.CELDA.SECTOR.EDIFICIO;
                    var sector = consulta_interno.CAMA.CELDA.SECTOR;
                    var celda = consulta_interno.CAMA.CELDA;
                    var lst_ingreso_reubicacion = new cIngresoUbicacionAnterior().ObtenerTodos(GlobalVar.gCentro, n.Anio, n.Imputado, n.IdIngreso, FFechaInicio.Value.Date, FFechaFin.Value.Date).Where(w => w.ID_UB_CENTRO == GlobalVar.gCentro).ToList();
                    if (lst_ingreso_reubicacion != null)
                    {
                        var demo = new List<cReubicacionInternoDetalle>(lst_ingreso_reubicacion.Select(w => new cReubicacionInternoDetalle()
                        {
                            Centro = w.ID_CENTRO,
                            Anio = w.ID_ANIO,
                            Imputado = w.ID_IMPUTADO,
                            IdIngreso = w.ID_INGRESO,
                            Ubicacion = string.Format("{0}-{1}-{2}-{3}", 
                            edificio != null ?  edificio.DESCR.TrimEnd() : string.Empty,
                            sector != null ? sector.DESCR.TrimEnd() : string.Empty,
                            celda != null ? celda.ID_CELDA.TrimEnd() : string.Empty,
                            w.CAMA != null ? w.CAMA.ID_CAMA : 0),
                            Fecha = w.REGISTRO_FEC.Value,
                            Motivo = w.MOTIVO_CAMBIO,
                            Ingreso = w,
                        }));
                        //int numero = 0;
                        foreach(var row in demo)
                        {
                            //numero += 1;
                            row.CountTotal += 1;
                            rid.Add(row);
                            termsList.Add(row.Imputado.ToString());
                        }
                    }
                    ri.Add(n);
                }
                // You can convert it back to an array if you would like to
                var bla = termsList.Cast<String>().ToArray();
                rid = new List<cReubicacionInternoDetalle>(rid);
                //ARMAMOS EL REPORTE
                Reporte.LocalReport.ReportPath = "Reportes/rInternosReubicacion.rdlc";
                Reporte.LocalReport.DataSources.Clear();

                Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds2.Name = "DataSet1";
                rds2.Value = datosReporte;
                Reporte.LocalReport.DataSources.Add(rds2);

                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds1.Name = "DataSet2";
                rds1.Value = ri;
                Reporte.LocalReport.DataSources.Add(rds1);

                #region Subreporte
                Reporte.LocalReport.SubreportProcessing += (s, e) =>
                {
                    if (e.ReportPath.Equals("srInternosReubicacionDetalle", StringComparison.InvariantCultureIgnoreCase))
                    {
                        ReportDataSource ds = new ReportDataSource("DataSet4", rid);
                        e.DataSources.Add(ds);
                    }
                };
                #endregion

                #region Parametros
                Reporte.LocalReport.SetParameters(new Microsoft.Reporting.WinForms.ReportParameter("Imputado", bla));
                #endregion

                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    Repositorio.Visibility = Visibility.Visible;
                    Reporte.Refresh();
                    Reporte.RefreshReport();
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
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_INTERNOS_REUBICACIONES.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
