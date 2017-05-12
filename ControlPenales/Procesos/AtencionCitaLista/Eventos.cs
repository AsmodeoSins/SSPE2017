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
    partial class AtencionCitaListaViewModel
    {
        #region Loads
        public ICommand WindowLoaded
        {
            get { return new DelegateCommand<AtencionCitaListaView>(WindowLoad); }
        }
        #endregion

        #region Click
        private ICommand onClick;
        public ICommand OnClick
        {
            get
            {
                return onClick ?? (onClick = new RelayCommand(clickSwitch));
            }
        }

        private ICommand _ClickAgenda;
        public ICommand ClickAgenda
        {
            get
            {
                return _ClickAgenda ?? (_ClickAgenda = new RelayCommand(AgendaSwitch));
            }
        }
        #endregion

        #region DoubleClick
        private ICommand doubleClickGridCommand;
        public ICommand DoubleClickGridCommand
        {
            get
            {
                return doubleClickGridCommand ?? (doubleClickGridCommand = new RelayCommand(SeleccionaInterno));
            }
        }
        #endregion

        #region Huella
        public ICommand WindowLoading
        {
            get { return new DelegateCommand<BuscarPorHuellaYNipView>(OnLoad); }
        }
        public ICommand BuscarHuella
        {
            get { return new DelegateCommand<string>(OnBuscarPorHuella); }
        }

        public ICommand CommandAceptar
        {
            get { return new DelegateCommand<Window>(Aceptar); }
        }

        public ICommand CommandOpem442
        {
            get { return new DelegateCommand<string>(Capture); }
        }

        public ICommand CommandContinue
        {
            get { return new DelegateCommand<string>((s) => { isKeepSearching = s == "True" ? true : false; }); }
        }
        #endregion
    }
}
