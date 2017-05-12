using ControlPenales;
using ControlPenales.BiometricoServiceReference;
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
    public partial class BitacoraIngresosVisitaLegalViewModel : ValidationViewModelBase
    {

        #region Constructor
        public BitacoraIngresosVisitaLegalViewModel() { }
        #endregion

        #region Metodos
        private async void SwitchClick(Object obj)
        {
            if (!base.HasErrors)
                await StaticSourcesViewModel.CargarDatosMetodoAsync(GenerarReporte);
            else
            {
                ReportViewerVisible = Visibility.Collapsed;
                await new Dialogos().ConfirmacionDialogoReturn("Validación", base.Error);
                ReportViewerVisible = Visibility.Visible;
            }
        }

        private void OnLoad(BitacoraIngresosVisitaLegalView Window = null)
        {                   
            try
            {
                Reporte = Window.Report;
                ValidarFiltros();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la bitacora de recepción legal", ex);
            }
        }
        #endregion

        #region Bitacora Recepcion Legal
        private void GenerarReporte()
        {
            try
            {
                var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();

                var datosReporte = new List<cReporteDatos>();
                datosReporte.Add(new cReporteDatos()
                {
                    Encabezado1 = Parametro.ENCABEZADO1.Trim(),
                    Encabezado2 = Parametro.ENCABEZADO2.Trim(),
                    Encabezado3 = Parametro.ENCABEZADO3.Trim(),
                    Titulo = "Bitácora de Recepción de Visita Legal",
                    Logo1 = Parametro.REPORTE_LOGO1,
                    Logo2 = Parametro.REPORTE_LOGO2,
                    Centro = centro.DESCR.Trim().ToUpper(),
                });


                var lstIngresos = new List<cBitacoraIngresosVisitaLegal>();
                var ingresos = new cAduanaIngreso().ObtenerTodosPorTipoVisita(FFechaInicio.Value.Date, FFechaFin.Value.Date, GlobalVar.gCentro, (short)enumTipoPersona.PERSONA_ABOGADO);
                if(ingresos != null)
                {
                    foreach (var ai in ingresos)
                    {
                        var obj = new cBitacoraIngresosVisitaLegal();
                        obj.FechaHoraEntrada = string.Format("{0:dd/MM/yyyy hh:mm tt}",ai.ADUANA.ENTRADA_FEC);
                        obj.FechaHoraSalida = string.Format("{0:dd/MM/yyyy hh:mm tt}", ai.ADUANA.SALIDA_FEC);
                        obj.Expediente = string.Format("{0}/{1}",ai.ID_ANIO,ai.ID_IMPUTADO);
                        obj.Interno = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(ai.INGRESO.IMPUTADO.NOMBRE) ? ai.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty, !string.IsNullOrEmpty(ai.INGRESO.IMPUTADO.PATERNO) ? ai.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(ai.INGRESO.IMPUTADO.MATERNO) ? ai.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty);
                        obj.Abogado = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(ai.ADUANA.PERSONA.NOMBRE) ? ai.ADUANA.PERSONA.NOMBRE.Trim() : string.Empty, !string.IsNullOrEmpty(ai.ADUANA.PERSONA.PATERNO) ? ai.ADUANA.PERSONA.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(ai.ADUANA.PERSONA.MATERNO) ? ai.ADUANA.PERSONA.MATERNO.Trim() : string.Empty);
                        lstIngresos.Add(obj);   
                    }
                }

                //ARMAMOS EL REPORTE
                Reporte.LocalReport.ReportPath = "Reportes/rBitacoraIngresoVisitaLegal.rdlc";
                Reporte.LocalReport.DataSources.Clear();

                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds1.Name = "DataSet1";
                rds1.Value = lstIngresos;
                Reporte.LocalReport.DataSources.Add(rds1);

                Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds2.Name = "DataSet2";
                rds2.Value = datosReporte;
                Reporte.LocalReport.DataSources.Add(rds2);
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    Reporte.RefreshReport();
                    ReportViewerVisible = Visibility.Visible;
                }));
                
                
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar reporte", ex);
            }
        }
        #endregion
    }
}
