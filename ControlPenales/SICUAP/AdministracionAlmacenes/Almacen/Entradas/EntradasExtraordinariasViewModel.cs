using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace ControlPenales
{
    class EntradasExtraordinariasViewModel : ValidationViewModelBase, IPageViewModel
    {

        #region constructor
        public EntradasExtraordinariasViewModel()
        {
            GridBuscarVisible = false;
            ProductosEntradaVisible = false;
            Lista2 = new ObservableCollection<DetalleTraspaso>();
            Lista2.Add(new DetalleTraspaso() { Cantidad = "21", Producto = "Paracetamol", UnidadMedida = "Pastilla" });
            Lista2.Add(new DetalleTraspaso() { Cantidad = "50", Producto = "Diclofenaco", UnidadMedida = "Capsula" });
        }
        #endregion

        #region variables
        private ObservableCollection<DetalleTraspaso> lista2;
        public ObservableCollection<DetalleTraspaso> Lista2
        {
            get { return lista2; }
            set { lista2 = value; OnPropertyChanged("Lista2"); }
        }
        private bool productosEntradaVisible;
        public bool ProductosEntradaVisible
        {
            get { return productosEntradaVisible; }
            set { productosEntradaVisible = value; OnPropertyChanged("ProductosEntradaVisible"); }
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
                return "entradas_extraordinarias";
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
                case "boton_buscar_calendario_entregas":
                    GridBuscarVisible = true;
                    ProductosEntradaVisible = false;
                    break;
                case "boton_guardar_producto_entrada_extraordinaria":
                    GridBuscarVisible = true;
                    ProductosEntradaVisible = false;
                    break;
                case "boton_quitar_producto_entrada_extraordinaria":
                    GridBuscarVisible = true;
                    ProductosEntradaVisible = false;
                    break;
                case "click_doble":
                    GridBuscarVisible = true;
                    ProductosEntradaVisible = true;
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
        #endregion

    }
}
