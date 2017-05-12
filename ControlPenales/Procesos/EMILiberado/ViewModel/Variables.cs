using SSP.Servidor;
using SSP.Modelo;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using ControlPenales.BiometricoServiceReference;
using System.Windows;
using SSP.Controlador.Catalogo.Justicia;

namespace ControlPenales
{
    partial class EMILiberadoViewModel
    {
        //#region Persona
        //private SSP.Servidor.PERSONA selectedPersona;
        //public SSP.Servidor.PERSONA SelectedPersona
        //{
        //    get { return selectedPersona; }
        //    set { selectedPersona = value; OnPropertyChanged("SelectedPersona"); }
        //}
        //#endregion

        #region Buscar
        private int? nip;
        public int? Nip
        {
            get { return nip; }
            set { nip = value; OnPropertyChanged("Nip"); }
        }

        private string paternoE;
        public string PaternoE
        {
            get { return paternoE; }
            set { paternoE = value; OnPropertyChanged("PaternoE"); }
        }

        private string maternoE;
        public string MaternoE
        {
            get { return maternoE; }
            set { maternoE = value; OnPropertyChanged("MaternoE"); }
        }

        private string nombreE;
        public string NombreE
        {
            get { return nombreE; }
            set { nombreE = value; OnPropertyChanged("NombreE"); }
        }

        private byte[] imagenEmpleadoPop = new Imagenes().getImagenPerson();
        public byte[] ImagenEmpleadoPop
        {
            get { return imagenEmpleadoPop; }
            set { imagenEmpleadoPop = value; OnPropertyChanged("ImagenEmpleadoPop"); }
        }

        private ObservableCollection<SSP.Servidor.PERSONA> lstEmpleadoPop;
        public ObservableCollection<SSP.Servidor.PERSONA> LstEmpleadoPop
        {
            get { return lstEmpleadoPop; }
            set { lstEmpleadoPop = value; OnPropertyChanged("LstEmpleadoPop"); }
        }

        private SSP.Servidor.PERSONA selectedEmpleadoPop;
        public SSP.Servidor.PERSONA SelectedEmpleadoPop
        {
            get { return selectedEmpleadoPop; }
            set
            {
                selectedEmpleadoPop = value;
                if (value != null)
                {
                    if (value.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                        ImagenEmpleadoPop = value.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                    else
                        ImagenEmpleadoPop = new Imagenes().getImagenPerson();
                }
                else
                    ImagenEmpleadoPop = new Imagenes().getImagenPerson();
                OnPropertyChanged("SelectedEmpleadoPop");
            }
        }

        private Visibility empleadoEmpty = Visibility.Collapsed;
        public Visibility EmpleadoEmpty
        {
            get { return empleadoEmpty; }
            set { empleadoEmpty = value; OnPropertyChanged("EmpleadoEmpty"); }
        }

        private bool nuevaMJEnabled = false;
        public bool NuevaMJEnabled
        {
            get { return nuevaMJEnabled; }
            set { nuevaMJEnabled = value; OnPropertyChanged("NuevaMJEnabled"); }
        }

        private bool selectMJEnabled = false;
        public bool SelectMJEnabled
        {
            get { return selectMJEnabled; }
            set { selectMJEnabled = value; OnPropertyChanged("SelectMJEnabled"); }
        }
        #endregion

        #region Permisos
        private bool pInsertar = false;
        public bool PInsertar
        {
            get { return pInsertar; }
            set { pInsertar = value; OnPropertyChanged("PInsertar"); }
        }

        private bool pEditar = false;
        public bool PEditar
        {
            get { return pEditar; }
            set { pEditar = value; OnPropertyChanged("PEditar"); }
        }

        private bool pConsultar = false;
        public bool PConsultar
        {
            get { return pConsultar; }
            set { pConsultar = value; OnPropertyChanged("PConsultar"); }
        }

        private bool pImprimir = false;
        public bool PImprimir
        {
            get { return pImprimir; }
            set { pImprimir = value; OnPropertyChanged("PImprimir"); }
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

        private bool menuReporteEnabled = false;
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
        #endregion

        #region Temporal
        private IMPUTADO tImputado;
        private LIBERADO_MEDIDA_JUDICIAL tMedidaJudicial;
        #endregion

        #region Liberados
        private string _NUCBuscar;
        public string NUCBuscar
        {
            get { return _NUCBuscar; }
            set { _NUCBuscar = value; OnPropertyChanged("NUCBuscar"); }
        }

        private RangeEnabledObservableCollection<cLiberados> lstLiberados;
        public RangeEnabledObservableCollection<cLiberados> LstLiberados
        {
            get { return lstLiberados; }
            set { lstLiberados = value; OnPropertyChanged("LstLiberados"); }
        }

        private cLiberados selectedLiberado;
        public cLiberados SelectedLiberado
        {
            get { return selectedLiberado; }
            set
            {
                selectedLiberado = value;
                if (value != null)
                {
                    SelectExpediente = new cImputado().Obtener(value.ID_IMPUTADO, value.ID_ANIO, value.ID_CENTRO).FirstOrDefault();
                }
                else
                {
                    SelectExpediente = null;
                }
                OnPropertyChanged("SelectedLiberado");
            }
        }
        #endregion

        #region Filtros
        private bool porNUC = true;
        public bool PorNUC
        {
            get { return porNUC; }
            set
            {
                porNUC = value;
                if (value)
                {
                    PorCP = false;
                    PorNUCVisible = Visibility.Visible;
                    PorCPVisible = Visibility.Collapsed;
                    AnioBuscar = null;
                    FolioBuscar = null;
                }
                else
                {
                    PorCP = true;
                    PorNUCVisible = Visibility.Collapsed;
                    PorCPVisible = Visibility.Visible;
                    NUCBuscar = string.Empty;
                }
                OnPropertyChanged("PorNUC");
            }
        }

        private bool porCP = false;
        public bool PorCP
        {
            get { return porCP; }
            set { porCP = value; OnPropertyChanged("PorCP"); }
        }

        private Visibility porNUCVisible = Visibility.Visible;
        public Visibility PorNUCVisible
        {
            get { return porNUCVisible; }
            set { porNUCVisible = value; OnPropertyChanged("PorNUCVisible"); }
        }

        private Visibility porCPVisible = Visibility.Collapsed;
        public Visibility PorCPVisible
        {
            get { return porCPVisible; }
            set { porCPVisible = value; OnPropertyChanged("PorCPVisible"); }
        }
        #endregion
    }
}