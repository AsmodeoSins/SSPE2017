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
    public partial class ReporteTiempoTramiteVisitaViewModel : ValidationViewModelBase
    {
        #region Constructor
        public ReporteTiempoTramiteVisitaViewModel() { }
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

        private void OnLoad(ReporteTiempoTramiteVisitaView Window = null)
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

        #region Tiempo Tramite de Visita
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
                    Titulo = "Bitacora de Tiempos",
                    Logo1 = Parametro.REPORTE_LOGO1,
                    Logo2 = Parametro.REPORTE_LOGO2,
                    Centro = centro.DESCR.Trim().ToUpper(),
                });

                #region Reporte
                var lista = new List<cTiempoTramiteVisita>();
                var promedio = new List<cTiempoTramiteVisitaPromedio>();
                var hoy = Fechas.GetFechaDateServer;
                var datos = new cVisitanteIngreso().ObtenerTodos(FFechaInicio, FFechaFin).Select(w => new cTiempoTramiteVisita()
                {
                    Fecha = w.FEC_ALTA.Value,
                    UNombre = w.USUARIO.EMPLEADO.PERSONA.NOMBRE,
                    UPaterno = w.USUARIO.EMPLEADO.PERSONA.PATERNO,
                    UMaterno = w.USUARIO.EMPLEADO.PERSONA.MATERNO,
                    VNombre = w.VISITANTE.PERSONA.NOMBRE,
                    VPaterno = w.VISITANTE.PERSONA.PATERNO,
                    VMaterno = w.VISITANTE.PERSONA.MATERNO,
                    FechaInicio = w.INICIO_REGISTRO.HasValue ? w.INICIO_REGISTRO : hoy,
                    FechaFin = w.FIN_REGISTRO.HasValue ? w.FIN_REGISTRO : hoy,
                }).ToList();
                //datos.ForEach(w => w.Tiempo = w.FechaFin.Value.Subtract(w.FechaInicio.Value).Seconds);
                if (datos != null)
                {
                    TimeSpan ts;
                    foreach (var d in datos)
                    {
                        ts = d.FechaFin.Value.Subtract(d.FechaInicio.Value);
                        d.aux = ts;
                        if (ts.Days > 0)
                            d.Diferencia = ts.Days.ToString();
                        if (ts.Hours > 0)
                            d.Diferencia = string.IsNullOrEmpty(d.Diferencia) ? ts.Hours.ToString() : string.Format("{0} {1}", d.Diferencia, ts.Hours);
                            d.Diferencia = string.IsNullOrEmpty(d.Diferencia) ? ts.Minutes.ToString() : string.Format("{0}:{1}", d.Diferencia, ts.Minutes);
                            d.Diferencia = string.IsNullOrEmpty(d.Diferencia) ? ts.Seconds.ToString() : string.Format("{0}:{1}", d.Diferencia, ts.Seconds);
                    }
                    double x = datos.Average(w => w.aux.Ticks);
                    long y = Convert.ToInt64(x);
                    var z = new TimeSpan(y);
                    var prom = string.Empty;
                    if (z.Days > 0)
                        prom = z.Days.ToString();
                    if (z.Hours > 0)
                        prom = string.IsNullOrEmpty(prom) ? z.Hours.ToString() : string.Format("{0} {1}", prom, z.Hours);
                        prom = string.IsNullOrEmpty(prom) ? z.Minutes.ToString() : string.Format("{0}:{1}", prom, z.Minutes);
                        prom = string.IsNullOrEmpty(prom) ? z.Seconds.ToString() : string.Format("{0}:{1}", prom, z.Seconds);
                    promedio.Add(new cTiempoTramiteVisitaPromedio() { Promedio = prom });
                }
                datos = new List<cTiempoTramiteVisita>(datos);
                

                #endregion

                //ARMAMOS EL REPORTE
                Reporte.LocalReport.ReportPath = "../../Reportes/rTiempoTramiteVisita.rdlc";
                Reporte.LocalReport.DataSources.Clear();

                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds1.Name = "DataSet1";
                rds1.Value = datos;
                Reporte.LocalReport.DataSources.Add(rds1);

                Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds2.Name = "DataSet2";
                rds2.Value = promedio;
                Reporte.LocalReport.DataSources.Add(rds2);

                Microsoft.Reporting.WinForms.ReportDataSource rds3 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds3.Name = "DataSet3";
                rds3.Value = datosReporte;
                Reporte.LocalReport.DataSources.Add(rds3);

               
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
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_BITACORA_TIEMPOS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
