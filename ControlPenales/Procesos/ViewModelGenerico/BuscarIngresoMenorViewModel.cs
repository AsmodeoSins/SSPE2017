using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases;
using Oracle.ManagedDataAccess.Client;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ControlPenales
{
    public class BuscarIngresoMenorViewModel : FingerPrintScanner
    {
        #region [Constructores]
        public BuscarIngresoMenorViewModel(ObservableCollection<EMI_INGRESO_ANTERIOR> tmp)
        {
            if(tmp != null)
                LstTemporal = new ObservableCollection<EMI_INGRESO_ANTERIOR>(tmp);
            else
                LstTemporal = new ObservableCollection<EMI_INGRESO_ANTERIOR>();
            ImagenInterconexion = new Imagenes().getImagenPerson();
            BuscarHuellaVisible = false;
            TextoBoton = "Agregar";
        }
        #endregion
        #region [Comandos]
        public ICommand WindowLoading
        {
            get { return new DelegateCommand<BuscarIngresoMenorView>(OnLoad); }
        }

        public ICommand CommandAceptar
        {
            get { return new DelegateCommand<BuscarIngresoMenorView>(Aceptar); }
        }

        public ICommand CommandCancelar
        {
            get { return new DelegateCommand<BuscarIngresoMenorView>(Cancelar); }
        }

        private ICommand _onClick;
        public ICommand OnClick
        {
            get
            {
                return _onClick ?? (_onClick = new RelayCommand(clickSwitch));
            }
        }

        private ICommand buscarClick;
        public ICommand BuscarClick
        {
            get
            {
                return buscarClick ?? (buscarClick = new RelayCommand(ClickEnter));
            }
        }
        #endregion
        #region [Propiedades]
        private bool _IsSucceed = false;
        public bool IsSucceed
        {
            get { return _IsSucceed; }
        }

        private Brush _ColorMessage;
        public Brush ColorMessage
        {
            get { return _ColorMessage; }
            set
            {
                _ColorMessage = value;
                RaisePropertyChanged("ColorMessage");
            }
        }

        private bool isIngresoAntMenoresAll;
        public bool IsIngresoAntMenoresAll
        {
            get { return isIngresoAntMenoresAll; }
            set
            {
                isIngresoAntMenoresAll = value;
                OnPropertyChanged("IsIngresoAntMenoresAll");
                foreach (var ingreso in LstIAS)
                {
                    ingreso.Seleccione = value;
                }
                LstIAS = new ObservableCollection<IngresoAinterior>(LstIAS);
            }
        }

        private ImageSource _ImputadoFoto;
        public ImageSource ImputadoFoto
        {
            get { return _ImputadoFoto; }
            set
            {
                _ImputadoFoto = value;
                OnPropertyChanged("ImputadoFoto");
            }
        }

        private IMPUTADO _Imputado;
        public IMPUTADO Imputado
        {
            get { return _Imputado; }
            set
            {
                _Imputado = value;
                OnPropertyChanged();
            }
        }

        private enumTipoBiometrico? _DD_Dedo = enumTipoBiometrico.INDICE_DERECHO;
        public enumTipoBiometrico? DD_Dedo
        {
            get { return _DD_Dedo; }
            set
            {
                LimpiarCampos();
                _DD_Dedo = value;
                OnPropertyChanged("DD_Dedo");
            }
        }
        private bool aceptarBusquedaHuellaFocus;
        public bool AceptarBusquedaHuellaFocus
        {
            get { return aceptarBusquedaHuellaFocus; }
            set
            {
                aceptarBusquedaHuellaFocus = value;
                OnPropertyChanged("AceptarBusquedaHuellaFocus");
            }
        }

        private string _Expediente;
        public string Expediente
        {
            get { return _Expediente; }
            set
            {
                _Expediente = value;
                OnPropertyChanged("Expediente");
            }
        }

        public string NIP
        {
            get { return Imputado != null ? Imputado.NIP.ToString() : string.Empty; }
            set { OnPropertyChanged("NIP"); }
        }

        private string _APaterno;
        public string APaterno
        {
            get { return _APaterno; }
            set
            {
                _APaterno = value;
                OnPropertyChanged("APaterno");
            }
        }
        private string _AMaterno;
        public string AMaterno
        {
            get { return _AMaterno; }
            set
            {
                _AMaterno = value;
                OnPropertyChanged("AMaterno");
            }
        }
        private string _Nombre;
        public string Nombre
        {
            get { return _Nombre; }
            set
            {
                _Nombre = value;
                OnPropertyChanged("Nombre");
            }
        }

        private string textoBoton;
        public string TextoBoton
        {
            get { return textoBoton; }
            set { textoBoton = value; OnPropertyChanged("TextoBoton"); }
        }
        //INTERCONEXION
        private string nuc;
        public string Nuc
        {
            get { return nuc; }
            set { nuc = value; OnPropertyChanged("Nuc"); }
        }

        private VM_IMPUTADOSDATOS selectedInterconexion;
        public VM_IMPUTADOSDATOS SelectedInterconexion
        {
            get { return selectedInterconexion; }
            set
            {
                selectedInterconexion = value;
                if (selectedInterconexion != null)
                {
                    if (selectedInterconexion.FOTO != null)
                    {
                        ImagenInterconexion = selectedInterconexion.FOTO;
                    }
                    else
                        ImagenInterconexion = new Imagenes().getImagenPerson();
                    BuscarHuellaVisible = true;
                }
                else
                    ImagenInterconexion = new Imagenes().getImagenPerson();
                OnPropertyChanged("SelectedInterconexion");
            }
        }

        private byte[] imagenInterconexion;
        public byte[] ImagenInterconexion
        {
            get { return imagenInterconexion; }
            set
            {
                imagenInterconexion = value;
                OnPropertyChanged("ImagenInterconexion");
            }
        }

        private ObservableCollection<VM_IMPUTADOSDATOS> lstInterconexion;
        public ObservableCollection<VM_IMPUTADOSDATOS> LstInterconexion
        {
            get { return lstInterconexion; }
            set { lstInterconexion = value; OnPropertyChanged("LstInterconexion"); }
        }

        private bool buscarHuellaVisible;
        public bool BuscarHuellaVisible
        {
            get { return buscarHuellaVisible; }
            set { buscarHuellaVisible = value; OnPropertyChanged("BuscarHuellaVisible"); }
        }
        #endregion
        #region [Metodos]
        private void OnLoad(BuscarIngresoMenorView Window)
        {
            Window.Closed += (s, e) =>
            {
                try
                {
                    if (OnProgress == null)
                        return;

                    if (!_IsSucceed)
                        Imputado = null;

                    OnProgress.Abort();
                    CancelCaptureAndCloseReader(OnCaptured);
                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar ingreso menor", ex);
                }
            };

            if (CurrentReader != null)
            {
                CurrentReader.Dispose();
                CurrentReader = null;
            }

            CurrentReader = Readers[0];

            if (CurrentReader == null)
                return;

            if (!OpenReader())
                Window.Close();

            if (!StartCaptureAsync(OnCaptured))
                Window.Close();

            OnProgress = new Thread(() => InvokeDelegate(Window));

            Application.Current.Dispatcher.Invoke((System.Action)(delegate
               {
                   ScannerMessage = "Capture Huella";
                   ColorMessage = new SolidColorBrush(Colors.Green);
               }));
        }

        public override void OnCaptured(DPUruNet.CaptureResult captureResult)
        {
            if (ScannerMessage == "Procesando... ")
                return;
            Application.Current.Dispatcher.Invoke((System.Action)(delegate
               {
                   ScannerMessage = "Procesando...";
                   ColorMessage = new SolidColorBrush(Color.FromRgb(51, 115, 242));
               }));
            base.OnCaptured(captureResult);
            Compare();
        }

        private void Compare(object Huella = null)
        {
            var aux = new ObservableCollection<IngresoAinterior>();
            if (FingerPrintData == null)
            {
                Application.Current.Dispatcher.Invoke((System.Action)(delegate
               {
                   ScannerMessage = "Vuelve a capturar la huella";
                   
                   ColorMessage = new SolidColorBrush(Colors.DarkOrange);
               }));
                return;
            }
            Application.Current.Dispatcher.Invoke((System.Action)(delegate
               {
                   ScannerMessage = "Procesando... ";
                   ColorMessage = new SolidColorBrush(Color.FromRgb(51, 115, 242));
                   AceptarBusquedaHuellaFocus = false;
               }));

            //var Service = new MenoresHuellasService.dpersonSoapClient();
            //var str = FeatureExtraction.CreateFmdFromFid(Importer.ImportFid(FingerPrintData.Bytes, Constants.Formats.Fid.ANSI).Data, Constants.Formats.Fmd.ANSI).Data.Bytes;
            //var CompareResult = "<ANO>2007</ANO><FOLIO>5438</FOLIO><BIOMETRICO>1</BIOMETRICO>";//Service.Compara(str, "INTERNO", "4");
            //var serializer = new XmlSerializer(typeof(RespuestaWS));
            //RespuestaWS result;
            //using (TextReader reader = new StringReader(CompareResult))
            //{
            //    result = (RespuestaWS)serializer.Deserialize(reader);
            //}
            //var tmp = new ObservableCollection<IngresoAinterior>();
            //LstIAMS = new ObservableCollection<IngresoAinterior>();
            int? anios, meses, dias;
            var Imputado = new cImputado().Obtener(5438, 2007, 4);
            if (Imputado != null)
            {
                var imp = Imputado.FirstOrDefault();
                Expediente = string.Format("{0}/{1}", imp.ID_ANIO, imp.ID_IMPUTADO);
                APaterno = imp.PATERNO;
                AMaterno = imp.MATERNO;
                Nombre = imp.NOMBRE;
                var lstIngresos = imp.INGRESO.Where(x => x.ID_ESTATUS_ADMINISTRATIVO == Parametro.ID_ESTATUS_ADMVO_LIBERADO);
                if (lstIngresos != null)
                {
                    foreach (var ing in lstIngresos)
                    {
                        anios = meses = dias = 0;
                        if (LstTemporal.Where(w => w.ID_ANIO == ing.ID_ANIO && w.ID_CENTRO == ing.ID_CENTRO && w.ID_IMPUTADO == ing.ID_IMPUTADO && w.ID_INGRESO == ing.ID_INGRESO).Count() == 0)
                        {
                            var obj = new IngresoAinterior();
                            obj.Seleccione = false;
                            obj.IdCentro = ing.ID_CENTRO;
                            obj.IdAnio = ing.ID_ANIO;
                            obj.IdImputado = ing.ID_IMPUTADO;
                            obj.IdIngreso = ing.ID_INGRESO;
                            ////EMISOR
                            obj.Emisor = ing.CENTRO.EMISOR;

                            obj.Delito = new cDelito().Obtener("C", 5).FirstOrDefault();
                            anios = 1;
                            //DELITO
                            //if (ing.CAUSA_PENAL != null)
                            //{
                            //    var cp = ing.CAUSA_PENAL.Where(w => w.ID_ESTATUS_CP == 4).FirstOrDefault();//estatus concluido
                            //    if (cp != null)
                            //    {
                            //        if (cp.CAUSA_PENAL_DELITO != null)
                            //        {
                            //            if (cp.CAUSA_PENAL_DELITO.Count > 0)
                            //            {
                            //                var del = cp.CAUSA_PENAL_DELITO.FirstOrDefault();
                            //                if (del != null)
                            //                    obj.Delito = LstDelitosCP.Where(w => w.ID_FUERO == del.ID_FUERO && w.ID_DELITO == del.ID_DELITO).FirstOrDefault();
                            //            }
                            //        }
                            //    }

                            //    //OBTENEMOS LA SENTENCIA
                            //    foreach (var x in ing.CAUSA_PENAL)
                            //    {
                            //        foreach (var y in x.SENTENCIAs)
                            //        {
                            //            anios = anios + y.ANIOS;
                            //            meses = meses + y.MESES;
                            //            dias = dias + y.DIAS;
                            //        }
                            //    }
                            //}
                            while (dias > 30)
                            {
                                meses++;
                                dias = dias - 30;
                            }
                            while (meses > 12)
                            {
                                anios++;
                                meses = meses - 12;
                            }
                            //CARGAMOS LOS INRSOS ANTERIORES
                            var periodo = string.Empty;
                            if (anios > 0)
                                periodo = string.Format("{0} AÑOS ", anios);
                            if (meses > 0)
                                periodo = string.Format("{0}{1} MESES ", periodo, meses);
                            if (dias > 0)
                                periodo = string.Format("{0}{1} DIAS ", periodo, dias);
                            obj.PerioroReclusion = periodo;
                            aux.Add(obj); 
                        }
                    }
                }

                Application.Current.Dispatcher.Invoke((System.Action)(delegate
                {
                    ScannerMessage = "Registro encontrado";
                    AceptarBusquedaHuellaFocus = true;
                    ColorMessage = new SolidColorBrush(Colors.Green);
                }));

                Nombre = imp.NOMBRE;
                APaterno = imp.PATERNO;
                AMaterno = imp.MATERNO;
                Expediente = string.Format("{0}/{1}", imp.ID_ANIO, imp.ID_IMPUTADO);
               
            }

            LstIAS = new ObservableCollection<IngresoAinterior>(aux);
            if (LstIAS != null)
            {
                if (LstIAS.Count > 0)
                    EmptyIAS = false;
                else
                    EmptyIAS = true;
            }
            else
                EmptyIAS = true;
        }

        private void Aceptar(BuscarIngresoMenorView Window)
        {
            if (ScannerMessage == "Procesando... ")
                return;
            //if (SelectedInterconexion != null)
            //{
                AgregarIngresosAnterioresSistema();
                _IsSucceed = true;
                Window.Close();
            //}

        }

        private void Cancelar(BuscarIngresoMenorView Window)
        {
            LimpiarBusqueda();
            Window.Close();
        }

        private void LimpiarCampos()
        {
            Application.Current.Dispatcher.Invoke((System.Action)(delegate
               {
                   ScannerMessage = "Capture Huella";
                   ColorMessage = new SolidColorBrush(Colors.Green);
                   AceptarBusquedaHuellaFocus = true;
               }));
            _Imputado = null;
            Nombre = string.Empty;
            APaterno = string.Empty;
            AMaterno = string.Empty;
            Expediente = string.Empty;
            NIP = string.Empty;
            PropertyImage = null;
            ImputadoFoto = null;
        }

        private void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "buscar_nuc":
                    BuscarNUCInterconexion();
                    break;
                case "buscar_nuevo":
                    LimpiarBusqueda();
                    break;
                case "rollbackBuscarIAs":
                    LimpiarBusqueda();
                    
                    break;
            }
        }

        private void ClickEnter(Object obj)
        {
            var tb = (TextBox)obj;
            if (tb != null)
            {
                Nuc = tb.Text;
                BuscarNUCInterconexion();
            }
        }

        /// <summary>
        /// Buscamos un NUC en las vistas materializadas de la Procuraduria
        /// </summary>
        private void BuscarNUCInterconexion()
        {
            if (!string.IsNullOrEmpty(Nuc))
            {
                LstInterconexion = new ObservableCollection<VM_IMPUTADOSDATOS>();
                using (EntidadInterconexion contexto = new EntidadInterconexion())
                {
                    LstInterconexion = new ObservableCollection<VM_IMPUTADOSDATOS>(contexto.Database.SqlQuery<VM_IMPUTADOSDATOS>("SELECT * FROM SSP.VM_IMPUTADOSDATOS WHERE EXPEDIENTEID LIKE '%'||:param1 ", new OracleParameter("param1", Nuc)));
                }
            }
        }

        private void LimpiarBusqueda()
        {
            Nuc = null;
            LstInterconexion = new ObservableCollection<VM_IMPUTADOSDATOS>();
            ImagenInterconexion = new Imagenes().getImagenPerson();
            LimpiarCampos();
            BuscarHuellaVisible = false;
            SelectedInterconexion = null;
            Imputado = null;
        }

        #endregion

        private ObservableCollection<INGRESO> ingresosMenor;
        public ObservableCollection<INGRESO> IngresosMenor
        {
            get { return ingresosMenor; }
            set { ingresosMenor = value; }
        }

        private ObservableCollection<IngresoAinterior> lstIAS;
        public ObservableCollection<IngresoAinterior> LstIAS
        {
            get { return lstIAS; }
            set { lstIAS = value; OnPropertyChanged("LstIAS"); }
        }

        private ObservableCollection<EMI_INGRESO_ANTERIOR> lstIngAntMen;
        public ObservableCollection<EMI_INGRESO_ANTERIOR> LstIngAntMen
        {
            get { return lstIngAntMen; }
            set { lstIngAntMen = value; OnPropertyChanged("LstIngAntMen"); }
        }

        private ObservableCollection<EMI_INGRESO_ANTERIOR> lstTemporal;
        public ObservableCollection<EMI_INGRESO_ANTERIOR> LstTemporal
        {
            get { return lstTemporal; }
            set { lstTemporal = value; OnPropertyChanged("LstTemporal"); }
        }

        private bool emptyIAS;
        public bool EmptyIAS
        {
            get { return emptyIAS; }
            set { emptyIAS = value; OnPropertyChanged("EmptyIAS"); }
        }


        private void AgregarIngresosAnterioresSistema()
        {
                if (LstIAS != null)
                {
                    foreach (var ias in LstIAS)
                    {
                        if (ias.Seleccione != null)
                            if (ias.Seleccione.Value)
                            {
                                if (LstIngAntMen == null)
                                    LstIngAntMen = new ObservableCollection<EMI_INGRESO_ANTERIOR>();
                                ///TODO: cambios delito
                                LstIngAntMen.Add(new EMI_INGRESO_ANTERIOR()
                                {
                                    ID_TIPO = 1,
                                    ID_EMISOR = ias.Emisor.ID_EMISOR,
                                    PERIODO_RECLUSION = ias.PerioroReclusion,
                                    SANCIONES = ias.Sanciones,
                                    ID_DELITO = ias.Delito.ID_DELITO,
                                    ID_FUERO = ias.Delito.ID_FUERO,
                                    EMISOR = ias.Emisor,
                                    //DELITO = ias.Delito,
                                    ID_CENTRO = ias.IdCentro,
                                    ID_ANIO = ias.IdAnio,
                                    ID_IMPUTADO = ias.IdImputado,
                                    ID_INGRESO = ias.IdIngreso

                                });
                            }
                    }
                }
        }
    }
}
