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
    partial class ActivacionCuentaViewModel : ValidationViewModelBase
    {
        #region constructor
        public ActivacionCuentaViewModel() { }
        #endregion

        #region metodos
        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "salir_menu":
                    PrincipalViewModel.SalirMenu();
                    break;
                case "buscar_usuario":
                    if (!pConsultar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    ObtenerTodoUsuarios();
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
                    PopUpsViewModels.ShowPopUp(this,PopUpsViewModels.TipoPopUp.AGREGAR_USUARIO);
                    break;
                case "guardar_usuario":
                    if (!pEditar)
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
                       // else
                       //     new Dialogos().ConfirmacionDialogo("Error", "No se guardo la información");
                    }
                    break;
                case "cancelar_usuario":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_USUARIO);
                    LimpiarUsuario();
                    base.ClearRules();
                    break;
                case "limpiar_menu":
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new ActivacionCuentaView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ActivacionCuentaViewModel();
                    break;
            }
        }

        private async void OnLoad(PrivilegiosView obj = null)
        {
            try
            {
                ConfiguraPermisos();
                if(pConsultar)
                    ObtenerTodoUsuarios();
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
                        case "Usuario":
                            BUsuario = textbox.Text;
                            ObtenerTodoUsuarios();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ingresar busqueda", ex);
            }
        }

        private async void Password(Object obj)
        {
            try
            {
                if(obj != null)
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
     
        #region Usuarios
        private void ObtenerTodoUsuarios()
        {
            try 
            {
                LstUsuario = new ObservableCollection<USUARIO>(new cUsuario().ObtenerTodos(BUsuario));
                UsuariosVisible = LstUsuario.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
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
                    }

                    if (SelectedUsuario.EMPLEADO.PERSONA == null)
                    {
                        UImagen = new Imagenes().getImagenPerson();
                        return; 
                    }

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
        //
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
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.ACTIVACION_CUENTA.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                foreach (var p in permisos)
                {
                    if (p.INSERTAR == 1)
                        pInsertar = true;
                    if (p.EDITAR == 1)
                        pEditar = true;
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
