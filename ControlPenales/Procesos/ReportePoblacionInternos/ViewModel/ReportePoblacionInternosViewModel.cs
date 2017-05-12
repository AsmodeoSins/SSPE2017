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
    public partial class ReportePoblacionInternosViewModel : ValidationViewModelBase
    {
        public async void OnLoad(ReportePoblacionInternos Window)
        {
            try{
                ConfiguraPermisos();
            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
            {
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    ReportViewerVisible = Visibility.Collapsed;
                }));
                Ventana = Window;
                Reporte = Ventana.ReportPoblacionInternos;
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    ReportViewerVisible = Visibility.Visible;
                }));
            });
            }
            catch(Exception ex)
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
            }
        }

        public string ObtenerFuero(string ID_FUERO)
        {
            var Fuero = string.Empty;
            switch (ID_FUERO)
            {
                case "F":
                    Fuero = "FEDERAL";
                    break;
                case "C":
                    Fuero = "COMUN";
                    break;
                case "M":
                    Fuero = "MILITAR";
                    break;
                default:
                    Fuero = "SIN FUERO";
                    break;

            }
            return Fuero;
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
            var lPoblacion = new List<cPoblacionInternos>();
            var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();
            var usuario = new cUsuario().ObtenerUsuario(GlobalVar.gUsr);


            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
            {
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    ReportViewerVisible = Visibility.Collapsed;
                }));
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


                lPoblacion = new cIngreso().
                    ObtenerIngresosActivos(GlobalVar.gCentro, null, null, true).
                    ToList().
                    Select(s => new cPoblacionInternos()
                    {
                        ClasificacionJuridica = s.CLASIFICACION_JURIDICA != null ? (!string.IsNullOrEmpty(s.CLASIFICACION_JURIDICA.DESCR) ? s.CLASIFICACION_JURIDICA.DESCR.TrimEnd() : string.Empty) : string.Empty,
                        Fuero = s.CAUSA_PENAL.Where(w => w.ID_ESTATUS_CP == Parametro.ID_CAUSA_PENAL_ACTIVA).FirstOrDefault() != null ? ObtenerFuero(s.CAUSA_PENAL.Where(w => w.ID_ESTATUS_CP == Parametro.ID_CAUSA_PENAL_ACTIVA).FirstOrDefault().CP_FUERO.TrimEnd()) : "SIN FUERO",
                        Genero = s.IMPUTADO.SEXO == FEMENINO ? "FEMENINO" : "MASCULINO",
                        Id_Anio = s.ID_ANIO,
                        Id_Centro = (int)s.ID_UB_CENTRO,
                        Id_Imputado = s.ID_IMPUTADO
                    }).
                    ToList();
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    ReportViewerVisible = Visibility.Visible;
                }));
            });


            #region Reporte
            Reporte.LocalReport.ReportPath = "Reportes/rPoblacionInternos.rdlc";
            Reporte.LocalReport.DataSources.Clear();

            ReportDataSource ReportDataSource_Encabezado = new ReportDataSource();
            ReportDataSource_Encabezado.Name = "DataSet1";
            ReportDataSource_Encabezado.Value = datosReporte;

            ReportDataSource ReportDataSource = new ReportDataSource();
            ReportDataSource.Name = "DataSet2";
            ReportDataSource.Value = lPoblacion;
            Console.WriteLine(lPoblacion.Where(w => w.Genero == "MASCULINO"));

            Reporte.LocalReport.DataSources.Add(ReportDataSource);
            Reporte.LocalReport.DataSources.Add(ReportDataSource_Encabezado);
            #endregion

            #region Parametro
            Reporte.LocalReport.SetParameters(new ReportParameter(("Usuario"), ObtenerNombreUsuario(usuario)));
            Reporte.LocalReport.SetParameters(new ReportParameter(("Fecha"), string.Format("{0} DE {1} DE {2}", Fechas.GetFechaDateServer.Day, ((eMesesAnio)Fechas.GetFechaDateServer.Month).ToString(), Fechas.GetFechaDateServer.Year)));
            Reporte.LocalReport.SetParameters(new ReportParameter(("Centro"), centro.DESCR.Trim().ToUpper()));
            Reporte.LocalReport.SetParameters(new ReportParameter(("ComandanteEstatal"), Parametro.COMANDANTE_ESTATAL_CENTROS));
            #endregion

            Reporte.Refresh();
            Reporte.RefreshReport();
        }

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_POBLACION_INTERNOS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
