using Microsoft.Reporting.WinForms;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace ControlPenales
{
    public partial class ReporteHorarioResponsableGruposViewModel : ValidationViewModelBase
    {

        #region Constructor
        public ReporteHorarioResponsableGruposViewModel() { }
        #endregion

        #region Metodos
        private async void SwitchClick(Object obj)
        {
            if (!pConsultar)
            {
                ReportViewerVisible = Visibility.Collapsed;
                new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                return;
            }
            ValidarFiltros();
            if (SelectedResponsable == 0)
                return;
            ReportViewerVisible = Visibility.Collapsed;
            await StaticSourcesViewModel.CargarDatosMetodoAsync(GenerarReporte);
        }

        private async void OnLoad(ReporteHorarioResponsableGruposView Window = null)
        {
            try
            {
                ConfiguraPermisos();
                LstResponsable = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<NombreEmpleado>>(() => new ObservableCollection<NombreEmpleado>(new cPersona().GetData().Where(w => w.ID_TIPO_PERSONA == 1).Select(s => new NombreEmpleado
                {
                    ID_PERSONA = s.ID_PERSONA,
                    NOMBRE_COMPLETO = s.PATERNO.Trim() + " " + s.MATERNO.Trim() + " " + s.NOMBRE.Trim()
                }).OrderBy(o => o.NOMBRE_COMPLETO)));

                Reporte = Window.Report;
                Reporte.RenderingComplete += (s, e) =>
                {

                    ReportViewerVisible = Visibility.Visible;
                };
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar pantalla", ex);
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
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar programas", ex);
            }
        }
        #endregion

        #region Reporte
        private void GenerarReporte()
        {
            try
            {
                var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();

                var datosReporte = new List<cReporteDatos>();
                datosReporte.Add(new cReporteDatos()
                {
                    Encabezado1 = Parametro.ENCABEZADO1.Trim(),
                    Encabezado2 = Parametro.ENCABEZADO2.Trim(),
                    Encabezado3 = Parametro.ENCABEZADO3.Trim(),
                    Titulo = "HORARIO DEL RESPONSABLE",
                    Logo1 = Parametro.REPORTE_LOGO1,
                    Logo2 = Parametro.REPORTE_LOGO2,
                    Centro = centro.DESCR.Trim().ToUpper(),
                });

                #region Reporte
                Reporte.LocalReport.ReportPath = "Reportes/rHorarioResponsableGrupo.rdlc";
                Reporte.LocalReport.DataSources.Clear();

                var rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds2.Name = "DataSet2";
                rds2.Value = datosReporte;
                Reporte.LocalReport.DataSources.Add(rds2);

                var rds3 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds3.Name = "DataSet3";
                rds3.Value = new cGrupo().GetData().Where(w => w.ID_GRUPO_RESPONSABLE == SelectedResponsable && w.ID_ESTATUS_GRUPO == 1).AsEnumerable().Select(s => new ReporteListaResponsable
                {
                    Eje = s.ACTIVIDAD_EJE.EJE.DESCR,
                    Programa = new cTipoPrograma().GetData().Where(w => w.ID_TIPO_PROGRAMA == s.ID_TIPO_PROGRAMA).FirstOrDefault().NOMBRE,
                    Actividad = s.ACTIVIDAD.DESCR,
                    Grupo = s.DESCR,
                    Responsable = (s.PERSONA.PATERNO != null ? s.PERSONA.PATERNO.Trim() : string.Empty) + " " + (s.PERSONA.MATERNO != null ? s.PERSONA.MATERNO.Trim() : string.Empty) + " " + (s.PERSONA.NOMBRE != null ? s.PERSONA.NOMBRE.Trim() : string.Empty),
                    Fecha_Inicio = s.GRUPO_HORARIO.OrderBy(o => o.HORA_INICIO).FirstOrDefault().HORA_INICIO.Value.ToShortDateString(),
                    Fecha_Fin = s.GRUPO_HORARIO.OrderByDescending(o => o.HORA_TERMINO).FirstOrDefault().HORA_TERMINO.Value.ToShortDateString(),
                    Recurrencia = s.RECURRENCIA,
                    ID_GRUPO = s.ID_GRUPO.ToString()
                });
                Reporte.LocalReport.DataSources.Add(rds3);


                Reporte.LocalReport.SubreportProcessing += (s, e) =>
                {
                    if (e.ReportPath.Equals("srHorarioResponsable", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var intid = Convert.ToInt16(e.Parameters[0].Values[0]);
                        var preHorarioParticipante = new List<ReporteKardexInternoHorario>();

                        foreach (var subitem in new cGrupo().GetData().Where(w => w.ID_GRUPO == intid).FirstOrDefault().GRUPO_HORARIO.OrderBy(o => o.HORA_INICIO))
                            preHorarioParticipante.Add(new ReporteKardexInternoHorario()
                            {
                                FECHA = subitem.HORA_INICIO.Value.ToShortDateString(),
                                HORARIO = subitem.HORA_INICIO.Value.ToShortTimeString() + " - " + subitem.HORA_TERMINO.Value.ToShortTimeString(),
                                AREA = subitem.AREA.DESCR
                            });

                        e.DataSources.Add(new ReportDataSource("DataSet1", preHorarioParticipante));
                    }
                };

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
        #endregion

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_LISTADO_RESPONSABLES_GRUPO.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
