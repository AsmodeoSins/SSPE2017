using ControlPenales.Clases.Estatus;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ControlPenales
{
    partial class UnidadReceptoraViewModel
    {

        #region Direccion
        //private ObservableCollection<PAIS_NACIONALIDAD> lstPais;
        //public ObservableCollection<PAIS_NACIONALIDAD> LstPais
        //{
        //    get { return lstPais; }
        //    set { lstPais = value; OnPropertyChanged("LstPais"); }
        //}
        //private PAIS_NACIONALIDAD selectedPais;
        //public PAIS_NACIONALIDAD SelectedPais
        //{
        //    get { return selectedPais; }
        //    set { selectedPais = value;
        //    LstEstado = new ObservableCollection<ENTIDAD>(value.ENTIDAD);
        //    LstMunicipio = new ObservableCollection<MUNICIPIO>();
        //    LstColonia = new ObservableCollection<COLONIA>();
        //    LstEstado.Insert(0, new ENTIDAD() { ID_ENTIDAD = -1, DESCR = "SELECCIONE" });
        //    LstMunicipio.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
        //    LstColonia.Insert(0, new COLONIA() { ID_COLONIA = -1, DESCR = "SELECCIONE" });
        //    OnPropertyChanged("SelectedPais"); }
        //}
        //private short pais;
        //public short Pais
        //{
        //    get { return pais; }
        //    set { pais = value; OnPropertyChanged("Pais"); }
        //}

        private ObservableCollection<ENTIDAD> lstEstado;
        public ObservableCollection<ENTIDAD> LstEstado
        {
            get { return lstEstado; }
            set { lstEstado = value; OnPropertyChanged("LstEstado"); }
        }
        private ENTIDAD selectedEstado;
        public ENTIDAD SelectedEstado
        {
            get { return selectedEstado; }
            set { selectedEstado = value;
            if (value != null)
                LstMunicipio = new ObservableCollection<MUNICIPIO>(value.MUNICIPIO);
            else
                LstMunicipio = new ObservableCollection<MUNICIPIO>();
            LstColonia = new ObservableCollection<COLONIA>();
            LstMunicipio.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
            LstColonia.Insert(0, new COLONIA() { ID_COLONIA = -1, DESCR = "SELECCIONE" });
            OnPropertyChanged("SelectedEstado"); }
        }
      

        private ObservableCollection<MUNICIPIO> lstMunicipio;
        public ObservableCollection<MUNICIPIO> LstMunicipio
        {
            get { return lstMunicipio; }
            set { lstMunicipio = value; 
                
                OnPropertyChanged("LstMunicipio"); }
        }
        private MUNICIPIO selectedMunicipio;
        public MUNICIPIO SelectedMunicipio
        {
            get { return selectedMunicipio; }
            set { selectedMunicipio = value;

            if (value != null)
                LstColonia = new ObservableCollection<COLONIA>(value.COLONIA);
            else
                LstColonia = new ObservableCollection<COLONIA>();
            LstColonia.Insert(0, new COLONIA() { ID_COLONIA = -1, DESCR = "SELECCIONE" });
            OnPropertyChanged("SelectedMunicipio"); }
        }
     
        private ObservableCollection<COLONIA> lstColonia;
        public ObservableCollection<COLONIA> LstColonia
        {
            get { return lstColonia; }
            set { lstColonia = value; OnPropertyChanged("LstColonia"); }
        }
        
        #endregion

        #region Unidad Receptora
        private ObservableCollection<UNIDAD_RECEPTORA> lstUnidadReceptora;
        public ObservableCollection<UNIDAD_RECEPTORA> LstUnidadReceptora
        {
            get { return lstUnidadReceptora; }
            set { lstUnidadReceptora = value;
            if (value == null)
                EmptyVisible = true;
            else
            {
                EmptyVisible = lstUnidadReceptora.Count > 0 ? false : true;
            }
                OnPropertyChanged("LstUnidadReceptora"); }
        }

        private UNIDAD_RECEPTORA _UnidadReceptora;
        public UNIDAD_RECEPTORA UnidadReceptora
        {
            get { return _UnidadReceptora; }
            set {
                _UnidadReceptora = value; 
                if(value == null)
                {
                    EstatusUR = "A";
                    NombreUR = DescripcionUR = CalleUR = NoInteriorUR = NoExteriorUR = CPUR = TelefonoUR = string.Empty;
                    EntidadUR = MunicipioUR  = -1;
                    ColoniaUR = -1;
                    EliminarMenuEnabled = false;
                    EditarMenuEnabled = false;
                }
                else
                {
                    EditarMenuEnabled = true;
                    NombreUR = value.NOMBRE;
                    DescripcionUR = value.DESCRIPCION;
                    EntidadUR = value.ID_ENTIDAD.HasValue ? value.ID_ENTIDAD : -1;
                    MunicipioUR = value.ID_MUNICIPIO.HasValue ? value.ID_MUNICIPIO : -1;
                    ColoniaUR = value.ID_COLONIA.HasValue ? value.ID_COLONIA : -1;
                    CalleUR = value.CALLE_DIRECCION;
                    NoInteriorUR = value.NUM_INT_DIRECCION;
                    NoExteriorUR = value.NUM_EXT_DIRECCION;
                    CPUR = value.CP_DIRECCION;
                    TelefonoUR = value.TELEFONO != null ? value.TELEFONO.ToString() : string.Empty;
                    EstatusUR = !string.IsNullOrEmpty(value.ESTATUS) ? value.ESTATUS : "A";
                    LstResponsables = new ObservableCollection<UNIDAD_RECEPTORA_RESPONSABLE>(new cUnidadReceptoraReponsable().ObtenerTodos((int)value.ID_UNIDAD_RECEPTORA));
                }
                OnPropertyChanged("UnidadReceptora"); }
        }

        private string nombreUR;
        public string NombreUR
        {
            get { return nombreUR; }
            set {

                if (UnidadReceptora != null)
                    nombreUR = UnidadReceptora.NOMBRE = value;
                else
                    nombreUR = value;
                OnPropertyChanged("NombreUR"); }
        }
        private string descripcionUR;
        public string DescripcionUR
        {
            get { return descripcionUR; }
            set
            {
            if (UnidadReceptora != null)
                descripcionUR = UnidadReceptora.DESCRIPCION = value;
            else
                descripcionUR = value;
                OnPropertyChanged("DescripcionUR"); }
        }

        private short? entidadUR = -1;
        public short? EntidadUR
        {
            get { return entidadUR; }
            set { 
            if (UnidadReceptora != null)
                entidadUR = UnidadReceptora.ID_ENTIDAD = value;
            else
                entidadUR = value;
                OnPropertyChanged("EntidadUR"); }
        }

        private short? municipioUR = -1;
        public short? MunicipioUR
        {
            get { return municipioUR; }
            set { 
            if (UnidadReceptora != null)
                municipioUR = UnidadReceptora.ID_MUNICIPIO = value;
            else
                municipioUR = value;
                
                OnPropertyChanged("MunicipioUR"); }
        }

        private int? coloniaUR = -1;
        public int? ColoniaUR
        {
            get { return coloniaUR; }
            set { 
            if (UnidadReceptora != null)
                coloniaUR = UnidadReceptora.ID_COLONIA = value;
            else
                coloniaUR = value;
                
                OnPropertyChanged("ColoniaUR"); }
        }

        private string calleUR;
        public string CalleUR
        {
            get { return calleUR; }
            set { 
            if (UnidadReceptora != null)
                calleUR = UnidadReceptora.CALLE_DIRECCION = value;
            else
                calleUR = value;
            
                OnPropertyChanged("CalleUR"); }
        }

        private string noInteriorUR;
        public string NoInteriorUR
        {
            get { return noInteriorUR; }
            set { noInteriorUR = value;
            if (UnidadReceptora != null)
                noInteriorUR = UnidadReceptora.NUM_INT_DIRECCION = value;
            else
                noInteriorUR = value;
                
                OnPropertyChanged("NoInteriorUR"); }
        }

        private string noExteriorUR;
        public string NoExteriorUR
        {
            get { return noExteriorUR; }
            set { 
            if (UnidadReceptora != null)
                noExteriorUR = UnidadReceptora.NUM_EXT_DIRECCION = value;
            else
                noExteriorUR = value;
                OnPropertyChanged("NoExteriorUR"); }
        }

        private string _CPUR;
        public string CPUR
        {
            get { return _CPUR; }
            set { 
            if (UnidadReceptora != null)
                _CPUR = UnidadReceptora.CP_DIRECCION = value;
            else
                _CPUR = value;
                
                OnPropertyChanged("CPUR"); }
        }

        private string telefonoUR;
        public string TelefonoUR
        {
            get { 
                if (telefonoUR == null)
                    return string.Empty;
                return new Converters().MascaraTelefono(telefonoUR);
            }
            set {
                if (UnidadReceptora != null)
                telefonoUR  = value;
            else
                telefonoUR = value;
                OnPropertyChanged("TelefonoUR"); }
        }

        private string estatusUR = "A";
        public string EstatusUR
        {
            get { return estatusUR; }
            set { 
                 if (UnidadReceptora != null)
                estatusUR = UnidadReceptora.ESTATUS = value;
            else
                estatusUR = value;
                OnPropertyChanged("EstatusUR"); }
        }
        #endregion

        #region Responsables
        private ObservableCollection<UNIDAD_RECEPTORA_RESPONSABLE> lstResponsables;
        public ObservableCollection<UNIDAD_RECEPTORA_RESPONSABLE> LstResponsables
        {
            get { return lstResponsables; }
            set { lstResponsables = value; OnPropertyChanged("LstResponsables"); }
        }

        private UNIDAD_RECEPTORA_RESPONSABLE responsable;
        public UNIDAD_RECEPTORA_RESPONSABLE Responsable
        {
            get { return responsable; }
            set { responsable = value; 
                 if (value == null)
                {
                   PaternoAlias = MaternoAlias = NombreAlias = string.Empty;
                }
                else
                {
                    PaternoAlias = value.PATERNO;
                    MaternoAlias = value.MATERNO;
                    NombreAlias = value.NOMBRE;
                }
                OnPropertyChanged("Responsable"); }
        }

        private string paternoAlias;
        public string PaternoAlias
        {
            get { return paternoAlias; }
            set {
                if (Responsable != null)
                    paternoAlias = responsable.PATERNO = value;
                else
                    paternoAlias = value;
                OnPropertyChanged("PaternoAlias"); }
        }

        private string maternoAlias;
        public string MaternoAlias
        {
            get { return maternoAlias; }
            set {

                if (Responsable != null)
                    maternoAlias = Responsable.MATERNO = value;
                else
                    maternoAlias = value;
                OnPropertyChanged("MaternoAlias"); }
        }

        private string nombreAlias;
        public string NombreAlias
        {
            get { return nombreAlias; }
            set {  
                if(Responsable != null)
                    nombreAlias = responsable.NOMBRE = value;
                else
                    nombreAlias = value;
                OnPropertyChanged("NombreAlias"); }
        }

        private bool EditaResponsable = false;
        #endregion

        #region Buscar
        private string _busqueda;
        public string Busqueda
        {
            get { return _busqueda; }
            set { _busqueda = value; OnPropertyChanged("Busqueda"); }
        }

        private short pais = Parametro.PAIS;
        #endregion

        #region Pantalla
        private bool _emptyVisible;
        public bool EmptyVisible
        {
            get { return _emptyVisible; }
            set { _emptyVisible = value; OnPropertyChanged("EmptyVisible"); }
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

        private bool _guardarMenuEnabled;
        public bool GuardarMenuEnabled
        {
            get { return _guardarMenuEnabled; }
            set { _guardarMenuEnabled = value; OnPropertyChanged("GuardarMenuEnabled"); }
        }
        #endregion

        #region Menu
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
        #endregion

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

    }
}