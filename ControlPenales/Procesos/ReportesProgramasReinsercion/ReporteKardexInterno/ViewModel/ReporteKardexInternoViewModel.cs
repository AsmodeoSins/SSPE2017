using ControlPenales.BiometricoServiceReference;
using Microsoft.Reporting.WinForms;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ControlPenales
{
    partial class ReporteKardexInternoViewModel
    {
        private void ReporteKardexInternoLoad(ReporteKardexInternoView Window)
        {
            try
            {
                ConfiguraPermisos();
                Reporte = Window.Report;
                Reporte.RenderingComplete += (s, e) =>
                {

                    ReportViewerVisible = Visibility.Visible;
                };
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar modulo de control participantes", ex);
            }
        }

        private async void clickSwitch(Object obj)
        {
            try
            {
                switch (obj.ToString())
                {
                    case "KARDEX":
                        if (!pConsultar)
                        {
                            ReportViewerVisible = Visibility.Collapsed;
                            new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                            break;
                        }
                        if (SelectIngreso == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un ingreso");
                            break; 
                        }

                        ReportViewerVisible = Visibility.Collapsed;
                        //await StaticSourcesViewModel.CargarDatosMetodoAsync(() => { GenerarReporte(obj.ToString()); });
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() => { ImprimirKardex(); });
                        break;
                    case "ACTIVIDADES":
                        if (!pConsultar)
                        {
                            ReportViewerVisible = Visibility.Collapsed;
                            new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                            break;
                        }
                        if (SelectIngreso == null)
                            return;

                        ReportViewerVisible = Visibility.Collapsed;
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() => { GenerarReporte(obj.ToString()); });
                        break;
                    case "HORARIO":
                        if (!pConsultar)
                        {
                            ReportViewerVisible = Visibility.Collapsed;
                            new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                            break;
                        }
                        if (SelectIngreso == null)
                            return;

                        ReportViewerVisible = Visibility.Collapsed;
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() => { GenerarReporte(obj.ToString()); });
                        break;
                    case "EMPALMES":
                        if (!pConsultar)
                        {
                            ReportViewerVisible = Visibility.Collapsed;
                            new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                            break;
                        }
                        if (SelectIngreso == null)
                            return;

                        ReportViewerVisible = Visibility.Collapsed;
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() => { GenerarReporte(obj.ToString()); });
                        break;
                    case "SANCIONES":
                        if (!pConsultar)
                        {
                            ReportViewerVisible = Visibility.Collapsed;
                            new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                            break;
                        }
                        if (SelectIngreso == null)
                            return;

                        ReportViewerVisible = Visibility.Collapsed;
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() => { GenerarReporte(obj.ToString()); });
                        break;
                    case "limpiar_Busqueda":
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = null;
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = null;
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new ReporteKardexInternoView();
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ReporteKardexInternoViewModel();
                        break;
                    case "nueva_busqueda":
                        AnioBuscar = FolioBuscar = null;
                        NombreBuscar = ApellidoPaternoBuscar = ApellidoMaternoBuscar = null;
                        ImagenIngreso = ImagenImputado = new Imagenes().getImagenPerson();
                        ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                        break;
                    case "buscar_imputado":
                        if (!pConsultar)
                        {
                            ReportViewerVisible = Visibility.Collapsed;
                            new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                            break;
                        }
                        if (ApellidoPaternoBuscar == null)
                            ApellidoPaternoBuscar = string.Empty;
                        if (ApellidoMaternoBuscar == null)
                            ApellidoMaternoBuscar = string.Empty;
                        if (NombreBuscar == null)
                            NombreBuscar = string.Empty;
                        BuscarImputado();
                        break;
                    case "buscar_salir":
                        var ingA = SelectIngresoAuxiliar;
                        SelectIngreso = ingA;
                        var expA = SelectExpedienteAuxiliar;
                        SelectExpediente = expA;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        ReportViewerVisible = Visibility.Visible;
                        break;
                    case "buscar_seleccionar":
                        try
                        {
                            if (SelectExpediente == null)
                            {
                                ReportViewerVisible = Visibility.Collapsed;
                                (new Dialogos()).ConfirmacionDialogo("Error", "Debes seleccionar un imputado.");
                                break;
                            }
                            if (SelectIngreso == null)
                            {
                                ReportViewerVisible = Visibility.Collapsed;
                                (new Dialogos()).ConfirmacionDialogo("Error", "Debes seleccionar un ingreso.");
                                break;
                            }
                            var EstatusInactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                            foreach (var item in EstatusInactivos)
                            {
                                if (SelectIngreso.ID_ESTATUS_ADMINISTRATIVO == item)
                                {
                                    ReportViewerVisible = Visibility.Collapsed;
                                    new Dialogos().ConfirmacionDialogo("Notificación!", "El ingreso seleccionado no esta activo.");
                                    return;
                                }
                            }
                            if (SelectIngreso.ID_UB_CENTRO != GlobalVar.gCentro)
                            {
                                ReportViewerVisible = Visibility.Collapsed;
                                new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                                return;
                            }

                            //SE ELIMINA CANDADO DE TRASLADO DEBIDO A QUE LOS TECNICOS NO DEBEN DE ESTAR BLOQUEADOS DE PODER CALIFICAR A UN IMPUTADO DEBIDO A UN TRASLADO PROXIMO.
                            //var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                            //if (SelectIngreso.TRASLADO_DETALLE.Any(a => (a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado).TimeOfDay <= Fechas.GetFechaDateServer.TimeOfDay))
                            //{
                            //    new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + SelectIngreso.ID_ANIO.ToString() + "/" +
                            //        SelectIngreso.ID_IMPUTADO.ToString() + "] tiene un traslado proximo y no puede recibir visitas.");
                            //    return;
                            //}
                            var expedient = SelectExpediente;
                            var ingres = SelectIngreso;
                            SelectIngresoAuxiliar = null;
                            SelectExpedienteAuxiliar = null;
                            SelectExpediente = expedient;
                            SelectIngreso = ingres;
                            CargarInformacionParticipante();
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                            ReportViewerVisible = Visibility.Visible;
                        }
                        catch (Exception ex)
                        {
                            StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos del imputado.", ex);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el flujo del modulo.", ex);
            }
        }

        private void GenerarReporte(string reporte)
        {
            try
            {
                var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();
                listaemp = new List<EmpalmeParticipante>();
                #region Reporte
                #region [encabezado]
                var datosReporte = new List<cReporteDatos>();
                datosReporte.Add(new cReporteDatos()
                {
                    Encabezado1 = Parametro.ENCABEZADO1.Trim(),
                    Encabezado2 = Parametro.ENCABEZADO2.Trim(),
                    Encabezado3 = Parametro.ENCABEZADO3.Trim(),
                    Titulo = "KARDEX INTERNO ",
                    Logo1 = Parametro.REPORTE_LOGO1,
                    Logo2 = Parametro.REPORTE_LOGO2,
                    Centro = centro.DESCR.Trim().ToUpper(),
                });

                Reporte.LocalReport.ReportPath = "Reportes/rKardexInterno.rdlc";
                Reporte.LocalReport.DataSources.Clear();

                var rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds2.Name = "DataSet2";
                rds2.Value = datosReporte;
                Reporte.LocalReport.DataSources.Add(rds2);
                #endregion
                #region [primera parte]
                var rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds1.Name = "DataSet1";
                rds1.Value = new List<ReporteKardexInterno>() { new ReporteKardexInterno(){ 
                 Interno=(SelectIngreso.IMPUTADO.PATERNO != null ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty) + " " + (SelectIngreso.IMPUTADO.MATERNO != null ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty) + " " + (SelectIngreso.IMPUTADO.NOMBRE != null ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty),
                  Expediente=SelectIngreso.ID_ANIO + "\\" + SelectIngreso.ID_IMPUTADO,
                  Ingreso=SelectIngreso.ID_INGRESO.ToString(),
                  Ubicacion=SelectIngreso.CAMA != null ? SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() + "-" + SelectIngreso.CAMA.CELDA.SECTOR.DESCR.Trim() + SelectIngreso.CAMA.CELDA.ID_CELDA.Trim() + "-" + (string.IsNullOrEmpty(SelectIngreso.CAMA.DESCR) ? SelectIngreso.CAMA.ID_CAMA.ToString().Trim() : SelectIngreso.CAMA.ID_CAMA + " " + SelectIngreso.CAMA.DESCR.Trim()) : string.Empty,
                  FechaIngreso= SelectIngreso.FEC_INGRESO_CERESO.Value.ToShortDateString(),
                  Planimetria=SelectIngreso.CAMA != null ? SelectIngreso.CAMA.SECTOR_OBSERVACION_CELDA != null ? SelectIngreso.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION != null ? SelectIngreso.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION.SECTOR_CLASIFICACION != null ? SelectIngreso.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION.SECTOR_CLASIFICACION.POBLACION : string.Empty : string.Empty : string.Empty : string.Empty,
                  Avance=AvanceTratamiento,
                  HorasTratamiento=HorasTratamiento,
                  Estatus =SelectIngreso.ESTATUS_ADMINISTRATIVO.DESCR,
                  HistorialSanciones = (reporte.Equals("KARDEX") || reporte.Equals("SANCIONES")) ?SelectIngreso.GRUPO_PARTICIPANTE.Where(w => w.GRUPO_PARTICIPANTE_CANCELADO.Any()).Any() ? "Tiene Historial": string.Empty: string.Empty
                }};
                Reporte.LocalReport.DataSources.Add(rds1);
                #endregion
                var rds3 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds3.Name = "DataSet3";
                rds3.Value = new List<ReporteKardexInternoActividades>();
                if (reporte.Equals("KARDEX") || reporte.Equals("ACTIVIDADES"))
                {
                    #region [segunda parte]
                    var preNotaTecnica = new List<ReporteKardexInternoActividades>();
                    string NotaTecnica,Acredita;
                    foreach (var item in SelectIngreso.GRUPO_PARTICIPANTE.OrderBy(o => o.GRUPO == null).ThenBy(t => t.ACTIVIDAD.TIPO_PROGRAMA.ORDEN).ThenBy(t => t.ACTIVIDAD.ORDEN).ThenBy(t => t.GRUPO != null ? t.GRUPO.DESCR : string.Empty))
                    {
                        NotaTecnica = Acredita  = string.Empty;
                        var nt = item.NOTA_TECNICA.Where(w => w.ID_CENTRO == item.ID_CENTRO && w.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.ID_GRUPO == item.ID_GRUPO).FirstOrDefault();
                        if (nt != null)
                        {
                            NotaTecnica = nt.NOTA;
                            Acredita = nt.NOTA_TECNICA_ESTATUS.DESCR;
                        }
                        else
                        {
                            NotaTecnica = Acredita = "NO CAPTURADA";
                        }
                        
                        preNotaTecnica.Add(new ReporteKardexInternoActividades()
                            {
                                ID_Grupo = item.GRUPO != null ? item.GRUPO.ID_GRUPO.ToString() : string.Empty,
                                Eje = item.EJE1.DESCR,
                                Programa = item.ACTIVIDAD.TIPO_PROGRAMA.NOMBRE,
                                Actividad = item.ACTIVIDAD.DESCR,
                                Grupo = item.GRUPO != null ? item.GRUPO.DESCR : item.GRUPO_PARTICIPANTE_ESTATUS.DESCR,
                                //EntityGRUPO = item.GRUPO,
                                Inicio = item.GRUPO != null ? item.GRUPO.GRUPO_HORARIO.Any() ? item.GRUPO.GRUPO_HORARIO.OrderBy(o => o.HORA_INICIO).FirstOrDefault().HORA_INICIO.Value.ToShortDateString() : string.Empty : string.Empty,
                                Fin = item.GRUPO != null ? item.GRUPO.GRUPO_HORARIO.Any() ? item.GRUPO.GRUPO_HORARIO.OrderByDescending(o => o.HORA_TERMINO).FirstOrDefault().HORA_TERMINO.Value.ToShortDateString() : string.Empty : string.Empty,
                                Asistencia = ObtenerPorcentajeAsistencia(item, SelectIngreso.GRUPO_PARTICIPANTE),
                                Nota_Tecnica = NotaTecnica,
                                Acreditado = Acredita,
                                //Nota_Tecnica = item.NOTA_TECNICA.Count != 0 ? item.NOTA_TECNICA.Where(w => w.ID_CENTRO == item.ID_CENTRO && w.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.ID_GRUPO == item.ID_GRUPO).FirstOrDefault().NOTA : "NO CAPTURADA",
                                //Acreditado = item.NOTA_TECNICA.Count != 0 ? item.NOTA_TECNICA.Where(w => w.ID_CENTRO == item.ID_CENTRO && w.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.ID_GRUPO == item.ID_GRUPO).FirstOrDefault().NOTA_TECNICA_ESTATUS.DESCR : "NO CAPTURADA"
                            });
                    }
                    rds3.Value = preNotaTecnica;

                    #endregion
                }
                Reporte.LocalReport.DataSources.Add(rds3);
                var rds4 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds4.Name = "DataSet4";
                rds4.Value = new List<ReporteKardexInternoActividades>();
                if (reporte.Equals("KARDEX") || reporte.Equals("HORARIO"))
                {
                    #region [tercera parte]
                    //var rds4 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    //rds4.Name = "DataSet4";
                    var preNotaTecnica2 = new List<ReporteKardexInternoActividades>();
                    string NotaTecnica, Acredita;
                    foreach (var item in SelectIngreso.GRUPO_PARTICIPANTE.OrderBy(o => o.GRUPO == null).ThenBy(t => t.ACTIVIDAD.TIPO_PROGRAMA.ORDEN).ThenBy(t => t.ACTIVIDAD.ORDEN).ThenBy(t => t.GRUPO != null ? t.GRUPO.DESCR : string.Empty))
                    {
                        NotaTecnica = Acredita = string.Empty;
                        var nt = item.NOTA_TECNICA.Where(w => w.ID_CENTRO == item.ID_CENTRO && w.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.ID_GRUPO == item.ID_GRUPO).FirstOrDefault();
                        if (nt != null)
                        {
                            NotaTecnica = nt.NOTA;
                            Acredita = nt.NOTA_TECNICA_ESTATUS.DESCR;
                        }
                        else
                        {
                            NotaTecnica = Acredita = "NO CAPTURADA";
                        }
                        preNotaTecnica2.Add(new ReporteKardexInternoActividades()
                           {
                               ID_Grupo = item.GRUPO != null ? item.GRUPO.ID_GRUPO.ToString() : string.Empty,
                               Eje = item.EJE1.DESCR,
                               Programa = item.ACTIVIDAD.TIPO_PROGRAMA.NOMBRE,
                               Actividad = item.ACTIVIDAD.DESCR,
                               Grupo = item.GRUPO != null ? item.GRUPO.DESCR : item.GRUPO_PARTICIPANTE_ESTATUS.DESCR,
                               //EntityGRUPO = item.GRUPO,
                               Inicio = item.GRUPO != null ? item.GRUPO.GRUPO_HORARIO.Any() ? item.GRUPO.GRUPO_HORARIO.OrderBy(o => o.HORA_INICIO).FirstOrDefault().HORA_INICIO.Value.ToShortDateString() : string.Empty : string.Empty,
                               Fin = item.GRUPO != null ? item.GRUPO.GRUPO_HORARIO.Any() ? item.GRUPO.GRUPO_HORARIO.OrderByDescending(o => o.HORA_TERMINO).FirstOrDefault().HORA_TERMINO.Value.ToShortDateString() : string.Empty : string.Empty,
                               Asistencia = ObtenerPorcentajeAsistencia(item, SelectIngreso.GRUPO_PARTICIPANTE),
                                Nota_Tecnica = NotaTecnica,
                                Acreditado = Acredita,
                               //Nota_Tecnica = item.NOTA_TECNICA.Count != 0 ? item.NOTA_TECNICA.Where(w => w.ID_CENTRO == item.ID_CENTRO && w.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.ID_GRUPO == item.ID_GRUPO).FirstOrDefault().NOTA : "NO CAPTURADA",
                               //Acreditado = item.NOTA_TECNICA.Count != 0 ? item.NOTA_TECNICA.Where(w => w.ID_CENTRO == item.ID_CENTRO && w.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.ID_GRUPO == item.ID_GRUPO).FirstOrDefault().NOTA_TECNICA_ESTATUS.DESCR : "NO CAPTURADA"
                           });
                    }
                    rds4.Value = preNotaTecnica2;

                    //Reporte.LocalReport.DataSources.Add(rds4);
                    #endregion
                }
                Reporte.LocalReport.DataSources.Add(rds4);

                var rds5 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds5.Name = "DataSet5";
                rds5.Value = new List<ReporteKardexHorasEmpalmadas>();
                if (reporte.Equals("KARDEX") || reporte.Equals("EMPALMES"))
                {
                    #region [cuarta parte]


                    var IdGH = new Nullable<DateTime>();
                    var cont = -1;
                    var horariolist = new cGrupoAsistencia().GetData().Where(w => w.GRUPO_PARTICIPANTE.ING_ID_CENTRO == SelectIngreso.ID_UB_CENTRO && w.GRUPO_PARTICIPANTE.ING_ID_ANIO == SelectIngreso.ID_ANIO && w.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && w.GRUPO_PARTICIPANTE.ING_ID_INGRESO == SelectIngreso.ID_INGRESO).OrderByDescending(o => o.GRUPO_HORARIO.HORA_INICIO).ToList();

                    if (horariolist.Count > 1)
                        foreach (var item in horariolist)
                        {
                            if (item == null)
                                continue;
                            if (IdGH == item.GRUPO_HORARIO.HORA_INICIO.Value.Date)
                            {
                                listaemp[cont].ListHorario.Add(new ListaEmpalmes()
                                {
                                    ACTIVIDAD = item.GRUPO_HORARIO.GRUPO.ACTIVIDAD.DESCR,
                                    EJE = item.GRUPO_PARTICIPANTE.EJE1.DESCR,
                                    ELEGIDA = item.EMP_APROBADO == 1,
                                    GRUPO = item.GRUPO_HORARIO.GRUPO.DESCR,
                                    HORARIO = item.GRUPO_HORARIO.HORA_INICIO.Value.ToShortTimeString() + " - " + item.GRUPO_HORARIO.HORA_TERMINO.Value.ToShortTimeString(),
                                    PROGRAMA = item.GRUPO_HORARIO.GRUPO.ACTIVIDAD.TIPO_PROGRAMA.NOMBRE
                                });
                                listaemp[cont].ListHorario = listaemp[cont].ListHorario.OrderByDescending(o => o.ELEGIDA).ThenBy(t => t.HORARIO).ToList();
                                continue;
                            }
                            IdGH = item.GRUPO_HORARIO.HORA_INICIO.Value.Date;

                            listaemp.Add(new EmpalmeParticipante()
                            {
                                HEADEREXPANDER = item.GRUPO_HORARIO.HORA_INICIO.Value.Date.ToShortDateString(),
                                ListHorario = new List<ListaEmpalmes>() { new ListaEmpalmes()
                                    {
                                        ACTIVIDAD = item.GRUPO_HORARIO.GRUPO.ACTIVIDAD.DESCR,
                                        EJE = item.GRUPO_PARTICIPANTE.EJE1.DESCR,
                                        ELEGIDA = item.EMP_APROBADO == 1,
                                        GRUPO = item.GRUPO_HORARIO.GRUPO.DESCR,
                                        HORARIO = item.GRUPO_HORARIO.HORA_INICIO.Value.ToShortTimeString() + " - " + item.GRUPO_HORARIO.HORA_TERMINO.Value.ToShortTimeString(),
                                        PROGRAMA = item.GRUPO_HORARIO.GRUPO.ACTIVIDAD.TIPO_PROGRAMA.NOMBRE
                                    }}
                            });
                            cont++;
                        }

                    rds5.Value = listaemp.Where(w => w.ListHorario.Count > 1).Select(s => new ReporteKardexHorasEmpalmadas { Dia_Header = s.HEADEREXPANDER }).OrderBy(o => Convert.ToDateTime(o.Dia_Header)).ToList();
                    #endregion
                }
                Reporte.LocalReport.DataSources.Add(rds5);

                Reporte.LocalReport.SubreportProcessing += (s, e) =>
                {
                    #region [rKardexHorarioInterno]
                    if (e.ReportPath.Equals("rKardexHorarioInterno", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var intid = Convert.ToInt16(e.Parameters[0].Values[0]);
                        var preHorarioParticipante = new List<ReporteKardexInternoHorario>();
                        //var grupo = new cGrupo().GetData().Where(w => w.ID_GRUPO == intid).FirstOrDefault();
                        foreach (var item in SelectIngreso.GRUPO_PARTICIPANTE.Where(w => w.GRUPO != null && w.GRUPO.ID_GRUPO == intid).OrderBy(t => t.ACTIVIDAD.TIPO_PROGRAMA.ORDEN).ThenBy(t => t.ACTIVIDAD.ORDEN).ThenBy(t => t.GRUPO != null ? t.GRUPO.DESCR : string.Empty))
                        {
                            if (item.GRUPO != null)
                                foreach (var subitem in item.GRUPO.GRUPO_HORARIO.OrderBy(o => o.HORA_INICIO))
                                    preHorarioParticipante.Add(new ReporteKardexInternoHorario()
                                    {
                                        ID_Grupo = item.GRUPO != null ? item.GRUPO.ID_GRUPO.ToString() : string.Empty,
                                        EJE = item.EJE1.DESCR,
                                        PROGRAMA = item.ACTIVIDAD.TIPO_PROGRAMA.NOMBRE,
                                        ACTIVIDAD = item.ACTIVIDAD.DESCR,
                                        FECHA = subitem.HORA_INICIO.Value.ToShortDateString(),
                                        HORARIO = subitem.HORA_INICIO.Value.ToShortTimeString() + " - " + subitem.HORA_TERMINO.Value.ToShortTimeString(),
                                        GRUPO = item.GRUPO != null ? item.GRUPO.DESCR : item.GRUPO_PARTICIPANTE_ESTATUS.DESCR,
                                        ASISTENCIA = subitem.GRUPO_ASISTENCIA.Where(w => w.ID_GRUPO_HORARIO == subitem.ID_GRUPO_HORARIO && w.ID_CENTRO == subitem.ID_CENTRO && w.ID_TIPO_PROGRAMA == subitem.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == subitem.ID_ACTIVIDAD && w.ID_GRUPO == subitem.ID_GRUPO && w.ID_CONSEC == subitem.GRUPO.GRUPO_PARTICIPANTE.Where(wh => wh == item).FirstOrDefault().ID_CONSEC).FirstOrDefault().ASISTENCIA == 1,
                                        INICIO = item.GRUPO.GRUPO_HORARIO.Any() ? item.GRUPO.GRUPO_HORARIO.OrderBy(o => o.HORA_INICIO).FirstOrDefault().HORA_INICIO.Value.ToShortDateString() : string.Empty,
                                        FIN = item.GRUPO.GRUPO_HORARIO.Any() ? item.GRUPO.GRUPO_HORARIO.OrderByDescending(o => o.HORA_TERMINO).FirstOrDefault().HORA_TERMINO.Value.ToShortDateString() : string.Empty,

                                        ESTATUS = subitem.ESTATUS != 1 ? subitem.GRUPO_HORARIO_ESTATUS.DESCR : subitem.GRUPO_ASISTENCIA.Where(w => w.ID_GRUPO_HORARIO == subitem.ID_GRUPO_HORARIO && w.ID_CENTRO == subitem.ID_CENTRO && w.ID_TIPO_PROGRAMA == subitem.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == subitem.ID_ACTIVIDAD && w.ID_GRUPO == subitem.ID_GRUPO && w.ID_CONSEC == subitem.GRUPO.GRUPO_PARTICIPANTE.Where(wh => wh == item).FirstOrDefault().ID_CONSEC).FirstOrDefault().ESTATUS != 1 ? subitem.GRUPO_ASISTENCIA.Where(w => w.ID_GRUPO_HORARIO == subitem.ID_GRUPO_HORARIO && w.ID_CENTRO == subitem.ID_CENTRO && w.ID_TIPO_PROGRAMA == subitem.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == subitem.ID_ACTIVIDAD && w.ID_GRUPO == subitem.ID_GRUPO && w.ID_CONSEC == subitem.GRUPO.GRUPO_PARTICIPANTE.Where(wh => wh == item).FirstOrDefault().ID_CONSEC).FirstOrDefault().GRUPO_ASISTENCIA_ESTATUS.DESCR : string.Empty,
                                        FechaHorario = subitem.HORA_INICIO,
                                        ShowCheck = subitem.ESTATUS != 1 ? Visibility.Collapsed : subitem.GRUPO_ASISTENCIA.Where(w => w.ID_GRUPO_HORARIO == subitem.ID_GRUPO_HORARIO && w.ID_CENTRO == subitem.ID_CENTRO && w.ID_TIPO_PROGRAMA == subitem.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == subitem.ID_ACTIVIDAD && w.ID_GRUPO == subitem.ID_GRUPO && w.ID_CONSEC == subitem.GRUPO.GRUPO_PARTICIPANTE.Where(wh => wh == item).FirstOrDefault().ID_CONSEC).FirstOrDefault().ESTATUS != 1 ? Visibility.Collapsed : Visibility.Visible,
                                        ShowLabel = subitem.ESTATUS != 1 ? Visibility.Visible : subitem.GRUPO_ASISTENCIA.Where(w => w.ID_GRUPO_HORARIO == subitem.ID_GRUPO_HORARIO && w.ID_CENTRO == subitem.ID_CENTRO && w.ID_TIPO_PROGRAMA == subitem.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == subitem.ID_ACTIVIDAD && w.ID_GRUPO == subitem.ID_GRUPO && w.ID_CONSEC == subitem.GRUPO.GRUPO_PARTICIPANTE.Where(wh => wh == item).FirstOrDefault().ID_CONSEC).FirstOrDefault().ESTATUS != 1 ? Visibility.Visible : Visibility.Collapsed
                                    });
                        }

                        //var ds = new ReportDataSource("DataSet1", lSector);
                        e.DataSources.Add(new ReportDataSource("DataSet1", preHorarioParticipante));

                        var hp = preHorarioParticipante.FirstOrDefault();
                        if (hp != null)
                        {
                            e.DataSources.Add(new ReportDataSource("DataSet2", new List<ReporteKardexInternoHorario>() {
                       new ReporteKardexInternoHorario()
                                    {
                                        ACTIVIDAD = preHorarioParticipante.FirstOrDefault().ACTIVIDAD,
                                        GRUPO = preHorarioParticipante.FirstOrDefault().GRUPO,
                                        INICIO = preHorarioParticipante.FirstOrDefault().INICIO,
                                        FIN = preHorarioParticipante.FirstOrDefault().FIN,

                                        nASISTENCIA = preHorarioParticipante.Any() ? preHorarioParticipante.Where(w => w.ASISTENCIA.Value).Count().ToString() : string.Empty,
                                        FALTAS = preHorarioParticipante.Any() ? preHorarioParticipante.Where(w => !w.ASISTENCIA.Value && w.FechaHorario < Fechas.GetFechaDateServer).Count().ToString() : string.Empty,
                                        JUSTIFICADAS = preHorarioParticipante.Any() ? preHorarioParticipante.Where(w => w.ShowLabel == Visibility.Visible).Count().ToString() : string.Empty
                                    }
                        }));
                        }

                        
                    }
                    #endregion
                    else
                        #region [srEmpalme]
                        if (e.ReportPath.Equals("srEmpalme", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var dateid = e.Parameters[0].Values[0];

                            e.DataSources.Add(new ReportDataSource("DataSet1", listaemp.Where(w => w.HEADEREXPANDER.Equals(dateid) && w.ListHorario.Count > 1).SelectMany(se => se.ListHorario).Select(se => new ReporteKardexHorasEmpalmadas
                            {
                                EJE = se.EJE,
                                PROGRAMA = se.PROGRAMA,
                                ACTIVIDAD = se.ACTIVIDAD,
                                GRUPO = se.GRUPO,
                                Horario = se.HORARIO,
                                Elegida = se.ELEGIDA
                            }).OrderByDescending(o => o.Elegida).ToList()));

                        }
                        #endregion
                        else
                            #region [srSanciones]
                            if (e.ReportPath.Equals("srSanciones", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var listsanciones = new cGrupoParticipanteCancelado().GetData().Where(w => w.GRUPO_PARTICIPANTE.ING_ID_ANIO == SelectIngreso.ID_ANIO && w.GRUPO_PARTICIPANTE.ING_ID_CENTRO == SelectIngreso.ID_CENTRO && w.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && w.GRUPO_PARTICIPANTE.ING_ID_INGRESO == SelectIngreso.ID_INGRESO).ToList();

                                e.DataSources.Add(new ReportDataSource("DataSet1", listsanciones.Select(se => new ReporteKardexSanciones
                                {
                                    EJE = se.GRUPO_PARTICIPANTE.EJE1.DESCR,
                                    PROGRAMA = se.GRUPO_PARTICIPANTE.ACTIVIDAD.TIPO_PROGRAMA.NOMBRE,
                                    ACTIVIDAD = se.GRUPO_PARTICIPANTE.ACTIVIDAD.DESCR,
                                    GRUPO = getNombreGrupo(se.ID_GRUPO),
                                    RESPONSABLE = getNombreResponsable(se.ID_GRUPO),
                                    SOLICITUD_FECHA = se.SOLICITUD_FEC.Value.ToShortDateString(),
                                    RESPUESTA_FECHA = se.RESPUESTA_FEC.HasValue ? se.RESPUESTA_FEC.Value.ToShortDateString() : string.Empty,
                                    MOTIVO = se.MOTIVO,
                                    RESPUESTA = se.RESPUESTA,
                                    ESTATUS = se.GRUPO_PARTICIPANTE_ESTATUS.DESCR
                                }).ToList()));
                            }
                            #endregion
                };

                #region Parametros
                var listgroup = new cGrupo().GetData().Select(s => s.ID_GRUPO).Cast<String>().ToArray();
                Reporte.LocalReport.SetParameters(new Microsoft.Reporting.WinForms.ReportParameter("ID_GRUPOParam", listgroup));
                #endregion

                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    Reporte.RefreshReport();
                }));
                #endregion
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar reporte", ex);
            }
        }

        private string getNombreResponsable(short ID_GRUPO)
        {
            var grupodesc = new cGrupo().GetData().Where(w => w.ID_GRUPO == ID_GRUPO).FirstOrDefault();
            return grupodesc != null ? (string.IsNullOrEmpty(grupodesc.PERSONA.NOMBRE) ? string.Empty : grupodesc.PERSONA.NOMBRE.Trim()) + " " + (string.IsNullOrEmpty(grupodesc.PERSONA.PATERNO) ? string.Empty : grupodesc.PERSONA.PATERNO.Trim()) + " " + (string.IsNullOrEmpty(grupodesc.PERSONA.MATERNO) ? string.Empty : grupodesc.PERSONA.MATERNO.Trim()) : string.Empty;
        }

        private string getNombreGrupo(short ID_GRUPO)
        {
            var grupodesc = new cGrupo().GetData().Where(w => w.ID_GRUPO == ID_GRUPO).FirstOrDefault();
            return grupodesc != null ? grupodesc.DESCR : string.Empty;
        }

        private string ObtenerPorcentajeAsistencia(GRUPO_PARTICIPANTE item, ICollection<GRUPO_PARTICIPANTE> collection)
        {
            try
            {
                var TotalHoras = 0.0;
                var AsistenciaHoras = 0.0;
                TotalHoras = item.ID_GRUPO.HasValue ? item.GRUPO.GRUPO_HORARIO.Where(w => w.ID_GRUPO == item.ID_GRUPO && w.ESTATUS == 1).Count() : 0;
                AsistenciaHoras = item.GRUPO_ASISTENCIA.Where(w => w.GRUPO_HORARIO.ESTATUS == 1 && (w.ESTATUS == 1 || w.ESTATUS == 3) && collection.Where(wh => wh.GRUPO != null && wh.GRUPO.GRUPO_HORARIO.Where(whe => whe.ESTATUS == 1).Any()).Contains(w.GRUPO_PARTICIPANTE) && w.ASISTENCIA == 1).Count();

                if (double.IsNaN((AsistenciaHoras / TotalHoras)))
                    return string.Empty;

                return string.Format("{0:P2}", (AsistenciaHoras / TotalHoras));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la información del participante.", ex);
                return string.Empty;
            }
        }

        private async void BuscarImputado()
        {
            try
            {
                ImagenIngreso = new Imagenes().getImagenPerson();
                ImagenImputado = new Imagenes().getImagenPerson();
                ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                if (ListExpediente.Count == 1)
                {
                    if (AnioBuscar != null && FolioBuscar != null)
                    {
                        var EstatusInactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                        foreach (var item in EstatusInactivos)
                        {
                            if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_ESTATUS_ADMINISTRATIVO == item)
                            {
                                new Dialogos().ConfirmacionDialogo("Notificación!", "El ingreso seleccionado no esta activo.");
                                clickSwitch("limpiar_Busqueda");
                                return;
                            }
                        }
                        if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_UB_CENTRO != GlobalVar.gCentro)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                            clickSwitch("limpiar_Busqueda");
                            return;
                        }
                        //SE ELIMINA CANDADO DE TRASLADO DEBIDO A QUE LOS TECNICOS NO DEBEN DE ESTAR BLOQUEADOS DE PODER CALIFICAR A UN IMPUTADO DEBIDO A UN TRASLADO PROXIMO.
                        //var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                        //if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().TRASLADO_DETALLE.Any(a => (a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado).TimeOfDay <= Fechas.GetFechaDateServer.TimeOfDay))
                        //{
                        //    new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_ANIO.ToString() + "/" +
                        //        ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_IMPUTADO.ToString() + "] tiene un traslado proximo y no puede recibir visitas.");
                        //    clickSwitch("limpiar_Busqueda");
                        //    return;
                        //}
                        SelectExpediente = ListExpediente[0];
                        SelectIngreso = ListExpediente[0].INGRESO.Where(w => w.ID_ESTATUS_ADMINISTRATIVO != Parametro.ID_ESTATUS_ADMVO_LIBERADO).FirstOrDefault();
                        var expedient = SelectExpediente;
                        var ingres = SelectIngreso;
                        SelectExpediente = expedient;
                        SelectIngreso = ingres;
                        CargarInformacionParticipante();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        ReportViewerVisible = Visibility.Visible;
                        return;
                    }
                }
                if (ListExpediente.Count == 0)
                {
                    if (!string.IsNullOrEmpty(NombreD) || !string.IsNullOrEmpty(PaternoD) || !string.IsNullOrEmpty(MaternoD))
                    {
                        new Dialogos().ConfirmacionDialogo("Notificacion!", "No se encontro ningun imputado con esos datos.");
                        clickSwitch("limpiar_Busqueda");
                    }
                }

                EmptyExpedienteVisible = ListExpediente.Count <= 0;
                ReportViewerVisible = Visibility.Collapsed;
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la busqueda.", ex);
                clickSwitch("limpiar_Busqueda");
            }
        }

        private async Task<List<IMPUTADO>> SegmentarResultadoBusqueda(int _Pag = 1)
        {
            try
            {
                if (string.IsNullOrEmpty(ApellidoPaternoBuscar) && string.IsNullOrEmpty(ApellidoMaternoBuscar) && string.IsNullOrEmpty(NombreBuscar) && !AnioBuscar.HasValue && !FolioBuscar.HasValue)
                    return new List<IMPUTADO>();
                Pagina = _Pag;
                ReportViewerVisible = Visibility.Collapsed;
                var result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<IMPUTADO>>(() =>
                    new cImputado().ObtenerTodos(ApellidoPaternoBuscar, ApellidoMaternoBuscar, NombreBuscar, AnioBuscar, FolioBuscar, _Pag));
                if (PopUpsViewModels.VisibleBusquedaImputado == Visibility.Collapsed)
                    ReportViewerVisible = Visibility.Visible;
                if (result.Any())
                {
                    Pagina++;
                    SeguirCargando = true;
                }
                else
                    SeguirCargando = false;
                return result.ToList();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al segmentar busqueda.", ex);
                return new List<IMPUTADO>();
            }
        }

        void CargarInformacionParticipante()
        {
            try
            {
                TextAnio = SelectExpediente.ID_ANIO.ToString();
                TextFolio = SelectExpediente.ID_IMPUTADO.ToString();
                TextPaternoImputado = SelectExpediente.PATERNO.Trim();
                TextMaternoImputado = SelectExpediente.MATERNO.Trim();
                TextNombreImputado = SelectExpediente.NOMBRE.Trim();
                TextUbicacion = SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() + "-" + SelectIngreso.CAMA.CELDA.SECTOR.DESCR.Trim() + SelectIngreso.CAMA.CELDA.ID_CELDA.Trim() + "-" + (string.IsNullOrEmpty(SelectIngreso.CAMA.DESCR) ? SelectIngreso.CAMA.ID_CAMA.ToString().Trim() : SelectIngreso.CAMA.ID_CAMA + " " + SelectIngreso.CAMA.DESCR.Trim());
                TextPlanimetria = SelectIngreso.CAMA != null ? SelectIngreso.CAMA.SECTOR_OBSERVACION_CELDA != null ? SelectIngreso.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION != null ? SelectIngreso.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION.SECTOR_CLASIFICACION != null ? SelectIngreso.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION.SECTOR_CLASIFICACION.POBLACION : string.Empty : string.Empty : string.Empty : string.Empty;
                Planimetriacolor = SelectIngreso.CAMA != null ? SelectIngreso.CAMA.SECTOR_OBSERVACION_CELDA != null ? SelectIngreso.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION != null ? SelectIngreso.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION.SECTOR_CLASIFICACION != null ? SelectIngreso.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION.SECTOR_CLASIFICACION.COLOR : string.Empty : string.Empty : string.Empty : string.Empty;
                TextSentenciaRes = CalcularSentencia(SelectIngreso);
                TextEstatus = SelectIngreso.GRUPO_PARTICIPANTE.Any() ? "CON TRATAMIENTO" : "SIN TRATAMIENTO";

                ObtenerCursosAprovadosTotalHoras(SelectIngreso.GRUPO_PARTICIPANTE, SelectIngreso.GRUPO_PARTICIPANTE.Count);

                StaticSourcesViewModel.SourceChanged = false;
                LimpiarReporte();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la información del participante.", ex);
            }
        }

        private string CalcularSentencia(INGRESO ingres)
        {
            try
            {
                if (ingres != null)
                {
                    int anios = 0, meses = 0, dias = 0, anios_abono = 0, meses_abono = 0, dias_abono = 0;
                    DateTime? FechaInicioCompurgacion = null, FechaFinCompurgacion = null;
                    if (ingres.CAUSA_PENAL != null)
                    {
                        foreach (var cp in ingres.CAUSA_PENAL)
                        {
                            var segundaInstancia = false;
                            if (cp.SENTENCIA != null)
                            {
                                if (cp.SENTENCIA.Count > 0)
                                {
                                    //BUSCAMOS SI TIENE 2DA INSTANCIA
                                    if (cp.RECURSO.Count > 0)
                                    {
                                        var r = cp.RECURSO.Where(w => w.SENTENCIA_ANIOS > 0 || w.SENTENCIA_MESES > 0 || w.SENTENCIA_DIAS > 0);
                                        if (r != null)
                                        {
                                            var res = r.FirstOrDefault();
                                            if (res != null)
                                            {
                                                //SENTENCIA
                                                anios = anios + (res.SENTENCIA_ANIOS != null ? res.SENTENCIA_ANIOS.Value : 0);
                                                meses = meses + (res.SENTENCIA_MESES != null ? res.SENTENCIA_MESES.Value : 0);
                                                dias = dias + (res.SENTENCIA_DIAS != null ? res.SENTENCIA_DIAS.Value : 0);

                                                segundaInstancia = true;
                                            }
                                        }
                                    }
                                    var s = cp.SENTENCIA.FirstOrDefault();
                                    if (s != null)
                                    {
                                        if (FechaInicioCompurgacion == null)
                                        {
                                            FechaInicioCompurgacion = s.FEC_INICIO_COMPURGACION;
                                        }
                                        else
                                        {
                                            if (FechaInicioCompurgacion > s.FEC_INICIO_COMPURGACION)
                                                FechaInicioCompurgacion = s.FEC_INICIO_COMPURGACION;
                                        }

                                        //SENTENCIA
                                        if (!segundaInstancia)
                                        {
                                            anios = anios + (s.ANIOS != null ? s.ANIOS.Value : 0);
                                            meses = meses + (s.MESES != null ? s.MESES.Value : 0);
                                            dias = dias + (s.DIAS != null ? s.DIAS.Value : 0);
                                        }

                                        //ABONO
                                        anios_abono = anios_abono + (s.ANIOS_ABONADOS != null ? s.ANIOS_ABONADOS.Value : 0);
                                        meses_abono = meses_abono + (s.MESES_ABONADOS != null ? s.MESES_ABONADOS.Value : 0);
                                        dias_abono = dias_abono + (s.DIAS_ABONADOS != null ? s.DIAS_ABONADOS.Value : 0);
                                    }
                                }
                            }
                        }
                    }

                    while (dias > 29)
                    {
                        meses++;
                        dias = dias - 30;
                    }
                    while (meses > 11)
                    {
                        anios++;
                        meses = meses - 12;
                    }

                    if (FechaInicioCompurgacion != null)
                    {
                        FechaFinCompurgacion = FechaInicioCompurgacion;
                        FechaFinCompurgacion = FechaFinCompurgacion.Value.AddYears(anios);
                        FechaFinCompurgacion = FechaFinCompurgacion.Value.AddMonths(meses);
                        FechaFinCompurgacion = FechaFinCompurgacion.Value.AddDays(dias);
                        //
                        FechaFinCompurgacion = FechaFinCompurgacion.Value.AddYears(-anios_abono);
                        FechaFinCompurgacion = FechaFinCompurgacion.Value.AddMonths(-meses_abono);
                        FechaFinCompurgacion = FechaFinCompurgacion.Value.AddDays(-dias_abono);

                        int a = 0, m = 0, d = 0;
                        new Fechas().DiferenciaFechas(Fechas.GetFechaDateServer.Date, FechaInicioCompurgacion.Value.Date, out a, out  m, out d);
                        a = m = d = 0;
                        new Fechas().DiferenciaFechas(FechaFinCompurgacion.Value.Date, Fechas.GetFechaDateServer.Date, out a, out  m, out d);

                        TextSentencia = anios + (anios == 1 ? " Año " : " Años ") + meses + (meses == 1 ? " Mes " : " Meses ") + dias + (dias == 1 ? " Dia" : " Dias");
                        return a + (a == 1 ? " Año " : " Años ") + m + (m == 1 ? " Mes " : " Meses ") + d + (d == 1 ? " Dia" : " Dias");
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al calcular sentencia", ex);
            }
            return string.Empty;
        }

        private void ObtenerCursosAprovadosTotalHoras(IEnumerable<GRUPO_PARTICIPANTE> enumerable, int TotalActividades)
        {
            var acreditados = 0;
            try
            {
                var HorasAsistencia = 0;
                var TotalAsistencia = 0;
                foreach (var item in enumerable)
                {
                    acreditados = acreditados + item.NOTA_TECNICA.Where(w => w.ESTATUS == 1).Count();
                    TotalAsistencia = TotalAsistencia + (item.ID_GRUPO.HasValue ? item.GRUPO.GRUPO_HORARIO.Where(w => w.ID_GRUPO == item.ID_GRUPO && w.ID_GRUPO == item.ID_GRUPO && w.ESTATUS == 1).Count() : 0);
                    HorasAsistencia = HorasAsistencia + item.GRUPO_ASISTENCIA.Where(w => w.GRUPO_HORARIO.ESTATUS == 1 && (w.ESTATUS == 1 || w.ESTATUS == 3) && enumerable.Where(wh => wh.GRUPO != null && wh.GRUPO.GRUPO_HORARIO.Where(whe => whe.ESTATUS == 1).Any()).Contains(w.GRUPO_PARTICIPANTE) && w.ASISTENCIA == 1).Count();
                }
                MaxValueProBar = TotalActividades == 0 ? 1 : TotalActividades;
                CantidadActividadesAprovadas = acreditados;

                HorasTratamiento = HorasAsistencia + "/" + TotalAsistencia;
                AvanceTratamiento = acreditados + "/" + TotalActividades;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener avances del participante.", ex);
            }
        }

        private void BuscarInterno(Object obj)
        {
            try
            {
                if (!pConsultar)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                    return;
                }
                if (obj is TextBox)
                    if (((TextBox)obj).Name == "FolioInterno")
                    {
                        TextNombreImputado = NombreBuscar = string.Empty;
                        TextPaternoImputado = ApellidoPaternoBuscar = string.Empty;
                        TextMaternoImputado = ApellidoMaternoBuscar = string.Empty;
                    }

                var ing = SelectIngreso;
                SelectIngresoAuxiliar = ing;
                var exp = SelectExpediente;
                SelectExpedienteAuxiliar = exp;
                AnioBuscar = !string.IsNullOrEmpty(TextAnio) ? int.Parse(TextAnio) : new Nullable<int>();
                FolioBuscar = !string.IsNullOrEmpty(TextFolio) ? int.Parse(TextFolio) : new Nullable<int>();
                NombreBuscar = !string.IsNullOrEmpty(TextNombreImputado) ? TextNombreImputado : string.Empty;
                ApellidoPaternoBuscar = !string.IsNullOrEmpty(TextPaternoImputado) ? TextPaternoImputado : string.Empty;
                ApellidoMaternoBuscar = !string.IsNullOrEmpty(TextMaternoImputado) ? TextMaternoImputado : string.Empty;
                SelectExpediente = null;
                BuscarImputado();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al traer datos del visitante.", ex);
            }
        }

        private void BuscarInternoPopup(Object obj)
        {
            try
            {
                if (!pConsultar)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                    return;
                }

                var ing = SelectIngreso;
                SelectIngresoAuxiliar = ing;
                var exp = SelectExpediente;
                SelectExpedienteAuxiliar = exp;
                BuscarImputado();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al traer datos del participante.", ex);
            }
        }

        void LimpiarReporte()
        {
            try
            {
                ReportViewerVisible = Visibility.Collapsed;
                Reporte.Reset();
                Reporte.RefreshReport();
                ReportViewerVisible = Visibility.Visible;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar reporte", ex);
            }
        }

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_KARDEX_INTERNO.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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

        #region Kardex
        private void ImprimirKardex()
        {
            try
            {
                var hoy = Fechas.GetFechaDateServer;
                var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();
                var datosReporte = new List<cReporteDatos>();
                var GeneralesKardex = new List<cKardex>();
                var JuridicoKardex = new List<cKardexJuridico>();
                datosReporte.Add(new cReporteDatos()
                {
                    Encabezado1 = Parametro.ENCABEZADO1,
                    Encabezado2 = Parametro.ENCABEZADO2,
                    Encabezado3 = centro.DESCR.ToUpper().Trim(),
                    Logo1 = Parametro.REPORTE_LOGO1,
                    Logo2 = Parametro.REPORTE_LOGO2,
                    Titulo = "Curricula de Tratamiento de Reinserción Social",
                    Centro = centro.DESCR.ToUpper().Trim()
                });

                #region Apodo
                var apodo = string.Empty;
                if(SelectExpediente.APODO != null)
                {
                    foreach(var a in SelectExpediente.APODO)
                    {
                        if(!string.IsNullOrEmpty(apodo))
                            apodo = apodo + ", ";
                        apodo = apodo + a.APODO1.Trim();
                    }
                }
                #endregion

                #region Ubicacion
                var ubicacion = string.Empty;
                if (SelectIngreso.CAMA != null)
                {
                    ubicacion = string.Format("{0}-{1}-{2}-{3}",
                        SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim(),
                        SelectIngreso.CAMA.CELDA.SECTOR.DESCR.Trim(),
                        SelectIngreso.CAMA.CELDA.ID_CELDA.Trim(),
                        SelectIngreso.CAMA.ID_CAMA);
                }
                #endregion

                #region Causa Penal
                var causa_penal = string.Empty;
                if (SelectIngreso.CAUSA_PENAL != null)
                { 
                    int [] estatus = {0,1,4,6};
                    foreach (var cp in SelectIngreso.CAUSA_PENAL.Where(w => estatus.Contains(w.ID_ESTATUS_CP.Value)))
                    {
                        if (!string.IsNullOrEmpty(causa_penal))
                            causa_penal = causa_penal + ", ";
                        causa_penal = causa_penal + string.Format("{0}/{1}", cp.CP_ANIO, cp.CP_FOLIO);
                    }
                }
                #endregion

                #region foto
                byte[] foto = new Imagenes().getImagenPerson();
                if (SelectIngreso.INGRESO_BIOMETRICO != null)
                {
                    var b = SelectIngreso.INGRESO_BIOMETRICO.FirstOrDefault(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO);
                    if (b != null)
                    {
                        foto = b.BIOMETRICO;
                    }
                }
                #endregion

                GeneralesKardex.Add(new cKardex() {
                    EXPEDIENTE = string.Format("{0}/{1}", SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO),
                    NOMBRE = string.Format("{0} {1} {2}",
                        SelectExpediente.NOMBRE.Trim(),
                        !string.IsNullOrEmpty(SelectExpediente.PATERNO) ? SelectExpediente.PATERNO.Trim() : string.Empty,
                        !string.IsNullOrEmpty(SelectExpediente.MATERNO) ? SelectExpediente.MATERNO.Trim() : string.Empty),
                    EDAD = new Fechas().CalculaEdad(SelectExpediente.NACIMIENTO_FECHA,hoy),
                    APODO = apodo,
                    FECHA_NACIMIENTO = SelectExpediente.NACIMIENTO_FECHA.Value,
                    SEXO = SelectExpediente.SEXO == "F" ? "FEMENINO" : "MASCULINO",
                    ESTADO_CIVIL = SelectIngreso.ESTADO_CIVIL != null ?  SelectIngreso.ESTADO_CIVIL.DESCR.Trim() : string.Empty,
                    CENTRO = centro.DESCR.Trim(),
                    ESTATUS_JURIDICO = SelectIngreso.CLASIFICACION_JURIDICA != null ? SelectIngreso.CLASIFICACION_JURIDICA.DESCR.Trim() : string.Empty,
                    UBICACION = ubicacion,
                    NO_INGRESO = SelectIngreso.ID_INGRESO,
                    CAUSA_PENAL = causa_penal,
                    CENTRO_PROCEDENCIA = "NINGUNO",
                    FOTO = foto,
                    FECHA_IMPRESION = hoy
                });

                var sentencia = new cSentencia().ObtenerSentenciaIngreso(
                    SelectIngreso.ID_CENTRO,
                    SelectIngreso.ID_ANIO,
                    SelectIngreso.ID_IMPUTADO,
                    SelectIngreso.ID_INGRESO,
                    hoy);
                if (sentencia != null)
                {
                    foreach (var s in sentencia)
                    {
                        #region 3 / 5 partes
                        DateTime? fecha_cumple = null; 
                        var cumple = "NO";
                        int dias = (((s.S_ANIO * 365) + (s.S_MES * 30) + s.S_DIA) / 5) * 3;
                        int dias_cumplidos = (s.C_ANIO * 365) + (s.C_MES * 30) + s.C_DIA;
                        if (dias_cumplidos > dias)
                        {
                            cumple = "SI";
                        }
                        else
                        {
                            fecha_cumple = hoy.Date;
                            fecha_cumple = fecha_cumple.Value.AddDays(dias - dias_cumplidos);
                        }
                        #endregion

                        #region Fecha Libertad
                        var fecha_libertad = hoy;
                        if(s.PC_ANIO > 0)
                            fecha_libertad = fecha_libertad.AddYears(s.PC_ANIO);
                        if(s.PC_MES > 0)
                            fecha_libertad = fecha_libertad.AddMonths(s.PC_MES);
                        if(s.PC_DIA > 0)
                            fecha_libertad = fecha_libertad.AddDays(s.PC_DIA);

                        #endregion
                        
                        JuridicoKardex.Add(new cKardexJuridico() { 
                         SENTENCIA_ANIOS = s.S_ANIO,
                         SENTENCIA_MESES = s.S_MES,
                         SENTENCIA_DIAS = s.S_DIA,
                         COMPURGADO_ANIOS = s.C_ANIO,
                         COMPURGADO_MESES = s.C_MES,
                         COMPURGADO_DIAS = s.C_DIA,
                         POR_COMPURGADO_ANIOS = s.PC_ANIO,
                         POR_COMPURGADO_MESES = s.PC_MES,
                         POR_COMPURGADO_DIAS = s.PC_DIA,
                         CUMPLE_3_5_PARTES = cumple,
                         FECHA_3_5_PARTES = fecha_cumple,
                         FECHA_LIBERTAD = fecha_libertad
                        });  
                    }
                }

                var datos = new cGrupoParticipante().ObtenerReporteKardex(
                    SelectIngreso.ID_CENTRO,
                    SelectIngreso.ID_ANIO,
                    SelectIngreso.ID_IMPUTADO,
                    SelectIngreso.ID_INGRESO).Select(w => new cKardexContenido() { 
                        ID_CONSEC = w.ID_CONSEC,
                        ID_CENTRO =w.ID_CENTRO,
                        ID_ANIO =w.ID_ANIO,
                        ID_IMPUTADO =w.ID_IMPUTADO,
                        ID_INGRESO =w.ID_INGRESO,
                        ID_DEPARTAMENTO= w.ID_DEPARTAMENTO,
                        DEPARTAMENTO =w.DEPARTAMENTO,
                        ID_TIPO_PROGRAMA =w.ID_TIPO_PROGRAMA,
                        TIPO_PROGRAMA =w.TIPO_PROGRAMA,
                        ESTATUS =w.ESTATUS,
                        ID_EJE =w.ID_EJE,
                        EJE = w.EJE,
                        GRUPO= w.GRUPO,
                        FEC_INICIO=w.FEC_INICIO,
                        FEC_FIN =w.FEC_FIN,
                        RECURRENCIA= w.RECURRENCIA,
                        //HORA_INICIO =w.HORA_INICIO,
                        //HORA_TERMINO =w.HORA_TERMINO
                        ID_ACTIVIDAD = w.ID_ACTIVIDAD,
                        ACTIVIDAD = w.ACTIVIDAD,
                        ID_GRUPO = w.ID_GRUPO
                    });

                var horario = new cGrupoParticipante().ObtenerReporteKardexHorario(
                    SelectIngreso.ID_CENTRO,
                    SelectIngreso.ID_ANIO,
                    SelectIngreso.ID_IMPUTADO,
                    SelectIngreso.ID_INGRESO).Select(w => new cKardexHorario()
                    {
                        ID_CENTRO = w.ID_CENTRO,
                        ID_TIPO_PROGRAMA =w.ID_TIPO_PROGRAMA,
                        ID_ACTIVIDAD = w.ID_ACTIVIDAD,
                        ID_GRUPO = w.ID_GRUPO,
                        ID_DIA = w.ID_DIA,
                        DIA = w.DIA,
                        ID_GRUPO_HORARIO = w.ID_GRUPO_HORARIO,
                        HORA_INICIO = w.HORA_INICIO,
                        HORA_TERMINO = w.HORA_TERMINO
                    });

                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    Reporte.Reset();
                }));
                Reporte.LocalReport.ReportPath = "Reportes/rKardex.rdlc";
                Reporte.LocalReport.DataSources.Clear();

                ReportDataSource rds1 = new ReportDataSource();
                rds1.Name = "DataSet2";
                rds1.Value = datosReporte;
                Reporte.LocalReport.DataSources.Add(rds1);

                ReportDataSource rds2 = new ReportDataSource();
                rds2.Name = "DataSet1";
                rds2.Value = GeneralesKardex;
                Reporte.LocalReport.DataSources.Add(rds2);

                ReportDataSource rds3 = new ReportDataSource();
                rds3.Name = "DataSet3";
                rds3.Value = JuridicoKardex;
                Reporte.LocalReport.DataSources.Add(rds3);

                ReportDataSource rds4 = new ReportDataSource();
                rds4.Name = "DataSet4";
                rds4.Value = datos;
                Reporte.LocalReport.DataSources.Add(rds4);

                #region Subreporte
                Reporte.LocalReport.SubreportProcessing += (s, e) =>
                {
                    ReportDataSource sr = new ReportDataSource("DataSet1", horario);
                    e.DataSources.Add(sr);
                };
                #endregion

                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    Reporte.RefreshReport();
                    ReportViewerVisible = Visibility.Visible;
                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al imprimir el kardex.", ex);
            }
        }
        #endregion

        #region Cambio SelectedItem de Busqueda de Expediente
        private async void OnModelChangedSwitch(object parametro)
        {
            if (parametro != null)
            {
                switch (parametro.ToString())
                {
                    case "cambio_expediente":
                        if (SelectExpediente != null && (SelectExpediente.INGRESO == null || SelectExpediente.INGRESO.Count == 0))
                        {
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {
                                selectExpediente = new cImputado().Obtener(selectExpediente.ID_IMPUTADO, selectExpediente.ID_ANIO, selectExpediente.ID_CENTRO).First();
                                RaisePropertyChanged("SelectExpediente");
                            });
                            //MUESTRA LOS INGRESOS
                            if (SelectExpediente.INGRESO != null && SelectExpediente.INGRESO.Count > 0)
                            {
                                EmptyIngresoVisible = false;
                                SelectIngreso = SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                            }
                            else
                                EmptyIngresoVisible = true;

                            //OBTENEMOS FOTO DE FRENTE
                            if (SelectIngreso != null)
                            {
                                if (SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                                    ImagenImputado = SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                                else
                                    ImagenImputado = new Imagenes().getImagenPerson();
                            }
                            else
                                ImagenImputado = new Imagenes().getImagenPerson();
                        }
                        break;
                }
            }
        }
        #endregion
    }
}
