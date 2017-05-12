using ControlPenales.BiometricoServiceReference;
using SSP.Servidor;
using System.Collections.ObjectModel;
using System.Linq;
//using MvvmFramework;

namespace ControlPenales
{
    partial class PadronVisitasViewModel
    {
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

        private byte[] ImagenIngresoDAsignacion = new Imagenes().getImagenPerson();
        private byte[] ImagenIngresoDPadron = new Imagenes().getImagenPerson();
        private byte[] ImagenIngresoDCredencial = new Imagenes().getImagenPerson();

        private byte[] ImagenImputadoAsignacion = new Imagenes().getImagenPerson();
        private byte[] ImagenImputadoPadron = new Imagenes().getImagenPerson();
        private byte[] ImagenImputadoCredencial = new Imagenes().getImagenPerson();

        public byte[] ImagenIngreso
        {
            get
            {
                switch (SelectedTab)
                {
                    case TabsVisita.ASIGNACION_DE_VISITAS:
                        return ImagenIngresoDAsignacion;
                    case TabsVisita.PADRON_DE_VISITAS:
                        return ImagenIngresoDPadron;
                    case TabsVisita.ENTREGA_CREDENCIALES:
                        return ImagenIngresoDCredencial;
                    default:
                        return null;
                }
            }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    ImagenIngresoDAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    ImagenIngresoDPadron = value;
                if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
                    ImagenIngresoDCredencial = value;

                OnPropertyChanged("ImagenIngreso");
            }
        }

        public byte[] ImagenImputado
        {
            get
            {
                switch (SelectedTab)
                {
                    case TabsVisita.ASIGNACION_DE_VISITAS:
                        return ImagenImputadoAsignacion;
                    case TabsVisita.PADRON_DE_VISITAS:
                        return ImagenImputadoPadron;
                    case TabsVisita.ENTREGA_CREDENCIALES:
                        return ImagenImputadoCredencial;
                    default:
                        return null;
                }
            }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    ImagenImputadoAsignacion = value;
                if (SelectedTab == TabsVisita.PADRON_DE_VISITAS)
                    ImagenImputadoPadron = value;
                if (SelectedTab == TabsVisita.ENTREGA_CREDENCIALES)
                    ImagenImputadoCredencial = value;

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

        private IMPUTADO SelectExpedientePadron;
        private IMPUTADO SelectExpedienteAsignacion;
        private IMPUTADO SelectExpedienteAuxiliar;
        public IMPUTADO SelectExpediente
        {
            get { return SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS ? SelectExpedienteAsignacion : SelectExpedientePadron; }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    SelectExpedienteAsignacion = value;
                else
                    SelectExpedientePadron = value;
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

        private INGRESO SelectIngresoPadron;
        private INGRESO SelectIngresoAsignacion;
        private INGRESO SelectIngresoAuxiliar;
        public INGRESO SelectIngreso
        {
            get { return SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS ? SelectIngresoAsignacion : SelectIngresoPadron; }
            set
            {
                if (SelectedTab == TabsVisita.ASIGNACION_DE_VISITAS)
                    SelectIngresoAsignacion = value;
                else
                    SelectIngresoPadron = value;
                if (value == null)
                {
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
                OnPropertyChanged("SelectIngreso");
            }
        }

        private INGRESO AuxIngreso;
        private bool aceptarBusquedaHuellaFocus;
        public bool AceptarBusquedaHuellaFocus
        {
            get { return aceptarBusquedaHuellaFocus; }
            set { aceptarBusquedaHuellaFocus = value; OnPropertyChanged("AceptarBusquedaHuellaFocus"); }
        }

        //VARIABLES SEGMENTACION 
        private int Pagina { get; set; }
        private bool SeguirCargando { get; set; }
    }
}
