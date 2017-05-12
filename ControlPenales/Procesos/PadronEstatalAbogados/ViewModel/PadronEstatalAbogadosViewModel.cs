using ControlPenales;
using Microsoft.Reporting.WinForms;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace ControlPenales
{
    public partial class PadronEstatalAbogadosViewModel : ValidationViewModelBase
    {

        #region Constructor
        public PadronEstatalAbogadosViewModel() { }
        #endregion

        #region Metodos
        private async void OnLoad(PadronEstatalAbogadosView Window = null)
        {
            try
            {
                Reporte = Window.Report;
                await StaticSourcesViewModel.CargarDatosMetodoAsync(GenerarReporte);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el padron estatal de abogados", ex);
            }
        }
        #endregion

        #region Bitacora Recepcion Legal
        private void GenerarReporte()
        {

            try
            {
                var datosReporte = new List<cReporteDatos>();
                datosReporte.Add(new cReporteDatos()
                {
                    Encabezado1 = Parametro.ENCABEZADO1.Trim(),
                    Encabezado2 = Parametro.ENCABEZADO2.Trim(),
                    Encabezado3 = Parametro.ENCABEZADO3.Trim(),
                    Titulo = "Padrón Estatal de Abogados",
                    Logo1 = Parametro.REPORTE_LOGO1,
                    Logo2 = Parametro.REPORTE_LOGO2,
                });

                var lstAbogados = new List<cPadronEstatalAbogados>();
                var lstColaboradores = new List<cPadronEstatalAbogadosColaborador>();
             
                var abogados = new cAbogado().ObtenerTodos(string.Empty, string.Empty, string.Empty, 0, (short)enumEstatusVisita.AUTORIZADO,"T",true);
                if (abogados != null)
                {
                    foreach(var a in abogados)
                    {
                        if (a.ABOGADO_TITULAR2 != null)
                        {
                            var colaboradores = a.ABOGADO_TITULAR2.Where(w => w.ESTATUS == "S");
                            if (colaboradores != null)
                            {
                                foreach (var c in colaboradores)
                                {
                                    lstColaboradores.Add(new cPadronEstatalAbogadosColaborador() { NumeroTitular = a.ID_ABOGADO.ToString(), NumeroCredencial = c.ID_ABOGADO.ToString(), NombreColaborador = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(c.ABOGADO.PERSONA.NOMBRE) ? c.ABOGADO.PERSONA.NOMBRE.Trim() : string.Empty, !string.IsNullOrEmpty(c.ABOGADO.PERSONA.PATERNO) ? c.ABOGADO.PERSONA.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(c.ABOGADO.PERSONA.MATERNO) ? c.ABOGADO.PERSONA.MATERNO.Trim() : string.Empty) });     
                                }
                            }
                        }
                        //if (a.ABOGADO1 != null)
                        //{
                        //    foreach (var c in a.ABOGADO1)
                        //    {
                        //        lstColaboradores.Add(new cPadronEstatalAbogadosColaborador() { NumeroTitular = a.ID_ABOGADO.ToString(), NumeroCredencial = c.ID_ABOGADO.ToString(), NombreColaborador = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(c.PERSONA.NOMBRE) ? c.PERSONA.NOMBRE.Trim() : string.Empty, !string.IsNullOrEmpty(c.PERSONA.PATERNO) ? c.PERSONA.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(c.PERSONA.MATERNO) ? c.PERSONA.MATERNO.Trim() : string.Empty) });    
                        //    }
                        //}
                        var obj = new cPadronEstatalAbogados();
                        obj.Numero = a.ID_ABOGADO.ToString();
                        obj.Nombre = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(a.PERSONA.NOMBRE) ? a.PERSONA.NOMBRE.Trim() : string.Empty , !string.IsNullOrEmpty(a.PERSONA.PATERNO) ? a.PERSONA.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(a.PERSONA.MATERNO) ? a.PERSONA.MATERNO.Trim() : string.Empty);
                        obj.Tipo = "TITULAR";
                        obj.IFE = a.PERSONA.IFE;
                        obj.RFC = a.PERSONA.RFC;
                        obj.Cedula = a.CEDULA;
                        obj.Correo = a.PERSONA.CORREO_ELECTRONICO;
                        obj.Celular = a.PERSONA.TELEFONO_MOVIL;
                        obj.FechaAlta = string.Format("{0:dd/MM/yyyy}",a.ALTA_FEC);
                        var foto = a.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == 102).FirstOrDefault();
                        if (foto != null)
                            obj.Foto = foto.BIOMETRICO;
                        else
                            obj.Foto = new Imagenes().getImagenPerson();
                        var nip = a.PERSONA.PERSONA_NIP.Where(w => w.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                        if (nip != null)
                            obj.NIP = nip.NIP != null ? nip.NIP.ToString() : "0";
                        else
                            obj.NIP = "0";
                        lstAbogados.Add(obj);
                    }
                }

                //ARMAMOS EL REPORTE
                Reporte.LocalReport.ReportPath = "Reportes/rPadronEstatalAbogados.rdlc";
                Reporte.LocalReport.DataSources.Clear();

                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds1.Name = "DataSet1";
                rds1.Value = lstAbogados;
                Reporte.LocalReport.DataSources.Add(rds1);

                Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds2.Name = "DataSet2";
                rds2.Value = datosReporte;
                Reporte.LocalReport.DataSources.Add(rds2);

                //Subreporte
                Reporte.LocalReport.SubreportProcessing += (s,e) => {
                    ReportDataSource ds = new ReportDataSource("DataSet1", lstColaboradores);
                    e.DataSources.Add(ds);
                };

                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    Reporte.RefreshReport();
                    ReportViewerVisible = Visibility.Visible;
                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar reporte", ex);
            }
        }
        #endregion
    }
}
