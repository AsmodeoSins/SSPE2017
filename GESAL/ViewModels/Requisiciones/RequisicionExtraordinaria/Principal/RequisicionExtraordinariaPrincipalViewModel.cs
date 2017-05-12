using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVMShared.ViewModels;
using System.ComponentModel;
using GESAL.Models;
using System.Windows.Input;
using MVVMShared.Commands;
using SSP.Servidor.ModelosExtendidos;
using MahApps.Metro.Controls.Dialogs;
namespace GESAL.ViewModels
{
    public partial class RequisicionExtraordinariaPrincipalViewModel:ValidationViewModelBase,IDataErrorInfo
    {
        private IDialogCoordinator _dialogCoordinator;
        private Usuario _usuario;

        public RequisicionExtraordinariaPrincipalViewModel(IDialogCoordinator dialogCoordinator, Usuario usuario)
        {
            _dialogCoordinator = dialogCoordinator;
            _usuario = usuario;
        }

        private bool CanExecute(object parameter) { return this.HasErrors == false; }

        #region Comandos Principal
        private ICommand cmdLoad;

        public ICommand CmdLoad
        {
            get { return cmdLoad ?? (cmdLoad = new RelayCommand(OnLoad)); }
        }

        private ICommand cmdAccionMenuSinValidar;
        public ICommand CmdAccionMenuSinValidar
        {
            get { return cmdAccionMenuSinValidar?? (cmdAccionMenuSinValidar=new RelayCommand(AccionMenuSwitch));}
        }


        private ICommand cmdAccionMenu;

        public ICommand CmdAccionMenu
        {
            get { return cmdAccionMenu ?? (cmdAccionMenu = new RelayCommand(AccionMenuSwitch,CanExecute)); }
        }

        private ICommand cmdCancelarBusquedaPopup;
        public ICommand CmdCancelarBusquedaPopup
        {
            get { return cmdCancelarBusquedaPopup ?? (cmdCancelarBusquedaPopup = new RelayCommand(BusquedaPopUpSwitch)); }
        }

        private ICommand cmdSeleccionarBusquedaPopUp;
        public ICommand CmdSeleccionarBusquedaPopUp
        {
            get { return cmdSeleccionarBusquedaPopUp ?? (cmdSeleccionarBusquedaPopUp = new RelayCommand(BusquedaPopUpSwitch,CanExecute)); }
        }

        #endregion

        #region Comandos Requisicion Extraordinaria

        private ICommand cmdOnClickRequisicion;
        public ICommand CmdOnClickRequisicion
        {
            get { return cmdOnClickRequisicion ?? (cmdOnClickRequisicion = new RelayCommand(OnClickRequisicionSwitch)); }
        }

        private ICommand cmdValidacionRequerido;
        public ICommand CmdValidacionRequerido
        {
            get { return cmdValidacionRequerido ?? (cmdValidacionRequerido = new RelayCommand(ValidarCantidadesRequeridad)); }
        }

        private ICommand cmdComboBoxFiltroRequisicionChanged;
        public ICommand CmdComboBoxFiltroRequisicionChanged
        {
            get { return cmdComboBoxFiltroRequisicionChanged ?? (cmdComboBoxFiltroRequisicionChanged = new RelayCommand(ValidarFiltroRequisicionCambio)); }
        }

        #endregion

        #region Comandos Traspaso Externo

        private ICommand cmdOnClickTraspasos;
        public ICommand CmdOnClickTraspasos
        {
            get { return cmdOnClickTraspasos ?? (cmdOnClickTraspasos = new RelayCommand(OnClickTraspasosSwitch)); }
        }

        private ICommand cmdModeloTraspasoChanged;
        public ICommand CmdModeloTraspasoChanged
        {
            get { return cmdModeloTraspasoChanged ?? (cmdModeloTraspasoChanged = new RelayCommand(ModeloTraspasoChangedSwitch)); }
        }

        private ICommand cmdValidacionTraspaso;
        public ICommand CmdValidacionTraspaso
        {
            get { return cmdValidacionTraspaso ?? (cmdValidacionTraspaso = new RelayCommand(ValidarTraspasoExterno)); }
        }


        #endregion

        private class Datos_Reporte
        {
            public List<EXT_Reporte_RequisicionExtraordinaria> datos { get; set; }
            public List<EXT_RequisicionExtraordinaria_Encabezado> encabezado { get; set; }
        }
    }
}
