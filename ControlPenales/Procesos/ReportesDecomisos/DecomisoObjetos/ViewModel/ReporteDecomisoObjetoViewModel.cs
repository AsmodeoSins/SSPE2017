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
    public partial class ReporteDecomisoObjetoViewModel : ValidationViewModelBase
    {
        public async void OnLoad(ReporteDecomisoObjetoView Window)
        {
            try
            {
                ConfiguraPermisos();
                ReportViewerVisible = Visibility.Collapsed;
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {

                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        Ventana = Window;
                        Reporte = Window.Report;
                        TodosLosObjetos = true;
                        ObtenerTipos();
                        SelectedTipo = ObjetoTipos.FirstOrDefault(f => f.ID_OBJETO_TIPO == -1);
                    }));
                });
                ReportViewerVisible = Visibility.Visible;
            }
            catch (Exception ex) {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la pantalla", ex);
            }
        }

        public void ClickSwitch(object obj)
        {
            switch (obj.ToString())
            {
                case "Busqueda":
                    if (!pConsultar)
                    {
                        ReportViewerVisible = Visibility.Collapsed;
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    GenerarReporte();
                    break;
            }
        }

        public void ObtenerTipos()
        {
            try
            {
                ObjetoTipos = new List<OBJETO_TIPO>();
                ObjetoTipos.Add(new OBJETO_TIPO()
                {
                    ID_OBJETO_TIPO = -1,
                    DESCR = "---SELECCIONE UN TIPO---"
                });
                ObjetoTipos.AddRange(new cObjetoTipo().ObtenerTodosTipos().ToList());
            }
            catch (Exception ex)
            {

                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Error al obtener los tipos de objeto.", ex);
            }
        }

        public async void GenerarReporte()
        {
            var datosReporte = new List<cReporteDatos>();
            var ingresosDecomiso = new List<cDecomisoCustodioIngreso>();
            var objetosDecomiso = new List<cDecomisoCustodioObjeto>();
            var visitantesDecomiso = new List<cDecomisoCustodioVisitante>();
            var decomisos = new List<cDecomisoFecha>();

            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ReportViewerVisible = Visibility.Collapsed;
                    }));

                    var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();

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


                    //var objetos_decomiso = new cDecomisoObjeto().ObtenerObjetos(null).ToList();
                    objetosDecomiso.AddRange(new cDecomisoObjeto().ObtenerObjetos(null).AsEnumerable().Select(s => new cDecomisoCustodioObjeto()
                    {
                        Cantidad = s.CANTIDAD.HasValue ? s.CANTIDAD.Value : 0,
                        Capacidad = !string.IsNullOrEmpty(s.CAPACIDAD) ? s.CAPACIDAD.TrimEnd() : string.Empty,
                        Comentarios = !string.IsNullOrEmpty(s.COMENTARIO) ? s.COMENTARIO.TrimEnd() : "SIN COMENTARIOS",
                        Compañia = s.ID_COMPANIA != null && s.ID_COMPANIA.HasValue ? (!string.IsNullOrEmpty(s.COMPANIA.DESCR) ? s.COMPANIA.DESCR.TrimEnd() : string.Empty) : string.Empty,
                        Descripcion = !string.IsNullOrEmpty(s.DESCR) ? s.DESCR.TrimEnd() : "SIN DESCRIPCIÓN",
                        Dosis = s.DOSIS != null && s.DOSIS.HasValue ? s.DOSIS.Value.ToString() : string.Empty,
                        Envoltorios = s.ENVOLTORIOS != null && s.ENVOLTORIOS.HasValue ? s.ENVOLTORIOS.ToString() : "0",
                        Fabricante = s.ID_COMPANIA != null && s.ID_FABRICANTE != null && s.ID_FABRICANTE.HasValue && s.ID_COMPANIA.HasValue ? (!string.IsNullOrEmpty(s.COMPANIA.DESCR) ? s.COMPANIA.DESCR.TrimEnd() : string.Empty) : "NO APLICA",
                        Id_Decomiso = s.ID_DECOMISO,
                        IMEI = !string.IsNullOrEmpty(s.IMEI) ? s.IMEI.TrimEnd() : string.Empty,
                        Modelo = s.DECOMISO_MODELO != null && s.ID_MODELO.HasValue ? (!string.IsNullOrEmpty(s.DECOMISO_MODELO.DESCR) ? s.DECOMISO_MODELO.DESCR.TrimEnd() : string.Empty) : string.Empty,
                        NumeroTelefono = !string.IsNullOrEmpty(s.TELEFONO) ? s.TELEFONO.TrimEnd() : string.Empty,
                        SerieNumero = !string.IsNullOrEmpty(s.SERIE) ? s.SERIE.TrimEnd() : string.Empty,
                        SimSerie = !string.IsNullOrEmpty(s.SIM_SERIE) ? s.SIM_SERIE.TrimEnd() : string.Empty,
                        TipoObjeto = !string.IsNullOrEmpty(s.OBJETO_TIPO.DESCR) ? s.OBJETO_TIPO.DESCR.TrimEnd() : string.Empty,
                        Unidad = s.ID_UNIDAD_MEDIDA != null && s.ID_UNIDAD_MEDIDA.HasValue && s.DROGA_UNIDAD_MEDIDA != null ? (!string.IsNullOrEmpty(s.DROGA_UNIDAD_MEDIDA.DESCR) ? s.DROGA_UNIDAD_MEDIDA.DESCR.TrimEnd() : "NO APLICA") : "NO APLICA"
                    }));
                    var consulta_decomiso = new cDecomiso();
                    foreach (var objeto in objetosDecomiso)
                    {
                        var decomiso = consulta_decomiso.Obtener(objeto.Id_Decomiso).FirstOrDefault();
                        if (!ingresosDecomiso.Any(a => a.Id_Decomiso == objeto.Id_Decomiso))
                            ingresosDecomiso.AddRange(decomiso.
                               DECOMISO_INGRESO.
                               Select(s => new cDecomisoCustodioIngreso()
                               {
                                   Celda = !string.IsNullOrEmpty(s.INGRESO.ID_UB_CELDA) ? s.INGRESO.ID_UB_CELDA.TrimEnd() : string.Empty,
                                   Edificio = s.INGRESO.ID_UB_EDIFICIO.HasValue ? s.INGRESO.ID_UB_EDIFICIO.Value.ToString() : string.Empty,
                                   Id_Anio = s.ID_ANIO,
                                   Id_Decomiso = s.ID_DECOMISO,
                                   Id_Imputado = s.ID_IMPUTADO,
                                   Materno = !string.IsNullOrEmpty(s.INGRESO.IMPUTADO.MATERNO) ? s.INGRESO.IMPUTADO.MATERNO.TrimEnd() : string.Empty,
                                   Nombre = !string.IsNullOrEmpty(s.INGRESO.IMPUTADO.NOMBRE) ? s.INGRESO.IMPUTADO.NOMBRE.TrimEnd() : string.Empty,
                                   Paterno = !string.IsNullOrEmpty(s.INGRESO.IMPUTADO.PATERNO) ? s.INGRESO.IMPUTADO.PATERNO.TrimEnd() : string.Empty,
                                   Sector = s.INGRESO.ID_UB_SECTOR.HasValue ? s.INGRESO.ID_UB_SECTOR.Value.ToString() : string.Empty
                               }));

                        if (!visitantesDecomiso.Any(a => a.Id_Decomiso == objeto.Id_Decomiso))
                            visitantesDecomiso.AddRange(decomiso.
                                DECOMISO_PERSONA.Where(w => w.ID_TIPO_PERSONA == VISITA).
                                Select(s => new cDecomisoCustodioVisitante()
                                {
                                    Discapacitado = s.PERSONA.ID_TIPO_DISCAPACIDAD.HasValue ? "SI" : "NO",//(!string.IsNullOrEmpty(s.PERSONA.TIPO_DISCAPACIDAD.DESCR) ? s.PERSONA.TIPO_DISCAPACIDAD.DESCR.TrimEnd() : string.Empty) : string.Empty,
                                    Estatus = s.PERSONA.VISITANTE.ID_ESTATUS_VISITA.HasValue ? (!string.IsNullOrEmpty(s.PERSONA.VISITANTE.ESTATUS_VISITA.DESCR) ? s.PERSONA.VISITANTE.ESTATUS_VISITA.DESCR.TrimEnd() : "SIN DEFINIR") : "SIN DEFINIR",
                                    FechaRegistro = s.PERSONA.VISITANTE.FEC_ALTA.HasValue ? (string.Format("{0}/{1}/{2}", s.PERSONA.VISITANTE.FEC_ALTA.Value.Day, s.PERSONA.VISITANTE.FEC_ALTA.Value.Month, s.PERSONA.VISITANTE.FEC_ALTA.Value.Year)) : string.Empty,
                                    Id_Decomiso = s.ID_DECOMISO,
                                    Materno = !string.IsNullOrEmpty(s.PERSONA.MATERNO) ? s.PERSONA.MATERNO.TrimEnd() : string.Empty,
                                    Paterno = !string.IsNullOrEmpty(s.PERSONA.PATERNO) ? s.PERSONA.PATERNO.TrimEnd() : string.Empty,
                                    Nombre = !string.IsNullOrEmpty(s.PERSONA.NOMBRE) ? s.PERSONA.NOMBRE.TrimEnd() : string.Empty,
                                    Sexo = !string.IsNullOrEmpty(s.PERSONA.SEXO) ? (s.PERSONA.SEXO.TrimEnd() == "F" ? "FEMENINO" : "MASCULINO") : string.Empty,
                                    TipoVisitante = "FAMILIAR" //TODO: Pendiente?
                                }));

                    }


                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        Reporte.LocalReport.ReportPath = "Reportes/rDecomisoObjetos.rdlc";
                        ReportDataSource ReportDataSource_Encabezado = new ReportDataSource();
                        ReportDataSource_Encabezado.Name = "DataSet1";
                        ReportDataSource_Encabezado.Value = datosReporte;

                        ReportDataSource ReportDataSource = new ReportDataSource();
                        ReportDataSource.Name = "DataSet2";
                        ReportDataSource.Value = objetosDecomiso;

                        Reporte.LocalReport.DataSources.Add(ReportDataSource_Encabezado);
                        Reporte.LocalReport.DataSources.Add(ReportDataSource);


                        Reporte.LocalReport.SubreportProcessing += (s, e) =>
                        {
                            if (e.ReportPath.Equals("srDecomisoIngreso", StringComparison.InvariantCultureIgnoreCase))
                            {
                                ReportDataSource rds2 = new ReportDataSource("DataSet1", ingresosDecomiso);
                                e.DataSources.Add(rds2);
                            }

                            if (e.ReportPath.Equals("srDecomisoVisitante", StringComparison.InvariantCultureIgnoreCase))
                            {
                                ReportDataSource rds4 = new ReportDataSource("DataSet1", visitantesDecomiso);
                                e.DataSources.Add(rds4);
                            }


                        };

                        //Reporte.LocalReport.SetParameters(new ReportParameter("FechaInicial", string.Format("{0}/{1}/{2}", SelectedFechaInicial.Day, SelectedFechaInicial.Month, SelectedFechaInicial.Year)));
                        //Reporte.LocalReport.SetParameters(new ReportParameter("FechaFinal", string.Format("{0}/{1}/{2}", SelectedFechaFinal.Day, SelectedFechaFinal.Month, SelectedFechaFinal.Year)));


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

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_DECOMISO_OBJETO.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
