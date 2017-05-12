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
    class CalendarioEntregasViewModel : ValidationViewModelBase, IPageViewModel
    {
        #region constructor
        public CalendarioEntregasViewModel()
        {
            GridVisible = false;
        }
        #endregion

        #region variables
        private bool gridVisible;
        public bool GridVisible
        {
            get { return gridVisible; }
            set { gridVisible = value; OnPropertyChanged("GridVisible"); }
        }
        public string Name
        {
            get
            {
                return "almacen_calendario_entregas";
            }
        }
        #endregion

        #region metodos
        void IPageViewModel.inicializa()
        { }
        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "boton_buscar_calendario_entregas":
                    if (GridVisible == false)
                    {
                        GridVisible = true;
                    }
                    else
                    {
                        GridVisible = false;
                        await TaskEx.Delay(1000);
                        GridVisible = true;
                    }
                    break;
            }
        }
        #endregion

        #region command
        private ICommand _onClick;
        public ICommand OnClick
        {
            get
            {
                return _onClick ?? (_onClick = new RelayCommand(clickSwitch));
            }
        }
        Usuario _selectedItem = null;
        public Usuario SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
            }
        }
        #endregion
    }
}
