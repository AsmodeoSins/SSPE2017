using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace ControlPenales
{
    public partial class ReporteHorarioGruposViewModel : ValidationViewModelBase
    {

        #region Constructor
        public ReporteHorarioGruposViewModel() { }
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
            if (!selectedGrupo.HasValue)
                return;
            ReportViewerVisible = Visibility.Collapsed;
            await StaticSourcesViewModel.CargarDatosMetodoAsync(GenerarReporte);
        }

        private async void OnLoad(ReporteHorarioGruposView Window = null)
        {
            try
            {
                ConfiguraPermisos();
                var index = 0;
                LstEje = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<EJE>>(() => new ObservableCollection<EJE>(new cEje().GetData().OrderBy(o => o.ORDEN)));

                //LstEje.Insert(0, new EJE() { ID_EJE = -1, DESCR = "SELECCIONE" });

                if (LstEje.Count > 0)
                {
                    //ejes tipo modelo
                    LstEje.Insert(0, new EJE() { COMPLEMENTARIO = "MODELO" });
                    index = LstEje.IndexOf(LstEje.Where(w => w.COMPLEMENTARIO != "MODELO").OrderBy(o => o.COMPLEMENTARIO == "S").ThenBy(t => t.ORDEN).Where(w => w.COMPLEMENTARIO == "S").FirstOrDefault());
                    //ejes tipo complementario
                    if (index > 0)
                        LstEje.Insert(index, new EJE() { COMPLEMENTARIO = "COMPLEMENTARIO" });
                }
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
        async void ProgramasLoad(short Id_eje)
        {
            try
            {
                ReportViewerVisible = Visibility.Collapsed;
                LstPrograma = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<TIPO_PROGRAMA>>(() => new ObservableCollection<TIPO_PROGRAMA>(new cActividadEje().GetData().Where(w => w.ID_EJE == Id_eje).Select(s => s.ACTIVIDAD.TIPO_PROGRAMA).Distinct().OrderBy(o => o.NOMBRE).ToList()));

                Reporte.Reset();
                Reporte.RefreshReport();
                ReportViewerVisible = Visibility.Visible;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar programas", ex);
            }
        }

        async void ActividadesLoad(short Id_tipo_programa)
        {
            try
            {
                ReportViewerVisible = Visibility.Collapsed;
                LstActividad = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<ACTIVIDAD>>(() => new ObservableCollection<ACTIVIDAD>(new cActividadEje().GetData().Where(w => w.ID_EJE == SelectedEje && w.ID_TIPO_PROGRAMA == Id_tipo_programa).Select(s => s.ACTIVIDAD).Distinct().OrderBy(o => o.DESCR).ToList()));
                Reporte.Reset();
                Reporte.RefreshReport();
                ReportViewerVisible = Visibility.Visible;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar actividades", ex);
            }
        }
        #endregion

        #region Reporte
        private void GenerarReporte()
        {
            try
            {
                var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();
                var reporteConsentrado = new cGrupoHorario().GetData().Where(w => w.ID_GRUPO == selectedGrupo && w.ESTATUS == 1).ToList();

                var datosReporte = new List<cReporteDatos>();
                datosReporte.Add(new cReporteDatos()
                {
                    Encabezado1 = Parametro.ENCABEZADO1.Trim(),
                    Encabezado2 = Parametro.ENCABEZADO2.Trim(),
                    Encabezado3 = Parametro.ENCABEZADO3.Trim(),
                    Titulo = "LISTA GRUPO " + reporteConsentrado.FirstOrDefault().GRUPO.DESCR,
                    Logo1 = Parametro.REPORTE_LOGO1,
                    Logo2 = Parametro.REPORTE_LOGO2,
                    Centro = centro.DESCR.Trim().ToUpper(),
                });

                #region Reporte
                Reporte.LocalReport.ReportPath = "Reportes/rReporteHorarioGrupo.rdlc";
                Reporte.LocalReport.DataSources.Clear();

                var rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds2.Name = "DataSet2";
                rds2.Value = datosReporte;
                Reporte.LocalReport.DataSources.Add(rds2);

                var rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds1.Name = "DataSet1";
                rds1.Value = new List<ReporteListaGrupo>() { new ReporteListaGrupo(){ 
                 Grupo=reporteConsentrado.FirstOrDefault().GRUPO.DESCR,
                  Actividad=reporteConsentrado.FirstOrDefault().GRUPO.ACTIVIDAD.DESCR,
                  Eje=reporteConsentrado.FirstOrDefault().GRUPO.ACTIVIDAD_EJE.EJE.DESCR,
                  Programa=reporteConsentrado.FirstOrDefault().GRUPO.ACTIVIDAD.TIPO_PROGRAMA.NOMBRE,
                  Responsable= (reporteConsentrado.FirstOrDefault().GRUPO.PERSONA.PATERNO != null ? reporteConsentrado.FirstOrDefault().GRUPO.PERSONA.PATERNO.Trim() : string.Empty)+" "+ (reporteConsentrado.FirstOrDefault().GRUPO.PERSONA.MATERNO != null ? reporteConsentrado.FirstOrDefault().GRUPO.PERSONA.MATERNO.Trim():string.Empty) +" "+  (reporteConsentrado.FirstOrDefault().GRUPO.PERSONA.NOMBRE != null?reporteConsentrado.FirstOrDefault().GRUPO.PERSONA.NOMBRE.Trim():string.Empty)
                }};
                Reporte.LocalReport.DataSources.Add(rds1);

                var rds3 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds3.Name = "DataSet3";
                rds3.Value = reporteConsentrado.Select(s => new ReporteHorarioGrupo
                {
                    Area = s.AREA.DESCR,
                    dthora_inicio = s.HORA_INICIO,
                    hora_inicio = s.HORA_INICIO.Value.ToShortDateString() + " " + s.HORA_INICIO.Value.ToShortTimeString(),
                    hora_termino = s.HORA_TERMINO.Value.ToShortDateString() + " " + s.HORA_TERMINO.Value.ToShortTimeString(),
                    Estatus = s.GRUPO_HORARIO_ESTATUS.DESCR
                }).OrderBy(o => o.dthora_inicio);
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
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_HORARIOS_GRUPOS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
