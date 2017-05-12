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
    partial class UnidadReceptoraViewModel : ValidationViewModelBase
    {
        public UnidadReceptoraViewModel()
        {
            
        }

     
        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "buscar":
                    Populate();
                    break;
                case "menu_editar":
                    if (UnidadReceptora.ID_UNIDAD_RECEPTORA > 0)
                    {
                        HeaderAgregar = "Editar Unidad Receptora";
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
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar una unidad receptora");
                    }
                    break;
                case "menu_agregar":
                    HeaderAgregar = "Agregar Unidad Receptora";
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
                    ValidacionesUnidadReceptora();
                    UnidadReceptora = new UNIDAD_RECEPTORA();
                    /********************************/
                    break;
                case "menu_guardar":
                    //if (base.HasErrors)
                    if (true)
                    {
                        if (Save())
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
                        new Dialogos().ConfirmacionDialogo("Validación", "Capture los campos requeridos " + base.Error);
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
                    UnidadReceptora = new UNIDAD_RECEPTORA();
                    LstResponsables = new ObservableCollection<UNIDAD_RECEPTORA_RESPONSABLE>();
                    Responsable = new UNIDAD_RECEPTORA_RESPONSABLE();
                    Busqueda = string.Empty;
                    Populate();
                    /****************************************/
                    break;
                case "menu_eliminar":
                    break;
                case "menu_exportar":
                    break;
                case "menu_ayuda":
                    break;
                case "menu_salir":
                    PrincipalViewModel.SalirMenu();
                    break;
                case "addResponsable":
                    EditaResponsable = false;
                    Responsable = new UNIDAD_RECEPTORA_RESPONSABLE();
                    ValidacionResponsable();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.ALIAS);
                    break;
                case "editResponsable":
                    EditaResponsable = true;
                    if (!string.IsNullOrEmpty(Responsable.NOMBRE))
                    {
                        ValidacionResponsable();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.ALIAS);
                    }
                    else
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un responsable");
                    }
                    break;

                case "deletResponsable":
                    if (!string.IsNullOrEmpty(Responsable.NOMBRE))
                        EliminarResponsable();
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar una actividad");
                    break;
                case "guardar_alias":
                    AgregarResponsable();
                    break;
                case "cancelar_alias":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.ALIAS);
                    ValidacionesUnidadReceptora();
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
      
        private async void Load(UnidadReceptoraView obj)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    ConfiguraPermisos();
                    EmptyVisible = false;
                    CatalogoHeader = "UNIDAD RECEPTORA";
                    HeaderAgregar = "Agregar Unidad Receptora";
                    //LLENAR 
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
                    Populate();
//                    StaticSourcesViewModel.SourceChanged = false;
                    LstEstado = new ObservableCollection<ENTIDAD>(new cEntidad().ObtenerTodosPais(pais));
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstEstado.Insert(0, new ENTIDAD() { ID_ENTIDAD = -1, DESCR = "SELECCIONE" });
                        EntidadUR = -1;
                        LstMunicipio = new ObservableCollection<MUNICIPIO>();
                        LstMunicipio.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                        MunicipioUR = -1;
                        LstColonia = new ObservableCollection<COLONIA>();
                        LstColonia.Insert(0, new COLONIA() { ID_COLONIA = -1, DESCR = "SELECCIONE" });
                        ColoniaUR = -1;
                    }));
                    var test = "";
                });
                
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos iniciales.", ex);
            }
        }

        #region Unidad Receptora
        private void Populate() 
        {
            try
            {
                LstUnidadReceptora = new ObservableCollection<UNIDAD_RECEPTORA>(new cUnidadReceptora().ObtenerTodos(Busqueda,string.Empty));
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
                if (UnidadReceptora.ID_UNIDAD_RECEPTORA == 0)
                {
                    if (new cUnidadReceptora().Insertar(UnidadReceptora, LstResponsables != null ? LstResponsables.ToList() : null) > 0)
                    {
                        new Dialogos().ConfirmacionDialogo("Éxito", "La informacion se guardo correctamente");
                        Populate();
                        return true;
                    }
                }
                else
                {
                    var List = new List<UNIDAD_RECEPTORA_RESPONSABLE>();
                    var obj = new UNIDAD_RECEPTORA();
                    obj.ID_UNIDAD_RECEPTORA = UnidadReceptora.ID_UNIDAD_RECEPTORA;
                    obj.NOMBRE = NombreUR;// UnidadReceptora.NOMBRE;
                    obj.DESCRIPCION = DescripcionUR;// UnidadReceptora.DESCRIPCION;
                    obj.ID_ENTIDAD = EntidadUR;// UnidadReceptora.ID_ENTIDAD;
                    obj.ID_MUNICIPIO = MunicipioUR;// UnidadReceptora.ID_MUNICIPIO;
                    obj.ID_COLONIA = ColoniaUR;// UnidadReceptora.ID_COLONIA;
                    obj.CALLE_DIRECCION = CalleUR;// UnidadReceptora.CALLE_DIRECCION;
                    obj.NUM_INT_DIRECCION = NoInteriorUR;// UnidadReceptora.NUM_INT_DIRECCION;
                    obj.NUM_EXT_DIRECCION = NoExteriorUR;// UnidadReceptora.NUM_EXT_DIRECCION;
                    obj.CP_DIRECCION = CPUR;// UnidadReceptora.CP_DIRECCION;
                    var tel = TelefonoUR.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ","");
                    obj.TELEFONO = !string.IsNullOrEmpty(tel) ? (long?)long.Parse(tel) : null;// UnidadReceptora.TELEFONO;
                    obj.ESTATUS = EstatusUR;// UnidadReceptora.ESTATUS;


                    /*
                      NombreUR = value.NOMBRE;
                    DescripcionUR = value.DESCRIPCION;
                    EntidadUR = value.ID_ENTIDAD;
                    MunicipioUR = value.ID_MUNICIPIO;
                    ColoniaUR = value.ID_COLONIA;
                    CalleUR = value.CALLE_DIRECCION;
                    NoInteriorUR = value.NUM_INT_DIRECCION;
                    NoExteriorUR = value.NUM_EXT_DIRECCION;
                    CPUR = value.CP_DIRECCION;
                    TelefonoUR = value.TELEFONO != null ? value.TELEFONO.ToString() : string.Empty;
                    EstatusUR = value.ESTATUS;
                     */

                    if (LstResponsables != null)
                        List = new List<UNIDAD_RECEPTORA_RESPONSABLE>(LstResponsables.Select(w => new UNIDAD_RECEPTORA_RESPONSABLE() { NOMBRE = w.NOMBRE, PATERNO = w.PATERNO, MATERNO = w.MATERNO }));
                    if (new cUnidadReceptora().Actualizar(obj, List))
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
        private void AgregarResponsable()
        {
            try
            {
                if (base.HasErrors)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar los campos requeridos. " + base.Error);
                    return;
                }

                if (!EditaResponsable)
                    LstResponsables.Add(Responsable);
                LstResponsables = new ObservableCollection<UNIDAD_RECEPTORA_RESPONSABLE>(LstResponsables);
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.ALIAS);
                ValidacionesUnidadReceptora();
                Responsable = null;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
            }
        }

        private void EliminarResponsable()
        {
            try
            {
                if (!string.IsNullOrEmpty(Responsable.NOMBRE))
                {
                    LstResponsables.Remove(Responsable);
                    LstResponsables = new ObservableCollection<UNIDAD_RECEPTORA_RESPONSABLE>(LstResponsables);
                }
                else
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un responsable");
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region [PERMISOS]
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CAT_ETNIAS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                foreach (var p in permisos)
                {
                    if (p.INSERTAR == 1)
                        AgregarMenuEnabled = true;
                    if (p.CONSULTAR == 1)
                    {
                        BuscarHabilitado = true;
                        TextoHabilitado = true;
                    }
                    if (p.EDITAR == 1)
                        EditarEnabled = true;
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