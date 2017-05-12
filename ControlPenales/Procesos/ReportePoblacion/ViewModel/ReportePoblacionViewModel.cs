using Microsoft.Reporting.WinForms;
using SSP.Controlador.Catalogo.Justicia;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlPenales
{
    public partial class ReportePoblacionViewModel : ValidationViewModelBase
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
                //NOTA: ANIO TEMPORALMENTE FIJO SOLO PARA PRUEBAS DEBIDO A QUE NO HAY REGISTROS PARA EL ANIO 2016
                //DESCOMENTAR MAS ADELANTE
                //var mes = Fechas.GetFechaDateServer.Month;
                //var anio = Fechas.GetFechaDateServer.Year;
                var fecha = Fechas.GetFechaDateServer;
                CultureInfo cultura = new CultureInfo("es-MX");
                var datosReporte = new List<cReporteDatos>();
                datosReporte.Add(new cReporteDatos()
                {
                    Encabezado1 = Parametro.ENCABEZADO1,
                    Encabezado2 = Parametro.ENCABEZADO2,
                    Encabezado3 = Parametro.ENCABEZADO3,
                    Logo1 = Parametro.REPORTE_LOGO1,
                    Logo2 = Parametro.REPORTE_LOGO2,
                    Titulo = string.Format("Población {0} del {1}",cultura.DateTimeFormat.GetMonthName(fecha.Month),fecha.Year),
                });
                var lst_internos_causa_penal_delito = new cCausaPenalDelito().ObtenerTodos().ToList();
                var internos_lista = new cIngreso().ObtenerIngresosActivos(GlobalVar.gCentro).ToList();//.Where(w => w.FEC_INGRESO_CERESO.Value.Year == anio && w.FEC_INGRESO_CERESO.Value.Month == mes).ToList();
                var lst_imp = new List<cPoblacion>();
                var lst_imp_grafica = new List<cGraficaPoblacionM>();
                var lst_imp_grafica_f = new List<cGraficaPoblacionF>();

                foreach (var item in internos_lista)
                {
                    var interno = lst_internos_causa_penal_delito.Where(w => w.ID_CENTRO == item.ID_CENTRO && w.ID_ANIO == item.ID_ANIO && w.ID_IMPUTADO == item.ID_IMPUTADO && w.ID_INGRESO == w.ID_INGRESO)
                        .FirstOrDefault();
                    if (interno == null)
                    {
                        var obj = new cPoblacion();
                        obj.Fuero = "Sin Fuero";//item.CAUSA_PENAL == null ? string.Empty : item.IMPUTADO.ENTIDAD.DESCR;
                        //obj.Causa = "Sin Fuero";
                        obj.ImpFem = item.ID_CLASIFICACION_JURIDICA == "I" && item.NUC == null && item.IMPUTADO.SEXO == "F" ? 1 : 0;
                        obj.ImpMasc = item.ID_CLASIFICACION_JURIDICA == "I" && item.NUC == null && item.IMPUTADO.SEXO == "M" ? 1 : 0;
                        obj.IndicFem = item.ID_CLASIFICACION_JURIDICA == "I" && item.IMPUTADO.SEXO == "F" ? 1 : 0;
                        obj.IndicMasc = item.ID_CLASIFICACION_JURIDICA == "I" && item.IMPUTADO.SEXO == "M" ? 1 : 0;
                        obj.ProcFem = item.ID_CLASIFICACION_JURIDICA == "2" && item.IMPUTADO.SEXO == "F" ? 1 : 0;
                        obj.ProcMasc = item.ID_CLASIFICACION_JURIDICA == "2" && item.IMPUTADO.SEXO == "M" ? 1 : 0;
                        obj.SentFem = item.ID_CLASIFICACION_JURIDICA == "3" && item.IMPUTADO.SEXO == "F" ? 1 : 0;
                        obj.SentMasc = item.ID_CLASIFICACION_JURIDICA == "3" && item.IMPUTADO.SEXO == "M" ? 1 : 0;
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
                            var obj = new cPoblacion();
                            obj.Fuero = "Comun";//item.CAUSA_PENAL.INGRESO.IMPUTADO.ENTIDAD.DESCR;
                            //obj.Causa = "Comun";
                            obj.ImpFem = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "I" && item.CAUSA_PENAL.INGRESO.NUC == null && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "F" ? 1 : 0;
                            obj.ImpMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "I" && item.CAUSA_PENAL.INGRESO.NUC == null && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                            obj.IndicFem = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "I" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "F" ? 1 : 0;
                            obj.IndicMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "I" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                            obj.ProcFem = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "2" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "F" ? 1 : 0;
                            obj.ProcMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "2" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                            obj.SentFem = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "3" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "F" ? 1 : 0;
                            obj.SentMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "3" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                            lst_imp.Add(obj);
                        }
                        else if (item.CAUSA_PENAL.ID_ESTATUS_CP.Value == 1 && item.ID_FUERO == "F")
                        {
                            var obj = new cPoblacion();
                            obj.Fuero = "Federal";
                            //obj.Causa = "Federal";
                            obj.ImpFem = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "I" && item.CAUSA_PENAL.INGRESO.NUC == null && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "F" ? 1 : 0;
                            obj.ImpMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "I" && item.CAUSA_PENAL.INGRESO.NUC == null && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                            obj.IndicFem = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "I" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "F" ? 1 : 0;
                            obj.IndicMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "I" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                            obj.ProcFem = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "2" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "F" ? 1 : 0;
                            obj.ProcMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "2" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                            obj.SentFem = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "3" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "F" ? 1 : 0;
                            obj.SentMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "3" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                            lst_imp.Add(obj);
                        }
                    }
                }
                //Para alimentar grafica Masculino
                foreach (var item in lst_imp)
                {

                    if (item.Fuero == "Comun")
                    {
                        if (item.ImpMasc > 0)
                        {
                            var obj = new cGraficaPoblacionM();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Imputado";
                            obj.Comun += item.ImpMasc;
                            lst_imp_grafica.Add(obj);
                        }
                        if (item.IndicMasc > 0)
                        {
                            var obj = new cGraficaPoblacionM();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Indiciado";
                            obj.Comun += item.IndicMasc;
                            lst_imp_grafica.Add(obj);
                        }
                        if (item.ProcMasc > 0)
                        {
                            var obj = new cGraficaPoblacionM();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Procesado";
                            obj.Comun += item.ProcMasc;
                            lst_imp_grafica.Add(obj);
                        }
                        if (item.SentMasc > 0)
                        {
                            var obj = new cGraficaPoblacionM();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Sentenciado";
                            obj.Comun += item.SentMasc;
                            lst_imp_grafica.Add(obj);
                        }
                    }
                    if (item.Fuero == "Federal")
                    {
                        if (item.ImpMasc > 0)
                        {
                            var obj = new cGraficaPoblacionM();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Imputado";
                            obj.Comun += item.ImpMasc;
                            lst_imp_grafica.Add(obj);
                        }
                        if (item.IndicMasc > 0)
                        {
                            var obj = new cGraficaPoblacionM();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Indiciado";
                            obj.Comun += item.IndicMasc;
                            lst_imp_grafica.Add(obj);
                        }
                        if (item.ProcMasc > 0)
                        {
                            var obj = new cGraficaPoblacionM();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Procesado";
                            obj.Comun += item.ProcMasc;
                            lst_imp_grafica.Add(obj);
                        }
                        if (item.SentMasc > 0)
                        {
                            var obj = new cGraficaPoblacionM();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Sentenciado";
                            obj.Comun += item.SentMasc;
                            lst_imp_grafica.Add(obj);
                        }
                    }
                    if (item.Fuero == "Sin Fuero")
                    {
                        if (item.ImpMasc > 0)
                        {
                            var obj = new cGraficaPoblacionM();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Imputado";
                            obj.Comun += item.ImpMasc;
                            lst_imp_grafica.Add(obj);
                        }
                        if (item.IndicMasc > 0)
                        {
                            var obj = new cGraficaPoblacionM();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Indiciado";
                            obj.Comun += item.IndicMasc;
                            lst_imp_grafica.Add(obj);
                        }
                        if (item.ProcMasc > 0)
                        {
                            var obj = new cGraficaPoblacionM();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Procesado";
                            obj.Comun += item.ProcMasc;
                            lst_imp_grafica.Add(obj);
                        }
                        if (item.SentMasc > 0)
                        {
                            var obj = new cGraficaPoblacionM();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Sentenciado";
                            obj.Comun += item.SentMasc;
                            lst_imp_grafica.Add(obj);
                        }
                    }
                }
                //Para alimentar grafica Femenino
                foreach (var item in lst_imp)
                {

                    if (item.Fuero == "Comun")
                    {
                        if (item.ImpFem > 0)
                        {
                            var obj = new cGraficaPoblacionF();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Imputado";
                            obj.Comun += item.ImpFem;
                            lst_imp_grafica_f.Add(obj);
                        }
                        if (item.IndicFem > 0)
                        {
                            var obj = new cGraficaPoblacionF();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Indiciado";
                            obj.Comun += item.IndicFem;
                            lst_imp_grafica_f.Add(obj);
                        }
                        if (item.ProcFem > 0)
                        {
                            var obj = new cGraficaPoblacionF();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Procesado";
                            obj.Comun += item.ProcFem;
                            lst_imp_grafica_f.Add(obj);
                        }
                        if (item.SentFem > 0)
                        {
                            var obj = new cGraficaPoblacionF();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Sentenciado";
                            obj.Comun += item.SentFem;
                            lst_imp_grafica_f.Add(obj);
                        }
                    }
                    if (item.Fuero == "Federal")
                    {
                        if (item.ImpFem > 0)
                        {
                            var obj = new cGraficaPoblacionF();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Imputado";
                            obj.Comun += item.ImpFem;
                            lst_imp_grafica_f.Add(obj);
                        }
                        if (item.IndicFem > 0)
                        {
                            var obj = new cGraficaPoblacionF();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Indiciado";
                            obj.Comun += item.IndicFem;
                            lst_imp_grafica_f.Add(obj);
                        }
                        if (item.ProcFem > 0)
                        {
                            var obj = new cGraficaPoblacionF();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Procesado";
                            obj.Comun += item.ProcFem;
                            lst_imp_grafica_f.Add(obj);
                        }
                        if (item.SentFem > 0)
                        {
                            var obj = new cGraficaPoblacionF();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Sentenciado";
                            obj.Comun += item.SentFem;
                            lst_imp_grafica_f.Add(obj);
                        }
                    }
                    if (item.Fuero == "Sin Fuero")
                    {
                        if (item.ImpFem > 0)
                        {
                            var obj = new cGraficaPoblacionF();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Imputado";
                            obj.Comun += item.ImpFem;
                            lst_imp_grafica_f.Add(obj);
                        }
                        if (item.IndicFem > 0)
                        {
                            var obj = new cGraficaPoblacionF();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Indiciado";
                            obj.Comun += item.IndicFem;
                            lst_imp_grafica_f.Add(obj);
                        }
                        if (item.ProcFem > 0)
                        {
                            var obj = new cGraficaPoblacionF();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Procesado";
                            obj.Comun += item.ProcFem;
                            lst_imp_grafica_f.Add(obj);
                        }
                        if (item.SentFem > 0)
                        {
                            var obj = new cGraficaPoblacionF();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Sentenciado";
                            obj.Comun += item.SentFem;
                            lst_imp_grafica_f.Add(obj);
                        }
                    }
                }
                var results = lst_imp.GroupBy(n => n.Fuero).
                     Select(group =>
                         new
                         {
                             Fuero = group.Key,
                             Sexo = group.ToList(),
                         });

                var lst_imp2 = new List<cPoblacion>();
                foreach (var row in results)
                {
                    var obj = new cPoblacion();
                    obj.Fuero = row.Fuero;
                    foreach (var item in row.Sexo)
                    {
                        obj.ImpFem += item.ImpFem;
                        obj.ImpMasc += item.ImpMasc;
                        obj.IndicFem += item.IndicFem;
                        obj.IndicMasc += item.IndicMasc;
                        obj.ProcFem += item.ProcFem;
                        obj.ProcMasc += item.ProcMasc;
                        obj.SentFem += item.SentFem;
                        obj.SentMasc += item.SentMasc;
                    }
                    lst_imp2.Add(obj);
                }
                Reporte.LocalReport.ReportPath = "Reportes/rPoblacion.rdlc";
                Reporte.LocalReport.DataSources.Clear();

                ReportDataSource rds1 = new ReportDataSource();
                rds1.Name = "DataSet1";
                rds1.Value = datosReporte;
                Reporte.LocalReport.DataSources.Add(rds1);

                ReportDataSource rds2 = new ReportDataSource();
                rds2.Name = "DataSet2";
                rds2.Value = lst_imp2.OrderBy(o => o.Fuero);
                Reporte.LocalReport.DataSources.Add(rds2);

                ReportDataSource rds3 = new ReportDataSource();
                rds3.Name = "DataSet3";
                rds3.Value = lst_imp_grafica_f;
                Reporte.LocalReport.DataSources.Add(rds3);

                ReportDataSource rds4 = new ReportDataSource();
                rds4.Name = "DataSet4";
                rds4.Value = lst_imp_grafica;
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

        private void OnLoad(ReportePoblacionView window)
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
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_POBLACION.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
