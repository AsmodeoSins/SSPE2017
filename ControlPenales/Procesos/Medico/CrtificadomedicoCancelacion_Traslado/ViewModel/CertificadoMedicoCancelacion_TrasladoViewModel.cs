using ControlPenales.Clases;
using ControlPenales;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using SSP.Controlador.Catalogo.Justicia.Liberados;
using SSP.Controlador.Catalogo.Justicia.Medico.CertificadoMedico;

namespace ControlPenales
{
    partial class CertificadoMedicoCancelacion_TrasladoViewModel : FingerPrintScanner
    {
        cTrasladoDetalle TrasladoDetalleControlador = new cTrasladoDetalle();
        cExcarcelacion ExCarcelacionControlador = new cExcarcelacion();
        cAtencionMedica AtencionMedicaCOntrolador = new cAtencionMedica();
        UserControl UserCtr;
        List<string> LstEstatusTras_Salida = new List<string>() { "PR", "EP" };
       List<string> LstEstatusExcar_SALIDA= new List<string>() { "EP", "AU" };

        private async void clickSwitch(object op)
        {

            switch (op.ToString())
            {
                #region Busqueda de Imputado y Procesos


                case "buscar_menu":
                    if (StaticSourcesViewModel.SourceChanged)
                    {
                        var dialogresult = await(new Dialogos()).ConfirmarEliminar("Advertencia", "Hay cambios sin guardar, ¿Seguro que desea realizar una nueva busqueda sin guardar?");
                        if (dialogresult != 0)
                            StaticSourcesViewModel.SourceChanged = false;
                        else
                            return;
                    }
                    if (SelectedProceso.Id_proceso > -1)
                    {
                        int Cantidad = 0;
                        //Parte Buscar
                        switch (SelectedProceso.DESCR)
                        {
                            case "Traslados-Salidas":
                                
                                TituloBusquedaGrid = "Certificado Médico para Traslados-Salidas";


                            //    var ListaCerificadoMedicoDetalles_TRASLADO = TrasladoDetalleControlador.ObtenerTodosTraslado().Where(where => where.ID_ATENCION_MEDICA == null && where.INGRESO.ID_UB_CENTRO == GlobalVar.gCentro && where.ID_ESTATUS.Equals("PR") || where.ID_ESTATUS.Equals("EP"))

                                var ListaCerificadoMedicoDetalles_TRASLADO = TrasladoDetalleControlador.ObtenerTodosTraslado().Where(where => where.ID_ATENCION_MEDICA == null && where.INGRESO.ID_UB_CENTRO == GlobalVar.gCentro && LstEstatusTras_Salida.Contains(where.ID_ESTATUS))
                                  .Select(s => new { Objeto = s, FOLIOCM = s.ID_IMPUTADO, ID_INGRESOCM = s.ID_INGRESO, ANIOCM = s.ID_ANIO, CENTROCM = s.ID_CENTRO, NombreCM = s.INGRESO.IMPUTADO.NOMBRE, ApPaternoCM = s.INGRESO.IMPUTADO.PATERNO, ApMaternoCM = s.INGRESO.IMPUTADO.MATERNO, IsSelectedCertificado = false });
                                var ListaCertificadoMedicoIngresos_TRASLADO_ = new List<CertificadoMedicoIngresos>();
                                Cantidad = 0;
                                foreach (var itemCertificadoMedicoDetalle in ListaCerificadoMedicoDetalles_TRASLADO)
                                {
                                    var ObjetoCrtMedico = new CertificadoMedicoIngresos();
                                    ObjetoCrtMedico.ObjetoTrasladoDetalle = itemCertificadoMedicoDetalle.Objeto;
                                    ObjetoCrtMedico.FOLIOCM = itemCertificadoMedicoDetalle.FOLIOCM;
                                    ObjetoCrtMedico.ID_INGRESOCM = itemCertificadoMedicoDetalle.ID_INGRESOCM;
                                    ObjetoCrtMedico.NombreCM = itemCertificadoMedicoDetalle.NombreCM;
                                    ObjetoCrtMedico.ApMaternoCM = itemCertificadoMedicoDetalle.ApMaternoCM;
                                    ObjetoCrtMedico.ApPaternoCM = itemCertificadoMedicoDetalle.ApPaternoCM;
                                    ObjetoCrtMedico.CENTROCM = itemCertificadoMedicoDetalle.CENTROCM;
                                    ObjetoCrtMedico.ANIOCM = itemCertificadoMedicoDetalle.ANIOCM;
                                    //ObjetoCrtMedico.IsSelectedCertificado = false;
                                    ListaCertificadoMedicoIngresos_TRASLADO_.Add(ObjetoCrtMedico);
                                    Cantidad++;
                                }

                                var ListaFiltrada_CertificadoMedicoIngresos_TRASLADO_ = new List<CertificadoMedicoIngresos>();
                                if (Cantidad > 0)
                                {
                                    ListaFiltrada_CertificadoMedicoIngresos_TRASLADO_ = new List<CertificadoMedicoIngresos>(BusquedaGenericaImputado(ListaCertificadoMedicoIngresos_TRASLADO_));
                                    ListaCertificadoMed = new ObservableCollection<CertificadoMedicoIngresos>(ListaFiltrada_CertificadoMedicoIngresos_TRASLADO_);
                                }
                                else
                                {
                                    ListaCertificadoMed = new ObservableCollection<CertificadoMedicoIngresos>();
                                }

                                break;
                            case "Excarcelación-Salidas":
                                TituloBusquedaGrid = "Certificado Médico para Excarcelación-Salidas";
                                //IsSelectedCertificado = false 

                                var ListaCerificadoMedicoDetalles_EXCARCELACION_SALIDA = ExCarcelacionControlador.ObtenerTodos().Where(where => where.CERT_MEDICO_SALIDA == null && where.INGRESO.ID_UB_CENTRO == GlobalVar.gCentro && LstEstatusExcar_SALIDA.Contains(where.ID_ESTATUS))
                                    .Select(s => new { Objeto = s, ID_IMUPATOCM = s.ID_IMPUTADO, FOLIOCM = s.ID_IMPUTADO, ID_INGRESOCM = s.ID_INGRESO, ANIOCM = s.ID_ANIO, CENTROCM = s.ID_CENTRO, NombreCM = s.INGRESO.IMPUTADO.NOMBRE, ApPaternoCM = s.INGRESO.IMPUTADO.PATERNO, ApMaternoCM = s.INGRESO.IMPUTADO.MATERNO });
                                var ListaCertificadoMedicoIngresos_EXC_SALIDA = new List<CertificadoMedicoIngresos>();
                                Cantidad = 0;
                                foreach (var itemCertificadoMedicoDetalle in ListaCerificadoMedicoDetalles_EXCARCELACION_SALIDA)
                                {
                                    var ObjetoCrtMedico = new CertificadoMedicoIngresos();
                                    ObjetoCrtMedico.ObjetoExcarcelacion = itemCertificadoMedicoDetalle.Objeto;
                                    ObjetoCrtMedico.ID_IMUPATOCM = itemCertificadoMedicoDetalle.ID_IMUPATOCM;
                                    ObjetoCrtMedico.FOLIOCM = itemCertificadoMedicoDetalle.FOLIOCM;
                                    ObjetoCrtMedico.ID_INGRESOCM = itemCertificadoMedicoDetalle.ID_INGRESOCM;
                                    ObjetoCrtMedico.NombreCM = itemCertificadoMedicoDetalle.NombreCM;
                                    ObjetoCrtMedico.ApMaternoCM = itemCertificadoMedicoDetalle.ApMaternoCM;
                                    ObjetoCrtMedico.ApPaternoCM = itemCertificadoMedicoDetalle.ApPaternoCM;
                                    ObjetoCrtMedico.CENTROCM = itemCertificadoMedicoDetalle.CENTROCM;
                                    ObjetoCrtMedico.ANIOCM = itemCertificadoMedicoDetalle.ANIOCM;
                                    ListaCertificadoMedicoIngresos_EXC_SALIDA.Add(ObjetoCrtMedico);
                                    Cantidad++;
                                }
                                var ListaFiltrada_CertificadoMedicoIngresos_EXC_SALIDA = new List<CertificadoMedicoIngresos>();
                                if (Cantidad > 0)
                                {
                                    ListaFiltrada_CertificadoMedicoIngresos_EXC_SALIDA = new List<CertificadoMedicoIngresos>(BusquedaGenericaImputado(ListaCertificadoMedicoIngresos_EXC_SALIDA));
                                    ListaCertificadoMed = new ObservableCollection<CertificadoMedicoIngresos>(ListaFiltrada_CertificadoMedicoIngresos_EXC_SALIDA);
                                }
                                else
                                {
                                    ListaCertificadoMed = new ObservableCollection<CertificadoMedicoIngresos>();
                                }
                                break;
                            case "Excarcalación-Retorno":
                                TituloBusquedaGrid = "Certificado Médico para Excarcalación-Retorno";
                                var ListaCerificadoMedicoDetalles_EXCARCELACION_RETORNO = ExCarcelacionControlador.ObtenerTodos().Where(where => where.CERT_MEDICO_RETORNO == null && where.INGRESO.ID_UB_CENTRO == GlobalVar.gCentro && where.ID_ESTATUS.Equals("CO") && where.CERTIFICADO_MEDICO == 1)
                                    .Select(s => new { Objeto = s, ID_IMUPATOCM = s.ID_IMPUTADO, FOLIOCM = s.ID_IMPUTADO, ID_INGRESOCM = s.ID_INGRESO, ANIOCM = s.ID_ANIO, CENTROCM = s.ID_CENTRO, NombreCM = s.INGRESO.IMPUTADO.NOMBRE, ApPaternoCM = s.INGRESO.IMPUTADO.PATERNO, ApMaternoCM = s.INGRESO.IMPUTADO.MATERNO });
                                var ListaCertificadoMedicoIngresos_EXC_RETORNO = new List<CertificadoMedicoIngresos>();
                                Cantidad = 0;
                                foreach (var itemCertificadoMedicoDetalle in ListaCerificadoMedicoDetalles_EXCARCELACION_RETORNO)
                                {
                                    var ObjetoCrtMedico = new CertificadoMedicoIngresos();
                                    ObjetoCrtMedico.ObjetoExcarcelacion = itemCertificadoMedicoDetalle.Objeto;
                                    ObjetoCrtMedico.FOLIOCM = itemCertificadoMedicoDetalle.FOLIOCM;
                                    ObjetoCrtMedico.ID_INGRESOCM = itemCertificadoMedicoDetalle.ID_INGRESOCM;
                                    ObjetoCrtMedico.NombreCM = itemCertificadoMedicoDetalle.NombreCM;
                                    ObjetoCrtMedico.ApMaternoCM = itemCertificadoMedicoDetalle.ApMaternoCM;
                                    ObjetoCrtMedico.ApPaternoCM = itemCertificadoMedicoDetalle.ApPaternoCM;
                                    ObjetoCrtMedico.CENTROCM = itemCertificadoMedicoDetalle.CENTROCM;
                                    ObjetoCrtMedico.ANIOCM = itemCertificadoMedicoDetalle.ANIOCM;
                                    ListaCertificadoMedicoIngresos_EXC_RETORNO.Add(ObjetoCrtMedico);
                                    Cantidad++;
                                }

                                var ListaFiltrada_CertificadoMedicoIngresos_EXC_RETORNO = new List<CertificadoMedicoIngresos>();
                                if (Cantidad > 0)
                                {
                                    ListaFiltrada_CertificadoMedicoIngresos_EXC_RETORNO = new List<CertificadoMedicoIngresos>(BusquedaGenericaImputado(ListaCertificadoMedicoIngresos_EXC_RETORNO));
                                    ListaCertificadoMed = new ObservableCollection<CertificadoMedicoIngresos>(ListaFiltrada_CertificadoMedicoIngresos_EXC_RETORNO);
                                }
                                else
                                {
                                    ListaCertificadoMed = new ObservableCollection<CertificadoMedicoIngresos>();
                                }


                                break;


                            default:
                                break;
                #endregion
                        }
                    }
                    else
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe Seleccionar un tipo de Proceso");
                    }

                    break;
                #region Proceso de Guardar Informacion
                case "menu_guardar":
                    try
                    {
                        if (ListaCertificadoMed == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un registro al cual desdea realizar comprobante médico");
                            break;
                        }
                        var listaTrasDetalle = new List<TRASLADO_DETALLE>();
                        var listaExcarcelacion = new List<EXCARCELACION>();
                        if (ListaCertificadoMed.Count() > 0)
                        {
                            var ListaComprobanteCertMedico = ListaCertificadoMed.Where(w => w.IsSelectedCertificado);
                            if (ListaComprobanteCertMedico.Count() > 0)
                            {
                                foreach (var itemTransDet in ListaComprobanteCertMedico)
                                {
                                    listaTrasDetalle.Add(itemTransDet.ObjetoTrasladoDetalle);
                                    listaExcarcelacion.Add(itemTransDet.ObjetoExcarcelacion);
                                }
                                short id_area_traslado = Parametro.ID_AREA_TRASLADO;

                                //  bool Insert = AtencionMedicaCOntrolador.Insertar_Comprobacion_CertificadoMedico(listaTrasDetalle, listaExcarcelacion, SelectedProceso.DESCR == "Traslados-Salidas" ? SelectedProceso.DESCR : "EXCARCELACIÓN", SelectedProceso.DESCR == "Excarcelación-Salidas" ? "SALIDA" : "RETORNO", id_area_traslado, Fechas.GetFechaDateServer);
                                if (AtencionMedicaCOntrolador.Insertar_Comprobacion_CertificadoMedico(listaTrasDetalle, listaExcarcelacion, SelectedProceso.DESCR == "Traslados-Salidas" ? SelectedProceso.DESCR : "EXCARCELACIÓN", SelectedProceso.DESCR == "Excarcelación-Salidas" ? "SALIDA" : "RETORNO", id_area_traslado, Fechas.GetFechaDateServer))
                                {
                                    StaticSourcesViewModel.SourceChanged = false;
                                    new Dialogos().ConfirmacionDialogo("Éxito", "La información se registro correctamente");
                                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new CrtificadomedicoCancelacion_TrasladoView();
                                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.CertificadoMedicoCancelacion_TrasladoViewModel();
                                }
                            }
                            else
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un registro al cual desdea realizar comprobante médico");
                            }
                        }
                        else
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un registro al cual desdea realizar comprobante médico");
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar", ex);
                    }
                    break;
                #endregion
                case "limpiar_menu":
                    if (StaticSourcesViewModel.SourceChanged)
                    {
                        var dialogresult = await (new Dialogos()).ConfirmarEliminar("Advertencia", "Hay cambios sin guardar, ¿Seguro que desea limpiar la pantalla sin guardar?");
                        if (dialogresult != 0)
                            StaticSourcesViewModel.SourceChanged = false;
                        else
                            return;
                    }
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new CrtificadomedicoCancelacion_TrasladoView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.CertificadoMedicoCancelacion_TrasladoViewModel();
                    break;
                case "salir_menu":
                    //if (!Changed)
                    PrincipalViewModel.SalirMenu();
                    StaticSourcesViewModel.SourceChanged = false;
                    break;

            }
        }
        #region Evento ClickEnter
        private async void ClickEnter(Object obj = null)
        {
            try
            {

                if (obj != null)
                {
                    //cuando es boton no se hace nada porque solamente existe el de buscar, si hay otro habra que castearlos a button y hacer la comparacion
                    var textbox = obj as TextBox;
                    if (textbox != null)
                    {
                        short? NULL = null;
                        switch (textbox.Name)
                        {
                            case "AnioBusc":
                               
                                AnioBuscarCertMed = !string.IsNullOrEmpty(textbox.Text) ? short.Parse(textbox.Text.ToString()) : NULL;
                                break;
                            case "ApellidoPaternoBusc":
                                ApellidoPaternoBuscarCertMed= textbox.Text;
                                break;
                            case "ApellidoMaternoBusc":
                                ApellidoMaternoBuscarCertMed= textbox.Text;
                                break;
                            case "NombreBusc":
                                NombreBuscarCertMed = textbox.Text;
                                break;
                            case "FolioBusc":
                                FolioBuscarCertMed = !string.IsNullOrEmpty(textbox.Text) ? short.Parse(textbox.Text.ToString()) : NULL;
                                break;
                            
                              
                        }
                    }
                }
                clickSwitch("buscar_menu");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar al imputado.", ex);
            }
        }
        #endregion

        #region Filtrado BusquedaGneral Imputado
        private List<CertificadoMedicoIngresos> BusquedaGenericaImputado(List<CertificadoMedicoIngresos> LstCertMedicoIngresos)
        {
          
            if (AnioBuscarCertMed.HasValue)
            {
                LstCertMedicoIngresos = new List<CertificadoMedicoIngresos>(LstCertMedicoIngresos.Where(w => w.ANIOCM == AnioBuscarCertMed));
            }
            if (FolioBuscarCertMed.HasValue)
            {
                
                LstCertMedicoIngresos = new List<CertificadoMedicoIngresos>(LstCertMedicoIngresos.Where(w =>w.FOLIOCM==FolioBuscarCertMed));
            }
            if (!string.IsNullOrEmpty(NombreBuscarCertMed))
            {
                LstCertMedicoIngresos = new List<CertificadoMedicoIngresos>(LstCertMedicoIngresos.Where(w => w.NombreCM.ToString().ToUpper().Contains(NombreBuscarCertMed.ToString().ToUpper())));

            }
            if (!string.IsNullOrEmpty(ApellidoPaternoBuscarCertMed))
            {
                LstCertMedicoIngresos = new List<CertificadoMedicoIngresos>(LstCertMedicoIngresos.Where(w => w.ApPaternoCM.ToString().ToUpper().Contains(ApellidoPaternoBuscarCertMed.ToString().ToUpper())));
            }
            if (!string.IsNullOrEmpty(ApellidoMaternoBuscarCertMed))
            {
                LstCertMedicoIngresos = new List<CertificadoMedicoIngresos>(LstCertMedicoIngresos.Where(w => w.ApMaternoCM.ToString().ToUpper().Contains(ApellidoMaternoBuscarCertMed.ToString().ToUpper())));
            }
            return LstCertMedicoIngresos;
        }
        #endregion




        private async void CertificadoMedicoCancelacion_TrasladoLoad(CrtificadomedicoCancelacion_TrasladoView obj = null)
        {
            try
            {

                await StaticSourcesViewModel.CargarDatosMetodoAsync(CargarListas);
                UserCtr = (UserControl)obj;
                 System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    StaticSourcesViewModel.SourceChanged = false;
                }));
                

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar decomisos", ex);
            }
        }

        private void CargarListas()
        {
            ListProceso = new ObservableCollection<Procesos>(Popularprocesos());
            OnPropertyChanged("ListProceso");
            // ListTipoCertificado = new ObservableCollection<TipoCertificado>(PopularTipoCertificado());
        }

        private List<Procesos> Popularprocesos()
        {
            return new List<Procesos>{
                                                     new Procesos{Id_proceso=-1,DESCR="SELECCIONE"},
                                                     new Procesos{Id_proceso=0,DESCR="Traslados-Salidas"},
                                                     new Procesos{Id_proceso=1,DESCR="Excarcalación-Retorno"},
                                                     new Procesos{Id_proceso=1,DESCR="Excarcelación-Salidas"}};

        }



    }
}
