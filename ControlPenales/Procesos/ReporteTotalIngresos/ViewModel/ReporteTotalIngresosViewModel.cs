using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Servidor;
using System.Windows;
using SSP.Controlador.Catalogo.Justicia;
using Microsoft.Reporting.WinForms;

namespace ControlPenales
{
    public partial class ReporteTotalIngresosViewModel : ValidationViewModelBase
    {
        public async void OnLoad(ReporteTotalIngresos Window)
        {
            try
            {
                ConfiguraPermisos();
                Ventana = Window;
                Reporte = Ventana.Report;
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
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la pantalla", ex);
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
            var datosReporte = new List<cReporteDatos>();
            var lIngreso = new List<cTotalIngreso>();
            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
            {
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    ReportViewerVisible = Visibility.Collapsed;
                }));
                try
                {
                    var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();

                    datosReporte.Add(new cReporteDatos()
                    {
                        Encabezado1 = Parametro.ENCABEZADO1.Trim(),
                        Encabezado2 = Parametro.ENCABEZADO2.Trim(),
                        Encabezado3 = Parametro.ENCABEZADO3.Trim(),
                        Titulo = "RELACIÓN DE INGRESOS",
                        Logo1 = Parametro.REPORTE_LOGO1,
                        Logo2 = Parametro.REPORTE_LOGO2,
                        Centro = centro.DESCR.Trim().ToUpper(),
                    });

                    lIngreso = new cIngreso().ObtenerIngresosActivosPorFecha(GlobalVar.gCentro, SelectedFechaInicial, SelectedFechaFinal).
                        AsEnumerable().
                        Select(s => new cTotalIngreso()
                        {
                            Nacionalidad = s.PAIS_NACIONALIDAD != null ?
                            (!string.IsNullOrEmpty(s.PAIS_NACIONALIDAD.NACIONALIDAD) ? (s.PAIS_NACIONALIDAD.NACIONALIDAD.TrimEnd() != "MEXICANA" ? "EXTRANJEROS" : "MEXICANOS") : "INDEFINIDO") : "INDEFINIDO",
                            Sexo = !string.IsNullOrEmpty(s.IMPUTADO.SEXO) ? (s.IMPUTADO.SEXO == FEMENINO ? "FEMENINO" : "MASCULINO") : "INDEFINIDO"
                        }).ToList();
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
            Reporte.LocalReport.ReportPath = "Reportes/rTotalIngresos.rdlc";
            Reporte.LocalReport.DataSources.Clear();

            ReportDataSource ReportDataSource_Encabezado = new ReportDataSource();
            ReportDataSource_Encabezado.Name = "DataSet1";
            ReportDataSource_Encabezado.Value = datosReporte;

            ReportDataSource ReportDataSource_TotalIngresos = new ReportDataSource();
            ReportDataSource_TotalIngresos.Name = "DataSet2";
            ReportDataSource_TotalIngresos.Value = lIngreso;

            Reporte.LocalReport.DataSources.Add(ReportDataSource_Encabezado);
            Reporte.LocalReport.DataSources.Add(ReportDataSource_TotalIngresos);

            Reporte.LocalReport.SetParameters(new ReportParameter("FechaInicial", string.Format("{0}/{1}/{2}", SelectedFechaInicial.Day, SelectedFechaInicial.Month, SelectedFechaInicial.Year)));
            Reporte.LocalReport.SetParameters(new ReportParameter("FechaFinal", string.Format("{0}/{1}/{2}", SelectedFechaFinal.Day, SelectedFechaFinal.Month, SelectedFechaFinal.Year)));

            Reporte.Refresh();
            Reporte.RefreshReport();
        }

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_TOTAL_INGRESOS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
