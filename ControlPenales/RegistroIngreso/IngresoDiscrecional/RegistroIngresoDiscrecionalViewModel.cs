using ControlPenales;

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
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;

namespace ControlPenales
{
    class RegistroIngresoDiscrecionalViewModel : ValidationViewModelBase, IPageViewModel
    {
        /***************************************************************/
        private cEstadoCivil objEstadoCivil = new cEstadoCivil();
        private cOcupacion objOcupacion = new cOcupacion();
        private cEscolaridad objEscoladidad = new cEscolaridad();
        private cReligion objReligion = new cReligion();
        private cEtnia objEtnia = new cEtnia();
        private cPaises objPais = new cPaises();
        private cEntidad objEntidad = new cEntidad();
        private cMunicipio objMunicipio = new cMunicipio();

        /***************************************************************/

        #region constructor
        public RegistroIngresoDiscrecionalViewModel()
        {
        }

        void IPageViewModel.inicializa()
        {
            TabVisible = false;
            DatosIngresoVisible = true;
            IngresosVisible = false;
            DatosCausaPenalVisible = false;
            AgregarCausaPenalVisible = false;
            DiscrecionalVisible = false;
            HeaderRegistro = "Registro Discrecional";
            TrasladoEnabled = false;
            IdentificacionEnabled = true;
            IngresoEnabled = true;
            TabIdentificacion = false;
            TabTraslado = false;
            TabIngreso = true;
            TabApodosAlias = false;
            TabDatosGenerales = true;
            TabFotosHuellas = false;
            TabMediaFiliacion = false;
            TabSenasParticulares = false;
            TabPandillas = false;
            DatosGeneralesEnabled = true;//false;
            ApodosAliasEnabled = true;//false;
            FotosHuellasEnabled = true; //false;
            MediaFiliacionEnabled = true; //false;
            SenasParticularesEnabled = true; //false;
            PandillasEnabled = true;//false;
            /*********************************************/
            //inicializamos las listas
            ListEstadoCivil =  new ObservableCollection<ESTADO_CIVIL>();
            ListOcupacion =  new ObservableCollection<OCUPACION>();
            ListEscolaridad =  new ObservableCollection<ESCOLARIDAD>();
            ListReligion =  new ObservableCollection<RELIGION>();
            ListEtnia = new ObservableCollection<ETNIA>();
            ListPaisNacionalidad = new ObservableCollection<PAIS_NACIONALIDAD>();
            ListEntidad = new ObservableCollection<ENTIDAD>();
            ListMunicipio = new ObservableCollection<MUNICIPIO>();
            //
            SelectPais = new PAIS_NACIONALIDAD();
            SelectEntidad = new ENTIDAD();
            SelectMunicipio = new MUNICIPIO();

            this.getAllComboBox();

            /**********************************************/
                
        }


        private ObservableCollection<ESTADO_CIVIL> listEstadoCivil;
        public ObservableCollection<ESTADO_CIVIL> ListEstadoCivil
        {
            get { return listEstadoCivil; }
            set { listEstadoCivil = value; OnPropertyChanged("ListEstadoCivil"); }
        }

        private ObservableCollection<OCUPACION> listOcupacion;
        public ObservableCollection<OCUPACION> ListOcupacion
        {
            get { return listOcupacion; }
            set { listOcupacion = value; OnPropertyChanged("ListOcupacion"); }
        }

        private ObservableCollection<ESCOLARIDAD> listEscolaridad;
        public ObservableCollection<ESCOLARIDAD> ListEscolaridad
        {
            get { return listEscolaridad; }
            set { listEscolaridad = value; OnPropertyChanged("ListEscolaridad"); }
        }

        private ObservableCollection<RELIGION> listReligion;
        public ObservableCollection<RELIGION> ListReligion
        {
            get { return listReligion; }
            set { listReligion = value; OnPropertyChanged("ListReligion"); }
        }

        private ObservableCollection<ETNIA> listEtnia;
        public ObservableCollection<ETNIA> ListEtnia
        {
            get { return listEtnia; }
            set { listEtnia = value; OnPropertyChanged("ListReligion"); }
        }

        //DOMICILIO
        private ObservableCollection<PAIS_NACIONALIDAD> listPaisNacionalidad;
        public ObservableCollection<PAIS_NACIONALIDAD> ListPaisNacionalidad
        {
            get { return listPaisNacionalidad; }
            set { listPaisNacionalidad = value; OnPropertyChanged("ListPaisNacionalidad"); }
        }

        private ObservableCollection<ENTIDAD> listEntidad;
        public ObservableCollection<ENTIDAD> ListEntidad
        {
            get { return listEntidad; }
            set { listEntidad = value; OnPropertyChanged("ListEntidad"); }
        }

        private ObservableCollection<MUNICIPIO> listMunicipio;
        public ObservableCollection<MUNICIPIO> ListMunicipio
        {
            get { return listMunicipio; }
            set { listMunicipio = value; OnPropertyChanged("ListMunicipio"); }
        }
        
        //NACIMINETO
        private ObservableCollection<ENTIDAD> listEntidadNacimiento;
        public ObservableCollection<ENTIDAD> ListEntidadNacimiento
        {
            get { return listEntidadNacimiento; }
            set { listEntidadNacimiento = value; OnPropertyChanged("ListEntidadNacimiento"); }
        }

        private ObservableCollection<MUNICIPIO> listMunicipioNacimiento;
        public ObservableCollection<MUNICIPIO> ListMunicipioNacimiento
        {
            get { return listMunicipioNacimiento; }
            set { listMunicipioNacimiento = value; OnPropertyChanged("ListMunicipioNacimiento"); }
        }
        
        //DOMICILIO
        private PAIS_NACIONALIDAD selectPais;
        public PAIS_NACIONALIDAD SelectPais
        {
            get { return selectPais; }
            set { 
                selectPais = value;
                this.getEstado();
                OnPropertyChanged("SelectPais"); 
            }
        }

        private ENTIDAD selectEntidad;
        public ENTIDAD SelectEntidad
        {
            get { return selectEntidad; }
            set { 
                selectEntidad = value;
                this.getMunicipio();
                OnPropertyChanged("SelectEntidad"); 
            }
        }

        private MUNICIPIO selectMunicipio;
        public MUNICIPIO SelectMunicipio
        {
            get { return selectMunicipio; }
            set { selectMunicipio = value; OnPropertyChanged("SelectMunicipio"); }
        }

        //NACIMIENTO
        private PAIS_NACIONALIDAD selectPaisNacimineto;
        public PAIS_NACIONALIDAD SelectPaisNacimiento
        {
            get { return selectPaisNacimineto; }
            set
            {
                selectPaisNacimineto = value;
                this.getEstadoNacimiento();
                OnPropertyChanged("SelectPaisNacimiento");
            }
        }

        private ENTIDAD selectEntidadNacimiento;
        public ENTIDAD SelectEntidadNacimiento
        {
            get { return selectEntidadNacimiento; }
            set
            {
                selectEntidadNacimiento = value;
                this.getMunicipioNacimiento();
                OnPropertyChanged("SelectEntidadNacimiento");
            }
        }

        #endregion

        #region variables

        #region tabEnabledsIdentifiacion
        private bool datosGeneralesEnabled;
        public bool DatosGeneralesEnabled
        {
            get { return datosGeneralesEnabled; }
            set { datosGeneralesEnabled = value; OnPropertyChanged("DatosGeneralesEnabled"); }
        }
        private bool apodosAliasEnabled;
        public bool ApodosAliasEnabled
        {
            get { return apodosAliasEnabled; }
            set { apodosAliasEnabled = value; OnPropertyChanged("ApodosAliasEnabled"); }
        }
        private bool fotosHuellasEnabled;
        public bool FotosHuellasEnabled
        {
            get { return fotosHuellasEnabled; }
            set { fotosHuellasEnabled = value; OnPropertyChanged("FotosHuellasEnabled"); }
        }
        private bool mediaFiliacionEnabled;
        public bool MediaFiliacionEnabled
        {
            get { return mediaFiliacionEnabled; }
            set { mediaFiliacionEnabled = value; OnPropertyChanged("MediaFiliacionEnabled"); }
        }
        private bool senasParticularesEnabled;
        public bool SenasParticularesEnabled
        {
            get { return senasParticularesEnabled; }
            set { senasParticularesEnabled = value; OnPropertyChanged("SenasParticularesEnabled"); }
        }
        private bool pandillasEnabled;
        public bool PandillasEnabled
        {
            get { return pandillasEnabled; }
            set { pandillasEnabled = value; OnPropertyChanged("PandillasEnabled"); }
        }
        #endregion

        #region tabSelectedsIdentificacion
        private bool tabDatosGenerales;
        public bool TabDatosGenerales
        {
            get { return tabDatosGenerales; }
            set { tabDatosGenerales = value; OnPropertyChanged("TabDatosGenerales"); }
        }
        private bool tabApodosAlias;
        public bool TabApodosAlias
        {
            get { return tabApodosAlias; }
            set { tabApodosAlias = value; OnPropertyChanged("TabApodosAlias"); }
        }
        private bool tabFotosHuellas;
        public bool TabFotosHuellas
        {
            get { return tabFotosHuellas; }
            set { tabFotosHuellas = value; OnPropertyChanged("TabFotosHuellas"); }
        }
        private bool tabMediaFiliacion;
        public bool TabMediaFiliacion
        {
            get { return tabMediaFiliacion; }
            set { tabMediaFiliacion = value; OnPropertyChanged("TabMediaFiliacion"); }
        }
        private bool tabSenasParticulares;
        public bool TabSenasParticulares
        {
            get { return tabSenasParticulares; }
            set { tabSenasParticulares = value; OnPropertyChanged("TabSenasParticulares"); }
        }
        private bool tabPandillas;
        public bool TabPandillas
        {
            get { return tabPandillas; }
            set { tabPandillas = value; OnPropertyChanged("TabPandillas"); }
        }
        #endregion

        #region tabEnableds
        private bool trasladoEnabled;
        public bool TrasladoEnabled
        {
            get { return trasladoEnabled; }
            set { trasladoEnabled = value; OnPropertyChanged("TrasladoEnabled"); }
        }
        private bool identificacionEnabled;
        public bool IdentificacionEnabled
        {
            get { return identificacionEnabled; }
            set { identificacionEnabled = value; OnPropertyChanged("IdentificacionEnabled"); }
        }
        private bool ingresoEnabled;
        public bool IngresoEnabled
        {
            get { return ingresoEnabled; }
            set { ingresoEnabled = value; OnPropertyChanged("IngresoEnabled"); }
        }
        #endregion

        #region tabSelecteds
        private bool tabTraslado;
        public bool TabTraslado
        {
            get { return tabTraslado; }
            set { tabTraslado = value; OnPropertyChanged("TabTraslado"); }
        }
        private bool tabIdentificacion;
        public bool TabIdentificacion
        {
            get { return tabIdentificacion; }
            set { tabIdentificacion = value; OnPropertyChanged("TabIdentificacion"); }
        }
        private bool tabIngreso;
        public bool TabIngreso
        {
            get { return tabIngreso; }
            set { tabIngreso = value; OnPropertyChanged("TabIngreso"); }
        }
        #endregion

        #region Visibles
        private bool discrecionalVisible;
        public bool DiscrecionalVisible
        {
            get { return discrecionalVisible; }
            set { discrecionalVisible = value; OnPropertyChanged("DiscrecionalVisible"); }
        }
        private bool agregarCausaPenalVisible;
        public bool AgregarCausaPenalVisible
        {
            get { return agregarCausaPenalVisible; }
            set { agregarCausaPenalVisible = value; OnPropertyChanged("AgregarCausaPenalVisible"); }
        }
        private bool datosCausaPenalVisible;
        public bool DatosCausaPenalVisible
        {
            get { return datosCausaPenalVisible; }
            set { datosCausaPenalVisible = value; OnPropertyChanged("DatosCausaPenalVisible"); }
        }
        private bool popupBuscarDelitoVisible;
        public bool PopupBuscarDelitoVisible
        {
            get { return popupBuscarDelitoVisible; }
            set { popupBuscarDelitoVisible = value; OnPropertyChanged("PopupBuscarDelitoVisible"); }
        }
        private bool datosIngresoVisible;
        public bool DatosIngresoVisible
        {
            get { return datosIngresoVisible; }
            set { datosIngresoVisible = value; OnPropertyChanged("DatosIngresoVisible"); }
        }
        private bool ingresosVisible;
        public bool IngresosVisible
        {
            get { return ingresosVisible; }
            set { ingresosVisible = value; OnPropertyChanged("IngresosVisible"); }
        }
        private bool discrecionVisible;
        public bool DiscrecionVisible
        {
            get { return discrecionVisible; }
            set { discrecionVisible = value; OnPropertyChanged("DiscrecionVisible"); }
        }
        private bool tabVisible;
        public bool TabVisible
        {
            get { return tabVisible; }
            set { tabVisible = value; OnPropertyChanged("TabVisible"); }
        }
        #endregion

        private string headerRegistro;
        public string HeaderRegistro
        {
            get { return headerRegistro; }
            set { headerRegistro = value; OnPropertyChanged("HeaderRegistro"); }
        }
        private bool bandera_buscar = false;
        public string Name
        {
            get
            {
                return "registro_ingreso_interno_discrecional";
            }
        }
        #endregion

        #region metodos
      
        private void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "buscar_visible":
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    TabVisible = false;
                    DiscrecionVisible = false;
                    AgregarCausaPenalVisible = false;
                    break;
                case "buscar_salir":
                    if (bandera_buscar)
                    {
                        TabVisible = false;
                        DiscrecionVisible = true;
                        AgregarCausaPenalVisible = false;
                    }
                    else
                    {
                        TabVisible = false;
                        DiscrecionVisible = false;
                        AgregarCausaPenalVisible = false;
                    }
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "buscar_seleccionar":
                    if (bandera_buscar)
                    {
                        TabVisible = false;
                        DiscrecionVisible = true;
                        AgregarCausaPenalVisible = false;
                    }
                    else
                    {
                        TabVisible = true;
                        DiscrecionVisible = false;
                        AgregarCausaPenalVisible = true;
                    }
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "agregar_causa_penal":
                    TabVisible = false;
                    DiscrecionVisible = true;
                    AgregarCausaPenalVisible = false;
                    bandera_buscar = true;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "ingresos_discrecionales":
                    TabVisible = false;
                    DiscrecionVisible = true;
                    DatosIngresoVisible = false;
                    IngresosVisible = true;
                    AgregarCausaPenalVisible = false;
                    //bandera_buscar = true;
                    //CHECAR SI YA SE GUARDO LA CAUSA PENAL CON ingreso, documentacion, ubicacion y compurgacion
                    //SI YA LOS TIENE MOSTRAR EL  IngresosVisible; SI NO, MOSTRAR MENSAJE DE QUE HACEN FALTA GUARDAR DATOS
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "seleccionar_delito_buscar":
                    PopupBuscarDelitoVisible = false;
                    break;
                case "cancelar_seleccionar_delito":
                    PopupBuscarDelitoVisible = false;
                    break;
                case "insertar_delito":
                    PopupBuscarDelitoVisible = true;
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
    
    
    
    
    private void getAllComboBox()
    {
        ListEstadoCivil.Clear();
        ListEstadoCivil = new ObservableCollection<ESTADO_CIVIL>(objEstadoCivil.ObtenerTodos());
        
        ListOcupacion.Clear();
        ListOcupacion = new ObservableCollection<OCUPACION>(objOcupacion.ObtenerTodos());
        
        ListEscolaridad.Clear();
        ListEscolaridad =new ObservableCollection<ESCOLARIDAD>( objEscoladidad.ObtenerTodos());
        
        ListReligion.Clear();
        ListReligion = new ObservableCollection<RELIGION>( objReligion.ObtenerTodos());

        ListEtnia.Clear();
        ListEtnia = new ObservableCollection<ETNIA>(objEtnia.ObtenerTodos());

        ListPaisNacionalidad.Clear();
        ListPaisNacionalidad =new ObservableCollection<PAIS_NACIONALIDAD>(objPais.ObtenerTodos());
    }

    //DOMICILIO
    private void getEstado()
    {
        ListEntidad = null;
        ListEntidad = new ObservableCollection<ENTIDAD>(SelectPais.ENTIDAD);
    }
    private void getMunicipio() 
    {
        ListMunicipio = null;
        ListMunicipio = new ObservableCollection<MUNICIPIO>(SelectEntidad.MUNICIPIO);
    }
    //NACIMIENTO
    private void getEstadoNacimiento()
    {
        ListEntidadNacimiento = null;
        ListEntidadNacimiento = new ObservableCollection<ENTIDAD>(SelectPaisNacimiento.ENTIDAD);
    }
    private void getMunicipioNacimiento()
    {
        ListMunicipioNacimiento = null;
        ListMunicipioNacimiento = new ObservableCollection<MUNICIPIO>(SelectEntidadNacimiento.MUNICIPIO);
    }
    

    }
}
