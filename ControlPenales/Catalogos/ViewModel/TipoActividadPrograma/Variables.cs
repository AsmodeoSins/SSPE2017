using ControlPenales.Clases.Estatus;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class CatalogoTipoActividadProgramaViewModel
    {

        public string Name
        {
            get
            {
                return "catalogo_tipo_actividad_programa";
            }
        }

        private string busqueda;
        public string Busqueda
        {
            get { return busqueda; }
            set { busqueda = value; OnPropertyChanged("Busqueda"); }
        }

        private short clave;
        public short Clave
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

        private string _Nombre;
        public string Nombre
        {
            get { return _Nombre; }
            set { _Nombre = value; OnPropertyChanged("Nombre"); }
        }

        private short? _Orden;
        public short? Orden
        {
            get { return _Orden; }
            set { _Orden = value; OnPropertyChanged("Orden"); }
        }

        private int _MaxLenghtOrden;
        public int MaxLenghtOrden
        {
            get { return _MaxLenghtOrden; }
            set { _MaxLenghtOrden = value; OnPropertyChanged("MaxLenghtOrden"); }
        }

        private bool emptyVisible;
        public bool EmptyVisible
        {
            get { return emptyVisible; }
            set { emptyVisible = value; OnPropertyChanged("EmptyVisible"); }
        }

        private int maxLength;
        public int MaxLength
        {
            get { return maxLength; }
            set { maxLength = value; OnPropertyChanged("MaxLength"); }
        }

        private bool focusText;
        public bool FocusText
        {
            get { return focusText; }
            set { focusText = value; OnPropertyChanged("FocusText"); }
        }

        public bool bandera_editar = false;

        private string cambio;
        public string Cambio
        {
            get { return cambio; }
            set { cambio = value; OnPropertyChanged("Cambio"); }
        }

        private string catalogHeader;
        public string CatalogoHeader
        {
            get { return catalogHeader; }
            set { catalogHeader = value; OnPropertyChanged("CatalogoHeader"); }
        }

        private string headerAgregar;
        public string HeaderAgregar
        {
            get { return headerAgregar; }
            set { headerAgregar = value; OnPropertyChanged("HeaderAgregar"); }
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

        #region [CONFIGURACION PERMISOS]
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

        private bool editarEnabled;
        public bool EditarEnabled
        {
            get { return editarEnabled; }
            set { editarEnabled = value; OnPropertyChanged("EditarEnabled"); }
        }

        private bool eliminarMenuEnabled;
        public bool EliminarMenuEnabled
        {
            get { return eliminarMenuEnabled; }
            set { eliminarMenuEnabled = value; OnPropertyChanged("EliminarMenuEnabled"); }
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

        private bool _textoHabilitado;
        public bool TextoHabilitado
        {
            get { return _textoHabilitado; }
            set{ _textoHabilitado = value;OnPropertyChanged("TextoHabilitado");}
        }

        private bool _buscarHabilitado;
        public bool BuscarHabilitado
        {
            get { return _buscarHabilitado; }
            set{ _buscarHabilitado = value;OnPropertyChanged("BuscarHabilitado");}
        }
        #endregion

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

        private bool nuevoVisible;
        public bool NuevoVisible
        {
            get { return nuevoVisible; }
            set { nuevoVisible = value; OnPropertyChanged("NuevoVisible"); }
        }

        private Estatus _selectedEstatus;
        public Estatus SelectedEstatus
        {
            get { return _selectedEstatus; }
            set { _selectedEstatus = value; OnPropertyChanged("SelectedEstatus"); }
        }

        private EstatusControl lista_estatus = new EstatusControl();
        public EstatusControl Lista_Estatus
        {
            get { return lista_estatus; }
            set { lista_estatus = value; RaisePropertyChanged("Lista_Estatus"); }
        }

        private ObservableCollection<DEPARTAMENTO> _ListaDepartamentos;
        public ObservableCollection<DEPARTAMENTO> ListaDepartamentos
        {
            get { return _ListaDepartamentos; }
            set { _ListaDepartamentos = value; OnPropertyChanged("ListaDepartamentos"); }
        }

        private DEPARTAMENTO _SelectedDepartamento;
        public DEPARTAMENTO SelectedDepartamento
        {
            get { return _SelectedDepartamento; }
            set { _SelectedDepartamento = value; OnPropertyChanged("SelectedDepartamento"); }
        }

        private int _SelectIndexDep;
        public int SelectIndexDep
        {
            get { return _SelectIndexDep; }
            set { _SelectIndexDep = value; OnPropertyChanged("SelectIndexDep"); }
        }

        private ObservableCollection<TIPO_PROGRAMA> listItems;
        public ObservableCollection<TIPO_PROGRAMA> ListItems
        {
            get { return listItems; }
            set { listItems = value; OnPropertyChanged("ListItems"); }
        }

        private TIPO_PROGRAMA selectedItem;
        public TIPO_PROGRAMA SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                if (selectedItem == null)
                {
                    EliminarMenuEnabled = false;
                    EditarMenuEnabled = false;
                }
                else
                {
                    if (EditarEnabled)
                    {
                        EditarMenuEnabled = EditarEnabled;
                    }
                }
                OnPropertyChanged("SelectedItem");
            }
        }
    }
}
