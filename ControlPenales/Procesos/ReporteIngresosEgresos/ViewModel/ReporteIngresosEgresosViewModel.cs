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
    public partial class ReporteIngresosEgresosViewModel : ValidationViewModelBase
    {
        public async void OnLoad(ReporteIngresosEgresos Window)
        {
            ConfiguraPermisos();
            Ventana = Window;
            Reporte = Ventana.ReportIngresosEgresos;
            Window.ListaMeses.ItemsSource = Enum.GetValues(typeof(eMesesAnio)).Cast<eMesesAnio>();
            Window.ListaMeses.SelectedItem = eMesesAnio.ENERO;
            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
            {
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    ReportViewerVisible = Visibility.Collapsed;
                }));
                SelectedFechaInicial = Fechas.GetFechaDateServer;
                SelectedFechaFinal = Fechas.GetFechaDateServer;

                SeleccionMesSelected = true;
                RangoFechasSelected = false;
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    ReportViewerVisible = Visibility.Visible;
                }));
            });
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

        public int ObtenerMes()
        {
            var Mes = 0;
            switch (Ventana.ListaMeses.SelectedItem.ToString())
            {
                case "ENERO":
                    Mes = 1;
                    break;
                case "FEBRERO":
                    Mes = 2;
                    break;
                case "MARZO":
                    Mes = 3;
                    break;
                case "ABRIL":
                    Mes = 4;
                    break;
                case "MAYO":
                    Mes = 5;
                    break;
                case "JUNIO":
                    Mes = 6;
                    break;
                case "JULIO":
                    Mes = 7;
                    break;
                case "AGOSTO":
                    Mes = 8;
                    break;
                case "SEPTIEMBRE":
                    Mes = 9;
                    break;
                case "OCTUBRE":
                    Mes = 10;
                    break;
                case "NOVIEMBRE":
                    Mes = 11;
                    break;
                case "DICIEMBRE":
                    Mes = 12;
                    break;

            }
            return Mes;
        }

        public string ObtenerNombreUsuario(USUARIO Usuario)
        {
            StringBuilder NombreUsuario = new StringBuilder(string.Empty);

            if (Usuario != null)
            {
                if (Usuario.EMPLEADO != null)
                {
                    NombreUsuario.Append(
                        string.Format("{0} {1} {2}",
                        !string.IsNullOrEmpty(Usuario.EMPLEADO.PERSONA.NOMBRE) ? Usuario.EMPLEADO.PERSONA.NOMBRE.TrimEnd() : string.Empty,
                        !string.IsNullOrEmpty(Usuario.EMPLEADO.PERSONA.PATERNO) ? Usuario.EMPLEADO.PERSONA.PATERNO.TrimEnd() : string.Empty,
                        !string.IsNullOrEmpty(Usuario.EMPLEADO.PERSONA.MATERNO) ? Usuario.EMPLEADO.PERSONA.MATERNO.TrimEnd() : string.Empty)
                    );
                }
            }
            return NombreUsuario.ToString();
        }

        public async void GenerarReporte()
        {
            var lIngresosEgresos = new List<cIngresoEgreso>();
            var datosReporte = new List<cReporteDatos>();
            var centro = new CENTRO();
            var Mes_Seleccionado = ObtenerMes();
            var usuario = new USUARIO();
            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
            {
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    ReportViewerVisible = Visibility.Collapsed;
                }));

                try
                {
                    usuario = new cUsuario().ObtenerUsuario(GlobalVar.gUsr);
                    centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();
                    datosReporte.Add(new cReporteDatos()
                    {
                        Encabezado1 = Parametro.ENCABEZADO1.Trim(),
                        Encabezado2 = Parametro.ENCABEZADO2.Trim(),
                        Encabezado3 = Parametro.ENCABEZADO3.Trim(),
                        Titulo = "INGRESOS/EGRESOS",
                        Logo1 = Parametro.REPORTE_LOGO1,
                        Logo2 = Parametro.REPORTE_LOGO2,
                        Centro = centro.DESCR.Trim().ToUpper(),
                    });



                    lIngresosEgresos = new cIngreso().
                        ObtenerTodosPorFecha(
                        SeleccionMesSelected ? true : false,
                        SeleccionMesSelected ? (int?)Mes_Seleccionado : null,
                        SeleccionMesSelected ? (DateTime?)Fechas.GetFechaDateServer : null,
                        SeleccionMesSelected ? null : (DateTime?)SelectedFechaInicial,
                        SeleccionMesSelected ? null : (DateTime?)SelectedFechaFinal).
                        Select(s => new cIngresoEgreso()
                        {
                            Sexo = !string.IsNullOrEmpty(s.IMPUTADO.SEXO) ? (s.IMPUTADO.SEXO.TrimEnd() == FEMENINO ? "FEMENINO" : "MASCULINO") : "INDEFINIDO",
                            Tipo = s.FEC_SALIDA_CERESO.HasValue ? "EGRESO" : "INGRESO"
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

            try
            {
                Reporte.LocalReport.ReportPath = "Reportes/rIngresosEgresos.rdlc";
                Reporte.LocalReport.DataSources.Clear();

                ReportDataSource ReportDataSource_Encabezado = new ReportDataSource();
                ReportDataSource_Encabezado.Name = "DataSet1";
                ReportDataSource_Encabezado.Value = datosReporte;

                ReportDataSource ReportDataSource = new ReportDataSource();
                ReportDataSource.Name = "DataSet2";
                ReportDataSource.Value = lIngresosEgresos;

                Reporte.LocalReport.DataSources.Add(ReportDataSource_Encabezado);
                Reporte.LocalReport.DataSources.Add(ReportDataSource);

                Reporte.LocalReport.SetParameters(new ReportParameter(("Usuario"), ObtenerNombreUsuario(usuario)));
                Reporte.LocalReport.SetParameters(new ReportParameter(("FechaActual"), string.Format("{0} DE {1} DE {2}", Fechas.GetFechaDateServer.Day, ((eMesesAnio)Fechas.GetFechaDateServer.Month).ToString(), Fechas.GetFechaDateServer.Year)));
                Reporte.LocalReport.SetParameters(new ReportParameter(("Centro"), centro.DESCR.Trim().ToUpper()));
                Reporte.LocalReport.SetParameters(new ReportParameter(("ComandanteEstatal"), Parametro.COMANDANTE_ESTATAL_CENTROS));
                Reporte.LocalReport.SetParameters(new ReportParameter(("FechaInicial"), string.Format("{0}/{1}/{2}", SelectedFechaInicial.Day, SelectedFechaInicial.Month, SelectedFechaInicial.Year)));
                Reporte.LocalReport.SetParameters(new ReportParameter(("FechaFinal"), string.Format("{0}/{1}/{2}", SelectedFechaFinal.Day, SelectedFechaFinal.Month, SelectedFechaFinal.Year)));

                Reporte.Refresh();
                Reporte.RefreshReport();
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
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_INGRESOS_EGRESOS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
