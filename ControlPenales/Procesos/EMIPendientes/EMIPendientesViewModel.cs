using ControlPenales;

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using ControlPenales.Clases;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Windows.Interop;
using System.IO;
using System.Windows.Controls;

namespace ControlPenales
{
    partial class EMIPendientesViewModel : ValidationViewModelBase
    {
        #region constructor
        public EMIPendientesViewModel() { }
        #endregion

        #region Metodos
        private async void clickSwitch(Object op = null)
        {
            switch (op.ToString())
            { 
                case "buscar_menu":
                    if (!pConsultar)
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                    else
                    {
                        if (Opcion != 0)
                            await Buscar();
                        else
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar una opción para la busqueda");
                    }
                    break;
                
                case "limpiar_menu":
                     ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new EMIPendientesView();
                     ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new EMIPendientesViewModel();
                    break;
                case "reporte_menu":
                    if (!pImprimir)
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                    else 
                    {
                        if (filtro != 0)
                            await Reporte();
                        else
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de realizar una busqueda");
                    }
                    break;
                case "salir_menu":
                    PrincipalViewModel.SalirMenu();
                    break;
            }
        }

        private async void OnLoad(EMIPendientesView obj = null)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(ObtenerPermisos);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar decomisos", ex);
            }

        }
        #endregion

        #region Buscar
        private async Task Buscar()
        {
            filtro = Opcion;
            Pagina = 1;
            SeguirCargando = true;
            ResultadosVisible = Visibility.Collapsed;
            LstResultado = new RangeEnabledObservableCollection<INGRESO>();
            TotalInternos();
            LstResultado.InsertRange(await SegmentarResultadoBusqueda());
            ResultadosVisible = LstResultado.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        private void TotalInternos()
        {
            try
            {

                int tp = new cIngreso().ObtenerTotalInternos(GlobalVar.gCentro);
                int temi = new cIngreso().ObtenerEMICompletoTotal(GlobalVar.gCentro);
                Total = string.Format("Población Activa:{0}  Población Entrevista Aplicada:{1}  Población sin Entrevista: {2}", tp, temi, (tp - temi));
                //int  
                //if (filtro == (short)enumEMIPendiente.SIN_EMI)
                //{ 
                //    int totalSEMI = new cIngreso().ObtenerSinEMITotal(GlobalVar.gCentro);
                //    int totalCEMI = new cIngreso().ObtenerTotalInternos(GlobalVar.gCentro);
                //    Total = string.Format("Total Internos Centro: {0}, Total Internos sin E.M.I.: {1}, Porcentaje de internos sin E.M.I.: {2}%", totalCEMI, totalSEMI, totalCEMI > 0 ? (totalSEMI * 100) / totalCEMI : 0);
                //}
                //else
                //{ 
                //    int totalIEMI = new cIngreso().ObtenerEMIIncompletoTotal(GlobalVar.gCentro, "P");
                //    int totalCEMI = new cIngreso().ObtenerEMICompletoTotal(GlobalVar.gCentro);
                //    Total = string.Format("Total internos con E.M.I. completo: {0}, Total Internos con E.M.I. incompleto: {1}, Porcentaje de internos con E.M.I. incompleto: {2}%", totalCEMI, totalIEMI, (totalCEMI + totalIEMI) > 0 ? (totalIEMI * 100) / (totalCEMI + totalIEMI) : 0);
                //}
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al consultar totales de internos.", ex);
            }
        }

        private async Task<List<INGRESO>> SegmentarResultadoBusqueda(int _Pag = 1)
        {
            try
            {
                Pagina = _Pag;
                var result = new ObservableCollection<INGRESO>();
                if (filtro == (short)enumEMIPendiente.SIN_EMI)
                    result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<INGRESO>>(() =>
                                 new ObservableCollection<INGRESO>(new cIngreso().ObtenerSinEMI(GlobalVar.gCentro, _Pag)));
                else
                    result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<INGRESO>>(() =>
                             new ObservableCollection<INGRESO>(new cIngreso().ObtenerEMIIncompleto(GlobalVar.gCentro,"P",_Pag)));
                if (result.Any())
                {
                    Pagina++;
                    SeguirCargando = true;
                }
                else
                    SeguirCargando = false;
                return result.ToList();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al consultar internos.", ex);
                return new List<INGRESO>();
            }
        }
        #endregion

        #region Reporte
        private async Task Reporte() 
        {
            try
            {
                var reporte = new List<cReporte>();
                var totales = new List<cEMIPendienteTotal>();
                var ingresos = new List<cEMIPendienteListado>();
                var centro = new cCentro().Obtener(GlobalVar.gCentro).SingleOrDefault();
                //DATOS DEL REPORTE
                reporte.Add(new cReporte()
                {
                    Logo1 = Parametro.LOGO_ESTADO,
                    Encabezado1 = Parametro.ENCABEZADO1,
                    Encabezado2 = Parametro.ENCABEZADO2,
                    Encabezado3 = centro.DESCR.Trim().ToUpper(),
                    Encabezado4 = filtro == (short)enumEMIPendiente.EMI_INCOMPLETO ? "INTERNOS CON ENTREVISTAS MULTIDICIPLINARIAS INCOMPLETAS" : "INTERNOS SIN ENTREVISTAS MULTIDICIPLINARIAS"
                });
                totales.Add(new cEMIPendienteTotal()
                {
                    Total = Total
                });
                if (filtro == (short)enumEMIPendiente.SIN_EMI)
                    ingresos = new List<cEMIPendienteListado>(new cIngreso().ObtenerSinEMI(GlobalVar.gCentro).Select(w => new cEMIPendienteListado { Anio = w.ID_ANIO, Folio = w.ID_IMPUTADO, Paterno = w.IMPUTADO.PATERNO, Materno = w.IMPUTADO.MATERNO, Nombre = w.IMPUTADO.NOMBRE, Cama = w.CAMA, Celda = w.CAMA.CELDA, Sector = w.CAMA.CELDA.SECTOR, Edificio = w.CAMA.CELDA.SECTOR.EDIFICIO, Pendiente = string.Empty, EsPendiente = 0 }));
                else
                {
                    ingresos = new List<cEMIPendienteListado>();
                    var emi_incompleto = new cIngreso().ObtenerEMIIncompleto(GlobalVar.gCentro, "P");
                    foreach(var obj in emi_incompleto)
                    {
                        var emi = obj.EMI_INGRESO.FirstOrDefault().EMI;
                        ingresos.Add(new cEMIPendienteListado
                        {
                            Anio = obj.ID_ANIO,
                            Folio = obj.ID_IMPUTADO,
                            Paterno = obj.IMPUTADO.PATERNO,
                            Materno = obj.IMPUTADO.MATERNO,
                            Nombre = obj.IMPUTADO.NOMBRE,
                            Cama = obj.CAMA,
                            Celda = obj.CAMA.CELDA,
                            Sector = obj.CAMA.CELDA.SECTOR,
                            Edificio = obj.CAMA.CELDA.SECTOR.EDIFICIO,
                            EsPendiente = 1,
                            PSituacionJuridica = emi.EMI_SITUACION_JURIDICA,
                            PFactoresGrupoFamiliar = emi.EMI_FACTORES_SOCIO_FAMILIARES,
                            PDrogas = emi.EMI_USO_DROGA.Count,
                            PHPS = emi.EMI_HPS,
                            PClasificacionCriminologica = emi.EMI_CLAS_CRIMINOLOGICA
                        });
                    }
                    //ingresos = new List<cEMIPendienteListado>(new cIngreso().ObtenerEMIIncompleto(GlobalVar.gCentro, "P").Select(w => new cEMIPendienteListado { Anio = w.ID_ANIO, Folio = w.ID_IMPUTADO, Paterno = w.IMPUTADO.PATERNO, Materno = w.IMPUTADO.MATERNO, Nombre = w.IMPUTADO.NOMBRE,Cama = w.CAMA,Celda = w.CAMA.CELDA,Sector = w.CAMA.CELDA.SECTOR, Edificio = w.CAMA.CELDA.SECTOR.EDIFICIO,
                    // EsPendiente = 1,
                    // PSituacionJuridica = w.EMI_INGRESO.FirstOrDefault().EMI.EMI_SITUACION_JURIDICA,
                    // //PFactoresGrupoFamiliar = w.EMI_INGRESO.FirstOrDefault().EMI.EMI_FACTORES_SOCIO_FAMILIARES,
                    // //PDrogas = w.EMI_INGRESO.FirstOrDefault().EMI.EMI_USO_DROGA.Count,
                    // //PHPS = w.EMI_INGRESO.FirstOrDefault().EMI.EMI_HPS,
                    // //PClasificacionCriminologica = w.EMI_INGRESO.FirstOrDefault().EMI.EMI_CLAS_CRIMINOLOGICA
                    //})); 
                }
                
                var view = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                view.Owner = PopUpsViewModels.MainWindow;
                view.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                view.Show();
                #region Genera Reporte
                view.Report.LocalReport.ReportPath = "Reportes/rEMIPendiente.rdlc";
                view.Report.LocalReport.DataSources.Clear();

                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds1.Name = "DataSet1";
                rds1.Value = reporte;
                view.Report.LocalReport.DataSources.Add(rds1);

                Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds2.Name = "DataSet2";
                rds2.Value = totales;
                view.Report.LocalReport.DataSources.Add(rds2);

                Microsoft.Reporting.WinForms.ReportDataSource rds3 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds3.Name = "DataSet3";
                rds3.Value = ingresos;
                view.Report.LocalReport.DataSources.Add(rds3);
                #endregion
                view.Report.RefreshReport();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al imprimir reporte.", ex);
            }
        }
        #endregion

        #region Permisos
        private void ObtenerPermisos() 
        {
            try 
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.EMI_PENDIENTE.ToString(), GlobalVar.gUsr);
                if (permisos != null)
                {
                    foreach (var p in permisos)
                    {
                        if (p.CONSULTAR == 1)
                        {
                            pConsultar = MenuBuscarEnabled = true;
                        }
                        if (p.IMPRIMIR == 1)
                        {
                            pImprimir = MenuReporteEnabled =  true;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener permisos.", ex);
            }
        }
        #endregion
    }

    enum enumEMIPendiente 
    { 
        SIN_EMI = 1,
        EMI_INCOMPLETO = 2
    }
}
