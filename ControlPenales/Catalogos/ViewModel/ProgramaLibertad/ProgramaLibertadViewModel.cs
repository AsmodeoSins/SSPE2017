using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Linq;
using System.Collections.Generic;
namespace ControlPenales
{
    partial class ProgramaLibertadViewModel : ValidationViewModelBase
    {
        public ProgramaLibertadViewModel(){}

    
        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "buscar":
                    Populate();
                    break;
                case "menu_editar":
                    if (ProgramaLibertad.ID_PROGRAMA_LIBERTAD > 0)
                    {
                        HeaderAgregar = "Editar Programa Libertad";
                        #region visiblePantalla
                        EditarVisible = true;
                        NuevoVisible = false;
                        GuardarMenuEnabled = true;
                        AgregarMenuEnabled = false;
                        EliminarMenuEnabled = false;
                        EditarMenuEnabled = false;
                        CancelarMenuEnabled = true;
                        AyudaMenuEnabled = true;
                        SalirMenuEnabled = true;
                        ExportarMenuEnabled = false;
                        AgregarVisible = true;
                        #endregion
                        bandera_editar = true;
                        //FocusText = true;
                        /*****************************************/
                        
                        /*****************************************/
                    }
                    else
                    {
                        bandera_editar = false;
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un programa en libertad");
                    }
                    break;
                case "menu_agregar":
                    HeaderAgregar = "Agregar Programa Libertad";
                    #region visiblePantalla
                    EditarVisible = false;
                    NuevoVisible = true;
                    GuardarMenuEnabled = true;
                    AgregarMenuEnabled = false;
                    EliminarMenuEnabled = false;
                    EditarMenuEnabled = false;
                    CancelarMenuEnabled = true;
                    AyudaMenuEnabled = true;
                    SalirMenuEnabled = true;
                    ExportarMenuEnabled = false;
                    AgregarVisible = true;
                    #endregion
                    bandera_editar = false;
                    //FocusText = true;
                    /********************************/
                    ProgramaLibertad = new PROGRAMA_LIBERTAD();
                    LstActividadPrograma = new ObservableCollection<ACTIVIDAD_PROGRAMA>();
                    ActividadPrograma = new ACTIVIDAD_PROGRAMA();
                    ValidacionesProgramaLibertad();
                    /********************************/
                    break;
                case "menu_guardar":
                    if (!base.HasErrors)
                    {
                        if (LstActividadPrograma == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar actividades para este programa. ");
                            break;
                        }
                        else
                        {
                            if (LstActividadPrograma.Count == 0)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar actividades para este programa. ");
                                break;
                            }
                        }
                        if(Save())
                        {
                        #region visiblePantalla
                        EditarVisible = false;
                        NuevoVisible = false;
                        GuardarMenuEnabled = false;
                        AgregarMenuEnabled = true;
                        EliminarMenuEnabled = false;
                        EditarMenuEnabled = false;
                        CancelarMenuEnabled = false;
                        AyudaMenuEnabled = true;
                        SalirMenuEnabled = true;
                        ExportarMenuEnabled = true;
                        AgregarVisible = false;
                        #endregion
                        }
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación","Capture los campos requeridos "+base.Error);
                    //else
                        //FocusText = true;
                    break;
                case "menu_cancelar":
                    #region visiblePantalla
                    NuevoVisible = false;
                    GuardarMenuEnabled = false;
                    AgregarMenuEnabled = true;
                    EliminarMenuEnabled = false;
                    EditarMenuEnabled = false;
                    CancelarMenuEnabled = false;
                    AyudaMenuEnabled = true;
                    SalirMenuEnabled = true;
                    ExportarMenuEnabled = true;
                    AgregarVisible = false;
                    #endregion
                    /****************************************/
                    base.ClearRules();
                    ProgramaLibertad = new PROGRAMA_LIBERTAD();
                    LstActividadPrograma = new ObservableCollection<ACTIVIDAD_PROGRAMA>();
                    ActividadPrograma = new ACTIVIDAD_PROGRAMA();
                    Populate();
                    /****************************************/
                    break;
                case "menu_eliminar":
                    break;
                case "menu_exportar":
                    SeleccionIndice = -1;
                    Cambio = string.Empty;
                    break;
                case "menu_ayuda":
                    SeleccionIndice = -1;
                    Cambio = string.Empty;
                    break;
                case "menu_salir":
                    SeleccionIndice = -1;
                    Cambio = string.Empty;
                    PrincipalViewModel.SalirMenu();
                    break;

                case "addActividad":
                    ActividadPrograma = new ACTIVIDAD_PROGRAMA();
                    ValidacionesActividadLibertad();
                    PopUpsViewModels.ShowPopUp(this,PopUpsViewModels.TipoPopUp.AGREGAR_ACTIVIDAD_PROGRAMA);
                    break;
                case "editActividad":
                    if (!string.IsNullOrEmpty(ActividadPrograma.DESCR))
                    {
                        ValidacionesActividadLibertad();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_ACTIVIDAD_PROGRAMA);
                    }
                    else
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar una actividad");
                    }
                    break;
                    
                case "deletActividad":
                    if (!string.IsNullOrEmpty(ActividadPrograma.DESCR))
                        EliminarActividad();
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar una actividad");
                    break;
                case "agregar_actividad":
                    AgregarActividad();
                    break;
                case "cancelar_actividad":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_ACTIVIDAD_PROGRAMA);
                    ValidacionesProgramaLibertad();
                    break;
            }
        }

        private void ClickEnter(Object obj)
        {
            if (obj != null)
            {
                Busqueda = ((System.Windows.Controls.TextBox)(obj)).Text;
                Populate();
            }
        }


        #region Programa Libertad
        private void Populate() 
        {
            try
            {
                LstProgramaLibertad = new ObservableCollection<PROGRAMA_LIBERTAD>(new cProgramaLibertad().ObtenerTodos(Busqueda));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
            }
        }

        private bool Save()
        {
            try 
            { 
                if(ProgramaLibertad.ID_PROGRAMA_LIBERTAD == 0)
                {
                    if (LstActividadPrograma != null)
                    {
                        ProgramaLibertad.ACTIVIDAD_PROGRAMA = LstActividadPrograma;
                    }
                    if (new cProgramaLibertad().Insertar(ProgramaLibertad) > 0)
                    {
                        new Dialogos().ConfirmacionDialogo("Éxito","La informacion se guardo correctamente");
                        Populate();
                        return true;
                    }
                }
                else
                {
                    var List = new List<ACTIVIDAD_PROGRAMA>();
                    var obj = new PROGRAMA_LIBERTAD();
                    obj.ID_PROGRAMA_LIBERTAD = ProgramaLibertad.ID_PROGRAMA_LIBERTAD;
                    obj.DESCR = ProgramaLibertad.DESCR;
                    obj.OBJETIVO = ProgramaLibertad.OBJETIVO;
                    obj.ESTATUS = ProgramaLibertad.ESTATUS;

                    if (LstActividadPrograma != null)
                        List = new List<ACTIVIDAD_PROGRAMA>(LstActividadPrograma.Select(w => new ACTIVIDAD_PROGRAMA() { DESCR = w.DESCR, ESTATUS = w.ESTATUS })); 
                    if (new cProgramaLibertad().Actualizar(obj, List))
                    {
                        new Dialogos().ConfirmacionDialogo("Éxito", "La informacion se guardo correctamente");
                        Populate();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
            }
            return false;
        }
        
        #endregion

        #region Actividad Programa
        private void AgregarActividad() 
        {
            try
            {
                if (base.HasErrors)
                {
                    new Dialogos().ConfirmacionDialogo("Validación","Favor de capturar los campos requeridos. "+base.Error);
                    return;
                }
                if (ActividadPrograma.PROGRAMA_LIBERTAD == null)
                    LstActividadPrograma.Add(ActividadPrograma);
                LstActividadPrograma = new ObservableCollection<ACTIVIDAD_PROGRAMA>(LstActividadPrograma);
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_ACTIVIDAD_PROGRAMA);
                ValidacionesProgramaLibertad();
                ActividadPrograma = null;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
            }
        }

        private void EliminarActividad()
        {
            try
            {
                if (!string.IsNullOrEmpty(ActividadPrograma.DESCR))
                {
                    LstActividadPrograma.Remove(ActividadPrograma);
                    LstActividadPrograma = new ObservableCollection<ACTIVIDAD_PROGRAMA>(LstActividadPrograma);
                }
                else
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar una activida");
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        private async void Load(ProgramaLibertadView obj)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    ConfiguraPermisos();
                    EmptyVisible = false;
                    EditarVisible = false;
                    NuevoVisible = false;
                    AgregarVisible = false;
                    GuardarMenuEnabled = false;
                    EliminarMenuEnabled = false;
                    EditarMenuEnabled = false;
                    CancelarMenuEnabled = false;
                    AyudaMenuEnabled = true;
                    SalirMenuEnabled = true;
                    ExportarMenuEnabled = true;
                    
          



                    //EmptyVisible = false;
                    ////Listado 
                    //ListItems = new List<ETNIA>();
                    //CatalogoHeader = "Etnia";
                    //HeaderAgregar = "Agregar Etnia";
                    ////LLENAR 
                    //EditarVisible = false;
                    //NuevoVisible = false;
                    //AgregarVisible = false;
                    //GuardarMenuEnabled = false;
                    //EliminarMenuEnabled = false;
                    //EditarMenuEnabled = false;
                    //CancelarMenuEnabled = false;
                    //AyudaMenuEnabled = true;
                    //SalirMenuEnabled = true;
                    //ExportarMenuEnabled = true;
                    ///*MAXLENGTH*/
                    //MaxLength = 14;
                    //SeleccionIndice = -1;
                    ////Obtenemos las Etnias
                    //this.GetEtnias();
                    //this.setValidationRules();
                    Populate();
                    //StaticSourcesViewModel.SourceChanged = false;
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos iniciales.", ex);
            }
        }

        #region [PERMISOS]
        private void ConfiguraPermisos()
        {
            try
            {
                //var permisos = new cProcesoUsuario().Obtener(enumProcesos.CAT_ETNIAS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                //foreach (var p in permisos)
                //{
                //    if (p.INSERTAR == 1)
                        AgregarMenuEnabled = true;
                //    if (p.CONSULTAR == 1)
                //    {
                        BuscarHabilitado = true;
                        TextoHabilitado = true;
                //    }
                //    if (p.EDITAR == 1)
                        EditarEnabled = true;
                //}
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }
        #endregion
    }
}