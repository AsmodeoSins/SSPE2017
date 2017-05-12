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
    class EntradaTraspasosViewModel : ValidationViewModelBase, IPageViewModel
    {
        #region constructor
        public EntradaTraspasosViewModel()
        {
            Lista1 = new ObservableCollection<TraspasoTest>();
            Lista1.Add(new TraspasoTest() { Almacen = "Prueba", Pedido = "4023", Fecha = "01/03/2015" });
            Lista1.Add(new TraspasoTest() { Almacen = "Test", Pedido = "4001", Fecha = "25/02/2015" });
            Lista2 = new ObservableCollection<DetalleTraspaso>();
            DetallePedidoVisible = false;
            FiltrarVisible = false;
            BotonesVisible = false;
            SeleccionIndice = -1;
        }
        #endregion

        #region variables
        private int seleccionIndice;
        public int SeleccionIndice
        {
            get { return seleccionIndice; }
            set { seleccionIndice = value; OnPropertyChanged("SeleccionIndice"); }
        }
        private bool botonesVisible;
        public bool BotonesVisible
        {
            get { return botonesVisible; }
            set { botonesVisible = value; OnPropertyChanged("BotonesVisible"); }
        }
        private bool filtrarVisible;
        public bool FiltrarVisible
        {
            get { return filtrarVisible; }
            set { filtrarVisible = value; OnPropertyChanged("FiltrarVisible"); }
        }
        private bool detallePedidoVisible;
        public bool DetallePedidoVisible
        {
            get { return detallePedidoVisible; }
            set { detallePedidoVisible = value; OnPropertyChanged("DetallePedidoVisible"); }
        }
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
        public string Name
        {
            get
            {
                return "almacen_entrada_traspasos";
            }
        }
        #endregion

        #region metodos
        void IPageViewModel.inicializa()
        { }
        private async void clickSwitch(Object obj)
        {
            var metro = Application.Current.Windows[0] as MetroWindow;
            switch (obj.ToString())
            {
                case "boton_quitar_pedido_traspaso":
                    //var metro = Application.Current.Windows[0] as MetroWindow;
                    if (_selectedItem != null)
                    {
                        var mySettings = new MetroDialogSettings()
                        {
                            AffirmativeButtonText = "Aceptar",
                            NegativeButtonText = "Cancelar",
                            AnimateShow = true,
                            AnimateHide = false
                        };
                        var result = await metro.ShowMessageAsync("Borrar", "¿Está seguro que desea borrar esta entrada? [ " + _selectedItem.Pedido +
                            ", " + _selectedItem.Almacen + ", " + _selectedItem.Fecha + " ]", MessageDialogStyle.AffirmativeAndNegative, mySettings);
                        if (result == MessageDialogResult.Affirmative)
                        {
                            var i = Lista1.IndexOf(_selectedItem);
                            if (i >= 0)
                            {
                                Lista1.RemoveAt(i);

                                var dialog = (BaseMetroDialog)metro.Resources["ConfirmacionDialog"];
                                await metro.ShowMetroDialogAsync(dialog);
                                await TaskEx.Delay(1500);
                                await metro.HideMetroDialogAsync(dialog);

                                DetallePedidoVisible = false;
                                FiltrarVisible = true;
                                BotonesVisible = false;
                                SeleccionIndice = -1;

                                //MENSAJE EXTERNO
                                //dialog = dialog.ShowDialogExternally();
                                //await TaskEx.Delay(1500);
                                //await dialog.RequestCloseAsync();
                            }
                        }
                    }
                    else
                        await metro.ShowMessageAsync("Validación", "Debe seleccionar una opcion");
                    break;
                case "boton_aceptar_pedido_traspaso":
                    if (_selectedItem != null)
                    {
                        var mySettings = new MetroDialogSettings()
                        {
                            AffirmativeButtonText = "Aceptar",
                            NegativeButtonText = "Cancelar",
                            AnimateShow = true,
                            AnimateHide = false
                        };
                        var result = await metro.ShowMessageAsync("Aceptar Traspaso", "¿Está seguro que desea aceptar esta entrada? [ " + _selectedItem.Pedido +
                            ", " + _selectedItem.Almacen + ", " + _selectedItem.Fecha + " ]", MessageDialogStyle.AffirmativeAndNegative, mySettings);
                        if (result == MessageDialogResult.Affirmative)
                        {
                            var i = Lista1.IndexOf(_selectedItem);
                            if (i >= 0)
                            {
                                Lista1.RemoveAt(i);

                                var dialog = (BaseMetroDialog)metro.Resources["ConfirmacionDialog"];
                                await metro.ShowMetroDialogAsync(dialog);
                                await TaskEx.Delay(1500);
                                await metro.HideMetroDialogAsync(dialog);

                                DetallePedidoVisible = false;
                                FiltrarVisible = true;
                                BotonesVisible = false;
                                SeleccionIndice = -1;

                                //MENSAJE EXTERNO
                                //dialog = dialog.ShowDialogExternally();
                                //await TaskEx.Delay(1500);
                                //await dialog.RequestCloseAsync();
                            }
                        }
                    }
                    else
                        await metro.ShowMessageAsync("Validación", "Debe seleccionar una opcion");
                    break;
                case "boton_filtrar_traspasos":
                    DetallePedidoVisible = false;
                    FiltrarVisible = true;
                    BotonesVisible = false;
                    SeleccionIndice = -1;
                    break;
                case "click_doble":
                    DetallePedidoVisible = true;
                    FiltrarVisible = true;
                    if (_selectedItem.Pedido == "4023")
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
        //private ICommand _onDoubleClick;
        //public ICommand OnDoubleClick
        //{
        //    get
        //    {
        //        return _onDoubleClick ?? (_onDoubleClick = new RelayCommand(clickSwitch));
        //    }
        //}
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
    public class DetalleTraspaso : ValidationViewModelBase
    {
        public DetalleTraspaso() { }

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }
        private string producto;
        public string Producto
        {
            get { return producto; }
            set
            {
                producto = value;
                OnPropertyChanged("Producto");
            }
        }
        private string unidadMedida;
        public string UnidadMedida
        {
            get { return unidadMedida; }
            set
            {
                unidadMedida = value;
                OnPropertyChanged("UnidadMedida");
            }
        }
        private string cantidad;
        public string Cantidad
        {
            get { return cantidad; }
            set
            {
                cantidad = value;
                OnPropertyChanged("Cantidad");
            }
        }

    }
    public class TraspasoTest : ValidationViewModelBase
    {
        public TraspasoTest() { }

        private string fecha;
        public string Fecha
        {
            get { return fecha; }
            set
            {
                fecha = value;
                OnPropertyChanged("Fecha");
            }
        }
        private string almacen;
        public string Almacen
        {
            get { return almacen; }
            set
            {
                almacen = value;
                OnPropertyChanged("Almacen");
            }
        }
        private string pedido;
        public string Pedido
        {
            get { return pedido; }
            set
            {
                pedido = value;
                OnPropertyChanged("Pedido");
            }
        }
    }
}
