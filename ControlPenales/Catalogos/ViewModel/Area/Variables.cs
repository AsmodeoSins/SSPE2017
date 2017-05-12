using SSP.Servidor;
using System.Collections.ObjectModel;


namespace ControlPenales
{
    partial class CatalogoAreasViewModel
    {
        public string Name
        {
            get
            {
                return "catalogo_areas";
            }
        }

        private string busqueda;
        public string Busqueda
        {
            get { return busqueda; }
            set { busqueda = value; OnPropertyChanged("Busqueda"); }
        }

        private bool focusText;
        public bool FocusText
        {
            get { return focusText; }
            set { focusText = value; OnPropertyChanged("FocusText"); }
        }

        public bool bandera_editar = false;

        private string filtroDisplay;
        public string FiltroDisplay
        {
            get { return filtroDisplay; }
            set { filtroDisplay = value; OnPropertyChanged("FiltroDisplay"); }
        }
        
        private string clave;
        public string Clave
        {
            get { return clave; }
            set { clave = value; OnPropertyChanged("Clave"); }
        }
        
        private string descripcion;
        public string Descripcion
        {
            get { return descripcion; }
            set { descripcion = value; OnPropertyChanged("Descripcion"); }
        }
        
        private short tipo;
        public short Tipo
        {
            get { return tipo; }
            set { tipo = value; OnPropertyChanged("Tipo"); }
        }
        
        private string catalogHeader;
        public string CatalogoHeader
        {
            get { return catalogHeader; }
            set { catalogHeader = value; }
        }
        
        private string headerAgregar;
        public string HeaderAgregar
        {
            get { return headerAgregar; }
            set { headerAgregar = value; }
        }
        
        private string comboBoxLigado;
        public string ComboBoxLigado
        {
            get { return comboBoxLigado; }
            set { comboBoxLigado = value; }
        }
        
        private int seleccionIndice;
        public int SeleccionIndice
        {
            get { return seleccionIndice; }
            set { seleccionIndice = value; OnPropertyChanged("SeleccionIndice"); }
        }
        
        private bool guardarMenuEnabled;
        public bool GuardarMenuEnabled
        {
            get { return guardarMenuEnabled; }
            set { guardarMenuEnabled = value; OnPropertyChanged("GuardarMenuEnabled"); }
        }
        
        private bool agregarMenuEnabled;
        public bool AgregarMenuEnabled
        {
            get { return agregarMenuEnabled; }
            set { agregarMenuEnabled = value; OnPropertyChanged("AgregarMenuEnabled"); }
        }
        
        private bool editarMenuEnabled;
        public bool EditarMenuEnabled
        {
            get { return editarMenuEnabled; }
            set { editarMenuEnabled = value; OnPropertyChanged("EditarMenuEnabled"); }
        }
        
        private bool eliminarMenuEnabled;
        public bool EliminarMenuEnabled
        {
            get { return eliminarMenuEnabled; }
            set { eliminarMenuEnabled = value; OnPropertyChanged("EliminarMenuEnabled"); }
        }
        
        private bool cancelarMenuEnabled;
        public bool CancelarMenuEnabled
        {
            get { return cancelarMenuEnabled; }
            set { cancelarMenuEnabled = value; OnPropertyChanged("CancelarMenuEnabled"); }
        }
        
        private bool exportarMenuEnabled;
        public bool ExportarMenuEnabled
        {
            get { return exportarMenuEnabled; }
            set { exportarMenuEnabled = value; OnPropertyChanged("ExportarMenuEnabled"); }
        }
        
        private bool salirMenuEnabled;
        public bool SalirMenuEnabled
        {
            get { return salirMenuEnabled; }
            set { salirMenuEnabled = value; OnPropertyChanged("SalirMenuEnabled"); }
        }
        
        private bool ayudaMenuEnabled;
        public bool AyudaMenuEnabled
        {
            get { return ayudaMenuEnabled; }
            set { ayudaMenuEnabled = value; OnPropertyChanged("AyudaMenuEnabled"); }
        }
        
        private bool emptyVisible;
        public bool EmptyVisible
        {
            get { return emptyVisible; }
            set { emptyVisible = value; OnPropertyChanged("EmptyVisible"); }
        }
        
        private bool filtroVisible;
        public bool FiltroVisible
        {
            get { return filtroVisible; }
            set { filtroVisible = value; OnPropertyChanged("FiltroVisible"); }
        }
        
        private bool agregarVisible;
        public bool AgregarVisible
        {
            get { return agregarVisible; }
            set { agregarVisible = value; OnPropertyChanged("AgregarVisible"); }
        }
        
        private bool editarVisible;
        public bool EditarVisible
        {
            get { return editarVisible; }
            set { editarVisible = value; OnPropertyChanged("EditarVisible"); }
        }
        
        private bool nuevoVisible;
        public bool NuevoVisible
        {
            get { return nuevoVisible; }
            set { nuevoVisible = value; OnPropertyChanged("NuevoVisible"); }
        }

        private ObservableCollection<CENTRO> listTipos;
        public ObservableCollection<CENTRO> ListTipos
        {
            get { return listTipos; }
            set { listTipos = value; OnPropertyChanged("ListTipos"); }
        }
       
        //private ObservableCollection<AREA> listItems;
        //public ObservableCollection<AREA> ListItems
        //{
        //    get { return listItems; }
        //    set { listItems = value; OnPropertyChanged("ListItems"); }
        //}
        
        //private AREA selectedItem;
        //public AREA SelectedItem
        //{
        //    get { return selectedItem; }
        //    set
        //    {
        //        selectedItem = value;
        //        OnPropertyChanged("SelectedItem");
        //        if (selectedItem == null)
        //        {
        //            EliminarMenuEnabled = false;
        //            EditarMenuEnabled = false;
        //        }
        //        else
        //        {
        //            EliminarMenuEnabled = true;
        //            EditarMenuEnabled = true;
        //        }
        //    }
        //}
        
        private CENTRO selectedTipo;
        public CENTRO SelectedTipo
        {
            get { return selectedTipo; }
            set { selectedTipo = value; OnPropertyChanged("SelectedTipo"); }
        }
    }
}