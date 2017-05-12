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
using System.Transactions;
using Microsoft.Reporting.WinForms;
using Cogent.Biometrics;
using ControlPenales.BiometricoServiceReference;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.IO;
using System.Drawing;
using DPUruNet;
using System.Runtime.InteropServices;


namespace ControlPenales
{
    partial class VisitaDomiciliariaViewModel : FingerPrintScanner
    {

        private void clickSwitch(object op)
        {
            switch (op.ToString())
            {

                case "buscar_menu":
                    if (!PConsultar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    auxProcesoLibertad = SelectedProcesoLibertad;
                    SelectExpediente = null;
                    SelectedProcesoLibertad = null;
                    LimpiarBusqueda();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_LIBERADO);
                    break;
                case "nueva_busqueda":
                    LimpiarBusqueda();
                    break;

                case "buscar_salir":
                    SelectedProcesoLibertad = auxProcesoLibertad;
                    if (SelectedProcesoLibertad != null)
                        SelectExpediente = SelectedProcesoLibertad.IMPUTADO;
                    auxProcesoLibertad = null;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_LIBERADO);
                    break;

                case "buscar_visible":
                    //if (!PConsultar)
                    //{
                    //    (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                    //    break;
                    //}
                    ClickEnter();
                    break;
                case "nuevo_expediente":
                    //SelectMJ = null;
                    //SelectExpediente = null;
                    //Nuevo();
                    break;


                case "buscar_nueva_medida":
                    SelectMJ = null; //Se agrega nueva medida
                    //      ValidacionesLiberado();
                    Obtener();
                    break;

                case "buscar_seleccionar":
                    if (SelectedProcesoLibertad == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación","Favor de seleccionar un proceso en libertad");
                        break;
                    }
                    MenuGuardarEnabled = true;
                    //ValidacionesLiberado();
                    Obtener();
                    SelectedVisitaDomiciliaria = SelectedProcesoLibertad.PRS_VISITA_DOMICILIARIA.FirstOrDefault();
                    if (SelectedVisitaDomiciliaria != null)
                        ObtenerVisitaDomiciliaria();
                    StaticSourcesViewModel.SourceChanged = false;
                   // ValidacionDatosGenerales();
                    break;
                case "reporte_menu":
                    if (!PImprimir)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    //Creacion de Reporte
                    if (SelectedProcesoLibertad != null)
                    {
                        if (base.HasErrors)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "Debe de llenar todos los campo Obligatorios.");
                        }
                        else
                        {
                            var View = new ReportesView();
                            var DatosReporte = new cEntrevistaInicial();
                            #region Iniciliza el entorno para mostrar el reporte al usuario
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                            View.Owner = PopUpsViewModels.MainWindow;
                            View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };


                            View.Show();
                            #endregion
                            #region Se forma el cuerpo del reporte
                            #region EntrevistaInicialPrincipal
                            DatosReporte.FechaEntrv = TextFechaEntrv.HasValue ? TextFechaEntrv.Value.ToString("dd/MM/yyyy") : string.Empty;
                            DatosReporte.LugarEntrv = TextLugarEntrevista;
                            //DatosReporte.HoraEntrv = HoraEntrv.Value.Hour.ToString() + ":" + HoraEntrv.Value.Minute.ToString();
                            DatosReporte.HoraEntrv = TextFechaEntrv.HasValue ? TextFechaEntrv.Value.ToString("hh:mm tt") : string.Empty;
                            DatosReporte.FechaEntrv = TextFechaEntrv.HasValue ? TextFechaEntrv.Value.ToString("dd/MM/yyyy") : string.Empty;
                            DatosReporte.NombreInterno = ApellidoPaternoBuscar + " " + apellidoMaternoBuscar + " " + NombreBuscar;
                            #endregion

                            DatosReporte.MotivooEntrv = TextMotivoVisita != null ? TextMotivoVisita.Trim() : "";
                            DatosReporte.NombreInterno = ApellidoPaternoBuscar + " " + apellidoMaternoBuscar + " " + NombreBuscar;
                            DatosReporte.EstadoCivil = SelectEstadoCivil > -1 ? LstEstadoCivil.Where(w => w.ID_ESTADO_CIVIL == SelectEstadoCivil).FirstOrDefault().DESCR : string.Empty;
                            DatosReporte.EdadInterno = TextEdad.ToString();
                            DatosReporte.FechaNacimiento = textFechaNacimiento.HasValue ? textFechaNacimiento.Value.ToString("dd/MM/yyyy") : "";
                            DatosReporte.Domicilio = !string.IsNullOrEmpty(TextCalle) ? TextCalle : string.Empty + " #" + TextNumeroExterior != null ? TextNumeroExterior.ToString() : string.Empty + " #" + TextNumeroInterior != null ? TextNumeroInterior.ToString() : string.Empty;
                            DatosReporte.Pais=SelectPaisNacimiento>-1? ListPaisNacimiento.Where(w=>w.ID_PAIS_NAC==SelectPaisNacimiento).FirstOrDefault().PAIS:"";
                            DatosReporte.Municipio = SelectMunicipioNacimiento > -1 ? ListMunicipioNacimiento.Where(w => w.ID_MUNICIPIO == SelectMunicipioNacimiento).FirstOrDefault().MUNICIPIO1 : string.Empty;
                            DatosReporte.Telefono = TextTelefono.ToString();
                            DatosReporte.EntidadFederativa = SelectEntidadNacEntrv > -1 ? ListEntidadNacimientoEntrv.Where(w => w.ID_ENTIDAD == SelectEntidadNacEntrv).FirstOrDefault().DESCR : string.Empty;
                            DatosReporte.UltimoGradoEstudio = SelectEscolaridad.Value > -1 ? ListEscolaridad.Where(w => w.ID_ESCOLARIDAD == SelectEscolaridad.Value).FirstOrDefault().DESCR : string.Empty;
                            DatosReporte.Ocupacion = SelectOcupacion > -1 ? ListOcupacion.Where(w => w.ID_OCUPACION == SelectOcupacion).FirstOrDefault().DESCR : string.Empty;
                            DatosReporte.MedidaCautelar = TextMedidaCautelar;
                            //DatosReporte.Pais = ListPaisDomicilio.Where(w => w.ID_PAIS_NAC == selectPais).FirstOrDefault().PAIS;
                            DatosReporte.Estado = SelectEntidadNacimiento > -1 ? ListEntidadNacimiento.Where(w => w.ID_ENTIDAD == SelectEntidadNacimiento).FirstOrDefault().DESCR : string.Empty;
                            if (ListMunicipio != null)
                                DatosReporte.Municipio = SelectMunicipioNacimiento > -1 ? ListMunicipio.Where(w => w.ID_MUNICIPIO == SelectMunicipioNacimiento).FirstOrDefault().MUNICIPIO1 : string.Empty;
                            DatosReporte.Observacion = TextObservaciones;
                            DatosReporte.NombreCroquis = TextNombreCroquis;
                            DatosReporte.DireccionCroquis = TextDireccionCroquis;
                            DatosReporte.TelefonoCroquis = TextTelCroquis;
                            //DatosReporte.Domicilio = string.Format("{0} {1} {2} {3}", SelectExpediente.DOMICILIO_CALLE, SelectExpediente.DOMICILIO_NUM_EXT.HasValue ? SelectExpediente.DOMICILIO_NUM_EXT != decimal.Zero ? 
                            //(SelectExpediente.MUNICIPIO.MUNICIPIO1) ? SelectExpediente.MUNICIPIO.MUNICIPIO1.Trim() : string.Empty : string.Empty); 
                            DatosReporte.Domicilio = string.Format("Calle:{0} Num Exterior:{1} Num Interior{2}", TextCalle, TextNumeroExterior, TextNumeroInterior);
                            DatosReporte.Sexo = selectExpediente.SEXO;
                            DatosReporte.Fecha = Fechas.GetFechaDateServer.ToShortDateString();
                            DatosReporte.oficio = textOficio;
                            DatosReporte.EstadoCivil = LstEstadoCivil.Where(w => w.ID_ESTADO_CIVIL == (short)SelectEstadoCivil.Value).FirstOrDefault().DESCR;
                           
                            #region Situacion Juridica
                            DatosReporte.Delito = TextDelitoImputa;
                            #region Datos Personal Enrevistadas
                            DatosReporte.NombreEntrevistado = TextNombreEntrevistado != null ? TextNombreEntrevistado.Trim() : "";
                            DatosReporte.EaddEntrevistado = TextEdadEntrevistado != null ? TextEdadEntrevistado.Trim() : "";
                            DatosReporte.DomicilioConocerceEntrevistado = string.Format("Calle: {0} NumInterior: {1} NumExterior: {2}", TextCalleEntrevistado, TextNumeroInteriorEntrevistado != null ? TextNumeroInteriorEntrevistado.Trim() : "", TextNumeroExteriorEntrevistado);
                            DatosReporte.TelefonoConocerceEntrevistado = TextTelefonoEntrevistado != null ? TextTelefonoEntrevistado.Trim() : "";
                            DatosReporte.TiempoConocerceEntrevistado = TextTiempoConocerceEntrvistado != null ? TextTiempoConocerceEntrvistado.Trim() : "";
                            DatosReporte.RelacionSentenciadoEntrevistado = TextRelacionSentenciadoEntrevistado != null ? TextRelacionSentenciadoEntrevistado.Trim() : "";
                            DatosReporte.EntidadfederativaEntrevistado = SelectEntidadNacEntrv > -1 ? ListEntidadNacimientoEntrv.Where(w => w.ID_ENTIDAD == SelectEntidadNacEntrv).FirstOrDefault().DESCR : "";
                            DatosReporte.MunicipioEntrevistado = SelectMunicipioNacEntrv > -1 ? ListMunicipioNacimientoEntrv.Where(w => w.ID_MUNICIPIO == SelectMunicipioNacEntrv).FirstOrDefault().MUNICIPIO1 : "";
                            DatosReporte.ParentescoEntrevistado = SelectParentesco > -1 ? ListParentesco.Where(w => w.ID_TIPO_REFERENCIA == SelectParentesco).FirstOrDefault().DESCR : "";
                            #endregion

                            #region Agrega Fotografias Reporte
                            int cont=0;
                            //RECORRE TODAS LAS IMAGENES RELACIONADAS SI NO EXISTE NINGUNA NO  SERA VISISBLE EN EL REPORTE
                            foreach (var itemFotografias in ListasFotografias.OrderBy(o=>o.ID_FOTO))
                            {
                                cont++;
                                switch (cont)
                            	{
                                    case 1://MUESTRA LA PRIMERA IMAGEN RELACIONADA AL IMPUTADO SELECCIONADO
                                        DatosReporte.Fotografias1 = itemFotografias.FOTOGRAFIA;
                                        break;
                                    case 2://MUESTRA LA SEGUNDA IMAGEN RELACIONADA AL IMPUTADO SELECCIONADO
                                        DatosReporte.Fotografias2 = itemFotografias.FOTOGRAFIA;
                                        break;
                                    case 3://MUESTRA LA TERCERA IMAGEN RELACIONADA AL IMPUTADO SELECCIONADO
                                        DatosReporte.Fotografias3 = itemFotografias.FOTOGRAFIA;
                                        break;
                                    case 4://MUESTRA LA CUARTA IMAGEN RELACIONADA AL IMPUTADO SELECCIONADO
                                        DatosReporte.Fotografias4 = itemFotografias.FOTOGRAFIA;
                                        break;
                                	}
                                
                            }
                            #endregion

                            //DatosReporte.Delito = ;
                            //DatosReporte.MedidaCautelar=SelectMJ.MEDIDA_JUDICIAL;
                            // }
                            #endregion

                            #region Se forma el encabezado del reporte
                            var Encabezado = new cEncabezado();
                            Encabezado.TituloUno = "JUSTICIA RESTAURATIVA";
                            Encabezado.TituloDos = Parametro.ENCABEZADO2;//SUBSECRETARIA
                            Encabezado.NombreReporte = "TRABAJO SOCIAL";
                            Encabezado.ImagenIzquierda = Parametro.REPORTE_LOGO1;
                            Encabezado.ImagenFondo = Parametro.REPORTE_LOGO2;
                            #endregion

                            #region Inicializacion de reporte
                            View.Report.ProcessingMode = ProcessingMode.Local;
                            View.Report.LocalReport.ReportPath = "Reportes/rVisitaDomiciliaria.rdlc";
                            View.Report.LocalReport.DataSources.Clear();
                            #endregion

                            #region Definicion de origenes de datos


                            //datasource Uno
                            var ds1 = new List<cEncabezado>();
                            ds1.Add(Encabezado);
                            Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                            rds1.Name = "DataSet2";
                            rds1.Value = ds1;
                            View.Report.LocalReport.DataSources.Add(rds1);


                            //datasource dos
                            var ds2 = new List<cEntrevistaInicial>();
                            ds2.Add(DatosReporte);
                            Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                            rds2.Name = "DataSet1";
                            rds2.Value = ds2;
                            View.Report.LocalReport.DataSources.Add(rds2);



                            //View.Report.LocalReport.DisplayName = string.Format("Fehca de Identificación{0} {1} {2} ",//Nombre del archivo que se va a generar con el reporte independientemente del formato que se elija
                            //    SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty ,
                            //    SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty,
                            //    SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty);


                            View.Report.ReportExport += (s, e) =>
                            {
                                try
                                {
                                    if (e != null && !e.Extension.LocalizedName.Equals("PDF"))//Solo permite pdf
                                    {
                                        e.Cancel = true;//Detiene el evento
                                        s = e = null;//Detiene el mapeo de los parametros para que no continuen en el arbol
                                        (new Dialogos()).ConfirmacionDialogo("Validación", "No tiene permitido exportar la información en este formato.");
                                    }
                                }

                                catch (Exception exc)
                                {
                                    throw exc;
                                }
                            };

                            View.Report.RefreshReport();
                        }
                    }
                    else { new Dialogos().ConfirmacionDialogo("Validación", "Favor de guardar antes de imprimir."); }

                            #endregion
                    break;

                case "limpiar_menu":
                    
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new VisitaDomiciliariaView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.VisitaDomiciliariaViewModel();
                    TabDatosPersonaEntrev = false; SelectDatosPersonaEntrev=false;
                    TabDatosGenerales = false;  SelectDatosgenerales=false;
                    TabDatosAnexos = false;SelectDatosAnexos=false;
                    break;
                case "guardar_menu":
                    if (SeleccionarProcesoEnabled == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación","Favor de seleccionar un proceso en libertad");
                        break;
                    }
                    Guardar();
                    break;
                case "Open442":
                     if (!pConsultar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        return;
                    }
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.HUELLAS);
                    this.ShowIdentification();
                    break;
                case "salir_menu":
                    //if (!Changed)
                    //    StaticSourcesViewModel.SourceChanged = false;
                    PrincipalViewModel.SalirMenu();
                    break;

            }
        }

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
                        switch (textbox.Name)
                        {
                            case "NUCBuscar":
                                NUCBuscar = textbox.Text;
                                break;
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
                                if (!string.IsNullOrWhiteSpace(textbox.Text))
                                    FolioBuscar = Convert.ToInt32(textbox.Text);
                                else
                                    FolioBuscar = null;
                                break;
                            case "AnioBuscar":
                                if (!string.IsNullOrWhiteSpace(textbox.Text))
                                    AnioBuscar = short.Parse(textbox.Text);
                                else
                                    AnioBuscar = null;
                                break;
                        }
                    }
                }
                ImagenIngreso = ImagenImputado = new Imagenes().getImagenPerson();
                #region comentado
                //ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                //ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                //if (ListExpediente != null)
                //    EmptyExpedienteVisible = ListExpediente.Count < 0;
                //else
                //    EmptyExpedienteVisible = true;
                #endregion
                #region nuevo
                LstLiberados = new RangeEnabledObservableCollection<cLiberados>();
                LstLiberados.InsertRange(await SegmentarResultadoBusquedaLiberados());
                EmptyExpedienteVisible = LstLiberados.Count > 0 ? false : true;
                #endregion
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_LIBERADO);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar al imputado.", ex);
            }
        }
        
        private async void VisitaDomiciliariaLoad(VisitaDomiciliariaView obj = null)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(CargarListas);
                MaxWidhtActualGeneric = ((TextBlock)obj.FindName("lblHoraVisita")).ActualWidth - 60;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar decomisos", ex);
            }
        }

        private async void AnexosLoad(AnexosView obj = null)
        {
            try
            {
                ActualWidthtelefonoCroquis = ((TextBlock)obj.FindName("lbltelefonoCroquis")).ActualWidth;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar decomisos", ex);
            }
        }

        private void CargarListas()
        {
            try
            {
                ConfiguraPermisos();
                LstEstadoCivil = new ObservableCollection<ESTADO_CIVIL>(new cEstadoCivil().ObtenerTodos());
                ListOcupacion = new ObservableCollection<OCUPACION>(new cOcupacion().ObtenerTodos());
                ListPaisNacimiento = new ObservableCollection<PAIS_NACIONALIDAD>(new cPaises().ObtenerNacionalidad());
                ListEscolaridad = new ObservableCollection<ESCOLARIDAD>(new cEscolaridad().ObtenerTodos());
                ListPaisNacimientoEntrv = new ObservableCollection<PAIS_NACIONALIDAD>(new cPaises().ObtenerNacionalidad());
                ListParentesco = new ObservableCollection<TIPO_REFERENCIA>(new cTipoReferencia().ObtenerTodos());


                System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                {

                    LstEstadoCivil.Insert(0, new ESTADO_CIVIL() { ID_ESTADO_CIVIL = -1, DESCR = "SELECCIONE" });
                    ListOcupacion.Insert(0, new OCUPACION() { ID_OCUPACION = -1, DESCR = "SELECCIONE" });
                    ListPaisNacimiento.Insert(0, new PAIS_NACIONALIDAD() { ID_PAIS_NAC = -1, PAIS = "SELECCIONE" });
                    ListParentesco.Insert(0, new TIPO_REFERENCIA() { ID_TIPO_REFERENCIA = -1, DESCR = "SELECCIONE" });
                    ListEscolaridad.Insert(0, new ESCOLARIDAD() { ID_ESCOLARIDAD = -1, DESCR = "SELECCIONE" });
                    ListPaisNacimientoEntrv.Insert(0, new PAIS_NACIONALIDAD() { ID_PAIS_NAC = -1, PAIS = "SELECCIONE" });
                    //    ValidacionesLiberado();
                    //  ValidacionCroquis();
                    // ValidacionCroquis();
                    //pendiente   ConfiguraPermisos();


                }));

                AsignarLenght();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar buscar por nuc", ex);
            }
        }

        private void AsignarLenght()
        {
            LugarMax = 100;
            HoraMax = 20;
            NombreEntrevistadoMax = 50;
            CalleEntrvMax = 70;
            TiempoConocerMax = 50;
            ObservacionesMax = 100;
            TiempoConocerleMax = 50;

            NombreCroquisMax = 100;
            DireccionCroquisMax = 100;
            RelacionSentenciadoMax = 100;
            MedidaCutMax = 100;
        }

        private async Task<List<IMPUTADO>> SegmentarResultadoBusqueda(int _Pag = 1)
        {
            if (string.IsNullOrEmpty(ApellidoPaternoBuscar) && string.IsNullOrEmpty(ApellidoMaternoBuscar) && string.IsNullOrEmpty(NombreBuscar) && !AnioBuscar.HasValue && !FolioBuscar.HasValue)
                return new List<IMPUTADO>();

            Pagina = _Pag;
            var result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<IMPUTADO>>(() => new cImputado().ObtenerTodos(ApellidoPaternoBuscar, ApellidoMaternoBuscar, NombreBuscar, AnioBuscar, FolioBuscar, _Pag));
            if (result.Any())
            {
                Pagina++;
                SeguirCargando = true;
            }
            else
                SeguirCargando = false;

            return result.ToList();
        }

        private async Task<List<cLiberados>> SegmentarResultadoBusquedaLiberados(int _Pag = 1)
        {
            if (string.IsNullOrEmpty(ApellidoPaternoBuscar) && string.IsNullOrEmpty(ApellidoMaternoBuscar) && string.IsNullOrEmpty(NombreBuscar) && string.IsNullOrEmpty(NUCBuscar) && !AnioBuscar.HasValue && !FolioBuscar.HasValue)
                return new List<cLiberados>();

            Pagina = _Pag;
            var result = await StaticSourcesViewModel.CargarDatosAsync<IEnumerable<cLiberados>>(() => new cImputado().ObtenerLiberados(NUCBuscar,AnioBuscar,FolioBuscar, NombreBuscar, ApellidoPaternoBuscar, ApellidoMaternoBuscar, _Pag).Select(w => new cLiberados()
            {
                ID_CENTRO = w.ID_CENTRO,
                ID_ANIO = w.ID_ANIO,
                ID_IMPUTADO = w.ID_IMPUTADO,
                CENTRO = w.CENTRO,
                NOMBRE = string.Format("{0}{1}", w.NOMBRE, !string.IsNullOrEmpty(w.APODO_NOMBRE) ? "(" + w.APODO_NOMBRE + ")" : string.Empty),
                PATERNO = string.Format("{0}{1}", w.PATERNO, !string.IsNullOrEmpty(w.PATERNO_A) ? "(" + w.PATERNO_A + ")" : string.Empty),
                MATERNO = string.Format("{0}{1}", w.MATERNO, !string.IsNullOrEmpty(w.MATERNO_A) ? "(" + w.MATERNO_A + ")" : string.Empty)
            }));
            if (result.Any())
            {
                Pagina++;
                SeguirCargando = true;
            }
            else
                SeguirCargando = false;

            return result.ToList();
        }
      
        private void Guardar()
        {
            try
            {
                if (base.HasErrors)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar los datos requeridos");
                    return;
                }
                else
                {
                    if (SelectExpediente != null)
                    {
                        var objVisitaDomcilliaria = new PRS_VISITA_DOMICILIARIA();
                        objVisitaDomcilliaria.LUGAR = TextLugarEntrevista.Trim();
                        objVisitaDomcilliaria.FECHA_VISITA = TextFechaEntrv;
                        //objVisitaDomcilliaria.HORA_VISITA = HoraEntrv.Value.Hour.ToString() + ":" + HoraEntrv.Value.Minute.ToString();

                        #region Datos Generales
                        objVisitaDomcilliaria.ID_IMPUTADO = SelectedProcesoLibertad.ID_IMPUTADO;
                        objVisitaDomcilliaria.ID_ANIO = SelectedProcesoLibertad.ID_ANIO;
                        objVisitaDomcilliaria.ID_CENTRO = SelectedProcesoLibertad.ID_CENTRO;
                        objVisitaDomcilliaria.ID_PROCESO_LIBERTAD = SelectedProcesoLibertad.ID_PROCESO_LIBERTAD;
                        objVisitaDomcilliaria.MEDIDA_CAUTELAR = TextMedidaCautelar.Trim();
                        #endregion

                        #region Datos Persona Entrevistada
                        objVisitaDomcilliaria.III12_NOMBRE_ENTREVISTADO = TextNombreEntrevistado.Trim();
                        objVisitaDomcilliaria.III13_EDAD = TextEdadEntrevistado.Trim();
                        objVisitaDomcilliaria.III14_PARENTESCO = SelectParentesco;

                        objVisitaDomcilliaria.III16_CALLE = TextCalleEntrevistado;// = objVisitaDomcilliaria.III16_CALLE;
                        objVisitaDomcilliaria.III16_NUM_EXTERIOR = TextNumeroExteriorEntrevistado;// = objVisitaDomcilliaria.III16_NUM_EXTERIOR;
                        objVisitaDomcilliaria.III16_NUM_INTERIOR = TextNumeroInteriorEntrevistado != null ? TextNumeroInteriorEntrevistado.Trim() : "";//" = objVisitaDomcilliaria.III16_NUM_INTERIOR;
                        //objVisitaDomcilliaria.III16_DOMICILIO =( TextCalleEntrevistado + " " + "#Exterior " + TextNumeroExterior + " " + "#Interior " + TextNumeroInteriorEntrevistado!= null?TextNumeroInteriorEntrevistado.Trim():"").Trim();
                        objVisitaDomcilliaria.III17_TELEFONO = TextTelefonoEntrevistado.Trim();

                        objVisitaDomcilliaria.III18_ENTIDAD_FEDERATIVA = SelectEntidadNacEntrv > -1 ? SelectEntidadNacEntrv : null;
                        objVisitaDomcilliaria.III19_MUNICIPIO = SelectMunicipioNacEntrv > -1 ? SelectMunicipioNacEntrv : null;

                        objVisitaDomcilliaria.III20_RELACION_SENTENCIADO = TextRelacionSentenciadoEntrevistado.Trim();
                        objVisitaDomcilliaria.IV_OBSERVACIONES = TextObservaciones.Trim();
                        objVisitaDomcilliaria.III15_TIEMPO_CONOCERSE = TextTiempoConocerceEntrvistado.Trim();
                        objVisitaDomcilliaria.IIMOTIVO_VISITA = TextMotivoVisita.Trim();
                        #endregion
                        
                        #region Croquis
                        objVisitaDomcilliaria.ANEXO1_NOMBRE = TextNombreCroquis.Trim();
                        objVisitaDomcilliaria.ANEXO1_DIRECCION = TextDireccionCroquis.Trim();
                        objVisitaDomcilliaria.ANEXO1_TELEFONO = TextTelCroquis.Trim();
                        #endregion

                        #region Fotografias
                        var fotos = new List<PRS_VISITA_FOTOGRAFIA>();
                        if (ListasFotografias != null)
                        {
                            foreach (var itemFotos in ListasFotografias.Where(w => w.ID_FOLIO == 0))
                            {
                                fotos.Add(new PRS_VISITA_FOTOGRAFIA()
                                {
                                    ID_ANIO = itemFotos.ID_ANIO,
                                    ID_CENTRO = itemFotos.ID_CENTRO,
                                    ID_IMPUTADO = itemFotos.ID_IMPUTADO,
                                    ID_FOLIO = itemFotos.ID_FOLIO,
                                    FOTOGRAFIA = itemFotos.FOTOGRAFIA,
                                });
                            }
                        }
                        #endregion

                        if (SelectedVisitaDomiciliaria != null)
                        {
                            if (!PEditar)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                                return;
                            }
                            objVisitaDomcilliaria.ID_FOLIO = SelectedVisitaDomiciliaria.ID_FOLIO;
                            if (new cVisitaDomiciliaria().Actualizar(objVisitaDomcilliaria, fotos))
                            {
                                StaticSourcesViewModel.SourceChanged = false;
                                new Dialogos().ConfirmacionDialogo("Éxito", "La información se guardo correctamente"); 
                            }
                            else
                                new Dialogos().ConfirmacionDialogo("Error", "Ocurrio un error al guardar la información");
                        }
                        else
                        {
                            if (!PInsertar)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                                return;
                            }
                            objVisitaDomcilliaria.PRS_VISITA_FOTOGRAFIA = fotos;
                            objVisitaDomcilliaria.ID_FOLIO = new cVisitaDomiciliaria().Insertar(objVisitaDomcilliaria);
                            if (objVisitaDomcilliaria.ID_FOLIO > 0)
                            {
                                SelectedVisitaDomiciliaria = new cVisitaDomiciliaria().Obtener(
                                    objVisitaDomcilliaria.ID_CENTRO,
                                    objVisitaDomcilliaria.ID_ANIO,
                                    objVisitaDomcilliaria.ID_IMPUTADO,
                                    objVisitaDomcilliaria.ID_FOLIO);
                                StaticSourcesViewModel.SourceChanged = false;
                                new Dialogos().ConfirmacionDialogo("Éxito", "La información se guardo correctamente");
                            }
                            else
                                new Dialogos().ConfirmacionDialogo("Error", "Ocurrio un error al guardar la información");
                        }
                        #region comentado
                        //Falta ver como se Agregaran las Fotografias
                        //using (var transaction = new TransactionScope())
                        //{
                            //try
                            //{
                            //    if (new cVisitaDomiciliaria().Insertar(objVisitaDomcilliaria))
                            //    {
                            //        MenuGuardarEnabled = false;
                            //        StaticSourcesViewModel.SourceChanged = false;
                            //        #region Fotografias
                            //        //bool OcurrioErro = false;
                            //        //foreach (var itemFotos in ListasFotografias.Where(w => w.ID_FOLIO == 0))
                            //        //{
                            //        //    itemFotos.ID_FOLIO = objVisitaDomcilliaria.ID_FOLIO;
                            //        //    if (!new cFotografias().Insertar(new PRS_VISITA_FOTOGRAFIA()
                            //        //    {

                            //        //        ID_ANIO = itemFotos.ID_ANIO,
                            //        //        ID_CENTRO = itemFotos.ID_CENTRO,
                            //        //        ID_IMPUTADO = itemFotos.ID_IMPUTADO,
                            //        //        ID_FOLIO = itemFotos.ID_FOLIO,
                            //        //        FOTOGRAFIA = itemFotos.FOTOGRAFIA,
                            //        //        PRS_VISITA_DOMICILIARIA = null
                            //        //    }))
                            //        //    {
                            //        //        OcurrioErro = true;

                            //        //    }
                            //        //}
                            //        #endregion
                            //        //if (!OcurrioErro)
                            //        //{
                            //            //transaction.Complete();
                            //            //transaction.Dispose();
                            //            new Dialogos().ConfirmacionDialogo("Éxito", "La información se registro correctamente");
                            //            //((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new VisitaDomiciliariaView();
                            //            //((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.VisitaDomiciliariaViewModel();
                            //        //}
                            //    }

                            //}
                            //catch (Exception ex)
                            //{
                            //    //transaction.Dispose();
                            //    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar", ex);
                            //    //((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new VisitaDomiciliariaView();
                            //    //((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.VisitaDomiciliariaViewModel();
                            //}
                        //}
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al Al Guardar la informacion", ex);
            }
        }

        private string MetodogenericoValidacionTextBox(string ObjValidar)
        {

            return !string.IsNullOrEmpty(ObjValidar) ? ObjValidar.Trim() : "";
        }

        private void ObtenerVisitaDomiciliaria()
        {
            try
            {
                if (SelectedVisitaDomiciliaria != null)
                {
                    TextLugarEntrevista = SelectedVisitaDomiciliaria.LUGAR;
                    TextFechaEntrv = SelectedVisitaDomiciliaria.FECHA_VISITA;

                    #region Datos Generales
                    TextMedidaCautelar = SelectedVisitaDomiciliaria.MEDIDA_CAUTELAR;
                    #endregion

                    #region Datos Persona Entrevistada
                    TextNombreEntrevistado = SelectedVisitaDomiciliaria.III12_NOMBRE_ENTREVISTADO;
                    TextEdadEntrevistado = SelectedVisitaDomiciliaria.III13_EDAD;
                    SelectParentesco = SelectedVisitaDomiciliaria.III14_PARENTESCO;

                    TextCalleEntrevistado = SelectedVisitaDomiciliaria.III16_CALLE;
                    TextNumeroExteriorEntrevistado = SelectedVisitaDomiciliaria.III16_NUM_EXTERIOR;
                    TextNumeroInteriorEntrevistado = SelectedVisitaDomiciliaria.III16_NUM_INTERIOR;
                    //objVisitaDomcilliaria.III16_DOMICILIO =( TextCalleEntrevistado + " " + "#Exterior " + TextNumeroExterior + " " + "#Interior " + TextNumeroInteriorEntrevistado!= null?TextNumeroInteriorEntrevistado.Trim():"").Trim();
                    TextTelefonoEntrevistado = SelectedVisitaDomiciliaria.III17_TELEFONO;

                    SelectEntidadNacEntrv = SelectedVisitaDomiciliaria.III18_ENTIDAD_FEDERATIVA.HasValue ? SelectedVisitaDomiciliaria.III18_ENTIDAD_FEDERATIVA : -1;
                    SelectMunicipioNacEntrv = SelectedVisitaDomiciliaria.III19_MUNICIPIO.HasValue ? SelectedVisitaDomiciliaria.III19_MUNICIPIO : -1;

                    TextRelacionSentenciadoEntrevistado = SelectedVisitaDomiciliaria.III20_RELACION_SENTENCIADO;
                    TextObservaciones = SelectedVisitaDomiciliaria.IV_OBSERVACIONES;
                    TextTiempoConocerceEntrvistado = SelectedVisitaDomiciliaria.III15_TIEMPO_CONOCERSE;
                    TextMotivoVisita = SelectedVisitaDomiciliaria.IIMOTIVO_VISITA;
                    #endregion

                    #region Croquis
                    TextNombreCroquis = SelectedVisitaDomiciliaria.ANEXO1_NOMBRE;
                    TextDireccionCroquis = SelectedVisitaDomiciliaria.ANEXO1_DIRECCION;
                    TextTelCroquis = SelectedVisitaDomiciliaria.ANEXO1_TELEFONO;
                    #endregion 

                    #region Fotos
                    if (SelectedVisitaDomiciliaria.PRS_VISITA_FOTOGRAFIA != null)
                    {
                        ListasFotografias = new ObservableCollection<PRS_VISITA_FOTOGRAFIA>(SelectedVisitaDomiciliaria.PRS_VISITA_FOTOGRAFIA);
                        foreach (var l in ListasFotografias)
                        {
                            if (ImagenSuperiorIzquierda == null)
                                ImagenSuperiorIzquierda = l.FOTOGRAFIA;
                            else
                            if (ImagenSuperiorDerecha == null)
                                ImagenSuperiorDerecha = l.FOTOGRAFIA;
                            else
                            if (ImagenInferiorIzquierda == null)
                                ImagenInferiorIzquierda = l.FOTOGRAFIA;
                            else
                            if (ImagenInferiorDerecha == null)
                                ImagenInferiorDerecha = l.FOTOGRAFIA;
                        }
                        //ImagenSuperiorIzquierda
                    }
                    #endregion


                }

                #region Comentado
                //if (SelectExpediente != null)
                //{
                //    var objVisitaDomcilliaria = new cVisitaDomiciliaria().ObtenerUltimoFolio(SelectExpediente.ID_ANIO, SelectExpediente.ID_CENTRO, SelectExpediente.ID_IMPUTADO);
                //    if (objVisitaDomcilliaria != null)
                //    {
                //        TextLugarEntrevista = MetodogenericoValidacionTextBox(objVisitaDomcilliaria.LUGAR);
                //        TextFechaEntrv = objVisitaDomcilliaria.FECHA_VISITA;
                //        //   HoraEntrv = objVisitaDomcilliaria.HORA_VISITA;
                //        #region Datos Generales
                //        //objVisitaDomcilliaria.ID_IMPUTADO = SelectExpediente.ID_IMPUTADO;
                //        //objVisitaDomcilliaria.ID_ANIO = SelectExpediente.ID_ANIO;
                //        //objVisitaDomcilliaria.ID_CENTRO = SelectExpediente.ID_CENTRO;
                //        TextMedidaCautelar = MetodogenericoValidacionTextBox(objVisitaDomcilliaria.MEDIDA_CAUTELAR);


                //        #endregion

                //        #region Datos Persona Entrevistada
                //        TextNombreEntrevistado = MetodogenericoValidacionTextBox(objVisitaDomcilliaria.III12_NOMBRE_ENTREVISTADO);
                //        TextEdadEntrevistado = MetodogenericoValidacionTextBox(objVisitaDomcilliaria.III13_EDAD);
                //        SelectParentesco = objVisitaDomcilliaria.III14_PARENTESCO;
                //        //objVisitaDomcilliaria.III16_DOMICILIO = (TextCalleEntrevistado + " " + "#Exterior " + TextNumeroExterior + " " + "#Interior " + TextNumeroInteriorEntrevistado).Trim();
                //        TextCalleEntrevistado = MetodogenericoValidacionTextBox(objVisitaDomcilliaria.III16_CALLE);
                //        TextNumeroExteriorEntrevistado = MetodogenericoValidacionTextBox(objVisitaDomcilliaria.III16_NUM_EXTERIOR);
                //        TextNumeroInteriorEntrevistado = MetodogenericoValidacionTextBox(objVisitaDomcilliaria.III16_NUM_INTERIOR);
                //        TextTelefonoEntrevistado = MetodogenericoValidacionTextBox(objVisitaDomcilliaria.III17_TELEFONO);

                //        SelectPaisNacEntrv = 82;
                //        SelectEntidadNacEntrv = objVisitaDomcilliaria.III18_ENTIDAD_FEDERATIVA;
                //        SelectMunicipioNacEntrv = objVisitaDomcilliaria.III19_MUNICIPIO;

                //        //TextHoraEntrevista = objVisitaDomcilliaria.HORA_VISITA;

                //        //SelectPaisNacimiento = objVisitaDomcilliaria.
                //        TextRelacionSentenciadoEntrevistado = MetodogenericoValidacionTextBox(objVisitaDomcilliaria.III20_RELACION_SENTENCIADO);
                //        TextObservaciones = MetodogenericoValidacionTextBox(objVisitaDomcilliaria.IV_OBSERVACIONES);
                //        TextTiempoConocerceEntrvistado = MetodogenericoValidacionTextBox(objVisitaDomcilliaria.III15_TIEMPO_CONOCERSE);
                //        TextMotivoVisita = MetodogenericoValidacionTextBox(objVisitaDomcilliaria.IIMOTIVO_VISITA);

                //        #endregion

                //        #region Croquis
                //        TextNombreCroquis = MetodogenericoValidacionTextBox(objVisitaDomcilliaria.ANEXO1_NOMBRE);
                //        TextDireccionCroquis = MetodogenericoValidacionTextBox(objVisitaDomcilliaria.ANEXO1_DIRECCION);
                //        TextTelCroquis = MetodogenericoValidacionTextBox(objVisitaDomcilliaria.ANEXO1_TELEFONO);
                //        #endregion

                //        #region Fotografias

                //        #endregion

                //    }
                //    else
                //    {
                //        LimpiarControles();
                //    }

                //}
                #endregion
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener la informacion", ex);

            }
        }

        private void LimpiarControles()
        {

            TextLugarEntrevista = TextNombreEntrevistado = TextEdadEntrevistado = TextCalleEntrevistado = TextNumeroInteriorEntrevistado = TextNumeroExteriorEntrevistado = TextTelefonoEntrevistado = TextTiempoConocerceEntrvistado = TextRelacionSentenciadoEntrevistado = TextObservaciones = TextNombreCroquis = TextTelCroquis = TextDireccionCroquis = string.Empty;
            SelectParentesco = -1;
            SelectPaisNacEntrv = Parametro.PAIS; //82;
            SelectEntidadNacEntrv = Parametro.ESTADO; //2;
            SelectMunicipioNacEntrv = 1002;
            TextFechaEntrv = null;
            TextMedidaCautelar = string.Empty;
            TextMotivoVisita = string.Empty;
            OnPropertyChanged("TextMotivoVisita");
            OnPropertyChanged("TextMedidaCautelar");
            //HoraEntrv = null;
        }

        private void Obtener()
        {
            try
            {
                TabControlVisible = System.Windows.Visibility.Visible;
                EnableDatosEntrv = true;
                TabsEnabled = true;
                //SelectMJ != null && 
                if (SelectExpediente != null)
                {
                    var ingreso = SelectExpediente.INGRESO.OrderByDescending(w => w.ID_INGRESO).FirstOrDefault();
                    short id = -1;
                    MenuGuardarEnabled = true;
                    NUCBuscar = SelectedProcesoLibertad.NUC;
                    AnioBuscar = SelectExpediente.ID_ANIO;
                    FolioBuscar = SelectExpediente.ID_IMPUTADO;
                    NombreBuscar = SelectExpediente.NOMBRE;
                    ApellidoPaternoBuscar = SelectExpediente.PATERNO;
                    ApellidoMaternoBuscar = SelectExpediente.MATERNO;
                    SelectSexo = SelectExpediente.SEXO;
                    SelectEstadoCivil = SelectedProcesoLibertad.ID_ESTADO_CIVIL != null ? SelectedProcesoLibertad.ID_ESTADO_CIVIL : -1;

                    TextFechaNacimiento = SelectExpediente.NACIMIENTO_FECHA;

                    TextTelefono = SelectedProcesoLibertad.TELEFONO.ToString();

                    SelectOcupacion = SelectedProcesoLibertad.ID_OCUPACION != null ? SelectedProcesoLibertad.ID_OCUPACION : -1;
                    TextCalle = SelectedProcesoLibertad.DOMICILIO_CALLE;
                    //TextNumeroExterior = SelectExpediente.DOMICILIO_NUM_EXT;
                    //TextNumeroInterior = !string.IsNullOrEmpty(SelectExpediente.DOMICILIO_NUM_INT) ? int.Parse(SelectExpediente.DOMICILIO_NUM_INT) : 0;

                    TextNumeroExterior = SelectedProcesoLibertad.DOMICILIO_NUM_EXT.ToString();
                    TextNumeroInterior = SelectedProcesoLibertad.DOMICILIO_NUM_INT;

                    SelectEscolaridad = SelectedProcesoLibertad.ID_ESCOLARIDAD != null ? SelectedProcesoLibertad.ID_ESCOLARIDAD : id;

                    //SelectPaisNacimiento = SelectExpediente.ID_PAIS != null ? SelectExpediente.ID_PAIS : -1;
                    //SelectEntidadNacimiento = SelectExpediente.ID_ENTIDAD != null ? SelectExpediente.ID_ENTIDAD : -1;
                    //SelectMunicipioNacimiento = SelectExpediente.ID_MUNICIPIO != null ? SelectExpediente.ID_MUNICIPIO : -1;
                    //if (SelectExpediente. != null && SelectExpediente.ID_ENTIDAD > 0)
                    //{
                        // SelectedPaisNacimiento=  SelectExpediente.ENTIDAD.PAIS_NACIONALIDAD;
                        SelectPaisNacimiento = SelectExpediente.NACIMIENTO_PAIS;
                        SelectEntidadNacimiento = SelectExpediente.NACIMIENTO_ESTADO;
                        SelectMunicipioNacimiento = SelectExpediente.NACIMIENTO_MUNICIPIO != null ? SelectExpediente.NACIMIENTO_MUNICIPIO : -1;
                    //}
                    //SelectMunicipioNacimiento = selectExpediente.ID_PAIS != null ? selectExpediente.ID_PAIS : -1;
                    //SelectEntidadNacimiento = selectExpediente.ID_ENTIDAD != null ? selectExpediente.ID_ENTIDAD : -1;
                    //SelectMunicipio = selectExpediente.ID_MUNICIPIO != null ? selectExpediente.ID_MUNICIPIO : -1;
                    TextDelitoImputa = string.Empty;//SelectMJ == null ? string.Empty : SelectMJ.DELITOS;
                    //pendiente    textEntidadFederativa=SelectExpediente.
                    #region Datos Personal Enrevistadas
                    #endregion


                    #region Fotografias Imputado
                    //int NumImagen = 0;
                    //ListasFotografias = new ObservableCollection<PRS_VISITA_FOTOGRAFIA>(new cFotografias().Obtener(selectExpediente.ID_IMPUTADO, selectExpediente.ID_ANIO, selectExpediente.ID_CENTRO, null).OrderBy(o => o.ID_FOTO));
                    ////Limpia Imagnes
                    //ImagenSuperiorIzquierda = null;
                    //ImagenSuperiorDerecha = null;
                    //ImagenInferiorIzquierda = null;
                    //ImagenInferiorDerecha = null;
                    //if (ListasFotografias != null)
                    //    foreach (var itemFot in ListasFotografias)
                    //    {

                    //        switch (NumImagen)
                    //        {
                    //            case 0:
                    //                ImagenSuperiorIzquierda = itemFot.FOTOGRAFIA;
                    //                break;
                    //            case 1:
                    //                ImagenSuperiorDerecha = itemFot.FOTOGRAFIA;
                    //                break;
                    //            case 2:
                    //                ImagenInferiorIzquierda = itemFot.FOTOGRAFIA;
                    //                break;
                    //            case 3:
                    //                ImagenInferiorDerecha = itemFot.FOTOGRAFIA;
                    //                break;
                    //            default:
                    //                break;
                    //        }
                    //        NumImagen++;
                    //        //  ImagenSuperiorIzquierda=
                    //    }
                    #endregion

                    #region Estudio Socio econocmio
                    #endregion
                            #endregion

                    //agrega apodos
                    MenuReporteEnabled = true;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_LIBERADO);
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener la informacion", ex);
            }
        }

        private void BuscarFotografia(Object obj)
        {
            try
            {
                if (ListasFotografias == null)
                    ListasFotografias = new ObservableCollection<PRS_VISITA_FOTOGRAFIA>();
                var CountFotografias = ListasFotografias.Count;
                var ObjFotog = new PRS_VISITA_FOTOGRAFIA();
                if (CountFotografias >= 4)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Ya se Guardaron la cantidad Maxima de Fotografias");
                }
                else
                {
                    var op = new System.Windows.Forms.OpenFileDialog();
                    op.Title = "Seleccione un  documento";
                    op.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg";//"Formatos Validos:|*.pdf";
                    if (op.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        if (new System.IO.FileInfo(op.FileName).Length > 5000000)
                            StaticSourcesViewModel.Mensaje("Arvhivo no soportada", "El archivo debe ser de menos de 5 Mb", StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA, 5);
                        else
                        {

                            if (ImagenSuperiorIzquierda == null)
                            {
                                ImagenSuperiorIzquierda = System.IO.File.ReadAllBytes(op.FileName);
                                ObjFotog.ID_ANIO = SelectExpediente.ID_ANIO;
                                ObjFotog.ID_IMPUTADO = SelectExpediente.ID_IMPUTADO;
                                ObjFotog.ID_CENTRO = SelectExpediente.ID_CENTRO;
                                ObjFotog.FOTOGRAFIA = ImagenSuperiorIzquierda;

                            }
                            else
                            {
                                if (ImagenSuperiorDerecha == null)
                                {
                                    ImagenSuperiorDerecha = System.IO.File.ReadAllBytes(op.FileName);
                                    ObjFotog.ID_ANIO = SelectExpediente.ID_ANIO;
                                    ObjFotog.ID_IMPUTADO = SelectExpediente.ID_IMPUTADO;
                                    ObjFotog.ID_CENTRO = SelectExpediente.ID_CENTRO;
                                    ObjFotog.FOTOGRAFIA = ImagenSuperiorDerecha;

                                }
                                else
                                {
                                    if (ImagenInferiorIzquierda == null)
                                    {
                                        ImagenInferiorIzquierda = System.IO.File.ReadAllBytes(op.FileName);
                                        ObjFotog.ID_ANIO = SelectExpediente.ID_ANIO;
                                        ObjFotog.ID_IMPUTADO = SelectExpediente.ID_IMPUTADO;
                                        ObjFotog.ID_CENTRO = SelectExpediente.ID_CENTRO;
                                        ObjFotog.FOTOGRAFIA = ImagenInferiorIzquierda;
                                    }
                                    else
                                    {
                                        if (ImagenInferiorDerecha == null)
                                        {
                                            ImagenInferiorDerecha = System.IO.File.ReadAllBytes(op.FileName);
                                            ObjFotog.ID_ANIO = SelectExpediente.ID_ANIO;
                                            ObjFotog.ID_IMPUTADO = SelectExpediente.ID_IMPUTADO;
                                            ObjFotog.ID_CENTRO = SelectExpediente.ID_CENTRO;
                                            ObjFotog.FOTOGRAFIA = ImagenInferiorIzquierda;
                                        }
                                        else
                                        {
                                            StaticSourcesViewModel.Mensaje("Mensaje de Aviso", "Ya contiene el numero de Fotografias Requerido", StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA, 5);
                                        }
                                    }
                                }
                            }

                            var CuDocumento = System.IO.File.ReadAllBytes(op.FileName);
                        }
                    }
                    //
                    ListasFotografias.Add(ObjFotog);
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener la informacion", ex);
            }
        }
       
        private void LimpiarBusqueda()
        {
            ApellidoPaternoBuscar = ApellidoMaternoBuscar = NombreBuscar = NUCBuscar = string.Empty;
            AnioBuscar  = null;
            FolioBuscar = null;
            SelectExpediente = null;
            SelectIngreso = null;
            ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
            ListExpediente = null;
            LstLiberados = null;
        }

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.VISITA_DOMICILIARIA.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                foreach (var p in permisos)
                {
                    if (p.INSERTAR == 1)
                        PInsertar = true;
                    if (p.EDITAR == 1)
                        PEditar = true;
                    if (p.CONSULTAR == 1)
                        PConsultar = true;
                    if (p.IMPRIMIR == 1)
                        PImprimir = true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }
        #endregion

        #region Huellas Digitales
        private void ShowIdentification(object obj = null)
        {
            ShowPopUp = Visibility.Visible;
            ShowFingerPrint = Visibility.Hidden;
            var Initial442 = new Thread((Init) =>
            {
                try
                {
                    var nRet = 0;

                    CLSFPCaptureDllWrapper.CLS_Initialize();
                    CLSFPCaptureDllWrapper.CLS_SetLanguage(CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_RESOURCE.ENGLISH);
                    nRet = CLSFPCaptureDllWrapper.CLS_CaptureFP(CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_TYPE.IDFLATS);

                    if (nRet == 0)
                    {
                        ScannerMessage = "Procesando...";
                        ShowFingerPrint = Visibility.Visible;
                        ShowLine = Visibility.Visible;
                        ShowOk = Visibility.Hidden;
                        Thread.Sleep(300);
                        HuellasCapturadas = new List<PlantillaBiometrico>();

                        var SaveFingerPrints = new Thread((saver) =>
                        {
                            try
                            {
                                #region [Huellas]
                                for (short i = 1; i <= 10; i++)
                                {
                                    var pBuffer = IntPtr.Zero;
                                    var nBufferLength = 0;
                                    var nNFIQ = 0;

                                    CLSFPCaptureDllWrapper.CLS_GetImage(CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_IMPRESSION_TYPE.PLAIN, (CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i, CLSFPCaptureDllWrapper.IMG_TYPE.BMP, ref pBuffer, ref nBufferLength);
                                    var bufferBMP = new byte[nBufferLength];
                                    if (pBuffer != IntPtr.Zero)
                                        Marshal.Copy(pBuffer, bufferBMP, 0, nBufferLength);

                                    CLSFPCaptureDllWrapper.CLS_GetImage(CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_IMPRESSION_TYPE.PLAIN, (CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i, CLSFPCaptureDllWrapper.IMG_TYPE.WSQ, ref pBuffer, ref nBufferLength);
                                    var bufferWSQ = new byte[nBufferLength];
                                    if (pBuffer != IntPtr.Zero)
                                        Marshal.Copy(pBuffer, bufferWSQ, 0, nBufferLength);

                                    CLSFPCaptureDllWrapper.CLS_GetImageNFIQ(((CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i), ref nNFIQ, CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_IMPRESSION_TYPE.PLAIN);

                                    Fmd FMD = null;
                                    if (bufferBMP.Length != 0)
                                    {
                                        GuardaHuella = CreateBitmapSourceFromBitmap(new MemoryStream(bufferBMP));
                                        FMD = ExtractFmdfromBmp(new Bitmap(new MemoryStream(bufferBMP)).Clone(new Rectangle(0, 0, 357, 392), System.Drawing.Imaging.PixelFormat.Format8bppIndexed)).Data;
                                    }

                                    Thread.Sleep(200);
                                    switch ((CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i)
                                    {
                                        #region [Pulgar Derecho]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_THUMB:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            Application.Current.Dispatcher.Invoke((Action)(delegate
                                            {
                                                PulgarDerecho = ObtenerCalidad(nNFIQ);
                                            }));
                                            break;
                                        #endregion
                                        #region [Indice Derecho]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_INDEX:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            Application.Current.Dispatcher.Invoke((Action)(delegate
                                            {
                                                IndiceDerecho = ObtenerCalidad(nNFIQ);
                                            }));
                                            break;
                                        #endregion
                                        #region [Medio Derecho]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_MIDDLE:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            Application.Current.Dispatcher.Invoke((Action)(delegate
                                            {
                                                MedioDerecho = ObtenerCalidad(nNFIQ);
                                            }));
                                            break;
                                        #endregion
                                        #region [Anular Derecho]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_RING:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            Application.Current.Dispatcher.Invoke((Action)(delegate
                                            {
                                                AnularDerecho = ObtenerCalidad(nNFIQ);
                                            }));
                                            break;
                                        #endregion
                                        #region [Meñique Derecho]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_LITTLE:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            Application.Current.Dispatcher.Invoke((Action)(delegate
                                            {
                                                MeñiqueDerecho = ObtenerCalidad(nNFIQ);
                                            }));
                                            break;
                                        #endregion
                                        #region [Pulgar Izquierdo]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_THUMB:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            Application.Current.Dispatcher.Invoke((Action)(delegate
                                            {
                                                PulgarIzquierdo = ObtenerCalidad(nNFIQ);
                                            }));
                                            break;
                                        #endregion
                                        #region [Indice Izquierdo]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_INDEX:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            Application.Current.Dispatcher.Invoke((Action)(delegate
                                            {
                                                IndiceIzquierdo = ObtenerCalidad(nNFIQ);
                                            }));
                                            break;
                                        #endregion
                                        #region [Medio Izquierdo]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_MIDDLE:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            Application.Current.Dispatcher.Invoke((Action)(delegate
                                            {
                                                MedioIzquierdo = ObtenerCalidad(nNFIQ);
                                            }));
                                            break;
                                        #endregion
                                        #region [Anular Izquierdo]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_RING:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            Application.Current.Dispatcher.Invoke((Action)(delegate
                                            {
                                                AnularIzquierdo = ObtenerCalidad(nNFIQ);
                                            }));
                                            break;
                                        #endregion
                                        #region [Meñique Izquierdo]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_LITTLE:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            Application.Current.Dispatcher.Invoke((Action)(delegate
                                            {
                                                MeñiqueIzquierdo = ObtenerCalidad(nNFIQ);
                                            }));
                                            break;
                                        #endregion
                                        default:
                                            break;
                                    }
                                }
                                #endregion
                                if (SelectExpediente == null)
                                    ScannerMessage = "Huellas Capturadas Correctamente";
                                else
                                    if (new cImputadoBiometrico().GetData().Where(w => w.ID_ANIO == SelectExpediente.ID_ANIO && w.ID_CENTRO == SelectExpediente.ID_CENTRO && w.ID_IMPUTADO == SelectExpediente.ID_IMPUTADO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_DP).ToList().Any())
                                        ScannerMessage = "Huellas Actualizadas Correctamente";
                                    else
                                        ScannerMessage = "Huellas Capturadas Correctamente";
                            }
                            catch (Exception ex)
                            {
                                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al procesar huellas.", ex);
                            }
                        });

                        SaveFingerPrints.Start();
                        SaveFingerPrints.Join();

                        ShowLine = Visibility.Hidden;
                        Thread.Sleep(1500);
                    }
                    ShowPopUp = Visibility.Hidden;
                    CLSFPCaptureDllWrapper.CLS_Terminate();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.HUELLAS);
                }
                catch
                {
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ShowPopUp = Visibility.Hidden;
                        (new Dialogos()).ConfirmacionDialogo("Error", "Revise que el escanner este bien configurado.");
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.HUELLAS);
                    }));
                }
            });

            Initial442.Start();
        }

        private System.Windows.Media.Brush ObtenerCalidad(int nNFIQ)
        {
            if (nNFIQ == 0)
                return new SolidColorBrush(Colors.White);
            if (nNFIQ == 3)
                return new SolidColorBrush(Colors.Yellow);
            if (nNFIQ == 4)
                return new SolidColorBrush(Colors.Red);
            return new SolidColorBrush(Colors.LightGreen);
        }

        private void OkClick(object ImgPrint = null)
        {
            ShowPopUp = Visibility.Hidden;
        }

        private async void OnBuscarPorHuella(string obj = "")
        {
            await Task.Factory.StartNew(() => PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO));

            await TaskEx.Delay(400);

            var nRet = -1;
            var bandera = true;
            var requiereGuardarHuellas = false;// Parametro.GuardarHuellaEnBusquedaRegistro;
            if (requiereGuardarHuellas)
                try
                {
                    nRet = CLSFPCaptureDllWrapper.CLS_Initialize();
                }
                catch
                {
                    bandera = false;
                }
            else
                bandera = false;

            var windowBusqueda = new BusquedaHuella();
            windowBusqueda.DataContext = new BusquedaHuellaViewModel(enumTipoPersona.IMPUTADO, nRet == 0, requiereGuardarHuellas);
            if (nRet != 0 ? ((ControlPenales.Clases.FingerPrintScanner)(windowBusqueda.DataContext)).Readers.Count == 0 : false)
            {
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.HUELLAS);
                StaticSourcesViewModel.Mensaje("ADVERTENCIA", "ASEGURESE DE CONECTAR SU LECTOR DE HUELLA DIGITAL", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION, 5);
                return;
            }
            windowBusqueda.Owner = PopUpsViewModels.MainWindow;
            windowBusqueda.KeyDown += (s, e) =>
            {
                try
                {
                    if (e.Key == System.Windows.Input.Key.Escape) windowBusqueda.Close();
                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar", ex);
                }
            };
            windowBusqueda.Closed += (s, e) =>
            {
                try
                {
                    HuellasCapturadas = ((BusquedaHuellaViewModel)windowBusqueda.DataContext).HuellasCapturadas;
                    if (bandera == true)
                        CLSFPCaptureDllWrapper.CLS_Terminate();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);

                    if (!((BusquedaHuellaViewModel)windowBusqueda.DataContext).IsSucceed)
                        return;

                    SelectExpediente = ((BusquedaHuellaViewModel)windowBusqueda.DataContext).SelectRegistro != null ? ((BusquedaHuellaViewModel)windowBusqueda.DataContext).SelectRegistro.Imputado : null;


                    if (SelectExpediente == null)
                    {
                        //CamposBusquedaEnabled = true;
                        MenuBuscarEnabled = pConsultar;
                    }
                    else
                    {
                        AnioBuscar = SelectExpediente.ID_ANIO;
                        FolioBuscar = SelectExpediente.ID_IMPUTADO;
                        ApellidoPaternoBuscar = SelectExpediente.PATERNO;
                        ApellidoMaternoBuscar = SelectExpediente.MATERNO;
                        NombreBuscar = SelectExpediente.NOMBRE;
                        clickSwitch("buscar_visible");
                        MenuBuscarEnabled = pConsultar;
                        MenuGuardarEnabled = pEditar;
                    }
                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cerrar búsqueda", ex);
                }
            };
            windowBusqueda.ShowDialog();
            //AceptarBusquedaHuellaFocus = true;
        }

        private void CargarHuellas()
        {
            try
            {
                //if (SelectExpediente == null)
                //    return;
                var LoadHuellas = new Thread((Init) =>
                {
                    //if (SelectExpediente!=null)
                    //{
                    //    var Huellas = new cImputadoBiometrico().GetData().Where(w => w.ID_ANIO == SelectExpediente.ID_ANIO && w.ID_CENTRO == SelectExpediente.ID_CENTRO && w.ID_IMPUTADO == SelectExpediente.ID_IMPUTADO && w.ID_TIPO_BIOMETRICO >= 11 && w.ID_TIPO_BIOMETRICO <= 30).ToList();
                    //    //HuellasCapturadas = Huellas;
                    //}


                    if (HuellasCapturadas != null)
                        foreach (var item in HuellasCapturadas.Where(w => w.ID_TIPO_FORMATO == enumTipoFormato.FMTO_DP))
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                switch ((enumTipoBiometrico)item.ID_TIPO_BIOMETRICO)
                                {
                                    case enumTipoBiometrico.PULGAR_DERECHO:
                                        PulgarDerecho = ObtenerCalidad(item.CALIDAD.HasValue ? (int)item.CALIDAD.Value : 0);
                                        break;
                                    case enumTipoBiometrico.INDICE_DERECHO:
                                        IndiceDerecho = ObtenerCalidad(item.CALIDAD.HasValue ? (int)item.CALIDAD.Value : 0);
                                        break;
                                    case enumTipoBiometrico.MEDIO_DERECHO:
                                        MedioDerecho = ObtenerCalidad(item.CALIDAD.HasValue ? (int)item.CALIDAD.Value : 0);
                                        break;
                                    case enumTipoBiometrico.ANULAR_DERECHO:
                                        AnularDerecho = ObtenerCalidad(item.CALIDAD.HasValue ? (int)item.CALIDAD.Value : 0);
                                        break;
                                    case enumTipoBiometrico.MENIQUE_DERECHO:
                                        MeñiqueDerecho = ObtenerCalidad(item.CALIDAD.HasValue ? (int)item.CALIDAD.Value : 0);
                                        break;
                                    case enumTipoBiometrico.PULGAR_IZQUIERDO:
                                        PulgarIzquierdo = ObtenerCalidad(item.CALIDAD.HasValue ? (int)item.CALIDAD.Value : 0);
                                        break;
                                    case enumTipoBiometrico.INDICE_IZQUIERDO:
                                        IndiceIzquierdo = ObtenerCalidad(item.CALIDAD.HasValue ? (int)item.CALIDAD.Value : 0);
                                        break;
                                    case enumTipoBiometrico.MEDIO_IZQUIERDO:
                                        MedioIzquierdo = ObtenerCalidad(item.CALIDAD.HasValue ? (int)item.CALIDAD.Value : 0);
                                        break;
                                    case enumTipoBiometrico.ANULAR_IZQUIERDO:
                                        AnularIzquierdo = ObtenerCalidad(item.CALIDAD.HasValue ? (int)item.CALIDAD.Value : 0);
                                        break;
                                    case enumTipoBiometrico.MENIQUE_IZQUIERDO:
                                        MeñiqueIzquierdo = ObtenerCalidad(item.CALIDAD.HasValue ? (int)item.CALIDAD.Value : 0);
                                        break;
                                    default:
                                        break;
                                }
                            }));
                        }
                });

                if (SelectExpediente != null)
                {
                    if (SelectExpediente.IMPUTADO_BIOMETRICO != null ? SelectExpediente.IMPUTADO_BIOMETRICO.Count > 0 : true)
                    {
                        HuellasCapturadas = new List<PlantillaBiometrico>();
                        foreach (var biometrico in SelectExpediente.IMPUTADO_BIOMETRICO)
                        {
                            switch (biometrico.ID_TIPO_BIOMETRICO)
                            {
                                case 0:
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_DP)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = biometrico.BIOMETRICO, CALIDAD = biometrico.CALIDAD });
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_WSQ)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = biometrico.BIOMETRICO });
                                    break;
                                case 1:
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_DP)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = biometrico.BIOMETRICO, CALIDAD = biometrico.CALIDAD });
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_WSQ)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = biometrico.BIOMETRICO });
                                    break;
                                case 2:
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_DP)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = biometrico.BIOMETRICO, CALIDAD = biometrico.CALIDAD });
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_WSQ)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = biometrico.BIOMETRICO });
                                    break;
                                case 3:
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_DP)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = biometrico.BIOMETRICO, CALIDAD = biometrico.CALIDAD });
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_WSQ)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = biometrico.BIOMETRICO });
                                    break;
                                case 4:
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_DP)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = biometrico.BIOMETRICO, CALIDAD = biometrico.CALIDAD });
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_WSQ)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = biometrico.BIOMETRICO });
                                    break;
                                case 5:
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_DP)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = biometrico.BIOMETRICO, CALIDAD = biometrico.CALIDAD });
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_WSQ)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = biometrico.BIOMETRICO });
                                    break;
                                case 6:
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_DP)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = biometrico.BIOMETRICO, CALIDAD = biometrico.CALIDAD });
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_WSQ)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = biometrico.BIOMETRICO });
                                    break;
                                case 7:
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_DP)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = biometrico.BIOMETRICO, CALIDAD = biometrico.CALIDAD });
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_WSQ)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = biometrico.BIOMETRICO });
                                    break;
                                case 8:
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_DP)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = biometrico.BIOMETRICO, CALIDAD = biometrico.CALIDAD });
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_WSQ)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = biometrico.BIOMETRICO });
                                    break;
                                case 9:
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_DP)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = biometrico.BIOMETRICO, CALIDAD = biometrico.CALIDAD });
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_WSQ)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = biometrico.BIOMETRICO });
                                    break;

                            }
                        }
                    }
                }

                LoadHuellas.Start();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
