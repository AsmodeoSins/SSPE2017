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


namespace ControlPenales
{
    partial class CambiarContrasenaViewModel
    {
        #region commands
        private ICommand _onClick;
        public ICommand OnClick
        {
            get
            {
                return _onClick ?? (_onClick = new RelayCommand(clickSwitch));
            }
        }
        public ICommand OnLoaded
        {
            get { return new DelegateCommand<CambiarContrasenaView>(OnLoad); }
        }

        private ICommand passwordCommand;
        public ICommand PasswordCommand
        {
            get
            {
                return passwordCommand ?? (passwordCommand = new RelayCommand(Password));
            }
        }

        private ICommand passwordCommandN;
        public ICommand PasswordCommandN
        {
            get
            {
                return passwordCommandN ?? (passwordCommandN = new RelayCommand(PasswordN));
            }
        }

        private ICommand passwordCommandNR;
        public ICommand PasswordCommandNR
        {
            get
            {
                return passwordCommandNR ?? (passwordCommandNR = new RelayCommand(PasswordNR));
            }
        }

        #endregion
    }
}
