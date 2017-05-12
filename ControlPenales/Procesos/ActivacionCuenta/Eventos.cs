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
    partial class ActivacionCuentaViewModel : ValidationViewModelBase
    {

        #region Click
        private ICommand onClick;
        public ICommand OnClick
        {
            get
            {
                return onClick ?? (onClick = new RelayCommand(clickSwitch));
            }
        }

        private ICommand buscarClick;
        public ICommand BuscarClick
        {
            get
            {
                return buscarClick ?? (buscarClick = new RelayCommand(ClickEnter));
            }
        }

        private ICommand _EnterClick;
        public ICommand EnterClick
        {
            get { return _EnterClick ?? (_EnterClick = new RelayCommand(EnterPersonas)); }
        }
        #endregion

        #region Load
        public ICommand OnLoaded
        {
            get { return new DelegateCommand<PrivilegiosView>(OnLoad); }
        }
        #endregion

        #region Password
        private ICommand passwordCommand;
        public ICommand PasswordCommand
        {
            get
            {
                return passwordCommand ?? (passwordCommand = new RelayCommand(Password));
            }
        }

        private ICommand passwordRCommand;
        public ICommand PasswordRCommand
        {
            get
            {
                return passwordRCommand ?? (passwordRCommand = new RelayCommand(PasswordRepetir));
            }
        }
        #endregion
    }
}
