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
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using ControlPenales.Clases;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Windows.Interop;
using System.IO;
using System.Windows.Controls;
using ControlPenales.BiometricoServiceReference;
using System.Windows.Media;
using Microsoft.Reporting.WinForms;


namespace ControlPenales
{
    partial class AtencionCitaViewModel : ValidationViewModelBase
    {
        #region Atencion Cita
        private ATENCION_CITA selectedAtencionCita;
        public ATENCION_CITA SelectedAtencionCita
        {
            get { return selectedAtencionCita; }
            set { selectedAtencionCita = value; OnPropertyChanged("SelectedAtencionCita"); }
        }

        private ATENCION_RECIBIDA SelectedAtencionRecibida;

        private string atencionTxt;
        public string AtencionTxt
        {
            get { return atencionTxt; }
            set { atencionTxt = value; OnPropertyChanged("AtencionTxt"); }
        }

        private ReportViewer reporte;
        public ReportViewer Reporte
        {
            get { return reporte; }
            set { reporte = value; OnPropertyChanged("Reporte"); }
        }
        #endregion

        #region Historico
        private ObservableCollection<ATENCION_RECIBIDA> lstAtencionRecibidaHistorico;
        public ObservableCollection<ATENCION_RECIBIDA> LstAtencionRecibidaHistorico
        {
            get { return lstAtencionRecibidaHistorico; }
            set { lstAtencionRecibidaHistorico = value; OnPropertyChanged("LstAtencionRecibidaHistorico"); }
        }

        private ATENCION_RECIBIDA selectedAtencionRecibidaHistorico;
        public ATENCION_RECIBIDA SelectedAtencionRecibidaHistorico
        {
            get { return selectedAtencionRecibidaHistorico; }
            set { selectedAtencionRecibidaHistorico = value; OnPropertyChanged("SelectedAtencionRecibidaHistorico"); }
        }
     
        #endregion

        #region Imputados
        private IMPUTADO Imputado;
        #endregion

        #region Datos Generales
        private short? anio;
        public short? Anio
        {
            get { return anio; }
            set { anio = value; OnPropertyChanged("Anio"); }
        }

        private int? folio;
        public int? Folio
        {
            get { return folio; }
            set { folio = value; OnPropertyChanged("Folio"); }
        }

        private string paterno;
        public string Paterno
        {
            get { return paterno; }
            set { paterno = value; OnPropertyChanged("Paterno"); }
        }

        private string materno;
        public string Materno
        {
            get { return materno; }
            set { materno = value; OnPropertyChanged("Materno"); }
        }

        private string nombre;
        public string Nombre
        {
            get { return nombre; }
            set { nombre = value; OnPropertyChanged("Nombre"); }
        }

        private byte[] imagenIngreso = new Imagenes().getImagenPerson();
        public byte[] ImagenIngreso
        {
            get { return imagenIngreso; }
            set { imagenIngreso = value; OnPropertyChanged("ImagenIngreso"); }
        }
        #endregion

        #region [Huellas]
        IList<PlantillaBiometrico> HuellasCapturadas;

        private enumTipoBiometrico? _DD_Dedo;
        public enumTipoBiometrico? DD_Dedo
        {
            get { return _DD_Dedo; }
            set { _DD_Dedo = value; }
        }

        private Visibility _ShowPopUp = Visibility.Hidden;
        public Visibility ShowPopUp
        {
            get { return _ShowPopUp; }
            set
            {
                _ShowPopUp = value;
                OnPropertyChanged("ShowPopUp");
            }
        }

        private Visibility _ShowFingerPrint = Visibility.Hidden;
        public Visibility ShowFingerPrint
        {
            get { return _ShowFingerPrint; }
            set
            {
                _ShowFingerPrint = value;
                OnPropertyChanged("ShowFingerPrint");
            }
        }

        private Visibility _ShowLine = Visibility.Visible;
        public Visibility ShowLine
        {
            get { return _ShowLine; }
            set
            {
                _ShowLine = value;
                OnPropertyChanged("ShowLine");
            }
        }

        private Visibility _ShowOk = Visibility.Hidden;
        public Visibility ShowOk
        {
            get { return _ShowOk; }
            set
            {
                _ShowOk = value;
                OnPropertyChanged("ShowOk");
            }
        }

        private ImageSource _GuardaHuella;
        public ImageSource GuardaHuella
        {
            get { return _GuardaHuella; }
            set
            {
                _GuardaHuella = value;
                OnPropertyChanged("GuardaHuella");
            }
        }

        private Brush _PulgarDerecho;
        public Brush PulgarDerecho
        {
            get { return _PulgarDerecho; }
            set
            {
                _PulgarDerecho = value;
                RaisePropertyChanged("PulgarDerecho");
            }
        }

        private ImageSource _PulgarDerechoBMP;
        public ImageSource PulgarDerechoBMP
        {
            get { return _PulgarDerechoBMP; }
            set
            {
                _PulgarDerechoBMP = value;
                RaisePropertyChanged("PulgarDerechoBMP");
            }
        }

        private Brush _IndiceDerecho;
        public Brush IndiceDerecho
        {
            get { return _IndiceDerecho; }
            set
            {
                _IndiceDerecho = value;
                OnPropertyChanged("IndiceDerecho");
            }
        }

        private Brush _MedioDerecho;
        public Brush MedioDerecho
        {
            get { return _MedioDerecho; }
            set
            {
                _MedioDerecho = value;
                OnPropertyChanged("MedioDerecho");
            }
        }

        private Brush _AnularDerecho;
        public Brush AnularDerecho
        {
            get { return _AnularDerecho; }
            set
            {
                _AnularDerecho = value;
                OnPropertyChanged("AnularDerecho");
            }
        }

        private Brush _MeñiqueDerecho;
        public Brush MeñiqueDerecho
        {
            get { return _MeñiqueDerecho; }
            set
            {
                _MeñiqueDerecho = value;
                OnPropertyChanged("MeñiqueDerecho");
            }
        }

        private Brush _PulgarIzquierdo;
        public Brush PulgarIzquierdo
        {
            get { return _PulgarIzquierdo; }
            set
            {
                _PulgarIzquierdo = value;
                OnPropertyChanged("PulgarIzquierdo");
            }
        }

        private Brush _IndiceIzquierdo;
        public Brush IndiceIzquierdo
        {
            get { return _IndiceIzquierdo; }
            set
            {
                _IndiceIzquierdo = value;
                OnPropertyChanged("IndiceIzquierdo");
            }
        }

        private Brush _MedioIzquierdo;
        public Brush MedioIzquierdo
        {
            get { return _MedioIzquierdo; }
            set
            {
                _MedioIzquierdo = value;
                OnPropertyChanged("MedioIzquierdo");
            }
        }

        private Brush _AnularIzquierdo;
        public Brush AnularIzquierdo
        {
            get { return _AnularIzquierdo; }
            set
            {
                _AnularIzquierdo = value;
                OnPropertyChanged("AnularIzquierdo");
            }
        }

        private Brush _MeñiqueIzquierdo;
        public Brush MeñiqueIzquierdo
        {
            get { return _MeñiqueIzquierdo; }
            set
            {
                _MeñiqueIzquierdo = value;
                OnPropertyChanged("MeñiqueIzquierdo");
            }
        }
        #endregion

        #region Pantalla
        private bool tabControlEnabled = false;
        public bool TabControlEnabled
        {
            get { return tabControlEnabled; }
            set { tabControlEnabled = value; OnPropertyChanged("TabControlEnabled"); }
        }

        private TXTextControl.WPF.TextControl editor;
        public TXTextControl.WPF.TextControl Editor
        {
            get { return editor; }
            set { editor = value; OnPropertyChanged("Editor"); }
        }

        private Visibility historicoVisible = Visibility.Collapsed;
        public Visibility HistoricoVisible
        {
            get { return historicoVisible; }
            set { historicoVisible = value; OnPropertyChanged("HistoricoVisible"); }
        }

        private bool menuGuardarEnabled = true;
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

        private bool menuLimpiarEnabled = false;
        public bool MenuLimpiarEnabled
        {
            get { return menuLimpiarEnabled; }
            set { menuLimpiarEnabled = value; OnPropertyChanged("MenuLimpiarEnabled"); }
        }
        
        private bool bHuellasEnabled = true;
        public bool BHuellasEnabled
        {
            get { return bHuellasEnabled; }
            set { bHuellasEnabled = value; OnPropertyChanged("BHuellasEnabled"); }
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

        

        #region Reporte
        private byte[] logo1 = Parametro.REPORTE_LOGO1;
        private byte[] logo2 = Parametro.REPORTE_LOGO2;
        private string encabezado1 = Parametro.ENCABEZADO1;
        private string encabezado2 = Parametro.ENCABEZADO2;
        #endregion
    }
}
