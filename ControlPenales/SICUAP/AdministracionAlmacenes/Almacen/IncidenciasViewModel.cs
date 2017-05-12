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
    class IncidenciasViewModel : ValidationViewModelBase, IPageViewModel
    {
        #region constructor
        public IncidenciasViewModel() 
        {
            GridBuscarVisible = false;
            ProductosVisible = false;
        }
        #endregion

        #region variables
        private bool productosVisible;
        public bool ProductosVisible
        {
            get { return productosVisible; }
            set { productosVisible = value; OnPropertyChanged("ProductosVisible"); }
        }
        private bool gridBuscarVisible;
        public bool GridBuscarVisible
        {
            get { return gridBuscarVisible; }
            set { gridBuscarVisible = value; OnPropertyChanged("GridBuscarVisible"); }
        }
        public string Name
        {
            get
            {
                return "almacen_incidencias";
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
                case "boton_guardar_producto_incidencia":
                    ProductosVisible = true;
                    break;
                case "boton_aceptar_producto_incidencia":
                    ProductosVisible = false;
                    break;
                case "boton_cerrar_producto_incidencia":
                    ProductosVisible = false;
                    break;
                case "boton_buscar_incidencias_productos":
                    ProductosVisible = false;
                    GridBuscarVisible = true;
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
