using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Reporting.WinForms;
using System.Windows.Controls;
using LinqKit;

namespace ControlPenales
{
    partial class SolicitudesAtencionPorEstatusViewModel:FingerPrintScanner
    {
        cAtencionIngreso AtencionIngresoControlador = new cAtencionIngreso();
        cAtencionCita AtencionCitaControlador= new cAtencionCita();
        Usuario User = StaticSourcesViewModel.UsuarioLogin;
        UserControl WindowControl = new UserControl();
        private async void clickSwitch(object op)
        {

            switch (op.ToString())
            {

                case "buscar":
                    try
                    {


                        if (SelectEstatus.Value!=-1)
                        {
                            if (!ValidacionFechas())
                            {
                                await StaticSourcesViewModel.CargarDatosMetodoAsync(GenerarReporte);
                            }

                            
                        }
                        else
                        {
                            StaticSourcesViewModel.Mensaje("Aviso", "Debe de seleccionar un Estatus", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION, 5);
                        }
                    
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al Generar Reporte", ex);
                     
                    }
                    break;
                case "limpiar_menu":
                    StaticSourcesViewModel.SourceChanged = false;
                     ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new SolicitudesAtencionPorEstatusView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.SolicitudesAtencionPorEstatusViewModel();
                    break;
                case "salir_menu":
                     PrincipalViewModel.SalirMenu();
                    StaticSourcesViewModel.SourceChanged = false;
                    
                    break;
                    
                    
            }
        }
    
        private void GenerarReporte()
        {
            ///var CerifImputado = SelectIngreso.IMPUTADO;
            ///
     
          

            #region Iniciliza el entorno para mostrar el reporte al usuario
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
            {
                var ReporteView_ = (ReportViewer)WindowControl.FindName("Report");

                //PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                //View.Owner = PopUpsViewModels.MainWindow;
                //View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };


                //View.Show();
            #endregion

                #region Se forma el encabezado del reporte
                var Encabezado = new cEncabezado();
                Encabezado.TituloUno = "GOBIERNO DEL ESTADO  DE BAJA CALIFORNIA";
                Encabezado.TituloDos = "SUBSECRETARÍA DEL SISTEMA ESTATAL PENITENCIARIO";
                Encabezado.NombreReporte = "SOLICITUDES DE ATENCIÓN POR ESTATUS";
                Encabezado.ImagenIzquierda = Parametro.LOGO_ESTADO;
                Encabezado.ImagenFondo = Parametro.REPORTE_LOGO2;

                #endregion

                #region Llenar Informacion
      
                var LstDatosBusqueda = BuscarResultado();
                int Count = LstDatosBusqueda.Count();
                
                var LstDatosFiltro = Count > 0 ? LstDatosBusqueda.OrderBy(o => o.ID_ANIO).ThenBy(then => then.ID_IMPUTADO).ThenBy(then => then.ATENCION_SOLICITUD.SOLICITUD_FEC) : null;
                
                var listaDatosReporte = new List<cReporteSolicitudPorEstatus>();
                if (Count == 0)
                {
                    ReporteView_.Clear();
                    StaticSourcesViewModel.Mensaje("Aviso", "La Busqueda no contiene Información", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION, 5);
                }
                else
                {
                    int cant = 0;


                    foreach (var itemAtencionIngreso in LstDatosFiltro.Select(s => new { s.ID_IMPUTADO, s.ID_ANIO, s.ID_CENTRO, s.ID_INGRESO, s.ATENCION_SOLICITUD.ACTIVIDAD, s.INGRESO.IMPUTADO.PATERNO, s.INGRESO.IMPUTADO.MATERNO, s.INGRESO.IMPUTADO.NOMBRE, s.ATENCION_SOLICITUD.SOLICITUD_FEC, s.ESTATUS, AREA_TECNICA_DESCR = s.ATENCION_SOLICITUD.AREA_TECNICA.DESCR, FECHA_SOLICITUD = (DateTime?)s.ATENCION_SOLICITUD.SOLICITUD_FEC, s.ID_ATENCION}))
                    {
                       
                        //-----------------------------------------VERIFICA SI TIENE SOLICITUD_CITA--------------------------------------------------
                        var querySolCitas = AtencionCitaControlador.ObtenerTodo().Where(w => w.ID_ANIO == itemAtencionIngreso.ID_ANIO && w.ID_CENTRO == itemAtencionIngreso.ID_CENTRO && w.ID_IMPUTADO == itemAtencionIngreso.ID_IMPUTADO && w.ID_INGRESO == itemAtencionIngreso.ID_INGRESO && w.ID_ATENCION == itemAtencionIngreso.ID_ATENCION).ToList();
                        cant++;
                        var DatosReporte = new cReporteSolicitudPorEstatus();
                        // *************Se Filtra Agrupacion ESTATUS-AREA TECNICA **********
                        string ESTAUS = itemAtencionIngreso.ESTATUS == 1 ? "ATENDIDA" : "NO ATENDIDA";
                        DatosReporte.GrupoEstatus_AreaTecn = ESTAUS + "-" + itemAtencionIngreso.AREA_TECNICA_DESCR;
                        DatosReporte.Estado = SelectEstatus == -1 ? "TODOS" : LstEstatus.Where(W => W.ID_ESTATUS == SelectEstatus).FirstOrDefault().DESCR;
                          System.Text.StringBuilder sb = new System.Text.StringBuilder ();
                          sb.Append(itemAtencionIngreso.ID_ANIO.ToString());
                          sb.Append("/");
                          sb.AppendLine(itemAtencionIngreso.ID_IMPUTADO.ToString());
                          DatosReporte.Anio = sb.ToString();
                        //itemAtencionIngreso.ID_ANIO.ToString() + "/" + itemAtencionIngreso.ID_IMPUTADO.ToString();
                        DatosReporte.Actividad = itemAtencionIngreso.ACTIVIDAD;
                        string Paterno = !string.IsNullOrEmpty(itemAtencionIngreso.PATERNO) ? itemAtencionIngreso.PATERNO.Trim() : "";
                        string Materno = !string.IsNullOrEmpty(itemAtencionIngreso.MATERNO) ? itemAtencionIngreso.MATERNO.Trim() : "";
                        string Nombre = !string.IsNullOrEmpty(itemAtencionIngreso.NOMBRE) ? itemAtencionIngreso.NOMBRE.Trim() : "";
                        DatosReporte.NombreImputado = Paterno + " " + Paterno + " " + Materno;
                        DatosReporte.FechaCita = "";
                        DatosReporte.FechaSolicitud = itemAtencionIngreso.SOLICITUD_FEC != null ? itemAtencionIngreso.SOLICITUD_FEC.Value.ToShortDateString() : "";
                        // var datoscita = AtencionCitaControlador.ObtenerTodo().Where(w => w.ID_ANIO == itemAtencionIngreso.ID_ANIO && w.ID_CENTRO == itemAtencionIngreso.ID_CENTRO && w.ID_IMPUTADO == itemAtencionIngreso.ID_IMPUTADO && w.ID_INGRESO == itemAtencionIngreso.ID_INGRESO && w.ID_ATENCION == itemAtencionIngreso.ID_ATENCION);

                     
                        DatosReporte.Total = Count.ToString();
                    
                        if (querySolCitas.Count() > 0 && itemAtencionIngreso.ESTATUS==1)//SI TIENE ESTATUS 1 busca la fecha ya que ya tiene cita 
                        {
                            DateTime? FECHA_CITA = querySolCitas.FirstOrDefault().CITA_FECHA_HORA;
                            DatosReporte.FechaCita = FECHA_CITA != null ? FECHA_CITA.Value.ToShortDateString() : "";
                        }
                        listaDatosReporte.Add(DatosReporte);
                    }
                    // ------------------TOTAL ------------------------------------

                    if (cant==0)
                    {
                        ReporteView_.Clear();
                        StaticSourcesViewModel.Mensaje("Aviso", "La Busqueda no contiene Información", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION, 5);
                    }
                    else
                    {
                        #region Inicializacion de reporte


                        ReporteView_.LocalReport.ReportPath = "Reportes/rSolicitudesAtencionPorEstatus.rdlc";
                        ReporteView_.LocalReport.DataSources.Clear();
                        #endregion


                        #region DATOS REPORTE TABLA

                        Microsoft.Reporting.WinForms.ReportDataSource rds = new Microsoft.Reporting.WinForms.ReportDataSource();
                        rds.Name = "DataSet1";
                        rds.Value = listaDatosReporte;
                        ReporteView_.LocalReport.DataSources.Add(rds);
                        #endregion

                        #region DATOS REPORTE ENCABEZADO REPORTE
                        var dsEncabezado = new List<cEncabezado>();
                        dsEncabezado.Add(Encabezado);
                        Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                        rds2.Name = "DataSet2";
                        rds2.Value = dsEncabezado;
                        ReporteView_.LocalReport.DataSources.Add(rds2);
                        #endregion

                      

                        ReporteView_.RefreshReport();
                    }
                #endregion
                   
                }
            }));
         
            
        }

        private bool ValidacionFechas()
        {
            
            if (FFechaInicio > FFechaFin)
            {
                StaticSourcesViewModel.Mensaje("Aviso", "La Fecha Inicial debe ser menor a la fecha Final", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION, 5);
             return   true;
            }
            
            if (FFechaFin < FFechaInicio)
            {
                StaticSourcesViewModel.Mensaje("Aviso", "La Fecha Final debe ser mayor a la fecha Inicial", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION, 5);
               return true;
            }

            return false;

        }
        private IQueryable<ATENCION_INGRESO> BuscarResultado()
        {
           return AtencionIngresoControlador.ObtenerSolicitudAtencionEstatus(GlobalVar.gCentro, FFechaInicio, FFechaFin, SelectEstatus.Value);
        }

        private async void OnLoad(SolicitudesAtencionPorEstatusView obj = null)

        {
            try
            {
                WindowControl = (UserControl)obj;

                await StaticSourcesViewModel.CargarDatosMetodoAsync(CargarListas);

               

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar decomisos", ex);
            }
        }

        private void CargarListas()
        {
            var LstEstatus_ = new List<Estatus>();
            LstEstatus_.Add(new Estatus() { ID_ESTATUS = -1, DESCR = "SELECCIONE" });
            LstEstatus_.Add(new Estatus() { ID_ESTATUS = 1, DESCR = "SOLICITADA" });
            LstEstatus_.Add(new Estatus() { ID_ESTATUS = 2, DESCR = "ATENDIDA" });
            LstEstatus_.Add(new Estatus() { ID_ESTATUS = 3, DESCR = "NO ATENDIDA" });
            LstEstatus = new ObservableCollection<Estatus>(LstEstatus_);
        }

      

    }
}
