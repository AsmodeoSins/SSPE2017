using Cogent.Biometrics;
using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases;
using DPUruNet;
using Microsoft.Reporting.WinForms;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Controlador.Catalogo.Justicia.Liberados;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace ControlPenales
{
    partial class TrabajoSocialViewModel : FingerPrintScanner
    {
        #region Variables
        cOcupacion OcupacionControlador = new cOcupacion();
        cTipoReferencia ParentezcoControlador = new cTipoReferencia();
        cEstadoCivil EstadoCivilControlador = new cEstadoCivil();
        cEscolaridad EscoladridadControlador = new cEscolaridad();
        cTipoReferencia TipoRef = new cTipoReferencia();
        cUsoDrogasFrec FrecDrogaControlador = new cUsoDrogasFrec();
        cDrogas DrogasControlador = new cDrogas();
        cDrogaFrecuencia DrogasFrecControlador = new cDrogaFrecuencia();
        UserControl UserControl_;
        #endregion

        private async void clickSwitch(object op)
        {
            switch (op.ToString())
            {

                #region Uso frecuencia Drogas
                case "insertar_consumodroga":
                    if (!PInsertar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");

                    }
                    else
                    {
                        SelectDrogaConsumo = null;
                        LimpiarDrogaFerecuencia();

                        ValidacionDrogafrecuencia();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_USO_DROGA_TS);
                    }


                    break;
                case "editar_consumodroga":

                    if (!PEditar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                    }
                    else
                    {

                        LimpiarDrogaFerecuencia();

                        if (SelectDrogaConsumo != null)
                        {
                            var ObjIgualConsumoDroga = ListDrogaConsumo.Select(s => new { obj = s }).Where(w => w.obj.Equals(SelectDrogaConsumo)).FirstOrDefault();
                            if (ObjIgualConsumoDroga != null)
                            {
                                popUpDrogaId = ObjIgualConsumoDroga.obj.ID_DROGA;
                                popUpFechaUltDosis = ObjIgualConsumoDroga.obj.INICIO_CONSUMO.Value.Date;
                                popUpFrecuenciaUso = (short)ObjIgualConsumoDroga.obj.ID_FRECUENCIA;
                                ValidacionDrogafrecuencia();
                                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_USO_DROGA_TS);
                            }
                        }
                        else
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "Debe de seleccionar un Registro para Editarlo.");
                        }
                    }



                    break;
                case "eliminar_consumodroga":
                    if (SelectDrogaConsumo != null)
                    {
                        EliminarConsumoDroga();
                    }
                    else
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "Debe de seleccionar un Registro para Editarlo.");
                    }
                    break;
                case "commitUsoDrogas":
                    if (validarCamaposFrecDrogas())
                    {

                        var objDrgaConsumo = new PRS_DROGA()
                        {
                            INICIO_CONSUMO = popUpFechaUltDosis,
                            ID_DROGA = popUpDrogaId,
                            ID_ANIO = SelectExpediente.ID_ANIO,
                            ID_CENTRO = SelectExpediente.ID_CENTRO,
                            ID_IMPUTADO = SelectExpediente.ID_IMPUTADO,
                            ID_FRECUENCIA = popUpFrecuenciaUso
                        };
                        //Verifica si ya existe un registro con los mismos campos 
                        if (!validarDatosIgualesBd(objDrgaConsumo))
                        {
                            Insert_edit_FrecuenciaUsoDroga();
                            RemoverValidacionDrogafrecuencia();
                        }
                        else
                        {

                            (new Dialogos()).ConfirmacionDialogo("Validación", "Ya existe un Registro con la misma Informacion que desea agregar.");
                        }

                    }
                    else
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "Debe de llenar todos los registros para Editarlo o Registrar uno Nuevo.");
                    }
                    break;
                case "rollbackUsoDrogas":
                    RemoverValidacionDrogafrecuencia();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_USO_DROGA_TS);
                    break;


                #endregion
                #region Apoyo Economico Pop
                case "agregar_apoyoEconomico_familiar":
                    SelectPersonasApoyo = null;
                    LimpiarCamposApoyoEcon();

                    ValidacionApoyoEconomicoFamiliar();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_APOYO_ECONOMICO);

                    break;

                case "editar_apoyoEconomico_familiar":
                    //SelectMJ = null;
                    //SelectExpediente = null;
                    //Nuevo();
                    if (!PEditar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                    }
                    else
                    {
                        LimpiarCamposApoyoEcon();
                        if (SelectPersonasApoyo != null)
                        {
                            TextNombreFamiliar = SelectPersonasApoyo.NOMBRE;
                            SelectOcupacionApoyoEconomic = (short)SelectPersonasApoyo.ID_OCUPACION;
                            TextAportaciones = SelectPersonasApoyo.APORTACION.ToString();
                            EditarApoyoEconomico = true;
                            ValidacionApoyoEconomicoFamiliar();
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_APOYO_ECONOMICO);
                        }
                        else
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "Debe de seleccionar un Registro para Editarlo.");
                        }
                    }


                    break;

                case "eliminar_apoyoEconomico_familiar":
                    if (SelectPersonasApoyo != null)
                    {
                        EliminarPersonaApoyo();
                    }
                    else
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "Debe de seleccionar un Registro para Editarlo.");
                    }

                    break;
                case "guardar_apoyoEconomico_familiar":

                    if (SelectPersonasApoyo != null)
                    {//-------------------------------------------------------EDITA------------------------------------------------------------------------------------
                        if (!string.IsNullOrEmpty(TextNombreFamiliar) && !string.IsNullOrEmpty(TextAportaciones) && SelectOcupacionApoyoEconomic > -1)
                        {
                            var ObjApoyoEconomic = new PRS_APOYO_ECONOMICO()
                            {
                                ID_IMPUTADO = SelectExpediente.ID_IMPUTADO,
                                ID_ANIO = SelectExpediente.ID_ANIO,
                                ID_CENTRO = SelectExpediente.ID_CENTRO,
                                NOMBRE = TextNombreFamiliar,
                                ID_OCUPACION = SelectOcupacionApoyoEconomic,
                                OCUPACION = OcupacionControlador.Obtener((int)SelectOcupacionApoyoEconomic).FirstOrDefault(),
                                APORTACION = decimal.Parse(TextAportaciones.ToString())
                            };

                            foreach (var itemPersApoyo in ListPersonasApoyo.Select(s => new { obj = s }).Where(w => w.obj.Equals(SelectPersonasApoyo)))
                            {
                                itemPersApoyo.obj.NOMBRE = ObjApoyoEconomic.NOMBRE;
                                itemPersApoyo.obj.ID_OCUPACION = ObjApoyoEconomic.ID_OCUPACION;
                                itemPersApoyo.obj.OCUPACION = OcupacionControlador.Obtener((int)ObjApoyoEconomic.ID_OCUPACION).FirstOrDefault();
                                itemPersApoyo.obj.APORTACION = ObjApoyoEconomic.APORTACION;
                            }
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_APOYO_ECONOMICO);

                        }
                        else
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "Debe de Registrar todos los datos para Editar el Registro.");
                        }
                    }
                    else
                    {//-------------------------------------------------------AGREGA ------------------------------------------------------------------------------------
                        if (!string.IsNullOrEmpty(TextNombreFamiliar) && !string.IsNullOrEmpty(TextAportaciones) && SelectOcupacionApoyoEconomic > -1)
                        {

                            decimal? Aport = decimal.Parse(TextAportaciones.ToString());
                            var ObjApoyoEconomic = new PRS_APOYO_ECONOMICO()
                            {
                                ID_IMPUTADO = SelectExpediente.ID_IMPUTADO,
                                ID_ANIO = SelectExpediente.ID_ANIO,
                                ID_CENTRO = SelectExpediente.ID_CENTRO,
                                NOMBRE = TextNombreFamiliar,
                                APORTACION = !string.IsNullOrEmpty(TextAportaciones) ? Aport : 0,
                                OCUPACION = OcupacionControlador.Obtener((int)SelectOcupacionApoyoEconomic).FirstOrDefault(),
                                ID_OCUPACION = SelectOcupacionApoyoEconomic
                            };
                            ListPersonasApoyo.Add(ObjApoyoEconomic);
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_APOYO_ECONOMICO);
                        }
                        else
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "Debe de Registrar todos los datos para crear un nuevo Registro.");
                        }
                    }
                    RemoverValidacionApoyoEconomicoFamiliar();
                    // <----------Resetea Variable si Agrega o Edita ---------------->
                    RemoverValidacionApoyoEconomicoFamiliar();
                    ListPersonasApoyo = new ObservableCollection<PRS_APOYO_ECONOMICO>(ListPersonasApoyo);
                    OnPropertyChanged("ListPersonasApoyo");


                    break;

                case "cancelar_apoyoEconomico_familiar":
                    //SelectMJ = null;
                    //SelectExpediente = null;
                    //Nuevo();
                    EditarApoyoEconomico = false;
                    RemoverValidacionApoyoEconomicoFamiliar();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_APOYO_ECONOMICO);
                    break;



                #endregion

                #region Nucelo Familiar Primario Popup
                case "guardar_nucelofamiliar_familiar"://<----Guarda y edita Para los Nucelos Familiar Primario y Secundario
                    if (ValidarCamposNuceloFamiliar())
                    {
                        switch (Tipo_Nucelo_Familiar)
                        {
                            case "P":
                                NucloFamiliar_Primario_Insert_Edit();
                                break;
                            case "S":
                                NucloFamiliar_Secundario_Insert_Edit();
                                break;

                        }
                        LimpiarGrupoFamiliar();
                        RemoverNuceloFamiliar();

                    }
                    else
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "Debe de Registrar todos los datos para Crear o Editar un  Registro.");
                    }
                   
                   

                    break;
                case "cancelar_nucelo_familiar":
                    RemoverNuceloFamiliar();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_NUCELO_FAMILIAR);
                    break;
                case "insertar_nucelo_familiar_primario":


                    if (!PInsertar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                    }
                    else
                    {
                        SelectNuceloPrimarioFam = null;
                        Tipo_Nucelo_Familiar = "P";
                        LimpiarGrupoFamiliar();
                        ValidarNuceloFamiliar();

                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_NUCELO_FAMILIAR);
                    }

                    break;
                case "editar_nucelo_familiar_primario":
                    if (!PEditar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                    }
                    else
                    {
                        LimpiarGrupoFamiliar();
                        if (SelectNuceloPrimarioFam != null)
                        {
                            var ObjetoIgual = ListNuceloPrimarioFam.Select(s => new { obj = s }).Where(w => w.obj.Equals(SelectNuceloPrimarioFam)).FirstOrDefault();
                            TextEdadNuceloFamiliar = ObjetoIgual.obj.EDAD;
                            TextNombreNuceloFamiliar = ObjetoIgual.obj.NOMBRE;
                            SelectEstadoCivilNuceloFamiliar = ObjetoIgual.obj.ID_ESTADO_CIVIL;
                            SelectEscolaridadNuceloFamiliar = ObjetoIgual.obj.ID_ESCOLARIDAD;
                            SelectParentescoNuceloFamiliar = ObjetoIgual.obj.ID_TIPO_REFERENCIA;
                            SelectOcupacionNuceloFamiliar = ObjetoIgual.obj.ID_OCUPACION;
                            Tipo_Nucelo_Familiar = "P";
                            ValidarCamposNuceloFamiliar();
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_NUCELO_FAMILIAR);
                        }
                        else
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "Debe de seleccionar un Registro.");
                        }
                    }

                    break;
                case "eliminar_nucelo_familiar_primario":
                    if (SelectNuceloPrimarioFam != null)
                    {
                        EliminarGrupoFamiliarPrimario();
                    }
                    else
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "Debe de seleccionar un Registro.");
                    }
                    break;
                #endregion

                #region Nucelo Familiar Secundario Popup



                case "insertar_nucleo_familiarSec":

                    if (!PInsertar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                    }
                    else
                    {
                        SelectSecundarioFam = null;
                        LimpiarGrupoFamiliar();
                        Tipo_Nucelo_Familiar = "S";

                        ValidarNuceloFamiliar();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_NUCELO_FAMILIAR);
                    }
                    break;
                case "editar_nucleo_familiarSec":
                    if (!PEditar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                    }
                    else
                    {
                        if (SelectSecundarioFam != null)
                        {
                            LimpiarGrupoFamiliar();
                            var ObjetoIgual = ListSecundarioFam.Select(s => new { obj = s }).Where(w => w.obj.Equals(SelectSecundarioFam)).FirstOrDefault();

                            TextEdadNuceloFamiliar = ObjetoIgual.obj.EDAD;
                            TextNombreNuceloFamiliar = ObjetoIgual.obj.NOMBRE;
                            SelectEstadoCivilNuceloFamiliar = ObjetoIgual.obj.ID_ESTADO_CIVIL;
                            SelectEscolaridadNuceloFamiliar = ObjetoIgual.obj.ID_ESCOLARIDAD;
                            SelectParentescoNuceloFamiliar = ObjetoIgual.obj.ID_TIPO_REFERENCIA;
                            SelectOcupacionNuceloFamiliar = ObjetoIgual.obj.ID_OCUPACION;
                            Tipo_Nucelo_Familiar = "S";
                            ValidarNuceloFamiliar();
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_NUCELO_FAMILIAR);
                        }
                        else
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "Debe de seleccionar un Registro.");
                        }
                    }

                    break;
                case "eliminar_nucleo_familiarSec":
                    if (SelectSecundarioFam != null)
                    {
                        EliminarGrupoFamiliarSecundario();
                    }
                    else
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "Debe de seleccionar un Registro.");
                    }
                    break;
                #endregion

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
                    if (!PConsultar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        break;
                    }
                    else
                    {
                        ClickEnter();
                    }

                    break;
                case "nuevo_expediente":
                    //SelectMJ = null;
                    //SelectExpediente = null;
                    //Nuevo();
                    break;
                case "buscar_nueva_medida":
                    SelectMJ = null; //Se agrega nueva medida
                    // ValidacionesLiberado();
                    Obtener();
                    break;
                case "buscar_seleccionar":
                    if (SelectedProcesoLibertad != null)
                    {
                        SelectedEntrevistaInicial = SelectedProcesoLibertad.PRS_ENTREVISTA_INICIAL.FirstOrDefault();
                        if (SelectedEntrevistaInicial != null)
                        {
                            ObtenerEntrevista(SelectedEntrevistaInicial);
                        }
                        Obtener();
                        #region Comentado
                        //var entrevista = new cEntrevistaInicialTrabajoSocial().ObtenerUltimaEntrevista(SelectExpediente.ID_CENTRO, SelectExpediente.ID_ANIO, SelectExpediente.ID_IMPUTADO);
                        //if (entrevista != null)
                        //    ObtenerEntrevista(entrevista);
                        ////  ValidacionesLiberado();
                        //Obtener();
                        //#endregion
                        StaticSourcesViewModel.SourceChanged = false;
                        // ValidacionesDatosTrabajoSocial();
                    }
                    else
                        (new Dialogos()).ConfirmacionDialogo("Validación", "Favor de seleccionar proceso en libertad.");
                    break;


                case "reporte_menu":
                    //Creacion de Reporte

                    if (!PImprimir)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        break;
                    }
                    else
                    {
                        if (SelectedEntrevistaInicial != null)
                        {
                            if (base.HasErrors)
                            {
                                (new Dialogos()).ConfirmacionDialogo("Validación", "Debe de llenar todos los campo Obligatorios.");
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

                                DatosReporte.Nuc = TextNucEntrevista.Trim();
                                DatosReporte.CausaPenalEntrv = TextCausaPenalEntrevista.Trim();
                                DatosReporte.LugarEntrv = TextLugarEntrevista.Trim();
                                DatosReporte.FechaEntrv = TextFechaEntrv.HasValue ? TextFechaEntrv.Value.ToString("dd/MM/yyyy") : string.Empty;
                                DatosReporte.NombreInterno = ApellidoPaternoBuscar + " " + apellidoMaternoBuscar + " " + NombreBuscar;
                                #endregion
                                //DatosReporte.Escolaridad = SelectExpediente.ESCOLARIDAD != null ? !string.IsNullOrEmpty(SelectExpediente.ESCOLARIDAD.DESCR) ? SelectExpediente.ESCOLARIDAD.DESCR.Trim() : string.Empty : string.Empty;
                                DatosReporte.FechaNacimiento = TextFechaNacimiento.HasValue ? TextFechaNacimiento.Value.ToString("dd/MM/yyyy") : string.Empty;
                                DatosReporte.LugarNacimiento = TextLugarNacimientoExtranjero;
                                DatosReporte.UltimoGradoEstudio = SelectEscolaridad.Value > -1 ? ListEscolaridad.Where(w => w.ID_ESCOLARIDAD == SelectEscolaridad.Value).FirstOrDefault().DESCR : string.Empty;
                                DatosReporte.EstadoCivil = SelectEstadoCivil > -1 ? LstEstadoCivil.Where(w => w.ID_ESTADO_CIVIL == SelectEstadoCivil).FirstOrDefault().DESCR : "";
                                DatosReporte.Telefono = ETelefonoFijo;
                                DatosReporte.GrupoEtnico = SelectGrupoEtnico > -1 ? LstGrupoEtnico.Where(w => w.ID_ETNIA == SelectGrupoEtnico).FirstOrDefault().DESCR : "";

                                DatosReporte.Idioma = SelectedIdioma > -1 ? LstIdioma.Where(w => w.ID_IDIOMA == SelectedIdioma.Value).FirstOrDefault().DESCR : string.Empty;
                                DatosReporte.Religion = SelectReligion > -1 ? ListReligion.Where(w => w.ID_RELIGION == SelectReligion).FirstOrDefault().DESCR : string.Empty;
                                DatosReporte.Sexo = SelectSexo != "SELECCIONE" ? SelectSexo : string.Empty;
                                DatosReporte.EdadInterno = TextEdad.ToString();
                                DatosReporte.Ocupacion = SelectOcupacion > -1 ? ListOcupacion.Where(w => w.ID_OCUPACION == SelectOcupacion).FirstOrDefault().DESCR : "";
                                DatosReporte.TiempoAntiguedad = "Años:" + AniosEstado != null ? AniosEstado.ToString().Trim() : "" + " Meses:" + MesesEstado != null ? MesesEstado.ToString().Trim() : "";



                                //DatosReporte.Domicilio = string.Format("{0} {1} {2} {3}", SelectExpediente.DOMICILIO_CALLE, SelectExpediente.DOMICILIO_NUM_EXT.HasValue ? SelectExpediente.DOMICILIO_NUM_EXT != decimal.Zero ? 
                                //(SelectExpediente.MUNICIPIO.MUNICIPIO1) ? SelectExpediente.MUNICIPIO.MUNICIPIO1.Trim() : string.Empty : string.Empty); 
                                DatosReporte.Domicilio = string.Format("Calle:{0} Num Exterior:{1} Num Interior{2}", TextCalle, TextNumeroExterior, TextNumeroInterior);
                                DatosReporte.Sexo = selectExpediente.SEXO;
                                DatosReporte.Fecha = Fechas.GetFechaDateServer.ToShortDateString();
                                DatosReporte.oficio = textOficio;

                                DatosReporte.DomicilioReferencia = TextDomicilioReferencia != null ? TextDomicilioReferencia.Trim() : "";
                                //DatosReporte.domil = textOficio;
                                // DatosReporte.EstadoCivil = LstEstadoCivil.Where(w => w.ID_ESTADO_CIVIL == (short)SelectEstadoCivil.Value).FirstOrDefault().DESCR;
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
                                            DatosReporte.Apodo += entidad.ID_APODO == ultimoApodo ? entidad.APODO1 + "" : entidad.APODO1 + ",";
                                        }
                                    }

                                }
                                DatosReporte.Apodo = DatosReporte.Apodo != null ? DatosReporte.Apodo.Trim() : string.Empty;
                                listAlias = new ObservableCollection<ALIAS>(selectExpediente.ALIAS);
                                if (listAlias != null && listAlias.Count > 0)
                                {

                                    short ultimoAlias = selectExpediente.ALIAS.LastOrDefault().ID_ALIAS;
                                    foreach (var entidad in selectExpediente.ALIAS.OrderBy(o => o.ID_ALIAS))
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

                                }

                                #region Situacion Juridica
                                DatosReporte.Delito = TextDelitoImputa.Trim();
                                #region Datos Persona Apoyo
                                DatosReporte.NombreAoyo = TextNombreApoyo.Trim();
                                DatosReporte.DomicilioAoyo = TextEdadApoyo.ToString();
                                DatosReporte.TelefonoAoyo = MetodogenericoValidacionTextBox(TextTelefonoApoyo);
                                DatosReporte.OcupacionAoyo = ListOcupacionesApoyo.Where(w => w.ID_OCUPACION == SelectOcupacionApoyo.Value).FirstOrDefault().DESCR;
                                DatosReporte.ParentescoAoyo = SelectParentesco > -1 ? ListParentesco.Where(w => w.ID_TIPO_REFERENCIA == SelectParentesco).FirstOrDefault().DESCR : "";
                                DatosReporte.TiempoConocerAoyo = TextTiempoConocerceApoyo.Trim();
                                DatosReporte.EdadApoyo = TextEdadApoyo.ToString().Trim();
                                DatosReporte.TiempoRadicaEstado = string.Format("Años:{0} Dias{1}", AniosEstado.Trim(), MesesEstado.Trim());
                                DatosReporte.DomicilioAoyo = string.Format("Calle: {0} Numero Exterior: {1} Numero Interior: {2}", TextCalleApoyo.Trim(), TextNumeroExteriorApoyo != null ? TextNumeroExteriorApoyo.ToString().Trim() : "", !string.IsNullOrEmpty(TextNumeroInteriorApoyo) ? TextNumeroInteriorApoyo.Trim() : "");

                                #endregion

                                #region Estudio Socio Economico
                                DatosReporte.TipoVivienda = TextTipoVivienda != null ? TextTipoVivienda.Trim() : "";
                                DatosReporte.PersonasResidenVivienda = TextNoPersonasResiden != null ? TextNoPersonasResiden.ToString().Trim() : "";
                                DatosReporte.PersonaViviaAntesSerDtenido = TextPersonaViviaAntes != null ? TextPersonaViviaAntes.Trim() : "";
                                DatosReporte.PersonaViviaAntesSerDtenidoOtra = TextOtrasPersonasViviaAntes != null ? TextOtrasPersonasViviaAntes.Trim() : "";
                                DatosReporte.TipoMterialVivienda = TextTipoMaterialVivienda != null ? TextTipoMaterialVivienda.Trim() : "";
                                DatosReporte.CondicionZona = TextCondicionesZona != null ? TextCondicionesZona.Trim() : "";
                                DatosReporte.SituacionEconomica = TextSituacionEconomica != null ? TextSituacionEconomica.Trim() : "";
                                DatosReporte.LuzE = TextEgresoLuz.ToString().Trim();
                                DatosReporte.EducacionE = TextEgresoEducacion.ToString().Trim();
                                DatosReporte.RentaE = TextEgresoRenta.ToString().Trim();
                                DatosReporte.AguaE = TextEgresoAgua.ToString().Trim();
                                DatosReporte.CombustibleE = TextEgresoCombustible.ToString().Trim();
                                DatosReporte.GasE = TextEgresoGas.ToString().Trim();
                                DatosReporte.TelefonoE = TextEgresoTelefono.ToString().Trim();
                                DatosReporte.vestimentaE = TextEgresoVestimenta.ToString().Trim();
                                DatosReporte.GastosMedicosE = TextEgresoGatosMedicos.ToString().Trim();
                                DatosReporte.DespensaE = TextEgresoDespensa.ToString().Trim();
                                DatosReporte.LecheE = TextEgresoLeche.ToString().Trim();
                                DatosReporte.LeguiminosasE = TextEgresoLeguiminosas.ToString().Trim();
                                DatosReporte.FrijolE = TextEgresoFrijol.ToString().Trim();
                                DatosReporte.TortillasE = TextEgresoTortillas.ToString().Trim();
                                DatosReporte.PolloE = TextEgresoPolllo.ToString().Trim();
                                DatosReporte.cerealesE = TextEgresoCereales.ToString().Trim();
                                DatosReporte.PastasE = TextEgresoPastas.ToString().Trim();
                                DatosReporte.CarneE = TextEgresoCarneRoja.ToString().Trim();
                                DatosReporte.VerdurasE = TextEgresoverduras.ToString().Trim();
                                DatosReporte.GolosinasE = TextEgresoGolosinas.ToString().Trim();
                                DatosReporte.OtroEgreso = TextEgresoOtros.ToString().Trim();

                                //<--------------------------------Suma de todos los Ingresos ---------------------------->
                                decimal? TotalEgreeso = 0;
                                TotalEgreeso += TextEgresoLuz != null ? TextEgresoLuz : 0;
                                TotalEgreeso += TextEgresoEducacion != null ? TextEgresoEducacion : 0;
                                TotalEgreeso += TextEgresoRenta != null ? TextEgresoRenta : 0;
                                TotalEgreeso += TextEgresoAgua != null ? TextEgresoAgua : 0;
                                TotalEgreeso += TextEgresoGas != null ? TextEgresoGas : 0;
                                TotalEgreeso += TextEgresoTelefono != null ? TextEgresoTelefono : 0;
                                TotalEgreeso += TextEgresoVestimenta != null ? TextEgresoVestimenta : 0;
                                TotalEgreeso += TextEgresoGatosMedicos != null ? TextEgresoGatosMedicos : 0;
                                TotalEgreeso += TextEgresoDespensa != null ? TextEgresoDespensa : 0;
                                TotalEgreeso += TextEgresoLeche != null ? TextEgresoLeche : 0;
                                TotalEgreeso += TextEgresoLeguiminosas != null ? TextEgresoLeguiminosas : 0;
                                TotalEgreeso += TextEgresoFrijol != null ? TextEgresoFrijol : 0;
                                TotalEgreeso += TextEgresoCombustible != null ? TextEgresoCombustible : 0;
                                TotalEgreeso += TextEgresoGolosinas != null ? TextEgresoGolosinas : 0;
                                TotalEgreeso += TextEgresoTortillas != null ? TextEgresoTortillas : 0;
                                TotalEgreeso += TextEgresoPolllo != null ? TextEgresoPolllo : 0;
                                TotalEgreeso += TextEgresoCereales != null ? TextEgresoCereales : 0;
                                TotalEgreeso += TextEgresoCarneRoja != null ? TextEgresoCarneRoja : 0;
                                TotalEgreeso += TextEgresoPastas != null ? TextEgresoPastas : 0;
                                TotalEgreeso += TextEgresoverduras != null ? TextEgresoverduras : 0;
                                TotalEgreeso += TextEgresoOtros != null ? TextEgresoOtros : 0;

                                DatosReporte.TotalEgresos = TotalEgreeso.ToString().Trim();
                                DatosReporte.ComidaAlDiaE = TextComidasAlDia.ToString().Trim();


                                #region Personas Apoyan Economicamente
                                var ListaDatosReportePersonasApoyanHogar = new List<cApoyoFamiliar>();
                                foreach (var itemApoyopEcon in ListPersonasApoyo)
                                {
                                    var DatosReportePersonasApoyanHogar = new cApoyoFamiliar();
                                    DatosReportePersonasApoyanHogar.NombreFamApoyEcon = itemApoyopEcon.NOMBRE.Trim();
                                    DatosReportePersonasApoyanHogar.oficioApoyoEcon = itemApoyopEcon.OCUPACION.DESCR;
                                    DatosReportePersonasApoyanHogar.AportacionesIngresoHogarEcon = itemApoyopEcon.APORTACION.ToString().Trim();
                                    ListaDatosReportePersonasApoyanHogar.Add(DatosReportePersonasApoyanHogar);
                                }
                                #endregion

                                #endregion
                                #region Situacion Actual
                                DatosReporte.ConoceVecinos = ConoceVecinosSi == true ? "Si" : "No";
                                DatosReporte.ProblemasVecionos = ProblemasConVecinosSi == true ? "Si" : "No";
                                DatosReporte.DedicaTiempoLibre = TextTiempoLibre;
                                DatosReporte.DedicaTiempoLibreOtro = TextTiempoLibreOtro;
                                DatosReporte.cartilla = chkCartilla == true ? "Si" : "No";
                                DatosReporte.VisaLaser = chkVisaLaser == true ? "Si" : "No";
                                DatosReporte.ActaNacimiento = chkActaNac == true ? "Si" : "No";
                                DatosReporte.Curp = chkCurp == true ? "Si" : "No";
                                DatosReporte.ComprobanteEstudio = chkComprobanteEstudio == true ? "Si" : "No";
                                DatosReporte.Licencia = chkLiciencia == true ? "Si" : "No";
                                DatosReporte.ActaMatrimonio = chkActaMatrimonio == true ? "Si" : "No";
                                DatosReporte.ComprobanteEstudios = chkComprobanteEstudio == true ? "Si" : "No";
                                DatosReporte.OtroDocumento = TextDocumentosOtro;
                                DatosReporte.PadecePadecioEnfermedad = PadeceEnfermedadSi == true ? "Si" : "No";
                                DatosReporte.PadecePadecioEnfermedadOtra = !string.IsNullOrEmpty(TextEspecifiqueOtraEnfermedad) ? TextEspecifiqueOtraEnfermedad.Trim() : "";
                                DatosReporte.TipoTRatamientoRecibido = !string.IsNullOrEmpty(TextTipoTratamientoRecibido) ? TextTipoTratamientoRecibido.Trim() : "";
                                DatosReporte.EspecifiqueEnfermedad = MetodogenericoValidacionTextBox(TextEspecifiqueOtraEnfermedad);
                                DatosReporte.Diagnostico = textDiagnostico.Trim();


                                #endregion

                                #region Estructura Dinamica Familiar(Completado)
                                DatosReporte.FomraPoruqe = !string.IsNullOrEmpty(TextFormaPorqueApoyo) ? TextFormaPorqueApoyo.Trim() : "";
                                DatosReporte.PadresVIvenJuntos = ViveConPadres == true ? "Si" : "No";
                                DatosReporte.AntecedentesMiembroFmiliar = ExitenAntecedentespenalesFamiiarSi == true ? "Si" : "No";
                                DatosReporte.FmiliaresConsumanSustanciasToxicas = FamiliarConsumeSustanciaSi == true ? "Si" : "No";
                                DatosReporte.ConsumeAlgunTipoDroga = ConsumidoAlgunTipoDrogaSi == true ? "Si" : "No";
                                DatosReporte.AntecedentesPenalesAlActual = AntecedentesPernalesSi == true ? "Si" : "No";
                                DatosReporte.PasaporteMexicano = chkPasaporteMex == true ? "Si" : "No";
                                DatosReporte.Ife = chkIFE == true ? "Si" : "No";


                                DatosReporte.DeQuienRecibioApoyo = !string.IsNullOrEmpty(TextDeQuienRecibioApoyoInternamiento) ? TextDeQuienRecibioApoyoInternamiento.Trim() : "";
                                DatosReporte.Frecuencia = !string.IsNullOrEmpty(TextFrecuencia) ? TextFrecuencia.Trim() : "";


                                //RecibioApoyoInternamientoSi
                                DatosReporte.RecibioApoyoDuranteInternamiento = RecibioApoyoInternamientoSi == true ? "Si" : "No";
                                DatosReporte.DesdeCuandoViveConPadres = MetodogenericoValidacionTextBox(DesdeCuandoVivePadres);
                                DatosReporte.DescrDinamicaFamiliar = DescrDinamicaFamiliar;
                                DatosReporte.ExistioProblemaFamiliar = ProblemaFamiliarSi == true ? "Si" : "No";
                                DatosReporte.MiembroFamiliarAbandonoHogar = MiembroFamiliaAbandonoHogarSi == true ? "Si" : "No";
                                DatosReporte.RecibidoApoyoFamiliarDuranteProcesoJudicial = RecibioApoyoEconomicoEnProcesojudicialSi == true ? "Si" : "No";
                                DatosReporte.TieneProblemasPareja = ProblemaParejaSi == true ? "Si" : "No";
                                DatosReporte.UnionesAnteriores = TextunionesAnteriores;
                                DatosReporte.NoHiijos = NoHijos.ToString();

                                #region Nucelo Familiar Popup
                                //Nucleo fam pRIMARIO
                                var ListaNuceloFamPrimario = new List<cNucleoFamiliarTS>();
                                foreach (var itemFamPrimario in ListNuceloPrimarioFam)
                                {
                                    var NucleoFamiliarPrimario = new cNucleoFamiliarTS();
                                    NucleoFamiliarPrimario.nombreNuceloFam = itemFamPrimario.NOMBRE.Trim();
                                    NucleoFamiliarPrimario.parentescoNuceloFam = ParentezcoControlador.Obtener((int)itemFamPrimario.ID_TIPO_REFERENCIA).FirstOrDefault().DESCR;
                                    NucleoFamiliarPrimario.edadNuceloFam = itemFamPrimario.EDAD.ToString().Trim();
                                    NucleoFamiliarPrimario.escolaridadNuceloFam = itemFamPrimario.ESCOLARIDAD.DESCR.Trim();
                                    NucleoFamiliarPrimario.estadocivilcoNuceloFam = itemFamPrimario.ESTADO_CIVIL.DESCR.Trim();
                                    NucleoFamiliarPrimario.ocupacionNuceloFam = itemFamPrimario.OCUPACION.DESCR.Trim();
                                    ListaNuceloFamPrimario.Add(NucleoFamiliarPrimario);


                                }
                                //Nucleo fam sECUNDARIO
                                var ListaNuceloFamSecundario = new List<cNucleoFamiliarSecundarioTS>();
                                foreach (var itemNuceloSec in ListSecundarioFam)
                                {
                                    var NucleoFamiliarSecundario = new cNucleoFamiliarSecundarioTS();
                                    NucleoFamiliarSecundario.nombreNuceloFamSec = itemNuceloSec.NOMBRE.Trim();
                                    NucleoFamiliarSecundario.parentescoNuceloFamSec = ParentezcoControlador.Obtener((int)itemNuceloSec.ID_TIPO_REFERENCIA).FirstOrDefault().DESCR;
                                    NucleoFamiliarSecundario.edadNuceloFamSec = itemNuceloSec.EDAD.ToString().Trim();
                                    NucleoFamiliarSecundario.escolaridadNuceloFamSec = itemNuceloSec.ESCOLARIDAD.DESCR.Trim();
                                    NucleoFamiliarSecundario.estadocivilcoNuceloFamSec = itemNuceloSec.ESTADO_CIVIL.DESCR.Trim();
                                    NucleoFamiliarSecundario.ocupacionNuceloFamSec = itemNuceloSec.OCUPACION.DESCR.Trim();
                                    ListaNuceloFamSecundario.Add(NucleoFamiliarSecundario);

                                }

                                #endregion

                                #region Consumo Droga Popup
                                var ListaFrecunciaUsoDroga = new List<cUsoDrogaFrecuenciaTS>();
                                foreach (var itemDrogaConsumo in ListDrogaConsumo)
                                {
                                    var ConsumoDrogaFrec = new cUsoDrogaFrecuenciaTS();
                                    ConsumoDrogaFrec.TipoDroga = itemDrogaConsumo.DROGA.DESCR;
                                    ConsumoDrogaFrec.InicioConsumo = itemDrogaConsumo.INICIO_CONSUMO.Value.ToShortDateString();
                                    ConsumoDrogaFrec.FrecuenciaUso = itemDrogaConsumo.DROGA_FRECUENCIA.DESCR;
                                    ListaFrecunciaUsoDroga.Add(ConsumoDrogaFrec);
                                }
                                #endregion
                                #endregion

                                #endregion

                                #region Estructura Dinamica Familiar
                                //ListaEstructuraVivienda = new ObservableCollection<EstructuraViviendaClass>(ListaEstructuraVivienda);
                                var listaEstructuraDinamicaFamiliar = new List<cEstructuraViviendaTS>();


                                short Num = 0;
                                var ComedorEstructura = new cEstructuraViviendaTS();
                                ComedorEstructura.CategoriaEV = "Comedor";
                                ComedorEstructura.NumEstructuraEV = TextComedorNum != null ? TextComedorNum.ToString().Trim() : "";
                                ComedorEstructura.observacionEV = !string.IsNullOrEmpty(TextComedorObserv) ? TextComedorObserv.Trim() : "";
                                listaEstructuraDinamicaFamiliar.Add(ComedorEstructura);


                                var RecamarasEstructura = new cEstructuraViviendaTS();
                                RecamarasEstructura.CategoriaEV = "Recamaras";
                                RecamarasEstructura.NumEstructuraEV = TextRecamaraNum != null ? TextRecamaraNum.ToString().Trim() : "";
                                RecamarasEstructura.observacionEV = !string.IsNullOrEmpty(TextRecamaraObserv) ? TextRecamaraObserv.Trim() : "";
                                listaEstructuraDinamicaFamiliar.Add(RecamarasEstructura);


                                var SalaEstructura = new cEstructuraViviendaTS();
                                SalaEstructura.CategoriaEV = "Sala";
                                SalaEstructura.NumEstructuraEV = TextSalaNum != null ? TextSalaNum.ToString().Trim() : "";
                                SalaEstructura.observacionEV = !string.IsNullOrEmpty(TextSalaObserv) ? TextSalaObserv.Trim() : "";
                                listaEstructuraDinamicaFamiliar.Add(SalaEstructura);

                                var CocinaEstructura = new cEstructuraViviendaTS();
                                CocinaEstructura.CategoriaEV = "Cocina";
                                CocinaEstructura.NumEstructuraEV = TextCocinaNum != null ? TextCocinaNum.ToString().Trim() : "";
                                CocinaEstructura.observacionEV = !string.IsNullOrEmpty(TextCocinaObserv) ? TextCocinaObserv.Trim() : "";
                                listaEstructuraDinamicaFamiliar.Add(CocinaEstructura);


                                var BañoEstructura = new cEstructuraViviendaTS();
                                BañoEstructura.CategoriaEV = "Baño";
                                BañoEstructura.NumEstructuraEV = TextBañoNum != null ? TextBañoNum.ToString().Trim() : "";
                                BañoEstructura.observacionEV = !string.IsNullOrEmpty(TextBañoObserv) ? TextBañoObserv.Trim() : "";
                                listaEstructuraDinamicaFamiliar.Add(BañoEstructura);


                                var ventanasEstructura = new cEstructuraViviendaTS();
                                ventanasEstructura.CategoriaEV = "Ventanas";
                                ventanasEstructura.NumEstructuraEV = TextVentanasNum != null ? TextVentanasNum.ToString().Trim() : "";
                                ventanasEstructura.observacionEV = !string.IsNullOrEmpty(TextVentanasObserv) ? TextVentanasObserv.Trim() : "";
                                listaEstructuraDinamicaFamiliar.Add(ventanasEstructura);


                                var PatiosEstructura = new cEstructuraViviendaTS();
                                PatiosEstructura.CategoriaEV = "Patio";
                                PatiosEstructura.NumEstructuraEV = TextPatioNum != null ? TextPatioNum.ToString().Trim() : "";
                                PatiosEstructura.observacionEV = !string.IsNullOrEmpty(TextPatioObserv) ? TextPatioObserv.Trim() : "";
                                listaEstructuraDinamicaFamiliar.Add(PatiosEstructura);

                                #endregion

                                //DatosReporte.Delito = ;
                                //DatosReporte.MedidaCautelar=SelectMJ.MEDIDA_JUDICIAL;
                                // }
                                #endregion

                                #region Se forma el encabezado del reporte
                                var Encabezado = new cEncabezado();
                                Encabezado.TituloUno = "UNIDAD DE JUSTICIA RESTAURATIVA Y TRATAMIENTO";
                                Encabezado.TituloDos = Parametro.ENCABEZADO2;
                                Encabezado.NombreReporte = "TRABAJO SOCIAL";
                                Encabezado.ImagenIzquierda = Parametro.LOGO_ESTADO;
                                Encabezado.ImagenFondo = Parametro.REPORTE_LOGO2;

                                #endregion

                                #region Inicializacion de reporte
                               // View.Report.ProcessingMode = ProcessingMode.Local;
                                View.Report.LocalReport.ReportPath = "Reportes/rEntrevistaInicialTrabajoSocial.rdlc";
                                View.Report.LocalReport.DataSources.Clear();
                                #endregion

                                #region Definicion de origenes de datos


                                //datasource Uno
                                //var ds1 = new List<cEncabezado>();
                                //ds1.Add(Encabezado);
                                //Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                                //rds1.Name = "DataSet3";
                                //rds1.Value = ds1;
                                //View.Report.LocalReport.DataSources.Add(rds1);


                                //datasource dos
                                var ds2 = new List<cEntrevistaInicial>();
                                ds2.Add(DatosReporte);
                                Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                                rds2.Name = "DataSet1";
                                rds2.Value = ds2;
                                View.Report.LocalReport.DataSources.Add(rds2);

                                #region DATOS REPORTE APOYO ECONOMICO POPUP
                                //ListaDatosReportePersonasApoyanHogar
                                Microsoft.Reporting.WinForms.ReportDataSource rds3 = new Microsoft.Reporting.WinForms.ReportDataSource();
                                rds3.Name = "DataSet2";
                                rds3.Value = ListaDatosReportePersonasApoyanHogar;
                                View.Report.LocalReport.DataSources.Add(rds3);
                                #endregion


                                #region DATOS REPORTE NUCLEO FAMILIAR PRIMARIO POPUP
                                Microsoft.Reporting.WinForms.ReportDataSource rds4 = new Microsoft.Reporting.WinForms.ReportDataSource();
                                rds4.Name = "DataSet3";
                                rds4.Value = ListaNuceloFamPrimario;
                                View.Report.LocalReport.DataSources.Add(rds4);
                                #endregion
                                #region DATOS REPORTE NUCLEO FAMILIAR SECUNDARIO  POPUP
                                Microsoft.Reporting.WinForms.ReportDataSource rds5 = new Microsoft.Reporting.WinForms.ReportDataSource();
                                rds5.Name = "DataSet6";
                                rds5.Value = ListaNuceloFamSecundario;
                                View.Report.LocalReport.DataSources.Add(rds5);
                                #endregion
                                #region DATOS REPORTE FRECUENCIA USO DROGAS POPUP
                                Microsoft.Reporting.WinForms.ReportDataSource rds6 = new Microsoft.Reporting.WinForms.ReportDataSource();
                                rds6.Name = "DataSet4";
                                rds6.Value = ListaFrecunciaUsoDroga;
                                View.Report.LocalReport.DataSources.Add(rds6);
                                #endregion

                                #region DATOS REPORTE ESTRUCTURA DINAMICA FAMILIAR POPUP

                                Microsoft.Reporting.WinForms.ReportDataSource rds7 = new Microsoft.Reporting.WinForms.ReportDataSource();
                                rds7.Name = "DataSet5";
                                rds7.Value = listaEstructuraDinamicaFamiliar;
                                View.Report.LocalReport.DataSources.Add(rds7);
                                #endregion
                                #region DATOS REPORTE ENCABEZADO REPORTE
                                var dsEncabezado = new List<cEncabezado>();
                                dsEncabezado.Add(Encabezado);
                                Microsoft.Reporting.WinForms.ReportDataSource rds8 = new Microsoft.Reporting.WinForms.ReportDataSource();
                                rds8.Name = "DataSet7";
                                rds8.Value = dsEncabezado;
                                View.Report.LocalReport.DataSources.Add(rds8);
                                #endregion


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

                                #endregion

                        }
                        else { new Dialogos().ConfirmacionDialogo("Validación", "Favor de guardar antes de imprimir."); }
                    }
                    break;

                case "limpiar_menu":
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new TrabajoSocialView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.TrabajoSocialViewModel();
                    break;
                case "guardar_menu":
                    if(SelectedProcesoLibertad == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación","Favor de seleccionar un proceso en libertad");
                        break;
                    }
                    Guardar();
                    break;
                case "Open442":
                    if (!PConsultar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
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

        private void LimpiarCamposApoyoEcon()
        {
            TextNombreFamiliar = string.Empty;
            TextAportaciones = string.Empty;
            SelectOcupacionApoyoEconomic = -1;
        }
        
        private void EliminarPersonaApoyo()
        {
            if (SelectPersonasApoyo.ID_FOLIO > 0)
            {
                (new Dialogos()).ConfirmacionDialogo("Validación", "No es Posible eliminar este registro ya que fue Guardado Anteriormente.");
            }
            else
            {
                ListPersonasApoyo.Remove(SelectPersonasApoyo);
                (new Dialogos()).ConfirmacionDialogo("Validación", "Registro fue eliminado con exito.");
            }

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
                   
                    var ObjetoEntrevistaInicial = new PRS_ENTREVISTA_INICIAL();
                    ObjetoEntrevistaInicial.ID_IMPUTADO = SelectedProcesoLibertad.ID_IMPUTADO;
                    ObjetoEntrevistaInicial.ID_ANIO = SelectedProcesoLibertad.ID_ANIO;
                    ObjetoEntrevistaInicial.ID_CENTRO = SelectedProcesoLibertad.ID_CENTRO;
                    ObjetoEntrevistaInicial.ID_PROCESO_LIBERTAD = SelectedProcesoLibertad.ID_PROCESO_LIBERTAD;
                    
                    ObjetoEntrevistaInicial.LUGAR = TextLugarEntrevista.Trim();
                    ObjetoEntrevistaInicial.NUC = TextNucEntrevista.Trim();
                    ObjetoEntrevistaInicial.CAUSA_PENAL = TextCausaPenalEntrevista.Trim();
                    ObjetoEntrevistaInicial.REGISTRO_FEC = TextFechaEntrv;

                    ObjetoEntrevistaInicial.II1_DELITO = TextDelitoImputa.Trim();
                    ObjetoEntrevistaInicial.I_DOMICILIO_REFERNCIA = TextDomicilioReferencia;
                    ObjetoEntrevistaInicial.III1_NOMBRE = TextNombreApoyo.Trim();
                    ObjetoEntrevistaInicial.III2_EDAD = TextEdadApoyo.ToString().Trim();

                    ObjetoEntrevistaInicial.III3_CALLE = TextCalleApoyo;
                    ObjetoEntrevistaInicial.III3_NUM_EXTERIOR = TextNumeroExteriorApoyo.ToString();
                    ObjetoEntrevistaInicial.III3_NUM_INTERIOR = TextNumeroInteriorApoyo != null ? TextNumeroInteriorApoyo.ToString() : string.Empty;
                    //ObjetoEntrevistaInicial.III3_DOMICILIO = (TextCalleApoyo + " " + TextNumeroExteriorApoyo + " " + TextNumeroInteriorApoyo).Trim();
                    ObjetoEntrevistaInicial.III4_TELEFONO = TextTelefonoApoyo.Trim();
                    ObjetoEntrevistaInicial.III5_OCUPACION = SelectOcupacionApoyo != null ? SelectOcupacionApoyo : -1;//ListOcupacionesApoyo.Where(w => w.ID_OCUPACION == SelectOcupacionesApoyo).FirstOrDefault().DESCR;
                    ObjetoEntrevistaInicial.III7_TIEMPO_CONOCERCE = TextTiempoConocerceApoyo.Trim();
                    ObjetoEntrevistaInicial.III6_PARENTESCO = SelectParentesco != null ? SelectParentesco : -1;//SelectParentesco > -1 ? ListParentesco.Where(w => w.ID_TIPO_REFERENCIA == SelectParentesco).FirstOrDefault().DESCR : "";

                    #region Estudio Socio Economico
                    ObjetoEntrevistaInicial.IV1_VIVIENDA = TextTipoVivienda.Trim();
                    ObjetoEntrevistaInicial.IV2_PERSONAS_VIVEN = short.Parse(TextNoPersonasResiden.ToString());

                    #region Documentos Personales

                    #endregion

                    ObjetoEntrevistaInicial.IV3_VIVIA = TextPersonaViviaAntes.Trim();
                    ObjetoEntrevistaInicial.IV4_OTRO = !string.IsNullOrEmpty(TextOtrasPersonasViviaAntes) ? TextOtrasPersonasViviaAntes.Trim() : "";
                    ObjetoEntrevistaInicial.IV5_MATERIAL_VIVIENDA = TextTipoMaterialVivienda.Trim();
                    ObjetoEntrevistaInicial.IV6_OTRO = !string.IsNullOrEmpty(TextOtrasPersonasViviaAntes) ? TextOtrasPersonasViviaAntes.Trim() : "";

                    #region GuardarEstructuraVivienda
                    short Num = 0;
                    ObjetoEntrevistaInicial.IV7_COMEDOR = TextComedorNum != null ? TextComedorNum : Num;
                    ObjetoEntrevistaInicial.IV7_OBS_COMEDOR = !string.IsNullOrEmpty(TextComedorObserv) ? TextComedorObserv.Trim() : "";

                    ObjetoEntrevistaInicial.IV7_RECAMARAS = TextRecamaraNum != null ? TextRecamaraNum : Num;
                    ObjetoEntrevistaInicial.IV7_OBS_RECAMARAS = !string.IsNullOrEmpty(TextRecamaraObserv) ? TextRecamaraObserv.Trim() : "";

                    ObjetoEntrevistaInicial.IV7_SALA = TextSalaNum != null ? TextSalaNum : Num;
                    ObjetoEntrevistaInicial.IV7_OBS_SALA = !string.IsNullOrEmpty(TextSalaObserv) ? TextSalaObserv.Trim() : "";


                    ObjetoEntrevistaInicial.IV7_COCINA = TextCocinaNum != null ? TextCocinaNum : 0;
                    ObjetoEntrevistaInicial.IV7_OBS_COCINA = !string.IsNullOrEmpty(TextCocinaObserv) ? TextCocinaObserv.Trim() : "";

                    ObjetoEntrevistaInicial.IV7_BANIO = TextBañoNum != null ? TextBañoNum : Num;
                    ObjetoEntrevistaInicial.IV7_OBS_BANIO = !string.IsNullOrEmpty(TextBañoObserv) ? TextBañoObserv.Trim() : "";

                    ObjetoEntrevistaInicial.IV7_VENTANAS = TextVentanasNum != null ? TextVentanasNum : Num;
                    ObjetoEntrevistaInicial.IV7_OBS_VENTANAS = !string.IsNullOrEmpty(TextVentanasObserv) ? TextVentanasObserv.Trim() : "";

                    ObjetoEntrevistaInicial.IV7_PATIOS = TextPatioNum != null ? TextPatioNum : Num;
                    ObjetoEntrevistaInicial.IV7_OBS_PATIOS = !string.IsNullOrEmpty(TextPatioObserv) ? TextPatioObserv.Trim() : "";




                    #endregion
                    ObjetoEntrevistaInicial.IV8_CONDICIONES_ZONA = TextCondicionesZona.Trim();
                    ObjetoEntrevistaInicial.IV9_SITUACION_ECONOMICA = TextSituacionEconomica.Trim();

                    //Guardar Apoyo Economico Lista(Pendiente)
                    ///
                    ObjetoEntrevistaInicial.IV11_LUZ = (int)TextEgresoLuz;
                    ObjetoEntrevistaInicial.IV11_EDUCACION = (int)TextEgresoEducacion;
                    ObjetoEntrevistaInicial.IV11_COMBUSTIBLE = (int)TextEgresoCombustible;
                    ObjetoEntrevistaInicial.IV11_RENTA = (int)TextEgresoRenta;
                    ObjetoEntrevistaInicial.IV11_AGUA = (int)TextEgresoAgua;
                    ObjetoEntrevistaInicial.IV11_GAS = (int)TextEgresoGas;
                    ObjetoEntrevistaInicial.IV11_TELEFONO = (int)TextEgresoTelefono;
                    ObjetoEntrevistaInicial.IV11_VESTIMENTA = (int)TextEgresoVestimenta;
                    ObjetoEntrevistaInicial.IV11_GASTO_MEDICO = (int)TextEgresoGatosMedicos;
                    ObjetoEntrevistaInicial.IV11_DESPENSA = (int)TextEgresoDespensa;
                    ObjetoEntrevistaInicial.IV11_VESTIMENTA = (int)TextEgresoVestimenta;
                    ObjetoEntrevistaInicial.IV12_LEGUMINOSA = TextEgresoLeguiminosas;
                    ObjetoEntrevistaInicial.IV12_LECHE = TextEgresoLeche;
                    ObjetoEntrevistaInicial.IV12_FRIJOL = TextEgresoFrijol;
                    ObjetoEntrevistaInicial.IV12_POLLO = TextEgresoPolllo;
                    ObjetoEntrevistaInicial.IV12_TORTILLAS = TextEgresoTortillas;
                    ObjetoEntrevistaInicial.IV12_PASTA = TextEgresoPastas;
                    ObjetoEntrevistaInicial.IV12_CARNE = TextEgresoCarneRoja;
                    ObjetoEntrevistaInicial.IV12_VERDURA = TextEgresoverduras;
                    ObjetoEntrevistaInicial.IV12_CEREALES = TextEgresoCereales;
                    ObjetoEntrevistaInicial.IV12_GOLOSINAS = TextEgresoGolosinas;
                    ObjetoEntrevistaInicial.IV12_OTROS = TextEgresoOtros != null ? TextEgresoOtros : null;
                    ObjetoEntrevistaInicial.IV12_COMIDA_ELABORA = TextComidasAlDia;
                    ObjetoEntrevistaInicial.IV12_OTROSDESCR = !string.IsNullOrEmpty(TextEgresoDescrOtros) ? TextEgresoDescrOtros.Trim() : "";
                    ObjetoEntrevistaInicial.IV11_TOTAL_EGRESO = TextEgresoTotal;
                    #endregion

                    #region Estructura Dinamica  Familiar
                    //ObjetoEntrevistaInicial.IV3_VIVIA = TextPersonaViviaAntes;
                    //ObjetoEntrevistaInicial.IV4_OTRO = TextOtrasPersonasViviaAntes;
                    //ObjetoEntrevistaInicial.IV5_MATERIAL_VIVIENDA = TextTipoMaterialVivienda;
                    ObjetoEntrevistaInicial.V2_DESCR_DINAMICA_FAM = DescrDinamicaFamiliar.Trim();
                    ObjetoEntrevistaInicial.V3_EXISTO_PROBLEMA_FAM = ProblemaFamiliarSi == true ? "S" : "N"; ;
                    ObjetoEntrevistaInicial.V4_MIEMBRO_FAM_ABANDONO = MiembroFamiliaAbandonoHogarSi == true ? "S" : "N";
                    ObjetoEntrevistaInicial.V5_APOYO_ECON_PROC_JUDICIAL = RecibioApoyoEconomicoEnProcesojudicialSi == true ? "S" : "N";
                    ObjetoEntrevistaInicial.V5_FORMA_PORQUE = !string.IsNullOrEmpty(TextFormaPorqueApoyo) ? TextFormaPorqueApoyo.Trim() : "";
                    ObjetoEntrevistaInicial.V6_ANTECEDENTES_PENALES_FAM = ExitenAntecedentespenalesFamiiarSi == true ? "S" : "N";
                    ObjetoEntrevistaInicial.V7_FAM_CONSUME_SUST_TOXICAS = FamiliarConsumeSustanciaSi == true ? "S" : "N";
                    ObjetoEntrevistaInicial.V8_CONSUME_TIPO_DROGA = ConsumidoAlgunTipoDrogaSi == true ? "S" : "N";
                    ObjetoEntrevistaInicial.V9_ANTECEDENTES_PENALES_ANTERI = AntecedentesPernalesSi == true ? "S" : "N";
                    ObjetoEntrevistaInicial.V10_RECIBIO_APOYO_INTERNAMIENT = RecibioApoyoInternamientoSi == true ? "S" : "N";
                    ObjetoEntrevistaInicial.V10_DE_QUIEN = !string.IsNullOrEmpty(TextDeQuienRecibioApoyoInternamiento) ? TextDeQuienRecibioApoyoInternamiento.Trim() : "";
                    ObjetoEntrevistaInicial.V10_FRECUENCIA = !string.IsNullOrEmpty(TextFrecuencia) ? TextFrecuencia.Trim() : "";
                    ObjetoEntrevistaInicial.V1_PADRES_VIVIEN_JUNTOS = ViveConPadres == true ? "S" : "N";
                    ObjetoEntrevistaInicial.V_DESDECUANDO = !string.IsNullOrEmpty(DesdeCuandoVivePadres) ? DesdeCuandoVivePadres.Trim() : "";
                    ObjetoEntrevistaInicial.V_UNIONES_ANTERIORES = TextunionesAnteriores.Trim();
                    ObjetoEntrevistaInicial.V_HIJOS_NUM = NoHijos != null ? (decimal)NoHijos : decimal.Zero;
                    ObjetoEntrevistaInicial.V_PROBLEMAS_PAREJA = ProblemaParejaSi == true ? "S" : "N";
                    ObjetoEntrevistaInicial.V_MIEMBRO_FAMILIA_ABANDONO_HOG = MiembroFamiliaAbandonoFamSi == true ? "S" : "N";

                    #region Situacion Actual
                    ObjetoEntrevistaInicial.VI1_CONOCE_VECINOS = ConoceVecinosSi == true ? "S" : "N";
                    ObjetoEntrevistaInicial.VI2_PROBLEMAS_VECINOS = ProblemasConVecinosSi == true ? "S" : "N";
                    ObjetoEntrevistaInicial.VI3_DEDICA_TIEMPO_LIBRE = TextTiempoLibre.Trim();
                    ObjetoEntrevistaInicial.VI3_OTROS = !string.IsNullOrEmpty(TextTiempoLibreOtro) ? TextTiempoLibreOtro.Trim() : string.Empty;
                    ObjetoEntrevistaInicial.VI4_ACTA_NAC = chkActaNac == true ? "S" : "N";
                    ObjetoEntrevistaInicial.VI4_ACTA_MAT = chkActaMatrimonio == true ? "S" : "N";
                    ObjetoEntrevistaInicial.VI4_COMPROBANTE_DOM = chkComprobanteEstudio == true ? "S" : "N";
                    ObjetoEntrevistaInicial.VI4_LICENCIA = chkLiciencia == true ? "S" : "N";
                    ObjetoEntrevistaInicial.VI4_PASAPORTE_MEX = chkPasaporteMex == true ? "S" : "N";
                    ObjetoEntrevistaInicial.VI4_IFE = chkPasaporteMex == true ? "S" : "N";
                    ObjetoEntrevistaInicial.VI4_VISA_LASER = chkVisaLaser == true ? "S" : "N";
                    ObjetoEntrevistaInicial.VI4_IFE = chkIFE == true ? "S" : "N";
                    ObjetoEntrevistaInicial.VI4_CARTILLA_MILITAR = chkCartilla == true ? "S" : "N";
                    ObjetoEntrevistaInicial.VI4_CURP = chkCurp == true ? "S" : "N";
                    ObjetoEntrevistaInicial.VI4_OTROS = !string.IsNullOrEmpty(TextDocumentosOtro) ? TextDocumentosOtro.Trim() : string.Empty;

                    #region Salud
                    ObjetoEntrevistaInicial.IX_DIAGNOSTICO = textDiagnostico.Trim();
                    ObjetoEntrevistaInicial.VII1_PADECIO_ENFERMEDAD = PadeceEnfermedadSi == true ? "S" : "N";
                    ObjetoEntrevistaInicial.VII1_ESPECIFIQUE = !string.IsNullOrEmpty(TextEspecifiqueOtraEnfermedad) ? TextEspecifiqueOtraEnfermedad.Trim() : "";
                    ObjetoEntrevistaInicial.VII2_TIPO_TRATAMIENTO = !string.IsNullOrEmpty(TextTipoTratamientoRecibido) ? TextTipoTratamientoRecibido.Trim() : "";
                    #endregion
                    #endregion
                    #endregion
                        #endregion
                    //bool OCURRIO_ERROR = false;
                    short i = 1;
                        #region Apoyo economico Guardar Informacion
                        var lstAE = new List<PRS_APOYO_ECONOMICO>();
                        foreach (var pa in ListPersonasApoyo)
                        {
                            lstAE.Add(new PRS_APOYO_ECONOMICO(){
                            ID_CENTRO = SelectedProcesoLibertad.ID_CENTRO,
                            ID_ANIO = SelectedProcesoLibertad.ID_ANIO,
                            ID_IMPUTADO = SelectedProcesoLibertad.ID_IMPUTADO,
                            //ID_FOLIO = SelectedEntrevistaInicial != null 
                            ID_PERS = i,
                            ID_OCUPACION = pa.ID_OCUPACION, 
                            NOMBRE = pa.NOMBRE,
                            APORTACION = pa.APORTACION
                            });
                            i++;
                        }
                        #endregion

                        #region Nucleo Familiar
                        i = 1;
                        var lstNF = new List<PRS_NUCLEO_FAMILIAR>();
                        //Primario
                        foreach (var nf in ListNuceloPrimarioFam)
                        {
                            lstNF.Add(new PRS_NUCLEO_FAMILIAR(){
                                ID_CENTRO = SelectedProcesoLibertad.ID_CENTRO,
                                ID_ANIO = SelectedProcesoLibertad.ID_ANIO,
                                ID_IMPUTADO = SelectedProcesoLibertad.ID_IMPUTADO,
                                //ID_FOLIO
                                ID_TIPO = nf.ID_TIPO,
                                ID_PERS = i,
                                ID_ESTADO_CIVIL = nf.ID_ESTADO_CIVIL,
                                ID_OCUPACION = nf.ID_OCUPACION,
                                ID_ESCOLARIDAD = nf.ID_ESCOLARIDAD,
                                ID_TIPO_REFERENCIA = nf.ID_TIPO_REFERENCIA,
                                NOMBRE = nf.NOMBRE,
                                EDAD = nf.EDAD
                            });
                            i++;
                        }
                        //Secundario
                        i = 1;
                        foreach (var nf in ListSecundarioFam)
                        {
                            lstNF.Add(new PRS_NUCLEO_FAMILIAR(){
                                ID_CENTRO = SelectedProcesoLibertad.ID_CENTRO,
                                ID_ANIO = SelectedProcesoLibertad.ID_ANIO,
                                ID_IMPUTADO = SelectedProcesoLibertad.ID_IMPUTADO,
                                //ID_FOLIO
                                ID_TIPO = nf.ID_TIPO,
                                ID_PERS = i,
                                ID_ESTADO_CIVIL = nf.ID_ESTADO_CIVIL,
                                ID_OCUPACION = nf.ID_OCUPACION,
                                ID_ESCOLARIDAD = nf.ID_ESCOLARIDAD,
                                ID_TIPO_REFERENCIA = nf.ID_TIPO_REFERENCIA,
                                NOMBRE = nf.NOMBRE,
                                EDAD = nf.EDAD
                            });
                            i++;
                        }
                        #endregion
                        
                        #region DROGA
                        i = 1;
                        var lstDroga = new List<PRS_DROGA>();
                        foreach (var d in ListDrogaConsumo)
                        {
                            if (!lstDroga.Any(w => w.ID_DROGA == d.ID_DROGA))
                            {
                                lstDroga.Add(new PRS_DROGA()
                                {
                                    ID_CENTRO = SelectedProcesoLibertad.ID_CENTRO,
                                    ID_ANIO = SelectedProcesoLibertad.ID_ANIO,
                                    ID_IMPUTADO = SelectedProcesoLibertad.ID_IMPUTADO,
                                    //ID_FOLIO
                                    ID_DROGA = d.ID_DROGA,
                                    ID_FRECUENCIA = d.ID_FRECUENCIA,
                                    INICIO_CONSUMO = d.INICIO_CONSUMO
                                });
                            }
                        }
                        #endregion

                    if (SelectedEntrevistaInicial != null)
                    {
                        if (!PEditar)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                            return;
                        }
                        ObjetoEntrevistaInicial.ID_FOLIO = SelectedEntrevistaInicial.ID_FOLIO;
                        if (new cEntrevistaInicialTrabajoSocial().Actualizar(ObjetoEntrevistaInicial,lstAE,lstNF,lstDroga))
                            new Dialogos().ConfirmacionDialogo("Éxito", "La información se guardo correctamente");
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
                        ObjetoEntrevistaInicial.PRS_APOYO_ECONOMICO = lstAE;
                        ObjetoEntrevistaInicial.PRS_NUCLEO_FAMILIAR = lstNF;
                        ObjetoEntrevistaInicial.PRS_DROGA = lstDroga;
                        ObjetoEntrevistaInicial.ID_FOLIO = new cEntrevistaInicialTrabajoSocial().Insertar(ObjetoEntrevistaInicial);
                        if(ObjetoEntrevistaInicial.ID_FOLIO > 0)
                        {
                            SelectedEntrevistaInicial = new cEntrevistaInicialTrabajoSocial().Obtener(
                                ObjetoEntrevistaInicial.ID_CENTRO,
                                ObjetoEntrevistaInicial.ID_ANIO,
                                ObjetoEntrevistaInicial.ID_IMPUTADO,
                                ObjetoEntrevistaInicial.ID_FOLIO);
                              new Dialogos().ConfirmacionDialogo("Éxito", "La información se guardo correctamente");
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Error", "Ocurrio un error al guardar la información");
                    }
                    #region comentado
                    //using (var transaction = new TransactionScope())
                    //{
                    //    try
                    //    {
                    //        if (new cEntrevistaInicialTrabajoSocial().Insertar(ObjetoEntrevistaInicial))
                    //        {
                    //            #region Apoyo economico Guardar Informacion

                    //            foreach (var itemPersonaApoyo in ListPersonasApoyo)
                    //            {

                    //                if (itemPersonaApoyo.ID_FOLIO == null || itemPersonaApoyo.ID_FOLIO == 0)
                    //                {
                    //                    itemPersonaApoyo.ID_FOLIO = ObjetoEntrevistaInicial.ID_FOLIO;
                    //                    itemPersonaApoyo.PRS_ENTREVISTA_INICIAL = ObjetoEntrevistaInicial;
                    //                    if (!new cApoyoEconomico().Insertar(new PRS_APOYO_ECONOMICO()
                    //                    {
                    //                        ID_ANIO = itemPersonaApoyo.ID_ANIO,
                    //                        ID_CENTRO = itemPersonaApoyo.ID_CENTRO,
                    //                        ID_FOLIO = itemPersonaApoyo.ID_FOLIO,
                    //                        ID_IMPUTADO = itemPersonaApoyo.ID_IMPUTADO,
                    //                        ID_OCUPACION = itemPersonaApoyo.ID_OCUPACION,
                    //                        ID_PERS = itemPersonaApoyo.ID_PERS,
                    //                        NOMBRE = itemPersonaApoyo.NOMBRE,
                    //                        OCUPACION = null,
                    //                        PRS_ENTREVISTA_INICIAL = null,
                    //                        APORTACION = itemPersonaApoyo.APORTACION
                    //                    }))
                    //                    {
                    //                        OCURRIO_ERROR = true;
                    //                    }
                    //                }
                    //                else
                    //                {
                    //                    if (!itemPersonaApoyo.Equals(new cApoyoEconomico().Obtener(itemPersonaApoyo.ID_IMPUTADO, itemPersonaApoyo.ID_ANIO, itemPersonaApoyo.ID_CENTRO, itemPersonaApoyo.ID_PERS).FirstOrDefault()))
                    //                    {

                    //                        if (!new cApoyoEconomico().Actualizar(new PRS_APOYO_ECONOMICO()
                    //                        {
                    //                            ID_ANIO = itemPersonaApoyo.ID_ANIO,
                    //                            ID_CENTRO = itemPersonaApoyo.ID_CENTRO,
                    //                            ID_FOLIO = itemPersonaApoyo.ID_FOLIO,
                    //                            ID_IMPUTADO = itemPersonaApoyo.ID_IMPUTADO,
                    //                            ID_OCUPACION = itemPersonaApoyo.ID_OCUPACION,
                    //                            ID_PERS = itemPersonaApoyo.ID_PERS,
                    //                            NOMBRE = itemPersonaApoyo.NOMBRE,
                    //                            OCUPACION = null,
                    //                            PRS_ENTREVISTA_INICIAL = null,
                    //                            APORTACION = itemPersonaApoyo.APORTACION
                    //                        }))
                    //                        {
                    //                            OCURRIO_ERROR = true;
                    //                        }
                    //                    }
                    //                }



                    //            }

                    //            if (OCURRIO_ERROR == false)
                    //            {

                    //                foreach (var itemFrecDrogas in ListDrogaConsumo)
                    //                {
                    //                    if (itemFrecDrogas.ID_FOLIO == null || itemFrecDrogas.ID_FOLIO == 0)
                    //                    {
                    //                        itemFrecDrogas.ID_FOLIO = ObjetoEntrevistaInicial.ID_FOLIO;


                    //                        if (!new cUsoDrogasFrec().Insertar(new PRS_DROGA()
                    //                        {
                    //                            ID_FOLIO = itemFrecDrogas.ID_FOLIO,
                    //                            ID_IMPUTADO = itemFrecDrogas.ID_IMPUTADO,
                    //                            ID_ANIO = itemFrecDrogas.ID_ANIO,
                    //                            ID_CENTRO = itemFrecDrogas.ID_CENTRO,
                    //                            ID_DROGA = itemFrecDrogas.ID_DROGA,
                    //                            ID_FRECUENCIA = itemFrecDrogas.ID_FRECUENCIA,
                    //                            INICIO_CONSUMO = itemFrecDrogas.INICIO_CONSUMO,
                    //                            PRS_ENTREVISTA_INICIAL = null,
                    //                            DROGA_FRECUENCIA = null,
                    //                            DROGA = null
                    //                        }))
                    //                        {
                    //                            OCURRIO_ERROR = false;
                    //                        }

                    //                    }
                    //                    else
                    //                    {

                    //                        // itemFrecDrogas.ID_FOLIO = ObjetoEntrevistaInicial.ID_FOLIO;//RESETEA ID FOLIO

                    //                        if (!new cUsoDrogasFrec().Actualizar(new PRS_DROGA()
                    //                            {
                    //                                ID_FOLIO = itemFrecDrogas.ID_FOLIO,
                    //                                ID_IMPUTADO = itemFrecDrogas.ID_IMPUTADO,
                    //                                ID_ANIO = itemFrecDrogas.ID_ANIO,
                    //                                ID_CENTRO = itemFrecDrogas.ID_CENTRO,
                    //                                ID_DROGA = itemFrecDrogas.ID_DROGA,
                    //                                ID_FRECUENCIA = itemFrecDrogas.ID_FRECUENCIA,
                    //                                INICIO_CONSUMO = itemFrecDrogas.INICIO_CONSUMO,
                    //                                PRS_ENTREVISTA_INICIAL = null,
                    //                                DROGA_FRECUENCIA = null,
                    //                                DROGA = null
                    //                            }))
                    //                        {
                    //                            OCURRIO_ERROR = false;
                    //                        }

                    //                    }
                    //                }
                    //            }
                    //            #endregion

                    //            #region Grupos Familiares Guardar Informacion
                    //            //Se agrega Nuevo registro a Nucleo familiar si no tiene asignado un folio

                    //            //NUCELO FAMILLAIR PRIMARIO

                    //            if (OCURRIO_ERROR == false)
                    //            {
                    //                foreach (var itemGrupoFamPrim in ListNuceloPrimarioFam)
                    //                {

                    //                    //Se asigna un folio cuando se agregan los datos itemGrupoFam.ID_FOLIO=
                    //                    if (itemGrupoFamPrim.ID_FOLIO == null || itemGrupoFamPrim.ID_FOLIO == 0)
                    //                    {
                    //                        itemGrupoFamPrim.ID_FOLIO = ObjetoEntrevistaInicial.ID_FOLIO;
                    //                        if (!new cNuceloFamiliar().Insertar(new PRS_NUCLEO_FAMILIAR()
                    //                        {
                    //                            EDAD = itemGrupoFamPrim.EDAD,
                    //                            ESCOLARIDAD = null,
                    //                            ESTADO_CIVIL = null,
                    //                            ID_ANIO = itemGrupoFamPrim.ID_ANIO,
                    //                            ID_CENTRO = itemGrupoFamPrim.ID_CENTRO,
                    //                            ID_ESCOLARIDAD = itemGrupoFamPrim.ID_ESCOLARIDAD,
                    //                            ID_ESTADO_CIVIL = itemGrupoFamPrim.ID_ESTADO_CIVIL,
                    //                            ID_FOLIO = itemGrupoFamPrim.ID_FOLIO,
                    //                            ID_PERS = itemGrupoFamPrim.ID_PERS,
                    //                            ID_IMPUTADO = itemGrupoFamPrim.ID_IMPUTADO,
                    //                            ID_OCUPACION = itemGrupoFamPrim.ID_OCUPACION,
                    //                            ID_TIPO = itemGrupoFamPrim.ID_TIPO,
                    //                            ID_TIPO_REFERENCIA = itemGrupoFamPrim.ID_TIPO_REFERENCIA,
                    //                            NOMBRE = itemGrupoFamPrim.NOMBRE,
                    //                            OCUPACION = null,
                    //                            PRS_ENTREVISTA_INICIAL = null,
                    //                            TIPO_REFERENCIA = null
                    //                        }))
                    //                        {
                    //                            OCURRIO_ERROR = true;
                    //                        }
                    //                    }
                    //                    else
                    //                    {

                    //                        //EDITAR GRUPO FAMILIAR PRIMARIO

                    //                        //itemGrupoFamPrim.ID_FOLIO = new cNuceloFamiliar().ObtenerSingle(itemGrupoFamPrim.ID_IMPUTADO, itemGrupoFamPrim.ID_ANIO, itemGrupoFamPrim.ID_CENTRO, itemGrupoFamPrim.ID_PERS, itemGrupoFamPrim.ID_TIPO).ID_FOLIO;
                    //                        if (!new cNuceloFamiliar().Actualizar(new PRS_NUCLEO_FAMILIAR()
                    //                        {
                    //                            EDAD = itemGrupoFamPrim.EDAD,
                    //                            ESCOLARIDAD = null,
                    //                            ESTADO_CIVIL = null,
                    //                            ID_ANIO = itemGrupoFamPrim.ID_ANIO,
                    //                            ID_CENTRO = itemGrupoFamPrim.ID_CENTRO,
                    //                            ID_ESCOLARIDAD = itemGrupoFamPrim.ID_ESCOLARIDAD,
                    //                            ID_ESTADO_CIVIL = itemGrupoFamPrim.ID_ESTADO_CIVIL,
                    //                            ID_FOLIO = itemGrupoFamPrim.ID_FOLIO,
                    //                            ID_PERS = itemGrupoFamPrim.ID_PERS,
                    //                            ID_IMPUTADO = itemGrupoFamPrim.ID_IMPUTADO,
                    //                            ID_OCUPACION = itemGrupoFamPrim.ID_OCUPACION,
                    //                            ID_TIPO = itemGrupoFamPrim.ID_TIPO,

                    //                            ID_TIPO_REFERENCIA = itemGrupoFamPrim.ID_TIPO_REFERENCIA,
                    //                            NOMBRE = itemGrupoFamPrim.NOMBRE,
                    //                            OCUPACION = null,
                    //                            PRS_ENTREVISTA_INICIAL = null,
                    //                            TIPO_REFERENCIA = null
                    //                        }))
                    //                        {
                    //                            OCURRIO_ERROR = true;
                    //                        }

                    //                    }


                    //                }

                    //                //NUCELO FAMILLAIR SECUNDARIO
                    //                foreach (var itemGrupoFamSec in ListSecundarioFam)
                    //                {
                    //                    //Se asigna un folio cuando se agregan los datos itemGrupoFam.ID_FOLIO=
                    //                    if (itemGrupoFamSec.ID_FOLIO == null || itemGrupoFamSec.ID_FOLIO == 0)
                    //                    {
                    //                        itemGrupoFamSec.ID_FOLIO = ObjetoEntrevistaInicial.ID_FOLIO;
                    //                        if (!new cNuceloFamiliar().Insertar(new PRS_NUCLEO_FAMILIAR()
                    //                        {
                    //                            EDAD = itemGrupoFamSec.EDAD,
                    //                            ESCOLARIDAD = null,
                    //                            ESTADO_CIVIL = null,
                    //                            ID_ANIO = itemGrupoFamSec.ID_ANIO,
                    //                            ID_CENTRO = itemGrupoFamSec.ID_CENTRO,
                    //                            ID_ESCOLARIDAD = itemGrupoFamSec.ID_ESCOLARIDAD,
                    //                            ID_ESTADO_CIVIL = itemGrupoFamSec.ID_ESTADO_CIVIL,
                    //                            ID_FOLIO = itemGrupoFamSec.ID_FOLIO,
                    //                            ID_PERS = itemGrupoFamSec.ID_PERS,
                    //                            ID_IMPUTADO = itemGrupoFamSec.ID_IMPUTADO,
                    //                            ID_OCUPACION = itemGrupoFamSec.ID_OCUPACION,
                    //                            ID_TIPO = itemGrupoFamSec.ID_TIPO,
                    //                            ID_TIPO_REFERENCIA = itemGrupoFamSec.ID_TIPO_REFERENCIA,
                    //                            NOMBRE = itemGrupoFamSec.NOMBRE,
                    //                            OCUPACION = null,
                    //                            PRS_ENTREVISTA_INICIAL = null,
                    //                            TIPO_REFERENCIA = null
                    //                        }))
                    //                        {
                    //                            OCURRIO_ERROR = true;
                    //                        }
                    //                    }
                    //                    else
                    //                    {

                    //                        //EDITAR GRUPO FAMILIAR SECUNDARIO 

                    //                        //itemGrupoFamSec.ID_FOLIO = new cNuceloFamiliar().ObtenerSingle(itemGrupoFamSec.ID_IMPUTADO, itemGrupoFamSec.ID_ANIO, itemGrupoFamSec.ID_CENTRO, itemGrupoFamSec.ID_PERS, itemGrupoFamSec.ID_TIPO).ID_FOLIO;
                    //                        if (!new cNuceloFamiliar().Actualizar(new PRS_NUCLEO_FAMILIAR()
                    //                        {
                    //                            EDAD = itemGrupoFamSec.EDAD,
                    //                            ESCOLARIDAD = null,
                    //                            ESTADO_CIVIL = null,
                    //                            ID_ANIO = itemGrupoFamSec.ID_ANIO,
                    //                            ID_CENTRO = itemGrupoFamSec.ID_CENTRO,
                    //                            ID_ESCOLARIDAD = itemGrupoFamSec.ID_ESCOLARIDAD,
                    //                            ID_ESTADO_CIVIL = itemGrupoFamSec.ID_ESTADO_CIVIL,
                    //                            ID_FOLIO = itemGrupoFamSec.ID_FOLIO,
                    //                            ID_PERS = itemGrupoFamSec.ID_PERS,
                    //                            ID_IMPUTADO = itemGrupoFamSec.ID_IMPUTADO,
                    //                            ID_OCUPACION = itemGrupoFamSec.ID_OCUPACION,
                    //                            ID_TIPO = itemGrupoFamSec.ID_TIPO,
                    //                            ID_TIPO_REFERENCIA = itemGrupoFamSec.ID_TIPO_REFERENCIA,
                    //                            NOMBRE = itemGrupoFamSec.NOMBRE,
                    //                            OCUPACION = null,
                    //                            PRS_ENTREVISTA_INICIAL = null,
                    //                            TIPO_REFERENCIA = null
                    //                        }))
                    //                        {
                    //                            OCURRIO_ERROR = true;
                    //                        }


                    //                    }

                    //                }
                    //            }

                    //            #endregion
                    //        }
                    //        else//FIN (INSERT) ENTREVISTA INICIAL
                    //        {
                    //            OCURRIO_ERROR = false;
                    //        }
                    //        if (OCURRIO_ERROR)
                    //        {
                    //            (new Dialogos()).ConfirmacionDialogo("Validación", "Ocurrio Un Error al Guardar La Información.");
                    //        }
                    //        else
                    //        {
                    //            transaction.Complete();
                    //            transaction.Dispose();
                    //            //MenuGuardarEnabled = false;
                    //            StaticSourcesViewModel.SourceChanged = false;
                    //            new Dialogos().ConfirmacionDialogo("Éxito", "La información se registro correctamente");
                    //        }
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        transaction.Dispose();
                    //        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar", ex);
                    //    }

                    //}
                //}
                #endregion
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar", ex);

            }
        }

        //private bool ValidacionApoyoEconomico_DatosIguales(ApoyoFamiliarClase objApoyoconomico)
        //{
        //    if (SelectPersonasApoyo.NOMBRE.Trim() == objApoyoconomico.NOMBRE.Trim() && SelectPersonasApoyo.OCUPACION.Trim() == objApoyoconomico.OCUPACION.Trim())
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        private void NuevoApoyoEconomico()
        {



        }

        private void GuardarEstructuraVivienda()
        {


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
                            //case "FolioBuscar":
                            //    if (!string.IsNullOrWhiteSpace(textbox.Text))
                            //        FolioBuscar = Convert.ToInt32(textbox.Text);
                            //    else
                            //        FolioBuscar = null;
                            //    break;
                            //case "AnioBuscar":
                            //    if (!string.IsNullOrWhiteSpace(textbox.Text))
                            //        AnioBuscar = int.Parse(textbox.Text);
                            //    else
                            //        AnioBuscar = null;

                            //    break;
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
        
        private async void TrabajoSocialLoad(TrabajoSocialView obj = null)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(CargarListas);

                #region ObtenerTamañoMaximoView
                //WidhtTotalTrabajoSocialView = ((Label)obj.FindName("lblFolio")).ActualWidth - 50;
                #endregion

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar decomisos", ex);
            }
        }

        private async void EntrevistaInicialLoading(EntrevistaInicial obj = null)
        {
            try
            {

                #region ObtenerTamañoMaximoView
                WidhtEntrevistainicial = ((TextBlock)obj.FindName("lblDelitoImputa")).ActualWidth - 50;
                #endregion

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar decomisos", ex);
            }
        }

        private async void EstudioSocioEconomicoLoading(EstudioSocioEconomicoTSocial obj = null)
        {
            try
            {

                #region ObtenerTamañoMaximoView
                WidhtEstudioSocioEconomico = ((Label)obj.FindName("lblCondicionesZona")).ActualWidth - 50;



                #endregion

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar decomisos", ex);
            }
        }
        
        private async void EstrucruraDinamicaFamiliarLoading(EstructuraDinamicaFamiliarView obj = null)
        {
            try
            {

                #region ObtenerTamañoMaximoView
                WidhtEstructuraDinamicaFamiliar = ((Label)obj.FindName("lblDesdeCuando")).ActualWidth - 50;
                #endregion

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar decomisos", ex);
            }
        }

        private async void SituacionActualLoading(SituacionActualView obj = null)
        {
            try
            {

                #region ObtenerTamañoMaximoView
                WidhtSituacionActual = ((Label)obj.FindName("lblDedicaTiempoLibre")).ActualWidth - 50;
                #endregion

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
                LstEstadoCivil = new ObservableCollection<ESTADO_CIVIL>(new cEstadoCivil().ObtenerTodos().OrderBy(o => o.DESCR));
                ListOcupacion = new ObservableCollection<OCUPACION>(OcupacionControlador.ObtenerTodos());
                ListReligion = new ObservableCollection<RELIGION>(new cReligion().ObtenerTodos().OrderBy(o => o.DESCR));
                ListEscolaridad = new ObservableCollection<ESCOLARIDAD>(new cEscolaridad().ObtenerTodos().OrderBy(o => o.DESCR));
                LstIdioma = new ObservableCollection<IDIOMA>(new cIdioma().ObtenerTodos().OrderBy(o => o.DESCR));
                // ListaEstructuraVivienda = new ObservableCollection<EstructuraViviendaClass>(ObtenerEstructuraVivienda());
                ListPersonasApoyo = new ObservableCollection<PRS_APOYO_ECONOMICO>();
                ListParentesco = new ObservableCollection<TIPO_REFERENCIA>(ParentezcoControlador.ObtenerTodos());
                LstGrupoEtnico = new ObservableCollection<ETNIA>(new cEtnia().ObtenerTodos());
                #region Datos Nucleo familiar Popoup
                ListOcupacionNucleoFamiliar = new ObservableCollection<OCUPACION>(OcupacionControlador.ObtenerTodos().OrderBy(o => o.DESCR));
                ListParentescoNucleoFamiliar = new ObservableCollection<TIPO_REFERENCIA>(ParentezcoControlador.ObtenerTodos().OrderBy(o => o.DESCR));
                ListEscolaridadNuceloFamiliar = new ObservableCollection<ESCOLARIDAD>(new cEscolaridad().ObtenerTodos().OrderBy(o => o.DESCR));
                ListEstadoCivilNuceloFamiliar = new ObservableCollection<ESTADO_CIVIL>(new cEstadoCivil().ObtenerTodos().OrderBy(o => o.DESCR));
                #endregion


                //Popup  Personas que apoyan econocmicamente

                ListOcupacionApoyo = new ObservableCollection<OCUPACION>(new cOcupacion().ObtenerTodos().OrderBy(o => o.DESCR));
                ListOcupacionApoyoEconomic = new ObservableCollection<OCUPACION>(new cOcupacion().ObtenerTodos().OrderBy(o => o.DESCR));
                ListOcupacionesApoyo = new ObservableCollection<OCUPACION>(new cOcupacion().ObtenerTodos());

                LstDrogas = new ObservableCollection<DROGA>(new cDrogas().ObtenerTodos());
                LstFrecuenciasUsoDrogas = new ObservableCollection<DROGA_FRECUENCIA>(new cDrogaFrecuencia().Obtener());


                System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                {

                    LstEstadoCivil.Insert(0, new ESTADO_CIVIL() { ID_ESTADO_CIVIL = -1, DESCR = "SELECCIONE" });
                    OnPropertyChanged("LstEstadoCivil");
                    ListOcupacion.Insert(0, new OCUPACION() { ID_OCUPACION = -1, DESCR = "SELECCIONE" });
                    OnPropertyChanged("ListOcupacion");
                    ListEscolaridad.Insert(0, new ESCOLARIDAD() { ID_ESCOLARIDAD = -1, DESCR = "SELECCIONE" });
                    OnPropertyChanged("ListEscolaridad");
                    ListReligion.Insert(0, new RELIGION() { ID_RELIGION = -1, DESCR = "SELECCIONE" });
                    OnPropertyChanged("ListReligion");
                    LstGrupoEtnico.Insert(0, new ETNIA() { ID_ETNIA = -1, DESCR = "SELECCIONE" });
                    OnPropertyChanged("LstGrupoEtnico");
                    LstIdioma.Insert(0, new IDIOMA() { ID_IDIOMA = -1, DESCR = "SELECCIONE" });
                    OnPropertyChanged("LstIdioma");
                    ListParentesco.Insert(0, new TIPO_REFERENCIA() { ID_TIPO_REFERENCIA = -1, DESCR = "SELECCIONE" });
                    OnPropertyChanged("ListParentesco");
                    ListParentescoNucleoFamiliar.Insert(0, new TIPO_REFERENCIA() { ID_TIPO_REFERENCIA = -1, DESCR = "SELECCIONE" });
                    OnPropertyChanged("ListParentescoNucleoFamiliar");
                    ListParentesco.Insert(0, new TIPO_REFERENCIA() { ID_TIPO_REFERENCIA = -1, DESCR = "SELECCIONE" });
                    OnPropertyChanged("ListParentesco");
                    ListOcupacionesApoyo.Insert(0, new OCUPACION { ID_OCUPACION = -1, DESCR = "SELECCIONE" });
                    OnPropertyChanged("ListOcupacionesApoyo");
                    ListEscolaridadNuceloFamiliar.Insert(0, new ESCOLARIDAD() { ID_ESCOLARIDAD = -1, DESCR = "SELECCIONE" });
                    OnPropertyChanged("ListEscolaridadNuceloFamiliar");
                    ListOcupacionNucleoFamiliar.Insert(0, new OCUPACION() { ID_OCUPACION = -1, DESCR = "SELECCIONE" });
                    OnPropertyChanged("ListOcupacionNucleoFamiliar");
                    ListEstadoCivilNuceloFamiliar.Insert(0, new ESTADO_CIVIL() { ID_ESTADO_CIVIL = -1, DESCR = "SELECCIONE" });
                    OnPropertyChanged("ListEstadoCivilNuceloFamiliar");
                    ListOcupacionApoyoEconomic.Insert(0, new OCUPACION() { ID_OCUPACION = -1, DESCR = "SELECCIONE" });
                    OnPropertyChanged("ListOcupacionApoyoEconomic");

                    LstDrogas.Insert(0, new DROGA { ID_DROGA = -1, DESCR = "SELECCIONE" });
                    OnPropertyChanged("LstDrogas");
                    LstFrecuenciasUsoDrogas.Insert(0, new DROGA_FRECUENCIA { ID_FRECUENCIA = -1, DESCR = "SELECCIONE" });
                    OnPropertyChanged("LstFrecuenciasUsoDrogas");
                    ConfiguraPermisos();
                    //pendiente   ConfiguraPermisos();
                    StaticSourcesViewModel.SourceChanged = false;
                }));
                AsignarMaxLenght();
                ConfiguraPermisos();


                //  ValidacionesLiberado();



            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar buscar por nuc", ex);
            }
        }

        private void AsignarMaxLenght()
        {
            LugarMax = 100;
            NucMax = 50;
            CausaPenalMax = 50;
            DelitoImputaMax = 100;
            ObservacioneEstructuraFamMax = 500;
            CondicionesZonaMax = 500;
            SituacionEconMax = 500;
            DesdeCuandoVivenJuntosMax = 100;
            DescrDinamicaFamiliarMax = 500;
            DeQuienDuranteInternamientoMax = 100;
            FrecuenciaRecibApoyoInternmMax = 100;
            TiempoLibreMax = 100;
            OtroTiempoLibrMax = 100;
            OtroDocumMax = 100;
            EpecifiqueEnfermedadMax = 100;
            TipoTratamientoRecibidoMax = 100;
            DiagnosticoMax = 100;
            NombreApoyoMax = 500;
            TiempoConocerleMax = 500;
        }

        #region Metodos Grupo familiar
        private void ObtenerGrupofamiliar()
        {
            ListNuceloPrimarioFam = new ObservableCollection<PRS_NUCLEO_FAMILIAR>(new cNuceloFamiliar().Obtener((short)SelectExpediente.ID_IMPUTADO, (short)SelectExpediente.ID_ANIO, (short)SelectExpediente.ID_CENTRO, 0, "P"));
            ListSecundarioFam = new ObservableCollection<PRS_NUCLEO_FAMILIAR>(new cNuceloFamiliar().Obtener((short)SelectExpediente.ID_IMPUTADO, (short)SelectExpediente.ID_ANIO, (short)SelectExpediente.ID_CENTRO, 0, "S"));

        }



        public void NucloFamiliar_Primario_Insert_Edit()
        {
            if (SelectNuceloPrimarioFam != null)
            {

                var ObjNucleoFam = new PRS_NUCLEO_FAMILIAR()
                {
                    NOMBRE = TextNombreNuceloFamiliar,
                    EDAD = (short)TextEdadNuceloFamiliar,
                    //EDAD = short.Parse(TextEdadNuceloFamiliar.ToString()),
                    ID_ESCOLARIDAD = SelectEscolaridadNuceloFamiliar,
                    ESCOLARIDAD = EscoladridadControlador.Obtener((int)SelectEscolaridadNuceloFamiliar).FirstOrDefault(),
                    ID_ESTADO_CIVIL = SelectEstadoCivilNuceloFamiliar,
                    ESTADO_CIVIL = EstadoCivilControlador.Obtener((int)SelectEstadoCivilNuceloFamiliar).FirstOrDefault(),
                    ID_IMPUTADO = selectExpediente.ID_IMPUTADO,
                    ID_ANIO = selectExpediente.ID_ANIO,
                    ID_CENTRO = selectExpediente.ID_CENTRO,
                    ID_TIPO = Tipo_Nucelo_Familiar,
                    ID_TIPO_REFERENCIA = SelectedParentescoNuceloFamiliar.ID_TIPO_REFERENCIA,
                    TIPO_REFERENCIA = TipoRef.Obtener(SelectedParentescoNuceloFamiliar.ID_TIPO_REFERENCIA).FirstOrDefault(),
                    //ID_FOLIO  Se agregan cuando se Guardan los datos
                    //ID_PERS   Se agregaan cuando se Gaurdan los datos
                    ID_OCUPACION = SelectOcupacionNuceloFamiliar,
                    OCUPACION = OcupacionControlador.Obtener((int)SelectOcupacionNuceloFamiliar).FirstOrDefault()
                };

                // <--------Actaliza Datos Objetos ------------------->
                short Folio = 0;
                short FolioEdit = -1;

                foreach (var itemNuceloFamiliarPrim in ListNuceloPrimarioFam.Select(s => new { obj = s }).Where(w => w.obj.Equals(SelectNuceloPrimarioFam)))
                {

                    itemNuceloFamiliarPrim.obj.NOMBRE = ObjNucleoFam.NOMBRE;
                    itemNuceloFamiliarPrim.obj.EDAD = ObjNucleoFam.EDAD;
                    itemNuceloFamiliarPrim.obj.ID_ESCOLARIDAD = ObjNucleoFam.ID_ESCOLARIDAD;
                    itemNuceloFamiliarPrim.obj.ESCOLARIDAD = ObjNucleoFam.ESCOLARIDAD;
                    itemNuceloFamiliarPrim.obj.ID_ESTADO_CIVIL = ObjNucleoFam.ID_ESTADO_CIVIL;
                    itemNuceloFamiliarPrim.obj.ESTADO_CIVIL = ObjNucleoFam.ESTADO_CIVIL;
                    itemNuceloFamiliarPrim.obj.ID_TIPO_REFERENCIA = ObjNucleoFam.ID_TIPO_REFERENCIA;
                    itemNuceloFamiliarPrim.obj.TIPO_REFERENCIA = ObjNucleoFam.TIPO_REFERENCIA;
                    itemNuceloFamiliarPrim.obj.ID_OCUPACION = ObjNucleoFam.ID_OCUPACION;
                    itemNuceloFamiliarPrim.obj.OCUPACION = ObjNucleoFam.OCUPACION;
                }
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_NUCELO_FAMILIAR);
            }
            else
            {

                var ObjNucleoFam = new PRS_NUCLEO_FAMILIAR()
                {
                    NOMBRE = TextNombreNuceloFamiliar,
                    EDAD = short.Parse(TextEdadNuceloFamiliar.ToString()),
                    ID_ESCOLARIDAD = SelectEscolaridadNuceloFamiliar,
                    ESCOLARIDAD = EscoladridadControlador.Obtener((int)SelectEscolaridadNuceloFamiliar).FirstOrDefault(),
                    ID_ESTADO_CIVIL = SelectEstadoCivilNuceloFamiliar,
                    ESTADO_CIVIL = EstadoCivilControlador.Obtener((int)SelectEstadoCivilNuceloFamiliar).FirstOrDefault(),
                    ID_IMPUTADO = selectExpediente.ID_IMPUTADO,
                    ID_ANIO = selectExpediente.ID_ANIO,
                    ID_CENTRO = selectExpediente.ID_CENTRO,
                    ID_TIPO = Tipo_Nucelo_Familiar,
                    ID_TIPO_REFERENCIA = SelectedParentescoNuceloFamiliar.ID_TIPO_REFERENCIA,
                    TIPO_REFERENCIA = TipoRef.Obtener(SelectedParentescoNuceloFamiliar.ID_TIPO_REFERENCIA).FirstOrDefault(),
                    //ID_FOLIO  Se agregan cuando se Guardan los datos
                    //ID_PERS   Se agregaan cuando se Gaurdan los datos
                    ID_OCUPACION = SelectOcupacionNuceloFamiliar,
                    OCUPACION = OcupacionControlador.Obtener((int)SelectOcupacionNuceloFamiliar).FirstOrDefault()
                };
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_NUCELO_FAMILIAR);
                ListNuceloPrimarioFam.Add(ObjNucleoFam);

            }
            ListNuceloPrimarioFam = new ObservableCollection<PRS_NUCLEO_FAMILIAR>(ListNuceloPrimarioFam);
            OnPropertyChanged("ListNuceloPrimarioFam");
        }

        public void NucloFamiliar_Secundario_Insert_Edit()
        {
            if (SelectSecundarioFam != null)
            {

                var ObjNucleoFam = new PRS_NUCLEO_FAMILIAR()
                {
                    NOMBRE = TextNombreNuceloFamiliar,
                    EDAD = (short)TextEdadNuceloFamiliar,
                    //EDAD = short.Parse(TextEdadNuceloFamiliar.ToString()),
                    ID_ESCOLARIDAD = SelectEscolaridadNuceloFamiliar,
                    ESCOLARIDAD = EscoladridadControlador.Obtener((int)SelectEscolaridadNuceloFamiliar).FirstOrDefault(),
                    ID_ESTADO_CIVIL = SelectEstadoCivilNuceloFamiliar,
                    ESTADO_CIVIL = EstadoCivilControlador.Obtener((int)SelectEstadoCivilNuceloFamiliar).FirstOrDefault(),
                    ID_IMPUTADO = selectExpediente.ID_IMPUTADO,
                    ID_ANIO = selectExpediente.ID_ANIO,
                    ID_CENTRO = selectExpediente.ID_CENTRO,
                    ID_TIPO = Tipo_Nucelo_Familiar,
                    ID_TIPO_REFERENCIA = SelectedParentescoNuceloFamiliar.ID_TIPO_REFERENCIA,
                    TIPO_REFERENCIA = TipoRef.Obtener(SelectedParentescoNuceloFamiliar.ID_TIPO_REFERENCIA).FirstOrDefault(),
                    //ID_FOLIO  Se agregan cuando se Guardan los datos
                    //ID_PERS   Se agregaan cuando se Gaurdan los datos
                    ID_OCUPACION = SelectOcupacionNuceloFamiliar,
                    OCUPACION = OcupacionControlador.Obtener((int)SelectOcupacionNuceloFamiliar).FirstOrDefault()
                };

                // <--------Actaliza Datos Objetos ------------------->
                short Folio = 0;
                short FolioEdit = -1;
                foreach (var itemNuceloFamiliarSec in ListSecundarioFam.Select(s => new { obj = s }).Where(w => w.obj.Equals(SelectSecundarioFam)))
                {

                    itemNuceloFamiliarSec.obj.NOMBRE = ObjNucleoFam.NOMBRE;
                    itemNuceloFamiliarSec.obj.EDAD = ObjNucleoFam.EDAD;
                    itemNuceloFamiliarSec.obj.ID_ESCOLARIDAD = ObjNucleoFam.ID_ESCOLARIDAD;
                    itemNuceloFamiliarSec.obj.ESCOLARIDAD = ObjNucleoFam.ESCOLARIDAD;
                    itemNuceloFamiliarSec.obj.ID_ESTADO_CIVIL = ObjNucleoFam.ID_ESTADO_CIVIL;
                    itemNuceloFamiliarSec.obj.ESTADO_CIVIL = ObjNucleoFam.ESTADO_CIVIL;
                    itemNuceloFamiliarSec.obj.ID_TIPO_REFERENCIA = ObjNucleoFam.ID_TIPO_REFERENCIA;
                    itemNuceloFamiliarSec.obj.TIPO_REFERENCIA = ObjNucleoFam.TIPO_REFERENCIA;
                    itemNuceloFamiliarSec.obj.ID_OCUPACION = ObjNucleoFam.ID_OCUPACION;
                    itemNuceloFamiliarSec.obj.OCUPACION = ObjNucleoFam.OCUPACION;
                }
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_NUCELO_FAMILIAR);
            }
            else
            {

                var ObjNucleoFam = new PRS_NUCLEO_FAMILIAR()
                {
                    NOMBRE = TextNombreNuceloFamiliar,
                    EDAD = short.Parse(TextEdadNuceloFamiliar.ToString()),
                    ID_ESCOLARIDAD = SelectEscolaridadNuceloFamiliar,
                    ESCOLARIDAD = EscoladridadControlador.Obtener((int)SelectEscolaridadNuceloFamiliar).FirstOrDefault(),
                    ID_ESTADO_CIVIL = SelectEstadoCivilNuceloFamiliar,
                    ESTADO_CIVIL = EstadoCivilControlador.Obtener((int)SelectEstadoCivilNuceloFamiliar).FirstOrDefault(),
                    ID_IMPUTADO = selectExpediente.ID_IMPUTADO,
                    ID_ANIO = selectExpediente.ID_ANIO,
                    ID_CENTRO = selectExpediente.ID_CENTRO,
                    ID_TIPO = Tipo_Nucelo_Familiar,
                    ID_TIPO_REFERENCIA = SelectedParentescoNuceloFamiliar.ID_TIPO_REFERENCIA,
                    TIPO_REFERENCIA = TipoRef.Obtener(SelectedParentescoNuceloFamiliar.ID_TIPO_REFERENCIA).FirstOrDefault(),
                    //ID_FOLIO  Se agregan cuando se Guardan los datos
                    //ID_PERS   Se agregaan cuando se Gaurdan los datos
                    ID_OCUPACION = SelectOcupacionNuceloFamiliar,
                    OCUPACION = OcupacionControlador.Obtener((int)SelectOcupacionNuceloFamiliar).FirstOrDefault()
                };
                ListSecundarioFam.Add(ObjNucleoFam);
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_NUCELO_FAMILIAR);

            }
            ListSecundarioFam = new ObservableCollection<PRS_NUCLEO_FAMILIAR>(ListSecundarioFam);
            OnPropertyChanged("ListSecundarioFam");
            LimpiarGrupoFamiliar();

        }
        private bool ValidarCamposNuceloFamiliar()
        {
            if (!string.IsNullOrEmpty(TextNombreNuceloFamiliar) && SelectOcupacionNuceloFamiliar > -1 && TextEdadNuceloFamiliar != null && SelectedParentescoNuceloFamiliar != null && SelectEscolaridadNuceloFamiliar > -1 && SelectEstadoCivilNuceloFamiliar > -1)
            {
                return true;
            }
            return false;
        }

        private void EliminarGrupoFamiliarPrimario()
        {
            if (SelectNuceloPrimarioFam.ID_FOLIO > 0)
            {
                (new Dialogos()).ConfirmacionDialogo("Validación", "No es Posible eliminar este registro ya que fue Guardado Anteriormente.");
            }
            else
            {
                ListNuceloPrimarioFam.Remove(SelectNuceloPrimarioFam);
                (new Dialogos()).ConfirmacionDialogo("Validación", "Registro fue eliminado con exito.");
            }

        }

        private void EliminarGrupoFamiliarSecundario()
        {
            if (SelectSecundarioFam.ID_FOLIO > 0)
            {
                (new Dialogos()).ConfirmacionDialogo("Validación", "No es Posible eliminar este registro ya que fue Guardado Anteriormente.");
            }
            else
            {
                ListSecundarioFam.Remove(SelectSecundarioFam);
                (new Dialogos()).ConfirmacionDialogo("Validación", "Registro fue eliminado con exito.");
            }

        }
        private void LimpiarGrupoFamiliar()
        {
            TextNombreNuceloFamiliar = string.Empty;
            TextEdadNuceloFamiliar = null;
            SelectEscolaridadNuceloFamiliar = -1;
            SelectEstadoCivilNuceloFamiliar = -1;
            SelectParentescoNuceloFamiliar = -1;
            SelectOcupacionNuceloFamiliar = -1;
        }
        #endregion

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

        //private List<EstructuraViviendaClass> ObtenerEstructuraVivienda()
        //{

        //    return new List<EstructuraViviendaClass>{new EstructuraViviendaClass("Comedor",null,""),
        //                                             new EstructuraViviendaClass("Recámaras",null,""),
        //                                             new EstructuraViviendaClass("Sala",null,""),
        //                                             new EstructuraViviendaClass("Cocina",null,""),
        //                                             new EstructuraViviendaClass("Baño",null,""),
        //                                             new EstructuraViviendaClass("Ventanas",null,""),
        //                                             new EstructuraViviendaClass("Patios",null,"")};
        //}
        
        public void ObtenerApoyoEconomico()
        {
            //var OcupacionControlador = new cOcupacion();
            //if (ListPersonasApoyo.Count == 0 || ListPersonasApoyo == null)
            //{
            //    var List = new List<ApoyoFamiliarClase>();

            //    //<-----Obtiene Apoyos economicos por Imputado --->
            //    short Id = short.Parse(SelectExpediente.ID_IMPUTADO.ToString());
            //    foreach (var item in new SSP.Controlador.Catalogo.Justicia.Liberados.cApoyoEconomico().ObtenerTodosImputado(Id).Select(s => new { s.ID_PERS, s.NOMBRE, s.ID_OCUPACION, s.APORTACION }))
            //    {
            //        var obj = new ApoyoFamiliarClase();
            //        obj.id_persona = item.ID_PERS;
            //        obj.NOMBRE = item.NOMBRE;
            //        obj.APORTACION = item.APORTACION;
            //        obj.OCUPACION = OcupacionControlador.Obtener(int.Parse(item.ID_OCUPACION.ToString())).FirstOrDefault().DESCR;
            //        obj.id_ocupacion = int.Parse(item.ID_OCUPACION.ToString());
            //        List.Add(obj);
            //    }

            ListPersonasApoyo = new ObservableCollection<PRS_APOYO_ECONOMICO>(new cApoyoEconomico().ObtenerTodos((short)SelectExpediente.ID_IMPUTADO, SelectExpediente.ID_ANIO, SelectExpediente.ID_CENTRO));
            //}

            //if (!string.IsNullOrEmpty(objApoyo.NOMBRE))
            //{
            //    ///ListPersonasApoyo.Add(objApoyo);
            //    OnPropertyChanged("ListPersonasApoyo");
            //}
        }

        private void LimpiarApoyoEconomico()
        {

            TextNombreApoyo = string.Empty;
            TextAportaciones = string.Empty;
            SelectOcupacionApoyo = -1;
        }
        
        //     public int id_persona { get; set; }
        //public string NOMBRE { get; set; }
        //public string OCUPACION { get; set; }
        //public int APORTACION { get; set; }
        //}

        private void Obtener()
        {
            try
            {
                TabsEnabled = true;
                //SelectMJ != null && 
                if (SelectExpediente != null)
                {

                    MenuGuardarEnabled = true;
                    #region Populas los Grid que contiene Popup
                    ObtenerApoyoEconomico();
                    ObtenerGrupofamiliar();
                    ObtenerConsumoDroga();
                    #endregion


                    RelacionesPersonalesVisible = Visibility.Hidden;
                    TabControlVisible = Visibility.Visible;
                    NUCBuscar = SelectedProcesoLibertad.NUC;
                    NombreBuscar = SelectExpediente.NOMBRE;
                    AnioBuscar = SelectExpediente.ID_ANIO;
                    FolioBuscar = SelectExpediente.ID_IMPUTADO;
                    ApellidoPaternoBuscar = SelectExpediente.PATERNO;
                    ApellidoMaternoBuscar = SelectExpediente.MATERNO;
                    SelectSexo = SelectExpediente.SEXO;

                    SelectEstadoCivil = SelectedProcesoLibertad.ESTADO_CIVIL != null ? SelectedProcesoLibertad.ID_ESTADO_CIVIL : -1;
                    SelectOcupacion = SelectedProcesoLibertad.OCUPACION != null ? SelectedProcesoLibertad.ID_OCUPACION : -1;
                    TextFechaNacimiento = SelectExpediente.NACIMIENTO_FECHA;
                    ETelefonoFijo = SelectedProcesoLibertad.TELEFONO != null ? SelectedProcesoLibertad.TELEFONO.ToString() : "";

                    //TextCalle = SelectExpediente.DOMICILIO_CALLE != null ? SelectExpediente.DOMICILIO_CALLE.Trim() : "";
                    //TextNumeroExterior = SelectExpediente.DOMICILIO_NUM_EXT;
                    //TextNumeroInterior = !string.IsNullOrEmpty(SelectExpediente.DOMICILIO_NUM_INT) ? int.Parse(SelectExpediente.DOMICILIO_NUM_INT) : 0;
                    TextCalle = SelectedProcesoLibertad.DOMICILIO_CALLE != null ? SelectedProcesoLibertad.DOMICILIO_CALLE.Trim() : "";
                    TextNumeroExterior = SelectedProcesoLibertad.DOMICILIO_NUM_EXT;
                    TextNumeroInterior = !string.IsNullOrEmpty(SelectedProcesoLibertad.DOMICILIO_NUM_INT) ? int.Parse(SelectedProcesoLibertad.DOMICILIO_NUM_INT) : 0;
                    SelectedIdioma = selectExpediente.ID_IDIOMA;
                    //SelectReligion = selectExpediente.ID_RELIGION;
                    SelectReligion = SelectedProcesoLibertad.RELIGION != null ? SelectedProcesoLibertad.ID_RELIGION : -1;
                    SelectGrupoEtnico = selectExpediente.ID_ETNIA;
                    //SelectEscolaridad = selectExpediente.ID_ESCOLARIDAD;
                    SelectEscolaridad = SelectedProcesoLibertad.ESCOLARIDAD != null ? SelectedProcesoLibertad.ID_ESCOLARIDAD : -1;
                    ListAlias = new ObservableCollection<ALIAS>(SelectExpediente.ALIAS);
                    ListApodo = new ObservableCollection<APODO>(SelectExpediente.APODO);
                    textLugarNacimientoExtranjero = SelectExpediente.NACIMIENTO_LUGAR;
                    //AniosEstado = SelectExpediente.RESIDENCIA_ANIOS != null ? SelectExpediente.RESIDENCIA_ANIOS.Value.ToString() : "0";
                    AniosEstado = SelectedProcesoLibertad.RESIDENCIA_ANIOS != null ? SelectedProcesoLibertad.RESIDENCIA_ANIOS.Value.ToString() : "0";
                    //MesesEstado = SelectExpediente.RESIDENCIA_MESES != null ? SelectExpediente.RESIDENCIA_MESES.Value.ToString() : "0";
                    MesesEstado = SelectedProcesoLibertad.RESIDENCIA_MESES != null ? SelectedProcesoLibertad.RESIDENCIA_MESES.Value.ToString() : "0";
                    //TextDelitoImputa = SelectMJ == null ? "" : SelectMJ.DELITOS;
                    TextTiempoRdicaEstado = string.Empty;
                    #region PersonaApoyo
                    //TextNombreApoyo = string.Empty;
                    //TextEdadApoyo = 0;
                    ////SelectOcupacion = ;
                    //TextNumeroExteriorApoyo = 0;
                    //TextNumeroInteriorApoyo = 0;
                    //TextTelefonoApoyo = string.Empty;
                    //TextTelefonoApoyo = string.Empty;
                    //SelectParentesco = 0;
                    //TextTiempoConocerceApoyo = string.Empty;


                    #endregion

                    #region Estudio Socio econocmio
                    //TextTipoVivienda = string.Empty;
                    //TextNoPersonasResiden = 0;
                    //TextPersonaViviaAntes = string.Empty;
                    //TextOtrasPersonasViviaAntes = string.Empty;
                    //TextCondicionesZona = string.Empty;
                    //TextOtroTipoMaterialVivienda = string.Empty;
                    //TextCondicionesZona = string.Empty;
                    //TextSituacionEconomica = string.Empty;

                    #endregion

                    //agrega alias
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
                    MenuReporteEnabled = true;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_LIBERADO);
                    //ValidacionesDatosTrabajoSocial();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener información.", ex);
            }
        }
        
        #region Metodos Droga Frecuencia POPUP
        private void ObtenerConsumoDroga()
        {
            ListDrogaConsumo = new ObservableCollection<PRS_DROGA>(FrecDrogaControlador.Obtener(SelectExpediente.ID_IMPUTADO, SelectExpediente.ID_ANIO, SelectExpediente.ID_CENTRO));

        }

        //--------Verifica si ya existe un registro con el mismo impitado con la misma Informacion --------------
        private bool validarDatosIgualesBd(PRS_DROGA objPRS_DROGA)
        {
            return ListDrogaConsumo.Any(w => w.ID_IMPUTADO == objPRS_DROGA.ID_IMPUTADO && w.ID_ANIO == objPRS_DROGA.ID_ANIO && w.ID_CENTRO == objPRS_DROGA.ID_CENTRO && w.INICIO_CONSUMO == objPRS_DROGA.INICIO_CONSUMO && w.ID_DROGA == objPRS_DROGA.ID_DROGA && w.ID_FRECUENCIA == objPRS_DROGA.ID_FRECUENCIA);
        }
        private void LimpiarDrogaFerecuencia()
        {
            popUpFechaUltDosis = null;
            popUpFrecuenciaUso = -1;
            popUpDrogaId = -1;
        }

        private bool validarCamaposFrecDrogas()
        {
            if (popUpFechaUltDosis != null && popUpFrecuenciaUso > 0 && popUpDrogaId > 0)
            {
                return true;
            }
            return false;
        }


        private void BuscarKey(Object objKey)
        {

            var texbox = objKey;
            var INPUT = ((System.Windows.Input.KeyEventArgs)objKey).KeyboardDevice.Target;
            var TextBox = (System.Windows.Controls.TextBox)INPUT;

            switch (TextBox.Name)
            {
                case "txtEgresPollo":
                    if (((System.Windows.Input.KeyEventArgs)objKey).Key == System.Windows.Input.Key.Back)
                    {
                        if (TextBox.Text.Length == 1)
                        {
                            TextEgresoPolllo = null;
                        }
                    }

                    break;
                case "txtEgresLuz":
                    if (((System.Windows.Input.KeyEventArgs)objKey).Key == System.Windows.Input.Key.Back)
                    {
                        if (TextBox.Text.Length == 1)
                        {
                            TextEgresoLuz = null;
                        }
                    }

                    break;
                case "txtEgresEduc":
                    if (((System.Windows.Input.KeyEventArgs)objKey).Key == System.Windows.Input.Key.Back)
                    {
                        if (TextBox.Text.Length == 1)
                        {
                            TextEgresoEducacion = null;
                        }
                    }

                    break;
                case "txtEgresCombus":
                    if (((System.Windows.Input.KeyEventArgs)objKey).Key == System.Windows.Input.Key.Back)
                    {
                        if (TextBox.Text.Length == 1)
                        {
                            TextEgresoCombustible = null;
                        }
                    }

                    break;

                case "txtEgresRenta":
                    if (((System.Windows.Input.KeyEventArgs)objKey).Key == System.Windows.Input.Key.Back)
                    {
                        if (TextBox.Text.Length == 1)
                        {
                            TextEgresoRenta = null;
                        }
                    }

                    break;
                case "txtEgresAgua":
                    if (((System.Windows.Input.KeyEventArgs)objKey).Key == System.Windows.Input.Key.Back)
                    {
                        if (TextBox.Text.Length == 1)
                        {
                            TextEgresoAgua = null;
                        }
                    }

                    break;
                case "txtEgresGas":
                    if (((System.Windows.Input.KeyEventArgs)objKey).Key == System.Windows.Input.Key.Back)
                    {
                        if (TextBox.Text.Length == 1)
                        {
                            TextEgresoGas = null;
                        }
                    }

                    break;
                case "txtEgresTelefono":
                    if (((System.Windows.Input.KeyEventArgs)objKey).Key == System.Windows.Input.Key.Back)
                    {
                        if (TextBox.Text.Length == 1)
                        {
                            TextEgresoTelefono = null;
                        }
                    }

                    break;
                case "txtEgresVestimenta":
                    if (((System.Windows.Input.KeyEventArgs)objKey).Key == System.Windows.Input.Key.Back)
                    {
                        if (TextBox.Text.Length == 1)
                        {
                            TextEgresoVestimenta = null;
                        }
                    }

                    break;
                case "txtEgresGatosMed":
                    if (((System.Windows.Input.KeyEventArgs)objKey).Key == System.Windows.Input.Key.Back)
                    {
                        if (TextBox.Text.Length == 1)
                        {
                            TextEgresoGatosMedicos = null;
                        }
                    }

                    break;
                case "txtEgresDespensa":
                    if (((System.Windows.Input.KeyEventArgs)objKey).Key == System.Windows.Input.Key.Back)
                    {
                        if (TextBox.Text.Length == 1)
                        {
                            TextEgresoDespensa = null;
                        }
                    }

                    break;
                case "txtEgresLeche":
                    if (((System.Windows.Input.KeyEventArgs)objKey).Key == System.Windows.Input.Key.Back)
                    {
                        if (TextBox.Text.Length == 1)
                        {
                            TextEgresoLeche = null;
                        }
                    }

                    break;
                case "txtEgresLeguimin":
                    if (((System.Windows.Input.KeyEventArgs)objKey).Key == System.Windows.Input.Key.Back)
                    {
                        if (TextBox.Text.Length == 1)
                        {
                            TextEgresoLeguiminosas = null;
                        }
                    }

                    break;
                case "txtEgresFrijol":
                    if (((System.Windows.Input.KeyEventArgs)objKey).Key == System.Windows.Input.Key.Back)
                    {
                        if (TextBox.Text.Length == 1)
                        {
                            TextEgresoFrijol = null;
                        }
                    }

                    break;
                case "txtEgresTortillas":
                    if (((System.Windows.Input.KeyEventArgs)objKey).Key == System.Windows.Input.Key.Back)
                    {
                        if (TextBox.Text.Length == 1)
                        {
                            TextEgresoTortillas = null;
                        }
                    }

                    break;
                case "txtEgresCereales":
                    if (((System.Windows.Input.KeyEventArgs)objKey).Key == System.Windows.Input.Key.Back)
                    {
                        if (TextBox.Text.Length == 1)
                        {
                            TextEgresoCereales = null;
                        }
                    }

                    break;
                case "txtEgrespastas":
                    if (((System.Windows.Input.KeyEventArgs)objKey).Key == System.Windows.Input.Key.Back)
                    {
                        if (TextBox.Text.Length == 1)
                        {
                            TextEgresoPastas = null;
                        }
                    }

                    break;
                case "txtEgrescarneRoja":
                    if (((System.Windows.Input.KeyEventArgs)objKey).Key == System.Windows.Input.Key.Back)
                    {
                        if (TextBox.Text.Length == 1)
                        {
                            TextEgresoCarneRoja = null;
                        }
                    }

                    break;
                case "txtEgresVerduras":
                    if (((System.Windows.Input.KeyEventArgs)objKey).Key == System.Windows.Input.Key.Back)
                    {
                        if (TextBox.Text.Length == 1)
                        {
                            TextEgresoverduras = null;
                        }
                    }

                    break;
                case "txtEgresGolos":
                    if (((System.Windows.Input.KeyEventArgs)objKey).Key == System.Windows.Input.Key.Back)
                    {
                        if (TextBox.Text.Length == 1)
                        {
                            TextEgresoGolosinas = null;
                        }
                    }

                    break;
                case "txtEgresOtros":
                    if (((System.Windows.Input.KeyEventArgs)objKey).Key == System.Windows.Input.Key.Back)
                    {
                        if (TextBox.Text.Length == 1)
                        {
                            TextEgresoOtros = null;
                        }
                    }

                    break;
                default:
                    break;
            }

            ///  var INPUT = (System.Windows.Input.KeyEventArgs)objKey;

            //  var d = (char)INPUT.KeyStates;



            //decimal? ob = objKey!=null?(decimal)objKey:0;
            //TextEgresoGas = ob;


        }
        private void Insert_edit_FrecuenciaUsoDroga()
        {
            if (SelectDrogaConsumo != null)
            {

                foreach (var itemFrecUsoDroga in ListDrogaConsumo.Select(s => new { obj = s }).Where(w => w.obj.Equals(SelectDrogaConsumo)))
                {


                    itemFrecUsoDroga.obj.ID_FRECUENCIA = (short)popUpFrecuenciaUso;
                    itemFrecUsoDroga.obj.DROGA_FRECUENCIA = DrogasFrecControlador.Obtener((int)popUpFrecuenciaUso);
                    itemFrecUsoDroga.obj.ID_DROGA = (short)popUpDrogaId;
                    itemFrecUsoDroga.obj.DROGA = DrogasControlador.Obtener(popUpDrogaId);
                    itemFrecUsoDroga.obj.INICIO_CONSUMO = popUpFechaUltDosis.Value.Date;
                }
            }
            else
            {
                //-----------------------------------AGREGA-------------------------------------------------------

                var ObjConsumoDrgFrec = new PRS_DROGA()
                {
                    ID_IMPUTADO = SelectExpediente.ID_IMPUTADO,
                    ID_ANIO = SelectExpediente.ID_ANIO,
                    ID_CENTRO = SelectExpediente.ID_CENTRO,
                    ID_DROGA = (short)popUpDrogaId,
                    DROGA = DrogasControlador.Obtener(popUpDrogaId),
                    ID_FRECUENCIA = (short)popUpFrecuenciaUso,
                    DROGA_FRECUENCIA = DrogasFrecControlador.Obtener((int)popUpFrecuenciaUso),
                    INICIO_CONSUMO = popUpFechaUltDosis.Value.Date
                };
                ListDrogaConsumo.Add(ObjConsumoDrgFrec);


            }
            RemoverValidacionDrogafrecuencia();
            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_USO_DROGA_TS);
            ListDrogaConsumo = new ObservableCollection<PRS_DROGA>(ListDrogaConsumo);
            OnPropertyChanged("ListDrogaConsumo");

        }

        private void EliminarConsumoDroga()
        {
            if (SelectDrogaConsumo.ID_FOLIO > 0)
            {
                (new Dialogos()).ConfirmacionDialogo("Validación", "No es Posible eliminar este registro ya que fue Guardado Anteriormente.");
            }
            else
            {
                ListDrogaConsumo.Remove(SelectDrogaConsumo);
                (new Dialogos()).ConfirmacionDialogo("Validación", "Registro fue eliminado con exito.");
            }

        }
        #endregion

        private void LimpiarBusqueda()
        {
            ApellidoPaternoBuscar = ApellidoMaternoBuscar = NombreBuscar = NUCBuscar =  string.Empty;
            AnioBuscar  = null;
            FolioBuscar = null;
            SelectExpediente = null;
            SelectedProcesoLibertad = null;
            ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
            ListExpediente = null;
            LstLiberados = null;
        }

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.ENTREVISTA_INICIAL_LIBERADOS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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

        #region Consulta

        private string MetodogenericoValidacionTextBox(string ObjValidar)
        {

            return !string.IsNullOrEmpty(ObjValidar) ? ObjValidar.Trim() : "";
        }

        private void ObtenerEntrevista(PRS_ENTREVISTA_INICIAL ei)
        {
            try
            {
                if (ei != null)
                {
                    TextLugarEntrevista = MetodogenericoValidacionTextBox(ei.LUGAR);
                    TextNucEntrevista = MetodogenericoValidacionTextBox(ei.NUC);
                    TextCausaPenalEntrevista = MetodogenericoValidacionTextBox(ei.CAUSA_PENAL);
                    TextFechaEntrv = ei.REGISTRO_FEC;

                    TextDelitoImputa = MetodogenericoValidacionTextBox(ei.II1_DELITO);
                    TextDomicilioReferencia = MetodogenericoValidacionTextBox(ei.I_DOMICILIO_REFERNCIA);
                    TextNombreApoyo = MetodogenericoValidacionTextBox(ei.III1_NOMBRE);
                    TextEdadApoyo = int.Parse(ei.III2_EDAD);

                    TextCalleApoyo = !string.IsNullOrEmpty(ei.III3_CALLE) ? ei.III3_CALLE.Trim() : string.Empty;
                    if (!string.IsNullOrEmpty(ei.III3_NUM_EXTERIOR))
                        TextNumeroExteriorApoyo = int.Parse(ei.III3_NUM_EXTERIOR);
                    else
                        TextNumeroExteriorApoyo = null;

                    TextNumeroInteriorApoyo = MetodogenericoValidacionTextBox(ei.III3_NUM_INTERIOR);

                    //ei.III3_DOMICILIO = (TextCalleApoyo + " " + TextNumeroExteriorApoyo + " " + TextNumeroInteriorApoyo).Trim();


                    TextTelefonoApoyo = MetodogenericoValidacionTextBox(ei.III4_TELEFONO);
                    SelectOcupacionApoyo = ei.III5_OCUPACION != null ? ei.III5_OCUPACION : -1;
                    //ei.III5_OCUPACION = ListOcupacionesApoyo.Where(w => w.ID_OCUPACION == SelectOcupacionesApoyo).FirstOrDefault().DESCR;
                    TextTiempoConocerceApoyo = MetodogenericoValidacionTextBox(ei.III7_TIEMPO_CONOCERCE);
                    //ei.III6_PARENTESCO = SelectParentesco > -1 ? ListParentesco.Where(w => w.ID_TIPO_REFERENCIA == SelectParentesco).FirstOrDefault().DESCR : string.Empty;
                    SelectParentesco = ei.III6_PARENTESCO != null ? ei.III6_PARENTESCO : -1;

                    TextTipoVivienda = !string.IsNullOrEmpty(ei.IV1_VIVIENDA) ? ei.IV1_VIVIENDA.Trim() : string.Empty;
                    TextNoPersonasResiden = ei.IV2_PERSONAS_VIVEN;

                    if (ei.VII1_PADECIO_ENFERMEDAD == "S")
                        PadeceEnfermedadSi = true;
                    else
                        PadeceEnfermedadNo = true;
                    TextEspecifiqueOtraEnfermedad = MetodogenericoValidacionTextBox(ei.VII1_ESPECIFIQUE);
                    TextTipoTratamientoRecibido = MetodogenericoValidacionTextBox(ei.VII2_TIPO_TRATAMIENTO);

                    TextPersonaViviaAntes = MetodogenericoValidacionTextBox(ei.IV3_VIVIA);
                    TextOtrasPersonasViviaAntes = MetodogenericoValidacionTextBox(ei.IV4_OTRO);
                    TextTipoMaterialVivienda = MetodogenericoValidacionTextBox(ei.IV5_MATERIAL_VIVIENDA);

                    TextComedorNum = ei.IV7_COMEDOR != null ? ei.IV7_COMEDOR : 0;
                    TextComedorObserv = MetodogenericoValidacionTextBox(ei.IV7_OBS_COMEDOR);

                    TextRecamaraNum = ei.IV7_RECAMARAS != null ? ei.IV7_RECAMARAS : 0;
                    TextRecamaraObserv = MetodogenericoValidacionTextBox(ei.IV7_OBS_RECAMARAS);

                    TextSalaNum = ei.IV7_SALA != null ? ei.IV7_SALA : 0;
                    TextSalaObserv = MetodogenericoValidacionTextBox(ei.IV7_OBS_SALA);

                    TextCocinaNum = ei.IV7_COCINA != null ? ei.IV7_COCINA : 0;
                    TextCocinaObserv = MetodogenericoValidacionTextBox(ei.IV7_OBS_COCINA);

                    TextBañoNum = ei.IV7_BANIO != null ? ei.IV7_BANIO : 0;
                    TextBañoObserv = MetodogenericoValidacionTextBox(ei.IV7_OBS_BANIO);

                    TextVentanasNum = ei.IV7_VENTANAS != null ? ei.IV7_VENTANAS : 0;
                    TextVentanasObserv = MetodogenericoValidacionTextBox(ei.IV7_OBS_VENTANAS);

                    TextPatioNum = ei.IV7_PATIOS != null ? ei.IV7_PATIOS : 0;
                    TextPatioObserv = MetodogenericoValidacionTextBox(ei.IV7_OBS_VENTANAS);

                    TextCondicionesZona = MetodogenericoValidacionTextBox(ei.IV8_CONDICIONES_ZONA);
                    TextSituacionEconomica = MetodogenericoValidacionTextBox(ei.IV9_SITUACION_ECONOMICA);

                    TextEgresoLuz = ei.IV11_LUZ;
                    TextEgresoEducacion = ei.IV11_EDUCACION;
                    TextEgresoCombustible = ei.IV11_COMBUSTIBLE;
                    TextEgresoRenta = ei.IV11_RENTA;
                    TextEgresoAgua = ei.IV11_AGUA;
                    TextEgresoGas = ei.IV11_GAS;
                    TextEgresoTelefono = ei.IV11_TELEFONO;
                    TextEgresoVestimenta = ei.IV11_VESTIMENTA;
                    TextEgresoGatosMedicos = ei.IV11_GASTO_MEDICO;
                    TextEgresoDespensa = ei.IV11_DESPENSA;
                    TextEgresoLeche = ei.IV12_LECHE;
                    TextEgresoLeguiminosas = ei.IV12_LEGUMINOSA;
                    TextEgresoFrijol = ei.IV12_FRIJOL;
                    TextEgresoPolllo = ei.IV12_POLLO;
                    TextEgresoTortillas = ei.IV12_TORTILLAS;
                    TextEgresoPastas = ei.IV12_PASTA;
                    TextEgresoCarneRoja = ei.IV12_CARNE;
                    TextEgresoverduras = ei.IV12_VERDURA;
                    TextEgresoCereales = ei.IV12_CEREALES;
                    TextEgresoGolosinas = ei.IV12_GOLOSINAS;
                    TextEgresoOtros = ei.IV12_OTROS;
                    TextEgresoDescrOtros = ei.IV12_OTROSDESCR;
                    TextEgresoTotal = ei.IV11_TOTAL_EGRESO!=null?ei.IV11_TOTAL_EGRESO:null;
                    TextComidasAlDia = (int)ei.IV12_COMIDA_ELABORA;
                    
                    DescrDinamicaFamiliar = MetodogenericoValidacionTextBox(ei.V2_DESCR_DINAMICA_FAM);

                    if (ei.V3_EXISTO_PROBLEMA_FAM == "S")
                        ProblemaFamiliarSi = true;
                    else
                        ProblemaFamiliarNo = true;

                    if (ei.V4_MIEMBRO_FAM_ABANDONO == "S")
                        MiembroFamiliaAbandonoHogarSi = true;
                    else
                        MiembroFamiliaAbandonoHogarNo = true;

                    if (ei.V5_APOYO_ECON_PROC_JUDICIAL == "S")
                        RecibioApoyoEconomicoEnProcesojudicialSi = true;
                    else
                        RecibioApoyoEconomicoEnProcesojudicialNo = true;

                    TextFormaPorqueApoyo = MetodogenericoValidacionTextBox(ei.V5_FORMA_PORQUE);

                    if (ei.V6_ANTECEDENTES_PENALES_FAM == "S")
                        ExitenAntecedentespenalesFamiiarSi = true;
                    else
                        ExitenAntecedentespenalesFamiiarNo = true;

                    if (ei.V7_FAM_CONSUME_SUST_TOXICAS == "S")
                        FamiliarConsumeSustanciaSi = true;
                    else
                        FamiliarConsumeSustanciaNo = true;

                    if (ei.V8_CONSUME_TIPO_DROGA == "S")
                        ConsumidoAlgunTipoDrogaSi = true;
                    else
                        ConsumidoAlgunTipoDrogaNo = true;

                    if (ei.V9_ANTECEDENTES_PENALES_ANTERI == "S")
                        AntecedentesPernalesSi = true;
                    else
                        AntecedentesPernalesNo = true;

                    if (ei.V10_RECIBIO_APOYO_INTERNAMIENT == "S")
                        RecibioApoyoInternamientoSi = true;
                    else
                        RecibioApoyoInternamientoNo = true;

                    TextDeQuienRecibioApoyoInternamiento = MetodogenericoValidacionTextBox(ei.V10_DE_QUIEN);
                    TextFrecuencia = MetodogenericoValidacionTextBox(ei.V10_FRECUENCIA);

                    if (ei.V1_PADRES_VIVIEN_JUNTOS == "S")
                        ViveConPadres = true;
                    else
                        ViveConPadresNo = true;

                    DesdeCuandoVivePadres = MetodogenericoValidacionTextBox(ei.V_DESDECUANDO);
                    TextunionesAnteriores = MetodogenericoValidacionTextBox(ei.V_UNIONES_ANTERIORES);
                    NoHijos = ei.V_HIJOS_NUM != null ? (int)ei.V_HIJOS_NUM : 0;

                    if (ei.V_PROBLEMAS_PAREJA == "S")
                        ProblemaParejaSi = true;
                    else
                        ProblemaParejaNo = true;

                    if (ei.V_MIEMBRO_FAMILIA_ABANDONO_HOG == "S")
                        MiembroFamiliaAbandonoHogarSi = true;
                    else
                        MiembroFamiliaAbandonoHogarNo = true;

                    if (ei.VI1_CONOCE_VECINOS == "S")
                        ConoceVecinosSi = true;
                    else
                        ConoceVecinosNo = true;

                    if (ei.VI2_PROBLEMAS_VECINOS == "S")
                        ProblemasConVecinosSi = true;
                    else
                        ProblemasConVecinosNo = true;

                    TextTiempoLibre = MetodogenericoValidacionTextBox(ei.VI3_DEDICA_TIEMPO_LIBRE);
                    TextTiempoLibreOtro = !string.IsNullOrEmpty(ei.VI3_OTROS) ? ei.VI3_OTROS.Trim() : string.Empty;

                    chkActaNac = ei.VI4_ACTA_NAC == "S" ? true : false;
                    chkActaMatrimonio = ei.VI4_ACTA_MAT == "S" ? true : false;
                    chkComprobanteEstudio = ei.VI4_COMPROBANTE_DOM == "S" ? true : false;
                    chkLiciencia = ei.VI4_LICENCIA == "S" ? true : false;
                    chkPasaporteMex = ei.VI4_PASAPORTE_MEX == "S" ? true : false;
                    chkVisaLaser = ei.VI4_VISA_LASER == "S" ? true : false;
                    chkIFE = ei.VI4_IFE == "S" ? true : false;
                    chkCartilla = ei.VI4_CARTILLA_MILITAR == "S" ? true : false;
                    chkCurp = ei.VI4_CURP == "S" ? true : false;
                    TextDocumentosOtro = !string.IsNullOrEmpty(ei.VI4_OTROS) ? ei.VI4_OTROS.Trim() : string.Empty;


                    TextEspecifiqueOtraEnfermedad = MetodogenericoValidacionTextBox(ei.VII1_ESPECIFIQUE);
                    TextTipoTratamientoRecibido = MetodogenericoValidacionTextBox(ei.VII2_TIPO_TRATAMIENTO);
                    textDiagnostico = MetodogenericoValidacionTextBox(ei.IX_DIAGNOSTICO);

                }
                else
                {
                    LimpiarEntrevista();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al consultar entrevista", ex);
            }
        }
        /// <summary>
        /// Limpia datos de la Entrevista si al seleccionar imputado no encuentra ninguna solicitud relacionada
        /// </summary>
        private void LimpiarEntrevista()
        {
            TextLugarEntrevista = string.Empty;
            TextNucEntrevista = string.Empty;
            TextCausaPenalEntrevista = string.Empty;
            TextFechaEntrv = null;

            TextDelitoImputa = string.Empty;
            TextDomicilioReferencia = string.Empty;
            TextNombreApoyo = string.Empty;
            TextEdadApoyo = null;

            TextCalleApoyo = string.Empty;

            TextNumeroExteriorApoyo = null;

            TextNumeroInteriorApoyo = string.Empty;

            //ei.III3_DOMICILIO = (TextCalleApoyo + " " + TextNumeroExteriorApoyo + " " + TextNumeroInteriorApoyo).Trim();


            TextTelefonoApoyo = string.Empty;
            SelectOcupacionApoyo = -1;
            //ei.III5_OCUPACION = ListOcupacionesApoyo.Where(w => w.ID_OCUPACION == SelectOcupacionesApoyo).FirstOrDefault().DESCR;
            TextTiempoConocerceApoyo = string.Empty;
            //ei.III6_PARENTESCO = SelectParentesco > -1 ? ListParentesco.Where(w => w.ID_TIPO_REFERENCIA == SelectParentesco).FirstOrDefault().DESCR : string.Empty;
            SelectParentesco = -1;

            TextTipoVivienda = string.Empty;
            TextNoPersonasResiden = null;

           
                PadeceEnfermedadSi = false;
     
                PadeceEnfermedadNo = false;
                TextEspecifiqueOtraEnfermedad = string.Empty;
                TextTipoTratamientoRecibido = string.Empty;

                TextPersonaViviaAntes = string.Empty;
                TextOtrasPersonasViviaAntes = string.Empty;
                TextTipoMaterialVivienda = string.Empty;

                TextComedorNum = null;
                TextComedorObserv = string.Empty;

                TextRecamaraNum = null;
                TextRecamaraObserv = string.Empty;

                TextSalaNum = null;
                TextSalaObserv = string.Empty;

                TextCocinaNum = null;
                TextCocinaObserv = string.Empty;

                TextBañoNum = null;
                TextBañoObserv = string.Empty;

                TextVentanasNum = null;
                TextVentanasObserv = string.Empty;

                TextPatioNum = null;
                TextPatioObserv = string.Empty;

                TextCondicionesZona = string.Empty;
                TextSituacionEconomica = string.Empty;

            TextEgresoLuz = null;
            TextEgresoEducacion = null;
            TextEgresoCombustible = null;
            TextEgresoRenta = null;
            TextEgresoAgua = null;
            TextEgresoGas = null;
            TextEgresoTelefono = null;
            TextEgresoVestimenta = null;
            TextEgresoGatosMedicos = null;
            TextEgresoDespensa = null;
            TextEgresoLeguiminosas = null;
            TextEgresoLeche = null;
            TextEgresoFrijol = null;
            TextEgresoPolllo = null;
            TextEgresoTortillas = null;
            TextEgresoPastas = null;
            TextEgresoCarneRoja = null;
            TextEgresoverduras = null;
            TextEgresoCereales = null;
            TextEgresoGolosinas = null;
            TextEgresoOtros = null;
            TextEgresoTotal = null;
            TextEgresoDescrOtros = string.Empty;
            TextComidasAlDia = null;
            DescrDinamicaFamiliar = string.Empty;


            
                ProblemaFamiliarSi = false;
            
                ProblemaFamiliarNo = false;

            
                MiembroFamiliaAbandonoHogarSi = false;
            
                MiembroFamiliaAbandonoHogarNo = false;

            
                RecibioApoyoEconomicoEnProcesojudicialSi = false;
            
                RecibioApoyoEconomicoEnProcesojudicialNo = false;

                TextFormaPorqueApoyo = string.Empty;

            
                ExitenAntecedentespenalesFamiiarSi = false;
            
                ExitenAntecedentespenalesFamiiarNo = false;

            
                FamiliarConsumeSustanciaSi = false;
            
                FamiliarConsumeSustanciaNo = false;

            
                ConsumidoAlgunTipoDrogaSi = false;
            
                ConsumidoAlgunTipoDrogaNo = false;

            
                AntecedentesPernalesSi = false;
            
                AntecedentesPernalesNo = false;

            
                RecibioApoyoInternamientoSi = false;
            
                RecibioApoyoInternamientoNo = false;

                TextDeQuienRecibioApoyoInternamiento = string.Empty;
                TextFrecuencia = string.Empty;

            
                ViveConPadres = false;
            
                ViveConPadresNo = false;

                DesdeCuandoVivePadres = string.Empty;
                TextunionesAnteriores = string.Empty;
                NoHijos = null;

            
                ProblemaParejaSi = false;
            
                ProblemaParejaNo = false;

            
                MiembroFamiliaAbandonoHogarSi = false;
            
                MiembroFamiliaAbandonoHogarNo = false;

            
                ConoceVecinosSi = false;
            
                ConoceVecinosNo = false;

            
                ProblemasConVecinosSi = false;
            
                ProblemasConVecinosNo = false;

                TextTiempoLibre = string.Empty;
                TextTiempoLibreOtro = string.Empty;

                chkActaNac = false;
                chkActaMatrimonio = false;
                chkComprobanteEstudio = false;
                chkLiciencia = false;
                chkPasaporteMex = false;
              
                chkVisaLaser = false;
                chkIFE = false;
                chkCartilla = false;
                chkCurp = false;
                TextDocumentosOtro = string.Empty;


                TextEspecifiqueOtraEnfermedad = string.Empty;
                TextTipoTratamientoRecibido = string.Empty;
                textDiagnostico = string.Empty;

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
            var requiereGuardarHuellas = false;//Parametro.GuardarHuellaEnBusquedaRegistro;
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
