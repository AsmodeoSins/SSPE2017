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
    public partial class BitacoraIngresosVisitaFamiliarViewModel : ValidationViewModelBase
    {

        #region Constructor
        public BitacoraIngresosVisitaFamiliarViewModel() { }
        #endregion

        #region Metodos
        private async void SwitchClick(Object obj)
        {
            if (!pConsultar)
            {
                ReportViewerVisible = Visibility.Collapsed;
                new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                return;
            }
            if (!base.HasErrors)
            {
                ReportViewerVisible = Visibility.Collapsed;
                await StaticSourcesViewModel.CargarDatosMetodoAsync(GenerarReporteNew); 
            }
            else
            {
                ReportViewerVisible = Visibility.Collapsed;
                await new Dialogos().ConfirmacionDialogoReturn("Validación", base.Error);
                ReportViewerVisible = Visibility.Visible;
            }
        }

        private void OnLoad(BitacoraIngresosVisitaFamiliarView Window = null)
        {                   
            try
            {
                Reporte = Window.Report;
                ConfiguraPermisos();
                ValidarFiltros();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la bitacora de recepción legal", ex);
            }
        }
        #endregion

        #region Bitacora Recepcion Legal
        private void GenerarReporteNew() 
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
                    Titulo = "Bitácora de Recepción de Visita Familiar",
                    Logo1 = Parametro.REPORTE_LOGO1,
                    Logo2 = Parametro.REPORTE_LOGO2,
                    Centro = centro.DESCR.Trim().ToUpper(),
                });
                var lstContenido = new List<cBitacoraVisitaFamiliar>();
                var resultado = new cAduanaIngreso().ObtenerTodosPorTipoVisita(FFechaInicio.Value.Date, GlobalVar.gCentro, (short)enumTipoPersona.PERSONA_VISITA);
                if (resultado != null)
                {
                    foreach (var r in resultado)
                    {
                        var o = new cBitacoraVisitaFamiliar();
                        o.HoraIngreso = string.Format("{0:hh:mm tt}",r.ADUANA.ENTRADA_FEC);
                        o.NoVisitante = r.ADUANA.ID_PERSONA.ToString();//r.ID_PERSONA.ToString();
                        o.Visitante =  string.Format("{0} {1} {2}", 
                            !string.IsNullOrEmpty(r.ADUANA.PERSONA.NOMBRE) ? r.ADUANA.PERSONA.NOMBRE.Trim() : string.Empty, 
                            !string.IsNullOrEmpty(r.ADUANA.PERSONA.PATERNO) ? r.ADUANA.PERSONA.PATERNO.Trim() : string.Empty, 
                            !string.IsNullOrEmpty(r.ADUANA.PERSONA.MATERNO) ? r.ADUANA.PERSONA.MATERNO.Trim() : string.Empty);
                        o.Expendiente = string.Format("{0}/{1}",r.ID_ANIO,r.ID_IMPUTADO);
                        o.Interno = string.Format("{0} {1} {2}", 
                            !string.IsNullOrEmpty(r.INGRESO.IMPUTADO.NOMBRE) ? r.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty, 
                            !string.IsNullOrEmpty(r.INGRESO.IMPUTADO.PATERNO) ? r.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty, 
                            !string.IsNullOrEmpty(r.INGRESO.IMPUTADO.MATERNO) ? r.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty);
                        o.Ubicacion = string.Format("{0}-{1}-{2}-{3}",
                                          r.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim(),
                                          r.INGRESO.CAMA.CELDA.SECTOR.DESCR.Trim(),
                                          r.INGRESO.CAMA.ID_CELDA.Trim(),
                                          r.INGRESO.CAMA.ID_CAMA);
                        o.AutorizoEntrada = r.ADUANA.ID_USUARIO;
                        o.HoraSalida = string.Format("{0:hh:mm tt}", r.ADUANA.SALIDA_FEC);
                        o.AutorizoSalida = r.ADUANA.ID_USUARIO;
                        o.TipoVisita = r.INTIMA == "S" ? "INTIMA" : "FAMILIAR";
                        lstContenido.Add(o);
                    }
                }
                //ARMAMOS EL REPORTE
                Reporte.LocalReport.ReportPath = "Reportes/rBitacoraIngresoVisitaFamiliarNew.rdlc";
                Reporte.LocalReport.DataSources.Clear();

                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds1.Name = "DataSet1";
                rds1.Value = lstContenido;
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
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error", ex);
            }
        }
        
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
                    Titulo = "Bitácora de Recepción de Visita Familiar",
                    Logo1 = Parametro.REPORTE_LOGO1,
                    Logo2 = Parametro.REPORTE_LOGO2,
                    Centro = centro.DESCR.Trim().ToUpper(),
                });


                var lstIngresos = new List<cBitacoraIngresosVisitaLegal>();
                var ingresos = new cAduanaIngreso().ObtenerTodosPorTipoVisita(FFechaInicio.Value.Date,FFechaFin.Value.Date,GlobalVar.gCentro,(short)enumTipoPersona.PERSONA_VISITA);
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
                Reporte.LocalReport.ReportPath = "Reportes/rBitacoraIngresoVisitaFamiliar.rdlc";
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

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.BITACORA_VISITA_FAMILIAR.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
