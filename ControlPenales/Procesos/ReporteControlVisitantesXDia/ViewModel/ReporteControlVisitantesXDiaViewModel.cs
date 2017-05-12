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
    public partial class ReporteControlVisitantesXDiaViewModel : ValidationViewModelBase
    {
        public async void OnLoad(ReporteControlVisitantesXDia Window)
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

        public int ObtenerEdad(DateTime Fecha_Nacimiento)
        {
            return (Fecha_Nacimiento.Month < Fechas.GetFechaDateServer.Month) ?
                Fechas.GetFechaDateServer.Year - Fecha_Nacimiento.Year :
                ((Fecha_Nacimiento.Month == Fechas.GetFechaDateServer.Month &&
                Fecha_Nacimiento.Day <= Fechas.GetFechaDateServer.Day) ? (Fechas.GetFechaDateServer.Year - Fecha_Nacimiento.Year) :
                (Fechas.GetFechaDateServer.Year - Fecha_Nacimiento.Year) - 1);
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
            var datosReporte = new List<cReporteDatos>();
            var centro = new CENTRO();
            var lVisitantes = new List<cControlVisitante>();
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
                        Titulo = "CONTROL VISITANTES POR DÍA (ÍNTIMA)",
                        Logo1 = Parametro.REPORTE_LOGO1,
                        Logo2 = Parametro.REPORTE_LOGO2,
                        Centro = centro.DESCR.Trim().ToUpper(),
                    });


                    lVisitantes = new cAduana().
                        ObtenerVisitantesXDiaPorFecha(SelectedFechaInicial, SelectedFechaFinal).
                        AsEnumerable().
                        Select(s => new cControlVisitante()
                        {
                            Categoria = ObtenerEdad(s.PERSONA.FEC_NACIMIENTO.Value) < MAYORIA_DE_EDAD ? MENORES : (s.PERSONA.SEXO == FEMENINO ? MUJERES : HOMBRES),
                            FechaEntrada = s.ENTRADA_FEC.Value.Date
                        }).
                        ToList();
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

            Reporte.LocalReport.ReportPath = "Reportes/rControlVisitantesXDia.rdlc";
            Reporte.LocalReport.DataSources.Clear();

            ReportDataSource ReportDataSource_Encabezado = new ReportDataSource();
            ReportDataSource_Encabezado.Name = "DataSet1";
            ReportDataSource_Encabezado.Value = datosReporte;

            ReportDataSource ReportDataSource = new ReportDataSource();
            ReportDataSource.Name = "DataSet2";
            ReportDataSource.Value = lVisitantes;

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

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_CONTROL_VISITANTES_DIA.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
