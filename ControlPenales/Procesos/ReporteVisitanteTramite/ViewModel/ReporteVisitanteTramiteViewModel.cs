using Microsoft.Reporting.WinForms;
using SSP.Controlador.Catalogo.Justicia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlPenales
{
    public partial class ReporteVisitanteTramiteViewModel : ValidationViewModelBase
    {
        public async void OnLoad(ReporteVisitanteTramiteView Window)
        {
            try
            {
                ConfiguraPermisos();
                ReportViewerVisible = Visibility.Collapsed;

                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {

                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {

                        Ventana = Window;
                        Reporte = Ventana.Report;
                    }));

                });
                ReportViewerVisible = Visibility.Visible;

            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }

        public async void ClickSwitch(object obj)
        {
            switch (obj.ToString())
            {
                case "GenerarReporte":
                    if (!pConsultar)
                    {
                        ReportViewerVisible = Visibility.Collapsed;
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    Reporte.Reset();
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                    {
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            ReportViewerVisible = Visibility.Collapsed;
                        }));
                        GenerarReporte();
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            ReportViewerVisible = Visibility.Visible;
                        }));

                    });
                    break;
            }
        }

        public void GenerarReporte()
        {
            try
            {
                var lVisitantesTramite = new List<cVisitanteTramite>();
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
                var visitante_ingreso = new cVisitanteIngreso();
                lVisitantesTramite = visitante_ingreso.ObtenerTodosFecha().
                    AsEnumerable().
                    Select(s => new cVisitanteTramite()
                    {
                        Anio = s.ID_ANIO,
                        Id_Imputado = s.ID_IMPUTADO,
                        Materno_Ingreso = !string.IsNullOrEmpty(s.INGRESO.IMPUTADO.MATERNO) ? s.INGRESO.IMPUTADO.MATERNO.TrimEnd() : string.Empty,
                        Materno_Visitante = !string.IsNullOrEmpty(s.VISITANTE.PERSONA.MATERNO) ? s.VISITANTE.PERSONA.MATERNO.TrimEnd() : string.Empty,
                        Nombre_Ingreso = !string.IsNullOrEmpty(s.INGRESO.IMPUTADO.NOMBRE) ? s.INGRESO.IMPUTADO.NOMBRE.TrimEnd() : string.Empty,
                        NumeroVisita = s.ID_PERSONA,
                        Parentesco = s.TIPO_REFERENCIA != null ? (!string.IsNullOrEmpty(s.TIPO_REFERENCIA.DESCR) ? s.TIPO_REFERENCIA.DESCR.TrimEnd() : "SIN DEFINIR") : "SIN DEFINIR"
                    })
                    .ToList();

                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    Reporte.LocalReport.ReportPath = "Reportes/rVisitantesTramite.rdlc";
                    Reporte.LocalReport.DataSources.Clear();

                    ReportDataSource ReportDataSource = new ReportDataSource();
                    ReportDataSource.Name = "DataSet2";
                    ReportDataSource.Value = lVisitantesTramite;

                    ReportDataSource ReportDataSource_Encabezado = new ReportDataSource();
                    ReportDataSource_Encabezado.Name = "DataSet1";
                    ReportDataSource_Encabezado.Value = datosReporte;

                    Reporte.LocalReport.DataSources.Add(ReportDataSource);
                    Reporte.LocalReport.DataSources.Add(ReportDataSource_Encabezado);

                    Reporte.LocalReport.SetParameters(new ReportParameter("Fecha", Fechas.GetFechaDateServer.ToString()));

                    Reporte.Refresh();
                    Reporte.RefreshReport();

                }));

            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_VISITANTES_TRAMITE.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
