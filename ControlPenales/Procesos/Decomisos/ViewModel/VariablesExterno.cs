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
using ControlPenales.BiometricoServiceReference;

namespace ControlPenales
{
    partial class RegistroDecomisoViewModel
    {
        #region Visita Externa
        private string nipEx;
        public string NipEx
        {
            get { return nipEx; }
            set { nipEx = value; OnPropertyChanged("NipEx"); }
        }

        private string paternoEx;
        public string PaternoEx
        {
            get { return paternoEx; }
            set { paternoEx = value; OnPropertyChanged("PaternoEx"); }
        }

        private string maternoEx;
        public string MaternoEx
        {
            get { return maternoEx; }
            set { maternoEx = value; OnPropertyChanged("MaternoEx"); }
        }

        private string nombreEx;
        public string NombreEx
        {
            get { return nombreEx; }
            set { nombreEx = value; OnPropertyChanged("NombreEx"); }
        }

        private bool externoEmpty;
        public bool ExternoEmpty
        {
            get { return externoEmpty; }
            set { externoEmpty = value; OnPropertyChanged("ExternoEmpty"); }
        }

        //private ObservableCollection<PERSONA_EXTERNO> lstExternoPop;
        //public ObservableCollection<PERSONA_EXTERNO> LstExternoPop
        //{
        //    get { return lstExternoPop; }
        //    set { lstExternoPop = value; 
        //        OnPropertyChanged("LstExternoPop"); }
        //}
        private RangeEnabledObservableCollection<PERSONA_EXTERNO> lstExternoPop;
        public RangeEnabledObservableCollection<PERSONA_EXTERNO> LstExternoPop
        {
            get { return lstExternoPop; }
            set { lstExternoPop = value; OnPropertyChanged("LstExternoPop"); }
        }
        private int PaginaExterno { get; set; }
        private bool SeguirCargandoExterno { get; set; }

        private PERSONA_EXTERNO selectedExternoPop;
        public PERSONA_EXTERNO SelectedExternoPop
        {
            get { return selectedExternoPop; }
            set
            {
                selectedExternoPop = value;
                if (value != null)
                {
                    if (value.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                        ImagenExternoPop = value.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                    else
                        ImagenExternoPop = new Imagenes().getImagenPerson();
                }
                else
                    ImagenExternoPop = new Imagenes().getImagenPerson();
                OnPropertyChanged("SelectedExternoPop");
            }
        }

        private ObservableCollection<DECOMISO_PERSONA> lstExterno;
        public ObservableCollection<DECOMISO_PERSONA> LstExterno
        {
            get { return lstExterno; }
            set { lstExterno = value; OnPropertyChanged("LstExterno"); }
        }

        private DECOMISO_PERSONA selectedExterno;
        public DECOMISO_PERSONA SelectedExterno
        {
            get { return selectedExterno; }
            set
            {
                selectedExterno = value;
                //if (value != null)
                //{
                //    if (value.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO&& w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                //        ImagenEmpleado = value.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO&& w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                //    else
                //        ImagenEmpleado = new Imagenes().getImagenPerson();
                //}
                //else
                //    ImagenEmpleado = new Imagenes().getImagenPerson();
                OnPropertyChanged("SelectedExterno");
            }
        }

        private byte[] imagenExternoPop = new Imagenes().getImagenPerson();
        public byte[] ImagenExternoPop
        {
            get { return imagenExternoPop; }
            set
            {
                imagenExternoPop = value;
                OnPropertyChanged("ImagenExternoPop");
            }
        }

        private byte[] imagenExterno = new Imagenes().getImagenPerson();
        public byte[] ImagenExterno
        {
            get { return imagenExterno; }
            set
            {
                imagenExterno = value;
                OnPropertyChanged("ImagenExterno");
            }
        }
        #endregion
    }
}
