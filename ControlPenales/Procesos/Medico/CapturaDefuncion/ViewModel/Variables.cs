using ControlPenales.BiometricoServiceReference;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ControlPenales
{
    partial class CapturaDefuncionViewModel
    {
        #region privadas
        private MODO_OPERACION modo_seleccionado = MODO_OPERACION.Insertar;
        private INGRESO selectedauxIngreso = null;
        private enumProcesos? proceso_origen=null;
        private bool errorGuardar = true;
        private short?[] estatus_inactivos = null;
        private byte[] reporte = null;
        #endregion

        #region variables para Habilitar/Deshabilitar controles
        private bool isMenuEnabled=true;
        public bool IsMenuEnabled
        {
            get { return isMenuEnabled; }
            set { isMenuEnabled = value; RaisePropertyChanged("IsMenuEnabled"); }
        }

        private bool isCapturaDefuncionEnabled = false;
        public bool IsCapturaDefuncionEnabled
        {
            get { return isCapturaDefuncionEnabled; }
            set { isCapturaDefuncionEnabled = value; RaisePropertyChanged("IsCapturaDefuncionEnabled"); }
        }

        private bool menuGuardarEnabled = false;
        public bool MenuGuardarEnabled
        {
            get { return menuGuardarEnabled; }
            set { menuGuardarEnabled = value; RaisePropertyChanged("MenuGuardarEnabled"); }
        }

        private bool menuAgregarEnabled = true;
        public bool MenuAgregarEnabled
        {
            get { return menuAgregarEnabled; }
            set { menuAgregarEnabled = value; RaisePropertyChanged("MenuAgregarEnabled"); }
        }

        private bool eliminarMenuEnabled = false;
        public bool EliminarMenuEnabled
        {
            get { return eliminarMenuEnabled;  }
            set { eliminarMenuEnabled = value; RaisePropertyChanged("EliminarMenuEnabled"); }
        }

        private bool menuReporteEnabled = false;
        public bool MenuReporteEnabled
        {
            get { return menuReporteEnabled; }
            set { menuReporteEnabled = value; RaisePropertyChanged("MenuReporteEnabled"); }
        }

        private bool menuBuscarEnabled = true;
        public bool MenuBuscarEnabled
        {
            get { return menuBuscarEnabled; }
            set { menuBuscarEnabled = false; RaisePropertyChanged("MenuBuscarEnabled"); }
        }
        #endregion

        #region variables para controlar visibilidad de controles
        private Visibility botonRegresarVisible = Visibility.Collapsed;
        public Visibility BotonRegresarVisible
        {
            get { return botonRegresarVisible; }
            set { botonRegresarVisible = value; RaisePropertyChanged("BotonRegresarVisible"); }
        }
        #endregion

        #region Datos de Ingreso
        private string textAnioImputado = string.Empty;
        public string TextAnioImputado
        {
            get { return textAnioImputado; }
            set { textAnioImputado = value; RaisePropertyChanged("TextAnioImputado"); }
        }

        private string textFolioImputado = string.Empty;
        public string TextFolioImputado
        {
            get { return textFolioImputado; }
            set { textFolioImputado = value; RaisePropertyChanged("TextFolioImputado"); }
        }

        private string textPaternoImputado = string.Empty;
        public string TextPaternoImputado
        {
            get { return textPaternoImputado; }
            set { textPaternoImputado = value; RaisePropertyChanged("TextPaternoImputado"); }
        }

        private string textMaternoImputado = string.Empty;
        public string TextMaternoImputado
        {
            get { return textMaternoImputado; }
            set { textMaternoImputado = value; RaisePropertyChanged("TextMaternoImputado"); }
        }

        private string textNombreImputado = string.Empty;
        public string TextNombreImputado
        { 
            get { return textNombreImputado; }
            set { textNombreImputado = value; RaisePropertyChanged("TextNombreImputado"); }
        }

        private string textAliasImputado = string.Empty;
        public string TextAliasImputado
        {
            get { return textAliasImputado; }
            set { textAliasImputado = value; RaisePropertyChanged("TextAliasImputado"); }
        }

        private string textEdadImputado = string.Empty;
        public string TextEdadImputado
        {
            get { return textEdadImputado; }
            set { textEdadImputado = value; }
        }

        private string textSexoImputado = string.Empty;
        public string TextSexoImputado
        {
            get { return textSexoImputado; }
            set { textSexoImputado = value; RaisePropertyChanged("TextSexoImputado"); }
        }

        private string textOriginarioImputado = string.Empty;
        public string TextOriginarioImputado
        {
            get { return textOriginarioImputado; }
            set { textOriginarioImputado = value; RaisePropertyChanged("TextOriginarioImputado"); }
        }

        private byte[] imagenIngresoDeceso = new Imagenes().getImagenPerson();
        public byte[] ImagenIngresoDeceso
        {
            get { return imagenIngresoDeceso; }
            set
            {
                imagenIngresoDeceso = value;
                OnPropertyChanged("ImagenIngresoDeceso");
            }
        }

        #endregion

        #region Datos Legales
        private string textFechaIngreso = string.Empty;
        public string TextFechaIngreso
        {
            get { return textFechaIngreso; }
            set { textFechaIngreso = value; RaisePropertyChanged("TextFechaIngreso"); }
        }

        private string textUltimaVisita = string.Empty;
        public string TextUltimaVisita
        {
            get { return textUltimaVisita; }
            set { textUltimaVisita = value; RaisePropertyChanged("TextUltimaVisita"); }
        }

        private string textUltimaVisitaFecha = string.Empty;
        public string TextUltimaVisitaFecha
        {
            get { return textUltimaVisitaFecha; }
            set { textUltimaVisitaFecha = value; RaisePropertyChanged("TextUltimaVisitaFecha"); }
        }

        private ObservableCollection<EXT_DELITOS> lstCausasPenales = null;
        public  ObservableCollection<EXT_DELITOS> LstCausasPenales
        {
            get { return lstCausasPenales; }
            set { lstCausasPenales = value; RaisePropertyChanged("LstCausasPenales"); }
        }
        #endregion

        #region Datos del deceso
        private string textCertMedDiagnostico = string.Empty;
        public string TextCertMedDiagnostico
        {
            get { return textCertMedDiagnostico; }
            set { textCertMedDiagnostico = value; RaisePropertyChanged("TextCertMedDiagnostico"); }
        }

        #region Autocomplete Enfermedad
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

        private ObservableCollection<ENFERMEDAD> _ListEnfermedades;
        public ObservableCollection<ENFERMEDAD> ListEnfermedades
        {
            get { return _ListEnfermedades; }
            set { _ListEnfermedades = value; OnPropertyChanged("ListEnfermedades"); }
        }
        private ENFERMEDAD _SelectEnfermedad;
        public ENFERMEDAD SelectEnfermedad
        {
            get { return _SelectEnfermedad; }
            set { _SelectEnfermedad = value; OnPropertyChanged("SelectEnfermedad"); }
        }

        private DateTime? fechaDeceso = null;
        public DateTime? FechaDeceso
        {
            get { return fechaDeceso; }
            set { fechaDeceso = value; OnPropertyValidateChanged("FechaDeceso"); }
        }

        private string textLugarDeceso = string.Empty;
        public string TextLugarDeceso
        {
            get { return textLugarDeceso; }
            set { textLugarDeceso = value; OnPropertyValidateChanged("TextLugarDeceso"); }
        }

        private string textHechosDeceso = string.Empty;
        public string TextHechosDeceso
        {
            get { return textHechosDeceso; }
            set { textHechosDeceso = value; OnPropertyValidateChanged("TextHechosDeceso"); }
        }

        private DateTime fechaMaximaDeceso = Fechas.GetFechaDateServer;
        public DateTime FechaMaximaDeceso
        {
            get { return fechaMaximaDeceso; }
            set { fechaMaximaDeceso = value; RaisePropertyChanged("FechaMaximaDeceso"); }
        }
        #endregion

        #region Variables para validacion
        private bool isFechaDecesoValida = false;
        public bool IsFechaDecesoValida
        {
            get { return isFechaDecesoValida; }
            set { isFechaDecesoValida = value; RaisePropertyChanged("IsFechaDecesoValida"); }
        }

        private bool isEnfermedadValida=false;
        public bool IsEnfermedadValida
        {
            get { return isEnfermedadValida; }
            set { isEnfermedadValida = value; RaisePropertyChanged("IsEnfermedadValida"); }
        }
        #endregion

        #region Buscar Ingreso

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

                TextBotonSeleccionarIngreso = "aceptar";
                SelectIngresoEnabled = true;
                if ((modo_seleccionado == MODO_OPERACION.Insertar && estatus_inactivos != null && SelectIngreso != null && estatus_inactivos.Contains(SelectIngreso.ID_ESTATUS_ADMINISTRATIVO)) || (modo_seleccionado == MODO_OPERACION.Reporte && estatus_inactivos != null && SelectIngreso != null && SelectIngreso.NOTA_DEFUNCION == null && SelectIngreso.ID_UB_CENTRO == GlobalVar.gCentro))
                {
                    TextBotonSeleccionarIngreso = "seleccionar ingreso";
                    SelectIngresoEnabled = false;
                }

                OnPropertyChanged("SelectIngreso");
            }
        }

        private string textBotonSeleccionarIngreso = "seleccionar ingreso";
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

        private bool emptyExpedienteVisible=true;
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
                    {
                        SelectIngreso = null;
                        EmptyIngresoVisible = true;
                    }


                    //OBTENEMOS FOTO DE FRENTE
                    if (SelectIngreso != null)
                    {
                        if (SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                            ImagenImputado = SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                        else
                            ImagenImputado = new Imagenes().getImagenPerson();
                        TextBotonSeleccionarIngreso = "aceptar";
                        SelectIngresoEnabled = true;


                        if ((modo_seleccionado == MODO_OPERACION.Insertar && estatus_inactivos != null && SelectIngreso != null && estatus_inactivos.Contains(SelectIngreso.ID_ESTATUS_ADMINISTRATIVO)) || (modo_seleccionado == MODO_OPERACION.Reporte && estatus_inactivos != null && SelectIngreso != null && SelectIngreso.NOTA_DEFUNCION == null && SelectIngreso.ID_UB_CENTRO == GlobalVar.gCentro))
                        {
                            TextBotonSeleccionarIngreso = "seleccionar ingreso";
                            SelectIngresoEnabled = false;
                        }
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

        private bool aceptarBusquedaHuellaFocus;
        public bool AceptarBusquedaHuellaFocus
        {
            get { return aceptarBusquedaHuellaFocus; }
            set { aceptarBusquedaHuellaFocus = value; OnPropertyChanged("AceptarBusquedaHuellaFocus"); }
        }

        private int Pagina { get; set; }
        private bool SeguirCargando { get; set; }

        private bool selectIngresoEnabled;
        public bool SelectIngresoEnabled
        {
            get { return selectIngresoEnabled; }
            set { selectIngresoEnabled = value; OnPropertyChanged("SelectIngresoEnabled"); }
        }
        #endregion


    }
}
