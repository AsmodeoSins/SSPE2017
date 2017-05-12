using SSP.Controlador.Catalogo.Justicia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public partial class ReportePoblacionActivaCierreViewModel : ValidationViewModelBase
    {
        public ReportePoblacionActivaCierreViewModel() { }
        private async void SwitchClick(System.Object obj)
        {
            if (!pConsultar)
            {
                ReportViewerVisible = System.Windows.Visibility.Collapsed;
                new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                return;
            }
            if (!base.HasErrors)
            {
                ReportViewerVisible = System.Windows.Visibility.Collapsed;
                await StaticSourcesViewModel.CargarDatosMetodoAsync(GenerarReporteConQuery);
                ReportViewerVisible = System.Windows.Visibility.Visible;
            }
            else
                new Dialogos().ConfirmacionDialogo("Validación","Favor de capturar los campos requeridos. " + base.Error);
            
        }

        private void OnLoad(ReportePoblacionActivaCierreView Window = null)
        {
            try
            {
                SetValidaciones();
                ConfiguraPermisos();
                Reporte = Window.Report;
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar pantalla", ex);
            }
        }


        private void GenerarReporteConQuery() {
            try
            {
                var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();
                var reporteDatos = new List<cReporteDatos>();
                reporteDatos.Add(new cReporteDatos(){
                    Encabezado1 = Parametro.ENCABEZADO1,
                    Encabezado2 = Parametro.ENCABEZADO2,
                    Encabezado3 = Parametro.ENCABEZADO3,
                    Logo1 = Parametro.REPORTE_LOGO1,
                    Logo2 = Parametro.REPORTE_LOGO2,
                    Titulo = string.Format("Reporte de Población Activa al Cierre \n Del {0} Al {1}",
                                                SelectedFechaInicial.HasValue ? SelectedFechaInicial.Value.ToString("dd/MM/yyyy") : string.Empty,
                                                SelectedFechaFinal.HasValue ? SelectedFechaFinal.Value.ToString("dd/MM/yyyy") : string.Empty),
                    Centro = centro.DESCR.ToUpper().Trim()
                });
                var reporte = new cPaises().ObtenerPoblacionActivaCierre(GlobalVar.gCentro, SelectedFechaInicial.Value.Date,SelectedFechaFinal.Value.Date).Select(w => new cReporteIngresosActivosCierre()
                {
                     NombrePais = w.PAIS,
                     TotalH = w.MASCULINO,
                     TotalM = w.FEMENINO,
                     GrandTotal = w.TOTAL
                });
                System.Windows.Application.Current.Dispatcher.Invoke((System.Action)(delegate
                {
                    Reporte.Reset();
                }));
                Reporte.LocalReport.ReportPath = "Reportes/rReporteIngresosActivosCierre.rdlc";
                Reporte.LocalReport.DataSources.Clear();

                Microsoft.Reporting.WinForms.ReportDataSource ReportDataSource_Encabezado = new Microsoft.Reporting.WinForms.ReportDataSource();
                ReportDataSource_Encabezado.Name = "DataSet1";
                ReportDataSource_Encabezado.Value = reporteDatos;

                Microsoft.Reporting.WinForms.ReportDataSource ReportDataSource_Motivos = new Microsoft.Reporting.WinForms.ReportDataSource();
                ReportDataSource_Motivos.Name = "DataSet2";
                ReportDataSource_Motivos.Value = reporte;

                Reporte.LocalReport.DataSources.Add(ReportDataSource_Encabezado);
                Reporte.LocalReport.DataSources.Add(ReportDataSource_Motivos);

                System.Windows.Application.Current.Dispatcher.Invoke((System.Action)(delegate
                {
                    Reporte.Refresh();
                    Reporte.RefreshReport();
                }));
                
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar reporte", ex);
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
                var listaIngresosActivos = new System.Collections.Generic.List<cReporteIngresosActivosCierre>();
                var centro = new SSP.Controlador.Catalogo.Justicia.cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();
                encabezado.Add(new cEncabezado()
                {
                    ImagenDerecha = Parametro.LOGO_ESTADO_BC,
                    TituloUno = Parametro.ENCABEZADO1,
                    TituloDos = Parametro.ENCABEZADO2,
                    NombreReporte = centro != null ? !string.IsNullOrEmpty(centro.DESCR) ? centro.DESCR.Trim() : string.Empty : string.Empty,
                    PieUno = string.Format("Reporte de Población Activa al Cierre \n Del {0} Al {1}",
                                                SelectedFechaInicial.HasValue ? SelectedFechaInicial.Value.ToString("dd/MM/yyyy") : string.Empty,
                                                SelectedFechaFinal.HasValue ? SelectedFechaFinal.Value.ToString("dd/MM/yyyy") : string.Empty)
                });

                var _IngresosActivos = new SSP.Controlador.Catalogo.Justicia.cIngreso().ObtenerIngresosActivosPorFecha(GlobalVar.gCentro, SelectedFechaInicial, SelectedFechaFinal);
                int alf = _IngresosActivos.Count(x => x.IMPUTADO.NACIMIENTO_PAIS != null);
                if (_IngresosActivos.Any())
                {
                    int _NoUbicados = new int();
                    int _HombresNoUbicados = new int();
                    int _MujeresNoUbicadas = new int();
                    var _Agrupados = _IngresosActivos.GroupBy(x => x.IMPUTADO.NACIMIENTO_PAIS);
                    if (_Agrupados.Any())
                    {
                        foreach (var item in _Agrupados)
                        {
                            var _NombrePais = new SSP.Controlador.Catalogo.Justicia.cPaises().GetData(x => x.ID_PAIS_NAC == item.Key).FirstOrDefault();
                            if (_NombrePais == null)
                            {
                                _NoUbicados = _NoUbicados + item.Count();
                                _HombresNoUbicados = _HombresNoUbicados + (item.Any() ? item.Count(x => x.IMPUTADO != null && x.IMPUTADO.SEXO == "M") : 0);
                                _MujeresNoUbicadas = _MujeresNoUbicadas + (item.Any() ? item.Count(x => x.IMPUTADO != null && x.IMPUTADO.SEXO == "F") : 0);
                                continue;
                            }
                            else
                                if (string.IsNullOrEmpty(_NombrePais.PAIS))
                                {
                                    _NoUbicados = _NoUbicados + item.Count();
                                    _HombresNoUbicados = _HombresNoUbicados + (item.Any() ? item.Count(x => x.IMPUTADO != null && x.IMPUTADO.SEXO == "M") : 0);
                                    _MujeresNoUbicadas = _MujeresNoUbicadas + (item.Any() ? item.Count(x => x.IMPUTADO != null && x.IMPUTADO.SEXO == "F") : 0);
                                    continue;
                                }
                                else
                                    listaIngresosActivos.Add(new cReporteIngresosActivosCierre
                                           {
                                               TotalM = item.Any() ? item.Count(x => x.IMPUTADO != null && x.IMPUTADO.SEXO == "F") : 0,
                                               TotalH = item.Any() ? item.Count(x => x.IMPUTADO != null && x.IMPUTADO.SEXO == "M") : 0,
                                               NombrePais = _NombrePais != null ? !string.IsNullOrEmpty(_NombrePais.PAIS) ? _NombrePais.PAIS.Trim() : "SIN CAPTURAR" : "SIN CAPTURAR",
                                               GrandTotal = item.Count(x => x.IMPUTADO != null && x.IMPUTADO.SEXO == "M") + item.Count(x => x.IMPUTADO != null && x.IMPUTADO.SEXO == "F")
                                           });
                        };

                        //Ya tengo el condensado de las personas que no tienen pais
                        listaIngresosActivos.Add(new cReporteIngresosActivosCierre
                            {
                                NombrePais = "SIN CAPTURAR",
                                TotalH = _HombresNoUbicados,
                                TotalM = _MujeresNoUbicadas,
                                GrandTotal = _HombresNoUbicados + _MujeresNoUbicadas
                            });
                    }
                };

                Reporte.LocalReport.ReportPath = "Reportes/rReporteIngresosActivosCierre.rdlc";
                Reporte.LocalReport.DataSources.Clear();

                Microsoft.Reporting.WinForms.ReportDataSource ReportDataSource_Encabezado = new Microsoft.Reporting.WinForms.ReportDataSource();
                ReportDataSource_Encabezado.Name = "DataSet1";
                ReportDataSource_Encabezado.Value = encabezado;

                Microsoft.Reporting.WinForms.ReportDataSource ReportDataSource_Motivos = new Microsoft.Reporting.WinForms.ReportDataSource();
                ReportDataSource_Motivos.Name = "DataSet2";
                ReportDataSource_Motivos.Value = listaIngresosActivos != null ? listaIngresosActivos.Any() ? listaIngresosActivos.OrderBy(x => x.NombrePais).ToList() : new List<cReporteIngresosActivosCierre>() : new List<cReporteIngresosActivosCierre>();

                Reporte.LocalReport.DataSources.Add(ReportDataSource_Encabezado);
                Reporte.LocalReport.DataSources.Add(ReportDataSource_Motivos);

                System.Windows.Application.Current.Dispatcher.Invoke((System.Action)(delegate
                {
                    Reporte.Refresh();
                    Reporte.RefreshReport();
                }));
            }

            catch (System.Exception exc)
            {
                throw;
            }
        }

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_POBLACION_ACTIVA_CIERRE.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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