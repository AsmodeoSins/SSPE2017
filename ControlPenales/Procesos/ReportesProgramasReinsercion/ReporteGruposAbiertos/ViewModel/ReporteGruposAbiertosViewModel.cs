using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace ControlPenales
{
    public partial class ReporteGruposAbiertosViewModel : ValidationViewModelBase
    {

        #region Constructor
        public ReporteGruposAbiertosViewModel() { }
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
            if (!SelectedActividad.HasValue)
                return;
            ReportViewerVisible = Visibility.Collapsed;
            await StaticSourcesViewModel.CargarDatosMetodoAsync(GenerarReporte);
        }

        private async void OnLoad(ReporteGruposAbiertosView Window = null)
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

                var datosReporte = new List<cReporteDatos>();
                datosReporte.Add(new cReporteDatos()
                {
                    Encabezado1 = Parametro.ENCABEZADO1.Trim(),
                    Encabezado2 = Parametro.ENCABEZADO2.Trim(),
                    Encabezado3 = Parametro.ENCABEZADO3.Trim(),
                    Titulo = "LISTA GRUPO " + LstActividad.Where(w => w.ID_ACTIVIDAD == SelectedActividad).FirstOrDefault().DESCR,
                    Logo1 = Parametro.REPORTE_LOGO1,
                    Logo2 = Parametro.REPORTE_LOGO2,
                    Centro = centro.DESCR.Trim().ToUpper(),
                });

                #region Reporte
                Reporte.LocalReport.ReportPath = "Reportes/rGrupoActivos.rdlc";
                Reporte.LocalReport.DataSources.Clear();

                var rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds2.Name = "DataSet2";
                rds2.Value = datosReporte;
                Reporte.LocalReport.DataSources.Add(rds2);

                var rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds1.Name = "DataSet1";
                rds1.Value = new List<ReporteListaGruposActivos>() { new ReporteListaGruposActivos(){ 
                  Actividad = LstActividad.Where(w => w.ID_ACTIVIDAD == SelectedActividad).FirstOrDefault().DESCR,
                  Eje = LstEje.Where(w => w.ID_EJE == SelectedEje).FirstOrDefault().DESCR,
                  Programa = LstPrograma.Where(w => w.ID_TIPO_PROGRAMA == SelectedPrograma).FirstOrDefault().NOMBRE
                }};
                Reporte.LocalReport.DataSources.Add(rds1);

                var rds3 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds3.Name = "DataSet3";
                rds3.Value = new cGrupo().GetData().Where(w => w.ID_EJE == SelectedEje && w.ID_TIPO_PROGRAMA == SelectedPrograma && w.ID_ACTIVIDAD == SelectedActividad && w.ID_ESTATUS_GRUPO == 1).AsEnumerable().Select(s => new ReporteDetalleGruposActivos
                {
                    Grupo = s.DESCR,
                    Responsable = (s.PERSONA.PATERNO != null ? s.PERSONA.PATERNO.Trim() : string.Empty) + " " + (s.PERSONA.MATERNO != null ? s.PERSONA.MATERNO.Trim() : string.Empty) + " " + (s.PERSONA.NOMBRE != null ? s.PERSONA.NOMBRE.Trim() : string.Empty),
                    Fecha_Inicio = s.GRUPO_HORARIO.OrderBy(o => o.HORA_INICIO).FirstOrDefault().HORA_INICIO.Value.ToShortDateString(),
                    Fecha_Fin = s.GRUPO_HORARIO.OrderByDescending(o => o.HORA_TERMINO).FirstOrDefault().HORA_TERMINO.Value.ToShortDateString(),
                    Departamento = s.CENTRO_DEPARTAMENTO.DEPARTAMENTO.DESCR,
                    Recurrencia = s.RECURRENCIA
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
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_LISTADO_GRUPOS_ACTIVOS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
