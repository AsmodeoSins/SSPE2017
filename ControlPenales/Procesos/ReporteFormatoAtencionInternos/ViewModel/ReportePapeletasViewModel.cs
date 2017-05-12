using Microsoft.Reporting.WinForms;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlPenales
{
    public partial class ReportePapeletasViewModel : ValidationViewModelBase
    {

        public async void OnLoad(ReportePapeletas Window)
        {
            try
            {
                ConfiguraPermisos();
                Ventana = Window;
                Reporte = Ventana.ReportPapeletas;

                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ReportViewerVisible = Visibility.Collapsed;
                    }));
                    ObtenerEdificios();
                    SelectedFechaInicial = Fechas.GetFechaDateServer;
                    SelectedFechaFinal = Fechas.GetFechaDateServer;
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ReportViewerVisible = Visibility.Visible;
                    }));
                });
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }

        public async void ClickSwitch(object obj)
        {
            switch (obj.ToString())
            {
                case "GenerarReporte":
                    if (!pImprimir)
                    {
                        ReportViewerVisible = Visibility.Collapsed;
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(GenerarReporteQuery);
                    //GenerarReporte();
                    break;
            }
        }

        public void ObtenerEdificios()
        {
            Edificios = new List<EDIFICIO>();
            Edificios.Add(new EDIFICIO() { ID_CENTRO = GlobalVar.gCentro, ID_EDIFICIO = TODOS_LOS_EDIFICIOS, ID_TIPO_EDIFICIO = null, DESCR = "TODOS", ESTATUS = EDIFICIO_ACTIVO });
            SelectedEdificio = Edificios.FirstOrDefault();
            Edificios.AddRange(new cEdificio().ObtenerTodos(null, 0, GlobalVar.gCentro, EDIFICIO_ACTIVO).ToList());
        }

        public async void ObtenerSectores()
        {

            Sectores = new List<SECTOR>();
            Sectores.Add(new SECTOR() { ID_SECTOR = TODOS_LOS_SECTORES, DESCR = "TODOS" });
            SelectedSector = Sectores.FirstOrDefault();
            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
            {
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    ReportViewerVisible = Visibility.Collapsed;
                }));
                if (SelectedEdificio.ID_EDIFICIO != TODOS_LOS_EDIFICIOS)
                    Sectores.AddRange(new cSector().ObtenerTodos(null, null, GlobalVar.gCentro, SelectedEdificio.ID_EDIFICIO).ToList());
                else
                    Sectores.RemoveRange(1, Sectores.Count - 1);
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    ReportViewerVisible = Visibility.Visible;
                }));
            });
        }

        public async void GenerarReporte()
        {
            try
            {
                List<cPapeleta> lPapeletas = new List<cPapeleta>();
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ReportViewerVisible = Visibility.Collapsed;
                    }));
                    var edificio = SelectedEdificio.ID_EDIFICIO != TODOS_LOS_EDIFICIOS ? SelectedEdificio.ID_EDIFICIO : (short?)0;
                    var sector = SelectedEdificio.ID_EDIFICIO != TODOS_LOS_EDIFICIOS ? (SelectedSector.ID_SECTOR != TODOS_LOS_SECTORES ? SelectedSector.ID_SECTOR : (short?)0) : (short?)0;
                    List<CELDA> lEstancia = new List<CELDA>();
                    lEstancia.AddRange(new cCelda().ObtenerCeldas(
                            GlobalVar.gCentro,
                            edificio,
                            sector).
                            AsEnumerable().
                            Where(w =>
                            w.CAMA.Count > 0).
                            ToList().
                        Select(s => new CELDA()
                        {
                            ID_CENTRO = s.ID_CENTRO,
                            ID_EDIFICIO = s.ID_EDIFICIO,
                            ID_SECTOR = s.ID_SECTOR,
                            ID_CELDA = s.ID_CELDA,
                            SECTOR = s.SECTOR
                        }).
                        OrderBy(o => o.ID_EDIFICIO).
                        ThenBy(t => t.ID_SECTOR).
                        ThenBy(t => t.ID_CELDA).
                        ToList());
                    lPapeletas = new cIngreso().
                        ObtenerIngresosEstancias(lEstancia).
                        AsEnumerable().
                        Select(s => new cPapeleta()
                        {
                            Encabezado1 = Parametro.ENCABEZADO1.Trim(),
                            Encabezado2 = Parametro.ENCABEZADO2.Trim(),
                            Encabezado3 = Parametro.ENCABEZADO3.Trim(),
                            Logo1 = Parametro.REPORTE_LOGO1,
                            Logo2 = Parametro.REPORTE_LOGO2,
                            Edificio = !string.IsNullOrEmpty(s.CAMA.CELDA.SECTOR.EDIFICIO.DESCR) ? s.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.TrimEnd() : string.Empty,
                            Sector = !string.IsNullOrEmpty(s.CAMA.CELDA.SECTOR.DESCR) ? s.CAMA.CELDA.SECTOR.DESCR.TrimEnd() : string.Empty,
                            Celda = !string.IsNullOrEmpty(s.ID_UB_CELDA) ? s.ID_UB_CELDA.TrimEnd() : string.Empty,
                            Id_Anio = s.ID_ANIO,
                            Id_Imputado = s.ID_IMPUTADO,
                            Nombre = !string.IsNullOrEmpty(s.IMPUTADO.NOMBRE) ? s.IMPUTADO.NOMBRE.TrimEnd() : string.Empty,
                            Paterno = !string.IsNullOrEmpty(s.IMPUTADO.PATERNO) ? s.IMPUTADO.PATERNO.TrimEnd() : string.Empty,
                            Materno = !string.IsNullOrEmpty(s.IMPUTADO.MATERNO) ? s.IMPUTADO.MATERNO.TrimEnd() : string.Empty
                        }).
                        ToList();
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        Reporte.LocalReport.ReportPath = "Reportes/rPapeletas.rdlc";
                        Reporte.LocalReport.DataSources.Clear();
                        ReportDataSource ReportDataSource = new ReportDataSource();
                        ReportDataSource.Name = "DataSet1";
                        ReportDataSource.Value = lPapeletas;
                        Reporte.LocalReport.DataSources.Add(ReportDataSource);
                        Reporte.LocalReport.SetParameters(new ReportParameter("FechaInicial", string.Format("{0} DE {1} DEL {2}", SelectedFechaInicial.Day, Enum.GetName(typeof(eMesesAnio), SelectedFechaInicial.Month), SelectedFechaInicial.Year)));
                        Reporte.LocalReport.SetParameters(new ReportParameter("FechaFinal", string.Format("{0} DE {1} DEL {2}", SelectedFechaFinal.Day, Enum.GetName(typeof(eMesesAnio), SelectedFechaFinal.Month), SelectedFechaFinal.Year)));
                        Reporte.Refresh();
                        Reporte.RefreshReport();
                        ReportViewerVisible = Visibility.Visible;
                    }));
                });





            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }


        }

        private void GenerarReporteQuery()
        {
            try
            {
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    reporte.Reset();
                    ReportViewerVisible = Visibility.Collapsed;
                }));
                var encabezado1 = Parametro.ENCABEZADO1.Trim();
                var encabezado2 = Parametro.ENCABEZADO2.Trim();
                var encabezado3 = Parametro.ENCABEZADO3.Trim();
                var logo1 = Parametro.REPORTE_LOGO1;
                var logo2 = Parametro.REPORTE_LOGO2; 
                var img_persona = new Imagenes().getImagenPerson();
                var cndh = new Imagenes().getImagenCNDH();
                var cedhbc = new Imagenes().getImagenCEDHBC();
                var lPapeletas = new cIngreso().ObtenerInternosPapeletas(GlobalVar.gCentro, FEdificio != -1 ? (short?)FEdificio : null, FSector != -1 ? (short?)FSector : null).Select(w => new cPapeleta()
                {
                    Encabezado1 = encabezado1,
                    Encabezado2 = encabezado2,
                    Encabezado3 = encabezado3,
                    Logo1 = logo1,
                    Logo2 = logo2,
                    Edificio = w.EDIFICIO,
                    Sector = w.EDIFICIO,
                    Celda = w.CELDA,
                    Cama = w.CAMA,
                    Id_Anio = w.ID_ANIO,
                    Id_Imputado = w.ID_IMPUTADO,
                    Nombre = w.NOMBRE,
                    Paterno = w.PATERNO,
                    Materno = w.MATERNO,
                    FotoInterno = w.FOTO != null ? w.FOTO : img_persona,
                    CNDH = cndh,
                    CEDHBC = cedhbc,
                });
                
                Reporte.LocalReport.ReportPath = "Reportes/rPapeletas.rdlc";
                Reporte.LocalReport.DataSources.Clear();
                ReportDataSource ReportDataSource = new ReportDataSource();
                ReportDataSource.Name = "DataSet1";
                ReportDataSource.Value = lPapeletas;
                Reporte.LocalReport.DataSources.Add(ReportDataSource);
                Reporte.LocalReport.SetParameters(new ReportParameter("FechaInicial", string.Format("{0} DE {1} DEL {2}", SelectedFechaInicial.Day, Enum.GetName(typeof(eMesesAnio), SelectedFechaInicial.Month), SelectedFechaInicial.Year)));
                Reporte.LocalReport.SetParameters(new ReportParameter("FechaFinal", string.Format("{0} DE {1} DEL {2}", SelectedFechaFinal.Day, Enum.GetName(typeof(eMesesAnio), SelectedFechaFinal.Month), SelectedFechaFinal.Year)));
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    Reporte.Refresh();
                    Reporte.RefreshReport();
                    ReportViewerVisible = Visibility.Visible;
                }));
                
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar reporte", ex);
            }
        }

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_PAPELETAS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
