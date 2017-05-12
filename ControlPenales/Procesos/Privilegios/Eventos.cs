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

        #region Check
        private ICommand _Checked;
        public ICommand Checked
        {
            get
            {
                return _Checked ?? (_Checked = new RelayCommand(OnCheck));
            }
        }

        private ICommand _Unchecked;
        public ICommand Unchecked
        {
            get
            {
                return _Unchecked ?? (_Unchecked = new RelayCommand(OnCheck));
            }
        }

        private ICommand _CheckedPermisos;
        public ICommand CheckedPermisos
        {
            get
            {
                return _CheckedPermisos ?? (_CheckedPermisos = new RelayCommand(OnCheckPermisos));
            }
        }

        private ICommand _UncheckedPermisos;
        public ICommand UncheckedPermisos
        {
            get
            {
                return _UncheckedPermisos ?? (_UncheckedPermisos = new RelayCommand(OnCheckPermisos));
            }
        }

        private ICommand _CheckedUR;
        public ICommand CheckedUR
        {
            get
            {
                return _CheckedUR ?? (_CheckedUR = new RelayCommand(OnCheckUR));
            }
        }

        private ICommand _UncheckedUR;
        public ICommand UncheckedUR
        {
            get
            {
                return _UncheckedUR ?? (_UncheckedUR = new RelayCommand(OnCheckUR));
            }
        }

        private ICommand _CheckedPR;
        public ICommand CheckedPR
        {
            get
            {
                return _CheckedPR ?? (_CheckedPR = new RelayCommand(OnCheckPR));
            }
        }

        private ICommand _CheckedPRP;
        public ICommand CheckedPRP
        {
            get
            {
                return _CheckedPRP ?? (_CheckedPRP = new RelayCommand(OnCheckPRP));
            }
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
