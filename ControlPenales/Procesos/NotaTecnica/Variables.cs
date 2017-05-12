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
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using ControlPenales.Clases;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Windows.Interop;
using System.IO;
using System.Windows.Controls;
using ControlPenales.BiometricoServiceReference;
using System.Windows.Media;
using Microsoft.Reporting.WinForms;


namespace ControlPenales
{
    partial class NotaTecnicaViewModel : ValidationViewModelBase
    {
        #region Area Tecnica
        private ObservableCollection<AREA_TECNICA> lstAreaTecnica;
        public ObservableCollection<AREA_TECNICA> LstAreaTecnica
        {
            get { return lstAreaTecnica; }
            set { lstAreaTecnica = value; OnPropertyChanged("LstAreaTecnica"); }
        }

        private short? areaTecnica = -1;
        public short? AreaTecnica
        {
            get { return areaTecnica; }
            set { areaTecnica = value; OnPropertyChanged("AreaTecnica"); }
        }

        private AREA_TECNICA selectedAreaTecnica;
        public AREA_TECNICA SelectedAreaTecnica
        {
            get { return selectedAreaTecnica; }
            set { selectedAreaTecnica = value; OnPropertyChanged("SelectedAreaTecnica"); }
        }
        #endregion

        #region Atencion Cita
        private ATENCION_CITA selectedAtencionCita;
        public ATENCION_CITA SelectedAtencionCita
        {
            get { return selectedAtencionCita; }
            set { selectedAtencionCita = value; OnPropertyChanged("SelectedAtencionCita"); }
        }

        private ATENCION_RECIBIDA SelectedAtencionRecibida;

        private string atencionTxt;
        public string AtencionTxt
        {
            get { return atencionTxt; }
            set { atencionTxt = value; OnPropertyChanged("AtencionTxt"); }
        }

        private ReportViewer reporte;
        public ReportViewer Reporte
        {
            get { return reporte; }
            set { reporte = value; OnPropertyChanged("Reporte"); }
        }
        #endregion

        #region Historico
        private ObservableCollection<ATENCION_RECIBIDA> lstAtencionRecibidaHistorico;
        public ObservableCollection<ATENCION_RECIBIDA> LstAtencionRecibidaHistorico
        {
            get { return lstAtencionRecibidaHistorico; }
            set { lstAtencionRecibidaHistorico = value; OnPropertyChanged("LstAtencionRecibidaHistorico"); }
        }

        private ATENCION_RECIBIDA selectedAtencionRecibidaHistorico;
        public ATENCION_RECIBIDA SelectedAtencionRecibidaHistorico
        {
            get { return selectedAtencionRecibidaHistorico; }
            set { selectedAtencionRecibidaHistorico = value; OnPropertyChanged("SelectedAtencionRecibidaHistorico"); }
        }
     
        #endregion

        #region Imputados
        private IMPUTADO Imputado;
        #endregion

        #region Datos Generales
        private int? folioD;
        public int? FolioD
        {
            get { return folioD; }
            set { folioD = value; OnPropertyChanged("FolioD"); }
        }

        private int? anioD;
        public int? AnioD
        {
            get { return anioD; }
            set { anioD = value; OnPropertyChanged("AnioD"); }
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

        private DateTime? fecIngresoD = null;
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
        #endregion

        #region [Huellas]
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

        private ImageSource _PulgarDerechoBMP;
        public ImageSource PulgarDerechoBMP
        {
            get { return _PulgarDerechoBMP; }
            set
            {
                _PulgarDerechoBMP = value;
                RaisePropertyChanged("PulgarDerechoBMP");
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

        #region Pantalla
        private bool tabControlEnabled = false;
        public bool TabControlEnabled
        {
            get { return tabControlEnabled; }
            set { tabControlEnabled = value; OnPropertyChanged("TabControlEnabled"); }
        }

        private TXTextControl.WPF.TextControl editor;
        public TXTextControl.WPF.TextControl Editor
        {
            get { return editor; }
            set { editor = value; OnPropertyChanged("Editor"); }
        }

        private Visibility historicoVisible = Visibility.Collapsed;
        public Visibility HistoricoVisible
        {
            get { return historicoVisible; }
            set { historicoVisible = value; OnPropertyChanged("HistoricoVisible"); }
        }

        private bool menuGuardarEnabled = true;
        public bool MenuGuardarEnabled
        {
            get { return menuGuardarEnabled; }
            set { menuGuardarEnabled = value; OnPropertyChanged("MenuGuardarEnabled"); }
        }

        private bool menuBuscarEnabled = true;
        public bool MenuBuscarEnabled
        {
            get { return menuBuscarEnabled; }
            set { menuBuscarEnabled = value; OnPropertyChanged("MenuBuscarEnabled"); }
        }

        private bool menuReporteEnabled = true;
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

        private bool menuLimpiarEnabled = true;
        public bool MenuLimpiarEnabled
        {
            get { return menuLimpiarEnabled; }
            set { menuLimpiarEnabled = value; OnPropertyChanged("MenuLimpiarEnabled"); }
        }
        
        private bool bHuellasEnabled = true;
        public bool BHuellasEnabled
        {
            get { return bHuellasEnabled; }
            set { bHuellasEnabled = value; OnPropertyChanged("BHuellasEnabled"); }
        }

        private bool areaTecnicaEnabled = false;
        public bool AreaTecnicaEnabled
        {
            get { return areaTecnicaEnabled; }
            set { areaTecnicaEnabled = value; OnPropertyChanged("AreaTecnicaEnabled"); }
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

        #region Reporte
        private byte[] logo1 = Parametro.REPORTE_LOGO1;
        private byte[] logo2 = Parametro.REPORTE_LOGO2;
        private string encabezado1 = Parametro.ENCABEZADO1;
        private string encabezado2 = Parametro.ENCABEZADO2;
        #endregion

        #region Buscar
        private int? anioBuscar;
        public int? AnioBuscar
        {
            get { return anioBuscar; }
            set { anioBuscar = value;
            if (value != null)
                AnioD = value;

                OnPropertyChanged("AnioBuscar"); }
        }

        private int? folioBuscar;
        public int? FolioBuscar
        {
            get { return folioBuscar; }
            set { folioBuscar = value;
            if (value != null)
                FolioD = value;
                OnPropertyChanged("FolioBuscar"); }
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
                    AreaTecnicaEnabled = false;
                    AreaTecnica = -1;
                    OnPropertyChanged("SelectIngreso");
                    return; 
                }
                AreaTecnicaEnabled = true;

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

        private INGRESO AuxIngreso = null;

        private INGRESO selectedIngreso;
        public INGRESO SelectedIngreso
        {
            get { return selectedIngreso; }
            set { selectedIngreso = value; OnPropertyChanged("SelectedIngreso"); }
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

        #region Auxiliares
        private short?[] EstatusInactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
        #endregion

    }
}
