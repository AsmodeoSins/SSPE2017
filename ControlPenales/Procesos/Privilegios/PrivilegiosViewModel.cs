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
using ControlPenales.BiometricoServiceReference;

namespace ControlPenales
{
    partial class PrivilegiosViewModel : ValidationViewModelBase
    {
        #region constructor
        public PrivilegiosViewModel() { }
        #endregion

        #region metodos
        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "salir_menu":
                    PrincipalViewModel.SalirMenu();
                    break;
                case "buscar_proceso":
                    if (!pConsultar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    
                    ObtenerTodoProceso();
                    break;
                case "add_proceso":
                    if (!pInsertar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    ProcesoTitulo = "Agregar Proceso";
                    SelectedProceso = null;
                    ValidacionProceso();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_PROCESO);
                    break;
                case "edit_proceso":
                    if (!pEditar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    ProcesoTitulo = "Editar Proceso";
                    if (SelectedProceso != null)
                    {
                        ObtenerProceso();
                        ValidacionProceso();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_PROCESO);
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un proceso");
                    break;
                case "del_proceso":
                    if (SelectedProceso != null)
                    {
                        var respuesta = await new Dialogos().ConfirmarSalida("Validación", "¿Confirma la eliminación de este proceso?");
                        if (respuesta == 1)
                        {
                            QuitarProceso();
                        }
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un proceso");
                    break;
                case "agregar_proceso":
                    if (!pInsertar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    if (!base.HasErrors)
                    {
                        AgregarProceso();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_PROCESO);
                        base.ClearRules();
                    }
                    break;
                case "cancelar_proceso":
                    LimpiarProcesos();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_PROCESO);
                    base.ClearRules();
                    break;
                case "add_proceso_usuario":
                    if (!pInsertar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    BuscarProceso();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_PROCESO_USUARIO);
                    break;
                case "del_proceso_usuario":
                    break;
                case "guardar_proceso_usuario":
                    if (!pEditar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    GuardarUsuarioProceso();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_PROCESO_USUARIO);
                    break;
                case "cancelar_proceso_usuario":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_PROCESO_USUARIO);
                    break;
                case "buscar_rol":
                    if (!pConsultar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    ObtenerTodoRol();
                    break;
                case "add_rol":
                    if (!pInsertar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    RolTitulo = "Agregar Rol";
                    LimpiarRoles();
                    ValidacionRol();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_ROL);
                    break;
                case "edit_rol":
                    if (!pEditar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    RolTitulo = "Editar Rol";
                    if (SelectedRol != null)
                    {
                        ObtenerRol();
                        ValidacionRol();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_ROL);
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un rol");
                    break;
                case "del_rol":
                    if (SelectedRol != null)
                    {
                        var respuesta = await new Dialogos().ConfirmarSalida("Validación", "¿Confirma la eliminación de este rol?");
                        if (respuesta == 1)
                        {
                            QuitarRol();
                        }
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un rol");
                    break;
                case "agregar_rol":
                    if (!pInsertar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    if (!base.HasErrors)
                    {
                        AgregarRol();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_ROL);
                        base.ClearRules();
                    }
                    break;
                case "cancelar_rol":
                    LimpiarRoles();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_ROL);
                    base.ClearRules();
                    break;
                case "buscar_usuario":
                    if (!pConsultar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    ObtenerTodoUsuarios();
                    break;
                case "add_usuario":
                    if (!pInsertar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    LimpiarBusqueda();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                    /*TituloPop = "Agregar Usuario";
                    ValidacionUsuario();
                    PopUpsViewModels.ShowPopUp(this,PopUpsViewModels.TipoPopUp.AGREGAR_USUARIO);*/
                    break;
                case "edit_usuario":
                    if (!pEditar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    TituloPop = "Editar Usuario";
                    ValidacionUsuario();
                    ObtenerUsuario();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_USUARIO);
                    break;
                case "guardar_usuario":
                    if (!pInsertar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    if (!base.HasErrors)
                    {
                        if (GuardarUsuario())
                        {
                            new Dialogos().ConfirmacionDialogo("Éxito", "La información se guardo correctamente");
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_USUARIO);
                            base.ClearRules();
                        }
                        //else
                        //    new Dialogos().ConfirmacionDialogo("Error", "No se guardo la información");
                    }
                    break;
                case "cancelar_usuario":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_USUARIO);
                    LimpiarUsuario();
                    base.ClearRules();
                    break;
                case "buscar_visitante":
                    if (!pConsultar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    BuscarEmpleado();
                    break;
                case "buscar_busqueda_visitante":
                    if (!pConsultar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    BuscarEmpleado();
                    break;
                case "nueva_busqueda_visitante":
                    LimpiarBusqueda();
                    break;
                case "seleccionar_buscar_persona":
                    if (SelectPersona != null)
                    {
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                        SelectedUsuario = null;
                        TituloPop = "Agregar Usuario";
                        ValidacionUsuario();
                        ObtenerUsuario();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_USUARIO);
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un empleado");
                    break;
                case "cancelar_buscar_persona":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                    LimpiarBusqueda();
                    break;
                case "limpiar_menu":
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new PrivilegiosView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new PrivilegiosViewModel();
                    break;
            }
        }

        private async void OnLoad(PrivilegiosView obj = null)
        {
            try
            {
                ConfiguraPermisos();
                if (pConsultar)
                {
                    ObtenerTodoProceso();
                    ObtenerTodoRol();
                    ObtenerTodoUsuarios();
                }
                else
                {
                    ProcesosVisible = LstProcesos != null ? LstProcesos.Count > 0 ? Visibility.Collapsed : Visibility.Visible : Visibility.Visible;
                    PermisoRolVisible = LstPermisosRol != null ? LstPermisosRol.Count > 0 ? Visibility.Collapsed : Visibility.Visible : Visibility.Visible;
                    UsuariosVisible = LstUsuario != null ? LstUsuario.Count > 0 ? Visibility.Collapsed : Visibility.Visible : Visibility.Visible;
                }
                ObtenerCentro();
                StaticSourcesViewModel.SourceChanged = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar pantalla de privilegios", ex);
            }
        }

        private async void ClickEnter(Object obj)
        {
            try
            {
                if (!pConsultar)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                    return;
                }
                if (obj != null)
                {
                    var textbox = (TextBox)obj;
                    switch (textbox.Name)
                    {
                        case "Proceso":
                            BPDescripcion = textbox.Text;
                            ObtenerTodoProceso();
                            break;
                        case "Rol":
                            BRDescripcion = textbox.Text;
                            ObtenerTodoRol();
                            break;
                        case "Usuario":
                            BUsuario = textbox.Text;
                            ObtenerTodoUsuarios();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ingresar búsqueda", ex);
            }
        }

        private async void OnCheck(Object obj)
        {
            try
            {
                var pr = (cPermisosRol)obj;
                if (pr.Seleccion)
                {
                    var res = await StaticSourcesViewModel.OperacionesAsync<bool>("Actualizando...", () =>
                    {
                        try
                        {
                            return ActualizarProcesoRol();
                        }
                        catch (Exception ex)
                        {
                            StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
                            return false;
                        }
                    });

                    //if (ActualizarProcesoRol())
                    if (res)
                    {
                        Mensaje(true, "Se actualizo el proceso del rol");
                    }
                    else
                        Mensaje(false, "No se actualizo el proceso al rol");
                }
                else
                {
                    var res = await StaticSourcesViewModel.OperacionesAsync<bool>("Actualizando...", () =>
                    {
                        try
                        {
                            return EliminarProcesoRol();
                        }
                        catch (Exception ex)
                        {
                            StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
                            return false;
                        }
                    });

                    //if (EliminarProcesoRol())
                    if(res)
                    {
                        pr.ProcesoRol.CONSULTAR = pr.ProcesoRol.EDITAR = pr.ProcesoRol.IMPRIMIR = pr.ProcesoRol.INSERTAR = 0;
                        Mensaje(true, "Se actualizo el proceso del rol");
                    }
                    else
                        Mensaje(false, "No se actualizo el proceso al rol");
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ingresar búsqueda", ex);
            }
        }

        private async void OnCheckPermisos(Object obj)
        {
            try
            {
                //if (SelectedUsuario == null)
                //    return;
                var pr = (cPermisosRol)obj;
                if (pr.Seleccion)
                {
                    var res = await StaticSourcesViewModel.OperacionesAsync<bool>("Actualizando...", () =>
                    {
                        try
                        {
                            return ActualizarProcesoRol();
                        }
                        catch (Exception ex)
                        {
                            StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
                            return false;
                        }
                    });

                    //if (ActualizarProcesoRol())
                    if(res)
                    {
                        SelectedUsuario = null;
                        Mensaje(true, "Se actualizo el proceso del rol");
                    }
                    else
                        Mensaje(false, "No se actualizo el proceso al rol");
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al actualizar roles", ex);
            }
        }

        private async void OnCheckUR(Object obj)
        {
            try
            {
                if (SelectedUsuarioRol == null)
                    return;
                var ur = (cUsrRol)obj;
                if (ur.Seleccion)
                {
                    var res = await StaticSourcesViewModel.OperacionesAsync<bool>("Actualizando...", () =>
                    {
                        try
                        {
                            return ActualizarUsuarioRol();
                        }
                        catch (Exception ex)
                        {
                            StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
                            return false;
                        }
                    });
                    //if (ActualizarUsuarioRol())
                    if(res)
                    {
                        SelectedUsuarioRol = null;
                        Mensaje(true, "Se actualizo el rol del usuario");
                    }
                    else
                        Mensaje(false, "No se actualizo el rol del usuario");
                }
                else
                {
                    
                    var res = await StaticSourcesViewModel.OperacionesAsync<bool>("Actualizando...", () =>
                    {
                        try
                        {
                            return EliminarUsuarioRol();
                        }
                        catch (Exception ex)
                        {
                            StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
                            return false;
                        }
                    });
                    
                    //if (EliminarUsuarioRol())
                    if(res)
                    {
                        SelectedUsuarioRol = null;
                        Mensaje(true, "Se actualizo el rol del usuario");
                    }
                    else
                        Mensaje(false, "No se actualizo el rol del usuario");
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al actualizar roles", ex);
            }
        }

        private async void OnCheckPR(Object obj)
        {
            try
            {
                if (SelectedProcesoUsuario == null)
                    return;
                var ur = (cProcesoUsr)obj;
                if (ur.Seleccion)
                {
                    var res = await StaticSourcesViewModel.OperacionesAsync<bool>("Actualizando...", () =>
                    {
                        try
                        {
                            return ActualizarUsuarioProceso();
                        }
                        catch (Exception ex)
                        {
                            StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
                            return false;
                        }
                    });
             
                    //if (ActualizarUsuarioProceso())
                    if(res)
                    {
                        SelectedProcesoUsuario = null;
                        Mensaje(true, "Se actualizo el proceso del rol");
                    }
                    else
                        Mensaje(false, "No se actualizo el proceso al rol");
                }
                else
                {
                    var res = await StaticSourcesViewModel.OperacionesAsync<bool>("Actualizando...", () =>
                    {
                        try
                        {
                            return EliminarUsuarioProceso();
                        }
                        catch (Exception ex)
                        {
                            StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
                            return false;
                        }
                    });
                    //if (EliminarUsuarioProceso())
                    if(res)
                    {
                        SelectedProcesoUsuario = null;
                        Mensaje(true, "Se actualizo el proceso del rol");
                    }
                    else
                        Mensaje(false, "No se actualizo el proceso al rol");
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar roles", ex);
            }
        }

        private async void OnCheckPRP(Object obj)
        {
            try
            {
                if (SelectedProcesoUsuario == null)
                    return;
                var ur = (cProcesoUsr)obj;
                if (ur.Seleccion)
                {
                    if (SelectedProcesoUsuario != null)
                    {
                        var res = await StaticSourcesViewModel.OperacionesAsync<bool>("Actualizando...", () =>
                        {
                            try
                            {
                                return ActualizarUsuarioProceso();
                            }
                            catch (Exception ex)
                            {
                                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
                                return false;
                            }
                        });
                        //if (ActualizarUsuarioProceso())
                        if(res)
                        {
                            SelectedProcesoUsuario = null;
                            Mensaje(true, "Se actualizo el proceso del rol");
                        }
                        else
                            Mensaje(false, "No se actualizo el proceso al rol");
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar roles", ex);
            }
        }

        private async void Password(Object obj)
        {
            try
            {
                if (obj != null)
                {
                    UPassControl = ((PasswordBox)(obj));
                    UPassword = ((PasswordBox)(obj)).Password;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al bindear password", ex);
            }
        }

        private async void PasswordRepetir(Object obj)
        {
            try
            {
                if (obj != null)
                {
                    UPassRepeatControl = ((PasswordBox)(obj));
                    UPasswordR = ((PasswordBox)(obj)).Password;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al bindear repetir password", ex);
            }
        }

        private void CargarListas()
        {
            try
            {

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar listados", ex);
            }
        }

        private void EnterPersonas(Object obj)
        {
            try
            {
                if (!pConsultar)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                    return;
                }
                BuscarEmpleado();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la búsqueda.", ex);
            }
        }

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
        #endregion

        #region Procesos
        private void LimpiarProcesos()
        {
            try
            {
                PDescripcion = PVentana = string.Empty;
                SelectedProceso = null;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar procesos", ex);
            }
        }

        private void ObtenerTodoProceso()
        {
            try
            {
                LstProcesos = new ObservableCollection<PROCESO>(new cProceso().ObtenerTodos(BPDescripcion));
                ProcesosVisible = LstProcesos.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al consultar procesos", ex);
            }
        }

        private void ObtenerProceso()
        {
            try
            {
                if (SelectedProceso != null)
                {
                    PDescripcion = SelectedProceso.DESCR;
                }
                else
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un proceso");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al consultar procesos", ex);
            }
        }

        private void AgregarProceso()
        {
            try
            {
                if (LstProcesos == null)
                    LstProcesos = new ObservableCollection<PROCESO>();
                var obj = new PROCESO();
                if (SelectedProceso == null)
                {
                    obj.DESCR = PDescripcion;
                    obj.VENTANA = PVentana;
                    obj.ID_PROCESO = new cProceso().Insertar(obj);
                    if (obj.ID_PROCESO > 0)
                    {
                        LstProcesos.Add(obj);
                        new Dialogos().ConfirmacionDialogo("Éxito", "La información se guardo correctamente");
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Error", "No se guardo la información");
                }
                else
                {
                    SelectedProceso.DESCR = PDescripcion;
                    SelectedProceso.VENTANA = PVentana;
                    obj.ID_PROCESO = SelectedProceso.ID_PROCESO;
                    obj.DESCR = SelectedProceso.DESCR;
                    if (new cProceso().Actualizar(obj))
                    {
                        LstProcesos = new ObservableCollection<PROCESO>(LstProcesos);
                        new Dialogos().ConfirmacionDialogo("Éxito", "La información se guardo correctamente");
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Error", "No se guardo la información");
                }
                ProcesosVisible = LstProcesos.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                LimpiarProcesos();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar procesos", ex);
            }
        }

        private bool QuitarProceso()
        {
            try
            {
                if (LstProcesos != null)
                {
                    if (SelectedProceso != null)
                    {
                        var obj = new PROCESO();
                        obj.ID_PROCESO = SelectedProceso.ID_PROCESO;
                        if (new cProceso().Eliminar(obj))
                        {
                            if (LstProcesos.Remove(SelectedProceso))
                            {
                                ProcesosVisible = LstProcesos.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar procesos", ex);
            }
            return false;
        }

        //buscar
        private void BuscarProceso()
        {
            LstProcesoBuscar = new ObservableCollection<cProc>();
            var procesos = new cProceso().ObtenerTodos(LstUsuarioRol.Where(w => w.Seleccion).Select(w => w.UsuarioRol).ToList());
            foreach (var p in procesos)
            {
                LstProcesoBuscar.Add(new cProc() { Seleccion = false, Proceso = p });
            }
        }
        #endregion

        #region Roles
        private void LimpiarRoles()
        {
            try
            {
                RDescripcion = string.Empty;
                SelectedRol = null;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar rol", ex);
            }
        }

        private void ObtenerTodoRol()
        {
            try
            {
                LstRoles = new ObservableCollection<SISTEMA_ROL>(new cSistemaRol().ObtenerTodos(BRDescripcion));
                RolesVisible = LstRoles.Count > 0 ? Visibility.Collapsed : Visibility.Visible;

                if (LstPermisosRol != null)
                    PermisoRolVisible = LstPermisosRol.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                else
                    PermisoRolVisible = Visibility.Visible;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al consultar rol", ex);
            }
        }

        private void ObtenerRol()
        {
            try
            {
                if (SelectedRol != null)
                {
                    RDescripcion = SelectedRol.DESCR;
                }
                else
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un rol");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al consultar rol", ex);
            }
        }

        private void AgregarRol()
        {
            try
            {
                if (LstRoles == null)
                    LstRoles = new ObservableCollection<SISTEMA_ROL>();
                var obj = new SISTEMA_ROL();
                if (SelectedRol == null)
                {
                    obj.DESCR = RDescripcion;
                    obj.ID_ROL = new cSistemaRol().Insertar(obj);
                    if (obj.ID_ROL > 0)
                    {
                        LstRoles.Add(obj);
                        if (LstPermisosRol != null)
                        {
                            PermisoRolVisible = LstPermisosRol.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                        }
                        else
                            PermisoRolVisible = Visibility.Visible;

                        new Dialogos().ConfirmacionDialogo("Éxito", "La información se guardo correctamente");
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Error", "No se guardo la información");
                }
                else
                {
                    SelectedRol.DESCR = RDescripcion;
                    obj.ID_ROL = SelectedRol.ID_ROL;
                    obj.DESCR = SelectedRol.DESCR;
                    if (new cSistemaRol().Actualizar(obj))
                    {
                        LstRoles = new ObservableCollection<SISTEMA_ROL>(LstRoles);
                        new Dialogos().ConfirmacionDialogo("Éxito", "La información se guardo correctamente");
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Error", "No se guardo la información");
                }
                RolesVisible = LstRoles.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                LimpiarRoles();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar rol", ex);
            }
        }

        private bool QuitarRol()
        {
            try
            {
                if (LstRoles != null)
                {
                    if (SelectedRol != null)
                    {
                        var rol = new cSistemaRol().Obtener(SelectedRol.ID_ROL);
                        if (rol.PROCESO_ROL != null)
                        {
                            if (rol.PROCESO_ROL.Count > 0)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "No se puede eliminar el rol, tiene información relacionada.");
                                return false;
                            }
                        }
                        if (rol.USUARIO_ROL != null)
                        {
                            if (rol.USUARIO_ROL.Count > 0)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "No se puede eliminar el rol, tiene información relacionada.");
                                return false;
                            }
                        }
                        var obj = new SISTEMA_ROL();
                        obj.ID_ROL = SelectedRol.ID_ROL;
                        if (new cSistemaRol().Eliminar(obj))
                        {
                            if (LstRoles.Remove(SelectedRol))
                            {
                                RolesVisible = LstRoles.Count > 0 ? Visibility.Collapsed : Visibility.Visible;

                                if (LstPermisosRol != null)
                                {
                                    PermisoRolVisible = LstPermisosRol.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                                }
                                else
                                    PermisoRolVisible = Visibility.Visible;

                                new Dialogos().ConfirmacionDialogo("Éxito", "El rol ha sido eliminado.");
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar rol", ex);
            }
            return false;
        }
        #endregion

        #region Proceso Rol
        private void ObtenerTodoProcesoRol()
        {
            try
            {
                var procesos = new cProceso().ObtenerTodos();
                LstPermisosRol = new ObservableCollection<cPermisosRol>();
                if (SelectedRol != null)
                {
                    List<PROCESO_ROL> pr = new List<PROCESO_ROL>(new cProcesoRol().ObtenerTodos(SelectedRol.ID_ROL));
                    if (procesos != null)
                    {
                        foreach (var p in procesos)
                        {
                            var obj = new PROCESO_ROL();
                            var x = pr.Where(w => w.ID_PROCESO == p.ID_PROCESO).FirstOrDefault();
                            if (x != null)
                            {
                                obj.ID_PROCESO = x.ID_PROCESO;
                                obj.ID_ROL = x.ID_ROL;
                                obj.INSERTAR = x.INSERTAR;
                                obj.EDITAR = x.EDITAR;
                                obj.CONSULTAR = x.CONSULTAR;
                                obj.IMPRIMIR = x.IMPRIMIR;
                                obj.PROCESO = x.PROCESO;
                                LstPermisosRol.Add(new cPermisosRol() { Seleccion = true, ProcesoRol = obj, BD = true });
                            }
                            else
                            {
                                obj.ID_PROCESO = p.ID_PROCESO;
                                obj.ID_ROL = 0;
                                obj.INSERTAR = 0;
                                obj.EDITAR = 0;
                                obj.CONSULTAR = 0;
                                obj.IMPRIMIR = 0;
                                obj.PROCESO = p;
                                LstPermisosRol.Add(new cPermisosRol() { Seleccion = false, ProcesoRol = obj, BD = false });
                            }
                        }
                    }
                }
                PermisoRolVisible = LstPermisosRol.Count > 0 ? Visibility.Collapsed : Visibility.Hidden;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener los procesos del rol", ex);
            }
        }

        private bool ActualizarProcesoRol()
        {
            try
            {
                if (SelectedPermisoRol != null)
                {
                    var obj = new PROCESO_ROL();
                    obj.ID_ROL = SelectedRol.ID_ROL;
                    obj.ID_PROCESO = SelectedPermisoRol.ProcesoRol.ID_PROCESO;
                    obj.INSERTAR = SelectedPermisoRol.ProcesoRol.INSERTAR;
                    obj.EDITAR = SelectedPermisoRol.ProcesoRol.EDITAR;
                    obj.CONSULTAR = SelectedPermisoRol.ProcesoRol.CONSULTAR;
                    obj.IMPRIMIR = SelectedPermisoRol.ProcesoRol.IMPRIMIR;
                    if (!SelectedPermisoRol.BD)
                    {
                        if (new cProcesoRol().Insertar(obj))
                        {
                            SelectedPermisoRol.BD = true;
                            //LstPermisosRol = new ObservableCollection<cPermisosRol>(LstPermisosRol);
                            return true;
                        }
                    }
                    else
                    {
                        if (new cProcesoRol().Actualizar(obj))
                        {
                            //LstPermisosRol = new ObservableCollection<cPermisosRol>(LstPermisosRol);
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al actualizar los procesos del rol", ex);
            }
            return false;
        }

        private bool EliminarProcesoRol()
        {
            try
            {
                if (SelectedPermisoRol != null)
                {
                    var obj = new PROCESO_ROL();
                    obj.ID_ROL = SelectedRol.ID_ROL;
                    obj.ID_PROCESO = SelectedPermisoRol.ProcesoRol.ID_PROCESO;
                    if (new cProcesoRol().Eliminar(obj))
                    {
                        SelectedPermisoRol.BD = false;
                        LstPermisosRol = new ObservableCollection<cPermisosRol>(LstPermisosRol);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar los procesos del rol", ex);
            }
            return false;
        }
        #endregion

        #region Usuarios
        private void ObtenerTodoUsuarios()
        {
            try
            {
                LstUsuario = new ObservableCollection<USUARIO>(new cUsuario().ObtenerTodos(BUsuario));
                UsuariosVisible = LstUsuario.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                LstUsuarioRol = new ObservableCollection<cUsrRol>();
                UsuarioRolVisible = Visibility.Visible;
                LstProcesoUsuario = new ObservableCollection<cProcesoUsr>();
                UsuarioProcesoVisible = Visibility.Visible;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al consultar usuarios", ex);
            }
        }

        private void ObtenerUsuario()
        {
            try
            {
                LimpiarUsuario();
                if (SelectedUsuario != null)
                {
                    ULoginEnabled = false;
                    UNoEmpleado = SelectedUsuario.ID_PERSONA;
                    UPaterno = SelectedUsuario.EMPLEADO != null ? SelectedUsuario.EMPLEADO.PERSONA != null ? SelectedUsuario.EMPLEADO.PERSONA.PATERNO : string.Empty : string.Empty;
                    UMaterno = SelectedUsuario.EMPLEADO != null ? SelectedUsuario.EMPLEADO.PERSONA != null ? SelectedUsuario.EMPLEADO.PERSONA.MATERNO : string.Empty : string.Empty;
                    UNombre = SelectedUsuario.EMPLEADO != null ? SelectedUsuario.EMPLEADO.PERSONA != null ? SelectedUsuario.EMPLEADO.PERSONA.NOMBRE : string.Empty : string.Empty;
                    ULogin = SelectedUsuario.ID_USUARIO;
                    UEstatus = SelectedUsuario.ESTATUS == "S" ? true : false;

                    if (SelectedUsuario.EMPLEADO == null)
                    {
                        UImagen = new Imagenes().getImagenPerson();
                        return;
                    };

                    if (SelectedUsuario.EMPLEADO.PERSONA == null)
                    {
                        UImagen = new Imagenes().getImagenPerson();
                        return;
                    };

                    if (SelectedUsuario.EMPLEADO.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                        UImagen = SelectedUsuario.EMPLEADO.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                    else
                        UImagen = new Imagenes().getImagenPerson();
                }
                else
                {
                    ULoginEnabled = true;
                    if (SelectPersona != null)
                    {
                        UNoEmpleado = SelectPersona.ID_PERSONA;
                        UPaterno = SelectPersona.PATERNO;
                        UMaterno = SelectPersona.MATERNO;
                        UNombre = SelectPersona.NOMBRE;
                        if (SelectPersona.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                            UImagen = SelectPersona.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                        else
                            UImagen = new Imagenes().getImagenPerson();
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un usuario");
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al consultar usuarios", ex);
            }
        }
        
        private void LimpiarUsuario()
        {
            try
            {
                UNoEmpleado = null;
                UPaterno = UMaterno = UNombre = ULogin = UPassword = UPasswordR = string.Empty;
                UEstatus = true;
                UImagen = new Imagenes().getImagenPerson();
                if (UPassControl != null)
                    UPassControl.Clear();
                if (UPassRepeatControl != null)
                    UPassRepeatControl.Clear();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar.", ex);
            }
        }

        private bool GuardarUsuario()
        {
            try
            {
                var obj = new USUARIO();
                obj.PASSWORD = cEncriptacion.HarshedText(UPassword.ToUpper());
                obj.ESTATUS = UEstatus ? "S" : "N";
                if (SelectedUsuario != null)
                {
                    obj.ID_USUARIO = SelectedUsuario.ID_USUARIO;
                    obj.ID_PERSONA = SelectedUsuario.ID_PERSONA;
                    //Si no pone una contraseña no la actualiza
                    if (string.IsNullOrEmpty(UPassword))
                    {
                        obj.PASSWORD = SelectedUsuario.PASSWORD;
                        obj.VENCE_PASS = SelectedUsuario.VENCE_PASS;
                    }
                    else
                    {
                        obj.VENCE_PASS = Fechas.GetFechaDateServer.AddDays(Parametro.DIAS_PASSWORD);
                        if (!ValidarPassword())
                            return false;
                    }
                    if (new cUsuario().Actualizar(obj, PassBD))
                    {
                        SelectedUsuario.ESTATUS = obj.ESTATUS;
                        return true;
                    }
                }
                else
                {
                    if (SelectPersona != null)
                    {
                        if (ValidarPassword())
                        {
                            obj.ID_USUARIO = ULogin;
                            obj.ID_PERSONA = SelectPersona.ID_PERSONA;
                            obj.VENCE_PASS = Fechas.GetFechaDateServer.AddDays(Parametro.DIAS_PASSWORD);
                            if (new cUsuario().Obtener(obj.ID_USUARIO) != null)
                            {
                                new Dialogos().ConfirmacionDialogo("Validacion!", "El login del usuario ya existe, favor de cambiarlo.");
                                return false;
                            }
                            else
                            {
                                if (new cUsuario().Insertar(obj, PassBD))
                                    return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar.", ex);
            }
            return false;
        }
        #endregion

        #region Usuario Rol
        private void ObtenerTodoUsuarioRol()
        {
            try
            {
                var roles = new cSistemaRol().ObtenerTodos();
                LstUsuarioRol = new ObservableCollection<cUsrRol>();
                if (SelectedUsuario != null)
                {
                    List<USUARIO_ROL> ur = new List<USUARIO_ROL>(new cUsuarioRol().ObtenerTodos(SelectedUsuario.ID_USUARIO.Trim(),SelectedCentro));
                    if (roles != null)
                    {
                        foreach (var r in roles)
                        {
                            var obj = new USUARIO_ROL();
                            var x = ur.Where(w => w.ID_ROL == r.ID_ROL).FirstOrDefault();
                            if (x != null)
                            {
                                obj.ID_ROL = x.ID_ROL;
                                obj.ID_USUARIO = x.ID_USUARIO;
                                obj.SISTEMA_ROL = x.SISTEMA_ROL;
                                LstUsuarioRol.Add(new cUsrRol() { Seleccion = true, UsuarioRol = obj, BD = true });
                            }
                            else
                            {
                                obj.ID_ROL = r.ID_ROL;
                                obj.ID_USUARIO = SelectedUsuario.ID_USUARIO;
                                obj.SISTEMA_ROL = r;
                                LstUsuarioRol.Add(new cUsrRol() { Seleccion = false, UsuarioRol = obj, BD = false });
                            }
                        }
                        //MOSTRAMOS ROLES
                        LstUsuarioRol = new ObservableCollection<cUsrRol>(LstUsuarioRol.OrderByDescending(w => w.Seleccion));
                        //MOSTRAMOS TODOS PROCESOS
                        if (LstUsuarioRol != null)
                        {
                            var lst = new List<SISTEMA_ROL>();
                            foreach (var r in LstUsuarioRol.Where(w => w.Seleccion))
                            {
                                lst.Add(new SISTEMA_ROL() { ID_ROL = r.UsuarioRol.ID_ROL });
                            }
                            LstProcesoUsuario = new ObservableCollection<cProcesoUsr>();
                            var lstProcesos = new ObservableCollection<PROCESO_ROL>();
                            if (lst.Count > 0)
                                lstProcesos = new ObservableCollection<PROCESO_ROL>(new cProcesoRol().Obtener(lst));
                            var lstProcesosUsuario = new ObservableCollection<PROCESO_USUARIO>(new cProcesoUsuario().ObtenerTodos(SelectedUsuario.ID_USUARIO,null,SelectedCentro));
                            if (lstProcesos != null)
                            {
                                foreach (var p in lstProcesos)
                                {
                                    var obj = new PROCESO_USUARIO();
                                    obj.ID_USUARIO = SelectedUsuario.ID_USUARIO;
                                    obj.ID_ROL = p.ID_ROL;
                                    obj.ID_PROCESO = p.ID_PROCESO;
                                    var up = lstProcesosUsuario.Where(w => w.ID_ROL == p.ID_ROL && w.ID_PROCESO == p.ID_PROCESO).FirstOrDefault();
                                    if (up != null)
                                    {
                                        obj.INSERTAR = up.INSERTAR;
                                        obj.EDITAR = up.EDITAR;
                                        obj.CONSULTAR = up.CONSULTAR;
                                        obj.IMPRIMIR = up.IMPRIMIR;
                                        LstProcesoUsuario.Add(new cProcesoUsr() { Seleccion = true, ProcesoUsuario = obj, BD = true, Proceso = p.PROCESO.DESCR, Rol = p.SISTEMA_ROL.DESCR });
                                    }
                                    else
                                    {
                                        obj.INSERTAR = 0;
                                        obj.EDITAR = 0;
                                        obj.CONSULTAR = 0;
                                        obj.IMPRIMIR = 0;
                                        LstProcesoUsuario.Add(new cProcesoUsr() { Seleccion = false, ProcesoUsuario = obj, BD = false, Proceso = p.PROCESO.DESCR, Rol = p.SISTEMA_ROL.DESCR });
                                    }
                                }
                                //procesos sin causa penal
                                foreach (var p in lstProcesosUsuario.Where(w => w.ID_ROL == 0))
                                {
                                    var obj = new PROCESO_USUARIO();
                                    obj.ID_USUARIO = SelectedUsuario.ID_USUARIO;
                                    obj.ID_ROL = p.ID_ROL;
                                    obj.ID_PROCESO = p.ID_PROCESO;
                                    obj.INSERTAR = p.INSERTAR;
                                    obj.EDITAR = p.EDITAR;
                                    obj.CONSULTAR = p.CONSULTAR;
                                    obj.IMPRIMIR = p.IMPRIMIR;
                                    LstProcesoUsuario.Add(new cProcesoUsr() { Seleccion = true, ProcesoUsuario = obj, BD = true, Proceso = p.PROCESO.DESCR });
                                }
                            }
                        }
                    }
                    //LstProcesoUsuario = new ObservableCollection<cProcesoUsr>();
                    UsuarioProcesoVisible = LstProcesoUsuario.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                }
                UsuarioRolVisible = LstUsuarioRol.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al consultar roles de usuario", ex);
            }
        }

        private bool ActualizarUsuarioRol()
        {
            try
            {
                if (SelectedUsuarioRol != null)
                {
                    var obj = new USUARIO_ROL();
                    obj.ID_USUARIO = SelectedUsuario.ID_USUARIO;
                    obj.ID_ROL = SelectedUsuarioRol.UsuarioRol.ID_ROL;
                    obj.ID_CENTRO = SelectedCentro.Value;
                    if (!SelectedUsuarioRol.BD)
                    {
                        if (new cUsuarioRol().Insertar(obj))
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                SelectedUsuarioRol.BD = true;
                                if (LstProcesoUsuario != null)
                                {
                                    var lstTmp = new ObservableCollection<cProcesoUsr>();
                                    //foreach (var pu in LstProcesoUsuario)
                                    //{
                                    //    pu.Seleccion = pu.BD = true;
                                    //}
                                    foreach (var rp in SelectedUsuarioRol.UsuarioRol.SISTEMA_ROL.PROCESO_ROL)
                                    {
                                        var o = new PROCESO_USUARIO();
                                        o.ID_PROCESO = rp.ID_PROCESO;
                                        o.ID_ROL = rp.ID_ROL;
                                        o.ID_USUARIO = SelectedUsuario.ID_USUARIO;
                                        o.ID_CENTRO = SelectedCentro.Value;
                                        o.INSERTAR = rp.INSERTAR;
                                        o.EDITAR = rp.EDITAR;
                                        o.CONSULTAR = rp.CONSULTAR;
                                        o.IMPRIMIR = rp.IMPRIMIR;
                                        LstProcesoUsuario.Add(new cProcesoUsr() { Seleccion = true, BD = true, ProcesoUsuario = o, Proceso = rp.PROCESO.DESCR, Rol = rp.SISTEMA_ROL.DESCR });
                                    }
                                    LstProcesoUsuario = new ObservableCollection<cProcesoUsr>(LstProcesoUsuario);
                                }
                                UsuarioProcesoVisible = LstProcesoUsuario.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                            }));
                            return true;
                        }
                    }
                    else
                    {
                        if (new cUsuarioRol().Actualizar(obj))
                        {
                            UsuarioProcesoVisible = LstProcesoUsuario.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al modificar roles de usuario", ex);
            }
            return false;
        }

        private bool EliminarUsuarioRol()
        {
            try
            {
                if (SelectedUsuarioRol != null)
                {
                    var obj = new USUARIO_ROL();
                    obj.ID_USUARIO = SelectedUsuario.ID_USUARIO;
                    obj.ID_ROL = SelectedUsuarioRol.UsuarioRol.ID_ROL;
                    obj.ID_CENTRO = SelectedCentro.Value;

                    if (new cUsuarioRol().Eliminar(obj))
                    {
                        SelectedUsuarioRol.BD = false;
                        //if (LstProcesoUsuario != null)
                        //{
                        //    foreach (var pu in LstProcesoUsuario)
                        //    {
                        //        pu.Seleccion = pu.BD = false;
                        //    }
                        //    LstProcesoUsuario = new ObservableCollection<cProcesoUsr>(LstProcesoUsuario);
                        //}

                        //var list = new List<cProcesoUsr>(LstProcesoUsuario.Where(w => w.ProcesoUsuario.ID_ROL == obj.ID_ROL));
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            var list = new List<cProcesoUsr>(LstProcesoUsuario.Where(w => w.ProcesoUsuario.ID_ROL == obj.ID_ROL));
                            foreach (var l in list)
                            {
                                LstProcesoUsuario.Remove(l);
                            }
                            LstProcesoUsuario = new ObservableCollection<cProcesoUsr>(LstProcesoUsuario);
                        }));

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar roles de usuario", ex);
            }
            return false;
        }
        #endregion

        #region Usuario Proceso
        private void ObtenerTodoUsuarioProceso()
        {
            try
            {
                if (SelectedUsuarioRol == null)
                    return;
                var procesos = new cProcesoRol().ObtenerTodos(SelectedUsuarioRol.UsuarioRol.ID_ROL);
                LstProcesoUsuario = new ObservableCollection<cProcesoUsr>();
                if (SelectedUsuarioRol != null)
                {
                    List<PROCESO_USUARIO> pu = new List<PROCESO_USUARIO>(new cProcesoUsuario().ObtenerTodos(SelectedUsuario.ID_USUARIO, SelectedUsuarioRol.UsuarioRol.ID_ROL, SelectedCentro));
                    if (procesos != null)
                    {
                        foreach (var p in procesos)
                        {
                            var obj = new PROCESO_USUARIO();
                            var x = pu.Where(w => w.ID_PROCESO == p.ID_PROCESO && w.ID_ROL == p.ID_ROL).FirstOrDefault();
                            if (x != null)
                            {
                                obj.ID_USUARIO = x.ID_USUARIO;
                                obj.ID_PROCESO = x.ID_PROCESO;
                                obj.ID_ROL = x.ID_ROL;
                                obj.ID_CENTRO = SelectedCentro.Value;
                                obj.INSERTAR = x.INSERTAR;
                                obj.EDITAR = x.EDITAR;
                                obj.CONSULTAR = x.CONSULTAR;
                                obj.IMPRIMIR = x.IMPRIMIR;
                                LstProcesoUsuario.Add(new cProcesoUsr() { Seleccion = true, Proceso = p.PROCESO.DESCR, ProcesoUsuario = obj, BD = true });
                            }
                            else
                            {
                                obj.ID_USUARIO = SelectedUsuario.ID_USUARIO;
                                obj.ID_PROCESO = p.ID_PROCESO;
                                obj.ID_ROL = p.ID_ROL;
                                obj.ID_CENTRO = SelectedCentro.Value;
                                obj.INSERTAR = p.INSERTAR;
                                obj.EDITAR = p.EDITAR;
                                obj.CONSULTAR = p.CONSULTAR;
                                obj.IMPRIMIR = p.IMPRIMIR;
                                LstProcesoUsuario.Add(new cProcesoUsr() { Seleccion = false, Proceso = p.PROCESO.DESCR, ProcesoUsuario = obj, BD = false });
                            }
                        }
                    }
                }
                UsuarioProcesoVisible = LstProcesoUsuario.Count > 0 ? Visibility.Collapsed : Visibility.Hidden;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al consultar procesos de usuario", ex);
            }
        }

        private bool ActualizarUsuarioProceso()
        {
            try
            {
                if (SelectedProcesoUsuario != null)
                {
                    var obj = new PROCESO_USUARIO();
                    obj.ID_USUARIO = SelectedUsuario.ID_USUARIO;
                    obj.ID_ROL = SelectedProcesoUsuario.ProcesoUsuario.ID_ROL;
                    obj.ID_PROCESO = SelectedProcesoUsuario.ProcesoUsuario.ID_PROCESO;
                    obj.ID_CENTRO = SelectedCentro.Value;
                    obj.INSERTAR = SelectedProcesoUsuario.ProcesoUsuario.INSERTAR;
                    obj.EDITAR = SelectedProcesoUsuario.ProcesoUsuario.EDITAR;
                    obj.CONSULTAR = SelectedProcesoUsuario.ProcesoUsuario.CONSULTAR;
                    obj.IMPRIMIR = SelectedProcesoUsuario.ProcesoUsuario.IMPRIMIR;
                    if (!SelectedProcesoUsuario.BD)
                    {
                        if (new cProcesoUsuario().Insertar(obj))
                        {
                            SelectedProcesoUsuario.BD = true;
                            return true;
                        }
                    }
                    else
                    {
                        if (new cProcesoUsuario().Actualizar(obj))
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al modificar procesos de usuario", ex);
            }
            return false;
        }

        private bool EliminarUsuarioProceso()
        {
            try
            {
                if (SelectedProcesoUsuario != null)
                {
                    var obj = new PROCESO_USUARIO();
                    obj.ID_USUARIO = SelectedUsuario.ID_USUARIO;
                    obj.ID_ROL = SelectedProcesoUsuario.ProcesoUsuario.ID_ROL;
                    obj.ID_PROCESO = SelectedProcesoUsuario.ProcesoUsuario.ID_PROCESO;
                    obj.ID_CENTRO = SelectedCentro.Value;
                    if (new cProcesoUsuario().Eliminar(obj))
                    {
                        SelectedProcesoUsuario.BD = false;
                        //LstPermisosRol = new ObservableCollection<cPermisosRol>(LstPermisosRol);
                        return true;
                    }
                }
                else
                    return true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar procesos de usuario", ex);
            }
            return false;
        }

        private bool GuardarUsuarioProceso()
        {
            if (LstProcesoBuscar != null)
            {
                if (LstProcesoUsuario == null)
                    LstProcesoUsuario = new ObservableCollection<cProcesoUsr>();
                foreach (var x in LstProcesoBuscar.Where(w => w.Seleccion))
                {
                    var obj = new PROCESO_USUARIO();
                    obj.ID_USUARIO = SelectedUsuario.ID_USUARIO;
                    obj.ID_ROL = 0;//SIN ROL
                    obj.ID_PROCESO = x.Proceso.ID_PROCESO;
                    obj.INSERTAR = x.Insertar;
                    obj.EDITAR = x.Editar;
                    obj.CONSULTAR = x.Consultar;
                    obj.IMPRIMIR = x.Imprimir;
                    if (new cProcesoUsuario().Insertar(obj))
                    {
                        var existe = LstProcesoUsuario.Where(w => w.ProcesoUsuario.ID_ROL == 0 && w.ProcesoUsuario.ID_PROCESO == obj.ID_PROCESO).FirstOrDefault();
                        if (existe != null)
                        {
                            existe.BD = true;
                            existe.Proceso = x.Proceso.DESCR;
                            existe.ProcesoUsuario = obj;
                            existe.Seleccion = true;
                        }
                        else
                            LstProcesoUsuario.Add(new cProcesoUsr() { Seleccion = true, BD = true, ProcesoUsuario = obj, Proceso = x.Proceso.DESCR });
                    }
                }
            }
            return false;
        }
        #endregion

        #region Centro
        private void ObtenerCentro()
        {
            try
            {
                LstCentros = new ObservableCollection<CENTRO>(new cCentro().ObtenerTodos(string.Empty,0,0,"S"));
                SelectedCentro = GlobalVar.gCentro;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar centros", ex);
            }
        }
        #endregion

        #region Buscar Empleado
        private void LimpiarBusqueda()
        {
            TextPaterno = TextMaterno = TextNombre = string.Empty;
            ListPersonas = new ObservableCollection<SSP.Servidor.PERSONA>();
            SelectPersona = null;
            EmptyBuscarRelacionInternoVisible = true;
            ImagenPersona = new Imagenes().getImagenPerson();
        }

        private void BuscarEmpleado()
        {
            ListPersonas = new ObservableCollection<SSP.Servidor.PERSONA>(new cPersona().ObtenerEmpleadosSinUsuario(TextNombre, TextPaterno, TextMaterno, GlobalVar.gCentro));
            EmptyBuscarRelacionInternoVisible = ListPersonas.Count > 0 ? false : true;
        }
        #endregion

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.PRIVILEGIOS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                foreach (var p in permisos)
                {
                    if (p.INSERTAR == 1)
                        pInsertar = true;
                    if (p.EDITAR == 1)
                    { 
                        pEditar = true;
                        BEditar = true;
                    }
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
