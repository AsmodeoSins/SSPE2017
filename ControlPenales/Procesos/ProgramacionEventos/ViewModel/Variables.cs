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
using MahApps.Metro.Controls.Dialogs;
using SSP.Servidor;
using ControlPenales.Clases;
using System.Windows.Controls;

namespace ControlPenales
{
    partial class ProgramacionEventosViewModel 
    {

        #region Programacion Eventos
        private int PaginaEvento { get; set; }
        private bool SeguirCargandoEventos { get; set; }
        private RangeEnabledObservableCollection<EVENTO> lstEventos;
        public RangeEnabledObservableCollection<EVENTO> LstEventos
        {
            get { return lstEventos; }
            set { lstEventos = value; OnPropertyChanged("LstEventos"); }
        }

        private EVENTO selectedEvento;
        public EVENTO SelectedEvento
        {
            get { return selectedEvento; }
            set { selectedEvento = value;
            if (value != null)
            {
                if (PEditar)
                    MenuGuardarEnabled = true;
                else
                    MenuGuardarEnabled = false;
            }
            else
            {
                if(PInsertar)
                    MenuGuardarEnabled = true;
                else
                    MenuGuardarEnabled = false;
            }
                OnPropertyChanged("SelectedEvento"); }
        }

        private ObservableCollection<EVENTO_TIPO> lstEventoTipo;
        public ObservableCollection<EVENTO_TIPO> LstEventoTipo
        {
            get { return lstEventoTipo; }
            set { lstEventoTipo = value; OnPropertyChanged("LstEventoTipo"); }
        }

        private ObservableCollection<CENTRO> lstCentros;
        public ObservableCollection<CENTRO> LstCentros
        {
            get { return lstCentros; }
            set { lstCentros = value; OnPropertyChanged("LstCentros"); }
        }

        private CENTRO selectedCentro;
        public CENTRO SelectedCentro
        {
            get { return selectedCentro; }
            set { selectedCentro = value;
            if (value != null)
            {
                if (value.ID_CENTRO != -1)
                {
                    EDireccion = value.CALLE;
                    EEstado = value.ID_ENTIDAD.Value;
                    LstMunicipios = new ObservableCollection<MUNICIPIO>(SelectedEntidad.MUNICIPIO);
                    LstMunicipios.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                    EMunicipio = value.ID_MUNICIPIO.Value;
                    ETelefono = value.TELEFONO != null ? value.TELEFONO.ToString() : string.Empty;
                    ELugarEnabled = false;
                }
                else
                {
                    EDireccion = string.Empty;
                    EEstado = -1;
                    LstMunicipios = new ObservableCollection<MUNICIPIO>();
                    LstMunicipios.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                    EMunicipio = -1;
                    ELugarEnabled = true;
                }
            }
                OnPropertyChanged("SelectedCentro"); }
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

        private ENTIDAD selectedEntidad;
        public ENTIDAD SelectedEntidad
        {
            get { return selectedEntidad; }
            set { selectedEntidad = value;
            if (value != null)
            {
                LstMunicipios = new ObservableCollection<MUNICIPIO>(value.MUNICIPIO);
            }
            else
            {
                LstMunicipios = new ObservableCollection<MUNICIPIO>();
            }
            LstMunicipios.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
            EMunicipio = -1;
           
                OnPropertyChanged("SelectedEntidad"); }
        }

        private ObservableCollection<EVENTO_IMPACTO> lstEventoImpacto;
        public ObservableCollection<EVENTO_IMPACTO> LstEventoImpacto
        {
            get { return lstEventoImpacto; }
            set { lstEventoImpacto = value; OnPropertyChanged("LstEventoImpacto"); }
        }

        private ObservableCollection<EVENTO_VESTIMENTA> lstEventoVestimenta;
        public ObservableCollection<EVENTO_VESTIMENTA> LstEventoVestimenta
        {
            get { return lstEventoVestimenta; }
            set { lstEventoVestimenta = value; OnPropertyChanged("LstEventoVestimenta"); }
        }

        private ObservableCollection<EVENTO_ESTATUS> lstEventoEstatus;
        public ObservableCollection<EVENTO_ESTATUS> LstEventoEstatus
        {
            get { return lstEventoEstatus; }
            set { lstEventoEstatus = value; OnPropertyChanged("LstEventoEstatus"); }
        }

        private string eNombre;
        public string ENombre
        {
            get { return eNombre; }
            set { eNombre = value; OnPropertyValidateChanged("ENombre"); }
        }
        
        private short eEventoTipo = -1;
        public short EEventoTipo
        {
            get { return eEventoTipo; }
            set { eEventoTipo = value; OnPropertyValidateChanged("EEventoTipo"); }
        }

        private short eCentro = GlobalVar.gCentro;
        public short ECentro
        {
            get { return eCentro; }
            set { eCentro = value;
            ELugar = string.Empty;
            //if (value != -1)
            //{
            //    ELugarEnabled = false;
            //}
            //else
            //{
            //    ELugarEnabled = true;   
            //}
            ValidacionEvento();
            OnPropertyValidateChanged("ECentro");
            }
        }

        private string eLugar;
        public string ELugar
        {
            get { return eLugar; }
            set { eLugar = value; OnPropertyValidateChanged("ELugar"); }
        }

        private bool eLugarEnabled = true;
        public bool ELugarEnabled
        {
            get { return eLugarEnabled; }
            set { eLugarEnabled = value; OnPropertyChanged("ELugarEnabled"); }
        }

        private string eDireccion;
        public string EDireccion
        {
            get { return eDireccion; }
            set { eDireccion = value; OnPropertyValidateChanged("EDireccion"); }
        }

        private short eEstado = -1;
        public short EEstado
        {
            get { return eEstado; }
            set { eEstado = value; OnPropertyValidateChanged("EEstado"); }
        }

        private short eMunicipio = -1;
        public short EMunicipio
        {
            get { return eMunicipio; }
            set { eMunicipio = value; OnPropertyValidateChanged("EMunicipio"); }
        }

        private string eTelefono;
        public string ETelefono
        {
            get { 
                if (eTelefono == null)
                    return string.Empty;
                return new Converters().MascaraTelefono(eTelefono);
            }
            set { 
                eTelefono = value;
                OnPropertyValidateChanged("ETelefono"); }
        }

        private short? eDuracionHrs;
        public short? EDuracionHrs
        {
            get { return eDuracionHrs; }
            set { eDuracionHrs = value; OnPropertyValidateChanged("EDuracionHrs"); }
        }

        private short? eDuracionMin;
        public short? EDuracionMin
        {
            get { return eDuracionMin; }
            set { eDuracionMin = value; OnPropertyValidateChanged("EDuracionMin"); }
        }

        private bool eFechaAux = false;
        public bool EFechaAux
        {
            get { return eFechaAux; }
            set { eFechaAux = value; OnPropertyChanged("EFechaAux"); }
        }

        private string eMensajeErrorFecha = "La fecha es requerida!";
        public string EMensajeErrorFecha
        {
            get { return eMensajeErrorFecha; }
            set { eMensajeErrorFecha = value; OnPropertyChanged("EMensajeErrorFecha"); }
        }
        
        private DateTime? eFecha;
        public DateTime? EFecha
        {
            get { return eFecha; }
            set { eFecha = value;
            if (value.HasValue)
            {
                if (SelectedEvento == null)
                {
                    DateTime hoy = Fechas.GetFechaDateServer;
                    if (value.Value.Date < hoy.Date.Date)
                    {
                        EMensajeErrorFecha = "La fecha debe ser mayor o igual al dia de hoy!";
                        EFechaAux = false;
                    }
                    else
                    {
                        EFechaAux = true;
                    }
                }
                else
                    EFechaAux = true;
            }
            else
            {
                EMensajeErrorFecha = "La fecha es requerida!";
                EFechaAux = false; 
            }
            OnPropertyValidateChanged("EFecha");
            }
        }

        private DateTime? eHoraInvitados;
        public DateTime? EHoraInvitados
        {
            get { return eHoraInvitados; }
            set { eHoraInvitados = value; OnPropertyValidateChanged("EHoraInvitados"); }
        }

        private DateTime? eHoraPresidium;
        public DateTime? EHoraPresidium
        {
            get { return eHoraPresidium; }
            set { eHoraPresidium = value; OnPropertyValidateChanged("EHoraPresidium"); }
        }

        private string eDependencia;
        public string EDependencia
        {
            get { return eDependencia; }
            set { eDependencia = value; OnPropertyValidateChanged("EDependencia"); }
        }

        private string ePerfilInvitados;
        public string EPerfilInvitados
        {
            get { return ePerfilInvitados; }
            set { ePerfilInvitados = value; OnPropertyValidateChanged("EPerfilInvitados"); }
        }

        private string eObjetivo;
        public string EObjetivo
        {
            get { return eObjetivo; }
            set { eObjetivo = value; OnPropertyValidateChanged("EObjetivo"); }
        }

        private string eObjetivoGral;
        public string EObjetivoGral
        {
            get { return eObjetivoGral; }
            set { eObjetivoGral = value; OnPropertyValidateChanged("EObjetivoGral"); }
        }

        private string eMaestroCeremonias;
        public string EMaestroCeremonias
        {
            get { return eMaestroCeremonias; }
            set { eMaestroCeremonias = value; OnPropertyValidateChanged("EMaestroCeremonias"); }
        }

        private bool eComite = false;
        public bool EComite
        {
            get { return eComite; }
            set { eComite = value; OnPropertyChanged("EComite"); }
        }

        private string eObservacion;
        public string EObservacion
        {
            get { return eObservacion; }
            set { eObservacion = value; OnPropertyValidateChanged("EObservacion"); }
        }

        private short eImpactoEvento =-1;
        public short EImpactoEvento
        {
            get { return eImpactoEvento; }
            set { eImpactoEvento = value; OnPropertyValidateChanged("EImpactoEvento"); }
        }

        private short eVestimentaSugerida =-1;
        public short EVestimentaSugerida
        {
            get { return eVestimentaSugerida; }
            set { eVestimentaSugerida = value; OnPropertyValidateChanged("EVestimentaSugerida"); }
        }

        private bool eConvocartoriaMedios = false;
        public bool EConvocartoriaMedios
        {
            get { return eConvocartoriaMedios; }
            set { eConvocartoriaMedios = value; OnPropertyValidateChanged("EConvocartoriaMedios"); }
        }

        private string eComentariosAdicionales;
        public string EComentariosAdicionales
        {
            get { return eComentariosAdicionales; }
            set { eComentariosAdicionales = value; OnPropertyValidateChanged("EComentariosAdicionales"); }
        }

        private string eInformacionBasica;
        public string EInformacionBasica
        {
            get { return eInformacionBasica; }
            set { eInformacionBasica = value; OnPropertyValidateChanged("EInformacionBasica"); }
        }

        private string eInformacionTecnica;
        public string EInformacionTecnica
        {
            get { return eInformacionTecnica; }
            set { eInformacionTecnica = value; OnPropertyValidateChanged("EInformacionTecnica"); }
        }

        private short eEstatus = 1;
        public short EEstatus
        {
            get { return eEstatus; }
            set { eEstatus = value; OnPropertyValidateChanged("EEstatus"); }
        }

        private bool eEstatusEnabled = false;
        public bool EEstatusEnabled
        {
            get { return eEstatusEnabled; }
            set { eEstatusEnabled = value; OnPropertyChanged("EEstatusEnabled"); }
        }

        private ObservableCollection<EMPLEADO> listResponsables;
        public ObservableCollection<EMPLEADO> ListResponsables
        {
            get { return listResponsables; }
            set { listResponsables = value; OnPropertyChanged("ListResponsables"); }
        }

        private int? selectResponsable = -1;
        public int? SelectResponsable
        {
            get { return selectResponsable; }
            set { selectResponsable = value; OnPropertyChanged("SelectResponsable"); }
        }
        #endregion

        #region Evento Programa
        private ObservableCollection<EVENTO_PROGRAMA> lstEventoPrograma;
        public ObservableCollection<EVENTO_PROGRAMA> LstEventoPrograma
        {
            get { return lstEventoPrograma; }
            set { lstEventoPrograma = value; OnPropertyValidateChanged("LstEventoPrograma"); }
        }

        private EVENTO_PROGRAMA selectedEventoPrograma;
        public EVENTO_PROGRAMA SelectedEventoPrograma
        {
            get { return selectedEventoPrograma; }
            set { selectedEventoPrograma = value; OnPropertyChanged("SelectedEventoPrograma"); }
        }

        private string ePDescripcion;
        public string EPDescripcion
        {
            get { return ePDescripcion; }
            set { ePDescripcion = value; OnPropertyChanged("EPDescripcion"); }
        }

        private string test;
        public string Test
        {
            get { return test; }
            set { test = value; OnPropertyChanged("Test"); }
        }
        private string ePDuracion;
        public string EPDuracion
        {
            get { return ePDuracion; }
            set { ePDuracion = value; OnPropertyChanged("EPDuracion"); }
        }

        private Visibility eProgramaVisible = Visibility.Visible;
        public Visibility EProgramaVisible
        {
            get { return eProgramaVisible; }
            set { eProgramaVisible = value; OnPropertyChanged("EProgramaVisible"); }
        }

        private string eProgramasTitulo = "Agregar Programa";
        public string EProgramasTitulo
        {
            get { return eProgramasTitulo; }
            set { eProgramasTitulo = value; OnPropertyChanged("EProgramasTitulo"); }
        }

        private int pIndex;
        public int PIndex
        {
            get { return pIndex; }
            set { pIndex = value; OnPropertyChanged("PIndex"); }
        }
        #endregion

        #region Evento Presidium
        private ObservableCollection<EVENTO_PRESIDIUM> lstEventoPresidium;
        public ObservableCollection<EVENTO_PRESIDIUM> LstEventoPresidium
        {
            get { return lstEventoPresidium; }
            set { lstEventoPresidium = value; OnPropertyValidateChanged("LstEventoPresidium"); }
        }

        private EVENTO_PRESIDIUM selectedEventoPresidium;
        public EVENTO_PRESIDIUM SelectedEventoPresidium
        {
            get { return selectedEventoPresidium; }
            set { selectedEventoPresidium = value; OnPropertyChanged("SelectedEventoPresidium"); }
        }

        private string ePNombre;
        public string EPNombre
        {
            get { return ePNombre; }
            set { ePNombre = value; OnPropertyChanged("EPNombre"); }
        }

        private string ePPuesto;
        public string EPPuesto
        {
            get { return ePPuesto; }
            set { ePPuesto = value; OnPropertyChanged("EPPuesto"); }
        }

        private Visibility ePresidiumVisible = Visibility.Visible;
        public Visibility EPresidiumVisible
        {
            get { return ePresidiumVisible; }
            set { ePresidiumVisible = value; OnPropertyChanged("EPresidiumVisible"); }
        }

        private string ePresidiumTitulo = "Agregar Presidium";
        public string EPresidiumTitulo
        {
            get { return ePresidiumTitulo; }
            set { ePresidiumTitulo = value; OnPropertyChanged("EPresidiumTitulo"); }
        }

        private int pRIndex;
        public int PRIndex
        {
            get { return pRIndex; }
            set { pRIndex = value; OnPropertyChanged("PRIndex"); }
        }
        #endregion
   
        #region Evento Inf Tecnica
        private ObservableCollection<EVENTO_INF_TECNICA> lstEventoInfTecnica;
        public ObservableCollection<EVENTO_INF_TECNICA> LstEventoInfTecnica
        {
            get { return lstEventoInfTecnica; }
            set { lstEventoInfTecnica = value; OnPropertyValidateChanged("LstEventoInfTecnica"); }
        }

        private EVENTO_INF_TECNICA selectedEventoInfTecnica;
        public EVENTO_INF_TECNICA SelectedEventoInfTecnica
        {
            get { return selectedEventoInfTecnica; }
            set { selectedEventoInfTecnica = value; OnPropertyChanged("SelectedEventoInfTecnica"); }
        }

        private string eTDescripcion;
        public string ETDescripcion
        {
            get { return eTDescripcion; }
            set { eTDescripcion = value; OnPropertyChanged("ETDescripcion"); }
        }

        private Visibility eInfTecnicaVisible = Visibility.Visible;
        public Visibility EInfTecnicaVisible
        {
            get { return eInfTecnicaVisible; }
            set { eInfTecnicaVisible = value; OnPropertyChanged("EInfTecnicaVisible"); }
        }
      
        private string eInfTecnicaTitulo = "Agregar Informacion Técnica";
        public string EInfTecnicaTitulo
        {
            get { return eInfTecnicaTitulo; }
            set { eInfTecnicaTitulo = value; OnPropertyChanged("EInfTecnicaTitulo"); }
        }
        #endregion

        #region Buscar
        private string bNombre;
        public string BNombre
        {
            get { return bNombre; }
            set { bNombre = value; OnPropertyChanged("BNombre"); }
        }
        
        private short bTipo = -1;
        public short BTipo
        {
            get { return bTipo; }
            set { bTipo = value; OnPropertyChanged("BTipo"); }
        }
        
        private DateTime? bFecha;
        public DateTime? BFecha
        {
            get { return bFecha; }
            set { bFecha = value; OnPropertyChanged("BFecha"); }
        }

        private Visibility bEventosEmpty = Visibility.Visible;
        public Visibility BEventosEmpty
        {
            get { return bEventosEmpty; }
            set { bEventosEmpty = value; OnPropertyChanged("BEventosEmpty"); }
        }
        #endregion

        #region Internos
        private int Pagina { get; set; }
        private bool SeguirCargandoIngresos { get; set; }

        private RangeEnabledObservableCollection<cTrasladoIngreso> lstIngresos;
        public RangeEnabledObservableCollection<cTrasladoIngreso> LstIngresos
        {
            get { return lstIngresos; }
            set { lstIngresos = value; OnPropertyChanged("LstIngresos"); }
        }

        private cTrasladoIngreso selectedIngreso;
        public cTrasladoIngreso SelectedIngreso
        {
            get { return selectedIngreso; }
            set { selectedIngreso = value; OnPropertyChanged("SelectedIngreso"); }
        }

        private ObservableCollection<INGRESO> lstIngresosSeleccionados;
        public ObservableCollection<INGRESO> LstIngresosSeleccionados
        {
            get { return lstIngresosSeleccionados; }
            set { lstIngresosSeleccionados = value;
            if (value != null)
            {
                if (value.Count > 0)
                {
                    base.RemoveRule("LstIngresosSeleccionados");
                }
                else
                {
                    base.RemoveRule("LstIngresosSeleccionados");
                    base.AddRule(() => LstIngresosSeleccionados, () => LstIngresosSeleccionados != null ? LstIngresosSeleccionados.Count > 0 : false, "INTERNOS PARTICIPANTES SON REUQERIDOS!");
                }
            }
            else
            {
                base.RemoveRule("LstIngresosSeleccionados");
                base.AddRule(() => LstIngresosSeleccionados, () => LstIngresosSeleccionados != null ? LstIngresosSeleccionados.Count > 0 : false, "INTERNOS PARTICIPANTES SON REUQERIDOS!");
            }
            
                OnPropertyChanged("LstIngresosSeleccionados"); }
        }

        private INGRESO selectedIngresoSeleccionado;
        public INGRESO SelectedIngresoSeleccionado
        {
            get { return selectedIngresoSeleccionado; }
            set { selectedIngresoSeleccionado = value; OnPropertyValidateChanged("SelectedIngresoSeleccionado"); }
        }

        private short? iAnio;
        public short? IAnio
        {
            get { return iAnio; }
            set { iAnio = value; OnPropertyChanged("IAnio"); }
        }

        private int? iFolio;
        public int? IFolio
        {
            get { return iFolio; }
            set { iFolio = value; OnPropertyChanged("IFolio"); }
        }

        private string iNombre;
        public string INombre
        {
            get { return iNombre; }
            set { iNombre = value; OnPropertyChanged("INombre"); }
        }

        private string iPaterno;
        public string IPaterno
        {
            get { return iPaterno;}
            set { iPaterno = value; OnPropertyChanged("IPaterno"); }
        }

        private string iMaterno;
        public string IMaterno
        {
            get { return iMaterno; }
            set { iMaterno = value; OnPropertyChanged("IMaterno"); }
        }
        #endregion 

        #region Configuracion Permisos
        private bool pInsertar = false;
        public bool PInsertar
        {
            get { return pInsertar; }
            set { pInsertar = value;
            if (value)
                MenuGuardarEnabled = value;
            }
        }

        private bool pEditar = false;
        public bool PEditar
        {
            get { return pEditar; }
            set { pEditar = value;}
        }

        private bool pConsultar = false;
        public bool PConsultar
        {
            get { return pConsultar; }
            set { pConsultar = value;
            if (value)
                MenuBuscarEnabled = value;
            }
        }

        private bool pImprimir = false;
        public bool PImprimir
        {
            get { return pImprimir; }
            set { pImprimir = value;
            if (value)
                MenuReporteEnabled = value;
            }
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

        private bool menuReporteEnabled = false;
        public bool MenuReporteEnabled
        {
            get { return menuReporteEnabled; }
            set { menuReporteEnabled = value; OnPropertyChanged("MenuReporteEnabled"); }
        }

        private bool menuFichaEnabled = false;
        public bool MenuFichaEnabled
        {
            get { return menuFichaEnabled; }
            set { menuFichaEnabled = value; OnPropertyChanged("MenuFichaEnabled"); }
        }
       
        
        #endregion

        private short?[] estatus_inactivos = null;

    }
}
