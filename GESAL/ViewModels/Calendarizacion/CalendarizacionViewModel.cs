using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVMShared.ViewModels;
using System.Collections.ObjectModel;
using SSP.Servidor;
using System.Windows.Input;
using MVVMShared.Commands;
using MVVMShared.ViewModels.Interfaces;
using System.ComponentModel;
using GESAL.Clases.Enums;
using MVVMShared.Views;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;
using GESAL.Models;
using SSP.Controlador.Catalogo.Almacenes;
using SSP.Controlador.Principales.Almacenes;
using SSP.Controlador.Catalogo.Justicia;

namespace GESAL.ViewModels
{
    public partial class CalendarizacionViewModel : ValidationViewModelBase, IDataErrorInfo
    {
        private IDialogCoordinator _dialogCoordinator;
        private Usuario _usuario;
        public CalendarizacionViewModel(IDialogCoordinator dialogCoordinator, Usuario usuario)
        {
            _dialogCoordinator = dialogCoordinator;           
            _usuario = usuario;
            BindCmdDayClick = "CmdAbrirAgenda";
            BindSelectedMesProperty = "SelectedMes";
            BindAnioMaximoProperty = "AnioMaximo";
            BindAnioMinimoProperty = "AnioMinimo";
            BindSelectedAnioProperty = "SelectedAnio";
            BindDiasAgendadosProperty = "DiasAgendados";
            AnioMinimo = DateTime.Now.Year - 10;
            AnioMaximo = DateTime.Now.Year + 1;
        }

        private bool CanExecute(object parameter) { return base.HasErrors == false; }


        #region Comandos
        private ICommand cmdAbrirAgenda;
        public ICommand CmdAbrirAgenda
        {
            get { return cmdAbrirAgenda ?? (cmdAbrirAgenda = new RelayCommand(AbrirAgenda,CanExecute)); }
        }

        private ICommand cmdAccionMenuSinValidar;
        public ICommand CmdAccionMenuSinValidar
        {
            get { return cmdAccionMenuSinValidar ?? (cmdAccionMenuSinValidar = new RelayCommand(AccionAgenda)); }
        }

        private ICommand cmdAccionMenu;
        public ICommand CmdAccionMenu
        {
            get { return cmdAccionMenu ?? (cmdAccionMenu = new RelayCommand(AccionAgenda, CanExecute)); }
        }


        private ICommand cmdOnClick;
        public ICommand CmdOnClick
        {
            get { return cmdOnClick ?? (cmdOnClick = new RelayCommand(AccionAgenda)); }
        }

        private ICommand cmdSeleccionarOC;
        public ICommand CmdSeleccionarOC
        {
            get { return cmdSeleccionarOC??(cmdSeleccionarOC=new RelayCommand(AccionOrdenCompra));}
        }

        private ICommand cmdCancelarOC;
        public ICommand CmdCancelarOC
        {
            get { return cmdCancelarOC ?? (cmdCancelarOC = new RelayCommand(AccionOrdenCompra)); }
        }

        private ICommand onChecked;
        public ICommand OnChecked
        {
            get { return onChecked ?? (onChecked = new RelayCommand(RaiseChangeCheckedOrdenes_Compra_Detalle)); }
        }

        private ICommand cmdValidacionProgramas;
        public ICommand CmdValidacionProgramas
        {
            get { return cmdValidacionProgramas ?? (cmdValidacionProgramas = new RelayCommand(VerificarProgramados)); }
        }

        private ICommand cmdRechazarProducto;
        public ICommand CmdRechazarProducto
        {
            get { return cmdRechazarProducto ?? (cmdRechazarProducto=new RelayCommand(RecalendarizarProductoSwitch,CanExecute));}
        }

        private ICommand cmdCancelarRechazarProducto;
        public ICommand CmdCancelarRechazarProducto
        {
            get { return cmdCancelarRechazarProducto?? (cmdCancelarRechazarProducto=new RelayCommand(RecalendarizarProductoSwitch));}
        }

        private ICommand cmdLoad;
        public ICommand CmdLoad
        {
            get { return cmdLoad ?? (cmdLoad = new RelayCommand(OnLoad)); }
        }

        #endregion
    }
}
