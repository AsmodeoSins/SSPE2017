using Microsoft.Reporting.WinForms;
using SSP.Controlador.Catalogo.Justicia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlPenales
{
    public partial class PoblacionTerceraEdadViewModel : ValidationViewModelBase
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
                var grafica = new List<cGraficaTerceraEdad>();
                var datosReporte = new List<cReporteDatos>();
                datosReporte.Add(new cReporteDatos()
                {
                    Encabezado1 = Parametro.ENCABEZADO1,
                    Encabezado2 = Parametro.ENCABEZADO2,
                    Encabezado3 = Parametro.ENCABEZADO3,
                    Logo1 = Parametro.REPORTE_LOGO1,
                    Logo2 = Parametro.REPORTE_LOGO2,
                    Titulo = "Tercera Edad"
                });

                var reporte = new cImputado().ObtenerInternosTerceraEdad(GlobalVar.gCentro).Select(w => new cPoblacionTerceraEdad()
                {
                    Rango = w.RANGO_EDAD,
                    ComunImpMasc = w.M_IMPUTADO_COMUN,
                    ComunIndicMasc = w.M_INDICIADO_COMUN,
                    ComunProcMasc = w.M_PROCESADO_COMUN,
                    FederalProcFem = w.F_PROCESADO_FEDERAL,
                    FederalProcMasc = w.M_PROCESADO_FEDERAL,
                    SinFueroProcMasc = w.M_PROCESADO_SIN_FUERO,
                    ComunSentMasc = w.M_SENTENCIADO_COMUN,
                    FederalSentFem = w.F_SENTENCIADO_FEDERAL,
                    FederalSentMasc = w.M_SENTENCIADO_FEDERAL,
                    Orden = w.ORDEN
                });
                    if (reporte != null)
                    {
                        foreach (var r in reporte)
                        {
                            grafica.Add(new cGraficaTerceraEdad()
                            {
                                Rango = r.Rango,
                                Comun = (r.ComunImpMasc + r.ComunIndicMasc + r.ComunProcMasc + r.ComunSentMasc),
                                Federal = (r.FederalProcFem + r.FederalProcMasc + r.FederalSentFem + r.FederalSentMasc),
                                SinFuero = r.SinFueroProcMasc,
                                Orden = r.Orden
                            });
                        }
                    }
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        Reporte.Reset();
                    }));
                    Reporte.LocalReport.ReportPath = "Reportes/rPoblacionTerceraEdad.rdlc";
                    Reporte.LocalReport.DataSources.Clear();

                    ReportDataSource rds1 = new ReportDataSource();
                    rds1.Name = "DataSet1";
                    rds1.Value = datosReporte;
                    Reporte.LocalReport.DataSources.Add(rds1);

                    ReportDataSource rds2 = new ReportDataSource();
                    rds2.Name = "DataSet2";
                    rds2.Value = reporte.OrderBy(w => w.Orden);
                    Reporte.LocalReport.DataSources.Add(rds2);

                    ReportDataSource rds3 = new ReportDataSource();
                    rds3.Name = "DataSet3";
                    rds3.Value = grafica.OrderBy(w => w.Orden);
                    Reporte.LocalReport.DataSources.Add(rds3);

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

        private void GenerarReporte()
        {
            try
            {
                
                //NOTA: ANIO TEMPORALMENTE FIJO SOLO PARA PRUEBAS DEBIDO A QUE NO HAY REGISTROS PARA EL ANIO 2016
                //DESCOMENTAR MAS ADELANTE
                var mes = Fechas.GetFechaDateServer.Month;
                var anio = Fechas.GetFechaDateServer.Year;
                var datosReporte = new List<cReporteDatos>();
                var tercera_Edad = Parametro.ID_TERCERA_EDAD;
                datosReporte.Add(new cReporteDatos()
                {
                    Encabezado1 = Parametro.ENCABEZADO1,
                    Encabezado2 = Parametro.ENCABEZADO2,
                    Encabezado3 = Parametro.ENCABEZADO3,
                    Logo1 = Parametro.REPORTE_LOGO1,
                    Logo2 = Parametro.REPORTE_LOGO2,
                    Titulo = "Tercera Edad"
                });
                var lst_internos_causa_penal_delito = new cCausaPenalDelito().ObtenerTodos().ToList();
                var internos_lista = new cIngreso().ObtenerIngresosActivos(GlobalVar.gCentro).Where(w => w.FEC_INGRESO_CERESO.Value.Month == mes && w.FEC_INGRESO_CERESO.Value.Year == anio).ToList();
                var lst_imp = new List<cPoblacionTerceraEdad>();
                var lst_imp_grafica = new List<cGraficaTerceraEdad>();

                foreach (var item in internos_lista)
                {
                    var interno = lst_internos_causa_penal_delito.Where(w => w.ID_CENTRO == item.ID_CENTRO && w.ID_ANIO == item.ID_ANIO && w.ID_IMPUTADO == item.ID_IMPUTADO && w.ID_INGRESO == w.ID_INGRESO).FirstOrDefault();
                    if (interno == null)
                    {
                        var bday = new Fechas().CalculaEdad(item.IMPUTADO.NACIMIENTO_FECHA.HasValue ? item.IMPUTADO.NACIMIENTO_FECHA : Fechas.GetFechaDateServer);
                        //DateTime now = DateTime.Today;
                        //int age = now.Year - bday.Value.Year;
                        //if (now < bday.Value.AddYears(age)) age--;


                        if (bday >= tercera_Edad)
                        {
                            if (bday >= 30 && bday <= 40)
                            {
                                var obj = new cPoblacionTerceraEdad();
                                obj.Edad = bday.ToString();
                                obj.Rango = "30 a 40";//age.ToString();//item.CAUSA_PENAL.INGRESO.IMPUTADO.ETNIA.DESCR;
                                obj.Causa = "Sin Fuero";
                                obj.SinFueroProcMasc = item.ID_CLASIFICACION_JURIDICA == "2" && item.IMPUTADO.SEXO == "M" ? 1 : 0;
                                lst_imp.Add(obj);
                            }
                            if (bday > 40 && bday <= 50)
                            {
                                var obj = new cPoblacionTerceraEdad();
                                obj.Edad = bday.ToString();
                                obj.Rango = "40 a 50";//age.ToString();//item.CAUSA_PENAL.INGRESO.IMPUTADO.ETNIA.DESCR;
                                obj.Causa = "Sin Fuero";
                                obj.SinFueroProcMasc = item.ID_CLASIFICACION_JURIDICA == "2" && item.IMPUTADO.SEXO == "M" ? 1 : 0;
                                lst_imp.Add(obj);
                            }
                            if (bday > 50 && bday <= 60)
                            {
                                var obj = new cPoblacionTerceraEdad();
                                obj.Edad = bday.ToString();
                                obj.Rango = "50 a 60";//age.ToString();//item.CAUSA_PENAL.INGRESO.IMPUTADO.ETNIA.DESCR;
                                obj.Causa = "Sin Fuero";
                                obj.SinFueroProcMasc = item.ID_CLASIFICACION_JURIDICA == "2" && item.IMPUTADO.SEXO == "M" ? 1 : 0;
                                lst_imp.Add(obj);
                            }
                            if (bday > 60)
                            {
                                var obj = new cPoblacionTerceraEdad();
                                obj.Edad = bday.ToString();
                                obj.Rango = "Mayor de 60";//age.ToString();//item.CAUSA_PENAL.INGRESO.IMPUTADO.ETNIA.DESCR;
                                obj.Causa = "Sin Fuero";
                                obj.SinFueroProcMasc = item.ID_CLASIFICACION_JURIDICA == "2" && item.IMPUTADO.SEXO == "M" ? 1 : 0;
                                lst_imp.Add(obj);
                            }
                        }
                    }
                }

                foreach (var item in lst_internos_causa_penal_delito)
                {
                    var interno = internos_lista.Where(w => w.ID_CENTRO == item.ID_CENTRO && w.ID_ANIO == item.ID_ANIO && w.ID_IMPUTADO == item.ID_IMPUTADO && w.ID_INGRESO == w.ID_INGRESO).FirstOrDefault();
                    if (interno != null)
                    {
                        var bday = interno.IMPUTADO.NACIMIENTO_FECHA;
                        DateTime now = DateTime.Today;
                        int age = now.Year - bday.Value.Year;
                        if (now < bday.Value.AddYears(age)) age--;

                        if (age >= tercera_Edad)
                        {
                            if (item.CAUSA_PENAL.ID_ESTATUS_CP.Value == 1 && item.ID_FUERO == "C")
                            {
                                if (age >= 30 && age <= 40)
                                {
                                    var obj = new cPoblacionTerceraEdad();
                                    obj.Edad = age.ToString();
                                    obj.Rango = "30 a 40";//age.ToString();//item.CAUSA_PENAL.INGRESO.IMPUTADO.ETNIA.DESCR;
                                    obj.Causa = "Comun";
                                    obj.ComunImpMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "I" && item.CAUSA_PENAL.INGRESO.NUC == null && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    obj.ComunIndicMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "I" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    obj.ComunProcMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "2" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    obj.ComunSentMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "3" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    lst_imp.Add(obj);
                                }
                                if (age > 40 && age <= 50)
                                {
                                    var obj = new cPoblacionTerceraEdad();
                                    obj.Edad = age.ToString();
                                    obj.Rango = "40 a 50";//age.ToString();//item.CAUSA_PENAL.INGRESO.IMPUTADO.ETNIA.DESCR;
                                    obj.Causa = "Comun";
                                    obj.ComunImpMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "I" && item.CAUSA_PENAL.INGRESO.NUC == null && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    obj.ComunIndicMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "I" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    obj.ComunProcMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "2" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    obj.ComunSentMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "3" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    lst_imp.Add(obj);
                                }
                                if (age > 50 && age <= 60)
                                {
                                    var obj = new cPoblacionTerceraEdad();
                                    obj.Edad = age.ToString();
                                    obj.Rango = "50 a 60";//age.ToString();//item.CAUSA_PENAL.INGRESO.IMPUTADO.ETNIA.DESCR;
                                    obj.Causa = "Comun";
                                    obj.ComunImpMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "I" && item.CAUSA_PENAL.INGRESO.NUC == null && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    obj.ComunIndicMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "I" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    obj.ComunProcMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "2" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    obj.ComunSentMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "3" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    lst_imp.Add(obj);
                                }
                                if (age > 60)
                                {
                                    var obj = new cPoblacionTerceraEdad();
                                    obj.Edad = age.ToString();
                                    obj.Rango = "Mayor de 60";//age.ToString();//item.CAUSA_PENAL.INGRESO.IMPUTADO.ETNIA.DESCR;
                                    obj.Causa = "Comun";
                                    obj.ComunImpMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "I" && item.CAUSA_PENAL.INGRESO.NUC == null && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    obj.ComunIndicMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "I" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    obj.ComunProcMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "2" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    obj.ComunSentMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "3" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    lst_imp.Add(obj);
                                }
                            }
                            if (item.CAUSA_PENAL.ID_ESTATUS_CP.Value == 1 && item.ID_FUERO == "F")
                            {
                                if (age >= 30 && age <= 40)
                                {
                                    var obj = new cPoblacionTerceraEdad();
                                    obj.Edad = age.ToString();
                                    obj.Rango = "30 a 40";//age.ToString();
                                    obj.Causa = "Federal";
                                    obj.FederalProcFem = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "2" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "F" ? 1 : 0;
                                    obj.FederalProcMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "2" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    obj.FederalSentFem = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "3" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "F" ? 1 : 0;
                                    obj.FederalSentMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "3" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    lst_imp.Add(obj);
                                }
                                if (age > 40 && age <= 50)
                                {
                                    var obj = new cPoblacionTerceraEdad();
                                    obj.Edad = age.ToString();
                                    obj.Rango = "40 a 50";//age.ToString();
                                    obj.Causa = "Federal";
                                    obj.FederalProcFem = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "2" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "F" ? 1 : 0;
                                    obj.FederalProcMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "2" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    obj.FederalSentFem = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "3" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "F" ? 1 : 0;
                                    obj.FederalSentMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "3" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    lst_imp.Add(obj);
                                }
                                if (age > 50 && age <= 60)
                                {
                                    var obj = new cPoblacionTerceraEdad();
                                    obj.Edad = age.ToString();
                                    obj.Rango = "50 a 60";//age.ToString();
                                    obj.Causa = "Federal";
                                    obj.FederalProcFem = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "2" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "F" ? 1 : 0;
                                    obj.FederalProcMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "2" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    obj.FederalSentFem = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "3" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "F" ? 1 : 0;
                                    obj.FederalSentMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "3" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    lst_imp.Add(obj);
                                }
                                if (age > 60)
                                {
                                    var obj = new cPoblacionTerceraEdad();
                                    obj.Edad = age.ToString();
                                    obj.Rango = "Mayor de 60";//age.ToString();
                                    obj.Causa = "Federal";
                                    obj.FederalProcFem = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "2" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "F" ? 1 : 0;
                                    obj.FederalProcMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "2" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    obj.FederalSentFem = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "3" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "F" ? 1 : 0;
                                    obj.FederalSentMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "3" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    lst_imp.Add(obj);
                                }
                            }
                        }
                    }
                }
                foreach (var item in lst_imp)
                {
                    if (item.Causa == "Comun")
                    {
                        if (item.ComunImpMasc > 0)
                        {
                            var obj = new cGraficaTerceraEdad();
                            obj.Rango = item.Rango;
                            obj.Causa = "Comun Imputado";
                            obj.Comun += item.ComunImpMasc;

                            lst_imp_grafica.Add(obj);
                        }
                        if (item.ComunIndicMasc > 0)
                        {
                            var obj = new cGraficaTerceraEdad();
                            obj.Rango = item.Rango;
                            obj.Causa = "Comun Indiciado";
                            obj.Comun += item.ComunIndicMasc;

                            lst_imp_grafica.Add(obj);
                        }
                        if (item.ComunProcMasc > 0)
                        {
                            var obj = new cGraficaTerceraEdad();
                            obj.Rango = item.Rango;
                            obj.Causa = "Comun Procesado";
                            obj.Comun += item.ComunProcMasc;

                            lst_imp_grafica.Add(obj);
                        }
                        if (item.ComunSentMasc > 0)
                        {
                            var obj = new cGraficaTerceraEdad();
                            obj.Rango = item.Rango;
                            obj.Causa = "Comun Sentenciado";
                            obj.Comun += item.ComunSentMasc;

                            lst_imp_grafica.Add(obj);
                        }
                    }
                    if (item.Causa == "Federal")
                    {
                        if (item.FederalProcMasc > 0)
                        {
                            var obj = new cGraficaTerceraEdad();
                            obj.Rango = item.Rango;
                            obj.Causa = "Federal Procesado";
                            obj.Comun += item.FederalProcMasc + item.FederalProcFem;
                            lst_imp_grafica.Add(obj);
                        }
                        if (item.FederalSentMasc > 0)
                        {
                            var obj = new cGraficaTerceraEdad();
                            obj.Rango = item.Rango;
                            obj.Causa = "Federal Sentenciado";
                            obj.Comun += item.FederalSentMasc + item.FederalSentFem;
                            lst_imp_grafica.Add(obj);
                        }
                    }
                    if (item.Causa == "Sin Fuero")
                    {
                        if (item.SinFueroProcMasc > 0)
                        {
                            var obj = new cGraficaTerceraEdad();
                            obj.Rango = item.Rango;
                            obj.Causa = "Sin Fuero Procesado";
                            obj.Comun += item.SinFueroProcMasc;
                            lst_imp_grafica.Add(obj);
                        }
                    }
                }
                var results = lst_imp.GroupBy(n => n.Rango).
                     Select(group =>
                         new
                         {
                             Rango = group.Key,
                             Sexo = group.ToList(),
                         });

                var lst_imp2 = new List<cPoblacionTerceraEdad>();
                foreach (var row in results)
                {
                    var obj = new cPoblacionTerceraEdad();
                    obj.Rango = row.Rango;
                    foreach (var item in row.Sexo)
                    {
                        obj.ComunImpMasc += item.ComunImpMasc;
                        obj.ComunIndicMasc += item.ComunIndicMasc;
                        obj.ComunProcMasc += item.ComunProcMasc;
                        obj.ComunSentMasc += item.ComunSentMasc;
                        obj.FederalProcFem += item.FederalProcFem;
                        obj.FederalProcMasc += item.FederalProcMasc;
                        obj.FederalSentFem += item.FederalSentFem;
                        obj.FederalSentMasc += item.FederalSentMasc;
                        obj.SinFueroProcMasc += item.SinFueroProcMasc;
                    }
                    lst_imp2.Add(obj);
                }
                Reporte.LocalReport.ReportPath = "../../Reportes/rPoblacionTerceraEdad.rdlc";
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

        private void OnLoad(PoblacionTerceraEdadView window)
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
