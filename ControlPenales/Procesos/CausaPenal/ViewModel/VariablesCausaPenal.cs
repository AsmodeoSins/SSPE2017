
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ControlPenales
{
    partial class CausaPenalViewModel : ValidationViewModelBase
    {
        #region AP
        private short? agenciaAP;
        public short? AgenciaAP
        {
            get { return agenciaAP; }
            set { agenciaAP = value; OnPropertyValidateChanged("AgenciaAP"); }
        }
        
        private short? anioAP;
        public short? AnioAP
        {
            get { return anioAP; }
            set { anioAP = value; OnPropertyValidateChanged("AnioAP"); }
        }
        
        private int? folioAP;
        public int? FolioAP
        {
            get { return folioAP; }
            set { folioAP = value; OnPropertyValidateChanged("FolioAP"); }
        }
        
        private string averiguacionPreviaAP;
        public string AveriguacionPreviaAP
        {
            get { return averiguacionPreviaAP; }
            set { averiguacionPreviaAP = value; OnPropertyValidateChanged("AveriguacionPreviaAP"); }
        }
        
        private DateTime? fecAveriguacionAP;
        public DateTime? FecAveriguacionAP
        {
            get { return fecAveriguacionAP; }
            set { fecAveriguacionAP = value; OnPropertyValidateChanged("FecAveriguacionAP"); }
        }
        
        private DateTime? fecConsignacionAP;
        public DateTime? FecConsignacionAP
        {
            get { return fecConsignacionAP; }
            set { fecConsignacionAP = value; OnPropertyValidateChanged("FecConsignacionAP"); }
        }
        #endregion

        #region CP
        private short? anioCP;
        public short? AnioCP
        {
            get { return anioCP; }
            set { anioCP = value;
            ValidarCausaPenalIngresoAnterior();
                OnPropertyValidateChanged("AnioCP"); }
        }
       
        private int? folioCP;
        public int? FolioCP
        {
            get { return folioCP; }
            set { folioCP = value;
            ValidarCausaPenalIngresoAnterior();
                OnPropertyValidateChanged("FolioCP"); }
        }
        
        private string bisCP;
        public string BisCP
        {
            get { return bisCP; }
            set { bisCP = value; OnPropertyValidateChanged("BisCP"); }
        }
        
        private string foraneoCP;
        public string ForaneoCP
        {
            get { return foraneoCP; }
            set { foraneoCP = value; OnPropertyValidateChanged("ForaneoCP"); }
        }
        
        private short? tipoOrdenCP;
        public short? TipoOrdenCP
        {
            get { return tipoOrdenCP; }
            set { tipoOrdenCP = value; OnPropertyValidateChanged("TipoOrdenCP"); }
        }
        
        private short? paisJuzgadoCP;
        public short? PaisJuzgadoCP
        {
            get { return paisJuzgadoCP; }
            set { 
                paisJuzgadoCP = value;
                GetJuzgados();
                OnPropertyValidateChanged("PaisJuzgadoCP");
            }
        }
        
        private short? edificioI;
        public short? EdificioI
        {
            get { return edificioI; }
            set { edificioI = value; OnPropertyValidateChanged("EdificioI"); }
        }
        
        private short? estadoJuzgadoCP;
        public short? EstadoJuzgadoCP
        {
            get { return estadoJuzgadoCP; }
            set { 
                estadoJuzgadoCP = value;
                GetJuzgados();
                OnPropertyValidateChanged("EstadoJuzgadoCP");
            }
        }
        
        private short? municipioJuzgadoCP;
        public short? MunicipioJuzgadoCP
        {
            get { return municipioJuzgadoCP; }
            set
            {
                municipioJuzgadoCP = value;
                GetJuzgados();
                OnPropertyValidateChanged("MunicipioJuzgadoCP");
            }
        }
        
        private string fueroCP;
        public string FueroCP
        {
            get { return fueroCP; }
            set
            {
                fueroCP = value;
                this.GetJuzgados();
                OnPropertyValidateChanged("FueroCP");
            }
        }
        
        private short? juzgadoCP;
        public short? JuzgadoCP
        {
            get { return juzgadoCP; }
            set { juzgadoCP = value; OnPropertyValidateChanged("JuzgadoCP"); }
        }
        
        private DateTime? fecRadicacionCP;
        public DateTime? FecRadicacionCP
        {
            get { return fecRadicacionCP; }
            set { fecRadicacionCP = value; OnPropertyValidateChanged("FecRadicacionCP"); }
        }
        
        private string ampliacionCP = string.Empty;
        public string AmpliacionCP
        {
            get { return ampliacionCP; }
            set { ampliacionCP = value; OnPropertyValidateChanged("AmpliacionCP"); }
        }
        
        private DateTime? fecVencimientoTerinoCP;
        public DateTime? FecVencimientoTerinoCP
        {
            get { return fecVencimientoTerinoCP; }
            set { fecVencimientoTerinoCP = value; OnPropertyValidateChanged("FecVencimientoTerinoCP"); }
        }
        
        private short? terminoCP;
        public short? TerminoCP
        {
            get { return terminoCP; }
            set { terminoCP = value; OnPropertyValidateChanged("TerminoCP"); }
        }
        
        private short? estatusCP;
        public short? EstatusCP
        {
            get { return estatusCP; }
            set {
                estatusCP = value;
                if (value == 0 || value == 1)//ACTIVO
                {
                    ValidarCambioEstatus();
                }
                OnPropertyValidateChanged("EstatusCP");
                }
        }
        
        private string juzgadoFuero;
        public string JuzgadoFuero
        {
            get { return juzgadoFuero; }
            set { juzgadoFuero = value; OnPropertyValidateChanged("JuzgadoFuero"); }
        }
        
        private AGENCIA selectedAgencia;
        public AGENCIA SelectedAgencia
        {
            get { return selectedAgencia; }
            set { selectedAgencia = value; OnPropertyValidateChanged("SelectedAgencia"); }
        }
        
        private PAIS_NACIONALIDAD selectedPais;
        public PAIS_NACIONALIDAD SelectedPais
        {
            get { return selectedPais; }
            set
            {
                selectedPais = value;
                if (value != null)
                {
                    if (value.ID_PAIS_NAC != -1)
                    {
                        if (value.ID_PAIS_NAC == Parametro.PAIS)//MEXICO
                        {
                            LstEntidades = new ObservableCollection<ENTIDAD>(value.ENTIDAD);
                            LstEntidades.Insert(0, new ENTIDAD() { ID_ENTIDAD = -1, DESCR = "SELECCIONE" });
                            EstadoJuzgadoCP = -1;
                            LstMunicipios = new ObservableCollection<MUNICIPIO>();
                            LstMunicipios.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                            MunicipioJuzgadoCP = -1;
                            EstadoJuzgadoEnabled = true;
                        }
                        else//EXTRANJERO
                        {
                            LstEntidades = new ObservableCollection<ENTIDAD>();
                            LstEntidades.Insert(0, new ENTIDAD() { ID_ENTIDAD = -1, DESCR = "EXTRANJERO" });
                            EstadoJuzgadoCP = -1;
                            LstMunicipios = new ObservableCollection<MUNICIPIO>();
                            LstMunicipios.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "EXTRANJERO" });
                            MunicipioJuzgadoCP = -1;
                            EstadoJuzgadoEnabled = false;
                        }

                    }
                    else
                    {
                        LstEntidades = new ObservableCollection<ENTIDAD>();
                        LstEntidades.Insert(0, new ENTIDAD() { ID_ENTIDAD = -1, DESCR = "SELECCIONE" });
                        EstadoJuzgadoCP = -1;
                        LstMunicipios = new ObservableCollection<MUNICIPIO>();
                        LstMunicipios.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                        MunicipioJuzgadoCP = -1;
                        EstadoJuzgadoEnabled = false;
                    }
                }
                else
                {
                    LstEntidades = new ObservableCollection<ENTIDAD>();
                    LstEntidades.Insert(0, new ENTIDAD() { ID_ENTIDAD = -1, DESCR = "SELECCIONE" });
                    EstadoJuzgadoCP = -1;
                    LstMunicipios = new ObservableCollection<MUNICIPIO>();
                    LstMunicipios.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                    MunicipioJuzgadoCP = -1;
                    EstadoJuzgadoEnabled = false;
                }
                #region Comentado
                //if (selectedPais != null)
                //{
                //    Entidades = new ObservableCollection<ENTIDAD>((new cEntidad()).Obtener(selectedPais.ID_PAIS_NAC));
                //}
                //else
                //    Entidades = new ObservableCollection<ENTIDAD>();
                //Entidades.Insert(0, new ENTIDAD { ID_ENTIDAD = -1, DESCR = "SELECCIONE" });
                //if (PaisJuzgadoCP == 82 || PaisJuzgadoCP == 1223)//Mexico
                //{
                //    EstadoJuzgadoCP = 2;//Baja California
                //    EstadoJuzgadoEnabled = true;
                //}
                //else if (PaisJuzgadoCP == -1)
                //{
                //    EstadoJuzgadoCP = -1;
                //    EstadoJuzgadoEnabled = false;
                //}
                //else
                //{
                //    EstadoJuzgadoCP = 33;
                //    EstadoJuzgadoEnabled = false;
                //}
                #endregion
                OnPropertyValidateChanged("SelectedPais");
            }
        }
        
        private ENTIDAD selectedEstado;
        public ENTIDAD SelectedEstado
        {
            get { return selectedEstado; }
            set
            {
                selectedEstado = value;
                if (value != null)
                {
                    LstMunicipios = new ObservableCollection<MUNICIPIO>(value.MUNICIPIO);
                    LstMunicipios.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                    MunicipioJuzgadoCP = -1;
                }
                else
                {
                    LstMunicipios = new ObservableCollection<MUNICIPIO>();
                    LstMunicipios.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                    MunicipioJuzgadoCP = -1;
                }
                #region Comentado
                //if (selectedEstado != null)
                //{
                //    Municipios = new ObservableCollection<MUNICIPIO>((new cMunicipio()).Obtener(selectedEstado.ID_ENTIDAD));
                //}
                //else
                //    Municipios = new ObservableCollection<MUNICIPIO>();
                //Municipios.Insert(0, new MUNICIPIO { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                //if (EstadoJuzgadoCP == 33)
                //{
                //    MunicipioJuzgadoCP = 1001;
                //    MunicipioJuzgadoEnabled = false;
                //}
                //else
                //{
                //    MunicipioJuzgadoCP = -1;
                //    MunicipioJuzgadoEnabled = true;
                //}
                #endregion
                OnPropertyValidateChanged("SelectedEstado");
            }
        }
        
        private MUNICIPIO selectedMunicipio;
        public MUNICIPIO SelectedMunicipio
        {
            get { return selectedMunicipio; }
            set { selectedMunicipio = value; OnPropertyValidateChanged("SelectedMunicipio"); }
        }
        
        private JUZGADO selectedJuzgado;
        public JUZGADO SelectedJuzgado
        {
            get { return selectedJuzgado; }
            set { selectedJuzgado = value; OnPropertyValidateChanged("SelectedJuzgado"); }
        }
        
        private CAUSA_PENAL selectedCausaPenal;
        public CAUSA_PENAL SelectedCausaPenal
        {
            get { return selectedCausaPenal; }
            set
            {
                selectedCausaPenal = value;
                if (selectedCausaPenal != null)
                {
                    GuardarBandera = true;
                    DCausaPenal = true;
                }
                else
                { 
                    GuardarBandera = false;
                    DCausaPenal = false;
                }
                OnPropertyChanged("SelectedCausaPenal");
            }
        }

        private string observacionesCP;
        public string ObservacionesCP
        {
            get { return observacionesCP; }
            set { observacionesCP = value; OnPropertyValidateChanged("ObservacionesCP"); }
        }

        private VM_IMPUTADOSDATOS selectedInterconexion;
        public VM_IMPUTADOSDATOS SelectedInterconexion
        {
            get { return selectedInterconexion; }
            set { selectedInterconexion = value; OnPropertyChanged("SelectedInterconexion"); }
        }
        #endregion

        #region COMBOBOX
        private ObservableCollection<PAIS_NACIONALIDAD> lstPaises;
        public ObservableCollection<PAIS_NACIONALIDAD> LstPaises
        {
            get { return lstPaises; }
            set { lstPaises = value; OnPropertyChanged("LstPaises"); }
        }
        private ObservableCollection<ENTIDAD> lstEntidades;
        public ObservableCollection<ENTIDAD> LstEntidades
        {
            get { return lstEntidades; }
            set { lstEntidades = value; OnPropertyChanged("LstEntidades"); }
        }
        private ObservableCollection<MUNICIPIO> lstMunicipios;
        public ObservableCollection<MUNICIPIO> LstMunicipios
        {
            get { return lstMunicipios; }
            set { lstMunicipios = value; OnPropertyChanged("LstMunicipios"); }
        }
        private ObservableCollection<TIPO_ORDEN> lstTiposOrden;
        public ObservableCollection<TIPO_ORDEN> LstTiposOrden
        {
            get { return lstTiposOrden; }
            set { lstTiposOrden = value; OnPropertyChanged("LstTiposOrden"); }
        }
        private ObservableCollection<TERMINO> lstTerminos;
        public ObservableCollection<TERMINO> LstTerminos
        {
            get { return lstTerminos; }
            set { lstTerminos = value; OnPropertyChanged("LstTerminos"); }
        }
        private ObservableCollection<FUERO> lstFueros;
        public ObservableCollection<FUERO> LstFueros
        {
            get { return lstFueros; }
            set { lstFueros = value; OnPropertyChanged("LstFueros"); }
        }
        private ObservableCollection<AGENCIA> lstAgencias;
        public ObservableCollection<AGENCIA> LstAgencias
        {
            get { return lstAgencias; }
            set { lstAgencias = value; OnPropertyChanged("LstAgencias"); }
        }
        private ObservableCollection<JUZGADO> lstJuzgados;
        public ObservableCollection<JUZGADO> LstJuzgados
        {
            get { return lstJuzgados; }
            set { lstJuzgados = value; OnPropertyChanged("LstJuzgados"); }
        }
        private bool tabCausaPenalSelected;
        public bool TabCausaPenalSelected
        {
            get { return tabCausaPenalSelected; }
            set { tabCausaPenalSelected = value; OnPropertyChanged("TabCausaPenalSelected"); }
        }
        private string tituloCausasPenales = "Causas Penales(0)";
        public string TituloCausasPenales
        {
            get { return tituloCausasPenales; }
            set { tituloCausasPenales = value; OnPropertyChanged("TituloCausasPenales"); }
        }
        #endregion

        #region CAUSA PENAL ESTATUS
        private ObservableCollection<CAUSA_PENAL_ESTATUS> lstCPEstatus;
        public ObservableCollection<CAUSA_PENAL_ESTATUS> LstCPEstatus
        {
            get { return lstCPEstatus; }
            set { lstCPEstatus = value; OnPropertyChanged("LstCPEstatus"); }
        }
        #endregion

        #region INGRESOS
        private ObservableCollection<CausaPenalIngreso> causasPenalesIngreso;
        public ObservableCollection<CausaPenalIngreso> CausasPenalesIngreso
        {
            get { return causasPenalesIngreso; }
            set { causasPenalesIngreso = value; OnPropertyChanged("CausasPenalesIngreso"); }
        }
        private CausaPenalIngreso selectedCausaPenalIngreso;
        public CausaPenalIngreso SelectedCausaPenalIngreso
        {
            get { return selectedCausaPenalIngreso; }
            set
            {
                selectedCausaPenalIngreso = value;
                if (selectedCausaPenalIngreso != null)
                {
                    if (selectedCausaPenalIngreso.CausaPenal != null)
                        SelectedCausaPenal = selectedCausaPenalIngreso.CausaPenal;
                    else
                        SelectedCausaPenal = null;
                }
                else
                    SelectedCausaPenal = null;
                OnPropertyChanged("SelectedCausaPenalIngreso");
            }
        }
        #endregion

        #region ARBOL OPCIONES
        private ICollection<OpcionesArbol> opciones;
        public ICollection<OpcionesArbol> Opciones
        {
            get { return opciones; }
            set { opciones = value; OnPropertyChanged("Opciones"); }
        }
        #endregion

        #region ArbolClickDelitos
        private CAUSA_PENAL_DELITO selectedCPD;
        #endregion

        #region Otros
        private bool guardarBandera = false;
        public bool GuardarBandera
        {
            get { return guardarBandera; }
            set { guardarBandera = value; OnPropertyChanged("GuardarBandera"); }
        }
        private bool estadoJuzgadoEnabled;
        public bool EstadoJuzgadoEnabled
        {
            get { return estadoJuzgadoEnabled; }
            set { estadoJuzgadoEnabled = value; OnPropertyChanged("EstadoJuzgadoEnabled"); }
        }
        private bool municipioJuzgadoEnabled;
        public bool MunicipioJuzgadoEnabled
        {
            get { return municipioJuzgadoEnabled; }
            set { municipioJuzgadoEnabled = value; OnPropertyChanged("MunicipioJuzgadoEnabled"); }
        }
        #endregion
    }
    
    class OpcionesArbol
    {
        private string descr;
        public string Descr
        {
            get { return descr; }
            set { descr = value; }
        }
    }
}
