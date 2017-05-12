using ControlPenales;
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
    public partial class BitacoraRecepcionLegalViewModel : ValidationViewModelBase// : ValidationViewModelBase,IPageViewModel
    {

        #region Constructor
        public BitacoraRecepcionLegalViewModel(){}
        #endregion

        #region Metodos
        private async void SwitchClick(Object obj)
        {
            if (!base.HasErrors)
                GenerarReporte();
            else
            {
                ReportViewerVisible = Visibility.Collapsed;
                await new Dialogos().ConfirmacionDialogoReturn("Validación", base.Error);
                ReportViewerVisible = Visibility.Visible;
            }
        }

        private void OnLoad(BitacoraRecepcionLegalView Window = null)
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

                var lstBRVL = new List<cBitacoraRecepcionVisitaLegal>();
                var aduana = new cAduana().ObtenerTodos(GlobalVar.gCentro, FFechaInicio.Value.Date, FFechaFin.Value.Date);
                if (aduana != null)
                {
                    foreach (var a in aduana)
                    {
                        var obj = new cBitacoraRecepcionVisitaLegal();
                        obj.FechaInicio = string.Format("{0:dd/MM/yyyy}", a.ENTRADA_FEC);
                        obj.HoraInicio = string.Format("{0:hh:mm tt}", a.ENTRADA_FEC);
                        obj.Abogado = string.Format("Abogado: {0} {1} {2}", !string.IsNullOrEmpty(a.PERSONA.NOMBRE) ? a.PERSONA.NOMBRE.Trim() : string.Empty, !string.IsNullOrEmpty(a.PERSONA.PATERNO) ? a.PERSONA.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(a.PERSONA.MATERNO) ? a.PERSONA.MATERNO : string.Empty);
                        obj.Autorizo = "SYSTEM";
                        obj.FechaFin = string.Format("{0:dd/MM/yyyy}", a.SALIDA_FEC);
                        obj.HoraFin = string.Format("{0:hh:mm tt}", a.SALIDA_FEC);
                        if (a.ADUANA_INGRESO.Count > 0)
                        {
                            foreach (var ai in a.ADUANA_INGRESO)
                            {
                                    //if(ai.ADUANA_INGRESO_CP != null)
                                    //{
                                    //    var exp = string.Empty;
                                    //    foreach (var e in ai.ADUANA_INGRESO_CP)
                                    //    { 
                                    //        if(!string.IsNullOrEmpty(obj.Expediente))
                                    //            exp = exp + ",";
                                    //        obj.Expediente = obj.Expediente + string.Format("{0}/{1}", e.CAUSA_PENAL.CP_ANIO, e.CAUSA_PENAL.CP_FOLIO);   
                                    //    }
                                    //    if (!string.IsNullOrEmpty(exp))
                                    //    {
                                    //        obj.Expediente = string.Format("Expediente:{0}",exp);
                                    //    }
                                    //}
                                obj.Expediente = string.Format("Expediente:{0}/{1}", ai.INGRESO.ID_ANIO, ai.INGRESO.ID_IMPUTADO);
                                obj.Interno = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(ai.INGRESO.IMPUTADO.NOMBRE) ? ai.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty, !string.IsNullOrEmpty(ai.INGRESO.IMPUTADO.PATERNO) ? ai.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(ai.INGRESO.IMPUTADO.MATERNO) ? ai.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty);
                            }
                        }
                        lstBRVL.Add(obj);
                    }
                }

                //ARMAMOS EL REPORTE
                Reporte.LocalReport.ReportPath = "Reportes/rBitacoraRecepcionVisitaLegal.rdlc";
                Reporte.LocalReport.DataSources.Clear();

                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds1.Name = "DataSet1";
                rds1.Value = lstBRVL;
                Reporte.LocalReport.DataSources.Add(rds1);

                Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds2.Name = "DataSet2";
                rds2.Value = datosReporte;
                Reporte.LocalReport.DataSources.Add(rds2);
                Reporte.RefreshReport();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar reporte", ex);
            }
        }
        #endregion
    }
}
