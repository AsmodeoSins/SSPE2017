using SSP.Controlador.Catalogo.Justicia;
using System;
using System.Linq;
namespace ControlPenales
{
    public partial class ReportesCNDHViewModel : ValidationViewModelBase
    {
        public ReportesCNDHViewModel() { }

        private void OnLoad(ReportesCNDHView Window = null)
        {
            try
            {
                ConfiguraPermisos();
                Reporte = Window.Report;
                LstClasificaciones = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.CLASIFICACION_JURIDICA>(new SSP.Controlador.Catalogo.Justicia.cClasificacionJuridica().GetData(x => x.PM == 0));
                LstClasificaciones.Insert(0, new SSP.Servidor.CLASIFICACION_JURIDICA { DESCR = "SELECCIONE", ID_CLASIFICACION_JURIDICA = string.Empty });

                SelectedReporte = -1;
                ClasificacionI = SelectedSexo = SelectedFuero = string.Empty;
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar pantalla", ex);
            }
        }


        private async void SwitchClick(System.Object obj)
        {
            if (!pConsultar)
            {
                ReportViewerVisible = System.Windows.Visibility.Collapsed;
                new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                return;
            }
            ReportViewerVisible = System.Windows.Visibility.Collapsed;

            if (SelectedReporte == null)
            {
                new Dialogos().ConfirmacionDialogo("Validación", "Seleccione el reporte a generar.");
                return;
            };

            if (SelectedReporte == -1)
            {
                new Dialogos().ConfirmacionDialogo("Validación", "Seleccione el reporte a generar.");
                return;
            };

            await StaticSourcesViewModel.CargarDatosMetodoAsync(GenerarReporte);
            ReportViewerVisible = System.Windows.Visibility.Visible;
        }

        public void GenerarReporte()
        {
            try
            {
                string _ruta = string.Empty;
                Reporte.LocalReport.DataSources.Clear();

                switch (SelectedReporte)
                {
                    case (short)eTipoReporte.ADULTO_MAYOR:
                        Reporte.LocalReport.DataSources.Add(EncabezadoReportesCNDH("ADULTOS MAYORES"));
                        Reporte.LocalReport.DataSources.Add(DatosAdultosMayores());
                        _ruta = "Reportes/rReporteAdultosMayoresCNDH.rdlc";
                        break;

                    case (short)eTipoReporte.ETNIAS:
                        Reporte.LocalReport.DataSources.Add(EncabezadoReportesCNDH("ETNIAS"));
                        Reporte.LocalReport.DataSources.Add(DatosEtnias());
                        Reporte.LocalReport.DataSources.Add(DatosCondensadoEtnias());
                        _ruta = "Reportes/rReporteIndigenasCNDH.rdlc";
                        break;

                    case (short)eTipoReporte.PREFERENCIAS_SEXUALES_DIFERENTES:
                        Reporte.LocalReport.DataSources.Add(EncabezadoReportesCNDH("CON PREFERENCIAS SEXUALES DISTINTAS"));
                        Reporte.LocalReport.DataSources.Add(DatosComportamientoHomoSexual());
                        _ruta = "Reportes/rReportePreferenciasSexualesCNDH.rdlc";
                        break;
                    case (short)eTipoReporte.LISTADO_GENERAL:
                        Reporte.LocalReport.DataSources.Add(EncabezadoReportesCNDH("LISTADO GENERAL"));
                        Reporte.LocalReport.DataSources.Add(ObtenerDatosReporteListadoGeneralNew());
                        _ruta = "Reportes/rListadoGeneralCNDH.rdlc";
                        break;
                    case (short)eTipoReporte.ADICCIONES:
                        Reporte.LocalReport.DataSources.Add(EncabezadoReportesCNDH("ADICCIONES"));
                        Reporte.LocalReport.DataSources.Add(ObtenerDatosReporteAdicciones());
                        _ruta = "Reportes/rAdiccionesCNDH.rdlc";
                        break;
                    case (short)eTipoReporte.DISCAPACIDAD_MENTAL:
                        Reporte.LocalReport.DataSources.Add(EncabezadoReportesCNDH("CON DISCAPACIDAD MENTAL"));
                        Reporte.LocalReport.DataSources.Add(ObtenerDatosReporteDiscapacidades(eTipoDiscapacidad.MENTAL));
                        _ruta = "Reportes/rDiscapacidadesMentalesCNDH.rdlc";
                        break;
                    case (short)eTipoReporte.DISCAPACIDAD_FISICA:
                        Reporte.LocalReport.DataSources.Add(EncabezadoReportesCNDH("CON DISCAPACIDAD FÍSICA"));
                        Reporte.LocalReport.DataSources.Add(ObtenerDatosReporteDiscapacidades(eTipoDiscapacidad.FISICA));
                        _ruta = "Reportes/rDiscapacidadesFisicasCNDH.rdlc";
                        break;
                    case (short)eTipoReporte.VIH:
                        Reporte.LocalReport.DataSources.Add(EncabezadoReportesCNDH("CON VIH"));
                        Reporte.LocalReport.DataSources.Add(DatosVIH());
                        _ruta = "Reportes/rReporteVIHCNDH.rdlc";
                        break;
                    default:
                        break;
                };

                Reporte.LocalReport.ReportPath = _ruta;
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

        /// <summary>
        /// METODO QUE RECIBE LEYENDA COMPLEMENTARIA DE REPORTES DE CNDH, ASI EVITA REPLICAR INECESARIAMENTE EL METODO PORQUE CAMBIE UN TEXTO SOLAMENTE
        /// </summary>
        /// <param name="_Complementario">
        ///     LEYENDA QUE SE QUIERE COMPLEMENTAR, PRESENTE EN CADA UNO DE LOS REPORTES DE LA CNDH
        /// </param>
        private Microsoft.Reporting.WinForms.ReportDataSource EncabezadoReportesCNDH(string _Complementario = "")
        {
            try
            {
                var encabezado = new System.Collections.Generic.List<cEncabezado>();
                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                var centro = new SSP.Controlador.Catalogo.Justicia.cCentro().GetData(x => x.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                encabezado.Add(new cEncabezado()
                {
                    ImagenDerecha = Parametro.REPORTE_LOGO2,
                    TituloUno = Parametro.ENCABEZADO1,
                    TituloDos = Parametro.ENCABEZADO2,
                    NombreReporte = centro != null ? !string.IsNullOrEmpty(centro.DESCR) ? centro.DESCR.Trim() : string.Empty : string.Empty,
                    PieUno = string.Format("RELACIÓN DE INTERNOS \n {0}", _Complementario)
                });

                rds1.Name = "DataSet1";
                rds1.Value = encabezado;
                return rds1;
            }

            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosComportamientoHomoSexual()
        {
            try
            {
                var datos = new System.Collections.Generic.List<cPreferenciaSexualDiferenteCNDH>();
                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                var info = new SSP.Controlador.Catalogo.Justicia.cIngreso().ObtenerEtniasReporte(GlobalVar.gCentro, string.Empty, ClasificacionI, SelectedSexo);//REUSO EL MISMO METODO PARA ESTA PARTE
                if (info.Any())
                    foreach (var item in info)
                    {
                        if (item.EMI_INGRESO != null)
                            if (item.EMI_INGRESO.Any())
                            {
                                var _UltimoEMI = item.EMI_INGRESO.OrderByDescending(x => x.ID_EMI_CONS).FirstOrDefault();
                                if (_UltimoEMI != null)
                                {
                                    var _DatosEMI = _UltimoEMI.EMI;
                                    if (_DatosEMI != null)
                                        if (_DatosEMI.EMI_HPS != null)
                                            if (_DatosEMI.EMI_HPS.COMPORTAMIENTO_HOMO != null)
                                            {
                                                SSP.Servidor.CAUSA_PENAL _Causa = new SSP.Servidor.CAUSA_PENAL();
                                                if (item.CAUSA_PENAL != null)
                                                    if (item.CAUSA_PENAL.Any())
                                                        _Causa = item.CAUSA_PENAL.FirstOrDefault(x => x.ID_ESTATUS_CP == (short)eEstatusCausaPenal.ACTIVO || x.ID_ESTATUS_CP == (short)eEstatusCausaPenal.EN_PROCESO);

                                                if (!string.IsNullOrEmpty(_SelectedFuero))// RE VALIDA EL FUERO DE LA CAUSA PENAL PARA ASEGURARSE QUE CONCUERDE CON EL FUERO ELEGIDO
                                                    if (_Causa != null)
                                                        if (_Causa.CP_FUERO != _SelectedFuero)
                                                            continue;

                                                if (_Causa != null)
                                                    if (!string.IsNullOrEmpty(_Causa.CP_FUERO))
                                                        datos.Add(new cPreferenciaSexualDiferenteCNDH
                                                            {
                                                                Clasificacion = item.CLASIFICACION_JURIDICA != null ? !string.IsNullOrEmpty(item.CLASIFICACION_JURIDICA.DESCR) ? item.CLASIFICACION_JURIDICA.DESCR.Trim() : string.Empty : string.Empty,
                                                                Expediente = string.Format("{0} / {1}", item.ID_ANIO, item.ID_IMPUTADO),
                                                                Nombre = string.Format("{0} {1} {2}",
                                                              item.IMPUTADO != null ? !string.IsNullOrEmpty(item.IMPUTADO.NOMBRE) ? item.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty,
                                                              item.IMPUTADO != null ? !string.IsNullOrEmpty(item.IMPUTADO.PATERNO) ? item.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty,
                                                              item.IMPUTADO != null ? !string.IsNullOrEmpty(item.IMPUTADO.MATERNO) ? item.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty),
                                                                Fuero = _Causa != null ? !string.IsNullOrEmpty(_Causa.CP_FUERO) ? _Causa.CP_FUERO == "F" ? "FEDERAL" : _Causa.CP_FUERO == "C" ? "COMÚN" : string.Empty : string.Empty : string.Empty,
                                                                Ubicacion = string.Format("{0}-{1}{2}-{3}",
                                                                               item.CAMA != null ? item.CAMA.CELDA != null ? item.CAMA.CELDA.SECTOR != null ? item.CAMA.CELDA.SECTOR.EDIFICIO != null ? !string.IsNullOrEmpty(item.CAMA.CELDA.SECTOR.EDIFICIO.DESCR) ? item.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                                                                               item.CAMA != null ? item.CAMA.CELDA != null ? item.CAMA.CELDA.SECTOR != null ? !string.IsNullOrEmpty(item.CAMA.CELDA.SECTOR.DESCR) ? item.CAMA.CELDA.SECTOR.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                                                                               item.CAMA != null ? item.CAMA.CELDA != null ? !string.IsNullOrEmpty(item.CAMA.CELDA.ID_CELDA) ? item.CAMA.CELDA.ID_CELDA.Trim() : string.Empty : string.Empty : string.Empty,
                                                                                       item.ID_UB_CAMA.HasValue ? item.ID_UB_CAMA.Value.ToString() : string.Empty),
                                                                Total = +1
                                                            });
                                            };
                                };
                            };
                    };

                rds1.Name = "DataSet2";
                rds1.Value = datos;
                return rds1;
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosVIH()
        {
            try
            {
                var datos = new System.Collections.Generic.List<cReporteVIHCNDH>();
                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                var info = new SSP.Controlador.Catalogo.Justicia.cIngreso().ObtenerEtniasReporte(GlobalVar.gCentro, string.Empty, ClasificacionI, SelectedSexo);//REUSO EL MISMO METODO PARA ESTA PARTE
                if (info.Any())
                    foreach (var item in info)
                    {
                        if (item.EMI_INGRESO != null)
                            if (item.EMI_INGRESO.Any())
                            {
                                var _UltimoEMI = item.EMI_INGRESO.OrderByDescending(x => x.ID_EMI_CONS).FirstOrDefault();
                                if (_UltimoEMI != null)
                                {
                                    var _DatosEMI = _UltimoEMI.EMI;
                                    if (_DatosEMI != null)
                                        if (_DatosEMI.EMI_HPS != null)
                                            if (_DatosEMI.EMI_ENFERMEDAD != null)
                                            {
                                                if (_DatosEMI.EMI_ENFERMEDAD.VIH_HEPATITIS == "S")
                                                {
                                                    SSP.Servidor.CAUSA_PENAL _Causa = new SSP.Servidor.CAUSA_PENAL();
                                                    if (item.CAUSA_PENAL != null)
                                                        if (item.CAUSA_PENAL.Any())
                                                            _Causa = item.CAUSA_PENAL.FirstOrDefault(x => x.ID_ESTATUS_CP == (short)eEstatusCausaPenal.ACTIVO || x.ID_ESTATUS_CP == (short)eEstatusCausaPenal.EN_PROCESO);

                                                    if (!string.IsNullOrEmpty(_SelectedFuero))// RE VALIDA EL FUERO DE LA CAUSA PENAL PARA ASEGURARSE QUE CONCUERDE CON EL FUERO ELEGIDO
                                                        if (_Causa != null)
                                                            if (_Causa.CP_FUERO != _SelectedFuero)
                                                                continue;

                                                    if (_Causa != null)
                                                        if (!string.IsNullOrEmpty(_Causa.CP_FUERO))
                                                            datos.Add(new cReporteVIHCNDH
                                                                {
                                                                    Clasificacion = item.CLASIFICACION_JURIDICA != null ? !string.IsNullOrEmpty(item.CLASIFICACION_JURIDICA.DESCR) ? item.CLASIFICACION_JURIDICA.DESCR.Trim() : string.Empty : string.Empty,
                                                                    Expediente = string.Format("{0} / {1}", item.ID_ANIO, item.ID_IMPUTADO),
                                                                    Nombre = string.Format("{0} {1} {2}",
                                                                  item.IMPUTADO != null ? !string.IsNullOrEmpty(item.IMPUTADO.NOMBRE) ? item.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty,
                                                                  item.IMPUTADO != null ? !string.IsNullOrEmpty(item.IMPUTADO.PATERNO) ? item.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty,
                                                                  item.IMPUTADO != null ? !string.IsNullOrEmpty(item.IMPUTADO.MATERNO) ? item.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty),
                                                                    Fuero = _Causa != null ? !string.IsNullOrEmpty(_Causa.CP_FUERO) ? _Causa.CP_FUERO == "F" ? "FEDERAL" : _Causa.CP_FUERO == "C" ? "COMÚN" : string.Empty : string.Empty : string.Empty,
                                                                    Ubicacion = string.Format("{0}-{1}{2}-{3}",
                                                                                   item.CAMA != null ? item.CAMA.CELDA != null ? item.CAMA.CELDA.SECTOR != null ? item.CAMA.CELDA.SECTOR.EDIFICIO != null ? !string.IsNullOrEmpty(item.CAMA.CELDA.SECTOR.EDIFICIO.DESCR) ? item.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                                                                                   item.CAMA != null ? item.CAMA.CELDA != null ? item.CAMA.CELDA.SECTOR != null ? !string.IsNullOrEmpty(item.CAMA.CELDA.SECTOR.DESCR) ? item.CAMA.CELDA.SECTOR.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                                                                                   item.CAMA != null ? item.CAMA.CELDA != null ? !string.IsNullOrEmpty(item.CAMA.CELDA.ID_CELDA) ? item.CAMA.CELDA.ID_CELDA.Trim() : string.Empty : string.Empty : string.Empty,
                                                                                   item.ID_UB_CAMA.HasValue ? item.ID_UB_CAMA.Value.ToString() : string.Empty),
                                                                    Total = +1
                                                                });
                                                };
                                            };
                                };
                            };
                    };

                rds1.Name = "DataSet2";
                rds1.Value = datos;
                return rds1;
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosEtnias()
        {
            try
            {
                var CondensadoEtnias = new System.Collections.Generic.List<cCondensadoEtniasReporteCNDH>();
                var ImputadosPorEtnia = new System.Collections.Generic.List<cReporteIndigenasCNDH>();
                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                var _datos = new SSP.Controlador.Catalogo.Justicia.cIngreso().ObtenerEtniasReporte(GlobalVar.gCentro, SelectedFuero, ClasificacionI, SelectedSexo);
                if (_datos.Any())
                {
                    foreach (var item in _datos)
                    {
                        if (item.IMPUTADO != null)
                        {
                            SSP.Servidor.CAUSA_PENAL _Causa = new SSP.Servidor.CAUSA_PENAL();
                            if (item.CAUSA_PENAL != null)
                                if (item.CAUSA_PENAL.Any())
                                    _Causa = item.CAUSA_PENAL.FirstOrDefault(x => x.ID_ESTATUS_CP == (short)eEstatusCausaPenal.ACTIVO || x.ID_ESTATUS_CP == (short)eEstatusCausaPenal.EN_PROCESO);

                            if (!string.IsNullOrEmpty(_SelectedFuero))
                                if (_Causa != null)
                                    if (_Causa.CP_FUERO != _SelectedFuero)
                                        continue;

                            if (_Causa != null)
                                if (!string.IsNullOrEmpty(_Causa.CP_FUERO))
                                    if (item.IMPUTADO.ID_ETNIA != null)
                                        if (item.IMPUTADO.ID_ETNIA != (short)eEtniasOmotidas.SIN_DATO)
                                            ImputadosPorEtnia.Add(new cReporteIndigenasCNDH
                                                {
                                                    Clasificacion = item.CLASIFICACION_JURIDICA != null ? !string.IsNullOrEmpty(item.CLASIFICACION_JURIDICA.DESCR) ? item.CLASIFICACION_JURIDICA.DESCR.Trim() : string.Empty : string.Empty,
                                                    Expediente = string.Format("{0} / {1}", item.ID_ANIO, item.ID_IMPUTADO),
                                                    Fuero = _Causa != null ? !string.IsNullOrEmpty(_Causa.CP_FUERO) ? _Causa.CP_FUERO == "F" ? "FEDERAL" : _Causa.CP_FUERO == "C" ? "COMÚN" : string.Empty : string.Empty : string.Empty,
                                                    Nombre = string.Format("{0} {1} {2}",
                                                             item.IMPUTADO != null ? !string.IsNullOrEmpty(item.IMPUTADO.NOMBRE) ? item.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty,
                                                             item.IMPUTADO != null ? !string.IsNullOrEmpty(item.IMPUTADO.PATERNO) ? item.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty,
                                                             item.IMPUTADO != null ? !string.IsNullOrEmpty(item.IMPUTADO.MATERNO) ? item.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty),
                                                    Etnia = item.IMPUTADO != null ? item.IMPUTADO.ID_ETNIA.HasValue ? !string.IsNullOrEmpty(item.IMPUTADO.ETNIA.DESCR) ? item.IMPUTADO.ETNIA.DESCR.Trim() : string.Empty : string.Empty : string.Empty,
                                                    Ubicacion = string.Format("{0}-{1}{2}-{3}",
                                                    item.CAMA != null ? item.CAMA.CELDA != null ? item.CAMA.CELDA.SECTOR != null ? item.CAMA.CELDA.SECTOR.EDIFICIO != null ? !string.IsNullOrEmpty(item.CAMA.CELDA.SECTOR.EDIFICIO.DESCR) ? item.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                                                                item.CAMA != null ? item.CAMA.CELDA != null ? item.CAMA.CELDA.SECTOR != null ? !string.IsNullOrEmpty(item.CAMA.CELDA.SECTOR.DESCR) ? item.CAMA.CELDA.SECTOR.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                                                                item.CAMA != null ? item.CAMA.CELDA != null ? !string.IsNullOrEmpty(item.CAMA.CELDA.ID_CELDA) ? item.CAMA.CELDA.ID_CELDA.Trim() : string.Empty : string.Empty : string.Empty,
                                                                item.ID_UB_CAMA.HasValue ? item.ID_UB_CAMA.Value.ToString() : string.Empty)
                                                });
                        };
                    };
                };

                rds1.Name = "DataSet2";
                rds1.Value = ImputadosPorEtnia;
                return rds1;
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosCondensadoEtnias()
        {
            try
            {
                var CondensadoEtnias = new System.Collections.Generic.List<cCondensadoEtniasReporteCNDH>();
                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                var ImputadosPorEtnia = new System.Collections.Generic.List<cReporteIndigenasCNDH>();
                var _datos = new SSP.Controlador.Catalogo.Justicia.cIngreso().ObtenerEtniasReporte(GlobalVar.gCentro, SelectedFuero, ClasificacionI, SelectedSexo);
                if (_datos.Any())
                {
                    foreach (var item in _datos)
                    {
                        if (item.IMPUTADO != null)
                        {
                            SSP.Servidor.CAUSA_PENAL _Causa = new SSP.Servidor.CAUSA_PENAL();
                            if (item.CAUSA_PENAL != null)
                                if (item.CAUSA_PENAL.Any())
                                    _Causa = item.CAUSA_PENAL.FirstOrDefault(x => x.ID_ESTATUS_CP == (short)eEstatusCausaPenal.ACTIVO || x.ID_ESTATUS_CP == (short)eEstatusCausaPenal.EN_PROCESO);

                            if (!string.IsNullOrEmpty(_SelectedFuero))
                                if (_Causa != null)
                                    if (_Causa.CP_FUERO != _SelectedFuero)
                                        continue;

                            if (_Causa != null)
                                if (!string.IsNullOrEmpty(_Causa.CP_FUERO))
                                    if (item.IMPUTADO.ID_ETNIA != null)
                                        if (item.IMPUTADO.ID_ETNIA != (short)eEtniasOmotidas.SIN_DATO)
                                            ImputadosPorEtnia.Add(new cReporteIndigenasCNDH { Etnia = item.IMPUTADO != null ? item.IMPUTADO.ID_ETNIA.HasValue ? !string.IsNullOrEmpty(item.IMPUTADO.ETNIA.DESCR) ? item.IMPUTADO.ETNIA.DESCR.Trim() : string.Empty : string.Empty : string.Empty });
                        };
                    };

                    if (ImputadosPorEtnia != null && ImputadosPorEtnia.Any())
                    {
                        var _EtniasFinales = ImputadosPorEtnia.GroupBy(x => x.Etnia);
                        if (_EtniasFinales != null && _EtniasFinales.Any())
                            foreach (var item in _EtniasFinales)
                            {
                                CondensadoEtnias.Add(new cCondensadoEtniasReporteCNDH
                                {
                                    Cantidad = item.Count(),
                                    NombreEtnia = item != null ? item.Any() ? item.FirstOrDefault().Etnia : string.Empty : string.Empty
                                });
                            };
                    }
                };

                rds1.Name = "DataSet3";
                rds1.Value = CondensadoEtnias;
                return rds1;
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosAdultosMayores()
        {
            try
            {
                var _datos = new System.Collections.Generic.List<cReporteAdultosMayoresCNDH>();
                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                var _IngresosActivos = new SSP.Controlador.Catalogo.Justicia.cIngreso().ObtenerIngresosTerceraEdad(ClasificacionI, string.Empty, SelectedSexo, GlobalVar.gCentro);
                if (_IngresosActivos.Any())
                {
                    if (!string.IsNullOrEmpty(SelectedFuero))
                    {
                        if (SelectedFuero == "C")
                            _IngresosActivos = _IngresosActivos.Where(x => x.CAUSA_PENAL.Any(y => y.CP_FUERO == "C"));
                        if (SelectedFuero == "F")
                            _IngresosActivos = _IngresosActivos.Where(x => x.CAUSA_PENAL.Any(y => y.CP_FUERO == "F"));
                    };

                    short _Total = new short();
                    short _ParametroTerceraEdad = Parametro.ID_TERCERA_EDAD;
                    foreach (var item in _IngresosActivos)
                    {
                        if (item.IMPUTADO != null)
                            if (item.IMPUTADO.NACIMIENTO_FECHA.HasValue)
                                if (new Fechas().CalculaEdad(item.IMPUTADO.NACIMIENTO_FECHA) >= _ParametroTerceraEdad)
                                {
                                    SSP.Servidor.CAUSA_PENAL _Causa = new SSP.Servidor.CAUSA_PENAL();
                                    if (item.CAUSA_PENAL != null)
                                        if (item.CAUSA_PENAL.Any())
                                            _Causa = item.CAUSA_PENAL.FirstOrDefault(x => x.ID_ESTATUS_CP == (short)eEstatusCausaPenal.ACTIVO || x.ID_ESTATUS_CP == (short)eEstatusCausaPenal.EN_PROCESO);

                                    if (_Causa != null)
                                    {
                                        if (!string.IsNullOrEmpty(_Causa.CP_FUERO))
                                        {
                                            _datos.Add(new cReporteAdultosMayoresCNDH
                                                {
                                                    Clasificacion = item.CLASIFICACION_JURIDICA != null ? !string.IsNullOrEmpty(item.CLASIFICACION_JURIDICA.DESCR) ? item.CLASIFICACION_JURIDICA.DESCR.Trim() : string.Empty : string.Empty,
                                                    Expediente = string.Format("{0} / {1}", item.ID_ANIO, item.ID_IMPUTADO),
                                                    Fuero = _Causa != null ? !string.IsNullOrEmpty(_Causa.CP_FUERO) ? _Causa.CP_FUERO == "F" ? "FEDERAL" : _Causa.CP_FUERO == "C" ? "COMÚN" : string.Empty : string.Empty : string.Empty,
                                                    Nombre = string.Format("{0} {1} {2}",
                                                             item.IMPUTADO != null ? !string.IsNullOrEmpty(item.IMPUTADO.NOMBRE) ? item.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty,
                                                             item.IMPUTADO != null ? !string.IsNullOrEmpty(item.IMPUTADO.PATERNO) ? item.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty,
                                                             item.IMPUTADO != null ? !string.IsNullOrEmpty(item.IMPUTADO.MATERNO) ? item.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty),
                                                    Ubicacion = string.Format("{0}-{1}{2}-{3}",
                                                    item.CAMA != null ? item.CAMA.CELDA != null ? item.CAMA.CELDA.SECTOR != null ? item.CAMA.CELDA.SECTOR.EDIFICIO != null ? !string.IsNullOrEmpty(item.CAMA.CELDA.SECTOR.EDIFICIO.DESCR) ? item.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                                                                item.CAMA != null ? item.CAMA.CELDA != null ? item.CAMA.CELDA.SECTOR != null ? !string.IsNullOrEmpty(item.CAMA.CELDA.SECTOR.DESCR) ? item.CAMA.CELDA.SECTOR.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                                                                item.CAMA != null ? item.CAMA.CELDA != null ? !string.IsNullOrEmpty(item.CAMA.CELDA.ID_CELDA) ? item.CAMA.CELDA.ID_CELDA.Trim() : string.Empty : string.Empty : string.Empty,
                                                                item.ID_UB_CAMA.HasValue ? item.ID_UB_CAMA.Value.ToString() : string.Empty)
                                                });

                                            _Total++;
                                        }
                                    };
                                };
                    };

                    _datos.ForEach(x => x.TotalInternos = _Total.ToString());
                };

                rds1.Name = "DataSet2";
                rds1.Value = _datos;
                return rds1;
            }

            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource ObtenerDatosReporteListadoGeneralNew()
        {
            try
            {
                var listado_general = new System.Collections.Generic.List<cListadoGeneralExpediente>();
                var lista = new cIngreso().ListadoGeneralCNDH(SelectedSexo,SelectedFuero,ClasificacionI,GlobalVar.gCentro);
                if (lista != null)
                {
                    foreach (var l in lista)
                    {
                        var r = new cListadoGeneralExpediente();
                        r.Anio = l.ID_ANIO;
                        r.Id_Imputado = l.ID_IMPUTADO;
                        r.Nombre = !string.IsNullOrEmpty(l.IMPUTADO.NOMBRE) ? l.IMPUTADO.NOMBRE.TrimEnd() : string.Empty;
                        r.Paterno = !string.IsNullOrEmpty(l.IMPUTADO.PATERNO) ? l.IMPUTADO.PATERNO.TrimEnd() : string.Empty;
                        r.Materno = !string.IsNullOrEmpty(l.IMPUTADO.MATERNO) ? l.IMPUTADO.MATERNO.TrimEnd() : string.Empty;
                        r.Clasificacion = !string.IsNullOrEmpty(l.CLASIFICACION_JURIDICA.DESCR) ? l.CLASIFICACION_JURIDICA.DESCR.TrimEnd() : string.Empty;
                        var cp = l.CAUSA_PENAL.FirstOrDefault(w => w.ID_ESTATUS_CP == (short)enumEstatusCausaPenal.ACTIVO);
                        if (cp != null)
                        {
                            if(cp.CP_FUERO == "C")
                                r.Fuero = "COMUN";
                            else
                                if(cp.CP_FUERO == "F")
                                    r.Fuero = "FEDERAL";
                        }
                        if (l.CAMA != null)
                        {
                            r.Edificio = !string.IsNullOrEmpty(l.CAMA.CELDA.SECTOR.EDIFICIO.DESCR) ? l.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.TrimEnd() : string.Empty;
                            r.Sector = !string.IsNullOrEmpty(l.CAMA.CELDA.SECTOR.DESCR) ? l.CAMA.CELDA.SECTOR.DESCR.TrimEnd() : string.Empty;
                            r.Celda = !string.IsNullOrEmpty(l.CAMA.CELDA.ID_CELDA) ? l.CAMA.CELDA.ID_CELDA.TrimEnd() : string.Empty;
                        }
                        listado_general.Add(r);
                    }
                }
                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds1.Name = "DataSet2";
                rds1.Value = listado_general;
                return rds1;
            }
            catch (Exception ex)
            {
                ReportViewerVisible = System.Windows.Visibility.Collapsed;
                throw ex;
                //StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error", ex);
            }
        }
        private Microsoft.Reporting.WinForms.ReportDataSource ObtenerDatosReporteListadoGeneral()
        {
            try
            {
                short[] ESTATUS_CAUSA_PENAL = new short[2];
                ESTATUS_CAUSA_PENAL[0] = CP_ACTIVA;
                ESTATUS_CAUSA_PENAL[1] = CP_EN_PROCESO;
                var consulta_edificio = new SSP.Controlador.Catalogo.Justicia.cEdificio();
                var consulta_sector = new SSP.Controlador.Catalogo.Justicia.cSector();
                var listado_general = new System.Collections.Generic.List<cListadoGeneralExpediente>();
                listado_general.AddRange(new SSP.Controlador.Catalogo.Justicia.cIngreso().ObtenerIngresosActivosListadoGeneral(ESTATUS_CAUSA_PENAL, string.Empty, SelectedFuero, GlobalVar.gCentro).AsEnumerable().
                    Where(w => w.CAUSA_PENAL != null && w.CAUSA_PENAL.Count(c => c.ID_ESTATUS_CP == CP_ACTIVA || c.ID_ESTATUS_CP == CP_EN_PROCESO) > 0).
                    Select(s =>
                        new cListadoGeneralExpediente()
                        {
                            Anio = s.ID_ANIO,
                            Celda = !string.IsNullOrEmpty(s.ID_UB_CELDA) && s.ID_UB_SECTOR.HasValue && s.ID_UB_EDIFICIO.HasValue ? s.ID_UB_CELDA.TrimEnd() : string.Empty,
                            Clasificacion = s.CLASIFICACION_JURIDICA != null ? (!string.IsNullOrEmpty(s.CLASIFICACION_JURIDICA.DESCR) ? s.CLASIFICACION_JURIDICA.DESCR.TrimEnd() : string.Empty) : string.Empty,
                            Edificio = !string.IsNullOrEmpty(s.ID_UB_CELDA) && s.ID_UB_SECTOR.HasValue && s.ID_UB_EDIFICIO.HasValue ? consulta_edificio.Obtener(s.ID_UB_EDIFICIO.Value, GlobalVar.gCentro).DESCR.TrimEnd() : string.Empty,
                            Fuero =
                            (s.CAUSA_PENAL.Any(a => a.ID_ESTATUS_CP == CP_ACTIVA) ?
                            (!string.IsNullOrEmpty(s.CAUSA_PENAL.FirstOrDefault(f => f.ID_ESTATUS_CP == CP_ACTIVA).CP_FUERO) ?
                            (s.CAUSA_PENAL.FirstOrDefault(f => f.ID_ESTATUS_CP == CP_ACTIVA).CP_FUERO.TrimEnd() == "C" ? "COMÚN" :
                            (s.CAUSA_PENAL.FirstOrDefault(f => f.ID_ESTATUS_CP == CP_ACTIVA).CP_FUERO.TrimEnd() == "F" ? "FEDERAL" :
                            (s.CAUSA_PENAL.FirstOrDefault(f => f.ID_ESTATUS_CP == CP_ACTIVA).CP_FUERO.TrimEnd() == "M" ? "MILITAR" : "SIN ESPECIFICAR")))
                            : "SIN ESPECIFICAR") :
                            (s.CAUSA_PENAL.Any(a => a.ID_ESTATUS_CP == CP_EN_PROCESO) ?
                            (!string.IsNullOrEmpty(s.CAUSA_PENAL.FirstOrDefault(f => f.ID_ESTATUS_CP == CP_EN_PROCESO).CP_FUERO) ?
                            (s.CAUSA_PENAL.FirstOrDefault(f => f.ID_ESTATUS_CP == CP_EN_PROCESO).CP_FUERO.TrimEnd() == "C" ? "COMÚN" :
                            (s.CAUSA_PENAL.FirstOrDefault(f => f.ID_ESTATUS_CP == CP_EN_PROCESO).CP_FUERO.TrimEnd() == "F" ? "FEDERAL" :
                            (s.CAUSA_PENAL.FirstOrDefault(f => f.ID_ESTATUS_CP == CP_EN_PROCESO).CP_FUERO.TrimEnd() == "M" ? "MILITAR" : "SIN ESPECIFICAR"))) :
                            "SIN ESPECIFICAR") : string.Empty)),
                            Id_Imputado = s.ID_IMPUTADO,
                            Materno = s.IMPUTADO != null ? (!string.IsNullOrEmpty(s.IMPUTADO.MATERNO) ? s.IMPUTADO.MATERNO.TrimEnd() : string.Empty) : string.Empty,
                            Nombre = s.IMPUTADO != null ? (!string.IsNullOrEmpty(s.IMPUTADO.NOMBRE) ? s.IMPUTADO.NOMBRE.TrimEnd() : string.Empty) : string.Empty,
                            Paterno = s.IMPUTADO != null ? (!string.IsNullOrEmpty(s.IMPUTADO.PATERNO) ? s.IMPUTADO.PATERNO.TrimEnd() : string.Empty) : string.Empty,
                            Sector = !string.IsNullOrEmpty(s.ID_UB_CELDA) && s.ID_UB_SECTOR.HasValue && s.ID_UB_EDIFICIO.HasValue ? consulta_sector.Obtener(s.ID_UB_SECTOR.Value, s.ID_UB_EDIFICIO.Value, GlobalVar.gCentro).DESCR.TrimEnd() : string.Empty,
                        }));

                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds1.Name = "DataSet2";
                rds1.Value = listado_general;
                return rds1;


            }
            catch (System.Exception ex)
            {
                throw ex;
            }

        }

        private Microsoft.Reporting.WinForms.ReportDataSource ObtenerDatosReporteAdicciones()
        {
            try
            {
                var consulta_edificio = new SSP.Controlador.Catalogo.Justicia.cEdificio();
                var consulta_sector = new SSP.Controlador.Catalogo.Justicia.cSector();

                var lIngresos_Adicciones = new System.Collections.Generic.List<cAdiccionesCNDH>();
                var lTotales = new System.Collections.Generic.List<cAdiccionesCNDH>();
                var ingresos_emi = new SSP.Controlador.Catalogo.Justicia.cIngreso().ObtenerIngresosActivosConAdicciones(GlobalVar.gCentro, !string.IsNullOrEmpty(SelectedSexo) ? SelectedSexo : string.Empty);



                foreach (var ingreso in ingresos_emi)
                {
                    // var drogas = new StringBuilder();
                    if (ingreso.EMI_INGRESO.Count > 0)
                    {
                        var mostrar = true;
                        foreach (var droga in ingreso.EMI_INGRESO.OrderByDescending(o => o.ID_EMI_CONS).FirstOrDefault().EMI.EMI_USO_DROGA)
                        {
                            // drogas.Append(string.Format("{0}{1}", droga.DROGA.DESCR.TrimEnd(), "\n"));
                            lIngresos_Adicciones.Add(new cAdiccionesCNDH()
                            {
                                Adiccion = droga.DROGA.DESCR.TrimEnd(),
                                Celda = !string.IsNullOrEmpty(ingreso.ID_UB_CELDA) && ingreso.ID_UB_SECTOR.HasValue && ingreso.ID_UB_EDIFICIO.HasValue ? ingreso.ID_UB_CELDA.TrimEnd() : string.Empty,
                                Edificio = !string.IsNullOrEmpty(ingreso.ID_UB_CELDA) && ingreso.ID_UB_SECTOR.HasValue && ingreso.ID_UB_EDIFICIO.HasValue ? consulta_edificio.Obtener(ingreso.ID_UB_EDIFICIO.Value, GlobalVar.gCentro).DESCR.TrimEnd() : string.Empty,
                                Id_Anio = ingreso.ID_ANIO,
                                Id_Imputado = ingreso.ID_IMPUTADO,
                                Materno = ingreso.IMPUTADO != null ? (!string.IsNullOrEmpty(ingreso.IMPUTADO.MATERNO) ? ingreso.IMPUTADO.MATERNO.TrimEnd() : string.Empty) : string.Empty,
                                Nombre = ingreso.IMPUTADO != null ? (!string.IsNullOrEmpty(ingreso.IMPUTADO.NOMBRE) ? ingreso.IMPUTADO.NOMBRE.TrimEnd() : string.Empty) : string.Empty,
                                Paterno = ingreso.IMPUTADO != null ? (!string.IsNullOrEmpty(ingreso.IMPUTADO.PATERNO) ? ingreso.IMPUTADO.PATERNO.TrimEnd() : string.Empty) : string.Empty,
                                Sector = !string.IsNullOrEmpty(ingreso.ID_UB_CELDA) && ingreso.ID_UB_SECTOR.HasValue && ingreso.ID_UB_EDIFICIO.HasValue ? consulta_sector.Obtener(ingreso.ID_UB_SECTOR.Value, ingreso.ID_UB_EDIFICIO.Value, GlobalVar.gCentro).DESCR.TrimEnd() : string.Empty,
                                mostrarUbicacion = mostrar
                            });
                            mostrar = false;
                        }
                    }
                }
                lIngresos_Adicciones = lIngresos_Adicciones.OrderBy(o => o.Id_Anio).ThenBy(t => t.Id_Imputado).ToList();
                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds1.Name = "DataSet2";
                rds1.Value = lIngresos_Adicciones;
                return rds1;

            }
            catch (System.Exception ex)
            {

                throw ex;
            }
        }


        private Microsoft.Reporting.WinForms.ReportDataSource ObtenerDatosReporteDiscapacidades(eTipoDiscapacidad TIPO_DISCAPACIDAD)
        {
            try
            {
                var consulta_edificio = new SSP.Controlador.Catalogo.Justicia.cEdificio();
                var consulta_sector = new SSP.Controlador.Catalogo.Justicia.cSector();

                var lIngresos_Discapacidades = new System.Collections.Generic.List<cDiscapacidadesCNDH>();
                var ingresos_emi = new SSP.Controlador.Catalogo.Justicia.cIngreso().ObtenerIngresosConDiscapacidad((int)TIPO_DISCAPACIDAD, GlobalVar.gCentro, !string.IsNullOrEmpty(SelectedSexo) ? SelectedSexo : string.Empty);



                foreach (var ingreso in ingresos_emi)
                {
                    if (ingreso.EMI_INGRESO.Count > 0)
                    {
                        var emi_ingreso = ingreso.EMI_INGRESO.OrderByDescending(o => o.ID_EMI_CONS).FirstOrDefault().EMI;
                        if (emi_ingreso.EMI_ENFERMEDAD != null &&
                            emi_ingreso.EMI_ENFERMEDAD.DISCAPACIDAD == "S" &&
                            emi_ingreso.EMI_ENFERMEDAD.ID_TIPO_DISCAPACIDAD == (short)TIPO_DISCAPACIDAD)
                            lIngresos_Discapacidades.Add(new cDiscapacidadesCNDH()
                            {
                                Discapacidad = !string.IsNullOrEmpty(emi_ingreso.EMI_ENFERMEDAD.DESCR_DISCAPACIDAD) ? emi_ingreso.EMI_ENFERMEDAD.DESCR_DISCAPACIDAD.TrimEnd() : "SIN DEFINIR",
                                Celda = !string.IsNullOrEmpty(ingreso.ID_UB_CELDA) && ingreso.ID_UB_SECTOR.HasValue && ingreso.ID_UB_EDIFICIO.HasValue ? ingreso.ID_UB_CELDA.TrimEnd() : string.Empty,
                                Edificio = !string.IsNullOrEmpty(ingreso.ID_UB_CELDA) && ingreso.ID_UB_SECTOR.HasValue && ingreso.ID_UB_EDIFICIO.HasValue ? consulta_edificio.Obtener(ingreso.ID_UB_EDIFICIO.Value, GlobalVar.gCentro).DESCR.TrimEnd() : string.Empty,
                                Id_Anio = ingreso.ID_ANIO,
                                Id_Imputado = ingreso.ID_IMPUTADO,
                                Materno = ingreso.IMPUTADO != null ? (!string.IsNullOrEmpty(ingreso.IMPUTADO.MATERNO) ? ingreso.IMPUTADO.MATERNO.TrimEnd() : string.Empty) : string.Empty,
                                Nombre = ingreso.IMPUTADO != null ? (!string.IsNullOrEmpty(ingreso.IMPUTADO.NOMBRE) ? ingreso.IMPUTADO.NOMBRE.TrimEnd() : string.Empty) : string.Empty,
                                Paterno = ingreso.IMPUTADO != null ? (!string.IsNullOrEmpty(ingreso.IMPUTADO.PATERNO) ? ingreso.IMPUTADO.PATERNO.TrimEnd() : string.Empty) : string.Empty,
                                Sector = !string.IsNullOrEmpty(ingreso.ID_UB_CELDA) && ingreso.ID_UB_SECTOR.HasValue && ingreso.ID_UB_EDIFICIO.HasValue ? consulta_sector.Obtener(ingreso.ID_UB_SECTOR.Value, ingreso.ID_UB_EDIFICIO.Value, GlobalVar.gCentro).DESCR.TrimEnd() : string.Empty,
                            });


                    }
                }
                lIngresos_Discapacidades = lIngresos_Discapacidades.OrderBy(o => o.Id_Anio).ThenBy(t => t.Id_Imputado).ToList();
                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds1.Name = "DataSet2";
                rds1.Value = lIngresos_Discapacidades;
                return rds1;

            }
            catch (System.Exception ex)
            {

                throw ex;
            }

        }
        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_CDNH.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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