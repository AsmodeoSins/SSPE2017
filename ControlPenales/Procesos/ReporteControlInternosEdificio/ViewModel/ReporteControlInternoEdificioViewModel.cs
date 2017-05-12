using ControlPenales.Clases.ControlInternos;
using Microsoft.Reporting.WinForms;
using SSP.Controlador.Catalogo.Justicia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlPenales
{
    public partial class ReporteControlInternoEdificioViewModel : ValidationViewModelBase
    {
        #region[METODOS]
        private async void ClickSwitch(object obj)
        {
            try
            {
                switch (obj.ToString())
                {
                    case "ordenar":
                        break;
                    case "generar":
                        if (!pConsultar)
                        {
                            Repositorio.Visibility = Visibility.Collapsed;
                            new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                            break;
                        }
                        if (!base.HasErrors)
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(GenerarReporteConQuery);
                        else
                        {
                            ReportViewerVisible = Visibility.Collapsed;
                            await new Dialogos().ConfirmacionDialogoReturn("Validación", base.Error);
                            ReportViewerVisible = Visibility.Visible;
                        } break;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
            }
        }

        private void GenerarReporteConQuery() 
        {
            try
            {
                var datosReporte = new List<cReporteDatos>();
                datosReporte.Add(new cReporteDatos()
                {
                    Encabezado1 = Parametro.ENCABEZADO1,
                    Encabezado2 = Parametro.ENCABEZADO2,
                    Encabezado3 = Parametro.ENCABEZADO3,
                    Logo1 = Parametro.REPORTE_LOGO1,
                    Logo2 = Parametro.REPORTE_LOGO2,
                    Titulo = "REPORTE CONTROL DE INTERNOS EN EDIFICIO"
                });

                var reporte = new cIngreso().ReporteUbicacionIngresoEdificio(GlobalVar.gCentro, FechaInicio.Value.Date,FechaFin.Value.Date).Select(w => new cControlInternosEdificio()
                {
                    EXPEDIENTE = string.Format("{0}-{1}",w.ID_ANIO,w.ID_IMPUTADO), 
                    NOMBRE = string.Format("{0} {1} {2}",w.NOMBRE,w.PATERNO,w.MATERNO),
                    UBICACION = string.Format("{0}-{1}-{2}-{3}",w.EDIFICIO,w.SECTOR,w.CELDA,w.CAMA),
                    FECHA = w.MOVIMIENTO_FEC.ToShortDateString(),
                    AREA = w.AREA,
                    SALIDA = w.ESTATUS == 1 ? (DateTime?)w.MOVIMIENTO_FEC : null,
                    ENTRADA = (w.ESTATUS == 0 || w.ESTATUS == 2) ? (DateTime?)w.MOVIMIENTO_FEC : null,
                    MOTIVO = w.ACTIVIDAD
                });
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    Repositorio.Visibility = Visibility.Collapsed;
                    Reporte.Reset();
                }));
                Reporte.LocalReport.ReportPath = "Reportes/rControlInternosEdificioNew.rdlc";
                Reporte.LocalReport.DataSources.Clear();

                ReportDataSource rds1 = new ReportDataSource();
                rds1.Name = "DataSet1";
                rds1.Value = datosReporte;
                Reporte.LocalReport.DataSources.Add(rds1);

                ReportDataSource rds2 = new ReportDataSource();
                rds2.Name = "DataSet2";
                rds2.Value = reporte;
                Reporte.LocalReport.DataSources.Add(rds2);

                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    Reporte.Refresh();
                    Reporte.RefreshReport();
                    Repositorio.Visibility = Visibility.Visible;
                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar reporte", ex);
            }
            
        }

        private void GenerarReporte()
        {
            try
            {
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    Repositorio.Visibility = Visibility.Collapsed;
                }));
                //var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();
                var internos_lista = new InternosAusentes().ListInternosAusentesReporte(GlobalVar.gCentro, FechaInicio, FechaFin);
                var temp_list_internos = new List<ReporteControlInternosEdificio>();
                var datosReporte = new List<cReporteDatos>();
                List<string> termsList = new List<string>();
                datosReporte.Add(new cReporteDatos()
                    {
                        Encabezado1 = Parametro.ENCABEZADO1,
                        Encabezado2 = Parametro.ENCABEZADO2,
                        Encabezado3 = Parametro.ENCABEZADO3,
                        Logo1 = Parametro.REPORTE_LOGO1,
                        Logo2 = Parametro.REPORTE_LOGO2,
                        Titulo = "REPORTE CONTROL DE INTERNOS EN EDIFICIO"
                    });

                var lst_internos = internos_lista.Select(s => new ReporteControlInternosEdificio()
                    {
                        Expediente = string.IsNullOrEmpty(s.Expediente) ? string.Empty : s.Expediente,
                        Nombre = string.Format("{0} {1} {2}", s.Nombre, string.IsNullOrEmpty(s.Paterno) ? string.Empty : s.Paterno, string.IsNullOrEmpty(s.Materno) ? string.Empty : s.Materno),
                        Ubicacion = string.IsNullOrEmpty(s.Estancia) ? string.Empty : s.Estancia,
                        UbicacionActual = s.Estatus.HasValue ? s.Estatus == 1 ? "TRANSITO" : s.Estatus == 2 ? s.Area : s.Estatus == 0 ? "ESTANCIA" : string.Empty : string.Empty,
                        FechaSalida = s.Fecha.Value.Date,
                        HoraSalida = s.Estatus == 1 ? s.Fecha.Value.TimeOfDay : new TimeSpan(0, 0, 0, 0, 0),
                        HoraEntrada = s.Estatus == 2 || s.Estatus == 0 ? s.Fecha.Value.TimeOfDay : new TimeSpan(0, 0, 0, 0, 0),
                        AutorizaSalida = string.Format("EDIF-{0}", string.IsNullOrEmpty(s.Ingreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR) ? string.Empty : s.Ingreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR),
                        Area = string.IsNullOrEmpty(s.Area) ? string.Empty : s.Area,
                        MotivoSalida = string.IsNullOrEmpty(s.Actividad) ? string.Empty : s.Actividad,
                        TotalPendiente = s.Estatus.HasValue ? s.Estatus != 0 ? 1 : 0 : 0,
                        TotalEntrada = s.Estatus.HasValue ? s.Estatus == 0 ? 1 : 0: 0
                    });

                var DistinctItems = lst_internos.GroupBy(x => new { x.Expediente }).Select(y => y.Last());
                foreach (var n in DistinctItems)
                {
                    temp_list_internos.Add(n);
                }
                foreach (var row in lst_internos)
                {
                    //numero += 1;
                    //row.CountTotal += 1;
                    //rid.Add(row);
                    termsList.Add(row.Expediente);
                }

                if (lst_internos != null)
                {
                    switch (OrdenarPor)
                    {
                        case 1://expediente
                            lst_internos = lst_internos.OrderBy(w => w.Expediente);
                            break;
                        case 2://nombre
                            lst_internos = lst_internos.OrderBy(w => w.Area);
                            break;
                        case 3://Ubicacion
                            lst_internos = lst_internos.OrderBy(w => w.MotivoSalida);
                            break;
                    }

                    Reporte.LocalReport.ReportPath = "../../Reportes/rControlInternosEdificio.rdlc";
                    Reporte.LocalReport.DataSources.Clear();

                    ReportDataSource rds1 = new ReportDataSource();
                    rds1.Name = "DataSet1";
                    rds1.Value = datosReporte;
                    Reporte.LocalReport.DataSources.Add(rds1);

                    ReportDataSource rds2 = new ReportDataSource();
                    rds2.Name = "DataSet2";
                    rds2.Value = temp_list_internos;
                    Reporte.LocalReport.DataSources.Add(rds2);
                }
               
                var bla = termsList.Cast<String>().ToArray();

                #region Subreporte
                Reporte.LocalReport.SubreportProcessing += (s, e) =>
                {
                    if (e.ReportPath.Equals("srInternosEnEdificio", StringComparison.InvariantCultureIgnoreCase))
                    {
                        ReportDataSource ds = new ReportDataSource("DataSet3", lst_internos);
                        e.DataSources.Add(ds);
                    }
                };
                #endregion

                #region Parametros
                Reporte.LocalReport.SetParameters(new Microsoft.Reporting.WinForms.ReportParameter("Imputado", bla));
                #endregion

                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                   Reporte.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                    Reporte.Refresh();
                    Reporte.RefreshReport();
                    Repositorio.Visibility = Visibility.Visible;
                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar reporte.", ex);
            }
        }

        private void ReporteControlInternoLoad(ReporteControlInternoEdificioView window)
        {
            try
            {
                ConfiguraPermisos();
                Repositorio = window.WFH;
                Reporte = window.Report;
                Validaciones();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar pantalla.", ex);
            }
        }
        #endregion

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_CONTROL_INTERNOS_EDIFICIO.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
