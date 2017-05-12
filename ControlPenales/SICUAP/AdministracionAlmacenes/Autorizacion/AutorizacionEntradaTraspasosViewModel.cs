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
    class AutorizacionEntradaTraspasosViewModel : ValidationViewModelBase, IPageViewModel
    {
        #region constructor
        public AutorizacionEntradaTraspasosViewModel()
        {
            ProductosVisible = false;
            BotonesVisible = false;
            Lista2 = new ObservableCollection<DetalleTraspaso>();
            Lista1 = new ObservableCollection<TraspasoTest>();
            Lista1.Add(new TraspasoTest() { Almacen = "Prueba", Fecha = "02/03/2015", Pedido = "1024" });
            Lista1.Add(new TraspasoTest() { Almacen = "Test", Fecha = "01/03/2015", Pedido = "1017" });
        }
        #endregion

        #region variables
        private ObservableCollection<TraspasoTest> lista1;
        public ObservableCollection<TraspasoTest> Lista1
        {
            get { return lista1; }
            set { lista1 = value; OnPropertyChanged("Lista1"); }
        }
        private ObservableCollection<DetalleTraspaso> lista2;
        public ObservableCollection<DetalleTraspaso> Lista2
        {
            get { return lista2; }
            set { lista2 = value; OnPropertyChanged("Lista2"); }
        }
        private bool botonesVisible;
        public bool BotonesVisible
        {
            get { return botonesVisible; }
            set { botonesVisible = value; OnPropertyChanged("BotonesVisible"); }
        }
        private bool productosVisible;
        public bool ProductosVisible
        {
            get { return productosVisible; }
            set { productosVisible = value; OnPropertyChanged("ProductosVisible"); }
        }
        public string Name
        {
            get
            {
                return "autorizacion_entrada_traspasos";
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
                case "boton_generar_requesicion":

                    break;
                case "click_doble":
                    ProductosVisible = true;
                    if (_selectedItem.Pedido == "1024")
                    {
                        Lista2.Clear();
                        Lista2.Add(new DetalleTraspaso() { Cantidad = "35", Producto = "Chicle", UnidadMedida = "Pieza" });
                        Lista2.Add(new DetalleTraspaso() { Cantidad = "5", Producto = "Dulce", UnidadMedida = "Kilo" });
                    }
                    else
                    {
                        Lista2.Clear();
                        Lista2.Add(new DetalleTraspaso() { Cantidad = "21", Producto = "Paracetamol", UnidadMedida = "Pastilla" });
                        Lista2.Add(new DetalleTraspaso() { Cantidad = "50", Producto = "Diclofenaco", UnidadMedida = "Capsula" });
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
        TraspasoTest _selectedItem = null;
        public TraspasoTest SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                if (_selectedItem == null)
                {
                    BotonesVisible = false;
                }
                else
                {
                    BotonesVisible = true;
                }
            }
        }
        #endregion
    }
}
