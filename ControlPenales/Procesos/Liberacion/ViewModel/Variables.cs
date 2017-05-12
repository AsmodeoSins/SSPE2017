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
using ControlPenales.BiometricoServiceReference;

namespace ControlPenales
{
    partial class LiberacionViewModel : ValidationViewModelBase, IPageViewModel
    {
        #region GENERALES
        public string Name
        {
            get
            {
                return "";
            }
        }

        private bool edicion = true;//false;
        public bool Edicion
        {
            get { return edicion; }
            set { edicion = value; OnPropertyChanged("Edicion"); }
        }
        #endregion

        #region Buscar
        private int? anioBuscar;
        public int? AnioBuscar
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

        private string nombreBuscar;
        public string NombreBuscar
        {
            get { return nombreBuscar; }
            set { nombreBuscar = value; OnPropertyChanged("NombreBuscar"); }
        }

        private string apellidoPaternoBuscar;
        public string ApellidoPaternoBuscar
        {
            get { return apellidoPaternoBuscar; }
            set { apellidoPaternoBuscar = value; OnPropertyChanged("ApellidoPaternoBuscar"); }
        }

        private string apellidoMaternoBuscar;
        public string ApellidoMaternoBuscar
        {
            get { return apellidoMaternoBuscar; }
            set { apellidoMaternoBuscar = value; OnPropertyChanged("ApellidoMaternoBuscar"); }
        }

        private RangeEnabledObservableCollection<IMPUTADO> listExpediente;
        public RangeEnabledObservableCollection<IMPUTADO> ListExpediente
        {
            get { return listExpediente; }
            set { listExpediente = value; OnPropertyChanged("ListExpediente"); }
        }

        private bool emptyExpedienteVisible = false;
        public bool EmptyExpedienteVisible
        {
            get { return emptyExpedienteVisible; }
            set { emptyExpedienteVisible = value; OnPropertyChanged("EmptyExpedienteVisible"); }
        }

        private bool emptyIngresoVisible = false;
        public bool EmptyIngresoVisible
        {
            get { return emptyIngresoVisible; }
            set { emptyIngresoVisible = value; OnPropertyChanged("EmptyIngresoVisible"); }
        }

        private IMPUTADO selectExpediente;
        public IMPUTADO SelectExpediente
        {
            get { return selectExpediente; }
            set
            {
                selectExpediente = value;
                if (selectExpediente != null)
                {
                    //MUESTRA LOS INGRESOS
                    if (selectExpediente.INGRESO!=null && selectExpediente.INGRESO.Count > 0)
                    {
                        EmptyIngresoVisible = false;
                        SelectIngreso = selectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                    }
                    else
                        EmptyIngresoVisible = true;

                    //OBTENEMOS FOTO DE FRENTE
                    if (SelectIngreso != null)
                    {
                        if (SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                            ImagenImputado = SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                        else
                            ImagenImputado = new Imagenes().getImagenPerson();
                    }
                    else
                        ImagenImputado = new Imagenes().getImagenPerson();
                }
                else
                {
                    ImagenImputado = new Imagenes().getImagenPerson();
                    EmptyIngresoVisible = true;
                }
                OnPropertyChanged("SelectExpediente");
            }
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
                    OnPropertyChanged("SelectIngreso");
                    return;
                }
                if (SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                    ImagenImputado = SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                else
                    ImagenImputado = new Imagenes().getImagenPerson();
                if (selectIngreso.INGRESO_BIOMETRICO.Any(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG))
                {
                    ImagenIngreso = selectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                }
                else
                    ImagenIngreso = new Imagenes().getImagenPerson();
                OnPropertyChanged("SelectIngreso");
            }
        }

        private INGRESO selectedIngreso;
        public INGRESO SelectedIngreso
        {
            get { return selectedIngreso; }
            set { selectedIngreso = value;
            if (value != null)
                Edicion = true;
            else
                Edicion = false;
                OnPropertyChanged("SelectedIngreso"); }
        }

        private bool crearNuevoExpedienteEnabled = false;
        public bool CrearNuevoExpedienteEnabled
        {
            get { return crearNuevoExpedienteEnabled; }
            set { crearNuevoExpedienteEnabled = value; OnPropertyChanged("CrearNuevoExpedienteEnabled"); }
        }

        private string textBotonSeleccionarIngreso = "Seleccionar Ingreso";
        public string TextBotonSeleccionarIngreso
        {
            get { return textBotonSeleccionarIngreso; }
            set { textBotonSeleccionarIngreso = value; }
        }

        private byte[] imagenIngreso = new Imagenes().getImagenPerson();
        public byte[] ImagenIngreso
        {
            get { return imagenIngreso; }
            set { imagenIngreso = value; OnPropertyChanged("ImagenIngreso"); }
        }
        
        private byte[] imagenImputado = new Imagenes().getImagenPerson();
        public byte[] ImagenImputado
        {
            get { return imagenImputado; }
            set { imagenImputado = value; OnPropertyChanged("ImagenImputado"); }
        }

        
        #endregion

        #region Kardex
        private short? anioD;
        public short? AnioD
        {
            get { return anioD; }
            set { anioD = value; OnPropertyChanged("AnioD"); }
        }

        private int? folioD;
        public int? FolioD
        {
            get { return folioD; }
            set { folioD = value; OnPropertyChanged("FolioD"); }
        }

        private string paternoD;
        public string PaternoD
        {
            get { return paternoD; }
            set { paternoD = value; OnPropertyChanged("PaternoD"); }
        }

        private string maternoD;
        public string MaternoD
        {
            get { return maternoD; }
            set { maternoD = value; OnPropertyChanged("MaternoD"); }
        }

        private string nombreD;
        public string NombreD
        {
            get { return nombreD; }
            set { nombreD = value; OnPropertyChanged("NombreD"); }
        }

        private short? ingresosD;
        public short? IngresosD
        {
            get { return ingresosD; }
            set { ingresosD = value; OnPropertyChanged("IngresosD"); }
        }

        private string noControlD;
        public string NoControlD
        {
            get { return noControlD; }
            set { noControlD = value; OnPropertyChanged("NoControlD"); }
        }

        private string ubicacionD;
        public string UbicacionD
        {
            get { return ubicacionD; }
            set { ubicacionD = value; OnPropertyChanged("UbicacionD"); }
        }

        private string tipoSeguridadD;
        public string TipoSeguridadD
        {
            get { return tipoSeguridadD; }
            set { tipoSeguridadD = value; OnPropertyChanged("TipoSeguridadD"); }
        }
        
        private DateTime? fecIngresoD;
        public DateTime? FecIngresoD
        {
            get { return fecIngresoD; }
            set { fecIngresoD = value; OnPropertyChanged("FecIngresoD"); }
        }

        private string clasificacionJuridicaD;
        public string ClasificacionJuridicaD
        {
            get { return clasificacionJuridicaD; }
            set { clasificacionJuridicaD = value; OnPropertyChanged("ClasificacionJuridicaD"); }
        }
        
        private string estatusD;
        public string EstatusD
        {
            get { return estatusD; }
            set { estatusD = value; OnPropertyChanged("EstatusD"); }
        }

        private int pagina;
        public int Pagina
        {
            get { return pagina; }
            set { pagina = value; OnPropertyChanged("Pagina"); }
        }

        private bool seguirCargando;
        public bool SeguirCargando
        {
            get { return seguirCargando; }
            set { seguirCargando = value; OnPropertyChanged("SeguirCargando"); }
        }
        #endregion

        #region Liberacion
        private ObservableCollection<LIBERACION_AUTORIDAD> lstAutoridad;
        public ObservableCollection<LIBERACION_AUTORIDAD> LstAutoridad
        {
            get { return lstAutoridad; }
            set { lstAutoridad = value; OnPropertyChanged("LstAutoridad"); }
        }

        private ObservableCollection<LIBERACION_MOTIVO> lstMotivo;
        public ObservableCollection<LIBERACION_MOTIVO> LstMotivo
        {
            get { return lstMotivo; }
            set { lstMotivo = value; OnPropertyChanged("LstMotivo"); }
        }

        private LIBERACION selectedLiberacion;
        public LIBERACION SelectedLiberacion
        {
            get { return selectedLiberacion; }
            set { selectedLiberacion = value; OnPropertyChanged("SelectedLiberacion"); }
        }

        private bool eFechaValid = false;
        public bool EFechaValid
        {
            get { return eFechaValid; }
            set { eFechaValid = value; OnPropertyChanged("EFechaValid"); }
        }

        private DateTime? eFecha;
        public DateTime? EFecha
        {
            get { return eFecha; }
            set { eFecha = value;
            if (value != null)
                EFechaValid = true;
            else
                EFechaValid = false;
                 OnPropertyValidateChanged("EFecha");
            }
        }

        private string eOficio;
        public string EOficio
        {
            get { return eOficio; }
            set { eOficio = value; OnPropertyValidateChanged("EOficio"); }
        }

        private short? eAutoridad;
        public short? EAutoridad
        {
            get { return eAutoridad; }
            set { eAutoridad = value; OnPropertyValidateChanged("EAutoridad"); }
        }

        private short? eMotivo;
        public short? EMotivo
        {
            get { return eMotivo; }
            set { eMotivo = value; OnPropertyValidateChanged("EMotivo"); }
        }
        #endregion

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

        #region CausasPenales
        private short? cPAnio;
        public short? CPAnio
        {
            get { return cPAnio; }
            set { cPAnio = value;
            if (cPAnio != null)
                if (SelectedCausaPenal != null)
                    SelectedCausaPenal = null;
                OnPropertyChanged("CPAnio"); }
        }

        private int? cPFolio;
        public int? CPFolio
        {
            get { return cPFolio; }
            set { cPFolio = value;
            if (cPFolio != null)
            if (SelectedCausaPenal != null)
                SelectedCausaPenal = null;
               OnPropertyChanged("CPFolio");
            }
        }

        private ObservableCollection<cCausaPenalLiberacion> lstLiberacionCausaPenal;
        public ObservableCollection<cCausaPenalLiberacion> LstLiberacionCausaPenal
        {
            get { return lstLiberacionCausaPenal; }
            set { lstLiberacionCausaPenal = value; OnPropertyChanged("LstLiberacionCausaPenal"); }
        }

        private cCausaPenalLiberacion selectedLiberacionCausaPenal;
        public cCausaPenalLiberacion SelectedLiberacionCausaPenal
        {
            get { return selectedLiberacionCausaPenal; }
            set { selectedLiberacionCausaPenal = value; OnPropertyChanged("SelectedLiberacionCausaPenal"); }
        }

        private ObservableCollection<CAUSA_PENAL> lstCausaPenal;
        public ObservableCollection<CAUSA_PENAL> LstCausaPenal
        {
            get { return lstCausaPenal; }
            set { lstCausaPenal = value; OnPropertyChanged("LstCausaPenal"); }
        }

        private CAUSA_PENAL selectedCausaPenal;
        public CAUSA_PENAL SelectedCausaPenal
        {
            get { return selectedCausaPenal; }
            set { selectedCausaPenal = value;
            if (value != null)
            {
                #region Validaciones
                base.RemoveRule("CPAnio");
                base.RemoveRule("CPFolio");
                OnPropertyChanged("CPAnio");
                OnPropertyChanged("CPFolio");
                CPAnio = null;
                CPFolio = null;
                #endregion
                Edicion = true;
                if (value.ID_CAUSA_PENAL == 0)
                {
                    LimpiarEgreso();
                    SelectedLiberacion = value.LIBERACION.FirstOrDefault();
                    if (SelectedLiberacion != null)
                    {
                        var x = SelectedLiberacion;
                        ObtenerEgreso();
                        SelectedLiberacion = x;
                    }
                }
                else
                if (value.LIBERACION != null)
                {
                    LimpiarEgreso();
                    SelectedLiberacion = value.INGRESO.LIBERACION.Where(w => w.ID_CAUSA_PENAL == value.ID_CAUSA_PENAL).FirstOrDefault();
                    if (SelectedLiberacion != null)
                    {
                        ObtenerEgreso();
                    }
                }
                else
                    SelectedLiberacion = null;

                if (value.ID_ESTATUS_CP == 4)
                    Edicion = false;
            }
            else
            {
                #region Validaciones
                base.RemoveRule("CPAnio");
                base.RemoveRule("CPFolio");
                base.AddRule(() => CPAnio, () => CPAnio.HasValue, "AÑO DE LA CAUSA PENAL ES REQUERIDA!");
                base.AddRule(() => CPFolio, () => CPFolio.HasValue, "FOLIO DE LA CASA PENAL ES REQUERIDA!");
                OnPropertyChanged("CPAnio");
                OnPropertyChanged("CPFolio");
                #endregion
                Edicion = true;
                SelectedLiberacion = null; 
            }
                StaticSourcesViewModel.SourceChanged = false;
                OnPropertyChanged("SelectedCausaPenal"); }
        }

        private Visibility causasPenalesVisible = Visibility.Collapsed;
        public Visibility CausasPenalesVisible
        {
            get { return causasPenalesVisible; }
            set { causasPenalesVisible = value; OnPropertyChanged("CausasPenalesVisible"); }
        }
        #endregion

        #region ProximaCausaPenal
        private ObservableCollection<CAUSA_PENAL> lstProximaCausaPenal;
        public ObservableCollection<CAUSA_PENAL> LstProximaCausaPenal
        {
            get { return lstProximaCausaPenal; }
            set { lstProximaCausaPenal = value; OnPropertyChanged("LstProximaCausaPenal"); }
        }

        private CAUSA_PENAL selectedProximaCausaPenal;
        public CAUSA_PENAL SelectedProximaCausaPenal
        {
            get { return selectedProximaCausaPenal; }
            set { selectedProximaCausaPenal = value; OnPropertyChanged("SelectedProximaCausaPenal"); }
        }
        #endregion

        #region Medidas Judiciales
        private bool mjActividadesEvaluacion = false;
        private bool mjEvaluacionRiesgos = false;
        private bool mjOpinionTecnica = false;
        private string mjOtra;

        private bool mjMC = false;
        private bool mjSPP = false;
        private bool mjPROVP = false;
        private string mjNUC;
        private string mjCP;

        private string mjDelitos;
        private bool? mjRecalificado;

        private string mjMedidaJudicial;
        private string mjPeridiocidad;
        private DateTime? mjApartirDia;
        private string mjDuracion;

        private short? mjEstadoCivil = -1;
        private short? mjOcupacion = -1;
        private string mjLugarOcupacion;

        //private string
        //private 
        
        #endregion
    }
}
