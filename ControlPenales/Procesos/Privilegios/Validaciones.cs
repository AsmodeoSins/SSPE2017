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

namespace ControlPenales
{
    partial class PrivilegiosViewModel : ValidationViewModelBase
    {
        #region Procesos
        private void ValidacionProceso() 
        {
            base.ClearRules();
            base.AddRule(() => PDescripcion, () => !string.IsNullOrEmpty(PDescripcion), "Proceso es requerido!");
            base.AddRule(() => PVentana, () => !string.IsNullOrEmpty(PVentana), "Ventana es requerida!");
            OnPropertyChanged("PDescripcion");
            OnPropertyChanged("PVentana");
        }
        #endregion

        #region Roles
        private void ValidacionRol()
        {
            base.ClearRules();
            base.AddRule(() => RDescripcion, () => !string.IsNullOrEmpty(RDescripcion), "Rol es requerido!");
            OnPropertyChanged("RDescripcion");
        }
        #endregion

        #region Usuarios
        private void ValidacionUsuario() 
        {
            base.ClearRules();
            base.AddRule(() => ULogin, () => !string.IsNullOrEmpty(ULogin), "Login es requerido!");
            OnPropertyChanged("RDescripcion");
        }

        private bool ValidarPassword()
        {
            if (!string.IsNullOrEmpty(UPassword) && !string.IsNullOrEmpty(UPasswordR))
            {
                if (UPassword == UPasswordR)
                    return true;
                else
                { 
                    new Dialogos().ConfirmacionDialogo("Validación", "Los passwords no son iguales, favor de volver a intentar");
                    UPassword = UPasswordR = string.Empty;
                }
            }
            return false;
        }
        #endregion
    }
}
