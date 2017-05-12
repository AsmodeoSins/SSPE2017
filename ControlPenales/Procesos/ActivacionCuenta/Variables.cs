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

namespace ControlPenales
{
    partial class ActivacionCuentaViewModel : ValidationViewModelBase
    {
        #region Usuario
        private ObservableCollection<USUARIO> lstUsuario;
        public ObservableCollection<USUARIO> LstUsuario
        {
            get { return lstUsuario; }
            set { lstUsuario = value; OnPropertyChanged("LstUsuario"); }
        }

        private USUARIO selectedUsuario;
        public USUARIO SelectedUsuario
        {
            get { return selectedUsuario; }
            set { selectedUsuario = value;
                OnPropertyChanged("SelectedUsuario"); }
        }

        private string bUsuario;
        public string BUsuario
        {
            get { return bUsuario; }
            set { bUsuario = value; OnPropertyChanged("BUsuario"); }
        }

        private Visibility usuariosVisible = Visibility.Visible;
        public Visibility UsuariosVisible
        {
            get { return usuariosVisible; }
            set { usuariosVisible = value; OnPropertyChanged("UsuariosVisible"); }
        }

        //pop
        private int? uNoEmpleado;
        public int? UNoEmpleado
        {
            get { return uNoEmpleado; }
            set { uNoEmpleado = value; OnPropertyChanged("UNoEmpleado"); }
        }

        private string uPaterno;
        public string UPaterno
        {
            get { return uPaterno; }
            set { uPaterno = value; OnPropertyChanged("UPaterno"); }
        }

        private string uMaterno;
        public string UMaterno
        {
            get { return uMaterno; }
            set { uMaterno = value; OnPropertyChanged("UMaterno"); }
        }

        private string uNombre;
        public string UNombre
        {
            get { return uNombre; }
            set { uNombre = value; OnPropertyChanged("UNombre"); }
        }

        private string uLogin;
        public string ULogin
        {
            get { return uLogin; }
            set { uLogin = value; OnPropertyChanged("ULogin"); }
        }

        private string uPassword;
        public string UPassword
        {
            get { return uPassword; }
            set { uPassword = value; OnPropertyChanged("UPassword"); }
        }

        private string uPasswordR;
        public string UPasswordR
        {
            get { return uPasswordR; }
            set { uPasswordR = value; OnPropertyChanged("UPasswordR"); }
        }

        private byte[] uImagen = new Imagenes().getImagenPerson();
        public byte[] UImagen
        {
            get { return uImagen; }
            set { uImagen = value; OnPropertyChanged("UImagen"); }
        }

        private bool uEstatus = true;
        public bool UEstatus
        {
            get { return uEstatus; }
            set { uEstatus = value; OnPropertyChanged("UEstatus"); }
        }

        private string tituloPop = "Agregar Usuario";
        public string TituloPop
        {
            get { return tituloPop; }
            set { tituloPop = value; OnPropertyChanged("TituloPop"); }
        }

        private bool uLoginEnabled = true;
        public bool ULoginEnabled
        {
            get { return uLoginEnabled; }
            set { uLoginEnabled = value; OnPropertyChanged("ULoginEnabled"); }
        }

        private string PassBD = Parametro.PASSWORD_USUARIO_BD;
        #endregion

        #region Menu
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

        #region Busqueda Empleado
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

        private string textNombre;
        public string TextNombre
        {
            get { return textNombre; }
            set { textNombre = value; OnPropertyChanged("TextNombre"); }
        }
        
        private ObservableCollection<SSP.Servidor.PERSONA> listPersonas;
        public ObservableCollection<SSP.Servidor.PERSONA> ListPersonas
        {
          get { return listPersonas; }
            set { listPersonas = value; OnPropertyChanged("ListPersonas"); }
        }
        
        private SSP.Servidor.PERSONA selectPersona;
        public SSP.Servidor.PERSONA SelectPersona
        {
            get { return selectPersona; }
            set { selectPersona = value;
            if (value != null)
            {
                if (value.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                    ImagenPersona = value.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                else
                    ImagenPersona = new Imagenes().getImagenPerson();
            }
            else
                ImagenPersona = new Imagenes().getImagenPerson();
                
                OnPropertyChanged("SelectPersona"); }
        }

        private bool emptyBuscarRelacionInternoVisible = true;
        public bool EmptyBuscarRelacionInternoVisible
        {
            get { return emptyBuscarRelacionInternoVisible; }
            set { emptyBuscarRelacionInternoVisible = value; OnPropertyChanged("EmptyBuscarRelacionInternoVisible"); }
        }

        private byte[] imagenPersona = new Imagenes().getImagenPerson();
        public byte[] ImagenPersona
        {
            get { return imagenPersona; }
            set { imagenPersona = value; OnPropertyChanged("ImagenPersona"); }
        }
        #endregion

        #region Panatalla
        private PasswordBox uPassControl;
        public PasswordBox UPassControl
        {
            get { return uPassControl; }
            set { uPassControl = value; }
        }

        private PasswordBox uPassRepeatControl;
        public PasswordBox UPassRepeatControl
        {
            get { return uPassRepeatControl; }
            set { uPassRepeatControl = value; }
        }
        #endregion

        #region Privilegios
        private bool pInsertar = false;
        private bool pEditar = false;
        private bool pConsultar = false;
        private bool pImprimir = false;
        #endregion
    }
}
