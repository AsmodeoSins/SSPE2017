using ControlPenales.BiometricoServiceReference;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace ControlPenales
{
    partial class ReportePsicologicoViewModel
    {
        #region MaxWidth
        private double  _MaxWhidthEntrevistadoRptpsicologic;

        public double MaxWhidthEntrevistadoRptpsicologic
        {
            get { return _MaxWhidthEntrevistadoRptpsicologic; }
            set { _MaxWhidthEntrevistadoRptpsicologic = value; OnPropertyChanged("MaxWhidthEntrevistadoRptpsicologic"); }
        }

        #endregion

        #region MaxLenght
        private decimal? _Lugar_MaxLenght=100;

        public decimal? Lugar_MaxLenght
        {
            get { return _Lugar_MaxLenght; }
            set { _Lugar_MaxLenght = value; OnPropertyChanged("Lugar_MaxLenght"); }
        }
        private decimal? _Hora_MaxLenght=100;

        public decimal? Hora_MaxLenght
        {
            get { return _Hora_MaxLenght; }
            set { _Hora_MaxLenght = value; OnPropertyChanged("Hora_MaxLenght"); }
        }
        private decimal? _DescrEntrevistado_MaxLenght=500;

        public decimal? DescrEntrevistado_MaxLenght
        {
            get { return _DescrEntrevistado_MaxLenght; }
            set { _DescrEntrevistado_MaxLenght = value; OnPropertyChanged("DescrEntrevistado_MaxLenght"); }
        }



        private decimal? _TecnicasUtiliz_MaxLenght = 500;

        public decimal? TecnicasUtiliz_MaxLenght
        {
            get { return _TecnicasUtiliz_MaxLenght; }
            set { _TecnicasUtiliz_MaxLenght = value; OnPropertyChanged("TecnicasUtiliz_MaxLenght"); }
        }
        private decimal? _ExamenMental_MaxLenght=500;

        public decimal? ExamenMental_MaxLenght
        {
            get { return _ExamenMental_MaxLenght; }
            set { _ExamenMental_MaxLenght = value; OnPropertyChanged("ExamenMental_MaxLenght"); }
        }

        private decimal? _Personalidad_MaxLenght=500;

        public decimal? Personalidad_MaxLenght
        {
            get { return _Personalidad_MaxLenght; }
            set { _Personalidad_MaxLenght = value; OnPropertyChanged("Personalidad_MaxLenght"); }
        }

        private decimal? _NuceloFamprimario_MaxLenght=500;

        public decimal? NuceloFamprimario_MaxLenght
        {
            get { return _NuceloFamprimario_MaxLenght; }
            set { _NuceloFamprimario_MaxLenght = value; OnPropertyChanged("NuceloFamprimario_MaxLenght"); }
        }
        private decimal? _NuceloFamsec_MaxLenght = 500;

        public decimal? NuceloFamsec_MaxLenght
        {
            get { return _NuceloFamsec_MaxLenght; }
            set { _NuceloFamsec_MaxLenght = value; OnPropertyChanged("NuceloFamsec_MaxLenght"); }
        }

        private decimal? _Observ_MaxLenght=500;

        public decimal? Observ_MaxLenght
        {
            get { return _Observ_MaxLenght; }
            set { _Observ_MaxLenght = value; OnPropertyChanged("Observ_MaxLenght"); }
        }


        private decimal? _Sugerencias_MaxLenght = 100;

        public decimal? Sugerencias_MaxLenght
        {
            get { return _Sugerencias_MaxLenght; }
            set { _Sugerencias_MaxLenght = value; OnPropertyChanged("Sugerencias_MaxLenght"); }
        }

        private decimal? _NombreFam_MaxLenght = 100;

        public decimal? NombreFam_MaxLenght
        {
            get { return _NombreFam_MaxLenght; }
            set { _NombreFam_MaxLenght = value; OnPropertyChanged("NombreFam_MaxLenght"); }
        }

        private decimal? _Domicilio_MaxLenght=100;

        public decimal? Domicilio_MaxLenght
        {
            get { return _Domicilio_MaxLenght; }
            set { _Domicilio_MaxLenght = value; OnPropertyChanged("Domicilio_MaxLenght"); }
        }

        #endregion
        
        #region Menu
        private bool menuFichaEnabled = false;
        public bool MenuFichaEnabled
        {
            get { return menuFichaEnabled; }
            set { menuFichaEnabled = value; OnPropertyChanged("MenuFichaEnabled"); }
        }

        private bool menuReporteEnabled = false;
        public bool MenuReporteEnabled
        {
            get { return menuReporteEnabled; }
            set { menuReporteEnabled = value; OnPropertyChanged("MenuReporteEnabled"); }
        }
        
        private bool menuGuardarEnabled = false;
        public bool MenuGuardarEnabled
        {
            get { return menuGuardarEnabled; }
            set { menuGuardarEnabled = value; OnPropertyChanged("MenuGuardarEnabled"); }
        }
        
        private bool menuBuscarEnabled = false;
        public bool MenuBuscarEnabled
        {
            get { return menuBuscarEnabled; }
            set { menuBuscarEnabled = value; OnPropertyChanged("MenuBuscarEnabled"); }
        }

        private bool _MenuAyudaEnabled = false;
        public bool MenuAyudaEnabled
        {
            get { return _MenuAyudaEnabled; }
            set { _MenuAyudaEnabled = value; OnPropertyChanged("MenuBuscarEnabled"); }
        }

        private System.Windows.Visibility _TabControlVisible = Visibility.Collapsed;
        public Visibility TabControlVisible
        {
            get { return _TabControlVisible; }
            set { _TabControlVisible = value; OnPropertyChanged("TabControlVisible"); }
        }
        #endregion

        #region Tab Control
        private bool _TabDatosGenerales;

        public bool TabDatosGenerales
        {
            get { return _TabDatosGenerales; }
            set {
                if (_TabDatosGenerales==false)
                {
                   
                
                  
                }
                _TabDatosGenerales = value; 
                
                OnPropertyChanged("TabDatosGenerales"); }
        }
        private bool _TabDatosPersonaEntrev;

        public bool TabDatosPersonaEntrev
        {
            get { return _TabDatosPersonaEntrev; }
            set {
                if (_TabDatosPersonaEntrev ==false)
                {
                   
                 
                }
                _TabDatosPersonaEntrev = value; 
                
                OnPropertyChanged("TabDatosPersonaEntrev"); }
        }
        #endregion

        #region Datos Entrevista
        private DateTime? _TextFechaEntrv;

        public DateTime? TextFechaEntrv
        {
            get { return _TextFechaEntrv; }
            set { _TextFechaEntrv = value; OnPropertyChanged("TextFechaEntrv"); }
        }

        private DateTime? _TextHoraEntrevista;

        public DateTime? TextHoraEntrevista
        {
            get { return _TextHoraEntrevista; }
            set { _TextHoraEntrevista = value; OnPropertyChanged("TextHoraEntrevista"); }
        }

        private string _TextLugarEntrevista;

        public string TextLugarEntrevista
        {
            get { return _TextLugarEntrevista; }
            set { _TextLugarEntrevista = value; OnPropertyChanged("TextLugarEntrevista"); }
        }

       

        #endregion

        private string _TextNombreEntrevistado;
        public string TextNombreEntrevistado
        {
            get { return _TextNombreEntrevistado; }
            set { _TextNombreEntrevistado = value; OnPropertyChanged("TextNombreEntrevistado"); }
        }

        private string _ApellidoPaternoBuscar;
        public string ApellidoPaternoBuscar
        {
            get { return _ApellidoPaternoBuscar; }
            set { _ApellidoPaternoBuscar = value; OnPropertyChanged("ApellidoPaternoBuscar"); }
        }

        private string _ApellidoMaternoBuscar;
        public string ApellidoMaternoBuscar
        {
            get { return _ApellidoMaternoBuscar; }
            set { _ApellidoMaternoBuscar = value; OnPropertyChanged("ApellidoMaternoBuscar"); }
        }

        private string nombreBuscar;
        public string NombreBuscar
        {
            get { return nombreBuscar; }
            set
            {
                nombreBuscar = value; OnPropertyChanged("NombreBuscar");
            }
        }

        private short? anioBuscar;
        public short? AnioBuscar
        {
            get { return anioBuscar; }
            set { anioBuscar = value; OnPropertyChanged("AnioBuscar"); }
        }

        private int? folioBuscar;
        public int? FolioBuscar
        {
            get { return folioBuscar; }
            set { folioBuscar = value; OnPropertyChanged("FolioBuscar"); }
        }

        private string textBotonSeleccionarIngreso = "seleccionar ingreso";
        public string TextBotonSeleccionarIngreso
        {
            get { return textBotonSeleccionarIngreso; }
            set { textBotonSeleccionarIngreso = value; OnPropertyChanged("TextBotonSeleccionarIngreso"); }
        }

        private byte[] imagenIngreso = new Imagenes().getImagenPerson();
        public byte[] ImagenIngreso
        {
            get { return imagenIngreso; }
            set
            {
                imagenIngreso = value;
                OnPropertyChanged("ImagenIngreso");
            }
        }

        private byte[] imagenImputado = new Imagenes().getImagenPerson();
        public byte[] ImagenImputado
        {
            get { return imagenImputado; }
            set
            {
                imagenImputado = value;
                OnPropertyChanged("ImagenImputado");
            }
        }

        private LIBERADO_MEDIDA_JUDICIAL selectMJ;
        public LIBERADO_MEDIDA_JUDICIAL SelectMJ
        {
            get { return selectMJ; }
            set
            {
                selectMJ = value;
              //  SelectMJEnabled = value != null ? true : false;
                OnPropertyChanged("SelectMJ");
            }
        }

        private bool selectMJEnabled = false;
        public bool SelectMJEnabled
        {
            get { return selectMJEnabled; }
            set { selectMJEnabled = value; OnPropertyChanged("SelectMJEnabled"); }
        }

        private bool _TabsEnabled;
        public bool TabsEnabled
        {
            get { return _TabsEnabled; }
            set { _TabsEnabled = value; OnPropertyChanged("TabsEnabled"); }
        }

        private bool _EnableDatosReporte;
        public bool EnableDatosReporte
        {
            get { return _EnableDatosReporte; }
            set { _EnableDatosReporte = value; OnPropertyChanged("EnableDatosReporte"); }
        }

        private bool crearNuevoExpedienteEnabled = false;
        public bool CrearNuevoExpedienteEnabled
        {
            get { return crearNuevoExpedienteEnabled; }
            set { crearNuevoExpedienteEnabled = value; OnPropertyChanged("CrearNuevoExpedienteEnabled"); }
        }
        //VARIABLES SEGMENTACION 
        private int Pagina { get; set; }

        private bool SeguirCargando { get; set; }

        private PRS_REPORTE_PSICOLOGICO selectedReportePsicologico;
        // #endregion

        #region Configuracion Permisos
        private bool pInsertar = false;
        public bool PInsertar
        {
            get { return pInsertar; }
            set
            {
                pInsertar = value;
                if (value)
                    MenuGuardarEnabled = value;
            }
        }

        private bool pEditar = false;
        public bool PEditar
        {
            get { return pEditar; }
            set { pEditar = value; }
        }

        private bool pConsultar = false;
        public bool PConsultar
        {
            get { return pConsultar; }
            set
            {
                pConsultar = value;
                //if (value)
                // MenuBuscarEnabled = value;
            }
        }

        private bool pImprimir = false;
        public bool PImprimir
        {
            get { return pImprimir; }
            set
            {
                pImprimir = value;
                if (value)
                    MenuReporteEnabled = value;
            }
        }
        #endregion

        #region Datos Generales
        private string _TextEntidadFederativa;

        public string TextEntidadFederativa
        {
            get { return _TextEntidadFederativa; }
            set { _TextEntidadFederativa = value; OnPropertyChanged("TextEntidadFederativa"); }
        }
        private string _TextTelefono;

        public string TextTelefono
        {
            get { return _TextTelefono; }
            set { _TextTelefono = value; OnPropertyChanged("TextTelefono"); }
        }

        private string _TextTelefonoFamiliar;

        public string TextTelefonoFamiliar
        {
            get { return _TextTelefonoFamiliar; }
            set { 
            _TextTelefonoFamiliar = !string.IsNullOrEmpty(value) ? new Converters().MascaraTelefono(value) : value;                
                OnPropertyChanged("TextTelefonoFamiliar"); }
        }

        private string _TextCalle;

        public string TextCalle
        {
            get { return _TextCalle; }
            set { _TextCalle = value;
            _TextCalle = !string.IsNullOrEmpty(value) ? new Converters().MascaraTelefono(value) : value;                
                OnPropertyChanged("TextCalle"); }
        }

        private int? _TextNumeroInterior;

        public int? TextNumeroInterior
        {
            get { return _TextNumeroInterior; }
            set { _TextNumeroInterior = value; OnPropertyChanged("TextNumeroInterior"); }
        }

        private int? _TextNumeroExterior;

        public int? TextNumeroExterior
        {
            get { return _TextNumeroExterior; }
            set { _TextNumeroExterior = value; OnPropertyChanged("TextNumeroExterior"); }
        }

        private short? selectedIdioma = 8;
        public short? SelectedIdioma
        {
            get { return selectedIdioma; }
            set { selectedIdioma = value; OnPropertyChanged("SelectedIdioma"); }
        }


        private ObservableCollection<ESCOLARIDAD> listEscolaridad;
        public ObservableCollection<ESCOLARIDAD> ListEscolaridad
        {
            get { return listEscolaridad; }
            set { listEscolaridad = value; OnPropertyChanged("ListEscolaridad"); }
        }


        private short? selectEscolaridad = -1;
        public short? SelectEscolaridad
        {
            get { return selectEscolaridad; }
            set
            {

                selectEscolaridad = value;

                OnPropertyValidateChanged("SelectEscolaridad");
            }
        }


        private string _TextParentescoEntrevistado;
        public string TextParentescoEntrevistado
        {
            get { return _TextParentescoEntrevistado; }
            set
            {
                _TextParentescoEntrevistado = value;
                OnPropertyValidateChanged("TextParentescoEntrevistado");
            }
        }



        private short? selectEstadoCivil = -1;
        public short? SelectEstadoCivil
        {
            get { return selectEstadoCivil; }
            set
            {
                if (value == selectEstadoCivil)
                    return;

                selectEstadoCivil = value;

                OnPropertyValidateChanged("SelectEstadoCivil");
            }
        }

        private ObservableCollection<OCUPACION> listOcupacion;
        public ObservableCollection<OCUPACION> ListOcupacion
        {
            get { return listOcupacion; }
            set { listOcupacion = value; OnPropertyChanged("ListOcupacion"); }
        }

        private short? selectOcupacion = -1;
        public short? SelectOcupacion
        {
            get { return selectOcupacion; }
            set
            {
                if (value == selectOcupacion)
                    return;

                selectOcupacion = value;

                OnPropertyValidateChanged("SelectOcupacion");
            }
        }


        private ObservableCollection<PAIS_NACIONALIDAD> listPaisNacimiento;
        public ObservableCollection<PAIS_NACIONALIDAD> ListPaisNacimiento
        {
            get { return listPaisNacimiento; }
            set { listPaisNacimiento = value; OnPropertyChanged("ListPaisNacimiento"); }
        }

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
    
        private PAIS_NACIONALIDAD selectedPaisNacimiento;
        public PAIS_NACIONALIDAD SelectedPaisNacimiento
        {
            get { return selectedPaisNacimiento; }
            set
            {
                selectedPaisNacimiento = value;
                ListEntidadNacimiento = value != null ? new ObservableCollection<ENTIDAD>(value.ENTIDAD) : new ObservableCollection<ENTIDAD>();
                ListEntidadNacimiento.Insert(0, new ENTIDAD() { ID_ENTIDAD = -1, DESCR = "SELECCIONE" });
                SelectEntidadNacimiento = -1;
                OnPropertyChanged("SelectedPaisNacimiento");
            }
        }

        private ENTIDAD selectedEntidadNacimiento;
        public ENTIDAD SelectedEntidadNacimiento
        {
            get { return selectedEntidadNacimiento; }
            set
            {
                selectedEntidadNacimiento = value;
                ListMunicipioNacimiento = value != null ? new ObservableCollection<MUNICIPIO>(value.MUNICIPIO) : new ObservableCollection<MUNICIPIO>();
                ListMunicipioNacimiento.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                SelectMunicipioNacimiento = -1;
                OnPropertyChanged("SelectedEntidadNacimiento");
            }
        }

        private short? selectPaisNacimiento = Parametro.PAIS;
        public short? SelectPaisNacimiento
        {
            get { return selectPaisNacimiento; }
            set
            {
                selectPaisNacimiento = value;
                //if (value == Parametro.PAIS)
                //   // LugarNacimientoEnabled = false;
                //else
                //LugarNacimientoEnabled = true;
                OnPropertyValidateChanged("SelectPaisNacimiento");
            }
        }

        private short? selectEntidadNacimiento = -1;
        public short? SelectEntidadNacimiento
        {
            get { return selectEntidadNacimiento; }
            set
            {
                selectEntidadNacimiento = value;
                OnPropertyValidateChanged("SelectEntidadNacimiento");
            }
        }

        private short? selectMunicipioNacimiento = -1;
        public short? SelectMunicipioNacimiento
        {
            get { return selectMunicipioNacimiento; }
            set
            {
                selectMunicipioNacimiento = value;
                OnPropertyValidateChanged("SelectMunicipioNacimiento");
            }
        }


        private ObservableCollection<ESTADO_CIVIL> lstEstadoCivil;
        public ObservableCollection<ESTADO_CIVIL> LstEstadoCivil
        {
            get { return lstEstadoCivil; }
            set { lstEstadoCivil = value; OnPropertyChanged("LstEstadoCivil"); }
        }

        private string _SelectSexo;


        public string SelectSexo
        {
            get { return _SelectSexo; }
            set { _SelectSexo = value; OnPropertyChanged("SelectSexo"); }
        }

        private string textLugarNacimientoExtranjero;
        public string TextLugarNacimientoExtranjero
        {
            get { return textLugarNacimientoExtranjero; }
            set
            {
                textLugarNacimientoExtranjero = value;
                OnPropertyValidateChanged("TextLugarNacimientoExtranjero");
            }
        }

        private DateTime? _InicioDiaDomingo;

        public DateTime? InicioDiaDomingo
        {
            get { return _InicioDiaDomingo; }
            set { _InicioDiaDomingo = value; OnPropertyChanged("InicioDiaDomingo"); }
        }

        private string tituloTop = "Reporte Psicológico ";
        public string TituloTop
        {
            get { return tituloTop; }
            set { tituloTop = value; OnPropertyChanged("TituloTop"); }
        }   
        private DateTime? textFechaNacimiento;
        public DateTime? TextFechaNacimiento
        {
            get { return textFechaNacimiento; }
            set
            {
                textFechaNacimiento = value;
                if (value != null)
                {
                    //Calcula Edad
                    TextEdad = new Fechas().CalculaEdad(value);
                }
                else
                    TextEdad = 0;
                OnPropertyValidateChanged("TextFechaNacimiento");
            }
        }

        private int? _TextEdad;


        public int? TextEdad
        {
            get { return _TextEdad; }
            set { _TextEdad = value; OnPropertyChanged("TextEdad"); }
        }


        private ObservableCollection<ALIAS> listAlias;
        public ObservableCollection<ALIAS> ListAlias
        {
            get { return listAlias; }
            set { listAlias = value; OnPropertyValidateChanged("ListAlias"); }
        }
        private ALIAS selectAlias;
        public ALIAS SelectAlias
        {
            get { return selectAlias; }
            set { selectAlias = value; OnPropertyChanged("SelectAlias"); }
        }

        private ObservableCollection<APODO> listApodo;
        public ObservableCollection<APODO> ListApodo
        {
            get { return listApodo; }
            set { listApodo = value; OnPropertyValidateChanged("ListApodo"); }
        }
        private string apodo;
        public string Apodo
        {
            get { return apodo; }
            set { apodo = value; OnPropertyChanged("Apodo"); }
        }



        private string _TextApodo;

        public string TextApodo
        {
            get { return _TextApodo; }
            set { _TextApodo = value; OnPropertyChanged("TextApodo"); }
        }


        private APODO selectApodo;
        public APODO SelectApodo
        {
            get { return selectApodo; }
            set { selectApodo = value; OnPropertyChanged("SelectApodo"); }
        }

        private string _TextAlias;

        public string TextAlias
        {
            get { return _TextAlias; }
            set { _TextAlias = value; OnPropertyChanged("TextAlias"); }
        }

        private string paternoAlias;
        public string PaternoAlias
        {
            get { return paternoAlias; }
            set { paternoAlias = value; OnPropertyChanged("PaternoAlias"); }
        }

        private string maternoAlias;
        public string MaternoAlias
        {
            get { return maternoAlias; }
            set { maternoAlias = value; OnPropertyChanged("MaternoAlias"); }
        }

        private string nombreAlias;
        public string NombreAlias
        {
            get { return nombreAlias; }
            set { nombreAlias = value; OnPropertyChanged("NombreAlias"); }
        }

        private string _TextCalleFamiliar;

        public string TextCalleFamiliar
        {
            get { return _TextCalleFamiliar; }
            set { _TextCalleFamiliar = value; OnPropertyChanged("TextCalleFamiliar"); }
        }


        private string _TextNumExteriorFamiliar;

        public string TextNumExteriorFamiliar
        {
            get { return _TextNumExteriorFamiliar; }
            set { _TextNumExteriorFamiliar = value; OnPropertyChanged("TextNumExteriorFamiliar"); }
        }
        private string _TextNumInteriorFamiliar;

        public string TextNumInteriorFamiliar
        {
            get { return _TextNumInteriorFamiliar; }
            set { _TextNumInteriorFamiliar = value; OnPropertyChanged("TextNumInteriorFamiliar"); }
        }
        private string _TextNombreFamiliar;

        public string TextNombreFamiliar
        {
            get { return _TextNombreFamiliar; }
            set { _TextNombreFamiliar = value; OnPropertyChanged("TextNombreFamiliar"); }
        }

        private bool _RadicadoBc;

        public bool RadicadoBc
        {
            get { return _RadicadoBc; }
            set { _RadicadoBc = value; OnPropertyChanged("RadicadoBc"); }
        }

        private ObservableCollection<TIPO_REFERENCIA> _ListParentesco;

        public ObservableCollection<TIPO_REFERENCIA> ListParentesco
        {
            get { return _ListParentesco; }
            set { _ListParentesco = value; OnPropertyChanged("ListParentesco"); }
        }

        private TIPO_REFERENCIA _SelectedParentesco;

        public TIPO_REFERENCIA SelectedParentesco
        {
            get { return _SelectedParentesco; }
            set { _SelectedParentesco = value; OnPropertyChanged("SelectedParentesco"); }
        }

        private short? _SelectParentesco=-1;

        public short? SelectParentesco
        {
            get { return _SelectParentesco; }
            set { _SelectParentesco = value; OnPropertyChanged("SelectParentesco"); }
        }

        #endregion

        #region ApodoyAlias

        private System.Windows.Visibility relacionesPersonalesVisible = Visibility.Collapsed;
        public Visibility RelacionesPersonalesVisible
        {
            get { return relacionesPersonalesVisible; }
            set { relacionesPersonalesVisible = value; OnPropertyChanged("RelacionesPersonalesVisible"); }
        }



        #endregion

        #region Personalidad
        private string _TextTecnicasUtilizadas;

        public string TextTecnicasUtilizadas
        {
            get { return _TextTecnicasUtilizadas; }
            set {
                if (value != null)
                {
                    value = value.Trim();
                }
                _TextTecnicasUtilizadas = value; 
                
                OnPropertyChanged("TextTecnicasUtilizadas"); }
        }

        private string _TextDescripcionEntrv;
        public string TextDescripcionEntrv
        {
            get { return _TextDescripcionEntrv; }
            set {
                if (value!=null)
                {
                    value = value.Trim();    
                }
                _TextDescripcionEntrv = value; 
                
                OnPropertyChanged("TextDescripcionEntrv"); }
        }

        private string _TextExamenMental;

        public string TextExamenMental
        {
            get { return _TextExamenMental; }
            set {

                if (value != null)
                {
                    
                    value = value.Trim();
                }
                _TextExamenMental = value; 
                
                OnPropertyChanged("TextExamenMental"); }
        }

        private string _TextPersonalidad;

        public string TextPersonalidad
        {
            get { return _TextPersonalidad; }
            set {
                if (value != null)
                {
                    value = value.Trim();
                }
                _TextPersonalidad = value; 
                
                OnPropertyChanged("TextPersonalidad"); }
        }

        private string _TextNuceloFamPrimario;

        public string TextNuceloFamPrimario
        {
            get { return _TextNuceloFamPrimario; }
            set {
                if (value != null)
                {
                    value = value.Trim();
                }
                _TextNuceloFamPrimario = value; 
                OnPropertyChanged("TextNuceloFamPrimario"); }
        }

        private string _TextNuceloFamSecundario;

        public string TextNuceloFamSecundario
        {
            get { return _TextNuceloFamSecundario; }
            set {
                if (value != null)
                {
                    value = value.Trim();
                }
                _TextNuceloFamSecundario = value; 
                OnPropertyChanged("TextNuceloFamSecundario"); }
        }

        private string _TextObsrv;

        public string TextObsrv
        {
            get { return _TextObsrv; }
            set {
                if (value != null)
                {
                    value = value.Trim();
                }
                _TextObsrv = value; 
                
                OnPropertyChanged("TextObsrv"); }
        }


        private string _TextSugerencia;

        public string TextSugerencia
        {
            get { return _TextSugerencia; }
            set {
                if (value != null)
                {
                    value = value.Trim();
                }
                _TextSugerencia = value; 
                
                OnPropertyChanged("TextSugerencia"); }
        }
        #endregion

        #region Busqueda
        private bool nuevoProcesoEnabled = false;
        public bool NuevoProcesoEnabled
        {
            get { return nuevoProcesoEnabled; }
            set { nuevoProcesoEnabled = value; OnPropertyChanged("NuevoProcesoEnabled"); }
        }

        private bool seleccionarProcesoEnabled = false;
        public bool SeleccionarProcesoEnabled
        {
            get { return seleccionarProcesoEnabled; }
            set { seleccionarProcesoEnabled = value; OnPropertyChanged("SeleccionarProcesoEnabled"); }
        }

        private Visibility emptyProceso = Visibility.Visible;
        public Visibility EmptyProceso
        {
            get { return emptyProceso; }
            set { emptyProceso = value; OnPropertyChanged("EmptyProceso"); }
        }

        private PROCESO_LIBERTAD auxProcesoLibertad;
        
        private PROCESO_LIBERTAD selectedProcesoLibertad;
        public PROCESO_LIBERTAD SelectedProcesoLibertad
        {
            get { return selectedProcesoLibertad; }
            set { selectedProcesoLibertad = value;
            if (value != null)
                SeleccionarProcesoEnabled = true;
            else
                SeleccionarProcesoEnabled = false;
                OnPropertyChanged("SelectedProcesoLibertad"); 
            
            }
        }

        private IMPUTADO selectExpediente;
        public IMPUTADO SelectExpediente
        {
            get { return selectExpediente; }
            set
            {
                selectExpediente = value;
                // NuevaMJEnabled = value != null ? true : false;
                if (value != null)
                {
                    var foto = value.IMPUTADO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).SingleOrDefault();
                    if (foto != null)
                        ImagenInterno = foto.BIOMETRICO;
                    else
                    {
                        if (value.INGRESO != null)
                        {
                            var ingreso = value.INGRESO.OrderByDescending(w => w.ID_INGRESO).FirstOrDefault();
                            if (ingreso != null)
                            {
                                var fotoIngreso = ingreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).SingleOrDefault();
                                if (fotoIngreso != null)
                                    ImagenInterno = fotoIngreso.BIOMETRICO;
                                else
                                    ImagenInterno = new Imagenes().getImagenPerson();
                            }
                            else
                                ImagenInterno = new Imagenes().getImagenPerson();
                        }
                    }
                    if (value.PROCESO_LIBERTAD != null)
                    {
                        if (value.PROCESO_LIBERTAD.Count == 0)
                        {
                            EmptyProceso = Visibility.Visible;
                        }
                        else
                            EmptyProceso = Visibility.Collapsed;
                    }
                    else
                        EmptyProceso = Visibility.Visible;
                }
                else
                {
                    EmptyProceso = Visibility.Visible;
                    ImagenInterno = new Imagenes().getImagenPerson();
                }
                OnPropertyChanged("SelectExpediente");
            }
        }
        
        private RangeEnabledObservableCollection<IMPUTADO> listExpediente;
        public RangeEnabledObservableCollection<IMPUTADO> ListExpediente
        {
            get { return listExpediente; }
            set
            {
                listExpediente = value;
                OnPropertyChanged("ListExpediente");
            }
        }
        
        private bool emptyExpedienteVisible;
        public bool EmptyExpedienteVisible
        {
            get { return emptyExpedienteVisible; }
            set { emptyExpedienteVisible = value; OnPropertyChanged("EmptyExpedienteVisible"); }
        }

        private byte[] imagenInterno = new Imagenes().getImagenPerson();
        public byte[] ImagenInterno
        {
            get { return imagenInterno; }
            set { imagenInterno = value; OnPropertyChanged("ImagenInterno"); }
        }

        private ObservableCollection<LIBERADO_MEDIDA_JUDICIAL> lstLiberadoMJ;
        public ObservableCollection<LIBERADO_MEDIDA_JUDICIAL> LstLiberadoMJ
        {
            get { return lstLiberadoMJ; }
            set { lstLiberadoMJ = value; OnPropertyChanged("LstLiberadoMJ"); }
        }

        private Visibility emptyMJVisible = System.Windows.Visibility.Collapsed;
        public Visibility EmptyMJVisible
        {
            get { return emptyMJVisible; }
            set { emptyMJVisible = value; OnPropertyChanged("EmptyMJVisible"); }
        }

        private bool nuevaMJEnabled = false;
        public bool NuevaMJEnabled
        {
            get { return nuevaMJEnabled; }
            set { nuevaMJEnabled = value; OnPropertyChanged("NuevaMJEnabled"); }
        }

        private INGRESO selectIngreso;
        public INGRESO SelectIngreso
        {
            get { return selectIngreso; }
            set
            {
                selectIngreso = value;
                if (selectIngreso == null)
                {
                    ImagenIngreso = ImagenImputado = new Imagenes().getImagenPerson();
                    return;
                }
                if (SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                    ImagenImputado = SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                else
                    ImagenImputado = new Imagenes().getImagenPerson();
                if (selectIngreso.INGRESO_BIOMETRICO.Any(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG))
                {
                    ImagenIngreso = selectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                    OnPropertyChanged("SelectIngreso");
                }
                else
                    ImagenIngreso = new Imagenes().getImagenPerson();
            }
        }

        private string tituloModal;
        public string TituloModal
        {
            get { return tituloModal; }
            set { tituloModal = value; OnPropertyChanged("TituloModal"); }
        }
        #endregion

        #region Liberados
        private string _NUCBuscar;
        public string NUCBuscar
        {
            get { return _NUCBuscar; }
            set { _NUCBuscar = value; OnPropertyChanged("NUCBuscar"); }
        }

        private RangeEnabledObservableCollection<cLiberados> lstLiberados;
        public RangeEnabledObservableCollection<cLiberados> LstLiberados
        {
            get { return lstLiberados; }
            set { lstLiberados = value; OnPropertyChanged("LstLiberados"); }
        }

        private cLiberados selectedLiberado;
        public cLiberados SelectedLiberado
        {
            get { return selectedLiberado; }
            set
            {
                selectedLiberado = value;
                if (value != null)
                {
                    SelectExpediente = new cImputado().Obtener(value.ID_IMPUTADO, value.ID_ANIO, value.ID_CENTRO).FirstOrDefault();
                }
                else
                {
                    SelectExpediente = null;
                }
                OnPropertyChanged("SelectedLiberado");
            }
        }
        #endregion

        #region Huellas
        IList<PlantillaBiometrico> HuellasCapturadas;

        private enumTipoBiometrico? _DD_Dedo;
        public enumTipoBiometrico? DD_Dedo
        {
            get { return _DD_Dedo; }
            set { _DD_Dedo = value; }
        }

        private Visibility _ShowPopUp = Visibility.Hidden;
        public Visibility ShowPopUp
        {
            get { return _ShowPopUp; }
            set
            {
                _ShowPopUp = value;
                OnPropertyChanged("ShowPopUp");
            }
        }

        private Visibility _ShowFingerPrint = Visibility.Hidden;
        public Visibility ShowFingerPrint
        {
            get { return _ShowFingerPrint; }
            set
            {
                _ShowFingerPrint = value;
                OnPropertyChanged("ShowFingerPrint");
            }
        }

        private Visibility _ShowLine = Visibility.Visible;
        public Visibility ShowLine
        {
            get { return _ShowLine; }
            set
            {
                _ShowLine = value;
                OnPropertyChanged("ShowLine");
            }
        }

        private Visibility _ShowOk = Visibility.Hidden;
        public Visibility ShowOk
        {
            get { return _ShowOk; }
            set
            {
                _ShowOk = value;
                OnPropertyChanged("ShowOk");
            }
        }

        private ImageSource _GuardaHuella;
        public ImageSource GuardaHuella
        {
            get { return _GuardaHuella; }
            set
            {
                _GuardaHuella = value;
                OnPropertyChanged("GuardaHuella");
            }
        }

        private Brush _PulgarDerecho;
        public Brush PulgarDerecho
        {
            get { return _PulgarDerecho; }
            set
            {
                _PulgarDerecho = value;
                RaisePropertyChanged("PulgarDerecho");
            }
        }

        private Brush _IndiceDerecho;
        public Brush IndiceDerecho
        {
            get { return _IndiceDerecho; }
            set
            {
                _IndiceDerecho = value;
                OnPropertyChanged("IndiceDerecho");
            }
        }

        private Brush _MedioDerecho;
        public Brush MedioDerecho
        {
            get { return _MedioDerecho; }
            set
            {
                _MedioDerecho = value;
                OnPropertyChanged("MedioDerecho");
            }
        }

        private Brush _AnularDerecho;
        public Brush AnularDerecho
        {
            get { return _AnularDerecho; }
            set
            {
                _AnularDerecho = value;
                OnPropertyChanged("AnularDerecho");
            }
        }

        private Brush _MeñiqueDerecho;
        public Brush MeñiqueDerecho
        {
            get { return _MeñiqueDerecho; }
            set
            {
                _MeñiqueDerecho = value;
                OnPropertyChanged("MeñiqueDerecho");
            }
        }

        private Brush _PulgarIzquierdo;
        public Brush PulgarIzquierdo
        {
            get { return _PulgarIzquierdo; }
            set
            {
                _PulgarIzquierdo = value;
                OnPropertyChanged("PulgarIzquierdo");
            }
        }

        private Brush _IndiceIzquierdo;
        public Brush IndiceIzquierdo
        {
            get { return _IndiceIzquierdo; }
            set
            {
                _IndiceIzquierdo = value;
                OnPropertyChanged("IndiceIzquierdo");
            }
        }

        private Brush _MedioIzquierdo;
        public Brush MedioIzquierdo
        {
            get { return _MedioIzquierdo; }
            set
            {
                _MedioIzquierdo = value;
                OnPropertyChanged("MedioIzquierdo");
            }
        }

        private Brush _AnularIzquierdo;
        public Brush AnularIzquierdo
        {
            get { return _AnularIzquierdo; }
            set
            {
                _AnularIzquierdo = value;
                OnPropertyChanged("AnularIzquierdo");
            }
        }

        private Brush _MeñiqueIzquierdo;
        public Brush MeñiqueIzquierdo
        {
            get { return _MeñiqueIzquierdo; }
            set
            {
                _MeñiqueIzquierdo = value;
                OnPropertyChanged("MeñiqueIzquierdo");
            }
        }
        #endregion

        #region Filtros
        private bool porNUC = true;
        public bool PorNUC
        {
            get { return porNUC; }
            set { porNUC = value;
            if (value)
            {
                PorCP = false;
                PorNUCVisible = Visibility.Visible;
                PorCPVisible = Visibility.Collapsed;
                AnioBuscar = null;
                FolioBuscar = null;
            }
            else
            {
                PorCP = true;
                PorNUCVisible = Visibility.Collapsed;
                PorCPVisible = Visibility.Visible;
                NUCBuscar = string.Empty;
            }
                OnPropertyChanged("PorNUC"); }
        }

        private bool porCP = false;
        public bool PorCP
        {
            get { return porCP; }
            set { porCP = value; OnPropertyChanged("PorCP"); }
        }
        
        private Visibility porNUCVisible = Visibility.Visible;
        public Visibility PorNUCVisible
        {
            get { return porNUCVisible; }
            set { porNUCVisible = value; OnPropertyChanged("PorNUCVisible"); }
        }

        private Visibility porCPVisible = Visibility.Collapsed;
        public Visibility PorCPVisible
        {
            get { return porCPVisible; }
            set { porCPVisible = value; OnPropertyChanged("PorCPVisible"); }
        }
        #endregion


    }
}
