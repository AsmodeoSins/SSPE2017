using ControlPenales.Clases.Estatus;
using SSP.Servidor;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace ControlPenales
{
    partial class ProcedimientosMaterialesViewModel
    {

        public string Name
        {
            get
            {
                return "catalogo_tipo_lunar";
            }
        }

        private string _busqueda;
        public string Busqueda
        {
            get { return _busqueda; }
            set { _busqueda = value; OnPropertyChanged("Busqueda"); }
        }

        private short _clave;
        public short Clave
        {
            get { return _clave; }
            set { _clave = value; OnPropertyChanged("Clave"); }
        }

        private string _descripcion;
        public string Descripcion
        {
            get { return _descripcion; }
            set { _descripcion = value; OnPropertyChanged("Descripcion"); }
        }

        private Visibility _EmptyProcMatsVisible = Visibility.Visible;
        public Visibility EmptyProcMatsVisible
        {
            get { return _EmptyProcMatsVisible; }
            set { _EmptyProcMatsVisible = value; OnPropertyChanged("EmptyProcMatsVisible"); }
        }

        private Visibility _EmptyProcMedsVisible = Visibility.Visible;
        public Visibility EmptyProcMedsVisible
        {
            get { return _EmptyProcMedsVisible; }
            set { _EmptyProcMedsVisible = value; OnPropertyChanged("EmptyProcMedsVisible"); }
        }

        private int _maxLength;
        public int MaxLength
        {
            get { return _maxLength; }
            set { _maxLength = value; OnPropertyChanged("MaxLength"); }
        }

        private bool _focusText;
        public bool FocusText
        {
            get { return _focusText; }
            set { _focusText = value; OnPropertyChanged("FocusText"); }
        }

        public bool bandera_editar = false;

        private string _cambio;
        public string Cambio
        {
            get { return _cambio; }
            set { _cambio = value; OnPropertyChanged("Cambio"); }
        }

        private string _catalogHeader;
        public string CatalogoHeader
        {
            get { return _catalogHeader; }
            set { _catalogHeader = value; OnPropertyChanged("CatalogoHeader"); }
        }

        private string _headerAgregar;
        public string HeaderAgregar
        {
            get { return _headerAgregar; }
            set { _headerAgregar = value; OnPropertyChanged("HeaderAgregar"); }
        }

        private int _seleccionIndice;
        public int SeleccionIndice
        {
            get { return _seleccionIndice; }
            set { _seleccionIndice = value; OnPropertyChanged("SeleccionIndice"); }
        }


        private Estatus _selectedEstatus = null;
        public Estatus SelectedEstatus
        {
            get { return _selectedEstatus; }
            set { _selectedEstatus = value; OnPropertyChanged("SelectedEstatus"); }
        }

        private EstatusControl _lista_Estatus = new EstatusControl();
        public EstatusControl Lista_Estatus
        {
            get { return _lista_Estatus; }
            set { _lista_Estatus = value; RaisePropertyChanged("Lista_Estatus"); }
        }

        private bool _guardarMenuEnabled;
        public bool GuardarMenuEnabled
        {
            get { return _guardarMenuEnabled; }
            set { _guardarMenuEnabled = value; OnPropertyChanged("GuardarMenuEnabled"); }
        }

        #region [CONFIGURACION PERMISOS]
        private bool _agregarMenuEnabled;
        public bool AgregarMenuEnabled
        {
            get { return _agregarMenuEnabled; }
            set { _agregarMenuEnabled = value; OnPropertyChanged("AgregarMenuEnabled"); }
        }

        private bool _editarMenuEnabled;
        public bool EditarMenuEnabled
        {
            get { return _editarMenuEnabled; }
            set { _editarMenuEnabled = value; OnPropertyChanged("EditarMenuEnabled"); }
        }

        private bool _editarEnabled;
        public bool EditarEnabled
        {
            get { return _editarEnabled; }
            set { _editarEnabled = value; OnPropertyChanged("EditarEnabled"); }
        }

        private bool _eliminarMenuEnabled;
        public bool EliminarMenuEnabled
        {
            get { return _eliminarMenuEnabled; }
            set { _eliminarMenuEnabled = value; OnPropertyChanged("EliminarMenuEnabled"); }
        }

        private bool _agregarVisible;
        public bool AgregarVisible
        {
            get { return _agregarVisible; }
            set { _agregarVisible = value; OnPropertyChanged("AgregarVisible"); }
        }

        private bool _editarVisible;
        public bool EditarVisible
        {
            get { return _editarVisible; }
            set { _editarVisible = value; OnPropertyChanged("EditarVisible"); }
        }

        private bool _textoHabilitado;
        public bool TextoHabilitado
        {
            get { return _textoHabilitado; }
            set { _textoHabilitado = value; OnPropertyChanged("TextoHabilitado"); }
        }

        private bool _buscarHabilitado;
        public bool BuscarHabilitado
        {
            get { return _buscarHabilitado; }
            set { _buscarHabilitado = value; OnPropertyChanged("BuscarHabilitado"); }
        }
        #endregion

        private bool _cancelarMenuEnabled;
        public bool CancelarMenuEnabled
        {
            get { return _cancelarMenuEnabled; }
            set { _cancelarMenuEnabled = value; OnPropertyChanged("CancelarMenuEnabled"); }
        }

        private bool _exportarMenuEnabled;
        public bool ExportarMenuEnabled
        {
            get { return _exportarMenuEnabled; }
            set { _exportarMenuEnabled = value; OnPropertyChanged("ExportarMenuEnabled"); }
        }

        private bool _salirMenuEnabled;
        public bool SalirMenuEnabled
        {
            get { return _salirMenuEnabled; }
            set { _salirMenuEnabled = value; OnPropertyChanged("SalirMenuEnabled"); }
        }

        private bool _ayudaMenuEnabled;
        public bool AyudaMenuEnabled
        {
            get { return _ayudaMenuEnabled; }
            set { _ayudaMenuEnabled = value; OnPropertyChanged("AyudaMenuEnabled"); }
        }

        private bool _nuevoVisible;
        public bool NuevoVisible
        {
            get { return _nuevoVisible; }
            set { _nuevoVisible = value; OnPropertyChanged("NuevoVisible"); }
        }

        private List<PROC_MED> _ListProcMeds;
        public List<PROC_MED> ListProcMeds
        {
            get { return _ListProcMeds; }
            set { _ListProcMeds = value; OnPropertyChanged("ListProcMeds"); }
        }

        private PROC_MED _SelectProcMed;
        public PROC_MED SelectProcMed
        {
            get { return _SelectProcMed; }
            set
            {
                _SelectProcMed = value;
                ListProcMats = value == null ? new ObservableCollection<PROC_MATERIAL>() : new ObservableCollection<PROC_MATERIAL>(value.PROC_MATERIAL);
                EmptyProcMatsVisible = ListProcMats.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                ProceimientoMedicoSeleccionado = value != null ? value.DESCR : string.Empty;
                SubtipoSeleccionado = value != null ? value.PROC_MED_SUBTIPO.DESCR : string.Empty;
                AgregarVisible = value == null ? false : !(value != null);
                AgregarMenuEnabled = value != null;
                CancelarMenuEnabled = SelectProcMat != null;
                OnPropertyChanged("SelectProcMed");
            }
        }

        private ObservableCollection<PROC_MED_SUBTIPO> _ListSubtipos;
        public ObservableCollection<PROC_MED_SUBTIPO> ListSubtipos
        {
            get { return _ListSubtipos; }
            set { _ListSubtipos = value; OnPropertyChanged("ListSubtipos"); }
        }

        private short _SelectSubtipo;
        public short SelectSubtipo
        {
            get { return _SelectSubtipo; }
            set { _SelectSubtipo = value; OnPropertyChanged("SelectSubtipo"); }
        }

        private PROC_MED_SUBTIPO _SelectSubtipoAgregar;
        public PROC_MED_SUBTIPO SelectSubtipoAgregar
        {
            get { return _SelectSubtipoAgregar; }
            set { _SelectSubtipoAgregar = value; OnPropertyChanged("SelectSubtipoAgregar"); }
        }

        private PROC_MATERIAL _SelectProcMat;
        public PROC_MATERIAL SelectProcMat
        {
            get { return _SelectProcMat; }
            set
            {
                _SelectProcMat = value;
                if (value == null)
                {
                    EliminarMenuEnabled = false;
                    EditarMenuEnabled = false;
                }
                else
                {
                    EditarMenuEnabled = EditarEnabled;
                }
                OnPropertyChanged("SelectProcMat");
            }
        }

        private ObservableCollection<PROC_MATERIAL> _ListProcMats;
        public ObservableCollection<PROC_MATERIAL> ListProcMats
        {
            get { return _ListProcMats; }
            set { _ListProcMats = value; OnPropertyChanged("ListProcMats"); }
        }
        private ControlPenales.Controls.AutoCompleteTextBox _AutoCompleteReceta;
        public ControlPenales.Controls.AutoCompleteTextBox AutoCompleteReceta
        {
            get { return _AutoCompleteReceta; }
            set { _AutoCompleteReceta = value; }
        }
        private ListBox _AutoCompleteRecetaLB;
        public ListBox AutoCompleteRecetaLB
        {
            get { return _AutoCompleteRecetaLB; }
            set { _AutoCompleteRecetaLB = value; }
        }
        private string _ProductoSeleccionado;
        public string ProductoSeleccionado
        {
            get { return _ProductoSeleccionado; }
            set { _ProductoSeleccionado = value; OnPropertyChanged("ProductoSeleccionado"); }
        }
        private PRODUCTO SelectProducto;

        private string _SubtipoSeleccionado;
        public string SubtipoSeleccionado
        {
            get { return _SubtipoSeleccionado; }
            set { _SubtipoSeleccionado = value; OnPropertyChanged("SubtipoSeleccionado"); }
        }
        private string _ProceimientoMedicoSeleccionado;
        public string ProceimientoMedicoSeleccionado
        {
            get { return _ProceimientoMedicoSeleccionado; }
            set { _ProceimientoMedicoSeleccionado = value; OnPropertyChanged("ProceimientoMedicoSeleccionado"); }
        }
        private bool _MaterialesEnabled = false;
        public bool MaterialesEnabled
        {
            get { return _MaterialesEnabled; }
            set { _MaterialesEnabled = value; OnPropertyChanged("MaterialesEnabled"); }
        }
    }
}