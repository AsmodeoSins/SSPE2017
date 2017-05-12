using Microsoft.Reporting.WinForms;
using SSP.Controlador.Catalogo.Justicia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlPenales
{
    public partial class ReporteDecomisoCustodioViewModel : ValidationViewModelBase
    {
        public async void OnLoad(ReporteDecomisoCustodioView Window)
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
                        MostrarTodos = false;
                    }));
                });
                ReportViewerVisible = Visibility.Visible;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la pantalla", ex);
            }
        }

        public void ClickSwitch(object obj)
        {
            switch (obj.ToString())
            {
                case "Busqueda":
                    if(!pConsultar)
                    {
                        ReportViewerVisible = Visibility.Collapsed;
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    GenerarReporte();
                    break;
            }
        }

        public async void GenerarReporte()
        {
            var datosReporte = new List<cReporteDatos>();
            var custodiosDecomiso = new List<cDecomisoCustodio>();
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


                    var Parametro_Custodios_IDs_Empleado = Parametro.ID_TIPO_EMPLEADO_CUSTODIO;
                    var IDs_Custodios_Empleado = new int[Parametro_Custodios_IDs_Empleado.Count()];
                    for (int i = 0; i < Parametro_Custodios_IDs_Empleado.Count(); i++)
                    {

                        if (!int.TryParse(Parametro_Custodios_IDs_Empleado[i], out  IDs_Custodios_Empleado[i]))
                            IDs_Custodios_Empleado[i] = 0;
                    }



                    var decomisos_custodios = MostrarTodos ? new cDecomisoPersona().
                        ObtenerDecomisosPorCustodios(IDs_Custodios_Empleado, null, string.Empty, string.Empty, string.Empty).
                        Select(s => new { s.ID_DECOMISO }).Distinct().OrderBy(o => o.ID_DECOMISO).ToList() : new cDecomisoPersona().
                        ObtenerDecomisosPorCustodios(IDs_Custodios_Empleado, ID_Persona, NombreCustodioBuscar, PaternoCustodioBuscar, MaternoCustodioBuscar).
                        Select(s => new { s.ID_DECOMISO }).Distinct().OrderBy(o => o.ID_DECOMISO).ToList();


                    var consulta_decomiso = new cDecomiso();
                    foreach (var id_decomiso in decomisos_custodios)
                    {
                        var decomiso = consulta_decomiso.Obtener(id_decomiso.ID_DECOMISO).FirstOrDefault();
                        decomisos.Add(new cDecomisoFecha()
                        {
                            Area = decomiso.ID_AREA.HasValue ? (!string.IsNullOrEmpty(decomiso.AREA.DESCR) ? decomiso.AREA.DESCR.TrimEnd() : "SIN DEFINIR") : "N/A",
                            Celda = !string.IsNullOrEmpty(decomiso.ID_CELDA) ? decomiso.ID_CELDA.TrimEnd() : "SIN DEFINIR",
                            Sector = (!string.IsNullOrEmpty(decomiso.ID_CELDA) && decomiso.ID_SECTOR.HasValue) ? (!string.IsNullOrEmpty(decomiso.CELDA.SECTOR.DESCR) ? decomiso.CELDA.SECTOR.DESCR.TrimEnd() : "SIN DEFINIR") : "SIN DEFINIR",
                            Edificio = (!string.IsNullOrEmpty(decomiso.ID_CELDA) && decomiso.ID_SECTOR.HasValue && decomiso.ID_EDIFICIO.HasValue) ? (!string.IsNullOrEmpty(decomiso.CELDA.SECTOR.EDIFICIO.DESCR) ? decomiso.CELDA.SECTOR.EDIFICIO.DESCR.TrimEnd() : "SIN DEFINIR") : "SIN DEFINIR",
                            FechaEvento = decomiso.EVENTO_FEC.Value,
                            GrupoTactico = decomiso.ID_GRUPO_TACTICO.HasValue ? (!string.IsNullOrEmpty(decomiso.GRUPO_TACTICO.DESCR) ? decomiso.GRUPO_TACTICO.DESCR.TrimEnd() : "SIN DEFINIR") : "SIN DEFINIR",
                            Id_Decomiso = decomiso.ID_DECOMISO,
                            NumeroOficio = !string.IsNullOrEmpty(decomiso.OFICIO) ? decomiso.OFICIO.TrimEnd() : "SIN DEFINIR",
                            Turno = decomiso.ID_TURNO.HasValue ? (!string.IsNullOrEmpty(decomiso.TURNO.DESCR) ? decomiso.TURNO.DESCR.TrimEnd() : "SIN DEFINIR") : "SIN DEFINIR"
                        });

                        custodiosDecomiso.AddRange(decomiso.
                            DECOMISO_PERSONA.Where(w => w.ID_TIPO_PERSONA == EMPLEADO).
                            Select(s => new cDecomisoCustodio()
                            {
                                Id_Decomiso = decomiso.ID_DECOMISO,
                                Grupo_Tactico = decomiso.ID_GRUPO_TACTICO.HasValue ? (!string.IsNullOrEmpty(decomiso.GRUPO_TACTICO.DESCR) ? decomiso.GRUPO_TACTICO.DESCR.TrimEnd() : "SIN DEFINIR") : "SIN DEFINIR",
                                Id_Grupo_Tactico = decomiso.ID_GRUPO_TACTICO.HasValue ? decomiso.ID_GRUPO_TACTICO.Value : 0,
                                Id_Persona = s.ID_PERSONA,
                                Id_Tipo_Empleado = s.PERSONA.EMPLEADO.ID_TIPO_EMPLEADO.Value,
                                Paterno = !string.IsNullOrEmpty(s.PERSONA.PATERNO) ? s.PERSONA.PATERNO.TrimEnd() : string.Empty,
                                Materno = !string.IsNullOrEmpty(s.PERSONA.MATERNO) ? s.PERSONA.MATERNO.TrimEnd() : string.Empty,
                                Nombre = !string.IsNullOrEmpty(s.PERSONA.NOMBRE) ? s.PERSONA.NOMBRE.TrimEnd() : string.Empty,
                            }));

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

                        objetosDecomiso.AddRange(decomiso.
                            DECOMISO_OBJETO.
                            Select(s => new cDecomisoCustodioObjeto()
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
                        Reporte.LocalReport.ReportPath = "Reportes/rDecomisoCustodio.rdlc";
                        ReportDataSource ReportDataSource_Encabezado = new ReportDataSource();
                        ReportDataSource_Encabezado.Name = "DataSet1";
                        ReportDataSource_Encabezado.Value = datosReporte;

                        ReportDataSource ReportDataSource = new ReportDataSource();
                        ReportDataSource.Name = "DataSet2";
                        ReportDataSource.Value = decomisos;

                        Reporte.LocalReport.DataSources.Add(ReportDataSource_Encabezado);
                        Reporte.LocalReport.DataSources.Add(ReportDataSource);


                        Reporte.LocalReport.SubreportProcessing += (s, e) =>
                        {
                            if (e.ReportPath.Equals("srDecomisoCustodios", StringComparison.InvariantCultureIgnoreCase))
                            {
                                ReportDataSource rds1 = new ReportDataSource("DataSet1", custodiosDecomiso);
                                e.DataSources.Add(rds1);
                            }

                            if (e.ReportPath.Equals("srDecomisoIngreso", StringComparison.InvariantCultureIgnoreCase))
                            {
                                ReportDataSource rds2 = new ReportDataSource("DataSet1", ingresosDecomiso);
                                e.DataSources.Add(rds2);
                            }

                            if (e.ReportPath.Equals("srDecomisoObjeto", StringComparison.InvariantCultureIgnoreCase))
                            {
                                ReportDataSource rds3 = new ReportDataSource("DataSet1", objetosDecomiso);
                                e.DataSources.Add(rds3);
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
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_DECOMISO_CUSTODIO.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
