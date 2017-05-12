using ControlPenales.Clases;
using Microsoft.Reporting.WinForms;
using SSP.Controlador.Catalogo.Justicia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlPenales
{
    public partial class ReporteDecomisoInvolucradoViewModel : ValidationViewModelBase
    {
        public async void OnLoad(ReporteDecomisoInvolucradoView Window)
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

                    var reporte = new List<cReporteDecomisoInvolucrados>();
                    var lista = new cDecomisoObjeto().ObtenerTodos(23);
                    if (lista != null)
                    {
                        foreach (var w in lista)
                        {
                            if (w.DECOMISO.DECOMISO_PERSONA != null)
                            {
                                foreach (var p in w.DECOMISO.DECOMISO_PERSONA.Where(x => x.OFICIAL_A_CARGO == "N"))
                                {
                                    reporte.Add(new cReporteDecomisoInvolucrados() { 
                                     Fecha = string.Format("{0:dd/MM/yyyy}",w.DECOMISO.EVENTO_FEC),
                                     Involucrado = p.TIPO_PERSONA.DESCR,
                                     Control = p.ID_PERSONA.ToString(),
                                     Nombre = string.Format("{0} {1} {2}",
                                        p.PERSONA.NOMBRE.Trim(),
                                        !string.IsNullOrEmpty(p.PERSONA.PATERNO) ? p.PERSONA.PATERNO.Trim() : string.Empty,
                                        !string.IsNullOrEmpty(p.PERSONA.MATERNO) ? p.PERSONA.MATERNO.Trim() : string.Empty),
                                     Droga = w.DROGA.DESCR,
                                     Cantidad = w.CANTIDAD.ToString(),
                                     //Presentacion = w.ENVOLTORIOS,
                                     Dosis = w.DOSIS.Value.ToString(),
                                    });
                                }
                            }
                            if (w.DECOMISO.DECOMISO_INGRESO != null)
                            {
                                foreach (var i in w.DECOMISO.DECOMISO_INGRESO)
                                {
                                    reporte.Add(new cReporteDecomisoInvolucrados()
                                    {
                                        Fecha = string.Format("{0:dd/MM/yyyy}", w.DECOMISO.EVENTO_FEC),
                                        Involucrado = "INTERNO",
                                        Control = string.Format("{0}/{1}",i.ID_ANIO,i.ID_IMPUTADO),
                                        Nombre = string.Format("{0} {1} {2}",
                                           i.INGRESO.IMPUTADO.NOMBRE.Trim(),
                                           !string.IsNullOrEmpty(i.INGRESO.IMPUTADO.PATERNO) ? i.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty,
                                           !string.IsNullOrEmpty(i.INGRESO.IMPUTADO.MATERNO) ? i.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty),
                                        Droga = w.DROGA.DESCR,
                                        Cantidad = w.CANTIDAD.ToString(),
                                        //Presentacion = w.ENVOLTORIOS,
                                        Dosis = w.DOSIS.Value.ToString(),
                                    });
                                }
                            }
                        }
                    }
                    
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        Reporte.LocalReport.ReportPath = "Reportes/rDecomisoInvolucrado.rdlc";
                      
                        ReportDataSource ReportDataSource = new ReportDataSource();
                        ReportDataSource.Name = "DataSet1";
                        ReportDataSource.Value = reporte;
                        Reporte.LocalReport.DataSources.Add(ReportDataSource);

                        ReportDataSource ReportDataSource_Encabezado = new ReportDataSource();
                        ReportDataSource_Encabezado.Name = "DataSet2";
                        ReportDataSource_Encabezado.Value = datosReporte;
                        Reporte.LocalReport.DataSources.Add(ReportDataSource_Encabezado);
                        #region comentado
                        //Reporte.LocalReport.ReportPath = "Reportes/rDecomisoInvolucrado.rdlc";
                        //ReportDataSource ReportDataSource_Encabezado = new ReportDataSource();
                        //ReportDataSource_Encabezado.Name = "DataSet1";
                        //ReportDataSource_Encabezado.Value = datosReporte;

                        //ReportDataSource ReportDataSource = new ReportDataSource();
                        //ReportDataSource.Name = "DataSet2";
                        //ReportDataSource.Value = lDecomisos;

                        //Reporte.LocalReport.DataSources.Add(ReportDataSource_Encabezado);
                        //Reporte.LocalReport.DataSources.Add(ReportDataSource);


                        //Reporte.LocalReport.SubreportProcessing += (s, e) =>
                        //{
                        //    if (e.ReportPath.Equals("srDecomisoFechaObjetos", StringComparison.InvariantCultureIgnoreCase))
                        //    {
                        //        ReportDataSource rds1 = new ReportDataSource("DataSet1", lObjetos_Decomiso);
                        //        e.DataSources.Add(rds1);
                        //    }
                        //};

                        //Reporte.LocalReport.SetParameters(new ReportParameter("FechaInicial", string.Format("{0}/{1}/{2}", SelectedFechaInicial.Day, SelectedFechaInicial.Month, SelectedFechaInicial.Year)));
                        //Reporte.LocalReport.SetParameters(new ReportParameter("FechaFinal", string.Format("{0}/{1}/{2}", SelectedFechaFinal.Day, SelectedFechaFinal.Month, SelectedFechaFinal.Year)));
                        #endregion
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
