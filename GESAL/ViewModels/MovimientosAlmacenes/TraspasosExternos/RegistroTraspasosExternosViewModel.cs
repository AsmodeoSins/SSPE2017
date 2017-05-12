using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVMShared.ViewModels;
using System.ComponentModel;
using GESAL.Models;
using System.Windows.Input;
using MVVMShared.Commands;
using MahApps.Metro.Controls.Dialogs;
namespace GESAL.ViewModels
{
    public partial class RegistroTraspasosExternosViewModel : ValidationViewModelBase, IDataErrorInfo
    {
        private IDialogCoordinator _dialogCoordinator;
        private Usuario _usuario;
        public RegistroTraspasosExternosViewModel(IDialogCoordinator dialogCoordinator, Usuario usuario)
        {
            _dialogCoordinator = dialogCoordinator;
            _usuario = usuario;
        }
        private bool CanExecute(object parameter) { return this.HasErrors == false; }

        #region Comandos
        private ICommand cmdLoad;

        public ICommand CmdLoad
        {
            get { return cmdLoad??(cmdLoad=new RelayCommand(OnLoad));}
        }

        private ICommand cmdOnClick;

        public ICommand CmdOnClick
        {
            get { return cmdOnClick??(cmdOnClick=new RelayCommand(OnClickSwitch)); }
        }
        #endregion
    }
}
