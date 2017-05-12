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
using ControlPenales.BiometricoServiceReference;
using SSP.Controlador.Catalogo.Justicia;

namespace ControlPenales
{
    partial class RegistroDecomisoViewModel : ValidationViewModelBase
    {
        #region Generales
        private DateTime? fechaEventoD;
        public DateTime? FechaEventoD
        {
            get { return fechaEventoD; }
            set { fechaEventoD = value; 
                SetValidacionesDecomiso(); 
                OnPropertyValidateChanged("FechaEventoD"); }
        }

        private DateTime? fechaInformeD;
        public DateTime? FechaInformeD
        {
            get { return fechaInformeD; }
            set { fechaInformeD = value; 
                SetValidacionesDecomiso(); 
                OnPropertyValidateChanged("FechaInformeD"); }
        }

        private string folioD;
        public string FolioD
        {
            get { return folioD; }
            set { folioD = value; OnPropertyValidateChanged("FolioD"); }
        }

        private ObservableCollection<GRUPO_TACTICO> lstGrupoTactico;
        public ObservableCollection<GRUPO_TACTICO> LstGrupoTactico
        {
            get { return lstGrupoTactico; }
            set { lstGrupoTactico = value; OnPropertyChanged("LstGrupoTactico"); }
        }

        private short? selectedGpoTactico = -1;
        public short? SelectedGpoTactico
        {
            get { return selectedGpoTactico; }
            set { selectedGpoTactico = value; OnPropertyValidateChanged("SelectedGpoTactico"); }
        } 

        private ObservableCollection<AREA> lstArea;
        public ObservableCollection<AREA> LstArea
        {
            get { return lstArea; }
            set { lstArea = value; OnPropertyChanged("LstArea"); }
        }

        private short? selectedArea = -1;
        public short? SelectedArea
        {
            get { return selectedArea; }
            set { selectedArea = value;
                SetValidacionesDecomiso();
                if (value != null)
                {
                    if (value == -1)
                    {
                        EnabledUbicacion = true;
                    }
                    else
                    {
                        EnabledUbicacion = false;
                        SelectedCelda = null;
                        Celda = string.Empty;
                    }
                }
                OnPropertyValidateChanged("SelectedArea");
            }
        }

        private ObservableCollection<TURNO> lstTurno;
        public ObservableCollection<TURNO> LstTurno
        {
            get { return lstTurno; }
            set { lstTurno = value; OnPropertyChanged("LstTurno"); }
        }

        private short? selectedTurno = -1;
        public short? SelectedTurno
        {
            get { return selectedTurno; }
            set { selectedTurno = value; OnPropertyValidateChanged("SelectedTurno"); }
        }

      //UBICACION
        private CENTRO ubicaciones;
        public CENTRO Ubicaciones
        {
            get { return ubicaciones; }
            set { ubicaciones = value; OnPropertyChanged("Ubicaciones"); }
        }

        private CELDA selectedCelda;
        public CELDA SelectedCelda
        {
            get { return selectedCelda; }
            set { selectedCelda = value; OnPropertyChanged("SelectedCelda"); }
        }

        private string celda;
        public string Celda
        {
            get { return celda; }
            set { celda = value; OnPropertyValidateChanged("Celda"); }
        }

        //OBJETO
        private ObservableCollection<OBJETO_TIPO> lstObjetoTipo;
        public ObservableCollection<OBJETO_TIPO> LstObjetoTipo
        {
            get { return lstObjetoTipo; }
            set { lstObjetoTipo = value; OnPropertyChanged("LstObjetoTipo"); }
        }

        private OBJETO_TIPO selectedObjetoTipo;
        public OBJETO_TIPO SelectedObjetoTipo
        {
            get { return selectedObjetoTipo; }
            set { selectedObjetoTipo = value;
            if (value != null)
                SetConfiguracion();
            LimpiarObjeto();
                OnPropertyChanged("SelectedObjetoTipo"); }
        }

        private ObservableCollection<COMPANIA> lstCompania;
        public ObservableCollection<COMPANIA> LstCompania
        {
            get { return lstCompania; }
            set { lstCompania = value; OnPropertyChanged("LstCompania"); }
        }

        private short selectedCompania;
        public short SelectedCompania
        {
            get { return selectedCompania; }
            set { selectedCompania = value; OnPropertyChanged("SelectedCompania"); }
        }

        private ObservableCollection<DROGA> lstDroga;
        public ObservableCollection<DROGA> LstDroga
        {
            get { return lstDroga; }
            set { lstDroga = value; OnPropertyChanged("LstDroga"); }
        }

        private short selectedDroga;
        public short SelectedDroga
        {
            get { return selectedDroga; }
            set { selectedDroga = value; OnPropertyChanged("SelectedDroga"); }
        }

        private ObservableCollection<DROGA_UNIDAD_MEDIDA> lstUnidadMedida;
        public ObservableCollection<DROGA_UNIDAD_MEDIDA> LstUnidadMedida
        {
            get { return lstUnidadMedida; }
            set { lstUnidadMedida = value; OnPropertyChanged("LstUnidadMedida"); }
        }

        //VISIBLE CONFIGURACION OBJETO
        private bool configuracion1Visible = false;
        public bool Configuracion1Visible
        {
            get { return configuracion1Visible; }
            set { configuracion1Visible = value; OnPropertyChanged("Configuracion1Visible"); }
        }

        private bool configuracion2Visible = false;
        public bool Configuracion2Visible
        {
            get { return configuracion2Visible; }
            set { configuracion2Visible = value; OnPropertyChanged("Configuracion2Visible"); }
        }

        private bool configuracion3Visible = false;
        public bool Configuracion3Visible
        {
            get { return configuracion3Visible; }
            set { configuracion3Visible = value; OnPropertyChanged("Configuracion3Visible"); }
        }

        private bool configuracion4Visible = false;
        public bool Configuracion4Visible
        {
            get { return configuracion4Visible; }
            set { configuracion4Visible = value; OnPropertyChanged("Configuracion4Visible"); }
        }

        private bool configuracion5Visible = false;
        public bool Configuracion5Visible
        {
            get { return configuracion5Visible; }
            set { configuracion5Visible = value; OnPropertyChanged("Configuracion5Visible"); }
        }

        private bool configuracion6Visible = false;
        public bool Configuracion6Visible
        {
            get { return configuracion6Visible; }
            set { configuracion6Visible = value; OnPropertyChanged("Configuracion6Visible"); }
        }

        private bool configuracion7Visible = false;
        public bool Configuracion7Visible
        {
            get { return configuracion7Visible; }
            set { configuracion7Visible = value; OnPropertyChanged("Configuracion7Visible"); }
        }

        private bool configuracion8Visible = false;
        public bool Configuracion8Visible
        {
            get { return configuracion8Visible; }
            set { configuracion8Visible = value; OnPropertyChanged("Configuracion8Visible"); }
        }

        private Visibility oficialInvolucradoVisible = Visibility.Visible;
        public Visibility OficialInvolucradoVisible
        {
            get { return oficialInvolucradoVisible; }
            set { oficialInvolucradoVisible = value; OnPropertyChanged("OficialInvolucradoVisible"); }
        }

        private Visibility internoInvolucradoVisible = Visibility.Visible;
        public Visibility InternoInvolucradoVisible
        {
            get { return internoInvolucradoVisible; }
            set { internoInvolucradoVisible = value; OnPropertyChanged("InternoInvolucradoVisible"); }
        }

        private Visibility visitaInvolucradoVisible = Visibility.Visible;
        public Visibility VisitaInvolucradoVisible
        {
            get { return visitaInvolucradoVisible; }
            set { visitaInvolucradoVisible = value; OnPropertyChanged("VisitaInvolucradoVisible"); }
        }

        private Visibility empleadoInvolucradoVisible = Visibility.Visible;
        public Visibility EmpleadoInvolucradoVisible
        {
            get { return empleadoInvolucradoVisible; }
            set { empleadoInvolucradoVisible = value; OnPropertyChanged("EmpleadoInvolucradoVisible"); }
        }

        private Visibility proveedorInvolucradoVisible = Visibility.Visible;
        public Visibility ProveedorInvolucradoVisible
        {
            get { return proveedorInvolucradoVisible; }
            set { proveedorInvolucradoVisible = value; OnPropertyChanged("ProveedorInvolucradoVisible"); }
        }

        private Visibility objetoVisible = Visibility.Visible;
        public Visibility ObjetoVisible
        {
            get { return objetoVisible; }
            set { objetoVisible = value; OnPropertyChanged("ObjetoVisible"); }
        }

        private Visibility imagenVisible = Visibility.Visible;
        public Visibility ImagenVisible
        {
            get { return imagenVisible; }
            set { imagenVisible = value; OnPropertyChanged("ImagenVisible"); }
        }

        private Visibility imagenesVisible = Visibility.Collapsed;
        public Visibility ImagenesVisible
        {
            get { return imagenesVisible; }
            set { imagenesVisible = value; OnPropertyChanged("ImagenesVisible"); }
        }

        private string ubicacionInterno;
        public string UbicacionInterno
        {
            get { return ubicacionInterno; }
            set { ubicacionInterno = value; OnPropertyChanged("UbicacionInterno"); }
        }

        private bool enabledUbicacion = true;
        public bool EnabledUbicacion
        {
            get { return enabledUbicacion; }
            set { enabledUbicacion = value; OnPropertyChanged("EnabledUbicacion"); }
        }

        private bool SeguirCargandoDecomisos = true;

        private int PaginaDecomiso = 1;

        private DateTime fechaActual = Fechas.GetFechaDateServer;
        public DateTime FechaActual
        {
            get { return fechaActual; }
            set { fechaActual = value; OnPropertyChanged("FechaActual"); }
        }

        private bool editar = true;
        public bool Editar
        {
            get { return editar; }
            set { editar = value;
            EnabledUbicacion = editar;
            if (value)
                VisibleEditar = Visibility.Visible;
            else
                VisibleEditar = Visibility.Collapsed;
                OnPropertyChanged("Editar"); }
        }

        private Visibility visibleEditar = Visibility.Visible;
        public Visibility VisibleEditar
        {
            get { return visibleEditar; }
            set { visibleEditar = value; OnPropertyChanged("VisibleEditar"); }
        }
        #endregion

        #region Buscar persona
        private string oNoControl;
        public string ONoControl
        {
            get { return oNoControl; }
            set { oNoControl = value; OnPropertyChanged("ONoControl"); }
        }

        private string oPaterno;
        public string OPaterno
        {
            get { return oPaterno; }
            set
            {
                oPaterno = value;
                OnPropertyChanged("OPaterno");
            }
        }

        private string oMaterno;
        public string OMaterno
        {
            get { return oMaterno; }
            set
            {
                oMaterno = value;
                OnPropertyChanged("OMaterno");
            }
        }

        private string oNombre;
        public string ONombre
        {
            get { return oNombre; }
            set
            {
                oNombre = value;
                OnPropertyChanged("ONombre");
            }
        }

        //private ObservableCollection<EMPLEADO> lstOficialPop;
        //public ObservableCollection<EMPLEADO> LstOficialPop
        //{
        //    get { return lstOficialPop; }
        //    set
        //    {
        //        lstOficialPop = value;
        //        if (value != null)
        //        {
        //            OficialEmpty = value.Any() ? true : false;
        //        }
        //        else
        //            OficialEmpty = true;
        //        OnPropertyChanged("LstOficialPop");
        //    }
        //}
        private RangeEnabledObservableCollection<EMPLEADO> lstOficialPop;
        public RangeEnabledObservableCollection<EMPLEADO> LstOficialPop
        {
            get { return lstOficialPop; }
            set { lstOficialPop = value; OnPropertyChanged("LstOficialPop"); }
        }
        private int PaginaEmpleado { get; set; }
        private bool SeguirCargandoEmpleado { get; set; }

        private EMPLEADO selectedOficialPop;
        public EMPLEADO SelectedOficialPop
        {
            get { return selectedOficialPop; }
            set
            {
                selectedOficialPop = value;
                if (value != null)
                {
                    if (value.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                        ImagenOficialPop = value.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                    else
                        ImagenOficialPop = new Imagenes().getImagenPerson();
                }
                else
                    ImagenOficialPop = new Imagenes().getImagenPerson();
                OnPropertyChanged("SelectedOficialPop");
            }
        }

        private bool oficialEmpty = true;
        public bool OficialEmpty
        {
            get { return oficialEmpty; }
            set { oficialEmpty = value; OnPropertyChanged("OficialEmpty"); }
        }

        private byte[] imagenOficialPop = new Imagenes().getImagenPerson();
        public byte[] ImagenOficialPop
        {
            get { return imagenOficialPop; }
            set { imagenOficialPop = value; OnPropertyChanged("ImagenOficialPop"); }
        }

        private bool esOficial = false;
        public bool EsOficial
        {
            get { return esOficial; }
            set { esOficial = value; OnPropertyChanged("EsOficial"); }
        }
        #endregion

        #region Oficiales a Cargo
        private ObservableCollection<DECOMISO_PERSONA> lstOficialesACargo;
        public ObservableCollection<DECOMISO_PERSONA> LstOficialesACargo
        {
            get { return lstOficialesACargo; }
            set { 
                lstOficialesACargo = value; 
                OnPropertyValidateChanged("LstOficialesACargo"); }
        }

        private DECOMISO_PERSONA selectedOficialACargo;
        public DECOMISO_PERSONA SelectedOficialACargo
        {
            get { return selectedOficialACargo; }
            set { selectedOficialACargo = value; OnPropertyChanged("SelectedOficialACargo"); }
        }
        #endregion

        #region Internos Involucrados
        private ObservableCollection<DECOMISO_INGRESO> lstInternoInvolucrado;
        public ObservableCollection<DECOMISO_INGRESO> LstInternoInvolucrado
        {
            get { return lstInternoInvolucrado; }
            set { lstInternoInvolucrado = value; OnPropertyValidateChanged("LstInternoInvolucrado"); }
        }

        private DECOMISO_INGRESO selectedInternoInvolucrado;
        public DECOMISO_INGRESO SelectedInternoInvolucrado
        {
            get { return selectedInternoInvolucrado; }
            set { 
                selectedInternoInvolucrado = value;
                
                OnPropertyChanged("SelectedInternoInvolucrado"); }
        }
        #endregion

        #region Visitas Involucradas
        private ObservableCollection<DECOMISO_PERSONA> lstVisitaInvolucrada;
        public ObservableCollection<DECOMISO_PERSONA> LstVisitaInvolucrada
        {
            get { return lstVisitaInvolucrada; }
            set { lstVisitaInvolucrada = value; OnPropertyValidateChanged("LstVisitaInvolucrada"); }
        }

        private DECOMISO_PERSONA selectedVisitaInvolucrada;
        public DECOMISO_PERSONA SelectedVisitaInvolucrada
        {
            get { return selectedVisitaInvolucrada; }
            set { selectedVisitaInvolucrada = value;
            if (selectedVisitaInvolucrada != null)
                InternoVisitaVisible = Visibility.Visible;
            else
                InternoVisitaVisible = Visibility.Collapsed;
                OnPropertyChanged("SelectedVisitaInvolucrada"); }
        }

        Visibility internoVisitaVisible = Visibility.Collapsed;
        public Visibility InternoVisitaVisible
        {
            get { return internoVisitaVisible; }
            set { internoVisitaVisible = value; OnPropertyChanged("InternoVisitaVisible"); }
        }
        #endregion

        #region Abogados Involucrados
        //Buscar
        private string textNombre;
        public string TextNombre
        {
            get { return textNombre; }
            set { textNombre = value; OnPropertyChanged("TextNombre"); }
        }
        private string textPaterno;
        public string TextPaterno
        {
            get { return textPaterno; }
            set { textPaterno = value; OnPropertyChanged("TextPaterno"); }
        }
        private string textMaterno;
        public string TextMaterno
        {
            get { return textMaterno; }
            set { textMaterno = value; OnPropertyChanged("TextMaterno"); }
        }
        private RangeEnabledObservableCollection<SSP.Servidor.PERSONA> listPersonas;
        public RangeEnabledObservableCollection<SSP.Servidor.PERSONA> ListPersonas
        {
            get { return listPersonas; }
            set { listPersonas = value; OnPropertyChanged("ListPersonas"); }
        }
        private bool emptyBuscarRelacionInternoVisible = false;
        public bool EmptyBuscarRelacionInternoVisible
        {
            get { return emptyBuscarRelacionInternoVisible; }
            set { emptyBuscarRelacionInternoVisible = value; OnPropertyChanged("EmptyBuscarRelacionInternoVisible"); }
        }
        private SSP.Servidor.PERSONA selectPersona;
        public SSP.Servidor.PERSONA SelectPersona
        {
            get { return selectPersona; }
            set
            {
                selectPersona = value;
                if (value == null)
                {
                    ImagenPersona = new Imagenes().getImagenPerson();
                }
                else
                if (value.PERSONA_BIOMETRICO != null)
                {
                    var biometrico = value.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault();
                    if (biometrico != null)
                    {
                        ImagenPersona = biometrico.BIOMETRICO;
                    }
                    else
                        ImagenPersona = new Imagenes().getImagenPerson();
                }
                else
                    ImagenPersona = new Imagenes().getImagenPerson();
                OnPropertyChanged("SelectPersona");
            }
        }
        private byte[] imagenPersona = new Imagenes().getImagenPerson();
        public byte[] ImagenPersona
        {
            get { return imagenPersona; }
            set { imagenPersona = value; OnPropertyChanged("ImagenPersona"); }
        }

        private ObservableCollection<DECOMISO_PERSONA> lstAbogadoInvolucrada;
        public ObservableCollection<DECOMISO_PERSONA> LstAbogadoInvolucrada
        {
            get { return lstAbogadoInvolucrada; }
            set { lstAbogadoInvolucrada = value; OnPropertyValidateChanged("LstAbogadoInvolucrada"); }
        }

        private DECOMISO_PERSONA selectedAbogadoInvolucrada;
        public DECOMISO_PERSONA SelectedAbogadoInvolucrada
        {
            get { return selectedAbogadoInvolucrada; }
            set
            {
                selectedAbogadoInvolucrada = value;
                if (selectedAbogadoInvolucrada != null)
                    AbogadoInvolucradoVisible = Visibility.Visible;
                else
                    AbogadoInvolucradoVisible = Visibility.Collapsed;
                OnPropertyChanged("SelectedVisitaInvolucrada");
            }
        }

        Visibility abogadoInvolucradoVisible = Visibility.Visible;
        public Visibility AbogadoInvolucradoVisible
        {
            get { return abogadoInvolucradoVisible; }
            set { abogadoInvolucradoVisible = value;
            if (LstAbogadoInvolucrada != null)
            {
                if(LstAbogadoInvolucrada.Count > 0)
                    abogadoInvolucradoVisible = Visibility.Collapsed;
                else
                    abogadoInvolucradoVisible = Visibility.Visible;
            }
            else
                abogadoInvolucradoVisible = Visibility.Visible;
                OnPropertyChanged("AbogadoInvolucradoVisible"); }
        }
        #endregion

        #region Empleados Involucrados
        private ObservableCollection<DECOMISO_PERSONA> lstEmpleadoInvolucrado;
        public ObservableCollection<DECOMISO_PERSONA> LstEmpleadoInvolucrado
        {
            get { return lstEmpleadoInvolucrado; }
            set { lstEmpleadoInvolucrado = value; OnPropertyValidateChanged("LstEmpleadoInvolucrado"); }
        }

        private DECOMISO_PERSONA selectedEmpleadoInvolucrado;
        public DECOMISO_PERSONA SelectedEmpleadoInvolucrado
        {
            get { return selectedEmpleadoInvolucrado; }
            set { selectedEmpleadoInvolucrado = value; 
                OnPropertyChanged("SelectedEmpleadoInvolucrado"); }
        }
        #endregion

        #region Proveedores Involucrados
        private ObservableCollection<DECOMISO_PERSONA> lstProveedoresInvolucrados;
        public ObservableCollection<DECOMISO_PERSONA> LstProveedoresInvolucrados
        {
            get { return lstProveedoresInvolucrados; }
            set { lstProveedoresInvolucrados = value; OnPropertyValidateChanged("LstProveedoresInvolucrados"); }
        }

        private DECOMISO_PERSONA selectedProveedoresInvolucrados;
        public DECOMISO_PERSONA SelectedProveedoresInvolucrados
        {
            get { return selectedProveedoresInvolucrados; }
            set { selectedProveedoresInvolucrados = value; OnPropertyChanged("SelectedProveedoresInvolucrados"); }
        }
        #endregion

        #region Objetos Decomisados
        private ObservableCollection<DECOMISO_OBJETO> lstObjetos;
        public ObservableCollection<DECOMISO_OBJETO> LstObjetos
        {
            get { return lstObjetos; }
            set { 
                lstObjetos = value; 
                OnPropertyValidateChanged("LstObjetos"); }
        }

        private DECOMISO_OBJETO selectedObjeto;
        public DECOMISO_OBJETO SelectedObjeto
        {
            get { return selectedObjeto; }
            set { selectedObjeto = value;
                 OnPropertyChanged("SelectedObjeto"); 
            }
        }

        private ObservableCollection<DECOMISO_IMAGEN> lstImagenes;
        public ObservableCollection<DECOMISO_IMAGEN> LstImagenes
        {
            get { return lstImagenes; }
            set { lstImagenes = value; 
                  OnPropertyValidateChanged("LstImagenes"); }
        }

        private DECOMISO_IMAGEN selectedImagen;
        public DECOMISO_IMAGEN SelectedImagen
        {
            get { return selectedImagen; }
            set { selectedImagen = value; OnPropertyChanged("SelectedImagen"); }
        }

        private ObservableCollection<DECOMISO_FABRICANTE> lstFabricante;
        public ObservableCollection<DECOMISO_FABRICANTE> LstFabricante
        {
            get { return lstFabricante; }
            set { lstFabricante = value; OnPropertyChanged("LstFabricante"); }
        }

        private DECOMISO_FABRICANTE selectedFabricante;
        public DECOMISO_FABRICANTE SelectedFabricante
        {
            get { return selectedFabricante; }
            set { selectedFabricante = value;
            LstModelo = new ObservableCollection<DECOMISO_MODELO>();
            if (value != null)
            {
                if (value.ID_FABRICANTE != -1)
                {
                    LstModelo = new ObservableCollection<DECOMISO_MODELO>(value.DECOMISO_MODELO);
                }
            }
            LstModelo.Insert(0, new DECOMISO_MODELO() { ID_MODELO = -1, DESCR = "SELECCIONE"});
            OModelo = -1;
            OnPropertyChanged("SelectedFabricante"); }
        }

        private ObservableCollection<DECOMISO_MODELO> lstModelo;
        public ObservableCollection<DECOMISO_MODELO> LstModelo
        {
            get { return lstModelo; }
            set { lstModelo = value; OnPropertyChanged("LstModelo"); }
        }

        private short? oTipo;
        public short? OTipo
        {
            get { return oTipo; }
            set { oTipo = value;
                if (value != null)
                {
                    if (value == -1)
                        ImagenesVisible = Visibility.Collapsed;
                    else
                        ImagenesVisible = Visibility.Visible;

                        int[] objetos = { 41, 49, 50, 51 };
                        if (objetos.Count(w => w == value) > 0)
                            LstCompania = new ObservableCollection<COMPANIA>(new cCompania().ObtenerTodos(string.Empty, value).OrderBy(w => w.DESCR));
                        else
                            LstCompania = new ObservableCollection<COMPANIA>();
                        LstCompania.Insert(0, new COMPANIA() { ID_COMPANIA = -1, DESCR = "SELECCIONE" });
                        OCompania = -1;
                }
                OnPropertyChanged("OTipo"); }
        }

        private string oDescripcion;
        public string ODescripcion
        {
            get { return oDescripcion; }
            set { oDescripcion = value; OnPropertyChanged("ODescripcion"); }
        }

        private short? oCantidad;
        public short? OCantidad
        {
            get { return oCantidad; }
            set { oCantidad = value; OnPropertyChanged("OCantidad"); }
        }

        private string oComentario;
        public string OComentario
        {
            get { return oComentario; }
            set { oComentario = value; OnPropertyChanged("OComentario"); }
        }

        private short? oFabricante;
        public short? OFabricante
        {
            get { return oFabricante; }
            set { oFabricante = value; OnPropertyChanged("OFabricante"); }
        }

        private short? oModelo;
        public short? OModelo
        {
            get { return oModelo; }
            set { oModelo = value; OnPropertyChanged("OModelo"); }
        }

        private string oSerie;
        public string OSerie
        {
            get { return oSerie; }
            set { oSerie = value; OnPropertyChanged("OSerie"); }
        }

        private short? oTipoDroga;
        public short? OTipoDroga
        {
            get { return oTipoDroga; }
            set { oTipoDroga = value; OnPropertyChanged("OTipoDroga"); }
        }

        private short? oUnidadMedida;
        public short? OUnidadMedida
        {
            get { return oUnidadMedida; }
            set { oUnidadMedida = value; OnPropertyChanged("OUnidadMedida"); }
        }

        private short? oDosis;
        public short? ODosis
        {
            get { return oDosis; }
            set { oDosis = value; OnPropertyChanged("ODosis"); }
        }

        private short? oEnvoltorios;
        public short? OEnvoltorios
        {
            get { return oEnvoltorios; }
            set { oEnvoltorios = value; OnPropertyChanged("OEnvoltorios"); }
        }

        private short? oCompania;
        public short? OCompania
        {
            get { return oCompania; }
            set { oCompania = value; OnPropertyChanged("OCompania"); }
        }

        private string oTelefono;
        public string OTelefono
        {
            get {
                if (oTelefono == null)
                    return string.Empty;
                return new Converters().MascaraTelefono(oTelefono);
            }
            set
            {
                oTelefono =value;
                OnPropertyChanged("OTelefono"); 
            }
        }
        
        private string oIMEI;
        public string OIMEI
        {
            get { return oIMEI; }
            set { oIMEI = value; OnPropertyChanged("OIMEI"); }
        }

        private string oSIMSerie;
        public string OSIMSerie
        {
            get { return oSIMSerie; }
            set { oSIMSerie = value; OnPropertyChanged("OSIMSerie"); }
        }

        private string oCapacidad;
        public string OCapacidad
        {
            get { return oCapacidad; }
            set { oCapacidad = value; OnPropertyChanged("OCapacidad"); }
        }

        private string btnLimpiarText = "Limpiar";
        public string BtnLimpiarText
        {
            get { return btnLimpiarText; }
            set { btnLimpiarText = value; OnPropertyChanged("BtnLimpiarText"); }
        }

        private Visibility lineasGuiaFoto = Visibility.Collapsed;
        public Visibility LineasGuiaFoto
        {
            get { return lineasGuiaFoto; }
            set { lineasGuiaFoto = value; OnPropertyChanged("LineasGuiaFoto"); }
        }

        private Visibility comboFrontBackFotoVisible = Visibility.Collapsed;
        public Visibility ComboFrontBackFotoVisible
        {
            get { return comboFrontBackFotoVisible; }
            set { comboFrontBackFotoVisible = value; OnPropertyChanged("ComboFrontBackFotoVisible"); }
        }

        private byte[] imagenObjeto;
        public byte[] ImagenObjeto
        {
            get { return imagenObjeto; }
            set { imagenObjeto = value; OnPropertyChanged("ImagenObjeto"); }
        }
        #endregion

        #region Resumen
        private string resumenD;
        public string ResumenD
        {
            get { return resumenD; }
            set { 
                resumenD = value; 
                OnPropertyValidateChanged("ResumenD"); }
        }
        #endregion

        #region Tomar Fotos
        private WebCam CamaraWeb;
        
        private ImageSourceToSave _FotoFrente;
        public ImageSourceToSave FotoFrente
        {
            get { return _FotoFrente; }
            set { _FotoFrente = value; }
        }
        
        private List<ImageSourceToSave> _ImagesToSave;
        public List<ImageSourceToSave> ImagesToSave
        {
            get { return _ImagesToSave; }
            set { _ImagesToSave = value; }
        }
        
        private List<ImageSourceToSave> _ImageFrontal;
        public List<ImageSourceToSave> ImageFrontal
        {
            get { return _ImageFrontal; }
            set { _ImageFrontal = value; }
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
        
        private bool botonTomarFotoEnabled;
        public bool BotonTomarFotoEnabled
        {
            get { return botonTomarFotoEnabled; }
            set { botonTomarFotoEnabled = value; OnPropertyChanged("BotonTomarFotoEnabled"); }
        }
        
        private bool FotoTomada = false;
        #endregion

        #region Buscar
        private int PaginaIngreso { get; set; }
        
        private bool SeguirCargandoIngreso { get; set; }

        private RangeEnabledObservableCollection<INGRESO> lstIngreso;
        public RangeEnabledObservableCollection<INGRESO> LstIngreso
        {
            get { return lstIngreso; }
            set { lstIngreso = value; OnPropertyChanged("LstIngreso"); }
        }
        #endregion

        #region Impresion
        private string iOficioSeguridad;
        public string IOficioSeguridad
        {
            get { return iOficioSeguridad; }
            set { iOficioSeguridad = value; OnPropertyChanged("IOficioSeguridad"); }
        }

        private string iJefeTurno;
        public string IJefeTurno
        {
            get { return iJefeTurno; }
            set { iJefeTurno = value; OnPropertyChanged("IJefeTurno"); }
        }

        private string iComandante;
        public string IComandante
        {
            get { return iComandante; }
            set { iComandante = value; OnPropertyChanged("IComandante"); }
        }

        private string iOficioComandancia1;
        public string IOficioComandancia1
        {
            get { return iOficioComandancia1; }
            set { iOficioComandancia1 = value; OnPropertyChanged("IOficioComandancia1"); }
        }

        private string iOficioComandancia2;
        public string IOficioComandancia2
        {
            get { return iOficioComandancia2; }
            set { iOficioComandancia2 = value; OnPropertyChanged("IOficioComandancia2"); }
        }

        private bool fComandante = true;
        public bool FComandante
        {
            get { return fComandante; }
            set { fComandante = value; OnPropertyChanged("FComandante"); }
        }

        private bool fDirector = false;
        public bool FDirector
        {
            get { return fDirector; }
            set { fDirector = value; OnPropertyChanged("FDirector"); }
        }

        private bool fParteInformativo = false;
        public bool FParteInformativo
        {
            get { return fParteInformativo; }
            set { fParteInformativo = value;
            DatosParteInformativo = value ? Visibility.Visible : Visibility.Collapsed;
            //if(value)
                //ValidacionImpresionDocumento();
            //else
              //  base.ClearRules();
                OnPropertyChanged("FParteInformativo"); }
        }

        private Visibility datosParteInformativo = Visibility.Collapsed;
        public Visibility DatosParteInformativo
        {
            get { return datosParteInformativo; }
            set { datosParteInformativo = value; OnPropertyChanged("DatosParteInformativo"); }
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

        #region Buscar Interno
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

        private bool emptyExpedienteVisible = false;
        public bool EmptyExpedienteVisible
        {
            get { return emptyExpedienteVisible; }
            set { emptyExpedienteVisible = value; OnPropertyChanged("EmptyExpedienteVisible"); }
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
                    SelectIngresoEnabled = false;
                    ImagenIngreso = ImagenImputado = new Imagenes().getImagenPerson();
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
                SelectIngresoEnabled = true;
                OnPropertyChanged("SelectIngreso");
            }
        }

        private bool emptyIngresoVisible = false;
        public bool EmptyIngresoVisible
        {
            get { return emptyIngresoVisible; }
            set { emptyIngresoVisible = value; OnPropertyChanged("EmptyIngresoVisible"); }
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

        private string textBotonSeleccionarIngreso = "seleccionar ingreso";
        public string TextBotonSeleccionarIngreso
        {
            get { return textBotonSeleccionarIngreso; }
            set { textBotonSeleccionarIngreso = value; OnPropertyChanged("TextBotonSeleccionarIngreso"); }
        }

        private bool selectIngresoEnabled = false;
        public bool SelectIngresoEnabled
        {
            get { return selectIngresoEnabled; }
            set { selectIngresoEnabled = value; OnPropertyChanged("SelectIngresoEnabled"); }
        }

        private bool crearNuevoExpedienteEnabled = false;
        public bool CrearNuevoExpedienteEnabled
        {
            get { return crearNuevoExpedienteEnabled; }
            set { crearNuevoExpedienteEnabled = value; OnPropertyChanged("CrearNuevoExpedienteEnabled"); }
        }

        private bool SeguirCargando;
        private int PaginaInterno = 1;
        #endregion

        #region Tabs
        private int indexTab;
        public int IndexTab
        {
            get { return indexTab; }
            set { indexTab = value; OnPropertyChanged("IndexTab"); }
        }
        #endregion
    }
}
