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
    public partial class ReportePaseListaViewModel : ValidationViewModelBase
    {

        public async void OnLoad(ReportePaseLista Window)
        {
            try
            {
            ConfiguraPermisos();
            Ventana = Window;
            Reporte = Window.ReportPaseLista;
            Window.ListaMeses.ItemsSource = Enum.GetValues(typeof(eMesesAnio)).Cast<eMesesAnio>();
            Window.ListaMeses.SelectedItem = eMesesAnio.ENERO;
            

                ReportViewerVisible = Visibility.Collapsed;
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    ObtenerEdificios();
                });
                ReportViewerVisible = Visibility.Visible;
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
                    if (!pConsultar)
                    {
                        ReportViewerVisible = Visibility.Collapsed;
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    ReportViewerVisible = Visibility.Collapsed;
                    Reporte.Reset();
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                    {
                        GenerarReporte();
                    });
                    ReportViewerVisible = Visibility.Visible;
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

        public void GenerarReporte()
        {

            var Centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();
            try
            {
                #region EncabezadoReporte
                var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();
                var datosReporte = new List<cReporteDatos>();
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
                #endregion

                #region ListaEstancias
                List<cEstancia> lEstancia = new List<cEstancia>();
                var edificio = SelectedEdificio.ID_EDIFICIO != TODOS_LOS_EDIFICIOS ? SelectedEdificio.ID_EDIFICIO : (short?)0;
                var sector = SelectedEdificio.ID_EDIFICIO != TODOS_LOS_EDIFICIOS ? (SelectedSector.ID_SECTOR != TODOS_LOS_SECTORES ? SelectedSector.ID_SECTOR : (short?)0) : (short?)0;
                lEstancia.AddRange(new cCelda().ObtenerCeldas(
                    GlobalVar.gCentro,
                    edificio,
                    sector).
                    AsEnumerable().
                    Where(w =>
                    w.CAMA.Count > 0).
                    ToList().
                Select(s => new cEstancia()
                {
                    Pagina = 0,
                    CantidadCamas = s.CAMA.Count,
                    Edificio = s.ID_EDIFICIO,
                    Sector = s.ID_SECTOR,
                    Celda = s.ID_CELDA,
                    EdificioDescr = new cEdificio().Obtener(s.ID_EDIFICIO, GlobalVar.gCentro).DESCR.TrimEnd(),
                    SectorDescr = new cSector().Obtener(s.ID_SECTOR, s.ID_EDIFICIO, GlobalVar.gCentro).DESCR.TrimEnd(),
                    NumeroLista = 1
                }).
                OrderBy(o => o.Edificio).
                ThenBy(t => t.Sector).
                ThenBy(t => t.Celda).
                ToList());


                #endregion

                #region PaseLista
                lEstancia = lEstancia.OrderBy(o => o.Edificio).ThenBy(t => t.Sector).ThenBy(t => t.Celda).ToList(); //ThenBy(tB => tB.NumeroLista).ToList();
                var lPaseLista = new cCama().
                    ObtenerCamasEstancias(lEstancia.Select(s =>
                    new CELDA()
                    {
                        ID_CENTRO = GlobalVar.gCentro,
                        ID_EDIFICIO = (short)s.Edificio,
                        ID_SECTOR = (short)s.Sector,
                        ID_CELDA = s.Celda
                    }).ToList()).
                    AsEnumerable().
                    Select(s => new cPaseLista()
                    {
                        Celda = s.ID_CELDA,
                        Edificio = s.ID_EDIFICIO,
                        EdificioDescr = s.CELDA != null ? s.CELDA.SECTOR != null ? s.CELDA.SECTOR.EDIFICIO != null ? !string.IsNullOrEmpty(s.CELDA.SECTOR.EDIFICIO.DESCR) ? s.CELDA.SECTOR.EDIFICIO.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                        Expediente = (s.INGRESO != null && s.ESTATUS != CAMA_LIBERADA) ? (s.INGRESO.Where(w => w.ID_ESTATUS_ADMINISTRATIVO != (short)enumEstatusAdministrativo.LIBERADO &&
                        w.ID_ESTATUS_ADMINISTRATIVO != (short)enumEstatusAdministrativo.TRASLADADO &&
                        w.ID_ESTATUS_ADMINISTRATIVO != (short)enumEstatusAdministrativo.SUJETO_A_PROCESO_EN_LIBERTAD &&
                        w.ID_ESTATUS_ADMINISTRATIVO != (short)enumEstatusAdministrativo.DISCRECIONAL).Any() ? new Func<string>(() =>
                        {
                            var Ingreso = s.INGRESO.Where(w => w.ID_ESTATUS_ADMINISTRATIVO != (short)enumEstatusAdministrativo.LIBERADO &&
                                          w.ID_ESTATUS_ADMINISTRATIVO != (short)enumEstatusAdministrativo.TRASLADADO &&
                                          w.ID_ESTATUS_ADMINISTRATIVO != (short)enumEstatusAdministrativo.SUJETO_A_PROCESO_EN_LIBERTAD &&
                                          w.ID_ESTATUS_ADMINISTRATIVO != (short)enumEstatusAdministrativo.DISCRECIONAL).
                                          OrderByDescending(oD => oD.ID_ANIO).
                                          ThenByDescending(tD => tD.ID_IMPUTADO).
                                          FirstOrDefault();
                            return string.Format("{0}/{1}", Ingreso != null ? Ingreso.ID_ANIO : new short(), Ingreso != null ? Ingreso.ID_IMPUTADO : new Int32());
                        })() : "/") : "/",
                        Nombre = (s.INGRESO != null && s.ESTATUS != CAMA_LIBERADA) ? (s.INGRESO.Where(w => w.ID_ESTATUS_ADMINISTRATIVO != (short)enumEstatusAdministrativo.LIBERADO &&
                        w.ID_ESTATUS_ADMINISTRATIVO != (short)enumEstatusAdministrativo.TRASLADADO &&
                        w.ID_ESTATUS_ADMINISTRATIVO != (short)enumEstatusAdministrativo.SUJETO_A_PROCESO_EN_LIBERTAD &&
                        w.ID_ESTATUS_ADMINISTRATIVO != (short)enumEstatusAdministrativo.DISCRECIONAL).Any() ? new Func<string>(() =>
                        {
                            var Imputado = s.INGRESO.Where(w => w.ID_ESTATUS_ADMINISTRATIVO != (short)enumEstatusAdministrativo.LIBERADO &&
                                          w.ID_ESTATUS_ADMINISTRATIVO != (short)enumEstatusAdministrativo.TRASLADADO &&
                                          w.ID_ESTATUS_ADMINISTRATIVO != (short)enumEstatusAdministrativo.SUJETO_A_PROCESO_EN_LIBERTAD &&
                                          w.ID_ESTATUS_ADMINISTRATIVO != (short)enumEstatusAdministrativo.DISCRECIONAL).
                                          OrderByDescending(oD => oD.ID_ANIO).
                                          ThenByDescending(tD => tD.ID_IMPUTADO).
                                          FirstOrDefault().IMPUTADO;
                            return string.Format("{1} {2} {0}", Imputado != null ? !string.IsNullOrEmpty(Imputado.NOMBRE) ? Imputado.NOMBRE.TrimEnd() : string.Empty : string.Empty,
                                                                Imputado != null ? !string.IsNullOrEmpty(Imputado.PATERNO) ? Imputado.PATERNO.TrimEnd() : string.Empty : string.Empty,
                                                                Imputado != null ? !string.IsNullOrEmpty(Imputado.MATERNO) ? Imputado.MATERNO.TrimEnd() : string.Empty : string.Empty);
                        })() : "/") : "/",
                        Sector = s.ID_SECTOR,
                        SectorDescr = s.CELDA.SECTOR.DESCR,
                        Ubicacion = string.Format("{0}-{1}{2}-{3}",
                        s.CELDA != null ? s.CELDA.SECTOR != null ? s.CELDA.SECTOR.EDIFICIO != null ? !string.IsNullOrEmpty(s.CELDA.SECTOR.EDIFICIO.DESCR) ? s.CELDA.SECTOR.EDIFICIO.DESCR.TrimEnd() : string.Empty : string.Empty : string.Empty : string.Empty,
                        s.CELDA != null ? s.CELDA.SECTOR != null ? !string.IsNullOrEmpty(s.CELDA.SECTOR.DESCR) ? s.CELDA.SECTOR.DESCR.TrimEnd() : string.Empty : string.Empty : string.Empty,
                        s.ID_CELDA.TrimStart().TrimEnd(), s.ID_CAMA)
                    }).ToList();
                #endregion

                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    #region Reporte
                    Reporte.LocalReport.ReportPath = "Reportes/rImpresionListas.rdlc";
                    Reporte.LocalReport.DataSources.Clear();
                    ReportDataSource ReportDataSource_Encabezado = new ReportDataSource();
                    ReportDataSource_Encabezado.Name = "DataSet1";
                    ReportDataSource_Encabezado.Value = datosReporte;
                    Reporte.LocalReport.DataSources.Add(ReportDataSource_Encabezado);

                    ReportDataSource Camas = new ReportDataSource();
                    Camas.Name = "DataSet2";
                    Camas.Value = lPaseLista;
                    Reporte.LocalReport.DataSources.Add(Camas);

                    #endregion

                    #region Parametros
                    Reporte.LocalReport.SetParameters(new ReportParameter("Mes_Seleccionado", Ventana.ListaMeses.SelectedItem.ToString()));
                    Reporte.LocalReport.SetParameters(new ReportParameter("Anio", Fechas.GetFechaDateServer.Year.ToString()));
                    Reporte.LocalReport.SetParameters(new ReportParameter("Mes", Fechas.GetFechaDateServer.Month.ToString()));
                    Reporte.LocalReport.SetParameters(new ReportParameter("Dia", Fechas.GetFechaDateServer.Day.ToString()));
                    #endregion

                    Reporte.Refresh();
                    Reporte.RefreshReport();
                }));
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
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_IMPRESION_LISTAS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
