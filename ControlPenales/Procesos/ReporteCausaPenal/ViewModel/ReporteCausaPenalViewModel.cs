using ControlPenales;
using Oracle.ManagedDataAccess.Client;
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
using System.Windows.Forms.Integration;
using System.Windows.Input;

namespace ControlPenales
{
    public partial class ReporteCausaPenalViewModel : ValidationViewModelBase
    {

        #region Constructor
        public ReporteCausaPenalViewModel() {}
        #endregion

        #region Metodos
        private async void SwitchClick(Object obj)
        {

            switch (obj.ToString())
            { 
                case "filtros":
                    if (!pConsultar)
                    {
                        Repositorio.Visibility = Visibility.Collapsed;
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        break;
                    }
                    LimpiarFiltros();
                    ReportViewerVisible = Visibility.Collapsed;
                    PopUpsViewModels.ShowPopUp(this,PopUpsViewModels.TipoPopUp.FILTRO_REPORTE_CAUSAS_PENALES);
                    break;
                case "calcular":
                    if (!pConsultar)
                    {
                        Repositorio.Visibility = Visibility.Collapsed;
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        break;
                    }
                    //Calcular(); 
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(Calcular);
                    break;
                case "generar":
                    if (!pConsultar)
                    {
                        Repositorio.Visibility = Visibility.Collapsed;
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        break;
                    }
                    //GenerarReporte();
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(GenerarReporte);
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.FILTRO_REPORTE_CAUSAS_PENALES);
                    ReportViewerVisible = Visibility.Visible;
                    break;
                case "cancelar":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.FILTRO_REPORTE_CAUSAS_PENALES);
                    ReportViewerVisible = Visibility.Visible;
                    break;
            }
            //if (!base.HasErrors)
               //StaticSourcesViewModel.CargarDatosMetodoAsync(GenerarReporte);
            //else
            //{
            //    ReportViewerVisible = Visibility.Collapsed;
            //    await new Dialogos().ConfirmacionDialogoReturn("Validación", base.Error);
            //    ReportViewerVisible = Visibility.Visible;
            //}
        }

        private void OnLoad(ReporteCausaPenalView Window = null)
        {                   
            try
            {
                ConfiguraPermisos();
                Reporte = Window.Report;
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.FILTRO_REPORTE_CAUSAS_PENALES);
                Repositorio = Window.WFH;
                //ValidarFiltros();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la bitacora de recepción legal", ex);
            }
        }
        #endregion

        #region Reporte
        private void LimpiarFiltros() {
            DAnios = DMeses = DDias = HAnios = HMeses = HDias = null;
            Compurgado = true;
            Fec = Fechas.GetFechaDateServer;
            TiempoRealCompurgacion = false;
            Resultado = null;
            LstInternos = null;
        }

        private void Calcular() 
        {
            try
            {

                int da = 0, dm = 0, dd = 0, ha = 0, hm = 0, hd = 0, tipo = 1;
                DateTime fecha = Fec.HasValue ? Fec.Value.Date : Fechas.GetFechaDateServer;
                //desde
                da = DAnios.HasValue ? DAnios.Value : 0;
                dm = DMeses.HasValue ? DMeses.Value : 0;
                dd = DDias.HasValue ? DDias.Value : 0;
                //hasta
                ha = HAnios.HasValue ? HAnios.Value : 0;
                hm = HMeses.HasValue ? HMeses.Value : 0;
                hd = HDias.HasValue ? HDias.Value : 0;
                tipo = Compurgado ? 1 : 0;
                LstInternos = new cCausaPenal().ObtenerReporte(((da * 365) + (dm * 30) + dd), ((ha * 365) + (hm * 30) + hd), tipo, fecha).Select(w => new cReporteCausaPenal()
                {
                    ID_ANIO = w.ID_ANIO,
                    ID_IMPUTADO = w.ID_IMPUTADO,
                    NOMBRE = w.NOMBRE,
                    PATERNO = w.PATERNO,
                    MATERNO = w.MATERNO,
                    ANIO_GOBIERNO = w.ANIO_GOBIERNO,
                    FOLIO_GOBIERNO = w.FOLIO_GOBIERNO,
                    FECHA_INICIO = w.FECHA_INICIO,
                    S_ANIO = w.S_ANIO,
                    S_MES = w.S_MES,
                    S_DIA = w.S_DIA,
                    C_ANIO = w.C_ANIO,
                    C_MES = w.C_MES,
                    C_DIA = w.C_DIA,
                    A_ANIO = w.A_ANIO,
                    A_MES = w.A_MES,
                    A_DIA = w.A_DIA,
                    PC_ANIO = w.PC_ANIO,
                    PC_MES = w.PC_MES,
                    PC_DIA = w.PC_DIA,
                    ID_UB_CENTRO = w.ID_UB_CENTRO
                });
                
                Resultado = LstInternos.Count();

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar reporte.", ex);
            }
        }

        private void GenerarReporte() 
        {
            try
            {
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    Repositorio.Visibility = Visibility.Collapsed;
                }));
                var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();

                var datosReporte = new List<cReporteDatos>();
                datosReporte.Add(new cReporteDatos()
                {
                    Encabezado1 = Parametro.ENCABEZADO1.Trim(),
                    Encabezado2 = Parametro.ENCABEZADO2.Trim(),
                    Encabezado3 = Parametro.ENCABEZADO3.Trim(),
                    Titulo = "TIEMPO POR COMPURGAR",
                    Logo1 = Parametro.REPORTE_LOGO1,
                    Logo2 = Parametro.REPORTE_LOGO2,
                    Centro = centro.DESCR.Trim().ToUpper(),
                });
                DateTime fecha = Fec.HasValue ? Fec.Value.Date : Fechas.GetFechaDateServer;
                int da = 0, dm = 0, dd = 0,ha = 0, hm = 0, hd = 0,tipo = 1;
                //desde
                da = DAnios.HasValue ? DAnios.Value : 0;
                dm = DMeses.HasValue ? DMeses.Value : 0;
                dd = DDias.HasValue ? DDias.Value : 0;
                //hasta
                ha = HAnios.HasValue ? HAnios.Value : 0;
                hm = HMeses.HasValue ? HMeses.Value : 0;
                hd = HDias.HasValue ? HDias.Value : 0;
                tipo = Compurgado ? 1 : 0;

                LstInternos = new cCausaPenal().ObtenerReporte(((da * 365) + (dm * 30) + dd), ((ha * 365) + (hm * 30) + hd),tipo,fecha).Select(w => new cReporteCausaPenal()
                    {
                        ID_ANIO = w.ID_ANIO,
                        ID_IMPUTADO = w.ID_IMPUTADO,
                        NOMBRE = w.NOMBRE,
                        PATERNO = w.PATERNO,
                        MATERNO = w.MATERNO,
                        ANIO_GOBIERNO = w.ANIO_GOBIERNO,
                        FOLIO_GOBIERNO = w.FOLIO_GOBIERNO,
                        FECHA_INICIO = w.FECHA_INICIO ,
                        S_ANIO = w.S_ANIO,
                        S_MES = w.S_MES,
                        S_DIA = w.S_DIA,
                        C_ANIO = w.C_ANIO,
                        C_MES = w.C_MES,
                        C_DIA = w.C_DIA,
                        A_ANIO = w.A_ANIO,
                        A_MES = w.A_MES,
                        A_DIA = w.A_DIA,
                        PC_ANIO = w.PC_ANIO,
                        PC_MES = w.PC_MES,
                        PC_DIA = w.PC_DIA,
                        ID_UB_CENTRO = w.ID_UB_CENTRO
                    });

                #region Reporte
                Reporte.LocalReport.ReportPath = "Reportes/rCausasPenales.rdlc";
                Reporte.LocalReport.DataSources.Clear();

                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds1.Name = "DataSet3";
                rds1.Value = datosReporte;
                Reporte.LocalReport.DataSources.Add(rds1);

                Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds2.Name = "DataSet1";
                rds2.Value = LstInternos;
                Reporte.LocalReport.DataSources.Add(rds2);

                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    Repositorio.Visibility = Visibility.Visible;
                    Reporte.RefreshReport();
                }));
                #endregion

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar reporte.", ex);
            }
        }
        #endregion

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_CAUSA_PENAL.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
