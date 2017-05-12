using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ControlPenales
{
    class RequesicionAlimentoPreparadoViewModel : ValidationViewModelBase, IPageViewModel
    {

        #region constructor
        public RequesicionAlimentoPreparadoViewModel()
        {
            ProporcionesEstaticasVisible = true;
            ProporcionesEditablesVisible = false;
            Lista2 = new ObservableCollection<DetalleTraspaso>();
            Lista2.Add(new DetalleTraspaso() { Cantidad = "21", Producto = "Paracetamol", UnidadMedida = "Pastilla", IsSelected = false });
            Lista2.Add(new DetalleTraspaso() { Cantidad = "50", Producto = "Diclofenaco", UnidadMedida = "Capsula", IsSelected = false });
            Lista2.Add(new DetalleTraspaso() { Cantidad = "40", Producto = "Prueba1", UnidadMedida = "Pastilla", IsSelected = false });
            Lista2.Add(new DetalleTraspaso() { Cantidad = "60", Producto = "Prueba2", UnidadMedida = "Capsula", IsSelected = false });
            Lista2.Add(new DetalleTraspaso() { Cantidad = "34", Producto = "Prueba3", UnidadMedida = "Pastilla", IsSelected = false });
            Lista2.Add(new DetalleTraspaso() { Cantidad = "1", Producto = "Prueba4", UnidadMedida = "Capsula", IsSelected = false });
        }
        #endregion

        #region variables
        private bool focusText;
        public bool FocusText
        {
            get { return focusText; }
            set { focusText = value; OnPropertyChanged("FocusText"); }
        }
        private ObservableCollection<DetalleTraspaso> lista2;
        public ObservableCollection<DetalleTraspaso> Lista2
        {
            get { return lista2; }
            set { lista2 = value; OnPropertyChanged("Lista2"); }
        }
        private bool proporcionesEstaticasVisible;
        public bool ProporcionesEstaticasVisible
        {
            get { return proporcionesEstaticasVisible; }
            set { proporcionesEstaticasVisible = value; OnPropertyChanged("ProporcionesEstaticasVisible"); }
        }
        private bool proporcionesEditablesVisible;
        public bool ProporcionesEditablesVisible
        {
            get { return proporcionesEditablesVisible; }
            set { proporcionesEditablesVisible = value; OnPropertyChanged("ProporcionesEditablesVisible"); }
        }
        public string Name
        {
            get
            {
                return "requesicion_alimento_preparado";
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
                case "checked_item":
                    if (selectedItem.IsSelected)
                    {
                        selectedItem.IsSelected = false;
                        FocusText = false;
                    }
                    else
                    {
                        FocusText = true;
                        selectedItem.IsSelected = true;
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
        private bool checkAll;
        public bool CheckAll
        {
            get { return checkAll; }
            set
            {
                checkAll = value;
                Lista2.ToList().ForEach(x => x.IsSelected = checkAll);
                OnPropertyChanged("CheckAll");
            }
        }
        private DetalleTraspaso selectedItem;
        public DetalleTraspaso SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }
        #endregion
    }
}
