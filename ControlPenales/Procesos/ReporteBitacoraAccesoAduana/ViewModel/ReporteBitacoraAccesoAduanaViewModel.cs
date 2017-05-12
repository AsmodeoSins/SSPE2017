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
    public partial class ReporteBitacoraAccesoAduanaViewModel : ValidationViewModelBase
    {

        public async void OnLoad(ReporteBitacoraAccesoAduana Window)
        {
            try
            {
                ConfiguraPermisos();
                Ventana = Window;
                Reporte = Ventana.ReportBitacoraAccesoAduana;
                
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    Application.Current.Dispatcher.Invoke((Action)(delegate 
                    {
                        ReportViewerVisible = Visibility.Collapsed;
                    }));
                    SelectedFecha = Fechas.GetFechaDateServer;
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

        public void ClickSwitch(object obj)
        {
            switch (obj.ToString())
            {
                case "GenerarReporte":
                    if (!pConsultar)
                    {
                        Ventana.WFH.Visibility = Visibility.Collapsed;
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        break;
                    }

                    Reporte.Reset();
                    GenerarReporte();
                    break;
            }
        }

        public string ObtenerTipoVisitante(ADUANA Aduana)
        {
            var TipoVisitante = string.Empty;
            var TipoPersona = (enumTipoPersona)Aduana.ID_TIPO_PERSONA;
            switch (TipoPersona)
            {
                case enumTipoPersona.EMPLEADO:
                    TipoVisitante = VISITA_EMPLEADO;
                    break;
                case enumTipoPersona.ABOGADO:
                    TipoVisitante = VISITA_LEGAL;
                    break;
                case enumTipoPersona.VISITA:
                    TipoVisitante = Aduana.ADUANA_INGRESO != null ? (Aduana.ADUANA_INGRESO.Where(w => w.ID_ADUANA == Aduana.ID_ADUANA).FirstOrDefault().INTIMA == "S" ? VISITA_LEGAL : VISITA_FAMILIAR) : string.Empty;
                    break;
                case enumTipoPersona.EXTERNA:
                    TipoVisitante = VISITA_EXTERNA;
                    break;
            }
            return TipoVisitante;
        }

        public async void GenerarReporte()
        {

            var lAduana = new List<cBitacoraAccesoAduana>();
            var datosReporte = new List<cReporteDatos>();
            Ventana.WFH.Visibility = Visibility.Collapsed;
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
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

                    lAduana = new cAduana().
                        ObtenerReporteBitacora(GlobalVar.gCentro, SelectedFecha).
                        AsEnumerable().
                        Select(s =>
                        new cBitacoraAccesoAduana()
                        {
                            Asunto = !string.IsNullOrEmpty(s.ASUNTO) ? s.ASUNTO.TrimEnd() : string.Empty,
                            HoraEntrada = string.Format("{0}:{1}", s.ENTRADA_FEC.Value.Hour < 10 ? string.Format("0{0}", s.ENTRADA_FEC.Value.Hour) : s.ENTRADA_FEC.Value.Hour.ToString(), s.ENTRADA_FEC.Value.Minute < 10 ? string.Format("0{0}", s.ENTRADA_FEC.Value.Minute) : s.ENTRADA_FEC.Value.Minute.ToString()),
                            HoraSalida = s.SALIDA_FEC.HasValue ? string.Format("{0}:{1}", s.SALIDA_FEC.Value.Hour < 10 ? string.Format("0{0}", s.SALIDA_FEC.Value.Hour) : s.SALIDA_FEC.Value.Hour.ToString(), s.SALIDA_FEC.Value.Minute < 10 ? string.Format("0{0}", s.SALIDA_FEC.Value.Minute) : s.SALIDA_FEC.Value.Minute.ToString()) : string.Empty,
                            Institucion = !string.IsNullOrEmpty(s.INSTITUCION) ? s.INSTITUCION : string.Empty,
                            LugarDestino = s.AREA != null ? (!string.IsNullOrEmpty(s.AREA.DESCR) ? s.AREA.DESCR.TrimEnd() : string.Empty) : string.Empty,
                            Paterno = !string.IsNullOrEmpty(s.PERSONA.PATERNO) ? s.PERSONA.PATERNO.TrimEnd() : string.Empty,
                            Materno = !string.IsNullOrEmpty(s.PERSONA.MATERNO) ? s.PERSONA.MATERNO.TrimEnd() : string.Empty,
                            Nombre = !string.IsNullOrEmpty(s.PERSONA.NOMBRE) ? s.PERSONA.NOMBRE.TrimEnd() : string.Empty,
                            Observaciones = !string.IsNullOrEmpty(s.OBSERV) ? s.OBSERV.TrimEnd() : string.Empty,
                            TipoVisitante = s.ID_TIPO_PERSONA != null ? ObtenerTipoVisitante(s) : string.Empty
                        }).
                        ToList();
                });
                Ventana.WFH.Visibility = Visibility.Visible;

                #region Reporte
                Reporte.LocalReport.ReportPath = "Reportes/rBitacoraAccesoAduana.rdlc";
                Reporte.LocalReport.DataSources.Clear();

                ReportDataSource ReportDataSource_Encabezado = new ReportDataSource();
                ReportDataSource_Encabezado.Name = "DataSet1";
                ReportDataSource_Encabezado.Value = datosReporte;

                ReportDataSource ReportDataSource_Bitacora = new ReportDataSource();
                ReportDataSource_Bitacora.Name = "DataSet2";
                ReportDataSource_Bitacora.Value = lAduana;

                Reporte.LocalReport.DataSources.Add(ReportDataSource_Encabezado);
                Reporte.LocalReport.DataSources.Add(ReportDataSource_Bitacora);
                #endregion

                #region Parametros
                Reporte.LocalReport.SetParameters(new ReportParameter("Fecha", string.Format("{0}/{1}/{2}", SelectedFecha.Month, SelectedFecha.Day, SelectedFecha.Year)));
                #endregion

                Reporte.Refresh();
                Reporte.RefreshReport();
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
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_BIT_ACCESO_ADUANA.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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