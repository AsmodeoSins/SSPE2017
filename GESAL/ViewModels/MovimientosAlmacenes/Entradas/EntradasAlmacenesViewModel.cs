using GESAL.Models;
using MVVMShared.Commands;
using MVVMShared.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using WPFPdfViewer;
using GESAL.Views;
using MahApps.Metro.Controls.Dialogs;
namespace GESAL.ViewModels
{
    public partial class EntradasAlmacenesViewModel : ValidationViewModelBase, IDataErrorInfo
    {
        private IDialogCoordinator _dialogCoordinator;
        private Usuario _usuario;
        public EntradasAlmacenesViewModel(IDialogCoordinator dialogCoordinator, Usuario usuario)
        {
            _dialogCoordinator = dialogCoordinator;
            _usuario = usuario;
        }
        private bool CanExecute(object parameter) { return this.HasErrors == false; }



        #region Comandos
        private ICommand cmdLoad;
        public ICommand CmdLoad
        { get { return cmdLoad ?? (cmdLoad = new RelayCommand(OnLoad)); } }

        private ICommand cmdDblClick;
        public ICommand CmdDblClick
        {
            get { return cmdDblClick ?? (cmdDblClick = new RelayCommand(DblClickSwitch)); }
        }

        private ICommand cmdCancelarLote;
        public ICommand CmdCancelarLote
        {
            get { return cmdCancelarLote?? (cmdCancelarLote=new RelayCommand(CapturaEntradaLote));}
        }

        private ICommand cmdCapturaLote;
        public ICommand CmdCapturaLote
        {
            get { return cmdCapturaLote ?? (cmdCapturaLote = new RelayCommand(AsignaLotesSwitch, CanExecute)); }
        }

        private ICommand cmdAsignarLote;
        public ICommand CmdAsignarLote
        {
            get { return cmdAsignarLote ?? (cmdAsignarLote=new RelayCommand(CapturaEntradaLote)); }
        }

        private ICommand cmdGridLote;
        public ICommand CmdGridLote
        {
            get { return cmdGridLote??(cmdGridLote=new RelayCommand(AsignaLotesSwitch));}
        }

        private ICommand cmdOnChecked;
        public ICommand CmdOnChecked
        {
            get { return cmdOnChecked??(cmdOnChecked=new RelayCommand(RaiseChangeCheckedProductos)); }
        }

        //private ICommand cmdGridTextBoxLostFocus;
        //public ICommand CmdGridTextBoxLostFocus
        //{
        //    get { return cmdGridTextBoxLostFocus ?? (cmdGridTextBoxLostFocus = new RelayCommand(ConfirmacionIncidenciaProducto)); }
        //}

        private ICommand cmdBuscarOC;
        public ICommand CmdBuscarOC
        {
            get { return cmdBuscarOC ?? (cmdBuscarOC = new RelayCommand(BuscarOC)); }
        }

        private ICommand cmdCancelarRechazarProducto;
        public ICommand CmdCancelarRechazarProducto
        {
            get { return cmdCancelarRechazarProducto??(cmdCancelarRechazarProducto=new RelayCommand(RechazarProductoSwitch));}
        }

        private ICommand cmdRechazarProducto;
        public ICommand CmdRechazarProducto
        {
            get { return cmdRechazarProducto ?? (cmdRechazarProducto = new RelayCommand(RechazarProductoSwitch,CanExecute)); }
        }

        //private ICommand cmdMessage_DialogAction;
        //public ICommand CmdMessage_DialogAction
        //{
        //    get { return cmdMessage_DialogAction ?? (cmdMessage_DialogAction = new RelayCommand(MessageDialogoSwitch)); }
        //}

        private ICommand cmdAccionMenu;
        public ICommand CmdAccionMenu
        {
            get { return cmdAccionMenu ?? (cmdAccionMenu = new RelayCommand(AccionMenuSwitch, CanExecute)); }
        }

        private ICommand cmdAccionMenuSinValidar;
        public ICommand CmdAccionMenuSinValidar
        {
            get { return cmdAccionMenuSinValidar ?? (cmdAccionMenuSinValidar = new RelayCommand(AccionMenuSwitch)); }
        }

        private ICommand cmdTabIndexOrdenesChanged;
        public ICommand CmdTabIndexOrdenesChanged
        {
            get { return new DelegateCommand<String>(OnSelectedTabIndexOrdenesChanged); }
        }

        #endregion

        #region Digitalizacion
        public ICommand startScanning
        {
            get { return new DelegateCommand<PdfViewer>(Scan); }
        }
        public ICommand OpenDocument
        {
            get { return new DelegateCommand<PdfViewer>(AbrirDocumento); }
        }
        private ICommand cmdAccionEscaner;
        public ICommand CmdAccionEscaner
        {
            get { return cmdAccionEscaner ?? (cmdAccionEscaner = new RelayCommand(AccionEscanerSwitch)); }
        }
        #endregion
    }
}
