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

namespace ControlPenales
{
    class RegistroEntradaViewModel : ValidationViewModelBase, IPageViewModel
    {
        #region constructor
        public RegistroEntradaViewModel() 
        {
            BuscarVisible = false;
        }
        #endregion

        #region variables
        private bool buscarVisible;
        public bool BuscarVisible
        {
            get { return buscarVisible; }
            set { buscarVisible = value; OnPropertyChanged("BuscarVisible"); }
        }
        public string Name
        {
            get
            {
                return "registro_entrada_bitacora";
            }
        }
        #endregion

        #region metodos
        void IPageViewModel.inicializa()
        { }
        private void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "buscar_menu":
                    BuscarVisible = true;
                    break;
                case "buscar_seleccionar":
                    BuscarVisible = false;
                    break;
                case "buscar_salir":
                    BuscarVisible = false;
                    break;
            }
        }
        #endregion

        #region commands
        private ICommand _clickPageCommand;
        public ICommand ClickPageCommand
        {
            get
            {
                return _clickPageCommand ?? (_clickPageCommand = new RelayCommand(clickSwitch));
            }
        }
        #endregion
    }
}
