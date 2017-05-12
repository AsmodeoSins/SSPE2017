using SSP.Controlador.Catalogo.Justicia;
using System;
using System.Linq;
namespace ControlPenales
{
    public partial class ReporteReingresoViewModel : ValidationViewModelBase
    {
        public ReporteReingresoViewModel() { }

        private async void SwitchClick(System.Object obj)
        {
            ReportViewerVisible = System.Windows.Visibility.Collapsed;

            if (!pConsultar)
            {
                new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                return;
            }
            if (SelectedFechaInicial == null)
            {
                new Dialogos().ConfirmacionDialogo("Validación", "Ingrese la fecha de inicio");
                return;
            }

            if (SelectedFechaFinal == null)
            {
                new Dialogos().ConfirmacionDialogo("Validación", "Ingrese la fecha de fin");
                return;
            }

            await StaticSourcesViewModel.CargarDatosMetodoAsync(GeneraReporteConQuery);
            ReportViewerVisible = System.Windows.Visibility.Visible;
        }

        private void OnLoad(ReporteReingresoView Window = null)
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


        public void GeneraReporteConQuery()
        {
            try 
            {
                var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();
                var encabezado = new System.Collections.Generic.List<cEncabezado>();
                encabezado.Add(new cEncabezado()
                {
                    ImagenIzquierda = Parametro.REPORTE_LOGO2,
                    ImagenDerecha = Parametro.REPORTE_LOGO1,
                    TituloUno = Parametro.ENCABEZADO1,
                    TituloDos = Parametro.ENCABEZADO2,
                    NombreReporte = centro != null ? !string.IsNullOrEmpty(centro.DESCR) ? centro.DESCR.Trim() : string.Empty : string.Empty,
                    PieUno = string.Format("Reporte de Reingresos por Fuero y Delito \n Del {0} Al {1}",
                                                SelectedFechaInicial.HasValue ? SelectedFechaInicial.Value.ToString("dd/MM/yyyy") : string.Empty,
                                                SelectedFechaFinal.HasValue ? SelectedFechaFinal.Value.ToString("dd/MM/yyyy") : string.Empty)
                });

                var reporte = new cIngreso().ReporteReingresos(GlobalVar.gCentro).Select(w => new cReporteReingreso() { 
                    FUERO = w.ID_FUERO == "C" ? "COMUN" : "FEDERAL",
                    ID_DELITO = w.ID_DELITO,
                    DELITO = w.DELITO,
                    TOTAL_M = w.TOTAL_M,
                    TOTAL_F = w.TOTAL_F,
                    TOTAL = w.TOTAL
                });

                System.Windows.Application.Current.Dispatcher.Invoke((System.Action)(delegate
                {
                    Reporte.Reset();
                    
                }));

                Reporte.LocalReport.ReportPath = "Reportes/rReporteReingresosNew.rdlc";
                Reporte.LocalReport.DataSources.Clear();

                Microsoft.Reporting.WinForms.ReportDataSource ds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                ds1.Name = "DataSet1";
                ds1.Value = encabezado;
                Reporte.LocalReport.DataSources.Add(ds1);
                
                Microsoft.Reporting.WinForms.ReportDataSource ds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                ds2.Name = "DataSet2";
                ds2.Value = reporte;
                Reporte.LocalReport.DataSources.Add(ds2);
                
                System.Windows.Application.Current.Dispatcher.Invoke((System.Action)(delegate
                {
                    Reporte.Refresh();
                    Reporte.RefreshReport();
                }));
            }
            catch(Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar el reporte", ex);
            }
        }

        public void GenerarReporte()
        {
            try
            {
                var encabezado = new System.Collections.Generic.List<cEncabezado>();
                var listaIngresosActivos = new System.Collections.Generic.List<cReporteReingresos>();
                var ListaDelitosPreliminar = new System.Collections.Generic.List<cReporteReingresos>();
                var listaIngresosDefinitivos = new System.Collections.Generic.List<cReporteReingresos>();//COMUNES
                var listaIngresosDefinitivos2 = new System.Collections.Generic.List<cReporteReingresos>();//FEDERALES
                var listaIngresosDefinitivos3 = new System.Collections.Generic.List<cReporteReincidentes>();//OTROS
                var centro = new SSP.Controlador.Catalogo.Justicia.cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();
                encabezado.Add(new cEncabezado()
                {
                    ImagenIzquierda = Parametro.REPORTE_LOGO1,
                    ImagenDerecha = Parametro.REPORTE_LOGO2,
                    TituloUno = Parametro.ENCABEZADO1,
                    TituloDos = Parametro.ENCABEZADO2,
                    NombreReporte = centro != null ? !string.IsNullOrEmpty(centro.DESCR) ? centro.DESCR.Trim() : string.Empty : string.Empty,
                    PieUno = string.Format("Reporte de Reingresos por Fuero y Delito \n Del {0} Al {1}",
                                                SelectedFechaInicial.HasValue ? SelectedFechaInicial.Value.ToString("dd/MM/yyyy") : string.Empty,
                                                SelectedFechaFinal.HasValue ? SelectedFechaFinal.Value.ToString("dd/MM/yyyy") : string.Empty)
                });

                var _IngresosActivos = new SSP.Controlador.Catalogo.Justicia.cIngreso().ObtenerIngresosActivosPorFecha(GlobalVar.gCentro, SelectedFechaInicial, SelectedFechaFinal);
                if (_IngresosActivos != null && _IngresosActivos.Any())
                    foreach (var item in _IngresosActivos)
                        if (item.CAUSA_PENAL != null && item.CAUSA_PENAL.Any())
                        {
                            var _detallesCausaPenal = item.CAUSA_PENAL.FirstOrDefault(x => x.ID_ESTATUS_CP == (short)eEstatusCausaPenal.ACTIVO);
                            if (_detallesCausaPenal != null)
                            {
                                if (_detallesCausaPenal.SENTENCIA != null)
                                {
                                    if (_detallesCausaPenal.SENTENCIA.Any(x => x.FEC_EJECUTORIA == null))//NO CAUSO EJECUTORIA
                                    {
                                        var _fuero = _detallesCausaPenal.CP_FUERO;
                                        if (!string.IsNullOrEmpty(_fuero))
                                        {
                                            var _Delitos = _detallesCausaPenal.CAUSA_PENAL_DELITO;
                                            if (_Delitos != null && _Delitos.Any())
                                            {
                                                if (_fuero == "C")
                                                {
                                                    foreach (var item2 in _Delitos)
                                                        ListaDelitosPreliminar.Add(new cReporteReingresos
                                                            {
                                                                Delito = item2.MODALIDAD_DELITO != null ? !string.IsNullOrEmpty(item2.MODALIDAD_DELITO.DESCR) ? item2.MODALIDAD_DELITO.DESCR.Trim() : string.Empty : string.Empty,
                                                                Femenino = item.IMPUTADO != null ? item.IMPUTADO.SEXO == "F" ? +1 : +0 : +0,
                                                                Masculino = item.IMPUTADO != null ? item.IMPUTADO.SEXO == "M" ? +1 : +0 : +0,
                                                                Fuero = _fuero,
                                                                Total = +1
                                                            });
                                                };

                                                if (_fuero == "F")
                                                {
                                                    foreach (var item2 in _Delitos)
                                                        ListaDelitosPreliminar.Add(new cReporteReingresos
                                                        {
                                                            Delito = item2.MODALIDAD_DELITO != null ? !string.IsNullOrEmpty(item2.MODALIDAD_DELITO.DESCR) ? item2.MODALIDAD_DELITO.DESCR.Trim() : string.Empty : string.Empty,
                                                            Femenino = item.IMPUTADO != null ? item.IMPUTADO.SEXO == "F" ? +1 : +0 : +0,
                                                            Masculino = item.IMPUTADO != null ? item.IMPUTADO.SEXO == "M" ? +1 : +0 : +0,
                                                            Fuero = _fuero,
                                                            Total = +1
                                                        });
                                                };
                                            };
                                        }
                                        else
                                        {
                                            var _Delitos = _detallesCausaPenal.CAUSA_PENAL_DELITO;//ESTOS DELITOS NO TIENEN FUERO PERO SI CAUSARON EJECUTORIA EN UNA CAUSA PENAL ACTIVA
                                            if (_Delitos != null && _Delitos.Any())
                                                foreach (var itemX in _Delitos)
                                                    ListaDelitosPreliminar.Add(new cReporteReingresos
                                                    {
                                                        Delito = itemX.MODALIDAD_DELITO != null ? !string.IsNullOrEmpty(itemX.MODALIDAD_DELITO.DESCR) ? itemX.MODALIDAD_DELITO.DESCR.Trim() : string.Empty : string.Empty,
                                                        Femenino = _detallesCausaPenal != null ? _detallesCausaPenal.INGRESO != null ? _detallesCausaPenal.INGRESO.IMPUTADO != null ? _detallesCausaPenal.INGRESO.IMPUTADO.SEXO == "F" ? +1 : +0 : +0 : +0 : +0,
                                                        Masculino = _detallesCausaPenal != null ? _detallesCausaPenal.INGRESO != null ? _detallesCausaPenal.INGRESO.IMPUTADO != null ? _detallesCausaPenal.INGRESO.IMPUTADO.SEXO == "M" ? +1 : +0 : +0 : +0 : +0,
                                                        Fuero = "X",
                                                        Total = +1
                                                    });
                                        }
                                    }//EJECUTORIA
                                };//SENTENCIA
                            };
                        };

                if (ListaDelitosPreliminar != null && ListaDelitosPreliminar.Any())
                {
                    var _Comunes = ListaDelitosPreliminar.Where(x => x.Fuero == "C");
                    var _Federales = ListaDelitosPreliminar.Where(x => x.Fuero == "F");
                    var _SinFuero = ListaDelitosPreliminar.Where(x => x.Fuero == "X");
                    if (_Comunes != null && _Comunes.Any())
                    {
                        var _DetalleComunes = _Comunes.GroupBy(x => x.Delito);
                        if (_DetalleComunes != null && _DetalleComunes.Any())
                            foreach (var item in _DetalleComunes)
                                if (!string.IsNullOrEmpty(item.Key))
                                    listaIngresosDefinitivos.Add(new cReporteReingresos
                                        {
                                            Delito = item.Key,
                                            Femenino = item.Sum(x => x.Femenino),
                                            Masculino = item.Sum(x => x.Masculino),
                                            Fuero = "COMÚN",
                                            Total = (item.Any() ? item.Sum(x => x.Femenino) : 0) + (item.Any() ? item.Sum(x => x.Masculino) : 0)
                                        });
                    };

                    if (_Federales != null && _Federales.Any())
                    {
                        var _DetalleFederales = _Federales.GroupBy(x => x.Delito);
                        if (_DetalleFederales != null && _DetalleFederales.Any())
                            foreach (var item in _DetalleFederales)
                                if (!string.IsNullOrEmpty(item.Key))
                                    listaIngresosDefinitivos2.Add(new cReporteReingresos
                                        {
                                            Delito = item.Key,
                                            Femenino = item.Sum(x => x.Femenino),
                                            Masculino = item.Sum(x => x.Masculino),
                                            Fuero = "FEDERAL",
                                            Total = (item.Any() ? item.Sum(x => x.Femenino) : 0) + (item.Any() ? item.Sum(x => x.Masculino) : 0)
                                        });
                    };

                    if (_SinFuero != null && _SinFuero.Any())
                    {
                        var _DetalleSinFuero = _Federales.GroupBy(x => x.Delito);
                        if (_DetalleSinFuero != null && _DetalleSinFuero.Any())
                            foreach (var item in _DetalleSinFuero)
                                if (!string.IsNullOrEmpty(item.Key))
                                    listaIngresosDefinitivos3.Add(new cReporteReincidentes
                                    {
                                        Delito = item.Key,
                                        Femenino = item.Sum(x => x.Femenino),
                                        Masculino = item.Sum(x => x.Masculino),
                                        Fuero = "SIN FUERO",
                                        Total = (item.Any() ? item.Sum(x => x.Femenino) : 0) + (item.Any() ? item.Sum(x => x.Masculino) : 0)
                                    });
                    };
                };

                Reporte.LocalReport.ReportPath = "Reportes/rReporteReingresos.rdlc";
                Reporte.LocalReport.DataSources.Clear();

                Microsoft.Reporting.WinForms.ReportDataSource ReportDataSource_Encabezado = new Microsoft.Reporting.WinForms.ReportDataSource();
                ReportDataSource_Encabezado.Name = "DataSet1";
                ReportDataSource_Encabezado.Value = encabezado;

                Microsoft.Reporting.WinForms.ReportDataSource ReportDataSource_Motivos = new Microsoft.Reporting.WinForms.ReportDataSource();
                ReportDataSource_Motivos.Name = "DataSet2";
                ReportDataSource_Motivos.Value = listaIngresosDefinitivos != null ? listaIngresosDefinitivos.Any() ? listaIngresosDefinitivos.OrderBy(x => x.Delito).ToList() : new System.Collections.Generic.List<cReporteReingresos>() : new System.Collections.Generic.List<cReporteReingresos>();

                Microsoft.Reporting.WinForms.ReportDataSource ReportDataSource_Motivos2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                ReportDataSource_Motivos2.Name = "DataSet3";
                ReportDataSource_Motivos2.Value = listaIngresosDefinitivos2 != null ? listaIngresosDefinitivos2.Any() ? listaIngresosDefinitivos2.OrderBy(x => x.Delito).ToList() : new System.Collections.Generic.List<cReporteReingresos>() : new System.Collections.Generic.List<cReporteReingresos>();

                Microsoft.Reporting.WinForms.ReportDataSource ReportDataSource_Motivos3 = new Microsoft.Reporting.WinForms.ReportDataSource();
                ReportDataSource_Motivos3.Name = "DataSet4";
                ReportDataSource_Motivos3.Value = listaIngresosDefinitivos3 != null ? listaIngresosDefinitivos3.Any() ? listaIngresosDefinitivos3.OrderBy(x => x.Delito).ToList() : new System.Collections.Generic.List<cReporteReincidentes>() : new System.Collections.Generic.List<cReporteReincidentes>();

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
                System.Windows.Application.Current.Dispatcher.Invoke((System.Action)(delegate
                {
                    ReportViewerVisible = System.Windows.Visibility.Collapsed;
                }));
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar el reporte", exc);
            }
        }

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new SSP.Controlador.Catalogo.Justicia.cProcesoUsuario().Obtener(enumProcesos.REPORTE_REINGRESOS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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