using System.Linq;
namespace ControlPenales
{
    public partial class ReporteSentenciadosSexoFueroDelitoViewModel : ValidationViewModelBase
    {
        public ReporteSentenciadosSexoFueroDelitoViewModel() { }

        private async void SwitchClick(System.Object obj)
        {
            if (!pConsultar)
            {
                ReportViewerVisible = System.Windows.Visibility.Collapsed;
                new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                return;
            }
            if (SelectedFechaInicial == null)
            {
                ReportViewerVisible = System.Windows.Visibility.Collapsed;
                new Dialogos().ConfirmacionDialogo("Validación", "Ingrese la fecha de inicio");
                return;
            }

            if (SelectedFechaFinal == null)
            {
                ReportViewerVisible = System.Windows.Visibility.Collapsed;
                new Dialogos().ConfirmacionDialogo("Validación", "Ingrese la fecha de fin");
                return;
            }

            ReportViewerVisible = System.Windows.Visibility.Collapsed;
            await StaticSourcesViewModel.CargarDatosMetodoAsync(GenerarReporte);
            ReportViewerVisible = System.Windows.Visibility.Visible;
        }

        private void OnLoad(ReporteSentenciadosSexoFueroDelitoView Window = null)
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
                var encabezado = new System.Collections.Generic.List<cEncabezado>();
                var listaIngresosActivos = new System.Collections.Generic.List<cReportePrimeraVezFueroDelito>();
                var listaIngresosDefinitivos = new System.Collections.Generic.List<cReportePrimeraVezFueroDelito>();//COMUNES
                var listaIngresosDefinitivos2 = new System.Collections.Generic.List<cReportePrimeraVezFueroDelito>();//FEDERALES
                var listaIngresosDefinitivos3 = new System.Collections.Generic.List<cReportePrimeraVezFueroDelito>();//OTROS
                var centro = new SSP.Controlador.Catalogo.Justicia.cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();
                encabezado.Add(new cEncabezado()
                {
                    ImagenDerecha = Parametro.LOGO_ESTADO_BC,
                    TituloUno = Parametro.ENCABEZADO1,
                    TituloDos = Parametro.ENCABEZADO2,
                    NombreReporte = centro != null ? !string.IsNullOrEmpty(centro.DESCR) ? centro.DESCR.Trim() : string.Empty : string.Empty,
                    PieUno = string.Format("Reporte de Sentenciados por Sexo, Fuero y Delito \n Del {0} Al {1}",
                                                SelectedFechaInicial.HasValue ? SelectedFechaInicial.Value.ToString("dd/MM/yyyy") : string.Empty,
                                                SelectedFechaFinal.HasValue ? SelectedFechaFinal.Value.ToString("dd/MM/yyyy") : string.Empty)
                });

                var _IngresosActivos = new SSP.Controlador.Catalogo.Justicia.cIngreso().ObtenerIngresosActivosPorFecha(GlobalVar.gCentro, SelectedFechaInicial, SelectedFechaFinal);
                var PrimerIngreso = _IngresosActivos.Where(x => x.ID_CLASIFICACION_JURIDICA == "3" && x.CAUSA_PENAL.Any(y => y.CAUSA_PENAL_DELITO.Any() && y.ID_ESTATUS_CP == (short)eEstatusCausaPenal.ACTIVO));
                foreach (var item in PrimerIngreso)
                {//CAUSAS PENALES CON DELITOS, ACTIVAS, PRODUCTO DE UN INGRESO SENTENCIADO ACTIVO
                    foreach (var item2 in item.CAUSA_PENAL)
                    {
                        foreach (var item3 in item2.CAUSA_PENAL_DELITO)
                        {
                            listaIngresosActivos.Add(new cReportePrimeraVezFueroDelito
                            {
                                Delito = item3.MODALIDAD_DELITO != null ? !string.IsNullOrEmpty(item3.MODALIDAD_DELITO.DESCR) ? item3.MODALIDAD_DELITO.DESCR.Trim() : string.Empty : string.Empty,
                                Fuero = item2.CP_FUERO,
                                Hombres = item2.INGRESO != null ? item2.INGRESO.IMPUTADO != null ? item2.INGRESO.IMPUTADO.SEXO == "M" ? +1 : +0 : +0 : +0,
                                Mujeres = item2.INGRESO != null ? item2.INGRESO.IMPUTADO != null ? item2.INGRESO.IMPUTADO.SEXO == "F" ? +1 : +0 : +0 : +0,
                                Total = (item2.INGRESO != null ? item2.INGRESO.IMPUTADO != null ? item2.INGRESO.IMPUTADO.SEXO == "M" ? +1 : +0 : +0 : +0) +
                                (item2.INGRESO != null ? item2.INGRESO.IMPUTADO != null ? item2.INGRESO.IMPUTADO.SEXO == "F" ? +1 : +0 : +0 : +0)
                            });
                        };
                    };
                };

                if (listaIngresosActivos != null && listaIngresosActivos.Any())
                {
                    var GruposDelitos = listaIngresosActivos.GroupBy(x => x.Delito);
                    foreach (var item in GruposDelitos)
                    {
                        var _Comunes = item.Any() ? item.Where(x => x.Fuero == "C") : null;
                        var _Federales = item.Any() ? item.Where(x => x.Fuero == "F") : null;
                        var _OtrosFueros = item.Any() ? item.Where(x => x.Fuero != "C" && x.Fuero != "F") : null;

                        if (_Comunes != null && _Comunes.Any())
                            if (_Comunes.Any())
                                listaIngresosDefinitivos.Add(new cReportePrimeraVezFueroDelito
                                {
                                    Delito = item.Key,
                                    Fuero = "COMÚN",
                                    Hombres = _Comunes.Sum(x => x.Hombres),
                                    Mujeres = _Comunes.Sum(x => x.Mujeres),
                                    Total = (_Comunes.Sum(x => x.Hombres)) + (_Comunes.Sum(x => x.Mujeres))
                                });

                        if (_Federales != null && _Federales.Any())
                            if (_Federales.Any())
                                listaIngresosDefinitivos2.Add(new cReportePrimeraVezFueroDelito
                                {
                                    Delito = item.Key,
                                    Fuero = "FEDERAL",
                                    Hombres = _Federales.Sum(x => x.Hombres),
                                    Mujeres = _Federales.Sum(x => x.Mujeres),
                                    Total = (_Federales.Sum(x => x.Hombres)) + (_Federales.Sum(x => x.Mujeres))
                                });


                        if (_OtrosFueros != null && _OtrosFueros.Any())
                            if (_OtrosFueros.Any())
                                listaIngresosDefinitivos3.Add(new cReportePrimeraVezFueroDelito
                                {
                                    Delito = item.Key,
                                    Fuero = "SIN FUERO",
                                    Hombres = _OtrosFueros.Sum(x => x.Hombres),
                                    Mujeres = _OtrosFueros.Sum(x => x.Mujeres),
                                    Total = (_OtrosFueros.Sum(x => x.Hombres)) + (_OtrosFueros.Sum(x => x.Mujeres))
                                });
                    };
                }

                Reporte.LocalReport.ReportPath = "Reportes/rReporteSentenciados.rdlc";
                Reporte.LocalReport.DataSources.Clear();

                Microsoft.Reporting.WinForms.ReportDataSource ReportDataSource_Encabezado = new Microsoft.Reporting.WinForms.ReportDataSource();
                ReportDataSource_Encabezado.Name = "DataSet1";
                ReportDataSource_Encabezado.Value = encabezado;

                Microsoft.Reporting.WinForms.ReportDataSource ReportDataSource_Motivos = new Microsoft.Reporting.WinForms.ReportDataSource();
                ReportDataSource_Motivos.Name = "DataSet2";
                ReportDataSource_Motivos.Value = listaIngresosDefinitivos != null ? listaIngresosDefinitivos.Any() ? listaIngresosDefinitivos.OrderBy(x => x.Delito).ToList() : new System.Collections.Generic.List<cReportePrimeraVezFueroDelito>() : new System.Collections.Generic.List<cReportePrimeraVezFueroDelito>();

                Microsoft.Reporting.WinForms.ReportDataSource ReportDataSource_Motivos2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                ReportDataSource_Motivos2.Name = "DataSet3";
                ReportDataSource_Motivos2.Value = listaIngresosDefinitivos2 != null ? listaIngresosDefinitivos2.Any() ? listaIngresosDefinitivos2.OrderBy(x => x.Delito).ToList() : new System.Collections.Generic.List<cReportePrimeraVezFueroDelito>() : new System.Collections.Generic.List<cReportePrimeraVezFueroDelito>();

                Microsoft.Reporting.WinForms.ReportDataSource ReportDataSource_Motivos3 = new Microsoft.Reporting.WinForms.ReportDataSource();
                ReportDataSource_Motivos3.Name = "DataSet4";
                ReportDataSource_Motivos3.Value = listaIngresosDefinitivos3 != null ? listaIngresosDefinitivos3.Any() ? listaIngresosDefinitivos3.OrderBy(x => x.Delito).ToList() : new System.Collections.Generic.List<cReportePrimeraVezFueroDelito>() : new System.Collections.Generic.List<cReportePrimeraVezFueroDelito>();

                Reporte.LocalReport.DataSources.Add(ReportDataSource_Encabezado);
                Reporte.LocalReport.DataSources.Add(ReportDataSource_Motivos);
                Reporte.LocalReport.DataSources.Add(ReportDataSource_Motivos2);
                Reporte.LocalReport.DataSources.Add(ReportDataSource_Motivos3);

                System.Windows.Application.Current.Dispatcher.Invoke((System.Action)(delegate
                {
                    Reporte.Refresh();
                    Reporte.RefreshReport();
                }));
            }

            catch (System.Exception exc)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar el reporte", exc);
            }
        }

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new SSP.Controlador.Catalogo.Justicia.cProcesoUsuario().Obtener(enumProcesos.REPORTE_SENTENCIADO_SEXO_FUERO_DELITO.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }
        #endregion
    }
}