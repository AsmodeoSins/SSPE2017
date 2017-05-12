using ControlPenales.Clases.ControlInternos;
using Microsoft.Reporting.WinForms;
using SSP.Controlador.Catalogo.Justicia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlPenales
{
    public partial class PoblacionEntidadProcedenciaViewModel : ValidationViewModelBase
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
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(GenerarReporteConQuery);
                        ReportViewerVisible = Visibility.Visible;
                        break;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
            }
        }

        private void GenerarReporteConQuery()
        {
            try
            {
                var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();
                var lst_imp_grafica = new List<cReporteGraficaPoblacionEntidadProcedenciaM>();
                var lst_imp_grafica_f = new List<cReporteGraficaPoblacionEntidadF>();
                var datosReporte = new List<cReporteDatos>();
                datosReporte.Add(new cReporteDatos()
                {
                    Encabezado1 = Parametro.ENCABEZADO1,
                    Encabezado2 = Parametro.ENCABEZADO2,
                    Encabezado3 = Parametro.ENCABEZADO3,
                    Logo1 = Parametro.REPORTE_LOGO1,
                    Logo2 = Parametro.REPORTE_LOGO2,
                    Titulo = "Población por entidad de procedencia",
                    Centro = centro.DESCR.ToUpper().Trim()
                });

                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    Reporte.Reset();
                }));
                var reporte = new cEntidad().ObtenerPoblacionPorEntidad(GlobalVar.gCentro).Select(w => new cPoblacionEntidadProcedencia() { 
                        Estado = w.ENTIDAD,
                        //Causa
                        ComunDiscMasc = w.DISCRECIONAL_COMUN_M,
                        ComunImpFem = w.IMPUTADO_COMUN_F,
                        ComunImpMasc = w.IMPUTADO_COMUN_M,
                        ComunIndicFem = w.INDICIADO_COMUN_F,
                        ComunIndicMasc = w.INDICIADO_COMUN_M,
                        ComunProcFem = w.PROCESADO_COMUN_F,
                        ComunProcMasc = w.PROCESADO_COMUN_M,
                        ComunSentFem = w.SENTENCIADO_COMUN_F,
                        ComunSentMasc = w.SENTENCIADO_COMUN_M,
                        FederalIndicMasc = w.INDICIADO_FEDERAL_M,
                        FederalProcFem = w.PROCESADO_FEDERAL_F,
                        FederalProcMasc = w.PROCESADO_FEDERAL_M,
                        FederalSentMasc = w.SENTENCIADO_FEDERAL_M,
                        FederalSentFem = w.SENTENCIADO_FEDERAL_F,
                        SinFueroDiscIndef = w.DISCRECIONAL_SIN_FUERO,
                        SinFueroIndicFem = w.INDICIADO_SIN_FUERO_F,
                        SinFueroIndicMasc = w.INDICIADO_SIN_FUERO_M,
                        SinFueroProcMasc = w.PROCESADO_SIN_FUERO_M
                });

                if (reporte != null)
                {
                    #region Masculino
                    //Discrecional
                    lst_imp_grafica.Add(new cReporteGraficaPoblacionEntidadProcedenciaM()
                    {
                       Causa = "Discrecional",
                       Comun = reporte.Sum(w => w.ComunDiscMasc),
                       Federal = 0,
                       SinFuero = 0
                    });
                    //Imputado
                    lst_imp_grafica.Add(new cReporteGraficaPoblacionEntidadProcedenciaM()
                    {
                        Causa = "Imputado",
                        Comun = reporte.Sum(w => w.ComunImpMasc),
                        Federal = 0,
                        SinFuero = 0
                    });
                    //Indiciado
                    lst_imp_grafica.Add(new cReporteGraficaPoblacionEntidadProcedenciaM()
                    {
                        Causa = "Indiciado",
                        Comun = reporte.Sum(w => w.ComunIndicMasc),
                        Federal = reporte.Sum(w => w.FederalIndicMasc),
                        SinFuero = reporte.Sum(w => w.SinFueroIndicMasc)
                    });
                    //Procesado
                    lst_imp_grafica.Add(new cReporteGraficaPoblacionEntidadProcedenciaM()
                    {
                        Causa = "Procesado",
                        Comun = reporte.Sum(w => w.ComunProcMasc),
                        Federal = reporte.Sum(w => w.FederalProcMasc),
                        SinFuero = reporte.Sum(w => w.SinFueroProcMasc)
                    });
                    //Sentenciado
                    lst_imp_grafica.Add(new cReporteGraficaPoblacionEntidadProcedenciaM()
                    {
                        Causa = "Sentenciado",
                        Comun = reporte.Sum(w => w.ComunSentMasc),
                        Federal = reporte.Sum(w => w.FederalSentMasc),
                        SinFuero = 0
                    });
                    #endregion

                    #region Femenino
                    //Discrecional
                    lst_imp_grafica_f.Add(new cReporteGraficaPoblacionEntidadF()
                    {
                        Causa = "Discrecional",
                        Comun = 0,
                        Federal = 0,
                        SinFuero = 0
                    });
                    //Imputado
                    lst_imp_grafica_f.Add(new cReporteGraficaPoblacionEntidadF()
                    {
                        Causa = "Imputado",
                        Comun = reporte.Sum(w => w.ComunImpFem),
                        Federal = 0,
                        SinFuero = 0
                    });
                    //Indiciado
                    lst_imp_grafica_f.Add(new cReporteGraficaPoblacionEntidadF()
                    {
                        Causa = "Indiciado",
                        Comun = reporte.Sum(w => w.ComunIndicFem),
                        Federal = 0,
                        SinFuero = reporte.Sum(w => w.SinFueroIndicFem)
                    });
                    //Procesado
                    lst_imp_grafica_f.Add(new cReporteGraficaPoblacionEntidadF()
                    {
                        Causa = "Procesado",
                        Comun = reporte.Sum(w => w.ComunProcFem),
                        Federal = reporte.Sum(w => w.FederalProcFem),
                        SinFuero = 0
                    });
                    //Sentenciado
                    lst_imp_grafica_f.Add(new cReporteGraficaPoblacionEntidadF()
                    {
                        Causa = "Sentenciado",
                        Comun = reporte.Sum(w => w.ComunSentFem),
                        Federal = reporte.Sum(w => w.FederalSentFem),
                        SinFuero = 0
                    });
                    #endregion
                }
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    Reporte.Reset();
                }));
                Reporte.LocalReport.ReportPath = "../../Reportes/rPoblacionEntidadProcedencia.rdlc";
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
                rds3.Name = "DataSet3";
                rds3.Value = lst_imp_grafica;
                Reporte.LocalReport.DataSources.Add(rds3);

                ReportDataSource rds4 = new ReportDataSource();
                rds4.Name = "DataSet4";
                rds4.Value = lst_imp_grafica_f;
                Reporte.LocalReport.DataSources.Add(rds4);

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
                    Titulo = "Población por entidad de procedencia"
                });
                var lst_internos_causa_penal_delito = new cCausaPenalDelito().ObtenerTodos().ToList();
                var internos_lista = new cIngreso().ObtenerIngresosActivos(GlobalVar.gCentro).Where(w => w.FEC_INGRESO_CERESO.Value.Year == anio && w.FEC_INGRESO_CERESO.Value.Month == mes).ToList();
                var lst_imp = new List<cPoblacionEntidadProcedencia>();
                var lst_imp_grafica = new List<cReporteGraficaPoblacionEntidadProcedenciaM>();
                var lst_imp_grafica_f = new List<cReporteGraficaPoblacionEntidadF>();

                foreach (var item in internos_lista)
                {
                    var interno = lst_internos_causa_penal_delito.Where(w => w.ID_CENTRO == item.ID_CENTRO && w.ID_ANIO == item.ID_ANIO && w.ID_IMPUTADO == item.ID_IMPUTADO && w.ID_INGRESO == w.ID_INGRESO).FirstOrDefault();
                    if (interno == null)
                    {
                        var obj = new cPoblacionEntidadProcedencia();
                        //obj.Estado = item.IMPUTADO.ENTIDAD == null ? string.Empty : item.IMPUTADO.ENTIDAD.DESCR;
                        obj.Estado = item.MUNICIPIO.ENTIDAD == null ? string.Empty : item.MUNICIPIO.ENTIDAD.DESCR;
                        obj.Causa = "Sin Fuero";
                        obj.SinFueroDiscIndef = item.ID_CLASIFICACION_JURIDICA == "4" && item.IMPUTADO.SEXO == null ? 1 : 0;
                        obj.SinFueroIndicFem = item.ID_CLASIFICACION_JURIDICA == "I" && item.IMPUTADO.SEXO == "F" ? 1 : 0;
                        obj.SinFueroIndicMasc = item.ID_CLASIFICACION_JURIDICA == "I" && item.IMPUTADO.SEXO == "M" ? 1 : 0;
                        obj.SinFueroProcMasc = item.ID_CLASIFICACION_JURIDICA == "2" && item.IMPUTADO.SEXO == "M" ? 1 : 0;
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
                            var obj = new cPoblacionEntidadProcedencia();
                            //obj.Estado = item.CAUSA_PENAL.INGRESO.IMPUTADO.ENTIDAD.DESCR;
                            obj.Estado = item.CAUSA_PENAL.INGRESO.MUNICIPIO.ENTIDAD.DESCR;
                            obj.Causa = "Comun";
                            obj.ComunDiscMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "4" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                            obj.ComunImpFem = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "I" && item.CAUSA_PENAL.INGRESO.NUC == null && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "F" ? 1 : 0;
                            obj.ComunImpMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "I" && item.CAUSA_PENAL.INGRESO.NUC == null && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                            obj.ComunIndicFem = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "I" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "F" ? 1 : 0;
                            obj.ComunIndicMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "I" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                            obj.ComunProcFem = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "2" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "F" ? 1 : 0;
                            obj.ComunProcMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "2" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                            obj.ComunSentFem = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "3" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "F" ? 1 : 0;
                            obj.ComunSentMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "3" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                            lst_imp.Add(obj);
                        }
                        else if (item.CAUSA_PENAL.ID_ESTATUS_CP.Value == 1 && item.ID_FUERO == "F")
                        {
                            var obj = new cPoblacionEntidadProcedencia();
                            //obj.Estado = item.CAUSA_PENAL.INGRESO.IMPUTADO.ENTIDAD.DESCR;
                            obj.Estado = item.CAUSA_PENAL.INGRESO.MUNICIPIO.ENTIDAD.DESCR;
                            obj.Causa = "Federal";
                            obj.FederalIndicMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "I" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                            obj.FederalProcFem = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "2" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "F" ? 1 : 0;
                            obj.FederalProcMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "2" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                            obj.FederalSentFem = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "3" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "F" ? 1 : 0;
                            obj.FederalSentMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "3" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                            lst_imp.Add(obj);
                        }
                    }
                }
                //Para alimentar grafica Masculino
                foreach (var item in lst_imp)
                {

                    if (item.Causa == "Comun")
                    {
                        if (item.ComunDiscMasc > 0)
                        {
                            var obj = new cReporteGraficaPoblacionEntidadProcedenciaM();
                            obj.Causa = "Discrecional";
                            obj.Comun += item.ComunDiscMasc;
                            lst_imp_grafica.Add(obj);
                        }
                        if (item.ComunImpMasc > 0)
                        {
                            var obj = new cReporteGraficaPoblacionEntidadProcedenciaM();
                            obj.Causa = "Imputado";
                            obj.Comun += item.ComunImpMasc;
                            lst_imp_grafica.Add(obj);
                        }
                        if (item.ComunIndicMasc > 0)
                        {
                            var obj = new cReporteGraficaPoblacionEntidadProcedenciaM();
                            obj.Causa = "Indiciado";
                            obj.Comun += item.ComunIndicMasc;
                            lst_imp_grafica.Add(obj);
                        }
                        if (item.ComunProcMasc > 0)
                        {
                            var obj = new cReporteGraficaPoblacionEntidadProcedenciaM();
                            obj.Causa = "Procesado";
                            obj.Comun += item.ComunProcMasc;
                            lst_imp_grafica.Add(obj);
                        }
                        if (item.ComunSentMasc > 0)
                        {
                            var obj = new cReporteGraficaPoblacionEntidadProcedenciaM();
                            obj.Causa = "Sentenciado";
                            obj.Comun += item.ComunSentMasc;
                            lst_imp_grafica.Add(obj);
                        }
                    }
                    if (item.Causa == "Federal")
                    {
                        if (item.FederalIndicMasc > 0)
                        {
                            var obj = new cReporteGraficaPoblacionEntidadProcedenciaM();
                            obj.Causa = "Indiciado";
                            obj.Federal += item.FederalIndicMasc;
                            lst_imp_grafica.Add(obj);
                        }
                        if (item.FederalProcMasc > 0)
                        {
                            var obj = new cReporteGraficaPoblacionEntidadProcedenciaM();
                            obj.Causa = "Procesado";
                            obj.Federal += item.FederalProcMasc;
                            lst_imp_grafica.Add(obj);
                        }
                        if (item.FederalSentMasc > 0)
                        {
                            var obj = new cReporteGraficaPoblacionEntidadProcedenciaM();
                            obj.Causa = "Sentenciado";
                            obj.Federal += item.FederalSentMasc;
                            lst_imp_grafica.Add(obj);
                        }
                    }
                    if (item.Causa == "Sin Fuero")
                    {
                        if (item.SinFueroDiscIndef > 0)
                        {
                            var obj = new cReporteGraficaPoblacionEntidadProcedenciaM();
                            obj.Causa = "Discrecional";
                            obj.SinFuero += item.SinFueroDiscIndef;
                            lst_imp_grafica.Add(obj);
                        }
                        if (item.SinFueroIndicMasc > 0)
                        {
                            var obj = new cReporteGraficaPoblacionEntidadProcedenciaM();
                            obj.Causa = "Indiciado";
                            obj.SinFuero += item.SinFueroIndicMasc;
                            lst_imp_grafica.Add(obj);
                        }
                        if (item.SinFueroProcMasc > 0)
                        {
                            var obj = new cReporteGraficaPoblacionEntidadProcedenciaM();
                            obj.Causa = "Procesado";
                            obj.SinFuero += item.SinFueroProcMasc;
                            lst_imp_grafica.Add(obj);
                        }
                    }
                }
                //Para alimentar grafica Femenino
                foreach (var item in lst_imp)
                {
                    if (item.Causa == "Comun")
                    {
                        if (item.ComunImpFem > 0)
                        {
                            var obj = new cReporteGraficaPoblacionEntidadF();
                            obj.Causa = "Imputado";
                            obj.Comun += item.ComunImpFem;
                            lst_imp_grafica_f.Add(obj);
                        }
                        if (item.ComunIndicFem > 0)
                        {
                            var obj = new cReporteGraficaPoblacionEntidadF();
                            obj.Causa = "Indiciado";
                            obj.Comun += item.ComunIndicFem;
                            lst_imp_grafica_f.Add(obj);
                        }
                        if (item.ComunProcFem > 0)
                        {
                            var obj = new cReporteGraficaPoblacionEntidadF();
                            obj.Causa = "Procesado";
                            obj.Comun += item.ComunProcFem;
                            lst_imp_grafica_f.Add(obj);
                        }
                        if (item.ComunSentFem > 0)
                        {
                            var obj = new cReporteGraficaPoblacionEntidadF();
                            obj.Causa = "Sentenciado";
                            obj.Comun += item.ComunSentFem;
                            lst_imp_grafica_f.Add(obj);
                        }
                    }
                    if (item.Causa == "Federal")
                    {
                        if (item.FederalProcFem > 0)
                        {
                            var obj = new cReporteGraficaPoblacionEntidadF();
                            obj.Causa = "Procesado";
                            obj.Federal += item.FederalProcFem;
                            lst_imp_grafica_f.Add(obj);
                        }
                        if (item.FederalSentFem > 0)
                        {
                            var obj = new cReporteGraficaPoblacionEntidadF();
                            obj.Causa = "Sentenciado";
                            obj.Federal += item.FederalSentFem;
                            lst_imp_grafica_f.Add(obj);
                        }
                    }
                    if (item.Causa == "Sin Fuero")
                    {
                        if (item.SinFueroIndicFem > 0)
                        {
                            var obj = new cReporteGraficaPoblacionEntidadF();
                            obj.Causa = "Indiciado";
                            obj.SinFuero += item.SinFueroIndicFem;
                            lst_imp_grafica_f.Add(obj);
                        }
                    }
                }
                var results = lst_imp.GroupBy(n => n.Estado).
                     Select(group =>
                         new
                         {
                             Estado = group.Key,
                             Sexo = group.ToList(),
                         });

                var lst_imp2 = new List<cPoblacionEntidadProcedencia>();
                foreach (var row in results)
                {
                    var obj = new cPoblacionEntidadProcedencia();
                    obj.Estado = row.Estado;
                    foreach (var item in row.Sexo)
                    {
                        obj.ComunDiscMasc += item.ComunDiscMasc;
                        obj.ComunImpFem += item.ComunImpFem;
                        obj.ComunImpMasc += item.ComunImpMasc;
                        obj.ComunIndicFem += item.ComunIndicFem;
                        obj.ComunIndicMasc += item.ComunIndicMasc;
                        obj.ComunProcFem += item.ComunProcFem;
                        obj.ComunProcMasc += item.ComunProcMasc;
                        obj.ComunSentFem += item.ComunSentFem;
                        obj.ComunSentMasc += item.ComunSentMasc;
                        obj.FederalIndicMasc += item.FederalIndicMasc;
                        obj.FederalProcFem += item.FederalProcFem;
                        obj.FederalProcMasc += item.FederalProcMasc;
                        obj.FederalSentFem += item.FederalSentFem;
                        obj.FederalSentMasc += item.FederalSentMasc;
                        obj.SinFueroDiscIndef += item.SinFueroDiscIndef;
                        obj.SinFueroIndicFem += item.SinFueroIndicFem;
                        obj.SinFueroIndicMasc += item.SinFueroIndicMasc;
                        obj.SinFueroProcMasc += item.SinFueroProcMasc;
                    }
                    lst_imp2.Add(obj);
                }
                Reporte.LocalReport.ReportPath = "../../Reportes/rPoblacionEntidadProcedencia.rdlc";
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
                rds3.Name = "DataSet3";
                rds3.Value = lst_imp_grafica;
                Reporte.LocalReport.DataSources.Add(rds3);

                ReportDataSource rds4 = new ReportDataSource();
                rds4.Name = "DataSet4";
                rds4.Value = lst_imp_grafica_f;
                Reporte.LocalReport.DataSources.Add(rds4);

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

        private void OnLoad(PoblacionEntidadProcedenciaView window)
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
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_POBLACION_ENTIDAD_PROCEDENCIA.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
