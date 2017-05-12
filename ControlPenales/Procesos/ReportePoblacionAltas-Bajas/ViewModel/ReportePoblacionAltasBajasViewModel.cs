using Microsoft.Reporting.WinForms;
using SSP.Controlador.Catalogo.Justicia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlPenales
{
    public partial class ReportePoblacionAltasBajasViewModel : ValidationViewModelBase
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
                var mes = Fechas.GetFechaDateServer.Month;
                switch (OrdenarPor)
                {
                    case 1://expediente
                        mes = 1;
                        break;
                    case 2://nombre
                        mes = 2;
                        break;
                    case 3://Ubicacion
                        mes = 3;
                        break;
                    case 4://expediente
                        mes = 4;
                        break;
                    case 5://nombre
                        mes = 5;
                        break;
                    case 6://Ubicacion
                        mes = 6;
                        break;
                    case 7://expediente
                        mes = 7;
                        break;
                    case 8://nombre
                        mes = 8;
                        break;
                    case 9://Ubicacion
                        mes = 9;
                        break;
                    case 10://expediente
                        mes = 10;
                        break;
                    case 11://nombre
                        mes = 11;
                        break;
                    case 12://Ubicacion
                        mes = 12;
                        break;
                }
                var anio = Fechas.GetFechaDateServer.Year;
                var datosReporte = new List<cReporteDatos>();
                datosReporte.Add(new cReporteDatos()
                {
                    Encabezado1 = Parametro.ENCABEZADO1,
                    Encabezado2 = Parametro.ENCABEZADO2,
                    Encabezado3 = Parametro.ENCABEZADO3,
                    Logo1 = Parametro.REPORTE_LOGO1,
                    Logo2 = Parametro.REPORTE_LOGO2,
                    Titulo = "Altas-Bajas"
                });
                var lst_internos_causa_penal_delito = new cCausaPenalDelito().ObtenerTodos().ToList();
                var internos_lista = new cIngreso().ObtenerIngresosActivos(GlobalVar.gCentro).ToList();//.Where(w => w.FEC_INGRESO_CERESO.Value.Year == anio && w.FEC_INGRESO_CERESO.Value.Month == mes).ToList();
                var lst_imp_altas = new List<cPoblacionAltasBajas>();
                var lst_imp_bajas = new List<cPoblacionAltasBajas>();
                var lst_total_sexo = new List<cTotalSexo>();
                var lst_imp_grafica_alta_masc = new List<cGraficaAltaBaja>();
                var lst_imp_grafica_baja_masc = new List<cGraficaAltaBaja>();
                var lst_imp_grafica_alta_fem = new List<cGraficaAltaBaja>();
                var lst_imp_grafica_baja_fem = new List<cGraficaAltaBaja>();

                foreach (var item in internos_lista)
                {
                    if (item.FEC_INGRESO_CERESO != null)
                    {
                        if ((item.FEC_INGRESO_CERESO.Value.Year == anio && item.FEC_INGRESO_CERESO.Value.Month == mes) || (item.FEC_INGRESO_CERESO >= FechaInicio && item.FEC_INGRESO_CERESO <= FechaFin))
                        {
                            var interno = lst_internos_causa_penal_delito.Where(w => w.ID_CENTRO == item.ID_CENTRO && w.ID_ANIO == item.ID_ANIO && w.ID_IMPUTADO == item.ID_IMPUTADO && w.ID_INGRESO == w.ID_INGRESO).FirstOrDefault();
                            if (interno == null)
                            {
                                var obj = new cPoblacionAltasBajas();
                                var row = new cTotalSexo();
                                obj.Fuero = "Sin Fuero";//item.CAUSA_PENAL == null ? string.Empty : item.IMPUTADO.ENTIDAD.DESCR;
                                obj.Tipo = "Alta";
                                row.Masculino = item.IMPUTADO.SEXO == "M" ? 1 : 0;
                                row.Femenino = item.IMPUTADO.SEXO == "F" ? 1 : 0;
                                row.SinSexo = item.IMPUTADO.SEXO == null ? 1 : 0;
                                obj.DiscFem = item.ID_CLASIFICACION_JURIDICA == "4" && item.IMPUTADO.SEXO == "F" ? 1 : 0;
                                obj.DiscMasc = item.ID_CLASIFICACION_JURIDICA == "4" && item.IMPUTADO.SEXO == "M" ? 1 : 0;
                                obj.IndicFem = item.ID_CLASIFICACION_JURIDICA == "I" && item.IMPUTADO.SEXO == "F" ? 1 : 0;
                                obj.IndicMasc = item.ID_CLASIFICACION_JURIDICA == "I" && item.IMPUTADO.SEXO == "M" ? 1 : 0;
                                obj.ProcFem = item.ID_CLASIFICACION_JURIDICA == "2" && item.IMPUTADO.SEXO == "F" ? 1 : 0;
                                obj.ProcMasc = item.ID_CLASIFICACION_JURIDICA == "2" && item.IMPUTADO.SEXO == "M" ? 1 : 0;
                                obj.SentFem = item.ID_CLASIFICACION_JURIDICA == "3" && item.IMPUTADO.SEXO == "F" ? 1 : 0;
                                obj.SentMasc = item.ID_CLASIFICACION_JURIDICA == "3" && item.IMPUTADO.SEXO == "M" ? 1 : 0;
                                lst_total_sexo.Add(row);
                                lst_imp_altas.Add(obj);
                            }
                        }
                    }
                    if (item.FEC_SALIDA_CERESO != null)
                    {
                        if ((item.FEC_SALIDA_CERESO.Value.Year == anio && item.FEC_SALIDA_CERESO.Value.Month == mes) || (item.FEC_SALIDA_CERESO >= FechaInicio && item.FEC_SALIDA_CERESO <= FechaFin))
                        {
                            var interno = lst_internos_causa_penal_delito.Where(w => w.ID_CENTRO == item.ID_CENTRO && w.ID_ANIO == item.ID_ANIO && w.ID_IMPUTADO == item.ID_IMPUTADO && w.ID_INGRESO == w.ID_INGRESO).FirstOrDefault();
                            if (interno == null)
                            {
                                var obj = new cPoblacionAltasBajas();
                                var row = new cTotalSexo();
                                obj.Fuero = "Sin Fuero";//item.CAUSA_PENAL == null ? string.Empty : item.IMPUTADO.ENTIDAD.DESCR;
                                obj.Tipo = "Baja";
                                row.Masculino = item.IMPUTADO.SEXO == "M" ? 1 : 0;
                                row.Femenino = item.IMPUTADO.SEXO == "F" ? 1 : 0;
                                row.SinSexo = item.IMPUTADO.SEXO == null ? 1 : 0;
                                obj.DiscFem = item.ID_CLASIFICACION_JURIDICA == "4" && item.IMPUTADO.SEXO == "F" ? 1 : 0;
                                obj.DiscMasc = item.ID_CLASIFICACION_JURIDICA == "4" && item.IMPUTADO.SEXO == "M" ? 1 : 0;
                                obj.ImpMasc = item.ID_CLASIFICACION_JURIDICA == "1" && item.NUC == null && item.IMPUTADO.SEXO == "M" ? 1 : 0;
                                obj.IndicFem = item.ID_CLASIFICACION_JURIDICA == "I" && item.IMPUTADO.SEXO == "F" ? 1 : 0;
                                obj.IndicMasc = item.ID_CLASIFICACION_JURIDICA == "I" && item.IMPUTADO.SEXO == "M" ? 1 : 0;
                                obj.ProcFem = item.ID_CLASIFICACION_JURIDICA == "2" && item.IMPUTADO.SEXO == "F" ? 1 : 0;
                                obj.ProcMasc = item.ID_CLASIFICACION_JURIDICA == "2" && item.IMPUTADO.SEXO == "M" ? 1 : 0;
                                obj.SentFem = item.ID_CLASIFICACION_JURIDICA == "3" && item.IMPUTADO.SEXO == "F" ? 1 : 0;
                                obj.SentMasc = item.ID_CLASIFICACION_JURIDICA == "3" && item.IMPUTADO.SEXO == "M" ? 1 : 0;
                                lst_total_sexo.Add(row);
                                lst_imp_bajas.Add(obj);
                            }
                        }
                    }
                }

                foreach (var item in lst_internos_causa_penal_delito)
                {
                    var interno = internos_lista.Where(w => w.ID_CENTRO == item.ID_CENTRO && w.ID_ANIO == item.ID_ANIO && w.ID_IMPUTADO == item.ID_IMPUTADO && w.ID_INGRESO == w.ID_INGRESO).FirstOrDefault();
                    if (interno != null)
                    {
                        if (item.CAUSA_PENAL.INGRESO.FEC_INGRESO_CERESO != null)
                        {
                            if ((item.CAUSA_PENAL.INGRESO.FEC_INGRESO_CERESO.Value.Year == anio && item.CAUSA_PENAL.INGRESO.FEC_INGRESO_CERESO.Value.Month == mes) || (item.CAUSA_PENAL.INGRESO.FEC_INGRESO_CERESO >= FechaInicio && item.CAUSA_PENAL.INGRESO.FEC_INGRESO_CERESO <= FechaFin))
                            {
                                if (item.CAUSA_PENAL.ID_ESTATUS_CP.Value == 1 && item.ID_FUERO == "C")
                                {
                                    var obj = new cPoblacionAltasBajas();
                                    var row = new cTotalSexo();
                                    obj.Fuero = "Comun";//item.CAUSA_PENAL.INGRESO.IMPUTADO.ENTIDAD.DESCR;
                                    obj.Tipo = "Alta";
                                    row.Masculino = item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    row.Femenino = item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "F" ? 1 : 0;
                                    row.SinSexo = item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == null ? 1 : 0;
                                    obj.DiscFem = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "4" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "F" ? 1 : 0;
                                    obj.DiscMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "4" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    obj.IndicFem = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "I" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "F" ? 1 : 0;
                                    obj.IndicMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "I" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    obj.ProcFem = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "2" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "F" ? 1 : 0;
                                    obj.ProcMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "2" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    obj.SentFem = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "3" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "F" ? 1 : 0;
                                    obj.SentMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "3" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    lst_total_sexo.Add(row);
                                    lst_imp_altas.Add(obj);
                                }
                                else if (item.CAUSA_PENAL.ID_ESTATUS_CP.Value == 1 && item.ID_FUERO == "F")
                                {
                                    var obj = new cPoblacionAltasBajas();
                                    var row = new cTotalSexo();
                                    obj.Fuero = "Federal";
                                    obj.Tipo = "Alta";
                                    row.Masculino = item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    row.Femenino = item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "F" ? 1 : 0;
                                    row.SinSexo = item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == null ? 1 : 0;
                                    obj.DiscFem = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "4" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "F" ? 1 : 0;
                                    obj.DiscMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "4" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    obj.IndicFem = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "I" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "F" ? 1 : 0;
                                    obj.IndicMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "I" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    obj.ProcFem = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "2" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "F" ? 1 : 0;
                                    obj.ProcMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "2" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    obj.SentFem = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "3" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "F" ? 1 : 0;
                                    obj.SentMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "3" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    lst_total_sexo.Add(row);
                                    lst_imp_altas.Add(obj);
                                }
                            }
                        }
                        if (item.CAUSA_PENAL.INGRESO.FEC_SALIDA_CERESO != null)
                        {
                            if ((item.CAUSA_PENAL.INGRESO.FEC_SALIDA_CERESO.Value.Year == anio && item.CAUSA_PENAL.INGRESO.FEC_SALIDA_CERESO.Value.Month == mes) || (item.CAUSA_PENAL.INGRESO.FEC_SALIDA_CERESO >= FechaInicio && item.CAUSA_PENAL.INGRESO.FEC_SALIDA_CERESO <= FechaFin))
                            {
                                if (item.CAUSA_PENAL.ID_ESTATUS_CP.Value == 1 && item.ID_FUERO == "C")
                                {
                                    var obj = new cPoblacionAltasBajas();
                                    var row = new cTotalSexo();
                                    obj.Fuero = "Comun";//item.CAUSA_PENAL.INGRESO.IMPUTADO.ENTIDAD.DESCR;
                                    obj.Tipo = "Baja";
                                    row.Masculino = item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    row.Femenino = item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "F" ? 1 : 0;
                                    row.SinSexo = item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == null ? 1 : 0;
                                    obj.DiscFem = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "4" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "F" ? 1 : 0;
                                    obj.DiscMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "4" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    obj.ImpMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "1" && item.CAUSA_PENAL.INGRESO.NUC == null && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    obj.IndicFem = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "I" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "F" ? 1 : 0;
                                    obj.IndicMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "I" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    obj.ProcFem = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "2" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "F" ? 1 : 0;
                                    obj.ProcMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "2" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    obj.SentFem = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "3" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "F" ? 1 : 0;
                                    obj.SentMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "3" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    lst_total_sexo.Add(row);
                                    lst_imp_bajas.Add(obj);
                                }
                                else if (item.CAUSA_PENAL.ID_ESTATUS_CP.Value == 1 && item.ID_FUERO == "F")
                                {
                                    var obj = new cPoblacionAltasBajas();
                                    var row = new cTotalSexo();
                                    obj.Fuero = "Federal";
                                    obj.Tipo = "Baja";
                                    row.Masculino = item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    row.Femenino = item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "F" ? 1 : 0;
                                    row.SinSexo = item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == null ? 1 : 0;
                                    obj.DiscFem = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "4" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "F" ? 1 : 0;
                                    obj.DiscMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "4" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    obj.ImpMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "1" && item.CAUSA_PENAL.INGRESO.NUC == null && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    obj.IndicFem = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "I" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "F" ? 1 : 0;
                                    obj.IndicMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "I" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    obj.ProcFem = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "2" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "F" ? 1 : 0;
                                    obj.ProcMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "2" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    obj.SentFem = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "3" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "F" ? 1 : 0;
                                    obj.SentMasc = item.CAUSA_PENAL.INGRESO.ID_CLASIFICACION_JURIDICA == "3" && item.CAUSA_PENAL.INGRESO.IMPUTADO.SEXO == "M" ? 1 : 0;
                                    lst_total_sexo.Add(row);
                                    lst_imp_bajas.Add(obj);
                                }
                            }
                        }
                    }
                }
                //Para alimentar grafica Alta masculino
                foreach (var item in lst_imp_altas)
                {
                    if (item.Fuero == "Comun" && item.Tipo == "Alta")
                    {
                        if (item.DiscMasc > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Discrecional";
                            obj.Comun += item.DiscMasc;
                            lst_imp_grafica_alta_masc.Add(obj);
                        }
                        if (item.IndicMasc > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Indiciado";
                            obj.Comun += item.IndicMasc;
                            lst_imp_grafica_alta_masc.Add(obj);
                        }
                        if (item.ProcMasc > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Procesado";
                            obj.Comun += item.ProcMasc;
                            lst_imp_grafica_alta_masc.Add(obj);
                        }
                        if (item.SentMasc > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Sentenciado";
                            obj.Comun += item.SentMasc;
                            lst_imp_grafica_alta_masc.Add(obj);
                        }
                    }
                    if (item.Fuero == "Federal" && item.Tipo == "Alta")
                    {
                        if (item.DiscMasc > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Discrecional";
                            obj.Comun += item.DiscMasc;
                            lst_imp_grafica_alta_masc.Add(obj);
                        }
                        if (item.IndicMasc > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Indiciado";
                            obj.Comun += item.IndicMasc;
                            lst_imp_grafica_alta_masc.Add(obj);
                        }
                        if (item.ProcMasc > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Procesado";
                            obj.Comun += item.ProcMasc;
                            lst_imp_grafica_alta_masc.Add(obj);
                        }
                        if (item.SentMasc > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Sentenciado";
                            obj.Comun += item.SentMasc;
                            lst_imp_grafica_alta_masc.Add(obj);
                        }
                    }
                    if (item.Fuero == "Sin Fuero" && item.Tipo == "Alta")
                    {
                        if (item.DiscMasc > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Discrecional";
                            obj.Comun += item.DiscMasc;
                            lst_imp_grafica_alta_masc.Add(obj);
                        }
                        if (item.IndicMasc > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Indiciado";
                            obj.Comun += item.IndicMasc;
                            lst_imp_grafica_alta_masc.Add(obj);
                        }
                        if (item.ProcMasc > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Procesado";
                            obj.Comun += item.ProcMasc;
                            lst_imp_grafica_alta_masc.Add(obj);
                        }
                        if (item.SentMasc > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Sentenciado";
                            obj.Comun += item.SentMasc;
                            lst_imp_grafica_alta_masc.Add(obj);
                        }
                    }
                }
                //Para alimentar grafica alta femenino
                foreach (var item in lst_imp_altas)
                {

                    if (item.Fuero == "Comun" && item.Tipo == "Alta")
                    {
                        if (item.DiscFem > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Discrecional";
                            obj.Comun += item.DiscFem;
                            lst_imp_grafica_alta_fem.Add(obj);
                        }
                        if (item.IndicFem > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Indiciado";
                            obj.Comun += item.IndicFem;
                            lst_imp_grafica_alta_fem.Add(obj);
                        }
                        if (item.ProcFem > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Procesado";
                            obj.Comun += item.ProcFem;
                            lst_imp_grafica_alta_fem.Add(obj);
                        }
                        if (item.SentFem > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Sentenciado";
                            obj.Comun += item.SentFem;
                            lst_imp_grafica_alta_fem.Add(obj);
                        }
                    }
                    if (item.Fuero == "Federal" && item.Tipo == "Alta")
                    {
                        if (item.DiscFem > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Discrecional";
                            obj.Comun += item.DiscFem;
                            lst_imp_grafica_alta_fem.Add(obj);
                        }
                        if (item.IndicFem > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Indiciado";
                            obj.Comun += item.IndicFem;
                            lst_imp_grafica_alta_fem.Add(obj);
                        }
                        if (item.ProcFem > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Procesado";
                            obj.Comun += item.ProcFem;
                            lst_imp_grafica_alta_fem.Add(obj);
                        }
                        if (item.SentFem > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Sentenciado";
                            obj.Comun += item.SentFem;
                            lst_imp_grafica_alta_fem.Add(obj);
                        }
                    }
                    if (item.Fuero == "Sin Fuero" && item.Tipo == "Alta")
                    {
                        if (item.DiscFem > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Discrecional";
                            obj.Comun += item.DiscFem;
                            lst_imp_grafica_alta_fem.Add(obj);
                        }
                        if (item.IndicFem > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Indiciado";
                            obj.Comun += item.IndicFem;
                            lst_imp_grafica_alta_fem.Add(obj);
                        }
                        if (item.ProcFem > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Procesado";
                            obj.Comun += item.ProcFem;
                            lst_imp_grafica_alta_fem.Add(obj);
                        }
                        if (item.SentFem > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Sentenciado";
                            obj.Comun += item.SentFem;
                            lst_imp_grafica_alta_fem.Add(obj);
                        }
                    }
                }
                //Para alimentar grafica baja masculino
                foreach (var item in lst_imp_bajas)
                {
                    if (item.Fuero == "Comun" && item.Tipo == "Baja")
                    {
                        if (item.DiscMasc > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Discrecional";
                            obj.Comun += item.DiscMasc;
                            lst_imp_grafica_baja_masc.Add(obj);
                        }
                        if (item.ImpMasc > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Imputado";
                            obj.Comun += item.ImpMasc;
                            lst_imp_grafica_baja_masc.Add(obj);
                        }
                        if (item.IndicMasc > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Indiciado";
                            obj.Comun += item.IndicMasc;
                            lst_imp_grafica_baja_masc.Add(obj);
                        }
                        if (item.ProcMasc > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Procesado";
                            obj.Comun += item.ProcMasc;
                            lst_imp_grafica_baja_masc.Add(obj);
                        }
                        if (item.SentMasc > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Sentenciado";
                            obj.Comun += item.SentMasc;
                            lst_imp_grafica_baja_masc.Add(obj);
                        }
                    }
                    if (item.Fuero == "Federal" && item.Tipo == "Baja")
                    {
                        if (item.DiscMasc > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Discrecional";
                            obj.Comun += item.DiscMasc;
                            lst_imp_grafica_baja_masc.Add(obj);
                        }
                        if (item.ImpMasc > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Imputado";
                            obj.Comun += item.ImpMasc;
                            lst_imp_grafica_baja_masc.Add(obj);
                        }
                        if (item.IndicMasc > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Indiciado";
                            obj.Comun += item.IndicMasc;
                            lst_imp_grafica_baja_masc.Add(obj);
                        }
                        if (item.ProcMasc > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Procesado";
                            obj.Comun += item.ProcMasc;
                            lst_imp_grafica_baja_masc.Add(obj);
                        }
                        if (item.SentMasc > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Sentenciado";
                            obj.Comun += item.SentMasc;
                            lst_imp_grafica_baja_masc.Add(obj);
                        }
                    }
                    if (item.Fuero == "Sin Fuero" && item.Tipo == "Baja")
                    {
                        if (item.DiscMasc > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Discrecional";
                            obj.Comun += item.DiscMasc;
                            lst_imp_grafica_baja_masc.Add(obj);
                        }
                        if (item.ImpMasc > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Imputado";
                            obj.Comun += item.ImpMasc;
                            lst_imp_grafica_baja_masc.Add(obj);
                        }
                        if (item.IndicMasc > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Indiciado";
                            obj.Comun += item.IndicMasc;
                            lst_imp_grafica_baja_masc.Add(obj);
                        }
                        if (item.ProcMasc > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Procesado";
                            obj.Comun += item.ProcMasc;
                            lst_imp_grafica_baja_masc.Add(obj);
                        }
                        if (item.SentMasc > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Sentenciado";
                            obj.Comun += item.SentMasc;
                            lst_imp_grafica_baja_masc.Add(obj);
                        }
                    }
                }
                //Para alimentar grafica Baja femenino
                foreach (var item in lst_imp_bajas)
                {

                    if (item.Fuero == "Comun" && item.Tipo == "Baja")
                    {
                        if (item.DiscFem > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Discrecional";
                            obj.Comun += item.DiscFem;
                            lst_imp_grafica_baja_fem.Add(obj);
                        }
                        if (item.IndicFem > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Indiciado";
                            obj.Comun += item.IndicFem;
                            lst_imp_grafica_baja_fem.Add(obj);
                        }
                        if (item.ProcFem > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Procesado";
                            obj.Comun += item.ProcFem;
                            lst_imp_grafica_baja_fem.Add(obj);
                        }
                        if (item.SentFem > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Sentenciado";
                            obj.Comun += item.SentFem;
                            lst_imp_grafica_baja_fem.Add(obj);
                        }
                    }
                    if (item.Fuero == "Federal" && item.Tipo == "Baja")
                    {
                        if (item.DiscFem > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Discrecional";
                            obj.Comun += item.DiscFem;
                            lst_imp_grafica_baja_fem.Add(obj);
                        }
                        if (item.IndicFem > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Indiciado";
                            obj.Comun += item.IndicFem;
                            lst_imp_grafica_baja_fem.Add(obj);
                        }
                        if (item.ProcFem > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Procesado";
                            obj.Comun += item.ProcFem;
                            lst_imp_grafica_baja_fem.Add(obj);
                        }
                        if (item.SentFem > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Sentenciado";
                            obj.Comun += item.SentFem;
                            lst_imp_grafica_baja_fem.Add(obj);
                        }
                    }
                    if (item.Fuero == "Sin Fuero" && item.Tipo == "Baja")
                    {
                        if (item.DiscFem > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Discrecional";
                            obj.Comun += item.DiscFem;
                            lst_imp_grafica_baja_fem.Add(obj);
                        }
                        if (item.IndicFem > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Indiciado";
                            obj.Comun += item.IndicFem;
                            lst_imp_grafica_baja_fem.Add(obj);
                        }
                        if (item.ProcFem > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Procesado";
                            obj.Comun += item.ProcFem;
                            lst_imp_grafica_baja_fem.Add(obj);
                        }
                        if (item.SentFem > 0)
                        {
                            var obj = new cGraficaAltaBaja();
                            obj.Fuero = item.Fuero;
                            obj.Causa = "Sentenciado";
                            obj.Comun += item.SentFem;
                            lst_imp_grafica_baja_fem.Add(obj);
                        }
                    }
                }
                var results_altas = lst_imp_altas.GroupBy(n => n.Fuero).
                     Select(group =>
                         new
                         {
                             Fuero = group.Key,
                             Sexo = group.ToList(),
                         });

                var lst_imp_altas2 = new List<cPoblacionAltasBajas>();
                foreach (var row in results_altas)
                {
                    var obj = new cPoblacionAltasBajas();
                    obj.Fuero = row.Fuero;
                    foreach (var item in row.Sexo)
                    {
                        obj.DiscFem += item.DiscFem;
                        obj.DiscMasc += item.DiscMasc;
                        obj.IndicFem += item.IndicFem;
                        obj.IndicMasc += item.IndicMasc;
                        obj.ProcFem += item.ProcFem;
                        obj.ProcMasc += item.ProcMasc;
                        obj.SentFem += item.SentFem;
                        obj.SentMasc += item.SentMasc;
                    }
                    lst_imp_altas2.Add(obj);
                }

                var results_bajas = lst_imp_bajas.GroupBy(n => n.Fuero).
                    Select(group =>
                        new
                        {
                            Fuero = group.Key,
                            Sexo = group.ToList(),
                        });

                var lst_imp_bajas2 = new List<cPoblacionAltasBajas>();
                foreach (var row in results_bajas)
                {
                    var obj = new cPoblacionAltasBajas();
                    obj.Fuero = row.Fuero;
                    foreach (var item in row.Sexo)
                    {
                        obj.DiscFem += item.DiscFem;
                        obj.DiscMasc += item.DiscMasc;
                        obj.ImpMasc += item.ImpMasc;
                        obj.IndicFem += item.IndicFem;
                        obj.IndicMasc += item.IndicMasc;
                        obj.ProcFem += item.ProcFem;
                        obj.ProcMasc += item.ProcMasc;
                        obj.SentFem += item.SentFem;
                        obj.SentMasc += item.SentMasc;
                    }
                    lst_imp_bajas2.Add(obj);
                }
                Reporte.LocalReport.ReportPath = "../../Reportes/rPoblacionAltasBajas.rdlc";
                Reporte.LocalReport.DataSources.Clear();

                ReportDataSource rds1 = new ReportDataSource();
                rds1.Name = "DataSet1";
                rds1.Value = datosReporte;
                Reporte.LocalReport.DataSources.Add(rds1);

                ReportDataSource rds2 = new ReportDataSource();
                rds2.Name = "DataSet2";
                rds2.Value = lst_imp_altas2.OrderBy(o => o.Fuero);
                Reporte.LocalReport.DataSources.Add(rds2);

                ReportDataSource rds3 = new ReportDataSource();
                rds3.Name = "DataSet3";
                rds3.Value = lst_imp_bajas2.OrderBy(o => o.Fuero);
                Reporte.LocalReport.DataSources.Add(rds3);

                ReportDataSource rds4 = new ReportDataSource();
                rds4.Name = "DataSet4";
                rds4.Value = lst_imp_grafica_alta_masc;
                Reporte.LocalReport.DataSources.Add(rds4);

                ReportDataSource rds5 = new ReportDataSource();
                rds5.Name = "DataSet5";
                rds5.Value = lst_imp_grafica_alta_fem;
                Reporte.LocalReport.DataSources.Add(rds5);

                ReportDataSource rds6 = new ReportDataSource();
                rds6.Name = "DataSet6";
                rds6.Value = lst_imp_grafica_baja_masc;
                Reporte.LocalReport.DataSources.Add(rds6);

                ReportDataSource rds7 = new ReportDataSource();
                rds7.Name = "DataSet7";
                rds7.Value = lst_imp_grafica_baja_fem;
                Reporte.LocalReport.DataSources.Add(rds7);

                ReportDataSource rds8 = new ReportDataSource();
                rds8.Name = "DataSet8";
                rds8.Value = lst_total_sexo;
                Reporte.LocalReport.DataSources.Add(rds8);

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

        private void OnLoad(ReportePoblacionAltasBajasView window)
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
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_ESTADISTICA_ALTAS_BAJAS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
