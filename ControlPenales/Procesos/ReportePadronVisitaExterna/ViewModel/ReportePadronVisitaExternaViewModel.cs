using ControlPenales.BiometricoServiceReference;
using Microsoft.Reporting.WinForms;
using SSP.Controlador.Catalogo.Justicia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlPenales
{
    public partial class ReportePadronVisitaExternaViewModel : ValidationViewModelBase
    {
        public async void OnLoad(ReportePadronVisitaExterna Window)
        {
            try
            {
                ConfiguraPermisos();
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ReportViewerVisible = Visibility.Collapsed;
                    }));
                    Ventana = Window;
                    Reporte = Ventana.ReportPadronVisitaExterna;
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ReportViewerVisible = Visibility.Visible;
                    }));
                });
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
            var lPadron = new List<cPadronVisitaExterna>();
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


                    lPadron = new cPersonaExterna().
                    ObtenerTodosPersonas(GlobalVar.gCentro).
                    AsEnumerable().
                    Select(s =>
                        new cPadronVisitaExterna()
                        {
                            Correo = !string.IsNullOrEmpty(s.PERSONA.CORREO_ELECTRONICO) ? s.PERSONA.CORREO_ELECTRONICO.TrimEnd() : string.Empty,
                            Discapacidad = s.PERSONA.TIPO_DISCAPACIDAD != null ? s.PERSONA.TIPO_DISCAPACIDAD.DESCR.TrimEnd() : string.Empty,
                            Estatus = string.Empty,
                            FechaAlta = string.Empty,
                            Foto = s.PERSONA.PERSONA_BIOMETRICO != null ?
                            (s.PERSONA.PERSONA_BIOMETRICO.Where(w =>
                                w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).Any() ?
                                s.PERSONA.PERSONA_BIOMETRICO.Where(w =>
                                w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO &&
                                w.ID_PERSONA == s.ID_PERSONA).FirstOrDefault().BIOMETRICO : new Imagenes().getImagenPerson()) : new Imagenes().getImagenPerson(),
                            //Foto = new Imagenes().getImagenPerson(),
                            Materno = !string.IsNullOrEmpty(s.PERSONA.MATERNO) ? s.PERSONA.MATERNO.TrimEnd() : string.Empty,
                            Paterno = !string.IsNullOrEmpty(s.PERSONA.PATERNO) ? s.PERSONA.PATERNO.TrimEnd() : string.Empty,
                            Nombre = !string.IsNullOrEmpty(s.PERSONA.NOMBRE) ? s.PERSONA.NOMBRE.TrimEnd() : string.Empty,
                            NIP = (s.PERSONA.PERSONA_NIP != null) ? (s.PERSONA.PERSONA_NIP.Where(w =>
                            w.ID_CENTRO == GlobalVar.gCentro &&
                            w.ID_PERSONA == s.ID_PERSONA).Count() > 0 ?
                            s.PERSONA.PERSONA_NIP.Where(w =>
                            w.ID_CENTRO == GlobalVar.gCentro &&
                            w.ID_PERSONA == s.ID_PERSONA).FirstOrDefault().NIP.Value.ToString() : string.Empty) : string.Empty,
                            //NIP = string.Empty,
                            NumeroVisita = string.Empty,
                            Observaciones = !string.IsNullOrEmpty(s.OBSERV) ? s.OBSERV.TrimEnd() : string.Empty,
                            Telefono = !string.IsNullOrEmpty(s.PERSONA.TELEFONO) ? s.PERSONA.TELEFONO.TrimEnd() : string.Empty,
                            TipoVisitante = s.TIPO_VISITANTE != null ? (!string.IsNullOrEmpty(s.TIPO_VISITANTE.DESCR) ? s.TIPO_VISITANTE.DESCR.TrimEnd() : string.Empty) : string.Empty
                        }).ToList();
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ReportViewerVisible = Visibility.Visible;
                    }));
                });


                Reporte.LocalReport.ReportPath = "Reportes/rPadronVisitaExterna.rdlc";
                Reporte.LocalReport.DataSources.Clear();

                ReportDataSource ReportDataSource_Encabezado = new ReportDataSource();
                ReportDataSource_Encabezado.Name = "DataSet1";
                ReportDataSource_Encabezado.Value = datosReporte;

                ReportDataSource ReportDataSource_PadronVisitaExterna = new ReportDataSource();
                ReportDataSource_PadronVisitaExterna.Name = "DataSet2";
                ReportDataSource_PadronVisitaExterna.Value = lPadron;


                Reporte.LocalReport.DataSources.Add(ReportDataSource_Encabezado);
                Reporte.LocalReport.DataSources.Add(ReportDataSource_PadronVisitaExterna);

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
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_PADRON_VISITA_EXTERNA.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
