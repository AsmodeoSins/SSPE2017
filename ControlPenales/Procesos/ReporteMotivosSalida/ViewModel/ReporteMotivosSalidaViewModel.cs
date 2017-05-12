using SSP.Controlador.Catalogo.Justicia;
using System;
using System.Linq;
using System.Windows;
namespace ControlPenales
{
    public partial class ReporteMotivosSalidaViewModel : ValidationViewModelBase
    {
        public ReporteMotivosSalidaViewModel() { }

        private async void SwitchClick(System.Object obj)
        {
            if (!pConsultar)
            {
                ReportViewerVisible = System.Windows.Visibility.Collapsed;
                new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                return;
            }
            ReportViewerVisible = System.Windows.Visibility.Collapsed;
            await StaticSourcesViewModel.CargarDatosMetodoAsync(GenerarReporte);
            //ReportViewerVisible = System.Windows.Visibility.Visible;
        }

        private void OnLoad(ReporteMotivosSalidaView Window = null)
        {
            try
            {
                ConfiguraPermisos();
                Reporte = Window.Report;
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar pantalla", ex);
            }
        }

        public void GenerarReporte()
        {
            try
            {
                if (SelectedFechaInicial == null)
                    return;

                if (SelectedFechaFinal == null)
                    return;

                var encabezado = new System.Collections.Generic.List<cEncabezado>();
                var listaMotivos = new System.Collections.Generic.List<cReporteMotivosSalida>();
                var centro = new SSP.Controlador.Catalogo.Justicia.cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();
                encabezado.Add(new cEncabezado()
                {
                    ImagenDerecha = Parametro.LOGO_ESTADO_BC,
                    TituloUno = Parametro.ENCABEZADO1,
                    TituloDos = Parametro.ENCABEZADO2,
                    NombreReporte = centro != null ? !string.IsNullOrEmpty(centro.DESCR) ? centro.DESCR.Trim() : string.Empty : string.Empty,
                    PieUno = string.Format("Reporte de Egresos por Motivos de Salida \n Del {0} Al {1}",
                                                SelectedFechaInicial.HasValue ? SelectedFechaInicial.Value.ToString("dd/MM/yyyy") : string.Empty,
                                                SelectedFechaFinal.HasValue ? SelectedFechaFinal.Value.ToString("dd/MM/yyyy") : string.Empty)
                });

                var _liberados = new SSP.Controlador.Catalogo.Justicia.cLiberacion().GetData(x => x.LIBERADO == "S" && x.LIBERACION_FEC >= SelectedFechaInicial && x.LIBERACION_FEC <= SelectedFechaFinal);
                if (_liberados.Any())
                {
                    var _MotivosLiberacion = _liberados.GroupBy(x => x.LIBERACION_MOTIVO).OrderBy(x => x.Key.DESCR);
                    if (_MotivosLiberacion != null && _MotivosLiberacion.Any())
                        foreach (var item in _MotivosLiberacion)
                        {
                            if (item.Key == null)
                                continue;

                            if (item.Any(x => x.CAUSA_PENAL != null && x.CAUSA_PENAL.CP_FUERO == "F"))
                            {
                                //son de fuero federal
                                int MujeresFederales = item.Count(x => x.CAUSA_PENAL != null && x.CAUSA_PENAL.CP_FUERO == "F" && x.INGRESO != null && x.INGRESO.IMPUTADO != null && x.INGRESO.IMPUTADO.SEXO == "F");
                                if (MujeresFederales > decimal.Zero)
                                    listaMotivos.Add(new cReporteMotivosSalida()
                                    {
                                        Fuero = "FEDERAL",
                                        Sexo = "FEMENINO",
                                        MotivoSalida = !string.IsNullOrEmpty(item.Key.DESCR) ? item.Key.DESCR.Trim() : string.Empty,
                                        TotalMotivos = MujeresFederales
                                    });


                                int HombresFederales = item.Count(x => x.CAUSA_PENAL != null && x.CAUSA_PENAL.CP_FUERO == "F" && x.INGRESO != null && x.INGRESO.IMPUTADO != null && x.INGRESO.IMPUTADO.SEXO == "M");
                                if (HombresFederales > decimal.Zero)//son de fuero federal y son hombres
                                    listaMotivos.Add(new cReporteMotivosSalida()
                                    {
                                        Fuero = "FEDERAL",
                                        Sexo = "MASCULINO",
                                        MotivoSalida = !string.IsNullOrEmpty(item.Key.DESCR) ? item.Key.DESCR.Trim() : string.Empty,
                                        TotalMotivos = HombresFederales
                                    });
                            }

                            if (item.Any(x => x.CAUSA_PENAL != null && x.CAUSA_PENAL.CP_FUERO == "C"))
                            {
                                //son de fuero comun
                                int MujeresComunes = item.Count(x => x.CAUSA_PENAL != null && x.CAUSA_PENAL.CP_FUERO == "C" && x.INGRESO != null && x.INGRESO.IMPUTADO != null && x.INGRESO.IMPUTADO.SEXO == "F");
                                if (MujeresComunes > decimal.Zero)//son de fuero comun y ademas son mujeres
                                    listaMotivos.Add(new cReporteMotivosSalida()
                                    {
                                        Fuero = "COMÚN",
                                        Sexo = "FEMENINO",
                                        MotivoSalida = !string.IsNullOrEmpty(item.Key.DESCR) ? item.Key.DESCR.Trim() : string.Empty,
                                        TotalMotivos = item.Any(x => x.INGRESO != null) ? item.Any(x => x.INGRESO.IMPUTADO != null) ? item.Count(x => x.INGRESO.IMPUTADO.SEXO == "F" && x.CAUSA_PENAL != null && x.CAUSA_PENAL.CP_FUERO == "C") : 0 : 0
                                    });

                                int HombresComunes = item.Count(x => x.CAUSA_PENAL != null && x.CAUSA_PENAL.CP_FUERO == "C" && x.INGRESO != null && x.INGRESO.IMPUTADO != null && x.INGRESO.IMPUTADO.SEXO == "M");
                                if (HombresComunes > decimal.Zero)//son de fuero comun y son hombres
                                    listaMotivos.Add(new cReporteMotivosSalida()
                                    {
                                        Fuero = "COMÚN",
                                        Sexo = "MASCULINO",
                                        MotivoSalida = !string.IsNullOrEmpty(item.Key.DESCR) ? item.Key.DESCR.Trim() : string.Empty,
                                        TotalMotivos = item.Any(x => x.INGRESO != null) ? item.Any(x => x.INGRESO.IMPUTADO != null) ? item.Count(x => x.INGRESO.IMPUTADO.SEXO == "M" && x.CAUSA_PENAL != null && x.CAUSA_PENAL.CP_FUERO == "C") : 0 : 0
                                    });
                            }
                        };
                };

                System.Windows.Application.Current.Dispatcher.Invoke((System.Action)(delegate
                {
                    ReportViewerVisible = Visibility.Collapsed;
                    Reporte.Reset();
                }));
                Reporte.LocalReport.ReportPath = "Reportes/rMotivosSalida.rdlc";
                Reporte.LocalReport.DataSources.Clear();

                Microsoft.Reporting.WinForms.ReportDataSource ReportDataSource_Encabezado = new Microsoft.Reporting.WinForms.ReportDataSource();
                ReportDataSource_Encabezado.Name = "DataSet1";
                ReportDataSource_Encabezado.Value = encabezado;

                Microsoft.Reporting.WinForms.ReportDataSource ReportDataSource_Motivos = new Microsoft.Reporting.WinForms.ReportDataSource();
                ReportDataSource_Motivos.Name = "DataSet2";
                ReportDataSource_Motivos.Value = listaMotivos;

                Reporte.LocalReport.DataSources.Add(ReportDataSource_Encabezado);
                Reporte.LocalReport.DataSources.Add(ReportDataSource_Motivos);

                System.Windows.Application.Current.Dispatcher.Invoke((System.Action)(delegate
                {
                    Reporte.Refresh();
                    Reporte.RefreshReport();
                    ReportViewerVisible = Visibility.Visible;
                }));
            }

            catch (System.Exception ex)
            {
                System.Windows.Application.Current.Dispatcher.Invoke((System.Action)(delegate
                {
                    ReportViewerVisible = Visibility.Collapsed;
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar pantalla", ex);
                }));
            }
        }

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_MOTIVOS_SALIDA.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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