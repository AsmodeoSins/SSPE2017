using ControlPenales.Clases;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ControlPenales
{
    partial class EntrevistaMultidiciplinariaViewModel
    {
        #region [Propiedades]
        #region [Generales]
        private string SelectedTabToSave;
        private short? MainTabGroupIndex = 0;
        private short? Tab1Index;
        private short? Tab2Index;
        private short? Tab3Index;
        private short? Tab5Index;


        public enum Tabs
        {
            FichaIdentificacion,
            SituacionJuridica,
            EstudioTraslado,
            IngresoAnteriorCereso,
            IngresoAnteriorCeresoMenor,
            FactoresSocioFamiliares,
            Factores,
            DatosGrupoFamiliar,
            AntecedentesGrupoFamilliar,
            ConductaParasocial,
            UsoDrogas,
            HomosexualidadPandillaSexualidad,
            Tatuajes,
            TopografiaHumanaView,
            Enfermedades,
            Actividades,
            ClasCrim,
            ClasificacionCriminologicaView,
            FactoresCriminodiagnosticoView,
        }

        private bool tatuajesVisible = false;
        public bool TatuajesVisible
        {
            get { return tatuajesVisible; }
            set
            {
                tatuajesVisible = value;
                OnPropertyChanged("TatuajesVisible");
            }
        }

        private bool botonesEnables2 = true;
        public bool BotonesEnables2
        {
            get { return botonesEnables2; }
            set
            {
                botonesEnables2 = value;
                OnPropertyChanged("BotonesEnables2");
            }
        }

        private bool botonesEnables = true;
        public bool BotonesEnables
        {
            get { return botonesEnables; }
            set
            {
                botonesEnables = value;
                OnPropertyChanged("BotonesEnables");
            }
        }

        private bool emiVisible = true;
        public bool EmiVisible
        {
            get { return emiVisible; }
            set
            {
                emiVisible = value;
                OnPropertyChanged("EmiVisible");
            }
        }

        private bool band1 = true;
        public bool Band1
        {
            get { return band1; }
            set
            {
                band1 = value;
                OnPropertyChanged("Band1");
            }
        }

        private bool band2 = true;
        public bool Band2
        {
            get { return band2; }
            set
            {
                band2 = value;
                OnPropertyChanged("Band2");
            }
        }

        private string tituloModal;
        public string TituloModal
        {
            get { return tituloModal; }
            set
            {
                tituloModal = value;
                OnPropertyChanged("TituloModal");
            }
        }


        public string Name
        {
            get { return "entrevista_multidiciplinaria"; }
        }
        #endregion
        #region [Encabezado]
        private string _Cereso;
        public string Cereso
        {
            get { return _Cereso; }
            set
            {
                _Cereso = value;
                OnPropertyChanged("Cereso");
            }
        }

        private string _ClasificacionJuridica;
        public string ClasificacionJuridica
        {
            get { return _ClasificacionJuridica; }
            set
            {
                _ClasificacionJuridica = value;
                OnPropertyChanged("ClasificacionJuridica");
            }
        }

        private string _Ubicacion;
        public string Ubicacion
        {
            get { return _Ubicacion; }
            set
            {
                _Ubicacion = value;
                OnPropertyChanged("Ubicacion");
            }
        }

        private string _Expediente;
        public string Expediente
        {
            get { return _Expediente; }
            set
            {
                _Expediente = value;
                OnPropertyChanged("Expediente");
            }
        }

        private string _CausaPenal;
        public string CausaPenal
        {
            get { return _CausaPenal; }
            set
            {
                _CausaPenal = value;
                OnPropertyChanged("CausaPenal");
            }
        }

        private DateTime? _Ingreso;
        public DateTime? Ingreso
        {
            get { return _Ingreso; }
            set
            {
                _Ingreso = value;
                OnPropertyChanged("Ingreso");
            }
        }
        #endregion
        #region[Ficha Identificacion]
        private string _ApellidoPaterno;
        public string ApellidoPaterno
        {
            get { return _ApellidoPaterno; }
            set
            {
                _ApellidoPaterno = value;
                OnPropertyChanged("ApellidoPaterno");
            }
        }

        private string _ApellidoMaterno;
        public string ApellidoMaterno
        {
            get { return _ApellidoMaterno; }
            set
            {
                _ApellidoMaterno = value;
                OnPropertyChanged("ApellidoMaterno");
            }
        }

        private string _Nombre;
        public string Nombre
        {
            get { return _Nombre; }
            set
            {
                _Nombre = value;
                OnPropertyChanged("Nombre");
            }
        }

        private string _EstadoCivil;
        public string EstadoCivil
        {
            get { return _EstadoCivil; }
            set
            {
                _EstadoCivil = value;
                OnPropertyChanged("EstadoCivil");
            }
        }

        private string _Sexo;
        public string Sexo
        {
            get { return _Sexo; }
            set
            {
                _Sexo = value;
                OnPropertyChanged("Sexo");
            }
        }

        private DateTime? _FechaNacimiento;
        public DateTime? FechaNacimiento
        {
            get { return _FechaNacimiento; }
            set
            {
                _FechaNacimiento = value;
                OnPropertyChanged("FechaNacimiento");
            }
        }

        private string _Edad;
        public string Edad
        {
            get { return _Edad; }
            set
            {
                _Edad = value;
                OnPropertyChanged("Edad");
            }
        }

        private string _Religion;
        public string Religion
        {
            get { return _Religion; }
            set
            {
                _Religion = value;
                OnPropertyChanged("Religion");
            }
        }

        private string _Etnia;
        public string Etnia
        {
            get { return _Etnia; }
            set
            {
                _Etnia = value;
                OnPropertyChanged("Etnia");
            }
        }

        private string _Apodo;
        public string Apodo
        {
            get { return _Apodo; }
            set
            {
                _Apodo = value;
                OnPropertyChanged("Apodo");
            }
        }
        #region [Lugar de Nacimiento]
        private string _LNMunicipio;
        public string LNMunicipio
        {
            get { return _LNMunicipio; }
            set
            {
                _LNMunicipio = value;
                OnPropertyChanged("LNMunicipio");
            }
        }

        private string _LNEstado;
        public string LNEstado
        {
            get { return _LNEstado; }
            set
            {
                _LNEstado = value;
                OnPropertyChanged("LNEstado");
            }
        }

        private string _LNPais;
        public string LNPais
        {
            get { return _LNPais; }
            set
            {
                _LNPais = value;
                OnPropertyChanged("LNPais");
            }
        }

        private string _Nacionalidad;
        public string Nacionalidad
        {
            get { return _Nacionalidad; }
            set
            {
                _Nacionalidad = value;
                OnPropertyChanged("Nacionalidad");
            }
        }

        #endregion
        #region [Tiempo de Residencia en Baja California]
        private DateTime? _FechaLlegada;
        public DateTime? FechaLlegada
        {
            get { return _FechaLlegada; }
            set
            {
                _FechaLlegada = value;
                OnPropertyChanged("FechaLlegada");
            }
        }

        private short? _Años;
        public short? Años
        {
            get { return _Años; }
            set
            {
                _Años = value;
                OnPropertyChanged("Años");
            }
        }

        private short? _Meses;
        public short? Meses
        {
            get { return _Meses; }
            set
            {
                _Meses = value;
                OnPropertyChanged("Meses");
            }
        }

        private string _Dias;
        public string Dias
        {
            get { return _Dias; }
            set
            {
                _Dias = value;
                OnPropertyChanged("Dias");
            }
        }
        #endregion
        #region [Domicilio]
        private string _DPais;
        public string DPais
        {
            get { return _DPais; }
            set
            {
                _DPais = value;
                OnPropertyChanged("DPais");
            }
        }

        private string _DEstado;
        public string DEstado
        {
            get { return _DEstado; }
            set
            {
                _DEstado = value;
                OnPropertyChanged("DEstado");
            }
        }

        private string _DMunicipio;
        public string DMunicipio
        {
            get { return _DMunicipio; }
            set
            {
                _DMunicipio = value;
                OnPropertyChanged("DMunicipio");
            }
        }

        private string _Colonia;
        public string Colonia
        {
            get { return _Colonia; }
            set
            {
                _Colonia = value;
                OnPropertyChanged("Colonia");
            }
        }

        private string _Calle;
        public string Calle
        {
            get { return _Calle; }
            set
            {
                _Calle = value;
                OnPropertyChanged("Calle");
            }
        }

        private int? _NumeroExterior;
        public int? NumeroExterior
        {
            get { return _NumeroExterior; }
            set
            {
                _NumeroExterior = value;
                OnPropertyChanged("NumeroExterior");
            }
        }

        private string _NumeroInterior;
        public string NumeroInterior
        {
            get { return _NumeroInterior; }
            set
            {
                _NumeroInterior = value;
                OnPropertyChanged("NumeroInterior");
            }
        }

        private int? _CodigoPostal;
        public int? CodigoPostal
        {
            get { return _CodigoPostal; }
            set
            {
                _CodigoPostal = value;
                OnPropertyChanged("CodigoPostal");
            }
        }
        #endregion
        #endregion
        #endregion

        #region [Delegados]
        public delegate void ParameterChange(string parameter);
        public ParameterChange _OnParameterChange { get; set; }
        #endregion

        private short? edadInterno;
        public short? EdadInterno
        {
            get { return edadInterno; }
            set { edadInterno = value; OnPropertyChanged("EdadInterno"); }
        }

        //TAB
        private bool tabActividadesSelected;
        public bool TabActividadesSelected
        {
            get { return tabActividadesSelected; }
            set { tabActividadesSelected = value; OnPropertyChanged("TabActividadesSelected"); }
        }

        private WebCam CamaraWeb;
        private List<ImageSourceToSave> _ImagesToSave;
        public List<ImageSourceToSave> ImagesToSave
        {
            get { return _ImagesToSave; }
            set { _ImagesToSave = value; }
        }

        private bool _Processing = false;
        public bool Processing
        {
            get { return _Processing; }
            set
            {
                _Processing = value;
                OnPropertyChanged("Processing");
            }
        }

        private bool cambioImputado = false;
        public bool CambioImputado
        {
            get { return cambioImputado; }
            set { cambioImputado = value; OnPropertyChanged("CambioImputado"); }
        }

        private int indexMenu = 1;
        public int IndexMenu
        {
            get { return indexMenu; }
            set { indexMenu = value; OnPropertyChanged("IndexMenu"); }
        }

        private short controlTab = 0;
        public short ControlTab
        {
            get { return controlTab; }
            set { controlTab = value; }
        }

        private EMI anteriorEMI;
        public EMI AnteriorEMI
        {
            get { return anteriorEMI; }
            set { anteriorEMI = value; OnPropertyChanged("AnteriorEMI"); }
        }


        ////////////////////////////////////////////////////////////
        /*CONTROL DE TABS*/
        private bool situacionJuridicaEnabled = false;
        public bool SituacionJuridicaEnabled
        {
            get { return situacionJuridicaEnabled; }
            set { situacionJuridicaEnabled = value; OnPropertyChanged("SituacionJuridicaEnabled"); }
        }

        private bool estudioTrasladoEnabled = false;
        public bool EstudioTrasladoEnabled
        {
            get { return estudioTrasladoEnabled; }
            set { estudioTrasladoEnabled = value; OnPropertyChanged("EstudioTrasladoEnabled"); }
        }

        private bool ingresoAnteriorEnabled = false;
        public bool IngresoAnteriorEnabled
        {
            get { return ingresoAnteriorEnabled; }
            set { ingresoAnteriorEnabled = value; OnPropertyChanged("IngresoAnteriorEnabled"); }
        }

        private bool ingresoAnteriorMenorEnabled = false;
        public bool IngresoAnteriorMenorEnabled
        {
            get { return ingresoAnteriorMenorEnabled; }
            set { ingresoAnteriorMenorEnabled = value; OnPropertyChanged("IngresoAnteriorMenorEnabled"); }
        }

        private bool factoresSocioFamiliaresEnabled = false;
        public bool FactoresSocioFamiliaresEnabled
        {
            get { return factoresSocioFamiliaresEnabled; }
            set { factoresSocioFamiliaresEnabled = value; OnPropertyChanged("FactoresSocioFamiliaresEnabled"); }
        }

        private bool factoresEnabled = false;
        public bool FactoresEnabled
        {
            get { return factoresEnabled; }
            set { factoresEnabled = value; OnPropertyChanged("FactoresEnabled"); }
        }

        private bool grupoFamiliarEnabled = false;
        public bool GrupoFamiliarEnabled
        {
            get { return grupoFamiliarEnabled; }
            set { grupoFamiliarEnabled = value; OnPropertyChanged("GrupoFamiliarEnabled"); }
        }

        private bool antecedenteGrupoFamiliarEnabled = false;
        public bool AntecedenteGrupoFamiliarEnabled
        {
            get { return antecedenteGrupoFamiliarEnabled; }
            set { antecedenteGrupoFamiliarEnabled = value; OnPropertyChanged("AntecedenteGrupoFamiliarEnabled"); }
        }

        private bool conductasParasocialesEnabled = false;
        public bool ConductasParasocialesEnabled
        {
            get { return conductasParasocialesEnabled; }
            set { conductasParasocialesEnabled = value; OnPropertyChanged("ConductasParasocialesEnabled"); }
        }

        private bool usoDrogaEnabled = false;
        public bool UsoDrogaEnabled
        {
            get { return usoDrogaEnabled; }
            set { usoDrogaEnabled = value; OnPropertyChanged("UsoDrogaEnabled"); }
        }

        private bool hPSEnabled = false;
        public bool HPSEnabled
        {
            get { return hPSEnabled; }
            set { hPSEnabled = value; OnPropertyChanged("HPSEnabled"); }
        }

        private bool tatuajesEnabled = false;
        public bool TatuajesEnabled
        {
            get { return tatuajesEnabled; }
            set { tatuajesEnabled = value; OnPropertyChanged("TatuajesEnabled"); }
        }

        private bool seniasParticularesEnabled = false;
        public bool SeniasParticularesEnabled
        {
            get { return seniasParticularesEnabled; }
            set { seniasParticularesEnabled = value; OnPropertyChanged("SeniasParticularesEnabled"); }
        }

        private bool enfermedadesEnabled = false;
        public bool EnfermedadesEnabled
        {
            get { return enfermedadesEnabled; }
            set { enfermedadesEnabled = value; OnPropertyChanged("EnfermedadesEnabled"); }
        }

        private bool actividadesEnabled = false;
        public bool ActividadesEnabled
        {
            get { return actividadesEnabled; }
            set { actividadesEnabled = value; OnPropertyChanged("ActividadesEnabled"); }
        }


        private bool clasCriminologicaEnabled = false;
        public bool ClasCriminologicaEnabled
        {
            get { return clasCriminologicaEnabled; }
            set { clasCriminologicaEnabled = value; OnPropertyChanged("ClasCriminologicaEnabled"); }
        }

        private bool clasificacionCriminologicaEnabled = false;
        public bool ClasificacionCriminologicaEnabled
        {
            get { return clasificacionCriminologicaEnabled; }
            set { clasificacionCriminologicaEnabled = value; OnPropertyChanged("ClasificacionCriminologicaEnabled"); }
        }

        private bool clasificacionCrimidiagnosticoEnabled = false;
        public bool ClasificacionCrimidiagnosticoEnabled
        {
            get { return clasificacionCrimidiagnosticoEnabled; }
            set { clasificacionCrimidiagnosticoEnabled = value; OnPropertyChanged("ClasificacionCrimidiagnosticoEnabled"); }
        }

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
                if (value)
                    MenuBuscarEnabled = value;
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

        #region Menu
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
        private bool menuFichaEnabled = false;
        public bool MenuFichaEnabled
        {
            get { return menuFichaEnabled; }
            set { menuFichaEnabled = value; OnPropertyChanged("MenuFichaEnabled"); }
        }
        #endregion

        ////////////////////////////////////////////////////////////
        #region [Propiedades Tratamiento]
        private int ROW;
        private int COLUMN;
        private bool isalertcolumnas;
        private bool isalertcolumnassame;
        private bool _handleSelection = true;
        public bool HandleSelection
        {
            get { return _handleSelection; }
            set
            {
                if (value)
                    isalertcolumnas = true;

                _handleSelection = value;
            }
        }
        private IList<EJE> ListadoEjes;
        private List<Reticula> ListaReticula;
        private string _ErrorText = string.Empty;
        public string ErrorText
        {
            get { return _ErrorText; }
            set
            {
                _ErrorText = value;
                OnPropertyChanged("ErrorText");
            }
        }
        private bool _IsEnabledTratamiento;
        public bool IsEnabledTratamiento
        {
            get { return _IsEnabledTratamiento; }
            set
            {
                _IsEnabledTratamiento = value;
                OnPropertyChanged("IsEnabledTratamiento");
            }
        }

        private System.Windows.Controls.Grid DynamicGrid;
        public System.Windows.Controls.Grid _DynamicGrid
        {
            get { return DynamicGrid; }
            set { DynamicGrid = value; OnPropertyChanged("_DynamicGrid"); }
        }
        #endregion
    }


    //public class Reticula
    //{
    //    public short? Eje { get; set; }
    //    public DateTime? FechaRegistroValue { get; set; }
    //    public List<TratamientoActividades> Actividad { get; set; }
    //}

    //public class TratamientoActividades
    //{
    //    public short? DepartamentoValue { get; set; }
    //    public short? ProgramaValue { get; set; }
    //    public short? ActividadValue { get; set; }
    //    public short? EstatusValue { get; set; }
    //    public GRUPO_PARTICIPANTE grupo_participante { get; set; }
    //}

    //public class GridTratamientoActividad
    //{
    //    public string DEPARTAMENTO { get; set; }
    //    public string PROGRAMA { get; set; }
    //    public string ACTIVIDAD { get; set; }
    //    public bool ELIGE { get; set; }
    //    public string ESTATUS { get; set; }
    //    public short? ESTATUSVALUE { get; set; }
    //    public ACTIVIDAD_EJE actividad_eje { get; set; }
    //}
}
