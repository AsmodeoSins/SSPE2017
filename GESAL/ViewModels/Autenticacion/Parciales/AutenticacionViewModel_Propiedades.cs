using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Biometrico.DigitalPersona;
using System.Configuration;
using System.Security;
namespace GESAL.ViewModels
{
    public partial class AutenticacionViewModel:FingerPrintScanner
    {
        #region variables
        private string error;

        public string Error
        {
            get { return error; }
            set { error = value; OnPropertyChanged("Error"); }
        }

        private bool bandError;

        public bool BandError
        {
            get { return bandError; }
            set { bandError = value; OnPropertyChanged("BandError"); }
        }

        //obtener dedo por default. Pero dar la posibilidad de cambiar el dedo en la verificacion.
        private BiometricoServiceReference.enumTipoBiometrico? _DD_Dedo = (BiometricoServiceReference.enumTipoBiometrico)Convert.ToInt16(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).AppSettings.Settings["DedoLoggin"].Value);
        public BiometricoServiceReference.enumTipoBiometrico? DD_Dedo
        {
            get { return _DD_Dedo; }
            set
            {
                _DD_Dedo = value;
                OnPropertyChanged("DD_Dedo");
            }
        }

        private bool capturaFalladaDigitalPersona = false;
        public bool CapturaFalladaDigitalPersona
        {
            get { return capturaFalladaDigitalPersona; }
            set { capturaFalladaDigitalPersona = value; RaisePropertyChanged("CapturaFalladaDigitalPersona"); }
        }

        private bool cmdOkHabilitado = false;
        public bool CmdOkHabilitado
        {
            get { return cmdOkHabilitado; }
            set { cmdOkHabilitado = value; RaisePropertyChanged("CmdOkHabilitado"); }
        }

        private SecureString password=null;
        public SecureString Password
        {
            get { return password; }
            set { password = value; RaisePropertyChanged("Password"); }
        }

        private List<byte[]> listaImagenes = new List<byte[]>();
        public List<byte[]> ListaImagenes
        {
            get { return listaImagenes; }
            set { listaImagenes = value; RaisePropertyChanged("ListaImagenes"); }
        }

        private bool? iniciaAnimacion = null;
        public bool? IniciaAnimacion
        {
            get { return iniciaAnimacion; }
            set { iniciaAnimacion = value; RaisePropertyChanged("IniciaAnimacion"); }
        }

        private bool autenticado = false;
        public bool Autenticado
        {
            get { return autenticado; }
            set { autenticado = value; RaisePropertyChanged("Autenticado"); }
        }

        #endregion
    }
}
