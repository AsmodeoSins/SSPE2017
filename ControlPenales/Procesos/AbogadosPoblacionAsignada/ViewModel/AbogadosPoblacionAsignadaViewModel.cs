using ControlPenales;
using Microsoft.Reporting.WinForms;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace ControlPenales
{
    public partial class AbogadosPoblacionAsignadaViewModel : ValidationViewModelBase
    {
        #region Metodos
        private async void OnLoad(AbogadosPoblacionAsignadaView Window = null)
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
        #region Datos del reporte
        private void GenerarReporte()
        {

            try
            {
                var datosReporte = new List<cReporteDatos>();
                datosReporte.Add(new cReporteDatos()
                {
                    Encabezado1 = Parametro.ENCABEZADO1,
                    Encabezado2 = Parametro.ENCABEZADO2,
                    Encabezado3 = Parametro.ENCABEZADO3,
                    Titulo = "Abogados con población asignada",
                    Logo1 = Parametro.REPORTE_LOGO1,
                    Logo2 = Parametro.REPORTE_LOGO2,
                });

                var lstImputadoAbogado = new List<cImputadoAbogado>();
                var lstImputadoCausaPenalAbogado = new List<cImputadoCausaPenalAbogado>();
                var lstImputadoPaseAbogado = new List<cImputadoPaseAbogado>();

                


                lstImputadoAbogado = new List<cImputadoAbogado>(new cAbogado().AbogadosAsignados(GlobalVar.gCentro, Parametro.ID_ESTATUS_VISITA_AUTORIZADO)
                    .Select(s=>new cImputadoAbogado{
                    ID_ABOGADO = s.ID_ABOGADO,
                    MATERNO=s.PERSONA.MATERNO.Trim(),
                    NOMBRE=s.PERSONA.NOMBRE.Trim(),
                    PATERNO=s.PERSONA.PATERNO.Trim(),
                    TITULAR=s.ABOGADO_TITULAR,
                    TITULAR_ORDEN=s.ABOGADO_TITULAR.HasValue?s.ABOGADO_TITULAR.Value:s.ID_ABOGADO,
                    TITULO=s.ABOGADO_TITULO.DESCR==null || s.ABOGADO_TITULO.DESCR.Trim()==""?"":s.ABOGADO_TITULO.DESCR.Trim(),
                    TITULO_ORDEN=s.ID_ABOGADO_TITULO=="T"?1:2
                }).OrderBy(o=>o.TITULAR_ORDEN).ThenBy(o=>o.TITULO_ORDEN));
                lstImputadoCausaPenalAbogado = new List<cImputadoCausaPenalAbogado>(new cAbogadoIngreso().ObtenerAsignados(GlobalVar.gCentro, Parametro.ID_ESTATUS_VISITA_AUTORIZADO)
                    .Select(s => new cImputadoCausaPenalAbogado
                    {
                        ID_ABOGADO =s.ID_ABOGADO,
                        ID_ANIO = s.ID_ANIO,
                        ID_CENTRO = s.ID_CENTRO,
                        ID_IMPUTADO = s.ID_IMPUTADO,
                        ID_INGRESO = s.ID_INGRESO,
                        MATERNO = s.INGRESO.IMPUTADO.MATERNO.Trim(),
                        NOMBRE = s.INGRESO.IMPUTADO.NOMBRE.Trim(),
                        PATERNO = s.INGRESO.IMPUTADO.PATERNO.Trim()
                    }).OrderBy(o => o.NOMBRE).ThenBy(o=>o.PATERNO));

                lstImputadoPaseAbogado = new List<cImputadoPaseAbogado>(new cAbogadoCausaPenal().ObtenerAsignados(GlobalVar.gCentro, Parametro.ID_ESTATUS_VISITA_AUTORIZADO)
                    .Select(s => new cImputadoPaseAbogado
                    {
                        CP_ANIO = s.CAUSA_PENAL.CP_ANIO,
                        CP_FOLIO = s.CAUSA_PENAL.CP_FOLIO,
                        ID_ABOGADO = s.ID_ABOGADO,
                        ID_ANIO = s.ID_ANIO,
                        ID_CAUSA_PENAL = s.ID_CAUSA_PENAL,
                        ID_CENTRO = s.ID_CENTRO,
                        ID_ESTATUS_VISITA = s.ID_ESTATUS_VISITA,
                        ID_IMPUTADO = s.ID_IMPUTADO,
                        ID_INGRESO = s.ID_INGRESO
                    }));
                
                if (lstImputadoAbogado != null)
                {
                    
                }


                ////ARMAMOS EL REPORTE
                Reporte.LocalReport.ReportPath = "Reportes/rImputadoAbogado.rdlc";
                Reporte.LocalReport.DataSources.Clear();

                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds1.Name = "DataSet1";
                rds1.Value = lstImputadoAbogado;
                Reporte.LocalReport.DataSources.Add(rds1);

                Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds2.Name = "DataSet2";
                rds2.Value = datosReporte;
                Reporte.LocalReport.DataSources.Add(rds2);

                //Subreporte
                Reporte.LocalReport.SubreportProcessing += (s, e) =>
                {

                    if (e.ReportPath.Equals("srImputadoCausaPenalAbogado",StringComparison.InvariantCultureIgnoreCase))
                    {
                        ReportDataSource ds = new ReportDataSource("DS_ImputadoCausaPenalAbogado", lstImputadoCausaPenalAbogado);
                        e.DataSources.Add(ds);
                    }
                    else
                    {
                        ReportDataSource ds2 = new ReportDataSource("DS_ImputadoPaseAbogado", lstImputadoPaseAbogado);
                        e.DataSources.Add(ds2);
                    }
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
