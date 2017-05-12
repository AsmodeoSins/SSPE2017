using Microsoft.Reporting.WinForms;
using SSP.Controlador.Catalogo.Justicia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlPenales
{
    public partial class ReporteDecomisoFechaViewModel : ValidationViewModelBase
    {
        public async void OnLoad(ReporteDecomisoFechaView Window)
        {
            try
            {
                ConfiguraPermisos();
                Ventana = Window;
                Reporte = Ventana.Report;
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ReportViewerVisible = Visibility.Collapsed;
                    }));
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
                    if (!pConsultar)
                    {
                        ReportViewerVisible = Visibility.Collapsed;
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    Reporte.Reset();
                    GenerarReporte();
                    break;
            }

        }

        public async void GenerarReporte()
        {
            var datosReporte = new List<cReporteDatos>();
            var lDecomisos = new List<cDecomisoFecha>();
            var lObjetos_Decomiso = new List<cDecomisoFechaObjeto>();
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
                    var decomisos = new cDecomiso().
                        ObtenerTodosPorFecha(SelectedFechaInicial, ConRangoEnabled, SelectedFechaFinal);
                    lDecomisos = new cDecomiso().
                        ObtenerTodosPorFecha(SelectedFechaInicial, ConRangoEnabled, SelectedFechaFinal).
                        Select(s =>
                            new cDecomisoFecha()
                            {
                                Area = s.ID_AREA.HasValue ? (!string.IsNullOrEmpty(s.AREA.DESCR) ? s.AREA.DESCR.TrimEnd() : "SIN DEFINIR") : "N/A",
                                Celda = !string.IsNullOrEmpty(s.ID_CELDA) ? s.ID_CELDA.TrimEnd() : "SIN DEFINIR",
                                Sector = (!string.IsNullOrEmpty(s.ID_CELDA) && s.ID_SECTOR.HasValue) ? (!string.IsNullOrEmpty(s.CELDA.SECTOR.DESCR) ? s.CELDA.SECTOR.DESCR.TrimEnd() : "SIN DEFINIR") : "SIN DEFINIR",
                                Edificio = (!string.IsNullOrEmpty(s.ID_CELDA) && s.ID_SECTOR.HasValue && s.ID_EDIFICIO.HasValue) ? (!string.IsNullOrEmpty(s.CELDA.SECTOR.EDIFICIO.DESCR) ? s.CELDA.SECTOR.EDIFICIO.DESCR.TrimEnd() : "SIN DEFINIR") : "SIN DEFINIR",
                                FechaEvento = s.EVENTO_FEC.Value,
                                GrupoTactico = s.ID_GRUPO_TACTICO.HasValue ? (!string.IsNullOrEmpty(s.GRUPO_TACTICO.DESCR) ? s.GRUPO_TACTICO.DESCR.TrimEnd() : "SIN DEFINIR") : "SIN DEFINIR",
                                Id_Decomiso = s.ID_DECOMISO,
                                NumeroOficio = !string.IsNullOrEmpty(s.OFICIO) ? s.OFICIO.TrimEnd() : "SIN DEFINIR",
                                Turno = s.ID_TURNO.HasValue ? (!string.IsNullOrEmpty(s.TURNO.DESCR) ? s.TURNO.DESCR.TrimEnd() : "SIN DEFINIR") : "SIN DEFINIR"
                            }).
                            ToList();


                    foreach (var lista_decomiso in decomisos)
                    {
                        
                        var cantidad_objetos = lista_decomiso.DECOMISO_OBJETO.Count;
                        lObjetos_Decomiso.AddRange(
                            lista_decomiso.
                            DECOMISO_OBJETO.
                            Select(s =>
                                new cDecomisoFechaObjeto()
                                {
                                    Cantidad = cantidad_objetos,
                                    Descripcion = !string.IsNullOrEmpty(s.DESCR) ? s.DESCR.TrimEnd() : string.Empty,
                                    Id_Consec = s.ID_CONSEC,
                                    Id_Decomiso = s.ID_DECOMISO,
                                    Id_Tipo_Objeto = s.ID_OBJETO_TIPO,
                                    TipoObjeto = !string.IsNullOrEmpty(s.OBJETO_TIPO.DESCR) ? s.OBJETO_TIPO.DESCR.TrimEnd() : string.Empty
                                }).
                            ToList());
                    }

                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        Reporte.LocalReport.ReportPath = "Reportes/rDecomisoFecha.rdlc";
                        ReportDataSource ReportDataSource_Encabezado = new ReportDataSource();
                        ReportDataSource_Encabezado.Name = "DataSet1";
                        ReportDataSource_Encabezado.Value = datosReporte;

                        ReportDataSource ReportDataSource = new ReportDataSource();
                        ReportDataSource.Name = "DataSet2";
                        ReportDataSource.Value = lDecomisos;

                        Reporte.LocalReport.DataSources.Add(ReportDataSource_Encabezado);
                        Reporte.LocalReport.DataSources.Add(ReportDataSource);


                        Reporte.LocalReport.SubreportProcessing += (s, e) =>
                        {
                            if (e.ReportPath.Equals("srDecomisoFechaObjetos", StringComparison.InvariantCultureIgnoreCase))
                            {
                                ReportDataSource rds1 = new ReportDataSource("DataSet1", lObjetos_Decomiso);
                                e.DataSources.Add(rds1);
                            }
                        };

                        Reporte.LocalReport.SetParameters(new ReportParameter("FechaInicial", string.Format("{0}/{1}/{2}", SelectedFechaInicial.Day, SelectedFechaInicial.Month, SelectedFechaInicial.Year)));
                        Reporte.LocalReport.SetParameters(new ReportParameter("FechaFinal", string.Format("{0}/{1}/{2}", SelectedFechaFinal.Day, SelectedFechaFinal.Month, SelectedFechaFinal.Year)));

                        ReportViewerVisible = Visibility.Visible;
                        Reporte.Refresh();
                        Reporte.RefreshReport();
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
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_DECOMISOS_FECHA.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
