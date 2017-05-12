using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace ControlPenales
{
    public partial class ReporteHorarioAreaViewModel : ValidationViewModelBase
    {

        #region Constructor
        public ReporteHorarioAreaViewModel() { }
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
            if (!SelectedArea.HasValue)
                return;
            ReportViewerVisible = Visibility.Collapsed;
            await StaticSourcesViewModel.CargarDatosMetodoAsync(GenerarReporte);
        }

        private async void OnLoad(ReporteHorarioAreaView Window = null)
        {
            try
            {
                ConfiguraPermisos();
                ListArea = ListArea ?? await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<AREA>>(() => new ObservableCollection<AREA>(new cArea().GetData().Where(w => w.ID_TIPO_AREA != 5)));
                               
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
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar reporte", ex);
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
                    Titulo = "HORARIO DEL ÁREA",
                    Logo1 = Parametro.REPORTE_LOGO1,
                    Logo2 = Parametro.REPORTE_LOGO2,
                    Centro = centro.DESCR.Trim().ToUpper(),
                });

                #region Reporte
                Reporte.LocalReport.ReportPath = "Reportes/rHorarioArea.rdlc";
                Reporte.LocalReport.DataSources.Clear();

                var rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds2.Name = "DataSet2";
                rds2.Value = datosReporte;
                Reporte.LocalReport.DataSources.Add(rds2);

                var rds3 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds3.Name = "DataSet3";
                rds3.Value = new cGrupoHorario().GetData().Where(w => w.ID_AREA == SelectedArea).AsEnumerable().Select(s => new ReporteHorarioArea
                {
                    Eje = s.GRUPO.ACTIVIDAD_EJE.EJE.DESCR,
                    Programa = new cTipoPrograma().GetData().Where(w => w.ID_TIPO_PROGRAMA == s.ID_TIPO_PROGRAMA).FirstOrDefault().NOMBRE,
                    Actividad = s.GRUPO.ACTIVIDAD.DESCR,
                    Grupo = s.GRUPO.DESCR,
                    Responsable = (s.GRUPO.PERSONA.PATERNO != null ? s.GRUPO.PERSONA.PATERNO.Trim() : string.Empty) + " " + (s.GRUPO.PERSONA.MATERNO != null ? s.GRUPO.PERSONA.MATERNO.Trim() : string.Empty) + " " + (s.GRUPO.PERSONA.NOMBRE != null ? s.GRUPO.PERSONA.NOMBRE.Trim() : string.Empty),
                    Area = s.AREA.DESCR,
                    hora_inicio = s.HORA_INICIO.Value.ToShortDateString() + " " + s.HORA_INICIO.Value.ToShortTimeString(),
                    hora_termino = s.HORA_TERMINO.Value.ToShortDateString() + " " + s.HORA_TERMINO.Value.ToShortTimeString(),
                    Estatus = s.GRUPO_HORARIO_ESTATUS.DESCR
                });
                Reporte.LocalReport.DataSources.Add(rds3);

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
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_HORARIO_AREAS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
