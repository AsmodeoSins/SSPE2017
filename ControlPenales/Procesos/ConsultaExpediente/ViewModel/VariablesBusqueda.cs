using ControlPenales.BiometricoServiceReference;
using SSP.Servidor;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
//using MvvmFramework;

namespace ControlPenales
{
    partial class ConsultaExpedienteInternoViewModel
    {
        private bool emptyExpedienteVisible;
        public bool EmptyExpedienteVisible
        {
            get { return emptyExpedienteVisible; }
            set { emptyExpedienteVisible = value; OnPropertyChanged("EmptyExpedienteVisible"); }
        }

        private string textBotonSeleccionarIngreso = "Seleccionar Ingreso";
        public string TextBotonSeleccionarIngreso
        {
            get { return textBotonSeleccionarIngreso; }
            set { textBotonSeleccionarIngreso = value; OnPropertyChanged("TextBotonSeleccionarIngreso"); }
        }

        private bool crearNuevoExpedienteEnabled = false;
        public bool CrearNuevoExpedienteEnabled
        {
            get { return crearNuevoExpedienteEnabled; }
            set { crearNuevoExpedienteEnabled = value; OnPropertyChanged("CrearNuevoExpedienteEnabled"); }
        }

        private bool selectIngresoEnabled = true;
        public bool SelectIngresoEnabled
        {
            get { return selectIngresoEnabled; }
            set { selectIngresoEnabled = value; OnPropertyChanged("SelectIngresoEnabled"); }
        }

        private int? _AnioBuscar;
        public int? AnioBuscar
        {
            get { return _AnioBuscar; }
            set { _AnioBuscar = value; OnPropertyChanged("AnioBuscar"); }
        }
        private int? _FolioBuscar;
        public int? FolioBuscar
        {
            get { return _FolioBuscar; }
            set { _FolioBuscar = value; OnPropertyChanged("FolioBuscar"); }
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

        private byte[] imagenIngreso = new Imagenes().getImagenPerson();
        public byte[] ImagenIngreso
        {
            get
            {
                return imagenIngreso;
            }
            set
            {
                imagenIngreso = value;

                OnPropertyChanged("ImagenIngreso");
            }
        }
        private byte[] imagenImputado = new Imagenes().getImagenPerson();
        public byte[] ImagenImputado
        {
            get
            {
                return imagenImputado;

            }
            set
            {
                imagenImputado = value;

                OnPropertyChanged("ImagenImputado");
            }
        }

        private byte[] _ImagenAcompanante = new Imagenes().getImagenPerson();
        public byte[] ImagenAcompanante
        {
            get
            {
                return _ImagenAcompanante;
            }
            set
            {
                _ImagenAcompanante = value;
                OnPropertyChanged("ImagenAcompanante");
            }
        }

        private bool emptyIngresoVisible;
        public bool EmptyIngresoVisible
        {
            get { return emptyIngresoVisible; }
            set { emptyIngresoVisible = value; OnPropertyChanged("EmptyIngresoVisible"); }
        }

        private ObservableCollection<IMPUTADO> listExpedientePadron;
        public ObservableCollection<IMPUTADO> ListExpedientePadron
        {
            get { return listExpedientePadron; }
            set { listExpedientePadron = value; }
        }
        private ObservableCollection<IMPUTADO> listExpedienteAsignacion;
        public ObservableCollection<IMPUTADO> ListExpedienteAsignacion
        {
            get { return listExpedienteAsignacion; }
            set { listExpedienteAsignacion = value; }
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

        public IMPUTADO selectExpediente { get; set; }
        private IMPUTADO SelectExpedienteAuxiliar;
        private IMPUTADO ImputadoSeleccionado;
        public IMPUTADO SelectExpediente
        {
            get { return selectExpediente; }
            set
            {
                selectExpediente = value;
                if (value != null)
                {
                    //MUESTRA LOS INGRESOS
                    if (SelectExpediente.INGRESO != null && SelectExpediente.INGRESO.Count > 0)
                    {
                        EmptyIngresoVisible = false;
                        SelectIngreso = SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
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

        public INGRESO selectIngreso { get; set; }
        private INGRESO SelectIngresoAuxiliar;
        public INGRESO SelectIngreso
        {
            get { return selectIngreso; }
            set
            {
                selectIngreso = value;
                if (value == null)
                {
                    LabelUbicacionFecha = "Ubicación";
                    LiberacionVisible = Visibility.Collapsed;
                    TextEstatusAdministrativoBuscar = string.Empty;
                    TextMotivoLiberacion = string.Empty;
                    TextUbicacionBuscar = string.Empty;
                    ImagenImputado = new Imagenes().getImagenPerson();
                    ImagenIngreso = new Imagenes().getImagenPerson();
                    OnPropertyChanged("SelectIngreso");
                    return;
                }

                if (value.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                    ImagenImputado = value.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                else
                    ImagenImputado = new Imagenes().getImagenPerson();

                if (value.INGRESO_BIOMETRICO.Any(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG))
                    ImagenIngreso = value.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                else
                    ImagenIngreso = new Imagenes().getImagenPerson();
                var IngresoReciente = SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                if (IngresoReciente.ID_CENTRO == value.ID_CENTRO && IngresoReciente.ID_ANIO == value.ID_ANIO && IngresoReciente.ID_IMPUTADO == value.ID_IMPUTADO && IngresoReciente.ID_INGRESO == value.ID_INGRESO)
                {
                    TextEstatusAdministrativoBuscar = value.ESTATUS_ADMINISTRATIVO.DESCR;
                    if (value.ID_ESTATUS_ADMINISTRATIVO == Parametro.ID_ESTATUS_ADMVO_LIBERADO)
                    {
                        LabelUbicacionFecha = "Fecha Liberación";
                        LiberacionVisible = Visibility.Visible;
                        TextMotivoLiberacion = value.LIBERACION.Any(a => a.LIBERACION_MOTIVO != null) ?
                            value.LIBERACION.OrderByDescending(o => o.LIBERACION_FEC).FirstOrDefault().LIBERACION_MOTIVO.DESCR : string.Empty;
                        TextUbicacionBuscar = value.LIBERACION.Any() ?
                            value.LIBERACION.OrderByDescending(o => o.LIBERACION_FEC).FirstOrDefault().LIBERACION_FEC.ToString("dd DE MMMM DEL yyyy").ToUpper() : string.Empty;
                    }
                    else
                    {
                        LabelUbicacionFecha = "Ubicación";
                        LiberacionVisible = Visibility.Collapsed;
                        TextMotivoLiberacion = string.Empty;
                        TextUbicacionBuscar = value.CAMA != null ?
                                                string.Format("{0} - {1}-{2}-{3}-{4}",
                                                value.CAMA != null ? value.CAMA.CELDA != null ? value.CAMA.CELDA.SECTOR != null ? value.CAMA.CELDA.SECTOR.EDIFICIO != null ? value.CAMA.CELDA.SECTOR.EDIFICIO.CENTRO != null ? !string.IsNullOrEmpty(value.CAMA.CELDA.SECTOR.EDIFICIO.CENTRO.DESCR) ? value.CAMA.CELDA.SECTOR.EDIFICIO.CENTRO.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                                                value.CAMA != null ? value.CAMA.CELDA != null ? value.CAMA.CELDA.SECTOR != null ? value.CAMA.CELDA.SECTOR.EDIFICIO != null ? !string.IsNullOrEmpty(value.CAMA.CELDA.SECTOR.EDIFICIO.DESCR) ? value.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                                                value.CAMA != null ? value.CAMA.CELDA != null ? value.CAMA.CELDA.SECTOR != null ? !string.IsNullOrEmpty(value.CAMA.CELDA.SECTOR.DESCR) ? value.CAMA.CELDA.SECTOR.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                                                value.CAMA != null ? value.CAMA.CELDA != null ? !string.IsNullOrEmpty(value.CAMA.CELDA.ID_CELDA) ? value.CAMA.CELDA.ID_CELDA.Trim() : string.Empty : string.Empty : string.Empty,
                                                value.CAMA != null ? value.CAMA.ID_CAMA.ToString() : string.Empty)
                                                :
                                                value.ID_ESTATUS_ADMINISTRATIVO == Parametro.ID_ESTATUS_ADMVO_TRASLADO ?
                                                    value.TRASLADO_DETALLE.Any(a => a.TRASLADO.CENTRO_DESTINO.HasValue) ?
                                                        value.TRASLADO_DETALLE.First(a => a.TRASLADO.CENTRO_DESTINO.HasValue).TRASLADO.CENTRO.DESCR.Trim()
                                                        :
                                                        value.TRASLADO_DETALLE.Any(a => !a.TRASLADO.CENTRO_ORIGEN.HasValue && a.TRASLADO.CENTRO_ORIGEN_FORANEO != null && a.TRASLADO.CENTRO_ORIGEN_FORANEO != string.Empty) ?
                                                            value.TRASLADO_DETALLE.First(a => !a.TRASLADO.CENTRO_ORIGEN.HasValue && a.TRASLADO.CENTRO_ORIGEN_FORANEO != null &&
                                                                a.TRASLADO.CENTRO_ORIGEN_FORANEO != string.Empty).TRASLADO.CENTRO_ORIGEN_FORANEO
                                                            :
                                                            string.Empty
                                                    :
                                                    value.CENTRO1 != null ? !string.IsNullOrEmpty(value.CENTRO1.DESCR) ? value.CENTRO1.DESCR.Trim() : string.Empty : string.Empty;
                    }
                }
                OnPropertyChanged("SelectIngreso");
            }
        }
        private bool aceptarBusquedaHuellaFocus;
        public bool AceptarBusquedaHuellaFocus
        {
            get { return aceptarBusquedaHuellaFocus; }
            set { aceptarBusquedaHuellaFocus = value; OnPropertyChanged("AceptarBusquedaHuellaFocus"); }
        }
        private Visibility _LiberacionVisible = Visibility.Collapsed;
        public Visibility LiberacionVisible
        {
            get { return _LiberacionVisible; }
            set { _LiberacionVisible = value; OnPropertyChanged("LiberacionVisible"); }
        }
        private string _LabelUbicacionFecha = "Ubicación";
        public string LabelUbicacionFecha
        {
            get { return _LabelUbicacionFecha; }
            set { _LabelUbicacionFecha = value; OnPropertyChanged("LabelUbicacionFecha"); }
        }
        private string _TextMotivoLiberacion;
        public string TextMotivoLiberacion
        {
            get { return _TextMotivoLiberacion; }
            set { _TextMotivoLiberacion = value; OnPropertyChanged("TextMotivoLiberacion"); }
        }
        private string _TextEstatusAdministrativoBuscar;
        public string TextEstatusAdministrativoBuscar
        {
            get { return _TextEstatusAdministrativoBuscar; }
            set { _TextEstatusAdministrativoBuscar = value; OnPropertyChanged("TextEstatusAdministrativoBuscar"); }
        }
        private string _TextUbicacionBuscar;
        public string TextUbicacionBuscar
        {
            get { return _TextUbicacionBuscar; }
            set { _TextUbicacionBuscar = value; OnPropertyChanged("TextUbicacionBuscar"); }
        }

        //VARIABLES SEGMENTACION 
        private int Pagina { get; set; }
        private bool SeguirCargando { get; set; }
    }
}
