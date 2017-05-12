using Microsoft.Reporting.WinForms;
using SSP.Controlador.Catalogo.Justicia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlPenales
{
    public partial class PoblacionExtranjeraViewModel : ValidationViewModelBase
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
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(GenerarReporteQuery);
                        ReportViewerVisible = Visibility.Visible;
                        break;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
            }
        }


        private void GenerarReporteQuery()
        { 
            try
            {
                var grafica = new List<cGraficaPoblacionExtranjeraF>();
                var datosReporte = new List<cReporteDatos>();
                datosReporte.Add(new cReporteDatos()
                {
                    Encabezado1 = Parametro.ENCABEZADO1,
                    Encabezado2 = Parametro.ENCABEZADO2,
                    Encabezado3 = Parametro.ENCABEZADO3,
                    Logo1 = Parametro.REPORTE_LOGO1,
                    Logo2 = Parametro.REPORTE_LOGO2,
                    Titulo = "Población Extranjera"
                });

                var reporte = new cPaises().ObtenerPoblacionExtranjera(GlobalVar.gCentro).Select(w => new cPoblacionExtranjera() { 
                  Pais = !string.IsNullOrEmpty(w.PAIS) ? w.PAIS : " ",
                  ComunProcesado = w.PROCESADO_COMUN,
                  SinFueroIndiciado = w.INDICIADO_SIN_FUERO,
                  SinFueroProcesado = w.PROCESADO_SIN_FUERO,
                  SinFueroSentenciado = w.SENTENCIADO_SIN_FUERO
                });

                if (reporte != null)
                {
                    foreach (var r in reporte)
                    {
                        grafica.Add(new cGraficaPoblacionExtranjeraF()
                        {
                           Pais = r.Pais,
                           Comun = r.ComunProcesado,
                           SinFuero = (r.SinFueroIndiciado + r.SinFueroProcesado + r.SinFueroSentenciado)
                        });
                    }
                }

                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    Reporte.Reset();
                }));
                Reporte.LocalReport.ReportPath = "Reportes/rPoblacionExtranjero.rdlc";
                Reporte.LocalReport.DataSources.Clear();

                ReportDataSource rds1 = new ReportDataSource();
                rds1.Name = "DataSet1";
                rds1.Value = datosReporte;
                Reporte.LocalReport.DataSources.Add(rds1);

                ReportDataSource rds2 = new ReportDataSource();
                rds2.Name = "DataSet2";
                rds2.Value = reporte;
                Reporte.LocalReport.DataSources.Add(rds2);

                ReportDataSource rds3 = new ReportDataSource();
                rds3.Name = "DataSet4";
                rds3.Value = grafica;
                Reporte.LocalReport.DataSources.Add(rds3);

                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    Reporte.Refresh();
                    Reporte.RefreshReport();
                }));
            }
            catch(Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar reporte", ex);
            }
        }

        private void GenerarReporte()
        {
            try
            {
                
                //NOTA: ANIO TEMPORALMENTE FIJO SOLO PARA PRUEBAS DEBIDO A QUE NO HAY REGISTROS PARA EL ANIO 2016
                //DESCOMENTAR MAS ADELANTE
                var mes = Fechas.GetFechaDateServer.Month;
                var anio = Fechas.GetFechaDateServer.Year;
                var datosReporte = new List<cReporteDatos>();
                datosReporte.Add(new cReporteDatos()
                {
                    Encabezado1 = Parametro.ENCABEZADO1,
                    Encabezado2 = Parametro.ENCABEZADO2,
                    Encabezado3 = Parametro.ENCABEZADO3,
                    Logo1 = Parametro.REPORTE_LOGO1,
                    Logo2 = Parametro.REPORTE_LOGO2,
                    Titulo = "Población Extranjera"
                });
                var lst_internos_causa_penal_delito = new cCausaPenalDelito().ObtenerTodos().ToList();
                var internos_lista = new cIngreso().ObtenerIngresosActivos(GlobalVar.gCentro).Where(w => w.FEC_INGRESO_CERESO.Value.Year == anio && w.FEC_INGRESO_CERESO.Value.Month == mes).ToList();
                var lst_imp = new List<cPoblacionExtranjera>();
                var lst_imp_grafica = new List<cGraficaPoblacionExtranjeraF>();
                //var lst_imp_grafica_f = new List<cReporteGraficaPoblacionEntidadF>();

                foreach (var item in internos_lista)
                {
                    var interno = lst_internos_causa_penal_delito.Where(w => w.ID_CENTRO == item.ID_CENTRO && w.ID_ANIO == item.ID_ANIO && w.ID_IMPUTADO == item.ID_IMPUTADO && w.ID_INGRESO == w.ID_INGRESO).FirstOrDefault();
                    if (interno == null)
                    {
                        var obj = new cPoblacionExtranjera();
                        obj.Pais = item.IMPUTADO.PAIS_NACIONALIDAD == null ? string.Empty : item.IMPUTADO.PAIS_NACIONALIDAD.PAIS;
                        obj.Causa = "Sin Fuero";
                        obj.SinFueroIndiciado = item.ID_CLASIFICACION_JURIDICA == "I" ? 1 : 0;
                        obj.SinFueroProcesado = item.ID_CLASIFICACION_JURIDICA == "2" ? 1 : 0;
                        obj.SinFueroSentenciado = item.ID_CLASIFICACION_JURIDICA == "3" ? 1 : 0;
                        lst_imp.Add(obj);
                    }
                }

                foreach (var item in lst_internos_causa_penal_delito)
                {
                    var interno = internos_lista.Where(w => w.ID_CENTRO == item.ID_CENTRO && w.ID_ANIO == item.ID_ANIO && w.ID_IMPUTADO == item.ID_IMPUTADO && w.ID_INGRESO == w.ID_INGRESO).FirstOrDefault();
                    if (interno != null)
                    {
                        if (item.CAUSA_PENAL.ID_ESTATUS_CP.Value == 1 && item.ID_FUERO == "C")
                        {
                            var obj = new cPoblacionExtranjera();
                            obj.Pais = item.CAUSA_PENAL.INGRESO.IMPUTADO.PAIS_NACIONALIDAD.PAIS;
                            obj.Causa = "Comun";
                            obj.ComunProcesado = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "I" ? 1 : 0;
                            lst_imp.Add(obj);
                        }
                    }
                }
                //Para alimentar grafica Masculino
                foreach (var item in lst_imp)
                {

                    if (item.Causa == "Comun")
                    {
                        if (item.ComunProcesado > 0)
                        {
                            var obj = new cGraficaPoblacionExtranjeraF();
                            obj.Pais = item.Pais;
                            obj.Causa = "Procesado Fuero Comun";
                            obj.Comun += item.ComunProcesado;
                            lst_imp_grafica.Add(obj);
                        }
                    }
                    if (item.Causa == "Sin Fuero")
                    {
                        if (item.SinFueroIndiciado > 0)
                        {
                            var obj = new cGraficaPoblacionExtranjeraF();
                            obj.Pais = item.Pais;
                            obj.Causa = "Indiciado sin Fuero";
                            obj.Comun += item.SinFueroIndiciado;
                            lst_imp_grafica.Add(obj);
                        }
                        if (item.SinFueroProcesado > 0)
                        {
                            var obj = new cGraficaPoblacionExtranjeraF();
                            obj.Pais = item.Pais;
                            obj.Causa = "Procesado sin Fuero";
                            obj.Comun += item.SinFueroProcesado;
                            lst_imp_grafica.Add(obj);
                        }
                        if (item.SinFueroSentenciado > 0)
                        {
                            var obj = new cGraficaPoblacionExtranjeraF();
                            obj.Pais = item.Pais;
                            obj.Causa = "Sentenciado sin Fuero";
                            obj.Comun += item.SinFueroSentenciado;
                            lst_imp_grafica.Add(obj);
                        }
                    }
                }
                var results = lst_imp.GroupBy(n => n.Pais).
                     Select(group =>
                         new
                         {
                             Pais = group.Key,
                             Sexo = group.ToList(),
                         });

                var lst_imp2 = new List<cPoblacionExtranjera>();
                foreach (var row in results)
                {
                    var obj = new cPoblacionExtranjera();
                    obj.Pais = row.Pais;
                    foreach (var item in row.Sexo)
                    {
                        obj.ComunProcesado += item.ComunProcesado;
                        obj.SinFueroIndiciado += item.SinFueroIndiciado;
                        obj.SinFueroProcesado += item.SinFueroProcesado;
                        obj.SinFueroSentenciado += item.SinFueroSentenciado;
                    }
                    lst_imp2.Add(obj);
                }
                Reporte.LocalReport.ReportPath = "../../Reportes/rPoblacionExtranjero.rdlc";
                Reporte.LocalReport.DataSources.Clear();

                ReportDataSource rds1 = new ReportDataSource();
                rds1.Name = "DataSet1";
                rds1.Value = datosReporte;
                Reporte.LocalReport.DataSources.Add(rds1);

                ReportDataSource rds2 = new ReportDataSource();
                rds2.Name = "DataSet2";
                rds2.Value = lst_imp2;
                Reporte.LocalReport.DataSources.Add(rds2);

                ReportDataSource rds3 = new ReportDataSource();
                rds3.Name = "DataSet4";
                rds3.Value = lst_imp_grafica;
                Reporte.LocalReport.DataSources.Add(rds3);

                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    Reporte.Refresh();
                    Reporte.RefreshReport();
                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar reporte.", ex);
            }
        }

        private void OnLoad(PoblacionExtranjeraView window)
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
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_POBLACION_EXTRANJERA.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
