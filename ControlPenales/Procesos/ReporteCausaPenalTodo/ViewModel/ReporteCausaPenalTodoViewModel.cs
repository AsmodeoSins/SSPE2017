using ControlPenales;
using Microsoft.Reporting.WinForms;
using Oracle.ManagedDataAccess.Client;
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
using System.Windows.Forms.Integration;
using System.Windows.Input;

namespace ControlPenales
{
    public partial class ReporteCausaPenalTodoViewModel : ValidationViewModelBase
    {

        #region Constructor
        public ReporteCausaPenalTodoViewModel() { }
        public ReporteCausaPenalTodoViewModel(bool Federal,bool Comun,bool Ambos) 
        {
            this.Federal = Federal;
            this.Comun = Comun;
            this.Ambos = Ambos;
        }
        #endregion

        #region Metodos
        private async void SwitchClick(Object obj)
        {
            switch (obj.ToString())
            { 
                case "generar":
                    if (!pConsultar)
                    {
                        Repositorio.Visibility = Visibility.Collapsed;
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        break;
                    }
                    //((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new ReporteCausaPenalTodoView();
                    //((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.ReporteCausaPenalTodoViewModel(Federal,Comun,Ambos);
                    ReportViewerVisible = Visibility.Collapsed;
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(GenerarReporte);
                    break;
            }
        }

        private async void OnLoad(ReporteCausaPenalTodoView Window = null)
        {                   
            try
            {
                ConfiguraPermisos();
                Repositorio = Window.WFH;
                Reporte = Window.Report;
                //GenerarReporte();
                //await StaticSourcesViewModel.CargarDatosMetodoAsync(GenerarReporte);
                ReportViewerVisible = Visibility.Visible;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la bitacora de recepción legal", ex);
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
                string delitos = string.Empty;
                var hoy = Fechas.GetFechaDateServer;
                var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();

                var datosReporte = new List<cReporteDatos>();
                datosReporte.Add(new cReporteDatos()
                {
                    Encabezado1 = Parametro.ENCABEZADO1.Trim(),
                    Encabezado2 = Parametro.ENCABEZADO2.Trim(),
                    Encabezado3 = Parametro.ENCABEZADO3.Trim(),
                    Titulo = "CAUSAS PENALES",
                    Logo1 = Parametro.REPORTE_LOGO1,
                    Logo2 = Parametro.REPORTE_LOGO2,
                    Centro = centro.DESCR.Trim().ToUpper(),
                });

                var internos = new List<cReporteCausaPenalTodos>();
                var internosDetalle = new List<cReporteCausaPenalTodosDetalle>();
                #region Causa Penal
                var ingresos = new cIngreso().ObtenerIngresosActivosCausaPenal(GlobalVar.gCentro,Ambos ? string.Empty : Federal ? "F" : "C");
                if (ingresos != null)
                {
                    foreach (var i in ingresos)
                    {
                        internos.Add(new cReporteCausaPenalTodos(){ 
                            ID_CENTRO = i.ID_CENTRO,
                            ID_ANIO = i.ID_ANIO,
                            ID_IMPUTADO = i.ID_IMPUTADO,
                            ID_INGRESO = i.ID_INGRESO,
                            NOMBRE = i.IMPUTADO != null ? !string.IsNullOrEmpty(i.IMPUTADO.NOMBRE) ? i.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty,
                            PATERNO = i.IMPUTADO != null ? !string.IsNullOrEmpty(i.IMPUTADO.PATERNO) ? i.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty,
                            MATERNO = i.IMPUTADO != null ? !string.IsNullOrEmpty(i.IMPUTADO.MATERNO) ? i.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty,
                            CLASIFICACION_JURIDICA = i.CLASIFICACION_JURIDICA != null ? !string.IsNullOrEmpty(i.CLASIFICACION_JURIDICA.DESCR) ? i.CLASIFICACION_JURIDICA.DESCR.Trim() : string.Empty : string.Empty
                        });

                        if (i.CAUSA_PENAL != null)
                        {
                            var cps = new List<CAUSA_PENAL>();
                            if (Federal)
                            {
                                cps = new List<CAUSA_PENAL>(i.CAUSA_PENAL.Where(w => w.CP_FUERO == "F"));
                            }
                            else
                                if (Comun)
                                {
                                    cps = new List<CAUSA_PENAL>(i.CAUSA_PENAL.Where(w => w.CP_FUERO == "C"));
                                }
                                else
                                    cps = new List<CAUSA_PENAL>(i.CAUSA_PENAL);
                            foreach (var cp in cps)
                            {
                                var sentencia = cp.SENTENCIA.Any() ? cp.SENTENCIA.Where(w => w.ESTATUS == "A").OrderByDescending(w => w.ID_SENTENCIA).FirstOrDefault() : null;
                                var cptd = new cReporteCausaPenalTodosDetalle();
                                cptd.ID_CENTRO = i.ID_CENTRO;
                                cptd.ID_ANIO = i.ID_ANIO;
                                cptd.ID_IMPUTADO = i.ID_IMPUTADO;
                                cptd.ID_INGRESO = i.ID_INGRESO;
                                cptd.CP_ANIO = cp.CP_ANIO.HasValue ? cp.CP_ANIO.Value : new int();
                                cptd.CP_FOLIO = cp.CP_FOLIO.HasValue ? cp.CP_FOLIO.Value : new int();
                                if (cp.CP_FUERO == "C")
                                    cptd.FUERO = "COMÚN";
                                else
                                if (cp.CP_FUERO == "F")
                                    cptd.FUERO = "FEDERAL";
                                if(cp.CP_JUZGADO != null)
                                    cptd.JUZGADO = !string.IsNullOrEmpty(cp.JUZGADO.DESCR) ? cp.JUZGADO.DESCR.Trim() : string.Empty;
                                if (sentencia != null)
                                {
                                    cptd.SETENCIA_ANIO = sentencia.ANIOS != null ? sentencia.ANIOS.Value : 0;
                                    cptd.SENTENCIA_MES = sentencia.MESES != null ? sentencia.MESES.Value : 0;
                                    cptd.SENTENCIA_DIA = sentencia.DIAS != null ? sentencia.DIAS.Value : 0;
                                    if (sentencia.FEC_INICIO_COMPURGACION.HasValue)
                                    {
                                        cptd.FEC_INICIO_COMPURGACION = sentencia.FEC_INICIO_COMPURGACION.Value;
                                        DateTime t = cptd.FEC_INICIO_COMPURGACION;
                                        t = t.AddYears(cptd.SETENCIA_ANIO);
                                        t = t.AddMonths(cptd.SENTENCIA_MES);
                                        t = t.AddDays(cptd.SENTENCIA_DIA);
                                        cptd.SENTENCIA_DIAS = (int)(t.Date - cptd.FEC_INICIO_COMPURGACION).TotalDays;
                                        cptd.SENTENCIA_DIAS_COMPURGADO = (int)(hoy - cptd.FEC_INICIO_COMPURGACION).TotalDays;
                                        if (cptd.SENTENCIA_DIA > 0 && cptd.SENTENCIA_DIAS_COMPURGADO > 0)
                                            cptd.SENTENCIA_DIAS_PORCENTAJE = (cptd.SENTENCIA_DIAS_COMPURGADO * 100) / cptd.SENTENCIA_DIAS;
                                    };

                                    if (sentencia.SENTENCIA_DELITO != null)
                                    {
                                        delitos = string.Empty;
                                        foreach (var sd in sentencia.SENTENCIA_DELITO)
                                        {
                                            if (!string.IsNullOrEmpty(delitos))
                                                delitos += Environment.NewLine;
                                            delitos += sd.DESCR_DELITO;
                                        }
                                        cptd.DELITOS = delitos;
                                    }
                                }
                                else
                                {
                                    cptd.SETENCIA_ANIO = cptd.SENTENCIA_MES = cptd.SENTENCIA_DIA = 0;
                                    cptd.SENTENCIA_DIA = 0;
                                    cptd.SENTENCIA_DIAS_COMPURGADO = 0;
                                    cptd.SENTENCIA_DIAS_PORCENTAJE = 0;
                                   
                                }
                                internosDetalle.Add(cptd);
                            }
                        }
                    }
                }
                #endregion

                #region Reporte
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    Reporte.Reset();
                }));
                Reporte.LocalReport.ReportPath = "Reportes/rCausasPenalesTodo.rdlc";
                Reporte.LocalReport.DataSources.Clear();

                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds1.Name = "DataSet3";
                rds1.Value = datosReporte;
                Reporte.LocalReport.DataSources.Add(rds1);

                Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds2.Name = "DataSet1";
                rds2.Value = internos;
                Reporte.LocalReport.DataSources.Add(rds2);

                //Subreporte
                Reporte.LocalReport.SubreportProcessing += (s, e) =>
                {

                    if (e.ReportPath.Equals("srCausasPenalesTodoDetalle", StringComparison.InvariantCultureIgnoreCase))
                    {
                        ReportDataSource ds = new ReportDataSource("DataSet1", internosDetalle);
                        e.DataSources.Add(ds);
                    }
                };

                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    Repositorio.Visibility = Visibility.Visible;
                    Reporte.RefreshReport();
                    Reporte.LocalReport.Refresh();
                }));
                #endregion

            }
            
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar reporte.", ex);
            }
        }
        #endregion

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_CAUSA_PENAL.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
