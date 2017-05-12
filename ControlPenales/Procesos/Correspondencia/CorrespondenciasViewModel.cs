using ControlPenales.BiometricoServiceReference;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ControlPenales
{
    partial class CorrespondenciasViewModel : ValidationViewModelBase
    {
        public CorrespondenciasViewModel()
        {
            //CargarDatos();
        }

        #region [Propiedades]
        #region [Registro Correspondencia]
        private string _DepPaterno;
        public string DepPaterno
        {
            get { return _DepPaterno; }
            set
            {
                _DepPaterno = value;
                OnPropertyChanged("DepPaterno");
            }
        }

        private string _DepMaterno;
        public string DepMaterno
        {
            get { return _DepMaterno; }
            set
            {
                _DepMaterno = value;
                OnPropertyChanged("DepMaterno");
            }
        }

        private string _DepNombre;
        public string DepNombre
        {
            get { return _DepNombre; }
            set
            {
                _DepNombre = value;
                OnPropertyChanged("DepNombre");
            }
        }

        private string _DesPaterno;
        public string DesPaterno
        {
            get { return _DesPaterno; }
            set
            {
                _DesPaterno = value;
                OnPropertyChanged("DesPaterno");
            }
        }

        private string _DesMaterno;
        public string DesMaterno
        {
            get { return _DesMaterno; }
            set
            {
                _DesMaterno = value;
                OnPropertyChanged("DesMaterno");
            }
        }

        private string _DesNombre;
        public string DesNombre
        {
            get { return _DesNombre; }
            set
            {
                _DesNombre = value;
                OnPropertyChanged("DesNombre");
            }
        }

        private string _UbicacionFisica;
        public string UbicacionFisica
        {
            get { return _UbicacionFisica; }
            set
            {
                _UbicacionFisica = value;
                OnPropertyChanged("UbicacionFisica");
            }
        }

        private string _Remitente;
        public string Remitente
        {
            get { return _Remitente; }
            set
            {
                _Remitente = value;
                OnPropertyChanged("Remitente");
            }
        }

        public string UsuarioRecepcion
        {
            get { return StaticSourcesViewModel.UsuarioLogin.Username; }
        }

        private string _ObservacionRecepcion;
        public string ObservacionRecepcion
        {
            get { return _ObservacionRecepcion; }
            set
            {
                _ObservacionRecepcion = value;
                OnPropertyChanged("ObservacionRecepcion");
            }
        }

        public string FechaRegistroCorrespondencia
        {
            get { return Fechas.GetFechaDateServer.ToShortDateString(); }
        }

        public string HoraRegistro
        {
            get { return Fechas.GetFechaDateServer.ToShortTimeString(); }
        }

        private List<RegistroEntrega> ListSelectRegistroEntrega = new List<RegistroEntrega>();

        private RegistroEntrega _SelectRegistroEntrega;
        public RegistroEntrega SelectRegistroEntrega
        {
            get { return _SelectRegistroEntrega; }
            set
            {
                _SelectRegistroEntrega = value;
                OnPropertyChanged("SelectRegistroEntrega");
            }
        }

        private ObservableCollection<RegistroEntrega> _ListaRegistroEntrega;
        public ObservableCollection<RegistroEntrega> ListaRegistroEntrega
        {
            get { return _ListaRegistroEntrega; }
            set
            {
                _ListaRegistroEntrega = value;

                EmptyRegistroEntrega = _ListaRegistroEntrega.Count > 0 ? Visibility.Collapsed : Visibility.Visible;

                OnPropertyChanged("ListaRegistroEntrega");
            }
        }

        private Visibility _EmptyRegistroEntrega = Visibility.Visible;
        public Visibility EmptyRegistroEntrega
        {
            get { return _EmptyRegistroEntrega; }
            set
            {
                _EmptyRegistroEntrega = value;
                OnPropertyChanged("EmptyRegistroEntrega");
            }
        }
        #endregion
        #region [Busqeuda Persona]
        private SSP.Servidor.PERSONA selectPersona;
        public SSP.Servidor.PERSONA SelectPersona
        {
            get { return selectPersona; }
            set
            {
                selectPersona = value;
                ImagenPersona = value == null ?
                    new Imagenes().getImagenPerson() :
                        value.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any() ?
                            value.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO :
                                new Imagenes().getImagenPerson();
                OnPropertyChanged("SelectPersona");
            }
        }

        private byte[] imagenPersona = new Imagenes().getImagenPerson();
        public byte[] ImagenPersona
        {
            get { return imagenPersona; }
            set { imagenPersona = value; OnPropertyChanged("ImagenPersona"); }
        }

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

        private ObservableCollection<SSP.Servidor.PERSONA> listPersonas;
        public ObservableCollection<SSP.Servidor.PERSONA> ListPersonas
        {
            get { return listPersonas; }
            set
            {
                listPersonas = value;
                EmptyBuscarRelacionInternoVisible = (ListPersonas.Count < 0);
                OnPropertyChanged("ListPersonas");
            }
        }

        private bool emptyBuscarRelacionInternoVisible = true;
        public bool EmptyBuscarRelacionInternoVisible
        {
            get { return emptyBuscarRelacionInternoVisible; }
            set { emptyBuscarRelacionInternoVisible = value; OnPropertyChanged("EmptyBuscarRelacionInternoVisible"); }
        }
        #endregion
        #region [Busqueda Imputado]
        private int? _AnioBuscar;
        public int? AnioBuscar
        {
            get { return _AnioBuscar; }
            set
            {
                _AnioBuscar = value;
                OnPropertyChanged("AnioBuscar");
            }
        }

        private int? _FolioBuscar;
        public int? FolioBuscar
        {
            get { return _FolioBuscar; }
            set
            {
                _FolioBuscar = value;
                OnPropertyChanged("FolioBuscar");
            }
        }

        private string _ApellidoPaternoBuscar;
        public string ApellidoPaternoBuscar
        {
            get { return _ApellidoPaternoBuscar; }
            set
            {
                _ApellidoPaternoBuscar = value;
                OnPropertyChanged("ApellidoPaternoBuscar");
            }
        }

        private string _ApellidoMaternoBuscar;
        public string ApellidoMaternoBuscar
        {
            get { return _ApellidoMaternoBuscar; }
            set
            {
                _ApellidoMaternoBuscar = value;
                OnPropertyChanged("ApellidoMaternoBuscar");
            }
        }

        private string _NombreBuscar;
        public string NombreBuscar
        {
            get { return _NombreBuscar; }
            set
            {
                _NombreBuscar = value;
                OnPropertyChanged("NombreBuscar");
            }
        }

        private ObservableCollection<IMPUTADO> listExpediente;
        public ObservableCollection<IMPUTADO> ListExpediente
        {
            get { return listExpediente; }
            set
            {
                listExpediente = value;
                EmptyExpedienteVisible = ListExpediente.Count < 0;
                OnPropertyChanged("ListExpediente");
            }
        }

        private bool emptyExpedienteVisible = true;
        public bool EmptyExpedienteVisible
        {
            get { return emptyExpedienteVisible; }
            set { emptyExpedienteVisible = value; OnPropertyChanged("EmptyExpedienteVisible"); }
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

        private bool emptyIngresoVisible = true;
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
            }
        }

        private bool crearNuevoExpedienteEnabled;
        public bool CrearNuevoExpedienteEnabled
        {
            get { return crearNuevoExpedienteEnabled; }
            set { crearNuevoExpedienteEnabled = value; OnPropertyChanged("CrearNuevoExpedienteEnabled"); }
        }

        private string textBotonSeleccionarIngreso = "seleccionar imputado";
        public string TextBotonSeleccionarIngreso
        {
            get { return textBotonSeleccionarIngreso; }
            set { textBotonSeleccionarIngreso = value; OnPropertyChanged("TextBotonSeleccionarIngreso"); }
        }
        #endregion
        #endregion

        #region [Evento]
        public ICommand WindowLoaded
        {
            get { return new DelegateCommand<RegistroCorrespondenciaView>(RegistroCorrespondenciaLoad); }
        }

        private ICommand _onClick;
        public ICommand OnClick
        {
            get { return _onClick ?? (_onClick = new RelayCommand(clickSwitch)); }
        }

        private ICommand enterClick;
        public ICommand EnterClick
        {
            get { return enterClick ?? (enterClick = new RelayCommand(ClickEnter)); }
        }

        private ICommand _BuscarClick;
        public ICommand BuscarClick
        {
            get
            {
                return _BuscarClick ?? (_BuscarClick = new RelayCommand(EnterExpediente));
            }
        }

        private ICommand mouseDoubleClickCommand;
        public ICommand MouseDoubleClickCommand
        {
            get
            {
                if (mouseDoubleClickCommand == null)
                {
                    mouseDoubleClickCommand = new RelayCommand(item =>
                        {
                            if (!ListSelectRegistroEntrega.Where(w => w == item).Any())
                            {
                                ListSelectRegistroEntrega.Add((RegistroEntrega)item);

                                _ListaRegistroEntrega.Where(w => w == item).FirstOrDefault().Entrega = true;
                                ListaRegistroEntrega = new ObservableCollection<RegistroEntrega>(_ListaRegistroEntrega);
                            }
                            else
                            {
                                ListSelectRegistroEntrega.Remove((RegistroEntrega)item);

                                _ListaRegistroEntrega.Where(w => w == item).FirstOrDefault().Entrega = false;
                                ListaRegistroEntrega = new ObservableCollection<RegistroEntrega>(_ListaRegistroEntrega);
                            }
                        });
                }
                return mouseDoubleClickCommand;
            }
        }
        #endregion

        #region [Metodos]
        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                #region CORRESPONDENCIA
                case "limpiar_menu":
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new RegistroCorrespondenciaView();
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.CorrespondenciasViewModel();
                    break;
                case "salir_menu":
                    PrincipalViewModel.SalirMenu();
                    break;
                case "MarcarRegistro_RegistroCorrespondencia":
                    if (!pEditar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    var dal = new cCorrespondencia();
                    foreach (var item in ListSelectRegistroEntrega)
                    {
                        dal.Actualizar(new CORRESPONDENCIA
                        {
                            CONFIRMACION_RECIBIDO = "S",
                            ENTREGA_FEC = Fechas.GetFechaDateServer,
                            ID_ANIO = item.Correspondencia.ID_ANIO,
                            ID_CENTRO = item.Correspondencia.ID_CENTRO,
                            ID_CONSEC = item.Correspondencia.ID_CONSEC,
                            ID_DEPOSITANTE = item.Correspondencia.ID_DEPOSITANTE,
                            ID_EMPLEADO = item.Correspondencia.ID_EMPLEADO,
                            ID_IMPUTADO = item.Correspondencia.ID_IMPUTADO,
                            ID_INGRESO = item.Correspondencia.ID_INGRESO,
                            OBSERV = item.Correspondencia.OBSERV,
                            RECEPCION_FEC = item.Correspondencia.RECEPCION_FEC,
                            REMITENTE = item.Correspondencia.REMITENTE
                        });
                    }

                    ListSelectRegistroEntrega = new List<RegistroEntrega>();
                    CargarDatos();
                    break;
                case "Registro_Correspondencia":

                    break;
                case "cancelar_RegistroCorrespondencia":
                    DepPaterno = string.Empty;
                    DepMaterno = string.Empty;
                    DepNombre = string.Empty;
                    DesPaterno = string.Empty;
                    DesMaterno = string.Empty;
                    DesNombre = string.Empty;
                    UbicacionFisica = string.Empty;
                    Remitente = string.Empty;
                    ObservacionRecepcion = string.Empty;

                    OnPropertyChanged("UsuarioRecepcion");
                    OnPropertyChanged("FechaRegistroCorrespondencia");
                    OnPropertyChanged("HoraRegistro");
                    //PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.REGISTRO_CORRESPONDENCIA);
                    break;
                case "LimpiarRegistroCorrespondencia":
                    DepPaterno = string.Empty;
                    DepMaterno = string.Empty;
                    DepNombre = string.Empty;
                    DesPaterno = string.Empty;
                    DesMaterno = string.Empty;
                    DesNombre = string.Empty;
                    UbicacionFisica = string.Empty;
                    Remitente = string.Empty;
                    ObservacionRecepcion = string.Empty;

                    OnPropertyChanged("UsuarioRecepcion");
                    OnPropertyChanged("FechaRegistroCorrespondencia");
                    OnPropertyChanged("HoraRegistro");

                    SelectExpediente = null;
                    SelectIngreso = null;
                    SelectPersona = null;
                    break;
                case "guardar_menu"://"RegistroCorrespondencia":
                    if (!pInsertar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }

                    if (SelectExpediente == null || SelectIngreso == null || SelectPersona == null || string.IsNullOrEmpty(Remitente) || string.IsNullOrEmpty(ObservacionRecepcion))
                    {
                        await new Dialogos().ConfirmacionDialogoReturn("Validación", "Faltan campos por capturar");
                        break;
                    }

                    ///TODO: Agregar el id del empleado cuanso se tenga la tabla empleado >>Ernesto G.
                    if (new cCorrespondencia().Insertar(new CORRESPONDENCIA { ID_ANIO = SelectIngreso.ID_ANIO, CONFIRMACION_RECIBIDO = "N", RECEPCION_FEC = Fechas.GetFechaDateServer, ID_INGRESO = SelectIngreso.ID_INGRESO, ID_IMPUTADO = selectIngreso.ID_IMPUTADO, ID_CENTRO = selectIngreso.ID_CENTRO, OBSERV = ObservacionRecepcion, REMITENTE = Remitente, ID_DEPOSITANTE = SelectPersona.ID_PERSONA, ID_EMPLEADO = /*StaticSourcesViewModel.UsuarioLogin.Id_Empleado */null }))
                    {
                        Remitente = ObservacionRecepcion = string.Empty;
                        await new Dialogos().ConfirmacionDialogoReturn("Éxito", "Registro capturado exitosamente");
                    }
                    else
                        await new Dialogos().ConfirmacionDialogoReturn("Error", "Error en el registro de correspondencia");

                    CargarDatos();
                    break;
                case "BuscarImputado_Correspondencia":
                    if (!pConsultar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();

                    NombreBuscar = DesNombre;
                    ApellidoPaternoBuscar = DesPaterno;
                    ApellidoMaternoBuscar = DesMaterno;

                    if (!string.IsNullOrEmpty(NombreBuscar) || !string.IsNullOrEmpty(ApellidoPaternoBuscar) || !string.IsNullOrEmpty(ApellidoMaternoBuscar) || FolioBuscar.HasValue || AnioBuscar.HasValue)
                        BuscarImputado();

                    //PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.REGISTRO_CORRESPONDENCIA);
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "BuscarVisitante_Correspondencia":
                    if (!pConsultar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    TextNombre = DepNombre;
                    TextPaterno = DepPaterno;
                    TextMaterno = DepMaterno;

                    if (!string.IsNullOrEmpty(TextNombre) || !string.IsNullOrEmpty(TextPaterno) || !string.IsNullOrEmpty(TextMaterno))
                        BuscarPersonas();

                    //PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.REGISTRO_CORRESPONDENCIA);
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                    break;
                #endregion
                #region BUSCAR_PERSONA
                case "seleccionar_buscar_persona":
                    if (SelectPersona == null)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Error!", "Debes seleccionar una persona.");
                        return;
                    }

                    DepPaterno = SelectPersona.PATERNO;
                    DepMaterno = SelectPersona.MATERNO;
                    DepNombre = SelectPersona.NOMBRE;

                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                    //PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.REGISTRO_CORRESPONDENCIA);
                    break;
                case "cancelar_buscar_persona":
                    TextNombre = string.Empty;
                    TextPaterno = string.Empty;
                    TextMaterno = string.Empty;

                    ListPersonas = new ObservableCollection<SSP.Servidor.PERSONA>();

                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                    //PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.REGISTRO_CORRESPONDENCIA);
                    break;
                case "buscar_visitante":
                    if (!string.IsNullOrEmpty(TextNombre) || !string.IsNullOrEmpty(TextPaterno) || !string.IsNullOrEmpty(TextMaterno))
                        BuscarPersonas();
                    break;
                #endregion
                #region BUSCAR_IMPUTADO
                case "buscar_salir":
                    AnioBuscar = FolioBuscar = null;
                    ApellidoPaternoBuscar = ApellidoMaternoBuscar = NombreBuscar = string.Empty;
                    ListExpediente = new ObservableCollection<IMPUTADO>();
                    SelectExpediente = null;
                    SelectIngreso = null;
                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();

                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    //PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.REGISTRO_CORRESPONDENCIA);
                    break;
                case "nueva_busqueda":
                    ListExpediente = new ObservableCollection<IMPUTADO>();
                    AnioBuscar = FolioBuscar = new Nullable<int>();
                    ApellidoPaternoBuscar = ApellidoMaternoBuscar = NombreBuscar = string.Empty;
                    SelectExpediente = new IMPUTADO();
                    EmptyExpedienteVisible = true;
                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    //SelectIngresoEnabled = false;
                    break;
                case "buscar_seleccionar":
                    if (SelectExpediente != null)
                    {
                        if (SelectIngreso != null)
                        {
                            if (SelectIngreso.ID_ESTATUS_ADMINISTRATIVO != Parametro.ID_ESTATUS_ADMVO_LIBERADO)
                            {
                                DesPaterno = SelectExpediente.PATERNO;
                                DesMaterno = SelectExpediente.MATERNO;
                                DesNombre = SelectExpediente.NOMBRE;

                                var ubicacion = SelectIngreso.CAMA;
                                if (ubicacion != null)
                                    UbicacionFisica = ubicacion.CELDA.SECTOR.EDIFICIO.DESCR.Trim() + " " + ubicacion.CELDA.SECTOR.DESCR.Trim() + " " + ubicacion.CELDA.ID_CELDA.Trim();

                                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                                ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                                //PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.REGISTRO_CORRESPONDENCIA);
                            }
                            else
                                (new Dialogos()).ConfirmacionDialogo("Ingreso no vigente.", "Debes seleccionar un ingreso vigente.");
                        }
                        else
                            (new Dialogos()).ConfirmacionDialogo("Validación", "Debes seleccionar un ingreso.");
                    }
                    else
                        (new Dialogos()).ConfirmacionDialogo("Validación", "Debes seleccionar un expediente o crear uno nuevo.");

                    break;
                #endregion
            }
        }

        private void ClickEnter(Object obj)
        {
            if (!pConsultar)
            {
                new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                return;
            }
            base.ClearRules();
            if (obj != null)
            {
                var textbox = (TextBox)obj;
                switch (textbox.Name)
                {
                    case "NombreBuscar":
                        TextNombre = textbox.Text;
                        break;
                    case "PaternoBuscar":
                        TextPaterno = textbox.Text;
                        break;
                    case "MaternoBuscar":
                        TextMaterno = textbox.Text;
                        break;

                    case "DepNombre":
                    case "DepMaterno":
                    case "DepPaterno":
                        TextNombre = DepNombre;
                        TextPaterno = DepPaterno;
                        TextMaterno = DepMaterno;
                        if (!string.IsNullOrEmpty(TextNombre) || !string.IsNullOrEmpty(TextPaterno) || !string.IsNullOrEmpty(TextMaterno))
                        {
                            //PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.REGISTRO_CORRESPONDENCIA);
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                        }
                        break;
                }
            }

            if (!string.IsNullOrEmpty(TextNombre) || !string.IsNullOrEmpty(TextPaterno) || !string.IsNullOrEmpty(TextMaterno))
                BuscarPersonas();
        }

        private async void BuscarPersonas()
        {
            if (!pConsultar)
            {
                new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                return;
            }
            if (string.IsNullOrEmpty(TextNombre))
                TextNombre = string.Empty;
            if (string.IsNullOrEmpty(TextPaterno))
                TextPaterno = string.Empty;
            if (string.IsNullOrEmpty(TextMaterno))
                TextMaterno = string.Empty;
            var lista = new List<PERSONAVISITAAUXILIAR>();

            var personas = await StaticSourcesViewModel.CargarDatosAsync<IQueryable<SSP.Servidor.PERSONA>>(() =>
                new cPersona().ObtenerXNombreYNIP_Externos(TextNombre, TextPaterno, TextMaterno, 0));

            ListPersonas = new ObservableCollection<SSP.Servidor.PERSONA>(personas);
        }

        private async void BuscarImputado()
        {
            if (!pConsultar)
            {
                new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                return;
            }
            ListExpediente = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<IMPUTADO>>(() => (new cImputado()).ObtenerTodos(ApellidoPaternoBuscar, ApellidoMaternoBuscar, NombreBuscar, AnioBuscar, FolioBuscar));

            if (ListExpediente.Count <= 0)
                new Dialogos().ConfirmacionDialogo("Notificacion!", "No se encontro ningun imputado con esos datos.");
        }

        private void EnterExpediente(Object obj)
        {
            if (!pConsultar)
            {
                new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                return;
            }
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
                            FolioBuscar = string.IsNullOrEmpty(textbox.Text) ? new Nullable<int>() : int.Parse(textbox.Text);
                            break;
                        case "AnioBuscar":
                            AnioBuscar = string.IsNullOrEmpty(textbox.Text) ? new Nullable<int>() : int.Parse(textbox.Text);
                            break;

                        case "DesNombre":
                        case "DesMaterno":
                        case "DesPaterno":
                            NombreBuscar = DesNombre;
                            ApellidoPaternoBuscar = DesPaterno;
                            ApellidoMaternoBuscar = DesMaterno;
                            if (!string.IsNullOrEmpty(NombreBuscar) || !string.IsNullOrEmpty(ApellidoPaternoBuscar) || !string.IsNullOrEmpty(ApellidoMaternoBuscar))
                            {
                                //PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.REGISTRO_CORRESPONDENCIA);
                                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                            }
                            break;
                    }
                }
                

                if (!string.IsNullOrEmpty(NombreBuscar) || !string.IsNullOrEmpty(ApellidoPaternoBuscar) || !string.IsNullOrEmpty(ApellidoMaternoBuscar) || FolioBuscar.HasValue || AnioBuscar.HasValue)
                    BuscarImputado();
            }
        }

        private async void CargarDatos()
        {
            if (pConsultar)
                ListaRegistroEntrega = await StaticSourcesViewModel.CargarDatosAsync(() => new ObservableCollection<RegistroEntrega>(new cCorrespondencia().ObtenerTodos().Select(s => new RegistroEntrega
                {
                    Correspondencia = s,

                    Anio = s.ID_ANIO,
                    Entrega = (s.CONFIRMACION_RECIBIDO == "N" ? false : true),
                    Fecha_Deposito = (s.RECEPCION_FEC.HasValue ? s.RECEPCION_FEC.Value.ToShortDateString() : string.Empty),
                    Folio = s.ID_IMPUTADO,
                    Hora_Deposito = (s.RECEPCION_FEC.HasValue ? s.RECEPCION_FEC.Value.ToShortTimeString() : string.Empty),
                    Interno = s.INGRESO.IMPUTADO.PATERNO.Trim() + " " + s.INGRESO.IMPUTADO.MATERNO.Trim() + " " + s.INGRESO.IMPUTADO.NOMBRE.Trim(),
                    Observaciones = s.OBSERV,
                    Remitente = s.REMITENTE
                }).OrderByDescending(o => o.Correspondencia.RECEPCION_FEC).ToList()));
            else
                ListaRegistroEntrega = new ObservableCollection<RegistroEntrega>();
        }

        private async void RegistroCorrespondenciaLoad(RegistroCorrespondenciaView obj = null)
        {
            try
            {
                ConfiguraPermisos();
                CargarDatos();
                Validacion();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar decomisos", ex);
            }
        }
        #endregion

        #region Validaciones
        private void Validacion() {
            base.ClearRules();
            base.AddRule(() => Remitente, () => !string.IsNullOrEmpty(Remitente), "REMITENTE ES REQUERIDO!");
            base.AddRule(() => ObservacionRecepcion, () => !string.IsNullOrEmpty(ObservacionRecepcion), "OBSERVACIÓN ES REQUERIDA!");
            OnPropertyChanged("Remitente");
            OnPropertyChanged("ObservacionRecepcion");
        }
        #endregion

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CORRESPONDENCIA.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                foreach (var p in permisos)
                {
                    if (p.INSERTAR == 1)
                        pInsertar = true;
                    if (p.EDITAR == 1)
                        pEditar = true;
                    if (p.CONSULTAR == 1)
                        pConsultar = true;
                    if (p.IMPRIMIR == 1)
                        pImprimir = true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }
        #endregion
    }
}
