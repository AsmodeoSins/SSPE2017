using ControlPenales.Clases;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Controlador.Catalogo.Justicia.EstudioMI;
using SSP.Servidor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ControlPenales
{
    partial class EntrevistaMultidiciplinariaViewModel : ValidationViewModelBase, IDataErrorInfo
    {
        private EMI emiActual;

        #region constructor
        public EntrevistaMultidiciplinariaViewModel() { }
        public EntrevistaMultidiciplinariaViewModel(INGRESO Ingreso)
        {
            try
            {
                this.SelectIngreso = Ingreso;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el modulo", ex);
            }
        }
        #endregion

        #region metodos
        private void buscar(object obj)
        {
            try
            {
                EmiVisible = false;
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                BotonesEnables = false;
                TatuajesVisible = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar", ex);
            }
        }

        private void salir(object obj)
        {
            try
            {
                EmiVisible = true;
                BotonesEnables = true;
                TatuajesVisible = false;
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al salir", ex);
            }
        }

        private void enable(object obj)
        {
            try
            {
                BotonesEnables = true;
                TatuajesVisible = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al habilitar", ex);
            }
        }

        private void disable(object obj)
        {
            try
            {
                BotonesEnables = false;
                TatuajesVisible = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al deshabilitar", ex);
            }
        }

        private void GuardarCambios()
        {
            try
            {
                if (SelectIngreso == null)
                    return;
                if (CambioImputado)
                    return;

                EliminarItemMenu = false;
                if (string.IsNullOrEmpty(SelectedTabToSave))
                    return;
                switch ((Tabs)Enum.Parse(typeof(Tabs), SelectedTabToSave, true))
                {
                    case Tabs.FichaIdentificacion:
                        //GuardarUltimosEmpleos();
                        GuardarFichaIdentificacion();
                        break;
                    case Tabs.SituacionJuridica:
                        //
                        break;
                    case Tabs.EstudioTraslado:
                        GuardarEstudiosTraslados();
                        break;
                    case Tabs.IngresoAnteriorCereso:
                        //GuardarIngresoAnterior();
                        break;
                    case Tabs.IngresoAnteriorCeresoMenor:
                        //GuardarIngresoAnteriorMenor();
                        break;
                    case Tabs.FactoresSocioFamiliares:
                        if (TabFactoresSocioFamiliaresSelected)
                        {
                            GuardarFactores();
                        }
                        else
                            if (TabGrupoFamiliarSelected)
                            {
                                GuardarDatosGrupoFamiliar();
                            }
                            else
                                if (TabGrupoFamiliarAntecedenteSelected)
                                {
                                    GuardarAntecedentesGrupoFamiliar();
                                    //GuardarGFD();
                                    //GuardarGFDroga();
                                }
                        break;
                    case Tabs.Factores:
                        GuardarFactores();
                        break;
                    case Tabs.DatosGrupoFamiliar:
                        GuardarDatosGrupoFamiliar();
                        break;
                    case Tabs.AntecedentesGrupoFamilliar:
                        GuardarAntecedentesGrupoFamiliar();
                        //GuardarGFD();
                        //GuardarGFDroga();
                        break;
                    case Tabs.ConductaParasocial:
                        //
                        break;
                    case Tabs.UsoDrogas:
                        GuardarUsoDroga();
                        break;
                    case Tabs.HomosexualidadPandillaSexualidad:
                        GuardarHomosexualidadPandillaSexualidad();
                        break;
                    case Tabs.Tatuajes:
                        GuardarTatuajes();
                        break;
                    case Tabs.TopografiaHumanaView:
                        break;
                    case Tabs.Enfermedades:
                        GuardarEnfermedades();
                        break;
                    case Tabs.Actividades:
                        GuardarActividades();
                        break;
                    case Tabs.ClasCrim:
                        break;
                    case Tabs.ClasificacionCriminologicaView:
                        GuardarClasCriminologica();
                        break;
                    case Tabs.FactoresCriminodiagnosticoView:
                        GuardarFactorCrimidiagnostico();
                        break;
                    default:
                        break;
                }
                OnPropertyChanged();
                if (SelectIngreso != null)
                    if (base.HasErrors)
                        (new Dialogos()).ConfirmacionDialogo("Error", "Faltan Campos por capturar");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar cambios", ex);
            }
        }

        private bool ValidarYGuardar()
        {
            try
            {
                if (!base.HasErrors)
                    GuardarCambios();
                return base.HasErrors;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al validar y guardar", ex);
                return false;
            }
        }

        private void OnTabChange(object obj)
        {
            try
            {
                #region inicialización de listas
                //PrepararListas();
                #endregion

                #region evento [Main TabControl]
                ((ControlPenales.EntrevistaMultidiciplinariaView)(obj)).TabGroup.SelectionChanged += (s, e) =>
                {
                    try
                    {
                        if (MainTabGroupIndex == (short?)((System.Windows.Controls.Primitives.Selector)(s)).SelectedIndex)
                            return;
                        if (CambioImputado)
                            return;
                        switch ((Tabs)Enum.Parse(typeof(Tabs), ((System.Windows.Controls.TabControl)(s)).SelectedContent == null ? ((System.Windows.Controls.UserControl)(((System.Windows.Controls.ContentControl)(((System.Windows.Controls.Primitives.Selector)(s)).SelectedItem)).Content)).GetType().Name : ((System.Windows.Controls.UserControl)(((System.Windows.Controls.TabControl)(s)).SelectedContent)).GetType().Name, true))
                        {
                            case Tabs.FichaIdentificacion:
                                if (((System.Windows.Controls.TabControl)(s)).SelectedContent != null)
                                    setValidacionesFichaIdentificacion();
                                break;
                            case Tabs.SituacionJuridica:
                                //setValidacionesSituacionJuridica();
                                break;
                            case Tabs.FactoresSocioFamiliares:
                                //setValidacionesFactoresSocioFamiliares();
                                break;
                            case Tabs.ConductaParasocial:
                                //setValidacionesConductaParasocial();
                                //setValidacionesFactores();
                                break;
                            case Tabs.Actividades:
                                //setValidacionesActividades();
                                break;
                            case Tabs.ClasCrim:
                                //setValidacionesClasificacionCriminologica();
                                //if (!base.HasErrors)
                                //    GuardarCambios();
                                break;
                            default:
                                break;
                        }
                        OnPropertyChanged();
                        if (!ValidarYGuardar())
                        {
                            MainTabGroupIndex = (short?)((System.Windows.Controls.Primitives.Selector)(s)).SelectedIndex;
                            if (MainTabGroupIndex != -1)
                                SelectedTabToSave = ((System.Windows.Controls.UserControl)(((System.Windows.Controls.ContentControl)(((System.Windows.Controls.Primitives.Selector)(s)).SelectedItem)).Content)).GetType().Name;
                        }
                        else
                            if (MainTabGroupIndex.HasValue)
                                ((ControlPenales.EntrevistaMultidiciplinariaView)(obj)).TabGroup.SelectedValue = ((ControlPenales.EntrevistaMultidiciplinariaView)(obj)).TabGroup.Items[MainTabGroupIndex.Value];
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cambio de tab", ex);
                    }
                };
                #endregion

                #region evento 1 [Situacion Juridica]
                ((ControlPenales.EntrevistaMultidiciplinariaView)(obj)).TabGroup.SelectedValue = null;
                ((ControlPenales.SituacionJuridica)(((System.Windows.Controls.ContentControl)(((System.Windows.Controls.ItemsControl)(((ControlPenales.EntrevistaMultidiciplinariaView)(obj)).TabGroup)).Items[1])).Content)).TabGroup.SelectionChanged += (s, e) =>
                {
                    try
                    {
                        if (Tab1Index == (short?)((System.Windows.Controls.Primitives.Selector)(s)).SelectedIndex)
                            return;

                        if (Tab1Index.HasValue)
                        {
                            switch ((Tabs)Enum.Parse(typeof(Tabs), ((System.Windows.Controls.TabControl)(s)).SelectedContent == null ? ((System.Windows.Controls.UserControl)(((System.Windows.Controls.ContentControl)(((System.Windows.Controls.Primitives.Selector)(s)).SelectedItem)).Content)).GetType().Name : ((System.Windows.Controls.UserControl)(((System.Windows.Controls.TabControl)(s)).SelectedContent)).GetType().Name, true))
                            {
                                case Tabs.EstudioTraslado:
                                    setValidacionesEstudiosTraslado();
                                    break;
                                case Tabs.IngresoAnteriorCereso:
                                    setValidacionesIngresosAnteriores();
                                    break;
                                case Tabs.IngresoAnteriorCeresoMenor:
                                    setValidacionesIngresosAnteriores();
                                    break;
                                default:
                                    break;
                            }
                            OnPropertyChanged();
                        }
                        else
                            if (SelectedTabToSave == "SituacionJuridica" && Tab1Index == null)
                            {

                            }
                        if (!ValidarYGuardar())
                        {
                            Tab1Index = (short?)((System.Windows.Controls.Primitives.Selector)(s)).SelectedIndex;
                            if (Tab1Index != -1)
                                SelectedTabToSave = ((System.Windows.Controls.UserControl)(((System.Windows.Controls.ContentControl)(((System.Windows.Controls.Primitives.Selector)(s)).SelectedItem)).Content)).GetType().Name;
                        }
                        else
                            if (Tab1Index.HasValue)
                            {
                                if (Tab1Index != -1)
                                    ((ControlPenales.SituacionJuridica)(((System.Windows.Controls.ContentControl)(((System.Windows.Controls.ItemsControl)(((ControlPenales.EntrevistaMultidiciplinariaView)(obj)).TabGroup)).Items[1])).Content)).TabGroup.SelectedValue = ((ControlPenales.SituacionJuridica)(((System.Windows.Controls.ContentControl)(((System.Windows.Controls.ItemsControl)(((ControlPenales.EntrevistaMultidiciplinariaView)(obj)).TabGroup)).Items[1])).Content)).TabGroup.Items[Tab1Index.Value];
                            }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cambio de tab", ex);
                    }
                };
                #endregion

                #region evento 2 [Sub Tab Factores SocioFamiliares]
                ((ControlPenales.FactoresSocioFamiliares)(((System.Windows.Controls.ContentControl)(((System.Windows.Controls.ItemsControl)(((ControlPenales.EntrevistaMultidiciplinariaView)(obj)).TabGroup)).Items[2])).Content)).TabGroup.SelectionChanged += (s, e) =>
                {
                    try
                    {
                        if (Tab2Index == (short?)((System.Windows.Controls.Primitives.Selector)(s)).SelectedIndex)
                            return;

                        if (Tab2Index.HasValue)
                        {
                            switch ((Tabs)Enum.Parse(typeof(Tabs), ((System.Windows.Controls.TabControl)(s)).SelectedContent == null ? ((System.Windows.Controls.UserControl)(((System.Windows.Controls.ContentControl)(((System.Windows.Controls.Primitives.Selector)(s)).SelectedItem)).Content)).GetType().Name : ((System.Windows.Controls.UserControl)(((System.Windows.Controls.TabControl)(s)).SelectedContent)).GetType().Name, true))
                            {
                                case Tabs.Factores:
                                    setValidacionesFactores();
                                    break;
                                case Tabs.DatosGrupoFamiliar:
                                    setValidacionesDatosGrupoFamiliar();
                                    break;
                                case Tabs.AntecedentesGrupoFamilliar:
                                    setValidacionesDatosGrupoFamiliar();
                                    break;
                                default:
                                    break;
                            }
                            OnPropertyChanged();
                        }
                        else
                            if (SelectedTabToSave == "FactoresSocioFamiliares" && Tab2Index == null)
                            {
                                //PopularTabFactoresSocioFamiliares(emiActual); 
                            }

                        if (!ValidarYGuardar())
                        {
                            try
                            {
                                Tab2Index = (short?)((System.Windows.Controls.Primitives.Selector)(s)).SelectedIndex;
                                if (Tab2Index != -1)
                                    SelectedTabToSave = ((System.Windows.Controls.UserControl)(((System.Windows.Controls.ContentControl)(((System.Windows.Controls.Primitives.Selector)(s)).SelectedItem)).Content)).GetType().Name;
                            }
                            catch { }
                        }
                        else
                            if (Tab2Index.HasValue)
                                if (Tab2Index != -1)
                                    ((ControlPenales.FactoresSocioFamiliares)(((System.Windows.Controls.ContentControl)(((System.Windows.Controls.ItemsControl)(((ControlPenales.EntrevistaMultidiciplinariaView)(obj)).TabGroup)).Items[2])).Content)).TabGroup.SelectedValue = ((ControlPenales.FactoresSocioFamiliares)(((System.Windows.Controls.ContentControl)(((System.Windows.Controls.ItemsControl)(((ControlPenales.EntrevistaMultidiciplinariaView)(obj)).TabGroup)).Items[2])).Content)).TabGroup.Items[Tab2Index.Value];
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cambio de tab", ex);
                    }
                };
                #endregion

                #region evento 3 [Conductas Parasociales]
                ((ControlPenales.EntrevistaMultidiciplinariaView)(obj)).TabGroup.SelectedValue = null;
                ((ControlPenales.ConductaParasocial)(((System.Windows.Controls.ContentControl)(((System.Windows.Controls.ItemsControl)(((ControlPenales.EntrevistaMultidiciplinariaView)(obj)).TabGroup)).Items[3])).Content)).TabGroup.SelectionChanged += (s, e) =>
                {
                    try
                    {
                        if (Tab3Index == (short?)((System.Windows.Controls.Primitives.Selector)(s)).SelectedIndex)
                            return;

                        if (Tab3Index.HasValue)
                        {
                            switch ((Tabs)Enum.Parse(typeof(Tabs), ((System.Windows.Controls.TabControl)(s)).SelectedContent == null ? ((System.Windows.Controls.UserControl)(((System.Windows.Controls.ContentControl)(((System.Windows.Controls.Primitives.Selector)(s)).SelectedItem)).Content)).GetType().Name : ((System.Windows.Controls.UserControl)(((System.Windows.Controls.TabControl)(s)).SelectedContent)).GetType().Name, true))
                            {
                                case Tabs.UsoDrogas:
                                    setValidacionesUsoDrogas();
                                    break;
                                case Tabs.HomosexualidadPandillaSexualidad:
                                    setValidacionesHPS();
                                    break;
                                case Tabs.Tatuajes:
                                    setValidacionesTatuajes();
                                    break;
                                case Tabs.TopografiaHumanaView:
                                    GuardarCambios();
                                    break;
                                case Tabs.Enfermedades:
                                    setValidacionesEnfermedades();
                                    break;
                                default:
                                    break;
                            }
                            OnPropertyChanged();
                        }
                        else
                            if (SelectedTabToSave == "ConductaParasocial" && Tab3Index == null) { }
                        if (!ValidarYGuardar())
                        {
                            Tab3Index = (short?)((System.Windows.Controls.Primitives.Selector)(s)).SelectedIndex;
                            if (Tab3Index != -1)
                                SelectedTabToSave = ((System.Windows.Controls.UserControl)(((System.Windows.Controls.ContentControl)(((System.Windows.Controls.Primitives.Selector)(s)).SelectedItem)).Content)).GetType().Name;
                        }
                        else
                            if (Tab3Index.HasValue)
                                if (Tab3Index != -1)
                                    ((ControlPenales.ConductaParasocial)(((System.Windows.Controls.ContentControl)(((System.Windows.Controls.ItemsControl)(((ControlPenales.EntrevistaMultidiciplinariaView)(obj)).TabGroup)).Items[3])).Content)).TabGroup.SelectedValue = ((ControlPenales.ConductaParasocial)(((System.Windows.Controls.ContentControl)(((System.Windows.Controls.ItemsControl)(((ControlPenales.EntrevistaMultidiciplinariaView)(obj)).TabGroup)).Items[3])).Content)).TabGroup.Items[Tab3Index.Value];
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cambio de tab", ex);
                    }
                };
                #endregion

                #region evento 5 [Clasificacion Criminologica]
                ((ControlPenales.ClasCrim)(((System.Windows.Controls.ContentControl)(((System.Windows.Controls.ItemsControl)(((ControlPenales.EntrevistaMultidiciplinariaView)(obj)).TabGroup)).Items[5])).Content)).TabGroup.SelectionChanged += (s, e) =>
                {
                    try
                    {
                        if (Tab5Index == (short?)((System.Windows.Controls.Primitives.Selector)(s)).SelectedIndex)
                            return;
                        if (Tab5Index.HasValue)
                        {
                            switch ((Tabs)Enum.Parse(typeof(Tabs), ((System.Windows.Controls.TabControl)(s)).SelectedContent == null ? ((System.Windows.Controls.UserControl)(((System.Windows.Controls.ContentControl)(((System.Windows.Controls.Primitives.Selector)(s)).SelectedItem)).Content)).GetType().Name : ((System.Windows.Controls.UserControl)(((System.Windows.Controls.TabControl)(s)).SelectedContent)).GetType().Name, true))
                            {
                                case Tabs.ClasificacionCriminologicaView:
                                    if (((System.Windows.Controls.TabControl)(s)).SelectedContent != null)
                                        setValidacionesClasificacionCriminologica();
                                    break;
                                case Tabs.FactoresCriminodiagnosticoView:
                                    setValidacionesCriminodiagnostico();
                                    break;
                                default:
                                    break;
                            }
                            OnPropertyChanged();
                        }
                        else
                            if (SelectedTabToSave == "ClasCrim" && Tab5Index == null)
                            {

                            }
                        if (!ValidarYGuardar())
                        {
                            Tab5Index = (short?)((System.Windows.Controls.Primitives.Selector)(s)).SelectedIndex;
                            if (Tab5Index != -1)
                                SelectedTabToSave = ((System.Windows.Controls.UserControl)(((System.Windows.Controls.ContentControl)(((System.Windows.Controls.Primitives.Selector)(s)).SelectedItem)).Content)).GetType().Name;
                        }
                        else
                            if (Tab5Index.HasValue)
                                if (Tab5Index != -1)
                                    ((ControlPenales.ClasCrim)(((System.Windows.Controls.ContentControl)(((System.Windows.Controls.ItemsControl)(((ControlPenales.EntrevistaMultidiciplinariaView)(obj)).TabGroup)).Items[5])).Content)).TabGroup.SelectedValue = ((ControlPenales.ClasCrim)(((System.Windows.Controls.ContentControl)(((System.Windows.Controls.ItemsControl)(((ControlPenales.EntrevistaMultidiciplinariaView)(obj)).TabGroup)).Items[5])).Content)).TabGroup.Items[Tab5Index.Value];
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cambio de tab", ex);
                    }
                };
                #endregion

                #region selección default
                ((ControlPenales.EntrevistaMultidiciplinariaView)(obj)).TabGroup.SelectedValue = ((ControlPenales.EntrevistaMultidiciplinariaView)(obj)).TabGroup.Items[0];
                #endregion
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cambio de tab", ex);
            }
        }

        public void clickSwitch(object obj)
        {
            try
            {
                switch (obj.ToString())
                {
                    case "limpiar_menu":
                        if (TabConductasParasocialesSelected)
                            if (TabSeniaParticularSelected)
                            {
                                LimpiarSenias();
                                break;
                            }
                        LimpiarEMI(1);
                        break;
                    case "buscar_menu":
                        //EmiVisible = false;
                        tImputado = SelectExpediente;
                        tIngreso = SelectIngreso;
                        SelectExpediente = null;
                        SelectIngreso = null;
                        TatuajesVisible = false;
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        break;
                    case "buscar_salir":
                        LimpiarBusqueda();
                        TatuajesVisible = false;
                        EmiVisible = true;
                        SelectExpediente = tImputado;
                        SelectIngreso = tIngreso;
                        tImputado = null;
                        tIngreso = null;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        break;
                    case "buscar_seleccionar":
                        if (SelectIngreso == null)
                        {
                            AvisoImputadoVacio();
                            break;
                        }
                        if (Parametro.ESTATUS_ADMINISTRATIVO_INACT.Any(a => a.HasValue ? SelectIngreso.ID_ESTATUS_ADMINISTRATIVO == a.Value : a.HasValue))
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "El ingreso seleccionado no esta activo.");
                            return;
                        }
                        if (SelectIngreso.ID_UB_CENTRO.HasValue ? SelectIngreso.ID_UB_CENTRO != GlobalVar.gCentro : false)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                            return;
                        }
                        var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                        if (SelectIngreso.TRASLADO_DETALLE.Any(a => (a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado) <= Fechas.GetFechaDateServer))
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + SelectIngreso.ID_ANIO.ToString() + "/" +
                                SelectIngreso.ID_IMPUTADO.ToString() + "] tiene un traslado próximo y no tiene permitido ningún cambio de información.");
                            return;
                        }

                        CambioImputado = true;
                        LimpiarEMI(2);
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        setValidacionesFichaIdentificacion();
                        break;
                    case "nueva_busqueda":
                        LimpiarBusqueda();
                        break;
                    case "señas_particulares":
                        TatuajesVisible = true;
                        EmiVisible = false;
                        BotonesEnables = true;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        break;
                    case "guardar_menu":
                        if (SelectIngreso != null)
                        {
                            GuardarMenu();
                        }
                        else
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación", "Favor de seleccionar un ingreso antes de guardar.");
                        }
                        break;
                    #region USO DROGAS
                    case "addUsoDrogas":
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.USO_DROGAS);
                        TituloModal = "Agregar Uso de Drogas";
                        LimpiarUsoDroga();
                        SelectedUsoDroga = null;
                        setValidacionesUsoDrogasPop();
                        break;
                    case "commitUsoDrogas":
                        //setValidacionesUsoDrogasPop();
                        if (!base.HasErrors)
                        {
                            //if (LstUsoDrogas.Where(w => w.ID_DROGA == popUpDrogaId).Count() <= 0)
                            //{
                            AgregarUsoDroga();
                            IsUsoDrogasEmpty = false;
                            LimpiarUsoDroga();
                            //setValidacionesConductaParasocial();
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.USO_DROGAS);
                            //}
                            //else
                            //    AvisoDuplicado();
                        }
                        break;
                    case "rollbackUsoDrogas":
                        LimpiarUsoDroga();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.USO_DROGAS);
                        break;
                    case "editUsoDrogas":
                        TituloModal = "Editar Uso de Drogas";
                        popUpUsoDroga = SelectedUsoDroga;
                        popUpFechaUltDosis = SelectedUsoDroga.FEC_ULTIMA_DOSIS;
                        popUpFrecuenciaUso = SelectedUsoDroga.FRECUENCIA_USO;
                        setValidacionesUsoDrogasPop();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.USO_DROGAS);
                        break;
                    case "delUsoDrogas":
                        EliminarUsoDrogas();
                        break;
                    #endregion
                    #region ULTIMOS EMPLEOS
                    case "addEmp":
                        //PopulateOcupacion();
                        TituloModal = "Agregar Ultimos Empleos";
                        SelectedEmpleo = null;
                        SelectedOcupacion = LstOcupacion.Where(w => w.ID_OCUPACION == -1).FirstOrDefault();
                        setValidacionesEmpleos();
                        OnPropertyChanged();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.ULTIMOS_EMPLEOS);
                        break;
                    case "editEmp":
                        //PopulateOcupacion();
                        TituloModal = "Editar Ultimos Empleos";
                        PopulateUltimosEmpleosModal();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.ULTIMOS_EMPLEOS);
                        break;
                    case "delEmp":
                        EliminarUltimoEmpleo();
                        break;
                    case "agregar_empleo":
                        AgregarUltimosEmpleos();
                        break;
                    case "cancelar_empleo":
                        LimpiarUltimosEmpleos();
                        setValidacionesFichaIdentificacion();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.ULTIMOS_EMPLEOS);
                        break;
                    #endregion
                    #region DATOS GRUPOS FAMILIAR
                    case "addGpoFam":
                        //PopulateDatosGrupoFamiliarCB();
                        SelectedGrupoFamiliar = null;
                        GrupoFamiliar = -1;
                        SelectedRelacionGrupoFamiliar = LstTipoReferencia.Where(w => w.ID_TIPO_REFERENCIA == -1).FirstOrDefault();
                        SelectedOcupacionGrupoFamiliar = LstOcupacion.Where(w => w.ID_OCUPACION == -1).FirstOrDefault();
                        SelectedEdoCivilGrupoFamiliar = LstEstadoCivil.Where(w => w.ID_ESTADO_CIVIL == -1).First();
                        TituloModal = "Agregar Datos Grupo Familiar";
                        setValidacionesDatosGrupoFamiliarPop();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.DATOS_GRUPO_FAMILIAR);
                        break;
                    case "editGpoFam":
                        //PopulateDatosGrupoFamiliarCB();
                        TituloModal = "Editar Datos Grupo Familiar";
                        setValidacionesDatosGrupoFamiliarPop();
                        PopulateDatosGrupoFamiliarModal();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.DATOS_GRUPO_FAMILIAR);
                        break;
                    case "delGpoFam":
                        EliminarGrupoFamiliar();
                        break;
                    case "agregar_dato_grupo_familiar":
                        setValidacionesDatosGrupoFamiliarPop();
                        AgregarDatosGrupoFamiliar();
                        LimpiarDatosGrupoFamiliar();
                        break;
                    case "cancelar_dato_grupo_familiar":
                        LimpiarDatosGrupoFamiliar();
                        setValidacionesDatosGrupoFamiliar();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.DATOS_GRUPO_FAMILIAR);
                        break;
                    #endregion
                    #region FAMILIAR CON DELITO
                    case "addFamDel":
                        //PopulateGFDCB();
                        SelectedFamiliarDelito = null;
                        SelectedParentescoFDel = LstTipoReferencia.Where(w => w.ID_TIPO_REFERENCIA == -1).FirstOrDefault();
                        SelectedEmisorFDel = LstEmisor.Where(w => w.ID_EMISOR == -1).FirstOrDefault();
                        //SelectedDelitoFDel = LstDelitos.Where(w => w.ID_INGRESO_DELITO == -1).FirstOrDefault();
                        SelectedRelacionFDel = LstTipoRelacion.Where(w => w.ID_RELACION == -1).FirstOrDefault();
                        TituloModal = "Agregar Antecedente Familiar Delictivo";
                        setValidacionesFamiliarDelitoPop();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.FAMILIAR_DELITO);
                        break;
                    case "editFamDel":
                        PopulateGFDCB();
                        TituloModal = "Editar Antecedente Familiar Delictivo";
                        PopulateGFDModal();
                        setValidacionesFamiliarDelitoPop();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.FAMILIAR_DELITO);
                        break;
                    case "delFamDel":
                        EliminarGFD();
                        break;
                    case "agregar_familiar_delito":
                        AgregarGFD();
                        break;
                    case "cancelar_familiar_delito":
                        LimpiarGFD();
                        setValidacionesSituacionJuridica();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.FAMILIAR_DELITO);
                        break;
                    #endregion
                    #region FAMILIAR CON DROGA
                    case "addFamDroga":
                        //PopulateGFDrogaCB();
                        LimpiarGFDroga();
                        SelectedFamiliarDelito = null;
                        SelectedParentescoFDroga = LstTipoReferencia.Where(w => w.ID_TIPO_REFERENCIA == -1).FirstOrDefault();
                        SelectedDrogaFDroga = LstDrogas.Where(w => w.ID_DROGA == -1).FirstOrDefault();
                        SelectedRelacionFDroga = LstTipoRelacion.Where(w => w.ID_RELACION == -1).FirstOrDefault();
                        TituloModal = "Agregar Familiar Uso Drogas";
                        setValidacionesFamiliarDrogaPop();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.FAMILIAR_DROGA);
                        break;
                    case "editFamDroga":
                        //PopulateGFDrogaCB();
                        TituloModal = "Editar Familiar Uso Drogas";
                        PopulateGFDrogaModal();
                        setValidacionesFamiliarDrogaPop();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.FAMILIAR_DROGA);
                        break;
                    case "delFamDroga":
                        EliminarGFDroga();
                        break;
                    case "agregar_familiar_droga":
                        var index = LstFamiliarDroga.LastOrDefault() != null ? short.Parse((LstFamiliarDroga.LastOrDefault().ID_EMI_ANT_CONS + 1).ToString()) : (short)1;
                        if (LstFamiliarDroga.Where(w => w.ID_EMI == emiActual.ID_EMI && w.ID_EMI_CONS == emiActual.ID_EMI_CONS && w.ID_EMI_ANT_CONS == index).Count() <= 0)
                            AgregarGFDroga();
                        else
                            AvisoDuplicado();
                        break;
                    case "cancelar_familiar_droga":
                        LimpiarGFDroga();
                        setValidacionesSituacionJuridica();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.FAMILIAR_DROGA);
                        break;
                    #endregion
                    #region INGRESOS ANTERIORES
                    case "addIngresoAnt":
                        TituloModal = "Agregar Ingreso CE.RE.SO.";
                        SelectedIngresoAnterior = null;
                        TipoIngresoAnterior = 2;
                        SelectedEmisorIngreso = LstEmisor.Where(w => w.ID_EMISOR == -1).FirstOrDefault();
                        //SelectedDelitoIngreso = LstDelitosCP.Where(w => w.ID_DELITO == -1).FirstOrDefault();
                        setValidacionesIngresosAnterioresPop();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.INGRESO_ANTERIOR);
                        break;
                    case "editIngresoAnt":
                        TituloModal = "Editar Ingreso CE.RE.SO.";
                        TipoIngresoAnterior = 2;
                        PopulateIngrsoAnteriorPop();
                        setValidacionesIngresosAnterioresPop();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.INGRESO_ANTERIOR);
                        break;
                    case "delIngresoAnt":
                        EliminarIngresoAnterior();
                        break;
                    #endregion
                    #region INGRESOS ANTERIORES MENOR
                    case "addIngresoAntMenor":
                        TituloModal = "Agregar Ingreso Centro Menores";
                        SelectedIngresoAnteriorMenor = null;
                        TipoIngresoAnterior = 1;
                        SelectedEmisorIngreso = LstEmisor.Where(w => w.ID_EMISOR == -1).FirstOrDefault();
                        SelectedDelitoIngreso = LstDelitosCP.Where(w => w.ID_DELITO == -1).FirstOrDefault();
                        setValidacionesIngresosAnterioresPop();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.INGRESO_ANTERIOR);
                        break;
                    case "editIngresoAntMenor":
                        TituloModal = "Editar Ingreso Centro Menores";
                        TipoIngresoAnterior = 1;
                        PopulateIngrsoAnterioMenorPop();
                        setValidacionesIngresosAnterioresPop();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.INGRESO_ANTERIOR);
                        break;
                    case "delIngresoAntMenor":
                        EliminarIngresoAnteriorMenor();
                        break;
                    case "agregar_ingreso_anterior":
                        if (!base.HasErrors)
                        {
                            if (TipoIngresoAnterior == 1)
                                AgregarIngresoAnteriorMenor();
                            else
                                AgregarIngresoAnterior();
                            setValidacionesIngresosAnteriores();
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.INGRESO_ANTERIOR);
                        }
                        break;
                    case "cancelar_ingreso_anterior":
                        LimpiarIngresoanterior();
                        setValidacionesIngresosAnterioresPop();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.INGRESO_ANTERIOR);
                        break;
                    #endregion
                    #region SANCIONES DISCIPLINARIAS
                    case "addSancion":
                        TituloModal = "Agregar Sanci\u00F3n Disciplinaria / Nuevo Proceso";
                        LimpiarClasCriminologica();
                        SelectedSancion = null;
                        setValidacionesClasificacionCriminologicaPop();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.SANCION_DISCIPLINARIA);
                        break;
                    case "editSancion":
                        TituloModal = "Editar Sanci\u00F3n Disciplinaria / Nuevo Proceso";
                        NewSancion = SelectedSancion;
                        //PoplularSancionPopUp();
                        setValidacionesClasificacionCriminologicaPop();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.SANCION_DISCIPLINARIA);
                        break;
                    case "commitSancion":
                        if (!base.HasErrors)
                        {
                            AddSancion();
                            LimpiarClasCriminologica();
                            setValidacionesClasificacionCriminologica();
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.SANCION_DISCIPLINARIA);
                        }
                        break;
                    case "rollbackSancion":
                        LimpiarClasCriminologica();
                        setValidacionesClasificacionCriminologica();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.SANCION_DISCIPLINARIA);
                        break;
                    case "delSancion":
                        EliminarSancion();
                        LimpiarClasCriminologica();
                        break;
                    #endregion
                    #region IMPRIMIR EMI
                    case "reporte_menu":
                        if (SelectIngreso != null)
                        {
                            if (emiActual != null)
                            {
                                if (emiActual.EMI_FACTOR_CRIMINODIAGNOSTICO != null)
                                    if (emiActual.EMI_FACTOR_CRIMINODIAGNOSTICO.ID_EMI != 0)
                                    {
                                        ImprimirEMI();
                                    }
                                    else
                                        new Dialogos().ConfirmacionDialogo("Notificación", "Debe completar la captura para poder imprimir.");
                                else
                                    new Dialogos().ConfirmacionDialogo("Notificación", "Debe completar la captura para poder imprimir.");
                            }
                            else
                                new Dialogos().ConfirmacionDialogo("Notificación", "Debe completar la captura para poder imprimir.");
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Notificación", "Favor de seleccionar un ingreso antes de imprimir.");
                        break;
                    #endregion
                    #region DICTAMEN
                    case "dictamen_add":
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_DICTAMEN);
                        break;
                    case "agregar_dictamen":
                        AgregarDictamen();
                        break;
                    case "cancelar_dictamen":
                        LimpiarDictamen();
                        break;
                    #endregion
                    #region GRUPO FAMILIAR
                    case "buscar_gfpv":
                        PopulateGrupoFamiliarPV();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_GFPV);
                        break;
                    case "agregar_gfpv":
                        AgregarGrupoFamiliarPV();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_GFPV);
                        break;
                    case "cancelar_gfpv":
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_GFPV);
                        break;
                    #endregion
                    #region ACTIVIDADES
                    case "addActividad":
                        TituloModal = "Agregar Actividad";
                        LimpiarActividad();
                        setValidacionesActividadesPop();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_ACTIVIDAD);
                        break;
                    case "editActividad":
                        TituloModal = "Editar Actividad";
                        PopUpActividad = SelectedActividad;
                        setValidacionesActividadesPop();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_ACTIVIDAD);
                        break;
                    case "commitActividad":
                        if (!base.HasErrors)
                        {
                            AddActividad();
                            LimpiarActividad();
                            base.ClearRules();
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_ACTIVIDAD);
                        }
                        break;
                    case "rollbackActividad":
                        base.ClearRules();
                        LimpiarActividad();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_ACTIVIDAD);
                        break;
                    case "delActividad":
                        EliminarActividad();
                        LimpiarActividad();
                        break;
                    #endregion
                    #region INGRESOS_ANTERIORES
                    case "buscar_ingreso_anterior":
                        TituloModal = "Buscar Ingresos Anteriores";
                        PopulateIngresosAnterioresSistema();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_IAS);
                        break;
                    case "agregar_ias":
                        AgregarIngresosAnterioresSistema();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_IAS);
                        break;
                    case "cancelar_ias":
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_IAS);
                        break;
                    #endregion
                    #region SENIAS PERTICULARES
                    case "seleccionar_sena_particular":
                        PopulateSeniaParticular();
                        break;
                    case "ampliar_descripcion":
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AMPLIAR_DESCRIPCION);
                        break;
                    case "tomar_foto_senas":
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.FOTOSSENIASPARTICULAES);
                        //BotonTomarFotoEnabled = false;
                        //Loading1()
                        TomarFotoLoad(PopUpsViewModels.MainWindow.FotosSenasView);
                        break;
                    case "aceptar_tomar_foto_senas":
                        ImagenTatuaje = ImagesToSave[0].ImageCaptured;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.FOTOSSENIASPARTICULAES);
                        if (CamaraWeb != null)
                        {
                            CamaraWeb.ReleaseVideoDevice();
                            CamaraWeb = null;
                        }
                        break;
                    case "cancelar_tomar_foto_senas":
                        //ImagenTatuaje = new ima
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.FOTOSSENIASPARTICULAES);
                        if (CamaraWeb != null)
                        {
                            CamaraWeb.ReleaseVideoDevice();
                            CamaraWeb = null;
                        }
                        break;
                    case "guardar_ampliar_descripcion":
                        TextSignificado = TextSignificado + " " + TextAmpliarDescripcion;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AMPLIAR_DESCRIPCION);
                        break;
                    case "cancelar_ampliar_descripcion":
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AMPLIAR_DESCRIPCION);
                        break;
                    #endregion

                    case "cerrar_historico_tratamiento":
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.HISTORICO_TRATAMIENTO);
                        break;

                    case "historial_tratamiento":
                        VerHistorialTratamiento();
                        break;
                    case "cargar_historial":
                        CargarHistorial();
                        break;
                    case "salir_menu":
                        PrincipalViewModel.SalirMenu();
                        break;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el flujo del modulo", ex);
            }
        }
        #endregion

        #region Populado de información


        private void PopulateFicha()
        {
            try
            {
                var ing = SelectIngreso;
                #region Encabezado
                Cereso = ing.CENTRO.DESCR;
                ClasificacionJuridica = ing.CLASIFICACION_JURIDICA.DESCR;

                if (ing.CAMA != null && 1 == 2)
                    Ubicacion = string.Format("{0}-{1}-{2}-{3}", ing.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim(), ing.CAMA.CELDA.SECTOR.DESCR.Trim(),
                        ing.CAMA.CELDA.ID_CELDA.Trim(),
                        ing.CAMA.ID_CAMA);

                Expediente = string.Format("{0}/{1}", ing.IMPUTADO.ID_ANIO, ing.IMPUTADO.ID_IMPUTADO);
                #region Causas Penales
                Ingreso = ing.FEC_INGRESO_CERESO.Value;
                var causas = string.Empty;
                foreach (var causa in ing.CAUSA_PENAL)
                {
                    if (causa == (ing.CAUSA_PENAL.First()))
                        causas = string.Format("{0}/{1}", causa.CP_ANIO, causa.CP_FOLIO);
                    else
                        causas += string.Format(", {0}/{1}", causa.CP_ANIO, causa.CP_FOLIO); ;
                }
                CausaPenal = causas;
                #endregion
                #endregion
                #region Ficha ID
                try
                {
                    ApellidoPaterno = ing.IMPUTADO.PATERNO;
                    ApellidoMaterno = ing.IMPUTADO.MATERNO;
                    Nombre = ing.IMPUTADO.NOMBRE;
                    //EstadoCivil = ing.IMPUTADO.ESTADO_CIVIL.DESCR;
                    EstadoCivil = ing.ESTADO_CIVIL != null ? ing.ESTADO_CIVIL.DESCR : string.Empty;
                    Sexo = ing.IMPUTADO.SEXO == "M" ? "MASCULINO" : "FEMENINO";
                    FechaNacimiento = ing.IMPUTADO.NACIMIENTO_FECHA;
                    if (ing.IMPUTADO.NACIMIENTO_FECHA != null)
                    {
                        EdadInterno = (short)new Fechas().CalculaEdad(ing.IMPUTADO.NACIMIENTO_FECHA);
                        Edad = new Fechas().CalculaEdad(ing.IMPUTADO.NACIMIENTO_FECHA).ToString();
                    }
                    else
                        EdadInterno = 0;
                    //Religion = ing.IMPUTADO.RELIGION.DESCR;
                    Religion = ing.RELIGION != null ? ing.RELIGION.DESCR : string.Empty;
                    Etnia = ing.IMPUTADO.ETNIA.DESCR;
                    var _apodos = string.Empty;
                    foreach (var apodo in ing.IMPUTADO.APODO)
                    {
                        if (apodo == (ing.IMPUTADO.APODO.First()))
                            _apodos += apodo.APODO1;
                        else
                            _apodos += string.Format(", {0}", apodo.APODO1); ;
                    }
                    Apodo = _apodos;
                    var objNac = new SSP.Controlador.Catalogo.Justicia.cMunicipio();
                    var LNacimiento = objNac.Obtener(ing.IMPUTADO.NACIMIENTO_ESTADO.Value, ing.IMPUTADO.NACIMIENTO_MUNICIPIO.Value).FirstOrDefault();
                    LNMunicipio = LNacimiento.MUNICIPIO1;
                    LNEstado = LNacimiento.ENTIDAD.DESCR;
                    LNPais = LNacimiento.ENTIDAD.PAIS_NACIONALIDAD.PAIS;
                    Nacionalidad = LNacimiento.ENTIDAD.PAIS_NACIONALIDAD.NACIONALIDAD;
                    //var llegadaImputado = Fechas.GetFechaDateServer.Date.AddYears(-ing.IMPUTADO.RESIDENCIA_ANIOS.Value);
                    //llegadaImputado = llegadaImputado.AddMonths(-ing.IMPUTADO.RESIDENCIA_MESES.Value);
                    //FechaLlegada = llegadaImputado;
                    //Años = ing.IMPUTADO.RESIDENCIA_ANIOS;
                    //Meses = ing.IMPUTADO.RESIDENCIA_MESES;
                    var llegadaImputado = Fechas.GetFechaDateServer.Date.AddYears(-ing.RESIDENCIA_ANIOS.Value);
                    llegadaImputado = llegadaImputado.AddMonths(-ing.RESIDENCIAS_MESES.Value);
                    FechaLlegada = llegadaImputado;
                    Años = ing.RESIDENCIA_ANIOS;
                    Meses = ing.RESIDENCIAS_MESES;
                    //if (ing.IMPUTADO.PAIS_NACIONALIDAD != null)
                    //    DPais = ing.IMPUTADO.PAIS_NACIONALIDAD.PAIS;
                    //DEstado = ing.IMPUTADO.ENTIDAD.DESCR;
                    //DMunicipio = ing.IMPUTADO.MUNICIPIO.MUNICIPIO1;
                    //Colonia = ing.IMPUTADO.COLONIA.DESCR;
                    if (ing.PAIS_NACIONALIDAD != null)
                        DPais = ing.PAIS_NACIONALIDAD.PAIS;
                    DEstado = ing.MUNICIPIO.ENTIDAD.DESCR;
                    DMunicipio = ing.MUNICIPIO.MUNICIPIO1;
                    Colonia = ing.COLONIA.DESCR;
                    Calle = ing.DOMICILIO_CALLE;
                    NumeroExterior = ing.DOMICILIO_NUM_EXT.Value;
                    NumeroInterior = ing.DOMICILIO_NUM_INT;
                    CodigoPostal = ing.DOMICILIO_CP;
                }
                catch (Exception ex)
                {
                    var metro = Application.Current.Windows[0] as MahApps.Metro.Controls.MetroWindow;
                    var mySettings = new MahApps.Metro.Controls.Dialogs.MetroDialogSettings()
                    {
                        AffirmativeButtonText = "OK",
                        AnimateHide = true,
                        AnimateShow = true
                    };
                    //var result = await metro.ShowOverlayAsync();
                    // dialogo.
                }
                #endregion
                #region Datos Ficha
                if (ing.EMI_INGRESO != null)
                {
                    //emiActual = ing.EMI_INGRESO.Where(w => w.ESTATUS == "A").FirstOrDefault();
                    if (emiActual == null)
                    {
                        //OBTENEMOS EL ULTIMO EMI PARA DAR UNA PLANTILLA DE INFORMACION
                        ObtenerUltimoEMI();
                        emiActual = new SSP.Servidor.EMI();
                    }
                    if (emiActual != null)
                    {
                        #region Ficha Identificación
                        if (emiActual.EMI_FICHA_IDENTIFICACION != null)
                        {
                            ControlTab = 1;
                            var ficha = emiActual.EMI_FICHA_IDENTIFICACION;
                            FechaCaptura = ficha.FEC_CAPTURA;
                            TiempoColonia = ficha.TIEMPO_RESID_COL;
                            UltimoGradoEducativoConcluido = ficha.ID_GRADO_EDUCATIVO_CONCLUIDO;
                            ViviaAntesDetencion = ficha.PERSONA_CONVIVENCIA_ANTERIOR;
                            ExFuncionarioSeguridadPublica = ficha.ID_EXFUNCIONARIO_SEGPUB;
                            Parentesco = ficha.ID_PARENTESCO;
                            CertificadoEducacion = ficha.ID_CERTIFICADO_EDUCACION;
                            CeresoProcedencia = ficha.ID_CERESO_PROCEDENCIA != null ? ficha.ID_CERESO_PROCEDENCIA : -1;
                            ActaNacimiento = ficha.ACTA_NACIMIENTO == "S" ? true : false;
                            Pasaporte = ficha.PASAPORTE == "S" ? true : false;
                            LicenciaManejo = ficha.LICENCIA_MANEJO == "S" ? true : false;
                            CredencialElector = ficha.CREDENCIAL_ELECTOR == "S" ? true : false;
                            CartillaMilitar = ficha.CARTILLA_MILITAR == "S" ? true : false;
                            CertificadoEducacion = ficha.ID_CERTIFICADO_EDUCACION;
                            OficiosHabilidades = ficha.OFICIOS_HABILIDADES;
                            UltimoAnio = ficha.CAMBIOS_DOMICILIO_ULTIMO_ANO;
                            Motivo = ficha.MOTIVOS_CAMBIOS_DOMICILIO;
                        }
                        else
                        {
                            //CARGAMOS LOS DATOS POR DEFAULT
                            UltimoGradoEducativoConcluido = -1;
                            ExFuncionarioSeguridadPublica = -1;
                            Parentesco = -1;
                            CertificadoEducacion = -1;
                            CeresoProcedencia = -1;
                            UltimoAnio = 0;
                        }
                        #endregion
                        #region Ultimos Empleos
                        if (emiActual.EMI_ULTIMOS_EMPLEOS != null)
                        {
                            LstUltimosEmpleos = new ObservableCollection<SSP.Servidor.EMI_ULTIMOS_EMPLEOS>(emiActual.EMI_ULTIMOS_EMPLEOS);
                            IsEmpleosEmpty = LstUltimosEmpleos == null ? true : LstUltimosEmpleos.Count <= 0 ? true : false;
                        }
                        else
                            IsEmpleosEmpty = true;
                        #endregion
                        #region Situacion Jurídica
                        #region Estudio de Traslado
                        #region Instanciación de pruebas
                        //emiActual.EMI_SITUACION_JURIDICA = new SSP.Servidor.EMI_SITUACION_JURIDICA();
                        //emiActual.EMI_SITUACION_JURIDICA.DESEA_TRASLADO = "S";
                        //emiActual.EMI_SITUACION_JURIDICA.PRACT_ESTUDIOS = "S";
                        //emiActual.EMI_SITUACION_JURIDICA.MOTIVO_DESEA_TRASLADO = "Para que me hagan estudios.";
                        #endregion
                        if (emiActual.EMI_SITUACION_JURIDICA != null)
                        {
                            ControlTab = 2;
                            VersionDelito = emiActual.EMI_SITUACION_JURIDICA.VERSION_DELITO_INTERNO;
                            PracticadoEstudios = emiActual.EMI_SITUACION_JURIDICA.PRACT_ESTUDIOS == null ? false : emiActual.EMI_SITUACION_JURIDICA.PRACT_ESTUDIOS == "S" ? true : false;
                            Traslado = emiActual.EMI_SITUACION_JURIDICA.DESEA_TRASLADO == null ? false : emiActual.EMI_SITUACION_JURIDICA.DESEA_TRASLADO == "S" ? true : false;
                            Cuando = emiActual.EMI_SITUACION_JURIDICA.CUANDO_PRACT_ESTUDIOS;
                            Donde = emiActual.EMI_SITUACION_JURIDICA.DONDE_DESEA_TRASLADO;
                            PorqueMotivo = emiActual.EMI_SITUACION_JURIDICA.MOTIVO_DESEA_TRASLADO;
                            MenorPeriodoLibreReingreso = emiActual.EMI_SITUACION_JURIDICA.MENOR_PERIODO_LIBRE_REING;
                            MayorPeriodoLibreReingreso = emiActual.EMI_SITUACION_JURIDICA.MAYOR_PERIODO_LIBRE_REING;
                        }
                        else
                        {
                            PracticadoEstudios = false;
                            Traslado = false;
                            Cuando = Donde = PorqueMotivo = string.Empty;
                            MenorPeriodoLibreReingreso = "NINGUNO";
                            MayorPeriodoLibreReingreso = "NINGUNO";
                        }
                        #region [COMENTADO]
                        //ULTIMOS INGRESOS AL CERSO
                        //if (emiActual.EMI_INGRESO_ANTERIOR.Count == 0)//SI ES NULL BUSCA LOS INGRESOS ANTERIORES
                        //{
                        //    var anteriores = new cIngreso().ObtenerIngresosAnteriores(ing.ID_CENTRO, ing.ID_ANIO, ing.ID_IMPUTADO, ing.ID_INGRESO);
                        //    if (anteriores != null)//SI EXISTEN INGRESOS
                        //    {
                        //        short index = 0;
                        //        int id_centro = 0;
                        //        CAUSA_PENAL_DELITO delito = null;
                        //        var ingresosAnteriores = new List<EMI_INGRESO_ANTERIOR>();
                        //        foreach (INGRESO ingreso in anteriores)
                        //        {
                        //            //CENTRO
                        //            id_centro = 0;
                        //            switch (ingreso.ID_UB_CENTRO)
                        //            { 
                        //                case 4:
                        //                    id_centro = 202;
                        //                    break;

                        //            }

                        //            //DELITO
                        //            if (ingreso.CAUSA_PENAL != null)
                        //            { 
                        //                //id_delito = ingreso.CAUSA_PENAL.FirstOrDefault()
                        //                var cp = ingreso.CAUSA_PENAL.FirstOrDefault();
                        //                if (cp != null)
                        //                {
                        //                    if (cp.CAUSA_PENAL_DELITO != null)
                        //                    {
                        //                        delito = cp.CAUSA_PENAL_DELITO.FirstOrDefault();
                        //                    }
                        //                }
                        //            }

                        //            //REGISTRAMOS LOS INGRESOS ANTERIORES
                        //            ingresosAnteriores.Add(new EMI_INGRESO_ANTERIOR() { 
                        //               ID_EMI = emiActual.ID_EMI,
                        //               ID_EMI_CONS = emiActual.ID_EMI_CONS,
                        //               ID_TIPO = 2,
                        //               ID_CONSEC = index,
                        //               ID_EMISOR = id_centro,
                        //               PERIODO_RECLUSION = "TEST",
                        //               SANCIONES = "TEST",
                        //               ID_FUERO = delito.ID_FUERO,
                        //               ID_DELITO = delito.ID_DELITO
                        //            });
                        //            index++;
                        //        }
                        //    }
                        //}
                        #endregion
                        #endregion
                        #endregion
                        PopularTabFactoresSocioFamiliares(emiActual);
                        PopularConductasParasociales(emiActual);
                    }
                    else
                    {
                        IsEmpleosEmpty = true;
                    }
                }
                else
                {
                    IsEmpleosEmpty = true;
                }
                #endregion
                //PopulateUltimosEmpleos();
                //PopulateIngresosAnteriores();
                //PopulateIngresosAnterioresMenores();
                //PopulateActividades();
                //PopularClasificacionCriminologica();
                //PopulateFactorCrimidiagnostico();
                //PopulateGeneralesSenias();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer ficha", ex);
            }
        }
        /// <summary>
        /// Llena con información los controles del tab Factores Socio-Familiares.
        /// </summary>
        /// <param name="emiActual"></param>
        private void PopularTabFactoresSocioFamiliares(EMI emiActual)
        {
            try
            {
                if (emiActual != null)
                {
                    #region Factores Socio Familiares
                    #region Instanciación de prueba
                    //if (emiActual.EMI_FACTORES_SOCIO_FAMILIARES == null)
                    //    emiActual.EMI_FACTORES_SOCIO_FAMILIARES = new SSP.Servidor.EMI_FACTORES_SOCIO_FAMILIARES();
                    //emiActual.EMI_FACTORES_SOCIO_FAMILIARES.VIVE_MADRE = "S";
                    //emiActual.EMI_FACTORES_SOCIO_FAMILIARES.VIVE_PADRE = "S";
                    //emiActual.EMI_FACTORES_SOCIO_FAMILIARES.PADRES_JUNTOS = "S";
                    //emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ABANDONO_FAMILIAR = "S";
                    //emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ESPECIF_ABANDONO_FAMILIAR = "Me fui porque no me gustaba mi familia.";
                    #endregion
                    #region Factores
                    if (emiActual.EMI_FACTORES_SOCIO_FAMILIARES != null)
                    {
                        ControlTab = 5;
                        try
                        {
                            RecibeVisitaFamiliar = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.RECIBE_VISITA_FAMILIAR;
                            Frecuencia = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.FRECUENCIA_VISITA_FAMILIAR;
                            VisitaIntima = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.RECIBE_VISITA_INTIMA;
                            ApoyoEconomico = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.RECIBE_APOYO_ECONOMICO;
                            CantidadFrecuencia = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.FRECUENCIA_APOYO_ECONOMICO;
                            CantidadApoyoEconomico = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.CANTIDAD_APOYO_ECONOMICO;
                            VivePadre = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.VIVE_PADRE == null ? SelectIngreso.PADRE_FINADO : emiActual.EMI_FACTORES_SOCIO_FAMILIARES.VIVE_PADRE;
                            ViveMadre = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.VIVE_MADRE == null ? SelectIngreso.MADRE_FINADO : emiActual.EMI_FACTORES_SOCIO_FAMILIARES.VIVE_MADRE;
                            PadresVivenJuntos = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.PADRES_JUNTOS;
                            FallecioPadre = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.EDAD_INTERNO_FALLE_PADRE;
                            FallecioMadre = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.EDAD_INTERNO_FALLE_MADRE;
                            MotivoSeparacion = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.MOTIVOS_SEPARACION;
                            EdadInternoSeparacionPadres = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.EDAD_INTERNO_SEPARACION;
                            TotalParejas = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.TOTAL_PAREJAS;
                            CuantasUnion = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.CANTIDAD_PAREJAS_UNION;
                            ContactoNombre = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.CONTACTO_NOMBRE;
                            ContactoParentesco = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ID_CONTACTO_PARENTESCO;
                            //ContactoTelefono = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.CONTACTO_TELEFONO;
                            TextContactoTelefono = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.CONTACTO_TELEFONO.ToString();
                            Social = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ID_NIVEL_SOCIAL;
                            Economico = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ID_NIVEL_ECONOMICO;
                            Cultural = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ID_NIVEL_CULTURAL;
                            Hijos = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.NUMERO_HIJOS;
                            Registrados = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.HIJOS_REGISTRADOS;
                            CuantosMantieneRelacion = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.CANTIDAD_HIJOS_RELACION;
                            CuantosVisitan = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.CANTIDAD_HIJOS_VISITA;
                            AbandonoFamiliar = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ABANDONO_FAMILIAR;
                            EspecifiqueAbandonoFamiliar = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ESPECIF_ABANDONO_FAMILIAR;
                            MaltratoEmocional = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.MALTRATO_EMOCIONAL;
                            EspecifiqueMaltratoEmocional = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ESPECIF_MALTRATO_EMOCIONAL;
                            MaltratoFisico = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.MALTRATO_FISICO;
                            EspecifiqueMaltratoFisico = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ESPECIFIQUE_MALTRATO_FISICO;
                            AbusoSexual = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ABUSO_SEXUAL;
                            EspecifiqueAbusoSexual = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ESPECIFIQUE_ABUSO_SEXUAL;
                            EdadAbuso = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.EDAD_ABUSO_SEXUAL;
                            EdadInicioContactoSexual = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.EDAD_INICIO_CONTACTO_SEXUAL;
                            HuidasHogar = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.HUIDAS_HOGAR_ABANDONO_FAMILIAR;
                        }
                        catch (Exception ex)
                        {
                            // throw new ApplicationException(ex.Message);
                        }
                    }
                    else
                    {
                        RecibeVisitaFamiliar = string.Empty;
                        Frecuencia = -1;
                        VisitaIntima = string.Empty;
                        ApoyoEconomico = string.Empty;
                        CantidadFrecuencia = -1;
                        CantidadApoyoEconomico = string.Empty;
                        VivePadre = string.Empty;
                        ViveMadre = string.Empty;
                        PadresVivenJuntos = string.Empty;
                        FallecioPadre = 0;
                        FallecioMadre = 0;
                        MotivoSeparacion = string.Empty;
                        EdadInternoSeparacionPadres = 0;
                        TotalParejas = 0;
                        CuantasUnion = 0;
                        ContactoNombre = string.Empty;
                        ContactoParentesco = -1;
                        ContactoTelefono = new Nullable<long>();
                        TextContactoTelefono = string.Empty;
                        Social = -1;
                        Economico = -1;
                        Cultural = -1;
                        Hijos = 0;
                        Registrados = 0;
                        CuantosMantieneRelacion = 0;
                        CuantosVisitan = 0;
                        AbandonoFamiliar = string.Empty;
                        EspecifiqueAbandonoFamiliar = -1;
                        MaltratoEmocional = string.Empty;
                        EspecifiqueMaltratoEmocional = string.Empty;
                        MaltratoFisico = string.Empty;
                        EspecifiqueMaltratoFisico = string.Empty;
                        AbusoSexual = string.Empty;
                        EspecifiqueAbusoSexual = string.Empty;
                        EdadAbuso = 0;
                        EdadInicioContactoSexual = 0;
                        HuidasHogar = -1;
                    }
                    #endregion
                    #region Datos Grupo Familiar
                    PopulateDatosGrupoFamiliar();
                    #endregion
                    #region Grupo Familiar Delito
                    PopulateGFD();
                    PopulateGFDroga();
                    #endregion

                    #endregion
                    OnPropertyChanged("VivePadre");
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer tab factores sociales familiares", ex);
            }
        }
        /// <summary>
        /// Llena con información los controles del tab Conductas Parasociales.
        /// </summary>
        /// <param name="emiActual"></param>
        private void PopularConductasParasociales(EMI emiActual)
        {
            try
            {
                if (emiActual != null)
                {
                    #region Conductas Parasociales
                    #region Uso de drogas
                    #region Instanciación de prueba
                    /*if (emiActual.EMI_USO_DROGA != null)
                {
                    var lstDrogas = new SSP.Controlador.Catalogo.Justicia.cDrogas().ObtenerTodos();
                    emiActual.EMI_USO_DROGA.Add(new EMI_USO_DROGA { ID_DROGA = 1, EDAD_INICIO = 19, CONSUMO_ACTUAL = "S", FEC_ULTIMA_DOSIS = DateTime.Now, FRECUENCIA_USO = 1, DROGA = new DROGA { ID_DROGA=1, DESCR="METANFETAMINAS"} });
                    LstUsoDrogas = new ObservableCollection<EMI_USO_DROGA>(emiActual.EMI_USO_DROGA);
                }
                else
                {
                    LstUsoDrogas = emiActual.EMI_USO_DROGA;
                }*/
                    #endregion
                    ControlTab = 8;
                    LstUsoDrogas = new ObservableCollection<EMI_USO_DROGA>(new cEMIUsoDroga().Obtener(emiActual.ID_EMI));
                    if (LstUsoDrogas == null || LstUsoDrogas.Count <= 0)
                        IsUsoDrogasEmpty = true;
                    #endregion
                    #region Homosexualidad, pandillas, sexualidad
                    #region Instanciacion de prueba
                    /*emiActual.EMI_HPS = new EMI_HPS();
                emiActual.EMI_HPS.MOTIVO_CICATRICES = "no compraba los tacos para su padre.";
                emiActual.EMI_HPS.PAGA_SEXUAL_MUJER = "S";
                emiActual.EMI_HPS.PAGA_SEXUAL_HOMBRE = "N";
                emiActual.EMI_HPS.VIVIO_CALLE_ORFANATO_NINO = "O";
                emiActual.EMI_HPS.EDAD_INICIO_CICATRICES = 12;
                emiActual.EMI_HPS.CICATRIZ_POR_RINA = "S";
                emiActual.EMI_HPS.PERTENECE_PANDILLA_ACTUAL = "S";
                emiActual.EMI_HPS.COMPORTAMIENTO_HOMOSEXUAL = "S";
                emiActual.EMI_HPS.EDAD_INICIAL_HOMOSEXUAL = 8;
                emiActual.EMI_HPS.ROL_HOMOSEXUAL = "P";
                emiActual.EMI_HPS.DESERCION_ESCOLAR = "S";
                emiActual.EMI_HPS.EXPULSION_ESCOLAR = "S";
                emiActual.EMI_HPS.MOTIVO_EXPULSION_ESCOLAR = "Golpeaba niños.";
                emiActual.EMI_HPS.ID_PANDILLA = 1;*/
                    #endregion
                    if (emiActual.EMI_HPS != null)
                    {
                        ControlTab = 9;
                        VivioCalleOrfanato = !string.IsNullOrEmpty(emiActual.EMI_HPS.VIVIO_CALLE_ORFANATO_NINO) ? emiActual.EMI_HPS.VIVIO_CALLE_ORFANATO_NINO : string.Empty;
                        #region Homosexual
                        Homosexual = !string.IsNullOrEmpty(emiActual.EMI_HPS.COMPORTAMIENTO_HOMOSEXUAL) ? emiActual.EMI_HPS.COMPORTAMIENTO_HOMOSEXUAL : string.Empty;
                        HomosexualEdadIncial = emiActual.EMI_HPS.EDAD_INICIAL_HOMOSEXUAL != null ? emiActual.EMI_HPS.EDAD_INICIAL_HOMOSEXUAL : 0;
                        HomosexualRol = !string.IsNullOrEmpty(emiActual.EMI_HPS.ROL_HOMOSEXUAL) ? emiActual.EMI_HPS.ROL_HOMOSEXUAL : string.Empty;
                        #endregion
                        #region pandilleo
                        PertenecePandilla = !string.IsNullOrEmpty(emiActual.EMI_HPS.PERTENECE_PANDILLA_ACTUAL) ? emiActual.EMI_HPS.PERTENECE_PANDILLA_ACTUAL : string.Empty;
                        PertenecioPandillaExterior = !string.IsNullOrEmpty(emiActual.EMI_HPS.PERTENECIO_PANDILAS_EXTERIOR) ? emiActual.EMI_HPS.PERTENECIO_PANDILAS_EXTERIOR : string.Empty;
                        PandillaExteriorEdadInicial = emiActual.EMI_HPS.EDAD_INICIAL_PANDILLAS != null ? emiActual.EMI_HPS.EDAD_INICIAL_PANDILLAS : 0;
                        PandillaExteriorMotivo = emiActual.EMI_HPS.MOTIVOS_PERTENENCIA_PANDILLAS;
                        PandillaNombre = emiActual.EMI_HPS.ID_PANDILLA != null ? emiActual.EMI_HPS.ID_PANDILLA : -1;
                        NombrePandilla = emiActual.EMI_HPS.PANDILLA ?? string.Empty;
                        #endregion
                        #region Vagancia
                        Vagancia = !string.IsNullOrEmpty(emiActual.EMI_HPS.VAGANCIA) ? emiActual.EMI_HPS.VAGANCIA : string.Empty;
                        VaganciaEdadIncial = emiActual.EMI_HPS.EDAD_INICIAL_VAGANCIA != null ? emiActual.EMI_HPS.EDAD_INICIAL_VAGANCIA : 0;
                        VaganciaMotivos = emiActual.EMI_HPS.MOTIVOS_VAGANCIA;
                        #endregion
                        #region Cicatrices
                        Cicatrices = !string.IsNullOrEmpty(emiActual.EMI_HPS.CICATRICES) ? emiActual.EMI_HPS.CICATRICES : string.Empty;
                        CicatricesEdadIncial = emiActual.EMI_HPS.EDAD_INICIO_CICATRICES != null ? emiActual.EMI_HPS.EDAD_INICIO_CICATRICES : 0;
                        CicatricesMotivo = emiActual.EMI_HPS.MOTIVO_CICATRICES;
                        CicatricesRina = string.IsNullOrEmpty(emiActual.EMI_HPS.CICATRIZ_POR_RINA) ? false : emiActual.EMI_HPS.CICATRIZ_POR_RINA == "S" ? true : false;
                        #endregion
                        #region Desercion Escolar
                        DesercionEscolar = !string.IsNullOrEmpty(emiActual.EMI_HPS.DESERCION_ESCOLAR) ? emiActual.EMI_HPS.DESERCION_ESCOLAR : string.Empty;
                        DesercionMotivo = emiActual.EMI_HPS.MOTIVO_DESERCION_ESCOLAR;
                        #endregion
                        #region Reprobación escolar
                        ReprobacionEscolar = !string.IsNullOrEmpty(emiActual.EMI_HPS.REPROBACION_ESCOLAR) ? emiActual.EMI_HPS.REPROBACION_ESCOLAR : string.Empty;
                        ReprobacionEscolarMotivo = emiActual.EMI_HPS.MOTIVO_REPROBACION_ESCOLAR;
                        ReprobacionGrado = emiActual.EMI_HPS.GRADO_REPROBACION_ESCOLAR != null ? emiActual.EMI_HPS.GRADO_REPROBACION_ESCOLAR : -1;
                        #endregion
                        #region Expulsion escolar
                        ExplusionEscolar = !string.IsNullOrEmpty(emiActual.EMI_HPS.EXPULSION_ESCOLAR) ? emiActual.EMI_HPS.EXPULSION_ESCOLAR : string.Empty;
                        ExplusionEscolarMotivo = emiActual.EMI_HPS.MOTIVO_EXPULSION_ESCOLAR;
                        ExpulsionGrado = emiActual.EMI_HPS.GRADO_EXPULSION_ESCOLAR != null ? emiActual.EMI_HPS.GRADO_EXPULSION_ESCOLAR : -1;
                        #endregion
                        #region Paga x servicio Sexual
                        ConHombres = string.IsNullOrEmpty(emiActual.EMI_HPS.PAGA_SEXUAL_HOMBRE) ? false : emiActual.EMI_HPS.PAGA_SEXUAL_HOMBRE == "S" ? true : false;
                        ConMujeres = string.IsNullOrEmpty(emiActual.EMI_HPS.PAGA_SEXUAL_MUJER) ? false : emiActual.EMI_HPS.PAGA_SEXUAL_MUJER == "S" ? true : false;
                        #endregion
                        #region se prostituia
                        SeProstituiaHombres = string.IsNullOrEmpty(emiActual.EMI_HPS.PROSTITUIA_HOMBRES) ? false : emiActual.EMI_HPS.PROSTITUIA_HOMBRES == "S" ? true : false;
                        SeProstituiaMujeres = string.IsNullOrEmpty(emiActual.EMI_HPS.PROSTITUIA_MUJERES) ? false : emiActual.EMI_HPS.PROSTITUIA_MUJERES == "S" ? true : false;
                        MotivoProstituye = emiActual.EMI_HPS.PROSTITUYE_POR != null ? emiActual.EMI_HPS.PROSTITUYE_POR : -1;

                        //LstMotivosProstituye = new ObservableCollection<MOTIVO_PROSTITUCION>(lst);
                        #endregion
                    }
                    else
                    {
                        VivioCalleOrfanato = string.Empty;
                        #region Homosexual
                        Homosexual = string.Empty;
                        HomosexualEdadIncial = 0;
                        HomosexualRol = string.Empty;
                        #endregion
                        #region pandilleo
                        PertenecePandilla = string.Empty;
                        PertenecioPandillaExterior = string.Empty;
                        PandillaExteriorEdadInicial = 0;
                        PandillaExteriorMotivo = string.Empty;
                        PandillaNombre = -1;
                        NombrePandilla = string.Empty;
                        #endregion
                        #region Vagancia
                        Vagancia = string.Empty;
                        VaganciaEdadIncial = 0;
                        VaganciaMotivos = string.Empty;
                        #endregion
                        #region Cicatrices
                        Cicatrices = string.Empty;
                        CicatricesEdadIncial = 0;
                        CicatricesMotivo = string.Empty;
                        CicatricesRina = false;
                        #endregion
                        #region Desercion Escolar
                        DesercionEscolar = string.Empty;
                        DesercionMotivo = string.Empty;
                        #endregion
                        #region Reprobación escolar
                        ReprobacionEscolar = string.Empty;
                        ReprobacionEscolarMotivo = string.Empty;
                        ReprobacionGrado = -1;
                        #endregion
                        #region Expulsion escolar
                        ExplusionEscolar = string.Empty;
                        ExplusionEscolarMotivo = string.Empty;
                        ExpulsionGrado = -1;
                        #endregion
                        #region Paga x servicio Sexual
                        ConHombres = false;
                        ConMujeres = false;
                        #endregion
                        #region se prostituia
                        SeProstituiaHombres = false;
                        SeProstituiaMujeres = false;
                        MotivoProstituye = -1;

                        //LstMotivosProstituye = new ObservableCollection<MOTIVO_PROSTITUCION>(lst);
                        #endregion
                    }
                    #endregion
                    #region Tatuajes
                    #region Instanciación de prueba
                    //if (emiActual.EMI_TATUAJE == null)
                    //{
                    //    emiActual.EMI_TATUAJE = new EMI_TATUAJE();
                    //    emiActual.EMI_TATUAJE.IDENTIFICACION_AI = 5;
                    //    emiActual.EMI_TATUAJE.RELIGIOSO_AI = 13;
                    //    emiActual.EMI_TATUAJE.TOTAL_TATUAJES = 18;
                    //    emiActual.EMI_TATUAJE.DESCR = "En la parte de la nuca una L y una A y abajo en el pescuezo el nombre de su mama tenia una cruz pintada en uno de sus brazos con el nombre de un amigo al que mataron abalazos luego trae otro tatuaje que son dos caras pintadas una de ellas esta llorando y la otra puras carcajadas ";
                    //}
                    #endregion
                    if (emiActual.EMI_TATUAJE != null)
                    {
                        #region cantidad de tatuajes
                        CantidadAntesIngresoAntisocial = emiActual.EMI_TATUAJE.ANTISOCIAL_AI;
                        CantidadIntramurosAntisocial = emiActual.EMI_TATUAJE.ANTISOCIAL_I;

                        CantidadAntesIngresoErotico = emiActual.EMI_TATUAJE.EROTICO_AI;
                        CantidadIntramurosErotico = emiActual.EMI_TATUAJE.EROTICO_I;

                        CantidadAntesIngresoReligioso = emiActual.EMI_TATUAJE.RELIGIOSO_AI;
                        CantidadIntramurosReligioso = emiActual.EMI_TATUAJE.RELIGIOSO_I;

                        CantidadAntesIngresoIdentificacion = emiActual.EMI_TATUAJE.IDENTIFICACION_AI;
                        CantidadIntramurosIdentificacion = emiActual.EMI_TATUAJE.IDENTIFICACION_I;

                        CantidadAntesIngresoDecorativo = emiActual.EMI_TATUAJE.DECORATIVO_AI;
                        CantidadIntramurosDecorativo = emiActual.EMI_TATUAJE.DECORATIVO_I;

                        CantidadAntesIngresoSentimental = emiActual.EMI_TATUAJE.SENTIMENTAL_AI;
                        CantidadIntramurosSentimental = emiActual.EMI_TATUAJE.SENTIMENTAL_I;

                        TatuajesDescripcion = emiActual.EMI_TATUAJE.DESCR;
                        TatuajesTotal = emiActual.EMI_TATUAJE.TOTAL_TATUAJES;
                        #endregion
                    }
                    #endregion
                    #region Enfermedades
                    #region Instanciacion de prueba
                    //if (emiActual.EMI_ENFERMEDAD == null)
                    //{
                    //    emiActual.EMI_ENFERMEDAD = new EMI_ENFERMEDAD();
                    //    emiActual.EMI_ENFERMEDAD.DESCR_ENFERMEDAD = "Multiples tumores en la cara, espalda y pecho que causan deformidad muy notable y una apariencia similar a la de Cuahutemoc Blanco.";
                    //    emiActual.EMI_ENFERMEDAD.APFISICA_ALINADO = "N";
                    //    emiActual.EMI_ENFERMEDAD.APFISICA_CONFORMADO = "N";
                    //    emiActual.EMI_ENFERMEDAD.APFISICA_INTEGRO = "S";
                    //    emiActual.EMI_ENFERMEDAD.APFISICA_LIMPIO = "S";
                    //    emiActual.EMI_ENFERMEDAD.DISCAPACIDAD = "S";
                    //    emiActual.EMI_ENFERMEDAD.DESCR_DISCAPACIDAD = "Tumores dolorosos.";
                    //    emiActual.EMI_ENFERMEDAD.ENFERMO_MENTAL = "N";
                    //    emiActual.EMI_ENFERMEDAD.DESCR_ENFERMO_MENTAL = "N/A";
                    //    emiActual.EMI_ENFERMEDAD.VIH_HEPATITIS = "N";
                    //    emiActual.EMI_ENFERMEDAD.EN_TRATAMIENTO_FARMACO = "N/A";
                    //    emiActual.EMI_ENFERMEDAD.DIAGNOSTICO_FORMAL = "N/A";

                    //}

                    #endregion
                    if (emiActual.EMI_ENFERMEDAD != null)
                    {
                        DescripcionPresentarlasAntecedentes = emiActual.EMI_ENFERMEDAD.DESCR_ENFERMEDAD;
                        AparienciaFisicaAlineado = emiActual.EMI_ENFERMEDAD.APFISICA_ALINADO;
                        AparienciaFisicaConformado = emiActual.EMI_ENFERMEDAD.APFISICA_CONFORMADO;
                        AparienciaFisicaIntegro = emiActual.EMI_ENFERMEDAD.APFISICA_INTEGRO;
                        AparienciaFisicaLimpio = emiActual.EMI_ENFERMEDAD.APFISICA_LIMPIO;
                        Discapacidades = emiActual.EMI_ENFERMEDAD.DISCAPACIDAD;
                        DiscapacidadesMotivo = emiActual.EMI_ENFERMEDAD.DESCR_DISCAPACIDAD;
                        EnfermoMental = emiActual.EMI_ENFERMEDAD.ENFERMO_MENTAL;
                        EnfermoMentalMotivo = emiActual.EMI_ENFERMEDAD.DESCR_ENFERMO_MENTAL;
                        VIHHepatitis = emiActual.EMI_ENFERMEDAD.VIH_HEPATITIS;
                        VIHHepatitisTratamientoFarmaco =  !string.IsNullOrEmpty(emiActual.EMI_ENFERMEDAD.EN_TRATAMIENTO_FARMACO) ? emiActual.EMI_ENFERMEDAD.EN_TRATAMIENTO_FARMACO : "";
                        VIHHepatitisDiagnosticoFormal = !string.IsNullOrEmpty(emiActual.EMI_ENFERMEDAD.DIAGNOSTICO_FORMAL) ? emiActual.EMI_ENFERMEDAD.DIAGNOSTICO_FORMAL : "";
                    }
                    else
                    {
                        DescripcionPresentarlasAntecedentes = string.Empty;
                        AparienciaFisicaAlineado = string.Empty;
                        AparienciaFisicaConformado = string.Empty;
                        AparienciaFisicaIntegro = string.Empty;
                        AparienciaFisicaLimpio = string.Empty;
                        Discapacidades = string.Empty;
                        DiscapacidadesMotivo = string.Empty;
                        EnfermoMental = string.Empty;
                        EnfermoMentalMotivo = string.Empty;
                        VIHHepatitisTratamientoFarmaco = string.Empty;
                        VIHHepatitisDiagnosticoFormal = string.Empty;
                        VIHHepatitis = string.Empty;
                    }
                    #endregion
                    #region Grupo Familiar Droga
                    PopulateGFDroga();
                    #endregion
                    OnPropertyChanged();
                    #endregion
                }
                else
                {
                    //Los elementos que queden vacíos...
                    IsUsoDrogasEmpty = true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer conductas parasociales", ex);
            }
        }
        //Clasificacion Criminológica

        #endregion
        //ULTIMOS EMPLEOS
        #region Ultimos Empleos
        private void PopulateOcupacion()
        {
            try
            {
                if (LstOcupacion == null)
                {
                    LstOcupacion = new ObservableCollection<OCUPACION>(new cOcupacion().ObtenerTodos());
                    LstOcupacion.Insert(0, (new OCUPACION() { ID_OCUPACION = -1, DESCR = "SELECCIONE" }));
                }
                SelectedOcupacion = LstOcupacion.Where(w => w.ID_OCUPACION == -1).FirstOrDefault();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer ocupación", ex);
            }
        }

        private void PopulateUltimosEmpleos2()
        {
            try
            {
                LstUltimosEmpleos = new ObservableCollection<EMI_ULTIMOS_EMPLEOS>(emiActual.EMI_ULTIMOS_EMPLEOS);
                IsEmpleosEmpty = true;
                if (LstUltimosEmpleos != null)
                    if (LstUltimosEmpleos.Count > 0)
                        IsEmpleosEmpty = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer últimos empleos", ex);
            }
        }
        private void PopulateUltimosEmpleosModal()
        {
            try
            {
                if (SelectedEmpleo != null)
                {
                    SelectedOcupacion = LstOcupacion.Where(w => w.ID_OCUPACION == SelectedEmpleo.ID_OCUPACION).FirstOrDefault();
                    Duracion = SelectedEmpleo.DURACION;
                    Empresa = SelectedEmpleo.EMPRESA;
                    MotivoDesempleo = SelectedEmpleo.MOTIVO_DESEMPLEO;
                    EmpleoFormal = SelectedEmpleo.EMPLEO_FORMAL.Equals("S") ? true : false;
                    UltimoAntesDetencion = SelectedEmpleo.ULTIMO_EMPLEO_ANTES_DETENCION.Equals("S") ? true : false;
                    InestabilidadLaboral = SelectedEmpleo.INESTABILIDAD_LABORAL.Equals("S") ? true : false;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer últimos empleos", ex);
            }
        }
        private void AgregarUltimosEmpleos()
        {
            try
            {
                if (!base.HasErrors)
                {
                    if (LstUltimosEmpleos == null)
                        LstUltimosEmpleos = new ObservableCollection<EMI_ULTIMOS_EMPLEOS>();
                    if (SelectedEmpleo == null)//AGREGAMOS
                    {
                        LstUltimosEmpleos.Add(new EMI_ULTIMOS_EMPLEOS()
                        {
                            ID_OCUPACION = selectedOcupacion.ID_OCUPACION,
                            DURACION = Duracion,
                            EMPRESA = Empresa,
                            MOTIVO_DESEMPLEO = MotivoDesempleo,
                            EMPLEO_FORMAL = EmpleoFormal == true ? "S" : "N",
                            ULTIMO_EMPLEO_ANTES_DETENCION = UltimoAntesDetencion == true ? "S" : "N",
                            INESTABILIDAD_LABORAL = InestabilidadLaboral == true ? "S" : "N",
                            OCUPACION = SelectedOcupacion
                        });
                    }
                    else//EDITAMOS LOS ULTIMOS 
                    {
                        SelectedEmpleo.ID_OCUPACION = selectedOcupacion.ID_OCUPACION;
                        SelectedEmpleo.DURACION = Duracion;
                        SelectedEmpleo.EMPRESA = Empresa;
                        SelectedEmpleo.MOTIVO_DESEMPLEO = MotivoDesempleo;
                        SelectedEmpleo.EMPLEO_FORMAL = EmpleoFormal == true ? "S" : "N";
                        SelectedEmpleo.ULTIMO_EMPLEO_ANTES_DETENCION = UltimoAntesDetencion == true ? "S" : "N";
                        SelectedEmpleo.INESTABILIDAD_LABORAL = InestabilidadLaboral == true ? "S" : "N";
                        SelectedEmpleo.OCUPACION = SelectedOcupacion;
                        LstUltimosEmpleos = new ObservableCollection<EMI_ULTIMOS_EMPLEOS>(LstUltimosEmpleos);
                    }
                    SelectedOcupacion = null;
                    Duracion = Empresa = MotivoDesempleo = string.Empty;
                    EmpleoFormal = UltimoAntesDetencion = InestabilidadLaboral = IsEmpleosEmpty = false;
                    setValidacionesFichaIdentificacion();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.ULTIMOS_EMPLEOS);
                }
                else
                {
                    new Dialogos().NotificacionDialog("Notificación", "Faltan Campos por capturar");
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar últimos empleos", ex);
            }
        }
        private void LimpiarUltimosEmpleos()
        {
            try
            {
                SelectedOcupacion = null;
                Duracion = Empresa = MotivoDesempleo = string.Empty;
                EmpleoFormal = UltimoAntesDetencion = InestabilidadLaboral = false;
                SelectedEmpleo = null;
                //EmpleosAnterioresEnabled = false;
                //LstUltimosEmpleos = null;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar últimos empleos", ex);
            }
        }
        private bool GuardarUltimosEmpleos()
        {
            try
            {
                if (emiActual != null)
                {
                    if (emiActual.ID_EMI != 0)
                    {
                        var ue = new List<EMI_ULTIMOS_EMPLEOS>();
                        if (LstUltimosEmpleos != null)
                        {
                            short index = 1;
                            foreach (var empleo in LstUltimosEmpleos)
                            {
                                ue.Add(new EMI_ULTIMOS_EMPLEOS()
                                {
                                    ID_EMI = emiActual.ID_EMI,
                                    ID_EMI_CONS = emiActual.ID_EMI_CONS,
                                    ID_EMI_ULTIMOS_EMPLEOS = index,
                                    ID_OCUPACION = empleo.ID_OCUPACION,
                                    DURACION = empleo.DURACION,
                                    EMPRESA = empleo.EMPRESA,
                                    MOTIVO_DESEMPLEO = empleo.MOTIVO_DESEMPLEO,
                                    EMPLEO_FORMAL = empleo.EMPLEO_FORMAL,
                                    ULTIMO_EMPLEO_ANTES_DETENCION = empleo.ULTIMO_EMPLEO_ANTES_DETENCION,
                                    INESTABILIDAD_LABORAL = empleo.INESTABILIDAD_LABORAL
                                });
                                index++;
                            }
                        }
                        if (new cEMIUltimosEmpleos().Agregar(emiActual.ID_EMI, emiActual.ID_EMI_CONS, ue))
                        {
                            LstUltimosEmpleos = new ObservableCollection<EMI_ULTIMOS_EMPLEOS>(new cEMIUltimosEmpleos().Obtener(emiActual.ID_EMI));
                            return true;
                        }
                        //Mensaje(resultado, Name);
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar últimos empleos", ex);
            }
            return false;
        }
        private void EliminarUltimoEmpleo()
        {
            try
            {
                if (LstUltimosEmpleos != null)
                {
                    if (SelectedEmpleo != null)
                        LstUltimosEmpleos.Remove(SelectedEmpleo);
                    SelectedEmpleo = null;
                    if (LstUltimosEmpleos.Count > 0)
                        IsEmpleosEmpty = false;
                    else
                        IsEmpleosEmpty = true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar último empleo", ex);
            }
        }
        #endregion
        //GRUPO FAMILIAR
        #region Grupo Familiar
        private void PopulateDatosGrupoFamiliarCB()
        {
            try
            {
                if (LstTipoReferencia == null)
                {
                    LstTipoReferencia = new ObservableCollection<TIPO_REFERENCIA>(new cTipoReferencia().ObtenerTodos());
                    LstTipoReferencia.Add(new TIPO_REFERENCIA() { ID_TIPO_REFERENCIA = -1, DESCR = "SELECCIONE" });
                }
                if (LstOcupacion == null)
                {
                    LstOcupacion = new ObservableCollection<OCUPACION>(new cOcupacion().ObtenerTodos());
                    LstOcupacion.Add(new OCUPACION() { ID_OCUPACION = -1, DESCR = "SELECCIONE" });
                }
                if (LstEstadoCivil == null)
                {
                    LstEstadoCivil = new ObservableCollection<ESTADO_CIVIL>(new cEstadoCivil().ObtenerTodos());
                    LstEstadoCivil.Add(new ESTADO_CIVIL() { ID_ESTADO_CIVIL = -1, DESCR = "SELECCIONE" });
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer combobox datos grupo familiar", ex);
            }
        }

        private void PopulateDatosGrupoFamiliarModal()
        {
            try
            {
                if (SelectedGrupoFamiliar != null)
                {
                    GrupoFamiliar = SelectedGrupoFamiliar.GRUPO;
                    NombreGrupoFamiliar = SelectedGrupoFamiliar.NOMBRE;
                    PaternoGrupoFamiliar = SelectedGrupoFamiliar.PATERNO;
                    MaternoGrupoFamiliar = SelectedGrupoFamiliar.MATERNO;
                    FechaNacGrupoFamiliar = SelectedGrupoFamiliar.NACIMIENTO_FEC;
                    SelectedRelacionGrupoFamiliar = LstTipoReferencia.Where(w => w.ID_TIPO_REFERENCIA == SelectedGrupoFamiliar.ID_RELACION).FirstOrDefault();
                    DomicilioGrupoFamiliar = SelectedGrupoFamiliar.DOMICILIO;
                    SelectedOcupacionGrupoFamiliar = LstOcupacion.Where(w => w.ID_OCUPACION == SelectedGrupoFamiliar.ID_OCUPACION).FirstOrDefault();
                    SelectedEdoCivilGrupoFamiliar = LstEstadoCivil.Where(w => w.ID_ESTADO_CIVIL == SelectedGrupoFamiliar.ID_ESTADO_CIVIL).FirstOrDefault();
                    ViveConElGrupoFamiliar = SelectedGrupoFamiliar.VIVE_C_EL.Equals("S") ? true : false;
                    EdadGrupoFamiliar = SelectedGrupoFamiliar.EDAD ?? SelectedGrupoFamiliar.EDAD.Value;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer datos grupo familiar", ex);
            }
        }
        private void AgregarDatosGrupoFamiliar()
        {
            try
            {
                if (LstGrupoFamiliar == null)
                    LstGrupoFamiliar = new ObservableCollection<EMI_GRUPO_FAMILIAR>();
                if (!base.HasErrors)
                {
                    if (SelectedGrupoFamiliar == null)//AGREGAMOS
                    {
                        LstGrupoFamiliar.Add(new EMI_GRUPO_FAMILIAR()
                        {
                            GRUPO = GrupoFamiliar,
                            NOMBRE = NombreGrupoFamiliar,
                            PATERNO = PaternoGrupoFamiliar,
                            MATERNO = MaternoGrupoFamiliar,
                            EDAD = EdadGrupoFamiliar,
                            NACIMIENTO_FEC = FechaNacGrupoFamiliar,
                            ID_RELACION = SelectedRelacionGrupoFamiliar.ID_TIPO_REFERENCIA,
                            DOMICILIO = DomicilioGrupoFamiliar,
                            ID_OCUPACION = SelectedOcupacionGrupoFamiliar.ID_OCUPACION,
                            ID_ESTADO_CIVIL = SelectedEdoCivilGrupoFamiliar.ID_ESTADO_CIVIL,
                            VIVE_C_EL = ViveConElGrupoFamiliar == true ? "S" : "N",
                            TIPO_REFERENCIA = SelectedRelacionGrupoFamiliar,
                            OCUPACION = SelectedOcupacionGrupoFamiliar,
                            ESTADO_CIVIL = SelectedEdoCivilGrupoFamiliar,
                            ID_EMI = emiActual.ID_EMI,
                            ID_EMI_CONS = emiActual.ID_EMI_CONS,
                            ID_CONS = LstGrupoFamiliar.LastOrDefault() != null ? (short)(LstGrupoFamiliar.LastOrDefault().ID_CONS + 1) : (short)1
                        });
                        if (LstGrupoFamiliar != null)
                            if (LstGrupoFamiliar.Count > 0)
                                IsGrupoFamiliarEmpty = false;
                    }
                    else//EDITAMOS 
                    {
                        SelectedGrupoFamiliar.GRUPO = GrupoFamiliar;
                        SelectedGrupoFamiliar.NOMBRE = NombreGrupoFamiliar;
                        SelectedGrupoFamiliar.PATERNO = PaternoGrupoFamiliar;
                        SelectedGrupoFamiliar.MATERNO = MaternoGrupoFamiliar;
                        SelectedGrupoFamiliar.EDAD = EdadGrupoFamiliar;
                        SelectedGrupoFamiliar.NACIMIENTO_FEC = FechaNacGrupoFamiliar;
                        SelectedGrupoFamiliar.ID_RELACION = SelectedRelacionGrupoFamiliar.ID_TIPO_REFERENCIA;
                        SelectedGrupoFamiliar.DOMICILIO = DomicilioGrupoFamiliar;
                        SelectedGrupoFamiliar.ID_OCUPACION = SelectedOcupacionGrupoFamiliar.ID_OCUPACION;
                        SelectedGrupoFamiliar.ID_ESTADO_CIVIL = SelectedEdoCivilGrupoFamiliar.ID_ESTADO_CIVIL;
                        SelectedGrupoFamiliar.VIVE_C_EL = ViveConElGrupoFamiliar == true ? "S" : "N";
                        SelectedGrupoFamiliar.TIPO_REFERENCIA = SelectedRelacionGrupoFamiliar;
                        SelectedGrupoFamiliar.OCUPACION = SelectedOcupacionGrupoFamiliar;
                        SelectedGrupoFamiliar.ESTADO_CIVIL = SelectedEdoCivilGrupoFamiliar;

                    }
                    LstGrupoFamiliar = new ObservableCollection<EMI_GRUPO_FAMILIAR>(LstGrupoFamiliar);
                    LimpiarDatosGrupoFamiliar();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.DATOS_GRUPO_FAMILIAR);
                }
                //else
                //    MensajeDialogo("", false);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar datos grupo familiar", ex);
            }
        }
        private void LimpiarDatosGrupoFamiliar()
        {
            try
            {
                SelectedGrupoFamiliar = null;
                GrupoFamiliar = EdadGrupoFamiliar = null;
                NombreGrupoFamiliar = PaternoGrupoFamiliar = MaternoGrupoFamiliar = DomicilioGrupoFamiliar = string.Empty;
                SelectedRelacionGrupoFamiliar = null;
                SelectedEdoCivilGrupoFamiliar = null;
                SelectedEdoCivilGrupoFamiliar = null;
                ViveConElGrupoFamiliar = null;
                FechaNacGrupoFamiliar = null;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar datos grupo familiar", ex);
            }
        }
        
        private void GuardarDatosGrupoFamiliar()
        {
            try
            {
                if (emiActual != null)
                {
                    var Name = "Datos Grupo Familiar";
                    #region Comentado
                    //var gf = new List<EMI_GRUPO_FAMILIAR>();
                    //if (LstGrupoFamiliar != null)
                    //{
                    //    foreach (var grupo in LstGrupoFamiliar)
                    //    {
                    //        gf.Add(new EMI_GRUPO_FAMILIAR()
                    //        {
                    //            ID_EMI = emiActual.ID_EMI,
                    //            ID_EMI_CONS = emiActual.ID_EMI_CONS,
                    //            ID_CONS = grupo.ID_CONS,
                    //            GRUPO = grupo.GRUPO,
                    //            NOMBRE = grupo.NOMBRE,
                    //            PATERNO = grupo.PATERNO,
                    //            MATERNO = grupo.MATERNO,
                    //            EDAD = grupo.EDAD,
                    //            NACIMIENTO_FEC = grupo.NACIMIENTO_FEC,
                    //            ID_RELACION = grupo.ID_RELACION,
                    //            DOMICILIO = grupo.DOMICILIO,
                    //            ID_OCUPACION = grupo.ID_OCUPACION,
                    //            ID_ESTADO_CIVIL = grupo.ID_ESTADO_CIVIL,
                    //            VIVE_C_EL = grupo.VIVE_C_EL,
                    //            ID_CENTRO = grupo.ID_CENTRO,
                    //            ID_ANIO = grupo.ID_ANIO,
                    //            ID_IMPUTADO = grupo.ID_IMPUTADO,
                    //            ID_INGRESO = grupo.ID_INGRESO,
                    //            ID_VISITA = grupo.ID_VISITA
                    //        });
                    //    }
                    //}
                    #endregion
                    var grupo = new List<EMI_GRUPO_FAMILIAR>(LstGrupoFamiliar == null ? null : LstGrupoFamiliar.Select(w => new EMI_GRUPO_FAMILIAR()
                    {
                        ID_EMI = emiActual.ID_EMI,
                        ID_EMI_CONS = emiActual.ID_EMI_CONS,
                        ID_CONS = w.ID_CONS,
                        GRUPO = w.GRUPO,
                        NOMBRE = w.NOMBRE,
                        PATERNO = w.PATERNO,
                        MATERNO = w.MATERNO,
                        EDAD = w.EDAD,
                        NACIMIENTO_FEC = w.NACIMIENTO_FEC,
                        ID_RELACION = w.ID_RELACION,
                        DOMICILIO = w.DOMICILIO,
                        ID_OCUPACION = w.ID_OCUPACION,
                        ID_ESTADO_CIVIL = w.ID_ESTADO_CIVIL,
                        VIVE_C_EL = w.VIVE_C_EL,
                        ID_CENTRO = w.ID_CENTRO,
                        ID_ANIO = w.ID_ANIO,
                        ID_IMPUTADO = w.ID_IMPUTADO,
                        ID_INGRESO = w.ID_INGRESO,
                        ID_VISITA = w.ID_VISITA
                    }));

                    if (emiActual.EMI_GRUPO_FAMILIAR.Count > 0)
                    {
                        if (!PEditar)
                        {
                            StaticSourcesViewModel.Mensaje("Validación", "Su usuario no cuenta con permisos para realizar esta acción.", StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA, 5);
                            return;
                        }
                    }
                    else
                    {
                        if (!PInsertar)
                        {
                            StaticSourcesViewModel.Mensaje("Validación", "Su usuario no cuenta con permisos para realizar esta acción.", StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA, 5);
                            return;
                        }
                    }
                    if (new cEMIGrupoFamiliar().Insertar(emiActual.ID_EMI, emiActual.ID_EMI_CONS, grupo))
                    {
                        LstGrupoFamiliar = new ObservableCollection<EMI_GRUPO_FAMILIAR>(new cEMIGrupoFamiliar().ObtenerTodos(emiActual.ID_EMI, emiActual.ID_EMI_CONS));
                        Mensaje(true, Name);
                    }
                    else
                        Mensaje(false, Name);
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar datos grupo familiar", ex);
            }
        }
        private void EliminarGrupoFamiliar()
        {
            try
            {
                if (LstGrupoFamiliar != null)
                {
                    if (SelectedGrupoFamiliar != null)
                        LstGrupoFamiliar.Remove(SelectedGrupoFamiliar);
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar grupo familiar", ex);
            }
        }

        //GRUPO FAMILIAR DELITO
        private void PopulateGFDCB()
        {
            try
            {
                if (LstTipoReferencia == null)
                {
                    LstTipoReferencia = new ObservableCollection<TIPO_REFERENCIA>(new cTipoReferencia().ObtenerTodos());
                    LstTipoReferencia.Add(new TIPO_REFERENCIA() { ID_TIPO_REFERENCIA = -1, DESCR = "SELECCIONE" });
                }
                if (LstDelitos == null)
                {
                    LstDelitos = new ObservableCollection<INGRESO_DELITO>(new cIngresoDelito().ObtenerTodos());
                    LstDelitos.Add(new INGRESO_DELITO() { ID_INGRESO_DELITO = -1, DESCR = "SELECCIONE" });
                }
                if (LstTipoRelacion == null)
                {
                    LstTipoRelacion = new ObservableCollection<TIPO_RELACION>(new cTipoRelacion().ObtenerTodos());
                    LstTipoRelacion.Add(new TIPO_RELACION() { ID_RELACION = -1, DESCR = "SELECCIONE" });
                }
                if (LstEmisor == null)
                {
                    LstEmisor = new ObservableCollection<EMISOR>(new cEmisor().Obtener());
                    LstEmisor.Add(new EMISOR() { ID_EMISOR = -1, DESCR = "SELECCIONE" });
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer combobox grupo familiar delito", ex);
            }
        }
        private void PopulateGFD()
        {
            try
            {
                ControlTab = 7;
                LstFamiliarDelito = new ObservableCollection<EMI_ANTECEDENTE_FAM_CON_DEL>(emiActual.EMI_ANTECEDENTE_FAM_CON_DEL);
                IsEmptyFamiliarDelito = true;
                if (LstFamiliarDelito != null)
                    if (LstFamiliarDelito.Count > 0)
                        IsEmptyFamiliarDelito = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer grupo familiar delito", ex);
            }
        }
        private void PopulateGFDModal()
        {
            try
            {
                if (SelectedFamiliarDelito != null)
                {
                    SelectedParentescoFDel = LstTipoReferencia.Where(w => w.ID_TIPO_REFERENCIA == SelectedFamiliarDelito.ID_PARENTESCO).FirstOrDefault();
                    AnioFDel = SelectedFamiliarDelito.ANIO;
                    DelitoFDel = SelectedFamiliarDelito.DELITO;
                    SelectedEmisorFDel = LstEmisor.Where(w => w.ID_EMISOR == SelectedFamiliarDelito.ID_EMISOR).FirstOrDefault();
                    SelectedDelitoFDel = LstDelitos.Where(w => w.ID_INGRESO_DELITO == SelectedFamiliarDelito.ID_DELITO).FirstOrDefault();
                    SelectedRelacionFDel = LstTipoRelacion.Where(w => w.ID_RELACION == SelectedFamiliarDelito.ID_RELACION).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer grupo familiar delito", ex);
            }
        }
        private void AgregarGFD()
        {
            try
            {
                if (LstFamiliarDelito == null)
                    LstFamiliarDelito = new ObservableCollection<EMI_ANTECEDENTE_FAM_CON_DEL>();
                if (!base.HasErrors)
                {
                    if (SelectedFamiliarDelito == null)//AGREGAMOS
                    {
                        LstFamiliarDelito.Add(new EMI_ANTECEDENTE_FAM_CON_DEL()
                        {
                            ID_PARENTESCO = SelectedParentescoFDel != null ? SelectedParentescoFDel.ID_TIPO_REFERENCIA : new short?(),
                            ANIO = AnioFDel,
                            ID_EMISOR = SelectedEmisorFDel != null ? SelectedEmisorFDel.ID_EMISOR : new int?(),
                            ID_DELITO = SelectedDelitoFDel != null ? SelectedDelitoFDel.ID_INGRESO_DELITO : new short?(),
                            ID_RELACION = SelectedRelacionFDel != null ? SelectedRelacionFDel.ID_RELACION : new short?(),
                            DELITO = DelitoFDel,
                            TIPO_REFERENCIA = SelectedParentescoFDel,
                            EMISOR = selectedEmisorFDel,
                            INGRESO_DELITO = SelectedDelitoFDel,
                            TIPO_RELACION = SelectedRelacionFDel,
                            ID_EMI_ANTECEDENTE = LstFamiliarDelito.LastOrDefault() != null ? short.Parse((LstFamiliarDelito.LastOrDefault().ID_EMI_ANTECEDENTE + (short)1).ToString()) : (short)1
                        });
                    }
                    else//EDITAMOS 
                    {

                        SelectedFamiliarDelito.ID_PARENTESCO = SelectedParentescoFDel != null ? SelectedParentescoFDel.ID_TIPO_REFERENCIA : new short?();
                        SelectedFamiliarDelito.ANIO = AnioFDel;
                        selectedFamiliarDelito.DELITO = DelitoFDel;
                        SelectedFamiliarDelito.ID_EMISOR = SelectedEmisorFDel != null ? SelectedEmisorFDel.ID_EMISOR : new int?();
                        SelectedFamiliarDelito.ID_DELITO = SelectedDelitoFDel != null ? SelectedDelitoFDel.ID_INGRESO_DELITO : new short?();
                        SelectedFamiliarDelito.ID_RELACION = SelectedRelacionFDel != null ? SelectedRelacionFDel.ID_RELACION : new short?();
                        SelectedFamiliarDelito.TIPO_REFERENCIA = SelectedParentescoFDel;
                        SelectedFamiliarDelito.EMISOR = selectedEmisorFDel;
                        SelectedFamiliarDelito.INGRESO_DELITO = SelectedDelitoFDel;
                        SelectedFamiliarDelito.TIPO_RELACION = SelectedRelacionFDel;
                        LstFamiliarDelito = new ObservableCollection<EMI_ANTECEDENTE_FAM_CON_DEL>(LstFamiliarDelito);
                    }

                    if (LstFamiliarDelito != null)
                        if (LstFamiliarDelito.Count > 0)
                            IsEmptyFamiliarDelito = false;
                    LimpiarGFD();
                    setValidacionesSituacionJuridica();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.FAMILIAR_DELITO);
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar grupo familiar delito", ex);
            }
        }
        private void LimpiarGFD()
        {
            try
            {
                SelectedFamiliarDelito = null;
                AnioFDel = null;
                SelectedParentescoFDel = null;
                SelectedEmisorFDel = null;
                SelectedDelitoFDel = null;
                SelectedRelacionFDel = null;
                DelitoFDel = string.Empty;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar grupo familiar delito", ex);
            }
        }
        private void GuardarGFD()
        {
            try
            {
                if (emiActual != null)
                {
                    var Name = "Antecedente de Grupo Familiar";
                    //var fd = new List<EMI_ANTECEDENTE_FAM_CON_DEL>();
                    //if (LstFamiliarDelito != null)
                    //{
                    //    foreach (var f in LstFamiliarDelito)
                    //    {
                    //        fd.Add(new EMI_ANTECEDENTE_FAM_CON_DEL()
                    //        {
                    //            ID_EMI = emiActual.ID_EMI,
                    //            ID_EMI_CONS = emiActual.ID_EMI_CONS,
                    //            ID_EMI_ANTECEDENTE = f.ID_EMI_ANTECEDENTE,
                    //            ID_PARENTESCO = f.ID_PARENTESCO,
                    //            ANIO = f.ANIO,
                    //            ID_DELITO = f.ID_DELITO,
                    //            ID_RELACION = f.ID_RELACION,
                    //            ID_EMISOR = f.ID_EMISOR,
                    //        });
                    //    }
                    //}

                    var delitos = new List<EMI_ANTECEDENTE_FAM_CON_DEL>(LstFamiliarDelito == null ? null : LstFamiliarDelito.Select((w, i) => new EMI_ANTECEDENTE_FAM_CON_DEL()
                    {
                        ID_EMI = emiActual.ID_EMI,
                        ID_EMI_CONS = emiActual.ID_EMI_CONS,
                        ID_EMI_ANTECEDENTE = Convert.ToInt16(i + 1),
                        ID_PARENTESCO = w.ID_PARENTESCO,
                        ANIO = w.ANIO,
                        ID_DELITO = w.ID_DELITO,
                        ID_RELACION = w.ID_RELACION,
                        ID_EMISOR = w.ID_EMISOR
                    }));

                    var drogas = new List<EMI_ANTECEDENTE_FAMILIAR_DROGA>(LstFamiliarDroga == null ? null : LstFamiliarDroga.Select((w, i) => new EMI_ANTECEDENTE_FAMILIAR_DROGA()
                    {
                        ID_EMI = emiActual.ID_EMI,
                        ID_EMI_CONS = emiActual.ID_EMI_CONS,
                        ID_EMI_ANT_CONS = Convert.ToInt16(i + 1),
                        ID_TIPO_REFERENCIA = w.ID_TIPO_REFERENCIA,
                        ANIO = w.ANIO,
                        ID_DROGA = w.ID_DROGA,
                        ID_RELACION = w.ID_RELACION,
                    }));

                    if (emiActual.EMI_ANTECEDENTE_FAM_CON_DEL.Count > 0 || emiActual.EMI_ANTECEDENTE_FAMILIAR_DROGA.Count > 0)
                    {
                        if (!PEditar)
                        {
                            StaticSourcesViewModel.Mensaje("Validación", "Su usuario no cuenta con permisos para realizar esta acción.", StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA, 5);
                            return;
                        }
                    }
                    else
                    {
                        if (!PInsertar)
                        {
                            StaticSourcesViewModel.Mensaje("Validación", "Su usuario no cuenta con permisos para realizar esta acción.", StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA, 5);
                            return;
                        }
                    }

                    if (new cEMIFamConDelito().Insertar(emiActual.ID_EMI, emiActual.ID_EMI_CONS, delitos, drogas))
                    {
                        //if (GuardarGFDroga())
                        //{
                        //    ConductasParasocialesEnabled = UsoDrogaEnabled = true;
                        //    Mensaje(true, Name);
                        //}
                        //else
                        Mensaje(true, Name);
                    }
                    else
                        Mensaje(false, Name);
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar grupo familiar delito", ex);
            }
        }
        private void EliminarGFD()
        {
            try
            {
                if (LstFamiliarDelito != null)
                    if (SelectedFamiliarDelito != null)
                        LstFamiliarDelito.Remove(SelectedFamiliarDelito);
                EliminarItemMenu = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar grupo familiar delito", ex);
            }
        }

        //GRUPO FAMILIAR DROGA
        private void PopulateGFDrogaCB()
        {
            try
            {
                if (LstTipoReferencia == null)
                {
                    LstTipoReferencia = new ObservableCollection<TIPO_REFERENCIA>(new cTipoReferencia().ObtenerTodos());
                    LstTipoReferencia.Add(new TIPO_REFERENCIA() { ID_TIPO_REFERENCIA = -1, DESCR = "SELECCIONE" });
                }
                if (LstDrogas == null)
                {
                    LstDrogas = new ObservableCollection<DROGA>(new cDrogas().ObtenerTodos());
                    LstDrogas.Add(new DROGA() { ID_DROGA = -1, DESCR = "SELECCIONE" });
                }
                if (LstTipoRelacion == null)
                {
                    LstTipoRelacion = new ObservableCollection<TIPO_RELACION>(new cTipoRelacion().ObtenerTodos());
                    LstTipoRelacion.Add(new TIPO_RELACION() { ID_RELACION = -1, DESCR = "SELECCIONE" });
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer combobox grupo familia droga", ex);
            }
        }
        private void PopulateGFDroga()
        {
            try
            {
                ControlTab = 7;
                LstFamiliarDroga = new ObservableCollection<EMI_ANTECEDENTE_FAMILIAR_DROGA>(emiActual.EMI_ANTECEDENTE_FAMILIAR_DROGA);
                IsEmptyFamiliarDroga = true;
                if (LstFamiliarDroga != null)
                    if (LstFamiliarDroga.Count > 0)
                        IsEmptyFamiliarDroga = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer grupo familia droga", ex);
            }
        }
        private void PopulateGFDrogaModal()
        {
            try
            {
                if (SelectedFamiliarDroga != null)
                {

                    SelectedParentescoFDroga = LstTipoReferencia.Where(w => w.ID_TIPO_REFERENCIA == SelectedFamiliarDroga.ID_TIPO_REFERENCIA).FirstOrDefault();
                    AnioFDroga = SelectedFamiliarDroga.ANIO;
                    SelectedDrogaFDroga = LstDrogas.Where(w => w.ID_DROGA == SelectedFamiliarDroga.ID_DROGA).FirstOrDefault();
                    SelectedRelacionFDroga = LstTipoRelacion.Where(w => w.ID_RELACION == SelectedFamiliarDroga.ID_RELACION).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer grupo familia droga", ex);
            }
        }
        private void AgregarGFDroga()
        {
            try
            {
                if (LstFamiliarDroga == null)
                    LstFamiliarDroga = new ObservableCollection<EMI_ANTECEDENTE_FAMILIAR_DROGA>();
                if (!base.HasErrors)
                {
                    if (SelectedFamiliarDroga == null)//AGREGAMOS
                    {
                        LstFamiliarDroga.Add(new EMI_ANTECEDENTE_FAMILIAR_DROGA()
                        {
                            ID_TIPO_REFERENCIA = SelectedParentescoFDroga.ID_TIPO_REFERENCIA,
                            ANIO = AnioFDroga,
                            ID_DROGA = SelectedDrogaFDroga.ID_DROGA,
                            ID_RELACION = SelectedRelacionFDroga.ID_RELACION,
                            TIPO_REFERENCIA = SelectedParentescoFDroga,
                            DROGA = SelectedDrogaFDroga,
                            TIPO_RELACION = SelectedRelacionFDroga,
                            ID_EMI_ANT_CONS = LstFamiliarDroga.LastOrDefault() != null ? short.Parse((LstFamiliarDroga.LastOrDefault().ID_EMI_ANT_CONS + 1).ToString()) : (short)1
                        });
                    }
                    else//EDITAMOS 
                    {

                        SelectedFamiliarDroga.ID_TIPO_REFERENCIA = SelectedParentescoFDroga.ID_TIPO_REFERENCIA;
                        SelectedFamiliarDroga.ANIO = AnioFDroga;
                        SelectedFamiliarDroga.ID_DROGA = SelectedDrogaFDroga.ID_DROGA;
                        SelectedFamiliarDroga.ID_RELACION = SelectedRelacionFDroga.ID_RELACION;
                        SelectedFamiliarDroga.TIPO_REFERENCIA = SelectedParentescoFDroga;
                        SelectedFamiliarDroga.DROGA = SelectedDrogaFDroga;
                        SelectedFamiliarDroga.TIPO_RELACION = SelectedRelacionFDroga;
                        //                    LstFamiliarDroga = new ObservableCollection<EMI_ANTECEDENTE_FAMILIAR_DROGA>(LstFamiliarDroga);
                    }
                    if (LstFamiliarDroga != null)
                        if (LstFamiliarDroga.Count > 0)
                            IsEmptyFamiliarDroga = false;
                    LstFamiliarDroga = new ObservableCollection<EMI_ANTECEDENTE_FAMILIAR_DROGA>(LstFamiliarDroga);
                    LimpiarGFD();
                    setValidacionesSituacionJuridica();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.FAMILIAR_DROGA);
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar grupo familia droga", ex);
            }
        }
        private void LimpiarGFDroga()
        {
            try
            {
                SelectedFamiliarDroga = null;
                AnioFDroga = null;
                SelectedDrogaFDroga = null;
                SelectedRelacionFDroga = null;
                SelectedParentescoFDroga = null;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar grupo familia droga", ex);
            }
        }
        private bool GuardarGFDroga()
        {
            try
            {
                var fd = new List<EMI_ANTECEDENTE_FAMILIAR_DROGA>();
                if (LstFamiliarDroga != null)
                {
                    foreach (var f in LstFamiliarDroga)
                    {
                        fd.Add(new EMI_ANTECEDENTE_FAMILIAR_DROGA()
                        {
                            ID_EMI = emiActual.ID_EMI,
                            ID_EMI_CONS = emiActual.ID_EMI_CONS,
                            ID_EMI_ANT_CONS = f.ID_EMI_ANT_CONS,
                            ID_TIPO_REFERENCIA = f.ID_TIPO_REFERENCIA,
                            ANIO = f.ANIO,
                            ID_DROGA = f.ID_DROGA,
                            ID_RELACION = f.ID_RELACION,

                        });
                    }
                }
                return new cEMIFamiliarDroga().Insertar(emiActual.ID_EMI, emiActual.ID_EMI_CONS, fd);
                //Mensaje(resultado, Name);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar grupo familia droga", ex);
                return false;
            }
        }
        private void EliminarGFDroga()
        {
            try
            {
                if (LstFamiliarDroga != null)
                    if (SelectedFamiliarDroga != null)
                        LstFamiliarDroga.Remove(SelectedFamiliarDroga);
                EliminarItemMenu = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar grupo familia droga", ex);
            }
        }
        #endregion
        //Actividades
        #region [Actividades]

        private void AddActividad()
        {
            //try
            //{
            //    //if (!base.HasErrors)
            //    //{
            //    var objeto = LstAuxiliarActividades.Where(w => w.Id == PopUpActividad.ID_EMI_ACTIVIDAD).SingleOrDefault();
            //    var todasActividades = new ObservableCollection<EMI_ACTIVIDAD>();
            //    foreach (var lista in LstAuxiliarActividades)
            //    {
            //        foreach (var actividad in lista.Actividades)
            //        {
            //            todasActividades.Add(actividad);
            //        }
            //    }

            //    var ultimo_elemento = todasActividades.OrderBy(s => s.ID_CONSEC).LastOrDefault();
            //    if (objeto != null)
            //    {
            //        if (SelectedActividad == null)
            //        {
            //            if (objeto.Actividades == null)
            //                objeto.Actividades = new ObservableCollection<EMI_ACTIVIDAD>();
            //            PopUpActividad.ID_EMI = emiActual.ID_EMI;
            //            PopUpActividad.ID_EMI_CONS = emiActual.ID_EMI_CONS;
            //            PopUpActividad.ID_CONSEC = ultimo_elemento != null ? (short)(ultimo_elemento.ID_CONSEC + ((short)1)) : (short)1;
            //            objeto.IsGridEmpty = false;
            //            objeto.IsGridVisible = true;
            //            objeto.Actividades.Add(PopUpActividad);
            //            objeto.Actividades = new ObservableCollection<EMI_ACTIVIDAD>(objeto.Actividades);
            //        }
            //        else
            //        {
            //            SelectedActividad = PopUpActividad;
            //        }
            //        LstAuxiliarActividades = new ObservableCollection<AuxiliarActividades>(LstAuxiliarActividades);
            //        OnPropertyChanged("LstAuxiliarActividades");
            //    }
            //    //}
            //}
            //catch (Exception ex)
            //{
            //    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar actividad", ex);
            //}
        }

        private void LimpiarActividad()
        {
            try
            {
                SelectedActividad = null;
                PopUpActividad = new EMI_ACTIVIDAD();
                popupEstatusPrograma = -1;
                popupTipoActividad = -1;
                EliminarItemMenu = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar actividad", ex);
            }
        }

        /// <summary>
        /// Guarda la información de las actividades que realiza el interno.
        /// </summary>
        private void GuardarActividades()
        {
            //try
            //{
            //    var Name = "Actividades";
            //    if (emiActual != null)
            //    {
            //        new cEMIActividad().Eliminar(emiActual.ID_EMI, emiActual.ID_EMI_CONS);
            //        var lista_actividades = new List<EMI_ACTIVIDAD>();
            //        if (LstAuxiliarActividades != null)
            //        {
            //            foreach (var tipo_actividad in LstAuxiliarActividades)
            //            {
            //                foreach (var actividad in tipo_actividad.Actividades)
            //                {
            //                    lista_actividades.Add
            //                        (
            //                        new EMI_ACTIVIDAD
            //                        {
            //                            ID_EMI = emiActual.ID_EMI,
            //                            ID_EMI_ACTIVIDAD = tipo_actividad.Id,
            //                            ID_EMI_CONS = emiActual.ID_EMI_CONS,
            //                            ANO_ACTIVIDADES = actividad.ANO_ACTIVIDADES,
            //                            DURACION_ACTIVIDADES = actividad.DURACION_ACTIVIDADES,
            //                            DESCRIPCION_ACTIVIDADES = actividad.DESCRIPCION_ACTIVIDADES,
            //                            ESTATUS_ACTIVIDADES = actividad.ESTATUS_ACTIVIDADES,
            //                            PROGRAMA_TERMINADO = actividad.PROGRAMA_TERMINADO,
            //                            ID_CONSEC = actividad.ID_CONSEC
            //                        }
            //                        );
            //                }
            //            }

            //            Mensaje(new cEMIActividad().Insertar(emiActual.ID_EMI, emiActual.ID_EMI_CONS, lista_actividades), Name);
            //            #region guardado
            //            //if (SelectIngreso.EMI.Count <= 0 || SelectIngreso.EMI == null)
            //            //{
            //            //    var result = AddNewEMI();
            //            //    if (result.ToLower().Contains("correctamente"))
            //            //    {
            //            //        var resultado = new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMIActividad().Agregar(lista_actividades);
            //            //        Mensaje(resultado, Name);
            //            //    }
            //            //    else
            //            //    {
            //            //        StaticSourcesViewModel.Mensaje("Error", "No se pudo crear EMI, contacte al administrador del sistema.", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR, 5);
            //            //    }
            //            //}
            //            //else
            //            //{
            //            //    var resultado = string.Empty;
            //            //    foreach (var actividad in lista_actividades)
            //            //    {
            //            //        if (new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMIActividad().Obtener(actividad.ID_EMI, actividad.ID_EMI_CONS, actividad.ID_CONSEC) != null)
            //            //            resultado = new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMIActividad().Actualizar(actividad);
            //            //        else
            //            //            resultado = new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMIActividad().Agregar(actividad);

            //            //        Mensaje(resultado, Name);
            //            //    }

            //            //}
            //            #endregion
            //        }
            //    }
            //    else
            //        AvisoImputadoVacio();
            //}
            //catch (Exception ex)
            //{
            //    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar actividad", ex);
            //}
        }

        private void EliminarActividad()
        {
            //try
            //{
            //    if (SelectedActividad != null)
            //    {
            //        var objActividades = LstAuxiliarActividades.Where(w => w.Id == SelectedActividad.ID_EMI_ACTIVIDAD).SingleOrDefault();
            //        objActividades.Actividades.Remove(SelectedActividad);
            //        if (objActividades.Actividades.Count <= 0)
            //        { objActividades.IsGridEmpty = true; objActividades.IsGridVisible = false; }
            //        objActividades.Actividades = new ObservableCollection<EMI_ACTIVIDAD>(objActividades.Actividades);
            //        LstAuxiliarActividades = new ObservableCollection<AuxiliarActividades>(LstAuxiliarActividades);
            //    }
            //    else
            //        StaticSourcesViewModel.Mensaje("Error", "Debe seleccionar un elemento de la lista primero.", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
            //}
            //catch (Exception ex)
            //{
            //    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar actividad", ex);
            //}
            //EliminarItemMenu = false;
        }
        #endregion

        #region [WebCam]
        private void OnTakePicture(System.Windows.Controls.Image Picture)
        {
            try
            {
                if (Processing)
                {
                    Processing = false;
                    return; 
                }
                Processing = true;
                ImagesToSave = ImagesToSave ?? new List<ImageSourceToSave>();
                if (CamaraWeb.ImageControls.Where(w => w.Name == Picture.Name).Any())
                {
                    Picture.Source = CamaraWeb.TomarFoto(Picture);
                    ImagesToSave.Add(new ImageSourceToSave { FrameName = Picture.Name, ImageCaptured = (BitmapSource)Picture.Source });
                    StaticSourcesViewModel.Mensaje(Picture.Name == "LeftFace" ? "LADO IZQUIERDO" : Picture.Name == "RightFace" ? "LADO DERECHO" : Picture.Name == "FrontFace" ? "CENTRO" : Picture.Name == "ImgSenaParticular" ? "Tipografía Humana" : "Camara", "Foto Capturada", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION, 1);
                }
                else
                {
                    CamaraWeb.QuitarFoto(Picture);
                    ImagesToSave.RemoveAll(w => w.FrameName == Picture.Name);
                    //ImagesToSave.Remove(ImagesToSave.Where(wm => wm.FrameName == Picture.Name).SingleOrDefault());
                    
                }
                if (ImagesToSave.Count > 0)
                    BotonTomarFotoEnabled = true;
                else
                    BotonTomarFotoEnabled = false;
                Processing = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al tomar fotografía", ex);
            }
        }
        private void OpenSetting(string obj)
        {
            try
            {
                CamaraWeb.AdvanceSetting();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al abrir propiedades de la cámara", ex);
            }
        }
        #endregion

        #region [GUARDADO DE INFORMACIÓN]

        /// <summary>
        /// Guarda ficha de identificacion
        /// </summary>
        private void GuardarFichaIdentificacion()
        {
            try
            {
                var Name = "Ficha de Identificaci\u00F3n";
                if (SelectIngreso != null)
                {
                    //VERIFICACMOS QUE EXISTA EMI
                    if (emiActual == null)//CREAMOS NUEVO EMI
                    {
                        if (!PInsertar)
                        {
                            StaticSourcesViewModel.Mensaje("Validación", "Su usuario no cuenta con permisos para realizar esta acción.", StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA, 5);
                            return;
                        }
                        if (!AddNewEMI())
                        {
                            new Dialogos().NotificacionDialog("Error", "Ocurrió un error al guardar el EMI");
                            return;
                        }
                    }
                    else
                    {
                        if (emiActual.ID_EMI == 0)
                        {
                            if (!PInsertar)
                            {
                                StaticSourcesViewModel.Mensaje("Validación", "Su usuario no cuenta con permisos para realizar esta acción.", StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA, 5);
                                return;
                            }
                            if (!AddNewEMI())
                            {
                                new Dialogos().NotificacionDialog("Error", "Ocurrió un error al guardar el EMI");
                                return;
                            }
                        }
                    }
                    var entidad = new EMI_FICHA_IDENTIFICACION();
                    entidad.ID_EMI = emiActual.ID_EMI;
                    entidad.ID_EMI_CONS = emiActual.ID_EMI_CONS;
                    entidad.FEC_CAPTURA = FechaCaptura;
                    entidad.TIEMPO_RESID_COL = TiempoColonia;
                    entidad.ID_GRADO_EDUCATIVO_CONCLUIDO = UltimoGradoEducativoConcluido;
                    entidad.PERSONA_CONVIVENCIA_ANTERIOR = ViviaAntesDetencion;
                    entidad.ID_PARENTESCO = Parentesco;
                    entidad.ID_EXFUNCIONARIO_SEGPUB = ExFuncionarioSeguridadPublica;
                    if (CeresoProcedencia != -1)
                        entidad.ID_CERESO_PROCEDENCIA = CeresoProcedencia;
                    entidad.ACTA_NACIMIENTO = ActaNacimiento == true ? "S" : "N";
                    entidad.PASAPORTE = Pasaporte == true ? "S" : "N";
                    entidad.LICENCIA_MANEJO = LicenciaManejo == true ? "S" : "N";
                    entidad.CREDENCIAL_ELECTOR = CredencialElector == true ? "S" : "N";
                    entidad.CARTILLA_MILITAR = CartillaMilitar == true ? "S" : "N";
                    entidad.ID_CERTIFICADO_EDUCACION = CertificadoEducacion;
                    entidad.OFICIOS_HABILIDADES = OficiosHabilidades;
                    entidad.CAMBIOS_DOMICILIO_ULTIMO_ANO = UltimoAnio;
                    entidad.MOTIVOS_CAMBIOS_DOMICILIO = Motivo;

                    //Ultimos Empleaod
                    var empleos = new List<EMI_ULTIMOS_EMPLEOS>(LstUltimosEmpleos == null ? null : LstUltimosEmpleos.Select((w, i) => new EMI_ULTIMOS_EMPLEOS()
                    {
                        ID_EMI = emiActual.ID_EMI,
                        ID_EMI_CONS = emiActual.ID_EMI_CONS,
                        ID_EMI_ULTIMOS_EMPLEOS = Convert.ToInt16(i + 1),
                        ID_OCUPACION = w.ID_OCUPACION,
                        DURACION = w.DURACION,
                        EMPRESA = w.EMPRESA,
                        MOTIVO_DESEMPLEO = w.MOTIVO_DESEMPLEO,
                        EMPLEO_FORMAL = w.EMPLEO_FORMAL,
                        ULTIMO_EMPLEO_ANTES_DETENCION = w.ULTIMO_EMPLEO_ANTES_DETENCION,
                        INESTABILIDAD_LABORAL = w.INESTABILIDAD_LABORAL
                    }));

                    if (emiActual.EMI_FICHA_IDENTIFICACION == null)//AGREGAR
                    {
                        if (PInsertar)
                        {
                            if (new cEMIFichaIdentificacion().Agregar(entidad, empleos))
                            {
                                emiActual.EMI_FICHA_IDENTIFICACION = entidad;
                                if (GuardarUltimosEmpleos())
                                    //{
                                    SituacionJuridicaEnabled = EstudioTrasladoEnabled = true;
                                Mensaje("Informaci\u00F3n registrada correctamente.", Name);
                                //}
                                //else
                                //    StaticSourcesViewModel.Mensaje("Error", "No se pudo crear EMI, contacte al administrador del sistema.", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR, 5);
                            }
                            else
                                StaticSourcesViewModel.Mensaje("Error", "No se pudo crear EMI, contacte al administrador del sistema.", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR, 5);
                        }
                        else
                            StaticSourcesViewModel.Mensaje("Validación", "Su usuario no cuenta con permisos para realizar esta acción.", StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA, 5);
                    }
                    else//EDITAR
                    {
                        if (PEditar)
                        {
                            if (new cEMIFichaIdentificacion().Actualizar(entidad, empleos))
                            {
                                if (GuardarUltimosEmpleos())
                                    Mensaje("Informaci\u00F3n registrada correctamente.", Name);
                                else
                                    StaticSourcesViewModel.Mensaje("Error", "No se pudo crear EMI, contacte al administrador del sistema.", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR, 5);
                            }
                            else
                                StaticSourcesViewModel.Mensaje("Error", "No se pudo crear EMI, contacte al administrador del sistema.", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR, 5);
                        }
                        else
                            StaticSourcesViewModel.Mensaje("Validación", "Su usuario no cuenta con permisos para realizar esta acción.", StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA, 5);
                    }

                    #region guardado
                    //if (SelectIngreso != null)
                    //{
                    //    if (SelectIngreso.EMI.Count <= 0 || SelectIngreso.EMI == null)
                    //    {
                    //        var result = AddNewEMI();
                    //        if (result.ToLower().Contains("correctamente"))
                    //        {
                    //            IndexPadre = IndexHijo = 1;
                    //            entidad.ID_EMI = emiActual.ID_EMI;
                    //            entidad.ID_EMI_CONS = emiActual.ID_EMI_CONS;
                    //            var resultado = new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMIFichaIdentificacion().Agregar(entidad);
                    //            GuardarUltimosEmpleos();
                    //            Mensaje(resultado, Name);
                    //        }
                    //        else
                    //        {
                    //            StaticSourcesViewModel.Mensaje("Error", "No se pudo crear EMI, contacte al administrador del sistema.", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR, 5);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        entidad.ID_EMI = emiActual.ID_EMI;
                    //        entidad.ID_EMI_CONS = emiActual.ID_EMI_CONS;
                    //        var resultado = string.Empty;
                    //        if (new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMIFichaIdentificacion().Obtener(entidad.ID_EMI) != null)
                    //            resultado = new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMIFichaIdentificacion().Actualizar(entidad);
                    //        else
                    //            resultado = new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMIFichaIdentificacion().Agregar(entidad);
                    //        Mensaje(resultado, Name);
                    //    }
                    //}
                    //else
                    //    AvisoImputadoVacio();
                    #endregion
                }
                else
                    AvisoImputadoVacio();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar ficha identificación", ex);
            }
        }

        /// <summary>
        /// Guarda la información de situación jurídica.
        /// </summary>
        private void GuardarEstudiosTraslados()
        {
            try
            {
                var Name = "Estudios y Traslados";
                if (SelectIngreso != null)
                {
                    var entidad = new EMI_SITUACION_JURIDICA();
                    entidad.ID_EMI = emiActual.ID_EMI;
                    entidad.ID_EMI_CONS = emiActual.ID_EMI_CONS;
                    entidad.VERSION_DELITO_INTERNO = VersionDelito;
                    entidad.PRACT_ESTUDIOS = PracticadoEstudios ? "S" : "N";
                    entidad.DESEA_TRASLADO = Traslado ? "S" : "N";
                    entidad.CUANDO_PRACT_ESTUDIOS = Cuando;
                    entidad.DONDE_DESEA_TRASLADO = Donde;
                    entidad.MOTIVO_DESEA_TRASLADO = PorqueMotivo;
                    entidad.MAYOR_PERIODO_LIBRE_REING = MayorPeriodoLibreReingreso;
                    entidad.MENOR_PERIODO_LIBRE_REING = MenorPeriodoLibreReingreso;
                    if (emiActual.EMI_SITUACION_JURIDICA == null)//AGREGAR
                    {
                        if (PInsertar)
                        {
                            if (new cEMISituacionJuridica().Agregar(entidad))
                            {
                                emiActual.EMI_SITUACION_JURIDICA = entidad;
                                IngresoAnteriorEnabled = true;
                                Mensaje(true, Name);
                            }
                            else
                                Mensaje(false, Name);
                        }
                        else
                            StaticSourcesViewModel.Mensaje("Validación", "Su usuario no cuenta con permisos para realizar esta acción.", StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA, 5);
                    }
                    else//EDITAR
                    {
                        if (PEditar)
                            Mensaje(new cEMISituacionJuridica().Actualizar(entidad), Name);
                        else
                            StaticSourcesViewModel.Mensaje("Validación", "Su usuario no cuenta con permisos para realizar esta acción.", StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA, 5);
                    }
                    #region guardado
                    //if (SelectIngreso.EMI.Count <= 0 || SelectIngreso.EMI == null)
                    //{
                    //    var result = AddNewEMI();
                    //    if (result.ToLower().Contains("correctamente"))
                    //    {
                    //        entidad.ID_EMI = emiActual.ID_EMI;
                    //        entidad.ID_EMI_CONS = emiActual.ID_EMI_CONS;
                    //        var resultado = new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMISituacionJuridica().Agregar(entidad);
                    //        Mensaje(resultado, Name);
                    //    }
                    //    else
                    //    {
                    //        StaticSourcesViewModel.Mensaje("Error", "No se pudo crear EMI, contacte al administrador del sistema.", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR, 5);
                    //    }
                    //}
                    //else
                    //{
                    //    var _ = SelectIngreso;
                    //    entidad.ID_EMI = emiActual.ID_EMI;
                    //    entidad.ID_EMI_CONS = emiActual.ID_EMI_CONS;
                    //    var resultado = string.Empty;
                    //    if (new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMISituacionJuridica().Obtener(entidad.ID_EMI) != null)
                    //        resultado = new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMISituacionJuridica().Actualizar(entidad);
                    //    else
                    //        resultado = new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMISituacionJuridica().Agregar(entidad);
                    //    Mensaje(resultado, Name);
                    //}

                    #endregion
                }
                else
                    AvisoImputadoVacio();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar estudio traslado", ex);
            }
        }

        /// <summary>
        /// Guardar estudios y traslados
        /// </summary>
        //private void GuardarEstudiosTraslados()
        //{
        //    var Name = "Estudios y Traslados";
        //    if (SelectIngreso != null)
        //    {
        //        var entidad = new EMI_SITUACION_JURIDICA();
        //        entidad.VERSION_DELITO_INTERNO = VersionDelito;
        //        entidad.MENOR_PERIODO_LIBRE_REING = MenorPeriodoLibreReingreso;
        //        entidad.MAYOR_PERIODO_LIBRE_REING = MayorPeriodoLibreReingreso;
        //        entidad.PRACT_ESTUDIOS = PracticadoEstudios ? "S" : "N";
        //        entidad.CUANDO_PRACT_ESTUDIOS = Cuando;
        //        entidad.DESEA_TRASLADO = Traslado ? "S" : "N";
        //        entidad.DONDE_DESEA_TRASLADO = Donde;
        //        entidad.MOTIVO_DESEA_TRASLADO = PorqueMotivo;
        //        #region guardado
        //        if (SelectIngreso.EMI.Count <= 0 || SelectIngreso.EMI == null)
        //        {
        //            var result = AddNewEMI();
        //            if (result.ToLower().Contains("correctamente"))
        //            {
        //                entidad.ID_EMI = emiActual.ID_EMI;
        //                entidad.ID_EMI_CONS = emiActual.ID_EMI_CONS;
        //                var resultado = new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMISituacionJuridica().Agregar(entidad);
        //                Mensaje(resultado, Name);
        //            }
        //            else
        //            {
        //                StaticSourcesViewModel.Mensaje("Error", "No se pudo crear EMI, contacte al administrador del sistema.", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR, 5);
        //            }
        //        }
        //        else
        //        {
        //            entidad.ID_EMI = emiActual.ID_EMI;
        //            entidad.ID_EMI_CONS = emiActual.ID_EMI_CONS;
        //            var resultado = string.Empty;
        //            if (new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMISituacionJuridica().Obtener(entidad.ID_EMI) != null)
        //                resultado = new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMISituacionJuridica().Actualizar(entidad);
        //            else
        //                resultado = new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMISituacionJuridica().Agregar(entidad);
        //            Mensaje(resultado, Name);
        //        }

        //        #endregion
        //    }
        //    else
        //        AvisoImputadoVacio();
        //}
        /// <summary>
        /// Guardar Ultimos Ingresos del imputado un CENTRO
        /// </summary>
        //private void GuardarUltimosIngresosCereso()
        //{
        //    //FALTA TABLA
        //    if (SelectIngreso != null)
        //    { }
        //    else
        //        AvisoImputadoVacio();
        //}
        /// <summary>
        /// Guardar Ingresos a centros de menores
        /// </summary>
        //private void GuardarIngresosAnterioresMenores()
        //{
        //    //FALTA TABLA
        //    if (SelectIngreso != null)
        //    { }
        //    else
        //        AvisoImputadoVacio();
        //}

        /// <summary>
        /// Guardar Factores Socio Familiares
        /// </summary>
        private void GuardarFactores()
        {
            try
            {
                var Name = "Factores Socio Familiares";
                if (SelectIngreso != null)
                {
                    var entidad = new EMI_FACTORES_SOCIO_FAMILIARES();
                    entidad.ID_EMI = emiActual.ID_EMI;
                    entidad.ID_EMI_CONS = emiActual.ID_EMI_CONS;
                    entidad.RECIBE_VISITA_FAMILIAR = RecibeVisitaFamiliar;
                    entidad.FRECUENCIA_VISITA_FAMILIAR = Frecuencia.HasValue ? !Frecuencia.Value.Equals(-1) ? Frecuencia.Value : new short?() : new short?();
                    entidad.RECIBE_VISITA_INTIMA = VisitaIntima;
                    entidad.RECIBE_APOYO_ECONOMICO = ApoyoEconomico;
                    entidad.CANTIDAD_APOYO_ECONOMICO = CantidadApoyoEconomico;
                    entidad.FRECUENCIA_APOYO_ECONOMICO = CantidadFrecuencia.HasValue ? !CantidadFrecuencia.Value.Equals(-1) ? CantidadFrecuencia.Value : new short?() : new short?();
                    entidad.VIVE_PADRE = VivePadre;
                    entidad.VIVE_MADRE = ViveMadre;
                    entidad.PADRES_JUNTOS = PadresVivenJuntos;
                    entidad.MOTIVOS_SEPARACION = MotivoSeparacion;
                    entidad.EDAD_INTERNO_FALLE_PADRE = EdadFallecioPadreEnabled ? FallecioPadre : 0;
                    entidad.EDAD_INTERNO_FALLE_MADRE = EdadFallecioMadreEnabled ? FallecioMadre : 0;
                    entidad.EDAD_INTERNO_SEPARACION = EdadInternoSeparacionPadres;
                    entidad.ID_NIVEL_SOCIAL = Social;
                    entidad.ID_NIVEL_CULTURAL = Cultural;
                    entidad.ID_NIVEL_ECONOMICO = Economico;
                    entidad.TOTAL_PAREJAS = TotalParejas;
                    entidad.CANTIDAD_PAREJAS_UNION = CuantasUnion;
                    entidad.NUMERO_HIJOS = Hijos;
                    entidad.HIJOS_REGISTRADOS = Registrados;
                    entidad.CANTIDAD_HIJOS_RELACION = CuantosMantieneRelacion;
                    entidad.CANTIDAD_HIJOS_VISITA = CuantosVisitan;
                    entidad.CONTACTO_NOMBRE = ContactoNombre;
                    entidad.ID_CONTACTO_PARENTESCO = ContactoParentesco;
                    entidad.CONTACTO_TELEFONO = long.Parse(TextContactoTelefono.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", ""));//ContactoTelefono;
                    entidad.ABUSO_SEXUAL = AbusoSexual;
                    entidad.ESPECIFIQUE_ABUSO_SEXUAL = EspecifiqueAbusoSexual;
                    entidad.EDAD_ABUSO_SEXUAL = EdadAbusoSexualEnabled ? EdadAbuso : 0;
                    entidad.EDAD_INICIO_CONTACTO_SEXUAL = EdadInicioContactoSexual;
                    entidad.HUIDAS_HOGAR_ABANDONO_FAMILIAR = HuidasHogar;
                    entidad.MALTRATO_FISICO = MaltratoFisico;
                    entidad.ESPECIFIQUE_MALTRATO_FISICO = EspecifiqueMaltratoFisico;
                    entidad.ABANDONO_FAMILIAR = AbandonoFamiliar;

                    if (entidad.ABANDONO_FAMILIAR.Equals("S"))
                    {
                        entidad.ESPECIF_ABANDONO_FAMILIAR = EspecifiqueAbandonoFamiliar;
                    }

                    entidad.MALTRATO_EMOCIONAL = MaltratoEmocional;
                    entidad.ESPECIF_MALTRATO_EMOCIONAL = EspecifiqueMaltratoEmocional;

                    if (emiActual.EMI_FACTORES_SOCIO_FAMILIARES == null)//AGREGAR
                    {
                        if (PInsertar)
                        {
                            if (new cEMIFactoresSocioFamiliares().Agregar(entidad))
                            {
                                emiActual.EMI_FACTORES_SOCIO_FAMILIARES = entidad;
                                GrupoFamiliarEnabled = true;
                                Mensaje(true, Name);
                            }
                            else
                                Mensaje(false, Name);
                        }
                        else
                            StaticSourcesViewModel.Mensaje("Validación", "Su usuario no cuenta con permisos para realizar esta acción.", StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA, 5);
                    }
                    else//ACTUALIZAR
                    {
                        if (PEditar)
                            Mensaje(new cEMIFactoresSocioFamiliares().Actualizar(entidad), Name);
                        else
                            StaticSourcesViewModel.Mensaje("Validación", "Su usuario no cuenta con permisos para realizar esta acción.", StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA, 5);
                    }

                    #region guardado
                    //if (SelectIngreso.EMI.Count <= 0 || SelectIngreso.EMI == null)
                    //{
                    //    var result = AddNewEMI();
                    //    if (result.ToLower().Contains("correctamente"))
                    //    {
                    //        entidad.ID_EMI = emiActual.ID_EMI;
                    //        entidad.ID_EMI_CONS = emiActual.ID_EMI_CONS;
                    //        var resultado = new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMIFactoresSocioFamiliares().Agregar(entidad);
                    //        Mensaje(resultado, Name);
                    //    }
                    //    else
                    //    {
                    //        StaticSourcesViewModel.Mensaje("Error", "No se pudo crear EMI, contacte al administrador del sistema.", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR, 5);
                    //    }
                    //}
                    //else
                    //{
                    //    entidad.ID_EMI = emiActual.ID_EMI;
                    //    entidad.ID_EMI_CONS = emiActual.ID_EMI_CONS;
                    //    var resultado = string.Empty;
                    //    if (new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMIFactoresSocioFamiliares().Obtener(entidad.ID_EMI) != null)
                    //        resultado = new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMIFactoresSocioFamiliares().Actualizar(entidad);
                    //    else
                    //        resultado = new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMIFactoresSocioFamiliares().Agregar(entidad);
                    //    Mensaje(resultado, Name);
                    //}

                    #endregion
                }
                else
                    AvisoImputadoVacio();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar factores", ex);
            }
        }

        /// <summary>
        /// Guardar Datos Grupos Familiares Delitos y uso de drogas
        /// </summary>
        private void GuardarAntecedentesGrupoFamiliar()
        {
            try
            {
                if (SelectIngreso != null)
                {
                    //DELITO
                    //var delito = new List<EMI_ANTECEDENTE_FAM_CON_DEL>();
                    //if (LstFamiliarDelito != null)
                    //{
                    //    var index = 1;
                    //    foreach (var familiar in LstFamiliarDelito)
                    //    {
                    //        delito.Add(new EMI_ANTECEDENTE_FAM_CON_DEL()
                    //        {
                    //            ID_EMI = emiActual.ID_EMI,
                    //            ID_EMI_CONS = emiActual.ID_EMI_CONS,
                    //            ID_PARENTESCO = familiar.ID_PARENTESCO,
                    //            ANIO = familiar.ANIO,
                    //            ID_DELITO = familiar.ID_DELITO,
                    //            DELITO = familiar.DELITO,
                    //            ID_RELACION = familiar.ID_RELACION,
                    //            ID_EMISOR = familiar.ID_EMISOR,
                    //            ID_EMI_ANTECEDENTE = familiar.ID_EMI_ANTECEDENTE,
                    //        });
                    //    }
                    //    index++;
                    //}
                    var delitos = new List<EMI_ANTECEDENTE_FAM_CON_DEL>(LstFamiliarDelito == null ? null : LstFamiliarDelito.Select((w, i) => new EMI_ANTECEDENTE_FAM_CON_DEL()
                    {
                        ID_EMI = emiActual.ID_EMI,
                        ID_EMI_CONS = emiActual.ID_EMI_CONS,
                        ID_PARENTESCO = w.ID_PARENTESCO,
                        ANIO = w.ANIO,
                        ID_DELITO = w.ID_DELITO,
                        DELITO = w.DELITO,
                        ID_RELACION = w.ID_RELACION,
                        ID_EMISOR = w.ID_EMISOR,
                        ID_EMI_ANTECEDENTE = Convert.ToInt16(i + 1)
                    }));
                    //new cEMIFamConDelito().Insertar(emiActual.ID_EMI, emiActual.ID_EMI_CONS, delito);
                    //LstFamiliarDelito = new ObservableCollection<SSP.Servidor.EMI_ANTECEDENTE_FAM_CON_DEL>(new cEMIFamConDelito().Obtener(emiActual.ID_EMI, emiActual.ID_EMI_CONS));
                    //DROGA
                    //var droga = new List<EMI_ANTECEDENTE_FAMILIAR_DROGA>();
                    //if (LstFamiliarDroga != null)
                    //{
                    //    foreach (var familiar in LstFamiliarDroga)
                    //    {
                    //        droga.Add(new EMI_ANTECEDENTE_FAMILIAR_DROGA()
                    //        {
                    //            ID_EMI = emiActual.ID_EMI,
                    //            ID_EMI_CONS = emiActual.ID_EMI_CONS,
                    //            ID_DROGA = familiar.ID_DROGA,
                    //            ID_TIPO_REFERENCIA = familiar.ID_TIPO_REFERENCIA,
                    //            ANIO = familiar.ANIO,
                    //            ID_RELACION = familiar.ID_RELACION,
                    //            ID_EMI_ANT_CONS = familiar.ID_EMI_ANT_CONS
                    //        });
                    //    }
                    //}
                    //new cEMIFamiliarDroga().Insertar(emiActual.ID_EMI, emiActual.ID_EMI_CONS, droga);
                    //LstFamiliarDroga = new ObservableCollection<EMI_ANTECEDENTE_FAMILIAR_DROGA>(new cEMIFamiliarDroga().Obtener(emiActual.ID_EMI, emiActual.ID_EMI_CONS));
                    var drogas = new List<EMI_ANTECEDENTE_FAMILIAR_DROGA>(LstFamiliarDroga == null ? null : LstFamiliarDroga.Select((w, i) => new EMI_ANTECEDENTE_FAMILIAR_DROGA()
                    {
                        ID_EMI = emiActual.ID_EMI,
                        ID_EMI_CONS = emiActual.ID_EMI_CONS,
                        ID_DROGA = w.ID_DROGA,
                        ID_TIPO_REFERENCIA = w.ID_TIPO_REFERENCIA,
                        ANIO = w.ANIO,
                        ID_RELACION = w.ID_RELACION,
                        ID_EMI_ANT_CONS = Convert.ToInt16(i + 1)
                    }));
                    if (emiActual.EMI_ANTECEDENTE_FAM_CON_DEL.Count > 0 || emiActual.EMI_ANTECEDENTE_FAMILIAR_DROGA.Count > 0)
                    {
                        if (!PEditar)
                        {
                            StaticSourcesViewModel.Mensaje("Validación", "Su usuario no cuenta con permisos para realizar esta acción.", StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA, 5);
                            return;
                        }
                    }
                    else
                    {
                        if (!PInsertar)
                        {
                            StaticSourcesViewModel.Mensaje("Validación", "Su usuario no cuenta con permisos para realizar esta acción.", StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA, 5);
                            return;
                        }
                    }
                    if (new cEMIFamConDelito().Insertar(emiActual.ID_EMI, emiActual.ID_EMI_CONS, delitos, drogas))
                        Mensaje(true, "Antecedentes de Grupo Familiar");
                    else
                        Mensaje(false, "Antecedentes de Grupo Familiar");
                }
                else
                    AvisoImputadoVacio();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al grabar antecedentes grupo familiar", ex);
            }
        }

        /// <summary>
        /// Guardar uso de drogas del interno
        /// </summary>
        private void GuardarUsoDroga()
        {
            try
            {
                var Name = "Uso de Drogas";
                if (SelectIngreso != null)
                {
                    //var lst = new List<EMI_USO_DROGA>();

                    //foreach (var elemento in lstUsosEliminar)
                    //{
                    //    lst.Add(new EMI_USO_DROGA
                    //    {
                    //        ID_DROGA = elemento.ID_DROGA,
                    //        ID_EMI = emiActual.ID_EMI,
                    //        ID_EMI_CONS = emiActual.ID_EMI_CONS,

                    //    });

                    //}
                    //new cEMIUsoDroga().Eliminar(emiActual.ID_EMI, emiActual.ID_EMI_CONS);
                    //var drogas = new List<EMI_USO_DROGA>();
                    if (LstUsoDrogas != null)
                    {
                        //short index = 1;
                        //foreach (var droga in LstUsoDrogas)
                        //{
                        //    drogas.Add(new EMI_USO_DROGA()
                        //    {
                        //        ID_EMI = emiActual.ID_EMI,
                        //        ID_EMI_CONS = emiActual.ID_EMI_CONS,
                        //        ID_EMI_USO_DROGA = index,
                        //        ID_DROGA = droga.ID_DROGA,
                        //        EDAD_INICIO = droga.EDAD_INICIO,
                        //        FEC_ULTIMA_DOSIS = droga.FEC_ULTIMA_DOSIS,
                        //        FRECUENCIA_USO = droga.FRECUENCIA_USO,
                        //        CONSUMO_ACTUAL = droga.CONSUMO_ACTUAL,
                        //        TIEMPO_CONSUMO = droga.TIEMPO_CONSUMO
                        //    });
                        //    index++;
                        //}
                        var drogas = new List<EMI_USO_DROGA>(LstUsoDrogas == null ? null : LstUsoDrogas.Select((w, i) => new EMI_USO_DROGA()
                        {
                            ID_EMI = emiActual.ID_EMI,
                            ID_EMI_CONS = emiActual.ID_EMI_CONS,
                            ID_EMI_USO_DROGA = Convert.ToInt16(i + 1),
                            ID_DROGA = w.ID_DROGA,
                            EDAD_INICIO = w.EDAD_INICIO,
                            FEC_ULTIMA_DOSIS = w.FEC_ULTIMA_DOSIS,
                            FRECUENCIA_USO = w.FRECUENCIA_USO,
                            CONSUMO_ACTUAL = w.CONSUMO_ACTUAL,
                            TIEMPO_CONSUMO = w.TIEMPO_CONSUMO
                        }));

                        if (emiActual.EMI_USO_DROGA.Count > 0)
                        {
                            if (!PEditar)
                            {
                                StaticSourcesViewModel.Mensaje("Validación", "Su usuario no cuenta con permisos para realizar esta acción.", StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA, 5);
                                return;
                            }
                        }
                        else
                        {
                            if (!PInsertar)
                            {
                                StaticSourcesViewModel.Mensaje("Validación", "Su usuario no cuenta con permisos para realizar esta acción.", StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA, 5);
                                return;
                            }
                        }
                        if (new cEMIUsoDroga().Insertar(emiActual.ID_EMI, emiActual.ID_EMI_CONS, drogas))
                        {
                            HPSEnabled = true;
                            LstUsoDrogas = new ObservableCollection<SSP.Servidor.EMI_USO_DROGA>(new cEMIUsoDroga().Obtener(emiActual.ID_EMI, emiActual.ID_EMI_CONS));
                            Mensaje(true, Name);
                        }
                        else
                            Mensaje(false, Name);

                        #region guardado
                        //if (SelectIngreso.EMI.Count <= 0 || SelectIngreso.EMI == null)
                        //{
                        //    var result = AddNewEMI();
                        //    if (result.ToLower().Contains("correctamente"))
                        //    {
                        //        foreach (var usoDroga in drogas)
                        //        {
                        //            usoDroga.ID_EMI = emiActual.ID_EMI;
                        //            usoDroga.ID_EMI_CONS = emiActual.ID_EMI_CONS;
                        //        }
                        //        var resultado = new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMIUsoDroga().Agregar(drogas);
                        //        LstUsoDrogas = new ObservableCollection<SSP.Servidor.EMI_USO_DROGA>(new cEMIUsoDroga().Obtener(emiActual.ID_EMI, emiActual.ID_EMI_CONS));
                        //        Mensaje(resultado, Name);
                        //    }
                        //    else
                        //    {
                        //        StaticSourcesViewModel.Mensaje("Error", "No se pudo crear EMI, contacte al administrador del sistema.", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR, 5);
                        //    }
                        //}
                        //else
                        //{
                        //    var resultado = string.Empty;
                        //    foreach (var UsoDroga in drogas)
                        //    {
                        //        UsoDroga.ID_EMI = emiActual.ID_EMI;
                        //        UsoDroga.ID_EMI_CONS = emiActual.ID_EMI_CONS;
                        //        if (new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMIUsoDroga().Obtener(UsoDroga.ID_EMI, UsoDroga.ID_DROGA) != null)
                        //            resultado = new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMIUsoDroga().Actualizar(UsoDroga);
                        //        else
                        //            resultado = new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMIUsoDroga().Agregar(UsoDroga);
                        //        Mensaje(resultado, Name);
                        //    }
                        //    //LstUsoDrogas = new ObservableCollection<SSP.Servidor.EMI_USO_DROGA>(new cEMIUsoDroga().Obtener(emiActual.ID_EMI, emiActual.ID_EMI_CONS));

                        //}

                        #endregion
                    }
                }
                else
                    AvisoImputadoVacio();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al grabar uso droga", ex);
            }
        }

        /// <summary>
        /// Guardar datos de homosexualidad, pandillas y sexualidad
        /// </summary>
        private void GuardarHomosexualidadPandillaSexualidad()
        {
            try
            {
                var Name = "Homosexualidad, Pandillas y Sexualidad";
                if (SelectIngreso != null)
                {
                    var entidad = new EMI_HPS();
                    entidad.ID_EMI = emiActual.ID_EMI;
                    entidad.ID_EMI_CONS = emiActual.ID_EMI_CONS;
                    entidad.VIVIO_CALLE_ORFANATO_NINO = VivioCalleOrfanato;
                    entidad.PERTENECE_PANDILLA_ACTUAL = PertenecePandilla;
                    entidad.ID_PANDILLA = PandillaNombre != -1 ? PandillaNombre : null;
                    entidad.PANDILLA = NombrePandilla;//campo en string en base a requerimientos.
                    entidad.COMPORTAMIENTO_HOMOSEXUAL = Homosexual;
                    entidad.EDAD_INICIAL_HOMOSEXUAL = HomosexualEdadIncial;
                    entidad.ROL_HOMOSEXUAL = HomosexualRol;
                    entidad.ID_HOMO = Id_Homo.HasValue ? !Id_Homo.Value.Equals(-1) ? Id_Homo : new short?() : new short?();
                    entidad.PERTENECIO_PANDILAS_EXTERIOR = PertenecioPandillaExterior;
                    entidad.EDAD_INICIAL_PANDILLAS = PandillaExteriorEdadInicial;
                    entidad.MOTIVOS_PERTENENCIA_PANDILLAS = PandillaExteriorMotivo;
                    entidad.VAGANCIA = Vagancia;
                    entidad.EDAD_INICIAL_VAGANCIA = VaganciaEdadIncial;
                    entidad.MOTIVOS_VAGANCIA = VaganciaMotivos;
                    entidad.CICATRICES = Cicatrices;
                    entidad.EDAD_INICIO_CICATRICES = CicatricesEdadIncial;
                    entidad.MOTIVO_CICATRICES = CicatricesMotivo;
                    entidad.CICATRIZ_POR_RINA = CicatricesRina == true ? "S" : "N";
                    entidad.DESERCION_ESCOLAR = DesercionEscolar;
                    entidad.MOTIVO_DESERCION_ESCOLAR = DesercionMotivo;
                    entidad.REPROBACION_ESCOLAR = ReprobacionEscolar;

                    if (ReprobacionEscolar != null)
                        if (ReprobacionEscolar.Equals("S"))
                            entidad.GRADO_REPROBACION_ESCOLAR = ReprobacionGrado.HasValue ? !ReprobacionGrado.Value.Equals(-1) ? ReprobacionGrado : new short?() : new short?();
                        else
                            entidad.GRADO_REPROBACION_ESCOLAR = null;

                    entidad.MOTIVO_REPROBACION_ESCOLAR = ReprobacionEscolarMotivo;
                    entidad.EXPULSION_ESCOLAR = ExplusionEscolar;
                    entidad.MOTIVO_EXPULSION_ESCOLAR = ExplusionEscolarMotivo;

                    if (ExplusionEscolar != null)
                        if (ExplusionEscolar.Equals("S"))
                            entidad.GRADO_EXPULSION_ESCOLAR = ExpulsionGrado.HasValue ? !ExpulsionGrado.Value.Equals(-1) ? ExpulsionGrado : new short?() : new short?();
                        else
                            entidad.GRADO_EXPULSION_ESCOLAR = null;

                    entidad.PAGA_SEXUAL_HOMBRE = ConHombres == true ? "S" : "N";
                    entidad.PAGA_SEXUAL_MUJER = ConMujeres == true ? "S" : "N";
                    entidad.PROSTITUIA_HOMBRES = SeProstituiaHombres == true ? "S" : "N";
                    entidad.PROSTITUIA_MUJERES = SeProstituiaMujeres == true ? "S" : "N";

                    if (MotivoProstituye != -1)
                        entidad.PROSTITUYE_POR = MotivoProstituye;
                    if (emiActual.EMI_HPS == null)//AGREGAR
                    {
                        if (PInsertar)
                        {
                            if (new cEMIHPS().Agregar(entidad))
                            {
                                emiActual.EMI_HPS = entidad;
                                TatuajesEnabled = true;
                                Mensaje(true, Name);
                            }
                            else
                                Mensaje(false, Name);
                        }
                        else
                            StaticSourcesViewModel.Mensaje("Validación", "Su usuario no cuenta con permisos para realizar esta acción.", StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA, 5);
                    }
                    else//ACTUALIZAR
                    {
                        if (PEditar)
                            Mensaje(new cEMIHPS().Actualizar(entidad), Name);
                        else
                            StaticSourcesViewModel.Mensaje("Validación", "Su usuario no cuenta con permisos para realizar esta acción.", StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA, 5);
                    }
                    #region guardado
                    //if (SelectIngreso.EMI.Count <= 0 || SelectIngreso.EMI == null)
                    //{
                    //    var result = AddNewEMI();
                    //    if (result.ToLower().Contains("correctamente"))
                    //    {
                    //        entidad.ID_EMI = emiActual.ID_EMI;
                    //        entidad.ID_EMI_CONS = emiActual.ID_EMI_CONS;
                    //        var resultado = new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMIHPS().Agregar(entidad);
                    //        Mensaje(resultado, Name);
                    //    }
                    //    else
                    //    {
                    //        StaticSourcesViewModel.Mensaje("Error", "No se pudo crear EMI, contacte al administrador del sistema.", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR, 5);
                    //    }
                    //}
                    //else
                    //{
                    //    var _ = SelectIngreso;
                    //    entidad.ID_EMI = emiActual.ID_EMI;
                    //    entidad.ID_EMI_CONS = emiActual.ID_EMI_CONS;
                    //    var resultado = string.Empty;
                    //    if (new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMIHPS().Obtener(entidad.ID_EMI) != null)
                    //        resultado = new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMIHPS().Actualizar(entidad);
                    //    else
                    //        resultado = new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMIHPS().Agregar(entidad);
                    //    Mensaje(resultado, Name);
                    //}

                    #endregion
                }
                else
                    AvisoImputadoVacio();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al grabar HPS", ex);
            }
        }

        /// <summary>
        /// Guarda la cantidad de tatuajes del interno hechos fuero y dentro del centro
        /// </summary>
        private void GuardarTatuajes()
        {
            try
            {
                var Name = "Tatuajes";
                if (SelectIngreso != null)
                {
                    var entidad = new EMI_TATUAJE();
                    entidad.ID_EMI = emiActual.ID_EMI;
                    entidad.ID_EMI_CONS = emiActual.ID_EMI_CONS;
                    entidad.ANTISOCIAL_AI = CantidadAntesIngresoAntisocial;
                    entidad.ANTISOCIAL_I = CantidadIntramurosAntisocial;
                    entidad.EROTICO_AI = CantidadAntesIngresoErotico;
                    entidad.EROTICO_I = CantidadIntramurosErotico;
                    entidad.RELIGIOSO_AI = CantidadAntesIngresoReligioso;
                    entidad.RELIGIOSO_I = CantidadIntramurosReligioso;
                    entidad.IDENTIFICACION_AI = CantidadAntesIngresoIdentificacion;
                    entidad.IDENTIFICACION_I = CantidadIntramurosIdentificacion;
                    entidad.DECORATIVO_AI = CantidadAntesIngresoDecorativo;
                    entidad.DECORATIVO_I = CantidadIntramurosDecorativo;
                    entidad.SENTIMENTAL_AI = CantidadAntesIngresoSentimental;
                    entidad.SENTIMENTAL_I = CantidadIntramurosSentimental;
                    entidad.TOTAL_TATUAJES = short.Parse(TatuajesTotal == null ? "0" : TatuajesTotal.ToString());
                    entidad.DESCR = TatuajesDescripcion;

                    if (emiActual.EMI_TATUAJE == null)//AGREGAR
                    {
                        if (PInsertar)
                        {
                            if (new cEMITatuaje().Agregar(entidad))
                            {
                                emiActual.EMI_TATUAJE = entidad;
                                SeniasParticularesEnabled = true;
                                Mensaje(true, Name);
                            }
                            else
                                Mensaje(false, Name);
                        }
                        else
                            StaticSourcesViewModel.Mensaje("Validación", "Su usuario no cuenta con permisos para realizar esta acción.", StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA, 5);
                    }
                    else//EDITAR
                    {
                        if (PEditar)
                            Mensaje(new cEMITatuaje().Actualizar(entidad), Name);
                        else
                            StaticSourcesViewModel.Mensaje("Validación", "Su usuario no cuenta con permisos para realizar esta acción.", StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA, 5);
                    }

                    #region guardado
                    //if (SelectIngreso.EMI.Count <= 0 || SelectIngreso.EMI == null)
                    //{
                    //    var result = AddNewEMI();
                    //    if (result.ToLower().Contains("correctamente"))
                    //    {
                    //        entidad.ID_EMI = emiActual.ID_EMI;
                    //        entidad.ID_EMI_CONS = emiActual.ID_EMI_CONS;
                    //        var resultado = new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMITatuaje().Agregar(entidad);
                    //        Mensaje(resultado, Name);
                    //    }
                    //    else
                    //    {
                    //        StaticSourcesViewModel.Mensaje("Error", "No se pudo crear EMI, contacte al administrador del sistema.", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR, 5);
                    //    }
                    //}
                    //else
                    //{
                    //    var _ = SelectIngreso;
                    //    entidad.ID_EMI = emiActual.ID_EMI;
                    //    entidad.ID_EMI_CONS = emiActual.ID_EMI_CONS;
                    //    var resultado = string.Empty;
                    //    if (new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMITatuaje().Obtener(entidad.ID_EMI) != null)
                    //        resultado = new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMITatuaje().Actualizar(entidad);
                    //    else
                    //        resultado = new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMITatuaje().Agregar(entidad);
                    //    Mensaje(resultado, Name);
                    //}

                    #endregion
                }
                else
                    AvisoImputadoVacio();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar tatuaje", ex);
            }
        }

        /// <summary>
        /// Guarda informacion de las enfermedades que padece el interno
        /// </summary>
        private void GuardarEnfermedades()
        {
            try
            {
                var Name = "Enfermedades";
                if (SelectIngreso != null)
                {
                    var entidad = new EMI_ENFERMEDAD();
                    entidad.ID_EMI = emiActual.ID_EMI;
                    entidad.ID_EMI_CONS = emiActual.ID_EMI_CONS;
                    entidad.DESCR_ENFERMEDAD = DescripcionPresentarlasAntecedentes;
                    entidad.APFISICA_ALINADO = AparienciaFisicaAlineado;
                    entidad.APFISICA_CONFORMADO = AparienciaFisicaConformado;
                    entidad.APFISICA_INTEGRO = AparienciaFisicaIntegro;
                    entidad.APFISICA_LIMPIO = AparienciaFisicaLimpio;
                    entidad.DISCAPACIDAD = Discapacidades;
                    entidad.DESCR_DISCAPACIDAD = DiscapacidadesMotivo;
                    entidad.ENFERMO_MENTAL = EnfermoMental;
                    entidad.DESCR_ENFERMO_MENTAL = EnfermoMentalMotivo;
                    entidad.VIH_HEPATITIS = VIHHepatitis;
                    entidad.EN_TRATAMIENTO_FARMACO = VIHHepatitisTratamientoFarmaco;
                    entidad.DIAGNOSTICO_FORMAL = VIHHepatitisDiagnosticoFormal;

                    if (emiActual.EMI_ENFERMEDAD == null)//AGREGAR
                    {
                        if (PInsertar)
                        {
                            if (new cEMIEnfermedad().Agregar(entidad))
                            {
                                emiActual.EMI_ENFERMEDAD = entidad;
                                ActividadesEnabled = ClasCriminologicaEnabled = ClasificacionCriminologicaEnabled = true;
                                Mensaje(true, Name);
                            }
                            else
                                Mensaje(false, Name);
                        }
                        else
                            StaticSourcesViewModel.Mensaje("Validación", "Su usuario no cuenta con permisos para realizar esta acción.", StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA, 5);
                    }
                    else//ACTUALIZAR
                    {
                        if (PEditar)
                            Mensaje(new cEMIEnfermedad().Actualizar(entidad), Name);
                        else
                            StaticSourcesViewModel.Mensaje("Validación", "Su usuario no cuenta con permisos para realizar esta acción.", StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA, 5);
                    }

                    #region guardado
                    //if (SelectIngreso.EMI.Count <= 0 || SelectIngreso.EMI == null)
                    //{
                    //    var result = AddNewEMI();
                    //    if (result.ToLower().Contains("correctamente"))
                    //    {
                    //        entidad.ID_EMI = emiActual.ID_EMI;
                    //        entidad.ID_EMI_CONS = emiActual.ID_EMI_CONS;
                    //        var resultado = new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMIEnfermedad().Agregar(entidad);
                    //        Mensaje(resultado, Name);
                    //    }
                    //    else
                    //    {
                    //        StaticSourcesViewModel.Mensaje("Error", "No se pudo crear EMI, contacte al administrador del sistema.", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR, 5);
                    //    }
                    //}
                    //else
                    //{
                    //    var _ = SelectIngreso;
                    //    entidad.ID_EMI = emiActual.ID_EMI;
                    //    entidad.ID_EMI_CONS = emiActual.ID_EMI_CONS;
                    //    var resultado = string.Empty;
                    //    if (new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMIEnfermedad().Obtener(entidad.ID_EMI) != null)
                    //        resultado = new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMIEnfermedad().Actualizar(entidad);
                    //    else
                    //        resultado = new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMIEnfermedad().Agregar(entidad);

                    //    Mensaje(resultado, Name);
                    //}

                    #endregion
                }
                else
                    AvisoImputadoVacio();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar enfermedad", ex);
            }
        }

        private void GuardarMenu()
        {
            try
            {
                var title = string.Empty;
                if (TabFichaIdentificacion)
                {
                    title = "Ficha de Identificación";
                    setValidacionesFichaIdentificacion();
                    if (!base.HasErrors)
                    {
                        try
                        {
                            GuardarFichaIdentificacion();
                            //MensajeDialogo(title, !HasErrors);
                        }
                        catch (Exception ex)
                        {
                            Mensaje(ex.InnerException != null ? ex.InnerException.Message : ex.Message, title);
                        }
                    }
                }
                else
                    if (TabSituacionJuridicaSelected)
                    {
                        setValidacionesSituacionJuridica();
                        if (!base.HasErrors)
                        {
                            if (TabEstudioTrasladoSelected)
                            {
                                title = "Estudios y Traslados";
                                try
                                {
                                    GuardarEstudiosTraslados();
                                    //MensajeDialogo(title, !HasErrors);
                                }
                                catch (Exception ex)
                                {
                                    Mensaje(ex.InnerException != null ? ex.InnerException.Message : ex.Message, title);
                                }
                            }
                            else
                                if (TabIngresoAnteriorSelected)
                                {
                                    title = "Ingresos Anteriores CE.RE.SO.";
                                    try
                                    {
                                        GuardarIngresoAnterior();
                                        //MensajeDialogo(title, !HasErrors);
                                    }
                                    catch (Exception ex)
                                    {
                                        Mensaje(ex.InnerException != null ? ex.InnerException.Message : ex.Message, title);
                                    }
                                }
                                else
                                    if (TabIngresoAnteriorMenorSelected)
                                    {
                                        title = "Ingresos Anteriores Menores";
                                        try
                                        {
                                            GuardarIngresoAnteriorMenor();
                                            //MensajeDialogo(title, !HasErrors);
                                        }
                                        catch (Exception ex)
                                        {
                                            Mensaje(ex.InnerException != null ? ex.InnerException.Message : ex.Message, title);
                                        }
                                    }
                        }
                        else
                            MensajeDialogo("Situación Jurídica", !HasErrors);
                    }
                    else
                        if (TabFactoresSocioFamiliaresSelected)
                        {
                            if (TabFactorSelected)
                            {
                                setValidacionesFactores();
                                if (!base.HasErrors)
                                {
                                    title = "Factores";
                                    try
                                    {
                                        GuardarFactores();
                                        //MensajeDialogo(title, !HasErrors);
                                    }
                                    catch (Exception ex)
                                    {
                                        Mensaje(ex.InnerException != null ? ex.InnerException.Message : ex.Message, title);
                                    }
                                }
                            }
                            else
                                if (TabGrupoFamiliarSelected)
                                {
                                    title = "Datos Grupo Familiar";
                                    try
                                    {
                                        setValidacionesDatosGrupoFamiliar();
                                        if (!base.HasErrors)
                                            GuardarDatosGrupoFamiliar();
                                        //MensajeDialogo(title, !HasErrors);
                                    }
                                    catch (Exception ex)
                                    {
                                        Mensaje(ex.InnerException != null ? ex.InnerException.Message : ex.Message, title);
                                    }
                                }
                                else
                                    if (TabGrupoFamiliarAntecedenteSelected)
                                    {
                                        title = "Antecedentes Grupo Familiar(Delictivos y Drogas)";
                                        try
                                        {
                                            //setValidacionesAntecedentesGrupoFamiliar();
                                            GuardarGFD();
                                            //MensajeDialogo(title, !HasErrors);
                                        }
                                        catch (Exception ex)
                                        {
                                            Mensaje(ex.InnerException != null ? ex.InnerException.Message : ex.Message, title);
                                        }
                                    }
                        }
                        else
                            if (TabConductasParasocialesSelected)
                            {
                                setValidacionesConductaParasocial();
                                if (TabUsoDrogaSelected)
                                {
                                    title = "Uso Drogas";
                                    try
                                    {
                                        setValidacionesUsoDrogas();
                                        if (!base.HasErrors)
                                            GuardarUsoDroga();
                                        //MensajeDialogo(title, !HasErrors);
                                    }
                                    catch (Exception ex)
                                    {
                                        Mensaje(ex.InnerException != null ? ex.InnerException.Message : ex.Message, title);
                                    }
                                }
                                else
                                    if (TabHPSSelected)
                                    {
                                        title = "Homosexualidad, Pandillas y Sexualidad";
                                        try
                                        {
                                            setValidacionesHPS();
                                            if (!base.HasErrors)
                                                GuardarHomosexualidadPandillaSexualidad();
                                            //MensajeDialogo(title, !HasErrors);
                                        }
                                        catch (Exception ex)
                                        {
                                            Mensaje(ex.InnerException != null ? ex.InnerException.Message : ex.Message, title);
                                        }
                                    }
                                    else
                                        if (TabTatuajesSelected)
                                        {
                                            title = "Tatuajes";
                                            try
                                            {
                                                setValidacionesTatuajes();
                                                if (!base.HasErrors)
                                                    GuardarTatuajes();
                                                //MensajeDialogo(title, !HasErrors);
                                            }
                                            catch (Exception ex)
                                            {
                                                Mensaje(ex.InnerException != null ? ex.InnerException.Message : ex.Message, title);
                                            }
                                        }
                                        else
                                            if (TabSeniaParticularSelected)
                                            {
                                                title = "Señas Particulares";
                                                try
                                                {
                                                    if (ValidarSeniasParticulares())
                                                    {
                                                        //MensajeDialogo(title, GuardarSenasParticulares());
                                                        GuardarSenasParticulares();
                                                            
                                                    }
                                                    else
                                                        new Dialogos().ConfirmacionDialogo("Notificación", "Favor de capturar los campos necesarios para guardar la seña particular (ubicación,tipo,cantidad)"); //MensajeDialogo(title, false);
                                                }
                                                catch (Exception ex)
                                                {
                                                    Mensaje(ex.InnerException != null ? ex.InnerException.Message : ex.Message, title);
                                                }
                                            }
                                            else
                                                if (TabEnfermedadesSelected)
                                                {
                                                    title = "Enfermedades";
                                                    try
                                                    {
                                                        setValidacionesEnfermedades();
                                                        if (!base.HasErrors)
                                                            GuardarEnfermedades();
                                                        //MensajeDialogo(title, !HasErrors);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Mensaje(ex.InnerException != null ? ex.InnerException.Message : ex.Message, title);
                                                    }
                                                }
                            }
                            else
                                if (TabActividadesSelected)
                                {
                                    title = "Actividades";
                                    try
                                    {
                                        GuardarActividades();
                                        //MensajeDialogo(title, true);
                                    }
                                    catch (Exception ex)
                                    {
                                        Mensaje(ex.InnerException != null ? ex.InnerException.Message : ex.Message, title);
                                    }
                                }
                                else
                                    if (TabClasificacionCriminologicaPadreSelected)
                                    {
                                        if (TabFactorCriminodiagnosticoSelected)
                                        {
                                            title = "Factor Crimidiagnóstico";
                                            try
                                            {
                                                setValidacionesCriminodiagnostico();
                                                if (!base.HasErrors)
                                                    GuardarFactorCrimidiagnostico();
                                                //MensajeDialogo(title, !HasErrors);
                                            }
                                            catch (Exception ex)
                                            {
                                                Mensaje(ex.InnerException != null ? ex.InnerException.Message : ex.Message, title);
                                            }
                                        }
                                        else
                                            if (TabClasificacionCriminologicaSelected)
                                            {
                                                title = "Clasificación Criminológica";
                                                try
                                                {
                                                    setValidacionesClasificacionCriminologica();
                                                    if (!base.HasErrors)
                                                        GuardarClasCriminologica();
                                                    //MensajeDialogo(title, !HasErrors);
                                                }
                                                catch (Exception ex)
                                                {
                                                    Mensaje(ex.InnerException != null ? ex.InnerException.Message : ex.Message, title);
                                                }
                                            }
                                    }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al grabar", ex);
            }
        }

        /// <summary>
        /// Agrega un nuevo EMI en caso de que el actual sea nulo, o no haya ninguno.
        /// </summary>
        /// <returns>Cadena de texto con el resultado del proceso de guardado.</returns>
        private bool AddNewEMI()
        {
            try
            {
                var _ = SelectIngreso;
                //emiActual = new EMI_INGRESO { ID_IMPUTADO = _.ID_IMPUTADO, ESTATUS = "A", ID_INGRESO = _.ID_INGRESO, ID_ANIO = _.ID_ANIO, ID_CENTRO = _.ID_CENTRO, ID_EMI_CONS = 1 };
                var idimputado = AgregarCerosDelante(4 - _.ID_IMPUTADO.ToString().Length, _.ID_IMPUTADO.ToString());
                var idingreso = AgregarCerosDelante(2 - _.ID_INGRESO.ToString().Length, _.ID_INGRESO.ToString());
                var idemi = string.Format("{0}{1}{2}{3}", _.ID_CENTRO, _.ID_ANIO.ToString().Substring(2, 2), idimputado, idingreso);
                emiActual.ID_EMI = int.Parse(idemi);
                //INGRESO
                var ingreso = new EMI_INGRESO();
                ingreso.ID_CENTRO = SelectIngreso.ID_CENTRO;
                ingreso.ID_ANIO = SelectIngreso.ID_ANIO;
                ingreso.ID_IMPUTADO = SelectIngreso.ID_IMPUTADO;
                ingreso.ID_INGRESO = SelectIngreso.ID_INGRESO;
                ingreso.FEC_CAPTURA = Fechas.GetFechaDateServer;
                emiActual.EMI_INGRESO = ingreso;
                emiActual.ESTATUS = "P"; //En proceso
                emiActual.FEC_CAPTURA = Fechas.GetFechaDateServer;

                if (new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMI().Agregar(emiActual))
                {
                    emiActual = new cEMI().Obtener(emiActual.ID_EMI).FirstOrDefault();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar nuevo EMI", ex);
                return false;
            }
        }

        private void LimpiarUsoDroga()
        {
            try
            {
                SelectedUsoDroga = null;
                popUpUsoDroga = new EMI_USO_DROGA();
                popUpEdadInicio = 0;
                popUpDrogaId = -1;
                popUpFrecuenciaUso = -1;
                popUpConsumoActual = "N";
                popUpTiempoConsumo = 1;
                popUpFechaUltDosis = new Nullable<DateTime>();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar uso droga", ex);
            }
        }

        private void AgregarUsoDroga()
        {
            try
            {
                if (LstUsoDrogas == null)
                    LstUsoDrogas = new ObservableCollection<EMI_USO_DROGA>();
                if (SelectedUsoDroga == null)
                {
                    popUpUsoDroga.DROGA = new SSP.Controlador.Catalogo.Justicia.cDrogas().Obtener(int.Parse(popUpUsoDroga.ID_DROGA.ToString()));
                    popUpUsoDroga.FEC_ULTIMA_DOSIS = popUpFechaUltDosis;

                    var droga_repetido = LstUsoDrogas.Where(w => w.ID_DROGA == popUpUsoDroga.ID_DROGA).Count();
                    if (droga_repetido > 0)
                    {
                        new Dialogos().ConfirmacionDialogo("Notificación", "La droga seleccionada ya existe");
                    }
                    else
                    {
                        LstUsoDrogas.Add(popUpUsoDroga);
                    }
                }
                else
                {
                    SelectedUsoDroga = popUpUsoDroga;
                    LstUsoDrogas = new ObservableCollection<EMI_USO_DROGA>(LstUsoDrogas);
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar uso droga", ex);
            }
        }

        private void EliminarUsoDrogas()
        {
            try
            {
                if (SelectedUsoDroga != null)
                {
                    lstUsosEliminar.Add(SelectedUsoDroga);
                    LstUsoDrogas.Remove(SelectedUsoDroga);
                }
                else
                    StaticSourcesViewModel.Mensaje("Error", "Debe seleccionar un elemento de la lista primero.", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                EliminarItemMenu = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar uso droga", ex);
            }
        }

        //Clasificacion criminologica
        #region Clasif. Criminologica
        private void GuardarClasCriminologica()
        {
            try
            {
                var Name = "Clasificación Criminológica";
                if (SelectIngreso != null)
                {
                    var entidad = new EMI_CLAS_CRIMINOLOGICA();
                    entidad.ID_EMI = emiActual.ID_EMI;
                    entidad.ID_EMI_CONS = emiActual.ID_EMI_CONS;
                    entidad.FEC_CAPTURA = Fechas.GetFechaDateServer;
                    entidad.ID_CRIMENO = PertenenciaCrimenOrganizado;
                    entidad.ID_EMI = emiActual.ID_EMI;
                    entidad.ID_EMI_CONS = emiActual.ID_EMI_CONS;
                    entidad.ID_CLAS = ClasificacionCriminologica;

                    #region Sanciones
                    var sanciones = new List<EMI_SANCION_DISCIPLINARIA>(LstSanciones == null ? null : LstSanciones.Select((w, i) => new EMI_SANCION_DISCIPLINARIA()
                    {
                        CANTIDAD_PARTICIPACION = w.CANTIDAD_PARTICIPACION,
                        MOTIVO_PROCESO = w.MOTIVO_PROCESO,
                        NUEVO_PROCESO = w.NUEVO_PROCESO,
                        TIEMPO_CASTIGO_SANCION_PROCESO = w.TIEMPO_CASTIGO_SANCION_PROCESO,
                        ID_EMI = emiActual.ID_EMI,
                        ID_EMI_CONS = emiActual.ID_EMI_CONS,
                        ID_SANCIONES_DISCIPLINARIAS = Convert.ToInt16(i + 1)
                    }));
                    #endregion

                    if (emiActual.EMI_CLAS_CRIMINOLOGICA == null)//AGREGAR
                    {
                        if (PInsertar)
                        {
                            if (new cEMIClasCriminologica().Agregar(entidad, sanciones))
                            {
                                emiActual.EMI_CLAS_CRIMINOLOGICA = entidad;
                                ClasificacionCrimidiagnosticoEnabled = true;
                                Mensaje(true, Name);
                                //return;
                            }
                            else
                                Mensaje(false, Name);
                        }
                        else
                            StaticSourcesViewModel.Mensaje("Validación", "Su usuario no cuenta con permisos para realizar esta acción.", StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA, 5);
                    }
                    else//ACTUALIZAR
                    {
                        if (PEditar)
                        {
                            if (new cEMIClasCriminologica().Actualizar(entidad, sanciones))
                            {
                                Mensaje(true, Name);
                                //return;
                            }
                            else
                                Mensaje(false, Name);
                        }
                        else
                            StaticSourcesViewModel.Mensaje("Validación", "Su usuario no cuenta con permisos para realizar esta acción.", StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA, 5);
                    }

                    #region guardado de clasificacion criminologica
                    //if (SelectIngreso.EMI.Count <= 0 || SelectIngreso.EMI == null)
                    //{
                    //    var result = AddNewEMI();
                    //    if (result.ToLower().Contains("correctamente"))
                    //    {
                    //        entidad.ID_EMI = emiActual.ID_EMI;
                    //        entidad.ID_EMI_CONS = emiActual.ID_EMI_CONS;
                    //        var resultado = new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMIClasCriminologica().Agregar(entidad);
                    //        Mensaje(resultado, Name);
                    //    }
                    //    else
                    //    {
                    //        StaticSourcesViewModel.Mensaje("Error", "No se pudo crear EMI, contacte al administrador del sistema.", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR, 5);
                    //    }
                    //}
                    //else
                    //{
                    //    entidad.ID_EMI = emiActual.ID_EMI;
                    //    entidad.ID_EMI_CONS = emiActual.ID_EMI_CONS;
                    //    var resultado = string.Empty;
                    //    if (new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMIClasCriminologica().Obtener(entidad.ID_EMI, entidad.ID_EMI_CONS) != null)
                    //        resultado = new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMIClasCriminologica().Actualizar(entidad);
                    //    else
                    //        resultado = new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMIClasCriminologica().Agregar(entidad);
                    //    Mensaje(resultado, Name);
                    //}

                    #endregion

                    #region Comentado
                    //var ListaNueva = new List<EMI_SANCION_DISCIPLINARIA>();
                    //int indice = 1;
                    //if (LstSanciones != null)
                    //    foreach (var sancion in LstSanciones)
                    //    {
                    //        ListaNueva.Add(
                    //            new EMI_SANCION_DISCIPLINARIA
                    //        {
                    //            CANTIDAD_PARTICIPACION = sancion.CANTIDAD_PARTICIPACION,
                    //            MOTIVO_PROCESO = sancion.MOTIVO_PROCESO,
                    //            NUEVO_PROCESO = sancion.NUEVO_PROCESO,
                    //            TIEMPO_CASTIGO_SANCION_PROCESO = sancion.TIEMPO_CASTIGO_SANCION_PROCESO,
                    //            ID_EMI = emiActual.ID_EMI,
                    //            ID_EMI_CONS = emiActual.ID_EMI_CONS,
                    //            ID_SANCIONES_DISCIPLINARIAS = indice
                    //        });
                    //        indice++;
                    //    }
                    //if (new cEMISancionDisciplinaria().Insertar(emiActual.ID_EMI, emiActual.ID_EMI_CONS, ListaNueva))
                    //{
                    //    ClasificacionCrimidiagnosticoEnabled = true;
                    //    LstSanciones = new ObservableCollection<EMI_SANCION_DISCIPLINARIA>(new cEMISancionDisciplinaria().Obtener(emiActual.ID_EMI, emiActual.ID_EMI_CONS));
                    //    Mensaje(true, Name);
                    //}
                    //else
                    //    Mensaje(false, Name);
                    #endregion
                }
                else
                    AvisoImputadoVacio();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar sanción criminológica", ex);
            }
        }

        private void PoplularSancionPopUp()
        {
            try
            {
                if (SelectedSancion == null)
                    AvisoImputadoVacio();
                else
                {
                    TituloModal = "Editar Sanción";
                    MotivoProceso = SelectedSancion.MOTIVO_PROCESO;
                    Cantidad = SelectedSancion.CANTIDAD_PARTICIPACION;
                    NuevoProceso = SelectedSancion.NUEVO_PROCESO == "S" ? true : false;
                    TiempoSancionProceso = SelectedSancion.TIEMPO_CASTIGO_SANCION_PROCESO;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer sanción", ex);
            }
        }

        private void AddSancion()
        {
            try
            {
                if (LstSanciones == null)
                    LstSanciones = new ObservableCollection<EMI_SANCION_DISCIPLINARIA>();
                NewSancion.ID_SANCIONES_DISCIPLINARIAS = LstSanciones.LastOrDefault() != null ? LstSanciones.LastOrDefault().ID_SANCIONES_DISCIPLINARIAS + 1 : 1;

                if (SelectedSancion == null)
                    LstSanciones.Add(NewSancion);
                else
                {
                    SelectedSancion = NewSancion;
                    LstSanciones = new ObservableCollection<EMI_SANCION_DISCIPLINARIA>(LstSanciones);
                }
                if (LstSanciones.Count > 0)
                    SancionesEmpty = false;
                else
                    SancionesEmpty = true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar sanción", ex);
            }
        }

        private void EliminarSancion()
        {
            try
            {
                if (SelectedSancion != null)
                {
                    LstSanciones.Remove(SelectedSancion);
                }
                else
                    StaticSourcesViewModel.Mensaje("Error", "Debe seleccionar un elemento de la lista primero.", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                EliminarItemMenu = false;
                if (LstSanciones.Count > 0)
                    SancionesEmpty = false;
                else
                    SancionesEmpty = true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar sanción", ex);
            }
        }

        private void LimpiarClasCriminologica()
        {
            try
            {
                SelectedSancion = null;
                NewSancion = new EMI_SANCION_DISCIPLINARIA();
                Cantidad = 0;
                MotivoProceso = string.Empty;
                TiempoSancionProceso = string.Empty;
                NuevoProceso = false;
                EliminarItemMenu = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar clasificación criminológica", ex);
            }
        }
        #endregion
        #endregion

        #region EXTRAS
        /// <summary>
        /// Agrega digitos '0' al inicio de una cadena de texto.
        /// </summary>
        /// <param name="cantidad">Número de digitos '0' que se agregarán.</param>
        /// <param name="cadena">Cadena de texto que será utilizada.</param>
        /// <returns>Cadena de texto resultante de la adición de los dígitos.</returns>
        private string AgregarCerosDelante(int cantidad, string cadena)
        {
            var lista = new List<char>();
            try
            {
                lista = cadena.ToList();
                for (int i = 1; i <= cantidad; i++)
                {
                    lista.Insert(0, '0');
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar ceros adelante", ex);
            }
            return new string(lista.ToArray());
        }

        /// <summary>
        /// Reacomoda los elementos de las listas en orden alfabético.
        /// </summary>
        private void PrepararListas()
        {
            try
            {
                //EdadAbuso = EdadInicioContactoSexual = EdadInternoSeparacionPadres = HomosexualEdadIncial = VaganciaEdadIncial = Hijos = Cantidad = TotalParejas = 0;
                #region Instanciacion

                if (LstDrogas == null)
                {
                    LstDrogas = new ObservableCollection<DROGA>(new cDrogas().ObtenerTodos().OrderBy(s => s.DESCR));
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstDrogas.Insert(0, (new DROGA() { ID_DROGA = -1, DESCR = "SELECCIONE" }));
                    }));
                }
                if (LstFrecuenciasUsoDrogas == null)
                {
                    LstFrecuenciasUsoDrogas = new ObservableCollection<DROGA_FRECUENCIA>(new cDrogaFrecuencia().Obtener().OrderBy(s => s.DESCR));
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstFrecuenciasUsoDrogas.Insert(0, (new DROGA_FRECUENCIA() { ID_FRECUENCIA = -1, DESCR = "SELECCIONE" }));
                    }));
                }
                if (LstEmisor == null)
                {
                    LstEmisor = new ObservableCollection<EMISOR>(new cEmisor().Obtener().OrderBy(s => s.DESCR));
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstEmisor.Insert(0, (new EMISOR() { ID_EMISOR = -1, DESCR = "SELECCIONE" }));
                    }));
                }
                if (LstMotivosProstituye == null)
                {
                    LstMotivosProstituye = new ObservableCollection<MOTIVO_PROSTITUCION>(new SSP.Controlador.Catalogo.Justicia.cMotivoProstitucion().Obtener().OrderBy(s => s.DESCR));
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstMotivosProstituye.Insert(0, (new MOTIVO_PROSTITUCION() { ID_MOTIVO = -1, DESCR = "SELECCIONE" }));
                    }));
                }
                if (LstComportamientoHomo == null)
                {
                    LstComportamientoHomo = new ObservableCollection<COMPORTAMIENTO_HOMO>(new SSP.Controlador.Catalogo.Justicia.cComportamientoHomo().ObtenerTodos().OrderBy(s => s.DESCR));
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstComportamientoHomo.Insert(0, (new COMPORTAMIENTO_HOMO() { ID_HOMO = -1, DESCR = "SELECCIONE" }));
                    }));
                }
                if (LstDelitos == null)
                {
                    LstDelitos = new ObservableCollection<INGRESO_DELITO>(new cIngresoDelito().ObtenerTodos().OrderBy(s => s.DESCR));
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstDelitos.Insert(0, (new INGRESO_DELITO() { ID_INGRESO_DELITO = -1, DESCR = "SELECCIONE" }));
                    }));
                }
                if (LstDelitosCP == null)
                {
                    LstDelitosCP = new ObservableCollection<DELITO>(new cDelito().ObtenerTodos().OrderBy(s => s.DESCR));
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstDelitosCP.Insert(0, (new DELITO() { ID_DELITO = -1, DESCR = "SELECCIONE" }));
                    }));
                }
                if (LstDrogas == null)
                {
                    LstDrogas = new ObservableCollection<DROGA>(new cDrogas().ObtenerTodos().OrderBy(s => s.DESCR));
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstDrogas.Insert(0, (new DROGA() { ID_DROGA = -1, DESCR = "SELECCIONE" }));
                    }));
                }
                if (LstEstadoCivil == null)
                {
                    LstEstadoCivil = new ObservableCollection<ESTADO_CIVIL>(new cEstadoCivil().ObtenerTodos().OrderBy(s => s.DESCR));
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstEstadoCivil.Insert(0, (new ESTADO_CIVIL() { ID_ESTADO_CIVIL = -1, DESCR = "SELECCIONE" }));
                    }));
                }
                if (LstExfuncionario == null)
                {
                    LstExfuncionario = new ObservableCollection<EXFUNCIONARIO_SEGPUB>(new cExfuncionarioSegPub().Obtener().OrderBy(s => s.DESCR));
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstExfuncionario.Insert(0, (new EXFUNCIONARIO_SEGPUB() { ID_EXFUNCIONARIO = -1, DESCR = "SELECCIONE" }));
                    }));
                }
                if (LstOcupacion == null)
                {
                    LstOcupacion = new ObservableCollection<OCUPACION>(new cOcupacion().ObtenerTodos().OrderBy(s => s.DESCR));
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstOcupacion.Insert(0, (new OCUPACION() { ID_OCUPACION = -1, DESCR = "SELECCIONE" }));
                    }));
                }
                if (LstPandillas == null)
                {
                    LstPandillas = new ObservableCollection<PANDILLA>(new cPandilla().ObtenerTodos().OrderBy(s => s.NOMBRE));
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstPandillas.Insert(0, (new PANDILLA() { ID_PANDILLA = -1, NOMBRE = "SELECCIONE" }));
                    }));
                }
                if (LstTipoRelacion == null)
                {
                    LstTipoRelacion = new ObservableCollection<TIPO_RELACION>(new cTipoRelacion().ObtenerTodos().OrderBy(s => s.DESCR));
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstTipoRelacion.Insert(0, (new TIPO_RELACION() { ID_RELACION = -1, DESCR = "SELECCIONE" }));
                    }));
                }
                if (LstFrecuencias == null)
                {
                    LstFrecuencias = new ObservableCollection<FRECUENCIA>(new cFrecuencia().Obtener().OrderBy(s => s.DESCR));
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstFrecuencias.Insert(0, (new FRECUENCIA() { ID_FRECUENCIA = -1, DESCR = "SELECCIONE" }));
                    }));
                }
                if (LstFactoresAbandono == null)
                {
                    LstFactoresAbandono = new ObservableCollection<EMI_FACTORES_ABANDONO>(new cEMIFactoresAbandono().Obtener().OrderBy(s => s.DESCR));
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstFactoresAbandono.Insert(0, (new EMI_FACTORES_ABANDONO() { ID_ABANDONO = -1, DESCR = "SELECCIONE" }));
                    }));
                }
                if (LstFactoresHuida == null)
                {
                    LstFactoresHuida = new ObservableCollection<EMI_FACTORES_HUIDA>(new cEMIFactoresHuida().Obtener().OrderBy(s => s.DESCR));
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstFactoresHuida.Insert(0, (new EMI_FACTORES_HUIDA() { ID_HUIDA = -1, DESCR = "SELECCIONE" }));
                    }));
                }
                if (LstClasCrim == null)
                {
                    LstClasCrim = new ObservableCollection<SSP.Servidor.CLASIFICACION_CRIMINOLOGICA>(new cClasificacionCriminologica().Obtener().OrderBy(s => s.DESCR));
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstClasCrim.Insert(0, (new CLASIFICACION_CRIMINOLOGICA() { ID_CLAS = -1, DESCR = "SELECCIONE" }));
                    }));
                }
                if (LstPertCrimenOrg == null)
                {
                    LstPertCrimenOrg = new ObservableCollection<PERTENECE_CRIMEN_ORG>(new cPerteneceCrimenOrg().Obtener().OrderBy(s => s.DESCR));
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstPertCrimenOrg.Insert(0, (new PERTENECE_CRIMEN_ORG() { ID_CRIMENO = -1, DESCR = "SELECCIONE" }));
                    }));
                }
                if (LstFactorNivel == null)
                {
                    LstFactorNivel = new ObservableCollection<EMI_FACTOR_NIVEL>(new cEMIFactorNivel().Obtener().OrderBy(s => s.DESCR));
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstFactorNivel.Insert(0, (new EMI_FACTOR_NIVEL() { ID_NIVEL = -1, DESCR = "SELECCIONE" }));
                    }));
                }
                if (LstFactorResultado == null)
                {
                    LstFactorResultado = new ObservableCollection<EMI_FACTOR_RESULTADO>(new cEMIFactorResultado().Obtener().OrderBy(s => s.DESCR));
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstFactorResultado.Insert(0, (new EMI_FACTOR_RESULTADO() { ID_RESULTADO = -1, DESCR = "SELECCIONE" }));
                    }));
                }
                if (LstFactorUbicacion == null)
                {
                    LstFactorUbicacion = new ObservableCollection<EMI_FACTOR_UBICACION>(new cEMIFactorUbicacion().Obtener().OrderBy(s => s.DESCR));
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstFactorUbicacion.Insert(0, (new EMI_FACTOR_UBICACION() { ID_UBICACION = -1, DESCR = "SELECCIONE" }));
                    }));
                }
                if (LstGradoEducativo == null)
                {
                    LstGradoEducativo = new ObservableCollection<EDUCACION_GRADO>(new cEducacionGrado().Obtener());
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstGradoEducativo.Insert(0, new EDUCACION_GRADO() { ID_GRADO = -1, DESCR = "SELECCIONE" });
                    }));
                }
                if (LstCertificadoEduacion == null)
                {
                    LstCertificadoEduacion = new ObservableCollection<EDUCACION_CERTIFICADO>(new cEducacionCertificado().Obtener());
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstCertificadoEduacion.Insert(0, new EDUCACION_CERTIFICADO() { ID_CERTIFICADO = -1, DESCR = "SELECCIONE" });
                    }));
                }
                if (LstTipoReferencia == null)
                {
                    LstTipoReferencia = new ObservableCollection<TIPO_REFERENCIA>(new cTipoReferencia().ObtenerTodos());
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstTipoReferencia.Insert(0, new TIPO_REFERENCIA() { ID_TIPO_REFERENCIA = -1, DESCR = "SELECCIONE" });
                    }));
                }
                if (ListClasificacionTatuaje == null)
                {
                    ListClasificacionTatuaje = new ObservableCollection<TATUAJE_CLASIFICACION>(new cTatuajeClasificacion().ObtenerTodos());
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ListClasificacionTatuaje.Insert(0, new TATUAJE_CLASIFICACION() { ID_TATUAJE_CLA = string.Empty, DESCR = "SELECCIONE" });
                    }));
                }
                if (ListTipoTatuaje == null)
                {
                    ListTipoTatuaje = new ObservableCollection<TATUAJE>(new cTatuaje().ObtenerTodos());
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ListTipoTatuaje.Insert(0, new TATUAJE() { ID_TATUAJE = -1, DESCR = "SELECCIONE" });
                    }));
                }
                if (ListRegionCuerpo == null)
                {
                    ListRegionCuerpo = new ObservableCollection<ANATOMIA_TOPOGRAFICA>(new cAnatomiaTopografica().ObtenerTodos());
                }
                if (LstTipoActividad == null)
                {
                    LstTipoActividad = new ObservableCollection<EMI_TIPO_ACTIVIDAD>(new cEMITipoActividad().Obtener());
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstTipoActividad.Insert(0, new EMI_TIPO_ACTIVIDAD() { ID_EMI_ACTIVIDAD = -1, DESCR = "SELECCIONE" });
                    }));
                }
                if (LstEstatusPrograma == null)
                {
                    LstEstatusPrograma = new ObservableCollection<EMI_ESTATUS_PROGRAMA>(new cEMIEstatusPrograma().ObtenerTodos());
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstEstatusPrograma.Insert(0, new EMI_ESTATUS_PROGRAMA() { ID_ESTATUS = -1, DESCR = "SELECCIONE" });
                    }));
                }
                if (LstGpoFam == null)
                {
                    LstGpoFam = new ObservableCollection<Clases.GrupoFam>();
                    LstGpoFam.Add(new Clases.GrupoFam() { Id = 1, Descr = "PRIMARIO" });
                    LstGpoFam.Add(new Clases.GrupoFam() { Id = 2, Descr = "SECUNDARIO" });
                    LstGpoFam.Add(new Clases.GrupoFam() { Id = 3, Descr = "NINGUNO" });
                }
                #endregion
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al preparar listas", ex);
            }
        }

        private void LlenarCombosFichaIdentificacion()
        {
            try
            {
                if (LstGradoEducativo == null || LstGradoEducativo.Count == 0)
                {
                    LstGradoEducativo = new ObservableCollection<EDUCACION_GRADO>(new cEducacionGrado().Obtener().OrderBy(s => s.DESCR));
                    LstGradoEducativo.Insert(0, (new EDUCACION_GRADO() { ID_GRADO = -1, DESCR = "SELECCIONE" }));
                }
                if (LstCertificadoEduacion == null || LstCertificadoEduacion.Count == 0)
                {
                    LstCertificadoEduacion = new ObservableCollection<EDUCACION_CERTIFICADO>(new cEducacionCertificado().Obtener().OrderBy(s => s.DESCR));
                    LstCertificadoEduacion.Insert(0, (new EDUCACION_CERTIFICADO() { ID_CERTIFICADO = -1, DESCR = "SELECCIONE" }));
                }
                if (LstTipoReferencia == null || LstTipoReferencia.Count == 0)
                {
                    LstTipoReferencia = new ObservableCollection<TIPO_REFERENCIA>(new cTipoReferencia().ObtenerTodos().OrderBy(s => s.DESCR));
                    LstTipoReferencia.Insert(0, (new TIPO_REFERENCIA() { ID_TIPO_REFERENCIA = -1, DESCR = "SELECCIONE" }));
                }
                if (LstEmisor == null || LstEmisor.Count == 0)
                {
                    LstEmisor = new ObservableCollection<EMISOR>(new cEmisor().Obtener().OrderBy(s => s.DESCR));
                    LstEmisor.Insert(0, (new EMISOR() { ID_EMISOR = -1, DESCR = "SELECCIONE" }));
                }
                if (LstExfuncionario == null || LstExfuncionario.Count == 0)
                {
                    LstExfuncionario = new ObservableCollection<EXFUNCIONARIO_SEGPUB>(new cExfuncionarioSegPub().Obtener().OrderBy(s => s.DESCR));
                    LstExfuncionario.Insert(0, (new EXFUNCIONARIO_SEGPUB() { ID_EXFUNCIONARIO = -1, DESCR = "SELECCIONE" }));
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al llenar combos ficha identificación", ex);
            }
        }

        /// <summary>
        /// Muestra un aviso al usuario basado en el valor de la variable.
        /// </summary>
        /// <param name="resultado">True o False deoendiendo del éxito de la operación realizada anteriormente.</param>
        /// <param name="name">Titulo a mostrar en el aviso.</param>
        private void Mensaje(bool resultado, string name)
        {
            try
            {
                StaticSourcesViewModel.Mensaje(
                    name, resultado ? "Información actualizada correctamente." : "Ocurrió un error al actualizar la información, intente de nuevo más tarde.",
                    resultado ? StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO : StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al dar mensaje", ex);
            }
        }

        /// <summary>
        /// Muestra un aviso al usuario basado en el resultado obtenido por la operación realizada anteriormente.
        /// </summary>
        /// <param name="resultado">Mensaje de status de la operación realizada anteriormente.</param>
        /// <param name="name">Titulo a mostrar en el aviso.</param>
        private void Mensaje(string resultado, string name)
        {
            try
            {
                StaticSourcesViewModel.Mensaje(name, resultado, resultado.ToLower().Contains("correctamente") ? StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO : StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al dar mensaje", ex);
            }
        }

        /// <summary>
        /// Arroja un aviso donde se le indica al usuario que no hay ingreso seleccionado.
        /// </summary>
        private void AvisoImputadoVacio()
        {
            try
            {
                StaticSourcesViewModel.Mensaje("Aviso", "Debe seleccionar un imputado antes de capturar información.", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION, 5);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al dar aviso imputado", ex);
            }
        }

        /// <summary>
        /// Deja nulas las listas que componen diferentes areas del EMI.
        /// </summary>
        private void VaciarListasEMI()
        {
            try
            {
                var props = this.GetType().GetProperties();
                foreach (PropertyInfo propiedad in props)
                {
                    var getSet = propiedad.GetAccessors();

                    if (propiedad.GetGetMethod() != null && propiedad.GetSetMethod() != null)
                    {
                        var valor = propiedad.GetValue(this, null);
                        if (valor != null)
                        {
                            if (propiedad.PropertyType.UnderlyingSystemType.FullName.ToString().ToLower().Contains("bool"))
                            {
                                Console.Write(propiedad.Name + ": ");
                                Console.WriteLine(valor.ToString());
                                propiedad.SetValue(this, false, null);
                            }
                            else if (propiedad.PropertyType.UnderlyingSystemType.FullName.ToString().ToLower().Contains("string"))
                            {
                                Console.Write(propiedad.Name + ": ");
                                Console.WriteLine(valor.ToString());
                                propiedad.SetValue(this, string.Empty, null);
                            }
                            else if (propiedad.PropertyType.UnderlyingSystemType.FullName.ToString().ToLower().Contains("int"))
                                propiedad.SetValue(this, string.Empty, null);
                            else
                                propiedad.SetValue(this, null, null);
                        }
                    }
                }
                OnPropertyChanged();
                EmiVisible = true;
                BotonesEnables = true;
                BotonesEnables2 = true;
                TatuajesVisible = false;
                Band1 = true;
                Band2 = true;
                EdadAbuso = EdadInicioContactoSexual = EdadInternoSeparacionPadres = HomosexualEdadIncial = VaganciaEdadIncial = 0;
                Hijos = 0;
                Cantidad = 0;
                TotalParejas = 0;
                //INICIALIZA VARIABLES DE BUSQUEDA
                NombreBuscar = ApellidoPaternoBuscar = ApellidoMaternoBuscar = string.Empty;
                TextBotonSeleccionarIngreso = "Seleccionar ingreso";

                //MODALS
                AgregarTrabajoVisible = false;
                TabFichaIdentificacion = true;
                FichaIdentificacionLoad();
                OnPropertyChanged();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al vaciar listas emi", ex);
            }
        }

        /// <summary>
        /// Arroja un aviso al usuario donde se le indica que en la lista ya existe un registro con las mismas características que el que intenta agregar.
        /// </summary>
        private void AvisoDuplicado()
        {
            try
            {
                StaticSourcesViewModel.Mensaje("Registro duplicado", "Actualmente ya existe un registro con este consumo, considere editarlo.", StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al dar aviso duplicado", ex);
            }
        }

        /// <summary>
        /// Cuenta los tatuajes capturados
        /// </summary>
        /// <returns></returns>
        private int? ContarTatuajes()
        {
            try
            {
                return CantidadAntesIngresoAntisocial + CantidadAntesIngresoDecorativo + CantidadAntesIngresoErotico + CantidadAntesIngresoIdentificacion + CantidadAntesIngresoReligioso + CantidadAntesIngresoSentimental + CantidadIntramurosAntisocial + CantidadIntramurosDecorativo + CantidadIntramurosErotico + CantidadIntramurosIdentificacion + CantidadIntramurosReligioso + CantidadIntramurosSentimental;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al contar tatuajes", ex);
                return 0;
            }
        }

        /// <summary>
        /// Limpia los campos del formulario de busqueda de ingresos de un imputado.
        /// </summary>
        private void LimpiarBusqueda()
        {
            try
            {
                NombreBuscar = ApellidoPaternoBuscar = ApellidoMaternoBuscar = string.Empty;
                AnioBuscar = FolioBuscar = null;
                var auxiliar = SelectIngreso;
                ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                SelectExpediente = null;
                SelectIngreso = auxiliar;
                ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar búsqueda", ex);
            }
        }

        /// <summary>
        /// Muestra un dialogo modal para el guardado de información
        /// </summary>
        /// <param name="title">Titulo que se le dará a la notificación</param>
        /// <param name="success">True si el resultado de la operación de guardado.</param>
        private void MensajeDialogo(string title, bool success)
        {
            try
            {
                new Dialogos().ConfirmacionDialogo(title, success ? "Guardado Exitoso !" : "Hay errores en los datos y no se ha podido guardar la información.");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en mensaje diálogo", ex);
            }
        }
        #endregion

        #region [REPORTE]
        private void ImprimirEMI()
        {
            try
            {
                if (emiActual != null)
                {
                    emiActual = new cEMI().Obtener(emiActual.ID_EMI).Where(w => w.ESTATUS == "C").OrderByDescending(w => w.ID_EMI_CONS).FirstOrDefault();
                    if (emiActual == null)
                        return;
                        var view = new ReportesView();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        view.Owner = PopUpsViewModels.MainWindow;
                        view.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                        view.Show();
                 
                    var ficha = new cFichaEMI();

                    if (emiActual.EMI_INGRESO.INGRESO.FEC_REGISTRO != null)
                        ficha.FechaCaptura = emiActual.EMI_INGRESO.INGRESO.FEC_REGISTRO.Value.ToString("dd/MM/yyyy");//"04/07/2011";
                    ficha.Centro = emiActual.EMI_INGRESO.INGRESO.CENTRO.DESCR;// "CERESO DE MEXICALI";
                    ficha.EstatusJuridico = emiActual.EMI_INGRESO.INGRESO.ESTATUS_ADMINISTRATIVO.DESCR;// "SENTENCIADO";
                    if (emiActual.EMI_INGRESO.INGRESO.CAMA != null)
                    {
                        ficha.Ubicacion = string.Format("{0}-{1}-{2}-{3}", emiActual.EMI_INGRESO.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim(), emiActual.EMI_INGRESO.INGRESO.CAMA.CELDA.SECTOR.DESCR.Trim(),
                                emiActual.EMI_INGRESO.INGRESO.CAMA.CELDA.ID_CELDA.Trim(),
                                emiActual.EMI_INGRESO.INGRESO.CAMA.ID_CAMA);//"M15-C23-4";
                    }
                    else
                    {
                        ficha.Ubicacion = string.Empty;
                    }
                    if (emiActual.EMI_INGRESO.INGRESO.FEC_INGRESO_CERESO != null)
                        ficha.FechaIngreso = emiActual.EMI_INGRESO.INGRESO.FEC_INGRESO_CERESO.Value.ToString("dd/MM/yyyy");//"01/01/2010";
                    ficha.Expediente = string.Format("{0}/{1}", emiActual.EMI_INGRESO.INGRESO.ID_ANIO, emiActual.EMI_INGRESO.INGRESO.ID_IMPUTADO);//"2010/7";

                    #region Pendiente
                    if (emiActual.EMI_INGRESO.INGRESO.CAUSA_PENAL != null)
                    {

                        foreach (var cp in emiActual.EMI_INGRESO.INGRESO.CAUSA_PENAL)
                        {
                            if (!string.IsNullOrEmpty(ficha.CausaPenal))
                                ficha.CausaPenal = ficha.CausaPenal + ",";
                            ficha.CausaPenal = ficha.CausaPenal + string.Format("{0}/{1}", cp.CP_ANIO, cp.CP_FOLIO);
                        }
                    }
                    else
                        ficha.CausaPenal = string.Empty;//"0001";


                    //ficha.CentroProcedencia = "NINGUNO";
                    #endregion

                    ficha.Nombre = string.Format("{0} {1} {2}", emiActual.EMI_INGRESO.INGRESO.IMPUTADO.NOMBRE.Trim(), emiActual.EMI_INGRESO.INGRESO.IMPUTADO.PATERNO.Trim(), emiActual.EMI_INGRESO.INGRESO.IMPUTADO.MATERNO.Trim());
                    ficha.Edad = new Fechas().CalculaEdad(emiActual.EMI_INGRESO.INGRESO.IMPUTADO.NACIMIENTO_FECHA).ToString();
                    ficha.Sexo = emiActual.EMI_INGRESO.INGRESO.IMPUTADO.SEXO.Equals("S") ? "MASCULINO" : "FEMENINO";
                    ficha.Apodo = string.Empty;
                    if (emiActual.EMI_INGRESO.INGRESO.IMPUTADO.APODO != null)
                    {
                        foreach (var apodo in emiActual.EMI_INGRESO.INGRESO.IMPUTADO.APODO)
                        {
                            if (!string.IsNullOrEmpty(ficha.Apodo))
                            {
                                ficha.Apodo = string.Format("{0},", ficha.Apodo);
                            }
                            ficha.Apodo = string.Format("{0}{1}", ficha.Apodo, apodo.APODO1);
                        }
                    }

                    if (emiActual.EMI_INGRESO.INGRESO.IMPUTADO.NACIMIENTO_FECHA != null)
                        ficha.FechaNacimiento = emiActual.EMI_INGRESO.INGRESO.IMPUTADO.NACIMIENTO_FECHA.Value.ToString("dd/MM/yyyy");//"17/05/1983";
                    //ficha.EstadoCivil = emiActual.EMI_INGRESO.INGRESO.IMPUTADO.ESTADO_CIVIL.DESCR;// "SOLTERO";
                    ficha.EstadoCivil = SelectIngreso.ESTADO_CIVIL != null ?  SelectIngreso.ESTADO_CIVIL.DESCR : string.Empty;// "SOLTERO";
                    ficha.LugarNacimiento = emiActual.EMI_INGRESO.INGRESO.IMPUTADO.NACIMIENTO_LUGAR;// "MEXICALI";
                    if (emiActual.EMI_INGRESO.INGRESO.IMPUTADO.PAIS_NACIONALIDAD != null)
                        ficha.Nacionalidad = emiActual.EMI_INGRESO.INGRESO.IMPUTADO.PAIS_NACIONALIDAD.NACIONALIDAD;// "MEXICANA";
                    else
                        ficha.Nacionalidad = string.Empty;

                    //ficha.Religion = emiActual.EMI_INGRESO.INGRESO.IMPUTADO.RELIGION.DESCR;// "PENTECOSTES";
                    ficha.Religion = SelectIngreso.RELIGION != null ? SelectIngreso.RELIGION.DESCR :  string.Empty;// "PENTECOSTES";
                    ficha.Etnia = emiActual.EMI_INGRESO.INGRESO.IMPUTADO.ETNIA.DESCR;// "CUCAPA";
                    //if (emiActual.EMI_INGRESO.INGRESO.IMPUTADO.COLONIA != null)
                    //    ficha.ColoniaResidencia = emiActual.EMI_INGRESO.INGRESO.IMPUTADO.COLONIA.DESCR;// "EJIDO OAXACA";
                    //else
                    //    ficha.ColoniaResidencia = string.Empty;
                    ficha.ColoniaResidencia = SelectIngreso.COLONIA != null ? SelectIngreso.COLONIA.DESCR : string.Empty;

                    //if (emiActual.EMI_INGRESO.INGRESO.IMPUTADO.MUNICIPIO != null)
                    //    ficha.CiudadEstadoResidencia = emiActual.EMI_INGRESO.INGRESO.IMPUTADO.MUNICIPIO.MUNICIPIO1;// "MEXICALI, BAJA CALIFORNIA";
                    //else
                    //    ficha.CiudadEstadoResidencia = string.Empty;
                    ficha.CiudadEstadoResidencia = SelectIngreso.MUNICIPIO != null ? SelectIngreso.MUNICIPIO.MUNICIPIO1 : string.Empty;
                    //ficha.TiempoResidenciaBC = emiActual.EMI_INGRESO.INGRESO.IMPUTADO.RESIDENCIA_ANIOS + " AÑOS";
                    ficha.TiempoResidenciaBC = string.Format("{0} AÑOS", SelectIngreso.RESIDENCIA_ANIOS);
                    //////////////////////////////////////////////////////////////

                    ficha.TiempoColonia = emiActual.EMI_FICHA_IDENTIFICACION.TIEMPO_RESID_COL;
                    if (emiActual.EMI_FICHA_IDENTIFICACION.EDUCACION_GRADO != null)
                        ficha.UltimoGradoEstudios = emiActual.EMI_FICHA_IDENTIFICACION.EDUCACION_GRADO.DESCR;// "DOCTORADO EN CIENCIAS";
                    else
                        ficha.UltimoGradoEstudios = string.Empty;
                    ficha.ViviaAntesDetencion = emiActual.EMI_FICHA_IDENTIFICACION.PERSONA_CONVIVENCIA_ANTERIOR;//"JIMENEZ CASTRO DIANA";
                    if (emiActual.EMI_FICHA_IDENTIFICACION.TIPO_REFERENCIA != null)
                        ficha.Parentesco = emiActual.EMI_FICHA_IDENTIFICACION.TIPO_REFERENCIA.DESCR;// "MADRE";
                    else
                        ficha.Parentesco = string.Empty;

                    ficha.ExfuncionarioSP = emiActual.EMI_FICHA_IDENTIFICACION.EXFUNCIONARIO_SEGPUB.DESCR;// "NO";
                    ficha.Pasaporte = emiActual.EMI_FICHA_IDENTIFICACION.PASAPORTE.Equals("S") ? "SI" : "NO";//"NO";
                    ficha.LicenciaManejo = emiActual.EMI_FICHA_IDENTIFICACION.LICENCIA_MANEJO.Equals("S") ? "SI" : "NO"; ;
                    ficha.CredencialElector = emiActual.EMI_FICHA_IDENTIFICACION.CREDENCIAL_ELECTOR.Equals("S") ? "SI" : "NO"; ;
                    ficha.CartillaMilitar = emiActual.EMI_FICHA_IDENTIFICACION.CARTILLA_MILITAR.Equals("S") ? "SI" : "NO"; ;

                    ficha.CertificadoEducacion = emiActual.EMI_FICHA_IDENTIFICACION.EDUCACION_CERTIFICADO.DESCR;//"DOCTORADO";
                    ficha.OficiosHabilidades = emiActual.EMI_FICHA_IDENTIFICACION.OFICIOS_HABILIDADES;// "ESTUDIO DE LA TEORIA DE LA CUERDA";
                    ficha.CambiosDomiciolio = emiActual.EMI_FICHA_IDENTIFICACION.CAMBIOS_DOMICILIO_ULTIMO_ANO.ToString();// "0";
                    ficha.MotivoCambio = emiActual.EMI_FICHA_IDENTIFICACION.MOTIVOS_CAMBIOS_DOMICILIO;// "NA";
                    ficha.CentroProcedencia = emiActual.EMI_FICHA_IDENTIFICACION.ID_CERESO_PROCEDENCIA != null ? emiActual.EMI_FICHA_IDENTIFICACION.EMISOR.DESCR : string.Empty;

                    var empleos = new List<cUltimosEmpleosEMI>();
                    if (emiActual.EMI_ULTIMOS_EMPLEOS != null)
                    {
                        foreach (var emp in emiActual.EMI_ULTIMOS_EMPLEOS)
                        {
                            empleos.Add(new cUltimosEmpleosEMI()
                            {
                                Ocupacion = emp.OCUPACION.DESCR,
                                Duracion = emp.DURACION,
                                Empresa = emp.EMPRESA,
                                MotivoDesempleo = emp.MOTIVO_DESEMPLEO,
                                EmpleoFormal = emp.EMPLEO_FORMAL.Equals("S") ? "SI" : "NO",
                                Ultimo = emp.ULTIMO_EMPLEO_ANTES_DETENCION.Equals("S") ? "SI" : "NO",
                                InestabilidadLaboral = emp.INESTABILIDAD_LABORAL.Equals("S") ? "SI" : "NO"
                            });
                        }
                    }

                    //PENDIENTE
                    var delitos = new List<cDelitosEMI>();

                    var juridico = new cSituacionJuridicaEMI();
                    if (emiActual.EMI_SITUACION_JURIDICA != null)
                    {
                        juridico.VersionDelito = emiActual.EMI_SITUACION_JURIDICA.VERSION_DELITO_INTERNO;

                        juridico.MenorTiempoEntreIngresos = string.Empty;
                        switch (emiActual.EMI_SITUACION_JURIDICA.MENOR_PERIODO_LIBRE_REING)
                        {
                            case "MENORA1":
                                juridico.MenorTiempoEntreIngresos = "MENOR A UN AÑO";
                                break;
                            case "DE1A5":
                                juridico.MenorTiempoEntreIngresos = "DE 1 A 5 AÑOS";
                                break;
                            case "MAS5":
                                juridico.MenorTiempoEntreIngresos = "MAS DE 5 AÑOS";
                                break;
                        }

                        juridico.MayorTiempoEntreIngresos = string.Empty;
                        switch (emiActual.EMI_SITUACION_JURIDICA.MAYOR_PERIODO_LIBRE_REING)
                        {
                            case "MENORA1":
                                juridico.MayorTiempoEntreIngresos = "MENOR A UN AÑO";
                                break;
                            case "DE1A5":
                                juridico.MayorTiempoEntreIngresos = "DE 1 A 5 AÑOS";
                                break;
                            case "MAS5":
                                juridico.MayorTiempoEntreIngresos = "MAS DE 5 AÑOS";
                                break;
                        }

                        juridico.PracticadoEstudios = emiActual.EMI_SITUACION_JURIDICA.PRACT_ESTUDIOS.Equals("S") ? "SI" : "NO";
                        juridico.CuandoPracticaronEstudios = emiActual.EMI_SITUACION_JURIDICA.CUANDO_PRACT_ESTUDIOS; ;
                        juridico.DeseaTraslado = emiActual.EMI_SITUACION_JURIDICA.DESEA_TRASLADO.Equals("S") ? "SI" : "NO";
                        juridico.DondeDeseaTraslado = emiActual.EMI_SITUACION_JURIDICA.DONDE_DESEA_TRASLADO;
                        juridico.MotivoDeseaTraslado = emiActual.EMI_SITUACION_JURIDICA.MOTIVO_DESEA_TRASLADO;
                    }


                    var ingresos = new List<cIngresosAnterioresCentroEMI>();
                    var ingresosMenores = new List<cIngresosAnterioresCentroMenoresEMI>();

                    if (emiActual.EMI_INGRESO_ANTERIOR != null)
                    {
                        foreach (var ing in emiActual.EMI_INGRESO_ANTERIOR)
                        {
                            if (ing.ID_TIPO == 1)//MENORES
                            {///TODO: cambios delito
                                ingresosMenores.Add(new cIngresosAnterioresCentroMenoresEMI()
                                {
                                    CentroM = ing.EMISOR.DESCR,
                                    //DelitoM = ing.DELITO.DESCR,
                                    PeriodoReclusionM = ing.PERIODO_RECLUSION,
                                    SancionesM = ing.SANCIONES
                                });
                            }
                            else //MAYORES
                            {///TODO: cambios delito
                                ingresos.Add(new cIngresosAnterioresCentroEMI()
                                {
                                    Centro = ing.EMISOR.DESCR,
                                    //Delito = ing.DELITO.DESCR,
                                    PeriodoReclusion = ing.PERIODO_RECLUSION,
                                    Sanciones = ing.SANCIONES
                                });
                            }
                        }
                    }

                    var sociofamiliares = new cSocioFamiliaresEMI();
                    if (emiActual.EMI_FACTORES_SOCIO_FAMILIARES != null)
                    {
                        sociofamiliares.VisitaFamiliar = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.RECIBE_VISITA_FAMILIAR.Equals("S") ? "SI" : "NO";
                        sociofamiliares.FrecuenciaVF =  emiActual.EMI_FACTORES_SOCIO_FAMILIARES.FRECUENCIA != null ?  emiActual.EMI_FACTORES_SOCIO_FAMILIARES.FRECUENCIA.DESCR : string.Empty;
                        sociofamiliares.VisitaIntima = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.RECIBE_VISITA_INTIMA.Equals("S") ? "SI" : "NO";
                        sociofamiliares.ApoyoEconomico = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.RECIBE_APOYO_ECONOMICO.Equals("S") ? "SI" : "NO";
                        sociofamiliares.CantidadAE = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.CANTIDAD_APOYO_ECONOMICO;
                        sociofamiliares.FrecuenciaAE = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.FRECUENCIA1 != null ? emiActual.EMI_FACTORES_SOCIO_FAMILIARES.FRECUENCIA1.DESCR : string.Empty;

                        sociofamiliares.VivePadre = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.VIVE_PADRE.Equals("S") ? "SI" : "NO";
                        sociofamiliares.ViveMadre = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.VIVE_MADRE.Equals("S") ? "SI" : "NO";
                        sociofamiliares.EdadImpFallecioPAdre = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.EDAD_INTERNO_FALLE_PADRE != null ? emiActual.EMI_FACTORES_SOCIO_FAMILIARES.EDAD_INTERNO_FALLE_PADRE.ToString() : string.Empty;
                        sociofamiliares.EdadImpFallecioMadre = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.EDAD_INTERNO_FALLE_MADRE != null ? emiActual.EMI_FACTORES_SOCIO_FAMILIARES.EDAD_INTERNO_FALLE_MADRE.ToString() : string.Empty; 
                        sociofamiliares.PadresJuntos = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.PADRES_JUNTOS.Equals("S") ? "SI" : "NO";
                        sociofamiliares.MotivoSeparacionPadres = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.MOTIVOS_SEPARACION;
                        sociofamiliares.EdadSeparacionPadres = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.EDAD_INTERNO_SEPARACION != null ? emiActual.EMI_FACTORES_SOCIO_FAMILIARES.EDAD_INTERNO_SEPARACION.ToString() : string.Empty;

                        switch (emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ID_NIVEL_SOCIAL)
                        {
                            case 1:
                                sociofamiliares.NivelSocial = "BAJO";
                                break;
                            case 2: sociofamiliares.NivelSocial = "MEDIO";
                                break;
                            case 3:
                                sociofamiliares.NivelSocial = "ALTO";
                                break;
                        }

                        switch (emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ID_NIVEL_CULTURAL)
                        {
                            case 1:
                                sociofamiliares.NivelCultural = "BAJO";
                                break;
                            case 2: sociofamiliares.NivelCultural = "MEDIO";
                                break;
                            case 3:
                                sociofamiliares.NivelCultural = "ALTO";
                                break;
                        }

                        switch (emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ID_NIVEL_SOCIAL)
                        {
                            case 1:
                                sociofamiliares.NivelEconomico = "BAJO";
                                break;
                            case 2: sociofamiliares.NivelEconomico = "MEDIO";
                                break;
                            case 3:
                                sociofamiliares.NivelEconomico = "ALTO";
                                break;
                        }

                        sociofamiliares.TotalParejas = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.TOTAL_PAREJAS.ToString();
                        sociofamiliares.Union = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.CANTIDAD_PAREJAS_UNION.ToString();

                        sociofamiliares.NoHijos = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.NUMERO_HIJOS.ToString();
                        sociofamiliares.HijosRegistrados = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.HIJOS_REGISTRADOS.ToString();
                        sociofamiliares.HijosRelacion = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.CANTIDAD_HIJOS_RELACION.ToString();
                        sociofamiliares.HijosVisitan = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.CANTIDAD_HIJOS_VISITA.ToString();

                        sociofamiliares.ContactoNombre = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.CONTACTO_NOMBRE;
                        sociofamiliares.ContactoPArentesco = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.TIPO_REFERENCIA.DESCR;
                        //sociofamiliares.ContactoTelefono = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.CONTACTO_TELEFONO.ToString();
                        TextContactoTelefono = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.CONTACTO_TELEFONO.ToString();

                        sociofamiliares.AbandoonoFamiliar = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ABANDONO_FAMILIAR.Equals("S") ? "SI" : "NO";
                        if (emiActual.EMI_FACTORES_SOCIO_FAMILIARES.EMI_FACTORES_ABANDONO != null)
                            sociofamiliares.DescrAFam = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.EMI_FACTORES_ABANDONO.DESCR;
                        else
                            sociofamiliares.DescrAFam = string.Empty;
                        if (HuidasHogar != -1)
                            sociofamiliares.HuidasHogar = LstFactoresHuida.Where(w => w.ID_HUIDA == HuidasHogar).FirstOrDefault().DESCR;
                        sociofamiliares.MaltratoEmocional = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.MALTRATO_EMOCIONAL.Equals("S") ? "SI" : "NO";
                        sociofamiliares.DescrME = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ESPECIF_MALTRATO_EMOCIONAL;
                        sociofamiliares.MaltratoFisico = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.MALTRATO_FISICO.Equals("S") ? "SI" : "NO";
                        sociofamiliares.DescrMF = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ESPECIFIQUE_MALTRATO_FISICO;
                        sociofamiliares.AbusoFisico = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ABUSO_SEXUAL.Equals("S") ? "SI" : "NO";
                        sociofamiliares.DescrAF = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.ESPECIFIQUE_ABUSO_SEXUAL;
                        sociofamiliares.EdadAbuso = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.EDAD_ABUSO_SEXUAL.ToString();
                        sociofamiliares.InicioContactoSexual = emiActual.EMI_FACTORES_SOCIO_FAMILIARES.EDAD_INICIO_CONTACTO_SEXUAL.ToString();
                    }
                    var grupoFamiliar = new List<cGrupoFamiliarEMI>();
                    if (emiActual.EMI_GRUPO_FAMILIAR != null)
                    {
                        var g = string.Empty;
                        foreach (var grupo in emiActual.EMI_GRUPO_FAMILIAR)
                        {
                            switch (grupo.GRUPO)
                            {
                                case 1:
                                    g = "PRIMARIO";
                                    break;
                                case 2:
                                    g = "SECUNDARIO";
                                    break;
                                case 3:
                                    g = "NINGUNO";
                                    break;
                            }
                            grupoFamiliar.Add(new cGrupoFamiliarEMI()
                            {
                                Grupo = g,
                                Paterno = grupo.PATERNO,
                                Materno = grupo.MATERNO,
                                Nombre = grupo.NOMBRE,
                                Edad = grupo.EDAD != null ? grupo.EDAD.ToString() : string.Empty,
                                Relacion = grupo.TIPO_REFERENCIA != null ? grupo.TIPO_REFERENCIA.DESCR : string.Empty,
                                Domicilio = grupo.DOMICILIO,
                                Ocupacion = grupo.OCUPACION != null ? grupo.OCUPACION.DESCR : string.Empty,
                                EdoCivil = grupo.ESTADO_CIVIL != null ? grupo.ESTADO_CIVIL.DESCR : string.Empty,
                                ViveConEl = grupo.VIVE_C_EL.Equals("S") ? "SI" : "NO"
                            });
                        }
                    }

                    var familiarDelito = new List<cFamiliarAntecedentesEMI>();
                    if (emiActual.EMI_ANTECEDENTE_FAM_CON_DEL != null)
                    {
                        foreach (var ant in emiActual.EMI_ANTECEDENTE_FAM_CON_DEL)
                        {
                            familiarDelito.Add(new cFamiliarAntecedentesEMI()
                            {
                                Parentesco = ant.TIPO_REFERENCIA != null ? ant.TIPO_REFERENCIA.DESCR : string.Empty,
                                //Anio = ant.ANIO.ToString(),
                                Centro = ant.EMISOR != null ? ant.EMISOR.DESCR : string.Empty,
                                Delito = ant.DELITO,
                                Relacion = ant.TIPO_RELACION != null ? ant.TIPO_RELACION.DESCR : string.Empty
                            });
                        }
                    }

                    var familiarDroga = new List<cFamiliarDrogaEMI>();
                    if (emiActual.EMI_ANTECEDENTE_FAMILIAR_DROGA != null)
                    {
                        foreach (var droga in emiActual.EMI_ANTECEDENTE_FAMILIAR_DROGA)
                        {
                            familiarDroga.Add(new cFamiliarDrogaEMI()
                            {
                                Parentesco = droga.TIPO_REFERENCIA !=null ? droga.TIPO_REFERENCIA.DESCR : string.Empty,
                                //Anio = droga.ANIO.ToString(),
                                Relacion = droga.TIPO_RELACION != null ? droga.TIPO_RELACION .DESCR : string.Empty,
                                TipoDroga = droga.DROGA != null ? droga.DROGA.DESCR : string.Empty
                            });
                        }
                    }

                    var conductaParasocial = new cConductaParasocialEMI();
                    if (emiActual.EMI_HPS != null)
                    {
                        conductaParasocial.VivioCalleOrfanato = emiActual.EMI_HPS.VIVIO_CALLE_ORFANATO_NINO.Equals("S") ? "SI" : "NO";
                        conductaParasocial.PertenecePandilla = emiActual.EMI_HPS.PERTENECE_PANDILLA_ACTUAL.Equals("S") ? "SI" : "NO";
                        if (emiActual.EMI_HPS.ID_PANDILLA != null && emiActual.EMI_HPS.ID_PANDILLA != -1)
                            conductaParasocial.Pandilla = LstPandillas.Where(w => w.ID_PANDILLA == emiActual.EMI_HPS.ID_PANDILLA).FirstOrDefault().NOMBRE;

                        conductaParasocial.ComportamientoHomosexual = emiActual.EMI_HPS.COMPORTAMIENTO_HOMOSEXUAL.Equals("S") ? "SI" : "NO";
                        conductaParasocial.EdadInicioCH = emiActual.EMI_HPS.EDAD_INICIAL_HOMOSEXUAL.ToString();
                        conductaParasocial.RolCH = emiActual.EMI_HPS.ROL_HOMOSEXUAL;
                        conductaParasocial.PandillaExterior = emiActual.EMI_HPS.PERTENECIO_PANDILAS_EXTERIOR.Equals("S") ? "SI" : "NO";
                        conductaParasocial.EdadInicioPE = emiActual.EMI_HPS.EDAD_INICIAL_PANDILLAS.ToString();
                        conductaParasocial.MotivoPE = emiActual.EMI_HPS.MOTIVOS_PERTENENCIA_PANDILLAS;
                        conductaParasocial.Vagancia = emiActual.EMI_HPS.VAGANCIA;
                        conductaParasocial.EdadInicioVagancia = emiActual.EMI_HPS.EDAD_INICIAL_VAGANCIA.ToString();
                        conductaParasocial.MotivoVagancia = emiActual.EMI_HPS.MOTIVOS_VAGANCIA;
                        conductaParasocial.Cicatrices = emiActual.EMI_HPS.CICATRICES.Equals("S") ? "SI" : "NO";
                        conductaParasocial.EdadInicioC = emiActual.EMI_HPS.EDAD_INICIO_CICATRICES.ToString();
                        conductaParasocial.CicatricesRinia = emiActual.EMI_HPS.CICATRIZ_POR_RINA.Equals("S") ? "SI" : "NO";
                        conductaParasocial.MotivoCicatrices = emiActual.EMI_HPS.MOTIVO_CICATRICES;
                        conductaParasocial.DesercionEscolar = emiActual.EMI_HPS.DESERCION_ESCOLAR.Equals("S") ? "SI" : "NO";
                        conductaParasocial.MotivoDE = emiActual.EMI_HPS.MOTIVO_DESERCION_ESCOLAR;

                        conductaParasocial.ReprobacionEscolar = emiActual.EMI_HPS.REPROBACION_ESCOLAR.Equals("S") ? "SI" : "NO";
                        if (emiActual.EMI_HPS.GRADO_REPROBACION_ESCOLAR != null && emiActual.EMI_HPS.GRADO_REPROBACION_ESCOLAR != -1)
                            conductaParasocial.GradoRE = LstGradoEducativo.Where(w => w.ID_GRADO == emiActual.EMI_HPS.GRADO_REPROBACION_ESCOLAR).FirstOrDefault().DESCR;
                        conductaParasocial.MotivoRE = emiActual.EMI_HPS.MOTIVO_REPROBACION_ESCOLAR;

                        conductaParasocial.ExpulsionEscolar = emiActual.EMI_HPS.EXPULSION_ESCOLAR.Equals("S") ? "SI" : "NO";
                        if (emiActual.EMI_HPS.GRADO_EXPULSION_ESCOLAR != null && emiActual.EMI_HPS.GRADO_EXPULSION_ESCOLAR != -1)
                            conductaParasocial.GradoEE = LstGradoEducativo.Where(w => w.ID_GRADO == emiActual.EMI_HPS.GRADO_EXPULSION_ESCOLAR).FirstOrDefault().DESCR;
                        conductaParasocial.MotivoEE = emiActual.EMI_HPS.MOTIVO_EXPULSION_ESCOLAR;

                        conductaParasocial.ProstituiaHombres = emiActual.EMI_HPS.PROSTITUIA_HOMBRES.Equals("S") ? "SI" : "NO";
                        conductaParasocial.ProstituiaMujeres = emiActual.EMI_HPS.PROSTITUIA_MUJERES.Equals("S") ? "SI" : "NO";
                        if (emiActual.EMI_HPS.PROSTITUYE_POR != null && emiActual.EMI_HPS.PROSTITUYE_POR != -1)
                            conductaParasocial.MotivoProstitucion = LstMotivosProstituye.Where(w => w.ID_MOTIVO == emiActual.EMI_HPS.PROSTITUYE_POR).FirstOrDefault().DESCR;

                        conductaParasocial.PagabaSexoHombres = emiActual.EMI_HPS.PAGA_SEXUAL_HOMBRE.Equals("S") ? "SI" : "NO";
                        conductaParasocial.PagabaSexoMujeres = emiActual.EMI_HPS.PAGA_SEXUAL_MUJER.Equals("S") ? "SI" : "NO";
                    }

                    var actividades = new List<cActividadesEMI>();

                    foreach (var actividad in emiActual.EMI_ACTIVIDAD)
                    {
                        actividades.Add(
                            new cActividadesEMI
                        {
                            IdArea = actividad.ID_EMI_ACTIVIDAD,
                            Anio = actividad.ANO_ACTIVIDADES.HasValue ? actividad.ANO_ACTIVIDADES.ToString() : string.Empty,
                            Actividades = actividad.EMI_TIPO_ACTIVIDAD != null ? actividad.EMI_TIPO_ACTIVIDAD.DESCR : string.Empty,
                            Duracion = actividad.DURACION_ACTIVIDADES,
                            ConclucionAbandono = actividad.EMI_ESTATUS_PROGRAMA != null ? actividad.EMI_ESTATUS_PROGRAMA.DESCR : string.Empty,
                            NoPrograma = actividad.PROGRAMA_TERMINADO.HasValue ? actividad.PROGRAMA_TERMINADO.ToString() : string.Empty
                        });
                    }


                    var enfermedad = new cEnfermedadEMI();
                    if (emiActual.EMI_ENFERMEDAD != null)
                    {
                        enfermedad.DescrEnfermedad = emiActual.EMI_ENFERMEDAD.DESCR_ENFERMEDAD;

                        enfermedad.Integro = emiActual.EMI_ENFERMEDAD.APFISICA_INTEGRO.Equals("S") ? "SI" : "NO";
                        enfermedad.Limpio = emiActual.EMI_ENFERMEDAD.APFISICA_LIMPIO.Equals("S") ? "SI" : "NO";
                        enfermedad.Conformado = emiActual.EMI_ENFERMEDAD.APFISICA_CONFORMADO.Equals("S") ? "SI" : "NO";
                        enfermedad.Aliniado = emiActual.EMI_ENFERMEDAD.APFISICA_ALINADO.Equals("S") ? "SI" : "NO";

                        enfermedad.Discapacidad = emiActual.EMI_ENFERMEDAD.DISCAPACIDAD.Equals("S") ? "SI" : "NO";
                        enfermedad.DescrDiscapacidad = emiActual.EMI_ENFERMEDAD.DESCR_ENFERMEDAD;
                        enfermedad.EnfermoMental = emiActual.EMI_ENFERMEDAD.ENFERMO_MENTAL.Equals("S") ? "SI" : "NO";
                        enfermedad.DescrEM = emiActual.EMI_ENFERMEDAD.DESCR_ENFERMO_MENTAL;
                        enfermedad.VIH = emiActual.EMI_ENFERMEDAD.VIH_HEPATITIS.Equals("S") ? "SI" : "NO";
                        enfermedad.EnTratamiento = !string.IsNullOrEmpty(emiActual.EMI_ENFERMEDAD.EN_TRATAMIENTO_FARMACO) ? emiActual.EMI_ENFERMEDAD.EN_TRATAMIENTO_FARMACO.Equals("S") ? "SI" : "NO" : "NO";
                        enfermedad.DiagnosticoFormal = !string.IsNullOrEmpty(emiActual.EMI_ENFERMEDAD.DIAGNOSTICO_FORMAL) ? emiActual.EMI_ENFERMEDAD.DIAGNOSTICO_FORMAL.Equals("S") ? "SI" : "NO" : "NO";
                    }

                    var clasificacion = new cClasificacionCriminologicaEMI();
                    if (emiActual.EMI_CLAS_CRIMINOLOGICA != null)
                    {
                        clasificacion.CCPorAntecedentes = emiActual.EMI_CLAS_CRIMINOLOGICA.CLASIFICACION_CRIMINOLOGICA.DESCR;
                        clasificacion.PerteneceCrimenOrganizado = emiActual.EMI_CLAS_CRIMINOLOGICA.PERTENECE_CRIMEN_ORG.DESCR;
                    }

                    if (emiActual.EMI_FACTOR_CRIMINODIAGNOSTICO != null)
                    {
                        clasificacion.Egocentrismo = emiActual.EMI_FACTOR_CRIMINODIAGNOSTICO.EMI_FACTOR_NIVEL5.DESCR;
                        clasificacion.Agresividad = emiActual.EMI_FACTOR_CRIMINODIAGNOSTICO.EMI_FACTOR_NIVEL3.DESCR;
                        clasificacion.IndiferenciaAfectiva = emiActual.EMI_FACTOR_CRIMINODIAGNOSTICO.EMI_FACTOR_NIVEL.DESCR;
                        clasificacion.LabilidadAfectiva = emiActual.EMI_FACTOR_CRIMINODIAGNOSTICO.EMI_FACTOR_NIVEL1.DESCR;

                        clasificacion.AdaptabilidadSocial = emiActual.EMI_FACTOR_CRIMINODIAGNOSTICO.EMI_FACTOR_NIVEL2.DESCR;
                        clasificacion.Liderazgo = emiActual.EMI_FACTOR_CRIMINODIAGNOSTICO.EMI_FACTOR_NIVEL7.DESCR;
                        clasificacion.ToleranciaFrustracion = emiActual.EMI_FACTOR_CRIMINODIAGNOSTICO.EMI_FACTOR_NIVEL9.DESCR;
                        clasificacion.ControlImpulsos = emiActual.EMI_FACTOR_CRIMINODIAGNOSTICO.EMI_FACTOR_NIVEL6.DESCR;

                        clasificacion.CapacidadCriminal = emiActual.EMI_FACTOR_CRIMINODIAGNOSTICO.EMI_FACTOR_NIVEL4.DESCR;
                        clasificacion.IndiceEdoPeligroso = emiActual.EMI_FACTOR_CRIMINODIAGNOSTICO.EMI_FACTOR_NIVEL8.DESCR;


                        clasificacion.PronosticoIntraInstitucional = emiActual.EMI_FACTOR_CRIMINODIAGNOSTICO.EMI_FACTOR_RESULTADO.DESCR;
                        clasificacion.UbicacionPorCC = emiActual.EMI_FACTOR_CRIMINODIAGNOSTICO.EMI_FACTOR_UBICACION.DESCR;

                        clasificacion.Dictamen = emiActual.EMI_FACTOR_CRIMINODIAGNOSTICO.DICTAMEN;
                    }

                    var sanciones = new List<cSancionesDiciplinariasEMI>();
                    foreach (var sanc in LstSanciones)
                    {
                        sanciones.Add(new cSancionesDiciplinariasEMI()
                        {
                            MotivoProceso = sanc.MOTIVO_PROCESO,
                            Cantidad = sanc.CANTIDAD_PARTICIPACION.HasValue ? sanc.CANTIDAD_PARTICIPACION.ToString() : string.Empty,
                            NuevoProceso = sanc.NUEVO_PROCESO.Equals("N") ? "NO" : "SI",
                            TiempoProceso = sanc.TIEMPO_CASTIGO_SANCION_PROCESO
                        });
                    }


                    //TATUAJES
                    var tatuajes = new List<cTatuajesEMI>();
                    if (emiActual.EMI_TATUAJE != null)
                    {
                        var tatu = new cTatuajesEMI();
                        //ANTES DE ENTRAR
                        tatu.AntisocialAE = (emiActual.EMI_TATUAJE.ANTISOCIAL_AI != null ? emiActual.EMI_TATUAJE.ANTISOCIAL_AI : 0).ToString();
                        tatu.EroticoAE = (emiActual.EMI_TATUAJE.EROTICO_AI != null ? emiActual.EMI_TATUAJE.EROTICO_AI : 0).ToString();
                        tatu.ReligiosolAE = (emiActual.EMI_TATUAJE.RELIGIOSO_AI != null ? emiActual.EMI_TATUAJE.RELIGIOSO_AI : 0).ToString();
                        tatu.IdentificacionAE = (emiActual.EMI_TATUAJE.IDENTIFICACION_AI != null ? emiActual.EMI_TATUAJE.IDENTIFICACION_AI : 0).ToString();
                        tatu.DecorativoAE = (emiActual.EMI_TATUAJE.DECORATIVO_AI != null ? emiActual.EMI_TATUAJE.DECORATIVO_AI : 0).ToString();
                        tatu.SentimentalAE = (emiActual.EMI_TATUAJE.SENTIMENTAL_AI != null ? emiActual.EMI_TATUAJE.SENTIMENTAL_AI : 0).ToString();
                        //INTRAMUROS
                        tatu.AntisocialI = (emiActual.EMI_TATUAJE.ANTISOCIAL_I != null ? emiActual.EMI_TATUAJE.ANTISOCIAL_I : 0).ToString();
                        tatu.EroticoI = (emiActual.EMI_TATUAJE.EROTICO_I != null ? emiActual.EMI_TATUAJE.EROTICO_I : 0).ToString();
                        tatu.ReligiosolI = (emiActual.EMI_TATUAJE.RELIGIOSO_I != null ? emiActual.EMI_TATUAJE.RELIGIOSO_I : 0).ToString();
                        tatu.IdentificacionI = (emiActual.EMI_TATUAJE.IDENTIFICACION_I != null ? emiActual.EMI_TATUAJE.IDENTIFICACION_I : 0).ToString();
                        tatu.DecorativoI = (emiActual.EMI_TATUAJE.DECORATIVO_I != null ? emiActual.EMI_TATUAJE.DECORATIVO_I : 0).ToString();
                        tatu.SentimentalI = (emiActual.EMI_TATUAJE.SENTIMENTAL_I != null ? emiActual.EMI_TATUAJE.SENTIMENTAL_I : 0).ToString();
                        //TOTAL
                        tatu.TotalTatuajes = (emiActual.EMI_TATUAJE.TOTAL_TATUAJES != null ? emiActual.EMI_TATUAJE.TOTAL_TATUAJES : 0).ToString();
                        //DESCRIPCION
                        tatu.DescripcionTatuajes = emiActual.EMI_TATUAJE.DESCR;

                        tatuajes.Add(tatu);
                    }
                    var tatuiajes = new List<cSancionesDiciplinariasEMI>();

                    //USO DE DROGAS IMPUTADO
                    var usoDrogas = new List<UsoDrogasEMI>();
                    if (emiActual.EMI_USO_DROGA != null)
                    {
                        foreach (var ud in emiActual.EMI_USO_DROGA)
                        {
                            string tc = string.Empty;
                            switch (ud.TIEMPO_CONSUMO)
                            {
                                case 1:
                                    tc = "NINGUNO";
                                    break;
                                case 2:
                                    tc = "MENOR A 1 AÑO";
                                    break;
                                case 3:
                                    tc = "DE 1 A 5 AÑOS";
                                    break;
                                case 4:
                                    tc = "DE 5 A 10 AÑOS";
                                    break;
                                case 5:
                                    tc = "MAYOR A 10 AÑOS";
                                    break;
                            }
                            usoDrogas.Add(new UsoDrogasEMI()
                            {
                                Droga = ud.DROGA != null ? !string.IsNullOrEmpty(ud.DROGA.DESCR) ? ud.DROGA.DESCR.Trim() : string.Empty : string.Empty,
                                EdadInicio = ud.EDAD_INICIO.HasValue ? ud.EDAD_INICIO.ToString() : string.Empty,
                                FecUltimaDosis = ud.FEC_ULTIMA_DOSIS.HasValue ? ud.FEC_ULTIMA_DOSIS.Value.ToString("dd/MM/yyyy") : string.Empty,
                                FrecuenciaUso = ud.DROGA_FRECUENCIA != null ? !string.IsNullOrEmpty(ud.DROGA_FRECUENCIA.DESCR) ? ud.DROGA_FRECUENCIA.DESCR.Trim() : string.Empty : string.Empty,
                                ConsumoActual = ud.CONSUMO_ACTUAL.Equals("S") ? "SI" : "NO",
                                TiempoConsumo = tc
                            });
                        }
                    }

                    var reporte = new List<cReporte>();
                    reporte.Add(new cReporte()
                    {
                        Encabezado1 = Parametro.ENCABEZADO1.Trim(),
                        Encabezado2 = Parametro.ENCABEZADO2.Trim(),
                        Encabezado3 = SelectIngreso.CENTRO.DESCR.Trim().ToUpper(),
                        Encabezado4 = "ENTREVISTA MULTIDICIPLINARIA INICIAL",
                        Logo1 = Parametro.REPORTE_LOGO1,
                        Logo2 = Parametro.REPORTE_LOGO2,
                    });
                    //ARMAMOS EL REPORTE
                    //Path.Combine(Application.StartupPath, "Reports", "report.rpt").
                    var wpf = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                    view.Report.LocalReport.ReportPath = "Reportes/rEMI.rdlc";
                    view.Report.LocalReport.DataSources.Clear();

                    //FICHA
                    var ds1 = new List<cFichaEMI>();
                    ds1.Add(ficha);
                    Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds1.Name = "DataSet1";
                    rds1.Value = ds1;
                    view.Report.LocalReport.DataSources.Add(rds1);

                    //ULTIMOS EMPLEOS
                    Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds2.Name = "DataSet2";
                    rds2.Value = empleos;
                    view.Report.LocalReport.DataSources.Add(rds2);

                    //DELITO
                    Microsoft.Reporting.WinForms.ReportDataSource rds3 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds3.Name = "DataSet3";
                    rds3.Value = delitos;
                    view.Report.LocalReport.DataSources.Add(rds3);

                    //SITUACION JURIDICA
                    var ds4 = new List<cSituacionJuridicaEMI>();
                    ds4.Add(juridico);
                    Microsoft.Reporting.WinForms.ReportDataSource rds4 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds4.Name = "DataSet4";
                    rds4.Value = ds4;
                    view.Report.LocalReport.DataSources.Add(rds4);

                    //INGRESOS CENTRO
                    Microsoft.Reporting.WinForms.ReportDataSource rds5 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds5.Name = "DataSet5";
                    rds5.Value = ingresos;
                    view.Report.LocalReport.DataSources.Add(rds5);

                    //FACTORES SOCIOFAMILIARES
                    var ds6 = new List<cSocioFamiliaresEMI>();
                    ds6.Add(sociofamiliares);
                    Microsoft.Reporting.WinForms.ReportDataSource rds6 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds6.Name = "DataSet6";
                    rds6.Value = ds6;
                    view.Report.LocalReport.DataSources.Add(rds6);

                    //GRUPO FAMILIAR
                    Microsoft.Reporting.WinForms.ReportDataSource rds7 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds7.Name = "DataSet7";
                    rds7.Value = grupoFamiliar;
                    view.Report.LocalReport.DataSources.Add(rds7);

                    //FAMILIAR ANTECEDENTES
                    Microsoft.Reporting.WinForms.ReportDataSource rds8 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds8.Name = "DataSet8";
                    rds8.Value = familiarDelito;
                    view.Report.LocalReport.DataSources.Add(rds8);

                    //FAMILIAR DROGA
                    Microsoft.Reporting.WinForms.ReportDataSource rds9 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds9.Name = "DataSet9";
                    rds9.Value = familiarDroga;
                    view.Report.LocalReport.DataSources.Add(rds9);

                    //CONDUCTAS PARASOCIALES
                    var ds10 = new List<cConductaParasocialEMI>();
                    ds10.Add(conductaParasocial);
                    Microsoft.Reporting.WinForms.ReportDataSource rds10 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds10.Name = "DataSet10";
                    rds10.Value = ds10;
                    view.Report.LocalReport.DataSources.Add(rds10);

                    //ACTIVIDADES
                    Microsoft.Reporting.WinForms.ReportDataSource rds11 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds11.Name = "DataSet11";
                    rds11.Value = actividades;
                    view.Report.LocalReport.DataSources.Add(rds11);

                    //ENFERMEDADES
                    var ds12 = new List<cEnfermedadEMI>();
                    ds12.Add(enfermedad);
                    Microsoft.Reporting.WinForms.ReportDataSource rds12 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds12.Name = "DataSet12";
                    rds12.Value = ds12;
                    view.Report.LocalReport.DataSources.Add(rds12);

                    //CLASIFICACION CRIMINOLOGICA
                    var ds13 = new List<cClasificacionCriminologicaEMI>();
                    ds13.Add(clasificacion);
                    Microsoft.Reporting.WinForms.ReportDataSource rds13 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds13.Name = "DataSet13";
                    rds13.Value = ds13;
                    view.Report.LocalReport.DataSources.Add(rds13);

                    //SANCIONES
                    Microsoft.Reporting.WinForms.ReportDataSource rds14 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds14.Name = "DataSet14";
                    rds14.Value = sanciones;
                    view.Report.LocalReport.DataSources.Add(rds14);

                    //INGRESOS MENORES
                    Microsoft.Reporting.WinForms.ReportDataSource rds15 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds15.Name = "DataSet15";
                    rds15.Value = ingresosMenores;
                    view.Report.LocalReport.DataSources.Add(rds15);

                    //GENERALES EMI
                    var ds16 = new List<cGeneralesEMI>();
                    ds16.Add(new cGeneralesEMI()
                    {
                        Centro = "CERESO DE PRUEBAS SUBSECRETARIA MEXICALI",
                        Dictaminador = string.Empty
                    });
                    Microsoft.Reporting.WinForms.ReportDataSource rds16 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds16.Name = "DataSet16";
                    rds16.Value = ds16;
                    view.Report.LocalReport.DataSources.Add(rds16);

                    //CAUSA PENAL EMI
                    var ds17 = new List<cCausaPenalEMI>();
                    ds17.Add(new cCausaPenalEMI()
                    {
                        AniosSentencia = AniosS.ToString(),
                        MesesSentencia = MesesS.ToString(),
                        DiasSentencia = DiasS.ToString(),
                        AniosCompurgados = AniosC.ToString(),
                        MesesCompurgados = MesesC.ToString(),
                        DiasCompurgados = DiasC.ToString(),
                        AniosPorCompurgar = AniosPC.ToString(),
                        MesesPorCompurgar = MesesPC.ToString(),
                        DiasPorCompurgar = DiasPC.ToString(),
                        Fuero = string.Empty,
                        Delito = Delitos
                    });
                    Microsoft.Reporting.WinForms.ReportDataSource rds17 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds17.Name = "DataSet17";
                    rds17.Value = ds17;
                    view.Report.LocalReport.DataSources.Add(rds17);

                    //TATUAJES
                    Microsoft.Reporting.WinForms.ReportDataSource rds18 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds18.Name = "DataSet18";
                    rds18.Value = tatuajes;
                    view.Report.LocalReport.DataSources.Add(rds18);

                    //USO DE DROGAS
                    //TATUAJES
                    Microsoft.Reporting.WinForms.ReportDataSource rds19 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds19.Name = "DataSet19";
                    rds19.Value = usoDrogas;
                    view.Report.LocalReport.DataSources.Add(rds19);

                    //reporte
                    Microsoft.Reporting.WinForms.ReportDataSource rds20 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds20.Name = "DataSet20";
                    rds20.Value = reporte;
                    view.Report.LocalReport.DataSources.Add(rds20);
                    
                    view.Report.RefreshReport();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al imprimir emi", ex);
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
            }
        }
        #endregion

        #region VALIDACIONES

        private bool ValidarUltimosEmpleos()
        {
            try
            {
                if (string.IsNullOrEmpty(TiempoColonia))
                    return false;
                if (!UltimoGradoEducativoConcluido.HasValue || UltimoGradoEducativoConcluido == -1)
                    return false;
                if (!FechaCaptura.HasValue)
                    return false;
                if (string.IsNullOrEmpty(ViviaAntesDetencion))
                    return false;
                if (!Parentesco.HasValue || Parentesco == (short)-1)
                    return false;
                if (!ExFuncionarioSeguridadPublica.HasValue || ExFuncionarioSeguridadPublica == -1)
                    return false;
                //if (!CeresoProcedencia.HasValue || CeresoProcedencia == -1)
                //    return false;
                if (!CertificadoEducacion.HasValue || CertificadoEducacion == -1)
                    return false;
                if (string.IsNullOrEmpty(OficiosHabilidades))
                    return false;
                if (!UltimoAnio.HasValue || UltimoAnio == -1)
                    return false;
                //if (string.IsNullOrEmpty(Motivo))
                //    return false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al validar ultimos empleos", ex);
            }
            return true;
        }
        #endregion

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.EMI.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                if (permisos.Any())
                {
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
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }
        #endregion

        #region FUNCIONES
        //load
        private async void Load_Window(EntrevistaMultidiciplinariaView Window)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(PrepararListas);
                ConfiguraPermisos();
                if (SelectIngreso != null)
                {
                    //PopulateFicha();
                    PopulateEMI();
                    OnLoadTratamiento(Window.DynamicGrid);
                    setValidacionesFichaIdentificacion();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar entrevista multidiciplinaria", ex);
            }
        }

        private void FactoresLoad(Factores Window = null)
        {
            try
            {
                if (TabFactoresSocioFamiliaresSelected)
                {
                    if (TabFactorSelected)
                    {
                        if (SelectIngreso != null)
                        {
                            GrupoFamiliarEnabled = true;
                            setValidacionesFactores();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar factores", ex);
            }
        }

        private void HPSLoad(HomosexualidadPandillaSexualidad Window = null)
        {
            try
            {
                if (TabConductasParasocialesSelected)
                {
                    if (TabHPSSelected)
                    {
                        if (SelectIngreso != null)
                        {
                            TatuajesEnabled = true;
                            setValidacionesHPS();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar HPS", ex);
            }
        }

        private void TatuajesLoad(Tatuajes Window = null)
        {
            try
            {
                if (TabConductasParasocialesSelected)
                {
                    if (TabTatuajesSelected)
                    {
                        if (SelectIngreso != null)
                        {
                            SeniasParticularesEnabled = true;
                            setValidacionesTatuajes();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar tatuajes", ex);
            }
        }

        private void EnfermedadesLoad(Enfermedades Window = null)
        {
            try
            {
                if (TabConductasParasocialesSelected)
                {
                    if (TabEnfermedadesSelected)
                    {
                        if (SelectIngreso != null)
                        {
                            //ActividadesEnabled = true;
                            ClasCriminologicaEnabled = true;
                            setValidacionesEnfermedades();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar enfermedades", ex);
            }
        }

        private void ConductaParasocialLoad(ConductaParasocial Window = null)
        {
            try
            {
                if (TabConductasParasocialesSelected)
                {
                    if (emiActual != null)
                    {
                        //PrepararListas();
                        //PopularConductasParasociales(emiActual);
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar conducta parasocial", ex);
            }
        }

        private void ClasCrimLoad(ClasCrim Window = null)
        {
            try
            {
                if (TabClasificacionCriminologicaPadreSelected)
                {
                    if (emiActual != null)
                    {
                        // PrepararListas();
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar clasificación criminológica padre", ex);
            }
        }

        private void SenaParticularLoad(TopografiaHumanaView Window = null)
        {
            try
            {
                if (TabConductasParasocialesSelected)
                    if (TabSeniaParticularSelected)
                    {
                        EnfermedadEnabled = true;
                        SelectPresentaIngresar = true;
                        if (emiActual != null)
                            PopulateGeneralesSenias();
                    }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar señas particulares", ex);
            }
        }

        private void SeniasFrenteLoad(SeniasFrenteView Window = null)
        {
            try
            {
                if (TabFrente)
                    if (emiActual != null)
                    {
                        ListRadioButons = new List<RadioButton>(new FingerPrintScanner().FindVisualChildren<RadioButton>(((Grid)Window.FindName("GridFrente"))));
                        if (SelectSenaParticular != null)
                            if (!string.IsNullOrEmpty(CodigoSenia))
                                if (ListRadioButons != null)
                                    foreach (var item in ListRadioButons)
                                    {
                                        if (item.CommandParameter != null)
                                            if (item.CommandParameter.ToString().Contains(SelectSenaParticular.ID_REGION.ToString() + "-" + CodigoSenia[1] + CodigoSenia[2] + CodigoSenia[3]))
                                                item.IsChecked = true;
                                            else
                                                item.IsChecked = false;
                                    }
                    }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar señas frente", ex);
            }
        }

        private void SeniasDorsoLoad(SeniasDorsoView Window = null)
        {
            try
            {
                if (TabDorso)
                    if (emiActual != null)
                    {
                        ListRadioButons = new List<RadioButton>(new FingerPrintScanner().FindVisualChildren<RadioButton>(((Grid)Window.FindName("GridDorso"))));
                        if (SelectSenaParticular != null)
                            if (!string.IsNullOrEmpty(CodigoSenia))
                                if (ListRadioButons != null)
                                    foreach (var item in ListRadioButons)
                                    {
                                        if (item.CommandParameter != null)
                                            if (item.CommandParameter.ToString().Contains(SelectSenaParticular.ID_REGION.ToString() + "-" + CodigoSenia[1] + CodigoSenia[2] + CodigoSenia[3]))
                                                item.IsChecked = true;
                                            else
                                                item.IsChecked = false;
                                    }
                    }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar señas dorso", ex);
            }
        }

        private void ActividadLoad(Actividades Window = null)
        {
            try
            {
                if (TabActividadesSelected)
                    if (SelectIngreso != null)
                        base.ClearRules();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar actividad", ex);
            }
        }


        //unload
        private void FactoresUnload(Factores Window = null)
        {
            try
            {
                if (SelectIngreso != null)
                    if (!base.HasErrors)
                        GuardarFactores();
                    else
                    {
                        setValidacionesFactores();
                        TabFactoresSocioFamiliaresSelected = true;
                        TabFactorSelected = true;
                    }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al salir de factores", ex);
            }
        }

        private void HPSUnload(HomosexualidadPandillaSexualidad Window = null)
        {
            try
            {
                if (SelectIngreso != null)
                {
                    if (!base.HasErrors)
                    {
                        GuardarHomosexualidadPandillaSexualidad();
                    }
                    else
                    {
                        TabConductasParasocialesSelected = true;
                        TabHPSSelected = true;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al salir de HPS", ex);
            }
        }

        private void TatuajesUnload(Tatuajes Window = null)
        {
            try
            {
                if (SelectIngreso != null)
                {
                    if (!base.HasErrors)
                    {
                        GuardarTatuajes();
                    }
                    else
                    {
                        TabConductasParasocialesSelected = true;
                        TabTatuajesSelected = true;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al salir de tatuajes", ex);
            }
        }

        private void EnfermedadesUnload(Enfermedades Window = null)
        {
            try
            {
                if (SelectIngreso != null)
                {
                    if (!base.HasErrors)
                    {
                        GuardarEnfermedades();
                    }
                    else
                    {
                        TabConductasParasocialesSelected = true;
                        TabEnfermedadesSelected = true;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al salir de enfermedades", ex);
            }
        }

        private void ActividadUnload(Actividades Window = null)
        {
            try
            {
                if (SelectIngreso != null)
                {
                    GuardarActividades();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al salir de actividad", ex);
            }
        }

        private void SeniaParticularUnload(TopografiaHumanaView Window = null)
        {
            try
            {
                if (SelectIngreso != null)
                {
                    SelectPresentaIngresar = true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al salir de señas particulares", ex);
            }
        }
        #endregion

        #region EMI
        private void PopulateEMI()
        {
            try
            {
                var ing = SelectIngreso;
                #region Encabezado
                Cereso = ing.CENTRO.DESCR;
                ClasificacionJuridica = ing.CLASIFICACION_JURIDICA.DESCR;

                if (ing.CAMA != null)
                    Ubicacion = string.Format("{0}-{1}-{2}-{3}", ing.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim(), ing.CAMA.CELDA.SECTOR.DESCR.Trim(),
                        ing.CAMA.CELDA.ID_CELDA.Trim(),
                        ing.CAMA.ID_CAMA);

                Expediente = string.Format("{0}/{1}", ing.IMPUTADO.ID_ANIO, ing.IMPUTADO.ID_IMPUTADO);
                #region Causas Penales
                Ingreso = ing.FEC_INGRESO_CERESO.Value;
                var causas = string.Empty;
                foreach (var causa in ing.CAUSA_PENAL)
                {
                    if (causa == (ing.CAUSA_PENAL.First()))
                        causas = string.Format("{0}/{1}", causa.CP_ANIO, causa.CP_FOLIO);
                    else
                        causas += string.Format(", {0}/{1}", causa.CP_ANIO, causa.CP_FOLIO); ;
                }
                CausaPenal = causas;
                #endregion
                #endregion
                #region Ficha ID
                try
                {
                    ApellidoPaterno = ing.IMPUTADO.PATERNO;
                    ApellidoMaterno = ing.IMPUTADO.MATERNO;
                    Nombre = ing.IMPUTADO.NOMBRE;
                    //if (ing.IMPUTADO.ESTADO_CIVIL != null)
                    //    EstadoCivil = ing.IMPUTADO.ESTADO_CIVIL.DESCR;
                    EstadoCivil = ing.ESTADO_CIVIL != null ? ing.ESTADO_CIVIL.DESCR : string.Empty;
                    if (ing.IMPUTADO != null)
                        Sexo = ing.IMPUTADO.SEXO == "M" ? "MASCULINO" : "FEMENINO";
                    FechaNacimiento = ing.IMPUTADO.NACIMIENTO_FECHA;
                    if (ing.IMPUTADO.NACIMIENTO_FECHA != null)
                    {
                        EdadInterno = (short)new Fechas().CalculaEdad(ing.IMPUTADO.NACIMIENTO_FECHA);
                        Edad = new Fechas().CalculaEdad(ing.IMPUTADO.NACIMIENTO_FECHA).ToString();
                    }
                    else
                        EdadInterno = 0;
                    //Religion = ing.IMPUTADO.RELIGION.DESCR;
                    Religion = ing.RELIGION != null ? ing.RELIGION.DESCR : string.Empty;
                    Etnia = ing.IMPUTADO.ETNIA.DESCR;
                    var _apodos = string.Empty;
                    foreach (var apodo in ing.IMPUTADO.APODO)
                    {
                        if (apodo == (ing.IMPUTADO.APODO.First()))
                            _apodos += apodo.APODO1;
                        else
                            _apodos += string.Format(", {0}", apodo.APODO1); ;
                    }
                    Apodo = _apodos;
                    var objNac = new SSP.Controlador.Catalogo.Justicia.cMunicipio();
                    var LNacimiento = objNac.Obtener(ing.IMPUTADO.NACIMIENTO_ESTADO.Value, ing.IMPUTADO.NACIMIENTO_MUNICIPIO.Value).FirstOrDefault();
                    LNMunicipio = LNacimiento.MUNICIPIO1;
                    LNEstado = LNacimiento.ENTIDAD.DESCR;
                    LNPais = LNacimiento.ENTIDAD.PAIS_NACIONALIDAD.PAIS;
                    Nacionalidad = LNacimiento.ENTIDAD.PAIS_NACIONALIDAD.NACIONALIDAD;
                    //var llegadaImputado = Fechas.GetFechaDateServer.Date.AddYears(-ing.IMPUTADO.RESIDENCIA_ANIOS.Value);
                    //llegadaImputado = llegadaImputado.AddMonths(-ing.IMPUTADO.RESIDENCIA_MESES.Value);
                    //FechaLlegada = llegadaImputado;
                    //Años = ing.IMPUTADO.RESIDENCIA_ANIOS;
                    //Meses = ing.IMPUTADO.RESIDENCIA_MESES;
                    var llegadaImputado = Fechas.GetFechaDateServer.Date.AddYears(-ing.RESIDENCIA_ANIOS.Value);
                    llegadaImputado = llegadaImputado.AddMonths(-ing.RESIDENCIAS_MESES.Value);
                    FechaLlegada = llegadaImputado;
                    Años = ing.RESIDENCIA_ANIOS;
                    Meses = ing.RESIDENCIAS_MESES;
                    //if (ing.IMPUTADO.PAIS_NACIONALIDAD != null)
                    //    DPais = ing.IMPUTADO.PAIS_NACIONALIDAD.PAIS;
                    //DEstado = ing.IMPUTADO.ENTIDAD.DESCR;
                    //DMunicipio = ing.IMPUTADO.MUNICIPIO.MUNICIPIO1;
                    //Colonia = ing.IMPUTADO.COLONIA.DESCR;
                    //Calle = ing.IMPUTADO.DOMICILIO_CALLE;
                    //NumeroExterior = ing.IMPUTADO.DOMICILIO_NUM_EXT.Value;
                    //NumeroInterior = ing.IMPUTADO.DOMICILIO_NUM_INT;
                    //CodigoPostal = ing.IMPUTADO.DOMICILIO_CODIGO_POSTAL;
                    if (ing.PAIS_NACIONALIDAD != null)
                        DPais = ing.PAIS_NACIONALIDAD.PAIS;
                    DEstado = ing.MUNICIPIO.ENTIDAD.DESCR;
                    DMunicipio = ing.MUNICIPIO.MUNICIPIO1;
                    Colonia = ing.COLONIA.DESCR;
                    Calle = ing.DOMICILIO_CALLE;
                    NumeroExterior = ing.DOMICILIO_NUM_EXT.Value;
                    NumeroInterior = ing.DOMICILIO_NUM_INT;
                    CodigoPostal = ing.DOMICILIO_CP;
                }
                catch (Exception ex)
                {
                    var metro = Application.Current.Windows[0] as MahApps.Metro.Controls.MetroWindow;
                    var mySettings = new MahApps.Metro.Controls.Dialogs.MetroDialogSettings()
                    {
                        AffirmativeButtonText = "OK",
                        AnimateHide = true,
                        AnimateShow = true
                    };
                    //var result = await metro.ShowOverlayAsync();
                    // dialogo.
                }
                #endregion
                if (ing.EMI_INGRESO != null)
                {
                    if (SelectIngreso.EMI_INGRESO != null)
                    {
                        var emi_ingresos = SelectIngreso.EMI_INGRESO.OrderByDescending(w => w.ID_EMI_CONS).FirstOrDefault();
                        if (emi_ingresos != null)
                        {
                            //if (emi_ingresos.EMI.ESTATUS == "A")
                            emiActual = emi_ingresos.EMI;
                        }
                    }
                    //OBTENEMOS EL ULTIMO EMI PARA INFORMACION HISTORICA
                    ObtenerUltimoEMI();
                    if (emiActual == null)
                        emiActual = new EMI();

                    //FICHA IDENTIFICACION
                    PopulateFichaIdentificacion();
                    //SITUACION JURIDICA
                    //PopulateSentenciaDelito();//SENTENCIA
                    CalcularSentencia();//SENTENCIA
                    PopulateEstudiosTraslados();
                    PopulateIngresosAnteriores();
                    PopulateIngresosAnterioresMenores();
                    //FACTORES SOCIOFAMILIARES
                    PopulateFactores();
                    PopulateDatosGrupoFamiliar();
                    GrupoFamiliarAntecedentes();
                    GrupoFamiliarDroga();
                    //CONDUCTAS PARASOCIALES
                    PopulateUsoDrogas();
                    PopulateHPS();
                    PopulateTatuajes();
                    PopulateSeniaParticular();
                    PopulateEnfermedades();
                    //CLASIFICACION CRIMINOLOGICA
                    PopularClasificacionCriminologica();
                    PopulateFactorCrimidiagnostico();
                    //ACTIVIDADES
                    PopulateActividades();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer emi", ex);
            }
        }

        private void LimpiarEMI(int op)
        {
            try
            {
                ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new EntrevistaMultidiciplinariaView();
                if (op == 1)
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.EntrevistaMultidiciplinariaViewModel();
                else
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.EntrevistaMultidiciplinariaViewModel(SelectIngreso);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar emi", ex);
            }
        }

        private void ObtenerUltimoEMI()
        {
            try
            {
                AnteriorEMI = null;
                if (SelectIngreso != null)
                {
                    var res = new cEmiIngreso().ObtenerEmiInterno(SelectIngreso.ID_CENTRO, SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO);
                    if (res != null)
                    {
                        AnteriorEMI = res.EMI;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener ultimo emi", ex);
            }
        }
        #endregion

        #region [Metodos Tratamiento]
        #region [Funcionalidad Ventana]
        private void OnLoadTratamiento(Grid DynamicGrid)
        {
            try
            {
                IsEnabledTratamiento = true;
                ListadoEjes = new cEjes().GetData().ToList();
                _DynamicGrid = DynamicGrid;
                LoadGrid(DynamicGrid);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar retícula", ex);
            }
        }

        private void AddEje(Grid DynamicGrid, object SelectedValue = null)
        {
            try
            {
                if (COLUMN >= ListadoEjes.Count)
                {
                    SetAlert("No puede agregar más ejes");
                    return;
                }
                if (COLUMN != 0)
                    if (ValidarCampos(DynamicGrid))
                        return;

                DynamicGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(625) });

                var Ejes = DynamicGrid.Children.Cast<UIElement>().Where(w => Grid.GetRow(w) == 0 && w is ComboBox).Cast<ComboBox>().Select(s => s.SelectedValue).ToList();

                AgregarComboBoxEje(DynamicGrid, ListadoEjes.Where(w => !Ejes.Contains(w.ID_EJE)).ToList(), "DESCR", "ID_EJE", SelectedValue);

                for (int i = 1; i <= ROW; i++)
                    for (int j = COLUMN - 1; j < COLUMN; j++)
                        GenerateFields(DynamicGrid, i, j);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar eje", ex);
            }
        }

        private void AddActividad(Grid DynamicGrid)
        {
            try
            {
                if (ROW != 1)
                    if (ValidarCampos(DynamicGrid))
                        return;

                if (ROW == 0 && COLUMN == 0)
                    DynamicGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(60) });
                else
                    DynamicGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                for (int j = 0; j < COLUMN; j++)
                    GenerateFields(DynamicGrid, ROW, j);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar actividad", ex);
            }
        }

        private void SaveReticula(Grid DynamicGrid)
        {
            try
            {
                if (emiActual.EMI_INGRESO.INGRESO.GRUPO_PARTICIPANTE.Count > 0)
                {
                    if (!PEditar)
                    {
                        StaticSourcesViewModel.Mensaje("Validación", "Su usuario no cuenta con permisos para realizar esta acción.", StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA, 5);
                        return;
                    }
                }
                else
                {
                    if (!PInsertar)
                    {
                        StaticSourcesViewModel.Mensaje("Validación", "Su usuario no cuenta con permisos para realizar esta acción.", StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA, 5);
                        return;
                    }
                }
                var hoy = Fechas.GetFechaDateServer;
                SetAlert("Espere Por Favor, Guardando Retícula...", 1000);
                for (int i = 0; i < COLUMN; i++)
                {
                    for (int j = 1; j <= ROW; j++)
                    {
                        var datagrid = DynamicGrid.Children.Cast<UIElement>().Where(w => Grid.GetColumn(w) == i && Grid.GetRow(w) == j).DefaultIfEmpty().Where(w => w is DataGrid).Cast<DataGrid>().FirstOrDefault();
                        if (datagrid.ItemsSource == null)
                            break;

                        var datagridsource = datagrid.ItemsSource.Cast<GridTratamientoActividad>().ToList().Where(w => w.ELIGE == true && w.ESTATUSVALUE == null).ToList();
                        foreach (var item in datagridsource.Select(s => s.actividad_eje))
                        {
                            new cGrupoParticipante().InsertarParticipante(new GRUPO_PARTICIPANTE
                            {
                                ID_CENTRO = SelectIngreso.ID_UB_CENTRO.Value,//SelectIngreso.ID_CENTRO,
                                ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA,
                                ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                ID_GRUPO = null,
                                ING_ID_CENTRO = SelectIngreso.ID_CENTRO,
                                ING_ID_ANIO = SelectIngreso.ID_ANIO,
                                ING_ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                ING_ID_INGRESO = SelectIngreso.ID_INGRESO,
                                FEC_REGISTRO = hoy,
                                ESTATUS = (short)1,
                                EJE = item.ID_EJE
                            });
                        }
                    }
                }
                StaticSourcesViewModel.Mensaje("Tratamiento", "Grabado Exitosamente", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);

                LoadGrid(DynamicGrid);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar retícula", ex);
            }
        }

        private void LoadGrid(Grid DynamicGrid)
        {
            try
            {
                if (IngresoProgramas != null)
                    SelectIngreso = IngresoProgramas;
                
                SetAlert("Espere Por Favor, Cargando Retícula...", 1000);

                DynamicGrid.Children.Clear();
                DynamicGrid.RowDefinitions.Clear();
                DynamicGrid.ColumnDefinitions.Clear();
                ROW = 0;
                COLUMN = 0;
                var Columnas = new List<IGrouping<EJE, GRUPO_PARTICIPANTE>>();

                if (SelectIngreso != null)
                    Columnas = new cGrupoParticipante().GetData()
                        .Where(w => w.ID_CENTRO == SelectIngreso.ID_UB_CENTRO && w.ING_ID_ANIO == SelectIngreso.ID_ANIO && w.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && w.ING_ID_INGRESO == SelectIngreso.ID_INGRESO)
                        .GroupBy(g => g.EJE1)
                        .OrderBy(o => o.Key.ORDEN)
                        .ToList();

                ListaReticula = new List<Reticula>();
                var ListActividades = new List<TratamientoActividades>();
                if (Columnas.Count == 0)
                    VisibleCargarHistorico = Visibility.Visible;
                foreach (var item in Columnas)
                {
                    VisibleCargarHistorico = Visibility.Collapsed;
                    ListActividades = new List<TratamientoActividades>();
                    foreach (var subitem in item.OrderBy(o => o.ACTIVIDAD.ORDEN).ToList())
                    {
                        ListActividades.Add(new TratamientoActividades
                        {
                            ActividadValue = subitem.ID_ACTIVIDAD,
                            DepartamentoValue = subitem.ACTIVIDAD.TIPO_PROGRAMA.DEPARTAMENTO.ID_DEPARTAMENTO,
                            ProgramaValue = subitem.ACTIVIDAD.TIPO_PROGRAMA.ID_TIPO_PROGRAMA,
                            EstatusValue = subitem.ESTATUS,
                            grupo_participante = subitem
                        });
                    }
                    ListaReticula.Add(new Reticula
                    {
                        Eje = item.Key.ID_EJE,
                        FechaRegistroValue = item.FirstOrDefault().FEC_REGISTRO,
                        Actividad = ListActividades
                    });
                }
                if (ListaReticula.Count > 0)
                    for (int i = 0; i < ListaReticula.Count; i++)
                    {
                        DynamicGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(625) });

                        var Ejes = DynamicGrid.Children.Cast<UIElement>().Where(w => Grid.GetRow(w) == 0 && w is ComboBox).Cast<ComboBox>().Select(s => s.SelectedValue).ToList();

                        AgregarComboBoxEje(DynamicGrid, ListadoEjes.Where(w => !Ejes.Contains(w.ID_EJE)).ToList(), "DESCR", "ID_EJE", ListaReticula[i].Eje, false);

                        if (ROW == 0)
                        {
                            DynamicGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(60) });
                            AddRow();
                        }
                        LoadFields(DynamicGrid, ROW, i, ListaReticula[i].Eje, ListaReticula[i].Actividad, ListaReticula[i].FechaRegistroValue);
                    }
                else
                    CleanGrid(DynamicGrid);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la información", ex);
            }
        }

        private void CleanGrid(Grid DynamicGrid)
        {
            try
            {
                DynamicGrid.Children.Clear();
                DynamicGrid.RowDefinitions.Clear();
                DynamicGrid.ColumnDefinitions.Clear();
                ROW = 0;
                COLUMN = 0;
                AddEje(DynamicGrid);
                DynamicGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(60) });
                AddRow();
                AddActividad(DynamicGrid);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar la retícula", ex);
            }
        }

        private void SetAlert(string Mensaje, short Duracion = 2500)
        {
            try
            {
                if (!ErrorText.Equals(string.Empty))
                    return;
                ErrorText = Mensaje.ToUpper();
                Task.Factory.StartNew(async () => { await TaskEx.Delay(Duracion); ErrorText = string.Empty; });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar alerta", ex);
            }
        }

        private bool ValidarCampos(Grid DynamicGrid)
        {
            try
            {
                var ValidarEjes = DynamicGrid.Children.Cast<UIElement>().Where(w => w is ComboBox).Cast<ComboBox>().ToList();
                if (ValidarEjes.Any(a => a.SelectedValue == null))
                {
                    SetAlert("Selecciona Eje Para Continuar");
                    return true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar alerta", ex);
                return true;
            }
            return false;
        }
        #endregion

        #region [Funcionalidad Grid]
        private void GenerateFields(Grid DynamicGrid, int Row, int Column)
        {
            try
            {
                CreateDataGrid(DynamicGrid, Row, Column);
                AgregarTextBlock(DynamicGrid, null, "REGISTRO " + Fechas.GetFechaDateServer.ToShortDateString(), 0, Column);
                AgregarCheckBox(DynamicGrid, "Desmarcar Todos", 0, Column);
                AgregarButton(DynamicGrid, "Quitar", 0, Column);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar los campos de la celda", ex);
            }
        }

        private void LoadFields(Grid DynamicGrid, int Row, int Column, short? Eje, List<TratamientoActividades> ListValue = null, DateTime? FechaRegistro = null)
        {
            try
            {
                if (ROW == 0 && COLUMN == 0)
                    DynamicGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(60) });
                else
                    DynamicGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                if (ListValue.Count > 0)
                {
                    if (Row != 0)
                    {
                        var dg = CreateDataGrid(DynamicGrid, Row, Column);
                        var listactividades = new cActividadEje().GetData().Where(w => w.ID_EJE == Eje.Value).OrderBy(o => o.ACTIVIDAD.TIPO_PROGRAMA.DEPARTAMENTO.ORDEN).ThenBy(t => t.ACTIVIDAD.TIPO_PROGRAMA.ORDEN).ThenBy(t => t.ACTIVIDAD.ORDEN).ToList();
                        var _departamento = string.Empty;
                        var _programa = string.Empty;
                        var _actividad = string.Empty;
                        var itemsourceDatagrid = new List<GridTratamientoActividad>();

                        foreach (var item in listactividades)
                        {
                            itemsourceDatagrid.Add(new GridTratamientoActividad
                            {
                                DEPARTAMENTO = _departamento.Equals(item.ACTIVIDAD.TIPO_PROGRAMA.DEPARTAMENTO.DESCR) ? string.Empty : item.ACTIVIDAD.TIPO_PROGRAMA.DEPARTAMENTO.DESCR,
                                PROGRAMA = _programa.Equals(item.ACTIVIDAD.TIPO_PROGRAMA.NOMBRE) ? string.Empty : item.ACTIVIDAD.TIPO_PROGRAMA.NOMBRE,
                                ACTIVIDAD = _actividad.Equals(item.ACTIVIDAD.DESCR) ? string.Empty : item.ACTIVIDAD.DESCR,
                                ELIGE = ListValue.Where(w => w.grupo_participante.ID_CENTRO == SelectIngreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == SelectIngreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == SelectIngreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == SelectIngreso.ID_INGRESO).Any(),

                                ESTATUS =

                                ListValue.Where(w => w.grupo_participante.ID_CENTRO == SelectIngreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == SelectIngreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == SelectIngreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == SelectIngreso.ID_INGRESO).Any() ?

                                ListValue.Where(w => w.grupo_participante.ID_CENTRO == SelectIngreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == SelectIngreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == SelectIngreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == SelectIngreso.ID_INGRESO).FirstOrDefault().grupo_participante.GRUPO != null ?

                                ListValue.Where(w => w.grupo_participante.ID_CENTRO == SelectIngreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == SelectIngreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == SelectIngreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == SelectIngreso.ID_INGRESO).FirstOrDefault().grupo_participante.GRUPO.ID_ESTATUS_GRUPO != 1 ?

                                ListValue.Where(w => w.grupo_participante.ID_CENTRO == SelectIngreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == SelectIngreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == SelectIngreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == SelectIngreso.ID_INGRESO).FirstOrDefault().grupo_participante.GRUPO.GRUPO_ESTATUS.DESCR :

                                ListValue.Where(w => w.grupo_participante.ID_CENTRO == SelectIngreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == SelectIngreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == SelectIngreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == SelectIngreso.ID_INGRESO).FirstOrDefault().grupo_participante.GRUPO_PARTICIPANTE_ESTATUS.DESCR

                                : ListValue.Where(w => w.grupo_participante.ID_CENTRO == SelectIngreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == SelectIngreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == SelectIngreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == SelectIngreso.ID_INGRESO).FirstOrDefault().grupo_participante.GRUPO_PARTICIPANTE_ESTATUS.DESCR

                                : string.Empty,

                                ESTATUSVALUE =
                                IsHistorico ? null :

                                ListValue.Where(w => w.grupo_participante.ID_CENTRO == SelectIngreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == SelectIngreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == SelectIngreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == SelectIngreso.ID_INGRESO).Any() ?

                                 ListValue.Where(w => w.grupo_participante.ID_CENTRO == SelectIngreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == SelectIngreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == SelectIngreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == SelectIngreso.ID_INGRESO).FirstOrDefault().grupo_participante.GRUPO != null ?

                                ListValue.Where(w => w.grupo_participante.ID_CENTRO == SelectIngreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == SelectIngreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == SelectIngreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == SelectIngreso.ID_INGRESO).FirstOrDefault().grupo_participante.GRUPO.ID_ESTATUS_GRUPO != 1 ?

                                 ListValue.Where(w => w.grupo_participante.ID_CENTRO == SelectIngreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == SelectIngreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == SelectIngreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == SelectIngreso.ID_INGRESO).FirstOrDefault().grupo_participante.GRUPO.GRUPO_ESTATUS.ID_ESTATUS_GRUPO :

                                ListValue.Where(w => w.grupo_participante.ID_CENTRO == SelectIngreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == SelectIngreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == SelectIngreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == SelectIngreso.ID_INGRESO).FirstOrDefault().grupo_participante.GRUPO_PARTICIPANTE_ESTATUS.ID_ESTATUS

                                : ListValue.Where(w => w.grupo_participante.ID_CENTRO == SelectIngreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == SelectIngreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == SelectIngreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == SelectIngreso.ID_INGRESO).FirstOrDefault().grupo_participante.GRUPO_PARTICIPANTE_ESTATUS.ID_ESTATUS

                                : new Nullable<short>(),

                                actividad_eje = item,

                                APROVADO =
                                ListValue.Where(w => w.grupo_participante.ID_CENTRO == SelectIngreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == SelectIngreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == SelectIngreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == SelectIngreso.ID_INGRESO).Any() ?

                                ListValue.Where(w => w.grupo_participante.ID_CENTRO == SelectIngreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == SelectIngreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == SelectIngreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == SelectIngreso.ID_INGRESO).FirstOrDefault().grupo_participante.NOTA_TECNICA.Any() ?

                                ListValue.Where(w => w.grupo_participante.ID_CENTRO == SelectIngreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == SelectIngreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == SelectIngreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == SelectIngreso.ID_INGRESO).FirstOrDefault().grupo_participante.NOTA_TECNICA.Where(w => w.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.ID_CENTRO == SelectIngreso.ID_CENTRO && w.ID_CONSEC == ListValue.Where(wh => wh.grupo_participante.ID_CENTRO == SelectIngreso.ID_UB_CENTRO && wh.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && wh.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && wh.grupo_participante.ING_ID_CENTRO == SelectIngreso.ID_CENTRO && wh.grupo_participante.ING_ID_ANIO == SelectIngreso.ID_ANIO && wh.grupo_participante.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && wh.grupo_participante.ING_ID_INGRESO == SelectIngreso.ID_INGRESO).FirstOrDefault().grupo_participante.ID_CONSEC && w.ID_GRUPO == ListValue.Where(wh => wh.grupo_participante.ID_CENTRO == SelectIngreso.ID_UB_CENTRO && wh.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && wh.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && wh.grupo_participante.ING_ID_CENTRO == SelectIngreso.ID_CENTRO && wh.grupo_participante.ING_ID_ANIO == SelectIngreso.ID_ANIO && wh.grupo_participante.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && wh.grupo_participante.ING_ID_INGRESO == SelectIngreso.ID_INGRESO).FirstOrDefault().grupo_participante.ID_GRUPO && w.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA).FirstOrDefault() != null ?

                                ListValue.Where(w => w.grupo_participante.ID_CENTRO == SelectIngreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == SelectIngreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == SelectIngreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == SelectIngreso.ID_INGRESO).FirstOrDefault().grupo_participante.NOTA_TECNICA.Where(w => w.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.ID_CENTRO == SelectIngreso.ID_CENTRO && w.ID_CONSEC == ListValue.Where(wh => wh.grupo_participante.ID_CENTRO == SelectIngreso.ID_UB_CENTRO && wh.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && wh.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && wh.grupo_participante.ING_ID_CENTRO == SelectIngreso.ID_CENTRO && wh.grupo_participante.ING_ID_ANIO == SelectIngreso.ID_ANIO && wh.grupo_participante.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && wh.grupo_participante.ING_ID_INGRESO == SelectIngreso.ID_INGRESO).FirstOrDefault().grupo_participante.ID_CONSEC && w.ID_GRUPO == ListValue.Where(wh => wh.grupo_participante.ID_CENTRO == SelectIngreso.ID_UB_CENTRO && wh.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && wh.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && wh.grupo_participante.ING_ID_CENTRO == SelectIngreso.ID_CENTRO && wh.grupo_participante.ING_ID_ANIO == SelectIngreso.ID_ANIO && wh.grupo_participante.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && wh.grupo_participante.ING_ID_INGRESO == SelectIngreso.ID_INGRESO).FirstOrDefault().grupo_participante.ID_GRUPO && w.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA).FirstOrDefault().NOTA_TECNICA_ESTATUS.ID_ESTATUS == 1
                                : false : false : false,
                                NOTA = ListValue.Where(w => w.grupo_participante.ID_CENTRO == SelectIngreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == SelectIngreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == SelectIngreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == SelectIngreso.ID_INGRESO).Any() ?

                                ListValue.Where(w => w.grupo_participante.ID_CENTRO == SelectIngreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == SelectIngreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == SelectIngreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == SelectIngreso.ID_INGRESO).FirstOrDefault().grupo_participante.NOTA_TECNICA.Any() ?

                                ListValue.Where(w => w.grupo_participante.ID_CENTRO == SelectIngreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == SelectIngreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == SelectIngreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == SelectIngreso.ID_INGRESO).FirstOrDefault().grupo_participante.NOTA_TECNICA.Where(w => w.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.ID_CENTRO == SelectIngreso.ID_CENTRO && w.ID_CONSEC == ListValue.Where(wh => wh.grupo_participante.ID_CENTRO == SelectIngreso.ID_UB_CENTRO && wh.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && wh.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && wh.grupo_participante.ING_ID_CENTRO == SelectIngreso.ID_CENTRO && wh.grupo_participante.ING_ID_ANIO == SelectIngreso.ID_ANIO && wh.grupo_participante.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && wh.grupo_participante.ING_ID_INGRESO == SelectIngreso.ID_INGRESO).FirstOrDefault().grupo_participante.ID_CONSEC && w.ID_GRUPO == ListValue.Where(wh => wh.grupo_participante.ID_CENTRO == SelectIngreso.ID_UB_CENTRO && wh.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && wh.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && wh.grupo_participante.ING_ID_CENTRO == SelectIngreso.ID_CENTRO && wh.grupo_participante.ING_ID_ANIO == SelectIngreso.ID_ANIO && wh.grupo_participante.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && wh.grupo_participante.ING_ID_INGRESO == SelectIngreso.ID_INGRESO).FirstOrDefault().grupo_participante.ID_GRUPO && w.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA).FirstOrDefault() != null ?

                                ListValue.Where(w => w.grupo_participante.ID_CENTRO == SelectIngreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == SelectIngreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == SelectIngreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == SelectIngreso.ID_INGRESO).FirstOrDefault().grupo_participante.NOTA_TECNICA.Where(w => w.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.ID_CENTRO == SelectIngreso.ID_CENTRO && w.ID_CONSEC == ListValue.Where(wh => wh.grupo_participante.ID_CENTRO == SelectIngreso.ID_UB_CENTRO && wh.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && wh.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && wh.grupo_participante.ING_ID_CENTRO == SelectIngreso.ID_CENTRO && wh.grupo_participante.ING_ID_ANIO == SelectIngreso.ID_ANIO && wh.grupo_participante.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && wh.grupo_participante.ING_ID_INGRESO == SelectIngreso.ID_INGRESO).FirstOrDefault().grupo_participante.ID_CONSEC && w.ID_GRUPO == ListValue.Where(wh => wh.grupo_participante.ID_CENTRO == SelectIngreso.ID_UB_CENTRO && wh.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && wh.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && wh.grupo_participante.ING_ID_CENTRO == SelectIngreso.ID_CENTRO && wh.grupo_participante.ING_ID_ANIO == SelectIngreso.ID_ANIO && wh.grupo_participante.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && wh.grupo_participante.ING_ID_INGRESO == SelectIngreso.ID_INGRESO).FirstOrDefault().grupo_participante.ID_GRUPO && w.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA).FirstOrDefault().NOTA
                                : string.Empty : string.Empty : string.Empty,

                                ASISTENCIA =
                                ListValue.Where(w => w.grupo_participante.ID_CENTRO == SelectIngreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == SelectIngreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == SelectIngreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == SelectIngreso.ID_INGRESO).Any() ?

                                ObtenerPorcentajeAsistencia(ListValue.Where(w => w.grupo_participante.ID_CENTRO == SelectIngreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == SelectIngreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == SelectIngreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == SelectIngreso.ID_INGRESO).FirstOrDefault().grupo_participante, ListValue.Where(w => w.grupo_participante.ID_CENTRO == SelectIngreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == SelectIngreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == SelectIngreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == SelectIngreso.ID_INGRESO).Select(s => s.grupo_participante).ToList())
                                : string.Empty
                            });

                            if (!_departamento.Equals(item.ACTIVIDAD.TIPO_PROGRAMA.DEPARTAMENTO.DESCR))
                                _departamento = item.ACTIVIDAD.TIPO_PROGRAMA.DEPARTAMENTO.DESCR;

                            if (!_programa.Equals(item.ACTIVIDAD.TIPO_PROGRAMA.NOMBRE))
                                _programa = item.ACTIVIDAD.TIPO_PROGRAMA.NOMBRE;

                            if (!_actividad.Equals(item.ACTIVIDAD.DESCR))
                                _actividad = item.ACTIVIDAD.DESCR;
                        }
                        dg.ItemsSource = itemsourceDatagrid;
                        AgregarTextBlock(DynamicGrid, null, "REGISTRO " + FechaRegistro.Value.ToShortDateString(), 0, Column);
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los campos desde la base de datos", ex);
            }
        }

        private int AddRow()
        {
            return ROW++;
        }

        private int AddColumn()
        {
            return COLUMN++;
        }
        #endregion
        #region [Funcionalidad Agregar Elementos]
        private DataGrid CreateDataGrid(Grid DynamicGrid, int Row, int Column, short EstatusValue = 1, bool Enabled = true)
        {
            var DynamicDataGrid = new DataGrid();
            try
            {
                DynamicDataGrid.Name = "ContenedorGrid";
                DynamicDataGrid.Columns.Add(new DataGridTextColumn() { Binding = new Binding("DEPARTAMENTO"), Header = "DEPARTAMENTO", CanUserSort = false, IsReadOnly = true, FontSize = 12, MaxWidth = 200, ElementStyle = new Style(typeof(TextBlock)) { Setters = { new Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap) } } });
                DynamicDataGrid.Columns.Add(new DataGridTextColumn() { Binding = new Binding("PROGRAMA"), Header = "PROGRAMA", CanUserSort = false, IsReadOnly = true, FontSize = 12, MaxWidth = 200, ElementStyle = new Style(typeof(TextBlock)) { Setters = { new Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap) } } });
                DynamicDataGrid.Columns.Add(new DataGridTextColumn() { Binding = new Binding("ACTIVIDAD"), Header = "ACTIVIDAD", CanUserSort = false, IsReadOnly = true, FontSize = 12, MaxWidth = 135, ElementStyle = new Style(typeof(TextBlock)) { Setters = { new Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap) } } });

                var RadioFactoryElige = new FrameworkElementFactory(typeof(CheckBox));
                RadioFactoryElige.SetValue(CheckBox.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                RadioFactoryElige.AddHandler(CheckBox.CheckedEvent, new RoutedEventHandler(chkSelect_Checked));
                RadioFactoryElige.AddHandler(CheckBox.UncheckedEvent, new RoutedEventHandler(chkSelect_Unchecked));
                RadioFactoryElige.SetBinding(CheckBox.IsCheckedProperty, new Binding("ELIGE") { Mode = BindingMode.TwoWay });

                DynamicDataGrid.Columns.Add(new DataGridTemplateColumn() { CellStyle = new Style() { Setters = { new Setter(CheckBox.HorizontalContentAlignmentProperty, HorizontalAlignment.Center) } }, Header = "ELIGE", CellTemplate = new DataTemplate() { VisualTree = RadioFactoryElige } });

                DynamicDataGrid.Columns.Add(new DataGridTextColumn() { Binding = new Binding("ESTATUS"), Header = "ESTATUS", CanUserSort = false, IsReadOnly = true, FontSize = 12, MaxWidth = 200, ElementStyle = new Style(typeof(TextBlock)) { Setters = { new Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap) } } });

                var RadioFactoryAprovado = new FrameworkElementFactory(typeof(CheckBox));
                RadioFactoryAprovado.SetValue(CheckBox.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                RadioFactoryAprovado.SetValue(CheckBox.IsEnabledProperty, false);
                RadioFactoryAprovado.SetBinding(CheckBox.IsCheckedProperty, new Binding("APROVADO") { Mode = BindingMode.TwoWay });

                DynamicDataGrid.Columns.Add(new DataGridTemplateColumn() { CellStyle = new Style() { Setters = { new Setter(CheckBox.HorizontalContentAlignmentProperty, HorizontalAlignment.Center) } }, IsReadOnly = true, Header = "APROBADO", CellTemplate = new DataTemplate() { VisualTree = RadioFactoryAprovado } });

                DynamicDataGrid.Columns.Add(new DataGridTextColumn() { Binding = new Binding("NOTA"), Header = "NOTA", CanUserSort = false, IsReadOnly = true, FontSize = 12, MaxWidth = 200, ElementStyle = new Style(typeof(TextBlock)) { Setters = { new Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap) } } });

                DynamicDataGrid.Columns.Add(new DataGridTextColumn() { Binding = new Binding("ASISTENCIA"), Header = "ASISTENCIA", CanUserSort = false, IsReadOnly = true, FontSize = 12, MaxWidth = 100, ElementStyle = new Style(typeof(TextBlock)) { Setters = { new Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap), new Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center) } } });


                DynamicDataGrid.FontSize = 14;
                DynamicDataGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
                DynamicDataGrid.VerticalAlignment = VerticalAlignment.Top;
                DynamicDataGrid.Margin = new Thickness(5, 5, 5, 5);
                DynamicDataGrid.AutoGenerateColumns = false;
                DynamicDataGrid.Style = (Style)Application.Current.FindResource("MetroDataGrid");
                DynamicDataGrid.CanUserAddRows = false;
                DynamicDataGrid.CanUserDeleteRows = false;
                DynamicDataGrid.VerticalGridLinesBrush = Brushes.Black;
                DynamicDataGrid.GridLinesVisibility = DataGridGridLinesVisibility.Vertical;
                DynamicDataGrid.SelectionMode = DataGridSelectionMode.Single;
                DynamicDataGrid.AutoGenerateColumns = false;

                Grid.SetRow(DynamicDataGrid, Row);
                Grid.SetColumn(DynamicDataGrid, Column);

                DynamicGrid.Children.Add(DynamicDataGrid);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al crear celda", ex);
            }
            return DynamicDataGrid;
        }

        private async void chkSelect_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                ((CheckBox)sender).Unchecked -= chkSelect_Unchecked;
                var row = ((GridTratamientoActividad)(((ContentPresenter)(((FrameworkElement)(sender)).TemplatedParent)).Content));
                if (row.ESTATUSVALUE != null)
                    if (row.ESTATUSVALUE != 1)
                    {
                        ((CheckBox)sender).IsChecked = true;
                        return;
                    }

                if (row.ESTATUSVALUE == 1)
                {
                    var Data = row.actividad_eje;
                    if (!PEditar)
                    {
                        StaticSourcesViewModel.Mensaje("Validación", "Su usuario no cuenta con permisos para realizar esta acción.", StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA, 5);
                        ((CheckBox)sender).IsChecked = true;
                        return;
                    }

                    if (await new Dialogos().ConfirmarEliminar("Tratamiento", "Seguro que desea quitar esta actividad?") == 1)
                    {
                        var dal = new cGrupoParticipante();

                        var entity = dal.GetData().Where(w => w.ID_CENTRO == SelectIngreso.ID_UB_CENTRO && w.ID_TIPO_PROGRAMA == Data.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == Data.ID_ACTIVIDAD && w.ING_ID_CENTRO == SelectIngreso.ID_CENTRO && w.ING_ID_ANIO == SelectIngreso.ID_ANIO && w.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && w.ING_ID_INGRESO == SelectIngreso.ID_INGRESO && w.EJE == Data.ID_EJE).FirstOrDefault();

                        if (entity != null)
                            dal.Delete(entity);

                        LoadGrid(_DynamicGrid);
                    }
                    else
                    {
                        ((CheckBox)sender).IsChecked = true;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al quitar actividad", ex);
            }
        }

        private void chkSelect_Checked(object sender, RoutedEventArgs e)
        {
            ((GridTratamientoActividad)(((ContentPresenter)(((FrameworkElement)(sender)).TemplatedParent)).Content)).ELIGE = ((CheckBox)sender).IsChecked.Value;
        }

        private void AgregarComboBoxEje(Grid DynamicGrid, IEnumerable ItemSource, string DisplayMemberPath, string SelectedValuePath, object SelectedValue, bool Enabled = true)
        {
            try
            {
                var addComboBox = new ComboBox();
                addComboBox.Name = "EJE_" + ROW + "_" + COLUMN;
                addComboBox.ItemsSource = ItemSource;
                addComboBox.DisplayMemberPath = DisplayMemberPath;
                addComboBox.SelectedValuePath = SelectedValuePath;
                addComboBox.SelectedValue = SelectedValue;
                addComboBox.SelectionChanged += (s, e) =>
                {
                    SelectionChanged(DynamicGrid, s, e);
                };
                addComboBox.FontSize = 14;
                addComboBox.FontWeight = FontWeights.Bold;
                addComboBox.VerticalAlignment = VerticalAlignment.Top;
                addComboBox.MaxHeight = 25;
                addComboBox.IsEnabled = Enabled;
                Grid.SetRow(addComboBox, 0);
                Grid.SetColumn(addComboBox, AddColumn());

                DynamicGrid.Children.Add(addComboBox);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al crear la lista de ejes", ex);
            }
        }

        private async void SelectionChanged(Grid DynamicGrid, object s, SelectionChangedEventArgs e)
        {
            try
            {
                if (HandleSelection)
                {
                    var previoscombo = ((Grid)((ComboBox)s).Parent).Children.Cast<UIElement>().Where(w => Grid.GetColumn(w) == (((int)((ComboBox)s).GetValue(Grid.ColumnProperty)) - 1) && w is ComboBox).Cast<ComboBox>().FirstOrDefault();
                    if (previoscombo != null)
                        if (previoscombo.SelectedValue == null)
                        {
                            ((ComboBox)s).SelectedValue = null;
                            SetAlert("No Se Puede Dejar Columnas Sin Eje");
                        }

                    var nextcombo = ((Grid)((ComboBox)s).Parent).Children.Cast<UIElement>().Where(w => Grid.GetColumn(w) == (((int)((ComboBox)s).GetValue(Grid.ColumnProperty)) + 1) && w is ComboBox).Cast<ComboBox>().FirstOrDefault();
                    if (nextcombo != null)
                        if (nextcombo.SelectedValue == null)
                            isalertcolumnas = false;

                    if (((int)((ComboBox)s).GetValue(Grid.ColumnProperty) + 1) == COLUMN)
                        isalertcolumnas = true;

                    for (int i = (((int)((ComboBox)s).GetValue(Grid.ColumnProperty)) + 1); i < COLUMN; i++)
                    {
                        if (i == (((int)((ComboBox)s).GetValue(Grid.ColumnProperty)) + 1))
                        {
                            if (isalertcolumnas)
                                isalertcolumnas = await new Dialogos().ConfirmarEliminar("Tratamiento", "la información de las columnas de la derecha serán borradas, desea continuar?") != 1;
                            if (isalertcolumnas)
                            {
                                HandleSelection = false;
                                ((ComboBox)s).SelectedValue = ((EJE)(((object[])(e.RemovedItems))[0])).ID_EJE;
                                isalertcolumnassame = true;
                                break;
                            }
                        }
                        var Eje = ((Grid)((ComboBox)s).Parent).Children.Cast<UIElement>().Where(w => Grid.GetColumn(w) == i && Grid.GetRow(w) == 0 && w is ComboBox).Cast<ComboBox>().FirstOrDefault();

                        Eje.ItemsSource = null;
                        var Ejes = ((Grid)((ComboBox)s).Parent).Children.Cast<UIElement>().Where(w => Grid.GetRow(w) == 0 && Grid.GetColumn(w) <= (int)((ComboBox)s).GetValue(Grid.ColumnProperty) && w is ComboBox).Cast<ComboBox>().Select(se => se.SelectedValue).Where(w => w != null).ToList();

                        Eje.ItemsSource = ListadoEjes.Where(w => !Ejes.Contains(w.ID_EJE)).ToList();
                    }

                    if (isalertcolumnassame)
                    {
                        isalertcolumnassame = false;
                        return;
                    }

                    var DynamicDataGrid = ((Grid)((ComboBox)s).Parent).Children.Cast<UIElement>().Where(w => Grid.GetColumn(w) == (int)((ComboBox)s).GetValue(Grid.ColumnProperty) && w is DataGrid).Cast<DataGrid>().FirstOrDefault();

                    if (((ComboBox)s).SelectedValue != null)
                    {
                        var checkbox = ((Grid)((ComboBox)s).Parent).Children.Cast<UIElement>().Where(w => Grid.GetColumn(w) == (int)((ComboBox)s).GetValue(Grid.ColumnProperty) && w is CheckBox).Cast<CheckBox>().FirstOrDefault();
                        var listactividades = new cActividadEje().GetData().Where(w => w.ID_EJE == (short)((ComboBox)s).SelectedValue).OrderBy(o => o.ACTIVIDAD.TIPO_PROGRAMA.DEPARTAMENTO.ORDEN).ThenBy(t => t.ACTIVIDAD.TIPO_PROGRAMA.ORDEN).ThenBy(t => t.ACTIVIDAD.ORDEN).ToList();

                        var _departamento = string.Empty;
                        var _programa = string.Empty;
                        var _actividad = string.Empty;

                        var itemsourceDatagrid = new List<GridTratamientoActividad>();
                        foreach (var item in listactividades)
                        {
                            itemsourceDatagrid.Add(new GridTratamientoActividad
                            {
                                DEPARTAMENTO = _departamento.Equals(item.ACTIVIDAD.TIPO_PROGRAMA.DEPARTAMENTO.DESCR) ? string.Empty : item.ACTIVIDAD.TIPO_PROGRAMA.DEPARTAMENTO.DESCR,
                                PROGRAMA = _programa.Equals(item.ACTIVIDAD.TIPO_PROGRAMA.NOMBRE) ? string.Empty : item.ACTIVIDAD.TIPO_PROGRAMA.NOMBRE,
                                ACTIVIDAD = _actividad.Equals(item.ACTIVIDAD.DESCR) ? string.Empty : item.ACTIVIDAD.DESCR,
                                ELIGE = checkbox != null ? checkbox.IsChecked.Value : true,
                                ESTATUS = string.Empty,
                                ESTATUSVALUE = new Nullable<short>(),
                                actividad_eje = item
                            });

                            if (!_departamento.Equals(item.ACTIVIDAD.TIPO_PROGRAMA.DEPARTAMENTO.DESCR))
                                _departamento = item.ACTIVIDAD.TIPO_PROGRAMA.DEPARTAMENTO.DESCR;

                            if (!_programa.Equals(item.ACTIVIDAD.TIPO_PROGRAMA.NOMBRE))
                                _programa = item.ACTIVIDAD.TIPO_PROGRAMA.NOMBRE;

                            if (!_actividad.Equals(item.ACTIVIDAD.DESCR))
                                _actividad = item.ACTIVIDAD.DESCR;
                        }

                        DynamicDataGrid.ItemsSource = itemsourceDatagrid;
                    }
                    else
                    {
                        CreateDataGrid(DynamicGrid, (int)(DynamicDataGrid).GetValue(Grid.RowProperty), (int)(DynamicDataGrid).GetValue(Grid.ColumnProperty));
                        ((Grid)((ComboBox)s).Parent).Children.Remove(DynamicDataGrid);
                    }
                }
                HandleSelection = true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar un eje", ex);
            }
        }

        private void AgregarTextBlock(Grid DynamicGrid, string Name, string Text, int Row, int Column)
        {
            try
            {
                var addTextBlock = new TextBlock();
                addTextBlock.Text = Text;
                addTextBlock.FontSize = 12;
                addTextBlock.Foreground = Brushes.Black;
                addTextBlock.FontWeight = FontWeights.Bold;
                addTextBlock.Margin = new Thickness(10, 35, 0, 0);

                Grid.SetRow(addTextBlock, Row);
                Grid.SetColumn(addTextBlock, Column);

                DynamicGrid.Children.Add(addTextBlock);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al crear etiqueta", ex);
            }
        }

        private void AgregarCheckBox(Grid DynamicGrid, string Text, int Row, int Column)
        {
            try
            {
                var addCheckBox = new CheckBox();
                addCheckBox.Content = Text;
                addCheckBox.IsChecked = true;
                addCheckBox.FontSize = 12;
                addCheckBox.FontWeight = FontWeights.Bold;
                addCheckBox.HorizontalAlignment = HorizontalAlignment.Right;
                addCheckBox.FlowDirection = FlowDirection.RightToLeft;
                addCheckBox.Margin = new Thickness(0, 30, 10, 0);
                addCheckBox.Checked += (s, e) =>
                {
                    try
                    {
                        ((CheckBox)s).Content = "Desmarcar Todo";
                        var Datagrid = ((Grid)((CheckBox)s).Parent).Children.Cast<UIElement>().Where(w => Grid.GetColumn(w) == (((int)((CheckBox)s).GetValue(Grid.ColumnProperty))) && w is DataGrid).Cast<DataGrid>().FirstOrDefault();
                        if (Datagrid.ItemsSource == null)
                            return;
                        var ItemsSource = Datagrid.ItemsSource.Cast<GridTratamientoActividad>().ToList();
                        foreach (var item in ItemsSource)
                            item.ELIGE = true;
                        Datagrid.ItemsSource = ItemsSource;
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al Marcar Todo", ex);
                    }
                };
                addCheckBox.Unchecked += (s, e) =>
                {
                    try
                    {
                        ((CheckBox)s).Content = "Marcar Todo";
                        var Datagrid = ((Grid)((CheckBox)s).Parent).Children.Cast<UIElement>().Where(w => Grid.GetColumn(w) == (((int)((CheckBox)s).GetValue(Grid.ColumnProperty))) && w is DataGrid).Cast<DataGrid>().FirstOrDefault();
                        if (Datagrid.ItemsSource == null)
                            return;
                        var ItemsSource = Datagrid.ItemsSource.Cast<GridTratamientoActividad>().ToList();
                        foreach (var item in ItemsSource)
                            item.ELIGE = false;
                        Datagrid.ItemsSource = ItemsSource;
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al Desmarcar Todo", ex);
                    }
                };

                Grid.SetRow(addCheckBox, Row);
                Grid.SetColumn(addCheckBox, Column);

                DynamicGrid.Children.Add(addCheckBox);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al crear checkbox", ex);
            }
        }

        private void AgregarButton(Grid DynamicGrid, string Content, int Row, int Column)
        {
            var addButton = new Button();
            addButton.Content = Content;
            addButton.FontSize = 12;
            addButton.FontWeight = FontWeights.Bold;
            addButton.Height = 30;
            addButton.Width = 60;
            addButton.Style = Application.Current.FindResource("AccentedSquareButtonStyle") as Style;
            addButton.HorizontalAlignment = HorizontalAlignment.Center;
            addButton.VerticalAlignment = VerticalAlignment.Center;
            addButton.Margin = new Thickness(0, 25, 0, 0);
            addButton.Click += (s, e) =>
            {
                ((Grid)((Button)s).Parent).Children.Cast<UIElement>().Where(w => Grid.GetColumn(w) == (((int)((Button)s).GetValue(Grid.ColumnProperty))) && w is CheckBox).Cast<CheckBox>().FirstOrDefault().IsChecked = false;

                var Datagrid = ((Grid)((Button)s).Parent).Children.Cast<UIElement>().Where(w => Grid.GetColumn(w) == (((int)((Button)s).GetValue(Grid.ColumnProperty))) && w is DataGrid).Cast<DataGrid>().FirstOrDefault();
                if (Datagrid.ItemsSource == null)
                    return;
                var ItemsSource = Datagrid.ItemsSource.Cast<GridTratamientoActividad>().ToList();
                foreach (var item in ItemsSource)
                    item.ELIGE = false;
                Datagrid.ItemsSource = ItemsSource;
            };
            Grid.SetRow(addButton, Row);
            Grid.SetColumn(addButton, Column);

            DynamicGrid.Children.Add(addButton);
        }
        #endregion
        /// <summary>
        /// metodo que obtiene el porcentaje de asistencia del interno
        /// </summary>
        /// <param name="item">objeto de tipo grupo participante</param>
        /// <param name="collection"> colleccion de grupo participante</param>
        /// <returns>cadena de texto con el resultado de la operacion %</returns>
        private string ObtenerPorcentajeAsistencia(GRUPO_PARTICIPANTE item, ICollection<GRUPO_PARTICIPANTE> collection)
        {
            try
            {
                var TotalHoras = 0.0;
                var AsistenciaHoras = 0.0;

                TotalHoras = item.ID_GRUPO.HasValue ? item.GRUPO.GRUPO_HORARIO.Where(w => w.ID_GRUPO == item.ID_GRUPO && w.ESTATUS == 1).Count() : 0;
                AsistenciaHoras = item.GRUPO_ASISTENCIA.Where(w => w.GRUPO_HORARIO.ESTATUS == 1 && (w.ESTATUS == 1 || w.ESTATUS == 3) && collection.Where(wh => wh.GRUPO != null && wh.GRUPO.GRUPO_HORARIO.Where(whe => whe.ESTATUS == 1).Any()).Contains(w.GRUPO_PARTICIPANTE) && w.ASISTENCIA == 1).Count();

                if (double.IsNaN((AsistenciaHoras / TotalHoras)))
                    return string.Empty;

                return string.Format("{0:P2}", (AsistenciaHoras / TotalHoras));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al calcular porcentaje de asistencia", ex);
                return string.Empty;
            }
        }
        #endregion

        #region Historial Tratamiento
        private void VerHistorialTratamiento()
        {
            try
            {
                if (SelectIngreso == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un ingreso");
                    return;
                }
                if (SelectIngreso.ID_INGRESO_DELITO > 1)
                {
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    var tratamiento = new VerHistorialTratamientoView();
                    short c = SelectIngreso.ID_INGRESO;
                    c--;
                    var ingreso = new cIngreso().Obtener(SelectIngreso.ID_CENTRO, SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO, c);
                    tratamiento.DataContext = new VerHistorialTratamientoViewModel(ingreso);
                    tratamiento.Closed += (s, e) =>
                    {
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    };
                    tratamiento.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar historial de tratamiento", ex);
            }
        }

        private void CargarHistorial()
        {
            try
            {
                if (SelectIngreso != null)
                {
                    if (SelectIngreso.ID_INGRESO > 0)
                    {
                        var ingreso = SelectIngreso;
                        short id = SelectIngreso.ID_INGRESO;
                        id--;
                        IngresoProgramas = new cIngreso().Obtener(SelectIngreso.ID_CENTRO, SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO,id);                        
                        IsHistorico = true;
                        LoadGrid(_DynamicGrid);
                        IsHistorico = false;
                        SelectIngreso = ingreso;
                        IngresoProgramas = null;
                        return;
                    }
                }
                #region comentado
                //if (AnteriorEMI != null)
                //{
                //    if (AnteriorEMI.EMI_INGRESO != null)
                //    {
                //        if (AnteriorEMI.EMI_INGRESO.INGRESO != null)
                //        {
                //            var ingreso = SelectIngreso;
                //            SelectIngreso = AnteriorEMI.EMI_INGRESO.INGRESO;
                //            IsHistorico = true;
                //            LoadGrid(_DynamicGrid);
                //            IsHistorico = false;
                //            SelectIngreso = ingreso;
                //            return;
                //        }
                //    }
                //}
                #endregion
                new Dialogos().ConfirmacionDialogo("Validación", "No existe información histórica");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar historial de tratamiento", ex);
            }
        }
        #endregion
    }
}
