using ControlPenales;
using ControlPenales.BiometricoServiceReference;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace ControlPenales
{
    partial class BitacoraIngresosVisitaLegalIndViewModel : ValidationViewModelBase
    {

        #region Constructor
        public BitacoraIngresosVisitaLegalIndViewModel() { }
        #endregion

        #region Metodos
        private async void SwitchClick(Object obj)
        {
            switch(obj.ToString())
            {
                case "buscar":
                    LimpiarBusqueda();
                    ReportViewerVisible = Visibility.Collapsed;
                    PopUpsViewModels.ShowPopUp(this,PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "nueva_busqueda":
                    LimpiarBusqueda();
                    break;
                case "buscar_salir":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    ReportViewerVisible = Visibility.Visible;
                    LimpiarBusqueda();
                    break;
                case "buscar_seleccionar":
                    if (SelectIngreso != null)
                    {
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        ReportViewerVisible = Visibility.Visible;
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(GenerarReporte);
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un ingreso");
                    break;
            }
        }

        private void OnLoad(BitacoraIngresosVisitaLegalIndView Window = null)
        {                   
            try
            {
                Reporte = Window.Report;
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la bitacora de recepción legal", ex);
            }
        }
        #endregion

        #region Buscar
        private void LimpiarBusqueda()
        {
            AnioBuscar = null;
            FolioBuscar = null;
            NombreBuscar = ApellidoPaternoBuscar = ApellidoMaternoBuscar = string.Empty;
            ListExpediente = null;
            SelectExpediente = null;
            SelectIngreso = null;
            ImagenIngreso = ImagenImputado = new Imagenes().getImagenPerson();
        }

        private async Task<List<IMPUTADO>> SegmentarResultadoBusqueda(int _Pag = 1)
        {
            try
            {
                if (string.IsNullOrEmpty(ApellidoPaternoBuscar) && string.IsNullOrEmpty(ApellidoMaternoBuscar) && string.IsNullOrEmpty(NombreBuscar) && !AnioBuscar.HasValue && !FolioBuscar.HasValue)
                    return new List<IMPUTADO>();
                Pagina = _Pag;
                var result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<IMPUTADO>>(() =>
                    new cImputado().ObtenerTodos( ApellidoPaternoBuscar, ApellidoMaternoBuscar, NombreBuscar, AnioBuscar, FolioBuscar, _Pag));
                Pagina = result.Any() ? Pagina + 1 : Pagina;
                SeguirCargando = result.Any();
                return result.ToList();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al querer guardar.", ex);
                return new List<IMPUTADO>();
            }
        }

        private async void buscarImputado(Object obj = null)
        {
            try
            {
                if (obj != null)
                {
                    //cuando es boton no se hace nada porque solamente existe el de buscar, si hay otro habra que castearlos a button y hacer la comparacion
                    var textbox = obj as TextBox;
                    if (textbox != null)
                    {
                        switch (textbox.Name)
                        {
                            case "NombreBuscar":
                                NombreBuscar = textbox.Text;
                                break;
                            case "ApellidoPaternoBuscar":
                                ApellidoPaternoBuscar = textbox.Text;
                                break;
                            case "ApellidoMaternoBuscar":
                                ApellidoMaternoBuscar = textbox.Text;
                                break;
                            case "FolioBuscar":
                                FolioBuscar = Convert.ToInt32(textbox.Text);
                                break;
                            case "AnioBuscar":
                                AnioBuscar = short.Parse(textbox.Text);
                                break;
                        }
                    }
                }
                ImagenIngreso = ImagenImputado = new Imagenes().getImagenPerson();
                ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                if (ListExpediente != null)
                    EmptyExpedienteVisible = ListExpediente.Count < 0;
                else
                    EmptyExpedienteVisible = true;
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar al imputado.", ex);
            }

        }


        private void ClickEnter(Object obj)
        {
            buscarImputado(obj);
        }
        #endregion

        #region Bitacora Recepcion Legal
        private void GenerarReporte()
        {
            try
            {
                var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();
                var datosReporte = new List<cReporteDatos>();
                datosReporte.Add(new cReporteDatos()
                {
                    Encabezado1 = Parametro.ENCABEZADO1.Trim(),
                    Encabezado2 = Parametro.ENCABEZADO2.Trim(),
                    Encabezado3 = Parametro.ENCABEZADO3.Trim(),
                    Titulo = "Bitácora de Ingresos por Visita Legal",
                    Logo1 = Parametro.REPORTE_LOGO1,
                    Logo2 = Parametro.REPORTE_LOGO2,
                    Centro = centro.DESCR.Trim().ToUpper(),
                });

                var Ingreso = new List<cBitacoraIngresosVisitaLegal>();
                var lstAbogados = new List<cBitacoraIngresosVisitaLegal>();
                if (SelectIngreso != null)
                {
                    var i = new cBitacoraIngresosVisitaLegal();
                    i.Expediente = string.Format("{0}/{1}", SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO);
                    i.Interno = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty, !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty);
                    Ingreso.Add(i);

                    var ingresos = new cAduanaIngreso().ObtenerTodosAbogadosIngreso(SelectIngreso.ID_CENTRO, SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO);
                    if (ingresos != null)
                    {
                        foreach (var ai in ingresos)
                        {
                            var obj = new cBitacoraIngresosVisitaLegal();
                            obj.FechaHoraEntrada = string.Format("{0:dd/MM/yyyy hh:mm tt}", ai.ADUANA.ENTRADA_FEC);
                            obj.FechaHoraSalida = string.Format("{0:dd/MM/yyyy hh:mm tt}", ai.ADUANA.SALIDA_FEC);
                            obj.Abogado = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(ai.ADUANA.PERSONA.NOMBRE) ? ai.ADUANA.PERSONA.NOMBRE.Trim() : string.Empty, !string.IsNullOrEmpty(ai.ADUANA.PERSONA.PATERNO) ? ai.ADUANA.PERSONA.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(ai.ADUANA.PERSONA.MATERNO) ? ai.ADUANA.PERSONA.MATERNO.Trim() : string.Empty);
                            var x = ai.ADUANA.PERSONA.PERSONA_NIP.Where(w => w.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                            if(x != null)
                            {
                                obj.Expediente = x.NIP.ToString();
                            }
                            lstAbogados.Add(obj);
                        }
                    }
                }
                //ARMAMOS EL REPORTE
                Reporte.LocalReport.ReportPath = "Reportes/rBitacoraIngresoVisitaLegalInd.rdlc";
                Reporte.LocalReport.DataSources.Clear();

                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds1.Name = "DataSet1";
                rds1.Value = lstAbogados;
                Reporte.LocalReport.DataSources.Add(rds1);

                Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds2.Name = "DataSet2";
                rds2.Value = datosReporte;
                Reporte.LocalReport.DataSources.Add(rds2);

                Microsoft.Reporting.WinForms.ReportDataSource rds3 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds3.Name = "DataSet3";
                rds3.Value = Ingreso;
                Reporte.LocalReport.DataSources.Add(rds3);

               System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
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

        #region Cambio SelectedItem de Busqueda de Expediente
        private async void OnModelChangedSwitch(object parametro)
        {
            if (parametro != null)
            {
                switch (parametro.ToString())
                {
                    case "cambio_expediente":
                        if (SelectExpediente != null && (SelectExpediente.INGRESO == null || SelectExpediente.INGRESO.Count == 0))
                        {
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {
                                selectExpediente = new cImputado().Obtener(selectExpediente.ID_IMPUTADO, selectExpediente.ID_ANIO, selectExpediente.ID_CENTRO).First();
                                RaisePropertyChanged("SelectExpediente");
                            });
                            //MUESTRA LOS INGRESOS
                            if (SelectExpediente.INGRESO != null && SelectExpediente.INGRESO.Count > 0)
                            {
                                EmptyIngresoVisible = false;
                                SelectIngreso = SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                            }
                            else
                                EmptyIngresoVisible = true;

                            //OBTENEMOS FOTO DE FRENTE
                            if (SelectIngreso != null)
                            {
                                if (SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                                    ImagenImputado = SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                                else
                                    ImagenImputado = new Imagenes().getImagenPerson();
                            }
                            else
                                ImagenImputado = new Imagenes().getImagenPerson();
                        }
                        break;
                }
            }
        }
        #endregion
    }
}
