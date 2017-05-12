using Microsoft.Reporting.WinForms;
using SSP.Controlador.Catalogo.Justicia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlPenales
{
    public partial class ReporteControlDeVisitantesViewModel : ValidationViewModelBase
    {

        public async void OnLoad(ReporteControlDeVisitantes Window)
        {
            try
            {
                ConfiguraPermisos();
                Ventana = Window;
                Reporte = Ventana.ReportControlVisitantes;
                ReporteGrafica = Ventana.ReportControlVisitantesGrafica;
                GraficaEnabled = false;
                ReportViewerVisible = Visibility.Collapsed;
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    SelectedFechaInicial = Fechas.GetFechaDateServer;
                    SelectedFechaFinal = Fechas.GetFechaDateServer;
                });
                ReportViewerVisible = Visibility.Visible;
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
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    Reporte.Reset();
                    ReporteGrafica.Reset();
                    //GenerarReporte();
                    GenerarReporteConQuery();
                    GraficaEnabled = true;
                    break;
            }
        }

        public int ObtenerEdad(DateTime Fecha_Nacimiento)
        {
            return (Fecha_Nacimiento.Month < Fechas.GetFechaDateServer.Month) ?
                Fechas.GetFechaDateServer.Year - Fecha_Nacimiento.Year :
                ((Fecha_Nacimiento.Month == Fechas.GetFechaDateServer.Month &&
                Fecha_Nacimiento.Day <= Fechas.GetFechaDateServer.Day) ? (Fechas.GetFechaDateServer.Year - Fecha_Nacimiento.Year) :
                (Fechas.GetFechaDateServer.Year - Fecha_Nacimiento.Year) - 1);
        }


        public void GenerarReporteConQuery()
        { 
            try
            {
                var hoy = Fechas.GetFechaDateServer;
                var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();
                var datosReporte = new List<cReporteDatos>();
                var totales = new List<cControlVisitaTotales>();
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
                
                var reporte = new cAduana().ObtenerReporteControlVisita(GlobalVar.gCentro, SelectedFechaInicial, SelectedFechaFinal).Select(w => new cControlVisita() { 
                        Hora_Ingreso = w.ENTRADA_FEC.ToString("hh:mm tt"),
                        Hora_Salida = w.SALIDA_FEC != null ? w.SALIDA_FEC.Value.ToString("hh:mm tt") : null,
                        Id_Persona = w.ID_PERSONA,
                        Nombre_Visitante = w.VISITA_NOMBRE,
                        Paterno_Visitante = w.VISITA_PATERNO,
                        Materno_Visitante = w.VISITA_MATERNO,
                        Centro = w.ID_CENTRO,
                        Id_Anio = w.ID_ANIO,
                        Id_Imputado = w.ID_IMPUTADO,
                        Nombre_Ingreso = w.INTERNO_NOMBRE,
                        Paterno_Ingreso = w.INTERNO_PATERNO,
                        Materno_Ingreso = w.INTERNO_MATERNO,
                        Ubicacion_Ingreso = string.Format("{0}-{1}-{2}-{3}",w.EDIFICIO,w.SECTOR,w.CELDA,w.CAMA),
                        Usuario = string.Format("{0} {1} {2}",w.USUARIO_NOMBRE,w.USUARIO_PATERNO,w.USUARIO_MATERNO),
                        //DioSalida
                        Tipo_Visita = w.INTIMA == "S" ?  w.TIPO_VISITA + "(INTIMA)" : w.TIPO_VISITA,
                        Intima = w.INTIMA,
                        Categoria = new Fechas().CalculaEdad(w.VISITA_NACIMIENTO,hoy) < MAYORIA_DE_EDAD ? MENORES : (w.VISITA_SEXO == "F" ? MUJERES : HOMBRES),
                });

                if (reporte != null)
                {
                    int tve = reporte.Count();
                    int tvs = reporte.Count(w => !string.IsNullOrEmpty(w.Hora_Salida));
                    int tvi = reporte.GroupBy(w => new { w.Centro, w.Id_Anio, w.Id_Imputado }).Count();

                    int tie = reporte.Count(w => w.Intima == "S");
                    int tis = reporte.Count(w => w.Intima == "S" && !string.IsNullOrEmpty(w.Hora_Salida));

                    totales.Add(new cControlVisitaTotales()
                    {
                       VisitantesIngresaron = tve,
                       VisitantesSalieron = tvs,
                       InternosVisitados = tvi,
                       VisitanteIntimaIngresaron = tie,
                       VisitanteIntimaSalieron = tis
                    });
                }

                Reporte.Refresh();
                Reporte.LocalReport.ReportPath = "Reportes/rControlVisitantes.rdlc";
                Reporte.LocalReport.DataSources.Clear();

                ReportDataSource ReportDataSource_Encabezado = new ReportDataSource();
                ReportDataSource_Encabezado.Name = "DataSet2";
                ReportDataSource_Encabezado.Value = datosReporte;

                ReportDataSource ReportDataSource = new ReportDataSource();
                ReportDataSource.Name = "DataSet1";
                ReportDataSource.Value = reporte;

                ReportDataSource ReportDataSource2 = new ReportDataSource();
                ReportDataSource2.Name = "DataSet3";
                ReportDataSource2.Value = totales;


                Reporte.LocalReport.DataSources.Add(ReportDataSource_Encabezado);
                Reporte.LocalReport.DataSources.Add(ReportDataSource);
                Reporte.LocalReport.DataSources.Add(ReportDataSource2);

                Reporte.LocalReport.SetParameters(new ReportParameter("FechaInicial", string.Format("{0} DE {1} DEL {2}", SelectedFechaInicial.Day, SelectedFechaInicial.Month, SelectedFechaInicial.Year)));
                Reporte.LocalReport.SetParameters(new ReportParameter("FechaFinal", string.Format("{0} DE {1} DEL {2}", SelectedFechaFinal.Day, SelectedFechaFinal.Month, SelectedFechaFinal.Year)));
                
                Reporte.Refresh();
                Reporte.RefreshReport();

                ReporteGrafica.Refresh();
                ReporteGrafica.LocalReport.ReportPath = "Reportes/rControlVisitantesGrafica.rdlc";
                ReporteGrafica.LocalReport.DataSources.Clear();

                ReportDataSource ReportDataSource_Encabezado_Grafica = new ReportDataSource();
                ReportDataSource_Encabezado_Grafica.Name = "DataSet1";
                ReportDataSource_Encabezado_Grafica.Value = datosReporte;

                ReportDataSource ReportDataSource_Grafica = new ReportDataSource();
                ReportDataSource_Grafica.Name = "DataSet3";
                ReportDataSource_Grafica.Value = reporte;

                ReporteGrafica.LocalReport.DataSources.Add(ReportDataSource_Encabezado_Grafica);
                ReporteGrafica.LocalReport.DataSources.Add(ReportDataSource_Grafica);

                ReporteGrafica.LocalReport.SetParameters(new ReportParameter("FechaInicial", string.Format("{0} DE {1} DEL {2}", SelectedFechaInicial.Day, SelectedFechaInicial.Month, SelectedFechaInicial.Year)));
                ReporteGrafica.LocalReport.SetParameters(new ReportParameter("FechaFinal", string.Format("{0} DE {1} DEL {2}", SelectedFechaFinal.Day, SelectedFechaFinal.Month, SelectedFechaFinal.Year)));
                ReporteGrafica.LocalReport.SetParameters(new ReportParameter("TotalVisitantes", reporte.Count().ToString()));

                ReporteGrafica.Refresh();
                ReporteGrafica.RefreshReport();
            }
            catch(Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar reporte", ex);
            }
        }

        public async void GenerarReporte()
        {
            try
            {
                var lVisitas = new List<cControlVisita>();
                var datosReporte = new List<cReporteDatos>();
                var VisitasGenero = new List<cVisitantesGenero>();

                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {

                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ReportViewerVisible = Visibility.Collapsed;
                    }));
                    var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();

                    var totales = new List<cControlVisitaTotales>();
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

                    var consulta_cama = new cCama();

                    double Total_Resultados = new cAduana().
                        ObtenerVisitas(SelectedFechaInicial, SelectedFechaFinal, Parametro.CONTROL_VISITANTES_REPORTE).Count();
                    var Paginas = Math.Abs(Total_Resultados - (int)Total_Resultados) < double.Epsilon ? ((int)((Total_Resultados / VISITANTES_POR_PAGINA) + 1)) : ((int)(Total_Resultados / VISITANTES_POR_PAGINA));

                    for (int i = 0; i < Paginas; i++)
                    {
                        lVisitas.AddRange(new cAduana().
                        ObtenerVisitas(SelectedFechaInicial, SelectedFechaFinal, Parametro.CONTROL_VISITANTES_REPORTE).OrderBy(o => o.ID_ADUANA).Skip(ULTIMO_REGISTRO).Take(VISITANTES_POR_PAGINA).
                        AsEnumerable().
                        Select(s => new cControlVisita()
                        {
                            Hora_Ingreso = string.Format("{0}:{1}", s.ENTRADA_FEC.Value.Hour < 10 ? string.Format("0{0}", s.ENTRADA_FEC.Value.Hour.ToString()) : s.ENTRADA_FEC.Value.Hour.ToString(), s.ENTRADA_FEC.Value.Minute < 10 ? string.Format("0{0}", s.ENTRADA_FEC.Value.Minute.ToString()) : s.ENTRADA_FEC.Value.Minute.ToString()),
                            Hora_Salida = s.SALIDA_FEC.HasValue ? string.Format("{0}:{1}", s.SALIDA_FEC.Value.Hour < 10 ? string.Format("0{0}", s.SALIDA_FEC.Value.Hour.ToString()) : s.SALIDA_FEC.Value.Hour.ToString(), s.SALIDA_FEC.Value.Minute < 10 ? string.Format("0{0}", s.SALIDA_FEC.Value.Minute.ToString()) : s.SALIDA_FEC.Value.Minute.ToString()) : string.Empty,
                            Centro = s.ADUANA_INGRESO.Any() ? s.ADUANA_INGRESO.Where(w => w.ID_ADUANA == s.ID_ADUANA).FirstOrDefault().ID_CENTRO : new short(),
                            Id_Anio = s.ADUANA_INGRESO.Any() ? s.ADUANA_INGRESO.Where(w => w.ID_ADUANA == s.ID_ADUANA).FirstOrDefault().ID_ANIO : new short(),
                            Id_Imputado = s.ADUANA_INGRESO.Any() ? s.ADUANA_INGRESO.Where(w => w.ID_ADUANA == s.ID_ADUANA).FirstOrDefault().ID_IMPUTADO : new short(),
                            Id_Persona = s.ID_PERSONA,
                            Paterno_Ingreso = s.ADUANA_INGRESO.Any() ? !string.IsNullOrEmpty(s.ADUANA_INGRESO.Where(w => w.ID_ADUANA == s.ID_ADUANA).FirstOrDefault().INGRESO.IMPUTADO.PATERNO) ?
                                    s.ADUANA_INGRESO.Where(w => w.ID_ADUANA == s.ID_ADUANA).FirstOrDefault().INGRESO.IMPUTADO.PATERNO.TrimEnd() : string.Empty : string.Empty,
                            Materno_Ingreso = s.ADUANA_INGRESO.Any() ? !string.IsNullOrEmpty(s.ADUANA_INGRESO.Where(w => w.ID_ADUANA == s.ID_ADUANA).FirstOrDefault().INGRESO.IMPUTADO.MATERNO) ?
                                    s.ADUANA_INGRESO.Where(w => w.ID_ADUANA == s.ID_ADUANA).FirstOrDefault().INGRESO.IMPUTADO.MATERNO.TrimEnd() : string.Empty : string.Empty,
                            Nombre_Ingreso = s.ADUANA_INGRESO.Any() ? !string.IsNullOrEmpty(s.ADUANA_INGRESO.Where(w => w.ID_ADUANA == s.ID_ADUANA).FirstOrDefault().INGRESO.IMPUTADO.NOMBRE) ?
                                    s.ADUANA_INGRESO.Where(w => w.ID_ADUANA == s.ID_ADUANA).FirstOrDefault().INGRESO.IMPUTADO.NOMBRE.TrimEnd() : string.Empty : string.Empty,
                            Ubicacion_Ingreso = new Func<string>(() =>
                            {
                                var ingreso = s.ADUANA_INGRESO.Any() ? s.ADUANA_INGRESO.Where(w => w.ID_ADUANA == s.ID_ADUANA).FirstOrDefault().INGRESO : null;

                                if (ingreso != null)
                                {
                                    if (ingreso.ID_UB_CENTRO != null &&
                                        ingreso.ID_UB_EDIFICIO != null &&
                                        ingreso.ID_UB_SECTOR != null &&
                                        ingreso.ID_UB_CELDA != null &&
                                        ingreso.ID_UB_CAMA != null)
                                    {
                                        var cama = consulta_cama.ObtenerCama((short)ingreso.ID_UB_CENTRO, (short)ingreso.ID_UB_EDIFICIO, (short)ingreso.ID_UB_SECTOR, ingreso.ID_UB_CELDA, (short)ingreso.ID_UB_CAMA);
                                        if (cama != null)
                                            return string.Format("{0}-{1}-{2}",
                                                cama.CELDA != null ? cama.CELDA.SECTOR != null ? cama.CELDA.SECTOR.EDIFICIO != null ? !string.IsNullOrEmpty(cama.CELDA.SECTOR.EDIFICIO.DESCR) ? cama.CELDA.SECTOR.EDIFICIO.DESCR.TrimEnd() : string.Empty : string.Empty : string.Empty : string.Empty,
                                                cama.CELDA != null ? cama.CELDA.SECTOR != null ? !string.IsNullOrEmpty(cama.CELDA.SECTOR.DESCR) ? cama.CELDA.SECTOR.DESCR.TrimEnd() : string.Empty : string.Empty : string.Empty,
                                                cama.CELDA != null ? cama.CELDA.ID_CELDA.TrimStart().TrimEnd() : string.Empty);
                                    }
                                }
                                return string.Empty;
                            })(),
                            Usuario = s.USUARIO != null ? s.USUARIO.ID_USUARIO : string.Empty,
                            Paterno_Visitante = s.PERSONA != null ? !string.IsNullOrEmpty(s.PERSONA.PATERNO) ? s.PERSONA.PATERNO.TrimEnd() : string.Empty : string.Empty,
                            Materno_Visitante = s.PERSONA != null ? !string.IsNullOrEmpty(s.PERSONA.MATERNO) ? s.PERSONA.MATERNO.TrimEnd() : string.Empty : string.Empty,
                            Nombre_Visitante = s.PERSONA != null ? !string.IsNullOrEmpty(s.PERSONA.NOMBRE) ? s.PERSONA.NOMBRE.TrimEnd() : string.Empty : string.Empty,
                            Tipo_Visita = new Func<string>(() =>
                            {
                                enumTipoPersona TipoPersona = (enumTipoPersona)s.ID_TIPO_PERSONA;
                                var TipoVisita = string.Empty;
                                switch (TipoPersona)
                                {
                                    case enumTipoPersona.ABOGADO:
                                        TipoVisita = string.Format(VISITA_LEGAL + " ({0})", s.TIPO_PERSONA != null ? !string.IsNullOrEmpty(s.TIPO_PERSONA.DESCR) ? s.TIPO_PERSONA.DESCR.TrimEnd() : string.Empty : string.Empty);
                                        break;
                                    case enumTipoPersona.VISITA:
                                        TipoVisita = s.ADUANA_INGRESO.Any() ? string.Format("{0}", s.ADUANA_INGRESO.Where(w => w.ID_ADUANA == s.ID_ADUANA).FirstOrDefault().INTIMA == ES_VISITA_INTIMA ? VISITA_INTIMA : VISITA_FAMILIAR) : string.Empty;
                                        break;
                                }
                                return TipoVisita;
                            })(),
                            Categoria = ObtenerEdad(s.PERSONA.FEC_NACIMIENTO.Value) < MAYORIA_DE_EDAD ? MENORES : (s.PERSONA.SEXO == FEMENINO ? MUJERES : HOMBRES),
                        }).
                        ToList());
                        ULTIMO_REGISTRO += VISITANTES_POR_PAGINA;
                    }


                    #region Informacion_Reporte
                    //lVisitas = new cAduana().
                    //    ObtenerVisitas(SelectedFechaInicial, SelectedFechaFinal, Parametro.CONTROL_VISITANTES_REPORTE).
                    //    AsEnumerable().
                    //    Select(s => new cControlVisita()
                    //    {
                    //        Hora_Ingreso = string.Format("{0}:{1}", s.ENTRADA_FEC.Value.Hour < 10 ? string.Format("0{0}", s.ENTRADA_FEC.Value.Hour.ToString()) : s.ENTRADA_FEC.Value.Hour.ToString(), s.ENTRADA_FEC.Value.Minute < 10 ? string.Format("0{0}", s.ENTRADA_FEC.Value.Minute.ToString()) : s.ENTRADA_FEC.Value.Minute.ToString()),
                    //        Hora_Salida = s.SALIDA_FEC.HasValue ? string.Format("{0}:{1}", s.SALIDA_FEC.Value.Hour < 10 ? string.Format("0{0}", s.SALIDA_FEC.Value.Hour.ToString()) : s.SALIDA_FEC.Value.Hour.ToString(), s.SALIDA_FEC.Value.Minute < 10 ? string.Format("0{0}", s.SALIDA_FEC.Value.Minute.ToString()) : s.SALIDA_FEC.Value.Minute.ToString()) : string.Empty,
                    //        Id_Anio = s.ADUANA_INGRESO.Any() ? s.ADUANA_INGRESO.Where(w => w.ID_ADUANA == s.ID_ADUANA).FirstOrDefault().ID_ANIO : new short(),
                    //        Id_Imputado = s.ADUANA_INGRESO.Any() ? s.ADUANA_INGRESO.Where(w => w.ID_ADUANA == s.ID_ADUANA).FirstOrDefault().ID_IMPUTADO : new short(),
                    //        Id_Persona = s.ID_PERSONA,
                    //        Paterno_Ingreso = s.ADUANA_INGRESO.Any() ? !string.IsNullOrEmpty(s.ADUANA_INGRESO.Where(w => w.ID_ADUANA == s.ID_ADUANA).FirstOrDefault().INGRESO.IMPUTADO.PATERNO) ?
                    //                s.ADUANA_INGRESO.Where(w => w.ID_ADUANA == s.ID_ADUANA).FirstOrDefault().INGRESO.IMPUTADO.PATERNO.TrimEnd() : string.Empty : string.Empty,
                    //        Materno_Ingreso = s.ADUANA_INGRESO.Any() ? !string.IsNullOrEmpty(s.ADUANA_INGRESO.Where(w => w.ID_ADUANA == s.ID_ADUANA).FirstOrDefault().INGRESO.IMPUTADO.MATERNO) ?
                    //                s.ADUANA_INGRESO.Where(w => w.ID_ADUANA == s.ID_ADUANA).FirstOrDefault().INGRESO.IMPUTADO.MATERNO.TrimEnd() : string.Empty : string.Empty,
                    //        Nombre_Ingreso = s.ADUANA_INGRESO.Any() ? !string.IsNullOrEmpty(s.ADUANA_INGRESO.Where(w => w.ID_ADUANA == s.ID_ADUANA).FirstOrDefault().INGRESO.IMPUTADO.NOMBRE) ?
                    //                s.ADUANA_INGRESO.Where(w => w.ID_ADUANA == s.ID_ADUANA).FirstOrDefault().INGRESO.IMPUTADO.NOMBRE.TrimEnd() : string.Empty : string.Empty,
                    //        Ubicacion_Ingreso = new Func<string>(() =>
                    //        {
                    //            var ingreso = s.ADUANA_INGRESO.Any() ? s.ADUANA_INGRESO.Where(w => w.ID_ADUANA == s.ID_ADUANA).FirstOrDefault().INGRESO : null;

                    //            if (ingreso != null)
                    //            {
                    //                if (ingreso.ID_UB_CENTRO != null &&
                    //                    ingreso.ID_UB_EDIFICIO != null &&
                    //                    ingreso.ID_UB_SECTOR != null &&
                    //                    ingreso.ID_UB_CELDA != null &&
                    //                    ingreso.ID_UB_CAMA != null)
                    //                {
                    //                    var cama = consulta_cama.ObtenerCama((short)ingreso.ID_UB_CENTRO, (short)ingreso.ID_UB_EDIFICIO, (short)ingreso.ID_UB_SECTOR, ingreso.ID_UB_CELDA, (short)ingreso.ID_UB_CAMA);
                    //                    if (cama != null)
                    //                        return string.Format("{0}-{1}-{2}",
                    //                            cama.CELDA != null ? cama.CELDA.SECTOR != null ? cama.CELDA.SECTOR.EDIFICIO != null ? !string.IsNullOrEmpty(cama.CELDA.SECTOR.EDIFICIO.DESCR) ? cama.CELDA.SECTOR.EDIFICIO.DESCR.TrimEnd() : string.Empty : string.Empty : string.Empty : string.Empty,
                    //                            cama.CELDA != null ? cama.CELDA.SECTOR != null ? !string.IsNullOrEmpty(cama.CELDA.SECTOR.DESCR) ? cama.CELDA.SECTOR.DESCR.TrimEnd() : string.Empty : string.Empty : string.Empty,
                    //                            cama.CELDA != null ? cama.CELDA.ID_CELDA.TrimStart().TrimEnd() : string.Empty);
                    //                }
                    //            }
                    //            return string.Empty;
                    //        })(),
                    //        Usuario = s.USUARIO != null ? s.USUARIO.ID_USUARIO : string.Empty,
                    //        Paterno_Visitante = s.PERSONA != null ? !string.IsNullOrEmpty(s.PERSONA.PATERNO) ? s.PERSONA.PATERNO.TrimEnd() : string.Empty : string.Empty,
                    //        Materno_Visitante = s.PERSONA != null ? !string.IsNullOrEmpty(s.PERSONA.MATERNO) ? s.PERSONA.MATERNO.TrimEnd() : string.Empty : string.Empty,
                    //        Nombre_Visitante = s.PERSONA != null ? !string.IsNullOrEmpty(s.PERSONA.NOMBRE) ? s.PERSONA.NOMBRE.TrimEnd() : string.Empty : string.Empty,
                    //        Tipo_Visita = new Func<string>(() =>
                    //        {
                    //            enumTipoPersona TipoPersona = (enumTipoPersona)s.ID_TIPO_PERSONA;
                    //            var TipoVisita = string.Empty;
                    //            switch (TipoPersona)
                    //            {
                    //                case enumTipoPersona.ABOGADO:
                    //                    TipoVisita = string.Format(VISITA_LEGAL + " ({0})", s.TIPO_PERSONA != null ? !string.IsNullOrEmpty(s.TIPO_PERSONA.DESCR) ? s.TIPO_PERSONA.DESCR.TrimEnd() : string.Empty : string.Empty);
                    //                    break;
                    //                case enumTipoPersona.VISITA:
                    //                    TipoVisita = s.ADUANA_INGRESO.Any() ? string.Format("{0}", s.ADUANA_INGRESO.Where(w => w.ID_ADUANA == s.ID_ADUANA).FirstOrDefault().INTIMA == ES_VISITA_INTIMA ? VISITA_INTIMA : VISITA_FAMILIAR) : string.Empty;
                    //                    break;
                    //            }
                    //            return TipoVisita;
                    //        })(),
                    //        Categoria = ObtenerEdad(s.PERSONA.FEC_NACIMIENTO.Value) < MAYORIA_DE_EDAD ? MENORES : (s.PERSONA.SEXO == FEMENINO ? MUJERES : HOMBRES),
                    //    }).
                    //    ToList();
                    #endregion
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ReportViewerVisible = Visibility.Visible;
                    }));

                });


                #region Reporte
                Reporte.LocalReport.ReportPath = "Reportes/rControlVisitantes.rdlc";
                Reporte.LocalReport.DataSources.Clear();

                ReportDataSource ReportDataSource_Encabezado = new ReportDataSource();
                ReportDataSource_Encabezado.Name = "DataSet2";
                ReportDataSource_Encabezado.Value = datosReporte;

                ReportDataSource ReportDataSource = new ReportDataSource();
                ReportDataSource.Name = "DataSet1";
                ReportDataSource.Value = lVisitas;


                Reporte.LocalReport.DataSources.Add(ReportDataSource_Encabezado);
                Reporte.LocalReport.DataSources.Add(ReportDataSource);
                #endregion

                #region Grafica
                ReporteGrafica.LocalReport.ReportPath = "Reportes/rControlVisitantesGrafica.rdlc";
                ReporteGrafica.LocalReport.DataSources.Clear();

                ReportDataSource ReportDataSource_Encabezado_Grafica = new ReportDataSource();
                ReportDataSource_Encabezado_Grafica.Name = "DataSet1";
                ReportDataSource_Encabezado_Grafica.Value = datosReporte;

                ReportDataSource ReportDataSource_Grafica = new ReportDataSource();
                ReportDataSource_Grafica.Name = "DataSet3";
                ReportDataSource_Grafica.Value = lVisitas;

                ReporteGrafica.LocalReport.DataSources.Add(ReportDataSource_Encabezado_Grafica);
                ReporteGrafica.LocalReport.DataSources.Add(ReportDataSource_Grafica);
                #endregion

                #region Parametros
                Reporte.LocalReport.SetParameters(new ReportParameter("FechaInicial", string.Format("{0} DE {1} DEL {2}", SelectedFechaInicial.Day, SelectedFechaInicial.Month, SelectedFechaInicial.Year)));
                Reporte.LocalReport.SetParameters(new ReportParameter("FechaFinal", string.Format("{0} DE {1} DEL {2}", SelectedFechaFinal.Day, SelectedFechaFinal.Month, SelectedFechaFinal.Year)));
                ReporteGrafica.LocalReport.SetParameters(new ReportParameter("FechaInicial", string.Format("{0} DE {1} DEL {2}", SelectedFechaInicial.Day, SelectedFechaInicial.Month, SelectedFechaInicial.Year)));
                ReporteGrafica.LocalReport.SetParameters(new ReportParameter("FechaFinal", string.Format("{0} DE {1} DEL {2}", SelectedFechaFinal.Day, SelectedFechaFinal.Month, SelectedFechaFinal.Year)));
                ReporteGrafica.LocalReport.SetParameters(new ReportParameter("TotalVisitantes", lVisitas.Count.ToString()));
                #endregion


                Reporte.Refresh();
                Reporte.RefreshReport();
                ReporteGrafica.Refresh();
                ReporteGrafica.RefreshReport();
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }

        }

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_CONTROL_VISITANTES.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
