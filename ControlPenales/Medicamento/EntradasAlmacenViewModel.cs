using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    class EntradasAlmacenViewModel : ValidationViewModelBase, IPageViewModel
    {
        #region constructor
        public EntradasAlmacenViewModel()
        {
            GeneralMedicamentoVisible = true;
            BuscarMedicamentoVisible = false;
            CodigoBarrasEnabled = false;
            GeneralesEnabled = false;
            RadioGeneralesCheck = false;
            RadioCodigoBarrasCheck = false;
        }
        #endregion

        #region variables

        #region Enableds
        private bool administracionEnabled;
        public bool AdministracionEnabled
        {
            get { return administracionEnabled; }
            set { administracionEnabled = value; OnPropertyChanged("AdministracionEnabled"); }
        }
        private bool formaFarmaceuticaEnabled;
        public bool FormaFarmaceuticaEnabled
        {
            get { return formaFarmaceuticaEnabled; }
            set { formaFarmaceuticaEnabled = value; OnPropertyChanged("FormaFarmaceuticaEnabled"); }
        }
        private bool marcaEnabled;
        public bool MarcaEnabled
        {
            get { return marcaEnabled; }
            set { marcaEnabled = value; OnPropertyChanged("MarcaEnabled"); }
        }
        private bool categoriaEnabled;
        public bool CategoriaEnabled
        {
            get { return categoriaEnabled; }
            set { categoriaEnabled = value; OnPropertyChanged("CategoriaEnabled"); }
        }
        private bool nombreEnabled;
        public bool NombreEnabled
        {
            get { return nombreEnabled; }
            set { nombreEnabled = value; OnPropertyChanged("NombreEnabled"); }
        }
        private bool generalesEnabled;
        public bool GeneralesEnabled
        {
            get { return generalesEnabled; }
            set { generalesEnabled = value; OnPropertyChanged("GeneralesEnabled"); }
        }
        private bool codigoBarrasEnabled;
        public bool CodigoBarrasEnabled
        {
            get { return codigoBarrasEnabled; }
            set { codigoBarrasEnabled = value; OnPropertyChanged("CodigoBarrasEnabled"); }
        }
        #endregion

        #region Visibles
        private bool generalMedicamentoVisible;
        public bool GeneralMedicamentoVisible
        {
            get { return generalMedicamentoVisible; }
            set { generalMedicamentoVisible = value; OnPropertyChanged("GeneralMedicamentoVisible"); }
        }
        private bool buscarMedicamentoVisible;
        public bool BuscarMedicamentoVisible
        {
            get { return buscarMedicamentoVisible; }
            set { buscarMedicamentoVisible = value; OnPropertyChanged("BuscarMedicamentoVisible"); }
        }
        #endregion

        #region Checkeds
        private bool radioCodigoBarrasCheck;
        public bool RadioCodigoBarrasCheck
        {
            get { return radioCodigoBarrasCheck; }
            set { radioCodigoBarrasCheck = value; OnPropertyChanged("RadioCodigoBarrasCheck"); }
        }
        private bool radioGeneralesCheck;
        public bool RadioGeneralesCheck
        {
            get { return radioGeneralesCheck; }
            set { radioGeneralesCheck = value; OnPropertyChanged("RadioGeneralesCheck"); }
        }
        private bool administracionCheck;
        public bool AdministracionCheck
        {
            get { return administracionCheck; }
            set
            {
                if (administracionCheck == false)
                    AdministracionEnabled = true;
                else
                    AdministracionEnabled = false;
                administracionCheck = value;
                OnPropertyChanged("AdministracionCheck");
            }
        }
        private bool formaFarmaceuticaCheck;
        public bool FormaFarmaceuticaCheck
        {
            get { return formaFarmaceuticaCheck; }
            set
            {
                if (formaFarmaceuticaCheck == false)
                    FormaFarmaceuticaEnabled = true;
                else
                    FormaFarmaceuticaEnabled = false;
                formaFarmaceuticaCheck = value;
                OnPropertyChanged("FormaFarmaceuticaCheck");
            }
        }
        private bool marcaCheck;
        public bool MarcaCheck
        {
            get { return marcaCheck; }
            set
            {
                if (marcaCheck == false)
                    MarcaEnabled = true;
                else
                    MarcaEnabled = false;
                marcaCheck = value;
                OnPropertyChanged("MarcaCheck");
            }
        }
        private bool categoriaCheck;
        public bool CategoriaCheck
        {
            get { return categoriaCheck; }
            set
            {
                if (categoriaCheck == false)
                    CategoriaEnabled = true;
                else
                    CategoriaEnabled = false;
                categoriaCheck = value;
                OnPropertyChanged("CategoriaCheck");
            }
        }
        private bool nombreCheck;
        public bool NombreCheck
        {
            get { return nombreCheck; }
            set
            {
                if (nombreCheck == false)
                    NombreEnabled = true;
                else
                    NombreEnabled = false;
                nombreCheck = value;
                OnPropertyChanged("NombreCheck");
            }
        }
        #endregion

        public string Name
        {
            get
            {
                return "entradas_almacen_medicamento";
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
                case "buscar_medicamento":
                    GeneralMedicamentoVisible = false;
                    BuscarMedicamentoVisible = true;
                    break;
                case "buscar_articulo_medicamento":
                    break;
                case "seleccionar_articulo_medicamento":
                    GeneralMedicamentoVisible = true;
                    BuscarMedicamentoVisible = false;
                    RadioGeneralesCheck = false;
                    RadioCodigoBarrasCheck = false;
                    CodigoBarrasEnabled = false;
                    GeneralesEnabled = false;
                    NombreCheck = false;
                    AdministracionCheck = false;
                    MarcaCheck = false;
                    CategoriaCheck = false;
                    FormaFarmaceuticaCheck = false;
                    NombreEnabled = false;
                    AdministracionEnabled = false;
                    MarcaEnabled = false;
                    CategoriaEnabled = false;
                    FormaFarmaceuticaEnabled = false;
                    break;
                case "cancelar_articulo_medicamento":
                    GeneralMedicamentoVisible = true;
                    BuscarMedicamentoVisible = false;
                    RadioGeneralesCheck = false;
                    RadioCodigoBarrasCheck = false;
                    CodigoBarrasEnabled = false;
                    GeneralesEnabled = false;
                    NombreCheck = false;
                    AdministracionCheck = false;
                    MarcaCheck = false;
                    CategoriaCheck = false;
                    FormaFarmaceuticaCheck = false;
                    NombreEnabled = false;
                    AdministracionEnabled = false;
                    MarcaEnabled = false;
                    CategoriaEnabled = false;
                    FormaFarmaceuticaEnabled = false;
                    break;
                case "generales_enabled":
                    CodigoBarrasEnabled = false;
                    GeneralesEnabled = true;
                    break;
                case "codigo_barras_enabled":
                    CodigoBarrasEnabled = true;
                    GeneralesEnabled = false;
                    NombreCheck = false;
                    AdministracionCheck = false;
                    MarcaCheck = false;
                    CategoriaCheck = false;
                    FormaFarmaceuticaCheck = false;
                    NombreEnabled = false;
                    AdministracionEnabled = false;
                    MarcaEnabled = false;
                    CategoriaEnabled = false;
                    FormaFarmaceuticaEnabled = false;
                    break;
            }
        }
        #endregion

        #region command
        private ICommand onClick;
        public ICommand OnClick
        {
            get
            {
                return onClick ?? (onClick = new RelayCommand(clickSwitch));
            }
        }
        #endregion
    }
}
