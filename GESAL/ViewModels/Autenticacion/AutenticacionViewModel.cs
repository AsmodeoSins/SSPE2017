using Biometrico.DigitalPersona;
using GESAL.Models;
using MahApps.Metro.Controls.Dialogs;
using MVVMShared.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace GESAL.ViewModels
{
    public partial class AutenticacionViewModel : FingerPrintScanner
    {
        private IDialogCoordinator _dialogCoordinator;
        private Usuario _usuario;

        public AutenticacionViewModel(IDialogCoordinator dialogCoordinator, Usuario usuario)
        {
            _dialogCoordinator = dialogCoordinator;
            _usuario = usuario;
        }

        private bool CanExecute(object parameter) { return base.HasErrors == false; }

        #region Comandos
        private ICommand cmdLoad;
        public ICommand CmdLoad
        {
            get { return cmdLoad??(cmdLoad=new RelayCommand(OnLoad));}
        }

        private ICommand cmdOk;
        public ICommand CmdOk
        {
            get { return cmdOk??(cmdOk=new RelayCommand(Autenticar,CanExecute));}
        }
        #endregion
    }
}
