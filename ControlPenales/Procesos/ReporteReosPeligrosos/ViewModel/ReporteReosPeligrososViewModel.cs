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
    public partial class ReporteReosPeligrososViewModel : ValidationViewModelBase
    {
        public async void OnLoad(ReporteReosPeligrosos Window)
        {
            try
            {
                ConfiguraPermisos();
                Ventana = Window;
                Reporte = Ventana.Report;
                TodosLosReos = true;
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

        public string ObtenerEstatusIngreso(INGRESO Ingreso)
        {
            enumEstatusAdministrativo EstatusAdministrativo = (enumEstatusAdministrativo)Ingreso.ID_ESTATUS_ADMINISTRATIVO;
            if (!(EstatusAdministrativo == enumEstatusAdministrativo.LIBERADO ||
                EstatusAdministrativo == enumEstatusAdministrativo.TRASLADADO ||
                EstatusAdministrativo == enumEstatusAdministrativo.SUJETO_A_PROCESO_EN_LIBERTAD ||
                EstatusAdministrativo == enumEstatusAdministrativo.DISCRECIONAL))
            {
                return "S";
            }
            return "N";
        }

        public async void GenerarReporte()
        {
            var lReosPeligrosos = new List<cReosPeligrosos>();
            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
            {
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    ReportViewerVisible = Visibility.Collapsed;
                }));
                if (TodosLosReos)
                    lReosPeligrosos = new cIngreso().
                        ObtenerIngresosAltaSeguridad(
                        GlobalVar.gCentro).
                        AsEnumerable().
                        Select(s => new cReosPeligrosos()
                        {
                            Activo = ObtenerEstatusIngreso(s),
                            Cereso = s.CENTRO.DESCR.TrimEnd(),
                            ClasificacionJuridica = s.CLASIFICACION_JURIDICA != null ? !string.IsNullOrEmpty(s.CLASIFICACION_JURIDICA.DESCR) ? s.CLASIFICACION_JURIDICA.DESCR.TrimEnd() : string.Empty : string.Empty,
                            Delito = string.Empty,
                            FechaIngreso = s.FEC_INGRESO_CERESO.Value,
                            Id_Anio = s.ID_ANIO,
                            Id_Imputado = s.ID_IMPUTADO,
                            Id_Ingreso = s.ID_INGRESO,
                            Paterno = !string.IsNullOrEmpty(s.IMPUTADO.PATERNO) ? s.IMPUTADO.PATERNO.TrimEnd() : string.Empty,
                            Materno = !string.IsNullOrEmpty(s.IMPUTADO.MATERNO) ? s.IMPUTADO.MATERNO.TrimEnd() : string.Empty,
                            Nombre = !string.IsNullOrEmpty(s.IMPUTADO.NOMBRE) ? s.IMPUTADO.NOMBRE.TrimEnd() : string.Empty,
                        }).ToList();
                else
                    lReosPeligrosos = new cIngreso().
                    ObtenerIngresosAltaSeguridad(
                    GlobalVar.gCentro).
                     Where(w =>
                            w.FEC_INGRESO_CERESO.HasValue &&
                            (w.FEC_INGRESO_CERESO.Value >= SelectedFechaInicial.Date &&
                            w.FEC_INGRESO_CERESO.Value <= SelectedFechaFinal.Date)).
                    AsEnumerable().
                    Select(s => new cReosPeligrosos()
                    {
                        Activo = ObtenerEstatusIngreso(s),
                        Cereso = s.CENTRO.DESCR.TrimEnd(),
                        ClasificacionJuridica = s.CLASIFICACION_JURIDICA != null ? !string.IsNullOrEmpty(s.CLASIFICACION_JURIDICA.DESCR) ? s.CLASIFICACION_JURIDICA.DESCR.TrimEnd() : string.Empty : string.Empty,
                        Delito = s.INGRESO_DELITO.DESCR,
                        FechaIngreso = s.FEC_INGRESO_CERESO.Value,
                        Id_Anio = s.ID_ANIO,
                        Id_Imputado = s.ID_IMPUTADO,
                        Id_Ingreso = s.ID_INGRESO,
                        Paterno = !string.IsNullOrEmpty(s.IMPUTADO.PATERNO) ? s.IMPUTADO.PATERNO.TrimEnd() : string.Empty,
                        Materno = !string.IsNullOrEmpty(s.IMPUTADO.MATERNO) ? s.IMPUTADO.MATERNO.TrimEnd() : string.Empty,
                        Nombre = !string.IsNullOrEmpty(s.IMPUTADO.NOMBRE) ? s.IMPUTADO.NOMBRE.TrimEnd() : string.Empty,
                    }).ToList();
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    ReportViewerVisible = Visibility.Visible;
                }));
            });

            Reporte.LocalReport.ReportPath = "Reportes/rReosPeligrosos.rdlc";
            Reporte.LocalReport.DataSources.Clear();

            ReportDataSource ReportDataSource = new ReportDataSource();
            ReportDataSource.Name = "DataSet1";
            ReportDataSource.Value = lReosPeligrosos;

            Reporte.LocalReport.DataSources.Add(ReportDataSource);


            Reporte.Refresh();
            Reporte.RefreshReport();

        }

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_REOS_PELIGROSOS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
