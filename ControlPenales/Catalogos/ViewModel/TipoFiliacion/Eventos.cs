using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;
using SSP.Servidor;
using SSP.Controlador.Catalogo.Justicia;
using System.ComponentModel;

namespace ControlPenales
{
    partial class CatalogoTipoFiliacionViewModel
    {
        private ICommand _buscarClick;
        public ICommand BuscarClick
        {
            get { return _buscarClick ?? (_buscarClick = new RelayCommand(ClickEnter)); }
        }

        private ICommand _onClick;
        public ICommand OnClick
        {
            get { return _onClick ?? (_onClick = new RelayCommand(clickSwitch)); }
        }

        private ICommand _onClickSelected;
        public ICommand OnClickSelected
        {
            get { return _onClickSelected ?? (_onClickSelected = new RelayCommand(clickTipo)); }
        }

        public ICommand TipoFiliacionLoading
        {
            get { return new DelegateCommand<CatalogoSimpleView>(TipoFiliacionLoad); }
        }
    }
}