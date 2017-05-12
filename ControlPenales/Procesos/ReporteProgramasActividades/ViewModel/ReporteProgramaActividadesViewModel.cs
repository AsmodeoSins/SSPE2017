using Microsoft.Reporting.WinForms;
using SSP.Controlador.Catalogo.Justicia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlPenales
{
    public partial class ReporteProgramaActividadesViewModel : ValidationViewModelBase
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
                        if (FReporte == 1)
                        {
                            if (FTipo == 1)
                                GenerarReporte1();
                            else
                                GenerarReporte2();
                        }
                        else
                            GenerarReporte3();
                        break;
                }
            }
            catch (Exception ex)
            {
                ReportViewerVisible = Visibility.Collapsed;
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
            }
        }

        private void GenerarReporte1()
        {
            try
            {
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    ReportViewerVisible = Visibility.Collapsed;
                    Reporte.Reset();

                }));
                var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();
                var header = new List<cReporteDatos>();
                header.Add(new cReporteDatos()
                {
                    Encabezado1 = Parametro.ENCABEZADO1,
                    Encabezado2 = Parametro.ENCABEZADO2,
                    Encabezado3 = Parametro.ENCABEZADO3,
                    Logo1 = Parametro.REPORTE_LOGO1,
                    Logo2 = Parametro.REPORTE_LOGO2,
                    Titulo = "Programas y Actividades"
                });

                var reporte = new List<cReporteProgramaActividad>(new cGrupoParticipante().ObtenerReporteProgramasActividades(GlobalVar.gCentro).Select(w => new cReporteProgramaActividad()
                {
                  TIPO_PROGRAMA = w.TIPO_PROGRAMA,
                  TIPO_ACTIVIDAD = w.ACTIVIDAD,
                  FEMENINO = w.FEMENINO,
                  MASCULINO = w.MASCULINO,
                  NO_DEFINIDO = w.NO_DEFINIDO,
                }));

                Reporte.LocalReport.ReportPath = "Reportes/rProgramasActividades.rdlc";
                Reporte.LocalReport.DataSources.Clear();

                ReportDataSource rds1 = new ReportDataSource();
                rds1.Name = "DataSet1";
                rds1.Value = header;
                Reporte.LocalReport.DataSources.Add(rds1);

                ReportDataSource rds2 = new ReportDataSource();
                rds2.Name = "DataSet2";
                rds2.Value = reporte;
                Reporte.LocalReport.DataSources.Add(rds2);

                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    Reporte.Refresh();
                    Reporte.RefreshReport();
                    ReportViewerVisible = Visibility.Visible;
                }));


            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke((Action)(delegate{ ReportViewerVisible = Visibility.Collapsed; }));
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar reporte.", ex);
            }
        }

        private void GenerarReporte2()
        {
            try
            {
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    ReportViewerVisible = Visibility.Collapsed;
                    Reporte.Reset();

                }));
                var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();
                var header = new List<cReporteDatos>();
                header.Add(new cReporteDatos()
                {
                    Encabezado1 = Parametro.ENCABEZADO1,
                    Encabezado2 = Parametro.ENCABEZADO2,
                    Encabezado3 = Parametro.ENCABEZADO3,
                    Logo1 = Parametro.REPORTE_LOGO1,
                    Logo2 = Parametro.REPORTE_LOGO2,
                    Titulo = "Programas y Actividades"
                });

                var reporte = new List<cReporteProgramaActividad>(new cGrupoParticipante().ObtenerReporteProgramasActividades(GlobalVar.gCentro).Select(w => new cReporteProgramaActividad()
                {
                    TIPO_PROGRAMA = w.TIPO_PROGRAMA,
                    FEMENINO = w.FEMENINO,
                    MASCULINO = w.MASCULINO,
                    NO_DEFINIDO = w.NO_DEFINIDO,
                    TOTAL = w.TOTAL
                }));

                Reporte.LocalReport.ReportPath = "Reportes/rProgramasActividades2.rdlc";
                Reporte.LocalReport.DataSources.Clear();

                ReportDataSource rds1 = new ReportDataSource();
                rds1.Name = "DataSet1";
                rds1.Value = header;
                Reporte.LocalReport.DataSources.Add(rds1);

                ReportDataSource rds2 = new ReportDataSource();
                rds2.Name = "DataSet2";
                rds2.Value = reporte;
                Reporte.LocalReport.DataSources.Add(rds2);

                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    Reporte.Refresh();
                    Reporte.RefreshReport();
                    ReportViewerVisible = Visibility.Visible;
                }));


            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar reporte.", ex);
            }
        }

        private void GenerarReporte3()
        {
            try
            {
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    ReportViewerVisible = Visibility.Collapsed;
                    Reporte.Reset();

                }));
                var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();
                var header = new List<cReporteDatos>();
                header.Add(new cReporteDatos()
                {
                    Encabezado1 = Parametro.ENCABEZADO1,
                    Encabezado2 = Parametro.ENCABEZADO2,
                    Encabezado3 = Parametro.ENCABEZADO3,
                    Logo1 = Parametro.REPORTE_LOGO1,
                    Logo2 = Parametro.REPORTE_LOGO2,
                    Titulo = "Plantilla de Personal Técnico"
                });

                var reporte = new List<cPlantillaPersonalTecnico>(new cUsuario().ObtenerReportePlantillaPersonalTecnico(GlobalVar.gCentro).Select(w => new cPlantillaPersonalTecnico()
                {
                    DEPARTAMENTO = w.DEPARTAMENTO,
                    ROL = w.ROL,
                    TOTAL = w.TOTAL
                }));

                Reporte.LocalReport.ReportPath = "Reportes/rPlantillaPersonalTecnico.rdlc";
                Reporte.LocalReport.DataSources.Clear();

                ReportDataSource rds1 = new ReportDataSource();
                rds1.Name = "DataSet1";
                rds1.Value = header;
                Reporte.LocalReport.DataSources.Add(rds1);

                ReportDataSource rds2 = new ReportDataSource();
                rds2.Name = "DataSet2";
                rds2.Value = reporte;
                Reporte.LocalReport.DataSources.Add(rds2);

                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    Reporte.Refresh();
                    Reporte.RefreshReport();
                    ReportViewerVisible = Visibility.Visible;
                }));


            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar reporte.", ex);
            }
        }

        private void OnLoad(ReporteProgramaActividadesView window)
        {
            try
            {
                ConfiguraPermisos();
                Reporte = window.Report;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar pantalla.", ex);
            }
        }
        #endregion

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_POBLACION_TERCERA_EDAD.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
