using Microsoft.Reporting.WinForms;
using SSP.Controlador.Catalogo.Justicia;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlPenales
{
    public partial class ReporteActividadViewModel : ValidationViewModelBase
    {
        #region [METODOS]
        private async void ClickSwitch(object obj)
        {
            try
            {
                switch (obj.ToString())
                {
                    case "generar":
                        if (!pConsultar)
                        {
                            ReportViewerVisible = Visibility.Collapsed;
                            new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                            break;
                        }
                        ReportViewerVisible = Visibility.Collapsed;
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(GenerarReporte);
                        ReportViewerVisible = Visibility.Visible;
                        break;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
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
                    Encabezado1 = Parametro.ENCABEZADO1,
                    Encabezado2 = Parametro.ENCABEZADO2,
                    Encabezado3 = centro != null ? centro.DESCR.Trim() : string.Empty,
                    Logo1 = Parametro.REPORTE_LOGO1,
                    Logo2 = Parametro.REPORTE_LOGO2,
                    Titulo = "Actividades"
                });

                var listado = new List<cReporteActividades>();
                var grafica = new List<cReporteActividadesGrafica>();
                var titulos1 = new List<cReporteActividadesTitulos>();
                var titulos2 = new List<cReporteActividadesTitulos>();
                bool t1 = true;
                #region Color
                Random randomGen = new Random();
                KnownColor[] names = (KnownColor[])Enum.GetValues(typeof(KnownColor));
                KnownColor randomColorName = names[randomGen.Next(names.Length)];
                #endregion

                #region Totales
                int tcp , tcs , tfp , tsfim , tsfin , tsfp , tsfs;
                #endregion
                var ltp = new cTipoPrograma().ObtenerTodos();
                if (ltp != null)
                {
                    foreach (var tp in ltp)
                    {
                        tcp = tcs = tfp = tsfim = tsfin = tsfp = tsfs = 0;
                        randomColorName = names[randomGen.Next(names.Length)];
                        Color randomColor = Color.FromKnownColor(randomColorName);

                        if (t1)
                        {
                            titulos1.Add(new cReporteActividadesTitulos()
                            {
                                Titulo = tp.NOMBRE,
                                Color = randomColor.Name,
                            });
                            t1 = false;
                        }
                        else
                        {
                            titulos2.Add(new cReporteActividadesTitulos()
                            {
                                Titulo = tp.NOMBRE,
                                Color = randomColor.Name,
                            });
                            t1 = true;
                        }
                        
                        if (tp.ACTIVIDAD != null)
                        {
                            foreach (var a in tp.ACTIVIDAD)
                            {
                                //comun procesado
                                var cp = a.GRUPO_PARTICIPANTE.Count(w => w.ESTATUS == 2 && w.ID_GRUPO.HasValue && w.INGRESO.ID_CLASIFICACION_JURIDICA == "2" 
                                    && w.INGRESO.CAUSA_PENAL.FirstOrDefault(x => x.ID_ESTATUS_CP == (short)enumEstatusCausaPenal.ACTIVO && x.CP_FUERO == "C") != null);
                                tcp = tcp + cp;
                                //comun sentenciado
                                var cs = a.GRUPO_PARTICIPANTE.Count(w => w.ESTATUS == 2 && w.ID_GRUPO.HasValue && w.INGRESO.ID_CLASIFICACION_JURIDICA == "3"  
                                    && w.INGRESO.CAUSA_PENAL.FirstOrDefault(x => x.ID_ESTATUS_CP == (short)enumEstatusCausaPenal.ACTIVO && x.CP_FUERO == "C") != null);
                                tcs = tcs + cs;
                                //federal procesado
                                var fp = a.GRUPO_PARTICIPANTE.Count(w => w.ESTATUS == 2 && w.ID_GRUPO.HasValue && w.INGRESO.ID_CLASIFICACION_JURIDICA == "2" 
                                    && w.INGRESO.CAUSA_PENAL.FirstOrDefault(x => x.ID_ESTATUS_CP == (short)enumEstatusCausaPenal.ACTIVO && x.CP_FUERO == "F") != null);
                                tfp = tfp + fp;
                                //sin fuero imputado
                                var sfim = a.GRUPO_PARTICIPANTE.Count(w => w.ESTATUS == 2 && w.ID_GRUPO.HasValue 
                                    && w.INGRESO.ID_CLASIFICACION_JURIDICA == "1"
                                    && !string.IsNullOrEmpty(w.INGRESO.NUC)
                                    && w.INGRESO.CAUSA_PENAL.FirstOrDefault(x => x.ID_ESTATUS_CP == (short)enumEstatusCausaPenal.ACTIVO) == null);
                                tsfim = tsfim + sfim;
                                //sin fuero indiciado
                                var sfin = a.GRUPO_PARTICIPANTE.Count(w => w.ESTATUS == 2 && w.ID_GRUPO.HasValue
                                    && w.INGRESO.ID_CLASIFICACION_JURIDICA == "1"
                                    && string.IsNullOrEmpty(w.INGRESO.NUC)
                                    && w.INGRESO.CAUSA_PENAL.FirstOrDefault(x => x.ID_ESTATUS_CP == (short)enumEstatusCausaPenal.ACTIVO) == null);
                                tsfin = tsfin + sfin;
                                //sin fuero procesado
                                var sfp = a.GRUPO_PARTICIPANTE.Count(w => w.ESTATUS == 2 && w.ID_GRUPO.HasValue
                                    && w.INGRESO.ID_CLASIFICACION_JURIDICA == "2"
                                    && w.INGRESO.CAUSA_PENAL.FirstOrDefault(x => x.ID_ESTATUS_CP == (short)enumEstatusCausaPenal.ACTIVO) == null);
                                tsfp = tsfp + sfp;
                                //sin fuero sentenciado
                                var sfs = a.GRUPO_PARTICIPANTE.Count(w => w.ESTATUS == 2 && w.ID_GRUPO.HasValue
                                    && w.INGRESO.ID_CLASIFICACION_JURIDICA == "3"
                                    && w.INGRESO.CAUSA_PENAL.FirstOrDefault(x => x.ID_ESTATUS_CP == (short)enumEstatusCausaPenal.ACTIVO) == null);
                                tsfs = tsfs + sfs;
                                listado.Add(new cReporteActividades()
                                {
                                     TipoPrograma = tp.NOMBRE,
                                     TipoActividad = a.DESCR,
                                     ComunProcesado = cp,
                                     ComunSentenciado = cs,
                                     FederalProcesado = fp,
                                     SinFueroImputado = sfim,
                                     SinFueroIndiciado = sfin,
                                     SinFueroProcesado = sfp,
                                     SinFueroSentenciado = sfs
                                });
                            }
                        }

                        #region Grafica
                        grafica.Add(new cReporteActividadesGrafica()
                        {
                            TipoPrograma = tp.NOMBRE,
                            ClasificacionJuridica = "COMUN PROCESADO",
                            Color = randomColor.Name,
                            Total = tcp
                        });

                        grafica.Add(new cReporteActividadesGrafica()
                        {
                            TipoPrograma = tp.NOMBRE,
                            ClasificacionJuridica = "COMUN SENTENCIADO",
                            Color = randomColor.Name,
                            Total = tcs
                        });
                      
                        grafica.Add(new cReporteActividadesGrafica()
                        {
                            TipoPrograma = tp.NOMBRE,
                            ClasificacionJuridica = "FEDERAL PROCESADO",
                            Color = randomColor.Name,
                            Total = tfp
                        });
                        
                        grafica.Add(new cReporteActividadesGrafica()
                        {
                            TipoPrograma = tp.NOMBRE,
                            ClasificacionJuridica = "SIN FUERO IMPUTADO",
                            Color = randomColor.Name,
                            Total = tsfim
                        });
                        
                        grafica.Add(new cReporteActividadesGrafica()
                        {
                            TipoPrograma = tp.NOMBRE,
                            ClasificacionJuridica = "SIN FUERO INDICIADO",
                            Color = randomColor.Name,
                            Total = tsfin
                        });
                        
                        grafica.Add(new cReporteActividadesGrafica()
                        {
                            TipoPrograma = tp.NOMBRE,
                            ClasificacionJuridica = "SIN FUERO PROCESADO",
                            Color = randomColor.Name,
                            Total = tsfp
                        });
                        
                        grafica.Add(new cReporteActividadesGrafica()
                        {
                            TipoPrograma = tp.NOMBRE,
                            ClasificacionJuridica = "SIN FUERO SENTENCIADO",
                            Color = randomColor.Name,
                            Total = tsfs
                        });
                        #endregion
                    }
                }

                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    Reporte.Reset();
                }));
                Reporte.LocalReport.ReportPath = "Reportes/rActividad.rdlc";
                Reporte.LocalReport.DataSources.Clear();

                ReportDataSource rds1 = new ReportDataSource();
                rds1.Name = "DataSet1";
                rds1.Value = datosReporte;
                Reporte.LocalReport.DataSources.Add(rds1);

                ReportDataSource rds2 = new ReportDataSource();
                rds2.Name = "DataSet2";
                rds2.Value = listado;
                Reporte.LocalReport.DataSources.Add(rds2);


                ReportDataSource rds3 = new ReportDataSource();
                rds3.Name = "DataSet3";
                rds3.Value = grafica;
                Reporte.LocalReport.DataSources.Add(rds3);

                ReportDataSource rds4 = new ReportDataSource();
                rds4.Name = "DataSet4";
                rds4.Value = titulos1;
                Reporte.LocalReport.DataSources.Add(rds4);

                ReportDataSource rds5 = new ReportDataSource();
                rds5.Name = "DataSet5";
                rds5.Value = titulos2;
                Reporte.LocalReport.DataSources.Add(rds5);
               
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    Reporte.Refresh();
                    Reporte.RefreshReport();
                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
            }
        }

        private void OnLoad(ReporteActividadView window)
        {
            try
            {
                ConfiguraPermisos();
                Reporte = window.Report;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
            }
        }
        #endregion

         #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_ACTIVIDADES.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
