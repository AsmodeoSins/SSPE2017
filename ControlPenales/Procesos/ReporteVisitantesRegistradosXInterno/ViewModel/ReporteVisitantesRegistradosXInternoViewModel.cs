using Microsoft.Reporting.WinForms;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlPenales
{
    public partial class ReporteVisitantesRegistradosXInternoViewModel : ValidationViewModelBase
    {
        public async void OnLoad(ReporteVisitantesRegistradosXInternoView Window)
        {
            try
            {
                ConfiguraPermisos();
                Ventana = Window;
                Reporte = Ventana.Report;
                SelectedFechaInicial = Fechas.GetFechaDateServer;
                SelectedFechaFinal = Fechas.GetFechaDateServer;

                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ReportViewerVisible = Visibility.Collapsed;
                    }));

                    SelectedFechaInicial = Fechas.GetFechaDateServer;
                    SelectedFechaFinal = Fechas.GetFechaDateServer;
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ReportViewerVisible = Visibility.Visible;
                    }));
                });


            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }

        public void ClickSwitch(object obj)
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
                    GenerarReporte();
                    break;
                default:
                    break;
            }
        }

        public async void GenerarReporte()
        {

            var lInternos = new List<cInternoVisitantes>();
            var datosReporte = new List<cReporteDatos>();
            var centro = new CENTRO();



            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
            {
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    ReportViewerVisible = Visibility.Collapsed;
                }));
                try
                {

                    centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();
                    datosReporte.Add(new cReporteDatos()
                    {
                        Encabezado1 = Parametro.ENCABEZADO1.Trim(),
                        Encabezado2 = Parametro.ENCABEZADO2.Trim(),
                        Encabezado3 = Parametro.ENCABEZADO3.Trim(),
                        Titulo = "VISITA FAMILIAR",
                        Logo1 = Parametro.REPORTE_LOGO1,
                        Logo2 = Parametro.REPORTE_LOGO2,
                        Centro = centro.DESCR.Trim().ToUpper(),
                    });



                    var ingresos = new cVisitanteIngreso().
                        ObtenerTodosIngresos().
                        Select(s => new { s.ID_CENTRO, s.ID_ANIO, s.ID_IMPUTADO, s.ID_INGRESO }).
                        Distinct().OrderBy(o => o.ID_CENTRO).ThenBy(t => t.ID_ANIO).ThenBy(t => t.ID_IMPUTADO).ThenBy(t => t.ID_INGRESO).ToList();

                    var consulta_ingreso = new cIngreso();
                    foreach (var ingreso in ingresos)
                    {
                        var visitantes_registrados = new cVisitanteIngreso().
                            ObtenerVisitantesIngreso(ingreso.ID_CENTRO, ingreso.ID_ANIO, ingreso.ID_IMPUTADO, ingreso.ID_INGRESO).
                            Select(s => new { s.ID_PERSONA }).
                            Distinct().OrderBy(o => o.ID_PERSONA).ToList();
                        if (visitantes_registrados.Count() != 0)
                        {
                            var visitantes_aduana = new cAduana().
                            ObtenerTodosVisitantesIngreso(ingreso.ID_CENTRO, ingreso.ID_ANIO, ingreso.ID_IMPUTADO, ingreso.ID_INGRESO, SelectedFechaInicial, SelectedFechaFinal).
                            Select(s => new { s.ID_PERSONA }).
                            Distinct().OrderBy(o => o.ID_PERSONA).ToList();

                            foreach (var visitante in visitantes_aduana)
                            {
                                visitantes_registrados.Remove(visitante);
                            }

                            var ultimo_ingreso = consulta_ingreso.ObtenerUltimoIngreso(GlobalVar.gCentro, ingreso.ID_ANIO, ingreso.ID_IMPUTADO);
                            if (visitantes_registrados.Count() != 0)
                            lInternos.Add(
                                new cInternoVisitantes()
                                {
                                    Anio = ingreso != null ? ingreso.ID_ANIO : new int(),
                                    Celda = ultimo_ingreso != null ? ultimo_ingreso.CAMA != null ? ultimo_ingreso.CAMA.ID_CELDA : string.Empty : string.Empty,
                                    Edificio = ultimo_ingreso != null ? ultimo_ingreso.CAMA != null ? ultimo_ingreso.CAMA.CELDA != null ? ultimo_ingreso.CAMA.CELDA.SECTOR != null ? ultimo_ingreso.CAMA.CELDA.SECTOR.EDIFICIO != null ? !string.IsNullOrEmpty(ultimo_ingreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR) ? ultimo_ingreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                                    Id_Imputado = ingreso != null ? ingreso.ID_IMPUTADO : new int(),
                                    Materno = ultimo_ingreso != null ? ultimo_ingreso.IMPUTADO != null ? !string.IsNullOrEmpty(ultimo_ingreso.IMPUTADO.MATERNO) ? ultimo_ingreso.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                                    Nombre = ultimo_ingreso != null ? ultimo_ingreso.IMPUTADO != null ? !string.IsNullOrEmpty(ultimo_ingreso.IMPUTADO.NOMBRE) ? ultimo_ingreso.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                                    Paterno = ultimo_ingreso != null ? ultimo_ingreso.IMPUTADO != null ? !string.IsNullOrEmpty(ultimo_ingreso.IMPUTADO.PATERNO) ? ultimo_ingreso.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                                    Sector = ultimo_ingreso != null ? ultimo_ingreso.CAMA != null ? ultimo_ingreso.CAMA.CELDA != null ? ultimo_ingreso.CAMA.CELDA.SECTOR != null ? !string.IsNullOrEmpty(ultimo_ingreso.CAMA.CELDA.SECTOR.DESCR) ? ultimo_ingreso.CAMA.CELDA.SECTOR.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                                    VisitantesRegistrados = visitantes_registrados != null ? visitantes_registrados.Any() ? visitantes_registrados.Count : new int() : new int(),
                                });
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw new ApplicationException(ex.Message);
                }

                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    ReportViewerVisible = Visibility.Visible;
                }));
            });


            Reporte.LocalReport.ReportPath = "Reportes/rVisitantesRegistradosXInterno.rdlc";
            Reporte.LocalReport.DataSources.Clear();

            ReportDataSource ReportDataSource_Encabezado = new ReportDataSource();
            ReportDataSource_Encabezado.Name = "DataSet1";
            ReportDataSource_Encabezado.Value = datosReporte;

            ReportDataSource ReportDataSource = new ReportDataSource();
            ReportDataSource.Name = "DataSet2";
            ReportDataSource.Value = lInternos;

            Reporte.LocalReport.DataSources.Add(ReportDataSource_Encabezado);
            Reporte.LocalReport.DataSources.Add(ReportDataSource);


            Reporte.LocalReport.SetParameters(new ReportParameter(("FechaInicial"), string.Format("{0}/{1}/{2}", SelectedFechaInicial.Day, SelectedFechaInicial.Month, SelectedFechaInicial.Year)));
            Reporte.LocalReport.SetParameters(new ReportParameter(("FechaFinal"), string.Format("{0}/{1}/{2}", SelectedFechaFinal.Day, SelectedFechaFinal.Month, SelectedFechaFinal.Year)));

            Reporte.Refresh();
            Reporte.RefreshReport();
        }

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_VISITANTES_REGISTRADOS_INTERNO.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
