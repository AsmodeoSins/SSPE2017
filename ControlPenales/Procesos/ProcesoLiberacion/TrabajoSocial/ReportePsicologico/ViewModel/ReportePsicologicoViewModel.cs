using Cogent.Biometrics;
using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases;
using DPUruNet;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Reporting.WinForms;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Controlador.Catalogo.Justicia.Ingreso;
using SSP.Controlador.Catalogo.Justicia.Liberados;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace ControlPenales
{
    partial class ReportePsicologicoViewModel : FingerPrintScanner
    {
        private void clickSwitch(object op)
        {
            switch (op.ToString())
            {
                case "insertar_alias":
                    //if (!PInsertar)
                    //{
                    //(new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                    //break;
                    //// }

                    //LimpiarAlias();
                    //ValidarAlias();
                    //SelectAlias = null;
                    //PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.ALIAS);
                    break;
                case "editar_alias":
                    //if (SelectAlias != null)
                    //{
                    //    if (!PEditar)
                    //    {
                    //        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                    //        break;
                    //    }
                    //    ValidarAlias();
                    //    PopulateAlias();
                    //    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.ALIAS);
                    //}
                    //else
                    //    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar alias");
                    break;
                case "eliminar_alias":
                    //if (SelectAlias != null)
                    //{
                    //    EliminarAlias();
                    //}
                    //else
                    //    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar alias");
                    break;
                case "guardar_alias":
                    //if (SelectAlias != null)
                    //    EditarAlias();
                    //else
                    //    AgregarAlias();
                    ////    ValidacionesLiberado();
                    //PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.ALIAS);
                    break;
                case "cancelar_alias":
                    //LimpiarAlias();
                    //// ValidacionesLiberado();
                    //PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.ALIAS);
                    break;
                case "insertar_apodo":
                    //if (!PInsertar)
                    //{
                    //    (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                    //    break;
                    //}

                    //ValidarApodo();
                    //LimpiarApodo();

                    //SelectApodo = null;
                    //PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.APODO);
                    break;
                case "editar_apodo":
                    //if (SelectApodo != null)
                    //{
                    //    if (!PEditar)
                    //    {
                    //        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                    //        break;
                    //    }

                    //    ValidarApodo();
                    //    PopulateApodo();
                    //    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.APODO);
                    //}
                    //else
                    //    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar apodo");
                    break;
                case "eliminar_apodo":
                    //if (SelectApodo != null)
                    //{
                    //    EliminarApodo();
                    //}
                    //else
                    //    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar apodo");
                    break;
                case "guardar_apodo":
                    //if (SelectApodo != null)
                    //    EditarApodo();
                    //else
                    //    AgregarApodo();
                    ////  ValidacionesLiberado();
                    //PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.APODO);
                    break;
                case "cancelar_apodo":
                    //LimpiarApodo();
                    ////   ValidacionesLiberado();
                    //PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.APODO);
                    break;
                case "buscar_menu":
                    if (!PConsultar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        break;
                    }
                    auxProcesoLibertad = SelectedProcesoLibertad;
                    SelectExpediente = null;
                    SelectedProcesoLibertad = null;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_LIBERADO);
                    break;
                case "nueva_busqueda":
                    LimpiarBusqueda();
                    break;
                case "buscar_salir":
                    SelectedProcesoLibertad = auxProcesoLibertad;
                    if (SelectedProcesoLibertad != null)
                    {
                        SelectExpediente = SelectedProcesoLibertad.IMPUTADO;
                    }
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_LIBERADO);
                    break;
                case "buscar_visible":
                    if (!PConsultar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        break;
                    }
                        ClickEnter();
                    break;
                case "nuevo_expediente":
                    //SelectMJ = null;
                    //SelectExpediente = null;
                    //Nuevo();
                    break;
                case "buscar_nueva_medida":
                    SelectMJ = null; //Se agrega nueva medida
                    //     ValidacionesLiberado();
                    Obtener();
                    break;
                case "buscar_seleccionar":
                    //if (SelectMJ != null)
                    //{
                    //   ValidacionesLiberado();
                    if (SelectedProcesoLibertad != null)
                    {
                        Obtener();
                        ValidacionDatosFamiliar();
                        OnPropertyChanged("TextLugarEntrevista");
                        OnPropertyChanged("TextFechaEntrv");
                        OnPropertyChanged("InicioDiaDomingo");
                        OnPropertyChanged("TextNombreFamiliar");
                        OnPropertyChanged("SelectParentesco");
                        OnPropertyChanged("TextTelefonoFamiliar");
                        OnPropertyChanged("TextCalleFamiliar");
                        OnPropertyChanged("TextNumExteriorFamiliar");
                        OnPropertyChanged("TextDescripcionEntrv");
                        OnPropertyChanged("TextTecnicasUtilizadas");
                        OnPropertyChanged("TextExamenMental");
                        OnPropertyChanged("TextPersonalidad");
                        OnPropertyChanged("TextNuceloFamPrimario");
                        OnPropertyChanged("TextNuceloFamSecundario");
                        OnPropertyChanged("TextObsrv");
                        OnPropertyChanged("TextSugerencia");
                        StaticSourcesViewModel.SourceChanged = false;
                    }
                    else
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "Favor de seleccionar un proceso en libertad.");
                    }
                    break;
                case "reporte_menu":
                    //Creacion de Reporte

                    if (!PImprimir)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                    }
                    else
                    {
                        if (SelectExpediente != null)
                        {

                            if (base.HasErrors)
                            {
                                (new Dialogos()).ConfirmacionDialogo("Validación", "Debe de llenar los campos Necesarios por Favor.");
                            }
                            else
                            {
                                ///var CerifImputado = SelectIngreso.IMPUTADO;
                                var View = new ReportesView();
                                var DatosReporte = new cEntrevistaInicial();
                                #region Iniciliza el entorno para mostrar el reporte al usuario
                                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                                View.Owner = PopUpsViewModels.MainWindow;
                                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };


                                View.Show();
                                #endregion

                                #region Se forma el cuerpo del reporte

                                //if (SelectIngreso.IMPUTADO != null)
                                //{
                                #region EntrevistaInicialPrincipal
                                //DatosReporte.Nuc = TextNucEntrevista;
                                //DatosReporte.CausaPenalEntrv = TextCausaPenalEntrevista;
                                DatosReporte.LugarEntrv = !string.IsNullOrEmpty(TextLugarEntrevista) ? TextLugarEntrevista.Trim() : string.Empty;
                                DatosReporte.FechaEntrv = TextFechaEntrv.HasValue ? TextFechaEntrv.Value.ToString("dd/MM/yyyy") : string.Empty;
                                DatosReporte.HoraEntrv = InicioDiaDomingo.HasValue ? string.Format("{0} : {1} ", InicioDiaDomingo.Value.Hour.ToString(), InicioDiaDomingo.Value.Minute.ToString()) : string.Empty;
                                DatosReporte.NombreInterno = ApellidoPaternoBuscar + " " + ApellidoMaternoBuscar + " " + NombreBuscar;
                                #endregion
                                //DatosReporte.Escolaridad = SelectExpediente.ESCOLARIDAD != null ? !string.IsNullOrEmpty(SelectExpediente.ESCOLARIDAD.DESCR) ? SelectExpediente.ESCOLARIDAD.DESCR.Trim() : string.Empty : string.Empty;
                                DatosReporte.Escolaridad = SelectEscolaridad.HasValue ? SelectEscolaridad.Value > -1 ? ListEscolaridad.Where(w => w.ID_ESCOLARIDAD == SelectEscolaridad.Value).FirstOrDefault().DESCR : string.Empty : string.Empty;
                                DatosReporte.FechaNacimiento = TextFechaNacimiento.HasValue ? TextFechaNacimiento.Value.ToString("dd/MM/yyyy") : string.Empty;
                                DatosReporte.LugarNacimiento = TextLugarNacimientoExtranjero;
                                DatosReporte.EntidadFederativa = TextEntidadFederativa;
                                DatosReporte.EdadInterno = TextEdad.HasValue ? TextEdad.ToString().Trim() : string.Empty;
                                DatosReporte.Ocupacion = selectOcupacion.HasValue ? ListOcupacion.Where(w => w.ID_OCUPACION == selectOcupacion).First().DESCR : string.Empty;

                                var ingreso = SelectExpediente.INGRESO.OrderByDescending(w => w.ID_INGRESO).FirstOrDefault();

                                DatosReporte.Domicilio = string.Format("{0} {1} {2} {3}", !string.IsNullOrEmpty(ingreso.DOMICILIO_CALLE) ? ingreso.DOMICILIO_CALLE.Trim() : string.Empty,
                                    ingreso.DOMICILIO_NUM_EXT.HasValue ? ingreso.DOMICILIO_NUM_EXT != decimal.Zero ? ingreso.DOMICILIO_NUM_EXT.ToString() : string.Empty : string.Empty,
                                ingreso.COLONIA != null ? !string.IsNullOrEmpty(ingreso.COLONIA.DESCR) ? ingreso.COLONIA.DESCR.Trim() : string.Empty : string.Empty,
                                ingreso.MUNICIPIO != null ? !string.IsNullOrEmpty(ingreso.MUNICIPIO.MUNICIPIO1) ? ingreso.MUNICIPIO.MUNICIPIO1.Trim() : string.Empty : string.Empty);
                                DatosReporte.RadicadoEnBc = RadicadoBc == true ? "Si" : "No";
                                DatosReporte.Telefono = !string.IsNullOrEmpty(TextTelefono) ? TextTelefono.ToString() : string.Empty;
                                //     DatosReporte.Domicilio = string.Format("Calle:{0} Num Exterior:{1} Num Interior{2}", TextCalle, TextNumeroExterior, TextNumeroInterior);
                                DatosReporte.Sexo = selectExpediente.SEXO;
                                //  DatosReporte.Fecha = Fechas.GetFechaDateServer.ToShortDateString();
                                //    DatosReporte.oficio = textOficio;
                                DatosReporte.EstadoCivil = SelectEstadoCivil.HasValue ? LstEstadoCivil.Where(w => w.ID_ESTADO_CIVIL == (short)SelectEstadoCivil.Value).FirstOrDefault().DESCR.Trim() : string.Empty;
                                //Prueba
                                //DatosReporte.HorariosDiasTrabajo=
                                ListApodo = new ObservableCollection<APODO>(selectExpediente.APODO);
                                if (ListApodo != null && ListApodo.Count > 0)
                                {

                                    short ultimoApodo = selectExpediente.APODO.LastOrDefault().ID_APODO;
                                    foreach (var entidad in selectExpediente.APODO.OrderBy(o => o.ID_APODO))
                                    {
                                        if (entidad.IMPUTADO != null)
                                        {
                                            DatosReporte.Apodo += entidad.ID_APODO == ultimoApodo ? entidad.APODO1.Trim() + "" : entidad.APODO1.Trim() + ",";
                                        }
                                    }

                                }

                                listAlias = new ObservableCollection<ALIAS>(selectExpediente.ALIAS);
                                if (listAlias != null && listAlias.Count > 0)
                                {
                                    short ultimoApodo = selectExpediente.ALIAS.LastOrDefault().ID_ALIAS;
                                    foreach (var entidad2 in selectExpediente.ALIAS.OrderBy(o => o.ID_ALIAS))
                                    {
                                        if (entidad2.IMPUTADO != null)
                                        {
                                            var Pat = !string.IsNullOrEmpty(entidad2.PATERNO) ? entidad2.PATERNO.Trim() : "";
                                            var Mate = !string.IsNullOrEmpty(entidad2.MATERNO) ? entidad2.MATERNO.Trim() : "";
                                            var Nom = !string.IsNullOrEmpty(entidad2.NOMBRE) ? entidad2.NOMBRE.Trim() : "";
                                            DatosReporte.Alias += entidad2.ID_ALIAS == ultimoApodo ? Pat + " " + Mate + " " + Nom + "" : Pat + " " + Mate + " " + Nom + ",";
                                        }
                                    }

                                }
                                //DatosReporte.Apodo = TextApodo;
                                //DatosReporte.Alias = TextAlias;
                                #endregion
                                
                                #region Datos Familiares
                                DatosReporte.NombreAoyo = !string.IsNullOrEmpty(TextNombreFamiliar) ? TextNombreFamiliar.Trim() : string.Empty;
                                DatosReporte.ParentescoAoyo = SelectedParentesco != null ? !string.IsNullOrEmpty(SelectedParentesco.DESCR) ? SelectedParentesco.DESCR.Trim() : string.Empty : string.Empty;
                                DatosReporte.TelefonoAoyo = !string.IsNullOrEmpty(TextTelefonoFamiliar) ? TextTelefonoFamiliar.Trim() : string.Empty;
                                DatosReporte.CalleAoyo = !string.IsNullOrEmpty(TextCalleFamiliar) ? TextCalleFamiliar.Trim() : string.Empty;
                                DatosReporte.DomicilioAoyo = string.Format("Calle: {0} Núm. Interior: {1}  Núm. Exterior: {2}",
                                    !string.IsNullOrEmpty(TextCalleFamiliar) ? TextCalleFamiliar.Trim() : string.Empty,
                                    !string.IsNullOrEmpty(TextNumInteriorFamiliar) ? TextNumInteriorFamiliar.Trim() : string.Empty,
                                    !string.IsNullOrEmpty(TextNumExteriorFamiliar) ? TextNumExteriorFamiliar.Trim() : string.Empty);
                                #endregion
                                
                                #region Situacion Actual
                                #endregion
                                
                                #region Datos de la persona Entrevistada
                                DatosReporte.DescripcionEntrv = !string.IsNullOrEmpty(TextDescripcionEntrv) ? TextDescripcionEntrv.Trim() : string.Empty;
                                DatosReporte.TecnicasUtilizadas = !string.IsNullOrEmpty(TextTecnicasUtilizadas) ? TextTecnicasUtilizadas.Trim() : string.Empty;
                                DatosReporte.Observacion = !string.IsNullOrEmpty(TextObsrv) ? TextObsrv.Trim() : string.Empty;
                                DatosReporte.Personalidad = !string.IsNullOrEmpty(TextPersonalidad) ? TextPersonalidad.Trim() : string.Empty;
                                DatosReporte.ExamenMental = !string.IsNullOrEmpty(TextExamenMental) ? TextExamenMental.Trim() : string.Empty;
                                DatosReporte.NuceloFamPrimario = !string.IsNullOrEmpty(TextNuceloFamPrimario) ? TextNuceloFamPrimario.Trim() : string.Empty;
                                DatosReporte.NucleoFamSecundario = !string.IsNullOrEmpty(TextNuceloFamSecundario) ? TextNuceloFamSecundario.Trim() : string.Empty;
                                DatosReporte.Sugerencias = TextSugerencia;
                                #endregion

                                #region Se forma el encabezado del reporte
                                var Encabezado = new cEncabezado();
                                Encabezado.TituloUno = "JUSTICIA RESTAURATIVA";
                                Encabezado.TituloDos = Parametro.ENCABEZADO2;
                                Encabezado.NombreReporte = "REPORTE PSICOLÓGICO";
                                Encabezado.ImagenIzquierda = Parametro.LOGO_ESTADO;
                                Encabezado.ImagenFondo = Parametro.REPORTE_LOGO2;

                                #endregion

                                #region Inicializacion de reporte
                                View.Report.ProcessingMode = ProcessingMode.Local;
                                View.Report.LocalReport.ReportPath = "Reportes/rEstudioPsicologico.rdlc";
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
                        else { new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un Imputado."); }
                    }


                                #endregion

                    break;

                case "limpiar_menu":
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new ReportePsicologicoView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ReportePsicologicoViewModel();
                    break;
                case "guardar_menu":
                    if (SelectedProcesoLibertad == null)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "Favor de seleccionar un proceso en libertad.");
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

        private async void ReportePsicologicoLoad(ReportePsicologicoView obj = null)
        {
            try
            {

                await StaticSourcesViewModel.CargarDatosMetodoAsync(CargarListas);

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar decomisos", ex);
            }
        }

        private async void EntrevistadoRptPsicologicoLoad(EntrevistadoRptPsicologicoView obj = null)
        {
            try
            {
                MaxWhidthEntrevistadoRptpsicologic = ((TextBlock)obj.FindName("DescripcionEntrv")).ActualWidth - 80;

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

                LstEstadoCivil = new ObservableCollection<ESTADO_CIVIL>(new cEstadoCivil().ObtenerTodos().OrderBy(o => o.DESCR));
                ListOcupacion = new ObservableCollection<OCUPACION>(new cOcupacion().ObtenerTodos().OrderBy(o => o.DESCR));
                ListParentesco = new ObservableCollection<TIPO_REFERENCIA>(new cTipoReferencia().ObtenerTodos().OrderBy(o => o.DESCR));
                ListEscolaridad = new ObservableCollection<ESCOLARIDAD>(new cEscolaridad().ObtenerTodos().OrderBy(o => o.DESCR));

                var paises = new cPaises().ObtenerTodos();


                System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                {

                    LstEstadoCivil.Insert(0, new ESTADO_CIVIL() { ID_ESTADO_CIVIL = -1, DESCR = "SELECCIONE" });
                    ListOcupacion.Insert(0, new OCUPACION() { ID_OCUPACION = -1, DESCR = "SELECCIONE" });
                    ListEscolaridad.Insert(0, new ESCOLARIDAD() { ID_ESCOLARIDAD = -1, DESCR = "SELECCIONE" });
                    ListParentesco.Insert(0, new TIPO_REFERENCIA() { ID_TIPO_REFERENCIA = -1, DESCR = "SELECCIONE" });

                    //  ValidacionesLiberado();

                    //pendiente   ConfiguraPermisos();

                    StaticSourcesViewModel.SourceChanged = false;
                }));
                ConfiguraPermisos();
                AsignarMaxLenght();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar buscar por nuc", ex);
            }
        }

        private void AsignarMaxLenght()
        {


        }
        
        public void Guardar()
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
                    var objReportepsicologico = new PRS_REPORTE_PSICOLOGICO();

                    #region Datos Entrevista
                    objReportepsicologico.LUGAR = TextLugarEntrevista.Trim();
                    objReportepsicologico.HORA = InicioDiaDomingo;
                    objReportepsicologico.FECHA = TextFechaEntrv;
                    #endregion

                    #region Datos Generales
                    objReportepsicologico.ID_IMPUTADO = SelectedProcesoLibertad.ID_IMPUTADO;
                    objReportepsicologico.ID_ANIO = SelectedProcesoLibertad.ID_ANIO;
                    objReportepsicologico.ID_CENTRO = SelectedProcesoLibertad.ID_CENTRO;
                    objReportepsicologico.ID_PROCESO_LIBERTAD = SelectedProcesoLibertad.ID_PROCESO_LIBERTAD;
                    #endregion

                    #region Datos Familiar
                    objReportepsicologico.I_NOMBRE_FAM = TextNombreFamiliar.Trim();
                    objReportepsicologico.I_TELEFONO = TextTelefonoFamiliar;
                    objReportepsicologico.I_CALLE = !string.IsNullOrEmpty(TextCalleFamiliar) ? TextCalleFamiliar : ""; ;
                    objReportepsicologico.I_NUM_EXTERIOR = !string.IsNullOrEmpty(TextNumExteriorFamiliar) ? TextNumExteriorFamiliar : "";
                    objReportepsicologico.I_NUM_INTERIOR = !string.IsNullOrEmpty(TextNumInteriorFamiliar) ? TextNumInteriorFamiliar : "";
                    string NumInterior = !string.IsNullOrEmpty(TextNumExteriorFamiliar) ? TextNumExteriorFamiliar : "";
                    //objReportepsicologico.I_DOMICILIO_FAM = (TextCalleFamiliar + " " + "#Exterior " + NumInterior+ " " + "#Interior " + TextNumInteriorFamiliar).Trim();
                    //Pendiente Telefono
                    objReportepsicologico.I13_PARENTESCO = SelectParentesco > -1 ? SelectParentesco : null;
                    #endregion

                    #region Datos de la persona Entrevistada

                    objReportepsicologico.II_DESCRIPCION_ENTREVISTADO = TextDescripcionEntrv.Trim();
                    objReportepsicologico.III_TECNICAS_UTILIZADAS = TextTecnicasUtilizadas.Trim();
                    objReportepsicologico.IV_EXAMEN_MENTAL = TextExamenMental.Trim();
                    objReportepsicologico.V_PERSONALIDAD = TextPersonalidad.Trim();
                    objReportepsicologico.VI_NUCLEO_FAMILIAR_PRIM = TextNuceloFamPrimario.Trim();
                    objReportepsicologico.VII_NUCLEO_FAMILIAR_SEC = TextNuceloFamSecundario.Trim();
                    objReportepsicologico.VII_OBSERVACIONES = TextObsrv.Trim();
                    objReportepsicologico.IX_SUGERENCIAS = TextSugerencia.Trim();

                    #endregion
                    if (selectedReportePsicologico != null)
                    {
                        if (!PEditar)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                            return;
                        }
                        objReportepsicologico.ID_FOLIO = selectedReportePsicologico.ID_FOLIO;
                        if (new cReportePsicologico().Actualizar(objReportepsicologico))
                            new Dialogos().ConfirmacionDialogo("Éxito", "La información se guardo correctamente");
                        else
                            new Dialogos().ConfirmacionDialogo("Error", "Ocurrio un error al guardar la informacion");
                    }
                    else
                    {
                        if (!PInsertar)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                            return;
                        }
                        if (new cReportePsicologico().Insertar(objReportepsicologico))
                        {
                            objReportepsicologico = new cReportePsicologico().Obtener(
                                SelectedProcesoLibertad.ID_CENTRO,
                                SelectedProcesoLibertad.ID_ANIO,
                                SelectedProcesoLibertad.ID_IMPUTADO,
                                SelectedProcesoLibertad.ID_PROCESO_LIBERTAD);
                            new Dialogos().ConfirmacionDialogo("Éxito", "La información se guardo correctamente"); 
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Error", "Ocurrio un error al guardar la informacion");
                        
                    }

                    //if (new cReportePsicologico().Insertar(objReportepsicologico))
                    //{
                    //    new Dialogos().ConfirmacionDialogo("Éxito", "La información se registro correctamente");
                    //    SelectExpediente = null;

                    //    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new ReportePsicologicoView();
                    //    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ReportePsicologicoViewModel();
                    //}
                    //else
                    //{

                    //    new Dialogos().ConfirmacionDialogo("Validación", "Ocurrio un error al agregar los datos");
                    //    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new ReportePsicologicoView();
                    //    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ReportePsicologicoViewModel();
                    //    SelectExpediente = null;
                    //}



                }
            }
            catch (Exception ex)
            {

                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al Al Guardar la informacion", ex);
            }


        }

        #region Buscar
        private void LimpiarBusqueda()
        {
            ApellidoPaternoBuscar = ApellidoMaternoBuscar = NombreBuscar = NUCBuscar =  string.Empty;
            AnioBuscar  = null;
            FolioBuscar = null;
            SelectExpediente = null;
            SelectIngreso = null;
            ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
            ListExpediente = null;
            LstLiberados = null;
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
        #endregion

        #region Alias
        private void LimpiarAlias()
        {
            SelectAlias = null;
        }

        private void PopulateAlias()
        {
            NombreAlias = SelectAlias.NOMBRE;
            PaternoAlias = SelectAlias.PATERNO;
            MaternoAlias = SelectAlias.MATERNO;
        }

        private void AgregarAlias()
        {
            if (!base.HasErrors)
            {
                if (ListAlias == null)
                    ListAlias = new ObservableCollection<ALIAS>();
                ListAlias.Add(new ALIAS() { NOMBRE = NombreAlias, PATERNO = PaternoAlias, MATERNO = MaternoAlias });
            }
        }

        private void EditarAlias()
        {
            if (!base.HasErrors)
            {
                SelectAlias.NOMBRE = NombreAlias;
                SelectAlias.PATERNO = PaternoAlias;
                SelectAlias.MATERNO = MaternoAlias;
                ListAlias = new ObservableCollection<ALIAS>(ListAlias);
            }

        }

        private void EliminarAlias()
        {
            ListAlias.Remove(SelectAlias);
        }

        #endregion

        #region Apodo
        private void LimpiarApodo()
        {
            Apodo = string.Empty;
            SelectApodo = null;
        }

        private void PopulateApodo()
        {
            Apodo = SelectApodo.APODO1;
        }

        private void AgregarApodo()
        {
            if (!base.HasErrors)
            {
                if (ListApodo == null)
                    ListApodo = new ObservableCollection<APODO>();
                ListApodo.Add(new APODO() { APODO1 = Apodo });
            }
        }

        private void EditarApodo()
        {
            if (!base.HasErrors)
            {
                SelectApodo.APODO1 = Apodo;
                ListApodo = new ObservableCollection<APODO>(ListApodo);
            }
        }

        private async void ClickEnter(Object obj = null)
        {
            try
            {
                if (!PConsultar)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                    return;
                }
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

                #region comentado
                //ImagenIngreso = ImagenImputado = new Imagenes().getImagenPerson();
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

        private string MetodogenericoValidacionTextBox(string ObjValidar)
        {

            return !string.IsNullOrEmpty(ObjValidar) ? ObjValidar.Trim() : "";
        }
        //Metodos donde solo se Popula la Informacion del reporte psicologico del Imputado seleccinado
        private void PopularUltimaEntrevistaImputado()
        {
            if (SelectedProcesoLibertad == null)
            {
                new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un proceso en libertad");
                return;
            }
            selectedReportePsicologico = new cReportePsicologico().Obtener(
                SelectedProcesoLibertad.ID_CENTRO,
                SelectedProcesoLibertad.ID_ANIO,
                SelectedProcesoLibertad.ID_IMPUTADO,
                SelectedProcesoLibertad.ID_PROCESO_LIBERTAD);
            if (selectedReportePsicologico != null)
            {
                TextFechaEntrv = selectedReportePsicologico.FECHA;
                TextLugarEntrevista = MetodogenericoValidacionTextBox(selectedReportePsicologico.LUGAR);
                InicioDiaDomingo = selectedReportePsicologico.HORA;
                TextNombreFamiliar = MetodogenericoValidacionTextBox(selectedReportePsicologico.I_NOMBRE_FAM);
                SelectParentesco = selectedReportePsicologico.I13_PARENTESCO;
                TextDescripcionEntrv = MetodogenericoValidacionTextBox(selectedReportePsicologico.II_DESCRIPCION_ENTREVISTADO);
                TextCalleFamiliar = MetodogenericoValidacionTextBox(selectedReportePsicologico.I_CALLE);
                TextNumInteriorFamiliar = MetodogenericoValidacionTextBox(selectedReportePsicologico.I_NUM_INTERIOR);
                TextNumExteriorFamiliar = MetodogenericoValidacionTextBox(selectedReportePsicologico.I_NUM_EXTERIOR);
                TextTelefonoFamiliar = MetodogenericoValidacionTextBox(selectedReportePsicologico.I_TELEFONO);
                TextTecnicasUtilizadas = MetodogenericoValidacionTextBox(selectedReportePsicologico.III_TECNICAS_UTILIZADAS);
                TextExamenMental = MetodogenericoValidacionTextBox(selectedReportePsicologico.IV_EXAMEN_MENTAL);
                TextPersonalidad = MetodogenericoValidacionTextBox(selectedReportePsicologico.V_PERSONALIDAD);
                TextNuceloFamPrimario = MetodogenericoValidacionTextBox(selectedReportePsicologico.VI_NUCLEO_FAMILIAR_PRIM);
                TextNuceloFamSecundario = MetodogenericoValidacionTextBox(selectedReportePsicologico.VII_NUCLEO_FAMILIAR_SEC);
                TextObsrv = MetodogenericoValidacionTextBox(selectedReportePsicologico.VII_OBSERVACIONES);
                TextSugerencia = MetodogenericoValidacionTextBox(selectedReportePsicologico.IX_SUGERENCIAS);
            }

        }
       
        private void Obtener()
        {
            try
            {
                TabsEnabled = true;
                LimpiarDatosPantalla();
                PopularUltimaEntrevistaImputado();
                //SelectMJ != null && 
                TabControlVisible = System.Windows.Visibility.Visible;
                // -----Obener Entrevista -----
                ////
                MenuGuardarEnabled = true;
                //MenuGuardarEnabled = PConsultar;
                #region BusquedaDatos
                NUCBuscar = SelectedProcesoLibertad.NUC;
                AnioBuscar = SelectExpediente.ID_ANIO;
                FolioBuscar = SelectExpediente.ID_IMPUTADO;
                NombreBuscar = SelectExpediente.NOMBRE;
                ApellidoPaternoBuscar = SelectExpediente.PATERNO;
                ApellidoMaternoBuscar = SelectExpediente.MATERNO;
                #endregion
                TextNombreEntrevistado = string.Format("{0} {1} {2}",
                    !string.IsNullOrEmpty(ApellidoPaternoBuscar) ? ApellidoPaternoBuscar.Trim() : string.Empty,
                    !string.IsNullOrEmpty(ApellidoMaternoBuscar) ? ApellidoMaternoBuscar.Trim() : string.Empty,
                    NombreBuscar.Trim());
                TextCalle = selectedProcesoLibertad.DOMICILIO_CALLE;
                SelectSexo = SelectExpediente.SEXO;
                SelectEstadoCivil = selectedProcesoLibertad.ID_ESTADO_CIVIL;
                SelectOcupacion = selectedProcesoLibertad.ID_OCUPACION;
                TextFechaNacimiento = SelectExpediente.NACIMIENTO_FECHA;
                TextTelefono = selectedProcesoLibertad.TELEFONO.ToString();

                TextNumeroExterior = selectedProcesoLibertad.DOMICILIO_NUM_EXT;
                TextNumeroInterior = !string.IsNullOrEmpty(selectedProcesoLibertad.DOMICILIO_NUM_INT) ? int.Parse(selectedProcesoLibertad.DOMICILIO_NUM_INT) : 0;
                SelectedIdioma = selectExpediente.ID_IDIOMA;

                SelectOcupacion = selectedProcesoLibertad.ID_OCUPACION;
                SelectEscolaridad = selectedProcesoLibertad.ID_ESCOLARIDAD;
                ListAlias = new ObservableCollection<ALIAS>(SelectExpediente.ALIAS);
                ListApodo = new ObservableCollection<APODO>(SelectExpediente.APODO);
                textLugarNacimientoExtranjero = SelectExpediente.NACIMIENTO_LUGAR;

                #region Alias
                if (SelectExpediente.ALIAS != null)
                {
                    ListAlias = new ObservableCollection<ALIAS>(SelectExpediente.ALIAS);
                }
                #endregion

                #region Apodos
                if (SelectExpediente.APODO != null)
                {
                    ListApodo = new ObservableCollection<APODO>(SelectExpediente.APODO);
                }
                #endregion

                #region Habilitar
                EnableDatosReporte = true;
                #endregion

                MenuReporteEnabled = true;
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_LIBERADO);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener información del imputado.", ex);
            }
            
        }

        private void LimpiarDatosPantalla()
        {
            TextLugarEntrevista = string.Empty;
            InicioDiaDomingo = null;
            TextFechaEntrv = null;


            TextNombreFamiliar = string.Empty;
            SelectParentesco = -1;
            TextTelefonoFamiliar = string.Empty;
            TextCalleFamiliar = string.Empty;
            TextNumInteriorFamiliar = string.Empty;
            TextNumExteriorFamiliar = string.Empty;

            TextDescripcionEntrv = string.Empty;
            TextTecnicasUtilizadas = string.Empty;
            TextExamenMental = string.Empty;
            TextPersonalidad = string.Empty;
            TextNuceloFamPrimario = string.Empty;
            TextNuceloFamSecundario = string.Empty;
            TextObsrv = string.Empty;
            TextSugerencia = string.Empty;
        }

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REGISTRO_LIBERADOS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                foreach (var p in permisos)
                {
                    if (p.INSERTAR == 1)
                    {
                        PInsertar = MenuGuardarEnabled = true;
                    }
                    if (p.EDITAR == 1)
                        PEditar = true;
                    if (p.CONSULTAR == 1)
                    {
                        PConsultar = true;
                    }
                    if (p.IMPRIMIR == 1)
                    {
                        PImprimir = MenuReporteEnabled = true;
                    }

                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }
        #endregion


        private void EliminarApodo()
        {
            ListApodo.Remove(SelectApodo);
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
