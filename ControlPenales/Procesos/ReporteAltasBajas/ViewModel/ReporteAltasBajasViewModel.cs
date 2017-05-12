using ControlPenales.Clases.ControlInternos;
using Microsoft.Reporting.WinForms;
using SSP.Controlador.Catalogo.Justicia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlPenales
{
    public partial class ReporteAltasBajasViewModel : ValidationViewModelBase
    {
        #region[METODOS]
        private async void ClickSwitch(object obj)
        {
            try
            {
                switch (obj.ToString())
                {
                    case "ordenar":
                        break;
                    case "generar":
                        if (!pConsultar)
                        {
                            Repositorio.Visibility = Visibility.Collapsed;
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                            break;
                        }
                        if (!base.HasErrors)
                        {
                            if (FechaInicio.Value.Date <= FechaFin.Value.Date)
                            {
                                await StaticSourcesViewModel.CargarDatosMetodoAsync(GenerarReporte);
                            }
                            else
                            {
                                Repositorio.Visibility = Visibility.Collapsed;
                                new Dialogos().ConfirmacionDialogo("Validación", "La fecha de inicio debe ser mayor o igual a la fecha de fin.");
                            }
                                
                        }
                        else 
                        {
                            Repositorio.Visibility = Visibility.Collapsed;
                            new Dialogos().ConfirmacionDialogo("Validación", base.Error);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
            }
        }

        private void GenerarReporte()
        {
            try
            {

                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    Reporte.Reset();
                    Repositorio.Visibility = Visibility.Collapsed;
                }));
                var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();
                var datosReporte = new List<cReporteDatos>();
                datosReporte.Add(new cReporteDatos()
                    {
                        Encabezado1 = Parametro.ENCABEZADO1,
                        Encabezado2 = Parametro.ENCABEZADO2,
                        Centro = centro.DESCR.Trim(),
                        Titulo = string.Format("{0} DEL {1} AL {2}", Altas ? "ALTAS" : "BAJAS", FechaInicio.Value.ToString("dd/MM/yyyy"), FechaFin.Value.ToString("dd/MM/yyyy")),
                        Logo1 = Parametro.REPORTE_LOGO1,
                        Logo2 = Parametro.REPORTE_LOGO2
                    });

                //Obtenemos el listado de personas
                var lista = new cIngreso().ObtenerAltasBajas(FechaInicio, FechaFin, Altas ? 1 : 2).Select(w => new cReporteAltasBajas() { 
                    Id_Anio = w.ID_ANIO,
                    Id_Imputado = w.ID_IMPUTADO,
                    Id_Ingreso = w.ID_INGRESO,
                    Edificio = w.CAMA.CELDA.SECTOR.EDIFICIO.DESCR,
                    Sector = w.CAMA.CELDA.SECTOR.DESCR,
                    Id_Celda = w.CAMA.CELDA.ID_CELDA,
                    Id_Cama = w.ID_UB_CAMA,
                    Nombre = w.IMPUTADO.NOMBRE,
                    Paterno = w.IMPUTADO.PATERNO,
                    Materno = w.IMPUTADO.MATERNO,
                    Anio_Gob = w.ANIO_GOBIERNO.HasValue ? w.ANIO_GOBIERNO.Value : 0,
                    Folio_Gob = w.FOLIO_GOBIERNO,
                    Fecha = Altas ? w.FEC_INGRESO_CERESO.Value : w.FEC_SALIDA_CERESO.Value
                });

                Reporte.LocalReport.ReportPath = "Reportes/rAltasBajas.rdlc";
                Reporte.LocalReport.DataSources.Clear();

                ReportDataSource rds1 = new ReportDataSource();
                rds1.Name = "DataSet3";
                rds1.Value = datosReporte;
                Reporte.LocalReport.DataSources.Add(rds1);

                ReportDataSource rds2 = new ReportDataSource();
                rds2.Name = "DataSet1";
                rds2.Value = lista;
                Reporte.LocalReport.DataSources.Add(rds2);

                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    Reporte.RefreshReport();
                    Repositorio.Visibility = Visibility.Visible;
                }));

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar reporte.", ex);
            }
        }

        private void ReporteControlInternoLoad(ReporteAltasBajasView window)
        {
            try
            {
                ConfiguraPermisos();
                Repositorio = window.WFH;
                Reporte = window.Report;
                Validaciones();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar pantalla.", ex);
            }
        }
        #endregion

        #region Validaciones
        void Validaciones()
        {
            try
            {
                base.ClearRules();
                base.AddRule(() => FechaInicio, () => FechaInicio.HasValue, "Fecha de incio es requerida");
                base.AddRule(() => FechaFin, () => FechaFin.HasValue, "Fecha de fin es requerida");
                OnPropertyChanged("FechaInicio");
                OnPropertyChanged("FechaFin");                    
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar validaciones.", ex);
            }
        }
        #endregion

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_ALTAS_BAJAS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
