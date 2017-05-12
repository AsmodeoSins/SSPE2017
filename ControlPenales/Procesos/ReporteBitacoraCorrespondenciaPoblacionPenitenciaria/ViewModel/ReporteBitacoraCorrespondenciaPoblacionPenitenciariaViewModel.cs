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
    public partial class ReporteBitacoraCorrespondenciaPoblacionPenitenciariaViewModel : ValidationViewModelBase
    {
        public async void OnLoad(ReporteBitacoraCorrespondenciaPoblacionPenitenciaria Window)
        {
            try
            {
                ConfiguraPermisos();
                Ventana = Window;
                Reporte = Ventana.ReportBitacoraCorrespondenciaPoblacionPenitenciaria;

                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ReportViewerVisible = Visibility.Collapsed;
                    }));
                    SelectedFecha = Fechas.GetFechaDateServer;
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
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        break;
                    }
                    Reporte.Reset();
                    GenerarReporte();
                    break;
            }
        }

        public async void GenerarReporte()
        {
            var lCorrespondencia = new List<cBitacoraCorrepondenciaPoblacionPenitenciaria>();
            var datosReporte = new List<cReporteDatos>();

            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ReportViewerVisible = Visibility.Collapsed;
                    }));
                    var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();

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

                    lCorrespondencia = new cCorrespondencia().
                    ObtenerTodosRecibidos(SelectedFecha).
                    AsEnumerable().
                    Select(s =>
                    new cBitacoraCorrepondenciaPoblacionPenitenciaria()
                    {
                        FechaDeposito = string.Format("{0}/{1}/{2}", s.RECEPCION_FEC.Value.Day, s.RECEPCION_FEC.Value.Month, s.RECEPCION_FEC.Value.Year),
                        FechaEntrega = s.ENTREGA_FEC.HasValue ? string.Format("{0}/{1}/{2}", s.ENTREGA_FEC.Value.Day, s.ENTREGA_FEC.Value.Month, s.ENTREGA_FEC.Value.Year) : string.Empty,
                        LoginEntrega = string.Empty,
                        Observaciones = !string.IsNullOrEmpty(s.OBSERV) ? s.OBSERV : string.Empty,

                        Id_Anio = s.ID_ANIO,
                        Id_Imputado = s.ID_IMPUTADO,
                        EdificioDescr = s.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.TrimEnd(),
                        SectorDescr = s.INGRESO.CAMA.CELDA.SECTOR.DESCR.TrimEnd(),
                        Celda = s.INGRESO.ID_UB_CELDA.TrimStart().TrimEnd(),
                        Paterno_Ingreso = !string.IsNullOrEmpty(s.INGRESO.IMPUTADO.PATERNO) ? s.INGRESO.IMPUTADO.PATERNO.TrimEnd() : string.Empty,
                        Materno_Ingreso = !string.IsNullOrEmpty(s.INGRESO.IMPUTADO.MATERNO) ? s.INGRESO.IMPUTADO.MATERNO.TrimEnd() : string.Empty,
                        Nombre_Ingreso = !string.IsNullOrEmpty(s.INGRESO.IMPUTADO.NOMBRE) ? s.INGRESO.IMPUTADO.NOMBRE.TrimEnd() : string.Empty,

                        Materno_Depositante = s.PERSONA != null ? (!string.IsNullOrEmpty(s.PERSONA.MATERNO) ? s.PERSONA.MATERNO.TrimEnd() : string.Empty) : string.Empty,
                        Paterno_Depositante = s.PERSONA != null ? (!string.IsNullOrEmpty(s.PERSONA.PATERNO) ? s.PERSONA.PATERNO.TrimEnd() : string.Empty) : string.Empty,
                        Nombre_Depositante = s.PERSONA != null ? (!string.IsNullOrEmpty(s.PERSONA.NOMBRE) ? s.PERSONA.NOMBRE.TrimEnd() : string.Empty) : string.Empty,

                        Paterno_Remitente = s.PERSONA1 != null ? (!string.IsNullOrEmpty(s.PERSONA1.PATERNO) ? s.PERSONA1.PATERNO.TrimEnd() : string.Empty) : string.Empty,
                        Materno_Remitente = s.PERSONA1 != null ? (!string.IsNullOrEmpty(s.PERSONA1.MATERNO) ? s.PERSONA1.MATERNO.TrimEnd() : string.Empty) : string.Empty,
                        Nombre_Remitente = s.PERSONA1 != null ? (!string.IsNullOrEmpty(s.PERSONA1.NOMBRE) ? s.PERSONA1.NOMBRE.TrimEnd() : string.Empty) : string.Empty
                    }).ToList();
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


            #region Reporte
            Reporte.LocalReport.ReportPath = "Reportes/rBitacoraCorrespondenciaPoblacionPenitenciaria.rdlc";
            Reporte.LocalReport.DataSources.Clear();

            ReportDataSource ReportDataSource_Encabezado = new ReportDataSource();
            ReportDataSource_Encabezado.Name = "DataSet1";
            ReportDataSource_Encabezado.Value = datosReporte;

            ReportDataSource ReportDataSource_Bitacora = new ReportDataSource();
            ReportDataSource_Bitacora.Name = "DataSet2";
            ReportDataSource_Bitacora.Value = lCorrespondencia;

            Reporte.LocalReport.DataSources.Add(ReportDataSource_Encabezado);
            Reporte.LocalReport.DataSources.Add(ReportDataSource_Bitacora);
            #endregion

            #region Parametros
            Reporte.LocalReport.SetParameters(new ReportParameter("Fecha", string.Format("{0}/{1}/{2}", SelectedFecha.Month, SelectedFecha.Day, SelectedFecha.Year)));
            #endregion

            Reporte.Refresh();
            Reporte.RefreshReport();
        }

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_BIT_CORRESP_POB_PENIT.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
