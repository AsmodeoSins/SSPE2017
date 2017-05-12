using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ControlPenales
{
    partial class RegistroPertenenciasViewModel
    {
        #region [variables]
        private ImageSource _ImagenObjeto = new Imagenes().ConvertByteToBitmap(new Imagenes().getImagenObjetos());
        public ImageSource ImagenObjeto
        {
            get { return _ImagenObjeto; }
            set
            {
                _ImagenObjeto = value;
                OnPropertyChanged("ImagenObjeto");
            }
        }

        private byte[] _ImagenIngreso = new Imagenes().getImagenPerson();
        public byte[] ImagenIngreso
        {
            get { return _ImagenIngreso; }
            set
            {
                _ImagenIngreso = value;
                OnPropertyChanged("ImagenIngreso");
            }
        }

        private byte[] _ImagenImputado = new Imagenes().getImagenPerson();
        public byte[] ImagenImputado
        {
            get { return _ImagenImputado; }
            set
            {
                _ImagenImputado = value;
                OnPropertyChanged("ImagenImputado");
            }
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

        private string nombreBuscar;
        public string NombreBuscar
        {
            get { return nombreBuscar; }
            set
            {
                nombreBuscar = value; OnPropertyChanged("NombreBuscar");
            }
        }

        private int? _AnioBuscar { get; set; }
        private int? _FolioBuscar { get; set; }
        public int? AnioBuscar
        {
            get { return _AnioBuscar; }
            set
            {
                _AnioBuscar = value;
                OnPropertyChanged("AnioBuscar");
            }
        }
        public int? FolioBuscar
        {
            get { return _FolioBuscar; }
            set
            {
                _FolioBuscar = value;

                OnPropertyChanged("FolioBuscar");
            }
        }


        private string _PaternoD { get; set; }
        private string _MaternoD { get; set; }
        private string _NombreD { get; set; }
        public string PaternoD
        {
            get
            {
                return _PaternoD;
            }
            set
            {
                _PaternoD = value;
                OnPropertyChanged("PaternoD");
            }
        }

        public string MaternoD
        {
            get
            {
                return _MaternoD;
            }
            set
            {
                _MaternoD = value;
                OnPropertyChanged("MaternoD");
            }
        }
        public string NombreD
        {
            get
            {
                return _NombreD;
            }
            set
            {
                _NombreD = value;
                OnPropertyChanged("NombreD");
            }
        }


        private string _AnioD { get; set; }
        private string _FolioD { get; set; }


        public string AnioD
        {
            get
            {
                return _AnioD;
            }
            set
            {
                _AnioD = value;
                OnPropertyChanged("AnioD");
            }
        }
        public string FolioD
        {
            get
            {
                return _FolioD;
            }
            set
            {
                _FolioD = value;
                OnPropertyChanged("FolioD");
            }
        }

        private RangeEnabledObservableCollection<IMPUTADO> listExpediente;
        public RangeEnabledObservableCollection<IMPUTADO> ListExpediente
        {
            get { return listExpediente; }
            set
            {
                listExpediente = value;
                if (value.Count > 0)
                {
                    SelectExpediente = value.OrderBy(o => o.ID_ANIO).ThenBy(t => t.ID_IMPUTADO).FirstOrDefault();
                }
                OnPropertyChanged("ListExpediente");
            }
        }

        private bool emptyIngresoVisible;
        public bool EmptyIngresoVisible
        {
            get { return emptyIngresoVisible; }
            set { emptyIngresoVisible = value; OnPropertyChanged("EmptyIngresoVisible"); }
        }

        private IMPUTADO SelectExpedienteAsignacion;
        public IMPUTADO SelectExpediente
        {
            get { return SelectExpedienteAsignacion; }
            set
            {
                SelectExpedienteAsignacion = value;
                if (value != null)
                {
                    //MUESTRA LOS INGRESOS
                    if (value.INGRESO!=null && value.INGRESO.Count > 0)
                    {
                        EmptyIngresoVisible = false;
                        SelectIngreso = value.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
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

        public INGRESO _SelectIngreso { get; set; }
        public INGRESO SelectIngreso
        {
            get { return _SelectIngreso; }
            set
            {
                _SelectIngreso = value;

                if (value == null)
                {
                    OnPropertyChanged("SelectIngreso");
                    return;
                }
                    

                if (value.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                    ImagenIngreso = value.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                else
                    ImagenIngreso = new Imagenes().getImagenPerson();

                if (value.INGRESO_BIOMETRICO.Any(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG))
                    ImagenIngreso = value.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                else
                    ImagenIngreso = new Imagenes().getImagenPerson();
                OnPropertyChanged("SelectIngreso");
            }
        }

        private int Pagina { get; set; }
        private bool SeguirCargando { get; set; }
        private bool emptyExpedienteVisible;
        public bool EmptyExpedienteVisible
        {
            get { return emptyExpedienteVisible; }
            set { emptyExpedienteVisible = value; OnPropertyChanged("EmptyExpedienteVisible"); }
        }

        private INGRESO _SelectImputadoIngreso;
        public INGRESO SelectImputadoIngreso
        {
            get
            {
                return _SelectImputadoIngreso;
            }
            set
            {
                _SelectImputadoIngreso = value;
                OnPropertyChanged("SelectImputadoIngreso");
            }
        }

        private string _IngresosD { get; set; }
        public string IngresosD
        {
            get
            {
                return _IngresosD;
            }
            set
            {
                _IngresosD = value;
                OnPropertyChanged("IngresosD");
            }
        }

        private string _UbicacionD { get; set; }
        public string UbicacionD
        {
            get
            {
                return _UbicacionD;
            }
            set
            {
                _UbicacionD = value;
                OnPropertyChanged("UbicacionD");
            }
        }

        private string _TipoSeguridadD { get; set; }
        public string TipoSeguridadD
        {
            get
            {
                return _TipoSeguridadD;
            }
            set
            {
                _TipoSeguridadD = value;
                OnPropertyChanged("TipoSeguridadD");
            }
        }

        private string _FecIngresoD { get; set; }
        public string FecIngresoD
        {
            get
            {
                return _FecIngresoD;
            }
            set
            {
                _FecIngresoD = value;
                OnPropertyChanged("FecIngresoD");
            }
        }

        private string _ClasificacionJuridicaD { get; set; }
        public string ClasificacionJuridicaD
        {
            get
            {
                return _ClasificacionJuridicaD;
            }
            set
            {
                _ClasificacionJuridicaD = value;
                OnPropertyChanged("ClasificacionJuridicaD");
            }
        }

        private string _EstatusD { get; set; }
        public string EstatusD
        {
            get
            {
                return _EstatusD;
            }
            set
            {
                _EstatusD = value;
                OnPropertyChanged("EstatusD");
            }
        }

        private Visibility _ComboFrontBackFotoVisible = Visibility.Collapsed;
        public Visibility ComboFrontBackFotoVisible
        {
            get { return _ComboFrontBackFotoVisible; }
            set
            {
                _ComboFrontBackFotoVisible = value;
                OnPropertyChanged("ComboFrontBackFotoVisible");
            }
        }

        private Visibility _LineasGuiaFoto = Visibility.Collapsed;
        public Visibility LineasGuiaFoto
        {
            get { return _LineasGuiaFoto; }
            set
            {
                _LineasGuiaFoto = value;
                OnPropertyChanged("LineasGuiaFoto");
            }
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

        private WebCam CamaraWeb;
        private List<ImageSourceToSave> _ImagesToSave;
        public List<ImageSourceToSave> ImagesToSave
        {
            get { return _ImagesToSave; }
            set { _ImagesToSave = value; }
        }

        private ObservableCollection<OBJETO_TIPO> _ListTipoObjeto;
        public ObservableCollection<OBJETO_TIPO> ListTipoObjeto
        {
            get { return _ListTipoObjeto; }
            set
            {
                _ListTipoObjeto = value;
                OnPropertyChanged("ListTipoObjeto");
            }
        }

        private bool _EnableGroupBoxes;
        public bool EnableGroupBoxes
        {
            get { return _EnableGroupBoxes; }
            set
            {
                _EnableGroupBoxes = value;
                OnPropertyChanged("EnableGroupBoxes");
            }
        }

        private short _SelectedClasificacionObjeto = -1;
        public short SelectedClasificacionObjeto
        {
            get { return _SelectedClasificacionObjeto; }
            set
            {
                _SelectedClasificacionObjeto = value;
                OnPropertyChanged("SelectedClasificacionObjeto");
            }
        }

        private ObservableCollection<INGRESO_PERTENENCIA_DET> _ListObjetoImputado;
        public ObservableCollection<INGRESO_PERTENENCIA_DET> ListObjetoImputado
        {
            get { return _ListObjetoImputado; }
            set
            {
                _ListObjetoImputado = value;
                OnPropertyChanged("ListObjetoImputado");
            }
        }

        private INGRESO_PERTENENCIA_DET _SelectObjetoImputado;
        public INGRESO_PERTENENCIA_DET SelectObjetoImputado
        {
            get { return _SelectObjetoImputado; }
            set
            {
                _SelectObjetoImputado = value;
                OnPropertyChanged("SelectObjetoImputado");
                if (value != null)
                    ImagenObjeto = new Imagenes().ConvertByteToImageSource(value.IMAGEN);
            }
        }

        private string _DescrObjeto;
        public string DescrObjeto
        {
            get { return _DescrObjeto; }
            set
            {
                _DescrObjeto = value;
                OnPropertyChanged("DescrObjeto");
            }
        }

        private DateTime? _FechaIngresoResponsable = Fechas.GetFechaDateServer;
        public DateTime? FechaIngresoResponsable
        {
            get { return _FechaIngresoResponsable; }
            set
            {
                _FechaIngresoResponsable = value;
                OnPropertyChanged("FechaIngresoResponsable");
            }
        }

        private DateTime? _FechaEgresoResponsable = Fechas.GetFechaDateServer;
        public DateTime? FechaEgresoResponsable
        {
            get { return _FechaEgresoResponsable; }
            set
            {
                _FechaEgresoResponsable = value;
                OnPropertyChanged("FechaEgresoResponsable");
            }
        }

        private string _TextIngresoResponsable;
        public string TextIngresoResponsable
        {
            get { return _TextIngresoResponsable; }
            set
            {
                _TextIngresoResponsable = value;
                OnPropertyChanged("TextIngresoResponsable");
            }
        }

        private string _TextEgresoResponsable;
        public string TextEgresoResponsable
        {
            get { return _TextEgresoResponsable; }
            set
            {
                _TextEgresoResponsable = value;
                OnPropertyChanged("TextEgresoResponsable");
            }
        }

        private string _TextIngresoPersonasAutorizadas;
        public string TextIngresoPersonasAutorizadas
        {
            get { return _TextIngresoPersonasAutorizadas; }
            set
            {
                _TextIngresoPersonasAutorizadas = value;
                OnPropertyChanged("TextIngresoPersonasAutorizadas");
            }
        }

        private string _TextEgresoPersonasAutorizadas;
        public string TextEgresoPersonasAutorizadas
        {
            get { return _TextEgresoPersonasAutorizadas; }
            set
            {
                _TextEgresoPersonasAutorizadas = value;
                OnPropertyChanged("TextEgresoPersonasAutorizadas");
            }
        }

        private string _TextBotonSeleccionarIngreso = "seleccionar";
        public string TextBotonSeleccionarIngreso
        {
            get { return _TextBotonSeleccionarIngreso; }
            set
            {
                _TextBotonSeleccionarIngreso = value;
                OnPropertyChanged("TextBotonSeleccionarIngreso");
            }
        }

        private bool _CrearNuevoExpedienteEnabled;
        public bool CrearNuevoExpedienteEnabled
        {
            get { return _CrearNuevoExpedienteEnabled; }
            set
            {
                _CrearNuevoExpedienteEnabled = value;
                OnPropertyChanged("CrearNuevoExpedienteEnabled");
            }
        }

        private byte[] documentoIngreso = null;
        public byte[] DocumentoIngreso
        {
            get { return documentoIngreso; }
            set { documentoIngreso = value; OnPropertyChanged("DocumentoIngreso"); }
        }

        private byte[] documentoEgreso = null;
        public byte[] DocumentoEgreso
        {
            get { return documentoEgreso; }
            set { documentoEgreso = value; OnPropertyChanged("DocumentoIngreso"); }
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
                //if (value)
                //    MenuReporteEnabled = value;
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
    }
}
