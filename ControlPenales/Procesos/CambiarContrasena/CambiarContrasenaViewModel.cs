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
using SSP.Servidor;
using SSP.Controlador.Catalogo.Justicia;
using ControlPenales.BiometricoServiceReference;
using System.Windows.Controls;
using System.Windows.Interop;
using ControlPenales.Clases;
using System.Threading;
using Cogent.Biometrics;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing;


namespace ControlPenales
{
    partial class CambiarContrasenaViewModel : ValidationViewModelBase
    {
        #region constructor
        public CambiarContrasenaViewModel() 
        {
            
        }
        #endregion

        #region Metodos
        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "guardar_menu":
                    if (!pEditar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    Guardar();                    
                    break;
                case "limpiar_menu":
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new CambiarContrasenaView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new CambiarContrasenaViewModel();
                    break;
                case "salir_menu":
                    PrincipalViewModel.SalirMenu();
                    break;
            }
        }

        private async void OnLoad(CambiarContrasenaView obj = null)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(Cargar);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar decomisos", ex);
            }

        }

        private void Cargar() 
        {
            try 
            {
                ConfiguraPermisos();
                System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
               {
                   
                   StaticSourcesViewModel.SourceChanged = false;
               }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar listados.", ex);
            }
        }

        private async void Password(Object obj)
        {
            try
            {
                if (obj != null)
                {
                    PPassword = ((PasswordBox)(obj)).Password;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al bindear password", ex);
            }
        }

        private async void PasswordN(Object obj)
        {
            try
            {
                if (obj != null)
                {
                    PPasswordNuevo  = ((PasswordBox)(obj)).Password;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al bindear password", ex);
            }
        }

        private async void PasswordNR(Object obj)
        {
            try
            {
                if (obj != null)
                {
                    PPasswordNuevoRepetir = ((PasswordBox)(obj)).Password;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al bindear password", ex);
            }
        }
        #endregion

        #region CPassword
        private async void Guardar() {

            var respuesta = await StaticSourcesViewModel.OperacionesAsync<short>("Guardando password", () =>
            {
                try
                {

                    if (!string.IsNullOrEmpty(PPasswordNuevo) && !string.IsNullOrEmpty(PPasswordNuevoRepetir))
                    {
                        ////validar el antiguo password
                        //if (cEncriptacion.IsEquals(PPassword, StaticSourcesViewModel.UsuarioLogin.Password))
                        //{
                            if (!cEncriptacion.IsEquals(PPasswordNuevo, StaticSourcesViewModel.UsuarioLogin.Password))
                            {
                                if (PPasswordNuevo == PPasswordNuevoRepetir)
                                {
                                    // return true;
                                    //if (ValidarContrasena())
                                    //{
                                    var u = new cUsuario().Obtener(GlobalVar.gUsr);
                                    if (u != null)
                                    {
                                        var obj = new USUARIO();
                                        #region copia
                                        obj.ID_USUARIO = u.ID_USUARIO;
                                        obj.ID_PERSONA = u.ID_PERSONA;
                                        obj.ESTATUS = u.ESTATUS;
                                        #endregion
                                        obj.PASSWORD = cEncriptacion.HarshedText(PPasswordNuevo.ToUpper());
                                        if (new cUsuario().Actualizar(obj,Parametro.PASSWORD_USUARIO_BD))
                                        {
                                            //new Dialogos().ConfirmacionDialogo("Éxito", "El password se ha actualizado correctamente");
                                            return 1;
                                        }
                                    }
                                    else
                                        //new Dialogos().ConfirmacionDialogo("Error", "Error al actualizar password");
                                        return 2;
                                    //}
                                }
                                else
                                    //new Dialogos().ConfirmacionDialogo("Validación", "Error al validar el nuevo password.");
                                    return 3;
                            }
                            else
                                //new Dialogos().ConfirmacionDialogo("Validación", "El nuevo password debe ser diferente al actual.");
                                return 4;
                        //}
                        //else
                        //    //new Dialogos().ConfirmacionDialogo("Validación", "El password actual es incorrecto.");
                        //    return 5;
                    }
                    else
                        //new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar los campos requeridos");
                        return 6;

                    
                    return 2;
                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo paso...", "Ocurrio un error...", ex);
                    return 2;
                }
            });

            switch(respuesta){
                case 1:
                    new Dialogos().ConfirmacionDialogo("Éxito", "El password se ha actualizado correctamente");
                    break;
                case 2:
                    new Dialogos().ConfirmacionDialogo("Error", "Error al actualizar password");
                    break;
                case 3:
                    new Dialogos().ConfirmacionDialogo("Validación", "Error al validar el nuevo password.");
                    break;
                case 4:
                    new Dialogos().ConfirmacionDialogo("Validación", "El nuevo password debe ser diferente al actual.");
                    break;
                case 5:
                    new Dialogos().ConfirmacionDialogo("Validación", "El password actual es incorrecto.");
                    break;
                case 6:
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar los campos requeridos");
                    break;
                case 7:
                    break;
            }
        }
        #endregion

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CAMBIO_CLAVE_ACCESO.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
