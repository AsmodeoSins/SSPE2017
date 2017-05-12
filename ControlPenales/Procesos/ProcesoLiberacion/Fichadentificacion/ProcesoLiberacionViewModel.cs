using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ControlPenales
{
    partial class ProcesoLiberacionViewModel : FingerPrintScanner
    {
        #region Pantalla
        public string Name
        {
            get { return "registro_ingreso_interno"; }
        }
        private IMPUTADO imputado;
        public IMPUTADO Imputado
        {
            get { return imputado; }
            set { imputado = value; OnPropertyChanged("Imputado"); }
        }
        private IMPUTADO imputadoSeleccionadoAuxiliar;
        public IMPUTADO ImputadoSeleccionadoAuxiliar
        {
            get { return imputadoSeleccionadoAuxiliar; }
            set { imputadoSeleccionadoAuxiliar = value; OnPropertyChanged("ImputadoSeleccionadoAuxiliar"); }
        }
        private INGRESO ingreso;
        public INGRESO Ingreso
        {
            get { return ingreso; }
            set
            {
                ingreso = value;
                OnPropertyChanged("Ingreso");
                // LoadSelectedTreeViewItem(Ingreso, Imputado);
            }
        }

        #region DatosImputado

        private string _TextPaterno;


  

        private string _TextCalle;

        public string TextCalle
        {
            get { return _TextCalle; }
            set { _TextCalle = value; OnPropertyChanged("TextCalle"); }
        }

        private string textTelefono;
        public string TextTelefono
        {
            get { return textTelefono; }
            set
            {
                textTelefono = !string.IsNullOrEmpty(value) ? new Converters().MascaraTelefono(value) : value;
                OnPropertyChanged("TextTelefono");
            }
        }

        


        private int? _TextNumeroExterior;
        public int? TextNumeroExterior
        {
            get { return _TextNumeroExterior; }
            set
            {
                _TextNumeroExterior= value;

                OnPropertyValidateChanged("TextNumeroExterior");
            }
        }
      

        private int? _TextCodigoPostal;
        public int? TextCodigoPostal
        {
            get { return _TextCodigoPostal; }
            set
            {
                _TextCodigoPostal = value;

                OnPropertyValidateChanged("TextCodigoPostal");
            }
        }

        private ObservableCollection<ESCOLARIDAD> listEscolaridad;
        public ObservableCollection<ESCOLARIDAD> ListEscolaridad
        {
            get { return listEscolaridad; }
            set { listEscolaridad = value; OnPropertyChanged("ListEscolaridad"); }
        }

        private ObservableCollection<OCUPACION> listOcupacion;
        public ObservableCollection<OCUPACION> ListOcupacion
        {
            get { return listOcupacion; }
            set { listOcupacion = value; OnPropertyChanged("ListOcupacion"); }
        }

        private string _TextNumeroInterior;
        public string TextNumeroInterior
        {
            get { return _TextNumeroInterior; }
            set
            {
                _TextNumeroInterior = value;

                OnPropertyValidateChanged("TextNumeroInterior");
            }
        }
        #endregion
        private INGRESO selectIngreso;
        public INGRESO SelectIngreso
        {
            get { return selectIngreso; }
            set
            {
                selectIngreso = value;
                if (selectIngreso == null)
                    return;

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

                if (value != null)
                    MenuReporteEnabled = true;
                else
                    MenuReporteEnabled = false;
                OnPropertyChanged("SelectIngreso");
            }
        }

        private ObservableCollection<ALIAS> listAlias;
        public ObservableCollection<ALIAS> ListAlias
        {
            get { return listAlias; }
            set { listAlias = value; OnPropertyValidateChanged("ListAlias"); }
        }

        private ALIAS selectAlias;
        public ALIAS SelectAlias
        {
            get { return selectAlias; }
            set { selectAlias = value; OnPropertyChanged("SelectAlias"); }
        }
        private ObservableCollection<APODO> listApodo;
        public ObservableCollection<APODO> ListApodo
        {
            get { return listApodo; }
            set { listApodo = value; OnPropertyValidateChanged("ListApodo"); }
        }

        private DateTime? textFechaNacimiento;
        public DateTime? TextFechaNacimiento
        {
            get { return textFechaNacimiento; }
            set
            {
                textFechaNacimiento = value;
                CalcularEdad();

                OnPropertyValidateChanged("TextFechaNacimiento");
            }
        }

        private APODO selectApodo;
        public APODO SelectApodo
        {
            get { return selectApodo; }
            set { selectApodo = value; OnPropertyChanged("SelectApodo"); }
        }

        private bool tipoTatuajeEnabled;
        public bool TipoTatuajeEnabled
        {
            get { return tipoTatuajeEnabled; }
            set { tipoTatuajeEnabled = value; OnPropertyChanged("TipoTatuajeEnabled"); }
        }
        private bool banderaEntrada;
        public bool BanderaEntrada
        {
            get { return banderaEntrada; }
            set { banderaEntrada = value; }
        }
        private bool visibleIngreso = true;
        public bool VisibleIngreso
        {
            get { return visibleIngreso; }
            set { visibleIngreso = value; OnPropertyChanged("VisibleIngreso"); }
        }
        private string tituloTop = "Registro";
        public string TituloTop
        {
            get { return tituloTop; }
            set { tituloTop = value; OnPropertyChanged("TituloTop"); }
        }

        private bool tipoRegistro;
        public bool TipoRegistro
        {
            get { return tipoRegistro; }
            set
            {
                tipoRegistro = value;
                if (tipoRegistro)
                    VisibleInterconexion = true;
                else
                    VisibleInterconexion = false;
                OnPropertyChanged("TipoRegistro");
            }
        }

        private bool _GeneralVisible;
        public bool  GeneralVisible
        {
            get { return _GeneralVisible; }
            set
            {
                _GeneralVisible = value;
                OnPropertyChanged("GeneralVisible");
            }
        }

        private bool visibleInterconexion;
        public bool VisibleInterconexion
        {
            get { return visibleInterconexion; }
            set
            {
                visibleInterconexion = value;
                OnPropertyChanged("VisibleInterconexion");
            }
        }

        private bool visibleBuscarNUC;
        public bool VisibleBuscarNUC
        {
            get { return visibleBuscarNUC; }
            set { visibleBuscarNUC = value; OnPropertyChanged("VisibleBuscarNUC"); }
        }

        //SELECCION DE SISTEMA TRADICIONAL / NUEVOS SISTEMA 
        private bool visibleSeleccionSistema = true;
        public bool VisibleSeleccionSistema
        {
            get { return visibleSeleccionSistema; }
            set { visibleSeleccionSistema = value; OnPropertyChanged("VisibleSeleccionSistema"); }
        }
        //private bool cambiosSinGuardar;
        //public bool CambiosSinGuardar
        //{
        //    get { return cambiosSinGuardar; }
        //    set { cambiosSinGuardar = value; OnPropertyChanged("CambiosSinGuardar"); }
        //}

        #region [TAB SELECTEDS]
        private bool nuevoImputado;
        public bool NuevoImputado
        {
            get { return nuevoImputado; }
            set { nuevoImputado = value; OnPropertyChanged("NuevoImputado"); }
        }
        private bool isSelectedDatosIngreso;
        public bool IsSelectedDatosIngreso
        {
            get { return isSelectedDatosIngreso; }
            set
            {
                isSelectedDatosIngreso = value;
                OnPropertyChanged("IsSelectedDatosIngreso");
            }
        }

        private string _TextActitudGeneralEntrv;

        public string TextActitudGeneralEntrv
        {
            get { return _TextActitudGeneralEntrv; }
            set { _TextActitudGeneralEntrv = value; OnPropertyChanged("TextActitudGeneralEntrv"); }
        }
        private bool isSelectedIdentificacion;
        public bool IsSelectedIdentificacion
        {
            get { return isSelectedIdentificacion; }
            set
            {
                isSelectedIdentificacion = value;
                OnPropertyChanged("IsSelectedIdentificacion");
            }
        }
        private string _TextObservaciones;


        public string TextObservaciones
        {
            get { return _TextObservaciones; }
            set { _TextObservaciones = value; OnPropertyChanged("TextObservaciones"); }
        }

        private bool isSelectedTraslado;
        public bool IsSelectedTraslado
        {
            get { return isSelectedTraslado; }
            set
            {
                isSelectedTraslado = value;
                OnPropertyChanged("IsSelectedTraslado");
            }
        }
        private bool tabDatosGenerales;
        public bool TabDatosGenerales
        {
            get { return tabDatosGenerales; }
            set
            {
                tabDatosGenerales = value;
                OnPropertyChanged("TabDatosGenerales");
            }
        }

        private string _TextLugarNacimiento;



        public string TextLugarNacimiento
        {
            get { return _TextLugarNacimiento; }
            set { _TextLugarNacimiento = value; OnPropertyChanged("TextLugarNacimiento"); }
        }

           private string _TextHorarioDiasTrabajados;


        public string TextHorarioDiasTrabajados
        {
            get { return _TextHorarioDiasTrabajados; }
            set { _TextHorarioDiasTrabajados = value; OnPropertyChanged("TextHorarioDiasTrabajados"); }
        }

        private bool tabApodosAlias;
        public bool TabApodosAlias
        {
            get { return tabApodosAlias; }
            set
            {
                tabApodosAlias = value;
                OnPropertyChanged("TabApodosAlias");
            }
        }

        //private string textTelefono;
        //public string TextTelefono
        //{
        //    get { return textTelefono; }
        //    set
        //    {
        //        textTelefono = !string.IsNullOrEmpty(value) ? new Converters().MascaraTelefono(value) : value;
        //        OnPropertyChanged("TextTelefono");
        //    }
        //}

        //private int? textCodigoPostal;
        //public int? TextCodigoPostal
        //{
        //    get { return textCodigoPostal; }
        //    set
        //    {
        //        textCodigoPostal = value;
        //        OnPropertyChanged("TextCodigoPostal");
        //    }
       // }

        private bool tabFotosHuellas;
        public bool TabFotosHuellas
        {
            get { return tabFotosHuellas; }
            set
            {
                tabFotosHuellas = value;
                OnPropertyChanged("TabFotosHuellas");
            }
        }
        private bool tabMediaFiliacion;
        public bool TabMediaFiliacion
        {
            get { return tabMediaFiliacion; }
            set
            {
                tabMediaFiliacion = value;
                OnPropertyChanged("TabMediaFiliacion");
                //getDatosMediaFiliacion();
            }
        }
        private bool tabSenasParticulares;
        public bool TabSenasParticulares
        {
            get { return tabSenasParticulares; }
            set
            {
                tabSenasParticulares = value;
                OnPropertyChanged("TabSenasParticulares");
            }
        }
        private bool tabPandillas;
        public bool TabPandillas
        {
            get { return tabPandillas; }
            set
            {
                tabPandillas = value;
                OnPropertyChanged("TabPandillas");
            }
        }
        #endregion

        #region [TAB ENABLEDS]
        private bool camposBusquedaEnabled;
        public bool CamposBusquedaEnabled
        {
            get { return camposBusquedaEnabled; }
            set
            {
                camposBusquedaEnabled = value;
                MenuBuscarEnabled = value;

                OnPropertyChanged("CamposBusquedaEnabled");
            }
        }
        private bool estatusAdministrativoEnabled = true;
        public bool EstatusAdministrativoEnabled
        {
            get { return estatusAdministrativoEnabled; }
            set { estatusAdministrativoEnabled = value; OnPropertyChanged("EstatusAdministrativoEnabled"); }
        }
        private bool clasificacionJuridicaEnabled = true;
        public bool ClasificacionJuridicaEnabled
        {
            get { return clasificacionJuridicaEnabled; }
            set { clasificacionJuridicaEnabled = value; OnPropertyChanged("ClasificacionJuridicaEnabled"); }
        }
        private bool datosGeneralesEnabled = true;
        public bool DatosGeneralesEnabled
        {
            get { return datosGeneralesEnabled; }
            set { datosGeneralesEnabled = value; OnPropertyChanged("DatosGeneralesEnabled"); }
        }
        private bool apodosAliasEnabled = true;
        public bool ApodosAliasEnabled
        {
            get { return apodosAliasEnabled; }
            set { apodosAliasEnabled = value; OnPropertyChanged("ApodosAliasEnabled"); }
        }
        private bool fotosHuellasEnabled = true;
        public bool FotosHuellasEnabled
        {
            get { return fotosHuellasEnabled; }
            set { fotosHuellasEnabled = value; OnPropertyChanged("FotosHuellasEnabled"); }
        }
        private bool mediaFiliacionEnabled = true;
        public bool MediaFiliacionEnabled
        {
            get { return mediaFiliacionEnabled; }
            set { mediaFiliacionEnabled = value; OnPropertyChanged("MediaFiliacionEnabled"); }
        }
        private bool senasParticularesEnabled = true;
        public bool SenasParticularesEnabled
        {
            get { return senasParticularesEnabled; }
            set { senasParticularesEnabled = value; OnPropertyChanged("SenasParticularesEnabled"); }
        }
        private bool pandillasEnabled;
        public bool PandillasEnabled
        {
            get { return pandillasEnabled; }
            set { pandillasEnabled = value; OnPropertyChanged("PandillasEnabled"); }
        }
        private bool ingresoEnabled;
        public bool IngresoEnabled
        {
            get { return ingresoEnabled; }
            set
            {
                ingresoEnabled = value;
                OnPropertyChanged("IngresoEnabled");
            }
        }
        private bool selectIngresoEnabled;
        public bool SelectIngresoEnabled
        {
            get { return selectIngresoEnabled; }
            set { selectIngresoEnabled = value; OnPropertyChanged("SelectIngresoEnabled"); }
        }
        private bool identificacionEnabled;
        public bool IdentificacionEnabled
        {
            get { return identificacionEnabled; }
            set { identificacionEnabled = value; OnPropertyChanged("IdentificacionEnabled"); }
        }
        private bool trasladoEnabled;
        public bool TrasladoEnabled
        {
            get { return trasladoEnabled; }
            set { trasladoEnabled = value; OnPropertyChanged("TrasladoEnabled"); }
        }
        #endregion

        #region [VISIBLES]
        private bool contenedorIdentificacionVisible = true;
        public bool ContenedorIdentificacionVisible
        {
            get { return contenedorIdentificacionVisible; }
            set { contenedorIdentificacionVisible = value; OnPropertyChanged("ContenedorIdentificacionVisible"); }
        }
        private bool tabVisible;
        public bool TabVisible
        {
            get { return tabVisible; }
            set { tabVisible = value; OnPropertyChanged("TabVisible"); }
        }
        private bool visibleTomarFotoSenasParticulares;
        public bool VisibleTomarFotoSenasParticulares
        {
            get { return visibleTomarFotoSenasParticulares; }
            set { visibleTomarFotoSenasParticulares = value; OnPropertyChanged("VisibleTomarFotoSenasParticulares"); }
        }

        private Visibility isNombreCentroVisible = Visibility.Collapsed;
        public Visibility IsNombreCentroVisible
        {
            get { return isNombreCentroVisible; }
            set { isNombreCentroVisible = value; RaisePropertyChanged("IsNombreCentroVisible"); }
        }
        #endregion

        #region [Menu Enableds]
        private bool menuGuardarEnabled;
        public bool MenuGuardarEnabled
        {
            get { return menuGuardarEnabled; }
            set { menuGuardarEnabled = value; OnPropertyChanged("MenuGuardarEnabled"); }
        }
        private bool menuInsertarEnabled;
        public bool MenuInsertarEnabled
        {
            get { return menuInsertarEnabled; }
            set { menuInsertarEnabled = value; OnPropertyChanged("MenuInsertarEnabled"); }
        }
        private bool menuBorrarEnabled;
        public bool MenuBorrarEnabled
        {
            get { return menuBorrarEnabled; }
            set { menuBorrarEnabled = value; OnPropertyChanged("MenuBorrarEnabled"); }
        }
        private bool menuBuscarEnabled;
        public bool MenuBuscarEnabled
        {
            get { return menuBuscarEnabled; }
            set { menuBuscarEnabled = value; OnPropertyChanged("MenuBuscarEnabled"); }
        }
        private bool menuDeshacerEnabled;
        public bool MenuDeshacerEnabled
        {
            get { return menuDeshacerEnabled; }
            set { menuDeshacerEnabled = value; OnPropertyChanged("MenuDeshacerEnabled"); }
        }
        private bool menuLimpiarEnabled;
        public bool MenuLimpiarEnabled
        {
            get { return menuLimpiarEnabled; }
            set { menuLimpiarEnabled = value; OnPropertyChanged("MenuLimpiarEnabled"); }
        }
        private bool menuReporteEnabled = false;
        public bool MenuReporteEnabled
        {
            get { return menuReporteEnabled; }
            set { menuReporteEnabled = value; OnPropertyChanged("MenuReporteEnabled"); }
        }
        private bool menuAyudaEnabled = true;
        public bool MenuAyudaEnabled
        {
            get { return menuAyudaEnabled; }
            set { menuAyudaEnabled = value; OnPropertyChanged("MenuAyudaEnabled"); }
        }
        private bool menuSalirEnabled = true;
        public bool MenuSalirEnabled
        {
            get { return menuSalirEnabled; }
            set { menuSalirEnabled = value; OnPropertyChanged("MenuSalirEnabled"); }
        }
        private bool menuFichaEnabled;
        public bool MenuFichaEnabled
        {
            get { return menuFichaEnabled; }
            set { menuFichaEnabled = value; OnPropertyChanged("MenuFichaEnabled"); }
        }
        #endregion

        //
        private ObservableCollection<IMPUTADO_FILIACION> lstImputadoFiliacion;
        public ObservableCollection<IMPUTADO_FILIACION> LstImputadoFiliacion
        {
            get { return lstImputadoFiliacion; }
            set { lstImputadoFiliacion = value; OnPropertyChanged(); }
        }


        private CENTRO centroActual;
        public CENTRO CentroActual
        {
            get { return centroActual; }
            set { centroActual = value; OnPropertyChanged("CentroActual"); }
        }

        //Validaciones
        private bool bHuellasEnabled = false;
        public bool BHuellasEnabled
        {
            get { return bHuellasEnabled; }
            set { bHuellasEnabled = value; OnPropertyValidateChanged("BHuellasEnabled"); }
        }

        //private bool menuReporteEnabled = false;

        private Visibility ubicacionArbolVisibility = Visibility.Visible;
        public Visibility UbicacionArbolVisibility
        {
            get { return ubicacionArbolVisibility; }
            set { ubicacionArbolVisibility = value; OnPropertyChanged("UbicacionArbolVisibility"); }
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

        //otra info

        private string textBotonSeleccionarIngreso = "seleccionar ingreso";
        public string TextBotonSeleccionarIngreso
        {
            get { return textBotonSeleccionarIngreso; }
            set { textBotonSeleccionarIngreso = value; OnPropertyChanged("TextBotonSeleccionarIngreso"); }
        }
        private bool crearNuevoExpedienteEnabled = true;
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

        private string _SelectSexo;
        public string SelectSexo
        {
            get
            {
                return _SelectSexo;
            }
            set
            {
                _SelectSexo = value; OnPropertyChanged("SelectSexo");
            }
        }

        private string _TextApodo;


        public string TextApodo
        {
            get { return _TextApodo; }
            set { _TextApodo = value; OnPropertyChanged("TextApodo"); }
        }

        private string _TextAlias;

        public string TextAlias
        {
            get { return _TextAlias; }
            set { _TextAlias = value; OnPropertyChanged("TextAlias"); }
        }

        private short? selectEscolaridad;
        public short? SelectEscolaridad
        {
            get { return selectEscolaridad; }
            set
            {
                if (value == selectEscolaridad)
                    return;

                selectEscolaridad = value;

                OnPropertyValidateChanged("SelectEscolaridad");
            }
        }

        private short? selectOcupacion;
        public short? SelectOcupacion
        {
            get { return selectOcupacion; }
            set
            {
                if (value == selectOcupacion)
                    return;

                selectOcupacion = value;

                OnPropertyValidateChanged("SelectOcupacion");
            }
        }

        private int textEdad;
        public int TextEdad
        {
            get { return textEdad; }
            set
            {
                textEdad = value;

                OnPropertyValidateChanged("TextEdad");
            }
        }

        private short? selectEstadoCivil;
        public short? SelectEstadoCivil
        {
            get { return selectEstadoCivil; }
            set
            {
                if (value == selectEstadoCivil)
                    return;

                selectEstadoCivil = value;

                OnPropertyValidateChanged("SelectEstadoCivil");
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
                    TextBotonSeleccionarIngreso = "nuevo ingreso";
                    SelectIngresoEnabled = true;
                    foreach (var item in selectExpediente.INGRESO)
                    {
                        if (item.ID_ESTATUS_ADMINISTRATIVO != Parametro.ID_ESTATUS_ADMVO_LIBERADO)
                        {
                            TextBotonSeleccionarIngreso = "seleccionar ingreso";
                            SelectIngresoEnabled = false;
                            break;
                        }
                    }
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
        #endregion
        private async void clickSwitch(Object obj)
        {


            BanderaEntrada = true;
            switch (obj.ToString())
            {

                //Busca al chango ☻
                case "buscar_visible":
                      SelectIngresoEnabled = false;
                      TextBotonSeleccionarIngreso = "seleccionar";

                    CrearNuevoExpedienteEnabled = false;
                    this.buscarImputado();

                    break;
                //Cierra Popup
                case "buscar_salir":
                     AnioBuscar = FolioBuscar = null;
                    ApellidoPaternoBuscar = ApellidoMaternoBuscar = NombreBuscar = string.Empty;
                    TabVisible = false;
                    Imputado = ImputadoSeleccionadoAuxiliar;
                    SelectIngreso = null;
                    MenuReporteEnabled = false;
                    //SelectIngresoEnabled = false;
                    //LIMPIAMOS FILTROS
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "limpiar_menu":
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new FichaIdentificacionView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ProcesoLiberacionViewModel();
                    break;
         

                case "buscar_seleccionar":
                    try
                    {

                        //Pendiente verificar
                        //if (!PInsertar)
                        //{
                        //    new Dialogos().ConfirmacionDialogo("Validación", string.Format("Su usuario no tiene permiso para guardar."));
                        //    break;
                        //}

                        //if (!SelectIngresoEnabled)
                        //    break;
                        if (SelectExpediente != null)
                        {
                            var ingreso = SelectExpediente.INGRESO.OrderByDescending(w => w.ID_INGRESO).FirstOrDefault();
                            HabilitarGroupBox();
                            //  await StaticSourcesViewModel.CargarDatosMetodoAsync(this.getDatosIngreso);
                            // await TreeViewViewModel();

                            //PENDIENTE LIMPIAR CAMPOS
                            //LimpiarCamposDatosIngreso();
                            //LimpiarCamposIdentificacion();
                            //LimpiarDatosTraslado();
                            //LimpiarModalDocumentos();
                            //No editara   NuevoImputado = false;

                            //Pendiente
                            // EditarImputado = true;
                            Imputado = SelectExpediente;

                            AnioBuscar = Imputado.ID_ANIO;
                            FolioBuscar = Imputado.ID_IMPUTADO;
                            NombreBuscar=imputado.NOMBRE;
                            TextCalle = ingreso.DOMICILIO_CALLE;
                            textTelefono = ingreso.TELEFONO.HasValue ? new Converters().MascaraTelefono(ingreso.TELEFONO.Value.ToString()) : string.Empty;
                            //  EditarIngreso = false;
                            TabVisible = false;
                            //LimpiarCampos();
                            ClasificacionJuridicaEnabled = false;
                            EstatusAdministrativoEnabled = false;
                            CamposBusquedaEnabled = false;
                            SetValidacionesGenerales();
                            //setDatosIngreso();
                            //SelectClasificacionJuridica = "I";
                            //SelectEstatusAdministrativo = 8;
                            IngresoEnabled = true;
                            IsSelectedDatosIngreso = true;
                            TabDatosGenerales = true;
                            getDatosImputado();

                            IngresoEnabled = true;
                            IdentificacionEnabled = true;
                            //Cambio para cambiar la pantalla de Traslado. Salvador. 12/2/2015
                            //if (SelectTipoIngreso == 3)
                            //    TrasladoEnabled = true;
                            //else
                            //    TrasladoEnabled = false;

                            //if (SelectTipoIngreso == Parametro.TRASLADO_FOREANO_TIPO_INGRESO)
                            //    TrasladoEnabled = true;
                            //else
                            //    TrasladoEnabled = false;

                            MenuInsertarEnabled = true;
                            MenuDeshacerEnabled = true;
                            MenuGuardarEnabled = true;
                            MenuLimpiarEnabled = true;
                            MenuReporteEnabled = true;
                            MenuBorrarEnabled = true;
                            MenuBuscarEnabled = true;
                            MenuFichaEnabled = false;
                            MenuAyudaEnabled = true;
                            MenuSalirEnabled = true;

                            ImputadoSeleccionadoAuxiliar = Imputado;
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);

                        }
                        else
                            (new Dialogos()).ConfirmacionDialogo("Validación", "Debes seleccionar un expediente o crear uno nuevo.");
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar ingreso.", ex);
                    }
                    break;
                default:
                    break;
            }
        }

        private void HabilitarGroupBox()
        {
            GeneralVisible = true;
          //  _VisibleGroupDatosGenerales = Visibility.Visible;
          //  _VisibleAlias = Visibility.Visible;
          //  _VisibleActitudGeneral = Visibility.Visible;
          //  _VisibleSituacionJuridica = Visibility.Visible;
          //  _VisibleObservaciones = Visibility.Visible;

          ////  RaisePropertyChanged("VisibleGroupDatosGenerales");
          //  RaisePropertyChanged("VisibleAlias");
          //  RaisePropertyChanged("VisibleActitudGeneral");
          //  RaisePropertyChanged("VisibleSituacionJuridica");
          //  RaisePropertyChanged("VisibleObservaciones");


        }


        /// <summary>
        /// Metodo Utilizado para bsuqueda de imputado
        /// </summary>
        /// <param name="obj"></param>
        private async void buscarImputado(Object obj = null)
        {
            try
            {
                if (obj != null)
                {
                    //cuando es boton no se hace nada porque solamente existe el de buscar, si hay otro habra que castearlos a button y hacer la comparacion
                    var textbox = obj as TextBox;
                    if (textbox != null)
                    {
                        switch (textbox.Name)
                        {
                            case "NombreBuscar":
                                NombreBuscar = textbox.Text;
                                break;
                            case "ApellidoPaternoBuscar":
                                ApellidoPaternoBuscar = textbox.Text;
                                break;
                            case "ApellidoMaternoBuscar":
                                ApellidoMaternoBuscar = textbox.Text;
                                break;
                            case "FolioBuscar":
                                FolioBuscar = Convert.ToInt32(textbox.Text);
                                break;
                            case "AnioBuscar":
                                AnioBuscar = int.Parse(textbox.Text);
                                break;
                        }
                    }
                }
                TabVisible = false;
                ImagenIngreso = ImagenImputado = new Imagenes().getImagenPerson();
                ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                SelectIngresoEnabled = false;
                if (ListExpediente != null)
                    EmptyExpedienteVisible = ListExpediente.Count < 0;
                else
                    EmptyExpedienteVisible = true;
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar al imputado.", ex);
            }

        }


        //LOAD
        private async void FichaIdentificacionLoad(FichaIdentificacionView Window = null)
        {
            try
            {
                CentroActual = await StaticSourcesViewModel.CargarDatosAsync<CENTRO>(() => new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault());
                await StaticSourcesViewModel.CargarDatosMetodoAsync(this.getDatosGenerales);
                CamposBusquedaEnabled = true;
                
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar ficha identificación", ex);
            }
        }
        private void getDatosImputado()
        {
            getDatosGeneralesIdentificacion();
        }


        private void getDatosGeneralesIdentificacion()
        {   
            try
            {
                if (Imputado == null)
                    Imputado = new IMPUTADO();
                var ingreso = Imputado.INGRESO.OrderByDescending(w => w.ID_INGRESO).FirstOrDefault();
                #region IDENTIFICACION
                #region DatosGenerales
                SelectEscolaridad = ingreso.ID_ESCOLARIDAD == null ? -1 : ingreso.ID_ESCOLARIDAD;
                SelectOcupacion = ingreso.ID_OCUPACION == null ? -1 : ingreso.ID_ESCOLARIDAD;
                SelectEstadoCivil = ingreso.ID_ESTADO_CIVIL == null ? -1 : ingreso.ID_ESTADO_CIVIL;
                SelectSexo = string.IsNullOrEmpty(Imputado.SEXO) ? "S" : Imputado.SEXO;


                ListAlias = new ObservableCollection<ALIAS>(Imputado.ALIAS);
                ListApodo= new ObservableCollection<APODO>(Imputado.APODO);
                /*RequiereTraductor = string.IsNullOrEmpty(Imputado.TRADUCTOR) ? false : Imputado.TRADUCTOR.Equals("S") ? true : false;
                if (Imputado.TRADUCTOR != null)
                    if (Imputado.TRADUCTOR.Equals("S"))
                        RequiereTraductor = true;
                    else
                        RequiereTraductor = false;*/

                #endregion

                #region Domicilio
                //SelectPais = Imputado.ID_PAIS == null ? CentroActual.MUNICIPIO.ENTIDAD.ID_PAIS_NAC : Imputado.ID_PAIS < 1 ? CentroActual.MUNICIPIO.ENTIDAD.ID_PAIS_NAC : Imputado.ID_PAIS;
                //SelectEntidad = Imputado.ID_ENTIDAD == null ? CentroActual.ID_ENTIDAD : Imputado.ID_ENTIDAD < 1 ? CentroActual.ID_ENTIDAD : Imputado.ID_ENTIDAD;
                //SelectMunicipio = Imputado.ID_MUNICIPIO == null ? CentroActual.ID_MUNICIPIO : Imputado.ID_MUNICIPIO < 1 ? CentroActual.ID_MUNICIPIO : Imputado.ID_MUNICIPIO;
                //SelectColoniaItem = Imputado.ID_COLONIA == null ? ListColonia.Where(w => w.ID_COLONIA == -1).FirstOrDefault() : Imputado.ID_COLONIA > 1 ? ListColonia.Where(w => w.ID_COLONIA == Imputado.ID_COLONIA).FirstOrDefault() : ListColonia.Where(w => w.ID_COLONIA == -1).FirstOrDefault();

                ApellidoPaternoBuscar = Imputado.PATERNO;
                ApellidoMaternoBuscar = Imputado.MATERNO;
                TextCalle = ingreso.DOMICILIO_CALLE;
                TextNumeroExterior = ingreso.DOMICILIO_NUM_EXT;
                TextNumeroInterior = ingreso.DOMICILIO_NUM_INT;
                //AniosEstado = Imputado.RESIDENCIA_ANIOS.ToString();
                //MesesEstado = Imputado.RESIDENCIA_MESES.ToString();
                //int mes = 0, anio = 0;
                //int.TryParse(MesesEstado, out mes);
                //int.TryParse(AniosEstado, out anio);

                TextTelefono = ingreso.TELEFONO.HasValue ? new Converters().MascaraTelefono(ingreso.TELEFONO.Value.ToString()) : string.Empty;
                TextCodigoPostal = ingreso.DOMICILIO_CP.HasValue ? (int)ingreso.DOMICILIO_CP : new Nullable<int>();
                //TextDomicilioTrabajo = Imputado.DOMICILIO_TRABAJO;
                #endregion

                #region Nacimiento

                TextFechaNacimiento = Imputado.NACIMIENTO_FECHA == null ?
                    Fechas.GetFechaDateServer.AddYears(-18) :
                    Imputado.NACIMIENTO_FECHA;
                TextLugarNacimiento = Imputado.NACIMIENTO_LUGAR;
                #endregion

                #region Padres
             //   getDatosDomiciliosPadres();
                #endregion

                //agrega alias
                if (ListAlias != null && ListAlias.Count > 0)
                {
                    short ultimoAlias= Imputado.ALIAS.LastOrDefault().ID_ALIAS;
                  
                }

                //agrega apodos
                if (ListApodo != null && ListApodo.Count > 0)
                {
                    short id_apodo = 1;
                    short ultimoApodo= Imputado.APODO.LastOrDefault().ID_APODO ;
                    foreach (var entidad in Imputado.APODO.OrderBy(o=>o.ID_APODO))
                    {
                        if (entidad.IMPUTADO != null)
                        {
                            TextApodo += entidad.ID_APODO == ultimoApodo ? entidad.APODO1.Trim() + "" : entidad.APODO1.Trim() + ",";
                        }
                    }

                }

                //ListApodo = new ObservableCollection<APODO>(Imputado.APODO);
                //ListRelacionPersonalInterno = new ObservableCollection<RELACION_PERSONAL_INTERNO>(Imputado.RELACION_PERSONAL_INTERNO);
                #endregion
                StaticSourcesViewModel.SourceChanged = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al al cargar los datos generales.", ex);
            }

        }
        private void getDatosGenerales()
        {
            try
            {
                
                if (ListOcupacion == null || ListOcupacion.Count < 1)
                {
                    ListOcupacion = ListOcupacion ?? new ObservableCollection<OCUPACION>(new cOcupacion().ObtenerTodos().OrderBy(o => o.DESCR));
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ListOcupacion.Insert(0, new OCUPACION() { ID_OCUPACION = -1, DESCR = "SELECCIONE" });
                        SelectOcupacion = -1;
                    }));
                }

                if (ListEscolaridad == null || ListEscolaridad.Count < 1)
                {
                    ListEscolaridad = ListEscolaridad ?? new ObservableCollection<ESCOLARIDAD>((new cEscolaridad()).ObtenerTodos().OrderBy(o => o.DESCR));
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ListEscolaridad.Insert(0, new ESCOLARIDAD() { ID_ESCOLARIDAD = -1, DESCR = "SELECCIONE" });
                        SelectEscolaridad = -1;
                    }));
                }

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar catalogos de datos generales.", ex);
            }

        }


        private void ClickEnter(Object obj)
        {
            buscarImputado(obj);
        }

        private void SetValidacionesGenerales()
        {
            //setValidacionesDatosIngreso();
            //setValidacionesIdentificacionDatosGenerales();
            //base.AddRule(() => ImagesToSave, () => ImagesToSave != null && ImagesToSave.Count == 3, "FOTOS DEL IMPUTADO SON REQUERIDAS!");
            //base.AddRule(() => HuellasCapturadas, () => HuellasCapturadas != null && HuellasCapturadas.Count >= 1, "HUELLAS DEL IMPUTADO SON REQUERIDAS!");
            //if (SelectTipoIngreso == Parametro.TRASLADO_FOREANO_TIPO_INGRESO)
            //    setValidacionesTraslado();
        }

        private void CalcularEdad()
        {
            DateTime hoy = Fechas.GetFechaDateServer;
            if (textFechaNacimiento.HasValue)
            {
                int edad = hoy.Year - textFechaNacimiento.Value.Year;
                if (hoy.Month < textFechaNacimiento.Value.Month || (hoy.Month == textFechaNacimiento.Value.Month && hoy.Day < textFechaNacimiento.Value.Day))
                    edad--;
                TextEdad = edad;
            }
        }

        private async Task<List<IMPUTADO>> SegmentarResultadoBusqueda(int _Pag = 1)
        {
            if (string.IsNullOrEmpty(ApellidoPaternoBuscar) && string.IsNullOrEmpty(ApellidoMaternoBuscar) && string.IsNullOrEmpty(NombreBuscar) && !AnioBuscar.HasValue && !FolioBuscar.HasValue)
                return new List<IMPUTADO>();

            Pagina = _Pag;
            var result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<IMPUTADO>>(() => new cImputado().ObtenerTodos(ApellidoPaternoBuscar, ApellidoMaternoBuscar, NombreBuscar, AnioBuscar, FolioBuscar, _Pag));
            if (result.Any())
            {
                Pagina++;
                SeguirCargando = true;
            }
            else
                SeguirCargando = false;

            return result.ToList();
        }


        #region [Aux]
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        public static void RaiseStaticPropertyChanged(string propName)
        {
            EventHandler<PropertyChangedEventArgs> handler = StaticPropertyChanged;
            if (handler != null)
                handler(null, new PropertyChangedEventArgs(propName));
        }
        #endregion
    }
}
