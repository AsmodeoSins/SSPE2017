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
    public partial class ReportePoblacionDelitoComunViewModel : ValidationViewModelBase
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
                var datosReporte = new List<cReporteDatos>();
                datosReporte.Add(new cReporteDatos()
                {
                    Encabezado1 = Parametro.ENCABEZADO1,
                    Encabezado2 = Parametro.ENCABEZADO2,
                    Encabezado3 = Parametro.ENCABEZADO3,
                    Logo1 = Parametro.REPORTE_LOGO1,
                    Logo2 = Parametro.REPORTE_LOGO2,
                    Titulo = OrdenarPor == 1 ? "Población por delito común" : "Población por delito federal",
                    Centro = centro.DESCR.ToUpper().Trim()
                });
                var reporte = new cSentenciaDelito().ObtenerPoblacionDelito(GlobalVar.gCentro, OrdenarPor == 1 ? "C" : "F").Select(w => new cPoblacionDelitoComun()
                {
                    DelitoComun = w.DELITO,
                    Sexo = w.SEXO,
                    Indiciados = w.INDICIADO,
                    Procesados = w.PROCESADO,
                    Sentenciados = w.SENTENCIADO,
                    Total = w.TOTAL
                });
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    Reporte.Reset();
                }));
                Reporte.LocalReport.ReportPath = "../../Reportes/rPoblacionPorDelitoComun.rdlc";
                Reporte.LocalReport.DataSources.Clear();

                ReportDataSource rds1 = new ReportDataSource();
                rds1.Name = "DataSet1";
                rds1.Value = datosReporte;
                Reporte.LocalReport.DataSources.Add(rds1);

                ReportDataSource rds2 = new ReportDataSource();
                rds2.Name = "DataSet2";
                rds2.Value = reporte;
                Reporte.LocalReport.DataSources.Add(rds2);

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
                switch (OrdenarPor)
                {
                    case 1://DELITO COMUN
                        var mes = 12;
                        //var mes = Fechas.GetFechaDateServer.Month;
                        //NOTA: ANIO TEMPORALMENTE FIJO SOLO PARA PRUEBAS DEBIDO A QUE NO HAY REGISTROS PARA EL ANIO 2016
                        //DESCOMENTAR MAS ADELANTE
                        var anio = 2015;//Fechas.GetFechaDateServer.Year;
                        //var lista_internos_mes = internos_lista.Where(w => w.FEC_INGRESO_CERESO.Value.Month == mes).ToList();
                        var datosReporte = new List<cReporteDatos>();
                        // List<string> termsList = new List<string>();
                        datosReporte.Add(new cReporteDatos()
                        {
                            Encabezado1 = Parametro.ENCABEZADO1,
                            Encabezado2 = Parametro.ENCABEZADO2,
                            Encabezado3 = Parametro.ENCABEZADO3,
                            Logo1 = Parametro.REPORTE_LOGO1,
                            Logo2 = Parametro.REPORTE_LOGO2,
                            Titulo = "Población por delito común"
                        });
                        var lst_internos_causa_penal_delito = new cCausaPenalDelito().ObtenerTodos().ToList();
                        var internos_lista = new cIngreso().ObtenerIngresosActivos(GlobalVar.gCentro).Where(w => w.FEC_INGRESO_CERESO.Value.Month == mes && w.FEC_INGRESO_CERESO.Value.Year == anio).ToList();
                        int x = 0;
                        var lst_imp = new List<cPoblacionDelitoComun>();

                        foreach (var item in lst_internos_causa_penal_delito)
                        {
                            var interno = internos_lista.Where(w => w.ID_CENTRO == item.ID_CENTRO && w.ID_ANIO == item.ID_ANIO && w.ID_IMPUTADO == item.ID_IMPUTADO && w.ID_INGRESO == w.ID_INGRESO).FirstOrDefault();
                            if (interno != null)
                            {
                                if (item.CAUSA_PENAL.ID_ESTATUS_CP.Value == 1 && item.ID_FUERO == "C")
                                {
                                    var obj = new cPoblacionDelitoComun();
                                    obj.DelitoComun = item.MODALIDAD_DELITO.DELITO.DESCR;
                                    obj.Sexo = item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO;
                                    obj.Indiciados = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "I" ? 1 : 0;
                                    obj.Procesados = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "2" ? 1 : 0;
                                    obj.Sentenciados = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "3" ? 1 : 0;

                                    lst_imp.Add(obj);
                                }
                            }
                        }
                        var results = lst_imp.GroupBy(n => n.DelitoComun).
                             Select(group =>
                                 new
                                 {
                                     DelitoComun = group.Key,
                                     Sexo = group.ToList(),
                                 });

                        var lst_imp2 = new List<cPoblacionDelitoComun>();

                        foreach (var row in results)
                        {
                            var obj = new cPoblacionDelitoComun();
                            obj.DelitoComun = row.DelitoComun;
                            foreach (var item in row.Sexo)
                            {
                                obj.Sexo = item.Sexo == "M" ? "MASCULINO" : item.Sexo == "F" ? "FEMENINO" : string.Empty;
                                obj.Indiciados += item.Indiciados;
                                obj.Procesados += item.Procesados;
                                obj.Sentenciados += item.Sentenciados;

                            }
                            lst_imp2.Add(obj);
                        }
                        Reporte.LocalReport.ReportPath = "../../Reportes/rPoblacionPorDelitoComun.rdlc";
                        Reporte.LocalReport.DataSources.Clear();

                        ReportDataSource rds1 = new ReportDataSource();
                        rds1.Name = "DataSet1";
                        rds1.Value = datosReporte;
                        Reporte.LocalReport.DataSources.Add(rds1);

                        ReportDataSource rds2 = new ReportDataSource();
                        rds2.Name = "DataSet2";
                        rds2.Value = lst_imp2;
                        Reporte.LocalReport.DataSources.Add(rds2);

                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            Reporte.Refresh();
                            Reporte.RefreshReport();
                        }));
                        break;
                    case 2://DELITO FEDERAL
                        var mes2 = 12;
                        //var mes2 = Fechas.GetFechaDateServer.Month;
                        //NOTA: ANIO TEMPORALMENTE FIJO SOLO PARA PRUEBAS DEBIDO A QUE NO HAY REGISTROS PARA EL ANIO 2016
                        //DESCOMENTAR MAS ADELANTE
                        var anio2 = 2015;//Fechas.GetFechaDateServer.Year;

                        //var lista_internos_mes = internos_lista.Where(w => w.FEC_INGRESO_CERESO.Value.Month == mes).ToList();
                        var datosReporte2 = new List<cReporteDatos>();
                        // List<string> termsList = new List<string>();
                        datosReporte2.Add(new cReporteDatos()
                        {
                            Encabezado1 = Parametro.ENCABEZADO1,
                            Encabezado2 = Parametro.ENCABEZADO2,
                            Encabezado3 = Parametro.ENCABEZADO3,
                            Logo1 = Parametro.REPORTE_LOGO1,
                            Logo2 = Parametro.REPORTE_LOGO2,
                            Titulo = "Población por delito federal"
                        });
                        var lst_internos_causa_penal_delito2 = new cCausaPenalDelito().ObtenerTodos().ToList();
                        var internos_lista2 = new cIngreso().ObtenerIngresosActivos(GlobalVar.gCentro).Where(w => w.FEC_INGRESO_CERESO.Value.Month == mes2 && w.FEC_INGRESO_CERESO.Value.Year == anio2).ToList();
                        var lst_imp3 = new List<cPoblacionDelitoFeferal>();

                        foreach (var item in lst_internos_causa_penal_delito2)
                        {
                            var interno = internos_lista2.Where(w => w.ID_CENTRO == item.ID_CENTRO && w.ID_ANIO == item.ID_ANIO && w.ID_IMPUTADO == item.ID_IMPUTADO && w.ID_INGRESO == w.ID_INGRESO).FirstOrDefault();
                             if (interno != null)
                             {
                                 if (item.CAUSA_PENAL.ID_ESTATUS_CP.Value == 1 && item.ID_FUERO == "F")
                                 {
                                     var obj = new cPoblacionDelitoFeferal();
                                     obj.DelitoFederal = item.MODALIDAD_DELITO.DELITO.DESCR;
                                     obj.Sexo = item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO;
                                     obj.Indiciados = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "I" ? 1 : 0;
                                     obj.Procesados = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "2" ? 1 : 0;
                                     obj.Sentenciados = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "3" ? 1 : 0;

                                     lst_imp3.Add(obj);
                                 }
                             }
                        }
                        var results2 = lst_imp3.GroupBy(n => n.DelitoFederal).
                             Select(group =>
                                 new
                                 {
                                     DelitoFederal = group.Key,
                                     Sexo = group.ToList(),
                                 });

                        var lst_imp4 = new List<cPoblacionDelitoFeferal>();

                        foreach (var row in results2)
                        {
                            var obj = new cPoblacionDelitoFeferal();
                            obj.DelitoFederal = row.DelitoFederal;
                            foreach (var item in row.Sexo)
                            {
                                obj.Sexo = item.Sexo == "M" ? "MASCULINO" : item.Sexo == "F" ? "FEMENINO" : string.Empty;
                                obj.Indiciados += item.Indiciados;
                                obj.Procesados += item.Procesados;
                                obj.Sentenciados += item.Sentenciados;
                            }
                            lst_imp4.Add(obj);
                        }
                        Reporte.LocalReport.ReportPath = "../../Reportes/rPoblacionDelitoFederal.rdlc";
                        Reporte.LocalReport.DataSources.Clear();

                        ReportDataSource rds12 = new ReportDataSource();
                        rds12.Name = "DataSet1";
                        rds12.Value = datosReporte2;
                        Reporte.LocalReport.DataSources.Add(rds12);

                        ReportDataSource rds22 = new ReportDataSource();
                        rds22.Name = "DataSet2";
                        rds22.Value = lst_imp4;
                        Reporte.LocalReport.DataSources.Add(rds22);
                 
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            Reporte.Refresh();
                            Reporte.RefreshReport();
                        }));
                        break;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar reporte.", ex);
            }
        }

        private void OnLoad(ReportePoblacionDelitoComunView window)
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
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_POBLACION_DELITO.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
