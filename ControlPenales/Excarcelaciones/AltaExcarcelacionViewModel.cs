using ControlPenales;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    class AltaExcarcelacionViewModel : ValidationViewModelBase, IPageViewModel
    {
        #region variables
        private bool botonesEnables;
        public bool BotonesEnables
        {
            get { return botonesEnables; }
            set { botonesEnables = value; OnPropertyChanged("BotonesEnables"); }
        }
        private bool insertarVisible;
        public bool InsertarVisible
        {
            get { return insertarVisible; }
            set { insertarVisible = value; OnPropertyChanged("InsertarVisible"); }
        }
        private bool excarcelacionVisible;
        public bool ExcarcelacionVisible
        {
            get { return excarcelacionVisible; }
            set { excarcelacionVisible = value; OnPropertyChanged("ExcarcelacionVisible"); }
        }

        private int test;

        public int Test//Asi utilizariamos el evento onchange
        {
            get { return test; }
            set
            {
                if (test != value)
                {
                    test = value;
                    OnPropertyChanged("Test");
                }
            }
        }

        public string Name
        {
            get
            {
                return "alta_excarcelacion";
            }
        }
        #endregion

        #region constructor
        public AltaExcarcelacionViewModel()
        {
            BotonesEnables = true;
            InsertarVisible = true;
            ExcarcelacionVisible = false;
            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
        }
        #endregion

        #region metodos
        void IPageViewModel.inicializa()
        { }
        private void onClickSwitch(object obj)
        {
            switch (obj.ToString())
            {
                case "insertar_menu":
                    ExcarcelacionVisible = true;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "buscar_menu":
                    InsertarVisible = false;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "buscar_salir":
                    InsertarVisible = true;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "buscar_seleccionar":
                    InsertarVisible = true;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
            }
        }
        #endregion

        #region commands
        private ICommand _onClick;
        public ICommand OnClick
        {
            get
            {
                return _onClick ?? (_onClick = new RelayCommand(onClickSwitch));
            }
        }
        #endregion
    }
}
