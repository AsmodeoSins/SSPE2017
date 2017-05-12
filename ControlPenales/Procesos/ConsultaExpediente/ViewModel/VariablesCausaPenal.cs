using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace ControlPenales
{
    partial class ConsultaExpedienteInternoViewModel
    {
        private ObservableCollection<CAUSA_PENAL> _ListCausasPenales;
        public ObservableCollection<CAUSA_PENAL> ListCausasPenales
        {
            get { return _ListCausasPenales; }
            set { _ListCausasPenales = value; OnPropertyChanged("ListCausasPenales"); }
        }

        private CAUSA_PENAL _CausaPenalSeleccionada;
        public CAUSA_PENAL CausaPenalSeleccionada
        {
            get { return _CausaPenalSeleccionada; }
            set { _CausaPenalSeleccionada = value; OnPropertyChanged("CausaPenalSeleccionada"); }
        }

        #region AP
        private short? agenciaAP;
        public short? AgenciaAP
        {
            get { return agenciaAP; }
            set { agenciaAP = value; OnPropertyChanged("AgenciaAP"); }
        }

        private short? anioAP;
        public short? AnioAP
        {
            get { return anioAP; }
            set { anioAP = value; OnPropertyChanged("AnioAP"); }
        }

        private int? folioAP;
        public int? FolioAP
        {
            get { return folioAP; }
            set { folioAP = value; OnPropertyChanged("FolioAP"); }
        }

        private string averiguacionPreviaAP;
        public string AveriguacionPreviaAP
        {
            get { return averiguacionPreviaAP; }
            set { averiguacionPreviaAP = value; OnPropertyChanged("AveriguacionPreviaAP"); }
        }

        private DateTime? fecAveriguacionAP;
        public DateTime? FecAveriguacionAP
        {
            get { return fecAveriguacionAP; }
            set { fecAveriguacionAP = value; OnPropertyChanged("FecAveriguacionAP"); }
        }

        private DateTime? fecConsignacionAP;
        public DateTime? FecConsignacionAP
        {
            get { return fecConsignacionAP; }
            set { fecConsignacionAP = value; OnPropertyChanged("FecConsignacionAP"); }
        }
        #endregion

        #region CP
        private short? anioCP;
        public short? AnioCP
        {
            get { return anioCP; }
            set { anioCP = value; OnPropertyChanged("AnioCP"); }
        }

        private int? folioCP;
        public int? FolioCP
        {
            get { return folioCP; }
            set { folioCP = value; OnPropertyChanged("FolioCP"); }
        }

        private string bisCP;
        public string BisCP
        {
            get { return bisCP; }
            set { bisCP = value; OnPropertyChanged("BisCP"); }
        }

        private string foraneoCP;
        public string ForaneoCP
        {
            get { return foraneoCP; }
            set { foraneoCP = value; OnPropertyChanged("ForaneoCP"); }
        }

        private short? tipoOrdenCP;
        public short? TipoOrdenCP
        {
            get { return tipoOrdenCP; }
            set { tipoOrdenCP = value; OnPropertyChanged("TipoOrdenCP"); }
        }

        private short? paisJuzgadoCP;
        public short? PaisJuzgadoCP
        {
            get { return paisJuzgadoCP; }
            set
            {
                paisJuzgadoCP = value;
                OnPropertyChanged("PaisJuzgadoCP");
            }
        }

        private short? edificioI;
        public short? EdificioI
        {
            get { return edificioI; }
            set { edificioI = value; OnPropertyChanged("EdificioI"); }
        }

        private short? estadoJuzgadoCP;
        public short? EstadoJuzgadoCP
        {
            get { return estadoJuzgadoCP; }
            set
            {
                estadoJuzgadoCP = value;
                if (value.HasValue ? value.Value > 0 : false)
                    LstMunicipios = new ObservableCollection<MUNICIPIO>(new cMunicipio().ObtenerTodos("", value));

                OnPropertyChanged("EstadoJuzgadoCP");
            }
        }

        private short? municipioJuzgadoCP;
        public short? MunicipioJuzgadoCP
        {
            get { return municipioJuzgadoCP; }
            set
            {
                municipioJuzgadoCP = value;
                OnPropertyChanged("MunicipioJuzgadoCP");
            }
        }

        private string fueroCP;
        public string FueroCP
        {
            get { return fueroCP; }
            set
            {
                fueroCP = value;
                OnPropertyChanged("FueroCP");
            }
        }

        private short? juzgadoCP;
        public short? JuzgadoCP
        {
            get { return juzgadoCP; }
            set { juzgadoCP = value; OnPropertyChanged("JuzgadoCP"); }
        }

        private DateTime? fecRadicacionCP;
        public DateTime? FecRadicacionCP
        {
            get { return fecRadicacionCP; }
            set { fecRadicacionCP = value; OnPropertyChanged("FecRadicacionCP"); }
        }

        private string ampliacionCP;
        public string AmpliacionCP
        {
            get { return ampliacionCP; }
            set { ampliacionCP = value; OnPropertyChanged("AmpliacionCP"); }
        }

        private DateTime? fecVencimientoTerinoCP;
        public DateTime? FecVencimientoTerinoCP
        {
            get { return fecVencimientoTerinoCP; }
            set { fecVencimientoTerinoCP = value; OnPropertyChanged("FecVencimientoTerinoCP"); }
        }

        private short? terminoCP;
        public short? TerminoCP
        {
            get { return terminoCP; }
            set { terminoCP = value; OnPropertyChanged("TerminoCP"); }
        }

        private short? estatusCP;
        public short? EstatusCP
        {
            get { return estatusCP; }
            set
            {
                estatusCP = value;
                if (value == 0 || value == 1)//ACTIVO
                {
                    //ValidarCambioEstatus();
                }
                OnPropertyChanged("EstatusCP");
            }
        }

        private string juzgadoFuero;
        public string JuzgadoFuero
        {
            get { return juzgadoFuero; }
            set { juzgadoFuero = value; OnPropertyChanged("JuzgadoFuero"); }
        }

        private AGENCIA selectedAgencia;
        public AGENCIA SelectedAgencia
        {
            get { return selectedAgencia; }
            set { selectedAgencia = value; OnPropertyChanged("SelectedAgencia"); }
        }

        private PAIS_NACIONALIDAD selectedPais;
        public PAIS_NACIONALIDAD SelectedPais
        {
            get { return selectedPais; }
            set
            {
                selectedPais = value;
                OnPropertyChanged("SelectedPais");
            }
        }

        private ENTIDAD selectedEstado;
        public ENTIDAD SelectedEstado
        {
            get { return selectedEstado; }
            set
            {
                selectedEstado = value;
                OnPropertyChanged("SelectedEstado");
            }
        }

        private MUNICIPIO selectedMunicipio;
        public MUNICIPIO SelectedMunicipio
        {
            get { return selectedMunicipio; }
            set { selectedMunicipio = value; OnPropertyChanged("SelectedMunicipio"); }
        }

        private JUZGADO selectedJuzgado;
        public JUZGADO SelectedJuzgado
        {
            get { return selectedJuzgado; }
            set { selectedJuzgado = value; OnPropertyChanged("SelectedJuzgado"); }
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
                    GetDatosCausasPenales();
                    GuardarBandera = true;
                }
                else
                    GuardarBandera = false;
                OnPropertyChanged("SelectedCausaPenal");
            }
        }

        private string observacionesCP;
        public string ObservacionesCP
        {
            get { return observacionesCP; }
            set { observacionesCP = value; OnPropertyChanged("ObservacionesCP"); }
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
        public ObservableCollection<CausaPenalIngreso> ListCausasPenalesIngreso
        {
            get { return causasPenalesIngreso; }
            set { causasPenalesIngreso = value; OnPropertyChanged("ListCausasPenalesIngreso"); }
        }
        private CausaPenalIngreso selectedCausaPenalIngreso;
        public CausaPenalIngreso SelectedCausaPenalIngreso
        {
            get { return selectedCausaPenalIngreso; }
            set
            {
                selectedCausaPenalIngreso = value;
                if (value != null)
                {
                    if (value.CausaPenal != null)
                        SelectedCausaPenal = value.CausaPenal;
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
        private Visibility _BotonDigitalizacionNucVisible = Visibility.Collapsed;
        public Visibility BotonDigitalizacionNucVisible
        {
            get { return _BotonDigitalizacionNucVisible; }
            set { _BotonDigitalizacionNucVisible = value; OnPropertyChanged("BotonDigitalizacionNucVisible"); }
        }
        #endregion
    }
}
