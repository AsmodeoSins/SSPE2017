using Cogent.Biometrics;
using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ControlPenales
{
    partial class CertificadoMedicoIngresoViewModel
    {
        #region BUSQUEDA_IMPUTADO
        private string textBotonSeleccionarIngreso = "seleccionar ingreso";
        public string TextBotonSeleccionarIngreso
        {
            get { return textBotonSeleccionarIngreso; }
            set { textBotonSeleccionarIngreso = value; OnPropertyChanged("TextBotonSeleccionarIngreso"); }
        }

        private bool crearNuevoExpedienteEnabled;
        public bool CrearNuevoExpedienteEnabled
        {
            get { return crearNuevoExpedienteEnabled; }
            set { crearNuevoExpedienteEnabled = value; OnPropertyChanged("CrearNuevoExpedienteEnabled"); }
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

        private bool emptyExpedienteVisible;
        public bool EmptyExpedienteVisible
        {
            get { return emptyExpedienteVisible; }
            set { emptyExpedienteVisible = value; OnPropertyChanged("EmptyExpedienteVisible"); }
        }

        private bool emptyIngresoVisible = true;
        public bool EmptyIngresoVisible
        {
            get { return emptyIngresoVisible; }
            set { emptyIngresoVisible = value; OnPropertyChanged("EmptyIngresoVisible"); }
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

        private IMPUTADO InputadoInterno { get; set; }

        private IMPUTADO selectExpediente;
        public IMPUTADO SelectExpediente
        {
            get { return selectExpediente; }
            set
            {
                selectExpediente = value;
                if (selectExpediente != null)
                {
                    if (selectExpediente.INGRESO.Count > 0)
                    {
                        EmptyIngresoVisible = false;
                        SelectIngreso = selectExpediente.INGRESO.OrderBy(o => o.FEC_INGRESO_CERESO).FirstOrDefault();
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
                    ImagenIngreso = ImagenImputado = new Imagenes().getImagenPerson();
                    return;
                }
                if (selectIngreso.ID_ESTATUS_ADMINISTRATIVO != Parametro.ID_ESTATUS_ADMVO_LIBERADO)
                    SelectIngresoEnabled = true;
                else
                    SelectIngresoEnabled = false;
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

        private bool selectIngresoEnabled;
        public bool SelectIngresoEnabled
        {
            get { return selectIngresoEnabled; }
            set { selectIngresoEnabled = value; OnPropertyChanged("SelectIngresoEnabled"); }
        }

        private INGRESO selectedIngreso;
        public INGRESO SelectedIngreso
        {
            get { return selectedIngreso; }
            set { selectedIngreso = value; OnPropertyChanged("SelectedIngreso"); }
        }

        private bool _ElementosDisponibles = true;
        public bool ElementosDisponibles
        {
            get { return _ElementosDisponibles; }
            set { _ElementosDisponibles = value; OnPropertyChanged("ElementosDisponibles"); }
        }

        //VARIABLES SEGMENTACION 
        private int Pagina { get; set; }
        private bool SeguirCargando { get; set; }

        private int? _TextAnioImputado;
        public int? TextAnioImputado
        {
            get { return _TextAnioImputado; }
            set { _TextAnioImputado = value; OnPropertyChanged("TextAnioImputado"); }
        }
        private int? _TextFolioImputado;
        public int? TextFolioImputado
        {
            get { return _TextFolioImputado; }
            set { _TextFolioImputado = value; OnPropertyChanged("TextFolioImputado"); }
        }
        private string _TextPaternoImputado;
        public string TextPaternoImputado
        {
            get { return _TextPaternoImputado; }
            set { _TextPaternoImputado = value; OnPropertyChanged("TextPaternoImputado"); }
        }
        private string _TextMaternoImputado;
        public string TextMaternoImputado
        {
            get { return _TextMaternoImputado; }
            set { _TextMaternoImputado = value; OnPropertyChanged("TextMaternoImputado"); }
        }
        private string _TextNombreImputado;
        public string TextNombreImputado
        {
            get { return _TextNombreImputado; }
            set { _TextNombreImputado = value; OnPropertyChanged("TextNombreImputado"); }
        }
        private byte[] _FotoIngreso = new Imagenes().getImagenPerson();
        public byte[] FotoIngreso
        {
            get { return _FotoIngreso; }
            set { _FotoIngreso = value; OnPropertyChanged("FotoIngreso"); }
        }
        private string _TextEdad;
        public string TextEdad
        {
            get { return _TextEdad; }
            set { _TextEdad = value; OnPropertyChanged("TextEdad"); }
        }
        private string _SelectSexo;
        public string SelectSexo
        {
            get { return _SelectSexo; }
            set { _SelectSexo = value; OnPropertyChanged("SelectSexo"); }
        }
        private string _SelectFechaNacimiento;
        public string SelectFechaNacimiento
        {
            get { return _SelectFechaNacimiento; }
            set { _SelectFechaNacimiento = value; OnPropertyChanged("SelectFechaNacimiento"); }
        }
        private string _TextLugarNacimiento;
        public string TextLugarNacimiento
        {
            get { return _TextLugarNacimiento; }
            set { _TextLugarNacimiento = value; OnPropertyChanged("TextLugarNacimiento"); }
        }
        private string _TextEscolaridad;
        public string TextEscolaridad
        {
            get { return _TextEscolaridad; }
            set { _TextEscolaridad = value; OnPropertyChanged("TextEscolaridad"); }
        }
        private string _TextOcupacion;
        public string TextOcupacion
        {
            get { return _TextOcupacion; }
            set { _TextOcupacion = value; OnPropertyChanged("TextOcupacion"); }
        }
        private string _TextFechaIngreso;
        public string TextFechaIngreso
        {
            get { return _TextFechaIngreso; }
            set { _TextFechaIngreso = value; OnPropertyChanged("TextFechaIngreso"); }
        }
        private string _TextDelito;
        public string TextDelito
        {
            get { return _TextDelito; }
            set { _TextDelito = value; OnPropertyChanged("TextDelito"); }
        }
        #endregion

        #region CERTIFICADO
        private List<RadioButton> _ListRadioButonsDorso;
        public List<RadioButton> ListRadioButonsDorso
        {
            get { return _ListRadioButonsDorso; }
            set { _ListRadioButonsDorso = value; }
        }
        private List<RadioButton> _ListRadioButonsFrente;
        public List<RadioButton> ListRadioButonsFrente
        {
            get { return _ListRadioButonsFrente; }
            set { _ListRadioButonsFrente = value; }
        }
        private string _TextAntecedentesPatologicos;
        public string TextAntecedentesPatologicos
        {
            get { return _TextAntecedentesPatologicos; }
            set { _TextAntecedentesPatologicos = value; OnPropertyChanged("TextAntecedentesPatologicos"); }
        }
        private string _TextToxicomanias;
        public string TextToxicomanias
        {
            get { return _TextToxicomanias; }
            set { _TextToxicomanias = value; OnPropertyChanged("TextToxicomanias"); }
        }
        private string _TextPadecimientoYTratamientoActual;
        public string TextPadecimientoYTratamientoActual
        {
            get { return _TextPadecimientoYTratamientoActual; }
            set { _TextPadecimientoYTratamientoActual = value; OnPropertyChanged("TextPadecimientoYTratamientoActual"); }
        }
        private string _TextSeDetecto;
        public string TextSeDetecto
        {
            get { return _TextSeDetecto; }
            set { _TextSeDetecto = value; OnPropertyChanged("TextSeDetecto"); }
        }
        private string _TextDiagnostico;
        public string TextDiagnostico
        {
            get { return _TextDiagnostico; }
            set { _TextDiagnostico = value; OnPropertyChanged("TextDiagnostico"); }
        }
        private string _TextPlanTerapeutico;
        public string TextPlanTerapeutico
        {
            get { return _TextPlanTerapeutico; }
            set { _TextPlanTerapeutico = value; OnPropertyChanged("TextPlanTerapeutico"); }
        }
        private string _TextObservaciones;
        public string TextObservaciones
        {
            get { return _TextObservaciones; }
            set { _TextObservaciones = value; OnPropertyChanged("TextObservaciones"); }
        }
        private bool _CheckedToxicomanias;
        public bool CheckedToxicomanias
        {
            get { return _CheckedToxicomanias; }
            set { _CheckedToxicomanias = value; OnPropertyChanged("CheckedToxicomanias"); }
        }
        private bool _CheckedSeguimiento;
        public bool CheckedSeguimiento
        {
            get { return _CheckedSeguimiento; }
            set { _CheckedSeguimiento = value; OnPropertyChanged("CheckedSeguimiento"); }
        }
        private bool _CheckedHospitalizacion;
        public bool CheckedHospitalizacion
        {
            get { return _CheckedHospitalizacion; }
            set { _CheckedHospitalizacion = value; OnPropertyChanged("CheckedHospitalizacion"); }
        }
        private bool _CheckedPeligroVida;
        public bool CheckedPeligroVida
        {
            get { return _CheckedPeligroVida; }
            set { _CheckedPeligroVida = value; OnPropertyChanged("CheckedPeligroVida"); }
        }
        private bool _Checked15DiasSanar;
        public bool Checked15DiasSanar
        {
            get { return _Checked15DiasSanar; }
            set { _Checked15DiasSanar = value; OnPropertyChanged("Checked15DiasSanar"); }
        }
        private bool _TabFrente;
        public bool TabFrente
        {
            get { return _TabFrente; }
            set { _TabFrente = value; OnPropertyChanged("TabFrente"); }
        }
        private bool _TabDorso;
        public bool TabDorso
        {
            get { return _TabDorso; }
            set { _TabDorso = value; OnPropertyChanged("TabDorso"); }
        }

        #region LESIONES
        private bool _BotonLesionEnabled;
        public bool BotonLesionEnabled
        {
            get { return _BotonLesionEnabled; }
            set { _BotonLesionEnabled = value; OnPropertyChanged("BotonLesionEnabled"); }
        }
        private short? _SelectRegion;
        public short? SelectRegion
        {
            get { return _SelectRegion; }
            set { _SelectRegion = value; OnPropertyChanged("SelectRegion"); }
        }
        private ObservableCollection<LesionesCustom> _ListLesiones;
        public ObservableCollection<LesionesCustom> ListLesiones
        {
            get { return _ListLesiones; }
            set { _ListLesiones = value; OnPropertyChanged("ListLesiones"); }
        }
        private LesionesCustom _SelectLesion;
        public LesionesCustom SelectLesion
        {
            get { return _SelectLesion; }
            set { _SelectLesion = value; OnPropertyChanged("SelectLesion"); }
        }
        private string _TextDescripcionLesion;
        public string TextDescripcionLesion
        {
            get { return _TextDescripcionLesion; }
            set { _TextDescripcionLesion = value; OnPropertyChanged("TextDescripcionLesion"); }
        }
        #endregion

        #endregion

        #region ENFERMEDAD
        private ControlPenales.Controls.AutoCompleteTextBox _AutoCompleteTB;
        public ControlPenales.Controls.AutoCompleteTextBox AutoCompleteTB
        {
            get { return _AutoCompleteTB; }
            set { _AutoCompleteTB = value; }
        }
        private ListBox _AutoComplete;
        public ListBox AutoCompleteLB
        {
            get { return _AutoComplete; }
            set { _AutoComplete = value; }
        }
        #endregion
    }
}